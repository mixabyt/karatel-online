internal class OceanLand : ITerrainGenerationMethod
{
	private float[,] lowerGround;

	public void GenerateTerrain(WorldData worldData, Chunk bottomChunk, int noiseBlockOffset)
	{
		lowerGround = new float[worldData.ChunkBlockWidth, worldData.ChunkBlockHeight];
		LandPass(worldData, bottomChunk, noiseBlockOffset);
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
				int num5 = worldData.DepthInBlocks / 2;
				lowerGround[i, j] = lowerGroundHeight;
				for (int num6 = worldData.DepthInBlocks - 1; num6 >= 0; num6--)
				{
					BlockType blockType = ((num6 <= num5) ? ((!((float)num6 > lowerGroundHeight)) ? BlockType.Sand : BlockType.Water) : BlockType.Air);
					WorldData.Instance.SetBlockType(num3, num4, num6, blockType);
				}
			}
		}
	}

	private static float GetLowerGroundHeight(int blockX, int blockY, int worldDepthInBlocks, int noiseBlockOffset)
	{
		int num = worldDepthInBlocks / 10;
		int num2 = (int)((float)worldDepthInBlocks * 0.5f);
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
