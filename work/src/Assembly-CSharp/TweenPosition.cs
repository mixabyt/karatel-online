using UnityEngine;

[AddComponentMenu("NGUI/Tween/Position")]
public class TweenPosition : UITweener
{
	public Vector3 from;

	public Vector3 to;

	private Transform mTrans;

	public Transform cachedTransform
	{
		get
		{
			if ((Object)(object)mTrans == (Object)null)
			{
				mTrans = ((Component)this).transform;
			}
			return mTrans;
		}
	}

	public Vector3 position
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return cachedTransform.localPosition;
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			cachedTransform.localPosition = value;
		}
	}

	protected override void OnUpdate(float factor, bool isFinished)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		cachedTransform.localPosition = from * (1f - factor) + to * factor;
	}

	public static TweenPosition Begin(GameObject go, float duration, Vector3 pos)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		TweenPosition tweenPosition = UITweener.Begin<TweenPosition>(go, duration);
		tweenPosition.from = tweenPosition.position;
		tweenPosition.to = pos;
		if (duration <= 0f)
		{
			tweenPosition.Sample(1f, isFinished: true);
			((Behaviour)tweenPosition).enabled = false;
		}
		return tweenPosition;
	}
}
