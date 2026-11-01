using AnimationOrTween;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button Tween")]
public class UIButtonTween : MonoBehaviour
{
	public GameObject tweenTarget;

	public int tweenGroup;

	public Trigger trigger;

	public Direction playDirection = Direction.Forward;

	public bool resetOnPlay;

	public EnableCondition ifDisabledOnPlay;

	public DisableCondition disableWhenFinished;

	public bool includeChildren;

	public GameObject eventReceiver;

	public string callWhenFinished;

	public UITweener.OnFinished onFinished;

	private UITweener[] mTweens;

	private bool mStarted;

	private bool mHighlighted;

	private void Start()
	{
		mStarted = true;
		if ((Object)(object)tweenTarget == (Object)null)
		{
			tweenTarget = ((Component)this).gameObject;
		}
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

	private void Update()
	{
		if (disableWhenFinished == DisableCondition.DoNotDisable || mTweens == null)
		{
			return;
		}
		bool flag = true;
		bool flag2 = true;
		int i = 0;
		for (int num = mTweens.Length; i < num; i++)
		{
			UITweener uITweener = mTweens[i];
			if (uITweener.tweenGroup == tweenGroup)
			{
				if (((Behaviour)uITweener).enabled)
				{
					flag = false;
					break;
				}
				if (uITweener.direction != (Direction)disableWhenFinished)
				{
					flag2 = false;
				}
			}
		}
		if (flag)
		{
			if (flag2)
			{
				NGUITools.SetActive(tweenTarget, state: false);
			}
			mTweens = null;
		}
	}

	public void Play(bool forward)
	{
		GameObject val = ((!((Object)(object)tweenTarget == (Object)null)) ? tweenTarget : ((Component)this).gameObject);
		if (!NGUITools.GetActive(val))
		{
			if (ifDisabledOnPlay != EnableCondition.EnableThenPlay)
			{
				return;
			}
			NGUITools.SetActive(val, state: true);
		}
		mTweens = ((!includeChildren) ? val.GetComponents<UITweener>() : val.GetComponentsInChildren<UITweener>());
		if (mTweens.Length == 0)
		{
			if (disableWhenFinished != DisableCondition.DoNotDisable)
			{
				NGUITools.SetActive(tweenTarget, state: false);
			}
			return;
		}
		bool flag = false;
		if (playDirection == Direction.Reverse)
		{
			forward = !forward;
		}
		int i = 0;
		for (int num = mTweens.Length; i < num; i++)
		{
			UITweener uITweener = mTweens[i];
			if (uITweener.tweenGroup == tweenGroup)
			{
				if (!flag && !NGUITools.GetActive(val))
				{
					flag = true;
					NGUITools.SetActive(val, state: true);
				}
				if (playDirection == Direction.Toggle)
				{
					uITweener.Toggle();
				}
				else
				{
					uITweener.Play(forward);
				}
				if (resetOnPlay)
				{
					uITweener.Reset();
				}
				uITweener.onFinished = onFinished;
				if ((Object)(object)eventReceiver != (Object)null && !string.IsNullOrEmpty(callWhenFinished))
				{
					uITweener.eventReceiver = eventReceiver;
					uITweener.callWhenFinished = callWhenFinished;
				}
			}
		}
	}
}
