using System;
using System.Collections.Generic;
using Photon;
using UnityEngine;

public class Level : MonoBehaviour
{
	public static Level Instance;

	private List<string> _Administrators = new List<string>();

	private List<string> _Moderators = new List<string>();

	private List<string> _Builders = new List<string>();

	private List<string> _Banned = new List<string>();

	private bool _CanBuild;

	public string[] Banned => _Banned.ToArray();

	public bool CanBuild => _CanBuild || GameType.BattleMode();

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		ProfileINI.Save();
		GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
		GC.WaitForPendingFinalizers();
		GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
		EntityBase.Entities.Clear();
		Rail.ClearAttachedRails();
		if (App.Instance.Settings.isServer)
		{
			WorldGameObject.Instance.StartServer();
			if (App.Instance.Settings.isServerAdministrator)
			{
				_Administrators.Add(PhotonNetwork.playerName);
			}
			_Builders.Add(PhotonNetwork.playerName);
			_CanBuild = true;
			return;
		}
		PhotonNetwork.isMessageQueueRunning = true;
		bool flag = false;
		PhotonPlayer[] otherPlayers = PhotonNetwork.otherPlayers;
		foreach (PhotonPlayer photonPlayer in otherPlayers)
		{
			if (photonPlayer.name == PhotonNetwork.playerName && (string)photonPlayer.customProperties["player_id"] == VkontakteAPI.INSTANCE._viewerId)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			PhotonNetwork.LeaveRoom();
			MainMenu.Instance.ShowLoading("LOADING_LEAVING", string.Empty);
			Application.LoadLevel("Menu");
			return;
		}
		WorldGameObject.Instance.photonView.RPC("SendPlayerProperties", PhotonTargets.AllBuffered, PhotonNetwork.playerName, ProfileINI.GetActualSkin(), VkontakteAPI.INSTANCE._viewerId, true, PhotonNetwork.player, ProfileINI.level);
		photonView.RPC("GetActualData", PhotonTargets.MasterClient);
	}

	[RPC]
	private void GetActualData(PhotonMessageInfo info)
	{
		photonView.RPC("SetActualData", info.sender, _Administrators.ToArray(), _Moderators.ToArray(), _Builders.ToArray(), _Banned.ToArray());
	}

	[RPC]
	private void SetActualData(string[] administrators, string[] moderators, string[] builders, string[] banned)
	{
		_Administrators = new List<string>(administrators);
		_Moderators = new List<string>(moderators);
		_Builders = new List<string>(builders);
		_Banned = new List<string>(banned);
		_CanBuild = false;
		foreach (string builder in _Builders)
		{
			if (builder == PhotonNetwork.playerName)
			{
				_CanBuild = true;
				break;
			}
		}
	}

	private bool IsTrustCall(PhotonMessageInfo info)
	{
		return info == null || IsAdministrator(info.sender.name) || IsModerator(info.sender.name);
	}

	[RPC]
	public void AddAdministrator(string playerName, PhotonMessageInfo info = null)
	{
		if (IsTrustCall(info))
		{
			_Administrators.Add(playerName);
			if (info == null)
			{
				photonView.RPC("AddAdministrator", PhotonTargets.Others, playerName);
			}
		}
	}

	[RPC]
	public void RemoveAdministrator(string playerName, PhotonMessageInfo info = null)
	{
		if (IsTrustCall(info))
		{
			_Administrators.Remove(playerName);
			if (info == null)
			{
				photonView.RPC("RemoveAdministrator", PhotonTargets.Others, playerName);
			}
		}
	}

	public bool IsAdministrator(string playerName = null)
	{
		return _Administrators.Contains((playerName == null) ? PhotonNetwork.playerName : playerName);
	}

	[RPC]
	public void AddModerator(string playerName, PhotonMessageInfo info = null)
	{
		if (IsTrustCall(info))
		{
			_Moderators.Add(playerName);
			if (info == null)
			{
				photonView.RPC("AddModerator", PhotonTargets.Others, playerName);
			}
		}
	}

	[RPC]
	public void RemoveModerator(string playerName, PhotonMessageInfo info = null)
	{
		if (IsTrustCall(info))
		{
			_Moderators.Remove(playerName);
			if (info == null)
			{
				photonView.RPC("RemoveModerator", PhotonTargets.Others, playerName);
			}
		}
	}

	public bool IsModerator(string playerName = null)
	{
		return _Moderators.Contains((playerName == null) ? PhotonNetwork.playerName : playerName);
	}

	[RPC]
	public void AddBuilder(string playerName, PhotonMessageInfo info = null)
	{
		if (IsTrustCall(info))
		{
			_Builders.Add(playerName);
			if (info == null)
			{
				photonView.RPC("AddBuilder", PhotonTargets.Others, playerName);
			}
			if (playerName == PhotonNetwork.playerName)
			{
				_CanBuild = true;
			}
		}
	}

	[RPC]
	public void RemoveBuilder(string playerName, PhotonMessageInfo info = null)
	{
		if (IsTrustCall(info))
		{
			_Builders.Remove(playerName);
			if (info == null)
			{
				photonView.RPC("RemoveBuilder", PhotonTargets.Others, playerName);
			}
			if (playerName == PhotonNetwork.playerName)
			{
				_CanBuild = false;
			}
		}
	}

	public bool IsBuilder(string playerName = null)
	{
		return _Builders.Contains((playerName == null) ? PhotonNetwork.playerName : playerName);
	}

	[RPC]
	public void AddBanned(string playerName, PhotonMessageInfo info = null)
	{
		if (IsTrustCall(info))
		{
			_Banned.Add(playerName);
			if (info == null)
			{
				photonView.RPC("AddBanned", PhotonTargets.Others, playerName);
			}
		}
	}

	[RPC]
	public void RemoveBanned(string playerName, PhotonMessageInfo info = null)
	{
		if (IsTrustCall(info))
		{
			_Banned.Remove(playerName);
			if (info == null)
			{
				photonView.RPC("RemoveBanned", PhotonTargets.Others, playerName);
			}
		}
	}

	public bool IsBanned(string playerName = null)
	{
		return _Banned.Contains((playerName == null) ? PhotonNetwork.playerName : playerName);
	}
}
