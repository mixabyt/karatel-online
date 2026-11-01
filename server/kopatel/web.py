"""HTTP side: the game page, the .unity3d client, and the PHP scripts the 2014 client calls.

The client keeps its original paths (/Test2/*.php for the WEBSITE platform, /WB/ for map saves,
/content/, /news/), only with our host; scripts are matched by file name, so any prefix works.
Response formats are documented in docs/protocol.md.
"""
import hashlib
import http.server
import logging
import mimetypes
import os
import re
import time
import urllib.parse

from .db import now_str

log = logging.getLogger("web")

SECRET = "Ysm9hrlkzzQas5DXzvHmm"  # VkontakteAPI.GetSecret() in the client

# Store.cs: purchases paid per day/week/forever/month, and the ones bought only once.
TIMED_PURCHASES = {7, 8, 9, 10, 18, 19, 20, 34, 35, 36, 40, 42, 43, 59, 61, 126, 129, 161, 197}
ONCE_PURCHASES = {39, 44, 46, 52, 53, 54, 55, 56, 57, 58, 60, 64, 68, 70, 77, 78, 79, 94, 95, 96, 97, 98,
                  99, 100, 106, 107, 127, 128, 144, 145, 146, 147, 152, 153, 154, 155, 156, 157, 158,
                  159, 160, 162, 163, 164, 165, 166, 167, 191, 192, 193, 194, 195, 196}
CHANGENICK = 33
TIMED_DAYS = {1: 1, 2: 7, 4: 30}  # dateIndex -> days; 3 is "forever"

# Russian: the game's font has no Ukrainian letters. Fields: text;picture;url;button;button url.
NEWS = ("Копатель Онлайн 2014 снова работает! Это частный сервер: монеты и кристаллы бесплатные, "
        "карты хранятся на сервере.;;;;")

MAPS_PER_PAGE = 20
COMMENTS_PER_PAGE = 10


def md5(s):
    return hashlib.md5(s.encode("utf-8")).hexdigest()


def signed(text, *parts):
    """text + "\\n" + md5(text_part1_part2...) as the client checks it."""
    return text + "\n" + md5("_".join((text,) + tuple(str(p) for p in parts)))


def world_path(data_dir, pid, slot):
    return os.path.join(data_dir, "worlds", md5(str(pid))[:2], "%s_%s.map" % (pid, slot))


def parse_multipart(body, boundary):
    fields = {}
    for part in body.split(b"--" + boundary):
        if b"\r\n\r\n" not in part:
            continue
        head, _, value = part.partition(b"\r\n\r\n")
        m = re.search(rb'name="([^"]*)"', head)
        if m:
            if value.endswith(b"\r\n"):
                value = value[:-2]
            fields[m.group(1).decode("utf-8", "replace")] = value
    return fields


class Scripts:
    """One method per PHP script; each gets the form (str values, bytes for uploads) and returns text."""

    def __init__(self, db, data_dir):
        self.db, self.data_dir = db, data_dir

    def _authed(self, f, id_field="id"):
        pid = f.get(id_field) or f.get("viewer_id") or f.get("id") or ""
        return pid if self.db.check(pid, f.get("auth_key", "")) else None

    # ---------------------------------------------- accounts

    def enter_game(self, f):
        result = self.db.login(f.get("email", "").strip().lower(), f.get("password", ""))
        if not result:
            return "LOGIN_FAILED"
        return signed("%s\t%s" % result, f.get("random", ""), SECRET)

    def register(self, f):
        email = f.get("email", "").strip().lower()
        password = f.get("password", "")
        rnd = f.get("random", "")
        if len(email) < 3 or "\t" in email or not password:
            return signed("WRONG_EMAIL", rnd, SECRET)
        if self.db.register(email, password) is None:
            return signed("EMAIL_FAILED", rnd, SECRET)
        log.info("registered %s", email)
        return signed("REGISTERED", rnd, SECRET)

    def password_recover(self, f):
        return "SEND_FAIL"

    def everyday_bonus(self, f):
        return "NO_BONUS"

    def get_profile(self, f):
        pid = self._authed(f)
        if pid is None:
            return "ACTIVATION"
        p = self.db.profile(pid)
        fields = [("rnd", f.get("random", "0")), ("soc", "ws"), ("player", pid), ("name", p["name"]),
                  ("crystal", p["crystals"]), ("coins", p["coins"]), ("destroy", p["destroy"]),
                  ("st", now_str()), ("skin", p["skin"]), ("experience", p["experience"])]
        for k, (value, until) in p["inv"].items():
            fields.append(("inv" + k, "%s\t%s" % (value, until or "")))
        text = "Saved Profile: " + ",".join("%s=%s" % kv for kv in fields)
        return signed(text, f.get("yoba_key", ""))

    def set_name(self, f):
        pid = self._authed(f)
        name = f.get("name", "").strip()
        if pid is None or not 2 <= len(name) <= 20 or re.search(r"[,=\t\n\r]", name) or name == "Копатель":
            return "NAME_BAD"
        if self.db.name_taken(name, pid):
            return "NAME_BUSY"

        def apply(p):
            if p["name"]:
                inv = p["inv"].setdefault(str(CHANGENICK), [0, ""])
                inv[0] = max(0, inv[0] - 1)
            p["name"] = name
        self.db.update(pid, apply)
        return "SET_NAME"

    # ---------------------------------------------- shop and profile fields

    def purchase_buy(self, f):
        pid = self._authed(f, "viewer_id")
        if pid is None:
            return "error auth"
        p_ind, count = int(f.get("p_ind", 0)), int(f.get("count", 1))
        coins, gems = int(f.get("pay_coins", 0)), int(f.get("pay_gems", 0))
        if coins < 0 or gems < 0 or count <= 0:
            return "error bad purchase"

        def apply(p):
            if p["coins"] < coins or p["crystals"] < gems:
                return False
            p["coins"] -= coins
            p["crystals"] -= gems
            inv = p["inv"].setdefault(str(p_ind), [0, ""])
            if p_ind in TIMED_PURCHASES:
                inv[0] = count
                if count == 3:
                    inv[1] = "9999-12-30 00:00"
                else:
                    inv[1] = now_str(time.time() + 86400 * TIMED_DAYS.get(count, 30))
            elif p_ind in ONCE_PURCHASES:
                inv[0] = count
            else:
                inv[0] += count
            return True
        if not self.db.update(pid, apply):
            return "NOT_ENOUGH_MONEY"
        return signed("SUCCESS", f.get("random", ""), f.get("yoba_key", ""))

    def purchase_use(self, f):
        pid = self._authed(f, "viewer_id")
        if pid is None:
            return "FAIL"
        p_ind = int(f.get("p_ind", 0))

        def apply(p):
            inv = p["inv"].get(str(p_ind))
            if inv:
                inv[0] = 0 if p_ind in TIMED_PURCHASES else max(0, inv[0] - 1)
        self.db.update(pid, apply)
        return "OK"

    def sell_crystalls(self, f):
        pid = self._authed(f, "viewer_id")
        count = int(f.get("count", 0))

        def apply(p):
            if count <= 0 or p["crystals"] < count:
                return False
            p["crystals"] -= count
            p["coins"] += count * 100
            return True
        return "SUCCSES" if pid and self.db.update(pid, apply) else "FAIL"

    def add_gold(self, f):
        pid = self._authed(f, "viewer_id")
        if pid:
            self.db.update(pid, lambda p: p.__setitem__("coins", p["coins"] + 5))
        return "OK"

    def set_destroy(self, f):
        pid = self._authed(f)
        if pid:
            self.db.update(pid, lambda p: p.__setitem__("destroy", int(f.get("ind", 1))))
        return "OK"

    def set_skin(self, f):
        pid = self._authed(f)
        if pid:
            self.db.update(pid, lambda p: p.__setitem__("skin", int(f.get("skin", 0))))
        return "OK"

    def unban(self, f):
        return "OK"

    new_map_timer = is_peregenerate_map = update_slot_info = unban

    def get_favorit_servers(self, f):
        return ""

    # ---------------------------------------------- map slots

    def get_slot_info(self, f):
        p = self.db.profile(f.get("id", ""))
        slot = int(f.get("map_place", 0))
        if p is None:
            return "NOT AVAILABLE SLOT"
        slots = 1 + int(p["inv"].get("6", [0])[0])
        if not 1 <= slot <= slots:
            return "NOT AVAILABLE SLOT"
        info = p["slots"].get(str(slot), {})
        if info.get("public_id"):
            return str(info["public_id"])
        if info.get("saved") and os.path.exists(world_path(self.data_dir, f["id"], slot)):
            return "1"
        return "EMPTY SLOT"

    def save_map_to_slot_new(self, f):
        pid = self._authed(f)
        slot = int(f.get("map_place", 0))
        data = f.get("buffer")
        if pid is None:
            return "AUTH_FAILED"
        if not isinstance(data, bytes) or not data or slot < 1:
            return "NO_DATA"
        path = world_path(self.data_dir, pid, slot)
        os.makedirs(os.path.dirname(path), exist_ok=True)
        with open(path + ".tmp", "wb") as out:
            out.write(data)
        os.replace(path + ".tmp", path)
        self.db.update(pid, lambda p: p["slots"].setdefault(str(slot), {}).__setitem__("saved", True))
        log.info("map saved: player %s slot %d, %d bytes", pid, slot, len(data))
        return "OK"

    # ---------------------------------------------- published maps

    def public_map(self, f):
        pid = self._authed(f)
        if pid is None:
            return "AUTH_FAILED"
        map_id = self.db.publish(pid, int(f.get("slot_id", 0)), f.get("name", ""), f.get("about", ""), f.get("url", ""))
        return str(map_id)

    def close_map(self, f):
        pid = self._authed(f)
        if pid:
            self.db.close_map(pid, int(f.get("slot_id", 0)), int(f.get("map_id", 0)))
        return "OK"

    def _map_list(self, items):
        if not items:
            return ""
        return "\t" + "\t".join("%d\t%s\t%d\t%s\t%d" % (k, m["owner"], m["slot"], m["name"].replace("\t", " "), m["rating"])
                                for k, m in items)

    def _list(self, f, since=None, sort="public_date"):
        return self._map_list(self.db.list_maps(since=since, sort=sort, page=int(f.get("page", 0)), per_page=MAPS_PER_PAGE))

    def get_latest_map(self, f):
        return self._list(f)

    def get_day_map(self, f):
        return self._list(f, time.time() - 86400, "rating")

    def get_weekly_map(self, f):
        return self._list(f, time.time() - 7 * 86400, "rating")

    def get_monthly_map(self, f):
        return self._list(f, time.time() - 30 * 86400, "rating")

    def get_all_time_map(self, f):
        return self._list(f, None, "rating")

    def search_maps(self, f):
        text = f.get("text", "")
        by_author = f.get("type", "0") != "0"
        return self._map_list(self.db.list_maps(
            sort=f.get("sortby", "public_date"), name=None if by_author else text, author=text if by_author else None,
            page=int(f.get("page", 0)), per_page=MAPS_PER_PAGE))

    def get_map_info(self, f):
        m = self.db.get_map(f.get("map_id", ""))
        if not m:
            return "\t\t\t\t0\t0\t0\t0\t0"
        clean = lambda s: str(s).replace("\t", " ")
        return "\t".join(clean(x) for x in (m["name"], m["about"], m["url"], m["author"], 0, 0, m["rating"], m["owner"], m["slot"]))

    def is_voute_by_map(self, f):
        p = self.db.profile(f.get("id", ""))
        m = self.db.get_map(f.get("map_id", ""))
        if not p or not m or m["owner"] == f.get("id"):
            return "YES"
        return p.get("votes", {}).get(str(f.get("map_id")), "NOT_VOTE")

    def _vote(self, f, vote, delta):
        pid = f.get("id", "")
        map_id = str(f.get("map_id", ""))
        m = self.db.get_map(map_id)
        if not m or m["owner"] == pid:
            return "FAIL"

        def apply(p):
            votes = p.setdefault("votes", {})
            if map_id in votes:
                return False
            votes[map_id] = vote
            return True
        if self.db.update(pid, apply):
            with self.db.lock:
                m["rating"] += delta
                self.db.save()
        return "OK"

    def voute_yes(self, f):
        return self._vote(f, "YES", 1)

    def voute_yes_not(self, f):
        return self._vote(f, "YES", 1)

    def voute_no_not(self, f):
        return self._vote(f, "NO", 0)

    def post_comment(self, f):
        m = self.db.get_map(f.get("map_id", ""))
        text = f.get("post_text", "").replace("\t", " ").strip()[:300]
        if m and text:
            with self.db.lock:
                m["comments"].insert(0, {"id": f.get("id", ""), "name": f.get("post_name", ""), "text": text})
                self.db.save()
        return "OK"

    def get_coments(self, f):
        m = self.db.get_map(f.get("map_id", ""))
        if not m:
            return ""
        start = int(f.get("start_ind", 0))
        page = m["comments"][start:start + COMMENTS_PER_PAGE]
        if not page:
            return ""
        return "\t" + "\t".join("%s\t%s\t%s" % (c["id"], c["name"].replace("\t", " "), c["text"]) for c in page)


def make_handler(scripts, www_dir, data_dir, page_html):
    class Handler(http.server.BaseHTTPRequestHandler):
        server_version = "Kopatel2014/1.0"

        def log_message(self, fmt, *args):
            log.debug("%s %s", self.address_string(), fmt % args)

        def send_text(self, text, status=200, ctype="text/plain; charset=utf-8"):
            body = text.encode("utf-8") if isinstance(text, str) else text
            self.send_response(status)
            self.send_header("Content-Type", ctype)
            self.send_header("Content-Length", str(len(body)))
            self.send_header("Cache-Control", "no-store")
            self.end_headers()
            if self.command != "HEAD":
                self.wfile.write(body)

        def send_file(self, path):
            if not os.path.isfile(path):
                self.send_text("Not found", 404)
                return
            ctype = mimetypes.guess_type(path)[0] or "application/octet-stream"
            if path.endswith(".unity3d"):
                ctype = "application/vnd.unity"
            size = os.path.getsize(path)
            self.send_response(200)
            self.send_header("Content-Type", ctype)
            self.send_header("Content-Length", str(size))
            self.send_header("Cache-Control", "no-cache")
            self.end_headers()
            if self.command == "HEAD":
                return
            with open(path, "rb") as f:
                while True:
                    chunk = f.read(1 << 16)
                    if not chunk:
                        break
                    self.wfile.write(chunk)

        def form(self):
            fields = dict(urllib.parse.parse_qsl(urllib.parse.urlsplit(self.path).query, keep_blank_values=True))
            if self.command == "POST":
                length = int(self.headers.get("Content-Length", 0))
                body = self.rfile.read(length)
                ctype = self.headers.get("Content-Type", "")
                if ctype.startswith("multipart/form-data"):
                    boundary = re.search(r'boundary="?([^";]+)"?', ctype).group(1).encode()
                    for k, v in parse_multipart(body, boundary).items():
                        fields[k] = v if k == "buffer" else v.decode("utf-8", "replace")
                else:
                    fields.update(urllib.parse.parse_qsl(body.decode("utf-8", "replace"), keep_blank_values=True))
            return fields

        def route(self):
            path = urllib.parse.unquote(urllib.parse.urlsplit(self.path).path)
            name = path.rsplit("/", 1)[-1]
            if path in ("/", "/index.html"):
                self.send_text(page_html, ctype="text/html; charset=utf-8")
            elif name == "crossdomain.xml":
                self.send_text('<?xml version="1.0"?><cross-domain-policy><allow-access-from domain="*" />'
                               '<allow-http-request-headers-from domain="*" headers="*" /></cross-domain-policy>',
                               ctype="text/xml")
            elif name.endswith(".php"):
                handler = getattr(scripts, name[:-4], None)
                f = self.form()
                if handler is None or name.startswith("_"):
                    log.warning("unknown script %s %r", path, {k: v for k, v in f.items() if k != "buffer"})
                    self.send_text("UNKNOWN_SCRIPT")
                    return
                try:
                    result = handler(f)
                except Exception:
                    log.exception("%s failed", name)
                    result = "ERROR"
                log.info("%s %s -> %s", name, {k: v for k, v in f.items() if k not in ("buffer", "password")},
                         result if len(result) < 120 else result[:117] + "...")
                self.send_text(result)
            elif "/worlds/" in path:
                rel = path.split("/worlds/", 1)[1]
                if ".." in rel or "\\" in rel:
                    self.send_text("Not found", 404)
                else:
                    self.send_file(os.path.join(data_dir, "worlds", *rel.split("/")))
            elif path.startswith("/news/"):
                self.send_text(NEWS)
            else:
                rel = path.lstrip("/")
                if ".." in rel or not rel:
                    self.send_text("Not found", 404)
                else:
                    self.send_file(os.path.join(www_dir, *rel.split("/")))

        do_GET = do_POST = do_HEAD = route

    return Handler


def serve_http(host, port, db, www_dir, data_dir):
    with open(os.path.join(www_dir, "index.html"), encoding="utf-8") as f:
        page = f.read()
    handler = make_handler(Scripts(db, data_dir), www_dir, data_dir, page)
    httpd = http.server.ThreadingHTTPServer((host, port), handler)
    httpd.daemon_threads = True
    log.info("HTTP on %s:%d", host, port)
    httpd.serve_forever()
