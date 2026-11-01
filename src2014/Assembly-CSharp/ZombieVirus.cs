using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieVirus : TeamBattle
{
	private enum GameState
	{
		None,
		WaitPlayers,
		WaitInfection,
		Infectioned,
		EndGame
	}

	public const int ZombiesTeamIndex = 1;

	public const int SurvivorsTeamIndex = 2;

	public const float InfectionTime = 30f;

	public const float ZombiesPercent = 30f;

	public const int MinPlayersForStart = 3;

	public const float ZombieSoundDuration = 10f;

	private GameState _State;

	private float _NextZombieSound;

	protected override void ServerStartGame()
	{
		base.ServerStartGame();
		_State = GameState.None;
		((MonoBehaviour)this).StartCoroutine(GameProcess());
	}

	protected override void OnPhotonPlayerConnected(PhotonPlayer connectedPlayer)
	{
		if (PhotonNetwork.isMasterClient)
		{
			photonView.RPC("SetGameState", connectedPlayer, (int)_State);
		}
		base.OnPhotonPlayerConnected(connectedPlayer);
	}

	protected override void OnMasterClientSwitched(PhotonPlayer newMasterClient)
	{
		base.OnMasterClientSwitched(newMasterClient);
		if (newMasterClient.isLocal)
		{
			((MonoBehaviour)this).StartCoroutine(GameProcess());
		}
	}

	private IEnumerator GameProcess()
	{
		if (_State == GameState.None)
		{
			photonView.RPC("SetGameState", PhotonTargets.All, 1);
			photonView.RPC("SetGameTime", PhotonTargets.All, 0f);
		}
		if (_State == GameState.WaitPlayers)
		{
			while (_Teams[2].Players.Count + _Teams[1].Players.Count < 3)
			{
				yield return 0;
			}
			photonView.RPC("SetGameState", PhotonTargets.All, 2);
			photonView.RPC("SetGameTime", PhotonTargets.All, 30f);
		}
		if (_State == GameState.WaitInfection)
		{
			yield return (object)new WaitForSeconds((float)_GameTime);
			photonView.RPC("SetGameState", PhotonTargets.All, 3);
			photonView.RPC("SetGameTime", PhotonTargets.All, TeamBattle.RoundDuration);
			WorldGameObject.Instance.PlaySoundForAll(SoundManager.Sound.ZombieVirusInfection);
			List<Player> allPlayers = new List<Player>();
			allPlayers.AddRange(_Teams[2].Players);
			allPlayers.AddRange(_Teams[1].Players);
			Utils.Shuffle(allPlayers);
			int zombieCount = Mathf.CeilToInt((float)allPlayers.Count * 30f / 100f);
			for (int i = 0; i < allPlayers.Count; i++)
			{
				PlayerNode playerNode = WorldGameObject.Instance.FindPlayerByName(allPlayers[i].Name);
				PhotonPlayer photonPlayer = playerNode.NetPlayer;
				int teamIndex = ((i < zombieCount) ? 1 : 2);
				photonView.RPC("StartPlay", photonPlayer, teamIndex);
			}
		}
		if (_State == GameState.Infectioned)
		{
			while (_Teams[2].Players.Count > 0)
			{
				yield return 0;
			}
			photonView.RPC("SetGameState", PhotonTargets.All, 4);
			photonView.RPC("SetGameTime", PhotonTargets.All, 20f);
		}
		if (_State == GameState.EndGame)
		{
			yield return (object)new WaitForSeconds((float)_GameTime);
			photonView.RPC("SetGameState", PhotonTargets.All, 0);
			photonView.RPC("SetGameTime", PhotonTargets.All, 0f);
			((MonoBehaviour)this).StartCoroutine(GameProcess());
			List<Player> allPlayers2 = new List<Player>();
			allPlayers2.AddRange(_Teams[2].Players);
			allPlayers2.AddRange(_Teams[1].Players);
			for (int j = 0; j < allPlayers2.Count; j++)
			{
				PlayerNode playerNode2 = WorldGameObject.Instance.FindPlayerByName(allPlayers2[j].Name);
				PhotonPlayer photonPlayer2 = playerNode2.NetPlayer;
				photonView.RPC("StartPlay", photonPlayer2, 2);
			}
		}
	}

	[RPC]
	private void SetGameState(int state)
	{
		_State = (GameState)state;
		if (_State == GameState.WaitPlayers)
		{
			SetTip("HUD_FEW_PLAYERS_TIP");
		}
		else if (_State == GameState.WaitInfection)
		{
			SetTip("HUD_UNTIL_INFECTION_TIP");
		}
		else if (_State == GameState.EndGame)
		{
			SetTip("HUD_GAME_ENDED");
		}
		else
		{
			SetTip(string.Empty);
		}
	}

	private void SetTip(string text)
	{
		KGUI.SetControlText("hud.battle.txt_generic_tip", (text.Length <= 0) ? string.Empty : Localize.GetText(text));
	}

	[RPC]
	public override void StartPlay(int teamIndex)
	{
		bool flag = GetPlayerTeam() == -1;
		base.StartPlay(teamIndex);
		if (!WorldGameObject.Instance.IsWorldGenerated)
		{
			return;
		}
		if (teamIndex == 1)
		{
			((MonoBehaviour)this).StartCoroutine(KillRespawnProcess());
			WorldGameObject.Instance.MainPlayer.GetComponent<PlayerInput>().SetSpeedModifier(1.3f);
			WorldGameObject.Instance.MainPlayer.GetComponent<PlayerMotor>().JumpModifier = 2f;
			PlayerNetwork component = WorldGameObject.Instance.MainPlayer.GetComponent<PlayerNetwork>();
			component.SelectWeapon(0);
			for (int i = 0; i < component.MainWeapon.Length; i++)
			{
				component.MainWeapon[i] = null;
			}
			MainMenu.Instance.RefreshBattleWeapon();
			((MonoBehaviour)this).StartCoroutine(ZombieTipProcess());
		}
		else
		{
			WorldGameObject.Instance.MainPlayer.GetComponent<PlayerInput>().SetSpeedModifier(1f);
			WorldGameObject.Instance.MainPlayer.GetComponent<PlayerMotor>().JumpModifier = 1f;
			if (!flag && teamIndex == 2)
			{
				MainMenu.Instance.SwitchMenu(Menu.SelectWeapon, false);
			}
		}
	}

	public override Vector3 GenerateSpawnPoint()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		return (!((Object)(object)WorldGameObject.Instance.MainPlayer != (Object)null)) ? GameType.FindRandomPlace(null, 10f) : WorldGameObject.Instance.MainPlayer.transform.position;
	}

	private IEnumerator KillRespawnProcess()
	{
		if (!WorldGameObject.Instance.MainPlayerDead)
		{
			WorldGameObject.Instance.KillPlayer();
		}
		yield return (object)new WaitForSeconds(3f);
		if (WorldGameObject.Instance.MainPlayerDead)
		{
			WorldGameObject.Instance.Respawn();
		}
	}

	private IEnumerator ZombieTipProcess()
	{
		SetTip("HUD_YOU_ARE_ZOMBIE_TIP");
		yield return (object)new WaitForSeconds(10f);
		SetTip(string.Empty);
	}

	[RPC]
	protected override void OnPlayerDead(string killer, string victim, int score)
	{
		base.OnPlayerDead(killer, victim, score);
		if (GetPlayerTeam(victim) == 2)
		{
			PlayerNode playerNode = WorldGameObject.Instance.FindPlayerByName(victim);
			if (playerNode != null)
			{
				photonView.RPC("StartPlay", playerNode.NetPlayer, 1);
			}
		}
	}

	public override int GetDefaultTeam()
	{
		if (_State == GameState.Infectioned)
		{
			return 1;
		}
		return 2;
	}

	public override int GetSkin()
	{
		if (GetPlayerTeam() == 1)
		{
			return 6;
		}
		return base.GetSkin();
	}

	public override void RefreshTabMenu(bool open)
	{
		base.RefreshTabMenu(open);
		KGUI.EnableNodes("team_battle.button_red_team", _State == GameState.Infectioned && GetPlayerTeam() != 1);
		KGUI.EnableNodes("team_battle.button_blue_team", enable: false);
	}

	private void RefreshScore()
	{
		KGUI.SetControlText("hud.battle.txt_zombies_count", string.Empty + _Teams[1].Players.Count);
		KGUI.SetControlText("hud.battle.txt_survivors_count", string.Empty + _Teams[2].Players.Count);
	}

	protected override void Update()
	{
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		base.Update();
		if (GetPlayerTeam() == 1)
		{
			if (Time.time >= _NextZombieSound)
			{
				_NextZombieSound = Time.time + 10f + Random.Range(0f, 10f);
				SoundManager.Instance.PlayAtPoint(SoundManager.Sound.ZombieActivity, WorldGameObject.Instance.MainPlayer.transform.position);
			}
			Color color = WorldGameObject.Instance.ZombieEffect.GetComponent<GUITexture>().color;
			color.a = 0.1f + Mathf.PingPong(Time.time / 2f, 0.4f);
			WorldGameObject.Instance.ZombieEffect.GetComponent<GUITexture>().color = color;
		}
		else
		{
			Color color2 = WorldGameObject.Instance.ZombieEffect.GetComponent<GUITexture>().color;
			if (color2.a != 0f)
			{
				color2.a = 0f;
				WorldGameObject.Instance.ZombieEffect.GetComponent<GUITexture>().color = color2;
			}
		}
	}

	public override bool IsTeammate(string playerName)
	{
		if (_State != GameState.Infectioned)
		{
			return true;
		}
		return base.IsTeammate(playerName);
	}

	public override bool IsOpposite(string playerName)
	{
		if (_State != GameState.Infectioned)
		{
			return false;
		}
		return base.IsOpposite(playerName);
	}

	public override void OnDead(string killer, string victim, int score)
	{
		base.OnDead(killer, victim, score);
	}

	[RPC]
	public override void AddToTeam(int teamIndex, string playerName, object[] playerData, PhotonMessageInfo info = null)
	{
		base.AddToTeam(teamIndex, playerName, playerData, info);
		RefreshScore();
	}

	[RPC]
	public override void RemoveFromTeam(int teamIndex, string playerName, PhotonMessageInfo info = null)
	{
		base.RemoveFromTeam(teamIndex, playerName, info);
		RefreshScore();
	}

	[RPC]
	protected override void EndGame(int winner)
	{
		base.EndGame(winner);
	}

	[RPC]
	public override void SetGameTime(float time)
	{
		base.SetGameTime(time);
	}

	[RPC]
	protected override void SetTeamData(int teamIndex, int teamScore)
	{
		base.SetTeamData(teamIndex, teamScore);
		RefreshScore();
	}

	[RPC]
	public override void UpdatePlayerData(int teamIndex, string playerName, object[] playerData)
	{
		base.UpdatePlayerData(teamIndex, playerName, playerData);
	}
}
