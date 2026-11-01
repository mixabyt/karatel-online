using UnityEngine;

public class NewMeshGenerator : IMeshGenerator
{
	private enum GenerateType
	{
		Land,
		Water,
		Glass
	}

	private delegate bool GenerateCondition(BlockType block);

	private readonly IBatchProcessor<Chunk> batchProcessor;

	private readonly WorldData worldData;

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

	private readonly Vector3[][] offsetsFence = new Vector3[6][]
	{
		new Vector3[4]
		{
			new Vector3(-0.35f, 0.35f, 0f),
			new Vector3(-0.35f, 0.35f, 1f),
			new Vector3(-0.35f, 0.65f, 1f),
			new Vector3(-0.35f, 0.65f, 0f)
		},
		new Vector3[4]
		{
			new Vector3(1.35f, 0.65f, 0f),
			new Vector3(1.35f, 0.65f, 1f),
			new Vector3(1.35f, 0.35f, 1f),
			new Vector3(1.35f, 0.35f, 0f)
		},
		new Vector3[4]
		{
			new Vector3(0.65f, -0.35f, 0f),
			new Vector3(0.65f, -0.35f, 1f),
			new Vector3(0.35f, -0.35f, 1f),
			new Vector3(0.35f, -0.35f, 0f)
		},
		new Vector3[4]
		{
			new Vector3(0.35f, 1.35f, 0f),
			new Vector3(0.35f, 1.35f, 1f),
			new Vector3(0.65f, 1.35f, 1f),
			new Vector3(0.65f, 1.35f, 0f)
		},
		new Vector3[4]
		{
			new Vector3(0.35f, 0.35f, 0f),
			new Vector3(0.35f, 0.65f, 0f),
			new Vector3(0.65f, 0.65f, 0f),
			new Vector3(0.65f, 0.35f, 0f)
		},
		new Vector3[4]
		{
			new Vector3(0.65f, 0.35f, 1f),
			new Vector3(0.65f, 0.65f, 1f),
			new Vector3(0.35f, 0.65f, 1f),
			new Vector3(0.35f, 0.35f, 1f)
		}
	};

	private readonly GenerateCondition[] generateTypeAssociate = new GenerateCondition[3]
	{
		BlockComparer.IsLand,
		BlockComparer.IsWater,
		BlockComparer.IsGlass
	};

	public NewMeshGenerator(IBatchProcessor<Chunk> batchProcessor, WorldData worldData)
	{
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0402: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0470: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0514: Unknown result type (might be due to invalid IL or missing references)
		//IL_0519: Unknown result type (might be due to invalid IL or missing references)
		//IL_0534: Unknown result type (might be due to invalid IL or missing references)
		//IL_0539: Unknown result type (might be due to invalid IL or missing references)
		//IL_0554: Unknown result type (might be due to invalid IL or missing references)
		//IL_0559: Unknown result type (might be due to invalid IL or missing references)
		this.batchProcessor = batchProcessor;
		this.worldData = worldData;
	}

	public void GenerateLandMeshes(PChunkList chunks)
	{
		batchProcessor.Process(1, chunks, GenerateLandMesh, waitUntilAllThreadsFinish: false);
	}

	public void GenerateLandMesh(Chunk chunk)
	{
		GenerateMesh(chunk.LandData, chunk, GenerateType.Land);
		worldData.AddFinishedLandChunk(chunk);
	}

	public void GenerateWaterMeshes(PChunkList chunks)
	{
		batchProcessor.Process(1, chunks, GenerateWaterMesh, waitUntilAllThreadsFinish: false);
	}

	public void GenerateWaterMesh(Chunk chunk)
	{
		GenerateMesh(chunk.WaterData, chunk, GenerateType.Water);
		worldData.AddFinishedWaterChunk(chunk);
	}

	public void GenerateGlassMeshes(PChunkList chunks)
	{
		batchProcessor.Process(1, chunks, GenerateGlassMesh, waitUntilAllThreadsFinish: false);
	}

	public void GenerateGlassMesh(Chunk chunk)
	{
		GenerateMesh(chunk.GlassData, chunk, GenerateType.Glass);
		worldData.AddFinishedGlassChunk(chunk);
	}

	public Chunk.MeshData[] GenerateMeshData(int x1, int x2, int y1, int y2, int z1, int z2, BlockGetter getter, BlockKindGetter kindGetter)
	{
		Chunk.MeshData[] array = new Chunk.MeshData[3]
		{
			new Chunk.MeshData(),
			new Chunk.MeshData(),
			new Chunk.MeshData()
		};
		int[] array2 = new int[3];
		for (int i = x1; i <= x2; i++)
		{
			for (int j = y1; j <= y2; j++)
			{
				for (int k = z1; k <= z2; k++)
				{
					IntVect intVect = new IntVect(i, j, k);
					for (int l = 0; l < 3; l++)
					{
						array2[l] = CreateMeshData(intVect, intVect, null, array[l], array2[l], getter, kindGetter, (GenerateType)l);
					}
				}
			}
		}
		return array;
	}

	private void ClearData(Chunk.MeshData mData)
	{
		mData.m_Vertices.Clear();
		mData.m_Indices.Clear();
		mData.m_Uvs.Clear();
		mData.m_Colors.Clear();
	}

	private void GenerateMesh(Chunk.MeshData meshData, Chunk chunk, GenerateType typeData)
	{
		int index = 0;
		ClearData(meshData);
		IntVect intVect = chunk.Position.MultiplyComponent(worldData.ChunkBlockVolume);
		for (int i = 0; i < worldData.ChunkBlockWidth; i++)
		{
			for (int j = 0; j < worldData.ChunkBlockHeight; j++)
			{
				for (int k = 0; k < worldData.ChunkBlockDepth; k++)
				{
					IntVect intVect2 = new IntVect(i, j, k);
					IntVect block = intVect2 + intVect;
					index = CreateMeshData(block, intVect2, chunk, meshData, index, null, null, typeData);
				}
			}
		}
	}

	private int CreateMeshData(IntVect block, IntVect chunkBlock, Chunk chunk, Chunk.MeshData data, int index, BlockGetter getter, BlockKindGetter kindGetter, GenerateType generateType)
	{
		//IL_051e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0523: Unknown result type (might be due to invalid IL or missing references)
		//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_05af: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0614: Unknown result type (might be due to invalid IL or missing references)
		//IL_0619: Unknown result type (might be due to invalid IL or missing references)
		//IL_0622: Unknown result type (might be due to invalid IL or missing references)
		//IL_0627: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ccf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ceb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0907: Unknown result type (might be due to invalid IL or missing references)
		//IL_090c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0915: Unknown result type (might be due to invalid IL or missing references)
		//IL_091a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d4b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d5c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d61: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d66: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cfb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e1f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_066a: Unknown result type (might be due to invalid IL or missing references)
		//IL_066f: Unknown result type (might be due to invalid IL or missing references)
		//IL_107e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1083: Unknown result type (might be due to invalid IL or missing references)
		//IL_1085: Unknown result type (might be due to invalid IL or missing references)
		//IL_108a: Unknown result type (might be due to invalid IL or missing references)
		//IL_10c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_10c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d3f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d44: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d1e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ddc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ded: Unknown result type (might be due to invalid IL or missing references)
		//IL_0df2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0df7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d9b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db6: Unknown result type (might be due to invalid IL or missing references)
		//IL_067f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0681: Unknown result type (might be due to invalid IL or missing references)
		//IL_0683: Unknown result type (might be due to invalid IL or missing references)
		//IL_0688: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f92: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fa3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fa8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e72: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e83: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e88: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e8d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fd4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fe5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eb4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ec5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ecf: Unknown result type (might be due to invalid IL or missing references)
		//IL_071a: Unknown result type (might be due to invalid IL or missing references)
		//IL_071c: Unknown result type (might be due to invalid IL or missing references)
		//IL_071e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0723: Unknown result type (might be due to invalid IL or missing references)
		//IL_089f: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_1016: Unknown result type (might be due to invalid IL or missing references)
		//IL_1027: Unknown result type (might be due to invalid IL or missing references)
		//IL_102c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ef6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f07: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f11: Unknown result type (might be due to invalid IL or missing references)
		//IL_0804: Unknown result type (might be due to invalid IL or missing references)
		//IL_0806: Unknown result type (might be due to invalid IL or missing references)
		//IL_0808: Unknown result type (might be due to invalid IL or missing references)
		//IL_080d: Unknown result type (might be due to invalid IL or missing references)
		//IL_086d: Unknown result type (might be due to invalid IL or missing references)
		//IL_086f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0876: Unknown result type (might be due to invalid IL or missing references)
		//IL_087b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0880: Unknown result type (might be due to invalid IL or missing references)
		//IL_073b: Unknown result type (might be due to invalid IL or missing references)
		//IL_073d: Unknown result type (might be due to invalid IL or missing references)
		//IL_073f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0744: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0abc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a54: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a56: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a58: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a26: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a30: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_09da: Unknown result type (might be due to invalid IL or missing references)
		//IL_09df: Unknown result type (might be due to invalid IL or missing references)
		//IL_1060: Unknown result type (might be due to invalid IL or missing references)
		//IL_1071: Unknown result type (might be due to invalid IL or missing references)
		//IL_1076: Unknown result type (might be due to invalid IL or missing references)
		//IL_107b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f40: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f51: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f56: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f5b: Unknown result type (might be due to invalid IL or missing references)
		//IL_083d: Unknown result type (might be due to invalid IL or missing references)
		//IL_083f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0846: Unknown result type (might be due to invalid IL or missing references)
		//IL_084b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0850: Unknown result type (might be due to invalid IL or missing references)
		//IL_080f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0811: Unknown result type (might be due to invalid IL or missing references)
		//IL_0813: Unknown result type (might be due to invalid IL or missing references)
		//IL_0818: Unknown result type (might be due to invalid IL or missing references)
		//IL_088a: Unknown result type (might be due to invalid IL or missing references)
		//IL_088c: Unknown result type (might be due to invalid IL or missing references)
		//IL_088e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0893: Unknown result type (might be due to invalid IL or missing references)
		//IL_077b: Unknown result type (might be due to invalid IL or missing references)
		//IL_077d: Unknown result type (might be due to invalid IL or missing references)
		//IL_077f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0784: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a75: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a77: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a79: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7e: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a00: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b2: Unknown result type (might be due to invalid IL or missing references)
		if (getter == null)
		{
			IntVect right = worldData.ChunkBlockVolume - IntVect.One;
			bool isEdge = chunkBlock.CompareOr(IntVect.Zero) || chunkBlock.CompareOr(right);
			IntVect delta = block - chunkBlock;
			getter = (int x, int y, int z) => (!isEdge) ? chunk.GetBlockType(x - delta.X, y - delta.Y, z - delta.Z) : worldData.GetBlockType(x, y, z);
			kindGetter = (int x, int y, int z) => (!isEdge) ? chunk.GetBlockKind(x - delta.X, y - delta.Y, z - delta.Z) : worldData.GetBlockKind(x, y, z);
		}
		bool flag = generateType == GenerateType.Water;
		if (flag)
		{
			BlockType block2 = ((chunk != null) ? chunk.GetBlockType(chunkBlock.X, chunkBlock.Y, chunkBlock.Z) : worldData.GetBlockType(block.X, block.Y, block.Z));
			BlockKind block3 = ((chunk != null) ? chunk.GetBlockKind(chunkBlock.X, chunkBlock.Y, chunkBlock.Z) : worldData.GetBlockKind(block.X, block.Y, block.Z));
			if (!block2.IsAir() && !block2.IsGlass() && block3.IsDefault())
			{
				return index;
			}
		}
		else
		{
			bool flag2 = false;
			for (int num = 0; num < 6; num++)
			{
				if (flag2)
				{
					break;
				}
				IntVect intVect;
				if (num < 2)
				{
					intVect = IntVect.Right;
				}
				else
				{
					intVect = ((num < 4) ? IntVect.Forward : IntVect.Up);
				}
				int num2 = (((num + 1) % 2 == 0) ? 1 : (-1));
				IntVect intVect2 = block + intVect * num2;
				BlockKind blockKind = kindGetter(intVect2.X, intVect2.Y, intVect2.Z);
				flag2 = blockKind != BlockKind.Default;
			}
			BlockType block2 = getter(block.X, block.Y, block.Z);
			BlockKind block3 = kindGetter(block.X, block.Y, block.Z);
			if (!flag2 && block2.IsLand() && block3.IsDefault())
			{
				return index;
			}
		}
		GenerateCondition generateCondition = generateTypeAssociate[(int)generateType];
		byte blockLight = World.Instance.Lighting.GetBlockLight(block.X, block.Y, block.Z);
		BlockType block4 = getter(block.X, block.Y, block.Z + 1);
		bool flag3 = !block4.IsWater() && !block4.IsAir();
		BlockType block5 = getter(block.X, block.Y, block.Z - 1);
		bool flag4 = !block5.IsWater() && !block5.IsAir();
		for (int num3 = 0; num3 < 6; num3++)
		{
			IntVect intVect3;
			if (num3 < 2)
			{
				intVect3 = IntVect.Right;
			}
			else
			{
				intVect3 = ((num3 < 4) ? IntVect.Forward : IntVect.Up);
			}
			int num4 = (((num3 + 1) % 2 == 0) ? 1 : (-1));
			IntVect intVect4 = block + intVect3 * num4;
			BlockType blockType = getter(intVect4.X, intVect4.Y, intVect4.Z);
			BlockKind blockKind2 = kindGetter(intVect4.X, intVect4.Y, intVect4.Z);
			if (!generateCondition(blockType))
			{
				continue;
			}
			BlockFace blockFace = ((num3 < 4) ? BlockFace.Side : ((num3 != 4) ? BlockFace.Bottom : BlockFace.Top));
			block4 = getter(intVect4.X, intVect4.Y, intVect4.Z + 1);
			block5 = getter(intVect4.X, intVect4.Y, intVect4.Z - 1);
			bool flag5 = blockKind2.IsFlip();
			if (flag5)
			{
				blockKind2 = blockKind2.DoFlip();
			}
			float num5 = 0f;
			float num6 = 0f;
			if (num3 < 4 && flag)
			{
				num6 = ((flag3 || (!block4.IsWater() && !block4.IsAir())) ? 0.1f : 0f);
				num5 = ((flag4 || (!block5.IsWater() && !block5.IsAir())) ? 0.1f : 0f);
			}
			if (blockKind2.IsStair())
			{
				Vector3[] array = new Vector3[4];
				StairAssociation association = StairAssociation.GetAssociation(blockKind2);
				BlockFaceStair blockFaceStair = association[num3];
				if (blockFaceStair == BlockFaceStair.Back || (blockFaceStair == BlockFaceStair.Bottom && !flag5) || (blockFaceStair == BlockFaceStair.Top && flag5))
				{
					for (int num7 = 0; num7 < 4; num7++)
					{
						ref Vector3 reference = ref array[num7];
						reference = chunkBlock + offsets[num3][num7];
					}
					float[] uvY = null;
					if (flag5 && blockFaceStair == BlockFaceStair.Back)
					{
						uvY = new float[4] { 1f, 0f, 0f, 1f };
					}
					AddBlockSide(array, data, index, blockType, blockFace, blockLight, flag, uvY, null, flag5);
					index += 4;
				}
				else if (blockFaceStair == BlockFaceStair.Forward || (blockFaceStair == BlockFaceStair.Top && !flag5) || (blockFaceStair == BlockFaceStair.Bottom && flag5))
				{
					Vector3 v = association.Top.v1;
					Vector3 v2 = association.Top.v2;
					int[] num8 = association.Top.num;
					float[][] uvY2 = association.Top.uvY;
					float[][] uvX = association.Top.uvX;
					if (blockFaceStair == BlockFaceStair.Forward)
					{
						uvY2 = association.Forward.uvY;
						uvX = association.Forward.uvX;
						v = association.Forward.v1;
						v2 = association.Forward.v2;
						num8 = association.Forward.num;
					}
					for (int num9 = 0; num9 < 2; num9++)
					{
						float[] uvX2 = uvX[num9];
						float[] uvY3 = uvY2[num9];
						for (int num10 = 0; num10 < 4; num10++)
						{
							Vector3 val = offsets[num3][num10];
							if (!flag5)
							{
								if (num9 == 0)
								{
									val += v;
									if (num10 == num8[0] || num10 == num8[1])
									{
										val += v2;
									}
								}
								else if (num10 == num8[2] || num10 == num8[3])
								{
									val += v2 * -1f;
								}
							}
							else if (num9 == 0)
							{
								if (blockFaceStair == BlockFaceStair.Forward)
								{
									uvY3 = new float[4] { 0.5f, 0f, 0f, 0.5f };
								}
								switch (blockKind2)
								{
								case BlockKind.StairNorth:
								case BlockKind.StairSouth:
									if (blockFaceStair == BlockFaceStair.Bottom)
									{
										val -= v;
									}
									if (num10 == num8[0] || num10 == num8[1])
									{
										val += v2;
									}
									break;
								case BlockKind.StairWest:
								case BlockKind.StairEast:
									switch (blockFaceStair)
									{
									case BlockFaceStair.Bottom:
										if (num10 == num8[0] || num10 == num8[1])
										{
											val -= v2;
										}
										break;
									case BlockFaceStair.Forward:
										if (num10 == num8[0] || num10 == num8[1])
										{
											val += v2;
										}
										break;
									}
									break;
								}
							}
							else
							{
								if (blockFaceStair == BlockFaceStair.Forward)
								{
									uvY3 = new float[4] { 1f, 0.5f, 0.5f, 1f };
								}
								if (blockKind2 == BlockKind.StairWest || blockKind2 == BlockKind.StairEast)
								{
									switch (blockFaceStair)
									{
									case BlockFaceStair.Bottom:
										if (num10 == num8[2] || num10 == num8[3])
										{
											val += v2;
										}
										val -= v;
										break;
									case BlockFaceStair.Forward:
										if (num10 == num8[2] || num10 == num8[3])
										{
											val += v2 * -1f;
										}
										break;
									}
								}
								else if (num10 == num8[2] || num10 == num8[3])
								{
									val += v2 * -1f;
								}
								if (blockFaceStair == BlockFaceStair.Forward)
								{
									val += v;
								}
							}
							ref Vector3 reference2 = ref array[num10];
							reference2 = (Vector3)chunkBlock + val;
						}
						AddBlockSide(array, data, index, blockType, blockFace, blockLight, flag, uvY3, uvX2, flag5);
						index += 4;
					}
				}
				else
				{
					if (blockFaceStair != BlockFaceStair.Right && blockFaceStair != BlockFaceStair.Left)
					{
						continue;
					}
					Vector3 v3 = association.Right.v1;
					Vector3 v4 = association.Right.v2;
					int[] num11 = association.Right.num;
					float[][] uvY2 = association.Right.uvY;
					float[][] uvX = association.Right.uvX;
					if (blockFaceStair == BlockFaceStair.Left)
					{
						uvY2 = association.Left.uvY;
						uvX = association.Left.uvX;
						num11 = association.Left.num;
					}
					for (int num12 = 0; num12 < 2; num12++)
					{
						float[] uvX3 = uvX[num12];
						float[] uvY4 = uvY2[num12];
						for (int num13 = 0; num13 < 4; num13++)
						{
							Vector3 val2 = offsets[num3][num13];
							if (!flag5)
							{
								if (num12 == 0)
								{
									if (num13 == num11[0] || num13 == num11[1])
									{
										val2 += v3;
									}
									if (num13 == num11[0] || num13 == num11[2])
									{
										val2 += v4;
									}
								}
								else if (num13 == num11[3] || num13 == num11[2])
								{
									val2 += v3 * -1f;
								}
							}
							else if (num12 == 0)
							{
								if (num13 == num11[0] || num13 == num11[1])
								{
									val2 += v3;
								}
								if (num13 == num11[1] || num13 == num11[3])
								{
									val2 -= v4;
								}
								uvY4 = new float[4] { 0.5f, 0f, 0f, 0.5f };
							}
							else
							{
								if (num13 == num11[3] || num13 == num11[2])
								{
									val2 += v3 * -1f;
								}
								uvY4 = new float[4] { 1f, 0f, 0f, 1f };
							}
							ref Vector3 reference3 = ref array[num13];
							reference3 = (Vector3)chunkBlock + val2;
						}
						AddBlockSide(array, data, index, blockType, blockFace, blockLight, flag, uvY4, uvX3, flag5);
						index += 4;
					}
				}
				continue;
			}
			Vector3[] array2 = new Vector3[4];
			float[] uvY5 = null;
			float[] uvX4 = null;
			if (blockKind2.IsHalf() && blockFace == BlockFace.Side)
			{
				uvY5 = ((!flag5) ? new float[4] { 0f, 0.5f, 0.5f, 0f } : new float[4] { 0.5f, 1f, 1f, 0.5f });
			}
			else if (blockKind2.IsFence())
			{
				switch (blockFace)
				{
				case BlockFace.Top:
				case BlockFace.Bottom:
					uvY5 = new float[4] { 0.25f, 0.75f, 0.75f, 0.25f };
					uvX4 = new float[4] { 0.25f, 0.25f, 0.75f, 0.75f };
					break;
				case BlockFace.Side:
					uvX4 = new float[4] { 0.25f, 0.25f, 0.75f, 0.75f };
					break;
				}
			}
			else if (blockKind2.IsDiagonal() && blockFace == BlockFace.Side)
			{
				int[] uVNumber = DiagonalAssociation.GetUVNumber(blockKind2);
				if (!flag5)
				{
					if (num3 == uVNumber[0])
					{
						uvY5 = new float[4] { 0f, 1f, 0f, 0f };
					}
				}
				else if (num3 == uVNumber[1])
				{
					uvY5 = new float[4] { 1f, 0f, 1f, 0f };
				}
			}
			else if (blockKind2.IsDiagonal() && blockFace == BlockFace.Top && (blockKind2 == BlockKind.DiagonalEast || blockKind2 == BlockKind.DiagonalWest))
			{
				uvX4 = new float[4] { 0f, 1f, 1f, 0f };
				uvY5 = new float[4] { 0f, 0f, 1f, 1f };
			}
			for (int num14 = 0; num14 < 4; num14++)
			{
				Vector3 val3 = Vector3.zero;
				Vector3 val4 = offsets[num3][num14];
				if (flag)
				{
					val3 = ((num3 >= 4) ? (val3 + new Vector3(0f, 0f, -0.1f)) : (val3 + ((num14 == 0 || num14 == 3) ? new Vector3(0f, 0f, num5 - 0.1f) : new Vector3(0f, 0f, num6 - 0.1f))));
				}
				if (blockKind2.IsHalf())
				{
					if (!flag5)
					{
						if ((num3 < 4 && (num14 == 1 || num14 == 2)) || num3 == 4)
						{
							val3 -= new Vector3(0f, 0f, 0.5f);
						}
					}
					else if ((num3 < 4 && (num14 == 0 || num14 == 3)) || num3 == 5)
					{
						val3 += new Vector3(0f, 0f, 0.5f);
					}
				}
				else if (blockKind2.IsFence())
				{
					val4 = offsetsFence[num3][num14];
				}
				else if (blockKind2.IsDiagonal())
				{
					DiagonalAssociation association2 = DiagonalAssociation.GetAssociation(blockKind2);
					if (!flag5)
					{
						if (num3 == association2.I[0] && (num14 == association2.J[0] || num14 == association2.J[1]))
						{
							val3 += new Vector3(0f, 0f, -1f);
						}
						else if (num3 == association2.I[1] && num14 == association2.J[0])
						{
							val3 += new Vector3(0f, 0f, -1f);
						}
						else if (num3 == association2.I[2] && num14 == association2.J[1])
						{
							val3 += new Vector3(0f, 0f, -1f);
						}
						else if (num3 == 4 && (num14 == association2.J[2] || num14 == association2.J[3]))
						{
							val3 += new Vector3(0f, 0f, -1f);
						}
					}
					else if (num3 == association2.I[0] && (num14 == association2.FlipJ[0] || num14 == association2.FlipJ[1]))
					{
						val3 += new Vector3(0f, 0f, 1f);
					}
					else if (num3 == association2.I[1] && num14 == association2.FlipJ[0])
					{
						val3 += new Vector3(0f, 0f, 1f);
					}
					else if (num3 == association2.I[2] && num14 == association2.FlipJ[1])
					{
						val3 += new Vector3(0f, 0f, 1f);
					}
					else if (num3 == 5 && (num14 == association2.FlipJ[2] || num14 == association2.FlipJ[3]))
					{
						val3 += new Vector3(0f, 0f, 1f);
					}
				}
				Vector3 val5 = (Vector3)chunkBlock + val4;
				ref Vector3 reference4 = ref array2[num14];
				reference4 = new Vector3(val3.x + val5.x, val3.y + val5.y, val3.z + val5.z);
			}
			AddBlockSide(array2, data, index, blockType, blockFace, blockLight, flag, uvY5, uvX4);
			index += 4;
		}
		return index;
	}

	private void AddBlockSide(Vector3[] vert, Chunk.MeshData data, int index, BlockType blockType, BlockFace blockFace, byte blockLight, bool fullUV, float[] uvY = null, float[] uvX = null, bool flip = false)
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		if (uvX == null)
		{
			uvX = new float[4] { 0f, 0f, 1f, 1f };
		}
		if (uvY == null)
		{
			uvY = new float[4] { 0f, 1f, 1f, 0f };
		}
		float num = World.Instance.Lighting.LightingSteps;
		float num2 = (float)(blockLight >> 4) / num;
		float num3 = (float)(blockLight & 0xF) / num;
		Color item = new Color(num2, num3, 0f, 0f);
		for (int i = 0; i < vert.Length; i++)
		{
			Vector3 val = vert[i];
			data.m_Vertices.Add(new Vector3(val.x, val.z, val.y));
			data.m_Colors.Add(item);
		}
		data.m_Indices.Add(index);
		data.m_Indices.Add(index + 1);
		data.m_Indices.Add(index + 2);
		data.m_Indices.Add(index + 2);
		data.m_Indices.Add(index + 3);
		data.m_Indices.Add(index);
		if (fullUV)
		{
			data.m_Uvs.Add(new Vector2(0f, 0f));
			data.m_Uvs.Add(new Vector2(1f, 0f));
			data.m_Uvs.Add(new Vector2(1f, 1f));
			data.m_Uvs.Add(new Vector2(0f, 1f));
			return;
		}
		Rect val2 = worldData.BlockUvCoordinates[blockType].BlockFaceUvCoordinates[(uint)blockFace];
		if (flip && blockFace != BlockFace.Side)
		{
			int num4 = ((blockFace != BlockFace.Bottom) ? 2 : 0);
			val2 = worldData.BlockUvCoordinates[blockType].BlockFaceUvCoordinates[num4];
		}
		data.m_Uvs.Add(new Vector2(val2.x + val2.width * uvX[0], val2.y + val2.height * uvY[0]));
		data.m_Uvs.Add(new Vector2(val2.x + val2.width * uvX[1], val2.y + val2.height * uvY[1]));
		data.m_Uvs.Add(new Vector2(val2.x + val2.width * uvX[2], val2.y + val2.height * uvY[2]));
		data.m_Uvs.Add(new Vector2(val2.x + val2.width * uvX[3], val2.y + val2.height * uvY[3]));
	}
}
