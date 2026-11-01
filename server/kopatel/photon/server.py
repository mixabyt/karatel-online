"""A small Photon LoadBalancing server for PUN 1.x clients over TCP.

One process plays both roles: the master (lobby, matchmaking) on one port and the game server
(rooms) on another; the master hands out the game port in its join/create responses. Game logic
lives in the clients (the master client of each room), so the server only relays and caches.
"""
import asyncio
import logging
import struct
import time
import uuid

from . import protocol16 as p16
from .crypto import KeyExchange
from .protocol16 import Byte, IntArray, StrArray, TypedArray

log = logging.getLogger("photon")

# Message types (second byte after 0xF3); 0x80 marks an encrypted body.
MSG_INIT, MSG_INIT_RESPONSE, MSG_OPERATION, MSG_RESPONSE, MSG_EVENT = 0, 1, 2, 3, 4
MSG_INTERNAL_OPERATION, MSG_INTERNAL_RESPONSE = 6, 7

# Operation codes
OP_AUTHENTICATE, OP_JOIN_LOBBY, OP_LEAVE_LOBBY = 230, 229, 228
OP_CREATE_GAME, OP_JOIN_GAME, OP_JOIN_RANDOM = 227, 226, 225
OP_FIND_FRIENDS, OP_LOBBY_STATS, OP_GET_REGIONS = 222, 221, 220
OP_LEAVE, OP_RAISE_EVENT, OP_SET_PROPERTIES, OP_GET_PROPERTIES, OP_CHANGE_GROUPS = 254, 253, 252, 251, 248

# Event codes
EV_GAME_LIST, EV_GAME_LIST_UPDATE, EV_APP_STATS = 230, 229, 226
EV_JOIN, EV_LEAVE, EV_PROPERTIES_CHANGED = 255, 254, 253

# Parameter codes
P_ADDRESS, P_PEER_COUNT, P_GAME_COUNT, P_MASTER_PEER_COUNT = 230, 229, 228, 227
P_USER_ID, P_GAME_LIST, P_APP_VERSION = 225, 222, 220
P_ROOM_NAME, P_ACTOR_NR, P_TARGET_ACTOR, P_ACTOR_LIST = 255, 254, 253, 252
P_PROPERTIES, P_BROADCAST, P_PLAYER_PROPS, P_GAME_PROPS = 251, 250, 249, 248
P_CACHE, P_RECEIVERS, P_DATA, P_CODE = 247, 246, 245, 244
P_CLEANUP_CACHE, P_GROUP, P_REMOVE, P_ADD = 241, 240, 239, 238
P_CREATE_IF_NOT_EXISTS = 215

# Well-known room properties (byte keys)
G_MAX_PLAYERS, G_IS_VISIBLE, G_IS_OPEN, G_PLAYER_COUNT = 255, 254, 253, 252
G_REMOVED, G_PROPS_IN_LOBBY, G_CLEANUP_CACHE = 251, 250, 249

# Return codes
OK = 0
ERR_OPERATION_NOT_ALLOWED = -3
ERR_INVALID_OPERATION = -2
ERR_GAME_ID_EXISTS = 32766
ERR_GAME_FULL = 32765
ERR_GAME_CLOSED = 32764
ERR_NO_RANDOM_MATCH = 32760
ERR_GAME_DOES_NOT_EXIST = 32758

# EventCaching
CACHE_NONE, CACHE_MERGE, CACHE_REPLACE, CACHE_REMOVE = 0, 1, 2, 3
CACHE_ADD, CACHE_ADD_GLOBAL, CACHE_REMOVE_FROM_ROOM, CACHE_REMOVE_LEFT = 4, 5, 6, 7

# ReceiverGroup
RECV_OTHERS, RECV_ALL, RECV_MASTER = 0, 1, 2


def frame(msg_type, body, channel=0, reliable=True):
    total = 7 + 2 + len(body)
    return (b"\xfb" + struct.pack(">I", total) + bytes((channel, 1 if reliable else 0))
            + bytes((0xF3, msg_type)) + body)


def tick():
    return int(time.monotonic() * 1000) & 0x7FFFFFFF


def normalize_room_props(props):
    """PUN sends maxPlayers as int but reads it back with a (byte) cast, as Photon stores it."""
    for k in list(props):
        if k == G_MAX_PLAYERS and not isinstance(k, str):
            value = props.pop(k)
            props[Byte(G_MAX_PLAYERS)] = Byte(int(value or 0) & 0xFF)
    return props


def merge_props(target, changes):
    for k, v in changes.items():
        if v is None and isinstance(k, str):
            target.pop(k, None)
        else:
            target[k] = v


class Peer:
    def __init__(self, app, reader, writer, role):
        self.app, self.reader, self.writer, self.role = app, reader, writer, role
        self.addr = writer.get_extra_info("peername")
        self.key_exchange = None
        self.cipher = None
        self.authenticated = False
        self.app_version = ""
        self.user_id = ""
        self.in_lobby = False
        self.room = None
        self.actor = None
        self.closed = False

    # -------------------------------------------------- transport

    def send_raw(self, data):
        if not self.closed:
            self.writer.write(data)

    def send_response(self, op, params=None, return_code=OK, debug=None, msg_type=MSG_RESPONSE):
        if return_code != OK:
            log.info("%s op %d -> %d %s", self.addr, op, return_code, debug or "")
        self.send_raw(frame(msg_type, p16.write_response(op, params, return_code, debug)))

    def send_event(self, code, params, reliable=True):
        self.send_raw(frame(MSG_EVENT, p16.write_event(code, params), reliable=reliable))

    async def run(self):
        try:
            while True:
                first = await self.reader.readexactly(1)
                if first == b"\xf0":  # ping: F0 + client tick
                    (client_time,) = struct.unpack(">i", await self.reader.readexactly(4))
                    self.send_raw(b"\xf0" + struct.pack(">ii", tick(), client_time))
                    continue
                if first != b"\xfb":
                    log.warning("%s bad frame start %r", self.addr, first)
                    return
                head = await self.reader.readexactly(6)
                (total,) = struct.unpack(">I", head[:4])
                if total < 9 or total > 4 * 1024 * 1024:
                    log.warning("%s bad frame length %d", self.addr, total)
                    return
                payload = await self.reader.readexactly(total - 7)
                self.handle_message(payload)
                await self.writer.drain()
        except (asyncio.IncompleteReadError, ConnectionError):
            pass
        except Exception:
            log.exception("%s crashed", self.addr)
        finally:
            self.close()

    def close(self):
        if self.closed:
            return
        self.closed = True
        if self.room:
            self.app.leave_room(self)
        self.app.peers.discard(self)
        try:
            self.writer.close()
        except Exception:
            pass

    # -------------------------------------------------- dispatch

    def handle_message(self, payload):
        if payload[0] != 0xF3 or len(payload) < 2:
            log.warning("%s unknown payload %s", self.addr, payload[:8].hex())
            return
        msg_type = payload[1] & 0x7F
        body = payload[2:]
        if payload[1] & 0x80:
            if not self.cipher:
                log.warning("%s encrypted message before key exchange", self.addr)
                return
            body = self.cipher.decrypt_cbc(body)
        if msg_type == MSG_INIT:
            self.send_raw(frame(MSG_INIT_RESPONSE, b""))
        elif msg_type == MSG_INTERNAL_OPERATION:
            op, params = p16.read_request(body)
            if op == 0:  # InitEncryption {1: client public key}
                kx = KeyExchange()
                self.cipher = kx.derive(params[1])
                self.send_response(0, {1: kx.public_key}, msg_type=MSG_INTERNAL_RESPONSE)
            else:
                log.warning("%s unknown internal op %d", self.addr, op)
        elif msg_type == MSG_OPERATION:
            op, params = p16.read_request(body)
            log.debug("%s %s op %d %r", self.addr, self.role, op, params)
            handler = (self.app.master_ops if self.role == "master" else self.app.game_ops).get(op)
            if handler is None:
                log.info("%s %s: unhandled op %d %r", self.addr, self.role, op, params)
                self.send_response(op, return_code=ERR_INVALID_OPERATION, debug="unsupported operation")
            else:
                handler(self, params)
        else:
            log.warning("%s unexpected message type %d", self.addr, msg_type)


class Actor:
    def __init__(self, peer, nr, props):
        self.peer, self.nr, self.props = peer, nr, props
        self.groups = set()


class Room:
    def __init__(self, app_version, name, props, cleanup_cache):
        self.app_version, self.name = app_version, name
        self.props = props
        self.cleanup_cache = cleanup_cache
        self.actors = {}  # nr -> Actor
        self.next_actor = 1
        self.cache = []  # (sender_nr, code, data, receivers_global) in send order

    @property
    def max_players(self):
        return int(self.props.get(G_MAX_PLAYERS, 0) or 0)

    @property
    def is_open(self):
        return bool(self.props.get(G_IS_OPEN, True))

    @property
    def is_visible(self):
        return bool(self.props.get(G_IS_VISIBLE, True))

    def lobby_props(self):
        out = {Byte(G_MAX_PLAYERS): Byte(self.max_players), Byte(G_IS_OPEN): self.is_open,
               Byte(G_IS_VISIBLE): self.is_visible, Byte(G_PLAYER_COUNT): Byte(len(self.actors))}
        listed = self.props.get(G_PROPS_IN_LOBBY)
        names = list(listed) if isinstance(listed, (list, StrArray)) else None
        for k, v in self.props.items():
            if isinstance(k, str) and (names is None or k in names):
                out[k] = v
        return out

    def full_props(self):
        out = dict(self.props)
        out[Byte(G_PLAYER_COUNT)] = Byte(len(self.actors))
        return out

    def actor_props(self):
        return {a.nr: a.props for a in self.actors.values()}

    def master_nr(self):
        return min(self.actors) if self.actors else 0


class PhotonServer:
    def __init__(self, host, master_port, game_port, public_host=None):
        self.host, self.master_port, self.game_port = host, master_port, game_port
        self.public_host = public_host
        self.peers = set()
        self.rooms = {}  # (app_version, name) -> Room
        self.master_ops = {
            OP_AUTHENTICATE: self.op_authenticate,
            OP_JOIN_LOBBY: self.op_join_lobby,
            OP_LEAVE_LOBBY: self.op_leave_lobby,
            OP_CREATE_GAME: self.master_create,
            OP_JOIN_GAME: self.master_join,
            OP_JOIN_RANDOM: self.master_join_random,
            OP_FIND_FRIENDS: self.op_find_friends,
            OP_LOBBY_STATS: lambda peer, p: peer.send_response(OP_LOBBY_STATS),
            OP_GET_REGIONS: lambda peer, p: peer.send_response(OP_GET_REGIONS, return_code=ERR_INVALID_OPERATION),
        }
        self.game_ops = {
            OP_AUTHENTICATE: self.op_authenticate,
            OP_CREATE_GAME: self.game_create,
            OP_JOIN_GAME: self.game_join,
            OP_LEAVE: self.game_leave,
            OP_RAISE_EVENT: self.game_raise_event,
            OP_SET_PROPERTIES: self.game_set_properties,
            OP_GET_PROPERTIES: self.game_get_properties,
            OP_CHANGE_GROUPS: self.game_change_groups,
        }
        self._lobby_dirty = set()

    async def serve(self):
        async def accept(role):
            async def on_client(reader, writer):
                peer = Peer(self, reader, writer, role)
                self.peers.add(peer)
                log.info("%s connected to %s", peer.addr, role)
                await peer.run()
                log.info("%s left %s", peer.addr, role)
            return on_client

        master = await asyncio.start_server(await accept("master"), self.host, self.master_port)
        game = await asyncio.start_server(await accept("game"), self.host, self.game_port)
        log.info("Photon master on %s:%d, game server on %s:%d", self.host, self.master_port, self.host, self.game_port)
        asyncio.get_running_loop().create_task(self.lobby_ticker())
        async with master, game:
            await asyncio.gather(master.serve_forever(), game.serve_forever())

    def game_address(self, peer):
        host = self.public_host or peer.writer.get_extra_info("sockname")[0]
        return "%s:%d" % (host, self.game_port)

    # -------------------------------------------------- shared

    def op_authenticate(self, peer, params):
        peer.app_version = params.get(P_APP_VERSION, "") or ""
        peer.user_id = params.get(P_USER_ID, "") or ""
        peer.authenticated = True
        peer.send_response(OP_AUTHENTICATE)
        if peer.role == "master":
            self.send_app_stats(peer)

    # -------------------------------------------------- master

    def rooms_for(self, app_version):
        return [r for (v, _), r in self.rooms.items() if v == app_version]

    def op_join_lobby(self, peer, params):
        peer.in_lobby = True
        peer.send_response(OP_JOIN_LOBBY)
        rooms = {r.name: r.lobby_props() for r in self.rooms_for(peer.app_version) if r.is_visible}
        peer.send_event(EV_GAME_LIST, {P_GAME_LIST: rooms})
        self.send_app_stats(peer)

    def op_leave_lobby(self, peer, params):
        peer.in_lobby = False
        peer.send_response(OP_LEAVE_LOBBY)

    def op_find_friends(self, peer, params):
        names = params.get(1) or []
        peer.send_response(OP_FIND_FRIENDS, {1: TypedArray(111, [False] * len(names)), 2: StrArray([""] * len(names))})

    def send_app_stats(self, peer):
        rooms = self.rooms_for(peer.app_version)
        in_rooms = sum(len(r.actors) for r in rooms)
        on_master = sum(1 for p in self.peers if p.role == "master" and p.app_version == peer.app_version)
        peer.send_event(EV_APP_STATS, {P_PEER_COUNT: in_rooms, P_MASTER_PEER_COUNT: on_master, P_GAME_COUNT: len(rooms)})

    def master_create(self, peer, params):
        name = params.get(P_ROOM_NAME) or str(uuid.uuid4())
        if (peer.app_version, name) in self.rooms:
            peer.send_response(OP_CREATE_GAME, return_code=ERR_GAME_ID_EXISTS, debug="A game with the specified id already exist.")
            return
        peer.send_response(OP_CREATE_GAME, {P_ROOM_NAME: name, P_ADDRESS: self.game_address(peer)})

    def master_join(self, peer, params):
        name = params.get(P_ROOM_NAME, "")
        room = self.rooms.get((peer.app_version, name))
        if room is None and not params.get(P_CREATE_IF_NOT_EXISTS):
            peer.send_response(OP_JOIN_GAME, return_code=ERR_GAME_DOES_NOT_EXIST, debug="Game does not exist")
            return
        error = room and self.join_error(room)
        if error:
            peer.send_response(OP_JOIN_GAME, return_code=error[0], debug=error[1])
            return
        peer.send_response(OP_JOIN_GAME, {P_ROOM_NAME: name, P_ADDRESS: self.game_address(peer)})

    def join_error(self, room):
        if not room.is_open:
            return ERR_GAME_CLOSED, "Game closed"
        if room.max_players and len(room.actors) >= room.max_players:
            return ERR_GAME_FULL, "Game full"
        return None

    def master_join_random(self, peer, params):
        expected = params.get(P_GAME_PROPS) or {}
        for room in self.rooms_for(peer.app_version):
            if not room.is_visible or self.join_error(room):
                continue
            ok = True
            for k, v in expected.items():
                if k == G_MAX_PLAYERS and not isinstance(k, str):
                    if int(v) != room.max_players:
                        ok = False
                elif room.props.get(k) != v:
                    ok = False
            if ok:
                peer.send_response(OP_JOIN_RANDOM, {P_ROOM_NAME: room.name, P_ADDRESS: self.game_address(peer)})
                return
        peer.send_response(OP_JOIN_RANDOM, return_code=ERR_NO_RANDOM_MATCH, debug="No match found")

    def mark_lobby_dirty(self, room, removed=False):
        self._lobby_dirty.add((room.app_version, room.name, removed))

    async def lobby_ticker(self):
        """Push room list changes to lobby peers, and app stats every 5 seconds like Photon does."""
        last_stats = 0
        while True:
            await asyncio.sleep(1)
            if self._lobby_dirty:
                dirty, self._lobby_dirty = self._lobby_dirty, set()
                by_version = {}
                for version, name, removed in dirty:
                    room = self.rooms.get((version, name))
                    if room is None or removed or not room.is_visible:
                        props = {Byte(G_REMOVED): True}
                    else:
                        props = room.lobby_props()
                    by_version.setdefault(version, {})[name] = props
                for peer in list(self.peers):
                    if peer.role == "master" and peer.in_lobby and peer.app_version in by_version:
                        peer.send_event(EV_GAME_LIST_UPDATE, {P_GAME_LIST: by_version[peer.app_version]})
            if time.monotonic() - last_stats >= 5:
                last_stats = time.monotonic()
                for peer in list(self.peers):
                    if peer.role == "master" and peer.authenticated:
                        self.send_app_stats(peer)

    # -------------------------------------------------- game server

    def game_create(self, peer, params):
        name = params.get(P_ROOM_NAME) or str(uuid.uuid4())
        if (peer.app_version, name) in self.rooms:
            peer.send_response(OP_CREATE_GAME, return_code=ERR_GAME_ID_EXISTS, debug="A game with the specified id already exist.")
            return
        room = self.create_room(peer, name, params)
        self.enter_room(peer, room, OP_CREATE_GAME, params)

    def create_room(self, peer, name, params):
        props = normalize_room_props(dict(params.get(P_GAME_PROPS) or {}))
        cleanup = bool(params.get(P_CLEANUP_CACHE, props.get(G_CLEANUP_CACHE, False)))
        room = Room(peer.app_version, name, props, cleanup)
        self.rooms[(peer.app_version, name)] = room
        log.info("room created %r (%s) by %s", name, peer.app_version, peer.addr)
        return room

    def game_join(self, peer, params):
        name = params.get(P_ROOM_NAME, "")
        room = self.rooms.get((peer.app_version, name))
        if room is None:
            if not params.get(P_CREATE_IF_NOT_EXISTS):
                peer.send_response(OP_JOIN_GAME, return_code=ERR_GAME_DOES_NOT_EXIST, debug="Game does not exist")
                return
            room = self.create_room(peer, name, params)
        else:
            error = self.join_error(room)
            if error:
                peer.send_response(OP_JOIN_GAME, return_code=error[0], debug=error[1])
                return
        self.enter_room(peer, room, OP_JOIN_GAME, params)

    def enter_room(self, peer, room, op, params):
        if peer.room:
            self.leave_room(peer)
        nr = room.next_actor
        room.next_actor += 1
        actor = Actor(peer, nr, dict(params.get(P_PLAYER_PROPS) or {}))
        room.actors[nr] = actor
        peer.room, peer.actor = room, actor
        peer.send_response(op, {P_ACTOR_NR: nr, P_PLAYER_PROPS: room.actor_props(), P_GAME_PROPS: room.full_props()})
        join = {P_ACTOR_NR: nr, P_PLAYER_PROPS: actor.props, P_ACTOR_LIST: IntArray(sorted(room.actors))}
        for other in room.actors.values():
            other.peer.send_event(EV_JOIN, join)
        for sender, code, data, _ in room.cache:
            params = {P_ACTOR_NR: sender}
            if data is not None:
                params[P_DATA] = data
            peer.send_event(code, params)
        self.mark_lobby_dirty(room)
        log.info("%s joined %r as actor %d (%d in room)", peer.addr, room.name, nr, len(room.actors))

    def game_leave(self, peer, params):
        self.leave_room(peer)
        peer.send_response(OP_LEAVE)

    def leave_room(self, peer):
        room, actor = peer.room, peer.actor
        peer.room = peer.actor = None
        if room is None or actor is None or room.actors.get(actor.nr) is not actor:
            return
        del room.actors[actor.nr]
        if room.cleanup_cache:
            room.cache = [c for c in room.cache if c[0] != actor.nr or c[3]]
        for other in room.actors.values():
            other.peer.send_event(EV_LEAVE, {P_ACTOR_NR: actor.nr, P_ACTOR_LIST: IntArray(sorted(room.actors))})
        if not room.actors:
            del self.rooms[(room.app_version, room.name)]
            self.mark_lobby_dirty(room, removed=True)
            log.info("room closed %r", room.name)
        else:
            self.mark_lobby_dirty(room)

    def game_raise_event(self, peer, params):
        room, actor = peer.room, peer.actor
        if room is None:
            peer.send_response(OP_RAISE_EVENT, return_code=ERR_OPERATION_NOT_ALLOWED, debug="not in a room")
            return
        code = int(params.get(P_CODE, 0))
        data = params.get(P_DATA)
        cache = int(params.get(P_CACHE, CACHE_NONE) or 0)
        receivers = int(params.get(P_RECEIVERS, RECV_OTHERS) or 0)
        targets = params.get(P_ACTOR_LIST)
        group = int(params.get(P_GROUP, 0) or 0)

        if cache in (CACHE_REMOVE_FROM_ROOM, CACHE_REMOVE):
            self.remove_from_cache(room, code, data, targets)
            return
        if cache == CACHE_REMOVE_LEFT:
            room.cache = [c for c in room.cache if c[0] in room.actors]
            return
        if cache in (CACHE_ADD, CACHE_ADD_GLOBAL, CACHE_MERGE, CACHE_REPLACE):
            room.cache.append((actor.nr, code, data, cache == CACHE_ADD_GLOBAL))

        event = {P_ACTOR_NR: actor.nr}
        if data is not None:
            event[P_DATA] = data
        if targets:
            recipients = [room.actors[n] for n in targets if n in room.actors]
        elif receivers == RECV_ALL:
            recipients = list(room.actors.values())
        elif receivers == RECV_MASTER:
            recipients = [room.actors[room.master_nr()]] if room.actors else []
        else:
            recipients = [a for a in room.actors.values() if a is not actor]
        if group and not targets:
            recipients = [a for a in recipients if group in a.groups]
        for a in recipients:
            a.peer.send_event(code, event)

    @staticmethod
    def remove_from_cache(room, code, data, targets):
        """RemoveFromRoomCache: code 0 = any code; targets = the senders whose events go;
        a Hashtable filter removes only events whose data contains all of its entries."""
        senders = set(targets) if targets else None

        def matches(entry):
            sender, ecode, edata, _ = entry
            if code and ecode != code:
                return False
            if senders is not None and sender not in senders:
                return False
            if isinstance(data, dict) and data:
                if not isinstance(edata, dict):
                    return False
                return all(k in edata and edata[k] == v for k, v in data.items())
            return True

        room.cache = [c for c in room.cache if not matches(c)]

    def game_set_properties(self, peer, params):
        room, actor = peer.room, peer.actor
        if room is None:
            peer.send_response(OP_SET_PROPERTIES, return_code=ERR_OPERATION_NOT_ALLOWED, debug="not in a room")
            return
        props = params.get(P_PROPERTIES) or {}
        target_nr = params.get(P_ACTOR_NR)
        if target_nr:
            target = room.actors.get(int(target_nr))
            if target is None:
                peer.send_response(OP_SET_PROPERTIES, return_code=ERR_OPERATION_NOT_ALLOWED, debug="no such actor")
                return
            merge_props(target.props, props)
        else:
            props = normalize_room_props(dict(props))
            merge_props(room.props, props)
            self.mark_lobby_dirty(room)
        peer.send_response(OP_SET_PROPERTIES)
        if params.get(P_BROADCAST):
            event = {P_TARGET_ACTOR: int(target_nr or 0), P_PROPERTIES: props, P_ACTOR_NR: actor.nr}
            for a in room.actors.values():
                if a is not actor:
                    a.peer.send_event(EV_PROPERTIES_CHANGED, event)

    def game_get_properties(self, peer, params):
        room = peer.room
        if room is None:
            peer.send_response(OP_GET_PROPERTIES, return_code=ERR_OPERATION_NOT_ALLOWED, debug="not in a room")
            return
        peer.send_response(OP_GET_PROPERTIES, {P_GAME_PROPS: room.full_props(), P_PLAYER_PROPS: room.actor_props()})

    def game_change_groups(self, peer, params):
        actor = peer.actor
        if actor is not None:
            remove = params.get(P_REMOVE)
            add = params.get(P_ADD)
            if remove is not None:
                if len(remove) == 0:
                    actor.groups.clear()
                else:
                    actor.groups.difference_update(remove)
            if add is not None:
                if len(add) == 0:
                    actor.groups.update(range(1, 256))
                else:
                    actor.groups.update(add)
        peer.send_response(OP_CHANGE_GROUPS)
