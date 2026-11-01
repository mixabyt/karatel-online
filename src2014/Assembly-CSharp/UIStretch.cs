using UnityEngine;

[AddComponentMenu("NGUI/UI/Stretch")]
[ExecuteInEditMode]
public class UIStretch : MonoBehaviour
{
	public enum Style
	{
		None,
		Horizontal,
		Vertical,
		Both,
		BasedOnHeight,
		FillKeepingRatio,
		FitInternalKeepingRatio
	}

	public Camera uiCamera;

	public UIWidget widgetContainer;

	public UIPanel panelContainer;

	public Style style;

	public bool runOnlyOnce;

	public Vector2 relativeSize = Vector2.one;

	public Vector2 initialSize = Vector2.one;

	private Transform mTrans;

	private UIRoot mRoot;

	private Animation mAnim;

	private Rect mRect;

	public UIStretch()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
	}

	private void Awake()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		mAnim = ((Component)this).animation;
		mRect = default;
		mTrans = ((Component)this).transform;
	}

	private void Start()
	{
		if ((Object)(object)uiCamera == (Object)null)
		{
			uiCamera = NGUITools.FindCameraForLayer(((Component)this).gameObject.layer);
		}
		mRoot = NGUITools.FindInParents<UIRoot>(((Component)this).gameObject);
		Update();
	}

	private void Update()
	{
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04da: Unknown result type (might be due to invalid IL or missing references)
		if (((Object)(object)mAnim != (Object)null && mAnim.isPlaying) || style == Style.None)
		{
			return;
		}
		float num = 1f;
		if ((Object)(object)panelContainer != (Object)null)
		{
			if (panelContainer.clipping == UIDrawCall.Clipping.None)
			{
				mRect.xMin = (float)(-Screen.width) * 0.5f;
				mRect.yMin = (float)(-Screen.height) * 0.5f;
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
			mRect = uiCamera.pixelRect;
			if ((Object)(object)mRoot != (Object)null)
			{
				num = mRoot.pixelSizeAdjustment;
			}
		}
		float num2 = mRect.width;
		float num3 = mRect.height;
		if (num != 1f && num3 > 1f)
		{
			float num4 = (float)mRoot.activeHeight / num3;
			num2 *= num4;
			num3 *= num4;
		}
		Vector3 localScale2 = mTrans.localScale;
		if (style == Style.BasedOnHeight)
		{
			localScale2.x = relativeSize.x * num3;
			localScale2.y = relativeSize.y * num3;
		}
		else if (style == Style.FillKeepingRatio)
		{
			float num5 = num2 / num3;
			float num6 = initialSize.x / initialSize.y;
			if (num6 < num5)
			{
				float num7 = num2 / initialSize.x;
				localScale2.x = num2;
				localScale2.y = initialSize.y * num7;
			}
			else
			{
				float num8 = num3 / initialSize.y;
				localScale2.x = initialSize.x * num8;
				localScale2.y = num3;
			}
		}
		else if (style == Style.FitInternalKeepingRatio)
		{
			float num9 = num2 / num3;
			float num10 = initialSize.x / initialSize.y;
			if (num10 > num9)
			{
				float num11 = num2 / initialSize.x;
				localScale2.x = num2;
				localScale2.y = initialSize.y * num11;
			}
			else
			{
				float num12 = num3 / initialSize.y;
				localScale2.x = initialSize.x * num12;
				localScale2.y = num3;
			}
		}
		else
		{
			if (style == Style.Both || style == Style.Horizontal)
			{
				localScale2.x = relativeSize.x * num2;
			}
			if (style == Style.Both || style == Style.Vertical)
			{
				localScale2.y = relativeSize.y * num3;
			}
		}
		if (mTrans.localScale != localScale2)
		{
			mTrans.localScale = localScale2;
		}
		if (runOnlyOnce && Application.isPlaying)
		{
			Object.Destroy((Object)(object)this);
		}
	}
}
