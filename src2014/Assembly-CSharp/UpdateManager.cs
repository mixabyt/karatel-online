using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Internal/Update Manager")]
[ExecuteInEditMode]
public class UpdateManager : MonoBehaviour
{
	public class UpdateEntry
	{
		public int index;

		public OnUpdate func;

		public MonoBehaviour mb;

		public bool isMonoBehaviour;
	}

	public class DestroyEntry
	{
		public Object obj;

		public float time;
	}

	public delegate void OnUpdate(float delta);

	private static UpdateManager mInst;

	private List<UpdateEntry> mOnUpdate = new List<UpdateEntry>();

	private List<UpdateEntry> mOnLate = new List<UpdateEntry>();

	private List<UpdateEntry> mOnCoro = new List<UpdateEntry>();

	private BetterList<DestroyEntry> mDest = new BetterList<DestroyEntry>();

	private float mTime;

	private static int Compare(UpdateEntry a, UpdateEntry b)
	{
		if (a.index < b.index)
		{
			return 1;
		}
		if (a.index > b.index)
		{
			return -1;
		}
		return 0;
	}

	private static void CreateInstance()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected Obj, but got Unknown
		if ((Object)(object)mInst == (Object)null)
		{
			mInst = Object.FindObjectOfType(typeof(UpdateManager)) as UpdateManager;
			if ((Object)(object)mInst == (Object)null && Application.isPlaying)
			{
				GameObject val = new GameObject("_UpdateManager");
				Object.DontDestroyOnLoad((Object)(object)val);
				mInst = val.AddComponent<UpdateManager>();
			}
		}
	}

	private void UpdateList(List<UpdateEntry> list, float delta)
	{
		int num = list.Count;
		while (num > 0)
		{
			UpdateEntry updateEntry = list[--num];
			if (updateEntry.isMonoBehaviour)
			{
				if ((Object)(object)updateEntry.mb == (Object)null)
				{
					list.RemoveAt(num);
					continue;
				}
				if (!((Behaviour)updateEntry.mb).enabled || !NGUITools.GetActive(((Component)updateEntry.mb).gameObject))
				{
					continue;
				}
			}
			updateEntry.func(delta);
		}
	}

	private void Start()
	{
		if (Application.isPlaying)
		{
			mTime = Time.realtimeSinceStartup;
			((MonoBehaviour)this).StartCoroutine(CoroutineFunction());
		}
	}

	private void OnApplicationQuit()
	{
		Object.DestroyImmediate((Object)(object)((Component)this).gameObject);
	}

	private void Update()
	{
		if ((Object)(object)mInst != (Object)(object)this)
		{
			NGUITools.Destroy((Object)(object)((Component)this).gameObject);
		}
		else
		{
			UpdateList(mOnUpdate, Time.deltaTime);
		}
	}

	private void LateUpdate()
	{
		UpdateList(mOnLate, Time.deltaTime);
		if (!Application.isPlaying)
		{
			CoroutineUpdate();
		}
	}

	private bool CoroutineUpdate()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		float num = realtimeSinceStartup - mTime;
		if (num < 0.001f)
		{
			return true;
		}
		mTime = realtimeSinceStartup;
		UpdateList(mOnCoro, num);
		bool isPlaying = Application.isPlaying;
		int num2 = mDest.size;
		while (num2 > 0)
		{
			DestroyEntry destroyEntry = mDest.buffer[--num2];
			if (!isPlaying || destroyEntry.time < mTime)
			{
				if (destroyEntry.obj != (Object)null)
				{
					NGUITools.Destroy(destroyEntry.obj);
					destroyEntry.obj = null;
				}
				mDest.RemoveAt(num2);
			}
		}
		if (mOnUpdate.Count == 0 && mOnLate.Count == 0 && mOnCoro.Count == 0 && mDest.size == 0)
		{
			NGUITools.Destroy((Object)(object)((Component)this).gameObject);
			return false;
		}
		return true;
	}

	private IEnumerator CoroutineFunction()
	{
		while (Application.isPlaying && CoroutineUpdate())
		{
			yield return null;
		}
	}

	private void Add(MonoBehaviour mb, int updateOrder, OnUpdate func, List<UpdateEntry> list)
	{
		int i = 0;
		for (int count = list.Count; i < count; i++)
		{
			UpdateEntry updateEntry = list[i];
			if (updateEntry.func == func)
			{
				return;
			}
		}
		UpdateEntry updateEntry2 = new UpdateEntry();
		updateEntry2.index = updateOrder;
		updateEntry2.func = func;
		updateEntry2.mb = mb;
		updateEntry2.isMonoBehaviour = (Object)(object)mb != (Object)null;
		list.Add(updateEntry2);
		if (updateOrder != 0)
		{
			list.Sort(Compare);
		}
	}

	public static void AddUpdate(MonoBehaviour mb, int updateOrder, OnUpdate func)
	{
		CreateInstance();
		mInst.Add(mb, updateOrder, func, mInst.mOnUpdate);
	}

	public static void AddLateUpdate(MonoBehaviour mb, int updateOrder, OnUpdate func)
	{
		CreateInstance();
		mInst.Add(mb, updateOrder, func, mInst.mOnLate);
	}

	public static void AddCoroutine(MonoBehaviour mb, int updateOrder, OnUpdate func)
	{
		CreateInstance();
		mInst.Add(mb, updateOrder, func, mInst.mOnCoro);
	}

	public static void AddDestroy(Object obj, float delay)
	{
		if (obj == (Object)null)
		{
			return;
		}
		if (Application.isPlaying)
		{
			if (delay > 0f)
			{
				CreateInstance();
				DestroyEntry destroyEntry = new DestroyEntry();
				destroyEntry.obj = obj;
				destroyEntry.time = Time.realtimeSinceStartup + delay;
				mInst.mDest.Add(destroyEntry);
			}
			else
			{
				Object.Destroy(obj);
			}
		}
		else
		{
			Object.DestroyImmediate(obj);
		}
	}
}
