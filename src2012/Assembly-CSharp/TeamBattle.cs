using System;
using System.Collections;
using System.Collections.Generic;
using Photon;
using UnityEngine;

public class TeamBattle : MonoBehaviour
{
	public class Player
	{
		public string Name;

		public int Kills;

		public int Deads;

		public object[] Data
		{
			get
			{
				return new object[2] { Kills, Deads };
			}
			set
			{
				Kills = (int)value[0];
				Deads = (int)value[1];
			}
		}

		public Player(string name, object[] data)
		{
			Name = name;
			if (data != null)
			{
				Data = data;
			}
		}
	}

	public class Team
	{
		public List<Player> Players = new List<Player>();

		public int Score;

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
	}

	public const int TeamsCount = 3;

	public const int RestartDuration = 20;

	public float AddedTime;

	public List<PlayerKilledPlayer> PlayerKilledPlayer = new List<PlayerKilledPlayer>();

	internal bool MenuEnabled;

	internal bool EnableVoiceChat = true;

	public static TeamBattle Instance;

	public Team[] _Teams = new Team[3];

	public bool _TeamSelected;

	protected bool _GameEnded;

	protected float _GameEndTime;

	protected int _WinnerTeam;

	protected float _GameTime = 900f;

	private Vector2 _Scroll;

	public int BlueTeamScore => _Teams[2].Score;

	public int RedTeamScore => _Teams[1].Score;

	protected void Awake()
	{
		Instance = this;
		for (int i = 0; i < _Teams.Length; i++)
		{
			_Teams[i] = new Team();
		}
	}

	public void Update()
	{
		if (bs.DebugKey((KeyCode)114))
		{
			photonView.RPC("EndGame", PhotonTargets.All, (BlueTeamScore != RedTeamScore) ? ((BlueTeamScore > RedTeamScore) ? 1 : 2) : 0);
		}
		if (Time.time - AddedTime > 3f && PlayerKilledPlayer.Count > 0)
		{
			AddedTime = Time.time;
			PlayerKilledPlayer.RemoveAt(0);
		}
		_GameTime -= Time.deltaTime;
		TimeSpan timeSpan = TimeSpan.FromSeconds(Math.Max(0f, _GameTime));
		bs._Hud.Time.text = timeSpan.Minutes + ":" + timeSpan.Seconds;
		if (PhotonNetwork.player.isMasterClient && _GameTime < 0f && !_GameEnded)
		{
			Debug.Log((object)"End Game");
			photonView.RPC("EndGame", PhotonTargets.All, (BlueTeamScore != RedTeamScore) ? ((BlueTeamScore > RedTeamScore) ? 1 : 2) : 0);
		}
	}

	protected virtual IEnumerator Start()
	{
		if (!App.instance.gameIni.isServer)
		{
			yield break;
		}
		while (!WorldGameObject.INSTANCE.IsWorldGenerated)
		{
			yield return 0;
		}
		bool isRedSpawn = false;
		bool isBlueSpawn = false;
		List<Vector3> arrowsPos = new List<Vector3>();
		Object[] array = Object.FindObjectsOfType(typeof(SpawnArrow));
		for (int i = 0; i < array.Length; i++)
		{
			SpawnArrow spawnArrow = (SpawnArrow)(object)array[i];
			if (spawnArrow.Type == EntityType.TEAM_SPAWN_ARROW_RED)
			{
				isRedSpawn = true;
				arrowsPos.Add(((Component)spawnArrow).transform.position);
			}
			else if (spawnArrow.Type == EntityType.TEAM_SPAWN_ARROW_BLUE)
			{
				isBlueSpawn = true;
				arrowsPos.Add(((Component)spawnArrow).transform.position);
			}
		}
		int spawnsCount = ((isRedSpawn == isBlueSpawn) ? 2 : arrowsPos.Count);
		if (!isRedSpawn)
		{
			for (int j = 0; j < spawnsCount; j++)
			{
				Vector3 pos = GameTypeUtils.FindRandomPlace(arrowsPos.ToArray(), 40f);
				arrowsPos.Add(pos);
				WorldGameObject.INSTANCE.photonView.RPC("AddEntityNetwork", PhotonTargets.MasterClient, 82, pos, Quaternion.Euler(270f, 0f, 0f), string.Empty, null);
			}
		}
		if (!isBlueSpawn)
		{
			for (int k = 0; k < spawnsCount; k++)
			{
				Vector3 pos2 = GameTypeUtils.FindRandomPlace(arrowsPos.ToArray(), 40f);
				arrowsPos.Add(pos2);
				WorldGameObject.INSTANCE.photonView.RPC("AddEntityNetwork", PhotonTargets.MasterClient, 83, pos2, Quaternion.Euler(270f, 0f, 0f), string.Empty, null);
			}
		}
		StartPlay(0);
	}

	protected void OnPhotonPlayerConnected(PhotonPlayer connectedPlayer)
	{
		if (!PhotonNetwork.isMasterClient)
		{
			return;
		}
		for (int i = 0; i < _Teams.Length; i++)
		{
			photonView.RPC("SetTeamData", connectedPlayer, i, _Teams[i].Score);
			foreach (Player player in _Teams[i].Players)
			{
				photonView.RPC("AddToTeam", connectedPlayer, i, player.Name, player.Data);
			}
		}
		photonView.RPC("StartPlay", connectedPlayer, 0);
		photonView.RPC("SetGameTime", connectedPlayer, _GameTime);
	}

	protected void OnPhotonPlayerDisconnected(PhotonPlayer disconnectedPlayer)
	{
		for (int i = 0; i < 3; i++)
		{
			if (_Teams[i][disconnectedPlayer.name] != null)
			{
				_Teams[i][disconnectedPlayer.name] = null;
			}
		}
	}

	[RPC]
	public virtual void SetGameTime(float time)
	{
		_GameTime = time;
	}

	[RPC]
	protected virtual void StartPlay(int teamIndex)
	{
		((MonoBehaviour)this).StartCoroutine(StartPlayProcess(teamIndex));
	}

	protected IEnumerator StartPlayProcess(int teamIndex)
	{
		while (!WorldGameObject.INSTANCE.IsWorldGenerated)
		{
			yield return 0;
		}
		yield return 0;
		for (int i = 0; i < 3; i++)
		{
			if (_Teams[i][PhotonNetwork.playerName] != null)
			{
				RemoveFromTeam(i, PhotonNetwork.playerName);
			}
		}
		AddToTeam(teamIndex, PhotonNetwork.playerName, null);
		WorldGameObject.INSTANCE.MainPlayer.transform.position = GenerateSpawnPoint();
		if (!_TeamSelected)
		{
			TabMenu.Instance.showMenu = true;
			WorldGameObject.INSTANCE.DisableMouseLook();
			Screen.lockCursor = false;
		}
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

	protected void SetupPlayer(int teamIndex, string playerName)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = null;
		PlayerNode playerNode = null;
		if (playerName == PhotonNetwork.playerName)
		{
			val = WorldGameObject.INSTANCE.MainPlayer;
		}
		else
		{
			playerNode = WorldGameObject.INSTANCE.FindPlayerByName(playerName);
			if (playerNode != null)
			{
				val = playerNode.avatar;
			}
		}
		if ((Object)(object)WorldGameObject.INSTANCE.MainPlayer == (Object)null || (Object)(object)val == (Object)null)
		{
			return;
		}
		val.GetComponent<SetLabelOnMessage>().SetColor(GetTeamColor(teamIndex));
		val.GetComponent<SetLabelOnMessage>().SetRaycast(enable: true);
		bool flag = teamIndex == 0;
		if ((Object)(object)WorldGameObject.INSTANCE.MainPlayer == (Object)(object)val)
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
				val.GetComponent<SkinManager>().SetHand(ProfileINI.skin);
			}
			val.GetComponent<CameraController>().DisableCameraSwitching = flag;
			MainMenu.INSTANCE.FlyBonus();
		}
		else
		{
			bool flag2 = IsObserving();
			Physics.IgnoreCollision(val.collider, WorldGameObject.INSTANCE.MainPlayer.collider, flag || flag2);
			if (flag)
			{
				val.GetComponent<SkinManager>().DisableAllSkins();
				val.GetComponent<SetLabelOnMessage>().Hide();
			}
			else
			{
				val.GetComponent<SkinManager>().SetSkin(playerNode.skin);
				val.GetComponent<SetLabelOnMessage>().Show();
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
	}

	[RPC]
	public virtual void UpdatePlayerData(int teamIndex, string playerName, object[] playerData)
	{
		_Teams[teamIndex][playerName].Data = playerData;
		SortPlayers(teamIndex);
	}

	public int GetPlayerTeam(string playerName)
	{
		for (int i = 0; i < 3; i++)
		{
			if (_Teams[i][playerName] != null)
			{
				return i;
			}
		}
		return 0;
	}

	public virtual bool IsTeammate(string playerName)
	{
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
		for (int i = 1; i < 3; i++)
		{
			if (_Teams[i][playerName] != null)
			{
				return _Teams[i][PhotonNetwork.playerName] == null;
			}
		}
		return false;
	}

	public bool IsObserving(string n)
	{
		return _Teams[0][n] != null;
	}

	public bool IsObserving()
	{
		return _Teams[0][PhotonNetwork.playerName] != null;
	}

	public void OnKill(string playerName)
	{
	}

	public void OnDead(string playerName)
	{
	}

	[RPC]
	public virtual void OnPlayerKill(string playerName)
	{
		if (_GameEnded)
		{
			return;
		}
		for (int i = 0; i < 3; i++)
		{
			Player player = _Teams[i][playerName];
			if (player != null)
			{
				player.Kills++;
				photonView.RPC("UpdatePlayerData", PhotonTargets.All, i, player.Name, player.Data);
				break;
			}
		}
	}

	public void OnPlayerDead2(string killer, string killed, int score)
	{
		if (_GameEnded)
		{
			return;
		}
		for (int i = 0; i < 3; i++)
		{
			Player player = _Teams[i][killer];
			if (player != null && killer != killed)
			{
				player.Kills += score;
				_Teams[i].Score += score;
				photonView.RPC("SetTeamData", PhotonTargets.Others, i, _Teams[i].Score);
				photonView.RPC("UpdatePlayerData", PhotonTargets.All, i, player.Name, player.Data);
				break;
			}
		}
		for (int j = 0; j < 3; j++)
		{
			Player player2 = _Teams[j][killed];
			if (player2 != null)
			{
				player2.Deads++;
				photonView.RPC("UpdatePlayerData", PhotonTargets.All, j, player2.Name, player2.Data);
				break;
			}
		}
	}

	[RPC]
	public virtual void OnPlayerDead(string playerName)
	{
		if (_GameEnded)
		{
			return;
		}
		for (int i = 0; i < 3; i++)
		{
			Player player = _Teams[i][playerName];
			if (player != null)
			{
				player.Deads++;
				photonView.RPC("UpdatePlayerData", PhotonTargets.All, i, player.Name, player.Data);
				_Teams[i].Score++;
				photonView.RPC("SetTeamData", PhotonTargets.Others, i, _Teams[i].Score);
				break;
			}
		}
	}

	[RPC]
	protected virtual void EndGame(int winner)
	{
		_GameEnded = true;
		_WinnerTeam = winner;
		_GameEndTime = Time.time;
		WorldGameObject.INSTANCE.DisableMouseLook();
		Screen.lockCursor = false;
	}

	[RPC]
	protected virtual void RestartGame()
	{
		_GameTime = 600f;
		WorldGameObject.INSTANCE.RestartLevel();
		_GameEnded = false;
		for (int i = 0; i < 3; i++)
		{
			_Teams[i].Score = 0;
			foreach (Player player in _Teams[i].Players)
			{
				player.Data = new object[2] { 0, 0 };
			}
		}
		WorldGameObject.INSTANCE.Respawn();
	}

	[RPC]
	protected virtual void SetTeamData(int teamIndex, int teamScore)
	{
		_Teams[teamIndex].Score = teamScore;
	}

	public virtual void OnGUI()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_04be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0508: Unknown result type (might be due to invalid IL or missing references)
		//IL_0541: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0623: Unknown result type (might be due to invalid IL or missing references)
		//IL_0671: Unknown result type (might be due to invalid IL or missing references)
		//IL_069e: Unknown result type (might be due to invalid IL or missing references)
		GUILayout.BeginArea(Rect.MinMaxRect(0.8f * (float)Screen.width, 0.13f * (float)Screen.height, (float)Screen.width, (float)Screen.height));
		TextAnchor alignment = GUI.skin.label.alignment;
		bool wordWrap = GUI.skin.label.wordWrap;
		foreach (PlayerKilledPlayer item in PlayerKilledPlayer)
		{
			GUI.skin.label.wordWrap = false;
			GUI.skin.label.alignment = (TextAnchor)4;
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label(item.KilledBy, new GUILayoutOption[2]
			{
				GUILayout.ExpandWidth(false),
				GUILayout.Height(64f)
			});
			Gun gun = bs._WorldGameObject.Player_Prefab.GetComponent<PlayerNetwork>().Guns[item.GunID];
			GUILayout.Label((Texture)(object)gun.weaponIcon, new GUILayoutOption[2]
			{
				GUILayout.Width(64f),
				GUILayout.Height(64f)
			});
			GUILayout.Label(item.Died, new GUILayoutOption[2]
			{
				GUILayout.ExpandWidth(false),
				GUILayout.Height(64f)
			});
			GUILayout.EndHorizontal();
		}
		GUI.skin.label.alignment = alignment;
		GUI.skin.label.wordWrap = wordWrap;
		GUILayout.EndArea();
		MenuEnabled = false;
		if (!WorldGameObject.INSTANCE.IsWorldGenerated || (!TabMenu.Instance.showMenu && !_GameEnded && _TeamSelected))
		{
			return;
		}
		MenuEnabled = true;
		if (_GameEnded)
		{
			if ((Object)(object)TabMenu.Instance.boxSkin != (Object)null)
			{
				GUI.skin = TabMenu.Instance.boxSkin;
			}
			GUI.Box(new Rect((float)(Screen.width / 2 - 220), (float)(Screen.height / 2 - 65), 440f, 200f), string.Empty);
			if ((Object)(object)TabMenu.Instance.guiSkin != (Object)null)
			{
				GUI.skin = TabMenu.Instance.guiSkin;
			}
			GUILayout.BeginArea(new Rect((float)(Screen.width / 2 - 200), (float)(Screen.height / 2 - 65), 400f, 200f));
			GUILayout.Space(35f);
			int num = Mathf.FloorToInt(Mathf.Clamp(20f - (Time.time - _GameEndTime), 0f, 20f));
			GUILayout.Label(((_WinnerTeam != 0) ? ("Победили " + ((_WinnerTeam != 2) ? "синие" : "красные")) : "Нечья") + "! Время до рестарта " + num + ".", new GUILayoutOption[0]);
			if (num == 0)
			{
				if (PhotonNetwork.isMasterClient)
				{
					photonView.RPC("RestartGame", PhotonTargets.All);
				}
				_GameEnded = false;
			}
			if (GUILayout.Button("Выйти", new GUILayoutOption[0]))
			{
				WorldGameObject.INSTANCE.ExitGame();
			}
			GUILayout.EndArea();
			return;
		}
		if ((Object)(object)TabMenu.Instance.boxSkin != (Object)null)
		{
			GUI.skin = TabMenu.Instance.boxSkin;
		}
		if (!_TeamSelected || _GameEnded)
		{
			GUI.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), (Texture)(object)bs._Igor.black);
		}
		GUI.Box(new Rect((float)(Screen.width / 2 - 345), (float)(Screen.height / 2 - 225), 690f, 450f), "Командный бой");
		if ((Object)(object)TabMenu.Instance.guiSkin != (Object)null)
		{
			GUI.skin = TabMenu.Instance.guiSkin;
		}
		GUI.Label(new Rect((float)(Screen.width / 2 - 300), (float)(Screen.height / 2 - 200), 200f, 50f), "Очки красных: " + _Teams[1].Score);
		DrawTeam(new Rect((float)(Screen.width / 2 - 320), (float)(Screen.height / 2 - 150), 300f, 300f), _Teams[1]);
		GUI.Label(new Rect((float)(Screen.width / 2 + 100), (float)(Screen.height / 2 - 200), 200f, 50f), "Очки синих: " + _Teams[2].Score);
		DrawTeam(new Rect((float)(Screen.width / 2 + 20), (float)(Screen.height / 2 - 150), 300f, 300f), _Teams[2]);
		EnableVoiceChat = GUI.Toggle(new Rect((float)(Screen.width / 2 - 75), (float)(Screen.height / 2 + 150 + 50), 350f, 90f), EnableVoiceChat, "Включить голосовой чат");
		if (_GameEnded)
		{
			return;
		}
		if ((_Teams[0][PhotonNetwork.playerName] == null || !_TeamSelected) && GUI.Button(new Rect((float)(Screen.width / 2 - 75), (float)(Screen.height / 2 + 150), 150f, 40f), "Наблюдать"))
		{
			if (!_TeamSelected)
			{
				HideMenu();
			}
			else
			{
				StartPlay(0);
			}
		}
		if (_Teams[1][PhotonNetwork.playerName] == null)
		{
			GUI.color = Color.red;
			if (GUI.Button(new Rect((float)(Screen.width / 2 - 300), (float)(Screen.height / 2 + 150), 200f, 40f), "Играть за красных"))
			{
				_TeamSelected = true;
				TabMenu.Instance.showMenu = false;
				bs._MainMenu.ShowWeaponWindow();
				StartPlay(1);
			}
		}
		if (_Teams[2][PhotonNetwork.playerName] == null)
		{
			GUI.color = Color.blue;
			if (GUI.Button(new Rect((float)(Screen.width / 2 + 100), (float)(Screen.height / 2 + 150), 200f, 40f), "Играть за синих"))
			{
				_TeamSelected = true;
				TabMenu.Instance.showMenu = false;
				bs._MainMenu.ShowWeaponWindow();
				StartPlay(2);
			}
		}
	}

	protected void HideMenu()
	{
		_TeamSelected = true;
		TabMenu.Instance.showMenu = false;
		WorldGameObject.INSTANCE.EnableMouseLook();
		Screen.lockCursor = true;
	}

	protected void DrawTeam(Rect area, Team team)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		GUILayout.BeginArea(area);
		_Scroll = GUILayout.BeginScrollView(_Scroll, new GUILayoutOption[0]);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("Игрок", new GUILayoutOption[1] { GUILayout.Width(200f) });
		GUILayout.Label("Очки", new GUILayoutOption[1] { GUILayout.Width(50f) });
		GUILayout.EndHorizontal();
		for (int i = 0; i < team.Players.Count; i++)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label(team.Players[i].Name, new GUILayoutOption[1] { GUILayout.Width(200f) });
			GUILayout.Label(team.Players[i].Kills + string.Empty, new GUILayoutOption[1] { GUILayout.Width(50f) });
			GUILayout.EndHorizontal();
		}
		GUILayout.EndScrollView();
		GUILayout.EndArea();
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
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3.zero;
		for (int i = 0; i < 3; i++)
		{
			if (_Teams[i][PhotonNetwork.playerName] != null)
			{
				val = ((i != 0) ? GetSpawnPoint(i) : GameTypeUtils.FindRandomPlace(null, 10f));
				break;
			}
		}
		if (val == Vector3.zero)
		{
			val = GetSpawnPoint(0);
		}
		int num = Mathf.Clamp((int)val.x + Random.Range(0, 7) - 3, 0, WorldData.Instance.WidthInBlocks - 1);
		int num2 = Mathf.Clamp((int)val.z + Random.Range(0, 7) - 3, 0, WorldData.Instance.HeightInBlocks - 1);
		int num3 = WorldData.Instance.DepthInBlocks;
		while (num3 >= 0 && num3 != 0 && WorldData.Instance.GetBlockType(num, num2, num3 - 1) == BlockType.Air)
		{
			num3--;
		}
		return new Vector3((float)num + 0.5f, (float)num3 + 0.5f, (float)num2 + 0.5f);
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
			return GameTypeUtils.FindRandomPlace(null, 10f);
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
		_Teams[teamIndex].Players.Sort((Player a, Player b) => b.Kills - a.Kills);
	}
}
