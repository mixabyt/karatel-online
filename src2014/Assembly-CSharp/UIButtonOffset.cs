using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button Offset")]
public class UIButtonOffset : MonoBehaviour
{
	public Transform tweenTarget;

	public Vector3 hover = Vector3.zero;

	public Vector3 pressed = new Vector3(2f, -2f);

	public float duration = 0.2f;

	private Vector3 mPos;

	private bool mStarted;

	private bool mHighlighted;

	public UIButtonOffset()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
	}

	private void Start()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!mStarted)
		{
			mStarted = true;
			if ((Object)(object)tweenTarget == (Object)null)
			{
				tweenTarget = ((Component)this).transform;
			}
			mPos = tweenTarget.localPosition;
		}
	}

	private void OnEnable()
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
			TweenPosition component = ((Component)tweenTarget).GetComponent<TweenPosition>();
			if ((Object)(object)component != (Object)null)
			{
				component.position = mPos;
				((Behaviour)component).enabled = false;
			}
		}
	}

	private void OnPress(bool isPressed)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (((Behaviour)this).enabled)
		{
			if (!mStarted)
			{
				Start();
			}
			TweenPosition.Begin(((Component)tweenTarget).gameObject, duration, isPressed ? (mPos + pressed) : ((!UICamera.IsHighlighted(((Component)this).gameObject)) ? mPos : (mPos + hover))).method = UITweener.Method.EaseInOut;
		}
	}

	private void OnHover(bool isOver)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (((Behaviour)this).enabled)
		{
			if (!mStarted)
			{
				Start();
			}
			TweenPosition.Begin(((Component)tweenTarget).gameObject, duration, (!isOver) ? mPos : (mPos + hover)).method = UITweener.Method.EaseInOut;
			mHighlighted = isOver;
		}
	}
}
