internal class FlatLand : ITerrainGenerationMethod
{
	public void GenerateTerrain(WorldData worldData, Chunk bottomChunk, int noiseBlockOffset)
	{
		LandPass(worldData, bottomChunk, noiseBlockOffset);
	}

	private void LandPass(WorldData worldData, Chunk bottomChunk, int noiseBlockOffset)
	{
		int num = bottomChunk.X * worldData.ChunkBlockWidth;
		int num2 = bottomChunk.Y * worldData.ChunkBlockHeight;
		for (int i = 0; i < worldData.ChunkBlockWidth; i++)
		{
			for (int j = 0; j < worldData.ChunkBlockHeight; j++)
			{
				int num3 = 25;
				for (int num4 = worldData.DepthInBlocks - 1; num4 >= 0; num4--)
				{
					BlockType blockType = ((num4 <= num3) ? ((num4 < num3 - 1) ? BlockType.Dirt : BlockType.TopSoil) : BlockType.Air);
					WorldData.Instance.SetBlockType(num + i, num2 + j, num4, blockType);
				}
			}
		}
	}
}
