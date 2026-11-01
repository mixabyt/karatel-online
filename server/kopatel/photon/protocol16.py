"""Photon "Protocol16" serialization, as in ExitGames.Client.Photon.Protocol (Photon3Unity3D 3.2).

Values decode to Python objects that remember their wire type, so whatever a client sends can be
relayed to other clients byte-for-byte equivalent:

    None, bool, str, bytes, int (Integer), Byte/Short/Long (int subclasses), Float (float subclass),
    float (Double), dict (Hashtable), list (object[]), IntArray, StrArray, TypedArray, Dictionary,
    Custom (registered custom types such as Vector3 are kept as raw bytes).
"""
import io
import struct


class Byte(int):
    pass


class Short(int):
    pass


class Long(int):
    pass


class Float(float):
    pass


class IntArray(list):
    pass


class StrArray(list):
    pass


class TypedArray:
    """'y' array: elem is the element type code; items are decoded values. For custom element
    types custom_code is set; for dictionary elements dict_types is (key_code, value_code)."""

    def __init__(self, elem, items, custom_code=None, dict_types=None):
        self.elem, self.items, self.custom_code, self.dict_types = elem, items, custom_code, dict_types

    # PUN sends int[] (e.g. target actors) this way, so let the server read it like a list.
    def __iter__(self):
        return iter(self.items)

    def __len__(self):
        return len(self.items)

    def __eq__(self, other):
        try:
            return list(self.items) == list(other)
        except TypeError:
            return False

    __hash__ = None

    def __repr__(self):
        return "TypedArray(%r, %r)" % (chr(self.elem), self.items)


class Dictionary:
    """'D' Dictionary<K,V>: key/value codes 0 mean "object" (each entry carries its own type)."""

    def __init__(self, key_code, value_code, pairs):
        self.key_code, self.value_code, self.pairs = key_code, value_code, pairs

    def __repr__(self):
        return "Dictionary(%r)" % (self.pairs,)


class Custom:
    def __init__(self, code, data):
        self.code, self.data = code, bytes(data)

    def __eq__(self, other):
        return isinstance(other, Custom) and (self.code, self.data) == (other.code, other.data)

    def __hash__(self):
        return hash((self.code, self.data))

    def __repr__(self):
        return "Custom(%r, %s)" % (chr(self.code), self.data.hex())


# ---------------------------------------------------------------- reading

class Reader:
    def __init__(self, data, pos=0):
        self.b = data
        self.p = pos

    def take(self, n):
        if self.p + n > len(self.b):
            raise ValueError("truncated Photon data")
        v = self.b[self.p:self.p + n]
        self.p += n
        return v

    def u8(self):
        return self.take(1)[0]

    def i16(self):
        return struct.unpack(">h", self.take(2))[0]

    def i32(self):
        return struct.unpack(">i", self.take(4))[0]

    def value(self, code=None):
        if code is None:
            code = self.u8()
        if code in (0, 42):  # Unknown / Null
            return None
        if code == 105:  # 'i'
            return self.i32()
        if code == 115:  # 's'
            n = self.i16()
            return self.take(n).decode("utf-8") if n > 0 else ""
        if code == 98:  # 'b'
            return Byte(self.u8())
        if code == 111:  # 'o'
            return self.u8() != 0
        if code == 107:  # 'k'
            return Short(self.i16())
        if code == 108:  # 'l'
            return Long(struct.unpack(">q", self.take(8))[0])
        if code == 102:  # 'f'
            return Float(struct.unpack(">f", self.take(4))[0])
        if code == 100:  # 'd'
            return struct.unpack(">d", self.take(8))[0]
        if code == 120:  # 'x'
            return bytes(self.take(self.i32()))
        if code == 110:  # 'n'
            n = self.i32()
            return IntArray(self.i32() for _ in range(n))
        if code == 97:  # 'a'
            n = self.i16()
            return StrArray(self.value(115) for _ in range(n))
        if code == 122:  # 'z'
            n = self.i16()
            return [self.value() for _ in range(n)]
        if code == 104:  # 'h'
            n = self.i16()
            h = {}
            for _ in range(n):
                k = self.value()
                h[k] = self.value()
            return h
        if code == 68:  # 'D'
            return self.dictionary()
        if code == 121:  # 'y'
            return self.array()
        if code == 99:  # 'c'
            cc = self.u8()
            return Custom(cc, self.take(self.i16()))
        raise ValueError("unsupported Photon type code %d" % code)

    def dictionary(self):
        kc, vc = self.u8(), self.u8()
        n = self.i16()
        pairs = []
        for _ in range(n):
            k = self.value(None if kc in (0, 42) else kc)
            v = self.value(None if vc in (0, 42) else vc)
            pairs.append((k, v))
        return Dictionary(kc, vc, pairs)

    def array(self):
        n = self.i16()
        elem = self.u8()
        if elem == 121:
            return TypedArray(elem, [self.array() for _ in range(n)])
        if elem == 99:
            cc = self.u8()
            return TypedArray(elem, [Custom(cc, self.take(self.i16())) for _ in range(n)], custom_code=cc)
        if elem == 68:
            kc, vc = self.u8(), self.u8()
            items = []
            for _ in range(n):
                m = self.i16()
                pairs = []
                for _ in range(m):
                    k = self.value(kc if kc else None)
                    v = self.value(vc if vc else None)
                    pairs.append((k, v))
                items.append(Dictionary(kc, vc, pairs))
            return TypedArray(elem, items, dict_types=(kc, vc))
        return TypedArray(elem, [self.value(elem) for _ in range(n)])

    def params(self):
        n = self.i16()
        out = {}
        for _ in range(n):
            k = self.u8()
            out[k] = self.value()
        return out


# ---------------------------------------------------------------- writing

def type_code(v):
    if v is None:
        return 42
    if isinstance(v, bool):
        return 111
    if isinstance(v, Byte):
        return 98
    if isinstance(v, Short):
        return 107
    if isinstance(v, Long):
        return 108
    if isinstance(v, int):
        return 105
    if isinstance(v, Float):
        return 102
    if isinstance(v, float):
        return 100
    if isinstance(v, str):
        return 115
    if isinstance(v, (bytes, bytearray)):
        return 120
    if isinstance(v, IntArray):
        return 110
    if isinstance(v, StrArray):
        return 97
    if isinstance(v, list):
        return 122
    if isinstance(v, dict):
        return 104
    if isinstance(v, Dictionary):
        return 68
    if isinstance(v, TypedArray):
        return 121
    if isinstance(v, Custom):
        return 99
    raise TypeError("cannot serialize %r" % (v,))


class Writer:
    def __init__(self):
        self.o = io.BytesIO()

    def getvalue(self):
        return self.o.getvalue()

    def u8(self, v):
        self.o.write(bytes((v & 0xFF,)))

    def i16(self, v):
        self.o.write(struct.pack(">h", v))

    def i32(self, v):
        self.o.write(struct.pack(">i", v))

    def value(self, v, with_type=True):
        code = type_code(v)
        if with_type:
            self.u8(code)
        if code == 42:
            return
        if code == 111:
            self.u8(1 if v else 0)
        elif code == 98:
            self.u8(v)
        elif code == 107:
            self.i16(v)
        elif code == 108:
            self.o.write(struct.pack(">q", v))
        elif code == 105:
            self.i32(v)
        elif code == 102:
            self.o.write(struct.pack(">f", v))
        elif code == 100:
            self.o.write(struct.pack(">d", v))
        elif code == 115:
            b = v.encode("utf-8")
            self.i16(len(b))
            self.o.write(b)
        elif code == 120:
            self.i32(len(v))
            self.o.write(v)
        elif code == 110:
            self.i32(len(v))
            for x in v:
                self.i32(x)
        elif code == 97:
            self.i16(len(v))
            for x in v:
                self.value(x, False)
        elif code == 122:
            self.i16(len(v))
            for x in v:
                self.value(x)
        elif code == 104:
            self.i16(len(v))
            for k, x in v.items():
                self.value(k)
                self.value(x)
        elif code == 68:
            self.u8(v.key_code)
            self.u8(v.value_code)
            self.dict_pairs(v)
        elif code == 121:
            self.array(v)
        elif code == 99:
            self.u8(v.code)
            self.i16(len(v.data))
            self.o.write(v.data)

    def dict_pairs(self, d):
        self.i16(len(d.pairs))
        for k, x in d.pairs:
            self.value(k, d.key_code in (0, 42))
            self.value(x, d.value_code in (0, 42))

    def array(self, a):
        self.i16(len(a.items))
        self.u8(a.elem)
        if a.elem == 121:
            for x in a.items:
                self.array(x)
        elif a.elem == 99:
            self.u8(a.custom_code)
            for x in a.items:
                self.i16(len(x.data))
                self.o.write(x.data)
        elif a.elem == 68:
            kc, vc = a.dict_types
            self.u8(kc)
            self.u8(vc)
            for x in a.items:
                self.dict_pairs(x)
        else:
            for x in a.items:
                self.value(x, False)

    def params(self, params):
        self.i16(len(params))
        for k, v in params.items():
            self.u8(k)
            self.value(v)


# ---------------------------------------------------------------- messages

def read_request(body):
    """body = opCode + parameter table (after the F3/type bytes)."""
    r = Reader(body)
    op = r.u8()
    return op, r.params()


def write_response(op, params=None, return_code=0, debug=None):
    w = Writer()
    w.u8(op)
    w.i16(return_code)
    if debug:
        w.value(debug)
    else:
        w.u8(42)
    w.params(params or {})
    return w.getvalue()


def write_event(code, params):
    w = Writer()
    w.u8(code)
    w.params(params)
    return w.getvalue()
