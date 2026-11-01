using System;
using System.Collections;
using System.IO;
using ExitGames.Client.Photon;
using UnityEngine;

public static class PhotonNetwork
{
	public const string versionPUN = "1.17";

	public const string serverSettingsAssetPath = "Assets/Photon Unity Networking/Resources/PhotonServerSettings.asset";

	internal static readonly PhotonHandler photonMono;

	internal static readonly NetworkingPeer networkingPeer;

	public static readonly int MAX_VIEW_IDS;

	public static ServerSettings PhotonServerSettings;

	public static float precisionForVectorSynchronization;

	public static float precisionForQuaternionSynchronization;

	public static float precisionForFloatSynchronization;

	public static PhotonLogLevel logLevel;

	private static bool isOfflineMode;

	private static bool offlineMode_inRoom;

	private static bool m_autoCleanUpPlayerObjects;

	private static bool autoJoinLobbyField;

	private static int sendInterval;

	private static int sendIntervalOnSerialize;

	private static bool m_isMessageQueueRunning;

	public static bool connected
	{
		get
		{
			if (offlineMode)
			{
				return true;
			}
			return connectionState == ConnectionState.Connected;
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
				return PeerState.Connected;
			}
			if (networkingPeer == null)
			{
				return PeerState.Disconnected;
			}
			return networkingPeer.State;
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
					return new Room("OfflineRoom", new Hashtable());
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
			m_autoCleanUpPlayerObjects = value;
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
			if (value != m_isMessageQueueRunning)
			{
				networkingPeer.IsSendingOnlyAcks = !value;
				m_isMessageQueueRunning = value;
				if (!value)
				{
					PhotonHandler.StartThread();
				}
			}
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

	public static bool isNonMasterClientInRoom => !isMasterClient && room != null;

	public static int countOfPlayersOnMaster => networkingPeer.mMasterCount;

	public static int countOfPlayersInRooms => countOfPlayers;

	public static int countOfPlayers => networkingPeer.mPeerCount + networkingPeer.mMasterCount;

	public static int countOfRooms
	{
		get
		{
			if (insideLobby)
			{
				return GetRoomList().Length;
			}
			return networkingPeer.mGameCount;
		}
	}

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

	static PhotonNetwork()
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected Obj, but got Unknown
		MAX_VIEW_IDS = 4000;
		PhotonServerSettings = (ServerSettings)(object)Resources.Load(Path.GetFileNameWithoutExtension("Assets/Photon Unity Networking/Resources/PhotonServerSettings.asset"), typeof(ServerSettings));
		precisionForVectorSynchronization = 9.9E-05f;
		precisionForQuaternionSynchronization = 1f;
		precisionForFloatSynchronization = 0.01f;
		logLevel = PhotonLogLevel.ErrorsOnly;
		isOfflineMode = false;
		offlineMode_inRoom = false;
		m_autoCleanUpPlayerObjects = true;
		autoJoinLobbyField = true;
		sendInterval = 50;
		sendIntervalOnSerialize = 100;
		m_isMessageQueueRunning = true;
		Application.runInBackground = true;
		GameObject val = new GameObject();
		photonMono = val.AddComponent<PhotonHandler>();
		((Object)val).name = "PhotonMono";
		((Object)val).hideFlags = (HideFlags)1;
		networkingPeer = new NetworkingPeer(photonMono, string.Empty, ConnectionProtocol.Udp);
		networkingPeer.LimitOfUnreliableCommands = 20;
		CustomTypes.Register();
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

	public static void ConnectUsingSettings(string gameVersion)
	{
		if ((Object)(object)PhotonServerSettings == (Object)null)
		{
			Debug.LogError((object)"Loading the settings file failed. Check path: Assets/Photon Unity Networking/Resources/PhotonServerSettings.asset");
		}
		else if (PhotonServerSettings.HostType == ServerSettings.HostingOption.OfflineMode)
		{
			offlineMode = true;
		}
		else
		{
			Connect(PhotonServerSettings.ServerAddress, PhotonServerSettings.ServerPort, PhotonServerSettings.AppID, gameVersion);
		}
	}

	[Obsolete("This method is obsolete; use ConnectUsingSettings with the gameVersion argument instead")]
	public static void ConnectUsingSettings()
	{
		ConnectUsingSettings("1.0");
	}

	[Obsolete("This method is obsolete; use Connect with the gameVersion argument instead")]
	public static void Connect(string serverAddress, int port, string uniqueGameID)
	{
		Connect(serverAddress, port, uniqueGameID, "1.0");
	}

	public static void Connect(string serverAddress, int port, string appID, string gameVersion)
	{
		if (port <= 0)
		{
			Debug.LogError((object)("Aborted Connect: invalid port: " + port));
			return;
		}
		if (serverAddress.Length <= 2)
		{
			Debug.LogError((object)("Aborted Connect: invalid serverAddress: " + serverAddress));
			return;
		}
		if (networkingPeer.PeerState != PeerStateValue.Disconnected)
		{
			Debug.LogWarning((object)("Connect() only works when disconnected. Current state: " + networkingPeer.PeerState));
			return;
		}
		if (offlineMode)
		{
			offlineMode = false;
			Debug.LogWarning((object)"Shut down offline mode due to a connect attempt");
		}
		if (!isMessageQueueRunning)
		{
			isMessageQueueRunning = true;
			Debug.LogWarning((object)"Forced enabling of isMessageQueueRunning because of a Connect()");
		}
		serverAddress = serverAddress + ":" + port;
		networkingPeer.mAppVersion = gameVersion + "1.17";
		networkingPeer.Connect(serverAddress, appID, 0);
	}

	public static void Disconnect()
	{
		if (networkingPeer != null)
		{
			networkingPeer.Disconnect();
		}
	}

	[Obsolete("Used for compatibility with Unity networking only. Encryption is automatically initialized while connecting.")]
	public static void InitializeSecurity()
	{
	}

	public static void CreateRoom(string roomName)
	{
		Debug.Log((object)("this custom props " + player.customProperties.ToStringFull()));
		if (connectionStateDetailed == PeerState.ConnectedToGameserver || connectionStateDetailed == PeerState.Joining || connectionStateDetailed == PeerState.Joined)
		{
			Debug.LogError((object)"CreateRoom aborted: You are already connecting to a room!");
		}
		else if (room != null)
		{
			Debug.LogError((object)"CreateRoom aborted: You are already in a room!");
		}
		else if (offlineMode)
		{
			offlineMode_inRoom = true;
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnCreatedRoom);
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnJoinedRoom);
		}
		else
		{
			networkingPeer.OpCreateGame(roomName, isVisible: true, isOpen: true, 0, autoCleanUpPlayerObjects, null, null);
		}
	}

	public static void CreateRoom(string roomName, bool isVisible, bool isOpen, int maxPlayers)
	{
		CreateRoom(roomName, isVisible, isOpen, maxPlayers, null, null);
	}

	public static void CreateRoom(string roomName, bool isVisible, bool isOpen, int maxPlayers, Hashtable customRoomProperties, string[] propsToListInLobby)
	{
		if (connectionStateDetailed == PeerState.Joining || connectionStateDetailed == PeerState.Joined || connectionStateDetailed == PeerState.ConnectedToGameserver)
		{
			Debug.LogError((object)"CreateRoom aborted: You can only create a room while not currently connected/connecting to a room.");
			return;
		}
		if (room != null)
		{
			Debug.LogError((object)"CreateRoom aborted: You are already in a room!");
			return;
		}
		if (offlineMode)
		{
			offlineMode_inRoom = true;
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnCreatedRoom);
			return;
		}
		if (maxPlayers > 255)
		{
			Debug.LogError((object)("Error: CreateRoom called with " + maxPlayers + " maxplayers. This has been reverted to the max of 255 players because internally a 'byte' is used."));
			maxPlayers = 255;
		}
		networkingPeer.OpCreateGame(roomName, isVisible, isOpen, (byte)maxPlayers, autoCleanUpPlayerObjects, customRoomProperties, propsToListInLobby);
	}

	public static void JoinRoom(RoomInfo listedRoom)
	{
		if (listedRoom == null)
		{
			Debug.LogError((object)"JoinRoom aborted: you passed a NULL room");
		}
		else
		{
			JoinRoom(listedRoom.name);
		}
	}

	public static void JoinRoom(string roomName)
	{
		if (connectionStateDetailed == PeerState.Joining || connectionStateDetailed == PeerState.Joined || connectionStateDetailed == PeerState.ConnectedToGameserver)
		{
			Debug.LogError((object)"JoinRoom aborted: You can only join a room while not currently connected/connecting to a room.");
		}
		else if (room != null)
		{
			Debug.LogError((object)"JoinRoom aborted: You are already in a room!");
		}
		else if (roomName == string.Empty)
		{
			Debug.LogError((object)"JoinRoom aborted: You must specifiy a room name!");
		}
		else if (offlineMode)
		{
			offlineMode_inRoom = true;
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnJoinedRoom);
		}
		else
		{
			networkingPeer.OpJoin(roomName);
		}
	}

	public static void JoinRandomRoom()
	{
		JoinRandomRoom(null, 0);
	}

	public static void JoinRandomRoom(Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers)
	{
		if (connectionStateDetailed == PeerState.Joining || connectionStateDetailed == PeerState.Joined || connectionStateDetailed == PeerState.ConnectedToGameserver)
		{
			Debug.LogError((object)"JoinRandomRoom aborted: You can only join a room while not currently connected/connecting to a room.");
			return;
		}
		if (room != null)
		{
			Debug.LogError((object)"JoinRandomRoom aborted: You are already in a room!");
			return;
		}
		if (offlineMode)
		{
			offlineMode_inRoom = true;
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnJoinedRoom);
			return;
		}
		Hashtable hashtable = new Hashtable();
		hashtable.MergeStringKeys(expectedCustomRoomProperties);
		if (expectedMaxPlayers > 0)
		{
			hashtable[byte.MaxValue] = expectedMaxPlayers;
		}
		networkingPeer.OpJoinRandomRoom(hashtable);
	}

	public static void LeaveRoom()
	{
		if (!offlineMode && connectionStateDetailed != PeerState.Joined)
		{
			Debug.LogError((object)"PhotonNetwork: Error, you cannot leave a room if you're not in a room!(1)");
		}
		else if (room == null)
		{
			Debug.LogError((object)"PhotonNetwork: Error, you cannot leave a room if you're not in a room!(2)");
		}
		else if (offlineMode)
		{
			offlineMode_inRoom = false;
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnLeftRoom);
		}
		else
		{
			networkingPeer.OpLeave();
		}
	}

	public static RoomInfo[] GetRoomList()
	{
		if (offlineMode)
		{
			return new RoomInfo[0];
		}
		if (networkingPeer == null)
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

	public static PhotonViewID AllocateViewID()
	{
		int i;
		for (i = 0; networkingPeer.allocatedIDs.ContainsKey(i); i++)
		{
		}
		if (i >= MAX_VIEW_IDS)
		{
			Debug.LogError((object)"ERROR: Too many view IDs used!");
			i = 0;
		}
		int iD = i;
		PhotonViewID photonViewID = new PhotonViewID(iD, player);
		networkingPeer.allocatedIDs.Add(i, photonViewID);
		return photonViewID;
	}

	public static void UnAllocateViewID(PhotonViewID viewID)
	{
		UnAllocateViewID(viewID.ID % MAX_VIEW_IDS);
	}

	private static void UnAllocateViewID(int ID)
	{
		networkingPeer.allocatedIDs.Remove(ID);
	}

	public static GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation, int group)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return Instantiate(prefabName, position, rotation, group, null);
	}

	public static GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation, int group, object[] data)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected Obj, but got Unknown
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		if (!VerifyCanUseNetwork())
		{
			Debug.LogError((object)("PhotonNetwork error: Could not Instantiate the prefab [" + prefabName + "] as the game is not connected."));
			return null;
		}
		GameObject val = (GameObject)Resources.Load(prefabName, typeof(GameObject));
		if ((Object)(object)val == (Object)null)
		{
			Debug.LogError((object)("PhotonNetwork error: Could not Instantiate the prefab [" + prefabName + "]. Please verify you have this gameobject in a Resources folder (and not in a subfolder)"));
			return null;
		}
		if ((Object)(object)val.GetComponent<PhotonView>() == (Object)null)
		{
			Debug.LogError((object)("PhotonNetwork error: Could not Instantiate the prefab [" + prefabName + "] as it has no PhotonView attached to the root."));
			return null;
		}
		Component[] componentsInChildren = (Component[])(object)val.GetComponentsInChildren<PhotonView>(true);
		PhotonViewID[] array = new PhotonViewID[componentsInChildren.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = AllocateViewID();
		}
		Hashtable evData = networkingPeer.SendInstantiate(prefabName, position, rotation, group, array, data, isGlobalObject: false);
		return networkingPeer.DoInstantiate(evData, networkingPeer.mLocalActor, val);
	}

	public static GameObject InstantiateSceneObject(string prefabName, Vector3 position, Quaternion rotation, int group, object[] data)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected Obj, but got Unknown
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		if (!VerifyCanUseNetwork())
		{
			return null;
		}
		if (!isMasterClient)
		{
			Debug.LogError((object)"PhotonNetwork error [InstantiateSceneObject]: Only the master client can Instantiate scene objects");
			return null;
		}
		GameObject val = (GameObject)Resources.Load(prefabName, typeof(GameObject));
		if ((Object)(object)val == (Object)null)
		{
			Debug.LogError((object)("PhotonNetwork error [InstantiateSceneObject]: Could not Instantiate the prefab [" + prefabName + "]. Please verify you have this gameobject in a Resources folder (and not in a subfolder)"));
			return null;
		}
		if ((Object)(object)val.GetComponent<PhotonView>() == (Object)null)
		{
			Debug.LogError((object)("PhotonNetwork error [InstantiateSceneObject]: Could not Instantiate the prefab [" + prefabName + "] as it has no PhotonView attached to the root."));
			return null;
		}
		Component[] componentsInChildren = (Component[])(object)val.GetComponentsInChildren<PhotonView>(true);
		PhotonViewID[] array = AllocateSceneViewIDs(componentsInChildren.Length);
		if (array == null)
		{
			Debug.LogError((object)("PhotonNetwork error [InstantiateSceneObject]: Could not Instantiate the prefab [" + prefabName + "] as no ViewIDs are free to use. Max is: " + MAX_VIEW_IDS));
			return null;
		}
		Hashtable evData = networkingPeer.SendInstantiate(prefabName, position, rotation, group, array, data, isGlobalObject: true);
		return networkingPeer.DoInstantiate(evData, networkingPeer.mLocalActor, val);
	}

	private static PhotonViewID[] AllocateSceneViewIDs(int number)
	{
		PhotonViewID[] array = new PhotonViewID[number];
		if (!(Resources.FindObjectsOfTypeAll(typeof(PhotonView)) is PhotonView[] array2) || array2.Length == MAX_VIEW_IDS)
		{
			return null;
		}
		int num = MAX_VIEW_IDS;
		for (int i = 0; i < number; i++)
		{
			int num2;
			for (num2 = num - 1; num2 >= 1; num2--)
			{
				bool flag = false;
				PhotonView[] array3 = array2;
				foreach (PhotonView photonView in array3)
				{
					if (photonView.viewID != null && photonView.viewID.ID == num2)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					array[i] = new PhotonViewID(num2, null);
					num = num2;
					break;
				}
			}
			if (num != num2)
			{
				Debug.Log((object)"SceneView ID lookup failed.");
				return null;
			}
		}
		return array;
	}

	public static int GetPing()
	{
		return networkingPeer.RoundTripTime;
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

	public static void CloseConnection(PhotonPlayer kickPlayer)
	{
		if (VerifyCanUseNetwork())
		{
			if (!player.isMasterClient)
			{
				Debug.LogError((object)"CloseConnection: Only the masterclient can kick another player.");
			}
			if (kickPlayer == null)
			{
				Debug.LogError((object)"CloseConnection: No such player connected!");
				return;
			}
			int[] targetActors = new int[1] { kickPlayer.ID };
			networkingPeer.OpRaiseEvent(203, null, sendReliable: true, 0, targetActors);
		}
	}

	public static void Destroy(PhotonView view)
	{
		if ((Object)(object)view != (Object)null && view.isMine)
		{
			int instantiatedObjectsId = networkingPeer.GetInstantiatedObjectsId(((Component)view).gameObject);
			if (instantiatedObjectsId == -1)
			{
				networkingPeer.DestroyPhotonView(view, localOnly: false);
			}
			else
			{
				Destroy(((Component)view).gameObject);
			}
		}
		else
		{
			Debug.LogError((object)string.Concat("Destroy: Could not destroy view ID [", view, "]. Does not exist, or is not ours!"));
		}
	}

	public static void Destroy(GameObject go)
	{
		PhotonView component = go.GetComponent<PhotonView>();
		if ((Object)(object)component == (Object)null)
		{
			Debug.LogError((object)("Cannot call Destroy(GameObject go); on the gameobject \"" + ((Object)go).name + "\" as it has no PhotonView attached."));
		}
		else if (component.isMine)
		{
			int instantiatedObjectsId = networkingPeer.GetInstantiatedObjectsId(go);
			if (instantiatedObjectsId == -1)
			{
				Debug.LogError((object)("Cannot call Destroy(GameObject go); on the gameobject \"" + ((Object)go).name + "\" as it was not instantiated using PhotonNetwork.Instantiate."));
			}
			else
			{
				networkingPeer.RemoveInstantiatedGO(go, localOnly: false);
			}
		}
		else
		{
			Debug.LogError((object)string.Concat("Cannot call Destroy(GameObject go); on the gameobject \"", ((Object)go).name, "\" as we don't control it (Owner: ", component.owner, ")."));
		}
	}

	public static void DestroyPlayerObjects(PhotonPlayer destroyPlayer)
	{
		if (VerifyCanUseNetwork())
		{
			if (player.isMasterClient || destroyPlayer == player)
			{
				networkingPeer.DestroyPlayerObjects(destroyPlayer, localOnly: false);
			}
			else
			{
				Debug.LogError((object)string.Concat("Couldn't destroy objects for player \"", destroyPlayer, "\" as we are not the masterclient."));
			}
		}
	}

	public static void RemoveAllInstantiatedObjects()
	{
		if (isMasterClient)
		{
			networkingPeer.RemoveAllInstantiatedObjects();
		}
		else
		{
			Debug.LogError((object)"Couldn't call RemoveAllInstantiatedObjects as only the master client is allowed to call this.");
		}
	}

	public static void RemoveAllInstantiatedObjects(PhotonPlayer targetPlayer)
	{
		if (VerifyCanUseNetwork())
		{
			if (player.isMasterClient || targetPlayer == player)
			{
				networkingPeer.RemoveAllInstantiatedObjectsByPlayer(targetPlayer, localOnly: false);
			}
			else
			{
				Debug.LogError((object)string.Concat("Couldn't RemoveAllInstantiatedObjects for player \"", targetPlayer, "\" as only the master client or the player itself is allowed to call this."));
			}
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

	internal static void UnreableRPC(PhotonView view, string methodName, PhotonTargets target, params object[] parameters)
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

	public static void RemoveRPCs()
	{
		if (VerifyCanUseNetwork())
		{
			RemoveRPCs(player);
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
				networkingPeer.RemoveRPCs(targetPlayer.ID);
			}
		}
	}

	public static void RemoveAllBufferedMessages()
	{
		if (VerifyCanUseNetwork())
		{
			RemoveAllBufferedMessages(player);
		}
	}

	public static void RemoveAllBufferedMessages(PhotonPlayer targetPlayer)
	{
		if (VerifyCanUseNetwork())
		{
			if (!targetPlayer.isLocal && !isMasterClient)
			{
				Debug.LogError((object)"Error; Only the MasterClient can call RemoveAllBufferedMessages for other players.");
			}
			else
			{
				networkingPeer.RemoveCompleteCacheOfPlayer(targetPlayer.ID);
			}
		}
	}

	public static void RemoveRPCs(PhotonView view)
	{
		if (VerifyCanUseNetwork())
		{
			networkingPeer.RemoveRPCs(view);
		}
	}

	public static void RemoveRPCsInGroup(int group)
	{
		if (VerifyCanUseNetwork())
		{
			networkingPeer.RemoveRPCsInGroup(group);
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
		if (networkingPeer != null && (offlineMode || connected))
		{
			return true;
		}
		Debug.LogError((object)"Cannot send messages when not connected; Either connect to Photon OR use offline mode!");
		return false;
	}
}
