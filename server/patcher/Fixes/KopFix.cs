// Code the fan build of the 2012 client had cut out (replaced by nop/ret), rebuilt after the same
// methods in the 2014 client and adapted to the 2012 classes.
//
// Every method is named Type__Method and takes the instance as its first parameter. KopPatcher copies
// its IL into Type: into the existing Type.Method (whose body is replaced), or into a new instance
// method called Method. The code therefore runs inside the original class and may use its private
// members (compiled here against a publicized copy of the assembly).
//
// Calls between these methods go through their KopFix names; the patcher points them at the copies.
// Kept to C# features that compile to plain IL: no lambdas, iterators or array initializers of
// primitive types (those would need extra compiler-generated types), and no instance calls on enums
// or other value types through object/interface methods (Roslyn emits "constrained.", which the web
// player's IL verifier rejects; box first).
using System.Collections;
using UnityEngine;

public static class KopFix
{
    // ---------------- WorldGameObject ----------------

    // Fan version: "isServer = true; StartServer()" for everyone. Room creation / joining is
    // driven by Level.Start again, as in the 2014 client.
    public static void WorldGameObject__Start(WorldGameObject self)
    {
        Igor__Kop_GunMode = false;
        WorldGameObject__Kop_IgnoreLayers(self, "Players", "SmallDecor");
        WorldGameObject__Kop_IgnoreLayers(self, "Players", "ParticleCubesDestroy");
        WorldGameObject__Kop_IgnoreLayers(self, "Players", "StorePreview");
    }

    public static void WorldGameObject__Kop_IgnoreLayers(WorldGameObject self, string a, string b)
    {
        int la = LayerMask.NameToLayer(a), lb = LayerMask.NameToLayer(b);
        if (la >= 0 && lb >= 0)
            Physics.IgnoreLayerCollision(la, lb);
    }

    // Fan version called OnCreatedRoom() directly, so the player never was in a Photon room and
    // nothing owned by the network worked. 2014: create the room; PUN then calls OnCreatedRoom.
    // Not connected yet: connect and come back from OnJoinedLobby; no server within 8 s: play offline.
    public static void WorldGameObject__StartServer(WorldGameObject self)
    {
        PeerState state = PhotonNetwork.connectionStateDetailed;
        bool atMaster = state == PeerState.JoinedLobby || state == PeerState.ConnectedToMaster;
        if (!atMaster && !PhotonNetwork.offlineMode)
        {
            MainMenu.INSTANCE.loading_text = "Подключение к серверу...";
            if (state == PeerState.Disconnected || state == PeerState.PeerCreated || state == PeerState.Uninitialized)
            {
                PhotonNetwork.autoJoinLobby = true;
                PhotonNetwork.ConnectUsingSettings(App__Kop_GameVersion(App.instance));
            }
            self.CancelInvoke("Kop_OfflineFallback");
            self.Invoke("Kop_OfflineFallback", 8f);
            return;
        }
        self.CancelInvoke("Kop_OfflineFallback");

        GameINI ini = App.instance.gameIni;
        ini.ServerRating = 10;
        Hashtable props = new Hashtable();
        props.Add("player_id", ini.playerID ?? "");
        props.Add("slot_id", ini.slotID);
        props.Add("public_id", ini.publicMapID);
        props.Add("password", ini.Password ?? "");
        props.Add("map_name", ini.server_name ?? "");
        props.Add("map_about", ini.server_about ?? "");
        props.Add("is_watch", ini.isWatch);
        props.Add("game_type", (int)ini.gameType);
        props.Add("rating", ini.ServerRating);
        string[] lobbyProps = new string[9];
        lobbyProps[0] = "player_id";
        lobbyProps[1] = "slot_id";
        lobbyProps[2] = "public_id";
        lobbyProps[3] = "password";
        lobbyProps[4] = "map_name";
        lobbyProps[5] = "map_about";
        lobbyProps[6] = "is_watch";
        lobbyProps[7] = "game_type";
        lobbyProps[8] = "rating";
        PhotonNetwork.CreateRoom(null, ini.isOnline, true, ini.mapPopulation, props, lobbyProps);
    }

    // PUN callback (new on WorldGameObject): the connection StartServer waited for is ready.
    public static void WorldGameObject__OnJoinedLobby(WorldGameObject self)
    {
        if (App.instance.gameIni.isServer && PhotonNetwork.room == null && !self.IsWorldGenerated)
            self.StartServer();
    }

    // No Photon server answered: build the world anyway in PUN's offline mode. A connection still in
    // progress is closed first; offline mode starts from OnDisconnectedFromPhoton, because PUN resets
    // the local player id when the disconnect completes (doing it earlier left the player with id -1).
    public static void WorldGameObject__Kop_OfflineFallback(WorldGameObject self)
    {
        if (PhotonNetwork.room != null || PhotonNetwork.offlineMode)
            return;
        PeerState state = PhotonNetwork.connectionStateDetailed;
        if (state != PeerState.PeerCreated && state != PeerState.Disconnected && state != PeerState.Uninitialized)
        {
            PhotonNetwork.Disconnect();
            return;
        }
        PhotonNetwork.offlineMode = true;
        self.StartServer();
    }

    // PUN callback (new on WorldGameObject). Before the world exists: the server start we were waiting
    // for failed, continue offline. In the game: the server went away; show the game's own
    // "connection lost, save your map" window instead of playing on with a broken network state.
    public static void WorldGameObject__OnDisconnectedFromPhoton(WorldGameObject self)
    {
        if (PhotonNetwork.offlineMode)
            return;
        if (!self.IsWorldGenerated)
        {
            if (App.instance.gameIni.isServer && PhotonNetwork.room == null)
            {
                self.CancelInvoke("Kop_OfflineFallback");
                PhotonNetwork.offlineMode = true;
                self.StartServer();
            }
            return;
        }
        MainMenu.INSTANCE.curWindow = MainMenu.Windows.FailedConnect;
        MainMenu.INSTANCE.showMenu = true;
        Screen.lockCursor = false;
    }

    public static void WorldGameObject__OnFailedToConnectToPhoton(WorldGameObject self)
    {
        WorldGameObject__OnDisconnectedFromPhoton(self);
    }

    // Inserted at the start of OnCreatedRoom (whose first 79 bytes the fan build blanked out).
    public static void WorldGameObject__Kop_OnCreatedRoom(WorldGameObject self)
    {
        PhotonNetwork.isMessageQueueRunning = true;
        App.instance.gameIni.isServer = true;
        MainMenu.INSTANCE.loading_text = "Создаём мир...";
        WorldGameObject__Kop_RegisterPlayer(self, false);
    }

    // Tells everybody (now and, buffered, later joiners) who we are; the host answers
    // want_level=true by sending the map. Same call as Level.Start in the 2014 client.
    public static void WorldGameObject__Kop_RegisterPlayer(WorldGameObject self, bool wantLevel)
    {
        int skin = ProfileINI.skin;
        object[] args = new object[5];
        args[0] = PhotonNetwork.playerName;
        args[1] = skin;
        args[2] = VkontakteAPI.INSTANCE._viewerId;
        args[3] = wantLevel;
        args[4] = PhotonNetwork.player;
        self.photonView.RPC("SendPlayerProperties", PhotonTargets.AllBuffered, args);
    }

    // Fan version spawned the player with Object.Instantiate, so its PhotonView had no owner and
    // PlayerNetwork/CameraController crashed; it also dropped FlyBonus (resets the ladder body).
    public static void WorldGameObject__CompleteWorldGeneration(WorldGameObject self)
    {
        self.WorldCamera.active = false;
        MainMenu.INSTANCE.showMenu = false;
        self.IsWorldGenerated = true;
        if (self.MainPlayer == null)
        {
            self.MainPlayer = PhotonNetwork.Instantiate(self.Player_Prefab.name, self.GenerateSpawnPoint(),
                                                        self.transform.rotation, 0);
            self.MainPlayer.name = "MAIN_PLAYER";
            self.MainPlayerEyes = self.MainPlayer.transform.FindChild("recoil/Main Camera").GetComponentInChildren<Camera>();
            self.SetNamesForAllPlayers();
            self.SetSkinsForAllPlayers();
        }
        else
        {
            self.MainPlayer.transform.position = self.GenerateSpawnPoint();
        }
        MainMenu.INSTANCE.SpeedBonus();
        MainMenu.INSTANCE.FlyBonus();
        ProfileINI.tutorial_watch = 1;
        self.ShowGameGui();
        MainMenu.INSTANCE.curWindow = MainMenu.Windows.Game;
        MainMenu.INSTANCE.showMenu = false;
        self.EnableMouseLook();
        Screen.lockCursor = true;
        self.SetAmbientSoundOnLevel();
        Debug.Log("Kop sounds: in game, sound volume " + ProfileINI.sound_volume + ", ambient volume " + ProfileINI.ambient_volume);
        EntityBase.OnWorldGenerated();
    }

    // Fan version: empty. 2014: keep the player inside the map.
    public static void WorldGameObject__LateUpdate(WorldGameObject self)
    {
        GameObject player = self.MainPlayer;
        if (player == null || !self.IsWorldGenerated)
            return;
        Vector3 pos = player.transform.position;
        float maxX = self.m_WorldData.GetMaxBlockX();
        float maxZ = self.m_WorldData.GetMaxBlockY();
        bool moved = false;
        if (pos.x < 1f) { pos.x = 1f; moved = true; }
        if (pos.z < 1f) { pos.z = 1f; moved = true; }
        if (pos.x > maxX) { pos.x = maxX; moved = true; }
        if (pos.z > maxZ) { pos.z = maxZ; moved = true; }
        if (moved)
            player.transform.position = pos;
    }

    // ---------------- App (menu side of the connection) ----------------

    // Fan version: "curWindow = 29" (a window that does not exist), so the client never connected to
    // Photon. Connect to the only shard of the web version and open the menu once connected.
    public static void App__ConnectToRandomServer(App self)
    {
        self.connect_to_random_shard = true;
        self.go_to_menu_on_conected = true;
        PhotonNetwork.autoJoinLobby = true;
        if (PhotonNetwork.connected)
        {
            self.OnConnectedToPhoton();
            return;
        }
        MainMenu.INSTANCE.curWindow = MainMenu.Windows.Loading;
        MainMenu.INSTANCE.loading_text = "Подключение к серверу...";
        PhotonNetwork.ConnectUsingSettings(App__Kop_GameVersion(self));
    }

    public static string App__Kop_GameVersion(App self)
    {
        // Boxed on purpose: platform.ToString() compiles to a "constrained." call, which the web player's
        // old IL verifier rejects for enums.
        object platform = self.platform;
        return platform.ToString() + self.game_verison + "Qshard_1";
    }

    // Fan version: empty, so a missing server left the player on the loading screen. Go to the menu;
    // maps then run in offline mode.
    public static void App__OnFailedToConnectToPhoton(App self)
    {
        Debug.Log("Photon server not reachable, playing offline");
        if (Application.loadedLevelName != "Menu")
            return; // in the game scene StartServer falls back to offline mode by itself
        MainMenu.INSTANCE.curWindow = MainMenu.Windows.Battles;
        MainMenu.INSTANCE.battlesRightMenu = MainMenu.BatllesRightMenu.NEWS;
    }

    // Frames since Kop_FpsStart (Time.realtimeSinceStartup), for the "Kop fps" log line.
    public static int App__Kop_FpsFrames;
    public static float App__Kop_FpsStart;

    // Unchanged, except that a room without "game_type" (PUN's offline room) no longer throws every frame.
    // Also logs the frame rate every 10 s (menu and map), with the size the game renders at.
    public static void App__Update(App self)
    {
        App__Kop_FpsFrames++;
        float now = Time.realtimeSinceStartup;
        if (now - App__Kop_FpsStart >= 10f)
        {
            if (App__Kop_FpsStart > 0f)
                Debug.Log("Kop fps: " + (int)(App__Kop_FpsFrames / (now - App__Kop_FpsStart)) + " in " + Application.loadedLevelName
                          + " at " + Screen.width + "x" + Screen.height + (Screen.fullScreen ? " (Unity full screen)" : "")
                          + ", quality " + QualitySettings.GetQualityLevel() + (Igor__Kop_GunMode ? ", guns" : ""));
            App__Kop_FpsStart = now;
            App__Kop_FpsFrames = 0;
        }
        if (self.gameIni != null && PhotonNetwork.room != null)
        {
            object gameType = PhotonNetwork.room.customProperties["game_type"];
            if (gameType is int)
                self.gameIni.gameType = (GameINI.GameType)(int)gameType;
        }
        ProfileINI.server_time = ProfileINI.server_time.AddSeconds(Time.deltaTime);
        if (self.is_waiting_shard_population)
            self.WaitingShardPopulation();
    }

    // Unchanged, except for the volume defaults. ProfileINI.sound_volume/ambient_volume start at 0 and
    // only LoadFromPrefs or Reset set them; a new player goes from the profile straight to choosing a
    // nick, whose CheckName then saved those zeros, so the game stayed silent for good. Give them the
    // values Reset uses and, once, repair prefs that already hold the two zeros. A second App (back in
    // the menu scene) is destroyed by Awake and must not touch the loaded settings.
    public static void App__Start(App self)
    {
        if (App.instance == self)
        {
            Application.targetFrameRate = 120;
            ProfileINI.sound_volume = 0.7f;
            ProfileINI.ambient_volume = 0.7f;
            if (PlayerPrefs.GetInt("kop_volume_fix", 0) == 0)
            {
                if (PlayerPrefs.HasKey("sound_volume") && PlayerPrefs.GetFloat("sound_volume") == 0f
                    && PlayerPrefs.GetFloat("ambient_volume") == 0f)
                {
                    PlayerPrefs.SetFloat("sound_volume", 0.7f);
                    PlayerPrefs.SetFloat("ambient_volume", 0.7f);
                    Debug.Log("Kop sounds: saved volumes were 0, reset to 0.7");
                }
                PlayerPrefs.SetInt("kop_volume_fix", 1);
            }
            object[] hello = new object[1];
            hello[0] = self.gameObject.name;
            Application.ExternalCall("KopHello", hello);
        }
        MainMenu.INSTANCE.curWindow = MainMenu.Windows.WelcomeScreen;
        ProfileINI.LoadLoginInfo();
    }

    // ---------------- full screen ----------------

    // The page runs in the launcher, whose Firefox has a full screen that Esc does not end (www/index.html,
    // launcher/mozilla.cfg). The page says so in answer to App.Start's KopHello.
    public static bool App__Kop_BrowserFullscreen;

    public static void App__Kop_SetBrowserFullscreen(App self, string value)
    {
        App__Kop_BrowserFullscreen = value == "1";
        Debug.Log("Kop fullscreen: browser full screen " + App__Kop_BrowserFullscreen);
    }

    // F12. In the launcher: the browser's full screen, via the page. Elsewhere unchanged: the game's own
    // full screen (which the Web Player ends on Esc, a page cannot enter one without a click).
    public static void MainMenu__toggleFullScreen(MainMenu self)
    {
        if (App__Kop_BrowserFullscreen)
        {
            Application.ExternalCall("fullscreen", new object[0]);
            return;
        }
        if (Screen.fullScreen)
        {
            Screen.SetResolution(800, 600, false);
            return;
        }
        Resolution resolution = Screen.currentResolution;
        Screen.SetResolution(resolution.width, resolution.height, true);
    }

    // ---------------- weapons on building maps ----------------

    // G on a building map: the player has taken out the weapons of the battle modes
    // (CameraController.Kop_SetGunMode). Every new map starts without them (WorldGameObject.Start).
    public static bool Igor__Kop_GunMode;

    // The room's GameINI.GameType, -1 without a room or without "game_type" (PUN's offline room).
    public static int Igor__Kop_RoomGameType(Igor self)
    {
        if (PhotonNetwork.room == null)
            return -1;
        object gameType = PhotonNetwork.room.customProperties["game_type"];
        if (!(gameType is int))
            return -1;
        return (int)gameType;
    }

    // Guns, crosshair, R to reload, no speed/fly bonuses: in the battle modes, and on a building map
    // while the player holds the weapons. Unchanged otherwise, except that a room without "game_type"
    // no longer throws.
    public static bool Igor__get_enableGuns(Igor self)
    {
        int type = Igor__Kop_RoomGameType(self);
        if (type == (int)GameINI.GameType.BUILDING)
            return Igor__Kop_GunMode;
        return Igor__Kop_BattleGuns(self);
    }

    // The battle modes alone. Things every client computes for itself and must agree on (blocks that
    // lost their support falling off, BlocksAvulsion) may not depend on whether this player holds a gun.
    public static bool Igor__Kop_BattleGuns(Igor self)
    {
        int type = Igor__Kop_RoomGameType(self);
        return type == (int)GameINI.GameType.TEAM_BATTLE || type == (int)GameINI.GameType.DEATHMATCH;
    }

    // Unchanged, plus G on a building map: take out the weapons or put them away again.
    public static void MainMenu__UpdateSpecialBonus(MainMenu self)
    {
        MainMenu__Kop_CaptureVoiceKey(self);
        if (MainMenu__Kop_VoiceKeyCapture)
            return;  // the key being chosen must not also toggle a bonus or the weapons
        if (!bs._Igor.enableGuns)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && ProfileINI.GetPurchesValue(StorePurchase.SPEED) != 0)
            {
                self._SpeedEnabled = !self._SpeedEnabled;
                self.SpeedBonus();
            }
            if (Input.GetKeyDown(KeyCode.Alpha2) && ProfileINI.GetPurchesValue(StorePurchase.FLY) != 0)
            {
                self._FlyEnabled = !self._FlyEnabled;
                self.FlyBonus();
            }
            if (Input.GetKeyDown(KeyCode.Alpha3) && ProfileINI.GetPurchesValue(StorePurchase.TIME_OF_DAY) != 0
                && !App.instance.gameIni.isWatch && Level.Instance.IsAdministrator())
            {
                TimeOfDay.SwitchDayNightForAll();
            }
        }
        WorldGameObject world = WorldGameObject.INSTANCE;
        if (world == null || !world.IsWorldGenerated || world.MainPlayer == null || world.IsMenuOpen())
            return;
        // Voice chat (P) without the battle HUD, which handles it itself (CameraController.UpdateWeapon).
        if (!bs._Igor.enableGuns)
        {
            CameraController voice = world.MainPlayer.GetComponent<CameraController>();
            if (voice != null)
                CameraController__Kop_UpdateVoice(voice);
        }
        if (!Input.GetKeyDown(KeyCode.G) || ChatClient.isEnabled || world.IsMainPlayerDead)
            return;
        if (Igor__Kop_RoomGameType(bs._Igor) != (int)GameINI.GameType.BUILDING)
            return;
        CameraController controller = world.MainPlayer.GetComponent<CameraController>();
        if (controller != null)
            CameraController__Kop_SetGunMode(controller, !Igor__Kop_GunMode);
    }

    // Takes out the weapons as in a battle (first person, the weapon window the first time) or puts
    // them away and gives back the shovel and the saved camera. Ammo is refilled each time: building
    // maps have no ammo gifts.
    public static void CameraController__Kop_SetGunMode(CameraController self, bool on)
    {
        PlayerNetwork player = self._PlayerNetwork;
        if (on)
        {
            Igor__Kop_GunMode = true;
            Gun[] guns = player.Guns;
            for (int i = 0; i < guns.Length; i++)
            {
                guns[i].bulletsLeft = (int)guns[i].bulletsMax;
                guns[i].Reset();
            }
            self.SetFirstPerson(false);
            if (player.MainWeapon[0] == null && player.MainWeapon[1] == null)
                MainMenu.INSTANCE.ShowWeaponWindow();
            else if (!self.SelectWeapon(1, true))
                self.SelectWeapon(0, true);
            return;
        }
        Igor__Kop_GunMode = false;
        self.SelectWeapon(0, true);
        self.Scope = false;
        self.HandsGunAnimations.gameObject.SetActiveRecursively(false);
        self.SniperScope.renderer.enabled = false;
        self.Damage.renderer.enabled = false;
        self._FirstPersonCamera.animation.Play("idle");
        self._Recoil = Vector3.zero;
        self.CameraRecoil.localRotation = Quaternion.identity;
        self.SetSavedCamera();
    }

    // ---------------- voice chat ----------------
    // Push-to-talk P records the microphone (VoiceChatRecorder, Speex), CameraController.Instance_NewSample
    // sends each 40 ms packet to the others as the RPC SendAudio of the speaker's avatar, and
    // PlayerNetwork.SendAudio plays it through that avatar's VoiceChatPlayer. The original only did this
    // in the battle modes.

    public static int CameraController__Kop_VoiceSent;
    public static int PlayerNetwork__Kop_VoiceReceived;

    // The push-to-talk key, any key or mouse button (KeyCode), saved as PlayerPrefs "kop_voice_key".
    // 0 (KeyCode.None) until read from the prefs.
    public static int VoiceChatRecorder__Kop_Key;

    // On MainMenu, which exists in both scenes (the menu scene has no VoiceChatRecorder).
    public static KeyCode MainMenu__Kop_VoiceKey(MainMenu self)
    {
        if (VoiceChatRecorder__Kop_Key == 0)
            VoiceChatRecorder__Kop_Key = PlayerPrefs.GetInt("kop_voice_key", (int)KeyCode.P);
        return (KeyCode)VoiceChatRecorder__Kop_Key;
    }

    // Unchanged, except that push-to-talk is the chosen key instead of always P.
    public static void VoiceChatRecorder__Update(VoiceChatRecorder self)
    {
        if (VoiceChatRecorder__Kop_Key == 0)
            VoiceChatRecorder__Kop_Key = PlayerPrefs.GetInt("kop_voice_key", (int)KeyCode.P);
        self.pushToTalkKey = ChatClient.isEnabled ? KeyCode.None : (KeyCode)VoiceChatRecorder__Kop_Key;
        if (!self.recording)
            return;
        self.forceTransmit -= Time.deltaTime;
        if (Input.GetKeyUp(self.toggleToTalkKey))
            self.transmitToggled = !self.transmitToggled;
        bool transmit = self.transmitToggled || Input.GetKey(self.pushToTalkKey);
        int position = Microphone.GetPosition(self.Device);
        if (position < self.previousPosition)
        {
            while (self.sampleIndex < self.recordFrequency)
                self.ReadSample(transmit);
            self.sampleIndex = 0;
        }
        self.previousPosition = position;
        while (self.sampleIndex + self.recordSampleSize <= position)
            self.ReadSample(transmit);
    }

    // Settings: waiting for the key that becomes push-to-talk (the button was pressed on frame
    // Kop_VoiceKeyFrame); KeyCode values, read once.
    public static bool MainMenu__Kop_VoiceKeyCapture;
    public static int MainMenu__Kop_VoiceKeyFrame;
    public static System.Array MainMenu__Kop_KeyCodes;

    // Called every frame (UpdateSpecialBonus): the first key or mouse button pressed after the settings
    // button becomes push-to-talk. Esc cancels.
    public static void MainMenu__Kop_CaptureVoiceKey(MainMenu self)
    {
        if (!MainMenu__Kop_VoiceKeyCapture || Time.frameCount <= MainMenu__Kop_VoiceKeyFrame)
            return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MainMenu__Kop_VoiceKeyCapture = false;
            return;
        }
        if (MainMenu__Kop_KeyCodes == null)
            MainMenu__Kop_KeyCodes = System.Enum.GetValues(typeof(KeyCode));
        for (int i = 0; i < MainMenu__Kop_KeyCodes.Length; i++)
        {
            KeyCode key = (KeyCode)MainMenu__Kop_KeyCodes.GetValue(i);
            if (key == KeyCode.None || !Input.GetKeyDown(key))
                continue;
            VoiceChatRecorder__Kop_Key = (int)key;
            PlayerPrefs.SetInt("kop_voice_key", (int)key);
            MainMenu__Kop_VoiceKeyCapture = false;
            Debug.Log("Kop voice: push-to-talk key " + MainMenu__Kop_KeyName(self, key));
            return;
        }
    }

    public static string MainMenu__Kop_KeyName(MainMenu self, KeyCode key)
    {
        if (key == KeyCode.Mouse0) return "Левая кнопка мыши";
        if (key == KeyCode.Mouse1) return "Правая кнопка мыши";
        if (key == KeyCode.Mouse2) return "Средняя кнопка мыши";
        if (key == KeyCode.Mouse3) return "Кнопка мыши 4";
        if (key == KeyCode.Mouse4) return "Кнопка мыши 5";
        if (key == KeyCode.Mouse5) return "Кнопка мыши 6";
        if (key == KeyCode.Mouse6) return "Кнопка мыши 7";
        object name = key;  // boxed: ToString on an enum would be a "constrained." call
        return name.ToString();
    }

    // Unchanged, plus the push-to-talk key: a button that waits for the next key or mouse button.
    public static void MainMenu__DrawOptions(MainMenu self)
    {
        GUI.Box(new Rect(0f, 70f, 340f, 450f), "Настройки");
        GUILayout.BeginArea(new Rect(10f, 70f, 320f, 450f));
        self.optionsListScroll = GUILayout.BeginScrollView(self.optionsListScroll, new GUILayoutOption[0]);
        GUILayout.Space(30f);
        GUILayout.Space(10f);
        GUILayoutOption[] labelWidth = new GUILayoutOption[1];
        labelWidth[0] = GUILayout.MinWidth(150f);
        GUILayoutOption[] controlWidth = new GUILayoutOption[1];
        controlWidth[0] = GUILayout.MaxWidth(150f);
        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
        GUILayout.Label("Чуствительность мыши", (GUIStyle)"PlainText", labelWidth);
        ProfileINI.mouse_sens = GUILayout.HorizontalSlider(ProfileINI.mouse_sens, 1f, 30f, controlWidth);
        GUILayout.EndHorizontal();
        GUILayout.Space(10f);
        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
        GUILayout.Label("Громкость звуков", (GUIStyle)"PlainText", labelWidth);
        ProfileINI.sound_volume = GUILayout.HorizontalSlider(ProfileINI.sound_volume, 0f, 1f, controlWidth);
        GUILayout.EndHorizontal();
        GUILayout.Space(10f);
        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
        GUILayout.Label("Громкость природы", (GUIStyle)"PlainText", labelWidth);
        ProfileINI.ambient_volume = GUILayout.HorizontalSlider(ProfileINI.ambient_volume, 0f, 1f, controlWidth);
        GUILayout.EndHorizontal();
        GUILayout.Space(10f);
        self.options_destroy.Draw();
        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
        GUILayout.Label("Показывать фразы над игроками", (GUIStyle)"PlainText", labelWidth);
        ProfileINI.showBaloons = GUILayout.Toggle(ProfileINI.showBaloons, string.Empty, controlWidth);
        GUILayout.EndHorizontal();
        GUILayout.Space(10f);
        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
        GUILayout.Label("Голосовой чат (держать)", (GUIStyle)"PlainText", labelWidth);
        string current = MainMenu__Kop_VoiceKeyCapture ? "Нажмите кнопку..."
            : MainMenu__Kop_KeyName(self, MainMenu__Kop_VoiceKey(self));
        if (GUILayout.Button(current, controlWidth) && !MainMenu__Kop_VoiceKeyCapture)
        {
            MainMenu__Kop_VoiceKeyCapture = true;
            MainMenu__Kop_VoiceKeyFrame = Time.frameCount;
        }
        GUILayout.EndHorizontal();
        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    // The voice part of UpdateWeapon, for maps without the battle HUD: P shows the speaking emote and,
    // the first time, asks for the microphone (InitMicrophone).
    public static void CameraController__Kop_UpdateVoice(CameraController self)
    {
        VoiceChatRecorder recorder = VoiceChatRecorder.Instance;
        if (recorder == null)
            return;
        KeyCode key = recorder.PushToTalkKey;
        if (!ChatClient.isEnabled && Input.GetKey(key) && Time.time - self._LastEmote > 3f)
        {
            self._LastEmote = Time.time;
            ChatClient.instance.SendEmote("voiceChat");
        }
        if (!self._MicInited && Input.GetKeyDown(key))
            self.StartCoroutine(self.InitMicrophone());
    }

    // Unchanged, except: only the encoded bytes are sent (the pool buffer is SampleSize bytes, a Speex
    // packet a few dozen), and the first packet and every 250th (10 s of speech) are logged.
    public static void CameraController__Instance_NewSample(CameraController self, VoiceChatPacket packet)
    {
        byte[] data = new byte[packet.Length];
        System.Array.Copy(packet.Data, data, packet.Length);
        VoiceChatBytePool.Instance.Return(packet.Data);
        CameraController__Kop_VoiceSent++;
        if (CameraController__Kop_VoiceSent % 250 == 1)
        {
            object compression = packet.Compression;
            Debug.Log("Kop voice: sent " + CameraController__Kop_VoiceSent + " packets (" + compression + ", "
                      + packet.Length + " bytes), microphone " + (VoiceChatRecorder.Instance.Device ?? "default")
                      + " of " + Microphone.devices.Length);
        }
        object[] args = new object[3];
        args[0] = data;
        args[1] = packet.Length;
        args[2] = PhotonNetwork.player.ID;
        self.photonView.RPC("SendAudio", PhotonTargets.Others, args);
    }

    // Unchanged, except: a building map has no TeamBattle and its voice switch (voice is on there), and
    // the "receive Audio" line every 40 ms became a "Kop voice" line every 250 packets.
    public static void PlayerNetwork__SendAudio(PlayerNetwork self, byte[] data, int length, int id)
    {
        PlayerNetwork__Kop_VoiceReceived++;
        bool log = PlayerNetwork__Kop_VoiceReceived % 250 == 1;
        if (TeamBattle.Instance != null && !TeamBattle.Instance.EnableVoiceChat)
        {
            if (log)
                Debug.Log("Kop voice: received " + PlayerNetwork__Kop_VoiceReceived + " packets, voice chat switched off");
            return;
        }
        VoiceChatPacket packet = new VoiceChatPacket();
        packet.Compression = VoiceChatCompression.Speex;
        packet.Data = data;
        packet.Length = length;
        packet.NetworkId = id;
        self._VoiceChatPlayer.OnNewSample(packet);
        if (log)
            Debug.Log("Kop voice: received " + PlayerNetwork__Kop_VoiceReceived + " packets, last from " + self.PlayerName
                      + " (" + length + " bytes), playing " + self._VoiceChatPlayer.audio.isPlaying);
    }

    // Unchanged, except that a building map has no team scores (no TeamBattle there).
    public static void CameraController__UpdateHud(CameraController self)
    {
        Hud hud = bs._Hud;
        hud.BulletText.text = self.CurGun.bullets + "/" + (int)self.CurGun.bulletsLeft;
        hud.Health.text = (int)(bs._WorldGameObject.MainPlayerNode.Life * 100f) + "";
        hud.Shield.text = (int)(bs._WorldGameObject.MainPlayerNode.Shield * 100f) + "";
        if (TeamBattle.Instance != null)
        {
            hud.BlueTeam.text = TeamBattle.Instance.BlueTeamScore + "";
            hud.RedTeam.text = TeamBattle.Instance.RedTeamScore + "";
        }
    }

    // Unchanged, except that a building map, which has no TeamBattle, gets health, ammo and the keys
    // instead of the hidden battle HUD while the player holds the weapons.
    public static void MainMenu__DrawSpecialBonus(MainMenu self)
    {
        if (bs._Igor.enableGuns)
        {
            if (TeamBattle.Instance == null && WorldGameObject.INSTANCE.MainPlayer != null)
                MainMenu__Kop_DrawGunHud(self);
            return;
        }
        if (ProfileINI.GetPurchesValue(StorePurchase.SPEED) != 0)
        {
            GUI.Label(new Rect(Screen.width / 2 - 107, Screen.height - 75, 68f, 68f), self.FastBarImages[self.SpeedEnabled ? 0 : 1]);
            GUI.Label(new Rect(Screen.width / 2 - 107, Screen.height - 75, 26f, 26f), self.FastBarNumbers[0]);
        }
        if (ProfileINI.GetPurchesValue(StorePurchase.FLY) != 0)
        {
            if (self._FlyEnabled)
            {
                GUILayout.BeginArea(new Rect(Screen.width / 2 - 200, Screen.height - 160, 400f, 200f));
                GUILayout.Label("Пробел - взлет", (GUIStyle)"CursorTextSmall", new GUILayoutOption[0]);
                GUILayout.Label("Левый Шифт - приземление", (GUIStyle)"CursorTextSmall", new GUILayoutOption[0]);
                GUILayout.EndArea();
            }
            GUI.Label(new Rect(Screen.width / 2 - 34, Screen.height - 75, 68f, 68f), self.FastBarImages[self.FlyEnabled ? 2 : 3]);
            GUI.Label(new Rect(Screen.width / 2 - 34, Screen.height - 75, 26f, 26f), self.FastBarNumbers[1]);
        }
        if (ProfileINI.GetPurchesValue(StorePurchase.TIME_OF_DAY) != 0 && !App.instance.gameIni.isWatch && Level.Instance.IsAdministrator())
        {
            Texture2D image = self.FastBarImages[TimeOfDay.NormalizedTime < 0.5f ? 4 : 5];
            Rect rect = new Rect(Screen.width / 2 + 39, Screen.height - 75, 68f, 68f);
            if (TimeOfDay.NormalizedTime == 0f || TimeOfDay.NormalizedTime == 1f)
            {
                GUI.Label(rect, image);
            }
            else
            {
                Matrix4x4 matrix = GUI.matrix;
                GUIUtility.RotateAroundPivot(TimeOfDay.NormalizedTime * 360f, new Vector2(Screen.width / 2 + 39 + 34, Screen.height - 75 + 34));
                GUI.Label(rect, image);
                GUI.matrix = matrix;
            }
            GUI.Label(new Rect(Screen.width / 2 + 39, Screen.height - 75, 26f, 26f), self.FastBarNumbers[2]);
        }
    }

    public static void MainMenu__Kop_DrawGunHud(MainMenu self)
    {
        PlayerNode node = WorldGameObject.INSTANCE.MainPlayerNode;
        Gun gun = WorldGameObject.INSTANCE.MainPlayer.GetComponent<CameraController>().CurGun;
        string status = "Здоровье " + (node == null ? 0 : (int)(node.Life * 100f));
        if (!gun.melee)
            status += "   Патроны " + gun.bullets + "/" + (int)gun.bulletsLeft;
        GUILayout.BeginArea(new Rect(Screen.width / 2 - 200, Screen.height - 75, 400f, 75f));
        GUILayout.Label(status, (GUIStyle)"CursorTextSmall", new GUILayoutOption[0]);
        GUILayout.Label("1-4 - оружие, R - перезарядка, G - убрать оружие", (GUIStyle)"CursorTextSmall", new GUILayoutOption[0]);
        GUILayout.EndArea();
    }

    // Unchanged, except that a building map has no TeamBattle: everybody there is an opponent, as in
    // a deathmatch.
    public static void Bullet__Update2(Bullet self, float deltaTime)
    {
        Gun gun = self.gun;
        if (gun.grenade)
            return;
        if ((float)gun.BulletGravity > 0f)
        {
            Transform transform = self.transform;
            transform.forward += Vector3.down * (float)gun.BulletGravity * deltaTime;
        }
        Vector3 step = self.transform.forward * (float)gun.bulletSpeed * deltaTime;
        Debug.DrawRay(self.transform.position, step.normalized);
        RaycastHit hit;
        int mask = (1 << LayerMask.NameToLayer("Terrain")) | (1 << LayerMask.NameToLayer("Players"));
        if (!Physics.Raycast(self.transform.position, step, out hit, step.magnitude + 2f, mask))
        {
            self.transform.position += step;
            return;
        }
        if (gun.playerCC != null && !gun.bazuka)
        {
            PlayerNode node = bs._WorldGameObject.FindPlayerByAvatar(hit.collider.gameObject);
            if (node != null)
            {
                if (TeamBattle.Instance == null || TeamBattle.Instance.IsOpposite(node.networkPlayer.name))
                {
                    float damage = gun.damage;
                    if ((hit.point - node.avatar.GetComponent<PlayerNetwork>().Head.position).magnitude < 0.5f)
                        damage *= (float)gun.HeadHit;
                    object[] args = new object[2];
                    args[0] = step.normalized * 1000f * gun.ragdollForce;
                    args[1] = hit.point;
                    node.avatar.GetComponent<PhotonView>().RPC("OnHit", PhotonTargets.All, args);
                    bs._WorldGameObject.PunchPlayer(node.networkPlayer, step.normalized, damage, gun.ArrayId);
                }
            }
            else
            {
                Object.Instantiate(bs._Igor.concerne, hit.point, Quaternion.LookRotation(hit.normal));
                Vector3 inside = hit.point - hit.normal * 0.1f;
                IntVect target = new IntVect((int)inside.x, (int)inside.z, (int)inside.y);
                BlockType block = WorldData.Instance.GetBlockType(target.X, target.Y, target.Z);
                if (block != BlockType.Air && block != BlockType.Water)
                    bs._WorldGameObject.m_World.Dig2(target, hit.point, (float)gun.damage * 2f);
            }
        }
        AudioClip[] hitSounds = bs._Igor.hitSound;
        AudioSource.PlayClipAtPoint(hitSounds[Random.Range(0, hitSounds.Length)], hit.point,
                                    ProfileINI.sound_volume * ProfileINI.sound_scale);
        self.transform.position = hit.point;
        Object.Destroy(self.gameObject);
    }

    // Unchanged, except: no TeamBattle on a building map (everybody in range is hit), and there only
    // players allowed to build blow holes into the map, as World.Dig2 already demands for bullets.
    public static void Bullet__OnDestroy(Bullet self)
    {
        Gun gun = self.gun;
        if ((!gun.grenade && !gun.bazuka) || gun.playerCC == null)
            return;
        Vector3 pos = self.pos;
        object[] at = new object[1];
        at[0] = pos;
        bs._WorldGameObject.photonView.RPC("Explosion", PhotonTargets.All, at);
        Object[] players = Object.FindObjectsOfType(typeof(PlayerNetwork));
        for (int i = 0; i < players.Length; i++)
        {
            PlayerNetwork player = (PlayerNetwork)players[i];
            if ((player.pos - pos).magnitude >= (float)gun.expDist)
                continue;
            if (TeamBattle.Instance != null && !TeamBattle.Instance.IsOpposite(player.PlayerName)
                && player.GetComponent<CameraController>() == null)
                continue;
            object[] args = new object[2];
            args[0] = (player.pos - pos).normalized * 1000f * gun.ragdollForce;
            args[1] = pos;
            player.GetComponent<PhotonView>().RPC("OnHit", PhotonTargets.All, args);
            bs._WorldGameObject.PunchPlayer(player.photonView.owner, pos - player.pos, gun.damage, gun.ArrayId);
        }
        if (TeamBattle.Instance == null && !Level.Instance.CanBuild)
            return;
        Vector3 center = self.transform.position + (gun.bazuka ? Vector3.zero : Vector3.down);
        int x = (int)center.x, y = (int)center.z, z = (int)center.y;
        WorldData data = WorldData.Instance;
        if (x >= 0 && y >= 0 && z >= 0 && x < data.WidthInBlocks && y < data.HeightInBlocks && z < data.DepthInBlocks)
        {
            object[] blocks = new object[4];
            blocks[0] = x;
            blocks[1] = y;
            blocks[2] = z;
            blocks[3] = 1;
            bs._WorldGameObject.photonView.RPC("RemoveBlocksAt", PhotonTargets.All, blocks);
        }
    }

    // ---------------- Level ----------------

    // Fan version kept only the first line. Rest as in the 2014 client: the host creates the room and
    // owns the map; a joining player resumes the message queue and asks the host for the map.
    public static void Level__Start(Level self)
    {
        self.StartCoroutine(self.InitializeGameTypes());
        GameINI ini = App.instance.gameIni;
        if (ini.isServer)
        {
            WorldGameObject.INSTANCE.StartServer();
            self._Administrators.Add(PhotonNetwork.playerName);
            self._Builders.Add(PhotonNetwork.playerName);
            self._CanBuild = true;
            return;
        }
        PhotonNetwork.isMessageQueueRunning = true;
        WorldGameObject__Kop_RegisterPlayer(WorldGameObject.INSTANCE, true);
        self.photonView.RPC("GetActualData", PhotonTargets.MasterClient, new object[0]);
    }

    // ---------------- sounds ----------------

    // Clip downloads still in progress (WWW, null once handled) and, at the same index, where each clip
    // goes: effect * 100 + sample. Owner is the loader that started them; FramesWaited counts frames
    // spent on clips whose data has arrived but that are not playable yet.
    public static ArrayList ResourceLoader__Kop_Downloads;
    public static ArrayList ResourceLoader__Kop_Slots;
    public static ArrayList ResourceLoader__Kop_Clips;
    public static ResourceLoader ResourceLoader__Kop_Owner;
    public static int ResourceLoader__Kop_FramesWaited;
    // The game never had an explosion sound for dynamite and the bomb: Explode_1..4.wav (Minecraft's
    // classic TNT sound) are loaded next to the eSounds clips, into slots -1..-4.
    public static AudioClip[] ResourceLoader__Kop_Explode;

    // Fan version: empty (it used to load Sounds3.unity3d, which no longer exists anywhere). The
    // clips are served one by one as WAV from <game folder>/sounds/<name>.wav instead; names follow
    // eSounds exactly as LoadSoundsBundle did ("FootDirt_4" -> FootDirt_1..FootDirt_4, "Jump_1" -> Jump).
    // Only the downloads start here; Update turns them into clips once they are complete (a clip taken
    // from an unfinished WWW stayed silent) and then sets SoundsBundleLoaded, which the game checks
    // before playing anything.
    public static void ResourceLoader__Start(ResourceLoader self)
    {
        if (ResourceLoader.I != null && ResourceLoader.I != self && ResourceLoader.I.SoundsBundleLoaded)
            return;
        string url = Application.absoluteURL;
        int query = url.IndexOf('?');
        if (query >= 0)
            url = url.Substring(0, query);
        string soundsUrl = url.Substring(0, url.LastIndexOf('/') + 1) + "sounds/";

        System.Array values = System.Enum.GetValues(typeof(ResourceLoader.eSounds));
        self.mSoundEffects = new ResourceLoader.SoundEffect[values.Length];
        ResourceLoader__Kop_Downloads = new ArrayList();
        ResourceLoader__Kop_Slots = new ArrayList();
        ResourceLoader__Kop_Clips = new ArrayList();
        for (int e = 0; e < values.Length; e++)
        {
            object value = values.GetValue(e);
            string[] parts = value.ToString().Split('_');
            int count = int.Parse(parts[1]);
            ResourceLoader.SoundEffect effect = new ResourceLoader.SoundEffect();
            effect.audio_clips = new AudioClip[count];
            for (int i = 1; i <= count; i++)
            {
                string file = count == 1 ? parts[0] : parts[0] + "_" + i;
                ResourceLoader__Kop_Downloads.Add(new WWW(soundsUrl + file + ".wav"));
                ResourceLoader__Kop_Slots.Add(e * 100 + i - 1);
                ResourceLoader__Kop_Clips.Add(null);
            }
            self.mSoundEffects[e] = effect;
        }
        ResourceLoader__Kop_Explode = new AudioClip[4];
        for (int i = 1; i <= ResourceLoader__Kop_Explode.Length; i++)
        {
            ResourceLoader__Kop_Downloads.Add(new WWW(soundsUrl + "Explode_" + i + ".wav"));
            ResourceLoader__Kop_Slots.Add(-i);
            ResourceLoader__Kop_Clips.Add(null);
        }
        ResourceLoader__Kop_Owner = self;
        ResourceLoader__Kop_FramesWaited = 0;
        self.SoundsBundleLoaded = false;
        Debug.Log("Kop sounds: loading " + ResourceLoader__Kop_Downloads.Count + " clips from " + soundsUrl);
    }

    // Fan version: empty (so was the original). Finishes the downloads Start began.
    public static void ResourceLoader__Update(ResourceLoader self)
    {
        ArrayList downloads = ResourceLoader__Kop_Downloads;
        if (downloads == null || ResourceLoader__Kop_Owner != self)
            return;
        bool downloading = false, waiting = false;
        for (int k = 0; k < downloads.Count; k++)
        {
            WWW www = (WWW)downloads[k];
            if (www == null)
                continue;
            if (!www.isDone)
            {
                downloading = true;
                continue;
            }
            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.Log("Kop sounds: " + www.url + " failed: " + www.error);
                downloads[k] = null;
                continue;
            }
            int slot = (int)ResourceLoader__Kop_Slots[k];
            AudioClip clip = (AudioClip)ResourceLoader__Kop_Clips[k];
            if (clip == null)
            {
                // Ambient loops play through the camera's AudioSource as 2D sounds, effects are 3D.
                clip = www.GetAudioClip(!www.url.Contains("/Loop"));
                ResourceLoader__Kop_Clips[k] = clip;
            }
            if (clip.isReadyToPlay)
            {
                if (slot < 0)
                    ResourceLoader__Kop_Explode[-slot - 1] = clip;
                else
                    self.mSoundEffects[slot / 100].audio_clips[slot % 100] = clip;
                downloads[k] = null;
            }
            else
            {
                waiting = true;
            }
        }
        if (downloading)
            return;
        // Data that never becomes playable (a broken file) must not keep the game silent for good.
        if (waiting && ++ResourceLoader__Kop_FramesWaited < 300)
            return;

        int loaded = 0, total = 0;
        for (int e = 0; e < self.mSoundEffects.Length; e++)
        {
            AudioClip[] clips = self.mSoundEffects[e].audio_clips;
            for (int i = 0; i < clips.Length; i++)
            {
                total++;
                if (clips[i] != null)
                    loaded++;
            }
        }
        for (int i = 0; i < ResourceLoader__Kop_Explode.Length; i++)
        {
            total++;
            if (ResourceLoader__Kop_Explode[i] != null)
                loaded++;
        }
        for (int k = 0; k < downloads.Count; k++)
        {
            WWW www = (WWW)downloads[k];
            if (www != null)
                Debug.Log("Kop sounds: " + www.url + " is not a playable clip");
        }
        ResourceLoader__Kop_Downloads = null;
        ResourceLoader__Kop_Slots = null;
        ResourceLoader__Kop_Clips = null;
        self.SoundsBundleLoaded = true;
        Debug.Log("Kop sounds: " + loaded + " of " + total + " clips ready; audio listeners: "
                  + Object.FindObjectsOfType(typeof(AudioListener)).Length + ", volume " + AudioListener.volume);
    }

    // One of the Explode clips at pos. Its own AudioSource instead of PlayClipAtPoint, so that a blast
    // stays at full volume for 10 m (PlayClipAtPoint fades from 1 m on, like a footstep).
    public static void ResourceLoader__Kop_PlayExplosion(ResourceLoader self, Vector3 pos)
    {
        AudioClip[] clips = ResourceLoader__Kop_Explode;
        if (clips == null)
            return;
        AudioClip clip = clips[Random.Range(0, clips.Length)];
        if (clip == null)
            return;
        GameObject sound = new GameObject("Kop_Explosion");
        sound.transform.position = pos;
        AudioSource source = sound.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = ProfileINI.sound_volume * ProfileINI.sound_scale;
        source.minDistance = 10f;
        source.Play();
        Object.Destroy(sound, clip.length + 0.5f);
    }

    // ---------------- explosives ----------------

    // Unchanged, except for the explosion sound.
    public static void TNT__Destruction(TNT self)
    {
        if (!self.is_start_activate)
            return;
        Vector3 pos = self.gameObject.transform.position;
        if (!WorldGameObject.INSTANCE.IsWorldGenerated)
        {
            self.DestroyAreaAt((int)pos.x, (int)pos.z, (int)pos.y, 4, false);
            return;
        }
        Object.Instantiate(self.DetonateEffect, pos, Quaternion.identity);
        if (ResourceLoader.I != null)
            ResourceLoader__Kop_PlayExplosion(ResourceLoader.I, pos);
        self.DestroyAreaAt((int)pos.x, (int)pos.z, (int)pos.y, 4, true);
    }

    public static void AviaBomb__Destruction(AviaBomb self)
    {
        if (!self.is_start_activate)
            return;
        Vector3 pos = self.gameObject.transform.position;
        if (!WorldGameObject.INSTANCE.IsWorldGenerated)
        {
            self.DestroyAreaAt((int)pos.x, (int)pos.z, (int)pos.y, 8, false);
            return;
        }
        Object.Instantiate(self.DetonateEffect, pos, Quaternion.identity);
        if (ResourceLoader.I != null)
            ResourceLoader__Kop_PlayExplosion(ResourceLoader.I, pos);
        self.DestroyAreaAt((int)pos.x, (int)pos.z, (int)pos.y, 8, true);
    }

    // ---------------- player ----------------

    // Fan version only called Reset(); _CameraRecoilOldPos stayed zero and the saved camera mode
    // was never applied.
    public static void CameraController__Start(CameraController self)
    {
        self._CameraRecoilOldPos = self.CameraRecoil.localPosition;
        self.SetSavedCamera();
        self.Reset();
    }

    // Fan version threw NullReferenceException for guns without a third-person model.
    public static void Gun__EnableVeapon2(Gun self, bool state)
    {
        self.ShowWep(state);
        if (self.weapon != null)
            self.weapon.SetActiveRecursively(false);
    }

    // Fan version: empty, so other players' avatars never moved. 2014 interpolation.
    public static void InterpolateTransform__Update(InterpolateTransform self)
    {
        if (self.m_TimestampCount == 0)
            return;
        double interpolationTime = PhotonNetwork.time - self.interpolationBackTime;
        if (self.m_BufferedState[0].timestamp > interpolationTime)
        {
            for (int i = 0; i < self.m_TimestampCount; i++)
            {
                if (self.m_BufferedState[i].timestamp <= interpolationTime || i == self.m_TimestampCount - 1)
                {
                    InterpolateTransform.State rhs = self.m_BufferedState[Mathf.Max(i - 1, 0)];
                    InterpolateTransform.State lhs = self.m_BufferedState[i];
                    double length = rhs.timestamp - lhs.timestamp;
                    float t = 0f;
                    if (length > 0.0001)
                        t = (float)((interpolationTime - lhs.timestamp) / length);
                    self.transform.localPosition = Vector3.Lerp(lhs.pos, rhs.pos, t);
                    self.transform.localRotation = Quaternion.Slerp(lhs.rot, rhs.rot, t);
                    return;
                }
            }
        }
        else
        {
            InterpolateTransform.State latest = self.m_BufferedState[0];
            self.transform.localPosition = latest.pos;
            self.transform.localRotation = latest.rot;
        }
    }

    // Fan version: empty. 2014: smooth remote objects towards the last received position.
    public static void PhotonNetworkScript34__Update(PhotonNetworkScript34 self)
    {
        if (self.photonView.isMine)
            return;
        self.transform.position = Vector3.Lerp(self.transform.position, self.correctPlayerPos, Time.deltaTime * 5f);
        self.transform.rotation = Quaternion.Lerp(self.transform.rotation, self.correctPlayerRot, Time.deltaTime * 5f);
    }
}
