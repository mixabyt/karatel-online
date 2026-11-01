using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Drag Object")]
public class UIDragObject : IgnoreTimeScale
{
	public enum DragEffect
	{
		None,
		Momentum,
		MomentumAndSpring
	}

	public Transform target;

	public Vector3 scale = Vector3.one;

	public float scrollWheelFactor;

	public bool restrictWithinPanel;

	public DragEffect dragEffect = DragEffect.MomentumAndSpring;

	public float momentumAmount = 35f;

	private Plane mPlane;

	private Vector3 mLastPos;

	private UIPanel mPanel;

	private bool mPressed;

	private Vector3 mMomentum = Vector3.zero;

	private float mScroll;

	private Bounds mBounds;

	public UIDragObject()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
	}

	private void FindPanel()
	{
		mPanel = ((!((Object)(object)target != (Object)null)) ? null : UIPanel.Find(((Component)target).transform, createIfMissing: false));
		if ((Object)(object)mPanel == (Object)null)
		{
			restrictWithinPanel = false;
		}
	}

	private void OnPress(bool pressed)
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		if (!((Behaviour)this).enabled || !NGUITools.GetActive(((Component)this).gameObject) || !((Object)(object)target != (Object)null))
		{
			return;
		}
		mPressed = pressed;
		if (pressed)
		{
			if (restrictWithinPanel && (Object)(object)mPanel == (Object)null)
			{
				FindPanel();
			}
			if (restrictWithinPanel)
			{
				mBounds = NGUIMath.CalculateRelativeWidgetBounds(mPanel.cachedTransform, target);
			}
			mMomentum = Vector3.zero;
			mScroll = 0f;
			SpringPosition component = ((Component)target).GetComponent<SpringPosition>();
			if ((Object)(object)component != (Object)null)
			{
				((Behaviour)component).enabled = false;
			}
			mLastPos = UICamera.lastHit.point;
			Transform transform = ((Component)UICamera.currentCamera).transform;
			mPlane = new Plane(((!((Object)(object)mPanel != (Object)null)) ? transform.rotation : mPanel.cachedTransform.rotation) * Vector3.back, mLastPos);
		}
		else if (restrictWithinPanel && mPanel.clipping != UIDrawCall.Clipping.None && dragEffect == DragEffect.MomentumAndSpring)
		{
			mPanel.ConstrainTargetToBounds(target, ref mBounds, immediate: false);
		}
	}

	private void OnDrag(Vector2 delta)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		if (!((Behaviour)this).enabled || !NGUITools.GetActive(((Component)this).gameObject) || !((Object)(object)target != (Object)null))
		{
			return;
		}
		UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;
		Ray val = UICamera.currentCamera.ScreenPointToRay(Vector2.op_Implicit(UICamera.currentTouch.pos));
		float num = 0f;
		if (!mPlane.Raycast(val, ref num))
		{
			return;
		}
		Vector3 point = val.GetPoint(num);
		Vector3 val2 = point - mLastPos;
		mLastPos = point;
		if (val2.x != 0f || val2.y != 0f)
		{
			val2 = target.InverseTransformDirection(val2);
			val2.Scale(scale);
			val2 = target.TransformDirection(val2);
		}
		if (dragEffect != DragEffect.None)
		{
			mMomentum = Vector3.Lerp(mMomentum, mMomentum + val2 * (0.01f * momentumAmount), 0.67f);
		}
		if (restrictWithinPanel)
		{
			Vector3 localPosition = target.localPosition;
			Transform val3 = target;
			val3.position += val2;
			mBounds.center += target.localPosition - localPosition;
			if (dragEffect != DragEffect.MomentumAndSpring && mPanel.clipping != UIDrawCall.Clipping.None && mPanel.ConstrainTargetToBounds(target, ref mBounds, immediate: true))
			{
				mMomentum = Vector3.zero;
				mScroll = 0f;
			}
		}
		else
		{
			Transform val4 = target;
			val4.position += val2;
		}
	}

	private void LateUpdate()
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		float deltaTime = UpdateRealTimeDelta();
		if ((Object)(object)target == (Object)null)
		{
			return;
		}
		if (mPressed)
		{
			SpringPosition component = ((Component)target).GetComponent<SpringPosition>();
			if ((Object)(object)component != (Object)null)
			{
				((Behaviour)component).enabled = false;
			}
			mScroll = 0f;
		}
		else
		{
			mMomentum += scale * ((0f - mScroll) * 0.05f);
			mScroll = NGUIMath.SpringLerp(mScroll, 0f, 20f, deltaTime);
			if (mMomentum.magnitude > 0.0001f)
			{
				if ((Object)(object)mPanel == (Object)null)
				{
					FindPanel();
				}
				if ((Object)(object)mPanel != (Object)null)
				{
					Transform val = target;
					val.position += NGUIMath.SpringDampen(ref mMomentum, 9f, deltaTime);
					if (!restrictWithinPanel || mPanel.clipping == UIDrawCall.Clipping.None)
					{
						return;
					}
					mBounds = NGUIMath.CalculateRelativeWidgetBounds(mPanel.cachedTransform, target);
					if (!mPanel.ConstrainTargetToBounds(target, ref mBounds, dragEffect == DragEffect.None))
					{
						SpringPosition component2 = ((Component)target).GetComponent<SpringPosition>();
						if ((Object)(object)component2 != (Object)null)
						{
							((Behaviour)component2).enabled = false;
						}
					}
					return;
				}
			}
			else
			{
				mScroll = 0f;
			}
		}
		NGUIMath.SpringDampen(ref mMomentum, 9f, deltaTime);
	}

	private void OnScroll(float delta)
	{
		if (((Behaviour)this).enabled && NGUITools.GetActive(((Component)this).gameObject))
		{
			if (Mathf.Sign(mScroll) != Mathf.Sign(delta))
			{
				mScroll = 0f;
			}
			mScroll += delta * scrollWheelFactor;
		}
	}
}
