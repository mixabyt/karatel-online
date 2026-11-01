using System.Collections.Generic;

public class WorldDecorator : IWorldDecorator
{
	private readonly IBatchProcessor<Chunk> m_BatchProcessor;

	private readonly List<IDecoration> m_Decorations;

	public WorldDecorator(WorldData worldData, IBatchProcessor<Chunk> batchProcessor, List<IDecoration> decorations)
	{
		m_BatchProcessor = batchProcessor;
		m_Decorations = decorations;
	}

	public void GenerateWorldDecorations(PChunkList chunks)
	{
		m_BatchProcessor.Process(chunks, GenerateDecorationsForChunk, waitUntilAllThreadsFinishh: true);
	}

	public void GenerateDecorationsForChunk(Chunk chunk)
	{
		RandomAdv random = new RandomAdv();
		foreach (IntVect topSoilBlock in chunk.TopSoilBlocks)
		{
			foreach (IDecoration decoration in m_Decorations)
			{
				decoration.Decorate(topSoilBlock.X, topSoilBlock.Y, topSoilBlock.Z, random);
			}
		}
	}
}
