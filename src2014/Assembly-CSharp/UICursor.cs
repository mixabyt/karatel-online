using UnityEngine;

[AddComponentMenu("NGUI/Examples/UI Cursor")]
[RequireComponent(typeof(UISprite))]
public class UICursor : MonoBehaviour
{
	private static UICursor mInstance;

	public Camera uiCamera;

	private Transform mTrans;

	private UISprite mSprite;

	private UIAtlas mAtlas;

	private string mSpriteName;

	private void Awake()
	{
		mInstance = this;
	}

	private void OnDestroy()
	{
		mInstance = null;
	}

	private void Start()
	{
		mTrans = ((Component)this).transform;
		mSprite = ((Component)this).GetComponentInChildren<UISprite>();
		mAtlas = mSprite.atlas;
		mSpriteName = mSprite.spriteName;
		mSprite.depth = 100;
		if ((Object)(object)uiCamera == (Object)null)
		{
			uiCamera = NGUITools.FindCameraForLayer(((Component)this).gameObject.layer);
		}
	}

	private void Update()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)mSprite.atlas != (Object)null))
		{
			return;
		}
		Vector3 mousePosition = Input.mousePosition;
		if ((Object)(object)uiCamera != (Object)null)
		{
			mousePosition.x = Mathf.Clamp01(mousePosition.x / (float)Screen.width);
			mousePosition.y = Mathf.Clamp01(mousePosition.y / (float)Screen.height);
			mTrans.position = uiCamera.ViewportToWorldPoint(mousePosition);
			if (uiCamera.isOrthoGraphic)
			{
				mTrans.localPosition = NGUIMath.ApplyHalfPixelOffset(mTrans.localPosition, mTrans.localScale);
			}
		}
		else
		{
			mousePosition.x -= (float)Screen.width * 0.5f;
			mousePosition.y -= (float)Screen.height * 0.5f;
			mTrans.localPosition = NGUIMath.ApplyHalfPixelOffset(mousePosition, mTrans.localScale);
		}
	}

	public static void Clear()
	{
		Set(mInstance.mAtlas, mInstance.mSpriteName);
	}

	public static void Set(UIAtlas atlas, string sprite)
	{
		if ((Object)(object)mInstance != (Object)null)
		{
			mInstance.mSprite.atlas = atlas;
			mInstance.mSprite.spriteName = sprite;
			mInstance.mSprite.MakePixelPerfect();
			mInstance.Update();
		}
	}
}
