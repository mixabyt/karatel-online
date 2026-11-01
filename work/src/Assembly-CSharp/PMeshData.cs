using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using hd;

public class PMeshData : HDPoolableObject
{
	private const int SizeStep = 64;

	private static readonly Dictionary<int, HDPool<PMeshData>> pools = new Dictionary<int, HDPool<PMeshData>>();

	private static readonly Dictionary<Mesh, PMeshData> inUse = new Dictionary<Mesh, PMeshData>();

	public readonly int QuadCount;

	private readonly int[] m_indices;

	private readonly Vector2[] m_uvs;

	private readonly Vector3[] m_vertices;

	private readonly Color[] m_colors;

	private bool isAcquired;

	private PMeshData(int alignedQuadCount)
	{
		QuadCount = alignedQuadCount;
		m_indices = new int[QuadCount * 6];
		m_uvs = new Vector2[QuadCount * 4];
		m_vertices = new Vector3[QuadCount * 4];
		m_colors = new Color[QuadCount * 4];
	}

	private static int AlignQuadCount(int quadCount)
	{
		return (quadCount % 64 != 0) ? (quadCount / 64 * 64 + 64) : quadCount;
	}

	public static PMeshData AcquireFor(Chunk.MeshData md)
	{
		PMeshData pMeshData = Acquire(md.m_Colors.Count / 4);
		pMeshData.AssignFrom(md);
		return pMeshData;
	}

	public static PMeshData Acquire(int quadCount)
	{
		quadCount = AlignQuadCount(quadCount);
		PMeshData pMeshData = null;
		if (!pools.ContainsKey(quadCount))
		{
			pools.Add(quadCount, new HDPool<PMeshData>());
		}
		pMeshData = pools[quadCount].Acquire();
		if (pMeshData != null && pMeshData.isAcquired)
		{
			throw new Exception("Object is already acquired!");
		}
		if (pMeshData == null)
		{
			pMeshData = new PMeshData(quadCount);
		}
		pMeshData.isAcquired = true;
		return pMeshData;
	}

	public void AssignFrom(Chunk.MeshData md)
	{
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		if (md.m_Colors.Count > m_colors.Length)
		{
			throw new Exception("Invalid PMeshData size");
		}
		int num = md.m_Vertices.Count;
		md.m_Indices.CopyTo(m_indices, 0);
		int num2 = md.m_Indices.Count;
		while (num2 < m_indices.Length)
		{
			m_indices[num2] = num;
			m_indices[num2 + 1] = num + 1;
			m_indices[num2 + 2] = num + 2;
			m_indices[num2 + 3] = num + 2;
			m_indices[num2 + 4] = num + 3;
			m_indices[num2 + 5] = num;
			num2 += 6;
			num += 4;
		}
		md.m_Uvs.CopyTo(m_uvs, 0);
		for (int i = md.m_Uvs.Count; i < m_uvs.Length; i++)
		{
			ref Vector2 reference = ref m_uvs[i];
			reference = Vector2.zero;
		}
		md.m_Vertices.CopyTo(m_vertices, 0);
		for (int j = md.m_Vertices.Count; j < m_vertices.Length; j++)
		{
			ref Vector3 reference2 = ref m_vertices[j];
			reference2 = md.m_Vertices[0];
		}
		md.m_Colors.CopyTo(m_colors, 0);
		for (int k = md.m_Colors.Count; k < m_colors.Length; k++)
		{
			ref Color reference3 = ref m_colors[k];
			reference3 = new Color(0f, 0f, 0f, 0f);
		}
	}

	public void ApplyTo(Mesh mesh)
	{
		PMeshData value = null;
		if (inUse.TryGetValue(mesh, out value))
		{
			inUse.Remove(mesh);
			value.Release();
		}
		else if (mesh.colors != null && mesh.colors.Length > 0)
		{
			Debug.LogError("PMeshData: Invalid data in ApplyTo");
		}
		inUse.Add(mesh, this);
		mesh.Clear();
		mesh.vertices = m_vertices;
		mesh.uv = m_uvs;
		mesh.colors = m_colors;
		mesh.triangles = m_indices;
	}

	public static void Reset()
	{
		Debug.LogWarning("PMeshData.Reset");
		foreach (PMeshData value in inUse.Values)
		{
			value.Release();
		}
		inUse.Clear();
	}

	public override void Release()
	{
		if (!isAcquired)
		{
			throw new Exception("Object is not acquired!");
		}
		if (!pools.ContainsKey(QuadCount))
		{
			Debug.LogError("PMeshData.Release called, but pools does not contain element for quadCount=" + QuadCount);
			return;
		}
		isAcquired = false;
		pools[QuadCount].Release(this);
	}

	public static string GetDebugInfo()
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<int, HDPool<PMeshData>> pool in pools)
		{
			stringBuilder.AppendFormat("PMeshData[{0}] InUse={1} Cached={2}", pool.Key, pool.Value.PoolCount, pool.Value.CachedCount);
			stringBuilder.AppendLine();
		}
		return stringBuilder.ToString();
	}
}
