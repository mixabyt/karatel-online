"""HTTP side: the game page, the client bundle and the PHP API the 2012 client calls (/api/*.php)."""
import base64
import hashlib
import html
import json
import logging
import math
import os
import re
import struct
import time
import zlib
from email.utils import formatdate
from http.server import BaseHTTPRequestHandler, ThreadingHTTPServer
from urllib.parse import parse_qs, urlsplit

from .db import FOREVER, SLOTS, md5, now_str

log = logging.getLogger("web")

PAGE_SIZE = 10
COMMENTS_PER_PAGE = 5
CRYSTAL_PACKS = {1: 6, 2: 15, 3: 30, 4: 60, 5: 120}   # MainMenu.ind1_count..ind5_count
TIME_DELTAS = {1: 1, 2: 7, 4: 30}                     # date_delta -> days, as in ByFromStoreCoins
NO_EFFECT_PURCHASES = {5, 33}                         # NONE (new map), CHANGENICK
CROSSDOMAIN = (b'<?xml version="1.0"?><cross-domain-policy><allow-access-from domain="*" />'
               b'<allow-http-request-headers-from domain="*" headers="*" /></cross-domain-policy>')
CONTENT_TYPES = {".html": "text/html; charset=utf-8", ".unity3d": "application/vnd.unity", ".js": "text/javascript",
                 ".css": "text/css", ".png": "image/png", ".jpg": "image/jpeg", ".ico": "image/x-icon"}


def _png(width, height, rgb):
    raw = b"".join(b"\0" + bytes(rgb) * width for _ in range(height))

    def chunk(kind, data):
        return struct.pack(">I", len(data)) + kind + data + struct.pack(">I", zlib.crc32(kind + data))
    return (b"\x89PNG\r\n\x1a\n" + chunk(b"IHDR", struct.pack(">IIBBBBB", width, height, 8, 2, 0, 0, 0))
            + chunk(b"IDAT", zlib.compress(raw)) + chunk(b"IEND", b""))


NEWS_PNG = _png(90, 90, (255, 179, 92))


def clean(text, limit=60):
    """Strip the separators the client's text formats use (| , ~ = and newlines)."""
    return re.sub(r"[|,~=\r\n]", " ", text or "").strip()[:limit]


def rsa_oaep_decrypt(b64, n, d):
    """RSA-OAEP(SHA-1) decryption, matching RSACryptoServiceProvider.Encrypt(data, fOAEP: true)."""
    c = int.from_bytes(base64.b64decode(b64.replace(" ", "+")), "big")
    k = (n.bit_length() + 7) // 8
    em = pow(c, d, n).to_bytes(k, "big")

    def mgf1(seed, length):
        out = b""
        for i in range(math.ceil(length / 20)):
            out += hashlib.sha1(seed + i.to_bytes(4, "big")).digest()
        return out[:length]

    masked_seed, masked_db = em[1:21], em[21:]
    seed = bytes(a ^ b for a, b in zip(masked_seed, mgf1(masked_db, 20)))
    db = bytes(a ^ b for a, b in zip(masked_db, mgf1(seed, k - 21)))
    if em[0] != 0 or db[:20] != hashlib.sha1(b"").digest():
        raise ValueError("bad OAEP padding")
    return db[db.index(b"\x01", 20) + 1:].decode("utf-8")


def parse_multipart(body, content_type):
    m = re.search(r'boundary="?([^";]+)"?', content_type)
    fields, files = {}, {}
    if not m:
        return fields, files
    for part in body.split(b"--" + m.group(1).encode()):
        head, sep, data = part.partition(b"\r\n\r\n")
        if not sep:
            continue
        name = re.search(rb'name="([^"]*)"', head)
        if not name:
            continue
        data = data[:-2] if data.endswith(b"\r\n") else data
        if b"filename=" in head:
            files[name.group(1).decode()] = data
        else:
            fields[name.group(1).decode()] = data.decode("utf-8", "replace")
    return fields, files


class Api:
    """One method per PHP script; each gets (form, files) and returns the response text."""

    def __init__(self, db, keys, www):
        self.db, self.secret, self.www = db, keys["secret"], www
        self.rsa_n, self.rsa_d = int(keys["rsa_n"], 16), int(keys["rsa_d"], 16)

    def signed(self, text, rnd, key):
        return f"{text}\n{md5(f'{text}_{rnd}_{key}')}"

    def me(self, form, id_field="id"):
        return self.db.player(form.get(id_field, ""), form.get("auth_key", ""))

    # ---- account ----
    def enter_game(self, f, _):
        pid = self.db.login(f.get("email", "").strip().lower(), f.get("password", ""))
        if pid is None:
            return "LOGIN_FAILED"
        return self.signed(f"{pid}|{self.db.player(pid)['auth']}", f.get("random", ""), self.secret)

    def register(self, f, _):
        email = f.get("email", "").strip().lower()
        pid = self.db.register(email, f.get("password", "")) if email and f.get("password") else None
        if pid:
            log.info("registered %s as player %s", email, pid)
        return self.signed("REGISTERED" if pid else "EMAIL_FAILED", f.get("random", ""), self.secret)

    def password_recover(self, f, _):
        email = f.get("email", "").strip().lower()
        new = self.db.reset_password(email)
        if new is None:
            return "EMAIL_NOT_FOUND"
        log.warning("password for %s reset to: %s   (tell the player; there is no mail)", email, new)
        return "SEND"

    def get_profile(self, f, _):
        p = self.me(f)
        if p is None:
            return "Unknown Profile\n-"
        inv = "".join(f",inv{k}={n}|{until or FOREVER}" for k, (n, until) in sorted(p["inv"].items()))
        text = (f"Saved Profile: rnd={f.get('random', '0')},soc=ws,player={f.get('id')},name={p['name']},"
                f"crystal={p['crystals']},coins={p['coins']},destroy={p['destroy']},st={now_str()},"
                f"skin={p['skin']}{inv}")
        return text + "\n-"

    def set_name(self, f, _):
        p, name = self.me(f), clean(f.get("name"), 20)
        if p is None or not name or name == "Копатель" or self.db.name_taken(name, f.get("id")):
            return "NAME_BUSY"
        with self.db.lock:
            p["name"] = name
            for m in self.db.public_maps():
                if m["owner"] == f.get("id"):
                    m["author"] = name
            self.db.save()
        return "SET_NAME"

    def set_destroy(self, f, _):
        return self._set(f, "destroy", "ind")

    def set_skin(self, f, _):
        return self._set(f, "skin", "skin")

    def _set(self, f, key, field):
        p = self.me(f)
        if p is not None and f.get(field, "").lstrip("-").isdigit():
            with self.db.lock:
                p[key] = int(f[field])
                self.db.save()
        return "OK"

    def get_favorit_servers(self, f, _):
        return ""

    # ---- slots and map files ----
    def get_slot_info(self, f, _):
        p = self.db.player(f.get("id", ""))
        slot = int(f.get("map_place", "0") or 0)
        if p is None or not 1 <= slot <= self.db.slot_count(p):
            return "NOT AVAILABLE SLOT"
        if not os.path.exists(self.db.map_path(f["id"], slot)):
            return "EMPTY SLOT"
        public = self.db.slot(p, slot)["public"]
        return str(public) if public > 1 else "1"

    def save_map_to_slot(self, f, files):
        if "buffer" not in files:  # GET from the in-game "buy crystals" button
            return self.pay_page(f)
        p = self.me(f)
        slot = int(f.get("map_place", "0") or 0)
        if p is None or not 1 <= slot <= self.db.slot_count(p):
            return "Copy unsuccessfull!"
        path = self.db.map_path(f["id"], slot)
        os.makedirs(os.path.dirname(path), exist_ok=True)
        with open(path + ".tmp", "wb") as out:
            out.write(files["buffer"])
        os.replace(path + ".tmp", path)
        log.info("player %s saved slot %s (%d bytes)", f["id"], slot, len(files["buffer"]))
        return "OK"

    def pay_page(self, f):
        p, amount = self.db.player(f.get("v", "")), int(f.get("out", "0") or 0)
        if p is not None and 0 < amount <= 1000:
            with self.db.lock:
                p["crystals"] += amount
                self.db.save()
            msg = f"Додано {amount} кристалів. Поверніться в гру й перезайдіть у меню, щоб побачити баланс."
        else:
            msg = "На локальному сервері кристали безкоштовні: оберіть кількість у грі."
        return ("<!doctype html><meta charset=utf-8><title>Кристали</title>"
                f"<body style='font:18px sans-serif;padding:40px'>{html.escape(msg)}</body>", "text/html; charset=utf-8")

    def is_peregenerate_map(self, f, _):
        path = self.db.map_path(f.get("id", ""), f.get("map_place", ""))
        if os.path.exists(path):
            backup = os.path.join(self.db.backups, f"{f['id']}_{f['map_place']}_{int(time.time())}.map")
            os.replace(path, backup)
            log.info("slot %s of player %s regenerated; old map kept in %s", f["map_place"], f["id"], backup)
        return "OK"

    def new_map_timer(self, f, _):
        return "OK"

    def update_slot_info(self, f, _):
        return "OK"

    # ---- published maps ----
    def public_map(self, f, _):
        p = self.me(f)
        slot = int(f.get("slot_id", "0") or 0)
        if p is None or not os.path.exists(self.db.map_path(f["id"], slot)):
            return "FAIL"
        return str(self.db.publish(f["id"], slot, clean(f.get("name")), clean(f.get("about"), 300),
                                   clean(f.get("url"), 300)))

    def close_map(self, f, _):
        if self.me(f) is not None:
            self.db.unpublish(f["id"], int(f.get("slot_id", "0") or 0))
        return "OK"

    def _map(self, f):
        return self.db.data["maps"].get(f.get("map_id", ""))

    def get_map_info(self, f, _):
        m = self._map(f)
        if m is None:
            return "Карта удалена||||0|0|0|0|0"
        return f"{m['name']}|{m['about']}|{m['url']}|{m['author']}|0|0|{m['rating']}|{m['owner']}|{m['slot']}"

    def post_comment(self, f, _):
        m, text = self._map(f), clean(f.get("post_text"), 500)
        if m is not None and text and self.db.player(f.get("id", "")):
            with self.db.lock:
                m["comments"].append({"id": f["id"], "name": clean(f.get("post_name"), 20), "text": text})
                self.db.save()
        return "OK"

    def get_coments(self, f, _):
        m = self._map(f)
        comments = list(reversed(m["comments"])) if m else []
        pages = math.ceil(len(comments) / COMMENTS_PER_PAGE)
        start = int(f.get("start_ind", "0") or 0) * COMMENTS_PER_PAGE
        page = comments[start:start + COMMENTS_PER_PAGE]
        return f"{pages}|" + "".join(f"|{c['id']}~{c['name']}~{c['text']}" for c in page)

    def is_voute_by_map(self, f, _):
        m = self._map(f)
        return (m or {}).get("votes", {}).get(f.get("id", ""), "NOT_VOTE")

    def _vote(self, f, wanted_before, after, delta):
        m = self._map(f)
        if m is None or self.me(f) is None:
            return "OK"
        with self.db.lock:
            if m["votes"].get(f["id"]) == wanted_before:
                if after:
                    m["votes"][f["id"]] = after
                else:
                    m["votes"].pop(f["id"], None)
                m["rating"] += delta
                self.db.save()
        return "OK"

    def voute_yes(self, f, _):
        return self._vote(f, None, "YES", 1)

    def voute_yes_not(self, f, _):
        return self._vote(f, "YES", None, -1)

    def voute_no_not(self, f, _):
        return self._vote(f, "NO", None, 1)

    def _list(self, f, maps, key):
        maps = sorted(maps, key=key, reverse=True)
        pages = math.ceil(len(maps) / PAGE_SIZE)
        page = int(f.get("page", "0") or 0)
        chunk = maps[page * PAGE_SIZE:(page + 1) * PAGE_SIZE]
        body = "".join(f"|{m['id']},{m['owner']},{m['slot']},{clean(m['name'])},{m['rating']}" for m in chunk)
        return f"{pages - 1}|100|{body}"

    def _recent(self, f, days):
        since = time.time() - days * 86400
        return self._list(f, [m for m in self.db.public_maps() if m["published"] >= since], lambda m: m["rating"])

    def get_all_time_map(self, f, _):
        return self._list(f, self.db.public_maps(), lambda m: m["rating"])

    def get_monthly_map(self, f, _):
        return self._recent(f, 30)

    def get_weekly_map(self, f, _):
        return self._recent(f, 7)

    def get_day_map(self, f, _):
        return self._recent(f, 1)

    def get_latest_map(self, f, _):
        return self._list(f, self.db.public_maps(), lambda m: m["published"])

    def search_maps(self, f, _):
        text = f.get("text", "").strip().lower()
        field = "author" if f.get("type") == "1" else "name"
        found = [m for m in self.db.public_maps() if text in m[field].lower()]
        key = (lambda m: m["rating"]) if f.get("sortby") == "rating" else (lambda m: m["published"])
        return self._list(f, found, key)

    # ---- money and store (no real payments on a local server) ----
    def _signed_purchase(self, f, key, currency):
        p = self.me(f, "viewer_id")
        rnd = f.get("random", "")
        cost = int(f.get("beta_key", "0") or 0)
        if p is None or cost < 0 or p[currency] < cost:
            return self.signed("FAIL", rnd, key)
        item, count = f.get("p_ind", "0"), int(f.get("count", "0") or 0)
        with self.db.lock:
            p[currency] -= cost
            n, until = p["inv"].get(item, [0, ""])
            if int(item) == SLOTS:
                n += 1
            elif int(item) not in NO_EFFECT_PURCHASES:
                n += count
            days = TIME_DELTAS.get(int(f.get("date_delta", "0") or 0))
            if days:
                until = now_str(days)
            p["inv"][item] = [n, until]
            self.db.save()
        return self.signed("SUCCESS", rnd, key)

    def by_from_store_crystal(self, f, _):
        return self._signed_purchase(f, self.secret, "crystals")

    def by_from_store_coins(self, f, _):
        try:
            key = rsa_oaep_decrypt(f.get("yoba_key", ""), self.rsa_n, self.rsa_d)
        except ValueError:
            log.warning("coin purchase with an undecryptable key (client not patched with this server's keys?)")
            return "FAIL\n-"
        return self._signed_purchase(f, key, "coins")

    def use_purches(self, f, _):
        p = self.me(f, "viewer_id")
        item = f.get("p_ind", "")
        if p is not None and item in p["inv"]:
            with self.db.lock:
                p["inv"][item][0] = max(0, p["inv"][item][0] - 1)
                self.db.save()
        return "OK"

    def sell_crystalls(self, f, _):
        p, count = self.me(f, "viewer_id"), int(f.get("count", "0") or 0)
        if p is None or count <= 0 or p["crystals"] < count:
            return "FAIL"
        with self.db.lock:
            p["crystals"] -= count
            p["coins"] += count * 100
            self.db.save()
        return "SUCCSES"  # sic, the client expects this spelling

    def by_crystalls(self, f, _):
        p, amount = self.me(f, "viewer_id"), CRYSTAL_PACKS.get(int(f.get("ind", "0") or 0))
        if p is None or amount is None:
            return "FAIL"
        with self.db.lock:
            p["crystals"] += amount
            self.db.save()
        return "SUCCESS"

    def send_money_to_player(self, f, _):
        p, other = self.me(f, "viewer_id"), self.db.player(f.get("player_id", ""))
        count = int(f.get("count", "0") or 0)
        if p is None or other is None or other is p or count <= 0 or p["coins"] < count:
            return "FAIL"
        with self.db.lock:
            p["coins"] -= count
            other["coins"] += count
            self.db.save()
        return "SUCCESS"

    def add_gold(self, f, _):
        p = self.me(f, "viewer_id")
        if p is not None:
            with self.db.lock:
                p["coins"] += 5
                self.db.save()
        return "OK"


class Handler(BaseHTTPRequestHandler):
    server_version = "KopatelLocal/1.0"
    api = None  # set by make_server

    def log_message(self, fmt, *args):
        log.debug("%s %s", self.address_string(), fmt % args)

    def do_GET(self):
        self.route({}, {})

    def do_POST(self):
        length = int(self.headers.get("Content-Length") or 0)
        body = self.rfile.read(length) if length else b""
        ctype = self.headers.get("Content-Type", "")
        if ctype.startswith("multipart/form-data"):
            fields, files = parse_multipart(body, ctype)
        else:
            fields = {k: v[0] for k, v in parse_qs(body.decode("utf-8", "replace"), keep_blank_values=True).items()}
            files = {}
        self.route(fields, files)

    def route(self, fields, files):
        url = urlsplit(self.path)
        form = {k: v[0] for k, v in parse_qs(url.query, keep_blank_values=True).items()}
        form.update(fields)
        path = re.sub(r"/+", "/", url.path)
        api = self.api
        if path.startswith("/api/worlds/"):
            return self.send_map(path[len("/api/worlds/"):])
        m = re.fullmatch(r"/api/([a-z_]+)\.php", path)
        if m:
            method = getattr(api, m.group(1), None)
            if method is None or m.group(1).startswith("_"):
                log.warning("unknown API call %s %s", m.group(1), form)
                return self.reply("OK")
            with api.db.lock:
                result = method(form, files)
            log.info("%s %s -> %s", m.group(1), {k: v for k, v in form.items() if k not in ("password", "auth_key", "yoba_key")},
                     (result if isinstance(result, str) else "page").split("\n")[0][:80])
            return self.reply(*(result if isinstance(result, tuple) else (result,)))
        if path == "/crossdomain.xml":
            return self.reply(CROSSDOMAIN, "text/xml")
        if path == "/news":  # MainMenu.LoadNewsBlock: "text;image url;link"
            host = self.headers.get("Host", "127.0.0.1")
            return self.reply(f"Локальный сервер Копателя;http://{host}/news.png;http://{host}/")
        if path == "/news.png":
            return self.reply(NEWS_PNG, "image/png", cache=True)
        m = re.fullmatch(r"/sounds/([A-Za-z0-9_]+)\.wav", path)  # sound effects, see tools/extract_sounds.py
        if m:
            file = os.path.join(api.www, "sounds", m.group(1) + ".wav")
            if not os.path.isfile(file):
                return self.reply("Not found", status=404)
            return self.send_file(file, "audio/wav")
        name = "index.html" if path == "/" else path.lstrip("/")
        file = os.path.join(api.www, name)
        if "/" in name or "\\" in name or not os.path.isfile(file):
            return self.reply("Not found", status=404)
        self.send_file(file, CONTENT_TYPES.get(os.path.splitext(name)[1], "application/octet-stream"))

    def send_file(self, file, ctype):
        """The browser keeps a copy but asks again on every start ("no-cache" + ETag): an unchanged
        17 MB client costs a 304, a rebuilt one (or a new sound) is downloaded at once."""
        st = os.stat(file)
        etag = f'"{st.st_mtime_ns:x}-{st.st_size:x}"'
        headers = {"ETag": etag, "Last-Modified": formatdate(st.st_mtime, usegmt=True)}
        if etag in [t.strip() for t in self.headers.get("If-None-Match", "").split(",")]:
            return self.reply(b"", ctype, status=304, headers=headers)
        with open(file, "rb") as fh:
            self.reply(fh.read(), ctype, headers=headers)

    def send_map(self, rest):
        parts = rest.split("?")[0].split("/")
        file = parts[-1]
        if not re.fullmatch(r"\d+_\d+\.map", file) or len(parts) > 2:
            return self.reply("Not found", status=404)
        pid, slot = file[:-4].split("_")
        path = self.api.db.map_path(pid, slot)
        if not os.path.isfile(path):
            return self.reply("Not found", status=404)
        with open(path, "rb") as fh:
            self.reply(fh.read(), "application/octet-stream")

    def reply(self, body, ctype="text/plain; charset=utf-8", status=200, cache=False, headers=None):
        data = body.encode("utf-8") if isinstance(body, str) else body
        self.send_response(status)
        self.send_header("Content-Type", ctype)
        if status != 304:
            self.send_header("Content-Length", str(len(data)))
        if headers:  # files: revalidated on every use
            self.send_header("Cache-Control", "no-cache")
            for key, value in headers.items():
                self.send_header(key, value)
        else:
            self.send_header("Cache-Control", "max-age=3600" if cache else "no-store")
        self.end_headers()
        if status != 304:
            self.wfile.write(data)


def make_server(bind, port, db, keys_path, www):
    with open(keys_path, encoding="utf-8") as f:
        keys = json.load(f)
    Handler.api = Api(db, keys, www)
    return ThreadingHTTPServer((bind, port), Handler)
