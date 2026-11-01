using System.Collections.Generic;
using UnityEngine;

public class Pathfinder
{
	private class PathNode
	{
		public int BlockX;

		public int BlockY;

		public int BlockZ;

		public List<PathNode> Child;

		public PathNode Parent;

		private bool _Processed;

		private float _DistanceToTarget;

		public PathNode(int x, int y, int z, PathNode parent, Vector3 targetPosition)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			BlockX = x;
			BlockY = y;
			BlockZ = z;
			Child = new List<PathNode>();
			Parent = parent;
			_Processed = false;
			_DistanceToTarget = Vector3.Distance(GetPosition(), targetPosition);
		}

		public void AddChild(int x, int y, int z, Vector3 targetPosition)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			Child.Add(new PathNode(x, y, z, this, targetPosition));
		}

		public PathNode GetNextToProcess()
		{
			PathNode bestNode = null;
			float bestDistance = 0f;
			GetNextToProcessInternal(ref bestNode, ref bestDistance);
			if (bestNode != null)
			{
				bestNode._Processed = true;
			}
			return bestNode;
		}

		private void GetNextToProcessInternal(ref PathNode bestNode, ref float bestDistance)
		{
			if (!_Processed && (bestNode == null || _DistanceToTarget < bestDistance))
			{
				bestNode = this;
				bestDistance = _DistanceToTarget;
			}
			foreach (PathNode item in Child)
			{
				item.GetNextToProcessInternal(ref bestNode, ref bestDistance);
			}
		}

		public Vector3 GetPosition()
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			return new Vector3((float)BlockX + 0.5f, (float)BlockZ, (float)BlockY + 0.5f);
		}
	}

	private static int[,,] _PathBlockProcessed;

	private static int _PathBlockProcessedIndex;

	private static List<int[]> _NodeOffsets;

	private static void GenerateNodeOffsets()
	{
		_NodeOffsets = new List<int[]>();
		for (int i = -1; i <= 1; i++)
		{
			for (int j = -1; j <= 1; j++)
			{
				for (int k = -1; k <= 1; k++)
				{
					if (k < 0 || i != 0 || j != 0)
					{
						_NodeOffsets.Add(new int[3] { i, j, k });
					}
				}
			}
		}
		_NodeOffsets.Add(new int[3] { 2, 0, 0 });
		_NodeOffsets.Add(new int[3] { -2, 0, 0 });
		_NodeOffsets.Add(new int[3] { 0, 2, 0 });
		_NodeOffsets.Add(new int[3] { 0, -2, 0 });
	}

	public static List<Vector3> Find(Vector3 startPosition, Vector3 targetPosition)
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		IntVect intVect = new IntVect((int)startPosition.x, (int)startPosition.z, (int)startPosition.y);
		IntVect intVect2 = new IntVect((int)targetPosition.x, (int)targetPosition.z, (int)targetPosition.y);
		if (intVect == intVect2)
		{
			List<Vector3> list = new List<Vector3>();
			list.Add(targetPosition);
			return list;
		}
		PathNode pathNode = new PathNode(intVect.X, intVect.Y, intVect.Z, null, targetPosition);
		int widthInBlocks = WorldData.Instance.WidthInBlocks;
		int heightInBlocks = WorldData.Instance.HeightInBlocks;
		int depthInBlocks = WorldData.Instance.DepthInBlocks;
		if (_PathBlockProcessed == null)
		{
			_PathBlockProcessed = new int[widthInBlocks, heightInBlocks, depthInBlocks];
		}
		_PathBlockProcessedIndex++;
		_PathBlockProcessed[pathNode.BlockX, pathNode.BlockY, pathNode.BlockZ] = _PathBlockProcessedIndex;
		if (_NodeOffsets == null)
		{
			GenerateNodeOffsets();
		}
		PathNode pathNode2 = null;
		for (int num = 100; num > 0; num--)
		{
			if (pathNode2 == null)
			{
				pathNode2 = pathNode.GetNextToProcess();
			}
			if (pathNode2 == null)
			{
				return null;
			}
			for (int i = 0; i < _NodeOffsets.Count; i++)
			{
				int num2 = pathNode2.BlockX + _NodeOffsets[i][0];
				int num3 = pathNode2.BlockY + _NodeOffsets[i][1];
				int num4 = pathNode2.BlockZ + _NodeOffsets[i][2];
				if (num2 < 0 || num2 >= widthInBlocks || num3 < 0 || num3 >= heightInBlocks || num4 <= 0 || num4 >= depthInBlocks || _PathBlockProcessed[num2, num3, num4] == _PathBlockProcessedIndex)
				{
					continue;
				}
				if (i >= _NodeOffsets.Count - 4)
				{
					int x = pathNode2.BlockX + _NodeOffsets[i][0] / 2;
					int y = pathNode2.BlockY + _NodeOffsets[i][1] / 2;
					if (WorldData.Instance.GetBlockType(x, y, num4) != BlockType.Air || WorldData.Instance.GetBlockType(x, y, num4 - 1) != BlockType.Air)
					{
						continue;
					}
				}
				_PathBlockProcessed[num2, num3, num4] = _PathBlockProcessedIndex;
				if (num2 == intVect2.X && num3 == intVect2.Y && num4 == intVect2.Z)
				{
					List<Vector3> list2 = new List<Vector3>();
					list2.Add(targetPosition);
					while (pathNode2.Parent != null)
					{
						list2.Add(pathNode2.GetPosition());
						pathNode2 = pathNode2.Parent;
					}
					list2.Reverse();
					return list2;
				}
				if (WorldData.Instance.GetBlockType(num2, num3, num4) == BlockType.Air)
				{
					BlockType blockType = WorldData.Instance.GetBlockType(num2, num3, num4 - 1);
					if (blockType != BlockType.Air && blockType != BlockType.Water && blockType != BlockType.Lava)
					{
						pathNode2.AddChild(num2, num3, num4, targetPosition);
					}
				}
			}
			pathNode2 = pathNode2.GetNextToProcess();
		}
		return null;
	}

	public static List<Vector3> FindRandom(Vector3 startPosition, Vector3 center, float distance)
	{
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		IntVect intVect = new IntVect((int)center.x, (int)center.z, (int)center.y);
		int num = Mathf.CeilToInt(distance);
		int num2 = Mathf.Max(intVect.X - num, 0);
		int num3 = Mathf.Min(intVect.X + num, WorldData.Instance.WidthInBlocks - 1);
		int num4 = Mathf.Max(intVect.Y - num, 0);
		int num5 = Mathf.Min(intVect.Y + num, WorldData.Instance.HeightInBlocks - 1);
		IntVect intVect2 = default;
		for (int i = 0; i < 5; i++)
		{
			int x = Random.Range(num2, num3);
			int y = Random.Range(num4, num5);
			bool flag = false;
			for (int j = 0; j < num * 2; j++)
			{
				int num6 = intVect.Z + ((j <= num) ? j : (-(j - num)));
				if (num6 >= 0 && num6 < WorldData.Instance.DepthInBlocks && WorldData.Instance.GetBlockType(x, y, num6) == BlockType.Air)
				{
					BlockType blockType = WorldData.Instance.GetBlockType(x, y, num6 - 1);
					if (blockType != BlockType.Air && blockType != BlockType.Water && blockType != BlockType.Lava)
					{
						intVect2 = new IntVect(x, y, num6);
						j = num * 2;
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				Vector3 targetPosition = new Vector3((float)intVect2.X, (float)intVect2.Z, (float)intVect2.Y);
				targetPosition.x += 0.5f;
				targetPosition.z += 0.5f;
				List<Vector3> list = Find(startPosition, targetPosition);
				if (list != null)
				{
					return list;
				}
			}
		}
		return null;
	}
}
