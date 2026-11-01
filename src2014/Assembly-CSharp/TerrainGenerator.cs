public class TerrainGenerator : ITerrainGenerator
{
	private readonly WorldData m_WorldData;

	private readonly IBatchProcessor<Chunk> m_BatchProcessor;

	private readonly ITerrainGenerationMethod m_TerrainGenerationMethod;

	public TerrainGenerator(WorldData worldData, IBatchProcessor<Chunk> batchProcessor, ITerrainGenerationMethod terrainGenerationMethod)
	{
		m_WorldData = worldData;
		m_BatchProcessor = batchProcessor;
		m_TerrainGenerationMethod = terrainGenerationMethod;
	}

	public void GenerateTerrain(Chunk chunk)
	{
		if (chunk.Z == 0)
		{
			m_TerrainGenerationMethod.GenerateTerrain(m_WorldData, chunk, (int)m_WorldData.NoiseBlockXOffset);
		}
	}

	public void GenerateChunkTerrain(PChunkList chunks)
	{
		PChunkList pChunkList = new PChunkList();
		foreach (Chunk chunk in chunks)
		{
			if (chunk.Z == 0)
			{
				pChunkList.Add(chunk);
			}
		}
		m_BatchProcessor.Process(pChunkList, GenerateTerrain, waitUntilAllThreadsFinishh: true);
	}
}
