using UnityEngine;

[AddComponentMenu("NGUI/UI/Anchor")]
[ExecuteInEditMode]
public class UIAnchor : MonoBehaviour
{
	public enum Side
	{
		BottomLeft,
		Left,
		TopLeft,
		Top,
		TopRight,
		Right,
		BottomRight,
		Bottom,
		Center
	}

	private bool mNeedsHalfPixelOffset;

	public Camera uiCamera;

	public UIWidget widgetContainer;

	public UIPanel panelContainer;

	public Side side = Side.Center;

	public bool halfPixelOffset = true;

	public bool runOnlyOnce;

	public Vector2 relativeOffset = Vector2.zero;

	private Transform mTrans;

	private Animation mAnim;

	private Rect mRect = default;

	private UIRoot mRoot;

	public UIAnchor()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
	}

	private void Awake()
	{
		mTrans = ((Component)this).transform;
		mAnim = ((Component)this).animation;
	}

	private void Start()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Invalid comparison between Unknown and I4
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Invalid comparison between Unknown and I4
		mRoot = NGUITools.FindInParents<UIRoot>(((Component)this).gameObject);
		mNeedsHalfPixelOffset = (int)Application.platform == 10 || (int)Application.platform == 7;
		if (mNeedsHalfPixelOffset)
		{
			mNeedsHalfPixelOffset = SystemInfo.graphicsShaderLevel < 40;
		}
		if ((Object)(object)uiCamera == (Object)null)
		{
			uiCamera = NGUITools.FindCameraForLayer(((Component)this).gameObject.layer);
		}
		Update();
	}

	private void Update()
	{
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0547: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_054e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04de: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0598: Unknown result type (might be due to invalid IL or missing references)
		//IL_059d: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0587: Unknown result type (might be due to invalid IL or missing references)
		//IL_0589: Unknown result type (might be due to invalid IL or missing references)
		//IL_058e: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)mAnim != (Object)null && ((Behaviour)mAnim).enabled && mAnim.isPlaying)
		{
			return;
		}
		bool flag = false;
		if ((Object)(object)panelContainer != (Object)null)
		{
			if (panelContainer.clipping == UIDrawCall.Clipping.None)
			{
				float num = ((!((Object)(object)mRoot != (Object)null)) ? 0.5f : ((float)mRoot.activeHeight / (float)Screen.height * 0.5f));
				mRect.xMin = (float)(-Screen.width) * num;
				mRect.yMin = (float)(-Screen.height) * num;
				mRect.xMax = 0f - mRect.xMin;
				mRect.yMax = 0f - mRect.yMin;
			}
			else
			{
				Vector4 clipRange = panelContainer.clipRange;
				mRect.x = clipRange.x - clipRange.z * 0.5f;
				mRect.y = clipRange.y - clipRange.w * 0.5f;
				mRect.width = clipRange.z;
				mRect.height = clipRange.w;
			}
		}
		else if ((Object)(object)widgetContainer != (Object)null)
		{
			Transform cachedTransform = widgetContainer.cachedTransform;
			Vector3 localScale = cachedTransform.localScale;
			Vector3 localPosition = cachedTransform.localPosition;
			Vector3 val = Vector2.op_Implicit(widgetContainer.relativeSize);
			Vector3 val2 = Vector2.op_Implicit(widgetContainer.pivotOffset);
			val2.y--;
			val2.x *= widgetContainer.relativeSize.x * localScale.x;
			val2.y *= widgetContainer.relativeSize.y * localScale.y;
			mRect.x = localPosition.x + val2.x;
			mRect.y = localPosition.y + val2.y;
			mRect.width = val.x * localScale.x;
			mRect.height = val.y * localScale.y;
		}
		else
		{
			if (!((Object)(object)uiCamera != (Object)null))
			{
				return;
			}
			flag = true;
			mRect = uiCamera.pixelRect;
		}
		float num2 = (mRect.xMin + mRect.xMax) * 0.5f;
		float num3 = (mRect.yMin + mRect.yMax) * 0.5f;
		Vector3 val3 = new Vector3(num2, num3, 0f);
		if (side != Side.Center)
		{
			if (side == Side.Right || side == Side.TopRight || side == Side.BottomRight)
			{
				val3.x = mRect.xMax;
			}
			else if (side == Side.Top || side == Side.Center || side == Side.Bottom)
			{
				val3.x = num2;
			}
			else
			{
				val3.x = mRect.xMin;
			}
			if (side == Side.Top || side == Side.TopRight || side == Side.TopLeft)
			{
				val3.y = mRect.yMax;
			}
			else if (side == Side.Left || side == Side.Center || side == Side.Right)
			{
				val3.y = num3;
			}
			else
			{
				val3.y = mRect.yMin;
			}
		}
		float width = mRect.width;
		float height = mRect.height;
		val3.x += relativeOffset.x * width;
		val3.y += relativeOffset.y * height;
		if (flag)
		{
			if (uiCamera.orthographic)
			{
				val3.x = Mathf.Round(val3.x);
				val3.y = Mathf.Round(val3.y);
				if (halfPixelOffset && mNeedsHalfPixelOffset)
				{
					val3.x -= 0.5f;
					val3.y += 0.5f;
				}
			}
			val3.z = uiCamera.WorldToScreenPoint(mTrans.position).z;
			val3 = uiCamera.ScreenToWorldPoint(val3);
		}
		else
		{
			val3.x = Mathf.Round(val3.x);
			val3.y = Mathf.Round(val3.y);
			if ((Object)(object)panelContainer != (Object)null)
			{
				val3 = panelContainer.cachedTransform.TransformPoint(val3);
			}
			else if ((Object)(object)widgetContainer != (Object)null)
			{
				Transform parent = widgetContainer.cachedTransform.parent;
				if ((Object)(object)parent != (Object)null)
				{
					val3 = parent.TransformPoint(val3);
				}
			}
			val3.z = mTrans.position.z;
		}
		if (mTrans.position != val3)
		{
			mTrans.position = val3;
		}
		if (runOnlyOnce && Application.isPlaying)
		{
			Object.Destroy((Object)(object)this);
		}
	}
}
