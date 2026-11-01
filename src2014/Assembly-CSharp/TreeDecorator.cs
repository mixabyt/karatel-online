using UnityEngine;

public class TreeDecorator : IDecoration
{
	private readonly WorldData m_WorldData;

	private GameObject[] model_list;

	public TreeDecorator(WorldData worldData, GameObject[] list)
	{
		m_WorldData = worldData;
		model_list = list;
	}

	public void Decorate(int blockX, int blockY, int blockZ, RandomAdv random)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected Obj, but got Unknown
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Expected Obj, but got Unknown
		GameObject val = (GameObject)Object.Instantiate((Object)(object)model_list[0], new Vector3((float)blockX + 0.5f, (float)(blockZ + 1), (float)blockY + 0.5f), model_list[0].transform.rotation);
		val.layer = 8;
		if (random.RandomRange(1, 100) >= 99)
		{
			int num = Random.Range(1, model_list.Length);
			val = (GameObject)Object.Instantiate((Object)(object)model_list[num], new Vector3((float)blockX + 0.5f, (float)(blockZ + 1), (float)blockY + 0.5f), model_list[num].transform.rotation);
			val.layer = 12;
		}
	}

	private bool IsAValidLocationforDecoration(int blockX, int blockY, int blockZ, RandomAdv random)
	{
		if (random.RandomRange(1, 100) < 99)
		{
			return false;
		}
		if (blockZ >= m_WorldData.DepthInBlocks - 20)
		{
			return false;
		}
		return SpaceAboveIsEmpty(blockX, blockY, blockZ, 8, 2, 2);
	}

	private void CreateDecorationAt(int blockX, int blockY, int blockZ, RandomAdv random)
	{
		int num = random.RandomRange(6, 10);
		for (int i = blockZ + 1; i <= blockZ + num; i++)
		{
			CreateTrunkAt(blockX, blockY, i);
		}
		CreateSphereAt(blockX, blockY, blockZ + num, random.RandomRange(3, 4));
	}

	private void CreateSphereAt(int blockX, int blockY, int blockZ, int radius)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		for (int i = blockX - radius; i <= blockX + radius; i++)
		{
			for (int j = blockY - radius; j <= blockY + radius; j++)
			{
				for (int k = blockZ - radius; k <= blockZ + radius; k++)
				{
					if (Vector3.Distance(new Vector3((float)blockX, (float)blockY, (float)blockZ), new Vector3((float)i, (float)j, (float)k)) <= (float)radius)
					{
						m_WorldData.SetBlockType(i, j, k, BlockType.Leaves);
					}
				}
			}
		}
	}

	private void CreateTrunkAt(int blockX, int blockY, int z)
	{
		m_WorldData.SetBlockType(blockX, blockY, z, BlockType.Wood);
	}

	private bool SpaceAboveIsEmpty(int blockX, int blockY, int blockZ, int depthAbove, int widthAround, int heightAround)
	{
		for (int i = blockZ + 1; i <= blockZ + depthAbove; i++)
		{
			for (int j = blockX - widthAround; j <= blockX + widthAround; j++)
			{
				for (int k = blockY - heightAround; k < blockY + heightAround; k++)
				{
					if (m_WorldData.GetBlockType(j, k, i) != BlockType.Air)
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	public override string ToString()
	{
		return "Standard Tree";
	}
}
