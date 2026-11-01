using System;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Sprite")]
[ExecuteInEditMode]
public class UISprite : UIWidget
{
	public enum Type
	{
		Simple,
		Sliced,
		Tiled,
		Filled
	}

	public enum FillDirection
	{
		Horizontal,
		Vertical,
		Radial90,
		Radial180,
		Radial360
	}

	[SerializeField]
	[HideInInspector]
	private UIAtlas mAtlas;

	[SerializeField]
	[HideInInspector]
	private string mSpriteName;

	[HideInInspector]
	[SerializeField]
	private bool mFillCenter = true;

	[SerializeField]
	[HideInInspector]
	private Type mType;

	[SerializeField]
	[HideInInspector]
	private FillDirection mFillDirection = FillDirection.Radial360;

	[HideInInspector]
	[SerializeField]
	private float mFillAmount = 1f;

	[HideInInspector]
	[SerializeField]
	private bool mInvert;

	protected UIAtlas.Sprite mSprite;

	protected Rect mInner;

	protected Rect mInnerUV;

	protected Rect mOuter;

	protected Rect mOuterUV;

	protected Vector3 mScale = Vector3.one;

	private bool mSpriteSet;

	public virtual Type type
	{
		get
		{
			return mType;
		}
		set
		{
			if (mType != value)
			{
				mType = value;
				MarkAsChanged();
			}
		}
	}

	public UIAtlas atlas
	{
		get
		{
			return mAtlas;
		}
		set
		{
			if ((Object)(object)mAtlas != (Object)(object)value)
			{
				mAtlas = value;
				mSpriteSet = false;
				mSprite = null;
				material = ((!((Object)(object)mAtlas != (Object)null)) ? null : mAtlas.spriteMaterial);
				if (string.IsNullOrEmpty(mSpriteName) && (Object)(object)mAtlas != (Object)null && mAtlas.spriteList.Count > 0)
				{
					SetAtlasSprite(mAtlas.spriteList[0]);
					mSpriteName = mSprite.name;
				}
				if (!string.IsNullOrEmpty(mSpriteName))
				{
					string text = mSpriteName;
					mSpriteName = string.Empty;
					spriteName = text;
					mChanged = true;
					UpdateUVs(force: true);
				}
			}
		}
	}

	public string spriteName
	{
		get
		{
			return mSpriteName;
		}
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				if (!string.IsNullOrEmpty(mSpriteName))
				{
					mSpriteName = string.Empty;
					mSprite = null;
					mChanged = true;
					mSpriteSet = false;
				}
			}
			else if (mSpriteName != value)
			{
				mSpriteName = value;
				mSprite = null;
				mChanged = true;
				mSpriteSet = false;
				if (isValid)
				{
					UpdateUVs(force: true);
				}
			}
		}
	}

	public bool isValid => GetAtlasSprite() != null;

	public override Material material
	{
		get
		{
			Material val = base.material;
			if ((Object)(object)val == (Object)null)
			{
				val = ((!((Object)(object)mAtlas != (Object)null)) ? null : mAtlas.spriteMaterial);
				mSprite = null;
				material = val;
				if ((Object)(object)val != (Object)null)
				{
					UpdateUVs(force: true);
				}
			}
			return val;
		}
	}

	public Rect innerUV
	{
		get
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			UpdateUVs(force: false);
			return mInnerUV;
		}
	}

	public Rect outerUV
	{
		get
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			UpdateUVs(force: false);
			return mOuterUV;
		}
	}

	public bool fillCenter
	{
		get
		{
			return mFillCenter;
		}
		set
		{
			if (mFillCenter != value)
			{
				mFillCenter = value;
				MarkAsChanged();
			}
		}
	}

	public FillDirection fillDirection
	{
		get
		{
			return mFillDirection;
		}
		set
		{
			if (mFillDirection != value)
			{
				mFillDirection = value;
				mChanged = true;
			}
		}
	}

	public float fillAmount
	{
		get
		{
			return mFillAmount;
		}
		set
		{
			float num = Mathf.Clamp01(value);
			if (mFillAmount != num)
			{
				mFillAmount = num;
				mChanged = true;
			}
		}
	}

	public bool invert
	{
		get
		{
			return mInvert;
		}
		set
		{
			if (mInvert != value)
			{
				mInvert = value;
				mChanged = true;
			}
		}
	}

	public override Vector4 relativePadding
	{
		get
		{
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			if (isValid && type == Type.Simple)
			{
				return new Vector4(mSprite.paddingLeft, mSprite.paddingTop, mSprite.paddingRight, mSprite.paddingBottom);
			}
			return base.relativePadding;
		}
	}

	public override Vector4 border
	{
		get
		{
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			if (type == Type.Sliced)
			{
				UIAtlas.Sprite atlasSprite = GetAtlasSprite();
				if (atlasSprite == null)
				{
					return Vector4.op_Implicit(Vector2.zero);
				}
				Rect rect = atlasSprite.outer;
				Rect rect2 = atlasSprite.inner;
				Texture val = mainTexture;
				if (atlas.coordinates == UIAtlas.Coordinates.TexCoords && (Object)(object)val != (Object)null)
				{
					rect = NGUIMath.ConvertToPixels(rect, val.width, val.height, round: true);
					rect2 = NGUIMath.ConvertToPixels(rect2, val.width, val.height, round: true);
				}
				return new Vector4(rect2.xMin - rect.xMin, rect2.yMin - rect.yMin, rect.xMax - rect2.xMax, rect.yMax - rect2.yMax) * atlas.pixelSize;
			}
			return base.border;
		}
	}

	public override bool pixelPerfectAfterResize => type == Type.Sliced;

	public UISprite()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
	}

	public UIAtlas.Sprite GetAtlasSprite()
	{
		if (!mSpriteSet)
		{
			mSprite = null;
		}
		if (mSprite == null && (Object)(object)mAtlas != (Object)null)
		{
			if (!string.IsNullOrEmpty(mSpriteName))
			{
				UIAtlas.Sprite sprite = mAtlas.GetSprite(mSpriteName);
				if (sprite == null)
				{
					return null;
				}
				SetAtlasSprite(sprite);
			}
			if (mSprite == null && mAtlas.spriteList.Count > 0)
			{
				UIAtlas.Sprite sprite2 = mAtlas.spriteList[0];
				if (sprite2 == null)
				{
					return null;
				}
				SetAtlasSprite(sprite2);
				if (mSprite == null)
				{
					Debug.LogError((object)(((Object)mAtlas).name + " seems to have a null sprite!"));
					return null;
				}
				mSpriteName = mSprite.name;
			}
			if (mSprite != null)
			{
				material = mAtlas.spriteMaterial;
				UpdateUVs(force: true);
			}
		}
		return mSprite;
	}

	protected void SetAtlasSprite(UIAtlas.Sprite sp)
	{
		mChanged = true;
		mSpriteSet = true;
		if (sp != null)
		{
			mSprite = sp;
			mSpriteName = mSprite.name;
		}
		else
		{
			mSpriteName = ((mSprite == null) ? string.Empty : mSprite.name);
			mSprite = sp;
		}
	}

	public virtual void UpdateUVs(bool force)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		if ((type == Type.Sliced || type == Type.Tiled) && cachedTransform.localScale != mScale)
		{
			mScale = cachedTransform.localScale;
			mChanged = true;
		}
		if (!isValid || !force)
		{
			return;
		}
		Texture val = mainTexture;
		if ((Object)(object)val != (Object)null)
		{
			mInner = mSprite.inner;
			mOuter = mSprite.outer;
			mInnerUV = mInner;
			mOuterUV = mOuter;
			if (atlas.coordinates == UIAtlas.Coordinates.Pixels)
			{
				mOuterUV = NGUIMath.ConvertToTexCoords(mOuterUV, val.width, val.height);
				mInnerUV = NGUIMath.ConvertToTexCoords(mInnerUV, val.width, val.height);
			}
		}
	}

	public override void MakePixelPerfect()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		if (!isValid)
		{
			return;
		}
		UpdateUVs(force: false);
		switch (type)
		{
		case Type.Sliced:
		{
			Vector3 localPosition = cachedTransform.localPosition;
			localPosition.x = Mathf.RoundToInt(localPosition.x);
			localPosition.y = Mathf.RoundToInt(localPosition.y);
			localPosition.z = Mathf.RoundToInt(localPosition.z);
			cachedTransform.localPosition = localPosition;
			Vector3 localScale = cachedTransform.localScale;
			localScale.x = Mathf.RoundToInt(localScale.x * 0.5f) << 1;
			localScale.y = Mathf.RoundToInt(localScale.y * 0.5f) << 1;
			localScale.z = 1f;
			cachedTransform.localScale = localScale;
			return;
		}
		case Type.Tiled:
		{
			Vector3 localPosition2 = cachedTransform.localPosition;
			localPosition2.x = Mathf.RoundToInt(localPosition2.x);
			localPosition2.y = Mathf.RoundToInt(localPosition2.y);
			localPosition2.z = Mathf.RoundToInt(localPosition2.z);
			cachedTransform.localPosition = localPosition2;
			Vector3 localScale2 = cachedTransform.localScale;
			localScale2.x = Mathf.RoundToInt(localScale2.x);
			localScale2.y = Mathf.RoundToInt(localScale2.y);
			localScale2.z = 1f;
			cachedTransform.localScale = localScale2;
			return;
		}
		}
		Texture val = mainTexture;
		Vector3 localScale3 = cachedTransform.localScale;
		if ((Object)(object)val != (Object)null)
		{
			Rect val2 = NGUIMath.ConvertToPixels(outerUV, val.width, val.height, round: true);
			float pixelSize = atlas.pixelSize;
			localScale3.x = (float)Mathf.RoundToInt(val2.width * pixelSize) * Mathf.Sign(localScale3.x);
			localScale3.y = (float)Mathf.RoundToInt(val2.height * pixelSize) * Mathf.Sign(localScale3.y);
			localScale3.z = 1f;
			cachedTransform.localScale = localScale3;
		}
		int num = Mathf.RoundToInt(Mathf.Abs(localScale3.x) * (1f + mSprite.paddingLeft + mSprite.paddingRight));
		int num2 = Mathf.RoundToInt(Mathf.Abs(localScale3.y) * (1f + mSprite.paddingTop + mSprite.paddingBottom));
		Vector3 localPosition3 = cachedTransform.localPosition;
		localPosition3.x = Mathf.CeilToInt(localPosition3.x * 4f) >> 2;
		localPosition3.y = Mathf.CeilToInt(localPosition3.y * 4f) >> 2;
		localPosition3.z = Mathf.RoundToInt(localPosition3.z);
		if (num % 2 == 1 && (pivot == Pivot.Top || pivot == Pivot.Center || pivot == Pivot.Bottom))
		{
			localPosition3.x += 0.5f;
		}
		if (num2 % 2 == 1 && (pivot == Pivot.Left || pivot == Pivot.Center || pivot == Pivot.Right))
		{
			localPosition3.y += 0.5f;
		}
		cachedTransform.localPosition = localPosition3;
	}

	protected override void OnStart()
	{
		if ((Object)(object)mAtlas != (Object)null)
		{
			UpdateUVs(force: true);
		}
	}

	public override void Update()
	{
		base.Update();
		if (mChanged || !mSpriteSet)
		{
			mSpriteSet = true;
			mSprite = null;
			mChanged = true;
			UpdateUVs(force: true);
		}
		else
		{
			UpdateUVs(force: false);
		}
	}

	public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		switch (type)
		{
		case Type.Simple:
			SimpleFill(verts, uvs, cols);
			break;
		case Type.Sliced:
			SlicedFill(verts, uvs, cols);
			break;
		case Type.Filled:
			FilledFill(verts, uvs, cols);
			break;
		case Type.Tiled:
			TiledFill(verts, uvs, cols);
			break;
		}
	}

	protected void SimpleFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		Vector2 item = new Vector2(mOuterUV.xMin, mOuterUV.yMin);
		Vector2 item2 = new Vector2(mOuterUV.xMax, mOuterUV.yMax);
		verts.Add(new Vector3(1f, 0f, 0f));
		verts.Add(new Vector3(1f, -1f, 0f));
		verts.Add(new Vector3(0f, -1f, 0f));
		verts.Add(new Vector3(0f, 0f, 0f));
		uvs.Add(item2);
		uvs.Add(new Vector2(item2.x, item.y));
		uvs.Add(item);
		uvs.Add(new Vector2(item.x, item2.y));
		Color val = color;
		val.a *= mPanel.alpha;
		Color32 item3 = Color32.op_Implicit((!atlas.premultipliedAlpha) ? val : NGUITools.ApplyPMA(val));
		cols.Add(item3);
		cols.Add(item3);
		cols.Add(item3);
		cols.Add(item3);
	}

	protected void SlicedFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0509: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_050b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0510: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_049a: Unknown result type (might be due to invalid IL or missing references)
		//IL_056e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0598: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0611: Unknown result type (might be due to invalid IL or missing references)
		//IL_0636: Unknown result type (might be due to invalid IL or missing references)
		//IL_065b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0680: Unknown result type (might be due to invalid IL or missing references)
		//IL_068b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0693: Unknown result type (might be due to invalid IL or missing references)
		//IL_069b: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a3: Unknown result type (might be due to invalid IL or missing references)
		if (mOuterUV == mInnerUV)
		{
			SimpleFill(verts, uvs, cols);
			return;
		}
		Vector2[] array = new Vector2[4];
		Vector2[] array2 = new Vector2[4];
		Texture val = mainTexture;
		ref Vector2 reference = ref array[0];
		reference = Vector2.zero;
		ref Vector2 reference2 = ref array[1];
		reference2 = Vector2.zero;
		ref Vector2 reference3 = ref array[2];
		reference3 = new Vector2(1f, -1f);
		ref Vector2 reference4 = ref array[3];
		reference4 = new Vector2(1f, -1f);
		if ((Object)(object)val != (Object)null)
		{
			float pixelSize = atlas.pixelSize;
			float num = (mInnerUV.xMin - mOuterUV.xMin) * pixelSize;
			float num2 = (mOuterUV.xMax - mInnerUV.xMax) * pixelSize;
			float num3 = (mInnerUV.yMax - mOuterUV.yMax) * pixelSize;
			float num4 = (mOuterUV.yMin - mInnerUV.yMin) * pixelSize;
			Vector3 localScale = cachedTransform.localScale;
			localScale.x = Mathf.Max(0f, localScale.x);
			localScale.y = Mathf.Max(0f, localScale.y);
			Vector2 val2 = new Vector2(localScale.x / (float)val.width, localScale.y / (float)val.height);
			Vector2 val3 = new Vector2(num / val2.x, num3 / val2.y);
			Vector2 val4 = new Vector2(num2 / val2.x, num4 / val2.y);
			Pivot pivot = base.pivot;
			if (pivot == Pivot.Right || pivot == Pivot.TopRight || pivot == Pivot.BottomRight)
			{
				array[0].x = Mathf.Min(0f, 1f - (val4.x + val3.x));
				array[1].x = array[0].x + val3.x;
				array[2].x = array[0].x + Mathf.Max(val3.x, 1f - val4.x);
				array[3].x = array[0].x + Mathf.Max(val3.x + val4.x, 1f);
			}
			else
			{
				array[1].x = val3.x;
				array[2].x = Mathf.Max(val3.x, 1f - val4.x);
				array[3].x = Mathf.Max(val3.x + val4.x, 1f);
			}
			if (pivot == Pivot.Bottom || pivot == Pivot.BottomLeft || pivot == Pivot.BottomRight)
			{
				array[0].y = Mathf.Max(0f, -1f - (val4.y + val3.y));
				array[1].y = array[0].y + val3.y;
				array[2].y = array[0].y + Mathf.Min(val3.y, -1f - val4.y);
				array[3].y = array[0].y + Mathf.Min(val3.y + val4.y, -1f);
			}
			else
			{
				array[1].y = val3.y;
				array[2].y = Mathf.Min(val3.y, -1f - val4.y);
				array[3].y = Mathf.Min(val3.y + val4.y, -1f);
			}
			ref Vector2 reference5 = ref array2[0];
			reference5 = new Vector2(mOuterUV.xMin, mOuterUV.yMax);
			ref Vector2 reference6 = ref array2[1];
			reference6 = new Vector2(mInnerUV.xMin, mInnerUV.yMax);
			ref Vector2 reference7 = ref array2[2];
			reference7 = new Vector2(mInnerUV.xMax, mInnerUV.yMin);
			ref Vector2 reference8 = ref array2[3];
			reference8 = new Vector2(mOuterUV.xMax, mOuterUV.yMin);
		}
		else
		{
			for (int i = 0; i < 4; i++)
			{
				ref Vector2 reference9 = ref array2[i];
				reference9 = Vector2.zero;
			}
		}
		Color val5 = color;
		val5.a *= mPanel.alpha;
		Color32 item = Color32.op_Implicit((!atlas.premultipliedAlpha) ? val5 : NGUITools.ApplyPMA(val5));
		for (int j = 0; j < 3; j++)
		{
			int num5 = j + 1;
			for (int k = 0; k < 3; k++)
			{
				if (mFillCenter || j != 1 || k != 1)
				{
					int num6 = k + 1;
					verts.Add(new Vector3(array[num5].x, array[k].y, 0f));
					verts.Add(new Vector3(array[num5].x, array[num6].y, 0f));
					verts.Add(new Vector3(array[j].x, array[num6].y, 0f));
					verts.Add(new Vector3(array[j].x, array[k].y, 0f));
					uvs.Add(new Vector2(array2[num5].x, array2[k].y));
					uvs.Add(new Vector2(array2[num5].x, array2[num6].y));
					uvs.Add(new Vector2(array2[j].x, array2[num6].y));
					uvs.Add(new Vector2(array2[j].x, array2[k].y));
					cols.Add(item);
					cols.Add(item);
					cols.Add(item);
					cols.Add(item);
				}
			}
		}
	}

	protected bool AdjustRadial(Vector2[] xy, Vector2[] uv, float fill, bool invert)
	{
		if (fill < 0.001f)
		{
			return false;
		}
		if (!invert && fill > 0.999f)
		{
			return true;
		}
		float num = Mathf.Clamp01(fill);
		if (!invert)
		{
			num = 1f - num;
		}
		num *= (float)Math.PI / 2f;
		float num2 = Mathf.Sin(num);
		float num3 = Mathf.Cos(num);
		if (num2 > num3)
		{
			num3 *= 1f / num2;
			num2 = 1f;
			if (!invert)
			{
				xy[0].y = Mathf.Lerp(xy[2].y, xy[0].y, num3);
				xy[3].y = xy[0].y;
				uv[0].y = Mathf.Lerp(uv[2].y, uv[0].y, num3);
				uv[3].y = uv[0].y;
			}
		}
		else if (num3 > num2)
		{
			num2 *= 1f / num3;
			num3 = 1f;
			if (invert)
			{
				xy[0].x = Mathf.Lerp(xy[2].x, xy[0].x, num2);
				xy[1].x = xy[0].x;
				uv[0].x = Mathf.Lerp(uv[2].x, uv[0].x, num2);
				uv[1].x = uv[0].x;
			}
		}
		else
		{
			num2 = 1f;
			num3 = 1f;
		}
		if (invert)
		{
			xy[1].y = Mathf.Lerp(xy[2].y, xy[0].y, num3);
			uv[1].y = Mathf.Lerp(uv[2].y, uv[0].y, num3);
		}
		else
		{
			xy[3].x = Mathf.Lerp(xy[2].x, xy[0].x, num2);
			uv[3].x = Mathf.Lerp(uv[2].x, uv[0].x, num2);
		}
		return true;
	}

	protected void Rotate(Vector2[] v, int offset)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < offset; i++)
		{
			Vector2 val = new Vector2(v[3].x, v[3].y);
			v[3].x = v[2].y;
			v[3].y = v[2].x;
			v[2].x = v[1].y;
			v[2].y = v[1].x;
			v[1].x = v[0].y;
			v[1].y = v[0].x;
			v[0].x = val.y;
			v[0].y = val.x;
		}
	}

	protected void FilledFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b96: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b9b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0baf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bba: Unknown result type (might be due to invalid IL or missing references)
		//IL_072a: Unknown result type (might be due to invalid IL or missing references)
		//IL_072f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0746: Unknown result type (might be due to invalid IL or missing references)
		//IL_074b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0762: Unknown result type (might be due to invalid IL or missing references)
		//IL_0767: Unknown result type (might be due to invalid IL or missing references)
		//IL_077e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0783: Unknown result type (might be due to invalid IL or missing references)
		//IL_079a: Unknown result type (might be due to invalid IL or missing references)
		//IL_079f: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b46: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b55: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b60: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a44: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a53: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5e: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05af: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ba: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		float num2 = 0f;
		float num3 = 1f;
		float num4 = -1f;
		float num5 = mOuterUV.xMin;
		float num6 = mOuterUV.yMin;
		float num7 = mOuterUV.xMax;
		float num8 = mOuterUV.yMax;
		if (mFillDirection == FillDirection.Horizontal || mFillDirection == FillDirection.Vertical)
		{
			float num9 = (num7 - num5) * mFillAmount;
			float num10 = (num8 - num6) * mFillAmount;
			if (fillDirection == FillDirection.Horizontal)
			{
				if (mInvert)
				{
					num = 1f - mFillAmount;
					num5 = num7 - num9;
				}
				else
				{
					num3 *= mFillAmount;
					num7 = num5 + num9;
				}
			}
			else if (fillDirection == FillDirection.Vertical)
			{
				if (mInvert)
				{
					num4 *= mFillAmount;
					num6 = num8 - num10;
				}
				else
				{
					num2 = 0f - (1f - mFillAmount);
					num8 = num6 + num10;
				}
			}
		}
		Vector2[] array = new Vector2[4];
		Vector2[] array2 = new Vector2[4];
		ref Vector2 reference = ref array[0];
		reference = new Vector2(num3, num2);
		ref Vector2 reference2 = ref array[1];
		reference2 = new Vector2(num3, num4);
		ref Vector2 reference3 = ref array[2];
		reference3 = new Vector2(num, num4);
		ref Vector2 reference4 = ref array[3];
		reference4 = new Vector2(num, num2);
		ref Vector2 reference5 = ref array2[0];
		reference5 = new Vector2(num7, num8);
		ref Vector2 reference6 = ref array2[1];
		reference6 = new Vector2(num7, num6);
		ref Vector2 reference7 = ref array2[2];
		reference7 = new Vector2(num5, num6);
		ref Vector2 reference8 = ref array2[3];
		reference8 = new Vector2(num5, num8);
		Color val = color;
		val.a *= mPanel.alpha;
		Color32 item = Color32.op_Implicit((!atlas.premultipliedAlpha) ? val : NGUITools.ApplyPMA(val));
		if (fillDirection == FillDirection.Radial90)
		{
			if (!AdjustRadial(array, array2, mFillAmount, mInvert))
			{
				return;
			}
		}
		else
		{
			if (fillDirection == FillDirection.Radial180)
			{
				Vector2[] array3 = new Vector2[4];
				Vector2[] array4 = new Vector2[4];
				for (int i = 0; i < 2; i++)
				{
					ref Vector2 reference9 = ref array3[0];
					reference9 = new Vector2(0f, 0f);
					ref Vector2 reference10 = ref array3[1];
					reference10 = new Vector2(0f, 1f);
					ref Vector2 reference11 = ref array3[2];
					reference11 = new Vector2(1f, 1f);
					ref Vector2 reference12 = ref array3[3];
					reference12 = new Vector2(1f, 0f);
					ref Vector2 reference13 = ref array4[0];
					reference13 = new Vector2(0f, 0f);
					ref Vector2 reference14 = ref array4[1];
					reference14 = new Vector2(0f, 1f);
					ref Vector2 reference15 = ref array4[2];
					reference15 = new Vector2(1f, 1f);
					ref Vector2 reference16 = ref array4[3];
					reference16 = new Vector2(1f, 0f);
					if (mInvert)
					{
						if (i > 0)
						{
							Rotate(array3, i);
							Rotate(array4, i);
						}
					}
					else if (i < 1)
					{
						Rotate(array3, 1 - i);
						Rotate(array4, 1 - i);
					}
					float num11;
					float num12;
					if (i == 1)
					{
						num11 = ((!mInvert) ? 1f : 0.5f);
						num12 = ((!mInvert) ? 0.5f : 1f);
					}
					else
					{
						num11 = ((!mInvert) ? 0.5f : 1f);
						num12 = ((!mInvert) ? 1f : 0.5f);
					}
					array3[1].y = Mathf.Lerp(num11, num12, array3[1].y);
					array3[2].y = Mathf.Lerp(num11, num12, array3[2].y);
					array4[1].y = Mathf.Lerp(num11, num12, array4[1].y);
					array4[2].y = Mathf.Lerp(num11, num12, array4[2].y);
					float fill = mFillAmount * 2f - (float)i;
					bool flag = i % 2 == 1;
					if (!AdjustRadial(array3, array4, fill, !flag))
					{
						continue;
					}
					if (mInvert)
					{
						flag = !flag;
					}
					if (flag)
					{
						for (int j = 0; j < 4; j++)
						{
							num11 = Mathf.Lerp(array[0].x, array[2].x, array3[j].x);
							num12 = Mathf.Lerp(array[0].y, array[2].y, array3[j].y);
							float num13 = Mathf.Lerp(array2[0].x, array2[2].x, array4[j].x);
							float num14 = Mathf.Lerp(array2[0].y, array2[2].y, array4[j].y);
							verts.Add(new Vector3(num11, num12, 0f));
							uvs.Add(new Vector2(num13, num14));
							cols.Add(item);
						}
						continue;
					}
					for (int num15 = 3; num15 > -1; num15--)
					{
						num11 = Mathf.Lerp(array[0].x, array[2].x, array3[num15].x);
						num12 = Mathf.Lerp(array[0].y, array[2].y, array3[num15].y);
						float num16 = Mathf.Lerp(array2[0].x, array2[2].x, array4[num15].x);
						float num17 = Mathf.Lerp(array2[0].y, array2[2].y, array4[num15].y);
						verts.Add(new Vector3(num11, num12, 0f));
						uvs.Add(new Vector2(num16, num17));
						cols.Add(item);
					}
				}
				return;
			}
			if (fillDirection == FillDirection.Radial360)
			{
				float[] array5 = new float[16]
				{
					0.5f, 1f, 0f, 0.5f, 0.5f, 1f, 0.5f, 1f, 0f, 0.5f,
					0.5f, 1f, 0f, 0.5f, 0f, 0.5f
				};
				Vector2[] array6 = new Vector2[4];
				Vector2[] array7 = new Vector2[4];
				for (int k = 0; k < 4; k++)
				{
					ref Vector2 reference17 = ref array6[0];
					reference17 = new Vector2(0f, 0f);
					ref Vector2 reference18 = ref array6[1];
					reference18 = new Vector2(0f, 1f);
					ref Vector2 reference19 = ref array6[2];
					reference19 = new Vector2(1f, 1f);
					ref Vector2 reference20 = ref array6[3];
					reference20 = new Vector2(1f, 0f);
					ref Vector2 reference21 = ref array7[0];
					reference21 = new Vector2(0f, 0f);
					ref Vector2 reference22 = ref array7[1];
					reference22 = new Vector2(0f, 1f);
					ref Vector2 reference23 = ref array7[2];
					reference23 = new Vector2(1f, 1f);
					ref Vector2 reference24 = ref array7[3];
					reference24 = new Vector2(1f, 0f);
					if (mInvert)
					{
						if (k > 0)
						{
							Rotate(array6, k);
							Rotate(array7, k);
						}
					}
					else if (k < 3)
					{
						Rotate(array6, 3 - k);
						Rotate(array7, 3 - k);
					}
					for (int l = 0; l < 4; l++)
					{
						int num18 = ((!mInvert) ? (k * 4) : ((3 - k) * 4));
						float num19 = array5[num18];
						float num20 = array5[num18 + 1];
						float num21 = array5[num18 + 2];
						float num22 = array5[num18 + 3];
						array6[l].x = Mathf.Lerp(num19, num20, array6[l].x);
						array6[l].y = Mathf.Lerp(num21, num22, array6[l].y);
						array7[l].x = Mathf.Lerp(num19, num20, array7[l].x);
						array7[l].y = Mathf.Lerp(num21, num22, array7[l].y);
					}
					float fill2 = mFillAmount * 4f - (float)k;
					bool flag2 = k % 2 == 1;
					if (!AdjustRadial(array6, array7, fill2, !flag2))
					{
						continue;
					}
					if (mInvert)
					{
						flag2 = !flag2;
					}
					if (flag2)
					{
						for (int m = 0; m < 4; m++)
						{
							float num23 = Mathf.Lerp(array[0].x, array[2].x, array6[m].x);
							float num24 = Mathf.Lerp(array[0].y, array[2].y, array6[m].y);
							float num25 = Mathf.Lerp(array2[0].x, array2[2].x, array7[m].x);
							float num26 = Mathf.Lerp(array2[0].y, array2[2].y, array7[m].y);
							verts.Add(new Vector3(num23, num24, 0f));
							uvs.Add(new Vector2(num25, num26));
							cols.Add(item);
						}
						continue;
					}
					for (int num27 = 3; num27 > -1; num27--)
					{
						float num28 = Mathf.Lerp(array[0].x, array[2].x, array6[num27].x);
						float num29 = Mathf.Lerp(array[0].y, array[2].y, array6[num27].y);
						float num30 = Mathf.Lerp(array2[0].x, array2[2].x, array7[num27].x);
						float num31 = Mathf.Lerp(array2[0].y, array2[2].y, array7[num27].y);
						verts.Add(new Vector3(num28, num29, 0f));
						uvs.Add(new Vector2(num30, num31));
						cols.Add(item);
					}
				}
				return;
			}
		}
		for (int n = 0; n < 4; n++)
		{
			verts.Add(Vector2.op_Implicit(array[n]));
			uvs.Add(array2[n]);
			cols.Add(item);
		}
	}

	protected void TiledFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		Texture val = material.mainTexture;
		if ((Object)(object)val == (Object)null)
		{
			return;
		}
		Rect rect = mInner;
		if (atlas.coordinates == UIAtlas.Coordinates.TexCoords)
		{
			rect = NGUIMath.ConvertToPixels(rect, val.width, val.height, round: true);
		}
		Vector2 val2 = Vector2.op_Implicit(cachedTransform.localScale);
		float pixelSize = atlas.pixelSize;
		float num = Mathf.Abs(rect.width / val2.x) * pixelSize;
		float num2 = Mathf.Abs(rect.height / val2.y) * pixelSize;
		if (num < 0.01f || num2 < 0.01f)
		{
			Debug.LogWarning((object)("The tiled sprite (" + NGUITools.GetHierarchy(((Component)this).gameObject) + ") is too small.\nConsider using a bigger one."));
			num = 0.01f;
			num2 = 0.01f;
		}
		Vector2 val3 = new Vector2(rect.xMin / (float)val.width, rect.yMin / (float)val.height);
		Vector2 val4 = new Vector2(rect.xMax / (float)val.width, rect.yMax / (float)val.height);
		Vector2 val5 = val4;
		Color val6 = color;
		val6.a *= mPanel.alpha;
		Color32 item = Color32.op_Implicit((!atlas.premultipliedAlpha) ? val6 : NGUITools.ApplyPMA(val6));
		for (float num3 = 0f; num3 < 1f; num3 += num2)
		{
			float num4 = 0f;
			val5.x = val4.x;
			float num5 = num3 + num2;
			if (num5 > 1f)
			{
				val5.y = val3.y + (val4.y - val3.y) * (1f - num3) / (num5 - num3);
				num5 = 1f;
			}
			for (; num4 < 1f; num4 += num)
			{
				float num6 = num4 + num;
				if (num6 > 1f)
				{
					val5.x = val3.x + (val4.x - val3.x) * (1f - num4) / (num6 - num4);
					num6 = 1f;
				}
				verts.Add(new Vector3(num6, 0f - num3, 0f));
				verts.Add(new Vector3(num6, 0f - num5, 0f));
				verts.Add(new Vector3(num4, 0f - num5, 0f));
				verts.Add(new Vector3(num4, 0f - num3, 0f));
				uvs.Add(new Vector2(val5.x, 1f - val3.y));
				uvs.Add(new Vector2(val5.x, 1f - val5.y));
				uvs.Add(new Vector2(val3.x, 1f - val5.y));
				uvs.Add(new Vector2(val3.x, 1f - val3.y));
				cols.Add(item);
				cols.Add(item);
				cols.Add(item);
				cols.Add(item);
			}
		}
	}
}
