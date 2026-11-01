"""Rebuild www/Digger2012.unity3d and data/keys.json from the fan site's 2012 client.

  python -I tools/build_client.py [path/to/original.unity3d]

Needs the .NET SDK (for the Mono.Cecil patcher). Without an argument the original bundle is
downloaded from kopatel-online.ru. The bundle and keys.json always have to come from the same build.
"""
import os
import shutil
import subprocess
import sys
import tempfile
import urllib.request

TOOLS = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, TOOLS)
import unityweb  # noqa: E402

SERVER = os.path.dirname(TOOLS)
SOURCE_URL = "https://kopatel-online.ru/2012/Digger2012_kopatel_v36.unity3d?build=2012-v36"


def main():
    work = tempfile.mkdtemp(prefix="kopbuild")
    try:
        if len(sys.argv) > 1:
            original = sys.argv[1]
        else:
            original = os.path.join(work, "original.unity3d")
            print("downloading", SOURCE_URL)
            urllib.request.urlretrieve(SOURCE_URL, original)
        info, files = unityweb.read_bundle(original)
        if info["engine"] != "3.5.7f6":
            sys.exit(f"unexpected Unity version {info['engine']}; this patcher targets the 2012 (3.5.7f6) client")
        orig_dir, patched_dir = os.path.join(work, "orig"), os.path.join(work, "patched")
        os.makedirs(orig_dir)
        for name, content in files:
            with open(os.path.join(orig_dir, os.path.basename(name)), "wb") as f:
                f.write(content)

        patcher = os.path.join(SERVER, "patcher")
        out = os.path.join(work, "patcher-bin")
        subprocess.run(["dotnet", "build", patcher, "-c", "Release", "-o", out, "--nologo", "-v", "q"], check=True)
        subprocess.run(["dotnet", os.path.join(out, "KopPatcher.dll"), orig_dir, patched_dir,
                        os.path.join(SERVER, "data", "keys.json")], check=True)

        replaced = {name: open(os.path.join(patched_dir, os.path.basename(name)), "rb").read()
                    for name, _ in files if os.path.exists(os.path.join(patched_dir, os.path.basename(name)))}
        files = [(name, replaced.get(name, content)) for name, content in files]
        target = os.path.join(SERVER, "www", "Digger2012.unity3d")
        size = unityweb.write_bundle(target, info, files)
        print(f"wrote {target} ({size} bytes); replaced {sorted(replaced)}")
    finally:
        shutil.rmtree(work, ignore_errors=True)


if __name__ == "__main__":
    main()
