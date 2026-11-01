public class SnowTreeDecorator : IDecoration
{
	private readonly WorldData m_WorldData;

	public SnowTreeDecorator(WorldData worldData)
	{
		m_WorldData = worldData;
	}

	public void Decorate(int blockX, int blockY, int blockZ, RandomAdv random)
	{
		if (IsAValidLocationforDecoration(blockX, blockY, blockZ, random))
		{
			CreateDecorationAt(blockX, blockY, blockZ, random);
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
		int num = random.RandomRange(3, 5);
		for (int i = blockZ + 1; i <= blockZ + num; i++)
		{
			CreateTrunkAt(blockX, blockY, i);
		}
		int num2 = random.RandomRange(2, 3);
		CreatePlain(blockX, blockY, blockZ + num, num2);
		CreatePlain(blockX, blockY, blockZ + num + 1, num2);
		CreatePlain(blockX, blockY, blockZ + num + 2, num2);
		CreatePlain(blockX, blockY, blockZ + num + 3, num2 - 1);
		CreatePlain(blockX, blockY, blockZ + num + 4, num2 - 2);
	}

	private void CreatePlain(int blockX, int blockY, int blockZ, int radius)
	{
		for (int i = blockX - radius; i <= blockX + radius; i++)
		{
			for (int j = blockY - radius; j <= blockY + radius; j++)
			{
				m_WorldData.SetBlockType(i, j, blockZ, BlockType.Snow);
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
