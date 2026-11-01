using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ExitGames.Client.Photon;
using Photon;
using UnityEngine;

public class WorldGameObject : MonoBehaviour
{
	protected class FlyMessage
	{
		public string Text = string.Empty;

		public PhotonView View;

		public Vector3 MoveDir = Vector3.zero;

		public Vector3 MoveOffset = Vector3.zero;

		public float EndTime;

		public Color TextColor = Color.white;

		public FlyMessage()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		}
	}

	public const float ContinuousDamageHitTime = 1f;

	private const int WaterBooblesMaxCount = 10;

	private const float UnderwaterMaxTime = 20f;

	internal BlockKind CurrentKind;

	internal CommonBlockKind CurrentCommonKind;

	public static WorldGameObject Instance;

	public BlockParametrs[] BlockParametrs;

	public EnityParametrs[] EnityParametrs;

	public BlockLifes[] BlockLifes;

	public GameObject SelectelPrefab;

	private GameObject _Selectel;

	public GameObject CubeDamagePrefab;

	internal GameObject CubeDamage;

	public GameObject Sparks;

	public GameObject SparksCubes1;

	public GameObject SparksCubes2;

	private WorldData _WorldData;

	private World _World;

	internal bool IsWorldGenerated;

	private List<IDecoration> _WorldDecorations = new List<IDecoration>();

	private Material _DefaultSkyBox;

	public Transform Chunk_Prefab;

	public Transform WaterPrefab;

	public Transform GlassPrefab;

	public GameObject Player_Prefab;

	internal GameObject MainPlayer;

	internal Camera MainPlayerEyes;

	internal PlayerNode MainPlayerNode;

	internal bool MainPlayerDead;

	private GameObject _WorldCamera;

	public Texture2D[] World_Textures;

	internal EntityBase EntityPreview;

	internal bool EntityPreviewIsTaked;

	internal EntityBase EntityPreviewTaked;

	internal EntityBase EntityUnderCursor;

	internal LevelDelta LevelDeltaLoading = new LevelDelta();

	internal Texture2D WorldTextureAtlas;

	private ChunkGameObject[,,,] _ChunkObjects;

	private Transform _ChunksParent;

	internal List<PlayerNode> PlayerList = new List<PlayerNode>();

	internal int KubokCountInGame;

	internal int KubokFindInGame;

	internal bool FindAllKubok;

	public GameObject HitEffect;

	public GameObject ZombieEffect;

	private int _CurZipedLevelChunk;

	private int _ZipedLevelChunkCount;

	private float _ZipedChunkReceiveTime;

	internal bool MakeContinuousDamage;

	private bool _ContinuousDamageEnabled;

	private float _ContinuousDamageLastHitTime;

	private bool _OnIce;

	private bool _InWater;

	private bool _FaceInWater;

	private float _FaceInWaterTime;

	private float _WaterBooblesCount;

	[NonSerialized]
	public float last_ladder_delta;

	[NonSerialized]
	public bool player_on_ladder;

	internal BlockType CurrentBlock = BlockType.WoodPlank;

	internal EntityType CurrentBlockEntity;

	private static int _CurBlockSibling;

	public bool bSkipSoundRpc;

	protected List<FlyMessage> _FlyMessages = new List<FlyMessage>();

	protected GUIStyle _FlyMessageStyle;

	private SecuredValue<double> _SHErrorRate = 1.15;

	private SecuredValue<DateTime> _SHOldDT;

	private SecuredValue<long> _SHOldTick;

	private SecuredValue<int> _SHErrorCount;

	private int _CreateGiftsInterval = 10;

	private bool _CheckCheating;

	private bool _CheatingVoted;

	private bool _CheatingVotedYes;

	private DateTime _CheatingVotingTime;

	private string _CheatingPlayerName;

	public ChunkGameObject[,,,] ChunkObjects => _ChunkObjects;

	public WorldData WorldData => _WorldData;

	public World World => _World;

	public int FindPlayerInd(PhotonPlayer player)
	{
		for (int i = 0; i < PlayerList.Count; i++)
		{
			if (PlayerList[i].NetPlayer == player)
			{
				return i;
			}
		}
		return -1;
	}

	public PlayerNode FindPlayer(PhotonPlayer player)
	{
		for (int i = 0; i < PlayerList.Count; i++)
		{
			if (PlayerList[i].NetPlayer == player)
			{
				return PlayerList[i];
			}
		}
		return null;
	}

	public PlayerNode FindPlayerByName(string name)
	{
		for (int i = 0; i < PlayerList.Count; i++)
		{
			if (PlayerList[i].Name == name)
			{
				return PlayerList[i];
			}
		}
		return null;
	}

	public PlayerNode FindPlayerByAvatar(GameObject avatar)
	{
		for (int i = 0; i < PlayerList.Count; i++)
		{
			if ((Object)(object)PlayerList[i].Avatar == (Object)(object)avatar)
			{
				return PlayerList[i];
			}
		}
		return null;
	}

	public PlayerNode FindPlayerByViewerID(string viewerID)
	{
		for (int i = 0; i < PlayerList.Count; i++)
		{
			if (PlayerList[i].ViewerID == viewerID)
			{
				return PlayerList[i];
			}
		}
		return null;
	}

	public EntityBase FindEntityByID(int id)
	{
		foreach (EntityBase entity in EntityBase.Entities)
		{
			if (((Component)entity).GetComponent<PhotonView>().viewID == id)
			{
				return entity;
			}
		}
		return null;
	}

	public void ClearEntities()
	{
		foreach (EntityBase entity in EntityBase.Entities)
		{
			Object.Destroy((Object)(object)((Component)entity).gameObject);
		}
		EntityBase.Entities.Clear();
	}

	public void Awake()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected Obj, but got Unknown
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected Obj, but got Unknown
		Instance = this;
		MasterServer.ipAddress = "67.225.180.24";
		MasterServer.port = 23466;
		_Selectel = (GameObject)Object.Instantiate((Object)(object)SelectelPrefab);
		CubeDamage = (GameObject)Object.Instantiate((Object)(object)CubeDamagePrefab);
		((Renderer)CubeDamage.GetComponent<MeshRenderer>()).enabled = false;
		_WorldCamera = GameObject.Find("WorldCamera");
		_DefaultSkyBox = RenderSettings.skybox;
	}

	private void Start()
	{
		PhotonNetwork.OnEventCall = (PhotonNetwork.EventCallback)Delegate.Combine(PhotonNetwork.OnEventCall, new PhotonNetwork.EventCallback(OnPhotonEvent));
		Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Players"), LayerMask.NameToLayer("SmallDecor"));
		Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Players"), LayerMask.NameToLayer("ParticleCubesDestroy"));
		Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Players"), LayerMask.NameToLayer("StorePreview"));
		StartSpeedHackDetection();
		((MonoBehaviour)this).StartCoroutine(CheckTimePurchasesProcess());
	}

	private void OnDestroy()
	{
		PhotonNetwork.OnEventCall = null;
	}

	public void StartServer()
	{
		App.Instance.Settings.ServerRating = 10;
		int mapPopulation = App.Instance.Settings.mapPopulation;
		bool isOnline = App.Instance.Settings.isOnline;
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable.Add("player_id", App.Instance.Settings.playerID);
		hashtable.Add("slot_id", App.Instance.Settings.slotID);
		hashtable.Add("public_id", App.Instance.Settings.publicMapID);
		hashtable.Add("password", App.Instance.Settings.Password);
		hashtable.Add("map_name", App.Instance.Settings.server_name);
		hashtable.Add("map_about", App.Instance.Settings.server_about);
		hashtable.Add("is_watch", App.Instance.Settings.isWatch);
		hashtable.Add("game_type", (int)App.Instance.Settings.gameType);
		string[] array = new string[hashtable.Keys.Count];
		hashtable.Keys.CopyTo(array, 0);
		RoomOptions roomOptions = new RoomOptions();
		roomOptions.isVisible = isOnline;
		roomOptions.isOpen = true;
		roomOptions.maxPlayers = mapPopulation;
		roomOptions.cleanupCacheOnLeave = PhotonNetwork.autoCleanUpPlayerObjects;
		roomOptions.customRoomProperties = hashtable;
		roomOptions.customRoomPropertiesForLobby = array;
		RoomOptions roomOptions2 = roomOptions;
		PhotonNetwork.CreateRoom(null, roomOptions2, TypedLobby.Default);
	}

	private void OnPhotonEvent(byte eventCode, object content, int senderId)
	{
		switch (eventCode)
		{
		case 2:
			if (content is string)
			{
				Chat.SendInfo(Localize.GetText((string)content), forAll: false);
			}
			else if (content is string[])
			{
				string[] array = (string[])content;
				string text = string.Empty;
				for (int i = 0; i < array.Length; i++)
				{
					text += Localize.GetText(array[i]);
				}
				Chat.SendInfo(text, forAll: false);
			}
			break;
		case 3:
			_CheckCheating = false;
			KGUI.SetControlText("hud.txt_cheating", string.Empty);
			break;
		}
	}

	[RPC]
	public void ExitGame()
	{
		PhotonNetwork.LeaveRoom();
		MainMenu.Instance.ShowLoading("LOADING_LEAVING", string.Empty);
		Application.LoadLevel("Menu");
	}

	[RPC]
	public void ServerClosed(PhotonMessageInfo info)
	{
		if (!((Object)(object)TeamBattle.Instance != (Object)null) && (Level.Instance.IsAdministrator(info.sender.name) || Level.Instance.IsModerator(info.sender.name)))
		{
			ProfileINI.server_was_closed = true;
			Application.LoadLevel("Menu");
			PhotonNetwork.LeaveRoom();
		}
	}

	public void CloseServerAndExit()
	{
		photonView.RPC("ServerClosed", PhotonTargets.Others);
		ExitGame();
	}

	public void PlayerCanBuild(PlayerNode player, bool canBuild)
	{
		if (player != null)
		{
			if (canBuild)
			{
				Level.Instance.AddBuilder(player.Name);
				Chat.SendInfo(player.Name + Localize.GetText("BUILD_PLAYER_CAN_BUILD"), forAll: true);
			}
			else
			{
				Level.Instance.RemoveBuilder(player.Name);
				Chat.SendWarning(Localize.GetText("BUILD_PLAYER") + player.Name + Localize.GetText("BUILD_PLAYER_CANT_BUILD"), forAll: true);
			}
		}
	}

	public void BanPlayer(PlayerNode player)
	{
		if (player == null)
		{
			return;
		}
		List<string> list = new List<string>();
		list.Add("132371665");
		list.Add("228418634");
		List<string> list2 = list;
		if (list2.Contains(player.ViewerID))
		{
			Chat.SendWarning(player.Name + Localize.GetText("PLAYER_CANT_BAN"), forAll: true);
			return;
		}
		Level.Instance.AddBanned(player.Name);
		photonView.RPC("TakeBan", player.NetPlayer);
		Chat.SendWarning(player.Name + Localize.GetText("PLAYER_BANNED"), forAll: true);
		int num = FindPlayerInd(player.NetPlayer);
		if (num != -1)
		{
			PlayerList.RemoveAt(num);
		}
		MainMenu.Instance.RefreshTabMenu();
	}

	public void UnbanPlayer(string playerName)
	{
		if (Level.Instance.IsBanned(playerName))
		{
			Level.Instance.RemoveBanned(playerName);
			Chat.SendInfo(playerName + Localize.GetText("PLAYER_UNBANNED"), forAll: true);
			MainMenu.Instance.RefreshTabMenu();
		}
	}

	[RPC]
	public void TakeBan(PhotonMessageInfo info)
	{
		if (!((Object)(object)TeamBattle.Instance != (Object)null) && (Level.Instance.IsAdministrator(info.sender.name) || Level.Instance.IsModerator(info.sender.name)) && !Level.Instance.IsAdministrator())
		{
			ProfileINI.was_banned = true;
			Application.LoadLevel("Menu");
			PhotonNetwork.LeaveRoom();
		}
	}

	public void OnPhotonPlayerDisconnected(PhotonPlayer player)
	{
		Debug.Log((object)"OnPhotonPlayerDisconnected");
		int num = FindPlayerInd(player);
		if (num != -1)
		{
			Chat.SendInfo(player.name + Localize.GetText("PLAYER_LEAVE"), forAll: false);
			PlayerList.RemoveAt(num);
			MainMenu.Instance.RefreshTabMenu();
		}
	}

	[RPC]
	public void SendPlayerProperties(string name, int skin, string viewer_id, bool want_level, PhotonPlayer player, int level, PhotonMessageInfo info)
	{
		if (Level.Instance.IsBanned(name))
		{
			photonView.RPC("TakeBan", info.sender);
			Chat.SendInfo(name + Localize.GetText("PLAYER_BANNED_ENTER"), forAll: false);
			return;
		}
		PlayerNode playerNode = new PlayerNode();
		playerNode.Name = name;
		playerNode.ViewerID = viewer_id;
		playerNode.NetPlayer = player;
		playerNode.Skin = skin;
		playerNode.Level = level;
		PlayerList.Add(playerNode);
		MainMenu.Instance.RefreshTabMenu();
		if (player.isLocal)
		{
			MainPlayerNode = playerNode;
		}
		if (PhotonNetwork.isMasterClient && want_level)
		{
			((MonoBehaviour)this).StartCoroutine(SendLevel(info.sender));
		}
		if (Level.Instance.IsAdministrator() && !Level.Instance.IsBuilder(name) && (App.Instance.Settings.gameType != GameINI.GameType.BUILDING || ProfileINI.newgamersislook))
		{
			Level.Instance.AddBuilder(name);
		}
	}

	[RPC]
	public void SetPlayerLevel(PhotonPlayer player, int level)
	{
		PlayerNode playerNode = FindPlayer(player);
		playerNode.Level = level;
		playerNode.Avatar.GetComponent<Nickname>().SetLevel(level);
		MainMenu.Instance.RefreshTabMenu();
	}

	[RPC]
	private void ChangePlayerSkin(string playerName, int skin)
	{
		PlayerNode playerNode = FindPlayerByName(playerName);
		playerNode.Skin = skin;
		if ((Object)(object)playerNode.Avatar != (Object)null)
		{
			if (playerNode.Avatar.GetComponent<SkinManager>().CurSkin != -1)
			{
				playerNode.Avatar.GetComponent<SkinManager>().SetSkin(skin);
			}
			playerNode.Avatar.GetComponent<PlayerNetwork>().RefreshNativeWeapon(skin);
		}
	}

	public IEnumerator SendLevel(PhotonPlayer player)
	{
		while (!IsWorldGenerated || !_World.zipedLevel.finished)
		{
			yield return 0;
		}
		GameINI gameIni = App.Instance.Settings;
		photonView.RPC("StartLoadLevel", player, _WorldData.ChunkBlockWidth, _WorldData.ChunkBlockHeight, _WorldData.ChunkBlockDepth, _WorldData.ChunksWide, _WorldData.ChunksHigh, _WorldData.ChunksDeep, (byte)gameIni.mapType, _World.zipedLevel.ziped.Length, World.ZipLevelThread.chunk_size, (int)gameIni.gameType, gameIni.destroyable, gameIni.gifts);
	}

	[RPC]
	private void StartLoadLevel(int chunkBlockWidth, int chunkBlockHeight, int chunkBlockDepth, int chunkWide, int chunkHigh, int chunkDeep, byte mapType, int zippedLevelSize, int zippedLevelChunkSize, int gametype, bool destroyable, bool gifts)
	{
		GameINI settings = App.Instance.Settings;
		settings.mapType = (GameINI.MapType)mapType;
		settings.gameType = (GameINI.GameType)gametype;
		settings.destroyable = destroyable;
		settings.gifts = gifts;
		InitWorldStructures(chunkBlockWidth, chunkBlockHeight, chunkBlockDepth, chunkWide, chunkHigh, chunkDeep);
		_CurZipedLevelChunk = 0;
		_ZipedLevelChunkCount = 0;
		_World.zipedLevel.ziped = new byte[zippedLevelSize];
		_World.zipedLevel.finished = false;
		World.ZipLevelThread.chunk_size = zippedLevelChunkSize;
		_ZipedChunkReceiveTime = Time.time;
		photonView.RPC("LoadChunk", PhotonTargets.MasterClient);
		MainMenu.Instance.ShowLoading("LOADING_LOADING", string.Empty);
	}

	[RPC]
	public void LoadChunk(PhotonMessageInfo info)
	{
		((MonoBehaviour)this).StartCoroutine(SendLevelData(info.sender));
	}

	public IEnumerator SendLevelData(PhotonPlayer player)
	{
		List<byte[]> level_list = _World.zipedLevel.GetCurrentZipedLevel();
		List<byte[]> delta_list = _World.CurLevelDelta.GetCurrentLevelDelta();
		photonView.RPC("SendLevelPartSizes", player, level_list.Count, delta_list.Count);
		yield return (object)new WaitForSeconds(2f);
		for (int i = 0; i < level_list.Count; i++)
		{
			photonView.RPC("R", player, 0, i, level_list[i]);
			yield return (object)new WaitForSeconds(0.01f);
		}
		for (int j = 0; j < delta_list.Count; j++)
		{
			photonView.RPC("R", player, 1, j, delta_list[j]);
			yield return (object)new WaitForSeconds(0.01f);
		}
	}

	[RPC]
	private void SendLevelPartSizes(int levelSize, int deltaSize)
	{
		_ZipedLevelChunkCount = levelSize + deltaSize;
		_ZipedChunkReceiveTime = Time.time;
	}

	[RPC]
	private void R(int type, int place, byte[] zipBlocks)
	{
		_ZipedChunkReceiveTime = Time.time;
		if (type == 0)
		{
			zipBlocks.CopyTo(_World.zipedLevel.ziped, place * World.ZipLevelThread.chunk_size);
		}
		if (type == 1)
		{
			_World.CurLevelDelta.Add(place * 1500, zipBlocks);
		}
		_CurZipedLevelChunk++;
		MainMenu.Instance.SetLoadingText("LOADING_LOADING", " " + _CurZipedLevelChunk * 100 / _ZipedLevelChunkCount + "%");
		if (_ZipedLevelChunkCount == 0 || _CurZipedLevelChunk < _ZipedLevelChunkCount || IsWorldGenerated)
		{
			return;
		}
		_World.zipedLevel.finished = true;
		_World.UnzippedInitialLevel = Utils.UnzipByte(_World.zipedLevel.ziped, 2 * _WorldData.ChunksWide * _WorldData.ChunksHigh * _WorldData.ChunksDeep * WorldData.Instance.m_ChunkBufferLength);
		MemoryStream input = new MemoryStream(_World.UnzippedInitialLevel);
		BinaryReader binaryReader = new BinaryReader(input);
		for (int i = 0; i < _WorldData.ChunksWide; i++)
		{
			for (int j = 0; j < _WorldData.ChunksHigh; j++)
			{
				for (int k = 0; k < _WorldData.ChunksDeep; k++)
				{
					byte[] blocksBuffer = binaryReader.ReadBytes(_WorldData.m_ChunkBufferLength);
					_WorldData.Chunks[i, j, k].SetBlocksBuffer(blocksBuffer);
					_WorldData.Chunks[i, j, k].NeedsRegeneration = false;
					byte[] blocksKindBuffer = binaryReader.ReadBytes(_WorldData.m_ChunkBufferLength);
					_WorldData.Chunks[i, j, k].SetBlocksKindBuffer(blocksKindBuffer);
				}
			}
		}
		_World.CurLevelDelta.MoveToLevel();
		LevelDeltaLoading.MoveToLevel();
		((MonoBehaviour)this).StartCoroutine(OnLevelRecieve());
	}

	public IEnumerator OnLevelRecieve()
	{
		_ZipedChunkReceiveTime = 0f;
		yield return ((MonoBehaviour)this).StartCoroutine(_World.LoadWorldProcess());
		Chat.SendInfo(PhotonNetwork.playerName + Localize.GetText("PLAYER_ENTER"), forAll: true);
	}

	public void RestartLevel()
	{
		if (!IsWorldGenerated)
		{
			ExitGame();
			return;
		}
		_World.CurLevelDelta = new LevelDelta();
		LevelDeltaLoading = new LevelDelta();
		if (_World.UnzippedInitialLevel != null)
		{
			MemoryStream input = new MemoryStream(_World.UnzippedInitialLevel);
			BinaryReader binaryReader = new BinaryReader(input);
			for (int i = 0; i < _WorldData.ChunksWide; i++)
			{
				for (int j = 0; j < _WorldData.ChunksHigh; j++)
				{
					for (int k = 0; k < _WorldData.ChunksDeep; k++)
					{
						byte[] blocksBuffer = binaryReader.ReadBytes(_WorldData.m_ChunkBufferLength);
						_WorldData.Chunks[i, j, k].SetBlocksBuffer(blocksBuffer);
						_WorldData.Chunks[i, j, k].NeedsRegeneration = true;
						byte[] blocksKindBuffer = binaryReader.ReadBytes(_WorldData.m_ChunkBufferLength);
						_WorldData.Chunks[i, j, k].SetBlocksKindBuffer(blocksKindBuffer);
					}
				}
			}
		}
		PChunkList allChunks = _WorldData.AllChunks;
		World.Instance.Lighting.InitializeLighting();
		for (int l = 0; l < allChunks.Count; l++)
		{
			World.Instance.Lighting.InitializeLightingChunk(allChunks[l]);
		}
		allChunks.Release();
	}

	private void OnFailedToConnect_OBSELETE(NetworkConnectionError error)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		Debug.Log((object)("Could not connect to server: " + error));
	}

	private void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		if ((int)msEvent == 3)
		{
			Debug.Log((object)"Server registered");
		}
	}

	public IEnumerator LoadWorldFromStorage(string playerID, int slotID)
	{
		MainMenu.Instance.SetLoadingText("LOADING_LOADING", string.Empty);
		yield return 0;
		yield return ((MonoBehaviour)this).StartCoroutine(LoadWorldFromServer(playerID, slotID));
		if (_World == null)
		{
			ExitGame();
			yield break;
		}
		_World.ZipLevel();
		yield return ((MonoBehaviour)this).StartCoroutine(_World.LoadWorldProcess());
	}

	public IEnumerator LoadWorldFromServer(string playerID, int slotID)
	{
		Debug.Log((object)("Load level from slot " + slotID));
		string mapPath;
		if (slotID != -1)
		{
			string subDirName = VkontakteAPI.MD52(playerID).Substring(0, 2);
			string fileName = playerID + "_" + slotID + ".map";
			Random random = new Random();
			mapPath = App.Instance.GetCurSavesUrl() + "worlds/" + subDirName + "/" + fileName + "?random=" + random.Next();
		}
		else
		{
			mapPath = playerID;
		}
		WWW level = new WWW(mapPath);
		yield return level;
		if (level.error != null)
		{
			Debug.Log((object)("Map loaded with error! " + level.error));
			yield break;
		}
		byte[] buffer = level.bytes;
		MemoryStream ms = new MemoryStream(buffer);
		BinaryReader br = new BinaryReader(ms);
		int unpackSize = br.ReadInt32();
		byte[] zipBuffer = br.ReadBytes(buffer.Length - 4);
		byte[] unzipBuffer = Utils.UnzipByte(zipBuffer, unpackSize);
		ms = new MemoryStream(unzipBuffer);
		br = new BinaryReader(ms);
		int format = br.ReadByte();
		if (format > 5)
		{
			Debug.Log((object)"Wrong format!");
			yield break;
		}
		int chunkBlockWidth = br.ReadInt32();
		int chunkBlockHeight = br.ReadInt32();
		int chunkBlockDepth = br.ReadInt32();
		int chunksWide = br.ReadInt32();
		int chunksHigh = br.ReadInt32();
		int chunksDeep = br.ReadInt32();
		if (format == 1)
		{
			App.Instance.Settings.mapType = GameINI.MapType.STANDART;
		}
		string viewerID = "0";
		if (format >= 2)
		{
			App.Instance.Settings.mapType = (GameINI.MapType)br.ReadByte();
			viewerID = br.ReadString();
		}
		if (format >= 3)
		{
			br.ReadByte();
		}
		bool isLightsBuf = false;
		if (format < 4)
		{
			int lenToEnd = (int)(br.BaseStream.Length - br.BaseStream.Position);
			int blocksBufLen = chunkBlockWidth * chunkBlockHeight * chunkBlockDepth * chunksWide * chunksHigh * chunksDeep * 2;
			if (lenToEnd > blocksBufLen + 4)
			{
				br.BaseStream.Seek(blocksBufLen, SeekOrigin.Current);
				int entityBufLen = br.ReadInt32();
				if (blocksBufLen + 4 + entityBufLen == lenToEnd)
				{
					isLightsBuf = true;
				}
				br.BaseStream.Seek(-(4 + blocksBufLen), SeekOrigin.Current);
			}
		}
		MemoryStream initialLevel = new MemoryStream();
		if (chunkBlockDepth != 128 || chunksDeep != 1)
		{
			InitWorldStructures(chunkBlockWidth, chunkBlockHeight, chunkBlockDepth, chunksWide, chunksHigh, chunksDeep);
			PChunkList allChunks = _WorldData.AllChunks;
			foreach (Chunk chunk in allChunks)
			{
				byte[] buf = br.ReadBytes(_WorldData.m_ChunkBufferLength);
				chunk.SetBlocksBuffer(buf);
				initialLevel.Write(buf, 0, buf.Length);
				if (format > 4)
				{
					byte[] kindBuf = br.ReadBytes(_WorldData.m_ChunkBufferLength);
					chunk.SetBlocksKindBuffer(kindBuf);
					initialLevel.Write(kindBuf, 0, kindBuf.Length);
				}
				else
				{
					byte[] zeroBuf = new byte[_WorldData.m_ChunkBufferLength];
					initialLevel.Write(zeroBuf, 0, zeroBuf.Length);
				}
				if (isLightsBuf)
				{
					br.ReadBytes(_WorldData.m_ChunkBufferLength);
				}
			}
			allChunks.Release();
		}
		else
		{
			chunkBlockDepth = 16;
			chunksDeep = 8;
			InitWorldStructures(chunkBlockWidth, chunkBlockHeight, chunkBlockDepth, chunksWide, chunksHigh, chunksDeep);
			PChunkList allChunks2 = _WorldData.AllChunks;
			for (int x = 0; x < chunksWide; x++)
			{
				for (int y = 0; y < chunksHigh; y++)
				{
					byte[] wholeData = br.ReadBytes(chunkBlockWidth * chunkBlockHeight * 128);
					if (isLightsBuf)
					{
						br.ReadBytes(chunkBlockWidth * chunkBlockHeight * 128);
					}
					for (int z = 0; z < chunksDeep; z++)
					{
						byte[] data = new byte[wholeData.Length / 8];
						Array.Copy(wholeData, z * data.Length, data, 0, data.Length);
						_WorldData.Chunks[x, y, z].SetBlocksBuffer(data);
						initialLevel.Write(data, 0, data.Length);
						byte[] zeroBuf2 = new byte[_WorldData.m_ChunkBufferLength];
						initialLevel.Write(zeroBuf2, 0, zeroBuf2.Length);
					}
				}
			}
			allChunks2.Release();
		}
		_World.UnzippedInitialLevel = new byte[initialLevel.Length];
		_World.UnzippedInitialLevel = initialLevel.ToArray();
		int entityBufferLength = br.ReadInt32();
		byte[] entityBuffer = br.ReadBytes(entityBufferLength);
		yield return ((MonoBehaviour)this).StartCoroutine(_World.SetEntityBuffer(entityBuffer));
	}

	public void InitWorldStructures(int ch_Bw, int ch_Bh, int ch_Bd, int ch_w, int ch_h, int ch_d)
	{
		_WorldData = new WorldData();
		_WorldData.SetDimensions(ch_w, ch_h, ch_d, ch_Bw, ch_Bh, ch_Bd);
		_ChunksParent = ((Component)this).transform.FindChild("Chunks");
		_WorldDecorations = new List<IDecoration>
		{
			new StandardTreeDecorator(_WorldData)
		};
		ITerrainGenerationMethod terrainGenerationMethod = new FortressWarStandartTerrain();
		if (App.Instance.Settings.mapType == GameINI.MapType.STANDART)
		{
			terrainGenerationMethod = new FortressWarStandartTerrain();
		}
		if (App.Instance.Settings.mapType == GameINI.MapType.FLAT)
		{
			terrainGenerationMethod = new FlatLand();
		}
		if (App.Instance.Settings.mapType == GameINI.MapType.SAND)
		{
			terrainGenerationMethod = new SandLand();
		}
		if (App.Instance.Settings.mapType == GameINI.MapType.OCEAN)
		{
			terrainGenerationMethod = new OceanLand();
		}
		if (App.Instance.Settings.mapType == GameINI.MapType.ISLAND)
		{
			terrainGenerationMethod = new IslandLand();
		}
		if (App.Instance.Settings.mapType == GameINI.MapType.SNOWLAND)
		{
			terrainGenerationMethod = new SnowLand();
			_WorldDecorations = new List<IDecoration>
			{
				new SnowTreeDecorator(_WorldData)
			};
		}
		_World = new World(WorldData, new TerrainGenerator(WorldData, new BatchProcessor<Chunk>(), terrainGenerationMethod), new LightProcessor(WorldData), new NewMeshGenerator(new BatchProcessor<Chunk>(), WorldData), new WorldDecorator(WorldData, new BatchProcessor<Chunk>(), _WorldDecorations));
		_World.InitializeGridChunks();
		InitializeTextures();
		_ChunkObjects = new ChunkGameObject[3, WorldData.ChunksWide, WorldData.ChunksHigh, WorldData.ChunksDeep];
	}

	public void GrowServerRating()
	{
		if (App.Instance.Settings.ServerRating < 250)
		{
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable.Add("rating", (byte)(App.Instance.Settings.ServerRating + 1));
			PhotonNetwork.room.SetCustomProperties(hashtable);
			App.Instance.Settings.ServerRating++;
		}
	}

	public void LowerServerRating()
	{
		if (App.Instance.Settings.ServerRating > 0)
		{
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable.Add("rating", (byte)(App.Instance.Settings.ServerRating - 1));
			PhotonNetwork.room.SetCustomProperties(hashtable);
			App.Instance.Settings.ServerRating--;
		}
	}

	private void OnCreatedRoom()
	{
		Debug.Log((object)"OnCreatedRoom");
		photonView.RPC("SendPlayerProperties", PhotonTargets.AllBuffered, ProfileINI.nickname, ProfileINI.GetActualSkin(), VkontakteAPI.INSTANCE._viewerId, false, PhotonNetwork.player, ProfileINI.level);
		if (!App.Instance.Settings.loadingSavedMap)
		{
			if (App.Instance.Settings.mapSize == GameINI.MapSize.SMALL)
			{
				InitWorldStructures(16, 16, 16, 8, 8, 8);
			}
			if (App.Instance.Settings.mapSize == GameINI.MapSize.MEDIUM)
			{
				InitWorldStructures(16, 16, 16, 14, 14, 14);
			}
			((MonoBehaviour)this).StartCoroutine(_World.GenerateWorldCourutin());
		}
		else
		{
			((MonoBehaviour)this).StartCoroutine(LoadWorldFromStorage(App.Instance.Settings.playerID, App.Instance.Settings.slotID));
		}
	}

	private void OnPhotonCreateRoomFailed()
	{
		Debug.Log((object)"OnPhotonCreateRoomFailed");
	}

	public void SetNamesForAllPlayers()
	{
		for (int i = 0; i < PlayerList.Count; i++)
		{
			if ((Object)(object)PlayerList[i].Avatar != (Object)null)
			{
				PlayerList[i].Avatar.GetComponent<Nickname>().SetPlayer(PlayerList[i]);
			}
		}
	}

	public void SetSkinsForAllPlayers()
	{
		for (int i = 0; i < PlayerList.Count; i++)
		{
			if ((Object)(object)PlayerList[i].Avatar != (Object)null)
			{
				PlayerList[i].Avatar.GetComponent<SkinManager>().SetSkin(PlayerList[i].Skin);
			}
		}
	}

	private void SetAmbientSoundOnLevel()
	{
		if (!SoundManager.Instance.SoundsLoaded)
		{
			((MonoBehaviour)this).Invoke("SetAmbientSoundOnLevel", 1f);
			return;
		}
		if (App.Instance.Settings.mapType == GameINI.MapType.FLAT)
		{
			((Component)MainPlayerEyes).gameObject.audio.clip = SoundManager.Instance.GetClip(SoundManager.Sound.LoopForest);
		}
		if (App.Instance.Settings.mapType == GameINI.MapType.ISLAND)
		{
			((Component)MainPlayerEyes).gameObject.audio.clip = SoundManager.Instance.GetClip(SoundManager.Sound.LoopForest);
		}
		if (App.Instance.Settings.mapType == GameINI.MapType.STANDART)
		{
			((Component)MainPlayerEyes).gameObject.audio.clip = SoundManager.Instance.GetClip(SoundManager.Sound.LoopForest);
		}
		if (App.Instance.Settings.mapType == GameINI.MapType.SNOWLAND)
		{
			((Component)MainPlayerEyes).gameObject.audio.clip = SoundManager.Instance.GetClip(SoundManager.Sound.LoopSnow);
		}
		if (App.Instance.Settings.mapType == GameINI.MapType.SAND)
		{
			((Component)MainPlayerEyes).gameObject.audio.clip = SoundManager.Instance.GetClip(SoundManager.Sound.LoopDesert);
		}
		if (App.Instance.Settings.mapType == GameINI.MapType.OCEAN)
		{
			((Component)MainPlayerEyes).gameObject.audio.clip = SoundManager.Instance.GetClip(SoundManager.Sound.LoopOcean);
		}
		((Component)MainPlayerEyes).gameObject.audio.loop = true;
		((Component)MainPlayerEyes).gameObject.audio.volume = ProfileINI.ambient_volume;
		((Component)MainPlayerEyes).gameObject.audio.Play();
	}

	public void TeleportMainPlayerToSpawnPoint()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = GenerateSpawnPoint();
		MainPlayer.transform.position = val;
		photonView.RPC("TeleportPlayer", PhotonTargets.Others, val);
	}

	[RPC]
	private void TeleportPlayer(Vector3 pos, PhotonMessageInfo info)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		PlayerNode playerNode = FindPlayer(info.sender);
		if (playerNode != null && (Object)(object)playerNode.Avatar != (Object)null)
		{
			playerNode.Avatar.transform.position = pos;
		}
	}

	public Vector3 GenerateSpawnPoint()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)TeamBattle.Instance != (Object)null)
		{
			return TeamBattle.Instance.GenerateSpawnPoint();
		}
		if ((Object)(object)SpawnArrow.CurSpawn != (Object)null)
		{
			return new Vector3((float)(int)((Component)SpawnArrow.CurSpawn).transform.position.x, (float)(int)((Component)SpawnArrow.CurSpawn).transform.position.y, (float)(int)((Component)SpawnArrow.CurSpawn).transform.position.z);
		}
		return new Vector3((float)(_WorldData.WidthInBlocks / 2), (float)_WorldData.DepthInBlocks, (float)(_WorldData.HeightInBlocks / 2));
	}

	public void CompleteWorldGeneration()
	{
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		_WorldCamera.SetActive(false);
		IsWorldGenerated = true;
		if ((Object)(object)MainPlayer == (Object)null)
		{
			MainPlayer = PhotonNetwork.Instantiate(((Object)Player_Prefab).name, GenerateSpawnPoint(), ((Component)this).transform.rotation, 0);
			((Object)MainPlayer).name = "MAIN_PLAYER";
			MainPlayerEyes = ((Component)MainPlayer.transform.FindChild("recoil/Main Camera")).GetComponentInChildren<Camera>();
			SetNamesForAllPlayers();
			SetSkinsForAllPlayers();
		}
		else
		{
			MainPlayer.transform.position = GenerateSpawnPoint();
		}
		MainMenu.Instance.RefreshAcceleration();
		MainMenu.Instance.RefreshFlying();
		ShowGameGUI();
		SetAmbientSoundOnLevel();
		EntityBase.OnWorldGenerated();
		if (App.Instance.Settings.isServer)
		{
			GameType.Activate();
		}
		GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
		GC.WaitForPendingFinalizers();
		GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
	}

	public void ShowGameGUI()
	{
		MainMenu.Instance.HideLoading();
		MainMenu.Instance.HideMenu();
		MainMenu.Instance.SetMenu(Menu.Hud, show: true);
		if ((Object)(object)TeamBattle.Instance != (Object)null)
		{
			if (TeamBattle.Instance is Deathmatch)
			{
				MainMenu.Instance.SwitchMenu(Menu.Deathmatch, true);
			}
			else if (!(TeamBattle.Instance is ZombieVirus))
			{
				MainMenu.Instance.SwitchMenu(Menu.TeamBattle, true);
			}
		}
		else if (ProfileINI.tutorial_watch == 0)
		{
			ProfileINI.tutorial_watch = 1;
			ProfileINI.Save();
			MainMenu.Instance.SwitchMenu(Menu.Tutorial);
		}
		MainMenu.Instance.RefreshScreenSize();
		UpdateKubokText();
	}

	public void UpdateKubokText()
	{
		if (KubokCountInGame > 0)
		{
			KGUI.EnableNodes("hud.gold_cup", enable: true);
			if (Level.Instance.IsAdministrator())
			{
				KGUI.SetControlText("hud.gold_cup.txt_text1", Localize.GetText("HUD_GOLD_CUP_HIDE"));
				KGUI.SetControlText("hud.gold_cup.txt_text2", Localize.GetText("HUD_GOLD_CUP_ALL1") + KubokCountInGame + Localize.GetText("HUD_GOLD_CUP_ALL2"));
				return;
			}
			KGUI.SetControlText("hud.gold_cup.txt_text1", Localize.GetText("HUD_GOLD_CUP_AVAILABLE"));
			KGUI.SetControlText("hud.gold_cup.txt_text2", Localize.GetText("HUD_GOLD_CUP_FIND1") + KubokFindInGame + Localize.GetText("HUD_GOLD_CUP_FIND2") + KubokCountInGame);
			if (KubokFindInGame > KubokCountInGame)
			{
				KGUI.EnableNodes("hud.gold_cup", enable: false);
			}
		}
		else
		{
			KGUI.EnableNodes("hud.gold_cup", enable: false);
		}
	}

	private void ProcessChunkGameObject(Chunk chunk, int chunkType)
	{
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		if (chunk == null)
		{
			return;
		}
		if ((chunkType == 0 && chunk.LandData.IsEmpty) || (chunkType == 1 && chunk.WaterData.IsEmpty) || (chunkType == 2 && chunk.GlassData.IsEmpty))
		{
			if ((Object)(object)_ChunkObjects[chunkType, chunk.X, chunk.Y, chunk.Z] != (Object)null)
			{
				Object.Destroy((Object)(object)((Component)_ChunkObjects[chunkType, chunk.X, chunk.Y, chunk.Z]).gameObject);
			}
			switch (chunkType)
			{
			case 0:
				chunk.LandRegeneration = false;
				chunk.LandChunk = null;
				break;
			case 1:
				chunk.WaterRegeneration = false;
				chunk.WaterChunk = null;
				break;
			default:
				chunk.GlassRegeneration = false;
				chunk.GlassChunk = null;
				break;
			}
			return;
		}
		if ((Object)(object)_ChunkObjects[chunkType, chunk.X, chunk.Y, chunk.Z] == (Object)null)
		{
			string chunkName = chunkType switch
			{
				0 => string.Empty, 
				1 => "Water_", 
				_ => "Glass_", 
			} + chunk.ToString();
			Vector3 chunkPos = new Vector3((float)(chunk.X * WorldData.ChunkBlockWidth) + WorldData.GlobalXOffset, (float)(chunk.Z * WorldData.ChunkBlockDepth), (float)(chunk.Y * WorldData.ChunkBlockHeight) + WorldData.GlobalZOffset);
			_ChunkObjects[chunkType, chunk.X, chunk.Y, chunk.Z] = CreateChunkGameObject(chunkType, chunkName, chunkPos, null);
		}
		ChunkGameObject chunkGameObject = _ChunkObjects[chunkType, chunk.X, chunk.Y, chunk.Z];
		switch (chunkType)
		{
		case 0:
			chunkGameObject.CreateFromChunk(chunk.LandData);
			chunk.LandRegeneration = false;
			chunk.LandChunk = ((Component)chunkGameObject).transform;
			break;
		case 1:
			chunkGameObject.CreateFromChunk(chunk.WaterData);
			chunk.WaterRegeneration = false;
			chunk.WaterChunk = ((Component)chunkGameObject).transform;
			break;
		default:
			chunkGameObject.CreateFromChunk(chunk.GlassData);
			chunk.GlassRegeneration = false;
			chunk.GlassChunk = ((Component)chunkGameObject).transform;
			break;
		}
	}

	public ChunkGameObject CreateChunkGameObject(int chunkType, string chunkName, Vector3 chunkPos, Chunk.MeshData meshData)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		Object val = Object.Instantiate((Object)(object)(chunkType switch
		{
			0 => Chunk_Prefab, 
			1 => WaterPrefab, 
			_ => GlassPrefab, 
		}), chunkPos, Quaternion.identity);
		Transform val2 = (Transform)(object)((val is Transform) ? val : null);
		val2.parent = _ChunksParent;
		((Object)val2).name = chunkName;
		ChunkGameObject component = ((Component)val2).GetComponent<ChunkGameObject>();
		if (meshData != null)
		{
			component.CreateFromChunk(meshData);
		}
		return component;
	}

	private void OnMouseDown()
	{
		Debug.Log((object)"OnMoseDown");
	}

	private void UpdateStorePreview()
	{
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_051b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0530: Unknown result type (might be due to invalid IL or missing references)
		//IL_0542: Unknown result type (might be due to invalid IL or missing references)
		//IL_0684: Unknown result type (might be due to invalid IL or missing references)
		//IL_068b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0695: Unknown result type (might be due to invalid IL or missing references)
		//IL_069a: Unknown result type (might be due to invalid IL or missing references)
		//IL_069f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0603: Unknown result type (might be due to invalid IL or missing references)
		//IL_0608: Unknown result type (might be due to invalid IL or missing references)
		//IL_065e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0566: Unknown result type (might be due to invalid IL or missing references)
		//IL_056d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0577: Unknown result type (might be due to invalid IL or missing references)
		//IL_057c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0581: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0723: Unknown result type (might be due to invalid IL or missing references)
		//IL_0728: Unknown result type (might be due to invalid IL or missing references)
		//IL_072c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0731: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0700: Unknown result type (might be due to invalid IL or missing references)
		//IL_0705: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_074f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0754: Unknown result type (might be due to invalid IL or missing references)
		//IL_0758: Unknown result type (might be due to invalid IL or missing references)
		//IL_075d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0799: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0928: Unknown result type (might be due to invalid IL or missing references)
		//IL_0887: Unknown result type (might be due to invalid IL or missing references)
		//IL_088c: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_086b: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0935: Unknown result type (might be due to invalid IL or missing references)
		//IL_0946: Unknown result type (might be due to invalid IL or missing references)
		//IL_094b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0950: Unknown result type (might be due to invalid IL or missing references)
		//IL_082e: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_097c: Unknown result type (might be due to invalid IL or missing references)
		//IL_098d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0992: Unknown result type (might be due to invalid IL or missing references)
		//IL_0997: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)EntityPreview != (Object)null))
		{
			return;
		}
		if (EntityPreviewIsTaked && (Object)(object)EntityPreviewTaked == (Object)null)
		{
			SetEntityFromPreview();
			return;
		}
		EntityType type = EntityPreview.Type;
		if (type == EntityType.LOCK_DOOR_RED || type == EntityType.LOCK_DOOR_YELLOW || type == EntityType.LOCK_DOOR_BLUE || type == EntityType.LOCK_DOOR_GREEN || type == EntityType.LOCK_DOOR_WHITE || type == EntityType.LOCK_DOOR_WHITE || type == EntityType.LOCK_DOOR_RED || type == EntityType.LOCK_KEY_RED || type == EntityType.LOCK_KEY_YELLOW || type == EntityType.LOCK_KEY_BLUE || type == EntityType.LOCK_KEY_GREEN || type == EntityType.LOCK_KEY_WHITE || type == EntityType.TEAM_DOOR_BLUE || type == EntityType.TEAM_DOOR_RED || type == EntityType.FOOD_SPOON || type == EntityType.FOOD_FORK || type == EntityType.FOOD_MUG || type == EntityType.FLAG_RED || type == EntityType.FLAG_BLUE || type == EntityType.GIFT_ARMOR || type == EntityType.GIFT_GRENADE || type == EntityType.GIFT_AMMO || type == EntityType.GIFT_LIFE || type == EntityType.FOOD_JUG || type == EntityType.FOOD_METAL_PLATE || type == EntityType.FOOD_BREAD || type == EntityType.FOOD_CHEES || type == EntityType.FOOD_CHIKEN || type == EntityType.FOOD_SVININA || type == EntityType.FOOD_FISH || type == EntityType.FOOD_STEIK || type == EntityType.FOOD_ORANGE || type == EntityType.FOOD_PIZZA || type == EntityType.FOOD_COLA || type == EntityType.FOOD_BURGER || type == EntityType.FOOD_PIROG || type == EntityType.FOOD_CAKE || type == EntityType.WEAT || type == EntityType.DOORW || type == EntityType.DOORM || type == EntityType.BELLSMALL || type == EntityType.FISH_1 || type == EntityType.FISH_2 || type == EntityType.FISH_3 || type == EntityType.BOAT || type == EntityType.TROLLEY || type == EntityType.CAT || type == EntityType.DOG || type == EntityType.CHICKEN || type == EntityType.BOOK || type == EntityType.MILITARY_ARMORY || type == EntityType.MILITARY_BED || type == EntityType.MILITARY_LAMP || type == EntityType.MILITARY_DOOR)
		{
			if (Input.GetAxis("Mouse ScrollWheel") < 0f)
			{
				iTween.RotateAdd(((Component)EntityPreview).gameObject, new Vector3(0f, 90f, 0f), 1f);
			}
			else if (Input.GetAxis("Mouse ScrollWheel") > 0f)
			{
				iTween.RotateAdd(((Component)EntityPreview).gameObject, new Vector3(0f, -90f, 0f), 1f);
			}
		}
		else if (type != EntityType.PICTURE2_1 && type != EntityType.PAPER && type != EntityType.PICTURE1_1 && type != EntityType.PICTURE1_2 && type != EntityType.PICTURE3_2 && type != EntityType.TABLICHKAW && type != EntityType.LADDER)
		{
			if (Input.GetAxis("Mouse ScrollWheel") < 0f)
			{
				iTween.RotateAdd(((Component)EntityPreview).gameObject, new Vector3(0f, 0f, 90f), 1f);
			}
			else if (Input.GetAxis("Mouse ScrollWheel") > 0f)
			{
				iTween.RotateAdd(((Component)EntityPreview).gameObject, new Vector3(0f, 0f, -90f), 1f);
			}
		}
		if ((Object)(object)CameraController.Instance == (Object)null)
		{
			return;
		}
		bool food = EnityParametrs[(int)type].food;
		Ray val = CameraController.RaycastCamera.ScreenPointToRay(new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f, 0f));
		int num = ((!food) ? (1 << LayerMask.NameToLayer("Terrain")) : ((1 << LayerMask.NameToLayer("Entity")) | (1 << LayerMask.NameToLayer("Terrain"))));
		RaycastHit hit = default;
		if (!Physics.Raycast(val, ref hit, CameraController.RaycastDistance, num))
		{
			return;
		}
		Vector3 val2 = hit.point + hit.normal * 0.01f;
		IntVect intVect = new IntVect((int)val2.x, (int)val2.z, (int)val2.y);
		BlockType blockType = _WorldData.GetBlockType(intVect.X, intVect.Y, intVect.Z);
		if (type == EntityType.TABLICHKAW || type == EntityType.LADDER || type == EntityType.PICTURE1_1 || type == EntityType.PICTURE2_1 || type == EntityType.PICTURE1_2 || type == EntityType.PICTURE3_2 || type == EntityType.PAPER || (type >= EntityType.PAINTING_01 && type <= EntityType.PAINTING_13) || type == EntityType.MILITARY_CAM || type == EntityType.MILITARY_LANTERN || type == EntityType.MILITARY_BRIEFING_BOARD)
		{
			FindClippingPos(hit, out var pos, out var rotate);
			((Component)EntityPreview).transform.localEulerAngles = rotate;
			((Component)EntityPreview).transform.position = pos;
		}
		else
		{
			switch (type)
			{
			case EntityType.TORCH_FLOOR:
			case EntityType.TORCH_FLOOR_STONE:
			{
				ProcessTorchFloorAdd(hit, out var pos2);
				iTween.MoveTo(((Component)EntityPreview).gameObject, pos2, 0.5f);
				break;
			}
			case EntityType.TORCH:
			case EntityType.TORCH_STONE:
			{
				ProcessTorchAdd(hit, out var pos3, out var rotate2);
				((Component)EntityPreview).transform.localEulerAngles = rotate2;
				iTween.MoveTo(((Component)EntityPreview).gameObject, pos3, 0.5f);
				break;
			}
			case EntityType.BOAT:
				if (blockType == BlockType.Water)
				{
					val2 = hit.point + hit.normal * 0.01f;
					intVect = new IntVect((int)val2.x, (int)val2.z, (int)val2.y);
					iTween.MoveTo(((Component)EntityPreview).gameObject, new Vector3((float)intVect.X + 0.5f, (float)intVect.Z + 1f, (float)intVect.Y + 0.5f), 0.5f);
				}
				else
				{
					val2 = hit.point + hit.normal * 0.01f;
					intVect = new IntVect((int)val2.x, (int)val2.z, (int)val2.y);
					iTween.MoveTo(((Component)EntityPreview).gameObject, new Vector3((float)intVect.X + 0.5f, (float)intVect.Z + 0.25f, (float)intVect.Y + 0.5f), 0.5f);
				}
				break;
			case EntityType.TEAM_DOOR_RED:
			case EntityType.TEAM_DOOR_BLUE:
			{
				val2 = hit.point + hit.normal * 0.01f;
				intVect = new IntVect((int)val2.x, (int)val2.z, (int)val2.y);
				Quaternion rotation = ((Component)EntityPreview).transform.rotation;
				if (rotation.eulerAngles.y > 350f)
				{
					Quaternion rotation2 = ((Component)EntityPreview).transform.rotation;
					if (rotation2.eulerAngles.y < 360f)
					{
						goto IL_0770;
					}
				}
				Quaternion rotation3 = ((Component)EntityPreview).transform.rotation;
				if (rotation3.eulerAngles.y > 0f)
				{
					Quaternion rotation4 = ((Component)EntityPreview).transform.rotation;
					if (rotation4.eulerAngles.y < 10f)
					{
						goto IL_0770;
					}
				}
				Quaternion rotation5 = ((Component)EntityPreview).transform.rotation;
				if (rotation5.eulerAngles.y > 170f)
				{
					Quaternion rotation6 = ((Component)EntityPreview).transform.rotation;
					if (rotation6.eulerAngles.y < 190f)
					{
						iTween.MoveTo(((Component)EntityPreview).gameObject, new Vector3((float)intVect.X + 0.5f, (float)intVect.Z, (float)intVect.Y), 0.5f);
						break;
					}
				}
				iTween.MoveTo(((Component)EntityPreview).gameObject, new Vector3((float)intVect.X, (float)intVect.Z, (float)intVect.Y + 0.5f), 0.5f);
				break;
			}
			default:
				{
					if (food)
					{
						val2 = hit.point;
						intVect = new IntVect((int)val2.x, (int)val2.z, (int)val2.y);
						((Component)EntityPreview).transform.position = val2;
						break;
					}
					val2 = hit.point + hit.normal * 0.01f;
					intVect = new IntVect((int)val2.x, (int)val2.z, (int)val2.y);
					Vector3 val3 = new Vector3((float)intVect.X + 0.5f, (float)intVect.Z, (float)intVect.Y + 0.5f);
					if (type == EntityType.ATOMBOMB)
					{
						val3 += new Vector3(0f, 0.5f, 0f);
					}
					BlockKind blockKind = _WorldData.GetBlockKind(intVect.X, intVect.Y, intVect.Z);
					if (blockKind == BlockKind.Half)
					{
						val3 += new Vector3(0f, 0.5f, 0f);
					}
					iTween.MoveTo(((Component)EntityPreview).gameObject, val3, 0.5f);
					break;
				}
				IL_0770:
				iTween.MoveTo(((Component)EntityPreview).gameObject, new Vector3((float)intVect.X + 0.5f, (float)intVect.Z, (float)intVect.Y), 0.5f);
				break;
			}
		}
		MainMenu.CrosshairAction crosshairAction = MainMenu.CrosshairAction.None;
		if (type != EntityType.TORCH && type != EntityType.TORCH_STONE)
		{
			crosshairAction |= MainMenu.CrosshairAction.Wheel;
		}
		crosshairAction |= MainMenu.CrosshairAction.Put;
		MainMenu.Instance.SetCrosshairInfo(EnityParametrs[(int)type].name, crosshairAction);
	}

	private void UpdateSelectelAndCursorText()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)CameraController.Instance == (Object)null)
		{
			return;
		}
		EntityUnderCursor = null;
		Ray val = CameraController.RaycastCamera.ScreenPointToRay(new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f, 0f));
		int num = (1 << LayerMask.NameToLayer("SmallDecor")) | (1 << LayerMask.NameToLayer("Entity"));
		int num2 = 1 << LayerMask.NameToLayer("Terrain");
		RaycastHit val2 = default;
		if (Physics.Raycast(val, ref val2, CameraController.RaycastDistance, num))
		{
			((Renderer)_Selectel.GetComponent<MeshRenderer>()).enabled = false;
			EntityBase entityBase = EntityBase.FindEntity(((Component)val2.transform).gameObject);
			EntityType type = entityBase.Type;
			EntityUnderCursor = entityBase;
			MainMenu.CrosshairAction crosshairAction = MainMenu.CrosshairAction.None;
			switch (type)
			{
			case EntityType.DOORW:
			case EntityType.DOORM:
			case EntityType.MILITARY_DOOR:
				crosshairAction = (MainMenu.CrosshairAction)5;
				break;
			case EntityType.TABLICHKAF:
			case EntityType.TABLICHKAW:
			case EntityType.BOOK:
				crosshairAction = (MainMenu.CrosshairAction)17;
				break;
			case EntityType.TORCH:
			case EntityType.TORCH_FLOOR:
			case EntityType.TORCH_STONE:
			case EntityType.TORCH_FLOOR_STONE:
				crosshairAction = MainMenu.CrosshairAction.Take;
				break;
			case EntityType.TNT:
			case EntityType.ATOMBOMB:
				crosshairAction = (MainMenu.CrosshairAction)9;
				break;
			case EntityType.PICTURE1_1:
			case EntityType.PAPER:
			case EntityType.PICTURE2_1:
			case EntityType.PICTURE1_2:
			case EntityType.PICTURE3_2:
				crosshairAction = (MainMenu.CrosshairAction)33;
				break;
			case EntityType.CHAIR:
			case EntityType.BED:
			case EntityType.BOAT:
			case EntityType.TROLLEY:
			case EntityType.BED_STONE:
			case EntityType.CHAIR_STONE:
			case EntityType.BENCH_WOOD:
			case EntityType.BENCH_STONE:
			case EntityType.MILITARY_BED:
			case EntityType.MILITARY_CHAIR:
			case EntityType.MILITARY_BENCH:
				crosshairAction = (MainMenu.CrosshairAction)3;
				break;
			default:
				if (!GameType.BattleMode() || (type != EntityType.FLAG_RED && type != EntityType.FLAG_BLUE))
				{
					crosshairAction = MainMenu.CrosshairAction.Take;
				}
				else if (type >= EntityType.PAINTING_01 && type <= EntityType.PAINTING_13)
				{
					crosshairAction = MainMenu.CrosshairAction.Take;
				}
				break;
			}
			MainMenu.Instance.SetCrosshairInfo(entityBase.Name, crosshairAction);
		}
		else if (Physics.Raycast(val, ref val2, CameraController.RaycastDistance, num2))
		{
			((Renderer)_Selectel.GetComponent<MeshRenderer>()).enabled = true;
			Vector3 val3 = val2.point - val2.normal * 0.5f;
			IntVect intVect = new IntVect((int)Math.Floor(val3.x), (int)Math.Floor(val3.z), (int)Math.Floor(val3.y));
			_Selectel.transform.localScale = new Vector3(1.01f, 1.01f, 1.01f);
			_Selectel.transform.position = new Vector3((float)intVect.X + 0.5f, (float)intVect.Z + 0.5f, (float)intVect.Y + 0.5f);
		}
		else
		{
			((Renderer)_Selectel.GetComponent<MeshRenderer>()).enabled = false;
		}
		if ((Object)(object)EntityUnderCursor == (Object)null)
		{
			MainMenu.Instance.SetCrosshairInfo(null, MainMenu.CrosshairAction.None);
		}
	}

	public void ProcessContinuousDamage()
	{
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		if (MakeContinuousDamage && MainPlayerDead)
		{
			MakeContinuousDamage = false;
		}
		if (MakeContinuousDamage)
		{
			if (!_ContinuousDamageEnabled)
			{
				_ContinuousDamageEnabled = true;
				_ContinuousDamageLastHitTime = 0f;
			}
			if (Time.time - _ContinuousDamageLastHitTime >= 1f)
			{
				if (_ContinuousDamageLastHitTime > 0f)
				{
					MakeContinuousDamage = false;
				}
				_ContinuousDamageLastHitTime = Time.time;
				photonView.RPC("DamageMe", PhotonTargets.AllViaServer, 0.4f);
				MainPlayerNode.DoDamage(0.4f);
				SoundManager.Instance.Play(SoundManager.Sound.FootLava, MainPlayer.audio);
			}
			Color color = HitEffect.GetComponent<GUITexture>().color;
			color.a = 0.1f + Mathf.PingPong(Time.time / 2f, 0.4f);
			HitEffect.GetComponent<GUITexture>().color = color;
		}
		else if (_ContinuousDamageEnabled)
		{
			_ContinuousDamageEnabled = false;
			Color color2 = HitEffect.GetComponent<GUITexture>().color;
			color2.a = 0f;
			HitEffect.GetComponent<GUITexture>().color = color2;
		}
	}

	private void ProcessLava()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_045f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0464: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_0488: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04be: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_0518: Unknown result type (might be due to invalid IL or missing references)
		//IL_0537: Unknown result type (might be due to invalid IL or missing references)
		//IL_053c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0552: Unknown result type (might be due to invalid IL or missing references)
		//IL_0557: Unknown result type (might be due to invalid IL or missing references)
		//IL_056d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0572: Unknown result type (might be due to invalid IL or missing references)
		//IL_0591: Unknown result type (might be due to invalid IL or missing references)
		//IL_0596: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0606: Unknown result type (might be due to invalid IL or missing references)
		//IL_060b: Unknown result type (might be due to invalid IL or missing references)
		//IL_061b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0620: Unknown result type (might be due to invalid IL or missing references)
		//IL_063d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0642: Unknown result type (might be due to invalid IL or missing references)
		//IL_0658: Unknown result type (might be due to invalid IL or missing references)
		//IL_065d: Unknown result type (might be due to invalid IL or missing references)
		//IL_066d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0672: Unknown result type (might be due to invalid IL or missing references)
		//IL_068f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0694: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0711: Unknown result type (might be due to invalid IL or missing references)
		//IL_0716: Unknown result type (might be due to invalid IL or missing references)
		//IL_0733: Unknown result type (might be due to invalid IL or missing references)
		//IL_0738: Unknown result type (might be due to invalid IL or missing references)
		//IL_074e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0753: Unknown result type (might be due to invalid IL or missing references)
		//IL_0769: Unknown result type (might be due to invalid IL or missing references)
		//IL_076e: Unknown result type (might be due to invalid IL or missing references)
		//IL_078b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0790: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0803: Unknown result type (might be due to invalid IL or missing references)
		//IL_0819: Unknown result type (might be due to invalid IL or missing references)
		//IL_081e: Unknown result type (might be due to invalid IL or missing references)
		//IL_083b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0840: Unknown result type (might be due to invalid IL or missing references)
		//IL_0856: Unknown result type (might be due to invalid IL or missing references)
		//IL_085b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0871: Unknown result type (might be due to invalid IL or missing references)
		//IL_0876: Unknown result type (might be due to invalid IL or missing references)
		//IL_0893: Unknown result type (might be due to invalid IL or missing references)
		//IL_0898: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0900: Unknown result type (might be due to invalid IL or missing references)
		//IL_0905: Unknown result type (might be due to invalid IL or missing references)
		//IL_0915: Unknown result type (might be due to invalid IL or missing references)
		//IL_091a: Unknown result type (might be due to invalid IL or missing references)
		//IL_093d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0942: Unknown result type (might be due to invalid IL or missing references)
		//IL_0958: Unknown result type (might be due to invalid IL or missing references)
		//IL_095d: Unknown result type (might be due to invalid IL or missing references)
		//IL_096d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0972: Unknown result type (might be due to invalid IL or missing references)
		//IL_0995: Unknown result type (might be due to invalid IL or missing references)
		//IL_099a: Unknown result type (might be due to invalid IL or missing references)
		//IL_09aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_09af: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a02: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a07: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a22: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a45: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a4a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a60: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a65: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a80: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0abe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ade: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b01: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b06: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b21: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b37: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b5f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b64: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b7a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b7f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b95: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b9a: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)MainPlayer) && !GameType.IsObserving())
		{
			Camera mainPlayerEyes = MainPlayerEyes;
			BlockType blockType = _WorldData.GetBlockType((int)((Component)mainPlayerEyes).transform.position.x, (int)((Component)mainPlayerEyes).transform.position.z, (int)((Component)mainPlayerEyes).transform.position.y - 2);
			BlockType blockType2 = _WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x - 0.6f), (int)((Component)mainPlayerEyes).transform.position.z, (int)((Component)mainPlayerEyes).transform.position.y - 1);
			BlockType blockType3 = _WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x + 0.6f), (int)((Component)mainPlayerEyes).transform.position.z, (int)((Component)mainPlayerEyes).transform.position.y - 1);
			BlockType blockType4 = _WorldData.GetBlockType((int)((Component)mainPlayerEyes).transform.position.x, (int)(((Component)mainPlayerEyes).transform.position.z + 0.6f), (int)((Component)mainPlayerEyes).transform.position.y - 1);
			BlockType blockType5 = _WorldData.GetBlockType((int)((Component)mainPlayerEyes).transform.position.x, (int)(((Component)mainPlayerEyes).transform.position.z - 0.6f), (int)((Component)mainPlayerEyes).transform.position.y - 1);
			BlockType blockType6 = _WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x - 0.6f), (int)(((Component)mainPlayerEyes).transform.position.z + 0.6f), (int)((Component)mainPlayerEyes).transform.position.y - 1);
			BlockType blockType7 = _WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x + 0.6f), (int)(((Component)mainPlayerEyes).transform.position.z - 0.6f), (int)((Component)mainPlayerEyes).transform.position.y - 1);
			BlockType blockType8 = _WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x - 0.6f), (int)(((Component)mainPlayerEyes).transform.position.z - 0.6f), (int)((Component)mainPlayerEyes).transform.position.y - 1);
			BlockType blockType9 = _WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x + 0.6f), (int)(((Component)mainPlayerEyes).transform.position.z + 0.6f), (int)((Component)mainPlayerEyes).transform.position.y - 1);
			BlockType blockType10 = _WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x - 0.6f), (int)((Component)mainPlayerEyes).transform.position.z, (int)((Component)mainPlayerEyes).transform.position.y - 2);
			BlockType blockType11 = _WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x + 0.6f), (int)((Component)mainPlayerEyes).transform.position.z, (int)((Component)mainPlayerEyes).transform.position.y - 2);
			BlockType blockType12 = _WorldData.GetBlockType((int)((Component)mainPlayerEyes).transform.position.x, (int)(((Component)mainPlayerEyes).transform.position.z + 0.6f), (int)((Component)mainPlayerEyes).transform.position.y - 2);
			BlockType blockType13 = _WorldData.GetBlockType((int)((Component)mainPlayerEyes).transform.position.x, (int)(((Component)mainPlayerEyes).transform.position.z - 0.6f), (int)((Component)mainPlayerEyes).transform.position.y - 2);
			BlockType blockType14 = _WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x - 0.6f), (int)(((Component)mainPlayerEyes).transform.position.z + 0.6f), (int)((Component)mainPlayerEyes).transform.position.y - 2);
			BlockType blockType15 = _WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x + 0.6f), (int)(((Component)mainPlayerEyes).transform.position.z - 0.6f), (int)((Component)mainPlayerEyes).transform.position.y - 2);
			BlockType blockType16 = _WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x - 0.6f), (int)(((Component)mainPlayerEyes).transform.position.z - 0.6f), (int)((Component)mainPlayerEyes).transform.position.y - 2);
			BlockType blockType17 = _WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x + 0.6f), (int)(((Component)mainPlayerEyes).transform.position.z + 0.6f), (int)((Component)mainPlayerEyes).transform.position.y - 2);
			BlockType blockType18 = _WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x - 0.6f), (int)((Component)mainPlayerEyes).transform.position.z, (int)((Component)mainPlayerEyes).transform.position.y);
			BlockType blockType19 = _WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x + 0.6f), (int)((Component)mainPlayerEyes).transform.position.z, (int)((Component)mainPlayerEyes).transform.position.y);
			BlockType blockType20 = _WorldData.GetBlockType((int)((Component)mainPlayerEyes).transform.position.x, (int)(((Component)mainPlayerEyes).transform.position.z + 0.6f), (int)((Component)mainPlayerEyes).transform.position.y);
			BlockType blockType21 = _WorldData.GetBlockType((int)((Component)mainPlayerEyes).transform.position.x, (int)(((Component)mainPlayerEyes).transform.position.z - 0.6f), (int)((Component)mainPlayerEyes).transform.position.y);
			BlockType blockType22 = _WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x - 0.6f), (int)(((Component)mainPlayerEyes).transform.position.z + 0.6f), (int)((Component)mainPlayerEyes).transform.position.y);
			BlockType blockType23 = _WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x + 0.6f), (int)(((Component)mainPlayerEyes).transform.position.z - 0.6f), (int)((Component)mainPlayerEyes).transform.position.y);
			BlockType blockType24 = _WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x + 0.6f), (int)(((Component)mainPlayerEyes).transform.position.z + 0.6f), (int)((Component)mainPlayerEyes).transform.position.y);
			BlockType blockType25 = _WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x - 0.6f), (int)(((Component)mainPlayerEyes).transform.position.z - 0.6f), (int)((Component)mainPlayerEyes).transform.position.y);
			BlockType blockType26 = _WorldData.GetBlockType((int)((Component)mainPlayerEyes).transform.position.x, (int)((Component)mainPlayerEyes).transform.position.z, (int)(((Component)mainPlayerEyes).transform.position.y + 1f));
			BlockType blockType27 = _WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x - 0.6f), (int)((Component)mainPlayerEyes).transform.position.z, (int)(((Component)mainPlayerEyes).transform.position.y + 1f));
			BlockType blockType28 = _WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x + 0.6f), (int)((Component)mainPlayerEyes).transform.position.z, (int)(((Component)mainPlayerEyes).transform.position.y + 1f));
			BlockType blockType29 = _WorldData.GetBlockType((int)((Component)mainPlayerEyes).transform.position.x, (int)(((Component)mainPlayerEyes).transform.position.z + 0.6f), (int)(((Component)mainPlayerEyes).transform.position.y + 1f));
			BlockType blockType30 = _WorldData.GetBlockType((int)((Component)mainPlayerEyes).transform.position.x, (int)(((Component)mainPlayerEyes).transform.position.z - 0.6f), (int)(((Component)mainPlayerEyes).transform.position.y + 1f));
			BlockType blockType31 = _WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x - 0.6f), (int)(((Component)mainPlayerEyes).transform.position.z + 0.6f), (int)(((Component)mainPlayerEyes).transform.position.y + 1f));
			BlockType blockType32 = _WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x + 0.6f), (int)(((Component)mainPlayerEyes).transform.position.z - 0.6f), (int)(((Component)mainPlayerEyes).transform.position.y + 1f));
			BlockType blockType33 = _WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x + 0.6f), (int)(((Component)mainPlayerEyes).transform.position.z + 0.6f), (int)(((Component)mainPlayerEyes).transform.position.y + 1f));
			BlockType blockType34 = _WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x - 0.6f), (int)(((Component)mainPlayerEyes).transform.position.z - 0.6f), (int)(((Component)mainPlayerEyes).transform.position.y + 1f));
			if (blockType34 == BlockType.Lava || blockType33 == BlockType.Lava || blockType32 == BlockType.Lava || blockType31 == BlockType.Lava || blockType30 == BlockType.Lava || blockType29 == BlockType.Lava || blockType28 == BlockType.Lava || blockType27 == BlockType.Lava || blockType25 == BlockType.Lava || blockType24 == BlockType.Lava || blockType23 == BlockType.Lava || blockType22 == BlockType.Lava || blockType17 == BlockType.Lava || blockType16 == BlockType.Lava || blockType15 == BlockType.Lava || blockType14 == BlockType.Lava || blockType13 == BlockType.Lava || blockType12 == BlockType.Lava || blockType11 == BlockType.Lava || blockType10 == BlockType.Lava || blockType9 == BlockType.Lava || blockType8 == BlockType.Lava || blockType7 == BlockType.Lava || blockType6 == BlockType.Lava || blockType26 == BlockType.Lava || blockType == BlockType.Lava || blockType2 == BlockType.Lava || blockType3 == BlockType.Lava || blockType4 == BlockType.Lava || blockType5 == BlockType.Lava || blockType18 == BlockType.Lava || blockType19 == BlockType.Lava || blockType20 == BlockType.Lava || blockType21 == BlockType.Lava)
			{
				MakeContinuousDamage = true;
			}
		}
	}

	private void ProcessIce()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)MainPlayer))
		{
			BlockType blockType = _WorldData.GetBlockType((int)((Component)MainPlayerEyes).transform.position.x, (int)((Component)MainPlayerEyes).transform.position.z, (int)((Component)MainPlayerEyes).transform.position.y - 2);
			if (blockType == BlockType.Ice)
			{
				MainPlayer.SendMessage("SetIceBody", (object)true);
				_OnIce = true;
			}
			else if (!_OnIce)
			{
				MainPlayer.SendMessage("SetIceBody", (object)false);
			}
			else if (blockType != BlockType.Air)
			{
				MainPlayer.SendMessage("SetIceBody", (object)false);
			}
		}
	}

	private void ProcessWater()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)MainPlayer))
		{
			return;
		}
		Camera raycastCamera = CameraController.RaycastCamera;
		BlockType blockType = _WorldData.GetBlockType((int)((Component)raycastCamera).transform.position.x, (int)(((Component)raycastCamera).transform.position.z + 0.15f), (int)((Component)raycastCamera).transform.position.y);
		if (blockType == BlockType.Water)
		{
			RenderSettings.fogColor = TimeOfDay.WaterFogColor;
			RenderSettings.fogDensity = 0.04f;
			RenderSettings.skybox = null;
			((Behaviour)((Component)((Component)raycastCamera).transform).GetComponent<ColorCorrectionCurves>()).enabled = true;
			if (!_FaceInWater)
			{
				if (SoundManager.Instance.SoundsLoaded)
				{
					((Component)MainPlayerEyes).gameObject.audio.clip = SoundManager.Instance.GetClip(SoundManager.Sound.LoopUnderwater);
					((Component)MainPlayerEyes).gameObject.audio.loop = true;
					((Component)MainPlayerEyes).gameObject.audio.volume = ProfileINI.ambient_volume;
					((Component)MainPlayerEyes).gameObject.audio.Play();
					ProfileINI.sound_scale = 0.25f;
				}
				_FaceInWater = true;
			}
		}
		else
		{
			RenderSettings.fogDensity = 0.007f;
			RenderSettings.fogColor = TimeOfDay.FogColor;
			RenderSettings.skybox = _DefaultSkyBox;
			((Behaviour)((Component)((Component)raycastCamera).transform).GetComponent<ColorCorrectionCurves>()).enabled = false;
			if (_FaceInWater)
			{
				ProfileINI.sound_scale = 1f;
				SoundManager.Instance.Play(SoundManager.Sound.WaterOut, MainPlayer.gameObject.audio);
				SetAmbientSoundOnLevel();
				_FaceInWater = false;
				KGUI.ResizeGrid("hud.boobles", 0, (GameObject slot, int index) =>
				{
				});
			}
		}
		Transform transform = MainPlayer.transform;
		int num = (int)transform.position.x;
		int num2 = (int)transform.position.z;
		int num3 = (int)transform.position.y;
		blockType = _WorldData.GetBlockType(num, num2, num3);
		if (blockType == BlockType.Water)
		{
			bool flag = false;
			float num4 = (float)num3 + 1f;
			if (_WorldData.GetBlockType(num, num2, num3 + 1) == BlockType.Water)
			{
				num4++;
			}
			if (num4 - transform.position.y < 0.7f)
			{
				bool flag2 = false;
				for (int num5 = -1; num5 <= 1; num5++)
				{
					for (int num6 = -1; num6 <= 1; num6++)
					{
						BlockType blockType2 = _WorldData.GetBlockType(num + num5, num2 + num6, num3);
						if (blockType2 != BlockType.Water && blockType2 != BlockType.Air)
						{
							flag2 = true;
							break;
						}
					}
				}
				if (!flag2)
				{
					flag = true;
				}
			}
			MainPlayer.SendMessage("SetUnderWaterBody", (object)new bool[2] { true, flag });
		}
		else
		{
			MainPlayer.SendMessage("SetUnderWaterBody", (object)new bool[2]);
		}
		if (blockType == BlockType.Water && !_InWater)
		{
			SoundManager.Instance.Play(SoundManager.Sound.WaterIn, MainPlayer.gameObject.audio);
			_InWater = true;
		}
		else if (blockType != BlockType.Water && _InWater)
		{
			SoundManager.Instance.Play(SoundManager.Sound.WaterOut, MainPlayer.gameObject.audio);
			_InWater = false;
		}
		if (_FaceInWater)
		{
			_FaceInWaterTime += Time.deltaTime;
			if (App.Instance.Settings.gameType == GameINI.GameType.BUILDING && (Level.Instance.IsAdministrator() || Level.Instance.IsModerator()))
			{
				_FaceInWaterTime = 0.0001f;
			}
			if (_FaceInWaterTime >= 20f)
			{
				MakeContinuousDamage = true;
			}
		}
		else if (_FaceInWaterTime > 0f)
		{
			_FaceInWaterTime -= Time.deltaTime * 3f;
		}
		_FaceInWaterTime = Mathf.Clamp(_FaceInWaterTime, 0f, 20f);
		float num7 = Mathf.Clamp01(1f - _FaceInWaterTime / 20f) * 10f;
		if (num7 == _WaterBooblesCount)
		{
			return;
		}
		_WaterBooblesCount = num7;
		bool flag3 = _WaterBooblesCount < 10f && !MainPlayerDead;
		int showCount = Mathf.CeilToInt(_WaterBooblesCount);
		KGUI.ResizeGrid("hud.boobles", flag3 ? showCount : 0, (GameObject slot, int index) =>
		{
			if (index == showCount - 1)
			{
				slot.GetComponent<UISprite>().fillAmount = _WaterBooblesCount % 1f;
			}
			else
			{
				slot.GetComponent<UISprite>().fillAmount = 1f;
			}
		});
	}

	public void ClearMap()
	{
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Expected Obj, but got Unknown
		if (IsWorldGenerated)
		{
			for (int i = 0; i < WorldData.ChunksWide; i++)
			{
				for (int j = 0; j < WorldData.ChunksHigh; j++)
				{
					for (int k = 0; k < WorldData.ChunksDeep; k++)
					{
						World.DestroyChunk(WorldData.Chunks[i, j, k]);
					}
				}
			}
			IsWorldGenerated = false;
		}
		GameObject val = GameObject.Find("Chunks");
		foreach (Transform item in val.transform)
		{
			Transform val2 = item;
			Object.Destroy((Object)(object)((Component)val2).gameObject);
		}
		_World = null;
	}

	public void CreateNewMap()
	{
		if (_World.IsHomeWorld)
		{
			MainMenu.Instance.ShowLoading(string.Empty, string.Empty);
			ClearEntities();
			IsWorldGenerated = false;
			((MonoBehaviour)this).StartCoroutine(_World.GenerateWorldCourutin());
		}
	}

	public IEnumerator AddMoney()
	{
		Chat.SendInfo(ProfileINI.nickname + Localize.GetText("FOUND_5_GOLD"), forAll: true);
		ProfileINI.coins += 5;
		WWWForm form = new WWWForm();
		form.AddField("viewer_id", VkontakteAPI.INSTANCE._viewerId);
		form.AddField("auth_key", VkontakteAPI.INSTANCE._authKey);
		if (App.Instance.CurPlatform == App.Platform.ONDOKLASNIKI)
		{
			form.AddField("session_key", VkontakteAPI.INSTANCE._sessionKey);
		}
		Random r = new Random();
		form.AddField("random", r.Next());
		WWW php_load = new WWW(App.Instance.GetCurScriptsUrl() + "add_gold.php", form);
		yield return php_load;
		MonoBehaviour.print((object)php_load.text);
	}

	public void LadderFlyTest()
	{
		if (player_on_ladder)
		{
			last_ladder_delta += Time.deltaTime;
			if ((double)last_ladder_delta > 0.1)
			{
				player_on_ladder = false;
				MainPlayer.SendMessage("SetLadderBody", (object)false);
				MainMenu.Instance.RefreshFlying();
			}
		}
	}

	private void LateUpdate()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)MainPlayer != (Object)null && IsWorldGenerated && !MainMenu.Instance.Flying)
		{
			if (MainPlayer.transform.position.x < 1f)
			{
				MainPlayer.transform.position = new Vector3(1f, MainPlayer.transform.position.y, MainPlayer.transform.position.z);
			}
			if (MainPlayer.transform.position.z < 1f)
			{
				MainPlayer.transform.position = new Vector3(MainPlayer.transform.position.x, MainPlayer.transform.position.y, 1f);
			}
			if (MainPlayer.transform.position.x > (float)_WorldData.GetMaxBlockX())
			{
				MainPlayer.transform.position = new Vector3((float)_WorldData.GetMaxBlockX(), MainPlayer.transform.position.y, MainPlayer.transform.position.z);
			}
			if (MainPlayer.transform.position.z > (float)_WorldData.GetMaxBlockY())
			{
				MainPlayer.transform.position = new Vector3(MainPlayer.transform.position.x, MainPlayer.transform.position.y, (float)_WorldData.GetMaxBlockY());
			}
		}
	}

	private void Update()
	{
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		if (bs._Igor.debug && Input.GetKeyDown((KeyCode)306))
		{
			photonView.RPC("Pause", PhotonTargets.All);
		}
		if (IsWorldGenerated)
		{
			LadderFlyTest();
			Menu menu;
			if ((Object)(object)TeamBattle.Instance != (Object)null)
			{
				menu = ((!(TeamBattle.Instance is Deathmatch)) ? Menu.TeamBattle : Menu.Deathmatch);
			}
			else
			{
				menu = Menu.TabMenu;
			}
			if (Input.GetKeyDown((KeyCode)9))
			{
				MainMenu.Instance.ToggleMenu(menu);
			}
			else if (Input.GetKeyUp((KeyCode)9) && MainMenu.Instance.CurMenu == menu)
			{
				MainMenu.Instance.HideMenu();
			}
			if (Input.GetKeyDown((KeyCode)27) || Input.GetKeyDown((KeyCode)96))
			{
				if (MainMenu.Instance.MenuActive)
				{
					MainMenu.Instance.ToggleMenu(Menu.None);
				}
				else
				{
					MainMenu.Instance.SwitchMenu(Menu.GameMenu);
				}
			}
			if (Input.GetKeyDown((KeyCode)323))
			{
				if (!MainMenu.Instance.MenuActive)
				{
					Screen.lockCursor = true;
					EnableMouseLook();
				}
				else
				{
					Screen.lockCursor = false;
				}
			}
			if (_CheckCheating && DateTime.Now < _CheatingVotingTime + TimeSpan.FromSeconds(30.0))
			{
				string text = "Считаете ли вы игрока " + _CheatingPlayerName + " читером\n";
				text = (_CheatingVoted ? (text + ((!_CheatingVotedYes) ? "НЕТ!" : "ДА!") + "\n") : (text + "Да - нажмите клавишу Д\nНет - нажмите клавишу Н\n"));
				string text2 = text;
				text = text2 + "До конца голосования осталось " + (30 - (int)(DateTime.Now - _CheatingVotingTime).TotalSeconds) + " секунд";
				KGUI.SetControlText("hud.txt_cheating", text);
				if (!_CheatingVoted && !Chat.IsEnabled() && !MainMenu.Instance.MenuActive)
				{
					KeyCode val = (KeyCode)((Localize.Locale != Localize.LocaleType.RU) ? 121 : 108);
					KeyCode val2 = (KeyCode)((Localize.Locale != Localize.LocaleType.RU) ? 110 : 121);
					if (Input.GetKeyDown(val) || Input.GetKeyDown(val2))
					{
						_CheatingVoted = true;
						_CheatingVotedYes = Input.GetKeyDown(val);
						photonView.RPC("VoteCheating", PhotonTargets.AllViaServer, _CheatingVotedYes);
					}
				}
			}
			if ((Object)(object)MainPlayer != (Object)null && MainPlayer.transform.position.y < 0f)
			{
				TeleportMainPlayerToSpawnPoint();
			}
			int actualSkin = ProfileINI.GetActualSkin();
			if (actualSkin != MainPlayerNode.Skin)
			{
				MainPlayerNode.Skin = actualSkin;
				photonView.RPC("ChangePlayerSkin", PhotonTargets.OthersBuffered, MainPlayerNode.Name, actualSkin);
			}
			ProcessWater();
			ProcessIce();
			ProcessLava();
			ProcessContinuousDamage();
			UpdateStorePreview();
			UpdateSelectelAndCursorText();
			if (!Chat.IsEnabled() && !MainMenu.Instance.MenuActive)
			{
				if (Input.GetKeyDown((KeyCode)101))
				{
					if ((Object)(object)EntityPreview == (Object)null)
					{
						if ((Object)(object)EntityUnderCursor != (Object)null)
						{
							EntityUnderCursor.OnButtonE(ProfileINI.nickname);
							MainMenu.Instance.SetCrosshairInfo(null, MainMenu.CrosshairAction.None);
						}
					}
					else
					{
						SetEntityFromPreview();
					}
				}
				if (Input.GetKeyDown((KeyCode)102))
				{
					if ((Object)(object)EntityPreview == (Object)null)
					{
						if ((Object)(object)EntityUnderCursor != (Object)null)
						{
							EntityUnderCursor.OnButtonF(ProfileINI.nickname);
							MainMenu.Instance.SetCrosshairInfo(null, MainMenu.CrosshairAction.None);
						}
						else if ((Object)(object)MainPlayer != (Object)null && !((Collider)MainPlayer.GetComponent<CharacterController>()).enabled)
						{
							Trolley.KickPlayer(MainPlayer);
							Boat.KickPlayer(MainPlayer);
						}
					}
					else
					{
						SetEntityFromPreview();
					}
				}
			}
			_World.RegenerateChunks();
			CreateFinishedChunk();
			ProcessPlayerInput();
			DisplayDiggings();
		}
		else if (_ZipedChunkReceiveTime != 0f && Time.time - _ZipedChunkReceiveTime > 5f)
		{
			Debug.Log((object)"Exit waiting level data");
			if (!App.Instance.Settings.isServer)
			{
				ExitGame();
			}
		}
	}

	[RPC]
	public void Pause()
	{
		Debug.Break();
	}

	private void OnGUI()
	{
		DrawFlyMessages();
	}

	public void InitializeTextures()
	{
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Expected Obj, but got Unknown
		foreach (Texture2D worldTexture in ContentUpdater.WorldTextures)
		{
			for (int i = 0; i < World_Textures.Length; i++)
			{
				if ((Object)(object)World_Textures[i] != (Object)null && ((Object)World_Textures[i]).name == ((Object)worldTexture).name)
				{
					World_Textures[i] = worldTexture;
					break;
				}
			}
		}
		Texture2D worldTextureAtlas = WorldTextureAtlas;
		WorldTextureAtlas = new Texture2D(2048, 2048);
		WorldData.WorldTextureAtlasUvs = WorldTextureAtlas.PackTextures(World_Textures, 0);
		((Texture)WorldTextureAtlas).filterMode = (FilterMode)0;
		((Texture)WorldTextureAtlas).anisoLevel = 9;
		WorldTextureAtlas.Apply();
		WorldData.GenerateUVCoordinatesForAllBlocks();
		if (!((Object)(object)worldTextureAtlas != (Object)null))
		{
			return;
		}
		ChunkGameObject[] array = (ChunkGameObject[])(object)Object.FindObjectsOfType(typeof(ChunkGameObject));
		ChunkGameObject[] array2 = array;
		foreach (ChunkGameObject chunkGameObject in array2)
		{
			if ((Object)(object)((Renderer)((Component)chunkGameObject).GetComponent<MeshRenderer>()).material.mainTexture == (Object)(object)worldTextureAtlas)
			{
				((Renderer)((Component)chunkGameObject).GetComponent<MeshRenderer>()).material.mainTexture = (Texture)(object)WorldTextureAtlas;
			}
		}
	}

	public void EnableMouseLook()
	{
		if ((Object)(object)MainPlayer != (Object)null)
		{
			((Behaviour)MainPlayer.GetComponent<MouseLook>()).enabled = true;
			((Behaviour)((Component)MainPlayer.transform.Find("recoil/Main Camera")).GetComponent<MouseLook>()).enabled = true;
		}
	}

	public void DisableMouseLook()
	{
		if ((Object)(object)MainPlayer != (Object)null)
		{
			((Behaviour)MainPlayer.GetComponent<MouseLook>()).enabled = false;
			((Behaviour)((Component)MainPlayer.transform.Find("recoil/Main Camera")).GetComponent<MouseLook>()).enabled = false;
		}
	}

	private void ProcessTorchFloorAdd(RaycastHit hit, out Vector3 pos)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = hit.point + hit.normal * 0.01f;
		IntVect intVect = new IntVect((int)val.x, (int)val.z, (int)val.y);
		Vector3 normal = hit.normal;
		if (normal.Equals((object)new Vector3(0f, 1f, 0f)))
		{
			pos = new Vector3((float)intVect.X + 0.5f, (float)intVect.Z, (float)intVect.Y + 0.5f);
		}
		else
		{
			pos = new Vector3(0f, 0f, 0f);
		}
		BlockKind blockKind = _WorldData.GetBlockKind(intVect.X, intVect.Y, intVect.Z);
		if (blockKind == BlockKind.Half)
		{
			pos += new Vector3(0f, 0.5f, 0f);
		}
	}

	private void FindClippingPos(RaycastHit hit, out Vector3 pos, out Vector3 rotate)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = hit.point + hit.normal * 0.01f;
		IntVect intVect = new IntVect((int)val.x, (int)val.z, (int)val.y);
		pos = new Vector3((float)intVect.X + 0.5f, (float)intVect.Z, (float)intVect.Y + 0.5f);
		Vector3 normal = hit.normal;
		if (normal.Equals((object)new Vector3(0f, 0f, -1f)))
		{
			rotate = new Vector3(0f, 90f, 0f);
			return;
		}
		Vector3 normal2 = hit.normal;
		if (normal2.Equals((object)new Vector3(0f, 0f, 1f)))
		{
			rotate = new Vector3(0f, 270f, 0f);
			return;
		}
		Vector3 normal3 = hit.normal;
		if (normal3.Equals((object)new Vector3(-1f, 0f, 0f)))
		{
			rotate = new Vector3(0f, 180f, 0f);
			return;
		}
		Vector3 normal4 = hit.normal;
		if (normal4.Equals((object)new Vector3(1f, 0f, 0f)))
		{
			rotate = new Vector3(0f, 0f, 0f);
			return;
		}
		Vector3 normal5 = hit.normal;
		if (normal5.Equals((object)new Vector3(0f, 1f, 0f)))
		{
			rotate = new Vector3(0f, 0f, 90f);
			pos += new Vector3(0.5f, 0.5f, 0f);
			BlockKind blockKind = _WorldData.GetBlockKind(intVect.X, intVect.Y, intVect.Z);
			if (blockKind == BlockKind.Half)
			{
				pos += new Vector3(0f, 0.5f, 0f);
			}
			return;
		}
		Vector3 normal6 = hit.normal;
		if (normal6.Equals((object)new Vector3(0f, -1f, 0f)))
		{
			rotate = new Vector3(0f, 0f, 270f);
			pos += new Vector3(-0.5f, 0.5f, 0f);
			BlockKind blockKind2 = _WorldData.GetBlockKind(intVect.X, intVect.Y, intVect.Z);
			if (blockKind2.IsFlip() && blockKind2.DoFlip() == BlockKind.Half)
			{
				pos += new Vector3(0f, -0.5f, 0f);
			}
		}
		else
		{
			rotate = new Vector3(0f, 0f, 0f);
		}
	}

	private void ProcessTorchAdd(RaycastHit hit, out Vector3 pos, out Vector3 rotate)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = hit.point + hit.normal * 0.01f;
		IntVect intVect = new IntVect((int)val.x, (int)val.z, (int)val.y);
		Vector3 normal = hit.normal;
		if (normal.Equals((object)new Vector3(0f, 0f, -1f)))
		{
			pos = new Vector3((float)intVect.X + 0.51f, (float)intVect.Z + 0.01f, (float)intVect.Y + 0.49f);
			rotate = new Vector3(270f, 270f, 0f);
			return;
		}
		Vector3 normal2 = hit.normal;
		if (normal2.Equals((object)new Vector3(0f, 0f, 1f)))
		{
			pos = new Vector3((float)intVect.X + 0.51f, (float)intVect.Z + 0.01f, (float)intVect.Y + 0.49f);
			rotate = new Vector3(270f, 90f, 0f);
			return;
		}
		Vector3 normal3 = hit.normal;
		if (normal3.Equals((object)new Vector3(-1f, 0f, 0f)))
		{
			pos = new Vector3((float)intVect.X + 0.51f, (float)intVect.Z + 0.01f, (float)intVect.Y + 0.49f);
			rotate = new Vector3(270f, 0f, 0f);
			return;
		}
		Vector3 normal4 = hit.normal;
		if (normal4.Equals((object)new Vector3(1f, 0f, 0f)))
		{
			pos = new Vector3((float)intVect.X + 0.51f, (float)intVect.Z + 0.01f, (float)intVect.Y + 0.49f);
			rotate = new Vector3(270f, 180f, 0f);
		}
		else
		{
			pos = new Vector3(0f, 0f, 0f);
			rotate = new Vector3(0f, 0f, 0f);
		}
	}

	public void ToggleInventory()
	{
		if (!App.Instance.Settings.isWatch && !GameType.BuildDisabled())
		{
			MainMenu.Instance.ToggleMenu(Menu.Inventory);
			if (MainMenu.Instance.CurMenu == Menu.Inventory && (Object)(object)EntityPreview != (Object)null)
			{
				SetEntityFromPreview();
			}
		}
	}

	public void ToggleEmotions()
	{
		if (!App.Instance.Settings.isWatch)
		{
			MainMenu.Instance.ToggleMenu(Menu.Emotions);
		}
	}

	public void ToggleShop()
	{
		if (!App.Instance.Settings.isWatch)
		{
			MainMenu.Instance.ToggleMenu(Menu.Shop);
		}
	}

	public void ToggleSave()
	{
		if (!App.Instance.Settings.isWatch && Level.Instance.IsAdministrator() && GameType.CanSaveMap())
		{
			MainMenu.Instance.ToggleMenu(Menu.MyMaps);
		}
	}

	private void ProcessPlayerInput()
	{
		if (Input.anyKey && !MainPlayerDead && !Chat.IsEnabled() && !((Object)(object)Book.Current != (Object)null))
		{
			if (Input.GetKeyDown((KeyCode)99))
			{
				ToggleInventory();
			}
			if (Input.GetKeyDown((KeyCode)113))
			{
				ToggleEmotions();
			}
			if (Input.GetKeyDown((KeyCode)109))
			{
				ToggleShop();
			}
			if (Input.GetKeyDown((KeyCode)291))
			{
				ToggleSave();
			}
		}
	}

	public void OnBuildTerrain()
	{
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_061a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0623: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_050e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_0518: Unknown result type (might be due to invalid IL or missing references)
		Debug.Log((object)"OnBuildTerrain");
		if (App.Instance.Settings.isWatch)
		{
			Chat.SendWarning(Localize.GetText("CANT_BUILD_GAME_TYPE"), forAll: false);
		}
		else if (GameType.BlocksBuildDisabled())
		{
			Chat.SendWarning(Localize.GetText("CANT_BUILD_GAME_TYPE"), forAll: false);
		}
		else if (!Level.Instance.CanBuild)
		{
			Chat.SendWarning(Localize.GetText("CANT_EDIT_MAP"), forAll: false);
		}
		else
		{
			if ((Object)(object)CameraController.Instance == (Object)null || (CurrentBlock == BlockType.Air && CurrentBlockEntity == EntityType.AIR))
			{
				return;
			}
			Ray val = CameraController.RaycastCamera.ScreenPointToRay(new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f, 0f));
			int num = 1 << LayerMask.NameToLayer("Terrain");
			RaycastHit hit = default;
			if (!Physics.Raycast(val, ref hit, CameraController.RaycastDistance, num))
			{
				return;
			}
			Vector3 val2 = hit.point + hit.normal * 0.01f;
			IntVect intVect = new IntVect((int)val2.x, (int)val2.z, (int)val2.y);
			if (CurrentBlock != BlockType.Air)
			{
				if ((Store.Blocks[CurrentBlock].Purchase.Value == StorePurchase.NONE || ProfileINI.GetPurchaseValue(Store.Blocks[CurrentBlock].Purchase) > 0) && BuildCheckCollisionWithPlayer(hit) && !((Object)(object)Rail.Find(intVect.X, intVect.Y, intVect.Z) != (Object)null))
				{
					BlockType blockType = _WorldData.GetBlockType(intVect.X, intVect.Y, intVect.Z);
					BlockKind blockKind = _WorldData.GetBlockKind(intVect.X, intVect.Y, intVect.Z);
					BlockType blockType2 = _WorldData.GetBlockType(intVect.X, intVect.Y, intVect.Z + 1);
					BlockType blockType3 = _WorldData.GetBlockType(intVect.X, intVect.Y, intVect.Z - 1);
					IntVect intVect2 = intVect;
					CurrentKind = CurrentCommonKind.GetKind(val.direction);
					BlockKind blockKind2 = CurrentKind;
					if (blockKind == BlockKind.Half && CurrentKind == BlockKind.Half && blockType == CurrentBlock)
					{
						blockKind2 = BlockKind.Default;
					}
					else if ((blockType == BlockType.Water && CurrentBlock == BlockType.Water) || !blockKind.IsDefault())
					{
						intVect2 = new IntVect(intVect2.X, intVect2.Y, intVect2.Z + 1);
					}
					if (blockType2 != BlockType.Air && blockType3 == BlockType.Air)
					{
						blockKind2 = blockKind2.DoFlip();
					}
					photonView.RPC("AddBlockAt", PhotonTargets.All, (int)CurrentBlock, (int)blockKind2, intVect2.X, intVect2.Y, intVect2.Z);
				}
			}
			else
			{
				if (Store.Entities[CurrentBlockEntity].Purchase.Value != StorePurchase.NONE && ProfileINI.GetPurchaseValue(Store.Entities[CurrentBlockEntity].Purchase) <= 0)
				{
					return;
				}
				if (CurrentBlockEntity == EntityType.RAIL)
				{
					if (!Level.Instance.IsAdministrator() || App.Instance.Settings.isWatch)
					{
						Chat.SendWarning(Localize.GetText("CANT_SET_RAILS"), forAll: false);
						return;
					}
					bool flag = false;
					Rail rail = Rail.Find(intVect.X, intVect.Y, intVect.Z);
					if ((Object)(object)rail != (Object)null)
					{
						for (int i = 0; i < 6; i++)
						{
							if (++_CurBlockSibling > 5)
							{
								_CurBlockSibling = 0;
							}
							if (_CurBlockSibling != rail.PlaneIndex)
							{
								int num2 = ((_CurBlockSibling == 3) ? 1 : ((_CurBlockSibling == 2) ? (-1) : 0));
								int num3 = ((_CurBlockSibling == 5) ? 1 : ((_CurBlockSibling == 4) ? (-1) : 0));
								int num4 = ((_CurBlockSibling == 1) ? 1 : ((_CurBlockSibling == 0) ? (-1) : 0));
								if (_WorldData.GetBlockType(intVect.X + num2, intVect.Y + num3, intVect.Z + num4) != BlockType.Air)
								{
									rail.SelfDelete();
									rail = null;
									val2 = new Vector3((float)intVect.X + 0.5f, (float)intVect.Z, (float)intVect.Y + 0.5f);
									val2 += Rail.PlanePosition[_CurBlockSibling];
									ProfileINI.SetPurchaseValue(StorePurchase.RAIL, ProfileINI.GetPurchaseValue(StorePurchase.RAIL) + 1);
									flag = true;
									break;
								}
							}
						}
						if ((Object)(object)rail != (Object)null)
						{
							return;
						}
					}
					else if (_WorldData.GetBlockType(intVect.X, intVect.Y, intVect.Z) == BlockType.Water)
					{
						return;
					}
					if (!flag)
					{
						((MonoBehaviour)MainMenu.Instance).StartCoroutine(MainMenu.Instance.PurchaseUse(StorePurchase.RAIL));
					}
					else
					{
						photonView.RPC("ReplaceRail", PhotonTargets.AllViaServer);
					}
				}
				else
				{
					((MonoBehaviour)MainMenu.Instance).StartCoroutine(MainMenu.Instance.PurchaseUse(Store.Entities[CurrentBlockEntity].Purchase));
				}
				ProfileINI.SetPurchaseValue(StorePurchase.RAIL, ProfileINI.GetPurchaseValue(StorePurchase.RAIL) - 1);
				photonView.RPC("AddEntityNetwork", PhotonTargets.MasterClient, (int)CurrentBlockEntity, val2, Quaternion.identity, ProfileINI.nickname, VkontakteAPI.INSTANCE._viewerId, null);
			}
		}
	}

	public bool BuildCheckCollisionWithPlayer(RaycastHit hit)
	{
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < PlayerList.Count; i++)
		{
			if (!((Object)(object)PlayerList[i].Avatar == (Object)null))
			{
				Vector3 val = PlayerList[i].Avatar.transform.position - hit.point;
				float magnitude = val.magnitude;
				if ((double)magnitude < 1.2)
				{
					return false;
				}
			}
		}
		Vector3 normal = hit.normal;
		if (normal.Equals((object)new Vector3(0f, 1f, 0f)))
		{
			Vector3 val2 = MainPlayer.transform.position - hit.point;
			float magnitude2 = val2.magnitude;
			if ((double)magnitude2 < 1.2)
			{
				return false;
			}
		}
		else if ((double)hit.distance < 1.6)
		{
			return false;
		}
		return true;
	}

	public void OnPlayerHitTerrain()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)CameraController.Instance == (Object)null)
		{
			return;
		}
		Ray val = CameraController.RaycastCamera.ScreenPointToRay(new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f, 0f));
		RaycastHit val2 = default;
		int num;
		if (GameType.BattleMode() && !GameType.IsObserving())
		{
			num = 1 << LayerMask.NameToLayer("Players");
			if (Physics.Raycast(val, ref val2, CameraController.RaycastDistance, num) && (Object)(object)((Component)val2.collider).gameObject != (Object)(object)MainPlayer)
			{
				PlayerNode playerNode = FindPlayerByAvatar(((Component)val2.collider).gameObject);
				if (playerNode != null && TeamBattle.Instance.IsOpposite(playerNode.NetPlayer.name))
				{
					playerNode.Avatar.GetComponent<PhotonView>().RPC("OnHit", PhotonTargets.All, MainPlayer.transform.forward, MainPlayer.transform.position);
					PunchPlayer(playerNode.NetPlayer, MainPlayer.transform.forward, 0.5f, MainPlayer.GetComponent<PlayerNetwork>().Knife.arrayId);
				}
				return;
			}
		}
		num = (1 << LayerMask.NameToLayer("Entity")) | (1 << LayerMask.NameToLayer("SmallDecor"));
		if (!GameType.HitDisabled() && Physics.Raycast(val, ref val2, CameraController.RaycastDistance, num))
		{
			EntityBase.FindEntity(((Component)val2.transform).gameObject).OnLeftMouseHit(ProfileINI.nickname);
			return;
		}
		num = 1 << LayerMask.NameToLayer("Terrain");
		if (!GameType.DigDisabled() && Physics.Raycast(val, ref val2, CameraController.RaycastDistance, num))
		{
			if (App.Instance.Settings.isWatch)
			{
				Chat.SendWarning(Localize.GetText("CANT_DESTROY_IN_WATCH"), forAll: false);
				return;
			}
			Vector3 point = val2.point;
			Vector3 direction = val.direction;
			Vector3 val3 = point + direction.normalized * 0.01f;
			IntVect targetLocation = new IntVect((int)val3.x, (int)val3.z, (int)val3.y);
			_World.Dig(targetLocation, val2.point, val2.normal);
		}
		else
		{
			SoundManager.Instance.Play(SoundManager.Sound.WeaponSwish, MainPlayer.gameObject.audio);
		}
	}

	public void SendSoundRpc(int sound_type, Vector3 sound_pos)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (bSkipSoundRpc)
		{
			bSkipSoundRpc = !bSkipSoundRpc;
			return;
		}
		photonView.RPC("SoundRPC", PhotonTargets.Others, sound_type, sound_pos);
		bSkipSoundRpc = !bSkipSoundRpc;
	}

	[RPC]
	private void SoundRPC(int sound_type, Vector3 sound_pos)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)MainPlayer != (Object)null)
		{
			SoundManager.Instance.PlayAtPoint((SoundManager.Sound)sound_type, sound_pos);
		}
	}

	[RPC]
	private void RemoveBlockAt(int x, int y, int z, PhotonMessageInfo info)
	{
		if (!App.Instance.Settings.destroyable)
		{
			return;
		}
		if (IsWorldGenerated)
		{
			if (Level.Instance.IsBuilder(info.sender.name))
			{
				_World.RemoveBlockAt(new IntVect(x, y, z), silent: false);
			}
		}
		else
		{
			LevelDeltaLoading.Add(BlockType.Air, x, y, z, checkAvulsion: true);
		}
	}

	[RPC]
	private void RemoveBlocksAt(int x, int y, int z, int radius, PhotonMessageInfo info)
	{
		if (!App.Instance.Settings.destroyable)
		{
			return;
		}
		if (IsWorldGenerated)
		{
			if (Level.Instance.IsBuilder(info.sender.name))
			{
				_World.RemoveBlocksAt(new IntVect(x, y, z), radius);
			}
			return;
		}
		for (int i = -radius; i <= radius; i++)
		{
			for (int j = -radius; j <= radius; j++)
			{
				for (int k = -radius; k <= radius; k++)
				{
					LevelDeltaLoading.Add(BlockType.Air, x + i, y + j, z + k, checkAvulsion: true);
				}
			}
		}
	}

	[RPC]
	private void AddBlockAt(int id, int kind, int x, int y, int z, PhotonMessageInfo info)
	{
		if (IsWorldGenerated)
		{
			if (Level.Instance.IsBuilder(info.sender.name))
			{
				_World.AddBlockAt(new IntVect(x, y, z), (BlockType)id, (BlockKind)kind);
			}
		}
		else
		{
			LevelDeltaLoading.Add((BlockType)id, x, y, z, checkAvulsion: false, (BlockKind)kind);
		}
	}

	public void CreateFinishedChunk()
	{
		while (WorldData.FinishedLandChunks.Count > 0)
		{
			ProcessChunkGameObject(WorldData.GetFinishedLandChunk(), 0);
		}
		while (WorldData.FinishedWaterChunks.Count > 0)
		{
			ProcessChunkGameObject(WorldData.GetFinishedWaterChunk(), 1);
		}
		while (WorldData.FinishedGlassChunks.Count > 0)
		{
			ProcessChunkGameObject(WorldData.GetFinishedGlassChunk(), 2);
		}
	}

	private void OnApplicationQuit()
	{
	}

	[RPC]
	public void AddEntityNetwork(int type, Vector3 pos, Quaternion rot, string creator, string creatorID, object[] data)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		type = Mathf.Abs(type);
		SoundManager.Instance.Play(SoundManager.Sound.SetDecor, MainPlayer.audio);
		_World.AddEntityNetwork((EntityType)type, pos, rot, creator, creatorID, data);
	}

	[RPC]
	public void MoveEntityNetwork(int entityViewID, Vector3 pos, Quaternion rot, PhotonMessageInfo info)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		if (!Level.Instance.IsBuilder(info.sender.name))
		{
			return;
		}
		PhotonView photonView = PhotonView.Find(entityViewID);
		if ((Object)(object)photonView == (Object)null)
		{
			return;
		}
		EntityBase component = ((Component)photonView).GetComponent<EntityBase>();
		if (!((Object)(object)component == (Object)null))
		{
			((Component)component).transform.position = pos;
			((Component)component).transform.rotation = rot;
			if (component is Trolley)
			{
				((Trolley)component).OnManualMove();
			}
		}
	}

	[RPC]
	public void DeleteEntityNetworkAll(int id, PhotonMessageInfo info)
	{
		if (Level.Instance.IsBuilder(info.sender.name) && !PhotonNetwork.isMasterClient)
		{
			EntityBase entityBase = FindEntityByID(id);
			if ((Object)(object)entityBase != (Object)null)
			{
				EntityBase.Entities.Remove(entityBase);
				((Component)entityBase).SendMessage("Destruction");
			}
		}
	}

	[RPC]
	public void DeleteEntityNetworkMasterClient(int id, PhotonMessageInfo info)
	{
		if (Level.Instance.IsBuilder(info.sender.name))
		{
			EntityBase entityBase = FindEntityByID(id);
			if ((Object)(object)entityBase != (Object)null)
			{
				EntityBase.Entities.Remove(entityBase);
				((Component)entityBase).SendMessage("Destruction");
				PhotonNetwork.Destroy(((Component)entityBase).gameObject);
			}
		}
	}

	public void SetEntityFromPreview()
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)EntityPreview == (Object)null))
		{
			if (!EntityPreviewIsTaked)
			{
				photonView.RPC("AddEntityNetwork", PhotonTargets.MasterClient, (int)EntityPreview.Type, ((Component)EntityPreview).transform.position, ((Component)EntityPreview).transform.rotation, ProfileINI.nickname, VkontakteAPI.INSTANCE._viewerId, EntityPreview.GetData());
			}
			else if ((Object)(object)EntityPreviewTaked != (Object)null)
			{
				photonView.RPC("MoveEntityNetwork", PhotonTargets.All, EntityPreviewTaked.photonView.viewID, ((Component)EntityPreview).transform.position, ((Component)EntityPreview).transform.rotation);
			}
			Object.Destroy((Object)(object)((Component)EntityPreview).gameObject);
			EntityPreview = null;
			EntityPreviewIsTaked = false;
			EntityPreviewTaked = null;
		}
	}

	public void AddPreview(EntityType ind)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected Obj, but got Unknown
		Vector3 position = MainPlayer.transform.position;
		Quaternion rotation = EnityParametrs[(int)ind].gameObject.transform.rotation;
		GameObject val = (GameObject)Object.Instantiate((Object)(object)EnityParametrs[(int)ind].gameObject, position, rotation);
		EntityPreview = val.GetComponent<EntityBase>();
		EntityPreview.InitializePreview(ind, null);
		EntityPreviewIsTaked = false;
		EntityPreviewTaked = null;
	}

	public void TakeEnity(GameObject entityGO)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Expected Obj, but got Unknown
		if (!App.Instance.Settings.isWatch)
		{
			if (Level.Instance.CanBuild)
			{
				EntityBase component = entityGO.GetComponent<EntityBase>();
				Vector3 position = ((Component)component).transform.position;
				Quaternion rotation = ((Component)component).transform.rotation;
				GameObject val = (GameObject)Object.Instantiate((Object)(object)EnityParametrs[(int)component.Type].gameObject, position, rotation);
				EntityPreview = val.GetComponent<EntityBase>();
				EntityPreview.InitializePreview(component.Type, component.GetData());
				MaterialExt.SetShader(((Component)EntityPreview).gameObject, Shader.Find("Transparent/Diffuse"));
				MaterialExt.SetTransparent(((Component)EntityPreview).gameObject, 0.5f);
				EntityPreviewIsTaked = true;
				EntityPreviewTaked = component;
			}
			else
			{
				Chat.SendWarning(Localize.GetText("CANT_EDIT_MAP"), forAll: false);
			}
		}
		else
		{
			Chat.SendWarning(Localize.GetText("CANT_EDIT_MAP_IN_WATCH"), forAll: false);
		}
	}

	private void DisplayDiggings()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (_World.Diggings.Count != 0)
		{
			Vector3 val = _World.Diggings.Dequeue();
			Object.Instantiate((Object)(object)Sparks, val, Quaternion.identity);
		}
	}

	public void OnMainPlayerDead()
	{
		((MonoBehaviour)this).StartCoroutine(MainPlayerDeadProcess());
	}

	private IEnumerator MainPlayerDeadProcess()
	{
		MainPlayerDead = true;
		CameraController cameraController = MainPlayer.GetComponent<CameraController>();
		cameraController.SetThirdPerson(freeLook: true, 45f, saveSettings: false);
		cameraController.DisableInput = true;
		MainPlayer.GetComponent<PlayerInput>().SetMovement(move: false);
		float waitDuration = 3f;
		if ((Object)(object)MainPlayer.animation != (Object)null && (TrackedReference)(object)MainPlayer.animation["death"] != (TrackedReference)null)
		{
			MainPlayer.animation.CrossFade("death");
			MainPlayer.GetComponent<NetworkSyncAnimation>().SyncAnimation(NetworkSyncAnimation.AnimState.death);
			waitDuration += MainPlayer.animation["death"].length;
		}
		yield return (object)new WaitForSeconds(waitDuration);
		if (!MainPlayerDead)
		{
			yield break;
		}
		KGUI.EnableNodes("hud.txt_respawn_tip", enable: true);
		((MonoBehaviour)this).StartCoroutine(bs.AddUpdate(() =>
		{
			if (Input.GetKeyDown((KeyCode)323) && Screen.lockCursor && MainPlayerDead)
			{
				Respawn();
			}
			return MainPlayerDead;
		}));
	}

	public void Respawn()
	{
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		KGUI.EnableNodes("hud.txt_respawn_tip", enable: false);
		MainPlayerDead = false;
		_FaceInWaterTime = 0f;
		if (MainPlayerNode != null)
		{
			MainPlayerNode.Life = MainPlayerNode.MaxLife;
			MainPlayerNode.Shield = 0f;
			if (!((Collider)MainPlayer.GetComponent<CharacterController>()).enabled)
			{
				Trolley.KickPlayer(MainPlayer);
				Boat.KickPlayer(MainPlayer);
			}
			MainPlayer.GetComponent<PlayerInput>().SetMovement(move: true);
			CameraController component = MainPlayer.GetComponent<CameraController>();
			component.DisableInput = false;
			component.SetSavedCamera();
			TeleportMainPlayerToSpawnPoint();
			Gun[] guns = ((Component)component).GetComponent<PlayerNetwork>().Guns;
			foreach (Gun gun in guns)
			{
				gun.Reset();
			}
			component.Reset();
			EnableMouseLook();
			Screen.lockCursor = true;
		}
		MainPlayer.GetComponent<PlayerNetwork>().Reset(die: false);
		photonView.RPC("RespawnRPC", PhotonTargets.Others, PhotonNetwork.player, MainPlayer.transform.position);
		MainPlayer.GetComponent<PlayerNetwork>().photonView.RPC("SetPos", PhotonTargets.All, MainPlayer.transform.position);
	}

	[RPC]
	private void RespawnRPC(PhotonPlayer player, Vector3 spawnPoint)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		PlayerNode playerNode = FindPlayer(player);
		if (playerNode != null && (Object)(object)playerNode.Avatar != (Object)null)
		{
			playerNode.Avatar.transform.position = spawnPoint;
			playerNode.Avatar.GetComponent<PlayerNetwork>().Reset(die: false);
		}
	}

	public void PunchPlayer(PhotonPlayer player, Vector3 dir, float damage = 0.5f, int gunId = -1)
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		bool flag = true;
		bool flag2 = true;
		photonView.RPC("HitRedness", PhotonTargets.All, player);
		PlayerNode playerNode = FindPlayer(player);
		if (playerNode != null && (Object)(object)playerNode.Avatar != (Object)null)
		{
			playerNode.Avatar.GetComponent<PhotonView>().RPC("HitBy", PhotonTargets.All, PhotonNetwork.player, gunId);
			photonView.RPC("ReceiveDamage", player, dir, damage, flag, flag2, playerNode.ViewerID);
		}
	}

	[RPC]
	public void Explosion(Vector3 pos)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected Obj, but got Unknown
		GameObject val = (GameObject)Object.Instantiate((Object)(object)bs._Igor.explosion, pos, Quaternion.identity);
		AudioSource component = val.GetComponent<AudioSource>();
		component.volume = ProfileINI.sound_volume * ProfileINI.sound_scale;
		Object.Destroy((Object)(object)val, 3f);
	}

	[RPC]
	private void ReceiveDamage(Vector3 dir, float damage, bool jolt, bool spurn, string viewerID)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (MainPlayerNode.Life <= 0f)
		{
			return;
		}
		MainPlayerNode.DoDamage(damage);
		if (jolt)
		{
			CameraController component = MainPlayer.GetComponent<CameraController>();
			if ((Object)(object)component != (Object)null)
			{
				component.JoltScreen();
			}
		}
		if (spurn)
		{
			((MonoBehaviour)this).StartCoroutine(PunchMainPlayerProcess(dir * 3f, 0.2f));
		}
	}

	[RPC]
	private void DamageMe(float damage)
	{
	}

	private IEnumerator PunchMainPlayerProcess(Vector3 force, float duration)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		float endTime = Time.time + duration;
		float tickTime = Time.time;
		while (Time.time < endTime)
		{
			yield return (object)new WaitForSeconds(0.01f);
			if ((Object)(object)MainPlayer == (Object)null)
			{
				break;
			}
			CharacterController characterController = MainPlayer.GetComponent<CharacterController>();
			if ((Object)(object)characterController == (Object)null || !((Collider)characterController).enabled)
			{
				break;
			}
			characterController.Move(force * (Time.time - tickTime));
			tickTime = Time.time;
		}
	}

	[RPC]
	private void HitRedness(PhotonPlayer player)
	{
		if (GameType.IsObserving())
		{
			return;
		}
		PlayerNode playerNode = FindPlayer(player);
		if (playerNode != null && (Object)(object)playerNode.Avatar != (Object)null)
		{
			playerNode.Avatar.GetComponent<bs>().PlayOneShot(bs._Igor.damageSound[Random.Range(0, bs._Igor.damageSound.Length)]);
			SkinManager component = playerNode.Avatar.GetComponent<SkinManager>();
			if ((Object)(object)component != (Object)null)
			{
				component.Redness();
			}
		}
	}

	public void AddFlyMessage(int viewID, string message, Color color, bool forAll)
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (forAll)
		{
			photonView.RPC("AddFlyMessageInternal", PhotonTargets.All, viewID, message, (object)new Vector3(color.r, color.g, color.b));
		}
		else
		{
			AddFlyMessageInternal(viewID, message, new Vector3(color.r, color.g, color.b));
		}
	}

	[RPC]
	private void AddFlyMessageInternal(int viewID, string message, Vector3 color)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		PhotonView photonView = PhotonView.Find(viewID);
		if (!((Object)(object)photonView == (Object)null))
		{
			FlyMessage flyMessage = new FlyMessage();
			flyMessage.Text = message;
			flyMessage.View = photonView;
			Vector3 val = new Vector3(Random.Range(-0.5f, 0.5f), 1f, 0f);
			flyMessage.MoveDir = val.normalized;
			flyMessage.EndTime = Time.time + 1f;
			flyMessage.TextColor = new Color(color.x, color.y, color.z);
			_FlyMessages.Add(flyMessage);
		}
	}

	private void DrawFlyMessages()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected Obj, but got Unknown
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		if ((int)Event.current.type != 7 || _FlyMessages.Count == 0)
		{
			return;
		}
		if (_FlyMessageStyle == null)
		{
			_FlyMessageStyle = new GUIStyle();
			_FlyMessageStyle.fontSize = 40;
			_FlyMessageStyle.normal.textColor = Color.white;
			_FlyMessageStyle.alignment = (TextAnchor)4;
		}
		for (int i = 0; i < _FlyMessages.Count; i++)
		{
			FlyMessage flyMessage = _FlyMessages[i];
			if ((Object)(object)flyMessage.View != (Object)null)
			{
				Vector3 val;
				if ((Object)(object)((Component)flyMessage.View).gameObject != (Object)(object)MainPlayer || CameraController.Instance.IsThirdPerson)
				{
					val = ((Component)flyMessage.View).transform.position;
					if ((Object)(object)((Component)flyMessage.View).collider != (Object)null)
					{
						Vector3 val2 = val;
						Bounds bounds = ((Component)flyMessage.View).collider.bounds;
						val = val2 + bounds.size.y * ((Component)flyMessage.View).transform.up;
					}
				}
				else
				{
					val = ((Component)CameraController.RaycastCamera).transform.position + ((Component)CameraController.RaycastCamera).transform.forward;
				}
				Vector3 val3 = CameraController.RaycastCamera.WorldToScreenPoint(val);
				val3 += flyMessage.MoveOffset;
				flyMessage.MoveOffset += flyMessage.MoveDir * Time.deltaTime * 100f;
				if (val3.z > 0f)
				{
					_FlyMessageStyle.normal.textColor = flyMessage.TextColor;
					GUI.Label(new Rect(val3.x - 50f, (float)Screen.height - val3.y - 50f, 100f, 100f), flyMessage.Text, _FlyMessageStyle);
				}
			}
			if (Time.time >= flyMessage.EndTime || (Object)(object)flyMessage.View == (Object)null)
			{
				_FlyMessages.RemoveAt(i);
				i--;
			}
		}
	}

	private void StartSpeedHackDetection()
	{
		_SHErrorCount = 0;
		_SHOldDT = DateTime.Now;
		_SHOldTick = Environment.TickCount;
		((MonoBehaviour)this).InvokeRepeating("HeartLifeCrt", 2f, 2f);
	}

	private void HeartLifeCrt()
	{
		TimeSpan timeSpan = DateTime.Now - _SHOldDT;
		_SHOldDT = DateTime.Now;
		long num = Environment.TickCount - (long)_SHOldTick;
		_SHOldTick = Environment.TickCount;
		if (timeSpan.TotalMilliseconds * (double)_SHErrorRate < (double)num)
		{
			_SHErrorCount = (int)_SHErrorCount + 1;
		}
		if ((int)_SHErrorCount > 5)
		{
			Debug.Log((object)"SH detected");
			ExitGame();
		}
	}

	public void CreateGifts(bool dontcheck = false)
	{
	}

	public void KillPlayer()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (MainPlayerNode.Life >= 0f)
		{
			PunchPlayer(PhotonNetwork.player, Vector3.zero, 100f);
		}
	}

	public void PlaySoundForAll(SoundManager.Sound sound)
	{
		photonView.RPC("PlaySound", PhotonTargets.All, (int)sound);
	}

	[RPC]
	private void PlaySound(int sound)
	{
		SoundManager.Instance.Play((SoundManager.Sound)sound);
	}

	public IEnumerator PhotonPurchaseBuy(string shopRequest, Action<string> callback)
	{
		bool waitAnswer = true;
		Action<string> callback2 = default;
		PhotonNetwork.EventCallback ec = (byte eventCode, object content, int senderId) =>
		{
			if (eventCode == 1)
			{
				callback2((string)content);
				waitAnswer = false;
			}
		};
		PhotonNetwork.OnEventCall = (PhotonNetwork.EventCallback)Delegate.Combine(PhotonNetwork.OnEventCall, ec);
		PhotonNetwork.RaiseEvent(1, shopRequest, sendReliable: true, RaiseEventOptions.Default);
		while (waitAnswer)
		{
			yield return (object)new WaitForEndOfFrame();
		}
		PhotonNetwork.OnEventCall = (PhotonNetwork.EventCallback)Delegate.Remove(PhotonNetwork.OnEventCall, ec);
	}

	private IEnumerator CheckTimePurchasesProcess()
	{
		while (true)
		{
			App.Instance.CheckTimePurchases();
			yield return (object)new WaitForSeconds(1f);
		}
	}

	public void StartCheckCheating(string playerName)
	{
		PlayerNode playerNode = FindPlayerByName(playerName);
		if (playerNode != null)
		{
			photonView.RPC("CheckCheating", PhotonTargets.AllViaServer, playerNode.ViewerID);
		}
	}

	[RPC]
	private void CheckCheating(string cheaterID, PhotonMessageInfo info)
	{
		PlayerNode playerNode = FindPlayerByViewerID(cheaterID);
		_CheckCheating = true;
		_CheatingVoted = false;
		_CheatingVotedYes = false;
		_CheatingVotingTime = DateTime.Now;
		_CheatingPlayerName = ((playerNode == null) ? cheaterID : playerNode.Name);
		if (info.sender.isLocal)
		{
			_CheatingVoted = true;
			_CheatingVotedYes = true;
		}
		if (VkontakteAPI.INSTANCE._viewerId == cheaterID)
		{
			_CheatingVoted = true;
			_CheatingVotedYes = false;
		}
	}
}
