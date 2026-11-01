using UnityEngine;

[AddComponentMenu("NGUI/Tween/Spring Position")]
public class SpringPosition : IgnoreTimeScale
{
	public delegate void OnFinished(SpringPosition spring);

	public Vector3 target = Vector3.zero;

	public float strength = 10f;

	public bool worldSpace;

	public bool ignoreTimeScale;

	public GameObject eventReceiver;

	public string callWhenFinished;

	public OnFinished onFinished;

	private Transform mTrans;

	private float mThreshold;

	public SpringPosition()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
	}

	private void Start()
	{
		mTrans = ((Component)this).transform;
	}

	private void Update()
	{
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		float deltaTime = ((!ignoreTimeScale) ? Time.deltaTime : UpdateRealTimeDelta());
		if (worldSpace)
		{
			if (mThreshold == 0f)
			{
				Vector3 val = target - mTrans.position;
				mThreshold = val.magnitude * 0.001f;
			}
			mTrans.position = NGUIMath.SpringLerp(mTrans.position, target, strength, deltaTime);
			float num = mThreshold;
			Vector3 val2 = target - mTrans.position;
			if (num >= val2.magnitude)
			{
				mTrans.position = target;
				if (onFinished != null)
				{
					onFinished(this);
				}
				if ((Object)(object)eventReceiver != (Object)null && !string.IsNullOrEmpty(callWhenFinished))
				{
					eventReceiver.SendMessage(callWhenFinished, (object)this, (SendMessageOptions)1);
				}
				((Behaviour)this).enabled = false;
			}
			return;
		}
		if (mThreshold == 0f)
		{
			Vector3 val3 = target - mTrans.localPosition;
			mThreshold = val3.magnitude * 0.001f;
		}
		mTrans.localPosition = NGUIMath.SpringLerp(mTrans.localPosition, target, strength, deltaTime);
		float num2 = mThreshold;
		Vector3 val4 = target - mTrans.localPosition;
		if (num2 >= val4.magnitude)
		{
			mTrans.localPosition = target;
			if (onFinished != null)
			{
				onFinished(this);
			}
			if ((Object)(object)eventReceiver != (Object)null && !string.IsNullOrEmpty(callWhenFinished))
			{
				eventReceiver.SendMessage(callWhenFinished, (object)this, (SendMessageOptions)1);
			}
			((Behaviour)this).enabled = false;
		}
	}

	public static SpringPosition Begin(GameObject go, Vector3 pos, float strength)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		SpringPosition springPosition = go.GetComponent<SpringPosition>();
		if ((Object)(object)springPosition == (Object)null)
		{
			springPosition = go.AddComponent<SpringPosition>();
		}
		springPosition.target = pos;
		springPosition.strength = strength;
		springPosition.onFinished = null;
		if (!((Behaviour)springPosition).enabled)
		{
			springPosition.mThreshold = 0f;
			((Behaviour)springPosition).enabled = true;
		}
		return springPosition;
	}
}
