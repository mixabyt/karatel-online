using UnityEngine;

[AddComponentMenu("NGUI/Tween/Alpha")]
public class TweenAlpha : UITweener
{
	public float from = 1f;

	public float to = 1f;

	private Transform mTrans;

	private UIWidget mWidget;

	private UIPanel mPanel;

	public float alpha
	{
		get
		{
			if ((Object)(object)mWidget != (Object)null)
			{
				return mWidget.alpha;
			}
			if ((Object)(object)mPanel != (Object)null)
			{
				return mPanel.alpha;
			}
			return 0f;
		}
		set
		{
			if ((Object)(object)mWidget != (Object)null)
			{
				mWidget.alpha = value;
			}
			else if ((Object)(object)mPanel != (Object)null)
			{
				mPanel.alpha = value;
			}
		}
	}

	private void Awake()
	{
		mPanel = ((Component)this).GetComponent<UIPanel>();
		if ((Object)(object)mPanel == (Object)null)
		{
			mWidget = ((Component)this).GetComponentInChildren<UIWidget>();
		}
	}

	protected override void OnUpdate(float factor, bool isFinished)
	{
		alpha = Mathf.Lerp(from, to, factor);
	}

	public static TweenAlpha Begin(GameObject go, float duration, float alpha)
	{
		TweenAlpha tweenAlpha = UITweener.Begin<TweenAlpha>(go, duration);
		tweenAlpha.from = tweenAlpha.alpha;
		tweenAlpha.to = alpha;
		if (duration <= 0f)
		{
			tweenAlpha.Sample(1f, isFinished: true);
			((Behaviour)tweenAlpha).enabled = false;
		}
		return tweenAlpha;
	}
}
