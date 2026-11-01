"""Probe a Photon (protocol 1.6, UDP) master server the way the 2012 Kopatel client does:
connect, send init with the app id, then OpAuthenticate (230) and print the server's answer.

Usage: python -I photon_probe.py <host> <port> <app_id> [game_version] [region]
"""
import random
import socket
import struct
import sys
import time

START = time.monotonic()


def now_ms():
    return int((time.monotonic() - START) * 1000) & 0x7FFFFFFF


class Probe:
    def __init__(self, host, port):
        self.addr = (socket.gethostbyname(host), port)
        self.sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        self.sock.settimeout(3.0)
        self.peer_id = -1
        self.challenge = random.randint(0, 0x7FFFFFFF)
        self.seq = {0: 0, 255: 0}

    def command(self, ctype, channel, payload, flags=1, seq=None):
        if seq is None:
            self.seq[channel] += 1
            seq = self.seq[channel]
        return struct.pack(">BBBBii", ctype, channel, flags, 4, 12 + len(payload), seq) + payload

    def send(self, *commands):
        header = struct.pack(">hBBii", self.peer_id, 0, len(commands), now_ms(), self.challenge)
        self.sock.sendto(header + b"".join(commands), self.addr)

    def receive(self):
        data, _ = self.sock.recvfrom(4096)
        peer_id, _crc, count, server_time, _challenge = struct.unpack_from(">hBBii", data, 0)
        pos, out = 12, []
        for _ in range(count):
            ctype, channel, flags, _res, size, seq = struct.unpack_from(">BBBBii", data, pos)
            header_len = 16 if ctype == 7 else 32 if ctype == 8 else 12
            out.append((ctype, channel, flags, seq, server_time, data[pos + header_len:pos + size]))
            pos += size
        return out

    def ack(self, cmd):
        ctype, channel, flags, seq, server_time, _ = cmd
        if flags & 1:
            self.send(struct.pack(">BBBBii", 1, channel, 1, 4, 20, seq) + struct.pack(">ii", seq, server_time))

    def wait_for(self, predicate, what, attempts=4, resend=None):
        for _ in range(attempts):
            try:
                for cmd in self.receive():
                    self.ack(cmd)
                    if predicate(cmd):
                        return cmd
            except socket.timeout:
                print(f"  ...no answer yet ({what})")
                if resend:
                    resend()
        return None


def pstring(key, text):
    raw = text.encode("utf-8")
    return struct.pack(">BBh", key, ord("s"), len(raw)) + raw


def read_value(buf, pos):
    t = buf[pos]; pos += 1
    if t == ord("s"):
        (n,) = struct.unpack_from(">h", buf, pos); pos += 2
        return buf[pos:pos + n].decode("utf-8", "replace"), pos + n
    if t == ord("*"):
        return None, pos
    if t == ord("i"):
        return struct.unpack_from(">i", buf, pos)[0], pos + 4
    if t == ord("b"):
        return buf[pos], pos + 1
    if t == ord("k"):
        return struct.unpack_from(">h", buf, pos)[0], pos + 2
    return f"<type {chr(t)!r}, raw {buf[pos:pos + 40].hex()}>", len(buf)


def main(host, port, app_id, version, region=None):
    p = Probe(host, port)
    print(f"target {host} -> {p.addr[0]}:{port}")

    connect_payload = bytearray(32)
    connect_payload[2:4] = struct.pack(">h", 1200)
    connect_payload[6] = 128
    connect_payload[11] = 2
    connect_payload[22:24] = bytes([19, 136])
    connect_payload[27] = 2
    connect_payload[31] = 2
    connect_cmd = p.command(2, 255, bytes(connect_payload))
    p.send(connect_cmd)
    print("sent CONNECT")
    verify = p.wait_for(lambda c: c[0] == 3, "verify connect", resend=lambda: p.send(connect_cmd))
    if not verify:
        print("RESULT: no VerifyConnect - the UDP server does not answer")
        return
    (p.peer_id,) = struct.unpack_from(">h", verify[5], 0)
    print(f"got VERIFY_CONNECT, peer id {p.peer_id}")

    init = bytes([243, 0, 1, 6, 1, 3, 0, 1, 7]) + app_id.encode("ascii")[:32].ljust(32, b"\0")
    p.send(p.command(6, 0, init))
    print("sent INIT")
    init_resp = p.wait_for(lambda c: c[0] == 6 and c[5][:2] == b"\xf3\x01", "init response")
    if not init_resp:
        print("RESULT: connected, but no InitResponse")
        return
    print("got INIT_RESPONSE")

    params = pstring(220, version) + pstring(224, app_id) + (pstring(210, region) if region else b"")
    op = bytes([0xF3, 0x02, 230]) + struct.pack(">h", 3 if region else 2) + params
    p.send(p.command(6, 0, op))
    print(f"sent OpAuthenticate(appVersion={version!r})")
    resp = p.wait_for(lambda c: c[0] == 6 and c[5][:2] == b"\xf3\x03", "auth response")
    if not resp:
        print("RESULT: no answer to OpAuthenticate")
        return
    body = resp[5]
    opcode = body[2]
    (ret,) = struct.unpack_from(">h", body, 3)
    msg, pos = read_value(body, 5)
    (count,) = struct.unpack_from(">h", body, pos); pos += 2
    params = {}
    for _ in range(count):
        key = body[pos]; pos += 1
        params[key], pos = read_value(body, pos)
    print(f"RESULT: op {opcode} returnCode={ret} debug={msg!r} params={params}")
    p.send(p.command(4, 255, b"", flags=2))


if __name__ == "__main__":
    a = sys.argv
    main(a[1], int(a[2]), a[3], a[4] if len(a) > 4 else "WEBSITE1Qshard_1", a[5] if len(a) > 5 else None)
