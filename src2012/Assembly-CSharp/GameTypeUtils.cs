using UnityEngine;

public class GameTypeUtils : MonoBehaviour
{
	public static bool BuildDisabled()
	{
		return (Object)(object)TeamBattle.Instance != (Object)null || (Object)(object)Survival.Instance != (Object)null;
	}

	public static bool DigDisabled()
	{
		return false;
	}

	public static bool CanSaveMap()
	{
		return (Object)(object)TeamBattle.Instance == (Object)null && (Object)(object)Survival.Instance == (Object)null && (Object)(object)TeamBattle.Instance == (Object)null;
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
}
