using System;
using UnityEngine;

[Serializable]
public class TimedObjectDestructor : MonoBehaviour
{
	public float timeOut;

	public bool detachChildren;

	public TimedObjectDestructor()
	{
		timeOut = 1f;
	}

	public override void Awake()
	{
		((MonoBehaviour)this).Invoke("DestroyNow", timeOut);
	}

	public override void DestroyNow()
	{
		if (detachChildren)
		{
			((Component)this).transform.DetachChildren();
		}
		Object.DestroyObject((Object)(object)((Component)this).gameObject);
	}

	public override void Main()
	{
	}
}
