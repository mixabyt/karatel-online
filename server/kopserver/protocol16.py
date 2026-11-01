"""Photon protocol 1.6 values (the Photon3 client SDK 3.0 used by the 2012 client).

Every parsed value keeps its exact wire bytes in `raw`, so room/player properties and event payloads
are relayed byte for byte, including the client's custom types (Vector3, PhotonPlayer, ...).
"""
import struct

NULL, DICT, STR_ARRAY, BYTE, CUSTOM, DOUBLE, FLOAT = 42, 68, 97, 98, 99, 100, 102
HASHTABLE, INT, SHORT, LONG, INT_ARRAY, BOOL, STR = 104, 105, 107, 108, 110, 111, 115
VECTOR, BYTE_ARRAY, ARRAY, OBJ_ARRAY = 118, 120, 121, 122

_FIXED = {BYTE: 1, BOOL: 1, SHORT: 2, INT: 4, LONG: 8, FLOAT: 4, DOUBLE: 8}


class Value:
    """A typed value: `type` code, decoded `v` and the exact `raw` bytes including the type code."""
    __slots__ = ("type", "v", "raw")

    def __init__(self, type_, v, raw):
        self.type, self.v, self.raw = type_, v, raw

    def __repr__(self):
        return f"Value({chr(self.type) if 32 < self.type < 127 else self.type}, {self.v!r})"


def _body(buf, pos, t):
    """Decode the body of a value of type `t` at `pos`; returns (decoded, new_pos)."""
    if t in (NULL, 0):
        return None, pos
    if t in _FIXED:
        n = _FIXED[t]
        chunk = buf[pos:pos + n]
        if len(chunk) != n:
            raise ValueError("truncated value")
        if t == BYTE:
            return chunk[0], pos + 1
        if t == BOOL:
            return chunk[0] != 0, pos + 1
        fmt = {SHORT: ">h", INT: ">i", LONG: ">q", FLOAT: ">f", DOUBLE: ">d"}[t]
        return struct.unpack(fmt, chunk)[0], pos + n
    if t == STR:
        (n,) = struct.unpack_from(">h", buf, pos)
        return buf[pos + 2:pos + 2 + n].decode("utf-8", "replace"), pos + 2 + n
    if t == BYTE_ARRAY:
        (n,) = struct.unpack_from(">i", buf, pos)
        return bytes(buf[pos + 4:pos + 4 + n]), pos + 4 + n
    if t == INT_ARRAY:
        (n,) = struct.unpack_from(">i", buf, pos)
        return list(struct.unpack_from(f">{n}i", buf, pos + 4)), pos + 4 + 4 * n
    if t == STR_ARRAY:
        (n,) = struct.unpack_from(">h", buf, pos)
        pos += 2
        out = []
        for _ in range(n):
            s, pos = _body(buf, pos, STR)
            out.append(s)
        return out, pos
    if t == HASHTABLE:
        (n,) = struct.unpack_from(">h", buf, pos)
        pos += 2
        items = []
        for _ in range(n):
            k, pos = read(buf, pos)
            v, pos = read(buf, pos)
            items.append((k, v))
        return items, pos
    if t == DICT:
        kt, vt = buf[pos], buf[pos + 1]
        (n,) = struct.unpack_from(">h", buf, pos + 2)
        pos += 4
        items = []
        for _ in range(n):
            if kt in (0, NULL):
                k, pos = read(buf, pos)
            else:
                k, pos = _body(buf, pos, kt)
            if vt in (0, NULL):
                v, pos = read(buf, pos)
            else:
                v, pos = _body(buf, pos, vt)
            items.append((k, v))
        return items, pos
    if t == OBJ_ARRAY:
        (n,) = struct.unpack_from(">h", buf, pos)
        pos += 2
        out = []
        for _ in range(n):
            v, pos = read(buf, pos)
            out.append(v)
        return out, pos
    if t == VECTOR:
        (n,) = struct.unpack_from(">h", buf, pos)
        pos += 2
        out = []
        if n > 0:
            et = buf[pos]
            pos += 1
            for _ in range(n):
                v, pos = _body(buf, pos, et)
                out.append(v)
        return out, pos
    if t == CUSTOM:
        code = buf[pos]
        (n,) = struct.unpack_from(">h", buf, pos + 1)
        return ("custom", code, bytes(buf[pos + 3:pos + 3 + n])), pos + 3 + n
    if t == ARRAY:
        return _array_body(buf, pos)
    raise ValueError(f"unsupported protocol16 type {t}")


def _array_body(buf, pos):
    (n,) = struct.unpack_from(">h", buf, pos)
    et = buf[pos + 2]
    pos += 3
    out = []
    if et == ARRAY:
        for _ in range(n):
            v, pos = _array_body(buf, pos)
            out.append(v)
    elif et == CUSTOM:
        code = buf[pos]
        pos += 1
        for _ in range(n):
            (ln,) = struct.unpack_from(">h", buf, pos)
            out.append(("custom", code, bytes(buf[pos + 2:pos + 2 + ln])))
            pos += 2 + ln
    elif et == DICT:
        kt, vt = buf[pos], buf[pos + 1]
        pos += 2
        for _ in range(n):
            (cnt,) = struct.unpack_from(">h", buf, pos)
            pos += 2
            d = []
            for _ in range(cnt):
                k, pos = read(buf, pos) if kt in (0, NULL) else _body(buf, pos, kt)
                v, pos = read(buf, pos) if vt in (0, NULL) else _body(buf, pos, vt)
                d.append((k, v))
            out.append(d)
    else:
        for _ in range(n):
            v, pos = _body(buf, pos, et)
            out.append(v)
    return out, pos


def read(buf, pos):
    """Read one typed value; returns (Value, new_pos)."""
    t = buf[pos]
    v, end = _body(buf, pos + 1, t)
    return Value(t, v, bytes(buf[pos:end])), end


def read_params(buf, pos):
    """Read a parameter table {byte key: Value}."""
    (n,) = struct.unpack_from(">h", buf, pos)
    pos += 2
    out = {}
    for _ in range(n):
        key = buf[pos]
        out[key], pos = read(buf, pos + 1)
    return out, pos


def parse(raw):
    return read(raw, 0)[0]


# ---- builders: each returns typed wire bytes ----

def null():
    return b"*"


def byte(x):
    return bytes([BYTE, x & 0xFF])


def boolean(x):
    return bytes([BOOL, 1 if x else 0])


def short(x):
    return bytes([SHORT]) + struct.pack(">h", x)


def integer(x):
    return bytes([INT]) + struct.pack(">i", x)


def string(s):
    raw = s.encode("utf-8")
    return bytes([STR]) + struct.pack(">h", len(raw)) + raw


def int_array(xs):
    return bytes([ARRAY]) + struct.pack(">h", len(xs)) + bytes([INT]) + b"".join(struct.pack(">i", x) for x in xs)


def hashtable(pairs):
    """`pairs`: iterable of (key_raw, value_raw) typed wire bytes."""
    pairs = list(pairs)
    return bytes([HASHTABLE]) + struct.pack(">h", len(pairs)) + b"".join(k + v for k, v in pairs)


def params(d):
    """`d`: {byte key: typed wire bytes}."""
    return struct.pack(">h", len(d)) + b"".join(bytes([k]) + v for k, v in d.items())


def operation_response(opcode, return_code=0, debug=None, parameters=None):
    return (b"\xf3\x03" + bytes([opcode]) + struct.pack(">h", return_code)
            + (string(debug) if debug else null()) + params(parameters or {}))


def event(code, parameters):
    return b"\xf3\x04" + bytes([code]) + params(parameters)


def items(value):
    """Hashtable Value -> {key_raw: value_raw} (insertion ordered); anything else -> {}."""
    if value is None or value.type != HASHTABLE:
        return {}
    return {k.raw: v.raw for k, v in value.v}


def key_byte(b):
    """Wire bytes of a byte-typed hashtable key, e.g. room property 255 (max players)."""
    return byte(b)
