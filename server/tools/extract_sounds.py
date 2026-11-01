"""Fill www/sounds/ with the 2012 client's sound effects, taken from the newer game build.

  python -I tools/extract_sounds.py <diggerworld.zip>

The 2012 client used to load them from Sounds3.unity3d, which no longer exists anywhere. The fan
launcher (diggerworld.zip on the kopatel-online.ru Yandex Disk) is a later Unity build of the same
game whose clips carry the same names. Needs UnityPy (pip install UnityPy); clips are written as
16-bit WAV, which the web player can load with WWW.GetAudioClip. Clips missing from the build are
written as short silence so the client never hits a 404.
"""
import os
import shutil
import struct
import sys
import tempfile
import zipfile

import UnityPy

# ResourceLoader.eSounds of the 2012 client: name_count
ESOUNDS = (
    "BangDirt_1 BangGlass_1 BangGrass_1 BangMetal_1 BangSand_1 BangStone_1 BangWood_1 FootDirt_4 FootGlass_4 "
    "FootGrass_4 FootMetal_4 FootSand_4 FootStone_4 FootWood_4 HitDirt_3 HitGlass_3 HitGrass_3 HitMetal_3 "
    "HitSand_3 HitStone_3 HitWood_3 WaterIn_1 WaterOut_1 LoopForest_1 WeaponSwish_2 BangWater_1 FootSnow_4 "
    "HitSnow_3 BangSnow_1 FootIce_4 HitIce_2 BangIce_1 LoopSnow_1 Jump_1 LoopOcean_1 FootLava_1 SetDecor_1 "
    "Note2_1 LoopDesert_1 DoorOpen_1 DoorClose_1 TorchLoop_1 LoopUnderwater_1 Treasure_1"
).split()


def wanted_files():
    for entry in ESOUNDS:
        base, count = entry.rsplit("_", 1)
        count = int(count)
        for i in range(1, count + 1):
            yield base if count == 1 else f"{base}_{i}"


def silence(seconds=0.5, rate=22050):
    data = b"\0\0" * int(rate * seconds)
    return (b"RIFF" + struct.pack("<I", 36 + len(data)) + b"WAVEfmt " + struct.pack("<IHHIIHH", 16, 1, 1, rate, rate * 2, 2, 16)
            + b"data" + struct.pack("<I", len(data)) + data)


def main(zip_path):
    out = os.path.join(os.path.dirname(os.path.dirname(os.path.abspath(__file__))), "www", "sounds")
    os.makedirs(out, exist_ok=True)
    names = set(wanted_files())
    work = tempfile.mkdtemp(prefix="kopsnd")
    try:
        with zipfile.ZipFile(zip_path) as z:
            for info in z.infolist():
                parts = info.filename.split("/")
                # only the top-level data files of <x>_Data/, never executables
                if len(parts) == 3 and parts[1].endswith("_Data") and parts[2] and \
                        not parts[2].endswith((".dll", ".exe", ".json", ".config")):
                    with z.open(info) as src, open(os.path.join(work, parts[2]), "wb") as dst:
                        shutil.copyfileobj(src, dst)
        found = {}
        for obj in UnityPy.load(work).objects:
            if obj.type.name != "AudioClip":
                continue
            clip = obj.read()
            if clip.m_Name in names and clip.m_Name not in found:
                samples = list(clip.samples.values())
                if samples:
                    found[clip.m_Name] = samples[0]
    finally:
        shutil.rmtree(work, ignore_errors=True)

    for name in sorted(names):
        with open(os.path.join(out, name + ".wav"), "wb") as f:
            f.write(found.get(name) or silence())
    missing = sorted(names - set(found))
    print(f"wrote {len(names)} sounds to {out}; replaced by silence: {', '.join(missing) or 'none'}")


if __name__ == "__main__":
    if len(sys.argv) != 2:
        sys.exit(__doc__)
    main(sys.argv[1])
