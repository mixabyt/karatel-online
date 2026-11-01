"""Players, slots and published maps in one JSON file (plus .map files on disk)."""
import hashlib
import json
import os
import secrets
import threading
import time

SLOTS = 6               # StorePurchase.SLOTS: number of extra map slots
STANDARD_SKIN = 39      # StorePurchase.STANDART_SKIN, owned forever by everyone
FOREVER = "2200-01-01 00:00:00"


def md5(text):
    return hashlib.md5(text.encode("utf-8")).hexdigest()


def now_str(offset_days=0):
    return time.strftime("%Y-%m-%d %H:%M:%S", time.localtime(time.time() + offset_days * 86400))


class Database:
    def __init__(self, data_dir, start_crystals=500, start_coins=50000):
        self.dir = data_dir
        self.path = os.path.join(data_dir, "db.json")
        self.worlds = os.path.join(data_dir, "worlds")
        self.backups = os.path.join(data_dir, "backups")
        self.start_crystals, self.start_coins = start_crystals, start_coins
        self.lock = threading.RLock()
        os.makedirs(self.worlds, exist_ok=True)
        os.makedirs(self.backups, exist_ok=True)
        if os.path.exists(self.path):
            with open(self.path, encoding="utf-8") as f:
                self.data = json.load(f)
        else:
            self.data = {"next_player": 1001, "next_map": 100, "players": {}, "maps": {}}

    def save(self):
        tmp = self.path + ".tmp"
        with open(tmp, "w", encoding="utf-8") as f:
            json.dump(self.data, f, ensure_ascii=False, indent=1)
        os.replace(tmp, self.path)

    # ---- players ----
    @staticmethod
    def _hash(password, salt):
        return hashlib.sha256((salt + password).encode("utf-8")).hexdigest()

    def find_email(self, email):
        return next((pid for pid, p in self.data["players"].items() if p["email"] == email), None)

    def register(self, email, password):
        with self.lock:
            if self.find_email(email):
                return None
            pid = str(self.data["next_player"])
            self.data["next_player"] += 1
            salt = secrets.token_hex(8)
            self.data["players"][pid] = {
                "email": email, "salt": salt, "pw": self._hash(password, salt), "auth": secrets.token_hex(16),
                "name": "", "crystals": self.start_crystals, "coins": self.start_coins, "destroy": 0, "skin": 0,
                "inv": {str(SLOTS): [1, ""], str(STANDARD_SKIN): [1, FOREVER]},
                "slots": {}, "votes": {},
            }
            self.save()
            return pid

    def login(self, email, password):
        with self.lock:
            pid = self.find_email(email)
            if pid is None:
                return None
            p = self.data["players"][pid]
            return pid if p["pw"] == self._hash(password, p["salt"]) else None

    def reset_password(self, email):
        with self.lock:
            pid = self.find_email(email)
            if pid is None:
                return None
            p = self.data["players"][pid]
            new = secrets.token_hex(4)
            p["salt"] = secrets.token_hex(8)
            p["pw"] = self._hash(new, p["salt"])
            self.save()
            return new

    def player(self, pid, auth_key=None):
        """The player record, or None if unknown or (when given) the auth key does not match."""
        p = self.data["players"].get(str(pid))
        if p is None or (auth_key is not None and auth_key != p["auth"]):
            return None
        return p

    def name_taken(self, name, pid):
        low = name.lower()
        return any(p["name"].lower() == low for k, p in self.data["players"].items() if k != str(pid))

    # ---- maps ----
    def map_path(self, pid, slot):
        sub = md5(str(pid))[:2]
        return os.path.join(self.worlds, sub, f"{pid}_{slot}.map")

    def slot_count(self, p):
        return 1 + p["inv"].get(str(SLOTS), [0, ""])[0]

    def slot(self, p, slot):
        return p["slots"].setdefault(str(slot), {"public": 0})

    def publish(self, pid, slot, name, about, url):
        with self.lock:
            p = self.data["players"][str(pid)]
            s = self.slot(p, slot)
            if s["public"] and str(s["public"]) in self.data["maps"]:
                m = self.data["maps"][str(s["public"])]
            else:
                mid = self.data["next_map"]
                self.data["next_map"] += 1
                m = self.data["maps"][str(mid)] = {"id": mid, "owner": str(pid), "slot": int(slot), "rating": 0,
                                                   "published": time.time(), "comments": [], "votes": {}}
                s["public"] = mid
            m.update(name=name, about=about, url=url, author=p["name"])
            self.save()
            return m["id"]

    def unpublish(self, pid, slot):
        with self.lock:
            p = self.data["players"][str(pid)]
            s = self.slot(p, slot)
            if s["public"]:
                self.data["maps"].pop(str(s["public"]), None)
                s["public"] = 0
                self.save()

    def public_maps(self):
        return list(self.data["maps"].values())
