using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Root")]
[ExecuteInEditMode]
public class UIRoot : MonoBehaviour
{
	public enum Scaling
	{
		PixelPerfect,
		FixedSize,
		FixedSizeOnMobiles
	}

	private static List<UIRoot> mRoots = new List<UIRoot>();

	public Scaling scalingStyle = Scaling.FixedSize;

	[HideInInspector]
	public bool automatic;

	public int manualHeight = 720;

	public int minimumHeight = 320;

	public int maximumHeight = 1536;

	private Transform mTrans;

	public static List<UIRoot> list => mRoots;

	public int activeHeight
	{
		get
		{
			int num = Mathf.Max(2, Screen.height);
			if (scalingStyle == Scaling.FixedSize)
			{
				return manualHeight;
			}
			if (num < minimumHeight)
			{
				return minimumHeight;
			}
			if (num > maximumHeight)
			{
				return maximumHeight;
			}
			return num;
		}
	}

	public float pixelSizeAdjustment => GetPixelSizeAdjustment(Screen.height);

	public static float GetPixelSizeAdjustment(GameObject go)
	{
		UIRoot uIRoot = NGUITools.FindInParents<UIRoot>(go);
		return (!((Object)(object)uIRoot != (Object)null)) ? 1f : uIRoot.pixelSizeAdjustment;
	}

	public float GetPixelSizeAdjustment(int height)
	{
		height = Mathf.Max(2, height);
		if (scalingStyle == Scaling.FixedSize)
		{
			return (float)manualHeight / (float)height;
		}
		if (height < minimumHeight)
		{
			return (float)minimumHeight / (float)height;
		}
		if (height > maximumHeight)
		{
			return (float)maximumHeight / (float)height;
		}
		return 1f;
	}

	private void Awake()
	{
		mTrans = ((Component)this).transform;
		mRoots.Add(this);
		if (automatic)
		{
			scalingStyle = Scaling.PixelPerfect;
			automatic = false;
		}
	}

	private void OnDestroy()
	{
		mRoots.Remove(this);
	}

	private void Start()
	{
		UIOrthoCamera componentInChildren = ((Component)this).GetComponentInChildren<UIOrthoCamera>();
		if ((Object)(object)componentInChildren != (Object)null)
		{
			Debug.LogWarning((object)"UIRoot should not be active at the same time as UIOrthoCamera. Disabling UIOrthoCamera.", (Object)(object)componentInChildren);
			Camera component = ((Component)componentInChildren).gameObject.GetComponent<Camera>();
			((Behaviour)componentInChildren).enabled = false;
			if ((Object)(object)component != (Object)null)
			{
				component.orthographicSize = 1f;
			}
		}
		else
		{
			Update();
		}
	}

	private void Update()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)mTrans != (Object)null))
		{
			return;
		}
		float num = activeHeight;
		if (num > 0f)
		{
			float num2 = 2f / num;
			Vector3 localScale = mTrans.localScale;
			if (!(Mathf.Abs(localScale.x - num2) <= float.Epsilon) || !(Mathf.Abs(localScale.y - num2) <= float.Epsilon) || !(Mathf.Abs(localScale.z - num2) <= float.Epsilon))
			{
				mTrans.localScale = new Vector3(num2, num2, num2);
			}
		}
	}

	public static void Broadcast(string funcName)
	{
		int i = 0;
		for (int count = mRoots.Count; i < count; i++)
		{
			UIRoot uIRoot = mRoots[i];
			if ((Object)(object)uIRoot != (Object)null)
			{
				((Component)uIRoot).BroadcastMessage(funcName, (SendMessageOptions)1);
			}
		}
	}

	public static void Broadcast(string funcName, object param)
	{
		if (param == null)
		{
			Debug.LogError((object)"SendMessage is bugged when you try to pass 'null' in the parameter field. It behaves as if no parameter was specified.");
			return;
		}
		int i = 0;
		for (int count = mRoots.Count; i < count; i++)
		{
			UIRoot uIRoot = mRoots[i];
			if ((Object)(object)uIRoot != (Object)null)
			{
				((Component)uIRoot).BroadcastMessage(funcName, param, (SendMessageOptions)1);
			}
		}
	}
}
