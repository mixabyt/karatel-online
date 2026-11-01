using LibNoise.Unity;
using LibNoise.Unity.Generator;
using UnityEngine;

public class StandardTerrainMediumCaves : ITerrainGenerationMethod
{
	private ModuleBase moduleBase;

	public void GenerateTerrain(WorldData worldData, Chunk bottomChunk, int noiseBlockOffset)
	{
		moduleBase = new Voronoi();
		int num = bottomChunk.X * worldData.ChunkBlockWidth;
		int num2 = bottomChunk.Y * worldData.ChunkBlockHeight;
		for (int i = 0; i < worldData.ChunkBlockWidth; i++)
		{
			int blockX = num + i;
			for (int j = 0; j < worldData.ChunkBlockHeight; j++)
			{
				int blockY = num2 + j;
				GenerateStandardTerrain(blockX, blockY, worldData.DepthInBlocks, noiseBlockOffset);
			}
		}
	}

	private void GenerateStandardTerrain(int blockX, int blockY, int worldDepthInBlocks, int noiseBlockOffset)
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		int num = (int)GetBlockNoise(blockX, blockY);
		if (num < 1)
		{
			num = 1;
		}
		else if (num > 128)
		{
			num = 96;
		}
		bool flag = true;
		BlockType blockType = BlockType.Air;
		WorldData.Instance.SetBlockType(blockX, blockY, num, BlockType.TopSoil);
		WorldData.Instance.SetBlockType(blockX, blockY, 0, BlockType.Stone);
		for (int num2 = worldDepthInBlocks - 1; num2 > 0; num2--)
		{
			if (num2 < num)
			{
				int num3 = blockX + noiseBlockOffset;
				float num4 = (float)moduleBase.GetValue(new Vector3((float)num3 * 0.009f, (float)blockY * 0.009f, (float)num2 * 0.009f)) * 0.25f;
				float num5 = num4;
				if (num5 > 0f)
				{
					blockType = BlockType.Dirt;
				}
				else
				{
					blockType = BlockType.Air;
					if (flag)
					{
						flag = false;
					}
					else if (!(num4 < 0.2f))
					{
					}
				}
			}
			WorldData.Instance.SetBlockType(blockX, blockY, num2, blockType);
		}
	}

	private float GetBlockNoise(int blockX, int blockY)
	{
		float num = PerlinSimplexNoise.noise((float)blockX / 300f, (float)blockY / 300f, 20f);
		float num2 = PerlinSimplexNoise.noise((float)blockX / 80f, (float)blockY / 80f, 30f);
		float num3 = PerlinSimplexNoise.noise((float)blockX / 800f, (float)blockY / 800f);
		float num4 = num3 * 64f + num * 32f + num2 * 16f;
		return num4 + 16f;
	}
}
