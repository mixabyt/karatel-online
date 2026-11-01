"""Unpack / repack a UnityWeb (.unity3d web player) bundle, format v3 (Unity 3.x/4.x).

  python -I unityweb.py unpack <bundle> <out_dir>
  python -I unityweb.py pack <original_bundle> <files_dir> <out_bundle>

pack keeps the original's header strings and file order and takes each file's
bytes from <files_dir> (so a single patched DLL can be dropped in).
"""
import lzma
import os
import struct
import sys

LZMA_DICT = 1 << 19  # same dictionary size as the original bundle


def read_cstr(b, p):
    e = b.index(b"\0", p)
    return b[p:e].decode("latin-1"), e + 1


def parse_header(b):
    p = 0
    sig, p = read_cstr(b, p)
    if sig != "UnityWeb":
        raise ValueError("not a UnityWeb bundle: %r" % sig)
    (fmt,) = struct.unpack_from(">I", b, p); p += 4
    player, p = read_cstr(b, p)
    engine, p = read_cstr(b, p)
    size, header_size, levels, levels2 = struct.unpack_from(">IIII", b, p)
    return dict(fmt=fmt, player=player, engine=engine, size=size, header_size=header_size,
                levels=levels, levels2=levels2)


def read_entries(b):
    h = parse_header(b)
    data = decompress_lzma_alone(b[h["header_size"]:])
    (count,) = struct.unpack_from(">I", data, 0)
    p = 4
    entries = []
    for _ in range(count):
        name, p = read_cstr(data, p)
        off, size = struct.unpack_from(">II", data, p); p += 8
        entries.append((name, data[off:off + size]))
    return h, entries


def unpack(path, out_dir):
    h, entries = read_entries(open(path, "rb").read())
    print(h)
    for name, blob in entries:
        dst = os.path.join(out_dir, name)
        os.makedirs(os.path.dirname(dst) or out_dir, exist_ok=True)
        with open(dst, "wb") as f:
            f.write(blob)
        print("%10d  %s" % (len(blob), name))


def compress_lzma_alone(raw):
    """.lzma stream with the real size in its header, as Unity writes it.

    Python's lzma always appends an end marker as well; the LZMA SDK decoder in the player
    stops at the declared size and never reads it."""
    filters = [{"id": lzma.FILTER_LZMA1, "dict_size": LZMA_DICT, "lc": 3, "lp": 0, "pb": 2}]
    comp = bytearray(lzma.compress(raw, format=lzma.FORMAT_ALONE, filters=filters))
    comp[5:13] = struct.pack("<Q", len(raw))
    return bytes(comp)


def decompress_lzma_alone(comp):
    try:
        return lzma.decompress(comp, format=lzma.FORMAT_ALONE)
    except lzma.LZMAError:
        # liblzma rejects "known size + end marker" (our own packs); read it as unknown size.
        return lzma.decompress(comp[:5] + b"\xff" * 8 + comp[13:], format=lzma.FORMAT_ALONE)


def pack(original, files_dir, out_path):
    h, entries = read_entries(open(original, "rb").read())
    names = [name for name, _ in entries]
    table_size = 4 + sum(len(n.encode("latin-1")) + 1 + 8 for n in names)
    offset = (table_size + 3) & ~3
    table = struct.pack(">I", len(names))
    blobs = []
    for name in names:
        blob = open(os.path.join(files_dir, name), "rb").read()
        table += name.encode("latin-1") + b"\0" + struct.pack(">II", offset, len(blob))
        blobs.append(blob)
        offset += len(blob)
    data_header = (table_size + 3) & ~3
    raw = table.ljust(data_header, b"\0") + b"".join(blobs)

    comp = compress_lzma_alone(raw)

    def header(total):
        hb = b"UnityWeb\0" + struct.pack(">I", h["fmt"])
        hb += h["player"].encode() + b"\0" + h["engine"].encode() + b"\0"
        fields = [total, 0, h["levels"], h["levels2"]]
        # Every level becomes available only once the whole stream is in: simpler than
        # recomputing streaming boundaries, and the game is a single download anyway.
        for _ in range(h["levels2"]):
            fields += [len(comp), len(raw)]
        fields += [total, data_header]
        return hb, fields

    hb, fields = header(0)
    header_size = len(hb) + 4 * len(fields) + 1  # the original ends its header with one zero byte
    total = header_size + len(comp)
    hb, fields = header(total)
    fields[1] = header_size
    out = hb + struct.pack(">" + "I" * len(fields), *fields) + b"\0" + bytes(comp)
    with open(out_path, "wb") as f:
        f.write(out)
    print("packed %d files, %d bytes -> %s" % (len(names), len(out), out_path))


if __name__ == "__main__":
    cmd = sys.argv[1]
    if cmd == "unpack":
        unpack(sys.argv[2], sys.argv[3])
    elif cmd == "pack":
        pack(sys.argv[2], sys.argv[3], sys.argv[4])
    else:
        sys.exit(__doc__)
