"""Read and rebuild UnityWeb (.unity3d) bundles of the Unity 3.x web player.

  python -I unityweb.py unpack <bundle.unity3d> <out_dir>
  python -I unityweb.py repack <original.unity3d> <replacement_dir> <out.unity3d>

`repack` keeps the original file order and swaps in every file that exists in <replacement_dir>.
"""
import lzma
import os
import struct
import sys

DICT_SIZE = 1 << 19  # same as the original bundles; the 32-bit plugin allocates this much


def _cstr(buf, pos):
    end = buf.index(b"\0", pos)
    return buf[pos:end].decode("ascii"), end + 1


def read_bundle(path):
    """Returns (header_info, [(name, bytes), ...])."""
    data = open(path, "rb").read()
    sig, pos = _cstr(data, 0)
    if sig != "UnityWeb":
        raise ValueError(f"not a UnityWeb bundle: {sig!r}")
    (fmt,) = struct.unpack_from(">I", data, pos); pos += 4
    player_ver, pos = _cstr(data, pos)
    engine_ver, pos = _cstr(data, pos)
    _min_streamed, header_size = struct.unpack_from(">II", data, pos)

    stream = data[header_size:]
    props = stream[0]
    (dict_size,) = struct.unpack_from("<I", stream, 1)
    (out_size,) = struct.unpack_from("<Q", stream, 5)
    # liblzma's .lzma reader rejects the tail of these streams, so decode raw LZMA1 up to the declared size.
    filt = {"id": lzma.FILTER_LZMA1, "dict_size": dict_size,
            "lc": props % 9, "lp": (props // 9) % 5, "pb": props // 45}
    raw = lzma.LZMADecompressor(format=lzma.FORMAT_RAW, filters=[filt]).decompress(stream[13:], max_length=out_size)
    if len(raw) != out_size:
        raise ValueError(f"short bundle: {len(raw)} of {out_size} bytes")

    (count,) = struct.unpack_from(">I", raw, 0)
    pos, files = 4, []
    for _ in range(count):
        name, pos = _cstr(raw, pos)
        offset, size = struct.unpack_from(">II", raw, pos); pos += 8
        files.append((name, raw[offset:offset + size]))
    return {"format": fmt, "player": player_ver, "engine": engine_ver}, files


def write_bundle(path, info, files):
    directory = struct.pack(">I", len(files))
    for name, _ in files:
        directory += name.encode("ascii") + b"\0" + b"\0" * 8
    info_size = (len(directory) + 3) & ~3  # the originals pad the file table to 4 bytes

    body = bytearray(struct.pack(">I", len(files)))
    offset = info_size
    for name, content in files:
        body += name.encode("ascii") + b"\0" + struct.pack(">II", offset, len(content))
        offset += len(content)
    body += b"\0" * (info_size - len(body))
    for _, content in files:
        body += content

    filt = {"id": lzma.FILTER_LZMA1, "preset": 9, "dict_size": DICT_SIZE, "lc": 3, "lp": 0, "pb": 2}
    packed = lzma.compress(bytes(body), format=lzma.FORMAT_RAW, filters=[filt])
    stream = bytes([3 + 0 * 9 + 2 * 45]) + struct.pack("<IQ", DICT_SIZE, len(body)) + packed

    head = (b"UnityWeb\0" + struct.pack(">I", info["format"])
            + info["player"].encode("ascii") + b"\0" + info["engine"].encode("ascii") + b"\0")
    header_size = (len(head) + 4 * 4 + 2 * 8 + 4 + 4 + 3) & ~3
    total = header_size + len(stream)
    head += struct.pack(">IIII", total, header_size, 2, 2)
    # Both "levels" end at the end of the data: the player simply waits for the whole (local) download.
    head += struct.pack(">II", len(stream), len(body)) * 2
    head += struct.pack(">II", total, info_size)
    head += b"\0" * (header_size - len(head))
    with open(path, "wb") as f:
        f.write(head)
        f.write(stream)
    return total


def main(argv):
    if len(argv) == 4 and argv[1] == "unpack":
        info, files = read_bundle(argv[2])
        os.makedirs(argv[3], exist_ok=True)
        print(info)
        for name, content in files:
            with open(os.path.join(argv[3], os.path.basename(name)), "wb") as f:
                f.write(content)
            print(f"  {len(content):>10}  {name}")
    elif len(argv) == 5 and argv[1] == "repack":
        info, files = read_bundle(argv[2])
        replaced = []
        for i, (name, _) in enumerate(files):
            candidate = os.path.join(argv[3], os.path.basename(name))
            if os.path.isfile(candidate):
                files[i] = (name, open(candidate, "rb").read())
                replaced.append(name)
        total = write_bundle(argv[4], info, files)
        print(f"replaced {replaced}; wrote {total} bytes to {argv[4]}")
    else:
        sys.exit(__doc__)


if __name__ == "__main__":
    main(sys.argv)
