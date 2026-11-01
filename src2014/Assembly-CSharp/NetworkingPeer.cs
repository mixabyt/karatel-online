using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.Lite;
using UnityEngine;

internal class NetworkingPeer : LoadbalancingPeer, IPhotonPeerListener
{
	protected internal const string CurrentSceneProperty = "curScn";

	public string mAppVersion;

	private string mAppId;

	private string playername = string.Empty;

	private IPhotonPeerListener externalListener;

	private JoinType mLastJoinType;

	private bool mPlayernameHasToBeUpdated;

	public Dictionary<int, PhotonPlayer> mActors = new Dictionary<int, PhotonPlayer>();

	public PhotonPlayer[] mOtherPlayerListCopy = new PhotonPlayer[0];

	public PhotonPlayer[] mPlayerListCopy = new PhotonPlayer[0];

	public PhotonPlayer mMasterClient;

	public bool hasSwitchedMC;

	public bool requestSecurity = true;

	private Dictionary<Type, List<MethodInfo>> monoRPCMethodsCache = new Dictionary<Type, List<MethodInfo>>();

	public static bool UsePrefabCache = true;

	public static Dictionary<string, GameObject> PrefabCache = new Dictionary<string, GameObject>();

	public Dictionary<string, RoomInfo> mGameList = new Dictionary<string, RoomInfo>();

	public RoomInfo[] mGameListCopy = new RoomInfo[0];

	public bool insideLobby;

	public Dictionary<int, GameObject> instantiatedObjects = new Dictionary<int, GameObject>();

	private HashSet<int> allowedReceivingGroups = new HashSet<int>();

	private HashSet<int> blockSendingGroups = new HashSet<int>();

	protected internal Dictionary<int, PhotonView> photonViewList = new Dictionary<int, PhotonView>();

	protected internal short currentLevelPrefix;

	private readonly Dictionary<string, int> rpcShortcuts;

	public bool IsInitialConnect;

	public string NameServerAddress = "ns.exitgamescloud.com:5058";

	private bool didAuthenticate;

	private int friendListTimestamp;

	private bool isFetchingFriends;

	private Dictionary<int, object[]> tempInstantiationData = new Dictionary<int, object[]>();

	protected internal bool loadingLevelAndPausedNetwork;

	public AuthenticationValues CustomAuthenticationValues { get; set; }

	public string MasterServerAddress { get; protected internal set; }

	public string PlayerName
	{
		get
		{
			return playername;
		}
		set
		{
			if (!string.IsNullOrEmpty(value) && !value.Equals(playername))
			{
				if (mLocalActor != null)
				{
					mLocalActor.name = value;
				}
				playername = value;
				if (mCurrentGame != null)
				{
					SendPlayerName();
				}
			}
		}
	}

	public PeerState State { get; internal set; }

	public Room mCurrentGame
	{
		get
		{
			if (mRoomToGetInto != null && mRoomToGetInto.isLocalClientInside)
			{
				return mRoomToGetInto;
			}
			return null;
		}
	}

	internal Room mRoomToGetInto { get; set; }

	internal RoomOptions mRoomOptionsForCreate { get; set; }

	internal TypedLobby mRoomToEnterLobby { get; set; }

	public PhotonPlayer mLocalActor { get; internal set; }

	public string mGameserver { get; internal set; }

	public int mQueuePosition { get; internal set; }

	public TypedLobby lobby { get; set; }

	public int mPlayersOnMasterCount { get; internal set; }

	public int mGameCount { get; internal set; }

	public int mPlayersInRoomsCount { get; internal set; }

	protected internal ServerConnection server { get; private set; }

	public bool IsUsingNameServer { get; protected internal set; }

	public string[] AvailableRegions { get; protected internal set; }

	public string[] AvailableRegionsServers { get; protected internal set; }

	public string CloudRegion { get; protected internal set; }

	public bool IsAuthorizeSecretAvailable => false;

	protected internal int FriendsListAge => (!isFetchingFriends && friendListTimestamp != 0) ? (Environment.TickCount - friendListTimestamp) : 0;

	public NetworkingPeer(IPhotonPeerListener listener, string playername, ConnectionProtocol connectionProtocol)
		: base(listener, connectionProtocol)
	{
		Listener = this;
		lobby = TypedLobby.Default;
		externalListener = listener;
		PlayerName = playername;
		mLocalActor = new PhotonPlayer(isLocal: true, -1, this.playername);
		AddNewPlayer(mLocalActor.ID, mLocalActor);
		rpcShortcuts = new Dictionary<string, int>(PhotonNetwork.PhotonServerSettings.RpcList.Count);
		for (int i = 0; i < PhotonNetwork.PhotonServerSettings.RpcList.Count; i++)
		{
			string key = PhotonNetwork.PhotonServerSettings.RpcList[i];
			rpcShortcuts[key] = i;
		}
		State = global::PeerState.PeerCreated;
	}

	public override bool Connect(string serverAddress, string applicationName)
	{
		Debug.LogError((object)"Avoid using this directly. Thanks.");
		return false;
	}

	public bool Connect(string serverAddress, ServerConnection type)
	{
		if (PhotonNetwork.connectionStateDetailed == global::PeerState.Disconnecting)
		{
			Debug.LogError((object)("Connect() failed. Can't connect while disconnecting (still). Current state: " + PhotonNetwork.connectionStateDetailed));
			return false;
		}
		bool flag = base.Connect(serverAddress, string.Empty);
		if (flag)
		{
			switch (type)
			{
			case ServerConnection.NameServer:
				State = global::PeerState.ConnectingToNameServer;
				break;
			case ServerConnection.MasterServer:
				State = global::PeerState.ConnectingToMasterserver;
				break;
			case ServerConnection.GameServer:
				State = global::PeerState.ConnectingToGameserver;
				break;
			}
		}
		return flag;
	}

	public bool ConnectToNameServer()
	{
		IsUsingNameServer = true;
		CloudRegion = null;
		if (State == global::PeerState.ConnectedToNameServer)
		{
			return true;
		}
		if (!base.Connect(NameServerAddress, "ns"))
		{
			return false;
		}
		State = global::PeerState.ConnectingToNameServer;
		return true;
	}

	public bool ConnectToRegionMaster(string region)
	{
		IsUsingNameServer = true;
		CloudRegion = region;
		if (State == global::PeerState.ConnectedToNameServer)
		{
			return OpAuthenticate(mAppId, mAppVersion, PlayerName, CustomAuthenticationValues, region);
		}
		if (!base.Connect(NameServerAddress, "ns"))
		{
			return false;
		}
		State = global::PeerState.ConnectingToNameServer;
		return true;
	}

	public bool GetRegions()
	{
		if (server != ServerConnection.NameServer)
		{
			return false;
		}
		bool flag = OpGetRegions(mAppId);
		if (flag)
		{
			AvailableRegions = null;
		}
		return flag;
	}

	public override void Disconnect()
	{
		if (PeerState == PeerStateValue.Disconnected)
		{
			Debug.LogWarning((object)$"Can't execute Disconnect() while not connected. Nothing changed. State: {State}");
			return;
		}
		State = global::PeerState.Disconnecting;
		base.Disconnect();
		LeftRoomCleanup();
		LeftLobbyCleanup();
	}

	private void DisconnectToReconnect()
	{
		switch (server)
		{
		case ServerConnection.NameServer:
			State = global::PeerState.DisconnectingFromNameServer;
			base.Disconnect();
			break;
		case ServerConnection.MasterServer:
			State = global::PeerState.DisconnectingFromMasterserver;
			base.Disconnect();
			LeftLobbyCleanup();
			break;
		case ServerConnection.GameServer:
			State = global::PeerState.DisconnectingFromGameserver;
			base.Disconnect();
			LeftRoomCleanup();
			break;
		}
	}

	private void LeftLobbyCleanup()
	{
		mGameList = new Dictionary<string, RoomInfo>();
		mGameListCopy = new RoomInfo[0];
		if (insideLobby)
		{
			insideLobby = false;
			SendMonoMessage(PhotonNetworkingMessage.OnLeftLobby);
		}
	}

	private void LeftRoomCleanup()
	{
		bool flag = mRoomToGetInto != null;
		bool flag2 = ((mRoomToGetInto == null) ? PhotonNetwork.autoCleanUpPlayerObjects : mRoomToGetInto.autoCleanUp);
		hasSwitchedMC = false;
		mRoomToGetInto = null;
		mActors = new Dictionary<int, PhotonPlayer>();
		mPlayerListCopy = new PhotonPlayer[0];
		mOtherPlayerListCopy = new PhotonPlayer[0];
		mMasterClient = null;
		allowedReceivingGroups = new HashSet<int>();
		blockSendingGroups = new HashSet<int>();
		mGameList = new Dictionary<string, RoomInfo>();
		mGameListCopy = new RoomInfo[0];
		isFetchingFriends = false;
		ChangeLocalID(-1);
		if (flag2)
		{
			LocalCleanupAnythingInstantiated(destroyInstantiatedGameObjects: true);
			PhotonNetwork.manuallyAllocatedViewIds = new List<int>();
		}
		if (flag)
		{
			SendMonoMessage(PhotonNetworkingMessage.OnLeftRoom);
		}
	}

	protected internal void LocalCleanupAnythingInstantiated(bool destroyInstantiatedGameObjects)
	{
		if (tempInstantiationData.Count > 0)
		{
			Debug.LogWarning((object)"It seems some instantiation is not completed, as instantiation data is used. You should make sure instantiations are paused when calling this method. Cleaning now, despite this.");
		}
		if (destroyInstantiatedGameObjects)
		{
			HashSet<GameObject> hashSet = new HashSet<GameObject>(instantiatedObjects.Values);
			foreach (GameObject item in hashSet)
			{
				RemoveInstantiatedGO(item, localOnly: true);
			}
		}
		tempInstantiationData.Clear();
		instantiatedObjects = new Dictionary<int, GameObject>();
		PhotonNetwork.lastUsedViewSubId = 0;
		PhotonNetwork.lastUsedViewSubIdStatic = 0;
	}

	private void ReadoutProperties(ExitGames.Client.Photon.Hashtable gameProperties, ExitGames.Client.Photon.Hashtable pActorProperties, int targetActorNr)
	{
		if (mCurrentGame != null && gameProperties != null)
		{
			mCurrentGame.CacheProperties(gameProperties);
			SendMonoMessage(PhotonNetworkingMessage.OnPhotonCustomRoomPropertiesChanged, gameProperties);
			if (PhotonNetwork.automaticallySyncScene)
			{
				LoadLevelIfSynced();
			}
		}
		if (pActorProperties == null || pActorProperties.Count <= 0)
		{
			return;
		}
		if (targetActorNr > 0)
		{
			PhotonPlayer playerWithID = GetPlayerWithID(targetActorNr);
			if (playerWithID != null)
			{
				ExitGames.Client.Photon.Hashtable actorPropertiesForActorNr = GetActorPropertiesForActorNr(pActorProperties, targetActorNr);
				playerWithID.InternalCacheProperties(actorPropertiesForActorNr);
				SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerPropertiesChanged, playerWithID, actorPropertiesForActorNr);
			}
			return;
		}
		foreach (object key in pActorProperties.Keys)
		{
			int num = (int)key;
			ExitGames.Client.Photon.Hashtable hashtable = (ExitGames.Client.Photon.Hashtable)pActorProperties[key];
			string name = (string)hashtable[byte.MaxValue];
			PhotonPlayer photonPlayer = GetPlayerWithID(num);
			if (photonPlayer == null)
			{
				photonPlayer = new PhotonPlayer(isLocal: false, num, name);
				AddNewPlayer(num, photonPlayer);
			}
			photonPlayer.InternalCacheProperties(hashtable);
			SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerPropertiesChanged, photonPlayer, hashtable);
		}
	}

	private void AddNewPlayer(int ID, PhotonPlayer player)
	{
		if (!mActors.ContainsKey(ID))
		{
			mActors[ID] = player;
			RebuildPlayerListCopies();
		}
		else
		{
			Debug.LogError((object)("Adding player twice: " + ID));
		}
	}

	private void RemovePlayer(int ID, PhotonPlayer player)
	{
		mActors.Remove(ID);
		if (!player.isLocal)
		{
			RebuildPlayerListCopies();
		}
	}

	private void RebuildPlayerListCopies()
	{
		mPlayerListCopy = new PhotonPlayer[mActors.Count];
		mActors.Values.CopyTo(mPlayerListCopy, 0);
		List<PhotonPlayer> list = new List<PhotonPlayer>();
		PhotonPlayer[] array = mPlayerListCopy;
		foreach (PhotonPlayer photonPlayer in array)
		{
			if (!photonPlayer.isLocal)
			{
				list.Add(photonPlayer);
			}
		}
		mOtherPlayerListCopy = list.ToArray();
	}

	private void ResetPhotonViewsOnSerialize()
	{
		foreach (PhotonView value in photonViewList.Values)
		{
			value.lastOnSerializeDataSent = null;
		}
	}

	private void HandleEventLeave(int actorID)
	{
		if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
		{
			Debug.Log((object)("HandleEventLeave for player ID: " + actorID));
		}
		if (actorID < 0 || !mActors.ContainsKey(actorID))
		{
			Debug.LogError((object)$"Received event Leave for unknown player ID: {actorID}");
			return;
		}
		PhotonPlayer playerWithID = GetPlayerWithID(actorID);
		if (playerWithID == null)
		{
			Debug.LogError((object)("HandleEventLeave for player ID: " + actorID + " has no PhotonPlayer!"));
		}
		CheckMasterClient(actorID);
		if (mCurrentGame != null && mCurrentGame.autoCleanUp)
		{
			DestroyPlayerObjects(actorID, localOnly: true);
		}
		RemovePlayer(actorID, playerWithID);
		SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerDisconnected, playerWithID);
	}

	private void CheckMasterClient(int leavingPlayerId)
	{
		bool flag = mMasterClient != null && mMasterClient.ID == leavingPlayerId;
		bool flag2 = leavingPlayerId > 0;
		if (flag2 && !flag)
		{
			return;
		}
		if (mActors.Count <= 1)
		{
			mMasterClient = mLocalActor;
		}
		else
		{
			int num = int.MaxValue;
			foreach (int key in mActors.Keys)
			{
				if (key < num && key != leavingPlayerId)
				{
					num = key;
				}
			}
			mMasterClient = mActors[num];
		}
		if (flag2)
		{
			SendMonoMessage(PhotonNetworkingMessage.OnMasterClientSwitched, mMasterClient);
		}
	}

	private static int ReturnLowestPlayerId(PhotonPlayer[] players, int playerIdToIgnore)
	{
		if (players == null || players.Length == 0)
		{
			return -1;
		}
		int num = int.MaxValue;
		foreach (PhotonPlayer photonPlayer in players)
		{
			if (photonPlayer.ID != playerIdToIgnore && photonPlayer.ID < num)
			{
				num = photonPlayer.ID;
			}
		}
		return num;
	}

	protected internal bool SetMasterClient(int playerId, bool sync)
	{
		if (mMasterClient == null || mMasterClient.ID == playerId || !mActors.ContainsKey(playerId))
		{
			return false;
		}
		if (sync && !OpRaiseEvent(208, new ExitGames.Client.Photon.Hashtable { 
		{
			(byte)1,
			playerId
		} }, sendReliable: true, null))
		{
			return false;
		}
		hasSwitchedMC = true;
		mMasterClient = mActors[playerId];
		SendMonoMessage(PhotonNetworkingMessage.OnMasterClientSwitched, mMasterClient);
		return true;
	}

	private ExitGames.Client.Photon.Hashtable GetActorPropertiesForActorNr(ExitGames.Client.Photon.Hashtable actorProperties, int actorNr)
	{
		if (actorProperties.ContainsKey(actorNr))
		{
			return (ExitGames.Client.Photon.Hashtable)actorProperties[actorNr];
		}
		return actorProperties;
	}

	private PhotonPlayer GetPlayerWithID(int number)
	{
		if (mActors != null && mActors.ContainsKey(number))
		{
			return mActors[number];
		}
		return null;
	}

	private void SendPlayerName()
	{
		if (State == global::PeerState.Joining)
		{
			mPlayernameHasToBeUpdated = true;
		}
		else if (mLocalActor != null)
		{
			mLocalActor.name = PlayerName;
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable[byte.MaxValue] = PlayerName;
			if (mLocalActor.ID > 0)
			{
				OpSetPropertiesOfActor(mLocalActor.ID, hashtable, broadcast: true, 0);
				mPlayernameHasToBeUpdated = false;
			}
		}
	}

	private void GameEnteredOnGameServer(OperationResponse operationResponse)
	{
		if (operationResponse.ReturnCode != 0)
		{
			switch (operationResponse.OperationCode)
			{
			case 227:
				if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
				{
					Debug.Log((object)("Create failed on GameServer. Changing back to MasterServer. Msg: " + operationResponse.DebugMessage));
				}
				SendMonoMessage(PhotonNetworkingMessage.OnPhotonCreateRoomFailed);
				break;
			case 226:
				if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
				{
					Debug.Log((object)("Join failed on GameServer. Changing back to MasterServer. Msg: " + operationResponse.DebugMessage));
				}
				if (operationResponse.ReturnCode == 32758)
				{
					Debug.Log((object)"Most likely the game became empty during the switch to GameServer.");
				}
				SendMonoMessage(PhotonNetworkingMessage.OnPhotonJoinRoomFailed);
				break;
			case 225:
				if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
				{
					Debug.Log((object)("Join failed on GameServer. Changing back to MasterServer. Msg: " + operationResponse.DebugMessage));
				}
				if (operationResponse.ReturnCode == 32758)
				{
					Debug.Log((object)"Most likely the game became empty during the switch to GameServer.");
				}
				SendMonoMessage(PhotonNetworkingMessage.OnPhotonRandomJoinFailed);
				break;
			}
			DisconnectToReconnect();
		}
		else
		{
			State = global::PeerState.Joined;
			mRoomToGetInto.isLocalClientInside = true;
			ExitGames.Client.Photon.Hashtable pActorProperties = (ExitGames.Client.Photon.Hashtable)operationResponse[249];
			ExitGames.Client.Photon.Hashtable gameProperties = (ExitGames.Client.Photon.Hashtable)operationResponse[248];
			ReadoutProperties(gameProperties, pActorProperties, 0);
			int newID = (int)operationResponse[254];
			ChangeLocalID(newID);
			CheckMasterClient(-1);
			if (mPlayernameHasToBeUpdated)
			{
				SendPlayerName();
			}
			switch (operationResponse.OperationCode)
			{
			case 227:
				SendMonoMessage(PhotonNetworkingMessage.OnCreatedRoom);
				break;
			case 225:
			case 226:
				break;
			}
		}
	}

	private ExitGames.Client.Photon.Hashtable GetLocalActorProperties()
	{
		if (PhotonNetwork.player != null)
		{
			return PhotonNetwork.player.allProperties;
		}
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable[byte.MaxValue] = PlayerName;
		return hashtable;
	}

	public void ChangeLocalID(int newID)
	{
		if (mLocalActor == null)
		{
			Debug.LogWarning((object)$"Local actor is null or not in mActors! mLocalActor: {mLocalActor} mActors==null: {mActors == null} newID: {newID}");
		}
		if (mActors.ContainsKey(mLocalActor.ID))
		{
			mActors.Remove(mLocalActor.ID);
		}
		mLocalActor.InternalChangeLocalID(newID);
		mActors[mLocalActor.ID] = mLocalActor;
		RebuildPlayerListCopies();
	}

	public bool OpCreateGame(string roomName, RoomOptions roomOptions, TypedLobby typedLobby)
	{
		bool flag = server == ServerConnection.GameServer;
		if (!flag)
		{
			mRoomOptionsForCreate = roomOptions;
			mRoomToGetInto = new Room(roomName, roomOptions);
			mRoomToEnterLobby = typedLobby ?? ((!insideLobby) ? null : lobby);
		}
		mLastJoinType = JoinType.CreateGame;
		return base.OpCreateRoom(roomName, roomOptions, mRoomToEnterLobby, GetLocalActorProperties(), flag);
	}

	public bool OpJoinRoom(string roomName, RoomOptions roomOptions, TypedLobby typedLobby, bool createIfNotExists)
	{
		bool flag = server == ServerConnection.GameServer;
		if (!flag)
		{
			mRoomOptionsForCreate = roomOptions;
			mRoomToGetInto = new Room(roomName, roomOptions);
			mRoomToEnterLobby = null;
			if (createIfNotExists)
			{
				mRoomToEnterLobby = typedLobby ?? ((!insideLobby) ? null : lobby);
			}
		}
		mLastJoinType = ((!createIfNotExists) ? JoinType.JoinGame : JoinType.JoinOrCreateOnDemand);
		return base.OpJoinRoom(roomName, roomOptions, mRoomToEnterLobby, createIfNotExists, GetLocalActorProperties(), flag);
	}

	public override bool OpJoinRandomRoom(ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers, ExitGames.Client.Photon.Hashtable playerProperties, MatchmakingMode matchingType, TypedLobby typedLobby, string sqlLobbyFilter)
	{
		mRoomToGetInto = new Room(null, null);
		mRoomToEnterLobby = null;
		mLastJoinType = JoinType.JoinRandomGame;
		return base.OpJoinRandomRoom(expectedCustomRoomProperties, expectedMaxPlayers, playerProperties, matchingType, typedLobby, sqlLobbyFilter);
	}

	public virtual bool OpLeave()
	{
		if (State != global::PeerState.Joined)
		{
			Debug.LogWarning((object)("Not sending leave operation. State is not 'Joined': " + State));
			return false;
		}
		return OpCustom(254, null, sendReliable: true, 0);
	}

	public override bool OpRaiseEvent(byte eventCode, object customEventContent, bool sendReliable, RaiseEventOptions raiseEventOptions)
	{
		if (PhotonNetwork.offlineMode)
		{
			return false;
		}
		return base.OpRaiseEvent(eventCode, customEventContent, sendReliable, raiseEventOptions);
	}

	public void DebugReturn(DebugLevel level, string message)
	{
		externalListener.DebugReturn(level, message);
	}

	public void OnOperationResponse(OperationResponse operationResponse)
	{
		if (PhotonNetwork.networkingPeer.State == global::PeerState.Disconnecting)
		{
			if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
			{
				Debug.Log((object)("OperationResponse ignored while disconnecting. Code: " + operationResponse.OperationCode));
			}
			return;
		}
		if (operationResponse.ReturnCode == 0)
		{
			if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
			{
				Debug.Log((object)operationResponse.ToString());
			}
		}
		else if (operationResponse.ReturnCode == -3)
		{
			Debug.LogError((object)("Operation " + operationResponse.OperationCode + " could not be executed yet. Wait for state JoinedLobby or ConnectedToMaster and their respective callbacks before calling OPs."));
		}
		else if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
		{
			Debug.LogError((object)("Operation failed: " + operationResponse.ToStringFull() + " Server: " + server));
		}
		if (operationResponse.Parameters.ContainsKey(221))
		{
			if (CustomAuthenticationValues == null)
			{
				CustomAuthenticationValues = new AuthenticationValues();
			}
			CustomAuthenticationValues.Secret = operationResponse[221] as string;
		}
		switch (operationResponse.OperationCode)
		{
		case 230:
			if (operationResponse.ReturnCode != 0)
			{
				if (operationResponse.ReturnCode == -2)
				{
					Debug.LogError((object)string.Format("If you host Photon yourself, make sure to start the 'Instance LoadBalancing' " + ServerAddress));
				}
				else if (operationResponse.ReturnCode == short.MaxValue)
				{
					Debug.LogError((object)$"The appId this client sent is unknown on the server (Cloud). Check settings. If using the Cloud, check account.");
					SendMonoMessage(PhotonNetworkingMessage.OnFailedToConnectToPhoton, DisconnectCause.InvalidAuthentication);
				}
				else if (operationResponse.ReturnCode == 32755)
				{
					Debug.LogError((object)$"Custom Authentication failed (either due to user-input or configuration or AuthParameter string format). Calling: OnCustomAuthenticationFailed()");
					SendMonoMessage(PhotonNetworkingMessage.OnCustomAuthenticationFailed, operationResponse.DebugMessage);
				}
				else
				{
					Debug.LogError((object)$"Authentication failed: '{operationResponse.DebugMessage}' Code: {operationResponse.ReturnCode}");
				}
				State = global::PeerState.Disconnecting;
				Disconnect();
				if (operationResponse.ReturnCode == 32757)
				{
					if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
					{
						Debug.LogWarning((object)$"Currently, the limit of users is reached for this title. Try again later. Disconnecting");
					}
					SendMonoMessage(PhotonNetworkingMessage.OnPhotonMaxCccuReached);
					SendMonoMessage(PhotonNetworkingMessage.OnConnectionFail, DisconnectCause.MaxCcuReached);
				}
				else if (operationResponse.ReturnCode == 32756)
				{
					if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
					{
						Debug.LogError((object)$"The used master server address is not available with the subscription currently used. Got to Photon Cloud Dashboard or change URL. Disconnecting.");
					}
					SendMonoMessage(PhotonNetworkingMessage.OnConnectionFail, DisconnectCause.InvalidRegion);
				}
			}
			else if (server == ServerConnection.NameServer)
			{
				MasterServerAddress = operationResponse[230] as string;
				DisconnectToReconnect();
			}
			else if (server == ServerConnection.MasterServer)
			{
				if (PhotonNetwork.autoJoinLobby)
				{
					State = global::PeerState.Authenticated;
					OpJoinLobby(lobby);
				}
				else
				{
					State = global::PeerState.ConnectedToMaster;
					SendMonoMessage(PhotonNetworkingMessage.OnConnectedToMaster);
				}
			}
			else if (server == ServerConnection.GameServer)
			{
				State = global::PeerState.Joining;
				if (mLastJoinType == JoinType.JoinGame || mLastJoinType == JoinType.JoinRandomGame || mLastJoinType == JoinType.JoinOrCreateOnDemand)
				{
					OpJoinRoom(mRoomToGetInto.name, mRoomOptionsForCreate, mRoomToEnterLobby, mLastJoinType == JoinType.JoinOrCreateOnDemand);
				}
				else if (mLastJoinType == JoinType.CreateGame)
				{
					OpCreateGame(mRoomToGetInto.name, mRoomOptionsForCreate, mRoomToEnterLobby);
				}
			}
			break;
		case 220:
			AvailableRegions = operationResponse[210] as string[];
			AvailableRegionsServers = operationResponse[230] as string[];
			Debug.Log((object)("GetRegions returned: " + operationResponse.ToStringFull()));
			break;
		case 227:
		{
			if (server == ServerConnection.GameServer)
			{
				GameEnteredOnGameServer(operationResponse);
				break;
			}
			if (operationResponse.ReturnCode != 0)
			{
				if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
				{
					Debug.LogWarning((object)$"CreateRoom failed, client stays on masterserver: {operationResponse.ToStringFull()}.");
				}
				SendMonoMessage(PhotonNetworkingMessage.OnPhotonCreateRoomFailed);
				break;
			}
			string text = (string)operationResponse[byte.MaxValue];
			if (!string.IsNullOrEmpty(text))
			{
				mRoomToGetInto.name = text;
			}
			mGameserver = (string)operationResponse[230];
			DisconnectToReconnect();
			break;
		}
		case 226:
			if (server != ServerConnection.GameServer)
			{
				if (operationResponse.ReturnCode != 0)
				{
					if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
					{
						Debug.Log((object)$"JoinRoom failed (room maybe closed by now). Client stays on masterserver: {operationResponse.ToStringFull()}. State: {State}");
					}
					SendMonoMessage(PhotonNetworkingMessage.OnPhotonJoinRoomFailed);
				}
				else
				{
					mGameserver = (string)operationResponse[230];
					DisconnectToReconnect();
				}
			}
			else
			{
				GameEnteredOnGameServer(operationResponse);
			}
			break;
		case 225:
			if (operationResponse.ReturnCode != 0)
			{
				if (operationResponse.ReturnCode == 32760)
				{
					if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
					{
						Debug.Log((object)"JoinRandom failed: No open game. Calling: OnPhotonRandomJoinFailed() and staying on master server.");
					}
				}
				else if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
				{
					Debug.LogWarning((object)$"JoinRandom failed: {operationResponse.ToStringFull()}.");
				}
				SendMonoMessage(PhotonNetworkingMessage.OnPhotonRandomJoinFailed);
			}
			else
			{
				string name = (string)operationResponse[byte.MaxValue];
				mRoomToGetInto.name = name;
				mGameserver = (string)operationResponse[230];
				DisconnectToReconnect();
			}
			break;
		case 229:
			State = global::PeerState.JoinedLobby;
			insideLobby = true;
			SendMonoMessage(PhotonNetworkingMessage.OnJoinedLobby);
			break;
		case 228:
			State = global::PeerState.Authenticated;
			LeftLobbyCleanup();
			break;
		case 254:
			DisconnectToReconnect();
			break;
		case 251:
		{
			ExitGames.Client.Photon.Hashtable pActorProperties = (ExitGames.Client.Photon.Hashtable)operationResponse[249];
			ExitGames.Client.Photon.Hashtable gameProperties = (ExitGames.Client.Photon.Hashtable)operationResponse[248];
			ReadoutProperties(gameProperties, pActorProperties, 0);
			break;
		}
		case 222:
		{
			bool[] array = operationResponse[1] as bool[];
			string[] array2 = operationResponse[2] as string[];
			if (array != null && array2 != null && PhotonNetwork.Friends != null && array.Length == PhotonNetwork.Friends.Count)
			{
				for (int i = 0; i < PhotonNetwork.Friends.Count; i++)
				{
					FriendInfo friendInfo = PhotonNetwork.Friends[i];
					friendInfo.Room = array2[i];
					friendInfo.IsOnline = array[i];
				}
			}
			else
			{
				Debug.LogError((object)"FindFriends failed to apply the result, as a required value wasn't provided or the friend list length differed from result.");
			}
			isFetchingFriends = false;
			friendListTimestamp = Environment.TickCount;
			if (friendListTimestamp == 0)
			{
				friendListTimestamp = 1;
			}
			SendMonoMessage(PhotonNetworkingMessage.OnUpdatedFriendList);
			break;
		}
		default:
			Debug.LogWarning((object)$"OperationResponse unhandled: {operationResponse.ToString()}");
			break;
		case 252:
		case 253:
			break;
		}
		externalListener.OnOperationResponse(operationResponse);
	}

	public override bool OpFindFriends(string[] friendsToFind)
	{
		if (isFetchingFriends)
		{
			return false;
		}
		isFetchingFriends = true;
		PhotonNetwork.Friends = new List<FriendInfo>(friendsToFind.Length);
		foreach (string name in friendsToFind)
		{
			PhotonNetwork.Friends.Add(new FriendInfo
			{
				Name = name
			});
		}
		return base.OpFindFriends(friendsToFind);
	}

	public void OnStatusChanged(StatusCode statusCode)
	{
		if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
		{
			Debug.Log((object)$"OnStatusChanged: {statusCode.ToString()}");
		}
		switch (statusCode)
		{
		case StatusCode.Connect:
			if (State == global::PeerState.ConnectingToNameServer)
			{
				if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
				{
					Debug.Log((object)"Connected to NameServer.");
				}
				server = ServerConnection.NameServer;
				if (CustomAuthenticationValues != null)
				{
					CustomAuthenticationValues.Secret = null;
				}
			}
			if (State == global::PeerState.ConnectingToGameserver)
			{
				if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
				{
					Debug.Log((object)"Connected to gameserver.");
				}
				server = ServerConnection.GameServer;
				State = global::PeerState.ConnectedToGameserver;
			}
			if (State == global::PeerState.ConnectingToMasterserver)
			{
				if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
				{
					Debug.Log((object)("Connected to masterserver. " + State));
				}
				server = ServerConnection.MasterServer;
				State = global::PeerState.ConnectedToMaster;
				if (IsInitialConnect)
				{
					IsInitialConnect = false;
					SendMonoMessage(PhotonNetworkingMessage.OnConnectedToPhoton);
				}
			}
			EstablishEncryption();
			if (IsAuthorizeSecretAvailable)
			{
				didAuthenticate = OpAuthenticate(mAppId, mAppVersion, PlayerName, CustomAuthenticationValues, CloudRegion);
				if (didAuthenticate)
				{
					State = global::PeerState.Authenticating;
				}
			}
			break;
		case StatusCode.EncryptionEstablished:
			if (server == ServerConnection.NameServer)
			{
				State = global::PeerState.ConnectedToNameServer;
				if (!didAuthenticate && string.IsNullOrEmpty(CloudRegion))
				{
					OpGetRegions(mAppId);
				}
			}
			if (!didAuthenticate && (!IsUsingNameServer || CloudRegion != null))
			{
				didAuthenticate = OpAuthenticate(mAppId, mAppVersion, PlayerName, CustomAuthenticationValues, CloudRegion);
				if (didAuthenticate)
				{
					State = global::PeerState.Authenticating;
				}
			}
			break;
		case StatusCode.EncryptionFailedToEstablish:
			Debug.LogError((object)string.Concat("Encryption wasn't established: ", statusCode, ". Going to authenticate anyways."));
			OpAuthenticate(mAppId, mAppVersion, PlayerName, CustomAuthenticationValues, CloudRegion);
			break;
		case StatusCode.Disconnect:
			didAuthenticate = false;
			isFetchingFriends = false;
			if (State == global::PeerState.DisconnectingFromMasterserver)
			{
				if (Connect(mGameserver, ServerConnection.GameServer))
				{
					State = global::PeerState.ConnectingToGameserver;
				}
				break;
			}
			if (State == global::PeerState.DisconnectingFromGameserver || State == global::PeerState.DisconnectingFromNameServer)
			{
				if (Connect(MasterServerAddress, ServerConnection.MasterServer))
				{
					State = global::PeerState.ConnectingToMasterserver;
				}
				break;
			}
			LeftRoomCleanup();
			if (CustomAuthenticationValues != null)
			{
				CustomAuthenticationValues.Secret = null;
			}
			State = global::PeerState.PeerCreated;
			SendMonoMessage(PhotonNetworkingMessage.OnDisconnectedFromPhoton);
			break;
		case StatusCode.SecurityExceptionOnConnect:
		case StatusCode.ExceptionOnConnect:
		{
			State = global::PeerState.PeerCreated;
			if (CustomAuthenticationValues != null)
			{
				CustomAuthenticationValues.Secret = null;
			}
			DisconnectCause disconnectCause = (DisconnectCause)statusCode;
			SendMonoMessage(PhotonNetworkingMessage.OnFailedToConnectToPhoton, disconnectCause);
			break;
		}
		case StatusCode.Exception:
			if (IsInitialConnect)
			{
				Debug.LogError((object)("Exception while connecting to: " + ServerAddress + ". Check if the server is available."));
				if (ServerAddress == null || ServerAddress.StartsWith("127.0.0.1"))
				{
					Debug.LogWarning((object)"The server address is 127.0.0.1 (localhost): Make sure the server is running on this machine. Android and iOS emulators have their own localhost.");
					if (ServerAddress == mGameserver)
					{
						Debug.LogWarning((object)"This might be a misconfiguration in the game server config. You need to edit it to a (public) address.");
					}
				}
				State = global::PeerState.PeerCreated;
				DisconnectCause disconnectCause = (DisconnectCause)statusCode;
				SendMonoMessage(PhotonNetworkingMessage.OnFailedToConnectToPhoton, disconnectCause);
			}
			else
			{
				State = global::PeerState.PeerCreated;
				DisconnectCause disconnectCause = (DisconnectCause)statusCode;
				SendMonoMessage(PhotonNetworkingMessage.OnConnectionFail, disconnectCause);
			}
			Disconnect();
			break;
		case StatusCode.ExceptionOnReceive:
		case StatusCode.TimeoutDisconnect:
		case StatusCode.DisconnectByServer:
		case StatusCode.DisconnectByServerUserLimit:
		case StatusCode.DisconnectByServerLogic:
			if (IsInitialConnect)
			{
				Debug.LogWarning((object)string.Concat(statusCode, " while connecting to: ", ServerAddress, ". Check if the server is available."));
				DisconnectCause disconnectCause = (DisconnectCause)statusCode;
				SendMonoMessage(PhotonNetworkingMessage.OnFailedToConnectToPhoton, disconnectCause);
			}
			else
			{
				DisconnectCause disconnectCause = (DisconnectCause)statusCode;
				SendMonoMessage(PhotonNetworkingMessage.OnConnectionFail, disconnectCause);
			}
			if (CustomAuthenticationValues != null)
			{
				CustomAuthenticationValues.Secret = null;
			}
			Disconnect();
			break;
		default:
			Debug.LogError((object)("Received unknown status code: " + statusCode));
			break;
		case StatusCode.QueueOutgoingReliableWarning:
		case StatusCode.QueueOutgoingUnreliableWarning:
		case StatusCode.SendError:
		case StatusCode.QueueOutgoingAcksWarning:
		case StatusCode.QueueSentWarning:
			break;
		}
		externalListener.OnStatusChanged(statusCode);
	}

	public void OnEvent(EventData photonEvent)
	{
		if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
		{
			Debug.Log((object)$"OnEvent: {photonEvent.ToString()}");
		}
		int num = -1;
		PhotonPlayer photonPlayer = null;
		if (photonEvent.Parameters.ContainsKey(254))
		{
			num = (int)photonEvent[254];
			if (mActors.ContainsKey(num))
			{
				photonPlayer = mActors[num];
			}
		}
		switch (photonEvent.Code)
		{
		case 230:
		{
			mGameList = new Dictionary<string, RoomInfo>();
			ExitGames.Client.Photon.Hashtable hashtable4 = (ExitGames.Client.Photon.Hashtable)photonEvent[222];
			foreach (DictionaryEntry item in hashtable4)
			{
				string text2 = (string)item.Key;
				mGameList[text2] = new RoomInfo(text2, (ExitGames.Client.Photon.Hashtable)item.Value);
			}
			mGameListCopy = new RoomInfo[mGameList.Count];
			mGameList.Values.CopyTo(mGameListCopy, 0);
			SendMonoMessage(PhotonNetworkingMessage.OnReceivedRoomListUpdate);
			break;
		}
		case 229:
		{
			ExitGames.Client.Photon.Hashtable hashtable3 = (ExitGames.Client.Photon.Hashtable)photonEvent[222];
			foreach (DictionaryEntry item2 in hashtable3)
			{
				string text = (string)item2.Key;
				RoomInfo roomInfo = new RoomInfo(text, (ExitGames.Client.Photon.Hashtable)item2.Value);
				if (roomInfo.removedFromList)
				{
					mGameList.Remove(text);
				}
				else
				{
					mGameList[text] = roomInfo;
				}
			}
			mGameListCopy = new RoomInfo[mGameList.Count];
			mGameList.Values.CopyTo(mGameListCopy, 0);
			SendMonoMessage(PhotonNetworkingMessage.OnReceivedRoomListUpdate);
			break;
		}
		case 228:
			if (photonEvent.Parameters.ContainsKey(223))
			{
				mQueuePosition = (int)photonEvent[223];
			}
			else
			{
				Debug.LogError((object)"Event QueueState must contain position!");
			}
			if (mQueuePosition == 0)
			{
				if (PhotonNetwork.autoJoinLobby)
				{
					State = global::PeerState.Authenticated;
					OpJoinLobby(lobby);
				}
				else
				{
					State = global::PeerState.ConnectedToMaster;
					SendMonoMessage(PhotonNetworkingMessage.OnConnectedToMaster);
				}
			}
			break;
		case 226:
			mPlayersInRoomsCount = (int)photonEvent[229];
			mPlayersOnMasterCount = (int)photonEvent[227];
			mGameCount = (int)photonEvent[228];
			break;
		case byte.MaxValue:
		{
			ExitGames.Client.Photon.Hashtable properties = (ExitGames.Client.Photon.Hashtable)photonEvent[249];
			if (photonPlayer == null)
			{
				bool isLocal = mLocalActor.ID == num;
				AddNewPlayer(num, new PhotonPlayer(isLocal, num, properties));
				ResetPhotonViewsOnSerialize();
			}
			if (num == mLocalActor.ID)
			{
				int[] array = (int[])photonEvent[252];
				int[] array2 = array;
				foreach (int num5 in array2)
				{
					if (mLocalActor.ID != num5 && !mActors.ContainsKey(num5))
					{
						AddNewPlayer(num5, new PhotonPlayer(isLocal: false, num5, string.Empty));
					}
				}
				if (mLastJoinType == JoinType.JoinOrCreateOnDemand && mLocalActor.ID == 1)
				{
					SendMonoMessage(PhotonNetworkingMessage.OnCreatedRoom);
				}
				SendMonoMessage(PhotonNetworkingMessage.OnJoinedRoom);
			}
			else
			{
				SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerConnected, mActors[num]);
			}
			break;
		}
		case 254:
			HandleEventLeave(num);
			break;
		case 253:
		{
			int num7 = (int)photonEvent[253];
			ExitGames.Client.Photon.Hashtable gameProperties = null;
			ExitGames.Client.Photon.Hashtable pActorProperties = null;
			if (num7 == 0)
			{
				gameProperties = (ExitGames.Client.Photon.Hashtable)photonEvent[251];
			}
			else
			{
				pActorProperties = (ExitGames.Client.Photon.Hashtable)photonEvent[251];
			}
			ReadoutProperties(gameProperties, pActorProperties, num7);
			break;
		}
		case 200:
			ExecuteRPC(photonEvent[245] as ExitGames.Client.Photon.Hashtable, photonPlayer);
			break;
		case 201:
		case 206:
		{
			ExitGames.Client.Photon.Hashtable hashtable2 = (ExitGames.Client.Photon.Hashtable)photonEvent[245];
			int networkTime = (int)hashtable2[(byte)0];
			short correctPrefix = -1;
			short num3 = 1;
			if (hashtable2.ContainsKey((byte)1))
			{
				correctPrefix = (short)hashtable2[(byte)1];
				num3 = 2;
			}
			for (short num4 = num3; num4 < hashtable2.Count; num4++)
			{
				OnSerializeRead(hashtable2[num4] as ExitGames.Client.Photon.Hashtable, photonPlayer, networkTime, correctPrefix);
			}
			break;
		}
		case 202:
			DoInstantiate((ExitGames.Client.Photon.Hashtable)photonEvent[245], photonPlayer, null);
			break;
		case 203:
			if (photonPlayer == null || !photonPlayer.isMasterClient)
			{
				Debug.LogError((object)string.Concat("Error: Someone else(", photonPlayer, ") then the masterserver requests a disconnect!"));
			}
			else
			{
				PhotonNetwork.LeaveRoom();
			}
			break;
		case 207:
		{
			ExitGames.Client.Photon.Hashtable hashtable = (ExitGames.Client.Photon.Hashtable)photonEvent[245];
			int num6 = (int)hashtable[(byte)0];
			if (num6 >= 0)
			{
				DestroyPlayerObjects(num6, localOnly: true);
				break;
			}
			if ((int)DebugOut >= 3)
			{
				Debug.Log((object)("Ev DestroyAll! By PlayerId: " + num));
			}
			DestroyAll(localOnly: true);
			break;
		}
		case 204:
		{
			ExitGames.Client.Photon.Hashtable hashtable = (ExitGames.Client.Photon.Hashtable)photonEvent[245];
			int num2 = (int)hashtable[(byte)0];
			GameObject value = null;
			instantiatedObjects.TryGetValue(num2, out value);
			if ((Object)(object)value == (Object)null || photonPlayer == null)
			{
				if ((int)DebugOut >= 1)
				{
					Debug.LogError((object)string.Concat("Can't execute received Destroy request for view ID=", num2, " as GO can't be found. From player/actorNr: ", num, " GO to destroy=", value, "  originating Player=", photonPlayer));
				}
			}
			else
			{
				RemoveInstantiatedGO(value, localOnly: true);
			}
			break;
		}
		case 208:
		{
			ExitGames.Client.Photon.Hashtable hashtable = (ExitGames.Client.Photon.Hashtable)photonEvent[245];
			int playerId = (int)hashtable[(byte)1];
			SetMasterClient(playerId, sync: false);
			break;
		}
		default:
			if (photonEvent.Code > 0 && photonEvent.Code < 200 && PhotonNetwork.OnEventCall != null)
			{
				object content = photonEvent[245];
				PhotonNetwork.OnEventCall(photonEvent.Code, content, num);
			}
			else
			{
				Debug.LogError((object)("Error. Unhandled event: " + photonEvent));
			}
			break;
		}
		externalListener.OnEvent(photonEvent);
	}

	public static void SendMonoMessage(PhotonNetworkingMessage methodString, params object[] parameters)
	{
		HashSet<GameObject> hashSet;
		if (PhotonNetwork.SendMonoMessageTargets != null)
		{
			hashSet = PhotonNetwork.SendMonoMessageTargets;
		}
		else
		{
			hashSet = new HashSet<GameObject>();
			Component[] array = (Component[])(object)Object.FindObjectsOfType(typeof(MonoBehaviour));
			for (int i = 0; i < array.Length; i++)
			{
				hashSet.Add(array[i].gameObject);
			}
		}
		string text = methodString.ToString();
		foreach (GameObject item in hashSet)
		{
			if (parameters != null && parameters.Length == 1)
			{
				item.SendMessage(text, parameters[0], (SendMessageOptions)1);
			}
			else
			{
				item.SendMessage(text, (object)parameters, (SendMessageOptions)1);
			}
		}
	}

	public void ExecuteRPC(ExitGames.Client.Photon.Hashtable rpcData, PhotonPlayer sender)
	{
		if (rpcData == null || !rpcData.ContainsKey((byte)0))
		{
			Debug.LogError((object)("Malformed RPC; this should never occur. Content: " + SupportClass.DictionaryToString(rpcData)));
			return;
		}
		int num = (int)rpcData[(byte)0];
		int num2 = 0;
		if (rpcData.ContainsKey((byte)1))
		{
			num2 = (short)rpcData[(byte)1];
		}
		string text;
		if (rpcData.ContainsKey((byte)5))
		{
			int num3 = (byte)rpcData[(byte)5];
			if (num3 > PhotonNetwork.PhotonServerSettings.RpcList.Count - 1)
			{
				Debug.LogError((object)("Could not find RPC with index: " + num3 + ". Going to ignore! Check PhotonServerSettings.RpcList"));
				return;
			}
			text = PhotonNetwork.PhotonServerSettings.RpcList[num3];
		}
		else
		{
			text = (string)rpcData[(byte)3];
		}
		object[] array = null;
		if (rpcData.ContainsKey((byte)4))
		{
			array = (object[])rpcData[(byte)4];
		}
		if (array == null)
		{
			array = new object[0];
		}
		PhotonView photonView = GetPhotonView(num);
		if ((Object)(object)photonView == (Object)null)
		{
			int num4 = num / PhotonNetwork.MAX_VIEW_IDS;
			bool flag = num4 == mLocalActor.ID;
			bool flag2 = num4 == sender.ID;
			if (flag)
			{
				Debug.LogWarning((object)("Received RPC \"" + text + "\" for viewID " + num + " but this PhotonView does not exist! View was/is ours." + ((!flag2) ? " Remote called." : " Owner called.")));
			}
			else
			{
				Debug.LogError((object)("Received RPC \"" + text + "\" for viewID " + num + " but this PhotonView does not exist! Was remote PV." + ((!flag2) ? " Remote called." : " Owner called.")));
			}
			return;
		}
		if (photonView.prefix != num2)
		{
			Debug.LogError((object)("Received RPC \"" + text + "\" on viewID " + num + " with a prefix of " + num2 + ", our prefix is " + photonView.prefix + ". The RPC has been ignored."));
			return;
		}
		if (text == string.Empty)
		{
			Debug.LogError((object)("Malformed RPC; this should never occur. Content: " + SupportClass.DictionaryToString(rpcData)));
			return;
		}
		if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
		{
			Debug.Log((object)("Received RPC: " + text));
		}
		if (photonView.group != 0 && !allowedReceivingGroups.Contains(photonView.group))
		{
			return;
		}
		Type[] array2 = new Type[0];
		if (array.Length > 0)
		{
			array2 = new Type[array.Length];
			int num5 = 0;
			foreach (object obj in array)
			{
				if (obj == null)
				{
					array2[num5] = null;
				}
				else
				{
					array2[num5] = obj.GetType();
				}
				num5++;
			}
		}
		int num6 = 0;
		int num7 = 0;
		MonoBehaviour[] components = ((Component)photonView).GetComponents<MonoBehaviour>();
		foreach (MonoBehaviour val in components)
		{
			if ((Object)(object)val == (Object)null)
			{
				Debug.LogError((object)"ERROR You have missing MonoBehaviours on your gameobjects!");
				continue;
			}
			Type type = ((object)val).GetType();
			List<MethodInfo> list = null;
			if (monoRPCMethodsCache.ContainsKey(type))
			{
				list = monoRPCMethodsCache[type];
			}
			if (list == null)
			{
				List<MethodInfo> methods = SupportClass.GetMethods(type, typeof(RPC));
				monoRPCMethodsCache[type] = methods;
				list = methods;
			}
			if (list == null)
			{
				continue;
			}
			for (int k = 0; k < list.Count; k++)
			{
				MethodInfo methodInfo = list[k];
				if (!(methodInfo.Name == text))
				{
					continue;
				}
				num7++;
				ParameterInfo[] parameters = methodInfo.GetParameters();
				if (parameters.Length == array2.Length)
				{
					if (CheckTypeMatch(parameters, array2))
					{
						num6++;
						object obj2 = methodInfo.Invoke(val, array);
						if ((object)methodInfo.ReturnType == typeof(IEnumerator))
						{
							val.StartCoroutine((IEnumerator)obj2);
						}
					}
				}
				else if (parameters.Length - 1 == array2.Length)
				{
					if (CheckTypeMatch(parameters, array2) && (object)parameters[parameters.Length - 1].ParameterType == typeof(PhotonMessageInfo))
					{
						num6++;
						int timestamp = (int)rpcData[(byte)2];
						object[] array3 = new object[array.Length + 1];
						array.CopyTo(array3, 0);
						array3[array3.Length - 1] = new PhotonMessageInfo(sender, timestamp, photonView);
						object obj3 = methodInfo.Invoke(val, array3);
						if ((object)methodInfo.ReturnType == typeof(IEnumerator))
						{
							val.StartCoroutine((IEnumerator)obj3);
						}
					}
				}
				else if (parameters.Length == 1 && parameters[0].ParameterType.IsArray)
				{
					num6++;
					object obj4 = methodInfo.Invoke(val, new object[1] { array });
					if ((object)methodInfo.ReturnType == typeof(IEnumerator))
					{
						val.StartCoroutine((IEnumerator)obj4);
					}
				}
			}
		}
		if (num6 == 1)
		{
			return;
		}
		string text2 = string.Empty;
		foreach (Type type2 in array2)
		{
			if (text2 != string.Empty)
			{
				text2 += ", ";
			}
			text2 = (((object)type2 != null) ? (text2 + type2.Name) : (text2 + "null"));
		}
		if (num6 == 0)
		{
			if (num7 == 0)
			{
				Debug.LogError((object)("PhotonView with ID " + num + " has no method \"" + text + "\" marked with the [RPC](C#) or @RPC(JS) property! Args: " + text2));
			}
			else
			{
				Debug.LogError((object)("PhotonView with ID " + num + " has no method \"" + text + "\" that takes " + array2.Length + " argument(s): " + text2));
			}
		}
		else
		{
			Debug.LogError((object)("PhotonView with ID " + num + " has " + num6 + " methods \"" + text + "\" that takes " + array2.Length + " argument(s): " + text2 + ". Should be just one?"));
		}
	}

	private bool CheckTypeMatch(ParameterInfo[] methodParameters, Type[] callParameterTypes)
	{
		if (methodParameters.Length < callParameterTypes.Length)
		{
			return false;
		}
		for (int i = 0; i < callParameterTypes.Length; i++)
		{
			Type parameterType = methodParameters[i].ParameterType;
			if ((object)callParameterTypes[i] != null && !parameterType.Equals(callParameterTypes[i]))
			{
				return false;
			}
		}
		return true;
	}

	internal ExitGames.Client.Photon.Hashtable SendInstantiate(string prefabName, Vector3 position, Quaternion rotation, int group, int[] viewIDs, object[] data, bool isGlobalObject)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		int num = viewIDs[0];
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable[(byte)0] = prefabName;
		if (position != Vector3.zero)
		{
			hashtable[(byte)1] = position;
		}
		if (rotation != Quaternion.identity)
		{
			hashtable[(byte)2] = rotation;
		}
		if (group != 0)
		{
			hashtable[(byte)3] = group;
		}
		if (viewIDs.Length > 1)
		{
			hashtable[(byte)4] = viewIDs;
		}
		if (data != null)
		{
			hashtable[(byte)5] = data;
		}
		if (currentLevelPrefix > 0)
		{
			hashtable[(byte)8] = currentLevelPrefix;
		}
		hashtable[(byte)6] = ServerTimeInMilliSeconds;
		hashtable[(byte)7] = num;
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
		raiseEventOptions.CachingOption = ((!isGlobalObject) ? EventCaching.AddToRoomCache : EventCaching.AddToRoomCacheGlobal);
		OpRaiseEvent(202, hashtable, sendReliable: true, raiseEventOptions);
		return hashtable;
	}

	internal GameObject DoInstantiate(ExitGames.Client.Photon.Hashtable evData, PhotonPlayer photonPlayer, GameObject resourceGameObject)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Expected Obj, but got Unknown
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Expected Obj, but got Unknown
		string text = (string)evData[(byte)0];
		int timestamp = (int)evData[(byte)6];
		int num = (int)evData[(byte)7];
		Vector3 val = ((!evData.ContainsKey((byte)1)) ? Vector3.zero : ((Vector3)evData[(byte)1]));
		Quaternion val2 = Quaternion.identity;
		if (evData.ContainsKey((byte)2))
		{
			val2 = (Quaternion)evData[(byte)2];
		}
		int num2 = 0;
		if (evData.ContainsKey((byte)3))
		{
			num2 = (int)evData[(byte)3];
		}
		short prefix = 0;
		if (evData.ContainsKey((byte)8))
		{
			prefix = (short)evData[(byte)8];
		}
		int[] array = ((!evData.ContainsKey((byte)4)) ? new int[1] { num } : ((int[])evData[(byte)4]));
		object[] instantiationData = ((!evData.ContainsKey((byte)5)) ? null : ((object[])evData[(byte)5]));
		if (num2 != 0 && !allowedReceivingGroups.Contains(num2))
		{
			return null;
		}
		if ((Object)(object)resourceGameObject == (Object)null)
		{
			if (!UsePrefabCache || !PrefabCache.TryGetValue(text, out resourceGameObject))
			{
				resourceGameObject = (GameObject)Resources.Load(text, typeof(GameObject));
				if (UsePrefabCache)
				{
					PrefabCache.Add(text, resourceGameObject);
				}
			}
			if ((Object)(object)resourceGameObject == (Object)null)
			{
				Debug.LogError((object)("PhotonNetwork error: Could not Instantiate the prefab [" + text + "]. Please verify you have this gameobject in a Resources folder."));
				return null;
			}
		}
		PhotonView[] photonViewsInChildren = resourceGameObject.GetPhotonViewsInChildren();
		if (photonViewsInChildren.Length != array.Length)
		{
			throw new Exception("Error in Instantiation! The resource's PhotonView count is not the same as in incoming data.");
		}
		for (int i = 0; i < array.Length; i++)
		{
			photonViewsInChildren[i].viewID = array[i];
			photonViewsInChildren[i].prefix = prefix;
			photonViewsInChildren[i].instantiationId = num;
		}
		StoreInstantiationData(num, instantiationData);
		GameObject val3 = (GameObject)Object.Instantiate((Object)(object)resourceGameObject, val, val2);
		for (int j = 0; j < array.Length; j++)
		{
			photonViewsInChildren[j].viewID = 0;
			photonViewsInChildren[j].prefix = -1;
			photonViewsInChildren[j].prefixBackup = -1;
			photonViewsInChildren[j].instantiationId = -1;
		}
		RemoveInstantiationData(num);
		if (instantiatedObjects.ContainsKey(num))
		{
			GameObject val4 = instantiatedObjects[num];
			string text2 = string.Empty;
			if ((Object)(object)val4 != (Object)null)
			{
				PhotonView[] photonViewsInChildren2 = val4.GetPhotonViewsInChildren();
				PhotonView[] array2 = photonViewsInChildren2;
				foreach (PhotonView photonView in array2)
				{
					if (!((Object)(object)photonView == (Object)null))
					{
						text2 = text2 + photonView.ToString() + ", ";
					}
				}
			}
			Debug.LogError((object)string.Format("DoInstantiate re-defines a GameObject. Destroying old entry! New: '{0}' (instantiationID: {1}) Old: {3}. PhotonViews on old: {4}. instantiatedObjects.Count: {2}. PhotonNetwork.lastUsedViewSubId: {5} PhotonNetwork.lastUsedViewSubIdStatic: {6} this.photonViewList.Count {7}.)", val3, num, instantiatedObjects.Count, val4, text2, PhotonNetwork.lastUsedViewSubId, PhotonNetwork.lastUsedViewSubIdStatic, photonViewList.Count));
			RemoveInstantiatedGO(val4, localOnly: true);
		}
		instantiatedObjects.Add(num, val3);
		object[] parameters = new object[1]
		{
			new PhotonMessageInfo(photonPlayer, timestamp, null)
		};
		MonoBehaviour[] componentsInChildren = val3.GetComponentsInChildren<MonoBehaviour>();
		foreach (MonoBehaviour val5 in componentsInChildren)
		{
			if (GetMethod(val5, PhotonNetworkingMessage.OnPhotonInstantiate.ToString(), out var mi))
			{
				object obj = mi.Invoke(val5, parameters);
				if ((object)mi.ReturnType == typeof(IEnumerator))
				{
					val5.StartCoroutine((IEnumerator)obj);
				}
			}
		}
		return val3;
	}

	private void StoreInstantiationData(int instantiationId, object[] instantiationData)
	{
		tempInstantiationData[instantiationId] = instantiationData;
	}

	public object[] FetchInstantiationData(int instantiationId)
	{
		object[] value = null;
		if (instantiationId == 0)
		{
			return null;
		}
		tempInstantiationData.TryGetValue(instantiationId, out value);
		return value;
	}

	private void RemoveInstantiationData(int instantiationId)
	{
		tempInstantiationData.Remove(instantiationId);
	}

	public void RemoveAllInstantiatedObjects()
	{
		GameObject[] array = new GameObject[instantiatedObjects.Count];
		instantiatedObjects.Values.CopyTo(array, 0);
		foreach (GameObject val in array)
		{
			if (!((Object)(object)val == (Object)null))
			{
				RemoveInstantiatedGO(val, localOnly: false);
			}
		}
		if (instantiatedObjects.Count > 0)
		{
			Debug.LogError((object)"RemoveAllInstantiatedObjects() this.instantiatedObjects.Count should be 0 by now.");
		}
		instantiatedObjects = new Dictionary<int, GameObject>();
	}

	public void DestroyPlayerObjects(int playerId, bool localOnly)
	{
		if (playerId <= 0)
		{
			Debug.LogError((object)("Failed to Destroy objects of playerId: " + playerId));
			return;
		}
		if (!localOnly)
		{
			OpRemoveFromServerInstantiationsOfPlayer(playerId);
			OpCleanRpcBuffer(playerId);
			SendDestroyOfPlayer(playerId);
		}
		Queue<GameObject> queue = new Queue<GameObject>();
		int num = playerId * PhotonNetwork.MAX_VIEW_IDS;
		int num2 = num + PhotonNetwork.MAX_VIEW_IDS;
		foreach (KeyValuePair<int, GameObject> instantiatedObject in instantiatedObjects)
		{
			if (instantiatedObject.Key > num && instantiatedObject.Key < num2)
			{
				queue.Enqueue(instantiatedObject.Value);
			}
		}
		foreach (GameObject item in queue)
		{
			RemoveInstantiatedGO(item, localOnly: true);
		}
	}

	public void DestroyAll(bool localOnly)
	{
		if (!localOnly)
		{
			OpRemoveCompleteCache();
			SendDestroyOfAll();
		}
		LocalCleanupAnythingInstantiated(destroyInstantiatedGameObjects: true);
	}

	public void RemoveInstantiatedGO(GameObject go, bool localOnly)
	{
		if ((Object)(object)go == (Object)null)
		{
			Debug.LogError((object)"Failed to 'network-remove' GameObject because it's null.");
			return;
		}
		PhotonView[] componentsInChildren = go.GetComponentsInChildren<PhotonView>();
		if (componentsInChildren == null || componentsInChildren.Length <= 0)
		{
			Debug.LogError((object)("Failed to 'network-remove' GameObject because has no PhotonView components: " + go));
			return;
		}
		PhotonView photonView = componentsInChildren[0];
		int ownerActorNr = photonView.OwnerActorNr;
		int instantiationId = photonView.instantiationId;
		if (!localOnly)
		{
			if (!photonView.isMine && (!mLocalActor.isMasterClient || mActors.ContainsKey(ownerActorNr)))
			{
				Debug.LogError((object)("Failed to 'network-remove' GameObject. Client is neither owner nor masterClient taking over for owner who left: " + photonView));
				return;
			}
			if (instantiationId < 1)
			{
				Debug.LogError((object)string.Concat("Failed to 'network-remove' GameObject because it is missing a valid InstantiationId on view: ", photonView, ". Not Destroying GameObject or PhotonViews!"));
				return;
			}
		}
		if (!localOnly)
		{
			ServerCleanInstantiateAndDestroy(instantiationId, ownerActorNr);
		}
		instantiatedObjects.Remove(instantiationId);
		for (int num = componentsInChildren.Length - 1; num >= 0; num--)
		{
			PhotonView photonView2 = componentsInChildren[num];
			if (!((Object)(object)photonView2 == (Object)null))
			{
				if (photonView2.instantiationId >= 1)
				{
					LocalCleanPhotonView(photonView2);
				}
				if (!localOnly)
				{
					OpCleanRpcBuffer(photonView2);
				}
			}
		}
		if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
		{
			Debug.Log((object)("Network destroy Instantiated GO: " + ((Object)go).name));
		}
		Object.Destroy((Object)(object)go);
	}

	public int GetInstantiatedObjectsId(GameObject go)
	{
		int result = -1;
		if ((Object)(object)go == (Object)null)
		{
			Debug.LogError((object)"GetInstantiatedObjectsId() for GO == null.");
			return result;
		}
		PhotonView[] photonViewsInChildren = go.GetPhotonViewsInChildren();
		if (photonViewsInChildren != null && photonViewsInChildren.Length > 0 && (Object)(object)photonViewsInChildren[0] != (Object)null)
		{
			return photonViewsInChildren[0].instantiationId;
		}
		if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
		{
			Debug.Log((object)("GetInstantiatedObjectsId failed for GO: " + go));
		}
		return result;
	}

	private void ServerCleanInstantiateAndDestroy(int instantiateId, int actorNr)
	{
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable[(byte)7] = instantiateId;
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
		raiseEventOptions.CachingOption = EventCaching.RemoveFromRoomCache;
		raiseEventOptions.TargetActors = new int[1] { actorNr };
		RaiseEventOptions raiseEventOptions2 = raiseEventOptions;
		OpRaiseEvent(202, hashtable, sendReliable: true, raiseEventOptions2);
		ExitGames.Client.Photon.Hashtable hashtable2 = new ExitGames.Client.Photon.Hashtable();
		hashtable2[(byte)0] = instantiateId;
		OpRaiseEvent(204, hashtable2, sendReliable: true, null);
	}

	private void SendDestroyOfPlayer(int actorNr)
	{
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable[(byte)0] = actorNr;
		OpRaiseEvent(207, hashtable, sendReliable: true, null);
	}

	private void SendDestroyOfAll()
	{
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable[(byte)0] = -1;
		OpRaiseEvent(207, hashtable, sendReliable: true, null);
	}

	private void OpRemoveFromServerInstantiationsOfPlayer(int actorNr)
	{
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
		raiseEventOptions.CachingOption = EventCaching.RemoveFromRoomCache;
		raiseEventOptions.TargetActors = new int[1] { actorNr };
		RaiseEventOptions raiseEventOptions2 = raiseEventOptions;
		OpRaiseEvent(202, null, sendReliable: true, raiseEventOptions2);
	}

	public void LocalCleanPhotonView(PhotonView view)
	{
		view.destroyedByPhotonNetworkOrQuit = true;
		photonViewList.Remove(view.viewID);
	}

	public PhotonView GetPhotonView(int viewID)
	{
		PhotonView value = null;
		photonViewList.TryGetValue(viewID, out value);
		if ((Object)(object)value == (Object)null)
		{
			PhotonView[] array = Object.FindObjectsOfType(typeof(PhotonView)) as PhotonView[];
			PhotonView[] array2 = array;
			foreach (PhotonView photonView in array2)
			{
				if (photonView.viewID == viewID)
				{
					Debug.LogWarning((object)("Had to lookup view that wasn't in dict: " + photonView));
					return photonView;
				}
			}
		}
		return value;
	}

	public void RegisterPhotonView(PhotonView netView)
	{
		if (!Application.isPlaying)
		{
			photonViewList = new Dictionary<int, PhotonView>();
		}
		else
		{
			if (netView.subId == 0)
			{
				return;
			}
			if (photonViewList.ContainsKey(netView.viewID))
			{
				if ((Object)(object)netView != (Object)(object)photonViewList[netView.viewID])
				{
					Debug.LogError((object)$"PhotonView ID duplicate found: {netView.viewID}. New: {netView} old: {photonViewList[netView.viewID]}. Maybe one wasn't destroyed on scene load?! Check for 'DontDestroyOnLoad'. Destroying old entry, adding new.");
				}
				RemoveInstantiatedGO(((Component)photonViewList[netView.viewID]).gameObject, localOnly: true);
			}
			photonViewList.Add(netView.viewID, netView);
			if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
			{
				Debug.Log((object)("Registered PhotonView: " + netView.viewID));
			}
		}
	}

	public void OpCleanRpcBuffer(int actorNumber)
	{
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
		raiseEventOptions.CachingOption = EventCaching.RemoveFromRoomCache;
		raiseEventOptions.TargetActors = new int[1] { actorNumber };
		RaiseEventOptions raiseEventOptions2 = raiseEventOptions;
		OpRaiseEvent(200, null, sendReliable: true, raiseEventOptions2);
	}

	public void OpRemoveCompleteCacheOfPlayer(int actorNumber)
	{
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
		raiseEventOptions.CachingOption = EventCaching.RemoveFromRoomCache;
		raiseEventOptions.TargetActors = new int[1] { actorNumber };
		RaiseEventOptions raiseEventOptions2 = raiseEventOptions;
		OpRaiseEvent(0, null, sendReliable: true, raiseEventOptions2);
	}

	public void OpRemoveCompleteCache()
	{
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
		raiseEventOptions.CachingOption = EventCaching.RemoveFromRoomCache;
		raiseEventOptions.Receivers = ReceiverGroup.MasterClient;
		RaiseEventOptions raiseEventOptions2 = raiseEventOptions;
		OpRaiseEvent(0, null, sendReliable: true, raiseEventOptions2);
	}

	private void RemoveCacheOfLeftPlayers()
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary[244] = (byte)0;
		dictionary[247] = (byte)7;
		OpCustom(253, dictionary, sendReliable: true, 0);
	}

	public void CleanRpcBufferIfMine(PhotonView view)
	{
		if (view.ownerId != mLocalActor.ID && !mLocalActor.isMasterClient)
		{
			Debug.LogError((object)string.Concat("Cannot remove cached RPCs on a PhotonView thats not ours! ", view.owner, " scene: ", view.isSceneView));
		}
		else
		{
			OpCleanRpcBuffer(view);
		}
	}

	public void OpCleanRpcBuffer(PhotonView view)
	{
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable[(byte)0] = view.viewID;
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
		raiseEventOptions.CachingOption = EventCaching.RemoveFromRoomCache;
		RaiseEventOptions raiseEventOptions2 = raiseEventOptions;
		OpRaiseEvent(200, hashtable, sendReliable: true, raiseEventOptions2);
	}

	public void RemoveRPCsInGroup(int group)
	{
		foreach (KeyValuePair<int, PhotonView> photonView in photonViewList)
		{
			PhotonView value = photonView.Value;
			if (value.group == group)
			{
				CleanRpcBufferIfMine(value);
			}
		}
	}

	public void SetLevelPrefix(short prefix)
	{
		currentLevelPrefix = prefix;
	}

	internal void RPC(PhotonView view, string methodName, PhotonPlayer player, params object[] parameters)
	{
		if (!blockSendingGroups.Contains(view.group))
		{
			if (view.viewID < 1)
			{
				Debug.LogError((object)("Illegal view ID:" + view.viewID + " method: " + methodName + " GO:" + ((Object)((Component)view).gameObject).name));
			}
			if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
			{
				Debug.Log((object)string.Concat("Sending RPC \"", methodName, "\" to player[", player, "]"));
			}
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable[(byte)0] = view.viewID;
			if (view.prefix > 0)
			{
				hashtable[(byte)1] = (short)view.prefix;
			}
			hashtable[(byte)2] = ServerTimeInMilliSeconds;
			int value = 0;
			if (rpcShortcuts.TryGetValue(methodName, out value))
			{
				hashtable[(byte)5] = (byte)value;
			}
			else
			{
				hashtable[(byte)3] = methodName;
			}
			if (parameters != null && parameters.Length > 0)
			{
				hashtable[(byte)4] = parameters;
			}
			if (mLocalActor == player)
			{
				ExecuteRPC(hashtable, player);
				return;
			}
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
			raiseEventOptions.TargetActors = new int[1] { player.ID };
			RaiseEventOptions raiseEventOptions2 = raiseEventOptions;
			OpRaiseEvent(200, hashtable, sendReliable: true, raiseEventOptions2);
		}
	}

	internal void RPC(PhotonView view, string methodName, PhotonTargets target, params object[] parameters)
	{
		if (blockSendingGroups.Contains(view.group))
		{
			return;
		}
		if (view.viewID < 1)
		{
			Debug.LogError((object)("Illegal view ID:" + view.viewID + " method: " + methodName + " GO:" + ((Object)((Component)view).gameObject).name));
		}
		if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
		{
			Debug.Log((object)("Sending RPC \"" + methodName + "\" to " + target));
		}
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable[(byte)0] = view.viewID;
		if (view.prefix > 0)
		{
			hashtable[(byte)1] = (short)view.prefix;
		}
		hashtable[(byte)2] = ServerTimeInMilliSeconds;
		int value = 0;
		if (rpcShortcuts.TryGetValue(methodName, out value))
		{
			hashtable[(byte)5] = (byte)value;
		}
		else
		{
			hashtable[(byte)3] = methodName;
		}
		if (parameters != null && parameters.Length > 0)
		{
			hashtable[(byte)4] = parameters;
		}
		switch (target)
		{
		case PhotonTargets.All:
		{
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
			raiseEventOptions.InterestGroup = (byte)view.group;
			RaiseEventOptions raiseEventOptions6 = raiseEventOptions;
			OpRaiseEvent(200, hashtable, sendReliable: true, raiseEventOptions6);
			ExecuteRPC(hashtable, mLocalActor);
			break;
		}
		case PhotonTargets.Others:
		{
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
			raiseEventOptions.InterestGroup = (byte)view.group;
			RaiseEventOptions raiseEventOptions5 = raiseEventOptions;
			OpRaiseEvent(200, hashtable, sendReliable: true, raiseEventOptions5);
			break;
		}
		case PhotonTargets.AllBuffered:
		{
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
			raiseEventOptions.CachingOption = EventCaching.AddToRoomCache;
			RaiseEventOptions raiseEventOptions8 = raiseEventOptions;
			OpRaiseEvent(200, hashtable, sendReliable: true, raiseEventOptions8);
			ExecuteRPC(hashtable, mLocalActor);
			break;
		}
		case PhotonTargets.OthersBuffered:
		{
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
			raiseEventOptions.CachingOption = EventCaching.AddToRoomCache;
			RaiseEventOptions raiseEventOptions7 = raiseEventOptions;
			OpRaiseEvent(200, hashtable, sendReliable: true, raiseEventOptions7);
			break;
		}
		case PhotonTargets.MasterClient:
		{
			if (mMasterClient == mLocalActor)
			{
				ExecuteRPC(hashtable, mLocalActor);
				break;
			}
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
			raiseEventOptions.Receivers = ReceiverGroup.MasterClient;
			RaiseEventOptions raiseEventOptions4 = raiseEventOptions;
			OpRaiseEvent(200, hashtable, sendReliable: true, raiseEventOptions4);
			break;
		}
		case PhotonTargets.AllViaServer:
		{
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
			raiseEventOptions.InterestGroup = (byte)view.group;
			raiseEventOptions.Receivers = ReceiverGroup.All;
			RaiseEventOptions raiseEventOptions3 = raiseEventOptions;
			OpRaiseEvent(200, hashtable, sendReliable: true, raiseEventOptions3);
			break;
		}
		case PhotonTargets.AllBufferedViaServer:
		{
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
			raiseEventOptions.InterestGroup = (byte)view.group;
			raiseEventOptions.Receivers = ReceiverGroup.All;
			raiseEventOptions.CachingOption = EventCaching.AddToRoomCache;
			RaiseEventOptions raiseEventOptions2 = raiseEventOptions;
			OpRaiseEvent(200, hashtable, sendReliable: true, raiseEventOptions2);
			break;
		}
		default:
			Debug.LogError((object)("Unsupported target enum: " + target));
			break;
		}
	}

	public void SetReceivingEnabled(int group, bool enabled)
	{
		if (group <= 0)
		{
			Debug.LogError((object)("Error: PhotonNetwork.SetReceivingEnabled was called with an illegal group number: " + group + ". The group number should be at least 1."));
		}
		else if (enabled)
		{
			if (!allowedReceivingGroups.Contains(group))
			{
				allowedReceivingGroups.Add(group);
				byte[] groupsToAdd = new byte[1] { (byte)group };
				OpChangeGroups(null, groupsToAdd);
			}
		}
		else if (allowedReceivingGroups.Contains(group))
		{
			allowedReceivingGroups.Remove(group);
			byte[] groupsToRemove = new byte[1] { (byte)group };
			OpChangeGroups(groupsToRemove, null);
		}
	}

	public void SetSendingEnabled(int group, bool enabled)
	{
		if (!enabled)
		{
			blockSendingGroups.Add(group);
		}
		else
		{
			blockSendingGroups.Remove(group);
		}
	}

	public void NewSceneLoaded()
	{
		if (loadingLevelAndPausedNetwork)
		{
			loadingLevelAndPausedNetwork = false;
			PhotonNetwork.isMessageQueueRunning = true;
		}
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, PhotonView> photonView in photonViewList)
		{
			PhotonView value = photonView.Value;
			if ((Object)(object)value == (Object)null)
			{
				list.Add(photonView.Key);
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			int key = list[i];
			photonViewList.Remove(key);
		}
		if (list.Count > 0 && PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
		{
			Debug.Log((object)("New level loaded. Removed " + list.Count + " scene view IDs from last level."));
		}
	}

	public void RunViewUpdate()
	{
		if (!PhotonNetwork.connected || PhotonNetwork.offlineMode || mActors == null || mActors.Count <= 1)
		{
			return;
		}
		Dictionary<int, ExitGames.Client.Photon.Hashtable> dictionary = new Dictionary<int, ExitGames.Client.Photon.Hashtable>();
		Dictionary<int, ExitGames.Client.Photon.Hashtable> dictionary2 = new Dictionary<int, ExitGames.Client.Photon.Hashtable>();
		foreach (KeyValuePair<int, PhotonView> photonView in photonViewList)
		{
			PhotonView value = photonView.Value;
			if (!((Object)(object)value.observed != (Object)null) || value.synchronization == ViewSynchronization.Off || (value.ownerId != mLocalActor.ID && (!value.isSceneView || mMasterClient != mLocalActor)) || !((Component)value).gameObject.activeInHierarchy || blockSendingGroups.Contains(value.group))
			{
				continue;
			}
			ExitGames.Client.Photon.Hashtable hashtable = OnSerializeWrite(value);
			if (hashtable == null)
			{
				continue;
			}
			if (value.synchronization == ViewSynchronization.ReliableDeltaCompressed || value.mixedModeIsReliable)
			{
				if (!hashtable.ContainsKey((byte)1) && !hashtable.ContainsKey((byte)2))
				{
					continue;
				}
				if (!dictionary.ContainsKey(value.group))
				{
					dictionary[value.group] = new ExitGames.Client.Photon.Hashtable();
					dictionary[value.group][(byte)0] = ServerTimeInMilliSeconds;
					if (currentLevelPrefix >= 0)
					{
						dictionary[value.group][(byte)1] = currentLevelPrefix;
					}
				}
				ExitGames.Client.Photon.Hashtable hashtable2 = dictionary[value.group];
				hashtable2.Add((short)hashtable2.Count, hashtable);
				continue;
			}
			if (!dictionary2.ContainsKey(value.group))
			{
				dictionary2[value.group] = new ExitGames.Client.Photon.Hashtable();
				dictionary2[value.group][(byte)0] = ServerTimeInMilliSeconds;
				if (currentLevelPrefix >= 0)
				{
					dictionary2[value.group][(byte)1] = currentLevelPrefix;
				}
			}
			ExitGames.Client.Photon.Hashtable hashtable3 = dictionary2[value.group];
			hashtable3.Add((short)hashtable3.Count, hashtable);
		}
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
		foreach (KeyValuePair<int, ExitGames.Client.Photon.Hashtable> item in dictionary)
		{
			raiseEventOptions.InterestGroup = (byte)item.Key;
			OpRaiseEvent(206, item.Value, sendReliable: true, raiseEventOptions);
		}
		foreach (KeyValuePair<int, ExitGames.Client.Photon.Hashtable> item2 in dictionary2)
		{
			raiseEventOptions.InterestGroup = (byte)item2.Key;
			OpRaiseEvent(201, item2.Value, sendReliable: false, raiseEventOptions);
		}
	}

	private ExitGames.Client.Photon.Hashtable OnSerializeWrite(PhotonView view)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected Obj, but got Unknown
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Expected Obj, but got Unknown
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		List<object> list = new List<object>();
		if (view.observed is MonoBehaviour)
		{
			PhotonStream photonStream = new PhotonStream(write: true, null);
			PhotonMessageInfo info = new PhotonMessageInfo(mLocalActor, ServerTimeInMilliSeconds, view);
			view.ExecuteOnSerialize(photonStream, info);
			if (photonStream.Count == 0)
			{
				return null;
			}
			list = photonStream.data;
		}
		else if (view.observed is Transform)
		{
			Transform val = (Transform)view.observed;
			if (view.onSerializeTransformOption == OnSerializeTransform.OnlyPosition || view.onSerializeTransformOption == OnSerializeTransform.PositionAndRotation || view.onSerializeTransformOption == OnSerializeTransform.All)
			{
				list.Add(val.localPosition);
			}
			else
			{
				list.Add(null);
			}
			if (view.onSerializeTransformOption == OnSerializeTransform.OnlyRotation || view.onSerializeTransformOption == OnSerializeTransform.PositionAndRotation || view.onSerializeTransformOption == OnSerializeTransform.All)
			{
				list.Add(val.localRotation);
			}
			else
			{
				list.Add(null);
			}
			if (view.onSerializeTransformOption == OnSerializeTransform.OnlyScale || view.onSerializeTransformOption == OnSerializeTransform.All)
			{
				list.Add(val.localScale);
			}
		}
		else
		{
			if (!(view.observed is Rigidbody))
			{
				Debug.LogError((object)("Observed type is not serializable: " + ((object)view.observed).GetType()));
				return null;
			}
			Rigidbody val2 = (Rigidbody)view.observed;
			if (view.onSerializeRigidBodyOption != OnSerializeRigidBody.OnlyAngularVelocity)
			{
				list.Add(val2.velocity);
			}
			else
			{
				list.Add(null);
			}
			if (view.onSerializeRigidBodyOption != OnSerializeRigidBody.OnlyVelocity)
			{
				list.Add(val2.angularVelocity);
			}
		}
		object[] array = list.ToArray();
		if (view.synchronization == ViewSynchronization.UnreliableOnChange)
		{
			if (AlmostEquals(array, view.lastOnSerializeDataSent))
			{
				if (view.mixedModeIsReliable)
				{
					return null;
				}
				view.mixedModeIsReliable = true;
				view.lastOnSerializeDataSent = array;
			}
			else
			{
				view.mixedModeIsReliable = false;
				view.lastOnSerializeDataSent = array;
			}
		}
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable[(byte)0] = view.viewID;
		hashtable[(byte)1] = array;
		if (view.synchronization == ViewSynchronization.ReliableDeltaCompressed)
		{
			bool flag = DeltaCompressionWrite(view, hashtable);
			view.lastOnSerializeDataSent = array;
			if (!flag)
			{
				return null;
			}
		}
		return hashtable;
	}

	private void OnSerializeRead(ExitGames.Client.Photon.Hashtable data, PhotonPlayer sender, int networkTime, short correctPrefix)
	{
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Expected Obj, but got Unknown
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Expected Obj, but got Unknown
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		int num = (int)data[(byte)0];
		PhotonView photonView = GetPhotonView(num);
		if ((Object)(object)photonView == (Object)null)
		{
			Debug.LogWarning((object)("Received OnSerialization for view ID " + num + ". We have no such PhotonView! Ignored this if you're leaving a room. State: " + State));
		}
		else if (photonView.prefix > 0 && correctPrefix != photonView.prefix)
		{
			Debug.LogError((object)("Received OnSerialization for view ID " + num + " with prefix " + correctPrefix + ". Our prefix is " + photonView.prefix));
		}
		else
		{
			if (photonView.group != 0 && !allowedReceivingGroups.Contains(photonView.group))
			{
				return;
			}
			if (photonView.synchronization == ViewSynchronization.ReliableDeltaCompressed)
			{
				if (!DeltaCompressionRead(photonView, data))
				{
					if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
					{
						Debug.Log((object)("Skipping packet for " + ((Object)photonView).name + " [" + photonView.viewID + "] as we haven't received a full packet for delta compression yet. This is OK if it happens for the first few frames after joining a game."));
					}
					return;
				}
				photonView.lastOnSerializeDataReceived = data[(byte)1] as object[];
			}
			if (photonView.observed is MonoBehaviour)
			{
				object[] incomingData = data[(byte)1] as object[];
				PhotonStream pStream = new PhotonStream(write: false, incomingData);
				PhotonMessageInfo info = new PhotonMessageInfo(sender, networkTime, photonView);
				photonView.ExecuteOnSerialize(pStream, info);
			}
			else if (photonView.observed is Transform)
			{
				object[] array = data[(byte)1] as object[];
				Transform val = (Transform)photonView.observed;
				if (array.Length >= 1 && array[0] != null)
				{
					val.localPosition = (Vector3)array[0];
				}
				if (array.Length >= 2 && array[1] != null)
				{
					val.localRotation = (Quaternion)array[1];
				}
				if (array.Length >= 3 && array[2] != null)
				{
					val.localScale = (Vector3)array[2];
				}
			}
			else if (photonView.observed is Rigidbody)
			{
				object[] array2 = data[(byte)1] as object[];
				Rigidbody val2 = (Rigidbody)photonView.observed;
				if (array2.Length >= 1 && array2[0] != null)
				{
					val2.velocity = (Vector3)array2[0];
				}
				if (array2.Length >= 2 && array2[1] != null)
				{
					val2.angularVelocity = (Vector3)array2[1];
				}
			}
			else
			{
				Debug.LogError((object)"Type of observed is unknown when receiving.");
			}
		}
	}

	private bool AlmostEquals(object[] lastData, object[] currentContent)
	{
		if (lastData == null && currentContent == null)
		{
			return true;
		}
		if (lastData == null || currentContent == null || lastData.Length != currentContent.Length)
		{
			return false;
		}
		for (int i = 0; i < currentContent.Length; i++)
		{
			object one = currentContent[i];
			object two = lastData[i];
			if (!ObjectIsSameWithInprecision(one, two))
			{
				return false;
			}
		}
		return true;
	}

	private bool DeltaCompressionWrite(PhotonView view, ExitGames.Client.Photon.Hashtable data)
	{
		if (view.lastOnSerializeDataSent == null)
		{
			return true;
		}
		object[] lastOnSerializeDataSent = view.lastOnSerializeDataSent;
		if (!(data[(byte)1] is object[] array))
		{
			return false;
		}
		if (lastOnSerializeDataSent.Length != array.Length)
		{
			return true;
		}
		object[] array2 = new object[array.Length];
		int num = 0;
		List<int> list = new List<int>();
		for (int i = 0; i < array2.Length; i++)
		{
			object obj = array[i];
			object two = lastOnSerializeDataSent[i];
			if (ObjectIsSameWithInprecision(obj, two))
			{
				num++;
				continue;
			}
			array2[i] = array[i];
			if (obj == null)
			{
				list.Add(i);
			}
		}
		if (num > 0)
		{
			data.Remove((byte)1);
			if (num == array.Length)
			{
				return false;
			}
			data[(byte)2] = array2;
			if (list.Count > 0)
			{
				data[(byte)3] = list.ToArray();
			}
		}
		return true;
	}

	private bool DeltaCompressionRead(PhotonView view, ExitGames.Client.Photon.Hashtable data)
	{
		if (data.ContainsKey((byte)1))
		{
			return true;
		}
		if (view.lastOnSerializeDataReceived == null)
		{
			return false;
		}
		if (!(data[(byte)2] is object[] array))
		{
			return false;
		}
		int[] array2 = data[(byte)3] as int[];
		if (array2 == null)
		{
			array2 = new int[0];
		}
		object[] lastOnSerializeDataReceived = view.lastOnSerializeDataReceived;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == null && !array2.Contains(i))
			{
				object obj = lastOnSerializeDataReceived[i];
				array[i] = obj;
			}
		}
		data[(byte)1] = array;
		return true;
	}

	private bool ObjectIsSameWithInprecision(object one, object two)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		if (one == null || two == null)
		{
			return one == null && two == null;
		}
		if (!one.Equals(two))
		{
			if (one is Vector3 target)
			{
				Vector3 second = (Vector3)two;
				if (target.AlmostEquals(second, PhotonNetwork.precisionForVectorSynchronization))
				{
					return true;
				}
			}
			else if (one is Vector2 target2)
			{
				Vector2 second2 = (Vector2)two;
				if (target2.AlmostEquals(second2, PhotonNetwork.precisionForVectorSynchronization))
				{
					return true;
				}
			}
			else if (one is Quaternion target3)
			{
				Quaternion second3 = (Quaternion)two;
				if (target3.AlmostEquals(second3, PhotonNetwork.precisionForQuaternionSynchronization))
				{
					return true;
				}
			}
			else if (one is float target4)
			{
				float second4 = (float)two;
				if (target4.AlmostEquals(second4, PhotonNetwork.precisionForFloatSynchronization))
				{
					return true;
				}
			}
			return false;
		}
		return true;
	}

	protected internal static bool GetMethod(MonoBehaviour monob, string methodType, out MethodInfo mi)
	{
		mi = null;
		if ((Object)(object)monob == (Object)null || string.IsNullOrEmpty(methodType))
		{
			return false;
		}
		List<MethodInfo> methods = SupportClass.GetMethods(((object)monob).GetType(), null);
		for (int i = 0; i < methods.Count; i++)
		{
			MethodInfo methodInfo = methods[i];
			if (methodInfo.Name.Equals(methodType))
			{
				mi = methodInfo;
				return true;
			}
		}
		return false;
	}

	protected internal void LoadLevelIfSynced()
	{
		if (!PhotonNetwork.automaticallySyncScene || PhotonNetwork.isMasterClient || PhotonNetwork.room == null || !PhotonNetwork.room.customProperties.ContainsKey("curScn"))
		{
			return;
		}
		object obj = PhotonNetwork.room.customProperties["curScn"];
		if (obj is int)
		{
			if (Application.loadedLevel != (int)obj)
			{
				PhotonNetwork.LoadLevel((int)obj);
			}
		}
		else if (obj is string && Application.loadedLevelName != (string)obj)
		{
			PhotonNetwork.LoadLevel((string)obj);
		}
	}

	protected internal void SetLevelInPropsIfSynced(object levelId)
	{
		if (!PhotonNetwork.automaticallySyncScene || !PhotonNetwork.isMasterClient || PhotonNetwork.room == null)
		{
			return;
		}
		if (levelId == null)
		{
			Debug.LogError((object)"Parameter levelId can't be null!");
			return;
		}
		if (PhotonNetwork.room.customProperties.ContainsKey("curScn"))
		{
			object obj = PhotonNetwork.room.customProperties["curScn"];
			if ((obj is int && Application.loadedLevel == (int)obj) || (obj is string && Application.loadedLevelName.Equals((string)obj)))
			{
				return;
			}
		}
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		if (levelId is int)
		{
			hashtable["curScn"] = (int)levelId;
		}
		else if (levelId is string)
		{
			hashtable["curScn"] = (string)levelId;
		}
		else
		{
			Debug.LogError((object)"Parameter levelId must be int or string!");
		}
		PhotonNetwork.room.SetCustomProperties(hashtable);
		SendOutgoingCommands();
	}

	public void SetApp(string appId, string gameVersion)
	{
		mAppId = appId.Trim();
		mAppVersion = gameVersion.Trim() + "_1.25";
	}
}
