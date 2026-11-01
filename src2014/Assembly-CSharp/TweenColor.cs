using UnityEngine;

[AddComponentMenu("NGUI/Tween/Color")]
public class TweenColor : UITweener
{
	public Color from = Color.white;

	public Color to = Color.white;

	private Transform mTrans;

	private UIWidget mWidget;

	private Material mMat;

	private Light mLight;

	public Color color
	{
		get
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)mWidget != (Object)null)
			{
				return mWidget.color;
			}
			if ((Object)(object)mLight != (Object)null)
			{
				return mLight.color;
			}
			if ((Object)(object)mMat != (Object)null)
			{
				return mMat.color;
			}
			return Color.black;
		}
		set
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)mWidget != (Object)null)
			{
				mWidget.color = value;
			}
			if ((Object)(object)mMat != (Object)null)
			{
				mMat.color = value;
			}
			if ((Object)(object)mLight != (Object)null)
			{
				mLight.color = value;
				((Behaviour)mLight).enabled = value.r + value.g + value.b > 0.01f;
			}
		}
	}

	public TweenColor()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
	}

	private void Awake()
	{
		mWidget = ((Component)this).GetComponentInChildren<UIWidget>();
		Renderer renderer = ((Component)this).renderer;
		if ((Object)(object)renderer != (Object)null)
		{
			mMat = renderer.material;
		}
		mLight = ((Component)this).light;
	}

	protected override void OnUpdate(float factor, bool isFinished)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		color = Color.Lerp(from, to, factor);
	}

	public static TweenColor Begin(GameObject go, float duration, Color color)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		TweenColor tweenColor = UITweener.Begin<TweenColor>(go, duration);
		tweenColor.from = tweenColor.color;
		tweenColor.to = color;
		if (duration <= 0f)
		{
			tweenColor.Sample(1f, isFinished: true);
			((Behaviour)tweenColor).enabled = false;
		}
		return tweenColor;
	}
}
