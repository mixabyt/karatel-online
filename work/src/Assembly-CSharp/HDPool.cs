using System.Collections.Generic;

public class HDPool<T> where T : class
{
	private readonly Stack<T> stack = new Stack<T>();

	public int AcquiredCount { get; private set; }

	public int ReleasedCount { get; private set; }

	public int PoolCount => AcquiredCount - ReleasedCount;

	public int CachedCount => stack.Count;

	public T Acquire()
	{
		AcquiredCount++;
		return (stack.Count != 0) ? stack.Pop() : ((T)null);
	}

	public void Release(T obj)
	{
		ReleasedCount++;
		stack.Push(obj);
	}
}
