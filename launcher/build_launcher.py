"""Build KopatelOnline2012.exe: one file with a portable Firefox 51, Unity Web Player and a small
launcher that asks for the server address and opens its game page.

  python -I kop/launcher/build_launcher.py [--server 1.2.3.4:8080] [--firefox DIR] [--webplayer DIR]

--server is the address the launcher suggests on first start (players can change it, the last one is
remembered). Needs only Windows: the launcher is compiled with the csc.exe of .NET Framework 4.
"""
import argparse
import hashlib
import os
import random
import struct
import subprocess
import sys
import tempfile
import winreg
import zipfile

HERE = os.path.dirname(os.path.abspath(__file__))
CSC = os.path.join(os.environ.get("WINDIR", r"C:\Windows"), r"Microsoft.NET\Framework\v4.0.30319\csc.exe")
# Files of the installed Firefox the portable copy does not need (installer, updater, logs).
FIREFOX_SKIP = {"uninstall", "maintenanceservice.exe", "maintenanceservice_installer.exe",
                "updater.exe", "updater.ini", "install.log"}
# Added to the portable Firefox: the AutoConfig that gives the game page an Esc-proof full screen.
FIREFOX_EXTRA = {"firefox/mozilla.cfg": "mozilla.cfg", "firefox/defaults/pref/autoconfig.js": "autoconfig.js"}


def default_webplayer():
    try:
        with winreg.OpenKey(winreg.HKEY_CURRENT_USER, r"Software\Unity\WebPlayer") as key:
            return winreg.QueryValueEx(key, "Directory")[0]
    except OSError:
        return os.path.join(os.environ["USERPROFILE"], r"AppData\LocalLow\Unity\WebPlayer")


def check_inputs(firefox, webplayer):
    ini = os.path.join(firefox, "application.ini")
    if not os.path.exists(ini):
        sys.exit(f"no Firefox at {firefox} (pass --firefox)")
    version = next((line.split("=", 1)[1].strip() for line in open(ini, encoding="utf-8")
                    if line.startswith("Version=")), "?")
    if int(version.split(".")[0]) > 51:
        sys.exit(f"Firefox {version} at {firefox} has no NPAPI plugins; Firefox 51 (32-bit) is needed")
    if not os.path.exists(os.path.join(webplayer, "loader", "npUnity3D32.dll")):
        sys.exit(f"no Unity Web Player at {webplayer} (pass --webplayer)")
    print(f"Firefox {version}: {firefox}\nUnity Web Player: {webplayer}")


def write_payload(path, firefox, webplayer):
    digest = hashlib.sha256()
    with zipfile.ZipFile(path, "w", zipfile.ZIP_DEFLATED, compresslevel=9) as z:
        for prefix, root, skip in (("firefox", firefox, FIREFOX_SKIP), ("webplayer", webplayer, set())):
            for dirpath, dirnames, filenames in os.walk(root):
                dirnames[:] = sorted(d for d in dirnames if d not in skip)
                for name in sorted(filenames):
                    if name in skip:
                        continue
                    full = os.path.join(dirpath, name)
                    arc = prefix + "/" + os.path.relpath(full, root).replace(os.sep, "/")
                    data = open(full, "rb").read()
                    digest.update(arc.encode() + b"\0" + hashlib.sha256(data).digest())
                    z.writestr(arc, data)
        for arc, name in FIREFOX_EXTRA.items():
            data = open(os.path.join(HERE, name), "rb").read()
            digest.update(arc.encode() + b"\0" + hashlib.sha256(data).digest())
            z.writestr(arc, data)
    return digest.hexdigest()[:16]


def block_icon(path):
    """A grass-and-dirt block, drawn at 16, 32 and 48 px."""
    images = []
    for size in (16, 32, 48):
        rnd = random.Random(size)
        rows = []
        for y in range(size):
            row = b""
            for x in range(size):
                edge = x in (0, size - 1) or y in (0, size - 1)
                grass = y < size * 0.3 + (2 if (x * 7 // size) % 2 else 0) * size / 32
                if edge:
                    r, g, b = 40, 26, 14
                elif grass:
                    r, g, b = (88, 170, 52) if rnd.random() > 0.2 else (64, 136, 38)
                else:
                    r, g, b = (134, 92, 56) if rnd.random() > 0.25 else (98, 66, 40)
                row += bytes((b, g, r, 255))
            rows.append(row)
        mask_row = b"\0" * (((size + 31) // 32) * 4)
        bmp = (struct.pack("<IiiHHIIiiII", 40, size, size * 2, 1, 32, 0, 0, 0, 0, 0, 0)
               + b"".join(reversed(rows)) + mask_row * size)
        images.append((size, bmp))
    offset = 6 + 16 * len(images)
    head, body = struct.pack("<HHH", 0, 1, len(images)), b""
    for size, bmp in images:
        head += struct.pack("<BBBBHHII", size, size, 0, 0, 1, 32, len(bmp), offset + len(body))
        body += bmp
    open(path, "wb").write(head + body)


def main():
    ap = argparse.ArgumentParser(description=__doc__.split("\n")[0])
    ap.add_argument("--server", default="127.0.0.1:8080", help="address suggested on first start")
    ap.add_argument("--firefox", default=r"C:\Program Files (x86)\MozillaFirefoxOld")
    ap.add_argument("--webplayer", default=default_webplayer())
    ap.add_argument("-o", "--output", default=os.path.join(HERE, "dist", "KopatelOnline2012.exe"))
    args = ap.parse_args()
    check_inputs(args.firefox, args.webplayer)

    with tempfile.TemporaryDirectory(prefix="koplauncher") as work:
        payload = os.path.join(work, "payload.zip")
        payload_id = write_payload(payload, args.firefox, args.webplayer)
        for name, text in (("payload_id.txt", payload_id), ("default_server.txt", args.server)):
            open(os.path.join(work, name), "w", encoding="utf-8").write(text)
        icon = os.path.join(work, "kop.ico")
        block_icon(icon)

        os.makedirs(os.path.dirname(os.path.abspath(args.output)), exist_ok=True)
        resources = [payload, os.path.join(work, "payload_id.txt"), os.path.join(work, "default_server.txt"),
                     os.path.join(HERE, "user.js"), os.path.join(HERE, "userChrome.css")]
        cmd = [CSC, "/nologo", "/target:winexe", "/optimize+", "/codepage:65001", f"/out:{args.output}",
               f"/win32icon:{icon}", "/r:System.IO.Compression.dll", "/r:System.IO.Compression.FileSystem.dll", "/r:System.Windows.Forms.dll",
               "/r:System.Drawing.dll", "/r:System.Core.dll"]
        cmd += [f"/resource:{r},{os.path.basename(r)}" for r in resources]
        cmd.append(os.path.join(HERE, "Launcher.cs"))
        subprocess.run(cmd, check=True)
    size = os.path.getsize(args.output)
    print(f"wrote {args.output} ({size / 1048576:.1f} MB), payload {payload_id}, default server {args.server}")


if __name__ == "__main__":
    main()
