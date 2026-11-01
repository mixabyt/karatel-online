"""List AudioClips inside unpacked Unity web-player files (needs UnityPy).

  python -I list_audio.py <unpacked bundle dir> [export dir]

With an export dir, every clip's encoded audio is written there as <name>.<ext>.
"""
import os
import sys

import UnityPy


def main(src, out=None):
    env = UnityPy.load(src)
    if out:
        os.makedirs(out, exist_ok=True)
    seen = {}
    for obj in env.objects:
        if obj.type.name != "AudioClip":
            continue
        clip = obj.read()
        name = clip.m_Name
        data = bytes(getattr(clip, "m_AudioData", None) or b"")
        kind = getattr(clip, "m_Type", None) or getattr(clip, "m_CompressionFormat", None)
        length = getattr(clip, "m_Length", None)
        print(f"{name:32s} type={kind} bytes={len(data)} length={length} file={os.path.basename(obj.assets_file.name)}")
        if not out:
            continue
        seen[name] = seen.get(name, 0) + 1
        fname = name if seen[name] == 1 else f"{name}~{seen[name]}"
        if data:  # Unity 3/4: encoded audio stored inline
            ext = "ogg" if data[:4] == b"OggS" else "wav" if data[:4] == b"RIFF" else "bin"
            with open(os.path.join(out, f"{fname}.{ext}"), "wb") as f:
                f.write(data)
        else:  # Unity 5+: FSB5 in a .resource file, decoded to WAV by UnityPy (FMOD)
            for i, wav in enumerate(clip.samples.values()):
                suffix = "" if i == 0 else f"#{i}"
                with open(os.path.join(out, f"{fname}{suffix}.wav"), "wb") as f:
                    f.write(wav)


if __name__ == "__main__":
    main(*sys.argv[1:])
