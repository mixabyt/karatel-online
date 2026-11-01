using System.Collections.Generic;
using UnityEngine;

public class TestMeshGenerator : MonoBehaviour
{
	private Mesh mesh;

	private List<Vector3> m_Vertices = new List<Vector3>();

	private List<int> m_Indices = new List<int>();

	private List<Vector2> m_Uvs = new List<Vector2>();

	private List<Color> m_Colors = new List<Color>();

	private readonly IntVect[][] offsets = new IntVect[6][]
	{
		new IntVect[4]
		{
			new IntVect(0, 0, 0),
			new IntVect(0, 0, 1),
			new IntVect(0, 1, 1),
			new IntVect(0, 1, 0)
		},
		new IntVect[4]
		{
			new IntVect(1, 1, 0),
			new IntVect(1, 1, 1),
			new IntVect(1, 0, 1),
			new IntVect(1, 0, 0)
		},
		new IntVect[4]
		{
			new IntVect(1, 0, 0),
			new IntVect(1, 0, 1),
			new IntVect(0, 0, 1),
			new IntVect(0, 0, 0)
		},
		new IntVect[4]
		{
			new IntVect(0, 1, 0),
			new IntVect(0, 1, 1),
			new IntVect(1, 1, 1),
			new IntVect(1, 1, 0)
		},
		new IntVect[4]
		{
			new IntVect(0, 0, 0),
			new IntVect(0, 1, 0),
			new IntVect(1, 1, 0),
			new IntVect(1, 0, 0)
		},
		new IntVect[4]
		{
			new IntVect(1, 0, 1),
			new IntVect(1, 1, 1),
			new IntVect(0, 1, 1),
			new IntVect(0, 0, 1)
		}
	};

	private void Start()
	{
		mesh = ((Component)this).GetComponent<MeshFilter>().mesh;
		CreateMesh();
		MeshApply();
	}

	private void CreateMesh()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		for (int i = 0; i < 6; i++)
		{
			Vector3[] array = new Vector3[4];
			BlockFaceStair blockFaceStair = (BlockFaceStair)i;
			switch (blockFaceStair)
			{
			case BlockFaceStair.Forward:
			case BlockFaceStair.Bottom:
			{
				for (int j = 0; j < 4; j++)
				{
					Vector3 val = offsets[i][j];
					array[j] = val;
				}
				AddBlockSide(array, num);
				num += 4;
				continue;
			}
			case BlockFaceStair.Back:
			case BlockFaceStair.Top:
			{
				Vector3 zero = Vector3.zero;
				Vector3 zero2 = Vector3.zero;
				int num2;
				int num3;
				int num4;
				int num5;
				if (blockFaceStair == BlockFaceStair.Back)
				{
					zero = new Vector3(0.5f, 0f, 0f);
					zero2 = new Vector3(0f, 0.5f, 0f);
					num2 = 0;
					num3 = 1;
					num4 = 2;
					num5 = 3;
				}
				else
				{
					zero = new Vector3(0f, -0.5f, 0f);
					zero2 = new Vector3(-0.5f, 0f, 0f);
					num2 = 2;
					num3 = 3;
					num4 = 0;
					num5 = 1;
				}
				for (int k = 0; k < 2; k++)
				{
					for (int l = 0; l < 4; l++)
					{
						Vector3 val2 = offsets[i][l];
						if (k == 0)
						{
							val2 += zero;
							if (l == num2 || l == num3)
							{
								val2 += zero2;
							}
						}
						else if (l == num4 || l == num5)
						{
							val2 += zero2 * -1f;
						}
						array[l] = val2;
					}
					AddBlockSide(array, num);
					num += 4;
				}
				continue;
			}
			}
			Vector3 val3 = new Vector3(-0.5f, 0f, 0f);
			Vector3 val4 = new Vector3(0f, -0.5f, 0f);
			int num6;
			int num7;
			int num8;
			int num9;
			if (blockFaceStair == BlockFaceStair.Right)
			{
				num6 = 2;
				num7 = 3;
				num8 = 1;
				num9 = 0;
			}
			else
			{
				num6 = 1;
				num7 = 0;
				num8 = 2;
				num9 = 3;
			}
			for (int m = 0; m < 2; m++)
			{
				for (int n = 0; n < 4; n++)
				{
					Vector3 val5 = offsets[i][n];
					if (m == 0)
					{
						if (n == num6 || n == num7)
						{
							val5 += val3;
						}
						if (n == num6 || n == num8)
						{
							val5 += val4;
						}
					}
					else if (n == num9 || n == num8)
					{
						val5 += val3 * -1f;
					}
					array[n] = val5;
				}
				AddBlockSide(array, num);
				num += 4;
			}
		}
	}

	private void MeshApply()
	{
		mesh.vertices = m_Vertices.ToArray();
		mesh.triangles = m_Indices.ToArray();
		mesh.uv = m_Uvs.ToArray();
		mesh.colors = m_Colors.ToArray();
		mesh.RecalculateNormals();
	}

	private void AddBlockSide(Vector3[] vert, int index)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		foreach (Vector3 item in vert)
		{
			m_Vertices.Add(item);
			m_Colors.Add(Color.white);
		}
		m_Indices.Add(index);
		m_Indices.Add(index + 1);
		m_Indices.Add(index + 2);
		m_Indices.Add(index + 2);
		m_Indices.Add(index + 3);
		m_Indices.Add(index);
		m_Uvs.Add(new Vector2(0f, 0f));
		m_Uvs.Add(new Vector2(1f, 0f));
		m_Uvs.Add(new Vector2(1f, 1f));
		m_Uvs.Add(new Vector2(0f, 1f));
	}
}
