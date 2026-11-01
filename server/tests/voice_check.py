"""Voice chat check: a Photon bot sits in a room and counts the SendAudio RPCs the players send.

  python tests/voice_check.py [--room NAME] [--seconds 30] [--echo]

With --echo it sends every captured packet back to its speaker afterwards, addressed to the speaker's own
avatar: the speaker then hears themself, and the client logs "Kop voice: received ...". Without --room
the bot joins the only room on the server.
"""
import argparse
import os
import socket
import struct
import sys
import time

sys.path.insert(0, os.path.join(os.path.dirname(os.path.abspath(__file__)), ".."))
from kopserver import protocol16 as p16  # noqa: E402

RPC_EVENT = 200  # PhotonNetwork RPCs travel as events with this code


class PhotonClient:
    def __init__(self, host, port):
        self.s = socket.create_connection((host, port), timeout=5)
        self.buf = b""
        init = bytes([243, 0, 1, 6, 1, 3, 0, 1, 7]) + b"Master".ljust(32, b"\0")
        self.s.sendall(b"\xfb" + struct.pack(">i", 7 + len(init)) + b"\x00\x01" + init)
        self.next_message()

    def next_message(self):
        while True:
            if len(self.buf) >= 9 and self.buf[0] == 0xF0:  # ping answer
                self.buf = self.buf[9:]
                continue
            if len(self.buf) >= 5 and self.buf[0] == 0xFB:
                (n,) = struct.unpack(">i", self.buf[1:5])
                if len(self.buf) >= n:
                    msg, self.buf = self.buf[7:n], self.buf[n:]
                    return msg
            chunk = self.s.recv(65536)
            if not chunk:
                raise ConnectionError("server closed the connection")
            self.buf += chunk

    def op(self, code, params=None):
        payload = b"\xf3\x02" + bytes([code]) + p16.params(params or {})
        self.s.sendall(b"\xfb" + struct.pack(">i", 7 + len(payload)) + b"\x00\x01" + payload)

    def wait(self, kind, code, timeout=5):
        end = time.time() + timeout
        while time.time() < end:
            msg = self.next_message()
            if msg[1] == kind and msg[2] == code:
                if kind == 3:
                    (ret,) = struct.unpack(">h", msg[3:5])
                    _, pos = p16.read(msg, 5)
                    return ret, p16.read_params(msg, pos)[0]
                return 0, p16.read_params(msg, 3)[0]
        raise TimeoutError(f"no message {kind}/{code}")


def authenticate(client):
    client.op(230, {224: p16.string("voice-check"), 220: p16.string("voice-check")})
    client.wait(3, 230)


def find_room(host, wanted):
    master = PhotonClient(host, 4530)
    authenticate(master)
    master.op(229)
    master.wait(3, 229)
    _, ev = master.wait(4, 230)
    rooms = [name.v for name, _ in ev[222].v]
    if wanted is None:
        if len(rooms) != 1:
            master.s.close()
            return None, rooms
        wanted = rooms[0]
    elif wanted not in rooms:
        master.s.close()
        return None, rooms
    master.op(226, {255: p16.string(wanted)})
    ret, resp = master.wait(3, 226)
    if ret != 0:
        sys.exit(f"cannot join room {wanted!r}: error {ret}")
    master.s.close()
    return wanted, int(resp[230].v.split(":")[1])


def strings_and_bytes(value, out):
    """All strings and byte arrays inside a parsed value, depth first."""
    if value.type == p16.STR:
        out.append(value.v)
    elif value.type == p16.BYTE_ARRAY:
        out.append(value.v)
    elif value.type == p16.HASHTABLE:
        for k, v in value.v:
            strings_and_bytes(k, out)
            strings_and_bytes(v, out)
    elif value.type == p16.OBJ_ARRAY:
        for v in value.v:
            strings_and_bytes(v, out)
    return out


def main():
    ap = argparse.ArgumentParser(description=__doc__.split("\n")[0])
    ap.add_argument("--host", default="127.0.0.1")
    ap.add_argument("--room")
    ap.add_argument("--seconds", type=float, default=30)
    ap.add_argument("--echo", action="store_true", help="send the captured packets back to their speaker")
    ap.add_argument("--wait", type=float, default=0, help="seconds to wait for the room to appear")
    args = ap.parse_args()

    deadline = time.time() + args.wait
    while True:
        room, port = find_room(args.host, args.room)
        if room is not None:
            break
        if time.time() >= deadline:
            sys.exit(f"rooms on the server: {port}; pick one with --room")
        time.sleep(3)
    game = PhotonClient(args.host, port)
    authenticate(game)
    game.op(226, {255: p16.string(room), 249: p16.hashtable([(p16.byte(255), p16.string("voice-check"))])})
    ret, joined = game.wait(3, 226)
    if ret != 0:
        sys.exit(f"cannot enter room {room!r}: error {ret}")
    print(f"in room {room!r} as player {joined[254].v}, listening for {args.seconds:.0f} s")

    captured = {}  # speaker actor -> [raw RPC data]
    sizes = []
    game.s.settimeout(0.5)
    end = time.time() + args.seconds
    while time.time() < end:
        try:
            msg = game.next_message()
        except socket.timeout:
            continue
        if msg[1] != 4 or msg[2] != RPC_EVENT:
            continue
        params = p16.read_params(msg, 3)[0]
        found = strings_and_bytes(params[245], [])
        if "SendAudio" not in found:
            continue
        actor = params[254].v
        captured.setdefault(actor, []).append(params[245].raw)
        sizes += [len(x) for x in found if isinstance(x, bytes)]
    for actor, packets in captured.items():
        print(f"player {actor}: {len(packets)} SendAudio packets ({len(packets) * 0.04:.1f} s of speech)")
    if sizes:
        print(f"voice payload {min(sizes)}..{max(sizes)} bytes per packet")
    else:
        print("no SendAudio packets: nobody held P (or the client did not send)")

    if args.echo:
        for actor, packets in captured.items():
            print(f"echoing {len(packets)} packets back to player {actor}")
            for raw in packets:
                game.op(253, {244: p16.byte(RPC_EVENT), 245: raw, 252: p16.int_array([actor])})
                time.sleep(0.04)
        time.sleep(1)
    game.op(254)
    game.s.close()
    sys.exit(0 if captured else 1)


if __name__ == "__main__":
    main()
