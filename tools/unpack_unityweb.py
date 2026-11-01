"""Unpack a UnityWeb (.unity3d, Unity 3.x web player) bundle into its files.

Usage: python -I unpack_unityweb.py <bundle.unity3d> <out_dir>
"""
import lzma
import os
import struct
import sys


def read_cstr(buf, pos):
    end = buf.index(b"\0", pos)
    return buf[pos:end].decode("ascii", "replace"), end + 1


def main(src, out_dir):
    data = open(src, "rb").read()
    sig, pos = read_cstr(data, 0)
    if sig != "UnityWeb":
        sys.exit(f"not a UnityWeb bundle: {sig!r}")
    (fmt,) = struct.unpack_from(">I", data, pos); pos += 4
    player_ver, pos = read_cstr(data, pos)
    engine_ver, pos = read_cstr(data, pos)
    min_streamed, header_size, _levels_before, level_count = struct.unpack_from(">IIII", data, pos)
    print(f"format={fmt} player={player_ver} engine={engine_ver} header={header_size} levels={level_count}")

    # Unity writes a classic .lzma header (5 props bytes + 8 size bytes). liblzma's .lzma reader
    # rejects the tail of these streams, so decode the raw LZMA1 data and stop at the declared size.
    stream = data[header_size:]
    props = stream[0]
    (dict_size,) = struct.unpack_from("<I", stream, 1)
    (out_size,) = struct.unpack_from("<Q", stream, 5)
    filt = {"id": lzma.FILTER_LZMA1, "dict_size": dict_size,
            "lc": props % 9, "lp": (props // 9) % 5, "pb": props // 45}
    dec = lzma.LZMADecompressor(format=lzma.FORMAT_RAW, filters=[filt])
    raw = dec.decompress(stream[13:], max_length=out_size)
    print(f"decompressed {len(raw)} of {out_size} bytes")

    (count,) = struct.unpack_from(">I", raw, 0)
    pos = 4
    os.makedirs(out_dir, exist_ok=True)
    for _ in range(count):
        name, pos = read_cstr(raw, pos)
        offset, size = struct.unpack_from(">II", raw, pos); pos += 8
        safe = os.path.basename(name.replace("\\", "/")) or "unnamed"
        with open(os.path.join(out_dir, safe), "wb") as f:
            f.write(raw[offset:offset + size])
        print(f"  {size:>10}  {name}")


if __name__ == "__main__":
    main(sys.argv[1], sys.argv[2])
