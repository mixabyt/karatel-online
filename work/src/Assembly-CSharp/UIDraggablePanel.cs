using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Draggable Panel")]
[RequireComponent(typeof(UIPanel))]
[ExecuteInEditMode]
public class UIDraggablePanel : IgnoreTimeScale
{
	public enum DragEffect
	{
		None,
		Momentum,
		MomentumAndSpring
	}

	public enum ShowCondition
	{
		Always,
		OnlyIfNeeded,
		WhenDragging
	}

	public delegate void OnDragFinished();

	public bool restrictWithinPanel = true;

	public bool disableDragIfFits;

	public DragEffect dragEffect = DragEffect.MomentumAndSpring;

	public bool smoothDragStart = true;

	public Vector3 scale = Vector3.one;

	public float scrollWheelFactor;

	public float momentumAmount = 35f;

	public Vector2 relativePositionOnReset = Vector2.zero;

	public bool repositionClipping;

	public bool iOSDragEmulation = true;

	public UIScrollBar horizontalScrollBar;

	public UISlider verticalScrollBar;

	public float verticalScrollBarValue;

	public ShowCondition showScrollBars = ShowCondition.OnlyIfNeeded;

	public OnDragFinished onDragFinished;

	private Transform mTrans;

	private UIPanel mPanel;

	private Plane mPlane;

	private Vector3 mLastPos;

	private bool mPressed;

	private Vector3 mMomentum = Vector3.zero;

	private float mScroll;

	private Bounds mBounds;

	private bool mCalculatedBounds;

	private bool mShouldMove;

	private bool mIgnoreCallbacks;

	private int mDragID = -10;

	private Vector2 mDragStartOffset = Vector2.zero;

	private bool mDragStarted;

	public UIPanel panel => mPanel;

	public Bounds bounds
	{
		get
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			if (!mCalculatedBounds)
			{
				mCalculatedBounds = true;
				mBounds = NGUIMath.CalculateRelativeWidgetBounds(mTrans, mTrans);
			}
			return mBounds;
		}
	}

	public bool shouldMoveHorizontally
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			Bounds val = bounds;
			float num = val.size.x;
			if (mPanel.clipping == UIDrawCall.Clipping.SoftClip)
			{
				num += mPanel.clipSoftness.x * 2f;
			}
			return num > mPanel.clipRange.z;
		}
	}

	public bool shouldMoveVertically
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			Bounds val = bounds;
			float num = val.size.y;
			if (mPanel.clipping == UIDrawCall.Clipping.SoftClip)
			{
				num += mPanel.clipSoftness.y * 2f;
			}
			return num > mPanel.clipRange.w;
		}
	}

	private bool shouldMove
	{
		get
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			if (!disableDragIfFits)
			{
				return true;
			}
			if ((Object)(object)mPanel == (Object)null)
			{
				mPanel = ((Component)this).GetComponent<UIPanel>();
			}
			Vector4 clipRange = mPanel.clipRange;
			Bounds val = bounds;
			float num = ((clipRange.z != 0f) ? (clipRange.z * 0.5f) : ((float)Screen.width));
			float num2 = ((clipRange.w != 0f) ? (clipRange.w * 0.5f) : ((float)Screen.height));
			if (!Mathf.Approximately(scale.x, 0f))
			{
				if (val.min.x < clipRange.x - num)
				{
					return true;
				}
				if (val.max.x > clipRange.x + num)
				{
					return true;
				}
			}
			if (!Mathf.Approximately(scale.y, 0f))
			{
				if (val.min.y < clipRange.y - num2)
				{
					return true;
				}
				if (val.max.y > clipRange.y + num2)
				{
					return true;
				}
			}
			return false;
		}
	}

	public Vector3 currentMomentum
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mMomentum;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			mMomentum = value;
			mShouldMove = true;
		}
	}

	public UIDraggablePanel()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
	}

	private void Awake()
	{
		mTrans = ((Component)this).transform;
		mPanel = ((Component)this).GetComponent<UIPanel>();
		UIPanel uIPanel = mPanel;
		uIPanel.onChange = (UIPanel.OnChangeDelegate)Delegate.Combine(uIPanel.onChange, new UIPanel.OnChangeDelegate(OnPanelChange));
	}

	private void OnDestroy()
	{
		if ((Object)(object)mPanel != (Object)null)
		{
			UIPanel uIPanel = mPanel;
			uIPanel.onChange = (UIPanel.OnChangeDelegate)Delegate.Remove(uIPanel.onChange, new UIPanel.OnChangeDelegate(OnPanelChange));
		}
	}

	private void OnPanelChange()
	{
		UpdateScrollbars(recalculateBounds: true);
	}

	private void Start()
	{
		UpdateScrollbars(recalculateBounds: true);
		if ((Object)(object)horizontalScrollBar != (Object)null)
		{
			UIScrollBar uIScrollBar = horizontalScrollBar;
			uIScrollBar.onChange = (UIScrollBar.OnScrollBarChange)Delegate.Combine(uIScrollBar.onChange, new UIScrollBar.OnScrollBarChange(OnHorizontalBar));
			horizontalScrollBar.alpha = ((showScrollBars != ShowCondition.Always && !shouldMoveHorizontally) ? 0f : 1f);
		}
		if ((Object)(object)verticalScrollBar != (Object)null)
		{
			verticalScrollBarValue = verticalScrollBar.sliderValue;
		}
	}

	public bool RestrictWithinBounds(bool instant)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		Bounds val = bounds;
		if (val.size.y < mPanel.clipRange.w)
		{
			val.min = new Vector3(val.min.x, val.min.y - (mPanel.clipRange.w - val.size.y), val.min.z);
		}
		Vector3 val2 = mPanel.CalculateConstrainOffset(Vector2.op_Implicit(val.min), Vector2.op_Implicit(val.max));
		if (val2.magnitude > 0.001f)
		{
			if (!instant && dragEffect == DragEffect.MomentumAndSpring)
			{
				SpringPanel.Begin(((Component)mPanel).gameObject, mTrans.localPosition + val2, 13f);
			}
			else
			{
				MoveRelative(val2);
				mMomentum = Vector3.zero;
				mScroll = 0f;
			}
			return true;
		}
		return false;
	}

	public void DisableSpring()
	{
		SpringPanel component = ((Component)this).GetComponent<SpringPanel>();
		if ((Object)(object)component != (Object)null)
		{
			((Behaviour)component).enabled = false;
		}
	}

	public void UpdateScrollbars(bool recalculateBounds)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)mPanel == (Object)null)
		{
			return;
		}
		if ((Object)(object)horizontalScrollBar != (Object)null || (Object)(object)verticalScrollBar != (Object)null)
		{
			if (recalculateBounds)
			{
				mCalculatedBounds = false;
				mShouldMove = shouldMove;
			}
			Bounds val = bounds;
			Vector2 val2 = Vector2.op_Implicit(val.min);
			Vector2 val3 = Vector2.op_Implicit(val.max);
			if (mPanel.clipping == UIDrawCall.Clipping.SoftClip)
			{
				Vector2 clipSoftness = mPanel.clipSoftness;
				val2 -= clipSoftness;
				val3 += clipSoftness;
			}
			if ((Object)(object)horizontalScrollBar != (Object)null && val3.x > val2.x)
			{
				Vector4 clipRange = mPanel.clipRange;
				float num = clipRange.z * 0.5f;
				float num2 = clipRange.x - num - val.min.x;
				float num3 = val.max.x - num - clipRange.x;
				float num4 = val3.x - val2.x;
				num2 = Mathf.Clamp01(num2 / num4);
				num3 = Mathf.Clamp01(num3 / num4);
				float num5 = num2 + num3;
				mIgnoreCallbacks = true;
				horizontalScrollBar.barSize = 1f - num5;
				horizontalScrollBar.scrollValue = ((!(num5 > 0.001f)) ? 0f : (num2 / num5));
				mIgnoreCallbacks = false;
			}
			if ((Object)(object)verticalScrollBar != (Object)null && val3.y > val2.y)
			{
				Vector4 clipRange2 = mPanel.clipRange;
				float num6 = clipRange2.w * 0.5f;
				float num7 = clipRange2.y - num6 - val2.y;
				float num8 = val3.y - num6 - clipRange2.y;
				float num9 = val3.y - val2.y;
				num7 = Mathf.Clamp01(num7 / num9);
				num8 = Mathf.Clamp01(num8 / num9);
				float num10 = num7 + num8;
				mIgnoreCallbacks = true;
				verticalScrollBar.sliderValue = 1f - ((!(num10 > 0.001f)) ? 0f : (1f - num7 / num10));
				verticalScrollBarValue = verticalScrollBar.sliderValue;
				mIgnoreCallbacks = false;
			}
		}
		else if (recalculateBounds)
		{
			mCalculatedBounds = false;
		}
	}

	public void SetDragAmount(float x, float y, bool updateScrollbars)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		DisableSpring();
		Bounds val = bounds;
		if (val.min.x == val.max.x || val.min.y == val.max.y)
		{
			return;
		}
		Vector4 clipRange = mPanel.clipRange;
		float num = clipRange.z * 0.5f;
		float num2 = clipRange.w * 0.5f;
		float num3 = val.min.x + num;
		float num4 = val.max.x - num;
		float num5 = val.min.y + num2;
		float num6 = val.max.y - num2;
		if (mPanel.clipping == UIDrawCall.Clipping.SoftClip)
		{
			num3 -= mPanel.clipSoftness.x;
			num4 += mPanel.clipSoftness.x;
			num5 -= mPanel.clipSoftness.y;
			num6 += mPanel.clipSoftness.y;
		}
		float num7 = Mathf.Lerp(num3, num4, x);
		float num8 = Mathf.Lerp(num6, num5, y);
		if (!updateScrollbars)
		{
			Vector3 localPosition = mTrans.localPosition;
			if (scale.x != 0f)
			{
				localPosition.x += clipRange.x - num7;
			}
			if (scale.y != 0f)
			{
				localPosition.y += clipRange.y - num8;
			}
			mTrans.localPosition = localPosition;
		}
		clipRange.x = num7;
		clipRange.y = num8;
		mPanel.clipRange = clipRange;
		if (updateScrollbars)
		{
			UpdateScrollbars(recalculateBounds: false);
		}
	}

	public void ResetPosition()
	{
		mCalculatedBounds = false;
		SetDragAmount(relativePositionOnReset.x, relativePositionOnReset.y, updateScrollbars: false);
		SetDragAmount(relativePositionOnReset.x, relativePositionOnReset.y, updateScrollbars: true);
	}

	private void OnHorizontalBar(UIScrollBar sb)
	{
		if (mIgnoreCallbacks)
		{
		}
	}

	private void OnVerticalBar(float val)
	{
		if (!mIgnoreCallbacks)
		{
			float x = ((!((Object)(object)horizontalScrollBar != (Object)null)) ? 0f : horizontalScrollBar.scrollValue);
			float y = ((!((Object)(object)verticalScrollBar != (Object)null)) ? 0f : (1f - val));
			SetDragAmount(x, y, updateScrollbars: false);
		}
	}

	public void MoveRelative(Vector3 relative)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		Transform val = mTrans;
		val.localPosition += relative;
		Vector4 clipRange = mPanel.clipRange;
		clipRange.x -= relative.x;
		clipRange.y -= relative.y;
		mPanel.clipRange = clipRange;
		UpdateScrollbars(recalculateBounds: false);
	}

	public void MoveAbsolute(Vector3 absolute)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = mTrans.InverseTransformPoint(absolute);
		Vector3 val2 = mTrans.InverseTransformPoint(Vector3.zero);
		MoveRelative(val - val2);
	}

	public void Press(bool pressed)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		if (smoothDragStart && pressed)
		{
			mDragStarted = false;
			mDragStartOffset = Vector2.zero;
		}
		if (!((Behaviour)this).enabled || !NGUITools.GetActive(((Component)this).gameObject))
		{
			return;
		}
		if (!pressed && mDragID == UICamera.currentTouchID)
		{
			mDragID = -10;
		}
		mCalculatedBounds = false;
		mShouldMove = shouldMove;
		if (!mShouldMove)
		{
			return;
		}
		mPressed = pressed;
		if (pressed)
		{
			mMomentum = Vector3.zero;
			mScroll = 0f;
			DisableSpring();
			mLastPos = UICamera.lastHit.point;
			mPlane = new Plane(mTrans.rotation * Vector3.back, mLastPos);
			return;
		}
		if (restrictWithinPanel && mPanel.clipping != UIDrawCall.Clipping.None && dragEffect == DragEffect.MomentumAndSpring)
		{
			RestrictWithinBounds(instant: false);
		}
		if (onDragFinished != null)
		{
			onDragFinished();
		}
	}

	public void Drag()
	{
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		if (!((Behaviour)this).enabled || !NGUITools.GetActive(((Component)this).gameObject) || !mShouldMove)
		{
			return;
		}
		if (mDragID == -10)
		{
			mDragID = UICamera.currentTouchID;
		}
		UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;
		if (smoothDragStart && !mDragStarted)
		{
			mDragStarted = true;
			mDragStartOffset = UICamera.currentTouch.totalDelta;
		}
		Ray val = ((!smoothDragStart) ? UICamera.currentCamera.ScreenPointToRay(Vector2.op_Implicit(UICamera.currentTouch.pos)) : UICamera.currentCamera.ScreenPointToRay(Vector2.op_Implicit(UICamera.currentTouch.pos - mDragStartOffset)));
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
			val2 = mTrans.InverseTransformDirection(val2);
			val2.Scale(scale);
			val2 = mTrans.TransformDirection(val2);
		}
		mMomentum = Vector3.Lerp(mMomentum, mMomentum + val2 * (0.01f * momentumAmount), 0.67f);
		if (!iOSDragEmulation)
		{
			MoveAbsolute(val2);
		}
		else
		{
			UIPanel uIPanel = mPanel;
			Bounds val3 = bounds;
			Vector2 min = Vector2.op_Implicit(val3.min);
			Bounds val4 = bounds;
			Vector3 val5 = uIPanel.CalculateConstrainOffset(min, Vector2.op_Implicit(val4.max));
			if (val5.magnitude > 0.001f)
			{
				MoveAbsolute(val2 * 0.5f);
				mMomentum *= 0.5f;
			}
			else
			{
				MoveAbsolute(val2);
			}
		}
		if (restrictWithinPanel && mPanel.clipping != UIDrawCall.Clipping.None && dragEffect != DragEffect.MomentumAndSpring)
		{
			RestrictWithinBounds(instant: true);
		}
	}

	public void Scroll(float delta)
	{
		if (((Behaviour)this).enabled && NGUITools.GetActive(((Component)this).gameObject) && scrollWheelFactor != 0f)
		{
			DisableSpring();
			mShouldMove = shouldMove;
			if (Mathf.Sign(mScroll) != Mathf.Sign(delta))
			{
				mScroll = 0f;
			}
			mScroll += delta * scrollWheelFactor;
		}
	}

	private void LateUpdate()
	{
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)verticalScrollBar != (Object)null && ((Component)verticalScrollBar).gameObject.activeInHierarchy && verticalScrollBar.sliderValue != verticalScrollBarValue)
		{
			verticalScrollBarValue = verticalScrollBar.sliderValue;
			OnVerticalBar(verticalScrollBarValue);
		}
		if (repositionClipping)
		{
			repositionClipping = false;
			mCalculatedBounds = false;
			SetDragAmount(relativePositionOnReset.x, relativePositionOnReset.y, updateScrollbars: true);
		}
		if (!Application.isPlaying)
		{
			return;
		}
		float num = UpdateRealTimeDelta();
		if (showScrollBars != ShowCondition.Always)
		{
			bool flag = false;
			if (showScrollBars != ShowCondition.WhenDragging || mDragID != -10 || mMomentum.magnitude > 0.01f)
			{
				flag = shouldMoveHorizontally;
			}
			if (Object.op_Implicit((Object)(object)horizontalScrollBar))
			{
				float alpha = horizontalScrollBar.alpha;
				alpha += ((!flag) ? ((0f - num) * 3f) : (num * 6f));
				alpha = Mathf.Clamp01(alpha);
				if (horizontalScrollBar.alpha != alpha)
				{
					horizontalScrollBar.alpha = alpha;
				}
			}
		}
		if (mShouldMove && !mPressed)
		{
			mMomentum -= scale * (mScroll * 0.05f);
			if (mMomentum.magnitude > 0.0001f)
			{
				mScroll = NGUIMath.SpringLerp(mScroll, 0f, 20f, num);
				Vector3 absolute = NGUIMath.SpringDampen(ref mMomentum, 9f, num);
				MoveAbsolute(absolute);
				if (restrictWithinPanel && mPanel.clipping != UIDrawCall.Clipping.None)
				{
					RestrictWithinBounds(instant: false);
				}
				if (mMomentum.magnitude < 0.0001f && onDragFinished != null)
				{
					onDragFinished();
				}
				return;
			}
			mScroll = 0f;
			mMomentum = Vector3.zero;
		}
		else
		{
			mScroll = 0f;
		}
		NGUIMath.SpringDampen(ref mMomentum, 9f, num);
	}
}
