using System;
using System.Threading;
using ExitGames.Client.Photon;
using Photon;
using UnityEngine;

internal class PhotonHandler : MonoBehaviour, IPhotonPeerListener
{
	public static PhotonHandler SP;

	public int updateInterval;

	public int updateIntervalOnSerialize;

	private int nextSendTickCount = Environment.TickCount;

	private int nextSendTickCountOnSerialize = Environment.TickCount;

	private void Awake()
	{
		if ((Object)(object)SP != (Object)null && (Object)(object)SP != (Object)(object)this)
		{
			Debug.LogError((object)"Error: we already have an PhotonMono around!");
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
		Object.DontDestroyOnLoad((Object)(object)this);
		SP = this;
		updateInterval = 1000 / PhotonNetwork.sendRate;
		updateIntervalOnSerialize = 1000 / PhotonNetwork.sendRateOnSerialize;
	}

	private void Update()
	{
		if (PhotonNetwork.networkingPeer == null)
		{
			Debug.LogError((object)"NetworkPeer broke!");
		}
		else
		{
			if (PhotonNetwork.connectionStateDetailed == PeerState.PeerCreated || PhotonNetwork.connectionStateDetailed == PeerState.Disconnected || !PhotonNetwork.isMessageQueueRunning)
			{
				return;
			}
			bool flag = true;
			while (PhotonNetwork.isMessageQueueRunning && flag)
			{
				flag = PhotonNetwork.networkingPeer.DispatchIncomingCommands();
			}
			if (PhotonNetwork.isMessageQueueRunning && Environment.TickCount > nextSendTickCountOnSerialize)
			{
				PhotonNetwork.networkingPeer.RunViewUpdate();
				nextSendTickCountOnSerialize = Environment.TickCount + updateIntervalOnSerialize;
			}
			if (Environment.TickCount > nextSendTickCount)
			{
				bool flag2 = true;
				while (PhotonNetwork.isMessageQueueRunning && flag2)
				{
					flag2 = PhotonNetwork.networkingPeer.SendOutgoingCommands();
				}
				nextSendTickCount = Environment.TickCount + updateInterval;
			}
		}
	}

	public void OnApplicationQuit()
	{
		PhotonNetwork.Disconnect();
	}

	public void OnLevelWasLoaded(int level)
	{
		PhotonNetwork.networkingPeer.NewSceneLoaded();
	}

	public static void StartThread()
	{
		Thread thread = new Thread(MyThread);
		thread.Start();
	}

	public static void MyThread()
	{
		while (PhotonNetwork.networkingPeer != null && PhotonNetwork.networkingPeer.IsSendingOnlyAcks)
		{
			while (PhotonNetwork.networkingPeer.SendOutgoingCommands())
			{
			}
			Thread.Sleep(200);
		}
	}

	public void DebugReturn(DebugLevel level, string message)
	{
		switch (level)
		{
		case DebugLevel.ERROR:
			Debug.LogError((object)message);
			return;
		case DebugLevel.WARNING:
			Debug.LogWarning((object)message);
			return;
		case DebugLevel.INFO:
			if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
			{
				Debug.Log((object)message);
				return;
			}
			break;
		}
		if (level == DebugLevel.ALL && PhotonNetwork.logLevel == PhotonLogLevel.Full)
		{
			Debug.Log((object)message);
		}
	}

	public void OnOperationResponse(OperationResponse operationResponse)
	{
	}

	public void OnStatusChanged(StatusCode statusCode)
	{
	}

	public void OnEvent(EventData photonEvent)
	{
	}
}
