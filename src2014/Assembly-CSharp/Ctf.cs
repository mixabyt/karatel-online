using UnityEngine;

public class Ctf : TeamBattle
{
	public const int FlagScore = 30;

	public const float FlagTakeDistance = 3f;

	[RPC]
	public override void AddToTeam(int teamIndex, string playerName, object[] playerData, PhotonMessageInfo info = null)
	{
		base.AddToTeam(teamIndex, playerName, playerData, info);
	}

	[RPC]
	public override void RemoveFromTeam(int teamIndex, string playerName, PhotonMessageInfo info = null)
	{
		base.RemoveFromTeam(teamIndex, playerName, info);
	}

	[RPC]
	protected override void OnPlayerDead(string killer, string victim, int score)
	{
		base.OnPlayerDead(killer, victim, score);
	}

	[RPC]
	protected override void EndGame(int winner)
	{
		base.EndGame(winner);
	}

	[RPC]
	public override void StartPlay(int teamIndex)
	{
		base.StartPlay(teamIndex);
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
	public override void UpdatePlayerData(int teamIndex, string playerName, object[] playerData)
	{
		base.UpdatePlayerData(teamIndex, playerName, playerData);
	}
}
