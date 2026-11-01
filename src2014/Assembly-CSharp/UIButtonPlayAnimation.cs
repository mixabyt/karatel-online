using AnimationOrTween;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button Play Animation")]
public class UIButtonPlayAnimation : MonoBehaviour
{
	public Animation target;

	public string clipName;

	public Trigger trigger;

	public Direction playDirection = Direction.Forward;

	public bool resetOnPlay;

	public bool clearSelection;

	public EnableCondition ifDisabledOnPlay;

	public DisableCondition disableWhenFinished;

	public GameObject eventReceiver;

	public string callWhenFinished;

	public ActiveAnimation.OnFinished onFinished;

	private bool mStarted;

	private bool mHighlighted;

	private void Start()
	{
		mStarted = true;
	}

	private void OnEnable()
	{
		if (mStarted && mHighlighted)
		{
			OnHover(UICamera.IsHighlighted(((Component)this).gameObject));
		}
	}

	private void OnHover(bool isOver)
	{
		if (((Behaviour)this).enabled)
		{
			if (trigger == Trigger.OnHover || (trigger == Trigger.OnHoverTrue && isOver) || (trigger == Trigger.OnHoverFalse && !isOver))
			{
				Play(isOver);
			}
			mHighlighted = isOver;
		}
	}

	private void OnPress(bool isPressed)
	{
		if (((Behaviour)this).enabled && (trigger == Trigger.OnPress || (trigger == Trigger.OnPressTrue && isPressed) || (trigger == Trigger.OnPressFalse && !isPressed)))
		{
			Play(isPressed);
		}
	}

	private void OnClick()
	{
		if (((Behaviour)this).enabled && trigger == Trigger.OnClick)
		{
			Play(forward: true);
		}
	}

	private void OnDoubleClick()
	{
		if (((Behaviour)this).enabled && trigger == Trigger.OnDoubleClick)
		{
			Play(forward: true);
		}
	}

	private void OnSelect(bool isSelected)
	{
		if (((Behaviour)this).enabled && (trigger == Trigger.OnSelect || (trigger == Trigger.OnSelectTrue && isSelected) || (trigger == Trigger.OnSelectFalse && !isSelected)))
		{
			Play(forward: true);
		}
	}

	private void OnActivate(bool isActive)
	{
		if (((Behaviour)this).enabled && (trigger == Trigger.OnActivate || (trigger == Trigger.OnActivateTrue && isActive) || (trigger == Trigger.OnActivateFalse && !isActive)))
		{
			Play(isActive);
		}
	}

	private void Play(bool forward)
	{
		if ((Object)(object)target == (Object)null)
		{
			target = ((Component)this).GetComponentInChildren<Animation>();
		}
		if (!((Object)(object)target != (Object)null))
		{
			return;
		}
		if (clearSelection && (Object)(object)UICamera.selectedObject == (Object)(object)((Component)this).gameObject)
		{
			UICamera.selectedObject = null;
		}
		int num = 0 - playDirection;
		Direction direction = ((!forward) ? ((Direction)num) : playDirection);
		ActiveAnimation activeAnimation = ActiveAnimation.Play(target, clipName, direction, ifDisabledOnPlay, disableWhenFinished);
		if (!((Object)(object)activeAnimation == (Object)null))
		{
			if (resetOnPlay)
			{
				activeAnimation.Reset();
			}
			activeAnimation.onFinished = onFinished;
			if ((Object)(object)eventReceiver != (Object)null && !string.IsNullOrEmpty(callWhenFinished))
			{
				activeAnimation.eventReceiver = eventReceiver;
				activeAnimation.callWhenFinished = callWhenFinished;
			}
			else
			{
				activeAnimation.eventReceiver = null;
			}
		}
	}
}
