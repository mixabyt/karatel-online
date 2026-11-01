using AnimationOrTween;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Checkbox")]
public class UICheckbox : MonoBehaviour
{
	public delegate void OnStateChange(bool state);

	public static UICheckbox current;

	public UISprite checkSprite;

	public Animation checkAnimation;

	public bool instantTween;

	public bool startsChecked = true;

	public Transform radioButtonRoot;

	public bool optionCanBeNone;

	public GameObject eventReceiver;

	public string functionName = "OnActivate";

	public OnStateChange onStateChange;

	[HideInInspector]
	[SerializeField]
	private bool option;

	private bool mChecked = true;

	private bool mStarted;

	private Transform mTrans;

	public bool isChecked
	{
		get
		{
			return mChecked;
		}
		set
		{
			if ((Object)(object)radioButtonRoot == (Object)null || value || optionCanBeNone || !mStarted)
			{
				Set(value);
			}
		}
	}

	private void Awake()
	{
		mTrans = ((Component)this).transform;
		if ((Object)(object)checkSprite != (Object)null)
		{
			checkSprite.alpha = ((!startsChecked) ? 0f : 1f);
		}
		if (option)
		{
			option = false;
			if ((Object)(object)radioButtonRoot == (Object)null)
			{
				radioButtonRoot = mTrans.parent;
			}
		}
	}

	private void Start()
	{
		if ((Object)(object)eventReceiver == (Object)null)
		{
			eventReceiver = ((Component)this).gameObject;
		}
		mChecked = !startsChecked;
		Set(startsChecked);
		mStarted = true;
	}

	private void OnClick()
	{
		if (((Behaviour)this).enabled)
		{
			isChecked = !isChecked;
		}
	}

	private void Set(bool state)
	{
		if (!mStarted)
		{
			mChecked = state;
			startsChecked = state;
			if ((Object)(object)checkSprite != (Object)null)
			{
				checkSprite.alpha = ((!state) ? 0f : 1f);
			}
		}
		else
		{
			if (mChecked == state)
			{
				return;
			}
			if ((Object)(object)radioButtonRoot != (Object)null && state)
			{
				UICheckbox[] componentsInChildren = ((Component)radioButtonRoot).GetComponentsInChildren<UICheckbox>(true);
				int i = 0;
				for (int num = componentsInChildren.Length; i < num; i++)
				{
					UICheckbox uICheckbox = componentsInChildren[i];
					if ((Object)(object)uICheckbox != (Object)(object)this && (Object)(object)uICheckbox.radioButtonRoot == (Object)(object)radioButtonRoot)
					{
						uICheckbox.Set(state: false);
					}
				}
			}
			mChecked = state;
			if ((Object)(object)checkSprite != (Object)null)
			{
				if (instantTween)
				{
					checkSprite.alpha = ((!mChecked) ? 0f : 1f);
				}
				else
				{
					TweenAlpha.Begin(((Component)checkSprite).gameObject, 0.15f, (!mChecked) ? 0f : 1f);
				}
			}
			current = this;
			if (onStateChange != null)
			{
				onStateChange(mChecked);
			}
			if ((Object)(object)eventReceiver != (Object)null && !string.IsNullOrEmpty(functionName))
			{
				eventReceiver.SendMessage(functionName, (object)mChecked, (SendMessageOptions)1);
			}
			current = null;
			if ((Object)(object)checkAnimation != (Object)null)
			{
				ActiveAnimation.Play(checkAnimation, state ? Direction.Forward : Direction.Reverse);
			}
		}
	}
}
