"""A small Photon LoadBalancing server (TCP, protocol 1.6) for the 2012 PUN client.

Master (lobby, room list, matchmaking) and game server (rooms, events, properties, event cache)
run in one process on two ports, like the original cloud setup. Game logic lives in the clients;
the server only relays and remembers what the clients ask it to.
"""
import asyncio
import logging
import struct
import time
import uuid

from . import protocol16 as p16

log = logging.getLogger("photon")

# operation / event / parameter codes of the LoadBalancing protocol used by this PUN version
OP_AUTH, OP_JOIN_LOBBY, OP_LEAVE_LOBBY, OP_CREATE, OP_JOIN, OP_JOIN_RANDOM = 230, 229, 228, 227, 226, 225
OP_LEAVE, OP_RAISE_EVENT, OP_SET_PROPS, OP_GET_PROPS = 254, 253, 252, 251
EV_GAME_LIST, EV_GAME_LIST_UPDATE, EV_APP_STATS = 230, 229, 226
EV_JOIN, EV_LEAVE, EV_PROPS_CHANGED = 255, 254, 253
P_ADDRESS, P_PEER_COUNT, P_GAME_COUNT, P_MASTER_PEER_COUNT, P_GAME_LIST = 230, 229, 228, 227, 222
P_ROOM_NAME, P_ACTOR_NR, P_TARGET_ACTOR, P_ACTOR_LIST, P_PROPERTIES, P_BROADCAST = 255, 254, 253, 252, 251, 250
P_PLAYER_PROPS, P_GAME_PROPS, P_RECEIVERS, P_CACHE, P_DATA, P_CODE, P_CLEANUP = 249, 248, 246, 247, 245, 244, 241
ERR_INVALID_OP, ERR_GAME_EXISTS, ERR_GAME_FULL, ERR_GAME_CLOSED = -2, 32766, 32765, 32764
ERR_NO_RANDOM_MATCH, ERR_GAME_MISSING = 32760, 32758
# room properties with byte keys
RP_MAX_PLAYERS, RP_VISIBLE, RP_OPEN, RP_PLAYER_COUNT, RP_REMOVED, RP_CLEANUP = 255, 254, 253, 252, 251, 249
# EventCaching / ReceiverGroup
CACHE_ADD, CACHE_ADD_GLOBAL, CACHE_REMOVE, CACHE_REMOVE_LEFT = 4, 5, 6, 7
RECV_OTHERS, RECV_ALL, RECV_MASTER = 0, 1, 2

POLICY = (b'<?xml version="1.0"?><cross-domain-policy>'
          b'<allow-access-from domain="*" to-ports="*" /></cross-domain-policy>\0')
START = time.monotonic()


def server_ms():
    return int((time.monotonic() - START) * 1000) & 0x7FFFFFFF


def _prop(props, code):
    raw = props.get(p16.key_byte(code))
    return p16.parse(raw).v if raw else None


class Actor:
    def __init__(self, nr, peer, props):
        self.nr, self.peer, self.props = nr, peer, props


class CachedEvent:
    def __init__(self, actor_nr, code, data_raw):
        self.actor_nr, self.code, self.data_raw = actor_nr, code, data_raw
        self.data_items = p16.items(p16.parse(data_raw)) if data_raw else {}


class Room:
    def __init__(self, name, props, cleanup):
        self.name, self.props, self.cleanup = name, props, cleanup
        self.actors = {}
        self.next_actor = 1
        self.cache = []

    @property
    def max_players(self):
        return _prop(self.props, RP_MAX_PLAYERS) or 0

    @property
    def is_open(self):
        v = _prop(self.props, RP_OPEN)
        return True if v is None else bool(v)

    @property
    def is_visible(self):
        v = _prop(self.props, RP_VISIBLE)
        return True if v is None else bool(v)

    @property
    def full(self):
        return 0 < self.max_players <= len(self.actors)

    def actor_list(self):
        return p16.int_array(sorted(self.actors))

    def actors_props(self):
        return p16.hashtable((p16.integer(nr), p16.hashtable(a.props.items())) for nr, a in sorted(self.actors.items()))

    def lobby_entry(self):
        """Room as the lobby sees it: standard values plus every custom (string-keyed) property."""
        pairs = [(p16.key_byte(RP_OPEN), p16.boolean(self.is_open)),
                 (p16.key_byte(RP_PLAYER_COUNT), p16.byte(len(self.actors)))]
        if self.max_players:
            pairs.append((p16.key_byte(RP_MAX_PLAYERS), p16.byte(self.max_players)))
        pairs += [(k, v) for k, v in self.props.items() if k[0] == p16.STR]
        return p16.hashtable(pairs)


class PhotonServer:
    def __init__(self, public_host=None, master_port=4530, game_port=4531):
        self.public_host, self.master_port, self.game_port = public_host, master_port, game_port
        self.rooms = {}
        self.peers = set()
        self.lobby = set()

    # ---- networking ----
    async def serve(self, bind="0.0.0.0", policy_port=843):
        servers = [await asyncio.start_server(lambda r, w: Peer(self, r, w, "master").run(), bind, self.master_port),
                   await asyncio.start_server(lambda r, w: Peer(self, r, w, "game").run(), bind, self.game_port)]
        try:
            servers.append(await asyncio.start_server(self._policy, bind, policy_port))
        except OSError as e:
            log.warning("socket policy port %s unavailable (%s); Unity Web Player needs it", policy_port, e)
        log.info("Photon master :%s, game :%s, policy :%s", self.master_port, self.game_port, policy_port)
        asyncio.get_running_loop().create_task(self._stats_loop())
        await asyncio.gather(*(s.serve_forever() for s in servers))

    async def _policy(self, reader, writer):
        try:
            await asyncio.wait_for(reader.readuntil(b"\0"), 5)
            writer.write(POLICY)
            await writer.drain()
        except (asyncio.TimeoutError, asyncio.IncompleteReadError, ConnectionError):
            pass
        finally:
            writer.close()

    async def _stats_loop(self):
        while True:
            await asyncio.sleep(5)
            for peer in list(self.lobby):
                peer.send_app_stats()

    def game_address(self, peer):
        host = self.public_host or peer.writer.get_extra_info("sockname")[0]
        return f"{host}:{self.game_port}"

    # ---- lobby ----
    def lobby_update(self, room, removed=False):
        entry = p16.hashtable([(p16.key_byte(RP_REMOVED), p16.boolean(True))]) if removed or not room.is_visible \
            else room.lobby_entry()
        ev = p16.event(EV_GAME_LIST_UPDATE, {P_GAME_LIST: p16.hashtable([(p16.string(room.name), entry)])})
        for peer in list(self.lobby):
            peer.send(ev)

    def game_list(self):
        return p16.hashtable((p16.string(r.name), r.lobby_entry()) for r in self.rooms.values() if r.is_visible)


class Peer:
    def __init__(self, server, reader, writer, role):
        self.server, self.reader, self.writer, self.role = server, reader, writer, role
        self.room = None
        self.actor = None
        self.addr = writer.get_extra_info("peername")

    # ---- transport ----
    def send(self, payload):
        self.writer.write(b"\xfb" + struct.pack(">i", 7 + len(payload)) + b"\x00\x01" + payload)

    async def run(self):
        srv = self.server
        srv.peers.add(self)
        log.debug("%s connected to %s", self.addr, self.role)
        try:
            while True:
                first = await self.reader.readexactly(1)
                if first == b"\xf0":  # ping: echo client time with ours
                    client_time = await self.reader.readexactly(4)
                    self.writer.write(b"\xf0" + struct.pack(">i", server_ms()) + client_time)
                elif first == b"\xfb":
                    head = await self.reader.readexactly(6)
                    (length,) = struct.unpack(">i", head[:4])
                    if not 7 <= length <= 16 * 1024 * 1024:
                        raise ValueError(f"bad message length {length}")
                    self.handle(await self.reader.readexactly(length - 7))
                elif first == b"<":  # socket policy request sent to the game port
                    await self.reader.readuntil(b"\0")
                    self.writer.write(POLICY)
                    await self.writer.drain()
                    break
                else:
                    raise ValueError(f"unknown frame start {first!r}")
                await self.writer.drain()
        except (asyncio.IncompleteReadError, ConnectionError):
            pass
        except Exception:
            log.exception("peer %s failed", self.addr)
        finally:
            self.leave_room(notify_self=False)
            srv.lobby.discard(self)
            srv.peers.discard(self)
            self.writer.close()
            log.debug("%s left %s", self.addr, self.role)

    def handle(self, msg):
        if len(msg) < 2 or msg[0] != 0xF3:
            raise ValueError(f"bad message {msg[:8].hex()}")
        kind = msg[1] & 0x7F
        if kind == 0:  # init (carries the app id, which we ignore)
            self.send(b"\xf3\x01\x00")
        elif kind == 2:
            opcode = msg[2]
            params, _ = p16.read_params(msg, 3)
            handler = (MASTER_OPS if self.role == "master" else GAME_OPS).get(opcode)
            if handler is None:
                self.respond(opcode, ERR_INVALID_OP, f"operation {opcode} not supported")
            else:
                handler(self, opcode, params)
        else:  # e.g. 6 = encryption key exchange, which the patched client no longer asks for
            log.info("%s sent unsupported message type %s, ignored", self.addr[0], kind)

    def respond(self, opcode, code=0, debug=None, parameters=None):
        self.send(p16.operation_response(opcode, code, debug, parameters))

    def send_app_stats(self):
        srv = self.server
        masters = sum(1 for p in srv.peers if p.role == "master")
        self.send(p16.event(EV_APP_STATS, {P_PEER_COUNT: p16.integer(len(srv.peers) - masters),
                                           P_GAME_COUNT: p16.integer(len(srv.rooms)),
                                           P_MASTER_PEER_COUNT: p16.integer(masters)}))

    # ---- master operations ----
    def op_auth(self, opcode, params):
        if self.role == "master":
            log.info("%s connected (game version %s)", self.addr[0], params[220].v if 220 in params else "?")
        self.respond(opcode)

    def op_join_lobby(self, opcode, params):
        self.respond(opcode)
        self.server.lobby.add(self)
        self.send_app_stats()
        self.send(p16.event(EV_GAME_LIST, {P_GAME_LIST: self.server.game_list()}))

    def op_leave_lobby(self, opcode, params):
        self.server.lobby.discard(self)
        self.respond(opcode)

    def master_create(self, opcode, params):
        name_v = params.get(P_ROOM_NAME)
        name = name_v.v if name_v and name_v.v else uuid.uuid4().hex
        if name in self.server.rooms:
            return self.respond(opcode, ERR_GAME_EXISTS, "A game with the specified id already exist.")
        self.respond(opcode, parameters={P_ROOM_NAME: p16.string(name),
                                         P_ADDRESS: p16.string(self.server.game_address(self))})

    def master_join(self, opcode, params):
        name_v = params.get(P_ROOM_NAME)
        room = self.server.rooms.get(name_v.v if name_v else None)
        error = self._join_error(room)
        if error:
            return self.respond(opcode, *error)
        self.respond(opcode, parameters={P_ADDRESS: p16.string(self.server.game_address(self))})

    def master_join_random(self, opcode, params):
        wanted = p16.items(params.get(P_GAME_PROPS))
        for room in self.server.rooms.values():
            if room.is_visible and room.is_open and not room.full \
                    and all(room.props.get(k) == v for k, v in wanted.items()):
                return self.respond(opcode, parameters={P_ROOM_NAME: p16.string(room.name),
                                                        P_ADDRESS: p16.string(self.server.game_address(self))})
        self.respond(opcode, ERR_NO_RANDOM_MATCH, "No match found")

    @staticmethod
    def _join_error(room):
        if room is None:
            return ERR_GAME_MISSING, "Game does not exist"
        if not room.is_open:
            return ERR_GAME_CLOSED, "Game closed"
        if room.full:
            return ERR_GAME_FULL, "Game full"
        return None

    # ---- game server operations ----
    def game_create(self, opcode, params):
        srv = self.server
        name_v = params.get(P_ROOM_NAME)
        name = name_v.v if name_v and name_v.v else uuid.uuid4().hex
        if name in srv.rooms:
            return self.respond(opcode, ERR_GAME_EXISTS, "A game with the specified id already exist.")
        props = p16.items(params.get(P_GAME_PROPS))
        cleanup_v = params.get(P_CLEANUP)
        cleanup = bool(cleanup_v.v) if cleanup_v else bool(_prop(props, RP_CLEANUP))
        room = Room(name, props, cleanup)
        srv.rooms[name] = room
        log.info("room %r created by %s", name, self.addr)
        self._enter(opcode, room, params)

    def game_join(self, opcode, params):
        name_v = params.get(P_ROOM_NAME)
        room = self.server.rooms.get(name_v.v if name_v else None)
        error = self._join_error(room)
        if error:
            return self.respond(opcode, *error)
        self._enter(opcode, room, params)

    def _enter(self, opcode, room, params):
        self.leave_room(notify_self=False)
        nr = room.next_actor
        room.next_actor += 1
        self.actor = Actor(nr, self, p16.items(params.get(P_PLAYER_PROPS)))
        room.actors[nr] = self.actor
        self.room = room
        log.info("%s entered room %r as player %d (%d in room)", self.addr[0], room.name, nr, len(room.actors))
        self.respond(opcode, parameters={P_ACTOR_NR: p16.integer(nr),
                                         P_PLAYER_PROPS: room.actors_props(),
                                         P_GAME_PROPS: p16.hashtable(room.props.items())})
        join = p16.event(EV_JOIN, {P_ACTOR_NR: p16.integer(nr),
                                   P_PLAYER_PROPS: p16.hashtable(self.actor.props.items()),
                                   P_ACTOR_LIST: room.actor_list()})
        for a in room.actors.values():
            a.peer.send(join)
        for ev in room.cache:
            self.send(p16.event(ev.code, {P_DATA: ev.data_raw or p16.null(), P_ACTOR_NR: p16.integer(ev.actor_nr)}))
        self.server.lobby_update(room)

    def op_leave(self, opcode, params):
        self.leave_room(notify_self=True)

    def leave_room(self, notify_self):
        room, actor = self.room, self.actor
        if room is None:
            return
        self.room = self.actor = None
        room.actors.pop(actor.nr, None)
        if notify_self:
            self.respond(OP_LEAVE)
        if room.cleanup:
            room.cache = [e for e in room.cache if e.actor_nr != actor.nr]
        if room.actors:
            ev = p16.event(EV_LEAVE, {P_ACTOR_NR: p16.integer(actor.nr), P_ACTOR_LIST: room.actor_list()})
            for a in room.actors.values():
                a.peer.send(ev)
            self.server.lobby_update(room)
        else:
            self.server.rooms.pop(room.name, None)
            self.server.lobby_update(room, removed=True)
            log.info("room %r closed", room.name)

    def op_raise_event(self, opcode, params):
        room, me = self.room, self.actor
        if room is None:
            return self.respond(opcode, -3, "not in a room")
        code_v, data_v = params.get(P_CODE), params.get(P_DATA)
        code = code_v.v if code_v else 0
        cache = params[P_CACHE].v if P_CACHE in params else 0
        targets_v = params.get(P_ACTOR_LIST)
        targets = targets_v.v if targets_v and targets_v.v is not None else None
        data_raw = data_v.raw if data_v is not None else None

        if cache == CACHE_REMOVE:
            wanted = p16.items(data_v)
            room.cache = [e for e in room.cache
                          if not ((code == 0 or e.code == code)
                                  and (targets is None or e.actor_nr in targets)
                                  and all(e.data_items.get(k) == v for k, v in wanted.items()))]
            return
        if cache == CACHE_REMOVE_LEFT:
            room.cache = [e for e in room.cache if e.actor_nr == 0 or e.actor_nr in room.actors]
            return
        if cache in (CACHE_ADD, CACHE_ADD_GLOBAL):
            room.cache.append(CachedEvent(me.nr if cache == CACHE_ADD else 0, code, data_raw))

        if targets is not None:
            receivers = [room.actors[n] for n in targets if n in room.actors]
        else:
            group = params[P_RECEIVERS].v if P_RECEIVERS in params else RECV_OTHERS
            if group == RECV_ALL:
                receivers = list(room.actors.values())
            elif group == RECV_MASTER:
                receivers = [room.actors[min(room.actors)]]
            else:
                receivers = [a for a in room.actors.values() if a is not me]
        ev = p16.event(code, {P_DATA: data_raw or p16.null(), P_ACTOR_NR: p16.integer(me.nr)})
        for a in receivers:
            a.peer.send(ev)

    def op_set_properties(self, opcode, params):
        room, me = self.room, self.actor
        if room is None:
            return self.respond(opcode, -3, "not in a room")
        changes = p16.items(params.get(P_PROPERTIES))
        target_v = params.get(P_ACTOR_NR)
        target = target_v.v if target_v else 0
        if target:
            actor = room.actors.get(target)
            if actor is None:
                return self.respond(opcode, -1, "unknown actor")
            actor.props.update(changes)
        else:
            room.props.update(changes)
        self.respond(opcode)
        broadcast = params.get(P_BROADCAST)
        if broadcast and broadcast.v:
            ev = p16.event(EV_PROPS_CHANGED, {P_TARGET_ACTOR: p16.integer(target),
                                              P_PROPERTIES: p16.hashtable(changes.items()),
                                              P_ACTOR_NR: p16.integer(me.nr)})
            for a in room.actors.values():
                if a is not me:
                    a.peer.send(ev)
        if not target:
            self.server.lobby_update(room)

    def op_get_properties(self, opcode, params):
        if self.room is None:
            return self.respond(opcode, -3, "not in a room")
        self.respond(opcode, parameters={P_GAME_PROPS: p16.hashtable(self.room.props.items()),
                                         P_PLAYER_PROPS: self.room.actors_props()})


MASTER_OPS = {OP_AUTH: Peer.op_auth, OP_JOIN_LOBBY: Peer.op_join_lobby, OP_LEAVE_LOBBY: Peer.op_leave_lobby,
              OP_CREATE: Peer.master_create, OP_JOIN: Peer.master_join, OP_JOIN_RANDOM: Peer.master_join_random}
GAME_OPS = {OP_AUTH: Peer.op_auth, OP_CREATE: Peer.game_create, OP_JOIN: Peer.game_join, OP_LEAVE: Peer.op_leave,
            OP_RAISE_EVENT: Peer.op_raise_event, OP_SET_PROPS: Peer.op_set_properties,
            OP_GET_PROPS: Peer.op_get_properties}
