using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

	internal bool FirstSelect = true;

	public GUITexture gui3dview;

	public Gift giftPrefab;

	public Hud Hud;

	internal bool InitingMicrophone;

	public GUITexture aim;

	public BlockParametrs[] BlockParametrs;

	public EnityParametrs[] EnityParametrs;

	public BlockLifes[] BlockLifes;

	[NonSerialized]
	public bool ONLINE;

	public bool NOT_USE_SAVED_MAP;

	public bool HOME_LIGHT_RECULCULATE;

	public static WorldGameObject INSTANCE;

	public GameObject SelectelPrefab;

	private GameObject Selectel;

	public GameObject CubeDamagePrefab;

	[NonSerialized]
	public GameObject CubeDamage;

	private WorldData m_WorldData;

	internal World m_World;

	[NonSerialized]
	public bool IsWorldGenerated;

	private List<IDecoration> m_WorldDecorations = new List<IDecoration>();

	private Material _DefaultSkyBox;

	public Transform Chunk_Prefab;

	public Transform WaterPrefab;

	public Transform GlassPrefab;

	public GameObject Player_Prefab;

	public GameObject GUI;

	[NonSerialized]
	public GameObject MainPlayer;

	[NonSerialized]
	public Camera MainPlayerEyes;

	[NonSerialized]
	public PlayerNode MainPlayerNode;

	[NonSerialized]
	public GameObject WorldCamera;

	public Texture2D[] World_Textures;

	[NonSerialized]
	public bool bPreviewMagazinItem;

	[NonSerialized]
	public GameObject preview;

	[NonSerialized]
	private GameObject underCursorObject;

	[NonSerialized]
	public SecuredValue<int> MagazinItemId = new SecuredValue<int>();

	[NonSerialized]
	public LevelDelta levelDeltaLoading = new LevelDelta();

	[NonSerialized]
	public Texture2D WorldTextureAtlas;

	[NonSerialized]
	public Transform Player;

	private ChunkGameObject[,,,] _ChunkObjects;

	public GameObject Sparks;

	public GameObject SparksCubes1;

	public GameObject SparksCubes2;

	private Transform m_ChunksParent;

	[NonSerialized]
	public List<PlayerNode> playerList = new List<PlayerNode>();

	public GameObject GoldKubokGUI;

	public GameObject GoldKubokGUI_TEXT1;

	public GameObject GoldKubokGUI_TEXT2;

	[NonSerialized]
	public int kubok_count_in_game;

	[NonSerialized]
	public int kubok_find_in_game;

	[NonSerialized]
	public bool find_all_kubok;

	public GUIText CenterText;

	public bool MainPlayerDead;

	private int _CurZipedLevelChunk;

	private int _ZipedLevelChunkCount;

	private float _ZipedChunkReceiveTime;

	public EntityType curSelItem;

	public GameObject hit_effect;

	private bool is_lava_hit;

	private bool on_ice;

	private bool bInWater;

	private bool bFaceInWater;

	[NonSerialized]
	public float last_ladder_delta;

	[NonSerialized]
	public bool player_on_ladder;

	private bool bEnableMouseClick = true;

	[NonSerialized]
	public BlockType currentBlock = BlockType.WoodPlank;

	[NonSerialized]
	public EntityType currentBlockEntity;

	private static int _CurBlockSibling;

	private bool do_not_use_rail;

	public bool bSkipSoundRpc;

	protected List<FlyMessage> _FlyMessages = new List<FlyMessage>();

	protected GUIStyle _FlyMessageStyle;

	private SecuredValue<double> _SHErrorRate = 1.15;

	private SecuredValue<DateTime> _SHOldDT;

	private SecuredValue<long> _SHOldTick;

	private SecuredValue<int> _SHErrorCount;

	private int _CreateGiftsInterval = 10;

	private PlayerNetwork MainPlayerNetwork => MainPlayer.GetComponent<PlayerNetwork>();

	public bool IsMainPlayerDead => MainPlayerNode != null && MainPlayerNode.Life <= 0f;

	public ChunkGameObject[,,,] ChunkObjects => _ChunkObjects;

	public WorldData WorldData => m_WorldData;

	public World World => m_World;

	public int FindPlayerInd(PhotonPlayer player)
	{
		for (int i = 0; i < playerList.Count; i++)
		{
			if (playerList[i].networkPlayer == player)
			{
				return i;
			}
		}
		return -1;
	}

	public PlayerNode FindPlayer(PhotonPlayer player)
	{
		for (int i = 0; i < playerList.Count; i++)
		{
			if (playerList[i].networkPlayer == player)
			{
				return playerList[i];
			}
		}
		return null;
	}

	public PlayerNode FindPlayerByName(string name)
	{
		for (int i = 0; i < playerList.Count; i++)
		{
			if (playerList[i].playerName == name)
			{
				return playerList[i];
			}
		}
		return null;
	}

	public PlayerNode FindPlayerByAvatar(GameObject avatar)
	{
		for (int i = 0; i < playerList.Count; i++)
		{
			if ((Object)(object)playerList[i].avatar == (Object)(object)avatar)
			{
				return playerList[i];
			}
		}
		return null;
	}

	public EntityBase FindEntityByID(int id)
	{
		foreach (EntityBase entity in EntityBase.Entities)
		{
			if (((Component)entity).GetComponent<PhotonView>().viewID.ID == id)
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
		INSTANCE = this;
		MasterServer.ipAddress = "67.225.180.24";
		MasterServer.port = 23466;
		Selectel = (GameObject)Object.Instantiate((Object)(object)SelectelPrefab);
		CubeDamage = (GameObject)Object.Instantiate((Object)(object)CubeDamagePrefab);
		((Renderer)CubeDamage.GetComponent<MeshRenderer>()).enabled = false;
		WorldCamera = GameObject.Find("Camera");
		_DefaultSkyBox = RenderSettings.skybox;
		((Component)bs._Hud).gameObject.SetActiveRecursively(false);
	}

	private void Start()
	{
		App.instance.gameIni.loadingSavedMap = false;
		App.instance.gameIni.isServer = true;
		StartServer();
	}

	public void StartServer()
	{
		OnCreatedRoom();
	}

	[RPC]
	public void ExitGame()
	{
		PhotonNetwork.LeaveRoom();
		MainMenu.INSTANCE.curWindow = MainMenu.Windows.Loading;
		MainMenu.INSTANCE.loading_text = "Выходим...";
		MainMenu.INSTANCE.showMenu = true;
		Screen.lockCursor = false;
		Application.LoadLevel("Menu");
	}

	[RPC]
	public void ServerClosed()
	{
		ProfileINI.server_was_closed = true;
		Screen.lockCursor = false;
		Application.LoadLevel("Menu");
		PhotonNetwork.LeaveRoom();
	}

	public void CloseServerAndExit()
	{
		photonView.RPC("ServerClosed", PhotonTargets.Others);
		ExitGame();
	}

	[RPC]
	public void TakeBan(PhotonMessageInfo info)
	{
		if (Level.Instance.IsAdministrator(info.sender.name) || Level.Instance.IsModerator(info.sender.name))
		{
			ProfileINI.was_banned = true;
			Screen.lockCursor = false;
			Application.LoadLevel("Menu");
			PhotonNetwork.LeaveRoom();
		}
	}

	public void PlayerCanBuild(PlayerNode player, bool canBuild)
	{
		if (player != null)
		{
			if (canBuild)
			{
				Level.Instance.AddBuilder(player.playerName);
				ChatClient.instance.SendInfoMessage(player.playerName + " теперь может строить");
			}
			else
			{
				Level.Instance.RemoveBuilder(player.playerName);
				ChatClient.instance.SendWarningMessage("Игроку " + player.playerName + "- запрещено строить!");
			}
		}
	}

	public void BanPlayer(PlayerNode player)
	{
		if (player == null)
		{
			return;
		}
		if (player.viewer_id == "132371665" || player.viewer_id == "136892922" || player.viewer_id == "161889391" || player.viewer_id == "154256052" || player.viewer_id == "33353438")
		{
			ChatClient.instance.SendWarningMessage(player.playerName + " банить нельзя. Этот игрок админ");
			return;
		}
		Level.Instance.AddBanned(player.playerName);
		photonView.RPC("TakeBan", player.networkPlayer);
		ChatClient.instance.SendWarningMessage(player.playerName + " был забанен модератором");
		int num = FindPlayerInd(player.networkPlayer);
		if (num > 0)
		{
			playerList.RemoveAt(num);
		}
	}

	public void UnBanPlayer(string playerName)
	{
		if (Level.Instance.IsBanned(playerName))
		{
			Level.Instance.RemoveBanned(playerName);
			ChatClient.instance.SendInfoMessage(playerName + " разбанен модератором");
		}
	}

	public void SendMoneyToPlayer(PlayerNode player, int count)
	{
		ChatClient.instance.SendInfoMessage(ProfileINI.nickname + " перевел " + player.playerName + " " + count + " золота");
		photonView.RPC("SendMoneyRPC", player.networkPlayer, count);
	}

	[RPC]
	private void SendMoneyRPC(int count)
	{
		ProfileINI.coins += count;
	}

	public void OnPhotonPlayerDisconneced(PhotonPlayer player)
	{
		Debug.Log((object)"PhotonPlayerDisconected");
		int num = FindPlayerInd(player);
		if (num > 0)
		{
			ChatClient.instance.SendInfoMessage(player.name + " покинул нас");
			playerList.RemoveAt(num);
		}
	}

	[RPC]
	public void SendPlayerProperties(string name, int skin, string viewer_id, bool want_level, PhotonPlayer player, PhotonMessageInfo info)
	{
		if (Level.Instance.IsBanned(name))
		{
			photonView.RPC("TakeBan", info.sender);
			ChatClient.instance.SendInfoMessage(name + " хочет войти в игру, но он забанен");
			return;
		}
		PlayerNode playerNode = new PlayerNode();
		playerNode.playerName = name;
		playerNode.viewer_id = viewer_id;
		playerNode.networkPlayer = player;
		playerNode.skin = skin;
		playerList.Add(playerNode);
		if (ProfileINI.nickname == name)
		{
			MainPlayerNode = playerNode;
		}
		if (PhotonNetwork.isMasterClient && want_level)
		{
			((MonoBehaviour)this).StartCoroutine(SendLevel(info.sender));
		}
		if (Level.Instance.IsAdministrator() && !ProfileINI.newgamersislook && !Level.Instance.IsBuilder(name))
		{
			Level.Instance.AddBuilder(name);
		}
	}

	public IEnumerator SendLevel(PhotonPlayer player)
	{
		while (!IsWorldGenerated || !m_World.zipedLevel.finished)
		{
			yield return 0;
		}
		GameINI gameIni = App.instance.gameIni;
		photonView.RPC("StartLoadLevel", player, m_WorldData.ChunkBlockWidth, m_WorldData.ChunkBlockHeight, m_WorldData.ChunkBlockDepth, m_WorldData.ChunksWide, m_WorldData.ChunksHigh, m_WorldData.ChunksDeep, (byte)gameIni.mapType, m_World.zipedLevel.ziped.Length, World.ZipLevelThread.chunk_size, (int)gameIni.gameType, gameIni.destroyable, gameIni.gifts);
	}

	[RPC]
	private void StartLoadLevel(int chunkBlockWidth, int chunkBlockHeight, int chunkBlockDepth, int chunkWide, int chunkHigh, int chunkDeep, byte mapType, int zippedLevelSize, int zippedLevelChunkSize, int gametype, bool destroyable, bool gifts)
	{
		GameINI gameIni = App.instance.gameIni;
		gameIni.mapType = (GameINI.MapType)mapType;
		gameIni.gameType = (GameINI.GameType)gametype;
		gameIni.destroyable = destroyable;
		gameIni.gifts = gifts;
		InitWorldStructures(chunkBlockWidth, chunkBlockHeight, chunkBlockDepth, chunkWide, chunkHigh, chunkDeep);
		_CurZipedLevelChunk = 0;
		_ZipedLevelChunkCount = 0;
		m_World.zipedLevel.ziped = new byte[zippedLevelSize];
		m_World.zipedLevel.finished = false;
		World.ZipLevelThread.chunk_size = zippedLevelChunkSize;
		_ZipedChunkReceiveTime = Time.time;
		photonView.RPC("LoadChunk", PhotonTargets.MasterClient);
		MainMenu.INSTANCE.showMenu = true;
		MainMenu.INSTANCE.curWindow = MainMenu.Windows.Loading;
	}

	[RPC]
	public void LoadChunk(PhotonMessageInfo info)
	{
		((MonoBehaviour)this).StartCoroutine(SendLevelData(info.sender));
	}

	public IEnumerator SendLevelData(PhotonPlayer player)
	{
		List<byte[]> level_list = m_World.zipedLevel.GetCurrentZipedLevel();
		List<byte[]> delta_list = m_World.CurLevelDelta.GetCurrentLevelDelta();
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
			zipBlocks.CopyTo(m_World.zipedLevel.ziped, place * World.ZipLevelThread.chunk_size);
		}
		if (type == 1)
		{
			m_World.CurLevelDelta.Add(place * 1400, zipBlocks);
		}
		_CurZipedLevelChunk++;
		MainMenu.INSTANCE.loading_text = "Загрузка [ " + _CurZipedLevelChunk + " из " + _ZipedLevelChunkCount + " ]";
		if (_ZipedLevelChunkCount == 0 || _CurZipedLevelChunk < _ZipedLevelChunkCount || IsWorldGenerated)
		{
			return;
		}
		m_World.zipedLevel.finished = true;
		m_World.UnzippedInitialLevel = Utils.UnzipByte(m_World.zipedLevel.ziped, m_WorldData.ChunksWide * m_WorldData.ChunksHigh * m_WorldData.ChunksDeep * WorldData.Instance.m_ChunkBufferLength);
		MemoryStream input = new MemoryStream(m_World.UnzippedInitialLevel);
		BinaryReader binaryReader = new BinaryReader(input);
		for (int i = 0; i < m_WorldData.ChunksWide; i++)
		{
			for (int j = 0; j < m_WorldData.ChunksHigh; j++)
			{
				for (int k = 0; k < m_WorldData.ChunksDeep; k++)
				{
					byte[] blocksBuffer = binaryReader.ReadBytes(m_WorldData.m_ChunkBufferLength);
					m_WorldData.Chunks[i, j, k].SetBlocksBuffer(blocksBuffer);
					m_WorldData.Chunks[i, j, k].NeedsRegeneration = false;
				}
			}
		}
		m_World.CurLevelDelta.MoveToLevel();
		levelDeltaLoading.MoveToLevel();
		((MonoBehaviour)this).StartCoroutine(OnLevelRecieve());
	}

	public IEnumerator OnLevelRecieve()
	{
		_ZipedChunkReceiveTime = 0f;
		yield return ((MonoBehaviour)this).StartCoroutine(m_World.LoadWorldProcess());
		ChatClient.instance.SendInfoMessage(PhotonNetwork.playerName + " присоединился к игре");
	}

	public void RestartLevel()
	{
		m_World.CurLevelDelta = new LevelDelta();
		levelDeltaLoading = new LevelDelta();
		if (m_World.UnzippedInitialLevel != null)
		{
			MemoryStream input = new MemoryStream(m_World.UnzippedInitialLevel);
			BinaryReader binaryReader = new BinaryReader(input);
			for (int i = 0; i < m_WorldData.ChunksWide; i++)
			{
				for (int j = 0; j < m_WorldData.ChunksHigh; j++)
				{
					for (int k = 0; k < m_WorldData.ChunksDeep; k++)
					{
						byte[] blocksBuffer = binaryReader.ReadBytes(m_WorldData.m_ChunkBufferLength);
						m_WorldData.Chunks[i, j, k].SetBlocksBuffer(blocksBuffer);
						m_WorldData.Chunks[i, j, k].NeedsRegeneration = true;
					}
				}
			}
		}
		PChunkList allChunks = m_WorldData.AllChunks;
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

	public IEnumerator LoadWorldFromStorage(string player_id, int slot_id)
	{
		MainMenu.INSTANCE.showMenu = true;
		MainMenu.INSTANCE.loading_text = "Загрузка мира...";
		yield return 0;
		yield return ((MonoBehaviour)this).StartCoroutine(LoadWorldFromServer(player_id, slot_id));
		m_World.ZipLevel();
		yield return ((MonoBehaviour)this).StartCoroutine(m_World.LoadWorldProcess());
	}

	public IEnumerator LoadWorldFromServer(string playerID, int slotID)
	{
		string subDirName = VkontakteAPI.MD52(playerID).Substring(0, 2);
		string fileName = playerID.ToString() + "_" + slotID + ".map";
		Random random = new Random();
		WWW level = ((App.instance.platform != App.ePlatform.VKONTATE) ? new WWW(App.instance.GetCurScriptsUrl() + "worlds/" + subDirName + "/" + fileName + "?random=" + random.Next()) : new WWW("https://kopatel-online.ru/2012/api/worlds/" + subDirName + "/" + fileName + "?random=" + random.Next()));
		yield return level;
		if (level.text.IndexOf("<!DOCTYPE html>") >= 0 || level.error != null)
		{
			fileName = playerID.ToString() + "_" + slotID + ".map";
			random = new Random();
			level = new WWW(App.instance.GetCurSavesUrl() + "worlds/" + fileName + "?random=" + random.Next());
			yield return level;
		}
		if (level.error != null)
		{
			Debug.Log((object)"Map loaded with error!");
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
		if (format > 4)
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
			App.instance.gameIni.mapType = GameINI.MapType.STANDART;
		}
		string viewerID = "0";
		if (format >= 2)
		{
			App.instance.gameIni.mapType = (GameINI.MapType)br.ReadByte();
			viewerID = br.ReadString();
		}
		if (format >= 2 && !App.instance.gameIni.isWatch && viewerID != VkontakteAPI.INSTANCE._viewerId)
		{
			Debug.Log((object)"Stealing protection!");
			yield break;
		}
		if (format >= 3 && br.ReadBoolean())
		{
			TimeOfDay.InitializeTime(1f);
		}
		else
		{
			TimeOfDay.InitializeTime(0f);
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
		if (chunkBlockDepth != 128 || chunksDeep != 1)
		{
			InitWorldStructures(chunkBlockWidth, chunkBlockHeight, chunkBlockDepth, chunksWide, chunksHigh, chunksDeep);
			PChunkList allChunks = m_WorldData.AllChunks;
			foreach (Chunk chunk in allChunks)
			{
				chunk.SetBlocksBuffer(br.ReadBytes(m_WorldData.m_ChunkBufferLength));
				if (isLightsBuf)
				{
					br.ReadBytes(m_WorldData.m_ChunkBufferLength);
				}
			}
			allChunks.Release();
		}
		else
		{
			chunkBlockDepth = 16;
			chunksDeep = 8;
			InitWorldStructures(chunkBlockWidth, chunkBlockHeight, chunkBlockDepth, chunksWide, chunksHigh, chunksDeep);
			PChunkList allChunks2 = m_WorldData.AllChunks;
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
						m_WorldData.Chunks[x, y, z].SetBlocksBuffer(data);
					}
				}
			}
			allChunks2.Release();
		}
		int blocksSize = chunksWide * chunksHigh * chunksDeep * m_WorldData.m_ChunkBufferLength;
		br.BaseStream.Seek(-blocksSize, SeekOrigin.Current);
		m_World.UnzippedInitialLevel = new byte[blocksSize];
		br.Read(m_World.UnzippedInitialLevel, 0, blocksSize);
		int entityBufferLength = br.ReadInt32();
		byte[] entityBuffer = br.ReadBytes(entityBufferLength);
		yield return ((MonoBehaviour)this).StartCoroutine(m_World.SetEntityBuffer(entityBuffer));
	}

	public void InitWorldStructures(int ch_Bw, int ch_Bh, int ch_Bd, int ch_w, int ch_h, int ch_d)
	{
		m_WorldData = new WorldData();
		m_WorldData.SetDimensions(ch_w, ch_h, ch_d, ch_Bw, ch_Bh, ch_Bd);
		m_ChunksParent = ((Component)this).transform.FindChild("Chunks");
		m_WorldDecorations = new List<IDecoration>
		{
			new StandardTreeDecorator(m_WorldData)
		};
		ITerrainGenerationMethod terrainGenerationMethod = new FortressWarStandartTerrain();
		if (App.instance.gameIni.mapType == GameINI.MapType.STANDART)
		{
			terrainGenerationMethod = new FortressWarStandartTerrain();
		}
		if (App.instance.gameIni.mapType == GameINI.MapType.FLAT)
		{
			terrainGenerationMethod = new FlatLand();
		}
		if (App.instance.gameIni.mapType == GameINI.MapType.SAND)
		{
			terrainGenerationMethod = new SandLand();
		}
		if (App.instance.gameIni.mapType == GameINI.MapType.OCEAN)
		{
			terrainGenerationMethod = new OceanLand();
		}
		if (App.instance.gameIni.mapType == GameINI.MapType.ISLAND)
		{
			terrainGenerationMethod = new IslandLand();
		}
		if (App.instance.gameIni.mapType == GameINI.MapType.SNOWLAND)
		{
			terrainGenerationMethod = new SnowLand();
			m_WorldDecorations = new List<IDecoration>
			{
				new SnowTreeDecorator(m_WorldData)
			};
		}
		m_World = new World(WorldData, new TerrainGenerator(WorldData, new BatchProcessor<Chunk>(), terrainGenerationMethod), new LightProcessor(WorldData), new MeshGenerator(new BatchProcessor<Chunk>(), WorldData), new WorldDecorator(WorldData, new BatchProcessor<Chunk>(), m_WorldDecorations));
		m_World.InitializeGridChunks();
		InitializeTextures();
		_ChunkObjects = new ChunkGameObject[3, WorldData.ChunksWide, WorldData.ChunksHigh, WorldData.ChunksDeep];
	}

	public void GrowServerRating()
	{
		if (App.instance.gameIni.ServerRating < 250)
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("rating", (byte)(App.instance.gameIni.ServerRating + 1));
			PhotonNetwork.room.SetCustomProperties(hashtable);
			App.instance.gameIni.ServerRating++;
		}
	}

	public void LowerServerRating()
	{
		if (App.instance.gameIni.ServerRating > 0)
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("rating", (byte)(App.instance.gameIni.ServerRating - 1));
			PhotonNetwork.room.SetCustomProperties(hashtable);
			App.instance.gameIni.ServerRating--;
		}
	}

	private void OnCreatedRoom()
	{
		if (!App.instance.gameIni.loadingSavedMap)
		{
			if (App.instance.gameIni.mapSize == GameINI.MapSize.SMALL)
			{
				InitWorldStructures(16, 16, 16, 8, 8, 8);
			}
			if (App.instance.gameIni.mapSize == GameINI.MapSize.MEDIUM)
			{
				InitWorldStructures(16, 16, 16, 14, 14, 14);
			}
			((MonoBehaviour)this).StartCoroutine(m_World.GenerateWorldCourutin());
		}
		else
		{
			((MonoBehaviour)this).StartCoroutine(LoadWorldFromStorage(App.instance.gameIni.playerID, App.instance.gameIni.slotID));
		}
	}

	public void SetNamesForAllPlayers()
	{
		for (int i = 0; i < playerList.Count; i++)
		{
			if ((Object)(object)playerList[i].avatar != (Object)null)
			{
				playerList[i].avatar.GetComponent<SetLabelOnMessage>().SetLabel(playerList[i].playerName);
			}
		}
	}

	public void SetSkinsForAllPlayers()
	{
		for (int i = 0; i < playerList.Count; i++)
		{
			if ((Object)(object)playerList[i].avatar != (Object)null)
			{
				playerList[i].avatar.GetComponent<SkinManager>().SetSkin(playerList[i].skin);
			}
		}
	}

	private void SetAmbientSoundOnLevel()
	{
		if (!ResourceLoader.I.SoundsBundleLoaded)
		{
			((MonoBehaviour)this).Invoke("SetAmbientSoundOnLevel", 1f);
			return;
		}
		if (App.instance.gameIni.mapType == GameINI.MapType.FLAT)
		{
			((Component)INSTANCE.MainPlayerEyes).gameObject.audio.clip = ResourceLoader.I.GetClip(ResourceLoader.eSounds.LoopForest_1);
		}
		if (App.instance.gameIni.mapType == GameINI.MapType.ISLAND)
		{
			((Component)INSTANCE.MainPlayerEyes).gameObject.audio.clip = ResourceLoader.I.GetClip(ResourceLoader.eSounds.LoopForest_1);
		}
		if (App.instance.gameIni.mapType == GameINI.MapType.STANDART)
		{
			((Component)INSTANCE.MainPlayerEyes).gameObject.audio.clip = ResourceLoader.I.GetClip(ResourceLoader.eSounds.LoopForest_1);
		}
		if (App.instance.gameIni.mapType == GameINI.MapType.SNOWLAND)
		{
			((Component)INSTANCE.MainPlayerEyes).gameObject.audio.clip = ResourceLoader.I.GetClip(ResourceLoader.eSounds.LoopSnow_1);
		}
		if (App.instance.gameIni.mapType == GameINI.MapType.SAND)
		{
			((Component)INSTANCE.MainPlayerEyes).gameObject.audio.clip = ResourceLoader.I.GetClip(ResourceLoader.eSounds.LoopDesert_1);
		}
		if (App.instance.gameIni.mapType == GameINI.MapType.OCEAN)
		{
			((Component)INSTANCE.MainPlayerEyes).gameObject.audio.clip = ResourceLoader.I.GetClip(ResourceLoader.eSounds.LoopOcean_1);
		}
		((Component)INSTANCE.MainPlayerEyes).gameObject.audio.loop = true;
		((Component)INSTANCE.MainPlayerEyes).gameObject.audio.volume = ProfileINI.ambient_volume;
		((Component)INSTANCE.MainPlayerEyes).gameObject.audio.Play();
	}

	public Vector3 GenerateSpawnPoint()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)TeamBattle.Instance != (Object)null)
		{
			return TeamBattle.Instance.GenerateSpawnPoint();
		}
		SpawnArrow[] array = Object.FindObjectsOfType(typeof(SpawnArrow)) as SpawnArrow[];
		SpawnArrow[] array2 = array;
		foreach (SpawnArrow spawnArrow in array2)
		{
			if (spawnArrow.Type == EntityType.SPAWN_ARROW)
			{
				return new Vector3((float)(int)((Component)spawnArrow).transform.position.x, (float)(int)((Component)spawnArrow).transform.position.y, (float)(int)((Component)spawnArrow).transform.position.z);
			}
		}
		return new Vector3((float)(m_WorldData.WidthInBlocks / 2), (float)m_WorldData.DepthInBlocks, (float)(m_WorldData.HeightInBlocks / 2));
	}

	public void CompleteWorldGeneration()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected Obj, but got Unknown
		WorldCamera.active = false;
		MainMenu.INSTANCE.showMenu = false;
		IsWorldGenerated = true;
		MainPlayer = (GameObject)Object.Instantiate((Object)(object)Player_Prefab, GenerateSpawnPoint(), ((Component)this).transform.rotation);
		((Object)MainPlayer).name = "MAIN_PLAYER";
		MainPlayerEyes = ((Component)MainPlayer.transform.FindChild("recoil/Main Camera")).GetComponentInChildren<Camera>();
		SetNamesForAllPlayers();
		SetSkinsForAllPlayers();
		MainMenu.INSTANCE.SpeedBonus();
		ProfileINI.tutorial_watch = 1;
		ShowGameGui();
		MainMenu.INSTANCE.curWindow = MainMenu.Windows.Game;
		MainMenu.INSTANCE.showMenu = false;
		EnableMouseLook();
		Screen.lockCursor = true;
		SetAmbientSoundOnLevel();
		EntityBase.OnWorldGenerated();
	}

	public void ShowGameGui()
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		GUI.SetActiveRecursively(true);
		if (ProfileINI.tutorial_watch == 0 && (Object)(object)TeamBattle.Instance == (Object)null)
		{
			DisableMouseLook();
			Screen.lockCursor = false;
			MainMenu.INSTANCE.curWindow = MainMenu.Windows.TutorialImages;
			MainMenu.INSTANCE.showMenu = true;
		}
		GUITexture component = GameObject.Find("hit_effect").GetComponent<GUITexture>();
		Resolution currentResolution = Screen.currentResolution;
		float num = -currentResolution.width / 2;
		Resolution currentResolution2 = Screen.currentResolution;
		float num2 = -currentResolution2.height / 2;
		Resolution currentResolution3 = Screen.currentResolution;
		float num3 = currentResolution3.width;
		Resolution currentResolution4 = Screen.currentResolution;
		component.pixelInset = new Rect(num, num2, num3, (float)currentResolution4.height);
		UpdateKubokText();
	}

	public void UpdateKubokText()
	{
		if (kubok_count_in_game > 0)
		{
			GoldKubokGUI.SetActiveRecursively(true);
			if (Level.Instance.IsAdministrator())
			{
				GoldKubokGUI_TEXT1.GetComponent<GUIText>().text = "Вы спрятали кубки на карте.";
				GoldKubokGUI_TEXT2.GetComponent<GUIText>().text = "Всего " + kubok_count_in_game + " кубков";
				return;
			}
			GoldKubokGUI_TEXT1.GetComponent<GUIText>().text = "На карте есть золотые кубки";
			GoldKubokGUI_TEXT2.GetComponent<GUIText>().text = "Найдено " + kubok_find_in_game + " из " + kubok_count_in_game;
			if (kubok_find_in_game > kubok_count_in_game)
			{
				GoldKubokGUI.SetActiveRecursively(false);
			}
		}
		else
		{
			GoldKubokGUI.SetActiveRecursively(false);
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
		val2.parent = m_ChunksParent;
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

	public bool IsMenuOpen()
	{
		return Store.INSTANCE.showMenu || MainMenu.INSTANCE.showMenu || TabMenu.Instance.showMenu || SpeechBubbles.ShowMenu || Book.IsAnyOpened || ((Object)(object)TeamBattle.Instance != (Object)null && TeamBattle.Instance.MenuEnabled) || InitingMicrophone;
	}

	private void UpdateStorePreview()
	{
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Unknown result type (might be due to invalid IL or missing references)
		//IL_054c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0464: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0475: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		if (!bPreviewMagazinItem)
		{
			return;
		}
		EntityType entityType = (EntityType)(int)MagazinItemId;
		switch (entityType)
		{
		case EntityType.BELLSMALL:
		case EntityType.WEAT:
		case EntityType.DOORW:
		case EntityType.DOORM:
		case EntityType.FISH_1:
		case EntityType.FISH_2:
		case EntityType.FISH_3:
		case EntityType.BOAT:
		case EntityType.TROLLEY:
		case EntityType.CAT:
		case EntityType.DOG:
		case EntityType.CHICKEN:
		case EntityType.BOOK:
			if (Input.GetAxis("Mouse ScrollWheel") < 0f)
			{
				iTween.RotateAdd(preview, new Vector3(0f, 90f, 0f), 1f);
			}
			else if (Input.GetAxis("Mouse ScrollWheel") > 0f)
			{
				iTween.RotateAdd(preview, new Vector3(0f, -90f, 0f), 1f);
			}
			break;
		default:
			if (Input.GetAxis("Mouse ScrollWheel") < 0f)
			{
				iTween.RotateAdd(preview, new Vector3(0f, 0f, 90f), 1f);
			}
			else if (Input.GetAxis("Mouse ScrollWheel") > 0f)
			{
				iTween.RotateAdd(preview, new Vector3(0f, 0f, -90f), 1f);
			}
			break;
		case EntityType.TABLICHKAW:
		case EntityType.LADDER:
		case EntityType.PICTURE1_1:
		case EntityType.PAPER:
		case EntityType.PICTURE2_1:
		case EntityType.PICTURE1_2:
		case EntityType.PICTURE3_2:
			break;
		}
		if ((Object)(object)CameraController.Instance == (Object)null)
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
		BlockType blockType = m_WorldData.GetBlockType(intVect.X, intVect.Y, intVect.Z);
		switch (entityType)
		{
		case EntityType.TABLICHKAW:
		case EntityType.LADDER:
		case EntityType.PICTURE1_1:
		case EntityType.PAPER:
		case EntityType.PICTURE2_1:
		case EntityType.PICTURE1_2:
		case EntityType.PICTURE3_2:
		{
			ProcessTablichkaWAdd(hit, out var pos2, out var rotate);
			preview.transform.localEulerAngles = rotate;
			preview.transform.position = pos2;
			break;
		}
		case EntityType.TORCH_FLOOR:
		case EntityType.TORCH_FLOOR_STONE:
		{
			ProcessTorchFloorAdd(hit, out var pos);
			iTween.MoveTo(preview, pos, 0.5f);
			break;
		}
		case EntityType.TORCH:
		case EntityType.TORCH_STONE:
		{
			ProcessTorchAdd(hit, out var pos3, out var rotate2);
			preview.transform.localEulerAngles = rotate2;
			iTween.MoveTo(preview, pos3, 0.5f);
			break;
		}
		case EntityType.ATOMBOMB:
			val2 = hit.point + hit.normal * 0.01f;
			intVect = new IntVect((int)val2.x, (int)val2.z, (int)val2.y);
			iTween.MoveTo(preview, new Vector3((float)intVect.X + 0.5f, (float)intVect.Z + 0.5f, (float)intVect.Y + 0.5f), 0.5f);
			break;
		case EntityType.BOAT:
			if (blockType == BlockType.Water)
			{
				val2 = hit.point + hit.normal * 0.01f;
				intVect = new IntVect((int)val2.x, (int)val2.z, (int)val2.y);
				iTween.MoveTo(preview, new Vector3((float)intVect.X + 0.5f, (float)intVect.Z + 1f, (float)intVect.Y + 0.5f), 0.5f);
			}
			else
			{
				val2 = hit.point + hit.normal * 0.01f;
				intVect = new IntVect((int)val2.x, (int)val2.z, (int)val2.y);
				iTween.MoveTo(preview, new Vector3((float)intVect.X + 0.5f, (float)intVect.Z + 0.25f, (float)intVect.Y + 0.5f), 0.5f);
			}
			break;
		default:
			val2 = hit.point + hit.normal * 0.01f;
			intVect = new IntVect((int)val2.x, (int)val2.z, (int)val2.y);
			iTween.MoveTo(preview, new Vector3((float)intVect.X + 0.5f, (float)intVect.Z, (float)intVect.Y + 0.5f), 0.5f);
			break;
		}
		MainMenu.INSTANCE.is_cursor_text = true;
		MainMenu.INSTANCE.cursor_take = false;
		MainMenu.INSTANCE.cursor_sit = false;
		if (entityType != EntityType.TORCH && entityType != EntityType.TORCH_STONE)
		{
			MainMenu.INSTANCE.cursor_wheel = true;
		}
		MainMenu.INSTANCE.cursor_put = true;
		MainMenu.INSTANCE.cursor_activate = false;
		MainMenu.INSTANCE.cursor_write = false;
		MainMenu.INSTANCE.cursor_draw = false;
		MainMenu.INSTANCE.cursor_text = EnityParametrs[(int)entityType].name;
	}

	private void UpdateSelectelAndCursorText()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)CameraController.Instance == (Object)null)
		{
			return;
		}
		Ray val = CameraController.RaycastCamera.ScreenPointToRay(new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f, 0f));
		int num = (1 << LayerMask.NameToLayer("SmallDecor")) | (1 << LayerMask.NameToLayer("Entity"));
		int num2 = 1 << LayerMask.NameToLayer("Terrain");
		RaycastHit val2 = default;
		if (Physics.Raycast(val, ref val2, CameraController.RaycastDistance, num))
		{
			EntityBase entityBase = EntityBase.FindEntity(((Component)val2.transform).gameObject);
			((Renderer)Selectel.GetComponent<MeshRenderer>()).enabled = false;
			MainMenu.INSTANCE.is_cursor_text = true;
			MainMenu.INSTANCE.cursor_text = entityBase.Name;
			curSelItem = entityBase.Type;
			if (curSelItem == EntityType.DOORM || curSelItem == EntityType.DOORW)
			{
				MainMenu.INSTANCE.cursor_take = true;
				MainMenu.INSTANCE.cursor_sit = false;
				MainMenu.INSTANCE.cursor_open = true;
				MainMenu.INSTANCE.cursor_write = false;
				MainMenu.INSTANCE.cursor_activate = false;
				MainMenu.INSTANCE.cursor_draw = false;
			}
			else if (curSelItem == EntityType.TABLICHKAF || curSelItem == EntityType.TABLICHKAW || curSelItem == EntityType.BOOK)
			{
				MainMenu.INSTANCE.cursor_take = true;
				MainMenu.INSTANCE.cursor_sit = false;
				MainMenu.INSTANCE.cursor_open = false;
				MainMenu.INSTANCE.cursor_write = true;
				MainMenu.INSTANCE.cursor_activate = false;
				MainMenu.INSTANCE.cursor_draw = false;
			}
			else if (curSelItem == EntityType.TORCH || curSelItem == EntityType.TORCH_FLOOR || curSelItem == EntityType.TORCH_STONE || curSelItem == EntityType.TORCH_FLOOR_STONE)
			{
				MainMenu.INSTANCE.cursor_take = true;
				MainMenu.INSTANCE.cursor_sit = false;
				MainMenu.INSTANCE.cursor_open = false;
				MainMenu.INSTANCE.cursor_write = false;
				MainMenu.INSTANCE.cursor_activate = false;
				MainMenu.INSTANCE.cursor_draw = false;
			}
			else if (curSelItem == EntityType.TNT || curSelItem == EntityType.ATOMBOMB)
			{
				MainMenu.INSTANCE.cursor_take = true;
				MainMenu.INSTANCE.cursor_sit = false;
				MainMenu.INSTANCE.cursor_open = false;
				MainMenu.INSTANCE.cursor_write = false;
				MainMenu.INSTANCE.cursor_activate = true;
				MainMenu.INSTANCE.cursor_draw = false;
			}
			else if (curSelItem == EntityType.PICTURE3_2 || curSelItem == EntityType.PICTURE1_2 || curSelItem == EntityType.PICTURE2_1 || curSelItem == EntityType.PICTURE1_1 || curSelItem == EntityType.PAPER)
			{
				MainMenu.INSTANCE.cursor_take = true;
				MainMenu.INSTANCE.cursor_sit = false;
				MainMenu.INSTANCE.cursor_draw = true;
				MainMenu.INSTANCE.cursor_open = false;
				MainMenu.INSTANCE.cursor_write = false;
				MainMenu.INSTANCE.cursor_activate = false;
			}
			else if (curSelItem == EntityType.BOAT || curSelItem == EntityType.TROLLEY || curSelItem == EntityType.BED || curSelItem == EntityType.BED_STONE || curSelItem == EntityType.CHAIR || curSelItem == EntityType.CHAIR_STONE || curSelItem == EntityType.BENCH_WOOD || curSelItem == EntityType.BENCH_STONE)
			{
				MainMenu.INSTANCE.cursor_take = true;
				MainMenu.INSTANCE.cursor_sit = true;
				MainMenu.INSTANCE.cursor_draw = false;
				MainMenu.INSTANCE.cursor_open = false;
				MainMenu.INSTANCE.cursor_write = false;
				MainMenu.INSTANCE.cursor_activate = false;
			}
			else
			{
				MainMenu.INSTANCE.cursor_take = true;
				MainMenu.INSTANCE.cursor_sit = false;
				MainMenu.INSTANCE.cursor_open = false;
				MainMenu.INSTANCE.cursor_write = false;
				MainMenu.INSTANCE.cursor_activate = false;
				MainMenu.INSTANCE.cursor_draw = false;
			}
			MainMenu.INSTANCE.cursor_put = false;
			MainMenu.INSTANCE.cursor_wheel = false;
			underCursorObject = ((Component)entityBase).gameObject;
		}
		else if (Physics.Raycast(val, ref val2, CameraController.RaycastDistance, num2))
		{
			((Renderer)Selectel.GetComponent<MeshRenderer>()).enabled = true;
			Vector3 val3 = val2.point - val2.normal * 0.5f;
			IntVect intVect = new IntVect((int)Math.Floor(val3.x), (int)Math.Floor(val3.z), (int)Math.Floor(val3.y));
			Selectel.transform.localScale = new Vector3(1.01f, 1.01f, 1.01f);
			Selectel.transform.position = new Vector3((float)intVect.X + 0.5f, (float)intVect.Z + 0.5f, (float)intVect.Y + 0.5f);
		}
		else
		{
			((Renderer)Selectel.GetComponent<MeshRenderer>()).enabled = false;
		}
	}

	public void LavaDamageSound()
	{
		if (!IsMainPlayerDead)
		{
			MainPlayerNode.LavaDamage();
			ResourceLoader.I.PlayEffect(ResourceLoader.eSounds.FootLava_1, MainPlayer.audio);
			((MonoBehaviour)this).Invoke("LavaDamageSound", 0.6f);
		}
	}

	private void SetLavaCollision()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_0479: Unknown result type (might be due to invalid IL or missing references)
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_050d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0512: Unknown result type (might be due to invalid IL or missing references)
		//IL_0528: Unknown result type (might be due to invalid IL or missing references)
		//IL_052d: Unknown result type (might be due to invalid IL or missing references)
		//IL_054c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0551: Unknown result type (might be due to invalid IL or missing references)
		//IL_0567: Unknown result type (might be due to invalid IL or missing references)
		//IL_056c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0582: Unknown result type (might be due to invalid IL or missing references)
		//IL_0587: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0600: Unknown result type (might be due to invalid IL or missing references)
		//IL_0605: Unknown result type (might be due to invalid IL or missing references)
		//IL_061b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0620: Unknown result type (might be due to invalid IL or missing references)
		//IL_0630: Unknown result type (might be due to invalid IL or missing references)
		//IL_0635: Unknown result type (might be due to invalid IL or missing references)
		//IL_0652: Unknown result type (might be due to invalid IL or missing references)
		//IL_0657: Unknown result type (might be due to invalid IL or missing references)
		//IL_066d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0672: Unknown result type (might be due to invalid IL or missing references)
		//IL_0682: Unknown result type (might be due to invalid IL or missing references)
		//IL_0687: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06be: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_070b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0710: Unknown result type (might be due to invalid IL or missing references)
		//IL_0726: Unknown result type (might be due to invalid IL or missing references)
		//IL_072b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0748: Unknown result type (might be due to invalid IL or missing references)
		//IL_074d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0763: Unknown result type (might be due to invalid IL or missing references)
		//IL_0768: Unknown result type (might be due to invalid IL or missing references)
		//IL_077e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0783: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07db: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0813: Unknown result type (might be due to invalid IL or missing references)
		//IL_0818: Unknown result type (might be due to invalid IL or missing references)
		//IL_082e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0833: Unknown result type (might be due to invalid IL or missing references)
		//IL_0850: Unknown result type (might be due to invalid IL or missing references)
		//IL_0855: Unknown result type (might be due to invalid IL or missing references)
		//IL_086b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0870: Unknown result type (might be due to invalid IL or missing references)
		//IL_0886: Unknown result type (might be due to invalid IL or missing references)
		//IL_088b: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0915: Unknown result type (might be due to invalid IL or missing references)
		//IL_091a: Unknown result type (might be due to invalid IL or missing references)
		//IL_092a: Unknown result type (might be due to invalid IL or missing references)
		//IL_092f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0952: Unknown result type (might be due to invalid IL or missing references)
		//IL_0957: Unknown result type (might be due to invalid IL or missing references)
		//IL_096d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0972: Unknown result type (might be due to invalid IL or missing references)
		//IL_0982: Unknown result type (might be due to invalid IL or missing references)
		//IL_0987: Unknown result type (might be due to invalid IL or missing references)
		//IL_09aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_09af: Unknown result type (might be due to invalid IL or missing references)
		//IL_09bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_09da: Unknown result type (might be due to invalid IL or missing references)
		//IL_09df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a02: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a07: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a17: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a32: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a37: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a75: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a90: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a95: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0abd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b16: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b31: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b36: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b4c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b51: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b74: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b79: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b8f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b94: Unknown result type (might be due to invalid IL or missing references)
		//IL_0baa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0baf: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)MainPlayer) || ((Object)(object)TeamBattle.Instance != (Object)null && TeamBattle.Instance.IsObserving()))
		{
			return;
		}
		Camera mainPlayerEyes = MainPlayerEyes;
		BlockType blockType = m_WorldData.GetBlockType((int)((Component)mainPlayerEyes).transform.position.x, (int)((Component)mainPlayerEyes).transform.position.z, (int)((Component)mainPlayerEyes).transform.position.y - 2);
		BlockType blockType2 = m_WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x - 0.6f), (int)((Component)mainPlayerEyes).transform.position.z, (int)((Component)mainPlayerEyes).transform.position.y - 1);
		BlockType blockType3 = m_WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x + 0.6f), (int)((Component)mainPlayerEyes).transform.position.z, (int)((Component)mainPlayerEyes).transform.position.y - 1);
		BlockType blockType4 = m_WorldData.GetBlockType((int)((Component)mainPlayerEyes).transform.position.x, (int)(((Component)mainPlayerEyes).transform.position.z + 0.6f), (int)((Component)mainPlayerEyes).transform.position.y - 1);
		BlockType blockType5 = m_WorldData.GetBlockType((int)((Component)mainPlayerEyes).transform.position.x, (int)(((Component)mainPlayerEyes).transform.position.z - 0.6f), (int)((Component)mainPlayerEyes).transform.position.y - 1);
		BlockType blockType6 = m_WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x - 0.6f), (int)(((Component)mainPlayerEyes).transform.position.z + 0.6f), (int)((Component)mainPlayerEyes).transform.position.y - 1);
		BlockType blockType7 = m_WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x + 0.6f), (int)(((Component)mainPlayerEyes).transform.position.z - 0.6f), (int)((Component)mainPlayerEyes).transform.position.y - 1);
		BlockType blockType8 = m_WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x - 0.6f), (int)(((Component)mainPlayerEyes).transform.position.z - 0.6f), (int)((Component)mainPlayerEyes).transform.position.y - 1);
		BlockType blockType9 = m_WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x + 0.6f), (int)(((Component)mainPlayerEyes).transform.position.z + 0.6f), (int)((Component)mainPlayerEyes).transform.position.y - 1);
		BlockType blockType10 = m_WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x - 0.6f), (int)((Component)mainPlayerEyes).transform.position.z, (int)((Component)mainPlayerEyes).transform.position.y - 2);
		BlockType blockType11 = m_WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x + 0.6f), (int)((Component)mainPlayerEyes).transform.position.z, (int)((Component)mainPlayerEyes).transform.position.y - 2);
		BlockType blockType12 = m_WorldData.GetBlockType((int)((Component)mainPlayerEyes).transform.position.x, (int)(((Component)mainPlayerEyes).transform.position.z + 0.6f), (int)((Component)mainPlayerEyes).transform.position.y - 2);
		BlockType blockType13 = m_WorldData.GetBlockType((int)((Component)mainPlayerEyes).transform.position.x, (int)(((Component)mainPlayerEyes).transform.position.z - 0.6f), (int)((Component)mainPlayerEyes).transform.position.y - 2);
		BlockType blockType14 = m_WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x - 0.6f), (int)(((Component)mainPlayerEyes).transform.position.z + 0.6f), (int)((Component)mainPlayerEyes).transform.position.y - 2);
		BlockType blockType15 = m_WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x + 0.6f), (int)(((Component)mainPlayerEyes).transform.position.z - 0.6f), (int)((Component)mainPlayerEyes).transform.position.y - 2);
		BlockType blockType16 = m_WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x - 0.6f), (int)(((Component)mainPlayerEyes).transform.position.z - 0.6f), (int)((Component)mainPlayerEyes).transform.position.y - 2);
		BlockType blockType17 = m_WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x + 0.6f), (int)(((Component)mainPlayerEyes).transform.position.z + 0.6f), (int)((Component)mainPlayerEyes).transform.position.y - 2);
		BlockType blockType18 = m_WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x - 0.6f), (int)((Component)mainPlayerEyes).transform.position.z, (int)((Component)mainPlayerEyes).transform.position.y);
		BlockType blockType19 = m_WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x + 0.6f), (int)((Component)mainPlayerEyes).transform.position.z, (int)((Component)mainPlayerEyes).transform.position.y);
		BlockType blockType20 = m_WorldData.GetBlockType((int)((Component)mainPlayerEyes).transform.position.x, (int)(((Component)mainPlayerEyes).transform.position.z + 0.6f), (int)((Component)mainPlayerEyes).transform.position.y);
		BlockType blockType21 = m_WorldData.GetBlockType((int)((Component)mainPlayerEyes).transform.position.x, (int)(((Component)mainPlayerEyes).transform.position.z - 0.6f), (int)((Component)mainPlayerEyes).transform.position.y);
		BlockType blockType22 = m_WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x - 0.6f), (int)(((Component)mainPlayerEyes).transform.position.z + 0.6f), (int)((Component)mainPlayerEyes).transform.position.y);
		BlockType blockType23 = m_WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x + 0.6f), (int)(((Component)mainPlayerEyes).transform.position.z - 0.6f), (int)((Component)mainPlayerEyes).transform.position.y);
		BlockType blockType24 = m_WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x + 0.6f), (int)(((Component)mainPlayerEyes).transform.position.z + 0.6f), (int)((Component)mainPlayerEyes).transform.position.y);
		BlockType blockType25 = m_WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x - 0.6f), (int)(((Component)mainPlayerEyes).transform.position.z - 0.6f), (int)((Component)mainPlayerEyes).transform.position.y);
		BlockType blockType26 = m_WorldData.GetBlockType((int)((Component)mainPlayerEyes).transform.position.x, (int)((Component)mainPlayerEyes).transform.position.z, (int)(((Component)mainPlayerEyes).transform.position.y + 1f));
		BlockType blockType27 = m_WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x - 0.6f), (int)((Component)mainPlayerEyes).transform.position.z, (int)(((Component)mainPlayerEyes).transform.position.y + 1f));
		BlockType blockType28 = m_WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x + 0.6f), (int)((Component)mainPlayerEyes).transform.position.z, (int)(((Component)mainPlayerEyes).transform.position.y + 1f));
		BlockType blockType29 = m_WorldData.GetBlockType((int)((Component)mainPlayerEyes).transform.position.x, (int)(((Component)mainPlayerEyes).transform.position.z + 0.6f), (int)(((Component)mainPlayerEyes).transform.position.y + 1f));
		BlockType blockType30 = m_WorldData.GetBlockType((int)((Component)mainPlayerEyes).transform.position.x, (int)(((Component)mainPlayerEyes).transform.position.z - 0.6f), (int)(((Component)mainPlayerEyes).transform.position.y + 1f));
		BlockType blockType31 = m_WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x - 0.6f), (int)(((Component)mainPlayerEyes).transform.position.z + 0.6f), (int)(((Component)mainPlayerEyes).transform.position.y + 1f));
		BlockType blockType32 = m_WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x + 0.6f), (int)(((Component)mainPlayerEyes).transform.position.z - 0.6f), (int)(((Component)mainPlayerEyes).transform.position.y + 1f));
		BlockType blockType33 = m_WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x + 0.6f), (int)(((Component)mainPlayerEyes).transform.position.z + 0.6f), (int)(((Component)mainPlayerEyes).transform.position.y + 1f));
		BlockType blockType34 = m_WorldData.GetBlockType((int)(((Component)mainPlayerEyes).transform.position.x - 0.6f), (int)(((Component)mainPlayerEyes).transform.position.z - 0.6f), (int)(((Component)mainPlayerEyes).transform.position.y + 1f));
		if (blockType34 == BlockType.Lava || blockType33 == BlockType.Lava || blockType32 == BlockType.Lava || blockType31 == BlockType.Lava || blockType30 == BlockType.Lava || blockType29 == BlockType.Lava || blockType28 == BlockType.Lava || blockType27 == BlockType.Lava || blockType25 == BlockType.Lava || blockType24 == BlockType.Lava || blockType23 == BlockType.Lava || blockType22 == BlockType.Lava || blockType17 == BlockType.Lava || blockType16 == BlockType.Lava || blockType15 == BlockType.Lava || blockType14 == BlockType.Lava || blockType13 == BlockType.Lava || blockType12 == BlockType.Lava || blockType11 == BlockType.Lava || blockType10 == BlockType.Lava || blockType9 == BlockType.Lava || blockType8 == BlockType.Lava || blockType7 == BlockType.Lava || blockType6 == BlockType.Lava || blockType26 == BlockType.Lava || blockType == BlockType.Lava || blockType2 == BlockType.Lava || blockType3 == BlockType.Lava || blockType4 == BlockType.Lava || blockType5 == BlockType.Lava || blockType18 == BlockType.Lava || blockType19 == BlockType.Lava || blockType20 == BlockType.Lava || blockType21 == BlockType.Lava)
		{
			if (!is_lava_hit)
			{
				iTween.FadeTo(hit_effect, 0.5f, 0.05f);
				((MonoBehaviour)this).CancelInvoke("LavaDamageSound");
				LavaDamageSound();
			}
			is_lava_hit = true;
		}
		else
		{
			if (is_lava_hit)
			{
				((MonoBehaviour)this).CancelInvoke("LavaDamageSound");
				iTween.FadeTo(hit_effect, 0f, 0.05f);
			}
			is_lava_hit = false;
		}
	}

	private void SetOnIceMovement()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)MainPlayer))
		{
			BlockType blockType = m_WorldData.GetBlockType((int)((Component)MainPlayerEyes).transform.position.x, (int)((Component)MainPlayerEyes).transform.position.z, (int)((Component)MainPlayerEyes).transform.position.y - 2);
			if (blockType == BlockType.Ice)
			{
				INSTANCE.MainPlayer.SendMessage("SetIceBody", (object)true);
				on_ice = true;
			}
			else if (!on_ice)
			{
				INSTANCE.MainPlayer.SendMessage("SetIceBody", (object)false);
			}
			else if (blockType != BlockType.Air)
			{
				INSTANCE.MainPlayer.SendMessage("SetIceBody", (object)false);
			}
		}
	}

	private void SetUnderWaterEffect()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)MainPlayer))
		{
			return;
		}
		Camera raycastCamera = CameraController.RaycastCamera;
		BlockType blockType = m_WorldData.GetBlockType((int)((Component)raycastCamera).transform.position.x, (int)(((Component)raycastCamera).transform.position.z + 0.15f), (int)((Component)raycastCamera).transform.position.y);
		if (blockType == BlockType.Water)
		{
			RenderSettings.fogColor = TimeOfDay.WaterFogColor;
			RenderSettings.fogDensity = 0.04f;
			RenderSettings.skybox = null;
			((Behaviour)((Component)((Component)raycastCamera).transform).GetComponent<ColorCorrectionCurves>()).enabled = true;
			if (!bFaceInWater)
			{
				if (ResourceLoader.I.SoundsBundleLoaded)
				{
					((Component)MainPlayerEyes).gameObject.audio.clip = ResourceLoader.I.GetClip(ResourceLoader.eSounds.LoopUnderwater_1);
					((Component)MainPlayerEyes).gameObject.audio.loop = true;
					((Component)MainPlayerEyes).gameObject.audio.volume = ProfileINI.ambient_volume;
					((Component)MainPlayerEyes).gameObject.audio.Play();
					ProfileINI.sound_scale = 0.25f;
				}
				bFaceInWater = true;
			}
		}
		else
		{
			RenderSettings.fogDensity = 0.007f;
			RenderSettings.fogColor = TimeOfDay.FogColor;
			RenderSettings.skybox = _DefaultSkyBox;
			((Behaviour)((Component)((Component)raycastCamera).transform).GetComponent<ColorCorrectionCurves>()).enabled = false;
			if (bFaceInWater)
			{
				ProfileINI.sound_scale = 1f;
				ResourceLoader.I.PlayEffect(ResourceLoader.eSounds.WaterOut_1, MainPlayer.gameObject.audio);
				SetAmbientSoundOnLevel();
				bFaceInWater = false;
			}
		}
		Transform transform = INSTANCE.MainPlayer.transform;
		int num = (int)transform.position.x;
		int num2 = (int)transform.position.z;
		int num3 = (int)transform.position.y;
		blockType = m_WorldData.GetBlockType(num, num2, num3);
		if (blockType == BlockType.Water)
		{
			bool flag = false;
			float num4 = (float)num3 + 1f;
			if (m_WorldData.GetBlockType(num, num2, num3 + 1) == BlockType.Water)
			{
				num4++;
			}
			if (num4 - transform.position.y < 0.7f)
			{
				bool flag2 = false;
				for (int i = -1; i <= 1; i++)
				{
					for (int j = -1; j <= 1; j++)
					{
						BlockType blockType2 = m_WorldData.GetBlockType(num + i, num2 + j, num3);
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
			INSTANCE.MainPlayer.SendMessage("SetUnderWaterBody", (object)new bool[2] { true, flag });
		}
		else
		{
			INSTANCE.MainPlayer.SendMessage("SetUnderWaterBody", (object)new bool[2]);
		}
		if (blockType == BlockType.Water && !bInWater)
		{
			ResourceLoader.I.PlayEffect(ResourceLoader.eSounds.WaterIn_1, MainPlayer.gameObject.audio);
			bInWater = true;
		}
		else if (blockType != BlockType.Water && bInWater)
		{
			ResourceLoader.I.PlayEffect(ResourceLoader.eSounds.WaterOut_1, MainPlayer.gameObject.audio);
			bInWater = false;
		}
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
		m_World = null;
	}

	public void CreateNewMap()
	{
		if (m_World.IsHomeWorld)
		{
			MainMenu.INSTANCE.showMenu = true;
			MainMenu.INSTANCE.curWindow = MainMenu.Windows.Loading;
			ClearEntities();
			IsWorldGenerated = false;
			((MonoBehaviour)this).StartCoroutine(m_World.GenerateWorldCourutin());
		}
	}

	public IEnumerator AddMoney()
	{
		ChatClient.instance.SendInfoMessage(ProfileINI.nickname + " нашел 5 золота");
		ProfileINI.coins += 5;
		WWWForm form = new WWWForm();
		form.AddField("viewer_id", VkontakteAPI.INSTANCE._viewerId);
		form.AddField("auth_key", VkontakteAPI.INSTANCE._authKey);
		if (App.instance.platform == App.ePlatform.ONDOKLASNIKI)
		{
			form.AddField("session_key", VkontakteAPI.INSTANCE._sessionKey);
		}
		Random r = new Random();
		form.AddField("random", r.Next());
		WWW php_load = new WWW(App.instance.GetCurScriptsUrl() + "add_gold.php", form);
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
				INSTANCE.MainPlayer.SendMessage("SetLadderBody", (object)false);
				MainMenu.INSTANCE.FlyBonus();
			}
		}
	}

	private void LateUpdate()
	{
	}

	private void Update()
	{
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238->IL024e: Incompatible stack heights: 2 vs 0
		if (bs._Igor.debug && Input.GetKeyDown((KeyCode)306))
		{
			photonView.RPC("Pause", PhotonTargets.All);
		}
		if (IsWorldGenerated)
		{
			MainMenu.INSTANCE.is_cursor_text = false;
			LadderFlyTest();
			if (!App.instance.gameIni.isWatch)
			{
				if (Input.GetKeyUp((KeyCode)9) && !bs._MainMenu.showMenu)
				{
					TabMenu.Instance.showMenu = false;
					Screen.lockCursor = true;
					EnableMouseLook();
				}
				if (Input.GetKeyDown((KeyCode)9))
				{
					TabMenu.Instance.showMenu = true;
					Screen.lockCursor = false;
					DisableMouseLook();
				}
			}
			else if (Input.GetKeyDown((KeyCode)9))
			{
				if (!MainMenu.INSTANCE.showMenu)
				{
					Screen.lockCursor = false;
					DisableMouseLook();
					MainMenu.INSTANCE.showMenu = true;
					MainMenu.INSTANCE.curWindow = MainMenu.Windows.MapCard;
				}
				else if (MainMenu.INSTANCE.showMenu && MainMenu.INSTANCE.curWindow == MainMenu.Windows.MapCard)
				{
					Screen.lockCursor = true;
					EnableMouseLook();
					MainMenu.INSTANCE.showMenu = false;
				}
			}
			if (Input.GetKeyDown((KeyCode)27))
			{
				if ((MainMenu.INSTANCE.curWindow == MainMenu.Windows.TutorialImages && ProfileINI.tutorial_watch == 0) || (MainMenu.INSTANCE.showMenu && MainMenu.INSTANCE.curWindow == MainMenu.Windows.DeathMenu) || MainPlayerDead)
				{
					return;
				}
				if (bEnableMouseClick)
				{
					DisableMouseLook();
					Screen.lockCursor = false;
					MainMenu.INSTANCE.showMenu = true;
					MainMenu.INSTANCE.curWindow = MainMenu.Windows.Game;
				}
				else
				{
					EnableMouseLook();
					Screen.lockCursor = true;
					MainMenu.INSTANCE.showMenu = false;
				}
			}
			if (Input.GetKeyDown((KeyCode)323))
			{
				if (!IsMenuOpen())
				{
					Screen.lockCursor = true;
					EnableMouseLook();
				}
				else
				{
					Screen.lockCursor = false;
				}
			}
			if ((Object)(object)MainPlayer != (Object)null)
			{
				_ = MainPlayer.transform.position.y;
				_ = 0f;
			}
			SetUnderWaterEffect();
			SetOnIceMovement();
			SetLavaCollision();
			UpdateStorePreview();
			UpdateSelectelAndCursorText();
			if (Input.GetKeyDown((KeyCode)101))
			{
				if (bPreviewMagazinItem)
				{
					SetEntityFromPreview();
					return;
				}
				if (MainMenu.INSTANCE.is_cursor_text)
				{
					underCursorObject.GetComponent<EntityBase>().OnButtonE(ProfileINI.nickname);
					MainMenu.INSTANCE.is_cursor_text = false;
				}
			}
			if (Input.GetKeyDown((KeyCode)102) && !bPreviewMagazinItem)
			{
				if (MainMenu.INSTANCE.is_cursor_text)
				{
					underCursorObject.GetComponent<EntityBase>().OnButtonF(ProfileINI.nickname);
					MainMenu.INSTANCE.is_cursor_text = false;
				}
				else if ((Object)(object)MainPlayer != (Object)null && !((Collider)MainPlayer.GetComponent<CharacterController>()).enabled && !ChatClient.isEnabled)
				{
					Trolley.KickPlayer(MainPlayer);
					Boat.KickPlayer(MainPlayer);
				}
			}
			m_World.RegenerateChunks();
			CreateFinishedChunk();
			ProcessPlayerInput();
			DisplayDiggings();
		}
		else if (_ZipedChunkReceiveTime != 0f && Time.time - _ZipedChunkReceiveTime > 5f)
		{
			Debug.Log((object)"Exit waiting level data");
			if (!App.instance.gameIni.isServer)
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

	private void InitializeTextures()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Expected Obj, but got Unknown
		WorldTextureAtlas = new Texture2D(2048, 2048);
		WorldData.WorldTextureAtlasUvs = WorldTextureAtlas.PackTextures(World_Textures, 0);
		((Texture)WorldTextureAtlas).filterMode = (FilterMode)0;
		((Texture)WorldTextureAtlas).anisoLevel = 9;
		WorldTextureAtlas.Apply();
		WorldData.GenerateUVCoordinatesForAllBlocks();
	}

	public void EnableMouseLook()
	{
		if ((Object)(object)MainPlayer != (Object)null)
		{
			((Behaviour)MainPlayer.GetComponent<MouseLook>()).enabled = true;
			((Behaviour)((Component)MainPlayer.transform.Find("recoil/Main Camera")).GetComponent<MouseLook>()).enabled = true;
			bEnableMouseClick = true;
		}
	}

	public void DisableMouseLook()
	{
		if ((Object)(object)MainPlayer != (Object)null)
		{
			((Behaviour)MainPlayer.GetComponent<MouseLook>()).enabled = false;
			((Behaviour)((Component)MainPlayer.transform.Find("recoil/Main Camera")).GetComponent<MouseLook>()).enabled = false;
			bEnableMouseClick = false;
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
	}

	private void ProcessTablichkaWAdd(RaycastHit hit, out Vector3 pos, out Vector3 rotate)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = hit.point + hit.normal * 0.01f;
		IntVect intVect = new IntVect((int)val.x, (int)val.z, (int)val.y);
		pos = new Vector3((float)intVect.X + 0.5f, (float)intVect.Z, (float)intVect.Y + 0.5f);
		Vector3 normal = hit.normal;
		if (normal.Equals((object)new Vector3(0f, 0f, -1f)))
		{
			Debug.Log((object)new Vector3(0f, 0f, -1f));
			rotate = new Vector3(0f, 90f, 0f);
			return;
		}
		Vector3 normal2 = hit.normal;
		if (normal2.Equals((object)new Vector3(0f, 0f, 1f)))
		{
			Debug.Log((object)new Vector3(0f, 0f, 1f));
			rotate = new Vector3(0f, 270f, 0f);
			return;
		}
		Vector3 normal3 = hit.normal;
		if (normal3.Equals((object)new Vector3(-1f, 0f, 0f)))
		{
			Debug.Log((object)new Vector3(-1f, 0f, 0f));
			rotate = new Vector3(0f, 180f, 0f);
			return;
		}
		Vector3 normal4 = hit.normal;
		if (normal4.Equals((object)new Vector3(1f, 0f, 0f)))
		{
			Debug.Log((object)new Vector3(1f, 0f, 0f));
			rotate = new Vector3(0f, 0f, 0f);
			return;
		}
		Vector3 normal5 = hit.normal;
		if (normal5.Equals((object)new Vector3(0f, 1f, 0f)))
		{
			rotate = new Vector3(0f, 0f, 90f);
			pos += new Vector3(0.5f, 0.5f, 0f);
			return;
		}
		Vector3 normal6 = hit.normal;
		if (normal6.Equals((object)new Vector3(0f, -1f, 0f)))
		{
			rotate = new Vector3(0f, 0f, 270f);
			pos += new Vector3(-0.5f, 0.5f, 0f);
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

	public void PressCubeMenu()
	{
		if ((Object)(object)TeamBattle.Instance != (Object)null)
		{
			return;
		}
		Store.INSTANCE.showMenu = !Store.INSTANCE.showMenu;
		if (Store.INSTANCE.showMenu)
		{
			if (bPreviewMagazinItem)
			{
				SetEntityFromPreview();
			}
			DisableMouseLook();
			Screen.lockCursor = false;
			if (MainMenu.INSTANCE.showMenu)
			{
				MainMenu.INSTANCE.showMenu = false;
			}
		}
		else
		{
			MainMenu.INSTANCE.showMenu = false;
			EnableMouseLook();
			Screen.lockCursor = true;
		}
	}

	private void ProcessPlayerInput()
	{
		if (!Input.anyKey || (MainMenu.INSTANCE.showMenu && MainMenu.INSTANCE.curWindow == MainMenu.Windows.DeathMenu) || MainPlayerDead)
		{
			return;
		}
		if (Input.GetKeyDown((KeyCode)99) && !App.instance.gameIni.isWatch && !ChatClient.isEnabled)
		{
			PressCubeMenu();
		}
		if (Input.GetKeyDown((KeyCode)113) && !App.instance.gameIni.isWatch && !ChatClient.isEnabled)
		{
			SpeechBubbles.ShowMenu = !SpeechBubbles.ShowMenu;
		}
		if (Input.GetKeyDown((KeyCode)109) && !App.instance.gameIni.isWatch && !ChatClient.isEnabled)
		{
			if (MainMenu.INSTANCE.showMenu)
			{
				EnableMouseLook();
				Screen.lockCursor = true;
				MainMenu.INSTANCE.showMenu = false;
			}
			else
			{
				Store.INSTANCE.showMenu = false;
				DisableMouseLook();
				Screen.lockCursor = false;
				MainMenu.INSTANCE.OpenMebelMenu();
			}
		}
		if (Input.GetKeyDown((KeyCode)291) && Level.Instance.IsAdministrator() && !App.instance.gameIni.isWatch && GameTypeUtils.CanSaveMap())
		{
			if (MainMenu.INSTANCE.showMenu)
			{
				EnableMouseLook();
				Screen.lockCursor = true;
				MainMenu.INSTANCE.showMenu = false;
			}
			else
			{
				Store.INSTANCE.showMenu = false;
				DisableMouseLook();
				Screen.lockCursor = false;
				MainMenu.INSTANCE.OpenSaveMenu();
			}
		}
	}

	public void OnBuildTerrain()
	{
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0496: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		Debug.Log((object)"OnBuildTerrain");
		do_not_use_rail = false;
		if (App.instance.gameIni.isWatch)
		{
			ChatClient.instance.SendWarningMessage("В режиме просмотра строительство запрещено");
		}
		else if (!Level.Instance.CanBuild)
		{
			ChatClient.instance.Warning("Изменять карту нельзя, обратитесь к администратору карты, чтобы получить возможность строить");
		}
		else
		{
			if ((Object)(object)CameraController.Instance == (Object)null || (currentBlock == BlockType.Air && currentBlockEntity == EntityType.AIR))
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
			if (currentBlock != BlockType.Air)
			{
				if (BuildCheckCollisionWithPlayer(hit) && !((Object)(object)Rail.Find(intVect.X, intVect.Y, intVect.Z) != (Object)null))
				{
					BlockType blockType = m_WorldData.GetBlockType(intVect.X, intVect.Y, intVect.Z);
					if (blockType == BlockType.Water && currentBlock == BlockType.Water)
					{
						photonView.RPC("AddBlockAt", PhotonTargets.All, (int)currentBlock, intVect.X, intVect.Y, intVect.Z + 1);
					}
					else
					{
						photonView.RPC("AddBlockAt", PhotonTargets.All, (int)currentBlock, intVect.X, intVect.Y, intVect.Z);
					}
				}
				return;
			}
			if (currentBlockEntity == EntityType.RAIL)
			{
				if (!Level.Instance.IsAdministrator() || App.instance.gameIni.isWatch)
				{
					ChatClient.instance.Warning("Рельсы можно устанавливать только на своей карте");
					return;
				}
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
							if (m_WorldData.GetBlockType(intVect.X + num2, intVect.Y + num3, intVect.Z + num4) != BlockType.Air)
							{
								rail.SelfDelete();
								rail = null;
								val2 = new Vector3((float)intVect.X + 0.5f, (float)intVect.Z, (float)intVect.Y + 0.5f);
								val2 += Rail.PlanePosition[_CurBlockSibling];
								ProfileINI.SetPurchesValue(StorePurchase.RAIL, ProfileINI.GetPurchesValue(StorePurchase.RAIL) + 1);
								Debug.Log((object)"RAIL+1");
								do_not_use_rail = true;
								break;
							}
						}
					}
					if ((Object)(object)rail != (Object)null)
					{
						return;
					}
				}
				else if (m_WorldData.GetBlockType(intVect.X, intVect.Y, intVect.Z) == BlockType.Water)
				{
					return;
				}
				if (ProfileINI.GetPurchesValue(StorePurchase.RAIL) <= 0)
				{
					return;
				}
				if (!do_not_use_rail)
				{
					((MonoBehaviour)MainMenu.INSTANCE).StartCoroutine(MainMenu.INSTANCE.UsePurches(StorePurchase.RAIL));
				}
				ProfileINI.SetPurchesValue(StorePurchase.RAIL, ProfileINI.GetPurchesValue(StorePurchase.RAIL) - 1);
			}
			photonView.RPC("AddEntityNetwork", PhotonTargets.MasterClient, (int)currentBlockEntity, val2, Quaternion.identity, ProfileINI.nickname, null);
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
		for (int i = 0; i < playerList.Count; i++)
		{
			if (!((Object)(object)playerList[i].avatar == (Object)null))
			{
				Vector3 val = playerList[i].avatar.transform.position - hit.point;
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
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)CameraController.Instance == (Object)null)
		{
			return;
		}
		Ray val = CameraController.RaycastCamera.ScreenPointToRay(new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f, 0f));
		RaycastHit val2 = default;
		int num;
		if ((Object)(object)TeamBattle.Instance != (Object)null && !TeamBattle.Instance.IsObserving())
		{
			num = 1 << LayerMask.NameToLayer("Players");
			if (Physics.Raycast(val, ref val2, CameraController.RaycastDistance, num))
			{
				if ((Object)(object)((Component)val2.collider).gameObject != (Object)(object)MainPlayer)
				{
					PlayerNode playerNode = FindPlayerByAvatar(((Component)val2.collider).gameObject);
					if (playerNode != null && TeamBattle.Instance.IsOpposite(playerNode.networkPlayer.name))
					{
						MainPlayer.GetComponent<PhotonView>().RPC("OnHit", PhotonTargets.All, MainPlayer.transform.forward, MainPlayer.transform.position);
						PunchPlayer(playerNode.networkPlayer, MainPlayer.transform.forward, 0.5f, MainPlayerNetwork.Knife.ArrayId);
					}
				}
				return;
			}
		}
		num = (1 << LayerMask.NameToLayer("Entity")) | (1 << LayerMask.NameToLayer("SmallDecor"));
		if (Physics.Raycast(val, ref val2, CameraController.RaycastDistance, num))
		{
			EntityBase.FindEntity(((Component)val2.transform).gameObject).OnLeftMouseHit(ProfileINI.nickname);
			return;
		}
		num = 1 << LayerMask.NameToLayer("Terrain");
		if (!GameTypeUtils.DigDisabled() && Physics.Raycast(val, ref val2, CameraController.RaycastDistance, num))
		{
			if (App.instance.gameIni.isWatch)
			{
				ChatClient.instance.SendWarningMessage("В режиме просмотра разрушение запрещено");
				return;
			}
			Vector3 point = val2.point;
			Vector3 direction = val.direction;
			Vector3 val3 = point + direction.normalized * 0.01f;
			IntVect targetLocation = new IntVect((int)val3.x, (int)val3.z, (int)val3.y);
			m_World.Dig(targetLocation, val2.point, val2.normal);
		}
		else
		{
			ResourceLoader.I.PlayEffect(ResourceLoader.eSounds.WeaponSwish_2, INSTANCE.MainPlayer.gameObject.audio);
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
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)MainPlayer != (Object)null)
		{
			ResourceLoader.I.PlayEffectAtPoint((ResourceLoader.eSounds)sound_type, MainPlayer.audio, sound_pos);
		}
	}

	[RPC]
	private void RemoveBlockAt(int x, int y, int z)
	{
		if (IsWorldGenerated)
		{
			m_World.RemoveBlockAt(new IntVect(x, y, z), silent: false);
		}
		else
		{
			levelDeltaLoading.Add(BlockType.Air, x, y, z, checkAvulsion: true);
		}
	}

	[RPC]
	private void RemoveBlocksAt(int x, int y, int z, int radius)
	{
		if (IsWorldGenerated)
		{
			m_World.RemoveBlocksAt(new IntVect(x, y, z), radius);
			return;
		}
		for (int i = -radius; i <= radius; i++)
		{
			for (int j = -radius; j <= radius; j++)
			{
				for (int k = -radius; k <= radius; k++)
				{
					if (WorldData.Instance.IsValidBlock(x + i, y + j, z + k))
					{
						levelDeltaLoading.Add(BlockType.Air, x + i, y + j, z + k, checkAvulsion: true);
					}
				}
			}
		}
	}

	[RPC]
	private void AddBlockAt(int id, int x, int y, int z)
	{
		if (IsWorldGenerated)
		{
			m_World.AddBlockAt(new IntVect(x, y, z), (BlockType)id);
		}
		else
		{
			levelDeltaLoading.Add((BlockType)id, x, y, z);
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
	public void AddEntityNetwork(int type, Vector3 pos, Quaternion rot, string creator, object[] data)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		ResourceLoader.I.PlayEffect(ResourceLoader.eSounds.SetDecor_1, INSTANCE.MainPlayer.audio);
		m_World.AddEntityNetwork((EntityType)type, pos, rot, creator, data);
	}

	[RPC]
	public void TakeEntityNetwork(int id, PhotonMessageInfo info)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		EntityBase entityBase = FindEntityByID(id);
		if ((Object)(object)entityBase != (Object)null)
		{
			photonView.RPC("AddPreviewEntity", info.sender, (int)entityBase.Type, ((Component)entityBase).transform.position, ((Component)entityBase).transform.rotation, entityBase.GetData());
			entityBase.SelfDelete();
		}
	}

	[RPC]
	public void AddPreviewEntity(int type, Vector3 pos, Quaternion rot, object[] data)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		AddPreview((EntityType)type, pos, rot, data);
	}

	[RPC]
	public void DeleteEntityNetworkAll(int id)
	{
		if (!PhotonNetwork.isMasterClient)
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
	public void DeleteEntityNetworkMasterClient(int id)
	{
		EntityBase entityBase = FindEntityByID(id);
		if ((Object)(object)entityBase != (Object)null)
		{
			EntityBase.Entities.Remove(entityBase);
			((Component)entityBase).SendMessage("Destruction");
			PhotonNetwork.Destroy(((Component)entityBase).gameObject);
		}
	}

	public void TakeEnity(GameObject entity)
	{
		if (!App.instance.gameIni.isWatch)
		{
			if (Level.Instance.CanBuild)
			{
				photonView.RPC("TakeEntityNetwork", PhotonTargets.MasterClient, entity.GetComponent<PhotonView>().viewID.ID);
			}
			else
			{
				ChatClient.instance.Warning("Изменять карту нельзя, обратитесь к администратору карты, чтобы получить возможность строить");
			}
		}
		else
		{
			ChatClient.instance.SendWarningMessage("В режиме просмотра изменять карту нельзя");
		}
	}

	public void SetEntityFromPreview()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (bPreviewMagazinItem)
		{
			photonView.RPC("AddEntityNetwork", PhotonTargets.MasterClient, (int)MagazinItemId, preview.transform.position, preview.transform.rotation, ProfileINI.nickname, preview.GetComponent<EntityBase>().GetData());
			Object.Destroy((Object)(object)preview);
			bPreviewMagazinItem = false;
		}
	}

	public void AddPreview(EntityType ind, Vector3 pos, Quaternion rot, object[] data)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected Obj, but got Unknown
		MagazinItemId = (int)ind;
		bPreviewMagazinItem = true;
		preview = (GameObject)Object.Instantiate((Object)(object)EnityParametrs[(int)ind].gameObject, pos, rot);
		EntityBase component = preview.GetComponent<EntityBase>();
		component.Type = ind;
		component.Name = EnityParametrs[(int)ind].name;
		Utils.SetLayerRecursively(preview, LayerMask.NameToLayer("StorePreview"));
		component.InitializePreview(data);
	}

	public void AddPreview(EntityType ind)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		AddPreview(ind, MainPlayer.transform.position, EnityParametrs[(int)ind].gameObject.transform.rotation, null);
	}

	private void DisplayDiggings()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (m_World.Diggings.Count != 0)
		{
			Vector3 val = m_World.Diggings.Dequeue();
			Object.Instantiate((Object)(object)Sparks, val, Quaternion.identity);
		}
	}

	public void OnMainPlayerDead()
	{
		if ((Object)(object)TeamBattle.Instance != (Object)null)
		{
			TeamBattle.Instance.OnDead(PhotonNetwork.playerName);
		}
		((MonoBehaviour)this).StartCoroutine(MainPlayerDeadProcess());
	}

	private IEnumerator MainPlayerDeadProcess()
	{
		CameraController cameraController = MainPlayer.GetComponent<CameraController>();
		cameraController.SetThirdPerson(freeLook: true, 45f, saveSettings: false);
		cameraController.DisableInput = true;
		MainPlayer.SendMessage("SetMovement", (object)false);
		float waitDuration = 3f;
		if ((Object)(object)MainPlayer.animation != (Object)null && (TrackedReference)(object)MainPlayer.animation["death"] != (TrackedReference)null)
		{
			MainPlayer.animation.CrossFade("death");
			MainPlayer.GetComponent<NetworkSyncAnimation>().SyncAnimation(NetworkSyncAnimation.AnimState.death);
			waitDuration += MainPlayer.animation["death"].length;
		}
		yield return (object)new WaitForSeconds(waitDuration);
		if (Store.INSTANCE.showMenu)
		{
			PressCubeMenu();
		}
		if (SpeechBubbles.ShowMenu)
		{
			SpeechBubbles.ShowMenu = false;
		}
		MainPlayerDead = true;
		CenterText.text = "нажмите огонь для возрождения";
		((Behaviour)CenterText).enabled = true;
		((MonoBehaviour)this).StartCoroutine(bs.AddUpdate(() =>
		{
			if (Input.GetKeyDown((KeyCode)323) && Screen.lockCursor)
			{
				((Behaviour)CenterText).enabled = false;
				MainMenu.INSTANCE.CleanScreenMode(mode: false);
				Respawn();
			}
			return MainPlayerDead;
		}));
	}

	public void Respawn()
	{
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		((Behaviour)CenterText).enabled = false;
		MainPlayerDead = false;
		if (MainPlayerNode != null)
		{
			MainPlayerNode.Life = 1f;
			MainPlayerNode.Shield = 0f;
			if (!((Collider)MainPlayer.GetComponent<CharacterController>()).enabled)
			{
				Trolley.KickPlayer(MainPlayer);
				Boat.KickPlayer(MainPlayer);
			}
			MainPlayer.SendMessage("SetMovement", (object)true);
			CameraController component = MainPlayer.GetComponent<CameraController>();
			component.DisableInput = false;
			component.SetSavedCamera();
			MainPlayer.transform.position = GenerateSpawnPoint();
			Gun[] guns = ((Component)component).GetComponent<PlayerNetwork>().Guns;
			foreach (Gun gun in guns)
			{
				gun.Reset();
			}
			component.Reset();
			EnableMouseLook();
			Screen.lockCursor = true;
			MonoBehaviour.print((object)"Respawn");
			MainMenu.INSTANCE.showMenu = false;
		}
		MainPlayer.GetComponent<PlayerNetwork>().Reset(die: false);
		photonView.RPC("RespawnRPC", PhotonTargets.Others, PhotonNetwork.player, MainPlayer.transform.position);
	}

	[RPC]
	public void RespawnRPC(PhotonPlayer player, Vector3 spawnPoint)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		PlayerNode playerNode = FindPlayer(player);
		if (playerNode != null)
		{
			playerNode.avatar.transform.position = spawnPoint;
		}
		playerNode.avatar.GetComponent<PlayerNetwork>().Reset(die: false);
	}

	public void PunchPlayer(PhotonPlayer player, Vector3 dir, float damage = 0.5f, int gunId = -1)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		bool flag = true;
		bool flag2 = true;
		photonView.RPC("HitRedness", PhotonTargets.All, player);
		FindPlayer(player).avatar.GetComponent<PhotonView>().RPC("HitBy", PhotonTargets.All, PhotonNetwork.player, gunId);
		photonView.RPC("ReceiveDamage", player, dir, damage, flag, flag2);
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
	private void ReceiveDamage(Vector3 dir, float damage, bool jolt, bool spurn, PhotonMessageInfo info)
	{
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		if (bs._Igor.spectate || Time.time - MainPlayer.GetComponent<PlayerNetwork>().ResetTime < bs.immportalTime || MainPlayerNode.Life <= 0f)
		{
			return;
		}
		MainPlayerNode.DoDamage(damage);
		if ((Object)(object)TeamBattle.Instance != (Object)null && MainPlayerNode.Life <= 0f)
		{
			TeamBattle.Instance.OnKill(info.sender.name);
		}
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
		if (bs._Igor.spectate)
		{
			return;
		}
		PlayerNode playerNode = FindPlayer(player);
		if (playerNode != null && (Object)(object)playerNode.avatar != (Object)null)
		{
			playerNode.avatar.GetComponent<bs>().PlayOneShot(bs._Igor.damageSound[Random.Range(0, bs._Igor.damageSound.Length)]);
			SkinManager component = playerNode.avatar.GetComponent<SkinManager>();
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
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
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
						val = val2 + bounds.size.y * ((Component)flyMessage.View).transform.localScale.y * ((Component)flyMessage.View).transform.up;
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

	public void CreateGifts()
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		if (PhotonNetwork.isMasterClient && Object.FindObjectsOfType(typeof(Gift)).Length < 20)
		{
			Vector3 position = new Vector3((float)Random.Range(0, WorldData.WidthInBlocks - 1), (float)WorldData.DepthInBlocks, (float)Random.Range(0, WorldData.HeightInBlocks - 1)) + Vector3.one / 2f;
			int num = Random.Range(0, 4);
			string prefabName = (new string[4] { "gift_ammo", "gift_armor", "gift_grenade", "gift_life" })[num];
			GameObject val = PhotonNetwork.InstantiateSceneObject(prefabName, position, Quaternion.identity, 0, new object[3]
			{
				78 + num,
				string.Empty,
				false
			});
			val.GetComponent<PhotonView>().RPC("Drop", PhotonTargets.All);
		}
		((MonoBehaviour)this).Invoke("CreateGifts", (float)_CreateGiftsInterval);
	}
}
