"""Копатель Онлайн 2014: private server.

    python server.py                       # everything on 0.0.0.0, page on :8080
    python server.py --public-host 1.2.3.4 # behind NAT: the address clients reach the game server at

Starts the HTTP site (page, client, PHP scripts), the Unity socket policy server (TCP 843) and the
Photon master (TCP 5055) and game server (TCP 5056). Data goes to ./data.
"""
import argparse
import asyncio
import logging
import os
import sys
import threading

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

from kopatel.db import Database  # noqa: E402
from kopatel.photon.server import PhotonServer  # noqa: E402
from kopatel.web import serve_http  # noqa: E402

HERE = os.path.dirname(os.path.abspath(__file__))

POLICY = (b'<?xml version="1.0"?><cross-domain-policy>'
          b'<allow-access-from domain="*" to-ports="*" /></cross-domain-policy>\0')


async def serve_policy(host, port):
    """Unity Web Player asks port 843 before any socket connection. It sends
    <policy-file-request/> with no terminating zero, so answer as soon as that much has arrived."""
    log = logging.getLogger("policy")

    async def on_client(reader, writer):
        try:
            await asyncio.wait_for(reader.readuntil(b"<policy-file-request/>"), 10)
            writer.write(POLICY)
            await writer.drain()
            log.info("policy sent to %s", writer.get_extra_info("peername")[0])
        except Exception as e:
            log.warning("policy request from %s failed: %r", writer.get_extra_info("peername"), e)
        finally:
            writer.close()

    server = await asyncio.start_server(on_client, host, port)
    log.info("socket policy on %s:%d", host, port)
    async with server:
        await server.serve_forever()


def main():
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("--host", default="0.0.0.0")
    ap.add_argument("--http-port", type=int, default=8080)
    ap.add_argument("--policy-port", type=int, default=843)
    ap.add_argument("--master-port", type=int, default=5055, help="the client always connects to 5055")
    ap.add_argument("--game-port", type=int, default=5056)
    ap.add_argument("--public-host", default=None,
                    help="address given to clients for the game server (default: the address they connected to)")
    ap.add_argument("--data", default=os.path.join(HERE, "data"))
    ap.add_argument("--verbose", "-v", action="store_true")
    args = ap.parse_args()

    logging.basicConfig(level=logging.DEBUG if args.verbose else logging.INFO,
                        format="%(asctime)s %(name)-6s %(message)s", datefmt="%H:%M:%S")
    os.makedirs(os.path.join(args.data, "worlds"), exist_ok=True)
    www = os.path.join(HERE, "www")
    if not os.path.exists(os.path.join(www, "Digger2014.unity3d")):
        logging.error("www/Digger2014.unity3d is missing: build it with tools/build_client.ps1")
        sys.exit(1)

    db = Database(os.path.join(args.data, "db.json"))
    threading.Thread(target=serve_http, args=(args.host, args.http_port, db, www, args.data), daemon=True).start()

    photon = PhotonServer(args.host, args.master_port, args.game_port, args.public_host)

    async def run():
        await asyncio.gather(serve_policy(args.host, args.policy_port), photon.serve())

    try:
        asyncio.run(run())
    except KeyboardInterrupt:
        pass


if __name__ == "__main__":
    main()
