using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

public static class PhotonNetwork
{
	public delegate void EventCallback(byte eventCode, object content, int senderId);

	public const string versionPUN = "1.25";

	public const string serverSettingsAssetFile = "PhotonServerSettings";

	public const string serverSettingsAssetPath = "Assets/Photon Unity Networking/Resources/PhotonServerSettings.asset";

	internal static readonly PhotonHandler photonMono;

	internal static NetworkingPeer networkingPeer;

	public static readonly int MAX_VIEW_IDS;

	public static ServerSettings PhotonServerSettings;

	public static float precisionForVectorSynchronization;

	public static float precisionForQuaternionSynchronization;

	public static float precisionForFloatSynchronization;

	public static bool InstantiateInRoomOnly;

	public static PhotonLogLevel logLevel;

	public static bool UsePrefabCache;

	public static Dictionary<string, GameObject> PrefabCache;

	private static bool isOfflineMode;

	private static bool offlineMode_inRoom;

	public static bool UseNameServer;

	public static HashSet<GameObject> SendMonoMessageTargets;

	private static bool _mAutomaticallySyncScene;

	private static bool m_autoCleanUpPlayerObjects;

	private static bool autoJoinLobbyField;

	private static int sendInterval;

	private static int sendIntervalOnSerialize;

	private static bool m_isMessageQueueRunning;

	public static EventCallback OnEventCall;

	internal static int lastUsedViewSubId;

	internal static int lastUsedViewSubIdStatic;

	internal static List<int> manuallyAllocatedViewIds;

	public static string ServerAddress => (networkingPeer == null) ? "<not connected>" : networkingPeer.ServerAddress;

	public static bool connected
	{
		get
		{
			if (offlineMode)
			{
				return true;
			}
			if (networkingPeer == null)
			{
				return false;
			}
			return !networkingPeer.IsInitialConnect && networkingPeer.State != PeerState.PeerCreated && networkingPeer.State != PeerState.Disconnected && networkingPeer.State != PeerState.Disconnecting && networkingPeer.State != PeerState.ConnectingToNameServer;
		}
	}

	public static bool connecting => networkingPeer.IsInitialConnect && !offlineMode;

	public static bool connectedAndReady
	{
		get
		{
			if (!connected)
			{
				return false;
			}
			if (offlineMode)
			{
				return true;
			}
			switch (connectionStateDetailed)
			{
			case PeerState.PeerCreated:
			case PeerState.ConnectingToGameserver:
			case PeerState.Joining:
			case PeerState.Leaving:
			case PeerState.ConnectingToMasterserver:
			case PeerState.Disconnecting:
			case PeerState.Disconnected:
			case PeerState.ConnectingToNameServer:
			case PeerState.Authenticating:
				return false;
			default:
				return true;
			}
		}
	}

	public static ConnectionState connectionState
	{
		get
		{
			if (offlineMode)
			{
				return ConnectionState.Connected;
			}
			if (networkingPeer == null)
			{
				return ConnectionState.Disconnected;
			}
			return networkingPeer.PeerState switch
			{
				PeerStateValue.Disconnected => ConnectionState.Disconnected, 
				PeerStateValue.Connecting => ConnectionState.Connecting, 
				PeerStateValue.Connected => ConnectionState.Connected, 
				PeerStateValue.Disconnecting => ConnectionState.Disconnecting, 
				PeerStateValue.InitializingApplication => ConnectionState.InitializingApplication, 
				_ => ConnectionState.Disconnected, 
			};
		}
	}

	public static PeerState connectionStateDetailed
	{
		get
		{
			if (offlineMode)
			{
				return (!offlineMode_inRoom) ? PeerState.ConnectedToMaster : PeerState.Joined;
			}
			if (networkingPeer == null)
			{
				return PeerState.Disconnected;
			}
			return networkingPeer.State;
		}
	}

	public static AuthenticationValues AuthValues
	{
		get
		{
			return (networkingPeer == null) ? null : networkingPeer.CustomAuthenticationValues;
		}
		set
		{
			if (networkingPeer != null)
			{
				networkingPeer.CustomAuthenticationValues = value;
			}
		}
	}

	public static Room room
	{
		get
		{
			if (isOfflineMode)
			{
				if (offlineMode_inRoom)
				{
					return new Room("OfflineRoom", null);
				}
				return null;
			}
			return networkingPeer.mCurrentGame;
		}
	}

	public static PhotonPlayer player
	{
		get
		{
			if (networkingPeer == null)
			{
				return null;
			}
			return networkingPeer.mLocalActor;
		}
	}

	public static PhotonPlayer masterClient
	{
		get
		{
			if (networkingPeer == null)
			{
				return null;
			}
			return networkingPeer.mMasterClient;
		}
	}

	public static string playerName
	{
		get
		{
			return networkingPeer.PlayerName;
		}
		set
		{
			networkingPeer.PlayerName = value;
		}
	}

	public static PhotonPlayer[] playerList
	{
		get
		{
			if (networkingPeer == null)
			{
				return new PhotonPlayer[0];
			}
			return networkingPeer.mPlayerListCopy;
		}
	}

	public static PhotonPlayer[] otherPlayers
	{
		get
		{
			if (networkingPeer == null)
			{
				return new PhotonPlayer[0];
			}
			return networkingPeer.mOtherPlayerListCopy;
		}
	}

	public static List<FriendInfo> Friends { get; set; }

	public static int FriendsListAge => (networkingPeer != null) ? networkingPeer.FriendsListAge : 0;

	public static bool offlineMode
	{
		get
		{
			return isOfflineMode;
		}
		set
		{
			if (value == isOfflineMode)
			{
				return;
			}
			if (value && connected)
			{
				Debug.LogError((object)"Can't start OFFLINE mode while connected!");
				return;
			}
			networkingPeer.Disconnect();
			isOfflineMode = value;
			if (isOfflineMode)
			{
				NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnConnectedToPhoton);
				networkingPeer.ChangeLocalID(1);
				networkingPeer.mMasterClient = player;
			}
			else
			{
				networkingPeer.ChangeLocalID(-1);
				networkingPeer.mMasterClient = null;
			}
		}
	}

	[Obsolete("Used for compatibility with Unity networking only.")]
	public static int maxConnections
	{
		get
		{
			if (room == null)
			{
				return 0;
			}
			return room.maxPlayers;
		}
		set
		{
			room.maxPlayers = value;
		}
	}

	public static bool automaticallySyncScene
	{
		get
		{
			return _mAutomaticallySyncScene;
		}
		set
		{
			_mAutomaticallySyncScene = value;
			if (_mAutomaticallySyncScene && room != null)
			{
				networkingPeer.LoadLevelIfSynced();
			}
		}
	}

	public static bool autoCleanUpPlayerObjects
	{
		get
		{
			return m_autoCleanUpPlayerObjects;
		}
		set
		{
			if (room != null)
			{
				Debug.LogError((object)"Setting autoCleanUpPlayerObjects while in a room is not supported.");
			}
			else
			{
				m_autoCleanUpPlayerObjects = value;
			}
		}
	}

	public static bool autoJoinLobby
	{
		get
		{
			return autoJoinLobbyField;
		}
		set
		{
			autoJoinLobbyField = value;
		}
	}

	public static bool insideLobby => networkingPeer.insideLobby;

	public static TypedLobby lobby
	{
		get
		{
			return networkingPeer.lobby;
		}
		set
		{
			networkingPeer.lobby = value;
		}
	}

	public static int sendRate
	{
		get
		{
			return 1000 / sendInterval;
		}
		set
		{
			sendInterval = 1000 / value;
			if ((Object)(object)photonMono != (Object)null)
			{
				photonMono.updateInterval = sendInterval;
			}
			if (value < sendRateOnSerialize)
			{
				sendRateOnSerialize = value;
			}
		}
	}

	public static int sendRateOnSerialize
	{
		get
		{
			return 1000 / sendIntervalOnSerialize;
		}
		set
		{
			if (value > sendRate)
			{
				Debug.LogError((object)"Error, can not set the OnSerialize SendRate more often then the overall SendRate");
				value = sendRate;
			}
			sendIntervalOnSerialize = 1000 / value;
			if ((Object)(object)photonMono != (Object)null)
			{
				photonMono.updateIntervalOnSerialize = sendIntervalOnSerialize;
			}
		}
	}

	public static bool isMessageQueueRunning
	{
		get
		{
			return m_isMessageQueueRunning;
		}
		set
		{
			if (value)
			{
				PhotonHandler.StartFallbackSendAckThread();
			}
			networkingPeer.IsSendingOnlyAcks = !value;
			m_isMessageQueueRunning = value;
		}
	}

	public static int unreliableCommandsLimit
	{
		get
		{
			return networkingPeer.LimitOfUnreliableCommands;
		}
		set
		{
			networkingPeer.LimitOfUnreliableCommands = value;
		}
	}

	public static double time
	{
		get
		{
			if (offlineMode)
			{
				return Time.time;
			}
			return (double)(uint)networkingPeer.ServerTimeInMilliSeconds / 1000.0;
		}
	}

	public static bool isMasterClient
	{
		get
		{
			if (offlineMode)
			{
				return true;
			}
			return networkingPeer.mMasterClient == networkingPeer.mLocalActor;
		}
	}

	public static bool inRoom => connectionStateDetailed == PeerState.Joined;

	public static bool isNonMasterClientInRoom => !isMasterClient && room != null;

	public static int countOfPlayersOnMaster => networkingPeer.mPlayersOnMasterCount;

	public static int countOfPlayersInRooms => networkingPeer.mPlayersInRoomsCount;

	public static int countOfPlayers => networkingPeer.mPlayersInRoomsCount + networkingPeer.mPlayersOnMasterCount;

	public static int countOfRooms => networkingPeer.mGameCount;

	public static bool NetworkStatisticsEnabled
	{
		get
		{
			return networkingPeer.TrafficStatsEnabled;
		}
		set
		{
			networkingPeer.TrafficStatsEnabled = value;
		}
	}

	public static int ResentReliableCommands => networkingPeer.ResentReliableCommands;

	public static bool CrcCheckEnabled
	{
		get
		{
			return networkingPeer.CrcEnabled;
		}
		set
		{
			if (!connected && !connecting)
			{
				networkingPeer.CrcEnabled = value;
			}
			else
			{
				Debug.Log((object)("Can't change CrcCheckEnabled while being connected. CrcCheckEnabled stays " + networkingPeer.CrcEnabled));
			}
		}
	}

	public static int PacketLossByCrcCheck => networkingPeer.PacketLossByCrc;

	public static int MaxResendsBeforeDisconnect
	{
		get
		{
			return networkingPeer.SentCountAllowance;
		}
		set
		{
			if (value < 3)
			{
				value = 3;
			}
			if (value > 10)
			{
				value = 10;
			}
			networkingPeer.SentCountAllowance = value;
		}
	}

	public static ServerConnection Server => networkingPeer.server;

	static PhotonNetwork()
	{
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Expected Obj, but got Unknown
		MAX_VIEW_IDS = 4000;
		PhotonServerSettings = (ServerSettings)(object)Resources.Load("PhotonServerSettings", typeof(ServerSettings));
		precisionForVectorSynchronization = 9.9E-05f;
		precisionForQuaternionSynchronization = 1f;
		precisionForFloatSynchronization = 0.01f;
		InstantiateInRoomOnly = true;
		logLevel = PhotonLogLevel.ErrorsOnly;
		UsePrefabCache = true;
		PrefabCache = new Dictionary<string, GameObject>();
		isOfflineMode = false;
		offlineMode_inRoom = false;
		UseNameServer = false;
		_mAutomaticallySyncScene = false;
		m_autoCleanUpPlayerObjects = true;
		autoJoinLobbyField = true;
		sendInterval = 50;
		sendIntervalOnSerialize = 100;
		m_isMessageQueueRunning = true;
		lastUsedViewSubId = 0;
		lastUsedViewSubIdStatic = 0;
		manuallyAllocatedViewIds = new List<int>();
		Application.runInBackground = true;
		GameObject val = new GameObject();
		photonMono = val.AddComponent<PhotonHandler>();
		val.AddComponent<PingCloudRegions>();
		((Object)val).name = "PhotonMono";
		((Object)val).hideFlags = (HideFlags)1;
		networkingPeer = new NetworkingPeer(photonMono, string.Empty, ConnectionProtocol.Udp);
		networkingPeer.LimitOfUnreliableCommands = 40;
		CustomTypes.Register();
	}

	public static bool SetMasterClient(PhotonPlayer masterClientPlayer)
	{
		if (!VerifyCanUseNetwork() || !isMasterClient)
		{
			return false;
		}
		return networkingPeer.SetMasterClient(masterClientPlayer.ID, sync: true);
	}

	public static void NetworkStatisticsReset()
	{
		networkingPeer.TrafficStatsReset();
	}

	public static string NetworkStatisticsToString()
	{
		if (networkingPeer == null || offlineMode)
		{
			return "Offline or in OfflineMode. No VitalStats available.";
		}
		return networkingPeer.VitalStatsToString(all: false);
	}

	public static void SwitchToProtocol(ConnectionProtocol cp)
	{
		if (networkingPeer.UsedProtocol != cp)
		{
			try
			{
				networkingPeer.Disconnect();
				networkingPeer.StopThread();
			}
			catch
			{
			}
			networkingPeer = new NetworkingPeer(photonMono, string.Empty, cp);
			networkingPeer.LimitOfUnreliableCommands = 40;
			Debug.Log((object)("Protocol switched to: " + cp));
		}
	}

	public static void InternalCleanPhotonMonoFromSceneIfStuck()
	{
		if (!(Object.FindObjectsOfType(typeof(PhotonHandler)) is PhotonHandler[] array) || array.Length <= 0)
		{
			return;
		}
		Debug.Log((object)"Cleaning up hidden PhotonHandler instances in scene. Please save it. This is not an issue.");
		PhotonHandler[] array2 = array;
		foreach (PhotonHandler photonHandler in array2)
		{
			((Object)((Component)photonHandler).gameObject).hideFlags = (HideFlags)0;
			if ((Object)(object)((Component)photonHandler).gameObject != (Object)null && ((Object)((Component)photonHandler).gameObject).name == "PhotonMono")
			{
				Object.DestroyImmediate((Object)(object)((Component)photonHandler).gameObject);
			}
			Object.DestroyImmediate((Object)(object)photonHandler);
		}
	}

	public static bool ConnectUsingSettings(string gameVersion)
	{
		if ((Object)(object)PhotonServerSettings == (Object)null)
		{
			Debug.LogError((object)"Can't connect: Loading settings failed. ServerSettings asset must be in any 'Resources' folder as: PhotonServerSettings");
			return false;
		}
		networkingPeer.SetApp(PhotonServerSettings.AppID, gameVersion);
		if (PhotonServerSettings.HostType == ServerSettings.HostingOption.OfflineMode)
		{
			offlineMode = true;
			return true;
		}
		if (offlineMode)
		{
			Debug.LogWarning((object)"ConnectToMaster() disabled the offline mode. No longer offline.");
		}
		offlineMode = false;
		isMessageQueueRunning = true;
		networkingPeer.IsInitialConnect = true;
		if (PhotonServerSettings.HostType == ServerSettings.HostingOption.SelfHosted || !UseNameServer)
		{
			networkingPeer.IsUsingNameServer = false;
			networkingPeer.MasterServerAddress = PhotonServerSettings.ServerAddress + ":" + PhotonServerSettings.ServerPort;
			return networkingPeer.Connect(networkingPeer.MasterServerAddress, ServerConnection.MasterServer);
		}
		return networkingPeer.ConnectToRegionMaster(PhotonServerSettings.Region);
	}

	public static bool ConnectToMaster(string masterServerAddress, int port, string appID, string gameVersion)
	{
		if (networkingPeer.PeerState != PeerStateValue.Disconnected)
		{
			Debug.LogWarning((object)("ConnectToMaster() failed. Can only connect while in state 'Disconnected'. Current state: " + networkingPeer.PeerState));
			return false;
		}
		if (offlineMode)
		{
			offlineMode = false;
			Debug.LogWarning((object)"ConnectToMaster() disabled the offline mode. No longer offline.");
		}
		if (!isMessageQueueRunning)
		{
			isMessageQueueRunning = true;
			Debug.LogWarning((object)"ConnectToMaster() enabled isMessageQueueRunning. Needs to be able to dispatch incoming messages.");
		}
		networkingPeer.SetApp(appID, gameVersion);
		networkingPeer.IsUsingNameServer = false;
		networkingPeer.IsInitialConnect = true;
		networkingPeer.MasterServerAddress = masterServerAddress + ":" + port;
		return networkingPeer.Connect(networkingPeer.MasterServerAddress, ServerConnection.MasterServer);
	}

	public static bool ConnectToBestCloudServer(string gameVersion)
	{
		if ((Object)(object)PhotonServerSettings == (Object)null)
		{
			Debug.LogError((object)"Can't connect: Loading settings failed. ServerSettings asset must be in any 'Resources' folder as: PhotonServerSettings");
			return false;
		}
		if (PhotonServerSettings.HostType == ServerSettings.HostingOption.OfflineMode)
		{
			ConnectUsingSettings(gameVersion);
			return false;
		}
		networkingPeer.IsInitialConnect = true;
		networkingPeer.SetApp(PhotonServerSettings.AppID, gameVersion);
		PingCloudRegions.ConnectToBestRegion();
		return true;
	}

	public static void OverrideBestCloudServer(CloudServerRegion region)
	{
		PingCloudRegions.OverrideRegion(region);
	}

	public static void RefreshCloudServerRating()
	{
		PingCloudRegions.RefreshCloudServerRating();
	}

	public static void Disconnect()
	{
		if (offlineMode)
		{
			offlineMode = false;
			networkingPeer.State = PeerState.Disconnecting;
			networkingPeer.OnStatusChanged(StatusCode.Disconnect);
		}
		else if (networkingPeer != null)
		{
			networkingPeer.Disconnect();
		}
	}

	[Obsolete("Used for compatibility with Unity networking only. Encryption is automatically initialized while connecting.")]
	public static void InitializeSecurity()
	{
	}

	public static bool FindFriends(string[] friendsToFind)
	{
		if (networkingPeer == null || isOfflineMode)
		{
			return false;
		}
		return networkingPeer.OpFindFriends(friendsToFind);
	}

	[Obsolete("Use overload with RoomOptions and TypedLobby parameters.")]
	public static bool CreateRoom(string roomName, bool isVisible, bool isOpen, int maxPlayers)
	{
		RoomOptions roomOptions = new RoomOptions();
		roomOptions.isVisible = isVisible;
		roomOptions.isOpen = isOpen;
		roomOptions.maxPlayers = maxPlayers;
		return CreateRoom(roomName, roomOptions, null);
	}

	[Obsolete("Use overload with RoomOptions and TypedLobby parameters.")]
	public static bool CreateRoom(string roomName, bool isVisible, bool isOpen, int maxPlayers, Hashtable customRoomProperties, string[] propsToListInLobby)
	{
		RoomOptions roomOptions = new RoomOptions();
		roomOptions.isVisible = isVisible;
		roomOptions.isOpen = isOpen;
		roomOptions.maxPlayers = maxPlayers;
		roomOptions.customRoomProperties = customRoomProperties;
		roomOptions.customRoomPropertiesForLobby = propsToListInLobby;
		return CreateRoom(roomName, roomOptions, null);
	}

	public static bool CreateRoom(string roomName)
	{
		return CreateRoom(roomName, null, null);
	}

	public static bool CreateRoom(string roomName, RoomOptions roomOptions, TypedLobby typedLobby)
	{
		if (offlineMode)
		{
			if (offlineMode_inRoom)
			{
				Debug.LogError((object)"CreateRoom failed. In offline mode you still have to leave a room to enter another.");
				return false;
			}
			offlineMode_inRoom = true;
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnCreatedRoom);
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnJoinedRoom);
			return true;
		}
		if (networkingPeer.server != ServerConnection.MasterServer || !connectedAndReady)
		{
			Debug.LogError((object)"CreateRoom failed. Client is not on Master Server or not yet ready to call operations. Wait for callback: OnJoinedLobby or OnConnectedToMaster.");
			return false;
		}
		return networkingPeer.OpCreateGame(roomName, roomOptions, typedLobby);
	}

	[Obsolete("Use overload with roomOptions and TypedLobby parameter.")]
	public static bool JoinRoom(string roomName, bool createIfNotExists)
	{
		if (connectionStateDetailed == PeerState.Joining || connectionStateDetailed == PeerState.Joined || connectionStateDetailed == PeerState.ConnectedToGameserver)
		{
			Debug.LogError((object)"JoinRoom aborted: You can only join a room while not currently connected/connecting to a room.");
		}
		else if (room != null)
		{
			Debug.LogError((object)"JoinRoom aborted: You are already in a room!");
		}
		else
		{
			if (!(roomName == string.Empty))
			{
				if (offlineMode)
				{
					offlineMode_inRoom = true;
					NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnJoinedRoom);
					return true;
				}
				return networkingPeer.OpJoinRoom(roomName, null, null, createIfNotExists);
			}
			Debug.LogError((object)"JoinRoom aborted: You must specifiy a room name!");
		}
		return false;
	}

	public static bool JoinRoom(string roomName)
	{
		if (offlineMode)
		{
			if (offlineMode_inRoom)
			{
				Debug.LogError((object)"JoinRoom failed. In offline mode you still have to leave a room to enter another.");
				return false;
			}
			offlineMode_inRoom = true;
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnJoinedRoom);
			return true;
		}
		if (networkingPeer.server != ServerConnection.MasterServer || !connectedAndReady)
		{
			Debug.LogError((object)"JoinRoom failed. Client is not on Master Server or not yet ready to call operations. Wait for callback: OnJoinedLobby or OnConnectedToMaster.");
			return false;
		}
		if (string.IsNullOrEmpty(roomName))
		{
			Debug.LogError((object)"JoinRoom failed. A roomname is required. If you don't know one, how will you join?");
			return false;
		}
		return networkingPeer.OpJoinRoom(roomName, null, null, createIfNotExists: false);
	}

	public static bool JoinOrCreateRoom(string roomName, RoomOptions roomOptions, TypedLobby typedLobby)
	{
		if (offlineMode)
		{
			if (offlineMode_inRoom)
			{
				Debug.LogError((object)"JoinOrCreateRoom failed. In offline mode you still have to leave a room to enter another.");
				return false;
			}
			offlineMode_inRoom = true;
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnCreatedRoom);
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnJoinedRoom);
			return true;
		}
		if (networkingPeer.server != ServerConnection.MasterServer || !connectedAndReady)
		{
			Debug.LogError((object)"JoinOrCreateRoom failed. Client is not on Master Server or not yet ready to call operations. Wait for callback: OnJoinedLobby or OnConnectedToMaster.");
			return false;
		}
		if (string.IsNullOrEmpty(roomName))
		{
			Debug.LogError((object)"JoinOrCreateRoom failed. A roomname is required. If you don't know one, how will you join?");
			return false;
		}
		return networkingPeer.OpJoinRoom(roomName, roomOptions, typedLobby, createIfNotExists: true);
	}

	public static bool JoinRandomRoom()
	{
		return JoinRandomRoom(null, 0, MatchmakingMode.FillRoom, null, null);
	}

	public static bool JoinRandomRoom(Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers)
	{
		return JoinRandomRoom(expectedCustomRoomProperties, expectedMaxPlayers, MatchmakingMode.FillRoom, null, null);
	}

	public static bool JoinRandomRoom(Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers, MatchmakingMode matchingType, TypedLobby typedLobby, string sqlLobbyFilter)
	{
		if (offlineMode)
		{
			if (offlineMode_inRoom)
			{
				Debug.LogError((object)"JoinRandomRoom failed. In offline mode you still have to leave a room to enter another.");
				return false;
			}
			offlineMode_inRoom = true;
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnJoinedRoom);
			return true;
		}
		if (networkingPeer.server != ServerConnection.MasterServer || !connectedAndReady)
		{
			Debug.LogError((object)"JoinRandomRoom failed. Client is not on Master Server or not yet ready to call operations. Wait for callback: OnJoinedLobby or OnConnectedToMaster.");
			return false;
		}
		Hashtable hashtable = new Hashtable();
		hashtable.MergeStringKeys(expectedCustomRoomProperties);
		if (expectedMaxPlayers > 0)
		{
			hashtable[byte.MaxValue] = expectedMaxPlayers;
		}
		return networkingPeer.OpJoinRandomRoom(hashtable, 0, null, matchingType, typedLobby, sqlLobbyFilter);
	}

	public static bool JoinLobby()
	{
		return JoinLobby(null);
	}

	public static bool JoinLobby(TypedLobby typedLobby)
	{
		if (connected && Server == ServerConnection.MasterServer)
		{
			if (typedLobby == null)
			{
				typedLobby = TypedLobby.Default;
			}
			bool flag = networkingPeer.OpJoinLobby(typedLobby);
			if (flag)
			{
				networkingPeer.lobby = typedLobby;
			}
			return flag;
		}
		return false;
	}

	public static bool LeaveLobby()
	{
		if (connected && Server == ServerConnection.MasterServer)
		{
			return networkingPeer.OpLeaveLobby();
		}
		return false;
	}

	public static bool LeaveRoom()
	{
		if (!offlineMode && connectionStateDetailed != PeerState.Joined)
		{
			Debug.LogError((object)("PhotonNetwork: Error, you cannot leave a room if you're not in a room! State: " + connectionStateDetailed));
			return false;
		}
		if (room == null)
		{
			Debug.LogError((object)"PhotonNetwork: Error, you cannot leave a room if you're not in a room! Room is null.");
			return false;
		}
		if (offlineMode)
		{
			offlineMode_inRoom = false;
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnLeftRoom);
			return true;
		}
		return networkingPeer.OpLeave();
	}

	public static RoomInfo[] GetRoomList()
	{
		if (offlineMode || networkingPeer == null)
		{
			return new RoomInfo[0];
		}
		return networkingPeer.mGameListCopy;
	}

	public static void SetPlayerCustomProperties(Hashtable customProperties)
	{
		if (customProperties == null)
		{
			customProperties = new Hashtable();
			foreach (object key in player.customProperties.Keys)
			{
				customProperties[(string)key] = null;
			}
		}
		if (room != null && room.isLocalClientInside)
		{
			player.SetCustomProperties(customProperties);
		}
		else
		{
			player.InternalCacheProperties(customProperties);
		}
	}

	public static bool RaiseEvent(byte eventCode, object eventContent, bool sendReliable, RaiseEventOptions options)
	{
		if (!inRoom || eventCode <= 0 || eventCode >= 200)
		{
			return false;
		}
		return networkingPeer.OpRaiseEvent(eventCode, eventContent, sendReliable, options);
	}

	public static int AllocateViewID()
	{
		int num = AllocateViewID(player.ID);
		manuallyAllocatedViewIds.Add(num);
		return num;
	}

	public static void UnAllocateViewID(int viewID)
	{
		manuallyAllocatedViewIds.Remove(viewID);
		if (networkingPeer.photonViewList.ContainsKey(viewID))
		{
			Debug.LogWarning((object)$"Unallocated manually used viewID: {viewID} but found it used still in a PhotonView: {networkingPeer.photonViewList[viewID]}");
		}
	}

	private static int AllocateViewID(int ownerId)
	{
		if (ownerId == 0)
		{
			int num = lastUsedViewSubIdStatic;
			int num2 = ownerId * MAX_VIEW_IDS;
			for (int i = 1; i < MAX_VIEW_IDS; i++)
			{
				num = (num + 1) % MAX_VIEW_IDS;
				if (num != 0)
				{
					int num3 = num + num2;
					if (!networkingPeer.photonViewList.ContainsKey(num3))
					{
						lastUsedViewSubIdStatic = num;
						return num3;
					}
				}
			}
			throw new Exception($"AllocateViewID() failed. Room (user {ownerId}) is out of subIds, as all room viewIDs are used.");
		}
		int num4 = lastUsedViewSubId;
		int num5 = ownerId * MAX_VIEW_IDS;
		for (int j = 1; j < MAX_VIEW_IDS; j++)
		{
			num4 = (num4 + 1) % MAX_VIEW_IDS;
			if (num4 != 0)
			{
				int num6 = num4 + num5;
				if (!networkingPeer.photonViewList.ContainsKey(num6) && !manuallyAllocatedViewIds.Contains(num6))
				{
					lastUsedViewSubId = num4;
					return num6;
				}
			}
		}
		throw new Exception($"AllocateViewID() failed. User {ownerId} is out of subIds, as all viewIDs are used.");
	}

	private static int[] AllocateSceneViewIDs(int countOfNewViews)
	{
		int[] array = new int[countOfNewViews];
		for (int i = 0; i < countOfNewViews; i++)
		{
			array[i] = AllocateViewID(0);
		}
		return array;
	}

	public static GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation, int group)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return Instantiate(prefabName, position, rotation, group, null);
	}

	public static GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation, int group, object[] data)
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Expected Obj, but got Unknown
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		if (!connected || (InstantiateInRoomOnly && !inRoom))
		{
			Debug.LogError((object)("Failed to Instantiate prefab: " + prefabName + ". Client should be in a room. Current connectionStateDetailed: " + connectionStateDetailed));
			return null;
		}
		if (!UsePrefabCache || !PrefabCache.TryGetValue(prefabName, out var value))
		{
			value = (GameObject)Resources.Load(prefabName, typeof(GameObject));
			if (UsePrefabCache)
			{
				PrefabCache.Add(prefabName, value);
			}
		}
		if ((Object)(object)value == (Object)null)
		{
			Debug.LogError((object)("Failed to Instantiate prefab: " + prefabName + ". Verify the Prefab is in a Resources folder (and not in a subfolder)"));
			return null;
		}
		if ((Object)(object)value.GetComponent<PhotonView>() == (Object)null)
		{
			Debug.LogError((object)("Failed to Instantiate prefab:" + prefabName + ". Prefab must have a PhotonView component."));
			return null;
		}
		Component[] photonViewsInChildren = (Component[])(object)value.GetPhotonViewsInChildren();
		int[] array = new int[photonViewsInChildren.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = AllocateViewID(player.ID);
		}
		Hashtable evData = networkingPeer.SendInstantiate(prefabName, position, rotation, group, array, data, isGlobalObject: false);
		return networkingPeer.DoInstantiate(evData, networkingPeer.mLocalActor, value);
	}

	public static GameObject InstantiateSceneObject(string prefabName, Vector3 position, Quaternion rotation, int group, object[] data)
	{
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Expected Obj, but got Unknown
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		if (!connected || (InstantiateInRoomOnly && !inRoom))
		{
			Debug.LogError((object)("Failed to InstantiateSceneObject prefab: " + prefabName + ". Client should be in a room. Current connectionStateDetailed: " + connectionStateDetailed));
			return null;
		}
		if (!isMasterClient)
		{
			Debug.LogError((object)("Failed to InstantiateSceneObject prefab: " + prefabName + ". Client is not the MasterClient in this room."));
			return null;
		}
		if (!UsePrefabCache || !PrefabCache.TryGetValue(prefabName, out var value))
		{
			value = (GameObject)Resources.Load(prefabName, typeof(GameObject));
			if (UsePrefabCache)
			{
				PrefabCache.Add(prefabName, value);
			}
		}
		if ((Object)(object)value == (Object)null)
		{
			Debug.LogError((object)("Failed to InstantiateSceneObject prefab: " + prefabName + ". Verify the Prefab is in a Resources folder (and not in a subfolder)"));
			return null;
		}
		if ((Object)(object)value.GetComponent<PhotonView>() == (Object)null)
		{
			Debug.LogError((object)("Failed to InstantiateSceneObject prefab:" + prefabName + ". Prefab must have a PhotonView component."));
			return null;
		}
		Component[] photonViewsInChildren = (Component[])(object)value.GetPhotonViewsInChildren();
		int[] array = AllocateSceneViewIDs(photonViewsInChildren.Length);
		if (array == null)
		{
			Debug.LogError((object)("Failed to InstantiateSceneObject prefab: " + prefabName + ". No ViewIDs are free to use. Max is: " + MAX_VIEW_IDS));
			return null;
		}
		Hashtable evData = networkingPeer.SendInstantiate(prefabName, position, rotation, group, array, data, isGlobalObject: true);
		return networkingPeer.DoInstantiate(evData, networkingPeer.mLocalActor, value);
	}

	public static int GetPing()
	{
		return networkingPeer.RoundTripTime;
	}

	public static void FetchServerTimestamp()
	{
		if (networkingPeer != null)
		{
			networkingPeer.FetchServerTimestamp();
		}
	}

	public static void SendOutgoingCommands()
	{
		if (VerifyCanUseNetwork())
		{
			while (networkingPeer.SendOutgoingCommands())
			{
			}
		}
	}

	public static bool CloseConnection(PhotonPlayer kickPlayer)
	{
		if (!VerifyCanUseNetwork())
		{
			return false;
		}
		if (!player.isMasterClient)
		{
			Debug.LogError((object)"CloseConnection: Only the masterclient can kick another player.");
			return false;
		}
		if (kickPlayer == null)
		{
			Debug.LogError((object)"CloseConnection: No such player connected!");
			return false;
		}
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
		raiseEventOptions.TargetActors = new int[1] { kickPlayer.ID };
		RaiseEventOptions raiseEventOptions2 = raiseEventOptions;
		return networkingPeer.OpRaiseEvent(203, null, sendReliable: true, raiseEventOptions2);
	}

	public static void Destroy(PhotonView targetView)
	{
		if ((Object)(object)targetView != (Object)null)
		{
			networkingPeer.RemoveInstantiatedGO(((Component)targetView).gameObject, !inRoom);
		}
		else
		{
			Debug.LogError((object)"Destroy(targetPhotonView) failed, cause targetPhotonView is null.");
		}
	}

	public static void Destroy(GameObject targetGo)
	{
		networkingPeer.RemoveInstantiatedGO(targetGo, !inRoom);
	}

	public static void DestroyPlayerObjects(PhotonPlayer targetPlayer)
	{
		if (player == null)
		{
			Debug.LogError((object)"DestroyPlayerObjects() failed, cause parameter 'targetPlayer' was null.");
		}
		DestroyPlayerObjects(targetPlayer.ID);
	}

	public static void DestroyPlayerObjects(int targetPlayerId)
	{
		if (VerifyCanUseNetwork())
		{
			if (player.isMasterClient || targetPlayerId == player.ID)
			{
				networkingPeer.DestroyPlayerObjects(targetPlayerId, localOnly: false);
			}
			else
			{
				Debug.LogError((object)("DestroyPlayerObjects() failed, cause players can only destroy their own GameObjects. A Master Client can destroy anyone's. This is master: " + isMasterClient));
			}
		}
	}

	public static void DestroyAll()
	{
		if (isMasterClient)
		{
			networkingPeer.DestroyAll(localOnly: false);
		}
		else
		{
			Debug.LogError((object)"Couldn't call DestroyAll() as only the master client is allowed to call this.");
		}
	}

	public static void RemoveRPCs(PhotonPlayer targetPlayer)
	{
		if (VerifyCanUseNetwork())
		{
			if (!targetPlayer.isLocal && !isMasterClient)
			{
				Debug.LogError((object)"Error; Only the MasterClient can call RemoveRPCs for other players.");
			}
			else
			{
				networkingPeer.OpCleanRpcBuffer(targetPlayer.ID);
			}
		}
	}

	public static void RemoveRPCs(PhotonView targetPhotonView)
	{
		if (VerifyCanUseNetwork())
		{
			networkingPeer.CleanRpcBufferIfMine(targetPhotonView);
		}
	}

	public static void RemoveRPCsInGroup(int targetGroup)
	{
		if (VerifyCanUseNetwork())
		{
			networkingPeer.RemoveRPCsInGroup(targetGroup);
		}
	}

	internal static void RPC(PhotonView view, string methodName, PhotonTargets target, params object[] parameters)
	{
		if (VerifyCanUseNetwork())
		{
			if (room == null)
			{
				Debug.LogWarning((object)"Cannot send RPCs in Lobby! RPC dropped.");
			}
			else if (networkingPeer != null)
			{
				networkingPeer.RPC(view, methodName, target, parameters);
			}
			else
			{
				Debug.LogWarning((object)("Could not execute RPC " + methodName + ". Possible scene loading in progress?"));
			}
		}
	}

	internal static void RPC(PhotonView view, string methodName, PhotonPlayer targetPlayer, params object[] parameters)
	{
		if (!VerifyCanUseNetwork())
		{
			return;
		}
		if (room == null)
		{
			Debug.LogWarning((object)"Cannot send RPCs in Lobby, only processed locally");
			return;
		}
		if (player == null)
		{
			Debug.LogError((object)("Error; Sending RPC to player null! Aborted \"" + methodName + "\""));
		}
		if (networkingPeer != null)
		{
			networkingPeer.RPC(view, methodName, targetPlayer, parameters);
		}
		else
		{
			Debug.LogWarning((object)("Could not execute RPC " + methodName + ". Possible scene loading in progress?"));
		}
	}

	public static void SetReceivingEnabled(int group, bool enabled)
	{
		if (VerifyCanUseNetwork())
		{
			networkingPeer.SetReceivingEnabled(group, enabled);
		}
	}

	public static void SetSendingEnabled(int group, bool enabled)
	{
		if (VerifyCanUseNetwork())
		{
			networkingPeer.SetSendingEnabled(group, enabled);
		}
	}

	public static void SetLevelPrefix(short prefix)
	{
		if (VerifyCanUseNetwork())
		{
			networkingPeer.SetLevelPrefix(prefix);
		}
	}

	private static bool VerifyCanUseNetwork()
	{
		if (connected)
		{
			return true;
		}
		Debug.LogError((object)"Cannot send messages when not connected. Either connect to Photon OR use offline mode!");
		return false;
	}

	public static void LoadLevel(int levelNumber)
	{
		networkingPeer.SetLevelInPropsIfSynced(levelNumber);
		isMessageQueueRunning = false;
		networkingPeer.loadingLevelAndPausedNetwork = true;
		Application.LoadLevel(levelNumber);
	}

	public static void LoadLevel(string levelName)
	{
		networkingPeer.SetLevelInPropsIfSynced(levelName);
		isMessageQueueRunning = false;
		networkingPeer.loadingLevelAndPausedNetwork = true;
		Application.LoadLevel(levelName);
	}
}
