using UnityEngine;

[AddComponentMenu("NGUI/Examples/Lag Position")]
public class LagPosition : MonoBehaviour
{
	public int updateOrder;

	public Vector3 speed = new Vector3(10f, 10f, 10f);

	public bool ignoreTimeScale;

	private Transform mTrans;

	private Vector3 mRelative;

	private Vector3 mAbsolute;

	public LagPosition()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
	}

	private void Start()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		mTrans = ((Component)this).transform;
		mRelative = mTrans.localPosition;
		if (ignoreTimeScale)
		{
			UpdateManager.AddCoroutine((MonoBehaviour)(object)this, updateOrder, CoroutineUpdate);
		}
		else
		{
			UpdateManager.AddLateUpdate((MonoBehaviour)(object)this, updateOrder, CoroutineUpdate);
		}
	}

	private void OnEnable()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		mTrans = ((Component)this).transform;
		mAbsolute = mTrans.position;
	}

	private void CoroutineUpdate(float delta)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		Transform parent = mTrans.parent;
		if ((Object)(object)parent != (Object)null)
		{
			Vector3 val = parent.position + parent.rotation * mRelative;
			mAbsolute.x = Mathf.Lerp(mAbsolute.x, val.x, Mathf.Clamp01(delta * speed.x));
			mAbsolute.y = Mathf.Lerp(mAbsolute.y, val.y, Mathf.Clamp01(delta * speed.y));
			mAbsolute.z = Mathf.Lerp(mAbsolute.z, val.z, Mathf.Clamp01(delta * speed.z));
			mTrans.position = mAbsolute;
		}
	}
}
