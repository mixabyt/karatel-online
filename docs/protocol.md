# Копатель Онлайн 2014: що говорить клієнт

Клієнт: `Digger2014.unity3d`, UnityWeb v3, Unity 4.5.1f3 (рантайм веб-плеєра `3.x.x`), версія гри `159f9`.
Розпаковка: `python tools/unityweb.py <bundle> <dir>`; код: `Assembly-CSharp.dll` (PUN + гра), `Photon3Unity3D.dll`.

## Що змінює патч (`tools/patcher`)
| Місце | Було | Стало |
|---|---|---|
| Усі `ldstr` з хостами (`vk.diggerworld.ru/VK2/`, `62.75.224.107/Test2/`, `diggerworld.ru/content/`, `diggerworld.ru/news/`, IP Photon…) | зашиті сервери | `KopatelPatch.Host(s)`: хост замінюється на той, з якого завантажено `.unity3d` (`Application.absoluteURL`) |
| `App.Start` | `CurPlatform` зі сцени, Photon Cloud | `KopatelPatch.PrefetchPolicy()` (policy з 843 до підключення Photon, результат у лозі плеєра), `CurPlatform = WEBSITE`, `LocalTest = true` → `UseMyServer(<хост>, 5055, "Master")` |
| `PhotonNetwork..cctor` | `ConnectionProtocol.Udp` | `ConnectionProtocol.Tcp` |
| `App.KeyEncrypt` | RSA-OAEP з чужим ключем | повертає рядок без змін |

Отже, шляхи на нашому HTTP-сервері такі самі, як були в оригіналі: `/Test2/*.php` (скрипти платформи WEBSITE), `/WB/` (збереження карт), `/content/*.unity3d` (паки, їх немає в архіві, віддаємо 404), `/news/news.txt`.

## Photon (TCP, порт 5055)
Unity Web Player вимагає socket policy на TCP 843 (`<policy-file-request/>\0` → XML `<cross-domain-policy>`).

Кадр клієнт→сервер: `FB len32BE channel reliable` + повідомлення; `len` рахує весь кадр.
Ping: сирі 5 байт `F0 time32` → відповідь 9 байт `F0 serverTime32 clientTime32`.
Повідомлення: `F3 type` + тіло. Типи: 0 Init (тіло: версія + appId[32]) → відповідь `F3 01`; 2 OpRequest; 3 OpResponse; 4 Event; 6/7 внутрішні (InitEncryption).
Біт `0x80` у type означає, що тіло (opCode + параметри) зашифроване AES-256-CBC, IV = 0, PKCS7, ключ = SHA256(спільний секрет DH).
DH: Oakley 768, g = 22, ключі big-endian без провідних нулів. Клієнт шле op 0 {1: pubkey}, сервер відповідає type 7 op 0 {1: pubkey}.
Шифрується тільки Authenticate (230); відповідати можна без шифрування.

Серіалізація Protocol16 (`ExitGames.Client.Photon.Protocol`): byte 'b', bool 'o', short 'k', int 'i', long 'l', float 'f', double 'd', string 's' (len16 + UTF-8), byte[] 'x' (len32), int[] 'n', string[] 'a', object[] 'z', typed array 'y', Hashtable 'h', Dictionary 'D', custom 'c' (код + len16 + байти; 'V' Vector3, 'W' Vector2, 'Q' Quaternion, 'P' PhotonPlayer), null '*'.

LoadBalancing (майстер і геймсервер — один процес, адреса геймсервера = адреса майстра):
- 230 Authenticate {220 appVersion, 224 appId, 225 userId}; на ок → клієнт робить 229 JoinLobby → сервер шле ev 230 GameList {222: {name: props}} і ev 226 AppStats {229, 227, 228}.
- 227 Create / 226 Join / 225 JoinRandom на майстрі → {230 адреса геймсервера, 255 ім'я кімнати}; на геймсервері → {254 actorNr, 249 props акторів, 248 props кімнати} + ev 255 Join {254, 249, 252 actorList}.
- 253 RaiseEvent {244 code, 245 data, 246 receivers (0 інші, 1 всі, 2 майстер), 247 кеш (4/5 додати, 6 прибрати, 7 прибрати для тих, хто вийшов), 252 targetActors, 240 group} → ev code {245, 254 sender}.
- 252 SetProperties {251, 254 actor?, 250 broadcast} → ev 253 {253 target, 251 props, 254 sender}.
- 248 ChangeGroups {239 remove, 238 add}; 254 Leave → ev 254 Leave {254}.
Кімнати відокремлені за appVersion (`WEBSITE159f9Qshard_1`).

## HTTP (WWWForm POST), секрет `Ysm9hrlkzzQas5DXzvHmm`
Підпис `MD52(x)` = md5 hex у нижньому регістрі.
- `enter_game.php` {email, password, random} → `"<id>\t<auth_key>\n" + md5("<id>\t<auth_key>_<random>_<secret>")` або `LOGIN_FAILED`.
- `register.php` {email, password, uniq_id, check_hash, random} → `"REGISTERED\n" + md5("REGISTERED_<random>_<secret>")` (`WRONG_EMAIL`/`REG_FAILED`/`EMAIL_FAILED` у тому ж форматі).
- `password_recover.php` {email} → `SEND` / `EMAIL_NOT_FOUND`.
- `everyday_bonus.php` {id} → `OK a b c` (екран бонусу) або будь-що інше (далі).
- `get_profile.php` {id, auth_key, random, yoba_key} → `"Saved Profile: rnd=<random>,soc=ws,player=<id>,name=…,crystal=…,coins=…,destroy=…,st=YYYY-MM-DD HH:MM,skin=…,experience=…,inv<N>=<v>\t<YYYY-MM-DD HH:MM>,…\n" + md5(text + "_" + yoba_key)`; `BANNED…`, `ACTIVATION`.
- `set_name.php` {id, auth_key, name} → `SET_NAME` або що завгодно (ім'я зайняте). Порожнє ім'я або «Копатель» → екран вибору імені.
- `purchase_buy.php` {viewer_id, auth_key, p_ind, count, pay_coins, pay_gems, random, yoba_key, beta_key} → `"SUCCESS\n" + md5("SUCCESS_<random>_<yoba_key>")`.
- `purchase_use.php` {viewer_id, p_ind} → `OK`. `sell_crystalls.php` {count} → `SUCCSES` (1 кристал = 100 монет). `add_gold.php` +5 монет.
- `set_destroy.php` {id, ind}, `set_skin.php` {id, skin}, `new_map_timer.php`, `is_peregenerate_map.php`, `update_slot_info.php`, `unban.php` — тільки зберегти/OK.
- Слоти: `get_slot_info.php` {id, map_place} → `EMPTY SLOT` | `NOT AVAILABLE SLOT` | `1` (є приватна карта) | `<map_id>` (опублікована) | `<0` (заблоковано).
- Збереження: POST `/WB/save_map_to_slot_new.php` {id, map_place, auth_key, buffer(file)} → `OK`; читання: GET `/WB/worlds/<md5(id)[:2]>/<id>_<slot>.map`.
- Публічні карти: `public_map.php` {id, slot_id, name, about, url}, `close_map.php` {id, slot_id, map_id}; списки `get_latest_map|get_day_map|get_weekly_map|get_monthly_map|get_all_time_map|search_maps` {page[, text, sortby, type]} → `"\t" + (map_id\tauthor_id\tslot\tname\trating)*`; `get_map_info.php` {map_id} → `name\tabout\turl\tauthor\t_\t_\trating\towner_id\tslot`.
- Голоси/коментарі: `is_voute_by_map.php` → `NOT_VOTE|YES|NO`; `voute_yes*.php`, `voute_no_not.php`; `get_coments.php` → `"\t" + (id\tname\ttext)*`; `post_comment.php`.
- `get_favorit_servers.php` → `"\t" + (name\tadmin_id\tslot)*`.
- `/news/news.txt`: блоки через `|`, поля через `;`: текст;url_картинки;url;кнопка;url_кнопки.
