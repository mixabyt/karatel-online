using System;
using System.Collections.Generic;
using System.Threading;

public class BatchProcessor<T> : IBatchProcessor<T> where T : class
{
	public void Process(IList<T> itemsToProcess, Action<T> action, bool waitUntilAllThreadsFinish)
	{
		int numberOfThreads = Environment.ProcessorCount - 1;
		Process(numberOfThreads, itemsToProcess, action, waitUntilAllThreadsFinish);
	}

	public void Process(int numberOfThreads, IList<T> itemsToProcess_, Action<T> action, bool waitUntilAllThreadsFinish)
	{
		List<T> itemsToProcess = new List<T>(itemsToProcess_);
		List<Thread> list = new List<Thread>();
		if (numberOfThreads < 1)
		{
			numberOfThreads = 1;
		}
		int count = itemsToProcess.Count;
		if (count == 0)
		{
			return;
		}
		if (numberOfThreads > count)
		{
			numberOfThreads = count;
		}
		int itemsPerThread = count / numberOfThreads;
		int num = itemsPerThread;
		for (int i = 0; i < numberOfThreads; i++)
		{
			if (i == numberOfThreads - 1)
			{
				num = count - i * itemsPerThread;
			}
			int number = i;
			int process = num;
			Thread thread = new Thread(() =>
			{
				ActionAgainstMultiple(itemsToProcess, number * itemsPerThread, process, action);
			});
			list.Add(thread);
			thread.Start();
		}
		if (!waitUntilAllThreadsFinish)
		{
			return;
		}
		foreach (Thread item in list)
		{
			item.Join();
		}
	}

	private static void ActionAgainstMultiple(IList<T> items, int startIndex, int length, Action<T> action)
	{
		for (int i = startIndex; i < startIndex + length; i++)
		{
			T val = items[i];
			if (val != null)
			{
				action(val);
			}
			Thread.Sleep(1);
		}
	}
}
