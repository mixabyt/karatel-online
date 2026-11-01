using UnityEngine;

[RequireComponent(typeof(UIPanel))]
[AddComponentMenu("NGUI/Internal/Spring Panel")]
public class SpringPanel : IgnoreTimeScale
{
	public delegate void OnFinished();

	public Vector3 target = Vector3.zero;

	public float strength = 10f;

	public OnFinished onFinished;

	private UIPanel mPanel;

	private Transform mTrans;

	private float mThreshold;

	private UIDraggablePanel mDrag;

	public SpringPanel()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
	}

	private void Start()
	{
		mPanel = ((Component)this).GetComponent<UIPanel>();
		mDrag = ((Component)this).GetComponent<UIDraggablePanel>();
		mTrans = ((Component)this).transform;
	}

	private void Update()
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		float deltaTime = UpdateRealTimeDelta();
		if (mThreshold == 0f)
		{
			Vector3 val = target - mTrans.localPosition;
			mThreshold = val.magnitude * 0.005f;
		}
		bool flag = false;
		Vector3 localPosition = mTrans.localPosition;
		Vector3 val2 = NGUIMath.SpringLerp(mTrans.localPosition, target, strength, deltaTime);
		if (mThreshold >= Vector3.Magnitude(val2 - target))
		{
			val2 = target;
			((Behaviour)this).enabled = false;
			flag = true;
		}
		mTrans.localPosition = val2;
		Vector3 val3 = val2 - localPosition;
		Vector4 clipRange = mPanel.clipRange;
		clipRange.x -= val3.x;
		clipRange.y -= val3.y;
		mPanel.clipRange = clipRange;
		if ((Object)(object)mDrag != (Object)null)
		{
			mDrag.UpdateScrollbars(recalculateBounds: false);
		}
		if (flag && onFinished != null)
		{
			onFinished();
		}
	}

	public static SpringPanel Begin(GameObject go, Vector3 pos, float strength)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		SpringPanel springPanel = go.GetComponent<SpringPanel>();
		if ((Object)(object)springPanel == (Object)null)
		{
			springPanel = go.AddComponent<SpringPanel>();
		}
		springPanel.target = pos;
		springPanel.strength = strength;
		springPanel.onFinished = null;
		if (!((Behaviour)springPanel).enabled)
		{
			springPanel.mThreshold = 0f;
			((Behaviour)springPanel).enabled = true;
		}
		return springPanel;
	}
}
