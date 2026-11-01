using System;
using System.Collections;
using System.Collections.Generic;

public class PChunkList : HDPoolableObject, IEnumerable, IEnumerable<Chunk>, IList<Chunk>, ICollection<Chunk>
{
	public static readonly HDPool<PChunkList> pool = new HDPool<PChunkList>();

	private readonly List<Chunk> list = new List<Chunk>();

	public int Count => list.Count;

	public bool IsReadOnly => false;

	public Chunk this[int index]
	{
		get
		{
			return list[index];
		}
		set
		{
			list[index] = value;
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public static PChunkList Acquire()
	{
		return pool.Acquire() ?? new PChunkList();
	}

	public override void Release()
	{
		list.Clear();
		pool.Release(this);
	}

	public IEnumerator<Chunk> GetEnumerator()
	{
		return list.GetEnumerator();
	}

	public void Add(Chunk item)
	{
		list.Add(item);
	}

	public void Clear()
	{
		list.Clear();
	}

	public bool Contains(Chunk item)
	{
		return list.Contains(item);
	}

	public void CopyTo(Chunk[] array, int arrayIndex)
	{
		list.CopyTo(array, arrayIndex);
	}

	public bool Remove(Chunk item)
	{
		return list.Remove(item);
	}

	public int IndexOf(Chunk item)
	{
		return list.IndexOf(item);
	}

	public void Insert(int index, Chunk item)
	{
		list.Insert(index, item);
	}

	public void RemoveAt(int index)
	{
		list.RemoveAt(index);
	}

	public void Sort(Comparison<Chunk> comparer)
	{
		list.Sort(comparer);
	}

	public void Sort(IComparer<Chunk> comparer)
	{
		list.Sort(comparer);
	}

	public void Sort()
	{
		list.Sort();
	}
}
