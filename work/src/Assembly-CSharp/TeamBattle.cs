using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamBattle : GameType
{
	protected class Player
	{
		public SecuredValue<string> Name;

		public SecuredValue<int> Score = 0;

		public SecuredValue<int> Kills = 0;

		public SecuredValue<int> Deads = 0;

		public object[] Data
		{
			get
			{
				return new object[3]
				{
					(int)Score,
					(int)Kills,
					(int)Deads
				};
			}
			set
			{
				Score = (int)value[0];
				Kills = (int)value[1];
				Deads = (int)value[2];
			}
		}

		public int Level => WorldGameObject.Instance.FindPlayerByName(Name)?.Level ?? 1;

		public Player(string name, object[] data)
		{
			Name = name;
			if (data != null)
			{
				Data = data;
			}
		}

		public void Reset()
		{
			Score = 0;
			Kills = 0;
			Deads = 0;
		}
	}

	protected class Team
	{
		public List<Player> Players = new List<Player>();

		public SecuredValue<int> Score = 0;

		public Player this[string playerName]
		{
			get
			{
				int num = Players.FindIndex((Player player) => player.Name == playerName);
				return (num == -1) ? null : Players[num];
			}
			set
			{
				int num = Players.FindIndex((Player player) => player.Name == playerName);
				if (value != null)
				{
					if (num != -1)
					{
						Players[num] = value;
					}
					else
					{
						Players.Add(value);
					}
				}
				else if (num != -1)
				{
					Players.RemoveAt(num);
				}
			}
		}

		public Team GetCopy()
		{
			Team team = new Team();
			team.Score = Score.Value;
			foreach (Player player in Players)
			{
				team.Players.Add(new Player(player.Name, player.Data));
			}
			return team;
		}
	}

	public const int TeamsCount = 3;

	public const int UnknownTeamIndex = -1;

	public const int ObserversTeamIndex = 0;

	public const int RedTeamIndex = 1;

	public const int BlueTeamIndex = 2;

	public const float SynchronizationPeriod = 5f;

	public const int MinPlayersToScore = 4;

	public static float RoundDuration = 900f;

	public static TeamBattle Instance;

	internal bool EnableVoiceChat = true;

	protected Team[] _Teams = new Team[3];

	protected Team[] _TeamsResult;

	protected Team[] _TeamsTabMenu;

	protected SecuredValue<bool> _GameEnded = false;

	protected SecuredValue<int> _WinnerTeam = 0;

	protected SecuredValue<float> _GameTime = RoundDuration;

	private float _TimeLastSynchronization;

	private SecuredValue<int> _TryStartTeam = 0;

	public int RedTeamScore => _Teams[1].Score;

	public int BlueTeamScore => _Teams[2].Score;

	protected void Awake()
	{
		Instance = this;
		for (int i = 0; i < _Teams.Length; i++)
		{
			_Teams[i] = new Team();
		}
	}

	protected virtual void Update()
	{
		_GameTime = (float)_GameTime - Time.deltaTime;
		if (PhotonNetwork.isMasterClient && Time.time - _TimeLastSynchronization >= 5f)
		{
			photonView.RPC("SetGameTime", PhotonTargets.Others, (float)_GameTime);
		}
		TimeSpan timeSpan = TimeSpan.FromSeconds(Math.Max(0f, _GameTime));
		KGUI.SetControlText("hud.battle.txt_time", timeSpan.Minutes.ToString("D2") + ":" + timeSpan.Seconds.ToString("D2"));
		if (PhotonNetwork.isMasterClient && (float)_GameTime < 0f && !(this is ZombieVirus))
		{
			if (!_GameEnded)
			{
				int num = ((BlueTeamScore == RedTeamScore) ? (-1) : ((BlueTeamScore <= RedTeamScore) ? 1 : 2));
				photonView.RPC("EndGame", PhotonTargets.All, num);
			}
			else
			{
				photonView.RPC("RestartGame", PhotonTargets.All);
			}
		}
	}

	private IEnumerator Start()
	{
		if (App.Instance.Settings.isServer)
		{
			while (!WorldGameObject.Instance.IsWorldGenerated)
			{
				yield return 0;
			}
			StartPlay(GetDefaultTeam());
			ServerStartGame();
		}
	}

	protected virtual void ServerStartGame()
	{
		_GameTime = RoundDuration;
		_GameEnded = false;
	}

	protected virtual void OnPhotonPlayerConnected(PhotonPlayer connectedPlayer)
	{
		if (PhotonNetwork.isMasterClient)
		{
			for (int i = 0; i < _Teams.Length; i++)
			{
				photonView.RPC("SetTeamData", connectedPlayer, i, (int)_Teams[i].Score);
				foreach (Player player in _Teams[i].Players)
				{
					photonView.RPC("AddToTeam", connectedPlayer, i, (string)player.Name, player.Data);
				}
			}
			photonView.RPC("StartPlay", connectedPlayer, GetDefaultTeam());
			photonView.RPC("SetGameTime", connectedPlayer, (float)_GameTime);
		}
		KGUI.EnableNodes("hud.txt_battle_more_players_tip", PhotonNetwork.playerList.Length < 4);
	}

	protected virtual void OnMasterClientSwitched(PhotonPlayer newMasterClient)
	{
	}

	protected virtual void OnPhotonPlayerDisconnected(PhotonPlayer disconnectedPlayer)
	{
		for (int i = 0; i < 3; i++)
		{
			if (_Teams[i][disconnectedPlayer.name] != null)
			{
				_Teams[i][disconnectedPlayer.name] = null;
			}
		}
		KGUI.EnableNodes("hud.txt_battle_more_players_tip", PhotonNetwork.playerList.Length < 4);
	}

	[RPC]
	public virtual void SetGameTime(float time)
	{
		_GameTime = time;
	}

	[RPC]
	public virtual void StartPlay(int teamIndex)
	{
		if (!WorldGameObject.Instance.IsWorldGenerated)
		{
			_TryStartTeam = teamIndex;
			((MonoBehaviour)this).StopCoroutine("TryStartPlayProcess");
			((MonoBehaviour)this).StartCoroutine("TryStartPlayProcess");
			return;
		}
		for (int i = 0; i < 3; i++)
		{
			if (_Teams[i][PhotonNetwork.playerName] != null)
			{
				RemoveFromTeam(i, PhotonNetwork.playerName);
			}
		}
		AddToTeam(teamIndex, PhotonNetwork.playerName, null);
		WorldGameObject.Instance.TeleportMainPlayerToSpawnPoint();
		for (int j = 0; j < 3; j++)
		{
			foreach (Player player in _Teams[j].Players)
			{
				if (player.Name != PhotonNetwork.playerName)
				{
					SetupPlayer(j, player.Name);
				}
			}
		}
	}

	protected IEnumerator TryStartPlayProcess()
	{
		while (!WorldGameObject.Instance.IsWorldGenerated)
		{
			yield return 0;
		}
		yield return 0;
		StartPlay(_TryStartTeam);
	}

	protected void SetupPlayer(int teamIndex, string playerName)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = null;
		PlayerNode playerNode = null;
		GameObject mainPlayer = WorldGameObject.Instance.MainPlayer;
		if (playerName == PhotonNetwork.playerName)
		{
			val = mainPlayer;
		}
		else
		{
			playerNode = WorldGameObject.Instance.FindPlayerByName(playerName);
			if (playerNode != null)
			{
				val = playerNode.Avatar;
			}
		}
		if ((Object)(object)mainPlayer == (Object)null || (Object)(object)val == (Object)null)
		{
			return;
		}
		val.GetComponent<Nickname>().SetColor(GetTeamColor(teamIndex));
		val.GetComponent<Nickname>().SetRaycast(enable: true);
		bool flag = teamIndex == 0;
		if ((Object)(object)mainPlayer == (Object)(object)val)
		{
			if (flag)
			{
				if (val.GetComponent<CameraController>().IsThirdPerson)
				{
					val.GetComponent<CameraController>().SetFirstPerson();
				}
				val.GetComponent<SkinManager>().DisableAllHands();
			}
			else
			{
				val.GetComponent<SkinManager>().SetHand(ProfileINI.GetActualSkin());
			}
			val.GetComponent<CameraController>().DisableCameraSwitching = flag;
			MainMenu.Instance.RefreshFlying();
			MainMenu.Instance.SetMenu(Menu.Hud, show: true);
		}
		else
		{
			bool flag2 = GameType.IsObserving();
			if (val.activeInHierarchy && val.collider.enabled && mainPlayer.activeInHierarchy && mainPlayer.collider.enabled)
			{
				Physics.IgnoreCollision(val.collider, mainPlayer.collider, flag || flag2);
			}
			if (flag)
			{
				val.GetComponent<SkinManager>().DisableAllSkins();
				val.GetComponent<Nickname>().Hide();
			}
			else
			{
				val.GetComponent<SkinManager>().SetSkin(playerNode.Skin);
				val.GetComponent<Nickname>().Show();
			}
		}
	}

	[RPC]
	public virtual void AddToTeam(int teamIndex, string playerName, object[] playerData, PhotonMessageInfo info = null)
	{
		_Teams[teamIndex][playerName] = new Player(playerName, playerData);
		if (info == null)
		{
			photonView.RPC("AddToTeam", PhotonTargets.Others, teamIndex, playerName, playerData);
		}
		SetupPlayer(teamIndex, playerName);
		SortPlayers(teamIndex);
		RefreshTabMenu();
	}

	[RPC]
	public virtual void RemoveFromTeam(int teamIndex, string playerName, PhotonMessageInfo info = null)
	{
		_Teams[teamIndex][playerName] = null;
		if (info == null)
		{
			photonView.RPC("RemoveFromTeam", PhotonTargets.Others, teamIndex, playerName);
		}
		SortPlayers(teamIndex);
		RefreshTabMenu();
	}

	[RPC]
	public virtual void UpdatePlayerData(int teamIndex, string playerName, object[] playerData)
	{
		if (_Teams[teamIndex][playerName] != null)
		{
			_Teams[teamIndex][playerName].Data = playerData;
			SortPlayers(teamIndex);
			RefreshTabMenu();
		}
	}

	public virtual int GetDefaultTeam()
	{
		return 0;
	}

	public virtual int GetSkin()
	{
		return ProfileINI.skin;
	}

	public int GetPlayerTeam(string playerName = null)
	{
		if (playerName == null)
		{
			playerName = PhotonNetwork.playerName;
		}
		for (int i = 0; i < 3; i++)
		{
			if (_Teams[i][playerName] != null)
			{
				return i;
			}
		}
		return -1;
	}

	public virtual bool IsTeammate(string playerName)
	{
		if ((bool)_GameEnded)
		{
			return true;
		}
		for (int i = 1; i < 3; i++)
		{
			if (_Teams[i][PhotonNetwork.playerName] != null)
			{
				return _Teams[i][playerName] != null;
			}
		}
		return false;
	}

	public virtual bool IsOpposite(string playerName)
	{
		if ((bool)_GameEnded)
		{
			return false;
		}
		for (int i = 1; i < 3; i++)
		{
			if (_Teams[i][playerName] != null)
			{
				return _Teams[i][PhotonNetwork.playerName] == null;
			}
		}
		return false;
	}

	public bool IsObserving(string playerName)
	{
		return _Teams[0][playerName] != null;
	}

	public virtual void OnDead(string killer, string victim, int score)
	{
		photonView.RPC("OnPlayerDead", PhotonTargets.MasterClient, killer, victim, score);
	}

	[RPC]
	protected virtual void OnPlayerDead(string killer, string victim, int score)
	{
		if ((bool)_GameEnded)
		{
			return;
		}
		if (killer != victim)
		{
			int playerTeam = GetPlayerTeam(killer);
			if (playerTeam != 0)
			{
				ModifyPlayerData(playerTeam, killer, score, 1, 0);
				AddTeamScore(playerTeam, score);
			}
		}
		int playerTeam2 = GetPlayerTeam(victim);
		if (playerTeam2 != 0)
		{
			ModifyPlayerData(playerTeam2, victim, 0, 0, 1);
		}
	}

	[RPC]
	protected virtual void EndGame(int winner)
	{
		_GameEnded = true;
		_WinnerTeam = winner;
		_GameTime = 20f;
		int num = -1;
		int num2 = 0;
		_TeamsResult = new Team[_Teams.Length];
		for (int i = 0; i < _Teams.Length; i++)
		{
			_TeamsResult[i] = _Teams[i].GetCopy();
			_Teams[i].Score = 0;
			foreach (Player player in _Teams[i].Players)
			{
				if (player.Name == PhotonNetwork.playerName)
				{
					num = i;
					num2 = player.Score;
				}
				player.Reset();
			}
		}
		StartPlay(0);
		if (winner == num)
		{
			SoundManager.Instance.Play(SoundManager.Sound.BattleWin);
			MainMenu.Instance.SwitchMenu(Menu.BattleResult, true, num2);
		}
		else if (num == 1 || num == 2)
		{
			SoundManager.Instance.Play(SoundManager.Sound.BattleLose);
			MainMenu.Instance.SwitchMenu(Menu.BattleResult, false, num2);
		}
		else
		{
			MainMenu.Instance.SwitchMenu(Menu.TeamBattle);
		}
	}

	[RPC]
	protected virtual void RestartGame()
	{
		_GameEnded = false;
		_GameTime = RoundDuration;
		_TeamsResult = null;
		WorldGameObject.Instance.RestartLevel();
		RefreshTabMenu(open: true);
	}

	public void AddTeamScore(int teamIndex, int score)
	{
		if (PhotonNetwork.playerList.Length >= 4)
		{
			photonView.RPC("SetTeamData", PhotonTargets.All, teamIndex, (int)_Teams[teamIndex].Score + score);
		}
	}

	public void ModifyPlayerData(int teamIndex, string playerName, int score, int kills, int deads)
	{
		if (PhotonNetwork.playerList.Length < 4)
		{
			return;
		}
		Player player = _Teams[teamIndex][playerName];
		if (player != null)
		{
			if (score != 0)
			{
				player.Score = (int)player.Score + score;
			}
			if (kills != 0)
			{
				player.Kills = (int)player.Kills + kills;
			}
			if (deads != 0)
			{
				player.Deads = (int)player.Deads + deads;
			}
			photonView.RPC("UpdatePlayerData", PhotonTargets.All, teamIndex, (string)player.Name, player.Data);
		}
	}

	[RPC]
	protected virtual void SetTeamData(int teamIndex, int teamScore)
	{
		_Teams[teamIndex].Score = teamScore;
		KGUI.SetControlText("hud.battle.txt_red_score", string.Empty + Instance.RedTeamScore);
		KGUI.SetControlText("hud.battle.txt_blue_score", string.Empty + Instance.BlueTeamScore);
	}

	public virtual Vector3 GenerateSpawnPoint()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3.zero;
		for (int i = 0; i < 3; i++)
		{
			if (_Teams[i][PhotonNetwork.playerName] != null)
			{
				val = ((i != 0) ? GetSpawnPoint(i) : GameType.FindRandomPlace(null, 10f));
				break;
			}
		}
		if (val == Vector3.zero)
		{
			val = GetSpawnPoint(0);
		}
		int num = Mathf.Clamp((int)val.x + Random.Range(0, 7) - 3, 0, WorldData.Instance.WidthInBlocks - 1);
		int num2 = Mathf.Clamp((int)val.z + Random.Range(0, 7) - 3, 0, WorldData.Instance.HeightInBlocks - 1);
		int num3 = (int)val.y;
		for (int j = 0; j < 4; j++)
		{
			if (WorldData.Instance.GetBlockType(num, num2, num3 + j) == BlockType.Air)
			{
				val = new Vector3((float)num + 0.5f, (float)(num3 + j) + 0.5f, (float)num2 + 0.5f);
				break;
			}
			if (j > 0 && WorldData.Instance.GetBlockType(num, num2, num3 - j) == BlockType.Air)
			{
				val = new Vector3((float)num + 0.5f, (float)(num3 - j) + 0.5f, (float)num2 + 0.5f);
				break;
			}
		}
		return val;
	}

	protected Vector3 GetSpawnPoint(int teamIndex)
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		SpawnArrow[] array = Object.FindObjectsOfType(typeof(SpawnArrow)) as SpawnArrow[];
		List<SpawnArrow> list = new List<SpawnArrow>();
		SpawnArrow[] array2 = array;
		foreach (SpawnArrow spawnArrow in array2)
		{
			if (spawnArrow.TeamIndex == teamIndex)
			{
				list.Add(spawnArrow);
			}
		}
		if (list.Count == 0)
		{
			return GameType.FindRandomPlace(null, 10f);
		}
		return ((Component)list[Random.Range(0, list.Count)]).transform.position;
	}

	public static Color GetTeamColor(int teamIndex)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		return teamIndex switch
		{
			0 => Color.green, 
			1 => Color.red, 
			2 => Color.blue, 
			3 => Color.white, 
			_ => Color.gray, 
		};
	}

	protected void SortPlayers(int teamIndex)
	{
		_Teams[teamIndex].Players.Sort((Player a, Player b) => (int)b.Score - (int)a.Score);
	}

	public virtual void RefreshTabMenu(bool open = false)
	{
		if (open && MainMenu.Instance.CurMenu != Menu.TeamBattle)
		{
			MainMenu.Instance.SwitchMenu(Menu.TeamBattle);
			return;
		}
		Team[] array = ((_TeamsResult == null) ? _Teams : _TeamsResult);
		_TeamsTabMenu = new Team[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			_TeamsTabMenu[i] = array[i].GetCopy();
		}
		RefreshTeamMenu(_TeamsTabMenu[1], "grid_red_team");
		RefreshTeamMenu(_TeamsTabMenu[2], "grid_blue_team");
		int playerTeam = GetPlayerTeam();
		KGUI.EnableNodes("team_battle.button_observe", playerTeam != 0 && !_GameEnded);
		KGUI.EnableNodes("team_battle.button_red_team", playerTeam != 1 && !_GameEnded);
		KGUI.EnableNodes("team_battle.button_blue_team", playerTeam != 2 && !_GameEnded);
	}

	private void RefreshTeamMenu(Team team, string gridName)
	{
		KGUI.ResizeGrid("team_battle." + gridName, team.Players.Count, (GameObject slot, int index) =>
		{
			Player player = team.Players[index];
			((Component)slot.transform.Find("txt_nickname")).GetComponent<UILabel>().text = string.Empty + player.Name;
			((Component)slot.transform.Find("txt_score")).GetComponent<UILabel>().text = string.Empty + (int)player.Score;
			KGUI.SetControlSprite(slot.transform.Find("level_icon"), "level_" + player.Level, 0f);
		});
	}

	public void CheckPlayerCheating(int teamIndex, int tabMenuIndex)
	{
		Player player = _TeamsTabMenu[teamIndex].Players[tabMenuIndex];
		WorldGameObject.Instance.StartCheckCheating(player.Name);
	}

	public int GetPlayerLevel(int teamIndex, int tabMenuIndex)
	{
		return _TeamsTabMenu[teamIndex].Players[tabMenuIndex].Level;
	}
}
