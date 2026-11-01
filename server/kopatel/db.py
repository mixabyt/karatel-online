"""Accounts, profiles, map slots and published maps, kept in one JSON file.

Small enough for a private server: everything lives in memory, guarded by one lock, and is written
back atomically after each change. Map files themselves are stored next to it (see web.py).
"""
import json
import os
import secrets
import threading
import time

# The inventory a new player starts with: StorePurchase index -> [count, end time of timed items].
# There is no payment here, so give enough to build with; the rest is bought in the shop with coins.
STARTING_INVENTORY = {
    "6": [4, ""],                    # SLOTS: 1 + 4 map slots
    "33": [1, ""],                   # CHANGENICK
    "39": [1, "2200-01-01 00:00"],   # STANDART_SKIN, as the client's own ProfileINI.Reset()
}
STARTING_COINS = 20000
STARTING_CRYSTALS = 2000


def now_str(t=None):
    return time.strftime("%Y-%m-%d %H:%M", time.localtime(t))


class Database:
    def __init__(self, path):
        self.path = path
        self.lock = threading.RLock()
        self.data = {"next_id": 100001, "next_map_id": 2, "accounts": {}, "profiles": {}, "maps": {}}
        if os.path.exists(path):
            with open(path, encoding="utf-8") as f:
                self.data.update(json.load(f))

    def save(self):
        with self.lock:
            tmp = self.path + ".tmp"
            with open(tmp, "w", encoding="utf-8") as f:
                json.dump(self.data, f, ensure_ascii=False, indent=1)
            os.replace(tmp, self.path)

    # -------------------------------------------------- accounts

    def register(self, email, password):
        """Returns the new player's id, or None if the email is taken."""
        with self.lock:
            if email in self.data["accounts"]:
                return None
            pid = str(self.data["next_id"])
            self.data["next_id"] += 1
            self.data["accounts"][email] = {"id": pid, "password": password, "auth_key": secrets.token_hex(16)}
            self.data["profiles"][pid] = {
                "email": email, "name": "", "coins": STARTING_COINS, "crystals": STARTING_CRYSTALS,
                "destroy": 1, "skin": 0, "experience": 0, "inv": {k: list(v) for k, v in STARTING_INVENTORY.items()},
                "slots": {}, "votes": {},
            }
            self.save()
            return pid

    def login(self, email, password):
        with self.lock:
            acc = self.data["accounts"].get(email)
            if acc and acc["password"] == password:
                return acc["id"], acc["auth_key"]
            return None

    def check(self, pid, auth_key):
        """True when auth_key belongs to player pid."""
        with self.lock:
            prof = self.data["profiles"].get(str(pid))
            if not prof:
                return False
            acc = self.data["accounts"].get(prof["email"])
            return bool(acc) and acc["auth_key"] == auth_key

    def profile(self, pid):
        with self.lock:
            return self.data["profiles"].get(str(pid))

    def name_taken(self, name, pid):
        with self.lock:
            low = name.lower()
            return any(p["name"].lower() == low and k != str(pid) for k, p in self.data["profiles"].items())

    def update(self, pid, fn):
        """Run fn(profile) under the lock and save; returns fn's result (None if no such player)."""
        with self.lock:
            prof = self.data["profiles"].get(str(pid))
            if prof is None:
                return None
            result = fn(prof)
            self.save()
            return result

    # -------------------------------------------------- published maps

    def publish(self, pid, slot, name, about, url):
        with self.lock:
            prof = self.data["profiles"][str(pid)]
            slot_info = prof["slots"].setdefault(str(slot), {})
            map_id = slot_info.get("public_id")
            if not map_id:
                map_id = self.data["next_map_id"]
                self.data["next_map_id"] += 1
            self.data["maps"][str(map_id)] = {
                "name": name, "about": about, "url": url, "author": prof["name"], "owner": str(pid),
                "slot": int(slot), "rating": 0, "date": time.time(), "comments": [], "open": True,
            }
            slot_info["public_id"] = map_id
            slot_info["saved"] = True
            self.save()
            return map_id

    def close_map(self, pid, slot, map_id):
        with self.lock:
            m = self.data["maps"].get(str(map_id))
            if m and m["owner"] == str(pid):
                del self.data["maps"][str(map_id)]
            prof = self.data["profiles"].get(str(pid))
            if prof:
                prof["slots"].get(str(slot), {}).pop("public_id", None)
            self.save()

    def get_map(self, map_id):
        with self.lock:
            return self.data["maps"].get(str(map_id))

    def list_maps(self, since=None, sort="public_date", name=None, author=None, page=0, per_page=20):
        with self.lock:
            items = [(int(k), m) for k, m in self.data["maps"].items()]
        if since is not None:
            items = [(k, m) for k, m in items if m["date"] >= since]
        if name:
            items = [(k, m) for k, m in items if name.lower() in m["name"].lower()]
        if author:
            items = [(k, m) for k, m in items if author.lower() in m["author"].lower()]
        key = (lambda km: km[1]["rating"]) if sort == "rating" else (lambda km: km[1]["date"])
        items.sort(key=key, reverse=True)
        return items[page * per_page:(page + 1) * per_page]
