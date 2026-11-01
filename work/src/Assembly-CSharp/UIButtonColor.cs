using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button Color")]
public class UIButtonColor : MonoBehaviour
{
	public GameObject tweenTarget;

	public Color hover = new Color(0.6f, 1f, 0.2f, 1f);

	public Color pressed = Color.grey;

	public float duration = 0.2f;

	protected Color mColor;

	protected bool mStarted;

	protected bool mHighlighted;

	public Color defaultColor
	{
		get
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			if (!mStarted)
			{
				Init();
			}
			return mColor;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			mColor = value;
		}
	}

	public UIButtonColor()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
	}

	private void Start()
	{
		if (!mStarted)
		{
			Init();
			mStarted = true;
		}
	}

	protected virtual void OnEnable()
	{
		if (mStarted && mHighlighted)
		{
			OnHover(UICamera.IsHighlighted(((Component)this).gameObject));
		}
	}

	private void OnDisable()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (mStarted && (Object)(object)tweenTarget != (Object)null)
		{
			TweenColor component = tweenTarget.GetComponent<TweenColor>();
			if ((Object)(object)component != (Object)null)
			{
				component.color = mColor;
				((Behaviour)component).enabled = false;
			}
		}
	}

	protected void Init()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)tweenTarget == (Object)null)
		{
			tweenTarget = ((Component)this).gameObject;
		}
		UIWidget component = tweenTarget.GetComponent<UIWidget>();
		if ((Object)(object)component != (Object)null)
		{
			mColor = component.color;
		}
		else
		{
			Renderer renderer = tweenTarget.renderer;
			if ((Object)(object)renderer != (Object)null)
			{
				mColor = renderer.material.color;
			}
			else
			{
				Light light = tweenTarget.light;
				if ((Object)(object)light != (Object)null)
				{
					mColor = light.color;
				}
				else
				{
					Debug.LogWarning((object)(NGUITools.GetHierarchy(((Component)this).gameObject) + " has nothing for UIButtonColor to color"), (Object)(object)this);
					((Behaviour)this).enabled = false;
				}
			}
		}
		OnEnable();
	}

	public virtual void OnPress(bool isPressed)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if (((Behaviour)this).enabled)
		{
			if (!mStarted)
			{
				Start();
			}
			TweenColor.Begin(tweenTarget, duration, isPressed ? pressed : ((!UICamera.IsHighlighted(((Component)this).gameObject)) ? mColor : hover));
		}
	}

	public virtual void OnHover(bool isOver)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (((Behaviour)this).enabled)
		{
			if (!mStarted)
			{
				Start();
			}
			TweenColor.Begin(tweenTarget, duration, (!isOver) ? mColor : hover);
			mHighlighted = isOver;
		}
	}
}
