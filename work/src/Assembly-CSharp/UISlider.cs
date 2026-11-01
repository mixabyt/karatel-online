using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Slider")]
public class UISlider : IgnoreTimeScale
{
	public enum Direction
	{
		Horizontal,
		Vertical
	}

	public delegate void OnValueChange(float val);

	public static UISlider current;

	public Transform foreground;

	public Transform thumb;

	public Direction direction;

	public GameObject eventReceiver;

	public string functionName = "OnSliderChange";

	public OnValueChange onValueChange;

	public int numberOfSteps;

	[SerializeField]
	[HideInInspector]
	private float rawValue = 1f;

	private BoxCollider mCol;

	private Transform mTrans;

	private Transform mFGTrans;

	private UIWidget mFGWidget;

	private UISprite mFGFilled;

	private bool mInitDone;

	private Vector2 mSize = Vector2.zero;

	private Vector2 mCenter = Vector2.op_Implicit(Vector3.zero);

	public float sliderValue
	{
		get
		{
			float num = rawValue;
			if (numberOfSteps > 1)
			{
				num = Mathf.Round(num * (float)(numberOfSteps - 1)) / (float)(numberOfSteps - 1);
			}
			return num;
		}
		set
		{
			Set(value, force: false);
		}
	}

	public Vector2 fullSize
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mSize;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (mSize != value)
			{
				mSize = value;
				ForceUpdate();
			}
		}
	}

	public UISlider()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
	}

	private void Init()
	{
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		mInitDone = true;
		if ((Object)(object)foreground != (Object)null)
		{
			mFGWidget = ((Component)foreground).GetComponent<UIWidget>();
			mFGFilled = ((!((Object)(object)mFGWidget != (Object)null)) ? null : (mFGWidget as UISprite));
			mFGTrans = ((Component)foreground).transform;
			if (mSize == Vector2.zero)
			{
				mSize = Vector2.op_Implicit(foreground.localScale);
			}
			if (mCenter == Vector2.zero)
			{
				mCenter = Vector2.op_Implicit(foreground.localPosition + foreground.localScale * 0.5f);
			}
		}
		else if ((Object)(object)mCol != (Object)null)
		{
			if (mSize == Vector2.zero)
			{
				mSize = Vector2.op_Implicit(mCol.size);
			}
			if (mCenter == Vector2.zero)
			{
				mCenter = Vector2.op_Implicit(mCol.center);
			}
		}
		else
		{
			Debug.LogWarning((object)"UISlider expected to find a foreground object or a box collider to work with", (Object)(object)this);
		}
	}

	private void Awake()
	{
		mTrans = ((Component)this).transform;
		Collider collider = ((Component)this).collider;
		mCol = (BoxCollider)(object)((collider is BoxCollider) ? collider : null);
	}

	private void Start()
	{
		Init();
		if (Application.isPlaying && (Object)(object)thumb != (Object)null && (Object)(object)((Component)thumb).collider != (Object)null)
		{
			UIEventListener uIEventListener = UIEventListener.Get(((Component)thumb).gameObject);
			uIEventListener.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uIEventListener.onPress, new UIEventListener.BoolDelegate(OnPressThumb));
			uIEventListener.onDrag = (UIEventListener.VectorDelegate)Delegate.Combine(uIEventListener.onDrag, new UIEventListener.VectorDelegate(OnDragThumb));
		}
		Set(rawValue, force: true);
	}

	private void OnPress(bool pressed)
	{
		if (pressed && UICamera.currentTouchID != -100)
		{
			UpdateDrag();
		}
	}

	private void OnDrag(Vector2 delta)
	{
		UpdateDrag();
	}

	private void OnPressThumb(GameObject go, bool pressed)
	{
		if (pressed)
		{
			UpdateDrag();
		}
	}

	private void OnDragThumb(GameObject go, Vector2 delta)
	{
		UpdateDrag();
	}

	private void OnKey(KeyCode key)
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Invalid comparison between Unknown and I4
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Invalid comparison between Unknown and I4
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Invalid comparison between Unknown and I4
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Invalid comparison between Unknown and I4
		float num = ((!((float)numberOfSteps > 1f)) ? 0.125f : (1f / (float)(numberOfSteps - 1)));
		if (direction == Direction.Horizontal)
		{
			if ((int)key == 276)
			{
				Set(rawValue - num, force: false);
			}
			else if ((int)key == 275)
			{
				Set(rawValue + num, force: false);
			}
		}
		else if ((int)key == 274)
		{
			Set(rawValue - num, force: false);
		}
		else if ((int)key == 273)
		{
			Set(rawValue + num, force: false);
		}
	}

	private void UpdateDrag()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)mCol == (Object)null) && !((Object)(object)UICamera.currentCamera == (Object)null) && UICamera.currentTouch != null)
		{
			UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;
			Ray val = UICamera.currentCamera.ScreenPointToRay(Vector2.op_Implicit(UICamera.currentTouch.pos));
			Plane val2 = new Plane(mTrans.rotation * Vector3.back, mTrans.position);
			float num = default;
			if (val2.Raycast(val, ref num))
			{
				Vector3 val3 = mTrans.localPosition + Vector2.op_Implicit(mCenter - mSize * 0.5f);
				Vector3 val4 = mTrans.localPosition - val3;
				Vector3 val5 = mTrans.InverseTransformPoint(val.GetPoint(num));
				Vector3 val6 = val5 + val4;
				Set((direction != Direction.Horizontal) ? (val6.y / mSize.y) : (val6.x / mSize.x), force: false);
			}
		}
	}

	private void Set(float input, bool force)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		if (!mInitDone)
		{
			Init();
		}
		float num = Mathf.Clamp01(input);
		if (num < 0.001f)
		{
			num = 0f;
		}
		float num2 = sliderValue;
		rawValue = num;
		float num3 = sliderValue;
		if (!force && num2 == num3)
		{
			return;
		}
		Vector3 localScale = Vector2.op_Implicit(mSize);
		if (direction == Direction.Horizontal)
		{
			localScale.x *= num3;
		}
		else
		{
			localScale.y *= num3;
		}
		if ((Object)(object)mFGFilled != (Object)null && mFGFilled.type == UISprite.Type.Filled)
		{
			mFGFilled.fillAmount = num3;
		}
		else if ((Object)(object)foreground != (Object)null)
		{
			mFGTrans.localScale = localScale;
			if ((Object)(object)mFGWidget != (Object)null)
			{
				if (num3 > 0.001f)
				{
					((Behaviour)mFGWidget).enabled = true;
					mFGWidget.MarkAsChanged();
				}
				else
				{
					((Behaviour)mFGWidget).enabled = false;
				}
			}
		}
		if ((Object)(object)thumb != (Object)null)
		{
			Vector3 localPosition = thumb.localPosition;
			if ((Object)(object)mFGFilled != (Object)null && mFGFilled.type == UISprite.Type.Filled)
			{
				if (mFGFilled.fillDirection == UISprite.FillDirection.Horizontal)
				{
					localPosition.x = ((!mFGFilled.invert) ? localScale.x : (mSize.x - localScale.x));
				}
				else if (mFGFilled.fillDirection == UISprite.FillDirection.Vertical)
				{
					localPosition.y = ((!mFGFilled.invert) ? localScale.y : (mSize.y - localScale.y));
				}
				else
				{
					Debug.LogWarning((object)"Slider thumb is only supported with Horizontal or Vertical fill direction", (Object)(object)this);
				}
			}
			else if (direction == Direction.Horizontal)
			{
				localPosition.x = localScale.x;
			}
			else
			{
				localPosition.y = localScale.y;
			}
			thumb.localPosition = localPosition;
		}
		current = this;
		if ((Object)(object)eventReceiver != (Object)null && !string.IsNullOrEmpty(functionName) && Application.isPlaying)
		{
			eventReceiver.SendMessage(functionName, (object)num3, (SendMessageOptions)1);
		}
		if (onValueChange != null)
		{
			onValueChange(num3);
		}
		if (Application.isPlaying)
		{
			KGUI.SavePanelScroll(this);
		}
		current = null;
	}

	public void ForceUpdate()
	{
		Set(rawValue, force: true);
	}
}
