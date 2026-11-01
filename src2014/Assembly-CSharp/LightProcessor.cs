using System.Collections.Generic;
using UnityEngine;

public class LightProcessor : ILightProcessor
{
	private readonly WorldData _WorldData;

	private byte[,,] _BlocksLight;

	public int LightingSteps => 9;

	public LightProcessor(WorldData worldData)
	{
		_WorldData = worldData;
	}

	public void InitializeLighting()
	{
		_BlocksLight = new byte[_WorldData.WidthInBlocks, _WorldData.HeightInBlocks, _WorldData.DepthInBlocks];
		for (int i = 0; i < _WorldData.WidthInBlocks; i++)
		{
			for (int j = 0; j < _WorldData.HeightInBlocks; j++)
			{
				for (int k = 0; k < _WorldData.DepthInBlocks; k++)
				{
					if (WorldData.Instance.GetBlockType(i, j, k) == BlockType.Lava)
					{
						SetBlockLight(i, j, k, (byte)LightingSteps);
					}
				}
			}
		}
	}

	public void InitializeLightingChunk(Chunk chunk)
	{
		if (chunk.Z == 0)
		{
			int x = chunk.X * _WorldData.ChunkBlockWidth;
			int x2 = chunk.X * _WorldData.ChunkBlockWidth + _WorldData.ChunkBlockWidth - 1;
			int y = chunk.Y * _WorldData.ChunkBlockHeight;
			int y2 = chunk.Y * _WorldData.ChunkBlockHeight + _WorldData.ChunkBlockHeight - 1;
			int z = 0;
			int z2 = _WorldData.DepthInBlocks - 1;
			RecalculateLighting(x, x2, y, y2, z, z2);
		}
	}

	public void AddLight(int x, int y, int z, bool recalculateLighting)
	{
		SetBlockLight(x, y, z, (byte)LightingSteps);
		if (recalculateLighting)
		{
			RecalculateLightingAroundBlock(x, y, z, 0, lightChanged: true);
		}
	}

	public void RemoveLight(int x, int y, int z, bool recalculateLighting)
	{
		SetBlockLight(x, y, z, 0);
		if (recalculateLighting)
		{
			RecalculateLightingAroundBlock(x, y, z, 0, lightChanged: true);
		}
	}

	public bool IsLight(int x, int y, int z)
	{
		return (GetBlockLight(x, y, z) & 0xF) == LightingSteps;
	}

	public void RecalculateLightingAroundBlock(int x, int y, int z, int radius, bool lightChanged)
	{
		int num = Mathf.Max(x - radius - LightingSteps, 0);
		int num2 = Mathf.Min(x + radius + LightingSteps, _WorldData.WidthInBlocks - 1);
		int num3 = Mathf.Max(y - radius - LightingSteps, 0);
		int num4 = Mathf.Min(y + radius + LightingSteps, _WorldData.HeightInBlocks - 1);
		int num5 = Mathf.Max(z - radius - LightingSteps, 0);
		int num6 = Mathf.Min(z + radius + LightingSteps, _WorldData.DepthInBlocks - 1);
		int num7;
		int x2;
		int num8;
		int num9;
		int num10;
		int num11;
		if (lightChanged)
		{
			num7 = num;
			x2 = num2;
			num8 = num3;
			num9 = num4;
			num10 = num5;
			num11 = num6;
		}
		else
		{
			num7 = Mathf.Max(x - radius, 0);
			x2 = Mathf.Min(x + radius, _WorldData.WidthInBlocks - 1);
			num8 = Mathf.Max(y - radius, 0);
			num9 = Mathf.Min(y + radius, _WorldData.HeightInBlocks - 1);
			num10 = Mathf.Max(z - 1, num5);
			num11 = Mathf.Min(z + 1, num6);
			for (int i = num; i <= num2; i++)
			{
				for (int j = num3; j <= num4; j++)
				{
					for (int k = num5; k <= num6; k++)
					{
						if ((GetBlockLight(i, j, k) & 0xF) == LightingSteps)
						{
							num7 = Mathf.Min(num7, i);
							x2 = Mathf.Max(num7, i);
							num8 = Mathf.Min(num8, j);
							num9 = Mathf.Max(num9, j);
							num10 = Mathf.Min(num10, k);
							num11 = Mathf.Max(num11, k);
						}
					}
				}
			}
		}
		RecalculateLighting(num7, x2, num8, num9, num10, num11);
	}

	public void RecalculateLighting(int x1, int x2, int y1, int y2, int z1, int z2)
	{
		List<IntVect> list = new List<IntVect>();
		int chunkBlockWidth = _WorldData.ChunkBlockWidth;
		int chunkBlockHeight = _WorldData.ChunkBlockHeight;
		int chunkBlockDepth = _WorldData.ChunkBlockDepth;
		for (int i = x1; i <= x2; i++)
		{
			for (int j = y1; j <= y2; j++)
			{
				int num = _WorldData.DepthInBlocks - 1;
				Chunk chunk = WorldData.Instance.GetChunk(i, j, num);
				bool flag = false;
				bool flag2 = false;
				while (num >= 0)
				{
					BlockType blockType = chunk.GetBlockType(i % chunkBlockWidth, j % chunkBlockHeight, num % chunkBlockDepth);
					BlockKind block = chunk.GetBlockKind(i % chunkBlockWidth, j % chunkBlockHeight, num % chunkBlockDepth);
					if (block.IsFlip())
					{
						block = block.DoFlip();
					}
					if (!flag && blockType.IsLand() && !block.IsFence())
					{
						flag = true;
					}
					bool flag3 = flag;
					if (flag3 && !flag2)
					{
						flag2 = true;
						if (!block.IsDefault())
						{
							flag3 = false;
						}
					}
					int num2 = (flag3 ? (LightingSteps / 2) : LightingSteps);
					if (num >= z1 && num <= z2)
					{
						if ((GetBlockLight(i, j, num) & 0xF) == LightingSteps)
						{
							list.Add(new IntVect(i, j, num));
						}
						SetBlockLight(i, j, num, (byte)(num2 << 4));
						chunk.NeedsRegeneration = true;
					}
					else if (GetBlockLight(i, j, num) >> 4 != num2)
					{
						SetBlockLight(i, j, num, (byte)((num2 << 4) | (GetBlockLight(i, j, num) & 0xF)));
						chunk.NeedsRegeneration = true;
					}
					num--;
					if (num % chunkBlockDepth == chunkBlockDepth - 1)
					{
						chunk = WorldData.Instance.GetChunk(i, j, num);
					}
				}
			}
		}
		foreach (IntVect item in list)
		{
			SetLightingAroundBlockRecursive(item.X, item.Y, item.Z, item.X, item.Y, item.Z, LightingSteps);
		}
		x1 = Mathf.Max(x1 - 1, 0);
		x2 = Mathf.Min(x2 + 1, _WorldData.WidthInBlocks - 1);
		y1 = Mathf.Max(y1 - 1, 0);
		y2 = Mathf.Min(y2 + 1, _WorldData.HeightInBlocks - 1);
		z1 = Mathf.Max(z1 - 1, 0);
		z2 = Mathf.Min(z2 + 1, _WorldData.DepthInBlocks - 1);
		for (int k = x1; k <= x2; k++)
		{
			for (int l = y1; l <= y2; l++)
			{
				ProcessPerimeterCube(k, l, z1);
			}
		}
		for (int m = x1; m <= x2; m++)
		{
			for (int n = y1; n <= y2; n++)
			{
				ProcessPerimeterCube(m, n, z2);
			}
		}
		for (int num3 = x1; num3 <= x2; num3++)
		{
			for (int num4 = z1; num4 <= z2; num4++)
			{
				ProcessPerimeterCube(num3, y1, num4);
			}
		}
		for (int num5 = x1; num5 <= x2; num5++)
		{
			for (int num6 = z1; num6 <= z2; num6++)
			{
				ProcessPerimeterCube(num5, y2, num6);
			}
		}
		for (int num7 = z1; num7 <= z2; num7++)
		{
			for (int num8 = y1; num8 <= y2; num8++)
			{
				ProcessPerimeterCube(x1, num8, num7);
			}
		}
		for (int num9 = z1; num9 <= z2; num9++)
		{
			for (int num10 = y1; num10 <= y2; num10++)
			{
				ProcessPerimeterCube(x2, num10, num9);
			}
		}
	}

	private void ProcessPerimeterCube(int x, int y, int z)
	{
		int num = GetBlockLight(x, y, z) & 0xF;
		if (num > 1)
		{
			num--;
			SetLightingAroundBlockRecursive(x - 1, y, z, x, y, z, num);
			SetLightingAroundBlockRecursive(x + 1, y, z, x, y, z, num);
			SetLightingAroundBlockRecursive(x, y - 1, z, x, y, z, num);
			SetLightingAroundBlockRecursive(x, y + 1, z, x, y, z, num);
			SetLightingAroundBlockRecursive(x, y, z - 1, x, y, z, num);
			SetLightingAroundBlockRecursive(x, y, z + 1, x, y, z, num);
		}
	}

	private void SetLightingAroundBlockRecursive(int x, int y, int z, int fromX, int fromY, int fromZ, int lightAmount)
	{
		if (x < 0 || y < 0 || x >= _WorldData.WidthInBlocks || y >= _WorldData.HeightInBlocks || z >= _WorldData.DepthInBlocks || z < 0)
		{
			return;
		}
		int num = x / _WorldData.ChunkBlockWidth;
		int num2 = y / _WorldData.ChunkBlockHeight;
		int num3 = z / _WorldData.ChunkBlockDepth;
		int x2 = x % _WorldData.ChunkBlockWidth;
		int y2 = y % _WorldData.ChunkBlockHeight;
		int z2 = z % _WorldData.ChunkBlockDepth;
		Chunk chunk = _WorldData.Chunks[num, num2, num3];
		BlockType blockType = chunk.GetBlockType(x2, y2, z2);
		if ((blockType != BlockType.Air && !WorldGameObject.Instance.BlockParametrs[(uint)blockType].is_glass && (lightAmount != LightingSteps || blockType != BlockType.Lava)) || (GetBlockLight(x, y, z) & 0xF) >= lightAmount)
		{
			return;
		}
		SetBlockLight(x, y, z, (byte)((GetBlockLight(x, y, z) & 0xF0) | lightAmount));
		chunk.NeedsRegeneration = true;
		if (lightAmount > 0)
		{
			lightAmount--;
			if (x - 1 != fromX || y != fromY || z != fromZ)
			{
				SetLightingAroundBlockRecursive(x - 1, y, z, x, y, z, lightAmount);
			}
			if (x + 1 != fromX || y != fromY || z != fromZ)
			{
				SetLightingAroundBlockRecursive(x + 1, y, z, x, y, z, lightAmount);
			}
			if (x != fromX || y - 1 != fromY || z != fromZ)
			{
				SetLightingAroundBlockRecursive(x, y - 1, z, x, y, z, lightAmount);
			}
			if (x != fromX || y + 1 != fromY || z != fromZ)
			{
				SetLightingAroundBlockRecursive(x, y + 1, z, x, y, z, lightAmount);
			}
			if (x != fromX || y != fromY || z - 1 != fromZ)
			{
				SetLightingAroundBlockRecursive(x, y, z - 1, x, y, z, lightAmount);
			}
			if (x != fromX || y != fromY || z + 1 != fromZ)
			{
				SetLightingAroundBlockRecursive(x, y, z + 1, x, y, z, lightAmount);
			}
		}
	}

	public byte GetBlockLight(int x, int y, int z)
	{
		return _BlocksLight[x, y, z];
	}

	private void SetBlockLight(int x, int y, int z, byte light)
	{
		_BlocksLight[x, y, z] = light;
	}
}
