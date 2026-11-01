using System.Collections.Generic;
using UnityEngine;

public class WorldData
{
	public static WorldData Instance;

	private int _ChunksWide = 8;

	private int _ChunksHigh = 8;

	private int _ChunksDeep = 8;

	private int _ChunkBlockWidth = 16;

	private int _ChunkBlockHeight = 16;

	private int _ChunkBlockDepth = 16;

	public int m_ChunkBufferLength;

	private float m_NoiseBlockXOffset = App.instance.random.Next(0, 10000);

	private readonly Dictionary<int, BlockUVCoordinates> m_BlockUVCoordinates = new Dictionary<int, BlockUVCoordinates>();

	public Dictionary<BlockType, Texture2D> BlockTextures = new Dictionary<BlockType, Texture2D>();

	private float m_GlobalXOffset;

	private float m_GlobalZOffset;

	public Chunk[,,] Chunks;

	public TQueue<Chunk> FinishedLandChunks = new TQueue<Chunk>();

	public TQueue<Chunk> FinishedWaterChunks = new TQueue<Chunk>();

	public TQueue<Chunk> FinishedGlassChunks = new TQueue<Chunk>();

	public int ChunksWide
	{
		get
		{
			return _ChunksWide;
		}
		set
		{
			_ChunksWide = value;
		}
	}

	public int ChunksHigh
	{
		get
		{
			return _ChunksHigh;
		}
		set
		{
			_ChunksHigh = value;
		}
	}

	public int ChunksDeep
	{
		get
		{
			return _ChunksDeep;
		}
		set
		{
			_ChunksDeep = value;
		}
	}

	public int ChunkBlockWidth
	{
		get
		{
			return _ChunkBlockWidth;
		}
		set
		{
			_ChunkBlockWidth = value;
		}
	}

	public int ChunkBlockHeight
	{
		get
		{
			return _ChunkBlockHeight;
		}
		set
		{
			_ChunkBlockHeight = value;
		}
	}

	public int ChunkBlockDepth
	{
		get
		{
			return _ChunkBlockDepth;
		}
		set
		{
			_ChunkBlockDepth = value;
		}
	}

	public int WidthInBlocks => _ChunksWide * _ChunkBlockWidth;

	public int HeightInBlocks => _ChunksHigh * _ChunkBlockHeight;

	public int DepthInBlocks => _ChunksDeep * _ChunkBlockDepth;

	public int TotalChunks => _ChunksWide * _ChunksHigh;

	public int CenterChunkX => _ChunksWide / 2;

	public int CenterChunkY => _ChunksHigh / 2;

	public PChunkList AllChunks
	{
		get
		{
			PChunkList pChunkList = PChunkList.Acquire();
			for (int i = 0; i < _ChunksWide; i++)
			{
				for (int j = 0; j < _ChunksHigh; j++)
				{
					for (int k = 0; k < _ChunksDeep; k++)
					{
						pChunkList.Add(Chunks[i, j, k]);
					}
				}
			}
			return pChunkList;
		}
	}

	public float GlobalXOffset
	{
		get
		{
			return m_GlobalXOffset;
		}
		set
		{
			m_GlobalXOffset = value;
		}
	}

	public float GlobalZOffset
	{
		get
		{
			return m_GlobalZOffset;
		}
		set
		{
			m_GlobalZOffset = value;
		}
	}

	public float NoiseBlockXOffset
	{
		get
		{
			return m_NoiseBlockXOffset;
		}
		set
		{
			m_NoiseBlockXOffset = value;
		}
	}

	public PChunkList ChunksNeedingWaterRegeneration
	{
		get
		{
			PChunkList pChunkList = PChunkList.Acquire();
			pChunkList.Clear();
			for (int i = 0; i < _ChunksWide; i++)
			{
				for (int j = 0; j < _ChunksHigh; j++)
				{
					for (int k = 0; k < _ChunksDeep; k++)
					{
						if (Chunks[i, j, k].NeedsWaterRegen)
						{
							pChunkList.Add(Chunks[i, j, k]);
						}
					}
				}
			}
			return pChunkList;
		}
	}

	public PChunkList ChunksNeedingGlassRegeneration
	{
		get
		{
			PChunkList pChunkList = PChunkList.Acquire();
			pChunkList.Clear();
			for (int i = 0; i < _ChunksWide; i++)
			{
				for (int j = 0; j < _ChunksHigh; j++)
				{
					for (int k = 0; k < _ChunksDeep; k++)
					{
						if (Chunks[i, j, k].NeedsGlassRegen)
						{
							pChunkList.Add(Chunks[i, j, k]);
						}
					}
				}
			}
			return pChunkList;
		}
	}

	public PChunkList ChunksNeedingLandRegeneration
	{
		get
		{
			PChunkList pChunkList = PChunkList.Acquire();
			pChunkList.Clear();
			for (int i = 0; i < _ChunksWide; i++)
			{
				for (int j = 0; j < _ChunksHigh; j++)
				{
					for (int k = 0; k < _ChunksDeep; k++)
					{
						if (Chunks[i, j, k].NeedsRegeneration)
						{
							pChunkList.Add(Chunks[i, j, k]);
						}
					}
				}
			}
			return pChunkList;
		}
	}

	public Rect[] WorldTextureAtlasUvs { get; set; }

	public Dictionary<int, BlockUVCoordinates> BlockUvCoordinates => m_BlockUVCoordinates;

	public WorldData()
	{
		Instance = this;
	}

	public void GenerateUVCoordinatesForAllBlocks()
	{
		SetBlockUVCoordinates(BlockType.TopSoil, 0, 5, 1);
		SetBlockUVCoordinates(BlockType.Dirt, 1, 1, 1);
		SetBlockUVCoordinates(BlockType.Leaves, 4, 4, 4);
		SetBlockUVCoordinates(BlockType.Lava, 3, 3, 3);
		SetBlockUVCoordinates(BlockType.Stone, 6, 6, 6);
		SetBlockUVCoordinates(BlockType.Wood, 2, 7, 2);
		SetBlockUVCoordinates(BlockType.Brick, 9);
		SetBlockUVCoordinates(BlockType.WoodPlank, 10);
		SetBlockUVCoordinates(BlockType.Brick2, 11);
		SetBlockUVCoordinates(BlockType.WoodPlank2, 12);
		SetBlockUVCoordinates(BlockType.WoodPlank3, 13);
		SetBlockUVCoordinates(BlockType.WoodPlank4, 14);
		SetBlockUVCoordinates(BlockType.Brick3, 8);
		SetBlockUVCoordinates(BlockType.Brick4, 25);
		SetBlockUVCoordinates(BlockType.Brick5, 26);
		SetBlockUVCoordinates(BlockType.Brick6, 27);
		SetBlockUVCoordinates(BlockType.Brick7, 29);
		SetBlockUVCoordinates(BlockType.WoodPlank5, 28);
		SetBlockUVCoordinates(BlockType.Colored1, 16);
		SetBlockUVCoordinates(BlockType.Colored2, 17);
		SetBlockUVCoordinates(BlockType.Colored3, 18);
		SetBlockUVCoordinates(BlockType.Colored4, 19);
		SetBlockUVCoordinates(BlockType.Colored5, 20);
		SetBlockUVCoordinates(BlockType.Colored6, 21);
		SetBlockUVCoordinates(BlockType.Colored7, 22);
		SetBlockUVCoordinates(BlockType.Colored8, 23);
		SetBlockUVCoordinates(BlockType.Colored9, 24);
		SetBlockUVCoordinates(BlockType.Stone17, 30);
		SetBlockUVCoordinates(BlockType.Stone17_1, 31);
		SetBlockUVCoordinates(BlockType.Stone17_2, 32);
		SetBlockUVCoordinates(BlockType.Stone9, 33);
		SetBlockUVCoordinates(BlockType.Stone9_2, 34);
		SetBlockUVCoordinates(BlockType.Stone10, 35);
		SetBlockUVCoordinates(BlockType.Stone10_1, 36);
		SetBlockUVCoordinates(BlockType.Stone14, 37);
		SetBlockUVCoordinates(BlockType.Stone14_1, 38);
		SetBlockUVCoordinates(BlockType.Stone14_2, 39);
		SetBlockUVCoordinates(BlockType.Stone14_3, 40);
		SetBlockUVCoordinates(BlockType.Stone14_4, 41);
		SetBlockUVCoordinates(BlockType.Stone4_1, 42);
		SetBlockUVCoordinates(BlockType.Stone4_2, 43);
		SetBlockUVCoordinates(BlockType.Stone15, 44);
		SetBlockUVCoordinates(BlockType.Stone15_1, 45);
		SetBlockUVCoordinates(BlockType.Stone15_2, 46);
		SetBlockUVCoordinates(BlockType.Stone15_3, 47);
		SetBlockUVCoordinates(BlockType.Stone16, 48);
		SetBlockUVCoordinates(BlockType.Stone16_1, 49);
		SetBlockUVCoordinates(BlockType.Stone16_2, 50);
		SetBlockUVCoordinates(BlockType.Stone16_3, 51);
		SetBlockUVCoordinates(BlockType.Stone18, 52);
		SetBlockUVCoordinates(BlockType.Stone18_1, 53);
		SetBlockUVCoordinates(BlockType.Stone18_2, 54);
		SetBlockUVCoordinates(BlockType.Stone19, 55);
		SetBlockUVCoordinates(BlockType.Stone19_1, 56);
		SetBlockUVCoordinates(BlockType.Stone19_2, 57);
		SetBlockUVCoordinates(BlockType.Coal, 58);
		SetBlockUVCoordinates(BlockType.Iron, 59);
		SetBlockUVCoordinates(BlockType.Sand, 60);
		SetBlockUVCoordinates(BlockType.Gold, 61);
		SetBlockUVCoordinates(BlockType.Crates, 62);
		SetBlockUVCoordinates(BlockType.Wood8, 63);
		SetBlockUVCoordinates(BlockType.Wood9, 64);
		SetBlockUVCoordinates(BlockType.Wood10, 65);
		SetBlockUVCoordinates(BlockType.Wood11, 66);
		SetBlockUVCoordinates(BlockType.Glass1, 67);
		SetBlockUVCoordinates(BlockType.Glass2, 68);
		SetBlockUVCoordinates(BlockType.Glass3, 69);
		SetBlockUVCoordinates(BlockType.Glass4, 70);
		SetBlockUVCoordinates(BlockType.Glass5, 71);
		SetBlockUVCoordinates(BlockType.Glass6, 72);
		SetBlockUVCoordinates(BlockType.Glass7, 73);
		SetBlockUVCoordinates(BlockType.Glass8, 74);
		SetBlockUVCoordinates(BlockType.Glass9, 75);
		SetBlockUVCoordinates(BlockType.Reshetka1, 76);
		SetBlockUVCoordinates(BlockType.Reshetka2, 77);
		SetBlockUVCoordinates(BlockType.Reshetka3, 78);
		SetBlockUVCoordinates(BlockType.Reshetka4, 79);
		SetBlockUVCoordinates(BlockType.Reshetka5, 80);
		SetBlockUVCoordinates(BlockType.Water, 81);
		SetBlockUVCoordinates(BlockType.Snow, 82);
		SetBlockUVCoordinates(BlockType.SnowDirt, 82, 83, 1);
		SetBlockUVCoordinates(BlockType.Ice, 84);
	}

	private void SetBlockUVCoordinates(BlockType blockType, int topIndex, int sideIndex, int bottomIndex)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		BlockUvCoordinates.Add((int)blockType, new BlockUVCoordinates(WorldTextureAtlasUvs[topIndex], WorldTextureAtlasUvs[sideIndex], WorldTextureAtlasUvs[bottomIndex]));
		BlockTextures.Add(blockType, WorldGameObject.INSTANCE.World_Textures[sideIndex]);
	}

	private void SetBlockUVCoordinates(BlockType blockType, int Index)
	{
		SetBlockUVCoordinates(blockType, Index, Index, Index);
	}

	public int GetMaxBlockX()
	{
		return _ChunksWide * _ChunkBlockWidth - 1;
	}

	public int GetMaxBlockY()
	{
		return _ChunksHigh * _ChunkBlockHeight - 1;
	}

	public void SetDimensions(int chunksWide, int chunksHigh, int chunksDeep, int chunkBlockWidth, int chunkBlockHeight, int chunkBlockDepth)
	{
		ChunksWide = chunksWide;
		ChunksHigh = chunksHigh;
		ChunksDeep = chunksDeep;
		ChunkBlockWidth = chunkBlockWidth;
		ChunkBlockHeight = chunkBlockHeight;
		ChunkBlockDepth = chunkBlockDepth;
		m_ChunkBufferLength = _ChunkBlockDepth * _ChunkBlockHeight * _ChunkBlockWidth;
	}

	public void InitializeGridChunks()
	{
		Chunks = new Chunk[_ChunksWide, _ChunksHigh, _ChunksDeep];
		for (int i = 0; i < _ChunksWide; i++)
		{
			for (int j = 0; j < _ChunksHigh; j++)
			{
				for (int k = 0; k < _ChunksDeep; k++)
				{
					Chunks[i, j, k] = new Chunk(i, j, k);
					Chunks[i, j, k].InitializeBlocks(ChunkBlockWidth, ChunkBlockHeight, ChunkBlockDepth);
				}
			}
		}
	}

	public Chunk GetChunk(int x, int y, int z)
	{
		int num = x / ChunkBlockWidth;
		int num2 = y / ChunkBlockHeight;
		int num3 = z / ChunkBlockDepth;
		return Chunks[num, num2, num3];
	}

	public Chunk GetChunkSafe(int x, int y, int z)
	{
		int num = x / ChunkBlockWidth;
		int num2 = y / ChunkBlockHeight;
		int num3 = z / ChunkBlockDepth;
		if (num < 0 || num > Chunks.GetUpperBound(0) || num2 < 0 || num2 > Chunks.GetUpperBound(1) || num3 < 0 || num3 > Chunks.GetUpperBound(2))
		{
			return null;
		}
		return Chunks[num, num2, num3];
	}

	public Vector3 GetChunkInd(int x, int y, int z)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		int num = x / ChunkBlockWidth;
		int num2 = y / ChunkBlockHeight;
		int num3 = z / ChunkBlockDepth;
		return new Vector3((float)num, (float)num2, (float)num3);
	}

	public void SetBlockTypeWithLightRegeneration(int x, int y, int z, BlockType blockType)
	{
		int num = x / ChunkBlockWidth;
		int num2 = y / ChunkBlockHeight;
		int num3 = z / ChunkBlockDepth;
		int num4 = x % ChunkBlockWidth;
		int num5 = y % ChunkBlockHeight;
		int num6 = z % ChunkBlockDepth;
		Chunks[num, num2, num3].SetBlockType(blockType, num4, num5, num6);
		SetBlockTypeRegeneration(blockType, num, num2, num3, num4, num5, num6);
	}

	public void SetBlockTypeWithRegeneration(int x, int y, int z, BlockType blockType)
	{
		int num = x / ChunkBlockWidth;
		int num2 = y / ChunkBlockHeight;
		int num3 = z / ChunkBlockDepth;
		int num4 = x % ChunkBlockWidth;
		int num5 = y % ChunkBlockHeight;
		int num6 = z % ChunkBlockDepth;
		BlockType blockType2 = Chunks[num, num2, num3].GetBlockType(num4, num5, num6);
		Chunks[num, num2, num3].SetBlockType(blockType, num4, num5, num6);
		SetBlockTypeRegeneration(blockType2, num, num2, num3, num4, num5, num6);
	}

	private void SetBlockTypeRegeneration(BlockType blockType, int chunkX, int chunkY, int chunkZ, int blockX, int blockY, int blockZ)
	{
		if (WorldGameObject.INSTANCE.BlockParametrs[(uint)blockType].is_glass)
		{
			Chunks[chunkX, chunkY, chunkZ].NeedsGlassRegen = true;
		}
		else if (blockType == BlockType.Water)
		{
			Chunks[chunkX, chunkY, chunkZ].NeedsWaterRegen = true;
		}
		else
		{
			Chunks[chunkX, chunkY, chunkZ].NeedsRegeneration = true;
		}
		if (blockX == 0 && chunkX > 0)
		{
			if (WorldGameObject.INSTANCE.BlockParametrs[(uint)blockType].is_glass)
			{
				Chunks[chunkX - 1, chunkY, chunkZ].NeedsGlassRegen = true;
			}
			else if (blockType == BlockType.Water)
			{
				Chunks[chunkX - 1, chunkY, chunkZ].NeedsWaterRegen = true;
			}
			else
			{
				Chunks[chunkX - 1, chunkY, chunkZ].NeedsRegeneration = true;
			}
		}
		else if (blockX == ChunkBlockWidth - 1 && chunkX < ChunksWide - 1)
		{
			if (WorldGameObject.INSTANCE.BlockParametrs[(uint)blockType].is_glass)
			{
				Chunks[chunkX + 1, chunkY, chunkZ].NeedsGlassRegen = true;
			}
			else if (blockType == BlockType.Water)
			{
				Chunks[chunkX + 1, chunkY, chunkZ].NeedsWaterRegen = true;
			}
			else
			{
				Chunks[chunkX + 1, chunkY, chunkZ].NeedsRegeneration = true;
			}
		}
		if (blockY == 0 && chunkY > 0)
		{
			if (WorldGameObject.INSTANCE.BlockParametrs[(uint)blockType].is_glass)
			{
				Chunks[chunkX, chunkY - 1, chunkZ].NeedsGlassRegen = true;
			}
			else if (blockType == BlockType.Water)
			{
				Chunks[chunkX, chunkY - 1, chunkZ].NeedsWaterRegen = true;
			}
			else
			{
				Chunks[chunkX, chunkY - 1, chunkZ].NeedsRegeneration = true;
			}
		}
		else if (blockY == ChunkBlockHeight - 1 && chunkY < ChunksHigh - 1)
		{
			if (WorldGameObject.INSTANCE.BlockParametrs[(uint)blockType].is_glass)
			{
				Chunks[chunkX, chunkY + 1, chunkZ].NeedsGlassRegen = true;
			}
			else if (blockType == BlockType.Water)
			{
				Chunks[chunkX, chunkY + 1, chunkZ].NeedsWaterRegen = true;
			}
			else
			{
				Chunks[chunkX, chunkY + 1, chunkZ].NeedsRegeneration = true;
			}
		}
		if (blockZ == 0 && chunkZ > 0)
		{
			if (WorldGameObject.INSTANCE.BlockParametrs[(uint)blockType].is_glass)
			{
				Chunks[chunkX, chunkY, chunkZ - 1].NeedsGlassRegen = true;
			}
			else if (blockType == BlockType.Water)
			{
				Chunks[chunkX, chunkY, chunkZ - 1].NeedsWaterRegen = true;
			}
			else
			{
				Chunks[chunkX, chunkY, chunkZ - 1].NeedsRegeneration = true;
			}
		}
		else if (blockZ == ChunkBlockDepth - 1 && chunkZ < ChunksDeep - 1)
		{
			if (WorldGameObject.INSTANCE.BlockParametrs[(uint)blockType].is_glass)
			{
				Chunks[chunkX, chunkY, chunkZ + 1].NeedsGlassRegen = true;
			}
			else if (blockType == BlockType.Water)
			{
				Chunks[chunkX, chunkY, chunkZ + 1].NeedsWaterRegen = true;
			}
			else
			{
				Chunks[chunkX, chunkY, chunkZ + 1].NeedsRegeneration = true;
			}
		}
	}

	public void SetBlockType(int x, int y, int z, BlockType blockType)
	{
		if (x >= 0 && y >= 0 && z >= 0 && x < WidthInBlocks && y < HeightInBlocks && z < DepthInBlocks)
		{
			int num = x / ChunkBlockWidth;
			int num2 = y / ChunkBlockHeight;
			int num3 = z / ChunkBlockDepth;
			int x2 = x % ChunkBlockWidth;
			int y2 = y % ChunkBlockHeight;
			int z2 = z % ChunkBlockDepth;
			Chunks[num, num2, num3].SetBlockType(blockType, x2, y2, z2);
		}
	}

	public BlockType GetBlockType(int x, int y, int z)
	{
		if (x < 0 || y < 0 || z < 0 || x >= WidthInBlocks || y >= HeightInBlocks || z >= DepthInBlocks)
		{
			return BlockType.Air;
		}
		int num = x / ChunkBlockWidth;
		int num2 = y / ChunkBlockHeight;
		int num3 = z / ChunkBlockDepth;
		int x2 = x % ChunkBlockWidth;
		int y2 = y % ChunkBlockHeight;
		int z2 = z % ChunkBlockDepth;
		return Chunks[num, num2, num3].GetBlockType(x2, y2, z2);
	}

	public bool IsValidBlock(int x, int y, int z)
	{
		return x >= 0 && y >= 0 && z >= 0 && x < WidthInBlocks && y < HeightInBlocks && z < DepthInBlocks;
	}

	public void SetChunkWaterRegen(int x, int y, int z)
	{
		Chunk chunkSafe = GetChunkSafe(x, y, z);
		if (chunkSafe != null)
		{
			chunkSafe.NeedsWaterRegen = true;
		}
	}

	public void AddFinishedLandChunk(Chunk chunk)
	{
		FinishedLandChunks.Enqueue(chunk);
	}

	public void AddFinishedWaterChunk(Chunk chunk)
	{
		FinishedWaterChunks.Enqueue(chunk);
	}

	public void AddFinishedGlassChunk(Chunk chunk)
	{
		FinishedGlassChunks.Enqueue(chunk);
	}

	public Chunk GetFinishedLandChunk()
	{
		if (FinishedLandChunks.Count == 0)
		{
			return null;
		}
		return FinishedLandChunks.Dequeue();
	}

	public Chunk GetFinishedWaterChunk()
	{
		if (FinishedWaterChunks.Count == 0)
		{
			return null;
		}
		return FinishedWaterChunks.Dequeue();
	}

	public Chunk GetFinishedGlassChunk()
	{
		if (FinishedGlassChunks.Count == 0)
		{
			return null;
		}
		return FinishedGlassChunks.Dequeue();
	}
}
