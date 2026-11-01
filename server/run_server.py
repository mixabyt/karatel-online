"""Local server for the 2012 "Копатель Онлайн" web client.

  python run_server.py [--http-port 8080] [--public-host 1.2.3.4] [--bind 0.0.0.0]

Then open http://<this computer>:8080/ in Firefox 51 (32-bit) with Unity Web Player.
Ports: HTTP (game page + API), 4530/4531 (Photon master/game), 843 (Unity socket policy).
"""
import argparse
import asyncio
import logging
import os
import socket
import threading

from kopserver.db import Database
from kopserver.photon import PhotonServer
from kopserver.web import make_server

HERE = os.path.dirname(os.path.abspath(__file__))


def lan_addresses():
    try:
        return sorted({a[4][0] for a in socket.getaddrinfo(socket.gethostname(), None, socket.AF_INET)})
    except OSError:
        return []


def main():
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("--bind", default="0.0.0.0", help="address to listen on (default: all)")
    ap.add_argument("--http-port", type=int, default=8080)
    ap.add_argument("--public-host", help="address players use to reach this server from the internet; "
                                          "only needed behind port forwarding")
    ap.add_argument("--data", default=os.path.join(HERE, "data"), help="folder for accounts and maps")
    ap.add_argument("--start-crystals", type=int, default=500)
    ap.add_argument("--start-coins", type=int, default=50000)
    ap.add_argument("-v", "--verbose", action="store_true")
    args = ap.parse_args()

    logging.basicConfig(level=logging.DEBUG if args.verbose else logging.INFO,
                        format="%(asctime)s %(name)-6s %(message)s", datefmt="%H:%M:%S")
    keys = os.path.join(args.data, "keys.json")
    if not os.path.exists(keys):
        raise SystemExit(f"{keys} is missing: it must come from the same build as www/Digger2012.unity3d")

    db = Database(args.data, args.start_crystals, args.start_coins)
    httpd = make_server(args.bind, args.http_port, db, keys, os.path.join(HERE, "www"))
    threading.Thread(target=httpd.serve_forever, daemon=True).start()

    print("\nКопатель Онлайн 2012 — локальний сервер запущено.")
    print(f"  На цьому комп'ютері:  http://127.0.0.1:{args.http_port}/")
    for ip in lan_addresses():
        if not ip.startswith("127."):
            print(f"  Для інших у мережі:  http://{ip}:{args.http_port}/")
    print("  Зупинити: Ctrl+C\n")

    photon = PhotonServer(public_host=args.public_host)
    try:
        asyncio.run(photon.serve(args.bind))
    except KeyboardInterrupt:
        pass
    finally:
        httpd.shutdown()


if __name__ == "__main__":
    main()
