using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
	public class MeshData
	{
		public readonly List<int> m_Indices = new List<int>();

		public readonly List<Vector2> m_Uvs = new List<Vector2>();

		public readonly List<Vector3> m_Vertices = new List<Vector3>();

		public readonly List<Color> m_Colors = new List<Color>();

		public bool IsEmpty => m_Vertices.Count == 0;
	}

	private byte[] m_BlockType;

	private int m_X;

	private int m_Y;

	private int m_Z;

	public readonly MeshData LandData = new MeshData();

	public readonly MeshData WaterData = new MeshData();

	public readonly MeshData GlassData = new MeshData();

	public bool WaterRegeneration;

	public bool LandRegeneration;

	public bool GlassRegeneration;

	public Transform LandChunk;

	public Transform WaterChunk;

	public Transform GlassChunk;

	public bool NeedsRegeneration;

	public bool NeedsWaterRegen;

	public bool NeedsGlassRegen;

	public readonly List<IntVect> TopSoilBlocks = new List<IntVect>();

	public int X => m_X;

	public int Y => m_Y;

	public int Z => m_Z;

	public static int WorldChunkYOffset { get; set; }

	public Chunk(int x, int y, int z)
	{
		m_X = x;
		m_Y = y;
		m_Z = z;
	}

	static Chunk()
	{
		WorldChunkYOffset = 0;
	}

	public void InitializeBlocks(int chunkBlockWidth, int chunkBlockHeight, int chunkBlockDepth)
	{
		m_BlockType = new byte[chunkBlockWidth * chunkBlockHeight * chunkBlockDepth];
	}

	public BlockType GetBlockType(int x, int y, int z)
	{
		if (m_BlockType == null)
		{
			return BlockType.Air;
		}
		int chunkBlockWidth = WorldData.Instance.ChunkBlockWidth;
		int chunkBlockHeight = WorldData.Instance.ChunkBlockHeight;
		return (BlockType)m_BlockType[x + y * chunkBlockWidth + z * chunkBlockWidth * chunkBlockHeight];
	}

	public void SetBlockType(BlockType type, int x, int y, int z)
	{
		if (m_BlockType != null)
		{
			int chunkBlockWidth = WorldData.Instance.ChunkBlockWidth;
			int chunkBlockHeight = WorldData.Instance.ChunkBlockHeight;
			m_BlockType[x + y * chunkBlockWidth + z * chunkBlockWidth * chunkBlockHeight] = (byte)type;
		}
	}

	public void SetBlocksBuffer(byte[] buffer)
	{
		m_BlockType = buffer;
	}

	public byte[] GetBlocksBuffer()
	{
		return m_BlockType;
	}

	public override string ToString()
	{
		return $"Chunk_{m_X},{m_Y},{m_Z}";
	}
}
