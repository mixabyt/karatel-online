using ExitGames.Client.Photon;
using Photon;
using UnityEngine;

public class GameType : MonoBehaviour
{
	public static bool BattleMode()
	{
		return (Object)(object)TeamBattle.Instance != (Object)null;
	}

	public static bool IsObserving()
	{
		return (Object)(object)TeamBattle.Instance != (Object)null && TeamBattle.Instance.IsObserving(PhotonNetwork.playerName);
	}

	public static bool BuildDisabled()
	{
		return BattleMode();
	}

	public static bool BlocksBuildDisabled()
	{
		return WorldGameObject.Instance.MainPlayerNode.IsZombie;
	}

	public static bool DigDisabled()
	{
		return false;
	}

	public static bool HitDisabled()
	{
		return BattleMode();
	}

	public static bool CanSaveMap()
	{
		return !BattleMode();
	}

	public static Vector3 FindRandomPlace(Vector3[] awayFrom = null, float awayDistance = 10f)
	{
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		while (num < 100)
		{
			int num2 = Random.Range(5, WorldData.Instance.GetMaxBlockX() - 5);
			int num3 = Random.Range(5, WorldData.Instance.GetMaxBlockY() - 5);
			int num4 = WorldData.Instance.DepthInBlocks;
			while (num4 >= 0 && num4 != 0 && WorldData.Instance.GetBlockType(num2, num3, num4 - 1) == BlockType.Air)
			{
				num4--;
			}
			Vector3 val = new Vector3((float)num2 + 0.5f, (float)num4 + 0.5f, (float)num3 + 0.5f);
			if (awayFrom != null)
			{
				bool flag = false;
				foreach (Vector3 val2 in awayFrom)
				{
					if (Vector3.Distance(val, val2) < awayDistance)
					{
						flag = true;
						break;
					}
				}
				if (flag && (float)num < awayDistance - 1f)
				{
					num++;
					continue;
				}
			}
			return val;
		}
		return Vector3.zero;
	}

	public static void Activate()
	{
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		string text = null;
		string text2 = null;
		if (App.Instance.Settings.gameType == GameINI.GameType.DEATHMATCH)
		{
			text = "GameType/Deathmatch";
		}
		else if (App.Instance.Settings.gameType == GameINI.GameType.SURVIVAL)
		{
			text = "GameType/Survival";
		}
		else if (App.Instance.Settings.gameType == GameINI.GameType.TEAM_BATTLE)
		{
			int num = 0;
			int num2 = 0;
			Object[] array = Object.FindObjectsOfType(typeof(SpawnArrow));
			for (int i = 0; i < array.Length; i++)
			{
				SpawnArrow spawnArrow = (SpawnArrow)(object)array[i];
				if (spawnArrow.Type == EntityType.TEAM_SPAWN_ARROW_RED)
				{
					num++;
				}
				else if (spawnArrow.Type == EntityType.TEAM_SPAWN_ARROW_BLUE)
				{
					num2++;
				}
			}
			if (num > 0 && num2 > 0)
			{
				text = "GameType/TeamBattle";
			}
			else if (num == 0)
			{
				text2 = "HINT_MISSED_SPAWN_POINTS";
			}
			else if (num2 == 0)
			{
				text2 = "HINT_MISSED_BLUE_SPAWN_POINTS";
			}
		}
		else if (App.Instance.Settings.gameType == GameINI.GameType.CTF)
		{
			int num3 = 0;
			int num4 = 0;
			Object[] array2 = Object.FindObjectsOfType(typeof(Flag));
			for (int j = 0; j < array2.Length; j++)
			{
				Flag flag = (Flag)(object)array2[j];
				if (!flag.IsBlue)
				{
					num3++;
				}
				else
				{
					num4++;
				}
			}
			if (num3 == 1 && num4 == 1)
			{
				text = "GameType/Ctf";
			}
			else if (num3 == 0)
			{
				text2 = "HINT_MISSED_RED_FLAG";
			}
			else if (num3 > 1)
			{
				text2 = "HINT_MANY_RED_FLAGS";
			}
			else if (num4 == 0)
			{
				text2 = "HINT_MISSED_BLUE_FLAG";
			}
			else if (num4 > 1)
			{
				text2 = "HINT_MANY_BLUE_FLAGS";
			}
		}
		else if (App.Instance.Settings.gameType == GameINI.GameType.ZOMBIE_VIRUS)
		{
			text = "GameType/ZombieVirus";
		}
		if (text != null)
		{
			PhotonNetwork.InstantiateSceneObject(text, Vector3.zero, Quaternion.identity, 0, null);
		}
		else if (App.Instance.Settings.gameType != GameINI.GameType.BUILDING)
		{
			App.Instance.Settings.gameType = GameINI.GameType.BUILDING;
			Hashtable hashtable = new Hashtable();
			hashtable.Add("game_type", App.Instance.Settings.gameType);
			PhotonNetwork.room.SetCustomProperties(hashtable);
		}
		WorldGameObject.Instance.ShowGameGUI();
		if (text2 != null)
		{
			MainMenu.Instance.ShowHint(text2, asNewMenu: true);
		}
	}
}
