using System.Collections;
using UnityEngine;

public class BlocksAvulsion : MonoBehaviour
{
	public const int AvulsionDepth = 300;

	public const float AvulsionDestroyTime = 10f;

	private static bool[,,] _ProcessedBlocks;

	private static int _BlocksWidth;

	private static int _BlocksHeight;

	private static int _BlocksDepth;

	private static int[] _ProcessedBlocksLimits;

	public static void VerifyAvulsionAroundBlocks(int x, int y, int z, int radius, bool silent)
	{
		if (App.Instance.Settings.gameType == GameINI.GameType.BUILDING)
		{
			return;
		}
		if (_ProcessedBlocks == null || _BlocksWidth != WorldData.Instance.WidthInBlocks || _BlocksHeight != WorldData.Instance.HeightInBlocks || _BlocksDepth != WorldData.Instance.DepthInBlocks)
		{
			_BlocksWidth = WorldData.Instance.WidthInBlocks;
			_BlocksHeight = WorldData.Instance.HeightInBlocks;
			_BlocksDepth = WorldData.Instance.DepthInBlocks;
			_ProcessedBlocks = new bool[_BlocksWidth, _BlocksHeight, _BlocksDepth];
		}
		for (int i = -radius; i <= radius; i++)
		{
			for (int j = -radius; j <= radius; j++)
			{
				for (int k = -radius; k <= radius; k++)
				{
					if (Mathf.Abs(i) == radius || Mathf.Abs(j) == radius || Mathf.Abs(k) == radius)
					{
						CheckBlocksAvulsion(x + i, y + j, z + k - 1, silent);
						CheckBlocksAvulsion(x + i, y + j, z + k + 1, silent);
						CheckBlocksAvulsion(x + i + 1, y + j, z + k, silent);
						CheckBlocksAvulsion(x + i - 1, y + j, z + k, silent);
						CheckBlocksAvulsion(x + i, y + j + 1, z + k, silent);
						CheckBlocksAvulsion(x + i, y + j - 1, z + k, silent);
					}
				}
			}
		}
		RevertProcessedBlocks();
	}

	private static void CheckBlocksAvulsion(int x, int y, int z, bool silent)
	{
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		if (WorldData.Instance.GetBlockType(x, y, z) == BlockType.Air || _ProcessedBlocks[x, y, z])
		{
			return;
		}
		RevertProcessedBlocks();
		int maxDepth = -1;
		int blocks = 0;
		_ProcessedBlocksLimits = new int[6] { x, x, y, y, z, z };
		CheckDepthRecursive(x, y, z, 0, ref maxDepth, ref blocks);
		if (maxDepth < 0 || maxDepth >= 300)
		{
			return;
		}
		if (!silent)
		{
			int x2 = Mathf.Max(_ProcessedBlocksLimits[0] - 1, 0);
			int x3 = Mathf.Min(_ProcessedBlocksLimits[1] + 1, _BlocksWidth - 1);
			int y2 = Mathf.Max(_ProcessedBlocksLimits[2] - 1, 0);
			int y3 = Mathf.Min(_ProcessedBlocksLimits[3] + 1, _BlocksHeight - 1);
			int z2 = Mathf.Max(_ProcessedBlocksLimits[4] - 1, 0);
			int z3 = Mathf.Min(_ProcessedBlocksLimits[5] + 1, _BlocksDepth - 1);
			BlockGetter blockGetter = (int xx, int yy, int zz) => (xx >= 0 && yy >= 0 && zz >= 0 && xx < _BlocksWidth && yy < _BlocksHeight && zz < _BlocksDepth) ? (_ProcessedBlocks[xx, yy, zz] ? WorldData.Instance.GetBlockType(xx, yy, zz) : BlockType.Air) : BlockType.Air;
			BlockKindGetter kindGetter = (int xx, int yy, int zz) => (xx >= 0 && yy >= 0 && zz >= 0 && xx < _BlocksWidth && yy < _BlocksHeight && zz < _BlocksDepth) ? (_ProcessedBlocks[xx, yy, zz] ? WorldData.Instance.GetBlockKind(xx, yy, zz) : BlockKind.Default) : BlockKind.Default;
			Chunk.MeshData[] array = World.Instance.MeshGen.GenerateMeshData(x2, x3, y2, y3, z2, z3, blockGetter, kindGetter);
			Vector3 chunkPos = default;
			ChunkGameObject chunkGameObject = null;
			for (int num = 0; num < 3; num++)
			{
				if (!array[num].IsEmpty)
				{
					ChunkGameObject chunkGameObject2 = WorldGameObject.Instance.CreateChunkGameObject(num, "Avulsion_Chunk", chunkPos, array[num]);
					((Component)chunkGameObject2).gameObject.layer = LayerMask.NameToLayer("Avulsion");
					((Component)chunkGameObject2).GetComponent<MeshCollider>().convex = true;
					if ((Object)(object)chunkGameObject == (Object)null)
					{
						chunkGameObject = chunkGameObject2;
					}
					else
					{
						((Component)chunkGameObject2).transform.parent = ((Component)chunkGameObject).transform;
					}
				}
			}
			((MonoBehaviour)chunkGameObject).StartCoroutine(ChunkAvulsionProcess(chunkGameObject, blocks));
		}
		DestroyBlocks(onlyWater: true);
		DestroyBlocks(onlyWater: false);
		if (!silent)
		{
			int x4 = Mathf.Max(_ProcessedBlocksLimits[0] - World.Instance.Lighting.LightingSteps, 0);
			int x5 = Mathf.Min(_ProcessedBlocksLimits[1] + World.Instance.Lighting.LightingSteps, _BlocksWidth - 1);
			int y4 = Mathf.Max(_ProcessedBlocksLimits[2] - World.Instance.Lighting.LightingSteps, 0);
			int y5 = Mathf.Min(_ProcessedBlocksLimits[3] + World.Instance.Lighting.LightingSteps, _BlocksHeight - 1);
			int z4 = Mathf.Max(_ProcessedBlocksLimits[4] - World.Instance.Lighting.LightingSteps, 0);
			int z5 = Mathf.Min(_ProcessedBlocksLimits[5] + World.Instance.Lighting.LightingSteps, _BlocksDepth - 1);
			World.Instance.Lighting.RecalculateLighting(x4, x5, y4, y5, z4, z5);
		}
		RevertProcessedBlocks();
	}

	private static IEnumerator ChunkAvulsionProcess(ChunkGameObject chunkGameObject, float mass)
	{
		yield return (object)new WaitForSeconds(0.1f);
		((Component)chunkGameObject).gameObject.AddComponent<Rigidbody>();
		((Component)chunkGameObject).rigidbody.mass = mass;
		yield return (object)new WaitForSeconds(10f);
		((Component)chunkGameObject).rigidbody.detectCollisions = false;
		yield return (object)new WaitForSeconds(5f);
		Object.Destroy((Object)(object)((Component)chunkGameObject).gameObject);
	}

	private static void CheckDepthRecursive(int x, int y, int z, int curDepth, ref int maxDepth, ref int blocks)
	{
		if (x < 0 || y < 0 || z < 0 || x >= _BlocksWidth || y >= _BlocksHeight || z >= _BlocksDepth)
		{
			maxDepth = 300;
		}
		else if (!_ProcessedBlocks[x, y, z] && WorldData.Instance.GetBlockType(x, y, z) != BlockType.Air)
		{
			_ProcessedBlocks[x, y, z] = true;
			if (x < _ProcessedBlocksLimits[0])
			{
				_ProcessedBlocksLimits[0] = x;
			}
			if (x > _ProcessedBlocksLimits[1])
			{
				_ProcessedBlocksLimits[1] = x;
			}
			if (y < _ProcessedBlocksLimits[2])
			{
				_ProcessedBlocksLimits[2] = y;
			}
			if (y > _ProcessedBlocksLimits[3])
			{
				_ProcessedBlocksLimits[3] = y;
			}
			if (z < _ProcessedBlocksLimits[4])
			{
				_ProcessedBlocksLimits[4] = z;
			}
			if (z > _ProcessedBlocksLimits[5])
			{
				_ProcessedBlocksLimits[5] = z;
			}
			if (curDepth > maxDepth)
			{
				maxDepth = curDepth;
			}
			blocks++;
			if (curDepth <= 300)
			{
				CheckDepthRecursive(x, y, z - 1, curDepth + 1, ref maxDepth, ref blocks);
				CheckDepthRecursive(x, y, z + 1, curDepth + 1, ref maxDepth, ref blocks);
				CheckDepthRecursive(x + 1, y, z, curDepth + 1, ref maxDepth, ref blocks);
				CheckDepthRecursive(x - 1, y, z, curDepth + 1, ref maxDepth, ref blocks);
				CheckDepthRecursive(x, y + 1, z, curDepth + 1, ref maxDepth, ref blocks);
				CheckDepthRecursive(x, y - 1, z, curDepth + 1, ref maxDepth, ref blocks);
			}
		}
	}

	private static void DestroyBlocks(bool onlyWater)
	{
		for (int num = _ProcessedBlocksLimits[5]; num >= _ProcessedBlocksLimits[4]; num--)
		{
			for (int i = _ProcessedBlocksLimits[0]; i <= _ProcessedBlocksLimits[1]; i++)
			{
				for (int j = _ProcessedBlocksLimits[2]; j <= _ProcessedBlocksLimits[3]; j++)
				{
					if (_ProcessedBlocks[i, j, num])
					{
						BlockType blockType = WorldData.Instance.GetBlockType(i, j, num);
						if (blockType != BlockType.Air && (!onlyWater || blockType == BlockType.Water))
						{
							World.Instance.RemoveBlockAt(new IntVect(i, j, num), silent: true);
						}
					}
				}
			}
		}
	}

	private static void RevertProcessedBlocks()
	{
		if (_ProcessedBlocksLimits == null)
		{
			return;
		}
		for (int num = _ProcessedBlocksLimits[5]; num >= _ProcessedBlocksLimits[4]; num--)
		{
			for (int i = _ProcessedBlocksLimits[0]; i <= _ProcessedBlocksLimits[1]; i++)
			{
				for (int j = _ProcessedBlocksLimits[2]; j <= _ProcessedBlocksLimits[3]; j++)
				{
					_ProcessedBlocks[i, j, num] = false;
				}
			}
		}
		_ProcessedBlocksLimits = null;
	}
}
