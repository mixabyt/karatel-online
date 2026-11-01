using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Survival : GameType
{
	private class Player
	{
		public string Name;

		public int Score;

		public object[] Data
		{
			get
			{
				return new object[1] { Score };
			}
			set
			{
				Score = (int)value[0];
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

	public const float GameDuration = 300f;

	public const float RestartDuration = 20f;

	public const int MaxEnemies = 50;

	public const float EnemySpawnTime = 3f;

	public const float EasyEnemySpeed = 2f;

	public const float HeavyEnemySpeed = 4f;

	public const int EasyEnemyLife = 5;

	public const int HeavyEnemyLife = 20;

	public const int EasyEnemyDamage = 1;

	public const int HeavyEnemyDamage = 2;

	public const int ScoreForEnemyKill = 10;

	public static Survival Instance;

	private float _LastSpawnTime;

	private int _SpawnIndex;

	private List<Player> _Players = new List<Player>();

	private bool _GameEnded;

	private float _GameTime;

	private void Awake()
	{
		Instance = this;
		_GameTime = Time.time;
	}

	private IEnumerator Start()
	{
		while (!WorldGameObject.Instance.IsWorldGenerated)
		{
			yield return 0;
		}
		foreach (Player player in _Players)
		{
			SetupPlayer(player.Name);
		}
		if (App.Instance.Settings.isServer)
		{
			photonView.RPC("AddPlayer", PhotonTargets.All, PhotonNetwork.playerName, null);
		}
	}

	private void OnPhotonPlayerConnected(PhotonPlayer connectedPlayer)
	{
		if (!PhotonNetwork.isMasterClient)
		{
			return;
		}
		photonView.RPC("SetData", connectedPlayer, _GameEnded, Time.time - _GameTime);
		foreach (Player player in _Players)
		{
			photonView.RPC("AddPlayer", connectedPlayer, player.Name, player.Data);
		}
		photonView.RPC("AddPlayer", PhotonTargets.All, connectedPlayer.name, null);
	}

	[RPC]
	private void SetData(bool gameEnded, float gameDuration)
	{
		_GameEnded = gameEnded;
		_GameTime = Time.time + gameDuration;
	}

	private void OnPhotonPlayerDisconnected(PhotonPlayer disconnectedPlayer)
	{
		int num = _Players.FindIndex((Player p) => p.Name == disconnectedPlayer.name);
		if (num != -1)
		{
			_Players.RemoveAt(num);
		}
	}

	[RPC]
	public void AddPlayer(string playerName, object[] playerData)
	{
		_Players.Add(new Player(playerName, playerData));
		SetupPlayer(playerName);
		SortPlayers();
	}

	[RPC]
	public void UpdatePlayerData(string playerName, object[] playerData)
	{
		Player player = _Players.Find((Player p) => p.Name == playerName);
		if (player != null)
		{
			player.Data = playerData;
		}
		SortPlayers();
	}

	private void SetupPlayer(string playerName)
	{
		if (WorldGameObject.Instance.IsWorldGenerated)
		{
		}
	}

	private void Update()
	{
		if (!WorldGameObject.Instance.IsWorldGenerated || !photonView.isMine)
		{
			return;
		}
		if (_GameEnded)
		{
			if (Time.time - _GameTime >= 20f)
			{
				photonView.RPC("StartGame", PhotonTargets.All, false);
			}
			return;
		}
		if (Time.time - _LastSpawnTime >= 3f)
		{
			_SpawnIndex++;
			_LastSpawnTime = Time.time;
			if (Object.FindObjectsOfType(typeof(NPC)).Length < 50)
			{
				SpawnEnemy(_SpawnIndex % 10 == 0);
			}
		}
		if (Time.time - _GameTime >= 300f)
		{
			photonView.RPC("EndGame", PhotonTargets.All);
		}
	}

	private void SpawnEnemy(bool heavy)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		Quaternion identity = Quaternion.identity;
		SpawnArrow[] array = Object.FindObjectsOfType(typeof(SpawnArrow)) as SpawnArrow[];
		Vector3 position = ((array.Length <= 0) ? GameType.FindRandomPlace(null, 10f) : ((Component)array[Random.Range(0, array.Length)]).transform.position);
		int num = ((!heavy) ? 6 : 7);
		GameObject val = PhotonNetwork.InstantiateSceneObject("npc", position, identity, 0, new object[3]
		{
			EntityType.NPC,
			string.Empty,
			num
		});
		AI component = val.GetComponent<AI>();
		component.Speed = ((!heavy) ? 2f : 4f);
		val.GetComponent<EntityBase>().Life = (byte)((!heavy) ? 5u : 20u);
		component.Damage = ((!heavy) ? 1 : 2);
		component.Aggressive = true;
		component.AggressiveDistance = 100f;
		component.AggressiveBackDistance = 150f;
		component.Patrolling = true;
		component.PatrollingDistance = 5f;
		component.PatrollingIdle = 3f;
		component.Follow = false;
		component.PlayersMonitoring = true;
	}

	public void OnKillEnemy(PhotonPlayer photonPlayer)
	{
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		if (_GameEnded)
		{
			return;
		}
		Player player = _Players.Find((Player p) => p.Name == photonPlayer.name);
		if (player != null)
		{
			player.Score += 10;
			photonView.RPC("UpdatePlayerData", PhotonTargets.All, player.Name, player.Data);
			PlayerNode playerNode = WorldGameObject.Instance.FindPlayer(photonPlayer);
			if (playerNode != null && (Object)(object)playerNode.Avatar != (Object)null)
			{
				WorldGameObject.Instance.AddFlyMessage(playerNode.Avatar.GetComponent<PhotonView>().viewID, "+" + 10, Color.green, forAll: true);
			}
		}
	}

	private void SortPlayers()
	{
		_Players.Sort((Player a, Player b) => b.Score - a.Score);
	}

	[RPC]
	private void EndGame()
	{
		_GameEnded = true;
		_GameTime = Time.time;
		WorldGameObject.Instance.DisableMouseLook();
		Screen.lockCursor = false;
	}

	[RPC]
	private void StartGame()
	{
		WorldGameObject.Instance.RestartLevel();
		_GameEnded = false;
		_GameTime = Time.time;
		foreach (Player player in _Players)
		{
			player.Data = new object[1] { 0 };
		}
	}
}
