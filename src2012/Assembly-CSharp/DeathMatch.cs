using System.Collections;
using System.Linq;
using UnityEngine;

public class DeathMatch : TeamBattle
{
	public override void OnGUI()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
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
			GameEnded();
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
		GUI.Box(new Rect((float)(Screen.width / 2 - 300), (float)(Screen.height / 2 - 225), 600f, 450f), "Война");
		if ((Object)(object)TabMenu.Instance.guiSkin != (Object)null)
		{
			GUI.skin = TabMenu.Instance.guiSkin;
		}
		DrawTeam(new Rect((float)(Screen.width / 2 - 250), (float)(Screen.height / 2 - 150), 500f, 280f), _Teams[1]);
		EnableVoiceChat = GUI.Toggle(new Rect((float)(Screen.width / 2 - 75), (float)(Screen.height / 2 + 150 + 50), 350f, 90f), EnableVoiceChat, "Использовать голосовой чат.[P]");
		if (_GameEnded)
		{
			return;
		}
		if ((_Teams[0][PhotonNetwork.playerName] == null || !_TeamSelected) && GUI.Button(new Rect((float)(Screen.width / 2 + 50), (float)(Screen.height / 2 + 150), 150f, 40f), "Наблюдать"))
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
			if (GUI.Button(new Rect((float)(Screen.width / 2 - 150), (float)(Screen.height / 2 + 150), 200f, 40f), "Начать Играть"))
			{
				_TeamSelected = true;
				TabMenu.Instance.showMenu = false;
				bs._MainMenu.ShowWeaponWindow();
				StartPlay(1);
			}
		}
	}

	private void GameEnded()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
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
		GUILayout.Label(" вы заняли " + (_Teams[1].Players.OrderBy((Player a) => a.Kills).TakeWhile((Player a) => a.Name == bs._Igor.playerName).Count() + 1) + " ое место! Время до рестарта " + num + ".", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		if (num == 0)
		{
			if (PhotonNetwork.isMasterClient)
			{
				photonView.RPC("RestartGame", PhotonTargets.All);
			}
			_GameEnded = false;
		}
		GUILayout.Space(50f);
		if (GUILayout.Button("Выйти", new GUILayoutOption[0]))
		{
			WorldGameObject.INSTANCE.ExitGame();
		}
		GUILayout.EndArea();
	}

	public override bool IsOpposite(string playerName)
	{
		return true;
	}

	public override bool IsTeammate(string playerName)
	{
		return false;
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

	protected override IEnumerator Start()
	{
		if (App.instance.gameIni.isServer)
		{
			while (!WorldGameObject.INSTANCE.IsWorldGenerated)
			{
				yield return 0;
			}
			StartPlay(0);
		}
	}

	[RPC]
	public override void AddToTeam(int teamIndex, string playerName, object[] playerData, PhotonMessageInfo info = null)
	{
		base.AddToTeam(teamIndex, playerName, playerData, info);
	}

	[RPC]
	protected override void EndGame(int winner)
	{
		base.EndGame(winner);
	}

	[RPC]
	public override void OnPlayerDead(string playerName)
	{
		base.OnPlayerDead(playerName);
	}

	[RPC]
	public override void OnPlayerKill(string playerName)
	{
		base.OnPlayerKill(playerName);
	}

	[RPC]
	public override void RemoveFromTeam(int teamIndex, string playerName, PhotonMessageInfo info = null)
	{
		base.RemoveFromTeam(teamIndex, playerName, info);
	}

	[RPC]
	protected override void RestartGame()
	{
		base.RestartGame();
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
	protected override void StartPlay(int teamIndex)
	{
		base.StartPlay(teamIndex);
	}

	[RPC]
	public override string ToString()
	{
		return ((Object)this).ToString();
	}

	[RPC]
	public override void UpdatePlayerData(int teamIndex, string playerName, object[] playerData)
	{
		base.UpdatePlayerData(teamIndex, playerName, playerData);
	}
}
