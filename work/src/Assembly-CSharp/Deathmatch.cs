using UnityEngine;

public class Deathmatch : TeamBattle
{
	public override void RefreshTabMenu(bool open)
	{
		if (open && MainMenu.Instance.CurMenu != Menu.Deathmatch)
		{
			MainMenu.Instance.SwitchMenu(Menu.Deathmatch);
			return;
		}
		Team[] array = ((_TeamsResult == null) ? _Teams : _TeamsResult);
		_TeamsTabMenu = new Team[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			_TeamsTabMenu[i] = array[i].GetCopy();
		}
		KGUI.ResizeGrid("deathmatch.grid_players", _TeamsTabMenu[1].Players.Count, (GameObject slot, int index) =>
		{
			Player player = _TeamsTabMenu[1].Players[index];
			((Component)slot.transform.Find("txt_nickname")).GetComponent<UILabel>().text = string.Empty + player.Name;
			((Component)slot.transform.Find("txt_score")).GetComponent<UILabel>().text = string.Empty + (int)player.Score;
			KGUI.SetControlSprite(slot.transform.Find("level_icon"), "level_" + player.Level, 0f);
		}, "deathmatch");
	}

	public override bool IsTeammate(string playerName)
	{
		if ((bool)_GameEnded)
		{
			return true;
		}
		return false;
	}

	public override bool IsOpposite(string playerName)
	{
		if ((bool)_GameEnded)
		{
			return false;
		}
		return true;
	}

	public override Vector3 GenerateSpawnPoint()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		WorldData worldData = bs._WorldGameObject.WorldData;
		Vector3 val = new Vector3((float)Random.Range(0, worldData.WidthInBlocks - 1), (float)worldData.DepthInBlocks, (float)Random.Range(0, worldData.HeightInBlocks - 1)) + Vector3.one / 2f;
		RaycastHit val2 = default;
		if (Physics.Raycast(val, Vector3.down, ref val2, 1000f, 1 << LayerMask.NameToLayer("Terrain")))
		{
			return val2.point + Vector3.up;
		}
		return val;
	}

	[RPC]
	public override void AddToTeam(int teamIndex, string playerName, object[] playerData, PhotonMessageInfo info = null)
	{
		base.AddToTeam(teamIndex, playerName, playerData, info);
	}

	[RPC]
	protected override void EndGame(int winner)
	{
		_GameEnded = true;
		_GameTime = 20f;
		int num = -1;
		int num2 = 0;
		_TeamsResult = new Team[_Teams.Length];
		for (int i = 0; i < 3; i++)
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
		if (num == 1 && _TeamsResult[1].Players.Count > 0 && _TeamsResult[1].Players.FindIndex((Player x) => x.Name == PhotonNetwork.playerName) < 3)
		{
			SoundManager.Instance.Play(SoundManager.Sound.BattleWin);
			MainMenu.Instance.SwitchMenu(Menu.BattleResult, true, num2);
		}
		else if (num != 0)
		{
			SoundManager.Instance.Play(SoundManager.Sound.BattleLose);
			MainMenu.Instance.SwitchMenu(Menu.BattleResult, false, num2);
		}
		else
		{
			MainMenu.Instance.SwitchMenu(Menu.Deathmatch);
		}
	}

	[RPC]
	protected override void RestartGame()
	{
		base.RestartGame();
	}

	[RPC]
	protected override void OnPlayerDead(string killer, string victim, int score)
	{
		base.OnPlayerDead(killer, victim, score);
	}

	[RPC]
	public override void RemoveFromTeam(int teamIndex, string playerName, PhotonMessageInfo info = null)
	{
		base.RemoveFromTeam(teamIndex, playerName, info);
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
	}

	[RPC]
	public override void StartPlay(int teamIndex)
	{
		base.StartPlay(teamIndex);
	}

	[RPC]
	public override void UpdatePlayerData(int teamIndex, string playerName, object[] playerData)
	{
		base.UpdatePlayerData(teamIndex, playerName, playerData);
	}
}
