internal class SnowLand : ITerrainGenerationMethod
{
	private float[,] lowerGround;

	private float[,] upperGround;

	private float[,] stoneGround;

	public void GenerateTerrain(WorldData worldData, Chunk bottomChunk, int noiseBlockOffset)
	{
		lowerGround = new float[worldData.ChunkBlockWidth, worldData.ChunkBlockHeight];
		upperGround = new float[worldData.ChunkBlockWidth, worldData.ChunkBlockHeight];
		LandPass(worldData, bottomChunk, noiseBlockOffset);
	}

	private void CoalIronResource(WorldData worldData, Chunk bottomChunk, int noiseBlockOffset)
	{
		int num = bottomChunk.X * worldData.ChunkBlockWidth;
		int num2 = bottomChunk.Y * worldData.ChunkBlockHeight;
		for (int i = 0; i < worldData.ChunkBlockWidth; i++)
		{
			int num3 = num + i;
			for (int j = 0; j < worldData.ChunkBlockHeight; j++)
			{
				int num4 = num2 + j;
				for (int num5 = worldData.DepthInBlocks - 1; num5 >= 0; num5--)
				{
					BlockType blockType = BlockType.Air;
					int num6 = num3 + noiseBlockOffset;
					float num7 = PerlinSimplexNoise.noise((float)num6 * 0.08f, (float)num4 * 0.08f, (float)num5 * 0.08f) * 0.25f;
					if ((!((float)num5 >= lowerGround[i, j] - 5f) || !((float)num5 < upperGround[i, j])) && (float)num5 < lowerGround[i, j] - 15f && num7 < -0.234f && WorldData.Instance.GetBlockType(num3, num4, num5) == BlockType.Stone)
					{
						blockType = BlockType.Gold;
						WorldData.Instance.SetBlockType(i, j, num5, blockType);
					}
				}
			}
		}
	}

	private void CaveSystem(WorldData worldData, Chunk bottomChunk, int noiseBlockOffset)
	{
		int num = bottomChunk.X * worldData.ChunkBlockWidth;
		int num2 = bottomChunk.Y * worldData.ChunkBlockHeight;
		for (int i = 0; i < worldData.ChunkBlockWidth; i++)
		{
			int num3 = num + i;
			for (int j = 0; j < worldData.ChunkBlockHeight; j++)
			{
				int num4 = num2 + j;
				for (int num5 = worldData.DepthInBlocks - 1; num5 >= 0; num5--)
				{
					BlockType blockType = BlockType.Air;
					if ((float)num5 <= lowerGround[i, j])
					{
						int num6 = num3 + noiseBlockOffset;
						float num7 = PerlinSimplexNoise.noise((float)num6 * 0.04f, (float)num4 * 0.04f, (float)num5 * 0.04f) * 0.7f;
						num7 += PerlinSimplexNoise.noise((float)num6 * 0.02f, (float)num4 * 0.02f, (float)num5 * 0.02f) * 0.25f;
						if (num7 < -0.5f)
						{
							blockType = BlockType.Air;
							WorldData.Instance.SetBlockType(num3, num4, num5, blockType);
						}
					}
				}
			}
		}
	}

	private void LandPass(WorldData worldData, Chunk bottomChunk, int noiseBlockOffset)
	{
		int num = bottomChunk.X * worldData.ChunkBlockWidth;
		int num2 = bottomChunk.Y * worldData.ChunkBlockHeight;
		for (int i = 0; i < worldData.ChunkBlockWidth; i++)
		{
			int num3 = num + i;
			for (int j = 0; j < worldData.ChunkBlockHeight; j++)
			{
				int num4 = num2 + j;
				float lowerGroundHeight = GetLowerGroundHeight(num3, num4, worldData.DepthInBlocks, noiseBlockOffset);
				int upperGroundHeight = GetUpperGroundHeight(worldData, num3, num4, lowerGroundHeight, noiseBlockOffset);
				lowerGround[i, j] = lowerGroundHeight;
				upperGround[i, j] = upperGroundHeight;
				bool flag = true;
				for (int num5 = worldData.DepthInBlocks - 1; num5 >= 0; num5--)
				{
					BlockType blockType;
					if (num5 > upperGroundHeight)
					{
						blockType = BlockType.Air;
					}
					else if ((float)num5 > lowerGroundHeight - 5f)
					{
						int num6 = num3 + noiseBlockOffset;
						float num7 = PerlinSimplexNoise.noise((float)num6 * 0.009f, (float)num4 * 0.009f, (float)num5 * 0.0009f) * 0.25f;
						num7 += PerlinSimplexNoise.noise((float)num6 * 0.04f, (float)num4 * 0.04f, (float)num5 * 0.01f) * 0.05f;
						num7 += PerlinSimplexNoise.noise((float)num6 * 0.01f, (float)num4 * 0.01f, (float)num5 * 0.01f) * 0.15f;
						if (num7 < 0.12f)
						{
							blockType = BlockType.Air;
							if (num5 < 48)
							{
								blockType = BlockType.Ice;
							}
						}
						else if (flag)
						{
							blockType = BlockType.SnowDirt;
							bottomChunk.TopSoilBlocks.Add(new IntVect(num3, num4, num5));
							flag = false;
						}
						else
						{
							blockType = BlockType.Stone;
						}
					}
					else
					{
						if (flag)
						{
							blockType = BlockType.SnowDirt;
							bottomChunk.TopSoilBlocks.Add(new IntVect(num3, num4, num5));
							flag = false;
						}
						else
						{
							blockType = BlockType.SnowDirt;
						}
						if ((float)num5 < lowerGroundHeight - 15f)
						{
							blockType = BlockType.Stone;
						}
					}
					WorldData.Instance.SetBlockType(num3, num4, num5, blockType);
				}
			}
		}
	}

	private static int GetUpperGroundHeight(WorldData worldData, int blockX, int blockY, float lowerGroundHeight, int noiseBlockOffset)
	{
		int num = blockX + noiseBlockOffset;
		float num2 = PerlinSimplexNoise.noise((float)num * 0.001f, (float)blockY * 0.001f) * 0.5f;
		float num3 = PerlinSimplexNoise.noise((float)(num + 100) * 0.002f, (float)blockY * 0.002f) * 0.25f;
		float num4 = PerlinSimplexNoise.noise((float)(num + 100) * 0.01f, (float)blockY * 0.01f) * 0.25f;
		float num5 = num2 + num3 + num4;
		return (int)(num5 * ((float)worldData.DepthInBlocks / 3f)) + (int)lowerGroundHeight - 5;
	}

	private static float GetLowerGroundHeight(int blockX, int blockY, int worldDepthInBlocks, int noiseBlockOffset)
	{
		int num = worldDepthInBlocks / 4;
		int num2 = (int)((float)worldDepthInBlocks * 0.75f);
		float num3 = PerlinSimplexNoise.noise((float)blockX * 5E-05f, (float)blockY * 5E-05f) * 0.5f;
		float num4 = PerlinSimplexNoise.noise((float)blockX * 0.0005f, (float)blockY * 0.0005f) * 0.25f;
		float num5 = PerlinSimplexNoise.noise((float)blockX * 0.005f, (float)blockY * 0.005f) * 0.12f;
		float num6 = PerlinSimplexNoise.noise((float)blockX * 0.01f, (float)blockY * 0.01f) * 0.12f;
		float num7 = PerlinSimplexNoise.noise((float)blockX * 0.03f, (float)blockY * 0.03f) * num6;
		float num8 = num3 + num4 + num5 + num6 + num7;
		num8 = num8 * (float)num2 + (float)num;
		for (int num9 = (int)num8; num9 >= 0; num9--)
		{
			WorldData.Instance.SetBlockType(blockX, blockY, num9, BlockType.Dirt);
		}
		return num8 - 20f;
	}
}
