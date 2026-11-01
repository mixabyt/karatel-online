"""End-to-end check of a running server, acting like the patched 2014 client.

    python -I tests/smoke_test.py [host] [http_port]

HTTP: register, log in, load profile, set name, buy, save a map and read it back.
Photon: policy file, key exchange + encrypted Authenticate, two players meeting in one room through
the master/game server switch, cached events reaching a late joiner, properties, leave.
"""
import hashlib
import os
import socket
import struct
import sys
import time
import urllib.parse
import urllib.request

sys.path.insert(0, os.path.join(os.path.dirname(os.path.abspath(__file__)), ".."))

from kopatel.photon import protocol16 as p16  # noqa: E402
from kopatel.photon.crypto import GENERATOR, OAKLEY_PRIME_768, int_to_bytes  # noqa: E402
from kopatel.photon.protocol16 import Byte, Custom, IntArray  # noqa: E402

HOST = sys.argv[1] if len(sys.argv) > 1 else "127.0.0.1"
HTTP = int(sys.argv[2]) if len(sys.argv) > 2 else 8080
SECRET = "Ysm9hrlkzzQas5DXzvHmm"
VERSION = "WEBSITE159f9Qshard_1"


def md5(s):
    return hashlib.md5(s.encode()).hexdigest()


def post(script, prefix="/Test2/", **fields):
    data = urllib.parse.urlencode(fields).encode()
    with urllib.request.urlopen("http://%s:%d%s%s" % (HOST, HTTP, prefix, script), data, timeout=10) as r:
        return r.read().decode("utf-8")


def post_multipart(script, prefix, fields, file_field, file_bytes):
    boundary = "kopatelboundary42"
    body = b""
    for k, v in fields.items():
        body += ('--%s\r\nContent-Disposition: form-data; name="%s"\r\n\r\n%s\r\n' % (boundary, k, v)).encode()
    body += ('--%s\r\nContent-Disposition: form-data; name="%s"; filename="x.map"\r\n'
             'Content-Type: application/octet-stream\r\n\r\n' % (boundary, file_field)).encode() + file_bytes + b"\r\n"
    body += ("--%s--\r\n" % boundary).encode()
    req = urllib.request.Request("http://%s:%d%s%s" % (HOST, HTTP, prefix, script), body,
                                 {"Content-Type": "multipart/form-data; boundary=" + boundary})
    with urllib.request.urlopen(req, timeout=10) as r:
        return r.read().decode()


def check(cond, what):
    print(("ok   " if cond else "FAIL ") + what)
    if not cond:
        sys.exit(1)


def http_flow(email):
    r = post("register.php", email=email, password="pw", uniq_id="x", check_hash="x", random=7)
    check(r == "REGISTERED\n" + md5("REGISTERED_7_" + SECRET), "register " + email)
    r = post("enter_game.php", email=email, password="pw", random=11)
    text, key = r.split("\n")
    check(key == md5(text + "_11_" + SECRET), "enter_game signature")
    pid, auth = text.split("\t")
    r = post("get_profile.php", id=pid, auth_key=auth, random=5, yoba_key="YOBA")
    text, key = r.split("\n")
    check(text.startswith("Saved Profile: rnd=5,soc=ws,player=" + pid) and key == md5(text + "_YOBA"), "get_profile")
    check(post("set_name.php", id=pid, auth_key=auth, name=email.split("@")[0]) == "SET_NAME", "set_name")
    r = post("purchase_buy.php", viewer_id=pid, auth_key=auth, p_ind=6, count=1, pay_coins=0, pay_gems=20,
             random=3, yoba_key="K", beta_key="")
    check(r == "SUCCESS\n" + md5("SUCCESS_3_K"), "purchase_buy (slot)")
    check(post("get_slot_info.php", id=pid, map_place=1, random=1) == "EMPTY SLOT", "slot 1 empty")
    check(post("get_slot_info.php", id=pid, map_place=7, random=1) == "NOT AVAILABLE SLOT", "slot 7 unavailable")
    blob = os.urandom(5000)
    r = post_multipart("save_map_to_slot_new.php", "/WB/", {"id": pid, "map_place": 1, "auth_key": auth}, "buffer", blob)
    check(r == "OK", "save map")
    with urllib.request.urlopen("http://%s:%d/WB/worlds/%s/%s_1.map?random=1" % (HOST, HTTP, md5(pid)[:2], pid)) as resp:
        check(resp.read() == blob, "load map back")
    check(post("get_slot_info.php", id=pid, map_place=1, random=1) == "1", "slot 1 saved")
    map_id = post("public_map.php", id=pid, slot_id=1, name="Тест", about="о карте", url="", auth_key=auth, random=1)
    check(post("get_latest_map.php", page=0, random=1).startswith("\t" + map_id + "\t" + pid), "map published")
    return pid, auth


# ---------------------------------------------------------------- Photon client

class Client:
    def __init__(self, port, name):
        self.name = name
        self.s = socket.create_connection((HOST, port), timeout=5)
        self.s.sendall(self.frame(b"\xf3\x00" + bytes([1, 6, 1, 3, 2, 1, 7]) + b"Master".ljust(32, b"\0")))
        check(self.read()[:2] == (1, b""), name + ": init")
        secret = 12345678901234567890
        pub = int_to_bytes(pow(GENERATOR, secret, OAKLEY_PRIME_768))
        self.s.sendall(self.frame(b"\xf3\x06" + self.request(0, {1: pub})))
        mtype, body = self.read()
        r = p16.Reader(body)
        r.u8(); r.i16(); r.value()
        server_pub = r.params()[1]
        check(mtype == 7 and len(server_pub) > 80, name + ": key exchange")

    @staticmethod
    def frame(payload):
        return b"\xfb" + struct.pack(">I", 7 + len(payload)) + b"\x00\x01" + payload

    @staticmethod
    def request(op, params):
        w = p16.Writer()
        w.u8(op)
        w.params(params)
        return w.getvalue()

    def recv(self, n):
        b = b""
        while len(b) < n:
            c = self.s.recv(n - len(b))
            if not c:
                raise ConnectionError("closed")
            b += c
        return b

    def read(self):
        head = self.recv(9)
        if head[0] == 0xF0:
            return self.read()
        (total,) = struct.unpack(">I", head[1:5])
        return head[8], self.recv(total - 9)

    def op(self, op, params):
        self.s.sendall(self.frame(b"\xf3\x02" + self.request(op, params)))
        return self.wait_response(op)

    def wait_response(self, op):
        while True:
            mtype, body = self.read()
            r = p16.Reader(body)
            if mtype == 3:
                code, rc = r.u8(), r.i16()
                r.value()
                params = r.params()
                if code == op:
                    return rc, params
            elif mtype == 4:
                self.events.append((r.u8(), r.params()))

    def event(self, code, timeout=5):
        end = time.time() + timeout
        while time.time() < end:
            for i, (c, p) in enumerate(self.events):
                if c == code:
                    return self.events.pop(i)[1]
            mtype, body = self.read()
            r = p16.Reader(body)
            if mtype == 4:
                self.events.append((r.u8(), r.params()))
        raise TimeoutError("%s: no event %d" % (self.name, code))

    events = []

    def close(self):
        self.s.close()


def connect_to(addr, name):
    host, port = addr.rsplit(":", 1)
    c = Client(int(port), name)
    c.events = []
    rc, _ = c.op(230, {220: VERSION, 224: "Master", 225: name})
    check(rc == 0, name + ": authenticate on " + addr)
    return c


def photon_flow():
    s = socket.create_connection((HOST, 843), timeout=5)
    s.sendall(b"<policy-file-request/>\0")
    check(b"cross-domain-policy" in s.recv(1000), "socket policy")
    s.close()

    a = connect_to("%s:5055" % HOST, "alice")
    rc, _ = a.op(229, {})
    check(rc == 0 and a.event(230) is not None, "alice: lobby + game list")
    rc, _ = a.op(225, {248: {"player_id": "1", "slot_id": 1}})
    check(rc == 32760, "alice: join random fails (no rooms)")
    rc, p = a.op(227, {})
    room, addr = p[255], p[230]
    check(rc == 0 and addr.endswith(":5056"), "alice: create on master -> " + addr)
    a.close()

    ag = connect_to(addr, "alice")
    props = {"player_id": "1", "slot_id": 1, Byte(253): True, Byte(254): True, Byte(255): 12,  # int, as PUN sends it
             Byte(250): p16.StrArray(["player_id", "slot_id"])}
    rc, p = ag.op(227, {255: room, 248: props, 249: {Byte(255): "alice"}, 250: True, 241: True})
    check(rc == 0 and p[254] == 1, "alice: create on game server, actor 1")
    check(type(p[248][255]) is Byte and p[248][255] == 12, "maxPlayers comes back as byte")
    check(ag.event(255)[254] == 1, "alice: own join event")
    vec = Custom(86, struct.pack(">fff", 1, 2, 3))
    ag.s.sendall(ag.frame(b"\xf3\x02" + ag.request(253, {244: Byte(202), 245: {Byte(0): "Player", Byte(1): vec, Byte(7): 1001},
                                                          247: Byte(4)})))

    b = connect_to("%s:5055" % HOST, "bob")
    b.op(229, {})
    time.sleep(1.5)
    lst = b.event(230)[222]
    check(room in lst and lst[room][Byte(252)] == 1 and lst[room]["slot_id"] == 1, "bob: sees alice's room in lobby")
    rc, p = b.op(225, {248: {"player_id": "1", "slot_id": 1}})
    check(rc == 0 and p[255] == room, "bob: join random finds it")
    b.close()
    bg = connect_to(p[230], "bob")
    rc, p = bg.op(226, {255: room, 249: {Byte(255): "bob"}, 250: True})
    check(rc == 0 and p[254] == 2 and set(p[249]) == {1, 2}, "bob: joined as actor 2")
    join = bg.event(255)
    check(join[254] == 2 and list(join[252]) == [1, 2], "bob: join event with actor list")
    inst = bg.event(202)
    check(inst[254] == 1 and inst[245][Byte(1)] == vec, "bob: got alice's cached instantiate (Vector3 intact)")
    check(ag.event(255)[254] == 2, "alice: sees bob join")

    bg.s.sendall(bg.frame(b"\xf3\x02" + bg.request(253, {244: Byte(200), 245: {Byte(3): "Chat", Byte(4): ["hi", 5]}})))
    rpc = ag.event(200)
    check(rpc[254] == 2 and rpc[245][Byte(4)] == ["hi", 5], "alice: RPC from bob")

    # PUN sends target actors as a typed int array ('y' + 'i'), not as 'n'.
    ag.s.sendall(ag.frame(b"\xf3\x02" + ag.request(253, {244: Byte(200), 245: {Byte(3): "ToBob"},
                                                          252: p16.TypedArray(105, [2])})))
    check(bg.event(200)[245][Byte(3)] == "ToBob", "bob: RPC targeted with typed int[]")

    rc, _ = ag.op(252, {251: {"map_name": "Test"}, 250: True})
    check(rc == 0 and bg.event(253)[251]["map_name"] == "Test", "room property broadcast")

    rc, _ = bg.op(254, {})
    check(rc == 0 and ag.event(254)[254] == 2, "bob leaves, alice notified")
    ag.close()
    bg.close()


if __name__ == "__main__":
    tag = str(int(time.time()))
    http_flow("tester%s@local" % tag)
    photon_flow()
    print("all good")
