"""End-to-end smoke test against a running server, speaking the same protocols as the 2012 client.

  python tests/smoke_test.py [host] [http_port]
"""
import base64
import hashlib
import json
import os
import random
import socket
import struct
import sys
import time
import urllib.parse
import urllib.request

sys.path.insert(0, os.path.join(os.path.dirname(os.path.abspath(__file__)), ".."))
from kopserver import protocol16 as p16  # noqa: E402

HOST = sys.argv[1] if len(sys.argv) > 1 else "127.0.0.1"
HTTP = int(sys.argv[2]) if len(sys.argv) > 2 else 8080
BASE = f"http://{HOST}:{HTTP}/"
KEYS = json.load(open(os.path.join(os.path.dirname(__file__), "..", "data", "keys.json"), encoding="utf-8"))
failures = []


def check(cond, what):
    print(("  ok   " if cond else "  FAIL ") + what)
    if not cond:
        failures.append(what)


def md5(s):
    return hashlib.md5(s.encode()).hexdigest()


def post(script, fields, files=None):
    if files:
        boundary = "----kop" + str(random.random())[2:]
        body = b""
        for k, v in fields.items():
            body += f'--{boundary}\r\nContent-Disposition: form-data; name="{k}"\r\n\r\n{v}\r\n'.encode()
        for k, (fname, data) in files.items():
            body += (f'--{boundary}\r\nContent-Disposition: form-data; name="{k}"; filename="{fname}"\r\n'
                     f'Content-Type: application/octet-stream\r\n\r\n').encode() + data + b"\r\n"
        body += f"--{boundary}--\r\n".encode()
        req = urllib.request.Request(BASE + "api/" + script, body,
                                     {"Content-Type": f"multipart/form-data; boundary={boundary}"})
    else:
        req = urllib.request.Request(BASE + "api/" + script, urllib.parse.urlencode(fields).encode())
    return urllib.request.urlopen(req, timeout=10).read().decode("utf-8")


def rsa_oaep_encrypt(msg):
    n, e = int(KEYS["rsa_n"], 16), int(KEYS["rsa_e"], 16)
    k = (n.bit_length() + 7) // 8

    def mgf1(seed, length):
        out = b""
        i = 0
        while len(out) < length:
            out += hashlib.sha1(seed + i.to_bytes(4, "big")).digest()
            i += 1
        return out[:length]
    m = msg.encode()
    db = hashlib.sha1(b"").digest() + b"\0" * (k - len(m) - 42) + b"\1" + m
    seed = os.urandom(20)
    masked_db = bytes(a ^ b for a, b in zip(db, mgf1(seed, k - 21)))
    masked_seed = bytes(a ^ b for a, b in zip(seed, mgf1(masked_db, 20)))
    em = b"\0" + masked_seed + masked_db
    return base64.b64encode(pow(int.from_bytes(em, "big"), e, n).to_bytes(k, "big")).decode()


def http_tests():
    print("HTTP API")
    email = f"t{random.randint(0, 10**9)}@test"
    r = post("register.php", {"email": email, "password": "pw", "uniq_id": "x", "check_hash": "x", "random": 7})
    check(r.split("\n")[0] == "REGISTERED", f"register -> {r.split(chr(10))[0]}")
    r = post("enter_game.php", {"email": email, "password": "bad", "random": 1})
    check(r == "LOGIN_FAILED", "wrong password rejected")
    r = post("enter_game.php", {"email": email, "password": "pw", "random": 1})
    pid, auth = r.split("\n")[0].split("|")
    check(r.split("\n")[1] == md5(f"{pid}|{auth}_1_{KEYS['secret']}"), "login signed with the client secret")
    me = {"id": pid, "auth_key": auth}

    r = post("get_profile.php", {**me, "random": 4242})
    check(r.startswith("Saved Profile: rnd=4242,soc=ws,player=" + pid) and "inv6=1|" in r, "profile format")
    check(post("set_name.php", {**me, "name": "Тестер" + pid}) == "SET_NAME", "set name")
    check(post("get_slot_info.php", {"id": pid, "map_place": 1}) == "EMPTY SLOT", "slot 1 empty")
    check(post("get_slot_info.php", {"id": pid, "map_place": 2}) == "EMPTY SLOT", "slot 2 empty")
    check(post("get_slot_info.php", {"id": pid, "map_place": 3}) == "NOT AVAILABLE SLOT", "slot 3 locked")

    blob = os.urandom(5000)
    check(post("save_map_to_slot.php", {**me, "map_place": 1}, {"buffer": (pid + ".map", blob)}) == "OK", "save map")
    check(post("get_slot_info.php", {"id": pid, "map_place": 1}) == "1", "slot 1 holds a private map")
    sub = md5(pid)[:2]
    got = urllib.request.urlopen(f"{BASE}api/worlds/{sub}/{pid}_1.map?random=5", timeout=10).read()
    check(got == blob, "map file served back byte for byte")

    post("public_map.php", {**me, "slot_id": 1, "name": "Замок, тест", "about": "опис", "url": ""})
    mid = post("get_slot_info.php", {"id": pid, "map_place": 1})
    check(mid.isdigit() and int(mid) > 1, f"published map id {mid}")
    info = post("get_map_info.php", {"map_id": mid}).split("|")
    check(len(info) == 9 and info[7] == pid and info[8] == "1", "map info format")
    lst = post("get_latest_map.php", {"page": 0})
    check(lst.split("|")[0] == "0" and f"|{mid},{pid},1," in lst, f"latest maps list: {lst[:60]}")
    check(mid in post("search_maps.php", {"text": "замок", "sortby": "rating", "type": 0, "page": 0}), "search by name")
    post("post_comment.php", {"id": pid, "post_name": "Тестер", "post_text": "клас|~", "map_id": mid})
    c = post("get_coments.php", {"map_id": mid, "start_ind": 0})
    check(c.startswith("1||") and c.count("~") == 2, f"comments format: {c}")
    check(post("is_voute_by_map.php", {"id": pid, "map_id": mid}) == "NOT_VOTE", "no vote yet")
    post("voute_yes.php", {**me, "map_id": mid})
    check(post("get_map_info.php", {"map_id": mid}).split("|")[6] == "1", "vote raises rating")

    r = post("by_from_store_crystal.php", {"viewer_id": pid, "auth_key": auth, "p_ind": 6, "count": 3,
                                           "random": 99, "beta_key": 20})
    check(r == "SUCCESS\n" + md5(f"SUCCESS_99_{KEYS['secret']}"), "crystal purchase signed like the client checks")
    check(post("get_slot_info.php", {"id": pid, "map_place": 3}) == "EMPTY SLOT", "bought slot 3")
    yoba = "ABCDEFGHIJKLMNOPQRST"
    r = post("by_from_store_coins.php", {"viewer_id": pid, "auth_key": auth, "p_ind": 42, "count": 1, "date_delta": 2,
                                         "random": 5, "yoba_key": rsa_oaep_encrypt(yoba), "beta_key": 100})
    check(r == "SUCCESS\n" + md5(f"SUCCESS_5_{yoba}"), "coin purchase: RSA key decrypted and signed")
    prof = post("get_profile.php", {**me, "random": 1})
    check("crystal=480" in prof and "coins=49900" in prof and "inv42=1|" in prof, "balances and inventory updated")
    check(post("sell_crystalls.php", {"viewer_id": pid, "auth_key": auth, "count": 1}) == "SUCCSES", "sell crystals")
    check(post("get_profile.php", {"id": pid, "auth_key": "wrong", "random": 1}).startswith("Unknown"), "bad auth key")
    page = urllib.request.urlopen(BASE, timeout=10).read().decode()
    check("Digger2012.unity3d" in page, "game page served")
    head = urllib.request.urlopen(BASE + "Digger2012.unity3d", timeout=30).read(9)
    check(head == b"UnityWeb\0", "client bundle served")


class PhotonClient:
    def __init__(self, port, name):
        self.name = name
        self.s = socket.create_connection((HOST, port), timeout=5)
        self.buf = b""
        init = bytes([243, 0, 1, 6, 1, 3, 0, 1, 7]) + b"Master".ljust(32, b"\0")
        self.s.sendall(b"\xfb" + struct.pack(">i", 7 + len(init)) + b"\x00\x01" + init)
        assert self.recv_msg()[:2] == b"\xf3\x01"

    def recv_msg(self):
        while True:
            if len(self.buf) >= 9 and self.buf[0] == 0xF0:
                self.buf = self.buf[9:]
                continue
            if len(self.buf) >= 5 and self.buf[0] == 0xFB:
                (n,) = struct.unpack(">i", self.buf[1:5])
                if len(self.buf) >= n:
                    msg, self.buf = self.buf[7:n], self.buf[n:]
                    return msg
            chunk = self.s.recv(65536)
            if not chunk:
                raise ConnectionError("closed")
            self.buf += chunk

    def op(self, code, params=None):
        payload = b"\xf3\x02" + bytes([code]) + p16.params(params or {})
        self.s.sendall(b"\xfb" + struct.pack(">i", 7 + len(payload)) + b"\x00\x01" + payload)

    def next(self, kind=None, code=None, timeout=5):
        """Next response (kind 3) or event (kind 4), optionally waiting for a specific code."""
        end = time.time() + timeout
        while time.time() < end:
            msg = self.recv_msg()
            k, c = msg[1], msg[2]
            if kind and k != kind or code is not None and c != code:
                continue
            if k == 3:
                (ret,) = struct.unpack(">h", msg[3:5])
                _, pos = p16.read(msg, 5)
                params, _ = p16.read_params(msg, pos)
                return {"code": c, "ret": ret, "params": params}
            params, _ = p16.read_params(msg, 3)
            return {"code": c, "params": params}
        raise TimeoutError(f"{self.name}: no message {kind}/{code}")

    def close(self):
        self.s.close()


def enter_room(name, create, props=None):
    m = PhotonClient(4530, name + "@master")
    m.op(230, {224: p16.string("94ec61da"), 220: p16.string("WEBSITE1.05Qshard_1")})
    check(m.next(3, 230)["ret"] == 0, f"{name}: authenticated on master")
    m.op(229)
    m.next(3, 229)
    games = m.next(4, 230)["params"][222].v
    room_params = {255: p16.string("room-1")}
    if create:
        room_params[248] = p16.hashtable([(p16.byte(255), p16.byte(10)), (p16.string("player_id"), p16.string("1001")),
                                          (p16.string("slot_id"), p16.integer(1))] + (props or []))
        room_params[249] = p16.hashtable([(p16.byte(255), p16.string(name))])
        room_params[241] = p16.boolean(True)
    m.op(227 if create else 226, {255: p16.string("room-1")})
    resp = m.next(3, 227 if create else 226)
    check(resp["ret"] == 0, f"{name}: master gave game server {resp['params'][230].v}")
    m.close()
    g = PhotonClient(int(resp["params"][230].v.split(":")[1]), name + "@game")
    g.op(230, {224: p16.string("94ec61da"), 220: p16.string("WEBSITE1.05Qshard_1")})
    g.next(3, 230)
    if not create:
        room_params[249] = p16.hashtable([(p16.byte(255), p16.string(name))])
    g.op(227 if create else 226, room_params)
    joined = g.next(3, 227 if create else 226)
    check(joined["ret"] == 0, f"{name}: in room as actor {joined['params'][254].v}")
    join_ev = g.next(4, 255)
    check(join_ev["params"][254].v == joined["params"][254].v, f"{name}: got own join event")
    return g, joined, games


def photon_tests():
    print("Photon")
    s = socket.create_connection((HOST, 843), timeout=5)
    s.sendall(b"<policy-file-request/>\0")
    check(b"allow-access-from" in s.recv(1024), "socket policy on port 843")
    s.close()

    a, ja, _ = enter_room("A", create=True)
    check(ja["params"][254].v == 1, "creator is actor 1")
    # A caches an instantiate-like event, then B joins and must receive it
    a.op(253, {244: p16.byte(202), 245: p16.hashtable([(p16.byte(7), p16.integer(55))]), 247: p16.byte(4)})
    b, jb, games = enter_room("B", create=False)
    check(len(games) == 1 and games[0][0].v == "room-1", "lobby list shows the room with custom props")
    custom = {k.v: v.v for k, v in games[0][1].v}
    check(custom.get("player_id") == "1001" and custom.get("slot_id") == 1, "lobby entry carries player_id/slot_id")
    cached = b.next(4, 202)
    check(cached["params"][254].v == 1, "B received A's cached event")
    actors = {k.v for k, _ in jb["params"][249].v}
    check(actors == {1, 2}, "B sees both actors")
    ev = a.next(4, 255)
    check(ev["params"][254].v == 2 and ev["params"][252].v == [1, 2], "A told that B joined")

    b.op(253, {244: p16.byte(200), 245: p16.hashtable([(p16.byte(3), p16.string("Hello"))])})
    rpc = a.next(4, 200)
    check(rpc["params"][254].v == 2, "RPC relayed from B to A")
    a.op(252, {251: p16.hashtable([(p16.string("rating"), p16.byte(3))]), 250: p16.boolean(True)})
    a.next(3, 252)
    pc = b.next(4, 253)
    check(pc["params"][253].v == 0 and pc["params"][254].v == 1, "room property change broadcast")

    a.op(253, {244: p16.byte(202), 245: p16.hashtable([(p16.byte(7), p16.integer(55))]), 247: p16.byte(6),
               252: p16.int_array([1])})
    c, _, _ = enter_room("C", create=False)
    try:
        c.next(4, 202, timeout=1)
        check(False, "removed cached event not replayed")
    except (TimeoutError, socket.timeout):
        check(True, "removed cached event not replayed")

    b.op(254)
    b.next(3, 254)
    lv = a.next(4, 254)
    check(lv["params"][254].v == 2, "A told that B left")
    for x in (a, b, c):
        x.close()


if __name__ == "__main__":
    http_tests()
    photon_tests()
    print("\nALL OK" if not failures else f"\n{len(failures)} FAILED")
    sys.exit(1 if failures else 0)
