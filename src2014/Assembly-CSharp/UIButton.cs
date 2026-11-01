using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button")]
public class UIButton : UIButtonColor
{
	public Color disabledColor = Color.grey;

	public bool isEnabled
	{
		get
		{
			Collider collider = ((Component)this).collider;
			return Object.op_Implicit((Object)(object)collider) && collider.enabled;
		}
		set
		{
			Collider collider = ((Component)this).collider;
			if (Object.op_Implicit((Object)(object)collider) && collider.enabled != value)
			{
				collider.enabled = value;
				UpdateColor(value, immediate: false);
			}
		}
	}

	public UIButton()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
	}

	protected override void OnEnable()
	{
		if (isEnabled)
		{
			base.OnEnable();
		}
		else
		{
			UpdateColor(shouldBeEnabled: false, immediate: true);
		}
	}

	public override void OnHover(bool isOver)
	{
		if (isEnabled)
		{
			base.OnHover(isOver);
		}
	}

	public override void OnPress(bool isPressed)
	{
		if (isEnabled)
		{
			base.OnPress(isPressed);
		}
	}

	public void UpdateColor(bool shouldBeEnabled, bool immediate)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)tweenTarget))
		{
			if (!mStarted)
			{
				mStarted = true;
				Init();
			}
			Color color = ((!shouldBeEnabled) ? disabledColor : defaultColor);
			TweenColor tweenColor = TweenColor.Begin(tweenTarget, 0.15f, color);
			if (immediate)
			{
				tweenColor.color = color;
				((Behaviour)tweenColor).enabled = false;
			}
		}
	}
}
