using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Scroll Bar")]
[ExecuteInEditMode]
public class UIScrollBar : MonoBehaviour
{
	public enum Direction
	{
		Horizontal,
		Vertical
	}

	public delegate void OnScrollBarChange(UIScrollBar sb);

	public delegate void OnDragFinished();

	[SerializeField]
	[HideInInspector]
	private UISprite mBG;

	[SerializeField]
	[HideInInspector]
	private UISprite mFG;

	[SerializeField]
	[HideInInspector]
	private Direction mDir;

	[HideInInspector]
	[SerializeField]
	private bool mInverted;

	[HideInInspector]
	[SerializeField]
	private float mScroll;

	[HideInInspector]
	[SerializeField]
	private float mSize = 1f;

	private Transform mTrans;

	private bool mIsDirty;

	private Camera mCam;

	private Vector2 mScreenPos = Vector2.zero;

	public OnScrollBarChange onChange;

	public OnDragFinished onDragFinished;

	public Transform cachedTransform
	{
		get
		{
			if ((Object)(object)mTrans == (Object)null)
			{
				mTrans = ((Component)this).transform;
			}
			return mTrans;
		}
	}

	public Camera cachedCamera
	{
		get
		{
			if ((Object)(object)mCam == (Object)null)
			{
				mCam = NGUITools.FindCameraForLayer(((Component)this).gameObject.layer);
			}
			return mCam;
		}
	}

	public UISprite background
	{
		get
		{
			return mBG;
		}
		set
		{
			if ((Object)(object)mBG != (Object)(object)value)
			{
				mBG = value;
				mIsDirty = true;
			}
		}
	}

	public UISprite foreground
	{
		get
		{
			return mFG;
		}
		set
		{
			if ((Object)(object)mFG != (Object)(object)value)
			{
				mFG = value;
				mIsDirty = true;
			}
		}
	}

	public Direction direction
	{
		get
		{
			return mDir;
		}
		set
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			if (mDir == value)
			{
				return;
			}
			mDir = value;
			mIsDirty = true;
			if (!((Object)(object)mBG != (Object)null))
			{
				return;
			}
			Transform val = mBG.cachedTransform;
			Vector3 localScale = val.localScale;
			if ((mDir == Direction.Vertical && localScale.x > localScale.y) || (mDir == Direction.Horizontal && localScale.x < localScale.y))
			{
				float x = localScale.x;
				localScale.x = localScale.y;
				localScale.y = x;
				val.localScale = localScale;
				ForceUpdate();
				if ((Object)(object)((Component)mBG).collider != (Object)null)
				{
					NGUITools.AddWidgetCollider(((Component)mBG).gameObject);
				}
				if ((Object)(object)((Component)mFG).collider != (Object)null)
				{
					NGUITools.AddWidgetCollider(((Component)mFG).gameObject);
				}
			}
		}
	}

	public bool inverted
	{
		get
		{
			return mInverted;
		}
		set
		{
			if (mInverted != value)
			{
				mInverted = value;
				mIsDirty = true;
			}
		}
	}

	public float scrollValue
	{
		get
		{
			return mScroll;
		}
		set
		{
			float num = Mathf.Clamp01(value);
			if (mScroll != num)
			{
				mScroll = num;
				mIsDirty = true;
				if (onChange != null)
				{
					onChange(this);
				}
			}
		}
	}

	public float barSize
	{
		get
		{
			return mSize;
		}
		set
		{
			float num = Mathf.Clamp01(value);
			if (mSize != num)
			{
				mSize = num;
				mIsDirty = true;
				if (onChange != null)
				{
					onChange(this);
				}
			}
		}
	}

	public float alpha
	{
		get
		{
			if ((Object)(object)mFG != (Object)null)
			{
				return mFG.alpha;
			}
			if ((Object)(object)mBG != (Object)null)
			{
				return mBG.alpha;
			}
			return 0f;
		}
		set
		{
			if ((Object)(object)mFG != (Object)null)
			{
				mFG.alpha = value;
				NGUITools.SetActiveSelf(((Component)mFG).gameObject, mFG.alpha > 0.001f);
			}
			if ((Object)(object)mBG != (Object)null)
			{
				mBG.alpha = value;
				NGUITools.SetActiveSelf(((Component)mBG).gameObject, mBG.alpha > 0.001f);
			}
		}
	}

	public UIScrollBar()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
	}

	private void CenterOnPos(Vector2 localPos)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)mBG == (Object)null) && !((Object)(object)mFG == (Object)null))
		{
			Bounds val = NGUIMath.CalculateRelativeInnerBounds(cachedTransform, mBG);
			Bounds val2 = NGUIMath.CalculateRelativeInnerBounds(cachedTransform, mFG);
			if (mDir == Direction.Horizontal)
			{
				float num = val.size.x - val2.size.x;
				float num2 = num * 0.5f;
				float num3 = val.center.x - num2;
				float num4 = ((!(num > 0f)) ? 0f : ((localPos.x - num3) / num));
				scrollValue = ((!mInverted) ? num4 : (1f - num4));
			}
			else
			{
				float num5 = val.size.y - val2.size.y;
				float num6 = num5 * 0.5f;
				float num7 = val.center.y - num6;
				float num8 = ((!(num5 > 0f)) ? 0f : (1f - (localPos.y - num7) / num5));
				scrollValue = ((!mInverted) ? num8 : (1f - num8));
			}
		}
	}

	private void Reposition(Vector2 screenPos)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		Transform val = cachedTransform;
		Plane val2 = new Plane(val.rotation * Vector3.back, val.position);
		Ray val3 = cachedCamera.ScreenPointToRay(Vector2.op_Implicit(screenPos));
		float num = default;
		if (val2.Raycast(val3, ref num))
		{
			CenterOnPos(Vector2.op_Implicit(val.InverseTransformPoint(val3.GetPoint(num))));
		}
	}

	private void OnPressBackground(GameObject go, bool isPressed)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		mCam = UICamera.currentCamera;
		Reposition(UICamera.lastTouchPosition);
		if (!isPressed && onDragFinished != null)
		{
			onDragFinished();
		}
	}

	private void OnDragBackground(GameObject go, Vector2 delta)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		mCam = UICamera.currentCamera;
		Reposition(UICamera.lastTouchPosition);
	}

	private void OnPressForeground(GameObject go, bool isPressed)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (isPressed)
		{
			mCam = UICamera.currentCamera;
			Bounds val = NGUIMath.CalculateAbsoluteWidgetBounds(mFG.cachedTransform);
			mScreenPos = Vector2.op_Implicit(mCam.WorldToScreenPoint(val.center));
		}
		else if (onDragFinished != null)
		{
			onDragFinished();
		}
	}

	private void OnDragForeground(GameObject go, Vector2 delta)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		mCam = UICamera.currentCamera;
		Reposition(mScreenPos + UICamera.currentTouch.totalDelta);
	}

	private void Start()
	{
		if ((Object)(object)background != (Object)null && (Object)(object)((Component)background).collider != (Object)null)
		{
			UIEventListener uIEventListener = UIEventListener.Get(((Component)background).gameObject);
			uIEventListener.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uIEventListener.onPress, new UIEventListener.BoolDelegate(OnPressBackground));
			uIEventListener.onDrag = (UIEventListener.VectorDelegate)Delegate.Combine(uIEventListener.onDrag, new UIEventListener.VectorDelegate(OnDragBackground));
		}
		if ((Object)(object)foreground != (Object)null && (Object)(object)((Component)foreground).collider != (Object)null)
		{
			UIEventListener uIEventListener2 = UIEventListener.Get(((Component)foreground).gameObject);
			uIEventListener2.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uIEventListener2.onPress, new UIEventListener.BoolDelegate(OnPressForeground));
			uIEventListener2.onDrag = (UIEventListener.VectorDelegate)Delegate.Combine(uIEventListener2.onDrag, new UIEventListener.VectorDelegate(OnDragForeground));
		}
		ForceUpdate();
	}

	private void Update()
	{
		if (mIsDirty)
		{
			ForceUpdate();
		}
	}

	public void ForceUpdate()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		mIsDirty = false;
		if (!((Object)(object)mBG != (Object)null) || !((Object)(object)mFG != (Object)null))
		{
			return;
		}
		mSize = Mathf.Clamp01(mSize);
		mScroll = Mathf.Clamp01(mScroll);
		Vector4 border = mBG.border;
		Vector4 border2 = mFG.border;
		Vector2 val = new Vector2(Mathf.Max(0f, mBG.cachedTransform.localScale.x - border.x - border.z), Mathf.Max(0f, mBG.cachedTransform.localScale.y - border.y - border.w));
		float num = ((!mInverted) ? mScroll : (1f - mScroll));
		if (mDir == Direction.Horizontal)
		{
			Vector2 val2 = new Vector2(val.x * mSize, val.y);
			mFG.pivot = UIWidget.Pivot.Left;
			mBG.pivot = UIWidget.Pivot.Left;
			mBG.cachedTransform.localPosition = Vector3.zero;
			mFG.cachedTransform.localPosition = new Vector3(border.x - border2.x + (val.x - val2.x) * num, 0f, 0f);
			mFG.cachedTransform.localScale = new Vector3(val2.x + border2.x + border2.z, val2.y + border2.y + border2.w, 1f);
			if (num < 0.999f && num > 0.001f)
			{
				mFG.MakePixelPerfect();
			}
		}
		else
		{
			Vector2 val3 = new Vector2(val.x, val.y * mSize);
			mFG.pivot = UIWidget.Pivot.Top;
			mBG.pivot = UIWidget.Pivot.Top;
			mBG.cachedTransform.localPosition = Vector3.zero;
			mFG.cachedTransform.localPosition = new Vector3(0f, 0f - border.y + border2.y - (val.y - val3.y) * num, 0f);
			mFG.cachedTransform.localScale = new Vector3(val3.x + border2.x + border2.z, val3.y + border2.y + border2.w, 1f);
			if (num < 0.999f && num > 0.001f)
			{
				mFG.MakePixelPerfect();
			}
		}
	}
}
