using AnimationOrTween;
using UnityEngine;

[RequireComponent(typeof(Animation))]
[AddComponentMenu("NGUI/Internal/Active Animation")]
public class ActiveAnimation : IgnoreTimeScale
{
	public delegate void OnFinished(ActiveAnimation anim);

	public OnFinished onFinished;

	public GameObject eventReceiver;

	public string callWhenFinished;

	private Animation mAnim;

	private Direction mLastDirection;

	private Direction mDisableDirection;

	private bool mNotify;

	public bool isPlaying
	{
		get
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Expected Obj, but got Unknown
			if ((Object)(object)mAnim == (Object)null)
			{
				return false;
			}
			foreach (AnimationState item in mAnim)
			{
				AnimationState val = item;
				if (!mAnim.IsPlaying(val.name))
				{
					continue;
				}
				if (mLastDirection == Direction.Forward)
				{
					if (!(val.time < val.length))
					{
						continue;
					}
					return true;
				}
				if (mLastDirection == Direction.Reverse)
				{
					if (!(val.time > 0f))
					{
						continue;
					}
					return true;
				}
				return true;
			}
			return false;
		}
	}

	public void Reset()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected Obj, but got Unknown
		if (!((Object)(object)mAnim != (Object)null))
		{
			return;
		}
		foreach (AnimationState item in mAnim)
		{
			AnimationState val = item;
			if (mLastDirection == Direction.Reverse)
			{
				val.time = val.length;
			}
			else if (mLastDirection == Direction.Forward)
			{
				val.time = 0f;
			}
		}
	}

	private void Update()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected Obj, but got Unknown
		float num = UpdateRealTimeDelta();
		if (num == 0f)
		{
			return;
		}
		if ((Object)(object)mAnim != (Object)null)
		{
			bool flag = false;
			foreach (AnimationState item in mAnim)
			{
				AnimationState val = item;
				if (!mAnim.IsPlaying(val.name))
				{
					continue;
				}
				float num2 = val.speed * num;
				val.time += num2;
				if (num2 < 0f)
				{
					if (val.time > 0f)
					{
						flag = true;
					}
					else
					{
						val.time = 0f;
					}
				}
				else if (val.time < val.length)
				{
					flag = true;
				}
				else
				{
					val.time = val.length;
				}
			}
			mAnim.Sample();
			if (flag)
			{
				return;
			}
			((Behaviour)this).enabled = false;
			if (mNotify)
			{
				mNotify = false;
				if (onFinished != null)
				{
					onFinished(this);
				}
				if ((Object)(object)eventReceiver != (Object)null && !string.IsNullOrEmpty(callWhenFinished))
				{
					eventReceiver.SendMessage(callWhenFinished, (object)this, (SendMessageOptions)1);
				}
				if (mDisableDirection != Direction.Toggle && mLastDirection == mDisableDirection)
				{
					NGUITools.SetActive(((Component)this).gameObject, state: false);
				}
			}
		}
		else
		{
			((Behaviour)this).enabled = false;
		}
	}

	private void Play(string clipName, Direction playDirection)
	{
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Expected Obj, but got Unknown
		if (!((Object)(object)mAnim != (Object)null))
		{
			return;
		}
		((Behaviour)this).enabled = true;
		((Behaviour)mAnim).enabled = false;
		if (playDirection == Direction.Toggle)
		{
			playDirection = ((mLastDirection != Direction.Forward) ? Direction.Forward : Direction.Reverse);
		}
		if (string.IsNullOrEmpty(clipName))
		{
			if (!mAnim.isPlaying)
			{
				mAnim.Play();
			}
		}
		else if (!mAnim.IsPlaying(clipName))
		{
			mAnim.Play(clipName);
		}
		foreach (AnimationState item in mAnim)
		{
			AnimationState val = item;
			if (string.IsNullOrEmpty(clipName) || val.name == clipName)
			{
				float num = Mathf.Abs(val.speed);
				val.speed = num * (float)playDirection;
				if (playDirection == Direction.Reverse && val.time == 0f)
				{
					val.time = val.length;
				}
				else if (playDirection == Direction.Forward && val.time == val.length)
				{
					val.time = 0f;
				}
			}
		}
		mLastDirection = playDirection;
		mNotify = true;
		mAnim.Sample();
	}

	public static ActiveAnimation Play(Animation anim, string clipName, Direction playDirection, EnableCondition enableBeforePlay, DisableCondition disableCondition)
	{
		if (!NGUITools.GetActive(((Component)anim).gameObject))
		{
			if (enableBeforePlay != EnableCondition.EnableThenPlay)
			{
				return null;
			}
			NGUITools.SetActive(((Component)anim).gameObject, state: true);
			UIPanel[] componentsInChildren = ((Component)anim).gameObject.GetComponentsInChildren<UIPanel>();
			int i = 0;
			for (int num = componentsInChildren.Length; i < num; i++)
			{
				componentsInChildren[i].Refresh();
			}
		}
		ActiveAnimation activeAnimation = ((Component)anim).GetComponent<ActiveAnimation>();
		if ((Object)(object)activeAnimation == (Object)null)
		{
			activeAnimation = ((Component)anim).gameObject.AddComponent<ActiveAnimation>();
		}
		activeAnimation.mAnim = anim;
		activeAnimation.mDisableDirection = (Direction)disableCondition;
		activeAnimation.eventReceiver = null;
		activeAnimation.callWhenFinished = null;
		activeAnimation.onFinished = null;
		activeAnimation.Play(clipName, playDirection);
		return activeAnimation;
	}

	public static ActiveAnimation Play(Animation anim, string clipName, Direction playDirection)
	{
		return Play(anim, clipName, playDirection, EnableCondition.DoNothing, DisableCondition.DoNotDisable);
	}

	public static ActiveAnimation Play(Animation anim, Direction playDirection)
	{
		return Play(anim, null, playDirection, EnableCondition.DoNothing, DisableCondition.DoNotDisable);
	}
}
