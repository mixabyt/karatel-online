using System.Collections;
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

	public bool CanBuild => _CanBuild;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		((MonoBehaviour)this).StartCoroutine(InitializeGameTypes());
	}

	private IEnumerator InitializeGameTypes()
	{
		while (!PhotonNetwork.isMasterClient)
		{
			yield return 0;
		}
		if (App.instance.gameIni.gameType == GameINI.GameType.TEAM_BATTLE)
		{
			PhotonNetwork.InstantiateSceneObject("GameType/TeamBattle", Vector3.zero, Quaternion.identity, 0, null);
		}
		else if (App.instance.gameIni.gameType == GameINI.GameType.SURVIVAL)
		{
			PhotonNetwork.InstantiateSceneObject("GameType/Survival", Vector3.zero, Quaternion.identity, 0, null);
		}
		else if (App.instance.gameIni.gameType == GameINI.GameType.DEATHMATCH)
		{
			PhotonNetwork.InstantiateSceneObject("GameType/DeathMatch", Vector3.zero, Quaternion.identity, 0, null);
		}
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
