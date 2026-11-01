using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

public class World : IWorld
{
	public class ZipLevelThread
	{
		public static int chunk_size = 250;

		public byte[] level;

		public byte[] ziped;

		public bool finished;

		public void Run()
		{
			finished = false;
			ziped = Utils.ZipByte(level);
			finished = true;
			level = null;
		}

		public byte[] GetLevelPart(int part)
		{
			MemoryStream memoryStream = new MemoryStream(ziped);
			BinaryReader binaryReader = new BinaryReader(memoryStream);
			int num = 0;
			for (int i = 0; i < ziped.Length; i += chunk_size)
			{
				int num2 = chunk_size;
				if (i + chunk_size > ziped.Length)
				{
					num2 = ziped.Length - i;
				}
				if (part == num)
				{
					return binaryReader.ReadBytes(num2);
				}
				memoryStream.Seek(num2, SeekOrigin.Current);
				num++;
			}
			return null;
		}

		public List<byte[]> GetCurrentZipedLevel()
		{
			MemoryStream input = new MemoryStream(ziped);
			BinaryReader binaryReader = new BinaryReader(input);
			List<byte[]> list = new List<byte[]>();
			for (int i = 0; i < ziped.Length; i += chunk_size)
			{
				int count = chunk_size;
				if (i + chunk_size > ziped.Length)
				{
					count = ziped.Length - i;
				}
				list.Add(binaryReader.ReadBytes(count));
			}
			return list;
		}
	}

	public static World Instance;

	public readonly ILightProcessor Lighting;

	public readonly IMeshGenerator MeshGen;

	public string CurMapId = string.Empty;

	public bool IsHomeWorld = true;

	public LevelDelta CurLevelDelta = new LevelDelta();

	private readonly ITerrainGenerator _TerrainGenerator;

	private readonly WorldData _WorldData;

	private readonly IWorldDecorator _WorldDecorator;

	private int _LastFrameDestruction;

	public ZipLevelThread zipedLevel = new ZipLevelThread();

	public byte[] UnzippedInitialLevel;

	private float lastFrame;

	private bool bFirstDig;

	private IntVect m_DiggingLocation;

	private DateTime m_LastDigTime;

	public readonly Queue<Vector3> Diggings = new Queue<Vector3>();

	private Dictionary<IntVect, CubeDamage2> _cubedamages = new Dictionary<IntVect, CubeDamage2>();

	private int m_DiggingAmount
	{
		get
		{
			return bs._Igor.GetInt("dig", 100);
		}
		set
		{
			bs._Igor.SetInt("dig", value);
		}
	}

	public World()
	{
	}

	public World(WorldData worldData, ITerrainGenerator terrainGenerator, ILightProcessor lightProcessor, IMeshGenerator meshGenerator, IWorldDecorator worldDecorator)
	{
		Instance = this;
		_WorldData = worldData;
		Lighting = lightProcessor;
		MeshGen = meshGenerator;
		_WorldDecorator = worldDecorator;
		_TerrainGenerator = terrainGenerator;
	}

	public void InitializeGridChunks()
	{
		_WorldData.InitializeGridChunks();
	}

	public void GenerateWorld()
	{
		PChunkList allChunks = _WorldData.AllChunks;
		DateTime now = DateTime.Now;
		GenerateTerrain(allChunks);
		Debug.Log((object)(DateTime.Now - now));
		GenerateWorldDecorations(allChunks);
		Debug.Log((object)(DateTime.Now - now));
		GenerateLighting();
		Debug.Log((object)(DateTime.Now - now));
		allChunks.Sort(ChunksComparedByDistanceFromMapCenter);
		MeshGen.GenerateLandMeshes(allChunks);
		ClearRegenerationLandStatus(allChunks);
		Debug.Log((object)(DateTime.Now - now));
		allChunks.Release();
	}

	public void RegenerateAllLight()
	{
		PChunkList allChunks = _WorldData.AllChunks;
		SetRegenerationStatus(allChunks);
		GenerateLighting();
		allChunks.Release();
	}

	public IEnumerator GenerateWorldCourutin()
	{
		PChunkList allChunks = _WorldData.AllChunks;
		for (int i = 0; i < allChunks.Count; i++)
		{
			_TerrainGenerator.GenerateTerrain(allChunks[i]);
			MainMenu.Instance.SetLoadingText("LOADING_LANDSCAPE_GENERATION", " " + i * 100 / allChunks.Count + "%");
			if (allChunks[i].Z == 0)
			{
				yield return 0;
			}
		}
		for (int j = 0; j < allChunks.Count; j++)
		{
			_WorldDecorator.GenerateDecorationsForChunk(allChunks[j]);
			MainMenu.Instance.SetLoadingText("LOADING_DECORATIONS_GENERATION", " " + j * 100 / allChunks.Count + "%");
			if (allChunks[j].Z == 0)
			{
				yield return 0;
			}
		}
		allChunks.Release();
		yield return ((MonoBehaviour)WorldGameObject.Instance).StartCoroutine(SaveWorldToServer(App.Instance.Settings.slotID));
		ZipLevel();
		IEnumerator e = LoadWorldProcess();
		while (e.MoveNext())
		{
			yield return e.Current;
		}
	}

	public IEnumerator LoadWorldProcess()
	{
		PChunkList allChunks = _WorldData.AllChunks;
		allChunks.Sort(ChunksComparedByDistanceFromMapCenter);
		MainMenu.Instance.SetLoadingText("LOADING_LANDSCAPE_LIGHTING", string.Empty);
		yield return 0;
		float t = Time.realtimeSinceStartup;
		Instance.Lighting.InitializeLighting();
		for (int i = 0; i < allChunks.Count; i++)
		{
			Instance.Lighting.InitializeLightingChunk(allChunks[i]);
			MainMenu.Instance.SetLoadingText("LOADING_LANDSCAPE_LIGHTING", " " + i * 100 / allChunks.Count + "%");
			if (Time.realtimeSinceStartup - t >= 0.3f)
			{
				t = Time.realtimeSinceStartup;
				yield return 0;
			}
		}
		yield return 0;
		MainMenu.Instance.SetLoadingText("LOADING_MAP_CREATION", string.Empty);
		yield return 0;
		for (int j = 0; j < allChunks.Count; j++)
		{
			MeshGen.GenerateLandMesh(allChunks[j]);
			MeshGen.GenerateGlassMesh(allChunks[j]);
			MeshGen.GenerateWaterMesh(allChunks[j]);
			MainMenu.Instance.SetLoadingText("LOADING_MAP_CREATION", " " + j * 100 / allChunks.Count + "%");
			if (Time.realtimeSinceStartup - t >= 0.3f)
			{
				t = Time.realtimeSinceStartup;
				yield return 0;
			}
		}
		yield return 0;
		ClearRegenerationLandStatus(allChunks);
		ClearRegenerationWaterStatus(allChunks);
		ClearRegenerationGlassStatus(allChunks);
		allChunks.Release();
		WorldGameObject.Instance.CreateFinishedChunk();
		WorldGameObject.Instance.CompleteWorldGeneration();
	}

	public void ZipLevel()
	{
		zipedLevel.level = new byte[2 * _WorldData.ChunksWide * _WorldData.ChunksHigh * _WorldData.ChunksDeep * WorldData.Instance.m_ChunkBufferLength];
		int num = 0;
		for (int i = 0; i < _WorldData.ChunksWide; i++)
		{
			for (int j = 0; j < _WorldData.ChunksHigh; j++)
			{
				for (int k = 0; k < _WorldData.ChunksDeep; k++)
				{
					byte[] blocksBuffer = _WorldData.Chunks[i, j, k].GetBlocksBuffer();
					blocksBuffer.CopyTo(zipedLevel.level, num);
					num += blocksBuffer.Length;
					byte[] blocksKindBuffer = _WorldData.Chunks[i, j, k].GetBlocksKindBuffer();
					blocksKindBuffer.CopyTo(zipedLevel.level, num);
					num += blocksKindBuffer.Length;
				}
			}
		}
		Thread thread = new Thread(zipedLevel.Run);
		thread.Priority = ThreadPriority.BelowNormal;
		thread.Start();
	}

	public IEnumerator SaveWorldToServer(int slotID)
	{
		Debug.Log((object)("Save world to slot " + slotID));
		MainMenu.Instance.ShowLoading("LOADING_SAVING_MAP", string.Empty);
		yield return 0;
		string error = null;
		MemoryStream ms = new MemoryStream();
		BinaryWriter bw = new BinaryWriter(ms);
		bw.Write((byte)5);
		bw.Write(_WorldData.ChunkBlockWidth);
		bw.Write(_WorldData.ChunkBlockHeight);
		bw.Write(_WorldData.ChunkBlockDepth);
		bw.Write(_WorldData.ChunksWide);
		bw.Write(_WorldData.ChunksHigh);
		bw.Write(_WorldData.ChunksDeep);
		bw.Write((byte)App.Instance.Settings.mapType);
		bw.Write(VkontakteAPI.INSTANCE._viewerId);
		bw.Write((byte)0);
		PChunkList allChunks = _WorldData.AllChunks;
		foreach (Chunk chunk in allChunks)
		{
			bw.Write(chunk.GetBlocksBuffer());
			bw.Write(chunk.GetBlocksKindBuffer());
		}
		allChunks.Release();
		byte[] enityBuffer = GetEntityBuffer();
		bw.Write(enityBuffer.Length);
		bw.Write(enityBuffer);
		byte[] buffer = ms.ToArray();
		byte[] zipBuffer = Utils.ZipByte(buffer);
		MemoryStream ms2 = new MemoryStream();
		BinaryWriter bw2 = new BinaryWriter(ms2);
		bw2.Write(buffer.Length);
		bw2.Write(zipBuffer);
		string fileName = VkontakteAPI.INSTANCE._viewerId + ".map";
		WWWForm saveMapToSlotForm = new WWWForm();
		saveMapToSlotForm.AddField("id", VkontakteAPI.INSTANCE._viewerId.ToString());
		saveMapToSlotForm.AddField("map_place", slotID);
		saveMapToSlotForm.AddField("auth_key", VkontakteAPI.INSTANCE._authKey);
		if (App.Instance.CurPlatform == App.Platform.ONDOKLASNIKI)
		{
			saveMapToSlotForm.AddField("session_key", VkontakteAPI.INSTANCE._sessionKey);
		}
		saveMapToSlotForm.AddBinaryData("buffer", ms2.ToArray(), fileName);
		WWW saveMapToSlot = new WWW(App.Instance.GetCurSavesUrl() + "save_map_to_slot_new.php", saveMapToSlotForm);
		yield return saveMapToSlot;
		Debug.Log((object)saveMapToSlot.text);
		if (saveMapToSlot.error == null)
		{
			if (saveMapToSlot.text == "OK")
			{
				if (App.Instance.CurPlatform == App.Platform.VKONTATE)
				{
					saveMapToSlotForm = new WWWForm();
					saveMapToSlotForm.AddField("id", VkontakteAPI.INSTANCE._viewerId.ToString());
					saveMapToSlotForm.AddField("map_place", slotID);
					saveMapToSlotForm.AddField("auth_key", VkontakteAPI.INSTANCE._authKey);
					saveMapToSlot = new WWW(App.Instance.GetCurScriptsUrl() + "update_slot_info.php", saveMapToSlotForm);
					yield return saveMapToSlot;
					Debug.Log((object)saveMapToSlot.text);
				}
			}
			else
			{
				error = saveMapToSlot.text;
			}
		}
		else
		{
			error = saveMapToSlot.error;
		}
		MainMenu.Instance.HideLoading();
		if (error == null)
		{
			MainMenu.Instance.HideMenu();
			yield break;
		}
		Debug.Log((object)error);
		MainMenu.Instance.ShowHint("Не удалось сохранить карту!\nОшибка:\n" + error);
	}

	private void GenerateWorldDecorations(PChunkList chunks)
	{
		_WorldDecorator.GenerateWorldDecorations(chunks);
	}

	public static void ClearRegenerationLandStatus(IEnumerable<Chunk> chunks)
	{
		foreach (Chunk chunk in chunks)
		{
			chunk.NeedsRegeneration = false;
		}
	}

	public static void ClearRegenerationGlassStatus(IEnumerable<Chunk> chunks)
	{
		foreach (Chunk chunk in chunks)
		{
			chunk.NeedsGlassRegen = false;
		}
	}

	public static void ClearRegenerationWaterStatus(IEnumerable<Chunk> chunks)
	{
		foreach (Chunk chunk in chunks)
		{
			chunk.NeedsWaterRegen = false;
		}
	}

	private static void SetRegenerationStatus(IEnumerable<Chunk> chunks)
	{
		foreach (Chunk chunk in chunks)
		{
			chunk.NeedsRegeneration = true;
		}
	}

	private void GenerateLighting()
	{
		Debug.Log((object)"GenerateLighting");
		PChunkList allChunks = _WorldData.AllChunks;
		Lighting.InitializeLighting();
		for (int i = 0; i < allChunks.Count; i++)
		{
			Lighting.InitializeLightingChunk(allChunks[i]);
		}
		allChunks.Release();
	}

	private void GenerateTerrain(PChunkList chunks)
	{
		_TerrainGenerator.GenerateChunkTerrain(chunks);
	}

	public int ChunksComparedByDistanceFromMapCenter(Chunk firstChunk, Chunk secondChunk)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = new Vector3((float)_WorldData.CenterChunkX, (float)_WorldData.CenterChunkY, 0f);
		return Vector3.Distance(new Vector3((float)firstChunk.X, (float)firstChunk.Y, (float)firstChunk.Z), val).CompareTo((int)Vector3.Distance(new Vector3((float)secondChunk.X, (float)secondChunk.Y, (float)secondChunk.Z), val));
	}

	public void RegenerateChunks(int chunkX, int chunkY, int chunkZ)
	{
		PChunkList chunksNeedingLandRegeneration = _WorldData.ChunksNeedingLandRegeneration;
		if (chunksNeedingLandRegeneration.Count != 0)
		{
			Chunk item = _WorldData.Chunks[chunkX, chunkY, chunkZ];
			if (chunksNeedingLandRegeneration.Contains(item))
			{
				chunksNeedingLandRegeneration.Remove(item);
				chunksNeedingLandRegeneration.Insert(0, item);
			}
			RegenerateLandChunks(chunksNeedingLandRegeneration);
		}
		chunksNeedingLandRegeneration.Release();
	}

	public void RegenerateChunks()
	{
		PChunkList chunksNeedingLandRegeneration = _WorldData.ChunksNeedingLandRegeneration;
		if (chunksNeedingLandRegeneration.Count != 0)
		{
			RegenerateLandChunks(chunksNeedingLandRegeneration);
		}
		chunksNeedingLandRegeneration.Release();
		PChunkList chunksNeedingWaterRegeneration = _WorldData.ChunksNeedingWaterRegeneration;
		if (chunksNeedingWaterRegeneration.Count != 0)
		{
			RegenerateWaterChunks(chunksNeedingWaterRegeneration);
		}
		chunksNeedingWaterRegeneration.Release();
		PChunkList chunksNeedingGlassRegeneration = _WorldData.ChunksNeedingGlassRegeneration;
		if (chunksNeedingGlassRegeneration.Count != 0)
		{
			RegenerateGlassChunks(chunksNeedingGlassRegeneration);
		}
		chunksNeedingGlassRegeneration.Release();
	}

	public void RegenerateGlassChunks(PChunkList chunksNeedingRegeneration)
	{
		PChunkList pChunkList = PChunkList.Acquire();
		foreach (Chunk item in chunksNeedingRegeneration)
		{
			if (!item.GlassRegeneration)
			{
				pChunkList.Add(item);
			}
		}
		MeshGen.GenerateGlassMeshes(pChunkList);
		ClearRegenerationGlassStatus(pChunkList);
		foreach (Chunk item2 in pChunkList)
		{
			item2.GlassRegeneration = true;
		}
		pChunkList.Release();
	}

	public void RegenerateWaterChunks(PChunkList chunksNeedingRegeneration)
	{
		PChunkList pChunkList = PChunkList.Acquire();
		foreach (Chunk item in chunksNeedingRegeneration)
		{
			if (!item.WaterRegeneration)
			{
				pChunkList.Add(item);
			}
		}
		MeshGen.GenerateWaterMeshes(pChunkList);
		ClearRegenerationWaterStatus(pChunkList);
		foreach (Chunk item2 in pChunkList)
		{
			item2.WaterRegeneration = true;
		}
		pChunkList.Release();
	}

	public void RegenerateLandChunks(PChunkList chunksNeedingRegeneration)
	{
		PChunkList pChunkList = PChunkList.Acquire();
		foreach (Chunk item in chunksNeedingRegeneration)
		{
			if (!item.LandRegeneration)
			{
				pChunkList.Add(item);
			}
		}
		MeshGen.GenerateLandMeshes(pChunkList);
		ClearRegenerationLandStatus(pChunkList);
		foreach (Chunk item2 in pChunkList)
		{
			item2.LandRegeneration = true;
		}
		pChunkList.Release();
	}

	public void CubeDestroyEffect(IntVect hitPoint, bool one_part)
	{
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Expected Obj, but got Unknown
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Expected Obj, but got Unknown
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected Obj, but got Unknown
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		if (lastFrame == (float)Time.frameCount)
		{
			return;
		}
		lastFrame = Time.frameCount;
		if (!one_part)
		{
			if (ProfileINI.options_destroy == 1)
			{
				for (int i = 0; i < 12; i++)
				{
					float num = Random.Range(-0.4f, 0.4f);
					float num2 = Random.Range(-0.4f, 0.4f);
					float num3 = Random.Range(-0.4f, 0.4f);
					GameObject val = (GameObject)Object.Instantiate((Object)(object)WorldGameObject.Instance.SparksCubes1, new Vector3((float)hitPoint.X + 0.5f + num, (float)hitPoint.Z + 0.5f + num3, (float)hitPoint.Y + 0.5f + num2), Quaternion.identity);
					val.layer = LayerMask.NameToLayer("ParticleCubesDestroy");
					val.rigidbody.AddForce(new Vector3(num * 300f, num2 * 300f, num3 * 300f));
					val.renderer.material.mainTexture = (Texture)(object)_WorldData.BlockTextures[_WorldData.GetBlockType(hitPoint.X, hitPoint.Y, hitPoint.Z)];
				}
			}
			if (ProfileINI.options_destroy == 2)
			{
				GameObject val2 = (GameObject)Object.Instantiate((Object)(object)WorldGameObject.Instance.SparksCubes2, new Vector3((float)hitPoint.X + 0.44f, (float)hitPoint.Z, (float)hitPoint.Y + 0.45f), Quaternion.identity);
				val2.GetComponent<CubeSparcs>().SetTexture(_WorldData.BlockTextures[_WorldData.GetBlockType(hitPoint.X, hitPoint.Y, hitPoint.Z)]);
			}
		}
		else if (ProfileINI.options_destroy == 1 && _WorldData.GetBlockType(hitPoint.X, hitPoint.Y, hitPoint.Z) != BlockType.Air)
		{
			float num4 = Random.Range(-0.4f, 0.4f);
			float num5 = Random.Range(-0.4f, 0.4f);
			float num6 = Random.Range(-0.4f, 0.4f);
			GameObject val3 = (GameObject)Object.Instantiate((Object)(object)WorldGameObject.Instance.SparksCubes1, new Vector3((float)hitPoint.X + 0.5f + num4, (float)hitPoint.Z + 0.5f + num6, (float)hitPoint.Y + 0.5f + num5), Quaternion.identity);
			val3.layer = LayerMask.NameToLayer("ParticleCubesDestroy");
			val3.rigidbody.AddForce(new Vector3(num4 * 300f, num5 * 300f, num6 * 300f));
			val3.renderer.material.mainTexture = (Texture)(object)_WorldData.BlockTextures[_WorldData.GetBlockType(hitPoint.X, hitPoint.Y, hitPoint.Z)];
		}
	}

	public void RemoveBlockAt(IntVect hitPoint, bool silent)
	{
		if (!App.Instance.Settings.destroyable)
		{
			return;
		}
		if (!silent)
		{
			CubeDestroyEffect(hitPoint, one_part: false);
		}
		CurLevelDelta.Add(BlockType.Air, hitPoint.X, hitPoint.Y, hitPoint.Z);
		_WorldData.SetBlockTypeWithRegeneration(hitPoint.X, hitPoint.Y, hitPoint.Z, BlockType.Air);
		CheckGlassNear(hitPoint);
		CheckWaterNear(hitPoint);
		if (!silent)
		{
			if (Lighting.IsLight(hitPoint.X, hitPoint.Y, hitPoint.Z))
			{
				Lighting.RemoveLight(hitPoint.X, hitPoint.Y, hitPoint.Z, recalculateLighting: true);
			}
			else
			{
				Lighting.RecalculateLightingAroundBlock(hitPoint.X, hitPoint.Y, hitPoint.Z, 0, lightChanged: false);
			}
			BlocksAvulsion.VerifyAvulsionAroundBlocks(hitPoint.X, hitPoint.Y, hitPoint.Z, 0, silent: false);
		}
	}

	public void RemoveBlocksAt(IntVect hitPoint, int radius)
	{
		bool lightChanged = false;
		for (int i = -radius; i <= radius; i++)
		{
			for (int j = -radius; j <= radius; j++)
			{
				for (int k = -radius; k <= radius; k++)
				{
					IntVect hitPoint2 = new IntVect(hitPoint.X + i, hitPoint.Y + j, hitPoint.Z + k);
					BlockType blockType = _WorldData.GetBlockType(hitPoint2.X, hitPoint2.Y, hitPoint2.Z);
					if (blockType != BlockType.Air && blockType != BlockType.TeamBlue && blockType != BlockType.TeamRed)
					{
						if (i == 0 && j == 0 && k == 0)
						{
							CubeDestroyEffect(hitPoint2, one_part: false);
						}
						RemoveBlockAt(hitPoint2, silent: true);
						if (Lighting.IsLight(hitPoint2.X, hitPoint2.Y, hitPoint2.Z))
						{
							Lighting.RemoveLight(hitPoint2.X, hitPoint2.Y, hitPoint2.Z, recalculateLighting: false);
							lightChanged = true;
						}
					}
				}
			}
		}
		Instance.Lighting.RecalculateLightingAroundBlock(hitPoint.X, hitPoint.Y, hitPoint.Z, radius, lightChanged);
		BlocksAvulsion.VerifyAvulsionAroundBlocks(hitPoint.X, hitPoint.Y, hitPoint.Z, radius, silent: false);
	}

	public void DropEntityFrom(IntVect hitPoint, BlockType blockType)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (WorldGameObject.Instance.BlockParametrs[(uint)blockType].dropEntity != EntityType.AIR)
		{
			AddEntityLocal(WorldGameObject.Instance.BlockParametrs[(uint)blockType].dropEntity, new Vector3((float)hitPoint.X + 0.5f, (float)hitPoint.Z + 0.5f, (float)hitPoint.Y + 0.5f));
		}
	}

	public void CheckWaterNear(IntVect pos)
	{
		if (_WorldData.GetBlockType(pos.X, pos.Y, pos.Z + 1) == BlockType.Water)
		{
			_WorldData.SetChunkWaterRegen(pos.X, pos.Y, pos.Z + 1);
			((MonoBehaviour)WorldGameObject.Instance).StartCoroutine(AddWater(pos));
		}
		if (_WorldData.GetBlockType(pos.X, pos.Y, pos.Z - 1) == BlockType.Water)
		{
			_WorldData.SetChunkWaterRegen(pos.X, pos.Y, pos.Z - 1);
		}
		if (_WorldData.GetBlockType(pos.X - 1, pos.Y, pos.Z) == BlockType.Water)
		{
			_WorldData.SetChunkWaterRegen(pos.X - 1, pos.Y, pos.Z);
		}
		if (_WorldData.GetBlockType(pos.X + 1, pos.Y, pos.Z) == BlockType.Water)
		{
			_WorldData.SetChunkWaterRegen(pos.X + 1, pos.Y, pos.Z);
		}
		if (_WorldData.GetBlockType(pos.X, pos.Y + 1, pos.Z) == BlockType.Water)
		{
			_WorldData.SetChunkWaterRegen(pos.X, pos.Y + 1, pos.Z);
		}
		if (_WorldData.GetBlockType(pos.X, pos.Y - 1, pos.Z) == BlockType.Water)
		{
			_WorldData.SetChunkWaterRegen(pos.X, pos.Y - 1, pos.Z);
		}
	}

	public void CheckGlassNear(IntVect pos)
	{
		if (WorldGameObject.Instance.BlockParametrs[(uint)_WorldData.GetBlockType(pos.X, pos.Y, pos.Z + 1)].is_glass)
		{
			_WorldData.GetChunk(pos.X, pos.Y, pos.Z + 1).NeedsGlassRegen = true;
		}
		if (WorldGameObject.Instance.BlockParametrs[(uint)_WorldData.GetBlockType(pos.X, pos.Y, pos.Z - 1)].is_glass)
		{
			_WorldData.GetChunk(pos.X, pos.Y, pos.Z - 1).NeedsGlassRegen = true;
		}
		if (WorldGameObject.Instance.BlockParametrs[(uint)_WorldData.GetBlockType(pos.X - 1, pos.Y, pos.Z)].is_glass)
		{
			_WorldData.GetChunk(pos.X - 1, pos.Y, pos.Z).NeedsGlassRegen = true;
		}
		if (WorldGameObject.Instance.BlockParametrs[(uint)_WorldData.GetBlockType(pos.X + 1, pos.Y, pos.Z)].is_glass)
		{
			_WorldData.GetChunk(pos.X + 1, pos.Y, pos.Z).NeedsGlassRegen = true;
		}
		if (WorldGameObject.Instance.BlockParametrs[(uint)_WorldData.GetBlockType(pos.X, pos.Y + 1, pos.Z)].is_glass)
		{
			_WorldData.GetChunk(pos.X, pos.Y + 1, pos.Z).NeedsGlassRegen = true;
		}
		if (WorldGameObject.Instance.BlockParametrs[(uint)_WorldData.GetBlockType(pos.X, pos.Y - 1, pos.Z)].is_glass)
		{
			_WorldData.GetChunk(pos.X, pos.Y - 1, pos.Z).NeedsGlassRegen = true;
		}
	}

	private void AddSand(IntVect pos)
	{
		if (_WorldData.GetBlockType(pos.X, pos.Y, pos.Z) != BlockType.Sand && pos.Z <= 85 && pos.X >= 0 && pos.Y >= 0 && pos.X <= _WorldData.GetMaxBlockX() && pos.Y <= _WorldData.GetMaxBlockY())
		{
			_WorldData.SetBlockTypeWithLightRegeneration(pos.X, pos.Y, pos.Z, BlockType.Sand);
			if (_WorldData.GetBlockType(pos.X, pos.Y, pos.Z - 1) == BlockType.Air)
			{
				AddSand(new IntVect(pos.X, pos.Y, pos.Z - 1));
			}
			if (_WorldData.GetBlockType(pos.X - 1, pos.Y, pos.Z) == BlockType.Air)
			{
				AddSand(new IntVect(pos.X - 1, pos.Y, pos.Z));
			}
			if (_WorldData.GetBlockType(pos.X + 1, pos.Y, pos.Z) == BlockType.Air)
			{
				AddSand(new IntVect(pos.X + 1, pos.Y, pos.Z));
			}
			if (_WorldData.GetBlockType(pos.X, pos.Y - 1, pos.Z) == BlockType.Air)
			{
				AddSand(new IntVect(pos.X, pos.Y - 1, pos.Z));
			}
			if (_WorldData.GetBlockType(pos.X, pos.Y + 1, pos.Z) == BlockType.Air)
			{
				AddSand(new IntVect(pos.X, pos.Y + 1, pos.Z));
			}
		}
	}

	private IEnumerator AddWater(IntVect pos)
	{
		if (_WorldData.GetBlockType(pos.X, pos.Y, pos.Z) == BlockType.Water)
		{
			yield return 0;
			yield break;
		}
		if (pos.X < 0 || pos.Y < 0 || pos.X > _WorldData.GetMaxBlockX() || pos.Y > _WorldData.GetMaxBlockY())
		{
			yield return 0;
			yield break;
		}
		_WorldData.SetBlockType(pos.X, pos.Y, pos.Z, BlockType.Water);
		_WorldData.SetChunkWaterRegen(pos.X, pos.Y, pos.Z);
		if (_WorldData.GetBlockType(pos.X, pos.Y, pos.Z + 1) == BlockType.Air)
		{
			_WorldData.SetChunkWaterRegen(pos.X, pos.Y, pos.Z + 1);
		}
		if (_WorldData.GetBlockType(pos.X - 1, pos.Y, pos.Z) == BlockType.Air)
		{
			_WorldData.SetChunkWaterRegen(pos.X - 1, pos.Y, pos.Z);
		}
		if (_WorldData.GetBlockType(pos.X + 1, pos.Y, pos.Z) == BlockType.Air)
		{
			_WorldData.SetChunkWaterRegen(pos.X + 1, pos.Y, pos.Z);
		}
		if (_WorldData.GetBlockType(pos.X, pos.Y + 1, pos.Z) == BlockType.Air)
		{
			_WorldData.SetChunkWaterRegen(pos.X, pos.Y + 1, pos.Z);
		}
		if (_WorldData.GetBlockType(pos.X, pos.Y - 1, pos.Z) == BlockType.Air)
		{
			_WorldData.SetChunkWaterRegen(pos.X, pos.Y - 1, pos.Z);
		}
		yield return (object)new WaitForSeconds(0.5f);
		if (_WorldData.GetBlockType(pos.X, pos.Y, pos.Z - 1) == BlockType.Air)
		{
			((MonoBehaviour)WorldGameObject.Instance).StartCoroutine(AddWater(new IntVect(pos.X, pos.Y, pos.Z - 1)));
		}
	}

	private IEnumerator AddDestroyer(IntVect pos)
	{
		if (_WorldData.GetBlockType(pos.X, pos.Y, pos.Z) == BlockType.Water)
		{
			yield return 0;
			yield break;
		}
		if (pos.X < 16 || pos.Y < 16 || pos.X > _WorldData.GetMaxBlockX() || pos.Y > _WorldData.GetMaxBlockY())
		{
			yield return 0;
			yield break;
		}
		_WorldData.SetBlockType(pos.X, pos.Y, pos.Z, BlockType.Water);
		_WorldData.SetChunkWaterRegen(pos.X, pos.Y, pos.Z);
		yield return (object)new WaitForSeconds(0.5f);
		if (_WorldData.GetBlockType(pos.X, pos.Y, pos.Z - 1) == BlockType.Air)
		{
			((MonoBehaviour)WorldGameObject.Instance).StartCoroutine(AddWater(new IntVect(pos.X, pos.Y, pos.Z - 1)));
		}
		if (_WorldData.GetBlockType(pos.X + 1, pos.Y, pos.Z) == BlockType.Air)
		{
			((MonoBehaviour)WorldGameObject.Instance).StartCoroutine(AddWater(new IntVect(pos.X + 1, pos.Y, pos.Z)));
		}
		if (_WorldData.GetBlockType(pos.X, pos.Y + 1, pos.Z) == BlockType.Air)
		{
			((MonoBehaviour)WorldGameObject.Instance).StartCoroutine(AddWater(new IntVect(pos.X, pos.Y + 1, pos.Z)));
		}
		if (_WorldData.GetBlockType(pos.X - 1, pos.Y, pos.Z) == BlockType.Air)
		{
			((MonoBehaviour)WorldGameObject.Instance).StartCoroutine(AddWater(new IntVect(pos.X - 1, pos.Y, pos.Z)));
		}
		if (_WorldData.GetBlockType(pos.X, pos.Y - 1, pos.Z) == BlockType.Air)
		{
			((MonoBehaviour)WorldGameObject.Instance).StartCoroutine(AddWater(new IntVect(pos.X, pos.Y - 1, pos.Z)));
		}
	}

	public void AddBlockAt(IntVect hitPoint, BlockType type, BlockKind kind)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (!App.Instance.Settings.destroyable)
		{
			return;
		}
		SoundManager.Instance.PlayAtPoint(WorldGameObject.Instance.BlockParametrs[(uint)type].HitEffect, new Vector3((float)hitPoint.X, (float)hitPoint.Z, (float)hitPoint.Y));
		if (_WorldData.GetBlockType(hitPoint.X, hitPoint.Y, hitPoint.Z) != BlockType.Water)
		{
			CheckWaterNear(hitPoint);
		}
		if (type == BlockType.Water)
		{
			CurLevelDelta.Add(type, hitPoint.X, hitPoint.Y, hitPoint.Z, checkAvulsion: false, kind);
			((MonoBehaviour)WorldGameObject.Instance).StartCoroutine(AddWater(hitPoint));
			return;
		}
		CurLevelDelta.Add(type, hitPoint.X, hitPoint.Y, hitPoint.Z, checkAvulsion: false, kind);
		_WorldData.SetBlockTypeWithLightRegeneration(hitPoint.X, hitPoint.Y, hitPoint.Z, type, kind);
		if (type == BlockType.Lava)
		{
			Instance.Lighting.AddLight(hitPoint.X, hitPoint.Y, hitPoint.Z, recalculateLighting: true);
		}
		else
		{
			Instance.Lighting.RecalculateLightingAroundBlock(hitPoint.X, hitPoint.Y, hitPoint.Z, 0, lightChanged: false);
		}
	}

	public void AddEntityNetwork(EntityType type, Vector3 pos, Quaternion rot, string creator, string creatorID, object[] data)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		GameObject gameObject = WorldGameObject.Instance.EnityParametrs[(int)type].gameObject;
		int num = data?.Length ?? 0;
		object[] array = new object[3 + num];
		array[0] = type;
		array[1] = creator;
		array[2] = creatorID;
		for (int i = 0; i < num; i++)
		{
			array[3 + i] = data[i];
		}
		PhotonNetwork.InstantiateSceneObject(((Object)gameObject).name, pos, rot, 0, array);
	}

	public GameObject AddEntityLocal(EntityType type, Vector3 pos)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected Obj, but got Unknown
		GameObject gameObject = WorldGameObject.Instance.EnityParametrs[(int)type].gameObject;
		GameObject val = (GameObject)Object.Instantiate((Object)(object)gameObject, pos, gameObject.transform.rotation);
		val.GetComponent<EntityBase>().Type = WorldGameObject.Instance.EnityParametrs[(int)type].Type;
		val.GetComponent<EntityBase>().Name = WorldGameObject.Instance.EnityParametrs[(int)type].name;
		val.layer = LayerMask.NameToLayer("Entity");
		return val;
	}

	public byte[] GetEntityBuffer()
	{
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		MemoryStream memoryStream = new MemoryStream();
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(EntityBase.Entities.Count);
		foreach (EntityBase entity in EntityBase.Entities)
		{
			int type = (int)entity.Type;
			object[] data = ((Component)entity).gameObject.GetComponent<EntityBase>().GetData();
			bool flag = data != null && data.Length > 0;
			binaryWriter.Write(type * ((!flag) ? 1 : (-1)));
			if (entity is Boat)
			{
				Boat boat = entity as Boat;
				binaryWriter.Write(boat.AwakePosition.x);
				binaryWriter.Write(boat.AwakePosition.y);
				binaryWriter.Write(boat.AwakePosition.z);
			}
			else
			{
				binaryWriter.Write(((Component)entity).gameObject.transform.position.x);
				binaryWriter.Write(((Component)entity).gameObject.transform.position.y);
				binaryWriter.Write(((Component)entity).gameObject.transform.position.z);
			}
			binaryWriter.Write(((Component)entity).gameObject.transform.localEulerAngles.x);
			binaryWriter.Write(((Component)entity).gameObject.transform.localEulerAngles.y);
			binaryWriter.Write(((Component)entity).gameObject.transform.localEulerAngles.z);
			if (!flag && entity.Type < EntityType.RAIL)
			{
				continue;
			}
			if (data != null && data.Length > 0)
			{
				if (data.Length < 255)
				{
					binaryWriter.Write((byte)data.Length);
				}
				else
				{
					binaryWriter.Write(byte.MaxValue);
					binaryWriter.Write(data.Length);
				}
				object[] array = data;
				foreach (object obj in array)
				{
					if (obj == null)
					{
						binaryWriter.Write((byte)0);
					}
					else if (obj is string)
					{
						binaryWriter.Write((byte)1);
						binaryWriter.Write((string)obj);
					}
					else if (obj is int)
					{
						binaryWriter.Write((byte)2);
						binaryWriter.Write((int)obj);
					}
					else if (obj is float)
					{
						binaryWriter.Write((byte)3);
						binaryWriter.Write((float)obj);
					}
					else if (obj is bool)
					{
						binaryWriter.Write((byte)4);
						binaryWriter.Write((bool)obj);
					}
					else
					{
						Debug.Log((object)("Entity '" + ((Object)((Component)entity).gameObject).name + "' contain wrong data for serialization! Valid only null, string, int, float, bool."));
					}
				}
			}
			else
			{
				binaryWriter.Write((byte)0);
			}
		}
		return memoryStream.GetBuffer();
	}

	public IEnumerator SetEntityBuffer(byte[] buffer)
	{
		MemoryStream ms = new MemoryStream(buffer);
		BinaryReader br = new BinaryReader(ms);
		int count = br.ReadInt32();
		for (int i = 0; i < count; i++)
		{
			int typeEx = br.ReadInt32();
			EntityType type = (EntityType)Mathf.Abs(typeEx);
			bool haveData = typeEx < 0;
			float x = br.ReadSingle();
			float y = br.ReadSingle();
			float z = br.ReadSingle();
			Vector3 pos = new Vector3(x, y, z);
			float rx = br.ReadSingle();
			float ry = br.ReadSingle();
			float rz = br.ReadSingle();
			Quaternion rot = Quaternion.Euler(rx, ry, rz);
			if (haveData || type >= EntityType.RAIL)
			{
				int dataLength = br.ReadByte();
				if (dataLength == 255)
				{
					dataLength = br.ReadInt32();
				}
				object[] data = null;
				if (dataLength > 0)
				{
					data = new object[dataLength];
					for (int j = 0; j < dataLength; j++)
					{
						switch (br.ReadByte())
						{
						case 0:
							data[j] = null;
							break;
						case 1:
							data[j] = br.ReadString();
							break;
						case 2:
							data[j] = br.ReadInt32();
							break;
						case 3:
							data[j] = br.ReadSingle();
							break;
						case 4:
							data[j] = br.ReadBoolean();
							break;
						}
					}
				}
				AddEntityNetwork(type, pos, rot, ProfileINI.nickname, null, data);
			}
			else
			{
				switch (type)
				{
				case EntityType.TABLICHKAF:
				case EntityType.TABLICHKAW:
					AddEntityNetwork(data: new object[4]
					{
						br.ReadString(),
						br.ReadString(),
						br.ReadString(),
						br.ReadString()
					}, type: type, pos: pos, rot: rot, creator: ProfileINI.nickname, creatorID: null);
					break;
				case EntityType.PICTURE1_1:
				case EntityType.PAPER:
				case EntityType.PICTURE2_1:
				case EntityType.PICTURE1_2:
				case EntityType.PICTURE3_2:
					AddEntityNetwork(data: new object[1] { br.ReadString() }, type: type, pos: pos, rot: rot, creator: ProfileINI.nickname, creatorID: null);
					break;
				default:
					AddEntityNetwork(type, pos, rot, ProfileINI.nickname, null, null);
					break;
				}
			}
			if (i % 10 == 0)
			{
				yield return 0;
			}
		}
	}

	public void FireNukeAt(IntVect hitPoint, Ray ray)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		Debug.Log((object)string.Concat(hitPoint, " - ", ray));
		float num = hitPoint.X;
		float num2 = hitPoint.Y;
		float num3 = hitPoint.Z;
		for (int i = 0; i <= 10; i++)
		{
			num += ray.direction.x;
			num2 += ray.direction.y;
			num3 += ray.direction.z;
			for (int j = 0; j < 10; j++)
			{
				int x = (int)(Random.insideUnitSphere.x * 3f + num);
				int y = (int)(Random.insideUnitSphere.y * 3f + num2);
				int z = (int)(Random.insideUnitSphere.z * 3f + num3);
				_WorldData.SetBlockTypeWithRegeneration(x, y, z, BlockType.Air);
			}
		}
	}

	public void Dig2(IntVect targetLocation, Vector3 hitPoint, float damage = 1f)
	{
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		if (App.Instance.Settings.isWatch)
		{
			Chat.SendWarning(Localize.GetText("CANT_DESTROY_IN_WATCH"), forAll: false);
			return;
		}
		if (!Level.Instance.CanBuild)
		{
			Chat.SendWarning(Localize.GetText("CANT_EDIT_MAP"), forAll: false);
			return;
		}
		BlockType blockType = _WorldData.GetBlockType(targetLocation.X, targetLocation.Y, targetLocation.Z);
		if (blockType != BlockType.Leaves && blockType != BlockType.Water && hitPoint != Vector3.zero)
		{
			Object.Instantiate((Object)(object)WorldGameObject.Instance.Sparks, hitPoint, Quaternion.identity);
		}
		if (!App.Instance.Settings.destroyable)
		{
			WorldGameObject.Instance.SendSoundRpc((int)WorldGameObject.Instance.BlockParametrs[(uint)blockType].HitEffect, hitPoint);
			SoundManager.Instance.Play(WorldGameObject.Instance.BlockParametrs[(uint)blockType].HitEffect, WorldGameObject.Instance.MainPlayer.gameObject.audio);
			return;
		}
		if ((Object)(object)TeamBattle.Instance != (Object)null && (blockType == BlockType.TeamBlue || blockType == BlockType.TeamRed))
		{
			WorldGameObject.Instance.SendSoundRpc((int)WorldGameObject.Instance.BlockParametrs[(uint)blockType].HitEffect, hitPoint);
			SoundManager.Instance.Play(WorldGameObject.Instance.BlockParametrs[(uint)blockType].HitEffect, WorldGameObject.Instance.MainPlayer.gameObject.audio);
			return;
		}
		if (blockType == BlockType.Water)
		{
			SoundManager.Instance.Play(SoundManager.Sound.BangWater, WorldGameObject.Instance.MainPlayer.gameObject.audio);
			WorldGameObject.Instance.photonView.RPC("RemoveBlockAt", PhotonTargets.All, targetLocation.X, targetLocation.Y, targetLocation.Z);
			return;
		}
		CubeDamage2 cubeDamage = null;
		if (!_cubedamages.ContainsKey(targetLocation))
		{
			CubeDamage2 cubeDamage2 = new CubeDamage2();
			_cubedamages[targetLocation] = cubeDamage2;
			cubeDamage = cubeDamage2;
			int num = (int)blockType;
			int life_index = WorldGameObject.Instance.BlockParametrs[num].life_index;
			cubeDamage.life = WorldGameObject.Instance.BlockLifes[life_index].amount;
			m_DiggingLocation = targetLocation;
			bFirstDig = true;
		}
		else
		{
			cubeDamage = _cubedamages[targetLocation];
			bFirstDig = false;
		}
		if (ProfileINI.GetPurchaseValue(StorePurchase.KIRCKA) == 0)
		{
			cubeDamage.life -= (int)(25f * damage);
		}
		else
		{
			cubeDamage.life -= (int)(50f * damage);
		}
		if (cubeDamage.life <= 0)
		{
			_cubedamages.Remove(targetLocation);
			DropEntityFrom(targetLocation, _WorldData.GetBlockType(targetLocation.X, targetLocation.Y, targetLocation.Z));
			WorldGameObject.Instance.photonView.RPC("RemoveBlockAt", PhotonTargets.All, targetLocation.X, targetLocation.Y, targetLocation.Z);
			if (bFirstDig)
			{
				SoundManager.Instance.Play(WorldGameObject.Instance.BlockParametrs[(uint)blockType].HitEffect, WorldGameObject.Instance.MainPlayer.gameObject.audio);
				WorldGameObject.Instance.photonView.RPC("SoundRPC", PhotonTargets.Others, (int)WorldGameObject.Instance.BlockParametrs[(uint)blockType].HitEffect, hitPoint);
			}
			else
			{
				WorldGameObject.Instance.photonView.RPC("SoundRPC", PhotonTargets.Others, (int)WorldGameObject.Instance.BlockParametrs[(uint)blockType].DestroyEffect, hitPoint);
				SoundManager.Instance.Play(WorldGameObject.Instance.BlockParametrs[(uint)blockType].DestroyEffect, WorldGameObject.Instance.MainPlayer.gameObject.audio);
			}
		}
		else
		{
			WorldGameObject.Instance.SendSoundRpc((int)WorldGameObject.Instance.BlockParametrs[(uint)blockType].HitEffect, hitPoint);
			SoundManager.Instance.Play(WorldGameObject.Instance.BlockParametrs[(uint)blockType].HitEffect, WorldGameObject.Instance.MainPlayer.gameObject.audio);
		}
	}

	public void Dig(IntVect targetLocation, Vector3 hitPoint, Vector3 normal)
	{
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		if (!Level.Instance.CanBuild)
		{
			Chat.SendWarning(Localize.GetText("CANT_EDIT_MAP"), forAll: false);
			return;
		}
		GameObject cubeDamage = WorldGameObject.Instance.CubeDamage;
		BlockType blockType = _WorldData.GetBlockType(targetLocation.X, targetLocation.Y, targetLocation.Z);
		if (blockType != BlockType.Leaves && blockType != BlockType.Water)
		{
			Object.Instantiate((Object)(object)WorldGameObject.Instance.Sparks, hitPoint, Quaternion.identity);
		}
		if (!App.Instance.Settings.destroyable)
		{
			WorldGameObject.Instance.SendSoundRpc((int)WorldGameObject.Instance.BlockParametrs[(uint)blockType].HitEffect, hitPoint);
			SoundManager.Instance.Play(WorldGameObject.Instance.BlockParametrs[(uint)blockType].HitEffect, WorldGameObject.Instance.MainPlayer.gameObject.audio);
			return;
		}
		if ((Object)(object)TeamBattle.Instance != (Object)null && (blockType == BlockType.TeamBlue || blockType == BlockType.TeamRed))
		{
			WorldGameObject.Instance.SendSoundRpc((int)WorldGameObject.Instance.BlockParametrs[(uint)blockType].HitEffect, hitPoint);
			SoundManager.Instance.Play(WorldGameObject.Instance.BlockParametrs[(uint)blockType].HitEffect, WorldGameObject.Instance.MainPlayer.gameObject.audio);
			return;
		}
		if (blockType == BlockType.Water)
		{
			SoundManager.Instance.Play(SoundManager.Sound.BangWater, WorldGameObject.Instance.MainPlayer.gameObject.audio);
			WorldGameObject.Instance.photonView.RPC("RemoveBlockAt", PhotonTargets.All, targetLocation.X, targetLocation.Y, targetLocation.Z);
			m_DiggingAmount = 0;
			((Renderer)cubeDamage.GetComponent<MeshRenderer>()).enabled = false;
			return;
		}
		if (targetLocation != m_DiggingLocation)
		{
			int num = (int)blockType;
			int life_index = WorldGameObject.Instance.BlockParametrs[num].life_index;
			m_DiggingAmount = WorldGameObject.Instance.BlockLifes[life_index].amount;
			m_DiggingLocation = targetLocation;
			bFirstDig = true;
			((Renderer)cubeDamage.GetComponent<MeshRenderer>()).enabled = true;
			cubeDamage.GetComponent<CubeDamage>().Reset();
			cubeDamage.transform.position = new Vector3((float)targetLocation.X + 0.5f, (float)targetLocation.Z + 0.5f, (float)targetLocation.Y + 0.5f);
		}
		else
		{
			bFirstDig = false;
		}
		if (ProfileINI.GetPurchaseValue(StorePurchase.KIRCKA) == 0)
		{
			m_DiggingAmount -= 25;
		}
		else
		{
			m_DiggingAmount -= 50;
		}
		cubeDamage.GetComponent<CubeDamage>().SetFrame((100 - m_DiggingAmount) / 10);
		if (blockType == BlockType.Reshetka1 || blockType == BlockType.Reshetka2 || blockType == BlockType.Reshetka3 || blockType == BlockType.Reshetka4 || blockType == BlockType.Reshetka4 || blockType == BlockType.Reshetka5)
		{
			((Renderer)cubeDamage.GetComponent<MeshRenderer>()).enabled = false;
		}
		else
		{
			((Renderer)cubeDamage.GetComponent<MeshRenderer>()).enabled = true;
		}
		if (m_DiggingAmount <= 0)
		{
			DropEntityFrom(targetLocation, _WorldData.GetBlockType(targetLocation.X, targetLocation.Y, targetLocation.Z));
			WorldGameObject.Instance.photonView.RPC("RemoveBlockAt", PhotonTargets.All, targetLocation.X, targetLocation.Y, targetLocation.Z);
			m_DiggingAmount = 100;
			((Renderer)cubeDamage.GetComponent<MeshRenderer>()).enabled = false;
			if (bFirstDig)
			{
				WorldGameObject.Instance.photonView.RPC("SoundRPC", PhotonTargets.Others, (int)WorldGameObject.Instance.BlockParametrs[(uint)blockType].HitEffect, hitPoint);
				SoundManager.Instance.Play(WorldGameObject.Instance.BlockParametrs[(uint)blockType].HitEffect, WorldGameObject.Instance.MainPlayer.gameObject.audio);
			}
			else
			{
				WorldGameObject.Instance.photonView.RPC("SoundRPC", PhotonTargets.Others, (int)WorldGameObject.Instance.BlockParametrs[(uint)blockType].DestroyEffect, hitPoint);
				SoundManager.Instance.Play(WorldGameObject.Instance.BlockParametrs[(uint)blockType].DestroyEffect, WorldGameObject.Instance.MainPlayer.gameObject.audio);
			}
		}
		else
		{
			WorldGameObject.Instance.SendSoundRpc((int)WorldGameObject.Instance.BlockParametrs[(uint)blockType].HitEffect, hitPoint);
			SoundManager.Instance.Play(WorldGameObject.Instance.BlockParametrs[(uint)blockType].HitEffect, WorldGameObject.Instance.MainPlayer.gameObject.audio);
		}
	}

	public static void DestroyChunk(Chunk chunk)
	{
		if ((Object)(object)chunk.LandChunk != (Object)null)
		{
			MeshFilter component = ((Component)chunk.LandChunk).GetComponent<MeshFilter>();
			component.mesh.Clear();
			Object.Destroy((Object)(object)component.mesh);
			component.mesh = null;
			Object.Destroy((Object)(object)component);
			Object.Destroy((Object)(object)((Component)chunk.LandChunk).gameObject);
			chunk.LandChunk = null;
		}
		if ((Object)(object)chunk.WaterChunk != (Object)null)
		{
			MeshFilter component2 = ((Component)chunk.WaterChunk).GetComponent<MeshFilter>();
			component2.mesh.Clear();
			Object.Destroy((Object)(object)component2.mesh);
			component2.mesh = null;
			Object.Destroy((Object)(object)component2);
			Object.Destroy((Object)(object)((Component)chunk.WaterChunk).gameObject);
			chunk.WaterChunk = null;
		}
		if ((Object)(object)chunk.GlassChunk != (Object)null)
		{
			MeshFilter component3 = ((Component)chunk.GlassChunk).GetComponent<MeshFilter>();
			component3.mesh.Clear();
			Object.Destroy((Object)(object)component3.mesh);
			component3.mesh = null;
			Object.Destroy((Object)(object)component3);
			Object.Destroy((Object)(object)((Component)chunk.GlassChunk).gameObject);
			chunk.GlassChunk = null;
		}
		chunk = null;
	}
}
