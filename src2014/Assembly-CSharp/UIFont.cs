using System.Collections.Generic;
using System.Text;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Font")]
public class UIFont : MonoBehaviour
{
	public enum Alignment
	{
		Left,
		Center,
		Right
	}

	public enum SymbolStyle
	{
		None,
		Uncolored,
		Colored
	}

	[SerializeField]
	[HideInInspector]
	private Material mMat;

	[SerializeField]
	[HideInInspector]
	private Rect mUVRect = new Rect(0f, 0f, 1f, 1f);

	[HideInInspector]
	[SerializeField]
	private BMFont mFont = new BMFont();

	[HideInInspector]
	[SerializeField]
	private int mSpacingX;

	[HideInInspector]
	[SerializeField]
	private int mSpacingY;

	[SerializeField]
	[HideInInspector]
	private UIAtlas mAtlas;

	[SerializeField]
	[HideInInspector]
	private UIFont mReplacement;

	[HideInInspector]
	[SerializeField]
	private float mPixelSize = 1f;

	[HideInInspector]
	[SerializeField]
	private List<BMSymbol> mSymbols = new List<BMSymbol>();

	[SerializeField]
	[HideInInspector]
	private Font mDynamicFont;

	[SerializeField]
	[HideInInspector]
	private int mDynamicFontSize = 16;

	[SerializeField]
	[HideInInspector]
	private FontStyle mDynamicFontStyle;

	[SerializeField]
	[HideInInspector]
	private float mDynamicFontOffset;

	private UIAtlas.Sprite mSprite;

	private int mPMA = -1;

	private bool mSpriteSet;

	private List<Color> mColors = new List<Color>();

	private static CharacterInfo mChar;

	public BMFont bmFont => (!((Object)(object)mReplacement != (Object)null)) ? mFont : mReplacement.bmFont;

	public int texWidth => ((Object)(object)mReplacement != (Object)null) ? mReplacement.texWidth : ((mFont == null) ? 1 : mFont.texWidth);

	public int texHeight => ((Object)(object)mReplacement != (Object)null) ? mReplacement.texHeight : ((mFont == null) ? 1 : mFont.texHeight);

	public bool hasSymbols => (!((Object)(object)mReplacement != (Object)null)) ? (mSymbols.Count != 0) : mReplacement.hasSymbols;

	public List<BMSymbol> symbols => (!((Object)(object)mReplacement != (Object)null)) ? mSymbols : mReplacement.symbols;

	public UIAtlas atlas
	{
		get
		{
			return (!((Object)(object)mReplacement != (Object)null)) ? mAtlas : mReplacement.atlas;
		}
		set
		{
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)mReplacement != (Object)null)
			{
				mReplacement.atlas = value;
			}
			else
			{
				if (!((Object)(object)mAtlas != (Object)(object)value))
				{
					return;
				}
				if ((Object)(object)value == (Object)null)
				{
					if ((Object)(object)mAtlas != (Object)null)
					{
						mMat = mAtlas.spriteMaterial;
					}
					if (sprite != null)
					{
						mUVRect = uvRect;
					}
				}
				mPMA = -1;
				mAtlas = value;
				MarkAsDirty();
			}
		}
	}

	public Material material
	{
		get
		{
			if ((Object)(object)mReplacement != (Object)null)
			{
				return mReplacement.material;
			}
			if ((Object)(object)mAtlas != (Object)null)
			{
				return mAtlas.spriteMaterial;
			}
			if ((Object)(object)mMat != (Object)null)
			{
				if ((Object)(object)mDynamicFont != (Object)null && (Object)(object)mMat != (Object)(object)mDynamicFont.material)
				{
					mMat.mainTexture = mDynamicFont.material.mainTexture;
				}
				return mMat;
			}
			if ((Object)(object)mDynamicFont != (Object)null)
			{
				return mDynamicFont.material;
			}
			return null;
		}
		set
		{
			if ((Object)(object)mReplacement != (Object)null)
			{
				mReplacement.material = value;
			}
			else if ((Object)(object)mMat != (Object)(object)value)
			{
				mPMA = -1;
				mMat = value;
				MarkAsDirty();
			}
		}
	}

	public float pixelSize
	{
		get
		{
			if ((Object)(object)mReplacement != (Object)null)
			{
				return mReplacement.pixelSize;
			}
			if ((Object)(object)mAtlas != (Object)null)
			{
				return mAtlas.pixelSize;
			}
			return mPixelSize;
		}
		set
		{
			if ((Object)(object)mReplacement != (Object)null)
			{
				mReplacement.pixelSize = value;
				return;
			}
			if ((Object)(object)mAtlas != (Object)null)
			{
				mAtlas.pixelSize = value;
				return;
			}
			float num = Mathf.Clamp(value, 0.25f, 4f);
			if (mPixelSize != num)
			{
				mPixelSize = num;
				MarkAsDirty();
			}
		}
	}

	public bool premultipliedAlpha
	{
		get
		{
			if ((Object)(object)mReplacement != (Object)null)
			{
				return mReplacement.premultipliedAlpha;
			}
			if ((Object)(object)mAtlas != (Object)null)
			{
				return mAtlas.premultipliedAlpha;
			}
			if (mPMA == -1)
			{
				Material val = material;
				mPMA = (((Object)(object)val != (Object)null && (Object)(object)val.shader != (Object)null && ((Object)val.shader).name.Contains("Premultiplied")) ? 1 : 0);
			}
			return mPMA == 1;
		}
	}

	public Texture2D texture
	{
		get
		{
			if ((Object)(object)mReplacement != (Object)null)
			{
				return mReplacement.texture;
			}
			Material val = material;
			object result;
			if ((Object)(object)val != (Object)null)
			{
				Texture mainTexture = val.mainTexture;
				result = ((mainTexture is Texture2D) ? mainTexture : null);
			}
			else
			{
				result = null;
			}
			return (Texture2D)result;
		}
	}

	public Rect uvRect
	{
		get
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)mReplacement != (Object)null)
			{
				return mReplacement.uvRect;
			}
			if ((Object)(object)mAtlas != (Object)null && mSprite == null && sprite != null)
			{
				Texture val = mAtlas.texture;
				if ((Object)(object)val != (Object)null)
				{
					mUVRect = mSprite.outer;
					if (mAtlas.coordinates == UIAtlas.Coordinates.Pixels)
					{
						mUVRect = NGUIMath.ConvertToTexCoords(mUVRect, val.width, val.height);
					}
					if (mSprite.hasPadding)
					{
						Rect val2 = mUVRect;
						mUVRect.xMin = val2.xMin - mSprite.paddingLeft * val2.width;
						mUVRect.yMin = val2.yMin - mSprite.paddingBottom * val2.height;
						mUVRect.xMax = val2.xMax + mSprite.paddingRight * val2.width;
						mUVRect.yMax = val2.yMax + mSprite.paddingTop * val2.height;
					}
					if (mSprite.hasPadding)
					{
						Trim();
					}
				}
			}
			return mUVRect;
		}
		set
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)mReplacement != (Object)null)
			{
				mReplacement.uvRect = value;
			}
			else if (sprite == null && mUVRect != value)
			{
				mUVRect = value;
				MarkAsDirty();
			}
		}
	}

	public string spriteName
	{
		get
		{
			return (!((Object)(object)mReplacement != (Object)null)) ? mFont.spriteName : mReplacement.spriteName;
		}
		set
		{
			if ((Object)(object)mReplacement != (Object)null)
			{
				mReplacement.spriteName = value;
			}
			else if (mFont.spriteName != value)
			{
				mFont.spriteName = value;
				MarkAsDirty();
			}
		}
	}

	public int horizontalSpacing
	{
		get
		{
			return (!((Object)(object)mReplacement != (Object)null)) ? mSpacingX : mReplacement.horizontalSpacing;
		}
		set
		{
			if ((Object)(object)mReplacement != (Object)null)
			{
				mReplacement.horizontalSpacing = value;
			}
			else if (mSpacingX != value)
			{
				mSpacingX = value;
				MarkAsDirty();
			}
		}
	}

	public int verticalSpacing
	{
		get
		{
			return (!((Object)(object)mReplacement != (Object)null)) ? mSpacingY : mReplacement.verticalSpacing;
		}
		set
		{
			if ((Object)(object)mReplacement != (Object)null)
			{
				mReplacement.verticalSpacing = value;
			}
			else if (mSpacingY != value)
			{
				mSpacingY = value;
				MarkAsDirty();
			}
		}
	}

	public bool isValid => (Object)(object)mDynamicFont != (Object)null || mFont.isValid;

	public int size
	{
		get
		{
			int result;
			if ((Object)(object)mReplacement != (Object)null)
			{
				result = mReplacement.size;
			}
			else
			{
				result = ((!isDynamic) ? mFont.charSize : mDynamicFontSize);
			}
			return result;
		}
	}

	public UIAtlas.Sprite sprite
	{
		get
		{
			if ((Object)(object)mReplacement != (Object)null)
			{
				return mReplacement.sprite;
			}
			if (!mSpriteSet)
			{
				mSprite = null;
			}
			if (mSprite == null)
			{
				if ((Object)(object)mAtlas != (Object)null && !string.IsNullOrEmpty(mFont.spriteName))
				{
					mSprite = mAtlas.GetSprite(mFont.spriteName);
					if (mSprite == null)
					{
						mSprite = mAtlas.GetSprite(((Object)this).name);
					}
					mSpriteSet = true;
					if (mSprite == null)
					{
						mFont.spriteName = null;
					}
				}
				int i = 0;
				for (int count = mSymbols.Count; i < count; i++)
				{
					symbols[i].MarkAsDirty();
				}
			}
			return mSprite;
		}
	}

	public UIFont replacement
	{
		get
		{
			return mReplacement;
		}
		set
		{
			UIFont uIFont = value;
			if ((Object)(object)uIFont == (Object)(object)this)
			{
				uIFont = null;
			}
			if ((Object)(object)mReplacement != (Object)(object)uIFont)
			{
				if ((Object)(object)uIFont != (Object)null && (Object)(object)uIFont.replacement == (Object)(object)this)
				{
					uIFont.replacement = null;
				}
				if ((Object)(object)mReplacement != (Object)null)
				{
					MarkAsDirty();
				}
				mReplacement = uIFont;
				MarkAsDirty();
			}
		}
	}

	public bool isDynamic => (Object)(object)mDynamicFont != (Object)null;

	public Font dynamicFont
	{
		get
		{
			return (!((Object)(object)mReplacement != (Object)null)) ? mDynamicFont : mReplacement.dynamicFont;
		}
		set
		{
			if ((Object)(object)mReplacement != (Object)null)
			{
				mReplacement.dynamicFont = value;
			}
			else if ((Object)(object)mDynamicFont != (Object)(object)value)
			{
				if ((Object)(object)mDynamicFont != (Object)null)
				{
					material = null;
				}
				mDynamicFont = value;
				MarkAsDirty();
			}
		}
	}

	public int dynamicFontSize
	{
		get
		{
			return (!((Object)(object)mReplacement != (Object)null)) ? mDynamicFontSize : mReplacement.dynamicFontSize;
		}
		set
		{
			if ((Object)(object)mReplacement != (Object)null)
			{
				mReplacement.dynamicFontSize = value;
				return;
			}
			value = Mathf.Clamp(value, 4, 128);
			if (mDynamicFontSize != value)
			{
				mDynamicFontSize = value;
				MarkAsDirty();
			}
		}
	}

	public FontStyle dynamicFontStyle
	{
		get
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			return (!((Object)(object)mReplacement != (Object)null)) ? mDynamicFontStyle : mReplacement.dynamicFontStyle;
		}
		set
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)mReplacement != (Object)null)
			{
				mReplacement.dynamicFontStyle = value;
			}
			else if (mDynamicFontStyle != value)
			{
				mDynamicFontStyle = value;
				MarkAsDirty();
			}
		}
	}

	private Texture dynamicTexture
	{
		get
		{
			if (Object.op_Implicit((Object)(object)mReplacement))
			{
				return mReplacement.dynamicTexture;
			}
			if (isDynamic)
			{
				return mDynamicFont.material.mainTexture;
			}
			return null;
		}
	}

	public UIFont()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
	}

	private void Trim()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		Texture val = mAtlas.texture;
		if ((Object)(object)val != (Object)null && mSprite != null)
		{
			Rect val2 = NGUIMath.ConvertToPixels(mUVRect, ((Texture)texture).width, ((Texture)texture).height, round: true);
			Rect val3 = ((mAtlas.coordinates != UIAtlas.Coordinates.TexCoords) ? mSprite.outer : NGUIMath.ConvertToPixels(mSprite.outer, val.width, val.height, round: true));
			int xMin = Mathf.RoundToInt(val3.xMin - val2.xMin);
			int yMin = Mathf.RoundToInt(val3.yMin - val2.yMin);
			int xMax = Mathf.RoundToInt(val3.xMax - val2.xMin);
			int yMax = Mathf.RoundToInt(val3.yMax - val2.yMin);
			mFont.Trim(xMin, yMin, xMax, yMax);
		}
	}

	private bool References(UIFont font)
	{
		if ((Object)(object)font == (Object)null)
		{
			return false;
		}
		if ((Object)(object)font == (Object)(object)this)
		{
			return true;
		}
		return (Object)(object)mReplacement != (Object)null && mReplacement.References(font);
	}

	public static bool CheckIfRelated(UIFont a, UIFont b)
	{
		if ((Object)(object)a == (Object)null || (Object)(object)b == (Object)null)
		{
			return false;
		}
		if (a.isDynamic && b.isDynamic && a.dynamicFont.fontNames[0] == b.dynamicFont.fontNames[0])
		{
			return true;
		}
		return (Object)(object)a == (Object)(object)b || a.References(b) || b.References(a);
	}

	public void MarkAsDirty()
	{
		if ((Object)(object)mReplacement != (Object)null)
		{
			mReplacement.MarkAsDirty();
		}
		RecalculateDynamicOffset();
		mSprite = null;
		UILabel[] array = NGUITools.FindActive<UILabel>();
		int i = 0;
		for (int num = array.Length; i < num; i++)
		{
			UILabel uILabel = array[i];
			if (((Behaviour)uILabel).enabled && NGUITools.GetActive(((Component)uILabel).gameObject) && CheckIfRelated(this, uILabel.font))
			{
				UIFont font = uILabel.font;
				uILabel.font = null;
				uILabel.font = font;
			}
		}
		int j = 0;
		for (int count = mSymbols.Count; j < count; j++)
		{
			symbols[j].MarkAsDirty();
		}
	}

	public bool RecalculateDynamicOffset()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)mDynamicFont != (Object)null)
		{
			mDynamicFont.RequestCharactersInTexture("j", mDynamicFontSize, mDynamicFontStyle);
			CharacterInfo val = default;
			mDynamicFont.GetCharacterInfo('j', ref val, mDynamicFontSize, mDynamicFontStyle);
			float num = (float)mDynamicFontSize + val.vert.yMax;
			if (!object.Equals(mDynamicFontOffset, num))
			{
				mDynamicFontOffset = num;
				return true;
			}
		}
		return false;
	}

	public Vector2 CalculatePrintedSize(string text, bool encoding, SymbolStyle symbolStyle)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Expected Obj, but got Unknown
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)mReplacement != (Object)null)
		{
			return mReplacement.CalculatePrintedSize(text, encoding, symbolStyle);
		}
		Vector2 zero = Vector2.zero;
		bool flag = isDynamic;
		if (flag || (mFont != null && mFont.isValid && !string.IsNullOrEmpty(text)))
		{
			if (encoding)
			{
				text = NGUITools.StripSymbols(text);
			}
			if (flag)
			{
				mDynamicFont.textureRebuildCallback = OnFontChanged;
				mDynamicFont.RequestCharactersInTexture(text, mDynamicFontSize, mDynamicFontStyle);
				mDynamicFont.textureRebuildCallback = null;
			}
			int length = text.Length;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = size;
			int num6 = num5 + mSpacingY;
			bool flag2 = encoding && symbolStyle != SymbolStyle.None && hasSymbols;
			for (int i = 0; i < length; i++)
			{
				char c = text[i];
				if (c == '\n')
				{
					if (num2 > num)
					{
						num = num2;
					}
					num2 = 0;
					num3 += num6;
					num4 = 0;
				}
				else if (c < ' ')
				{
					num4 = 0;
				}
				else if (!flag)
				{
					BMSymbol bMSymbol = ((!flag2) ? null : MatchSymbol(text, i, length));
					if (bMSymbol == null)
					{
						BMGlyph glyph = mFont.GetGlyph(c);
						if (glyph != null)
						{
							num2 += mSpacingX + ((num4 == 0) ? glyph.advance : (glyph.advance + glyph.GetKerning(num4)));
							num4 = c;
						}
					}
					else
					{
						num2 += mSpacingX + bMSymbol.width;
						i += bMSymbol.length - 1;
						num4 = 0;
					}
				}
				else if (mDynamicFont.GetCharacterInfo(c, ref mChar, mDynamicFontSize, mDynamicFontStyle))
				{
					num2 += (int)((float)mSpacingX + mChar.width);
				}
			}
			float num7 = ((num5 <= 0) ? 1f : (1f / (float)num5));
			zero.x = num7 * (float)((num2 <= num) ? num : num2);
			zero.y = num7 * (float)(num3 + num6);
		}
		return zero;
	}

	private static void EndLine(ref StringBuilder s)
	{
		int num = s.Length - 1;
		if (num > 0 && s[num] == ' ')
		{
			s[num] = '\n';
		}
		else
		{
			s.Append('\n');
		}
	}

	public string GetEndOfLineThatFits(string text, float maxWidth, bool encoding, SymbolStyle symbolStyle)
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected Obj, but got Unknown
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)mReplacement != (Object)null)
		{
			return mReplacement.GetEndOfLineThatFits(text, maxWidth, encoding, symbolStyle);
		}
		int num = Mathf.RoundToInt(maxWidth * (float)size);
		if (num < 1)
		{
			return text;
		}
		int length = text.Length;
		int num2 = num;
		BMGlyph bMGlyph = null;
		int num3 = length;
		bool flag = encoding && symbolStyle != SymbolStyle.None && hasSymbols;
		bool flag2 = isDynamic;
		if (flag2)
		{
			mDynamicFont.textureRebuildCallback = OnFontChanged;
			mDynamicFont.RequestCharactersInTexture(text, mDynamicFontSize, mDynamicFontStyle);
			mDynamicFont.textureRebuildCallback = null;
		}
		while (num3 > 0 && num2 > 0)
		{
			char c = text[--num3];
			BMSymbol bMSymbol = ((!flag) ? null : MatchSymbol(text, num3, length));
			int num4 = mSpacingX;
			if (!flag2)
			{
				if (bMSymbol != null)
				{
					num4 += bMSymbol.advance;
				}
				else
				{
					BMGlyph glyph = mFont.GetGlyph(c);
					if (glyph == null)
					{
						bMGlyph = null;
						continue;
					}
					num4 += glyph.advance + (bMGlyph?.GetKerning(c) ?? 0);
					bMGlyph = glyph;
				}
			}
			else if (mDynamicFont.GetCharacterInfo(c, ref mChar, mDynamicFontSize, mDynamicFontStyle))
			{
				num4 += (int)mChar.width;
			}
			num2 -= num4;
		}
		if (num2 < 0)
		{
			num3++;
		}
		return text.Substring(num3, length - num3);
	}

	public string WrapText(string text, float maxWidth, int maxLineCount, bool encoding, SymbolStyle symbolStyle)
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Expected Obj, but got Unknown
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)mReplacement != (Object)null)
		{
			return mReplacement.WrapText(text, maxWidth, maxLineCount, encoding, symbolStyle);
		}
		int num = Mathf.RoundToInt(maxWidth * (float)size);
		if (num < 1)
		{
			return text;
		}
		StringBuilder s = new StringBuilder();
		int length = text.Length;
		int num2 = num;
		int num3 = 0;
		int i = 0;
		int j = 0;
		bool flag = true;
		bool flag2 = maxLineCount != 1;
		int num4 = 1;
		bool flag3 = encoding && symbolStyle != SymbolStyle.None && hasSymbols;
		bool flag4 = isDynamic;
		if (flag4)
		{
			mDynamicFont.textureRebuildCallback = OnFontChanged;
			mDynamicFont.RequestCharactersInTexture(text, mDynamicFontSize, mDynamicFontStyle);
			mDynamicFont.textureRebuildCallback = null;
		}
		for (; j < length; j++)
		{
			char c = text[j];
			if (c == '\n')
			{
				if (!flag2 || num4 == maxLineCount)
				{
					break;
				}
				num2 = num;
				if (i < j)
				{
					s.Append(text.Substring(i, j - i + 1));
				}
				else
				{
					s.Append(c);
				}
				flag = true;
				num4++;
				i = j + 1;
				num3 = 0;
				continue;
			}
			if (c == ' ' && num3 != 32 && i < j)
			{
				s.Append(text.Substring(i, j - i + 1));
				flag = false;
				i = j + 1;
				num3 = c;
			}
			if (encoding && c == '[' && j + 2 < length)
			{
				if (text[j + 1] == '-' && text[j + 2] == ']')
				{
					j += 2;
					continue;
				}
				if (j + 7 < length && text[j + 7] == ']' && NGUITools.EncodeColor(NGUITools.ParseColor(text, j + 1)) == text.Substring(j + 1, 6).ToUpper())
				{
					j += 7;
					continue;
				}
			}
			BMSymbol bMSymbol = ((!flag3) ? null : MatchSymbol(text, j, length));
			int num5 = mSpacingX;
			if (!flag4)
			{
				if (bMSymbol != null)
				{
					num5 += bMSymbol.advance;
				}
				else
				{
					BMGlyph bMGlyph = ((bMSymbol != null) ? null : mFont.GetGlyph(c));
					if (bMGlyph == null)
					{
						continue;
					}
					num5 += ((num3 == 0) ? bMGlyph.advance : (bMGlyph.advance + bMGlyph.GetKerning(num3)));
				}
			}
			else if (mDynamicFont.GetCharacterInfo(c, ref mChar, mDynamicFontSize, mDynamicFontStyle))
			{
				num5 += Mathf.RoundToInt(mChar.width);
			}
			num2 -= num5;
			if (num2 < 0)
			{
				if (!flag && flag2 && num4 != maxLineCount)
				{
					for (; i < length && text[i] == ' '; i++)
					{
					}
					flag = true;
					num2 = num;
					j = i - 1;
					num3 = 0;
					if (!flag2 || num4 == maxLineCount)
					{
						break;
					}
					num4++;
					EndLine(ref s);
					continue;
				}
				s.Append(text.Substring(i, Mathf.Max(0, j - i)));
				if (!flag2 || num4 == maxLineCount)
				{
					i = j;
					break;
				}
				EndLine(ref s);
				flag = true;
				num4++;
				if (c == ' ')
				{
					i = j + 1;
					num2 = num;
				}
				else
				{
					i = j;
					num2 = num - num5;
				}
				num3 = 0;
			}
			else
			{
				num3 = c;
			}
			if (!flag4 && bMSymbol != null)
			{
				j += bMSymbol.length - 1;
				num3 = 0;
			}
		}
		if (i < j)
		{
			s.Append(text.Substring(i, j - i));
		}
		return s.ToString();
	}

	public string WrapText(string text, float maxWidth, int maxLineCount, bool encoding)
	{
		return WrapText(text, maxWidth, maxLineCount, encoding, SymbolStyle.None);
	}

	public string WrapText(string text, float maxWidth, int maxLineCount)
	{
		return WrapText(text, maxWidth, maxLineCount, encoding: false, SymbolStyle.None);
	}

	private void Align(BetterList<Vector3> verts, int indexOffset, Alignment alignment, int x, int lineWidth)
	{
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		if (alignment == Alignment.Left)
		{
			return;
		}
		int num = size;
		if (num <= 0)
		{
			return;
		}
		float num2 = 0f;
		if (alignment == Alignment.Right)
		{
			num2 = Mathf.RoundToInt((float)(lineWidth - x));
			if (num2 < 0f)
			{
				num2 = 0f;
			}
			num2 /= (float)size;
		}
		else
		{
			num2 = Mathf.RoundToInt((float)(lineWidth - x) * 0.5f);
			if (num2 < 0f)
			{
				num2 = 0f;
			}
			num2 /= (float)size;
			if ((lineWidth & 1) == 1)
			{
				num2 += 0.5f / (float)num;
			}
		}
		for (int i = indexOffset; i < verts.size; i++)
		{
			Vector3 val = verts.buffer[i];
			val.x += num2;
			verts.buffer[i] = val;
		}
	}

	private void OnFontChanged()
	{
		MarkAsDirty();
	}

	public void Print(string text, Color32 color, BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols, bool encoding, SymbolStyle symbolStyle, Alignment alignment, int lineWidth, bool premultiply)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Expected Obj, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0810: Unknown result type (might be due to invalid IL or missing references)
		//IL_053f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0544: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0843: Unknown result type (might be due to invalid IL or missing references)
		//IL_085d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0877: Unknown result type (might be due to invalid IL or missing references)
		//IL_0891: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0917: Unknown result type (might be due to invalid IL or missing references)
		//IL_0930: Unknown result type (might be due to invalid IL or missing references)
		//IL_0949: Unknown result type (might be due to invalid IL or missing references)
		//IL_0962: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0615: Unknown result type (might be due to invalid IL or missing references)
		//IL_062e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0647: Unknown result type (might be due to invalid IL or missing references)
		//IL_0660: Unknown result type (might be due to invalid IL or missing references)
		//IL_067a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0694: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)mReplacement != (Object)null)
		{
			mReplacement.Print(text, color, verts, uvs, cols, encoding, symbolStyle, alignment, lineWidth, premultiply);
		}
		else
		{
			if (text == null)
			{
				return;
			}
			if (!isValid)
			{
				Debug.LogError((object)"Attempting to print using an invalid font!");
				return;
			}
			bool flag = isDynamic;
			if (flag)
			{
				mDynamicFont.textureRebuildCallback = OnFontChanged;
				mDynamicFont.RequestCharactersInTexture(text, mDynamicFontSize, mDynamicFontStyle);
				mDynamicFont.textureRebuildCallback = null;
			}
			mColors.Clear();
			mColors.Add(Color32.op_Implicit(color));
			int num = size;
			Vector2 val = (Vector2)((num <= 0) ? Vector2.one : new Vector2(1f / (float)num, 1f / (float)num));
			int num2 = verts.size;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = num + mSpacingY;
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			Vector2 zero3 = Vector2.zero;
			Vector2 zero4 = Vector2.zero;
			Rect val2 = uvRect;
			float num8 = val2.width / (float)mFont.texWidth;
			float num9 = mUVRect.height / (float)mFont.texHeight;
			int length = text.Length;
			bool flag2 = encoding && symbolStyle != SymbolStyle.None && hasSymbols && sprite != null;
			for (int i = 0; i < length; i++)
			{
				char c = text[i];
				if (c == '\n')
				{
					if (num4 > num3)
					{
						num3 = num4;
					}
					if (alignment != Alignment.Left)
					{
						Align(verts, num2, alignment, num4, lineWidth);
						num2 = verts.size;
					}
					num4 = 0;
					num5 += num7;
					num6 = 0;
					continue;
				}
				if (c < ' ')
				{
					num6 = 0;
					continue;
				}
				if (encoding && c == '[')
				{
					int num10 = NGUITools.ParseSymbol(text, i, mColors, premultiply);
					if (num10 > 0)
					{
						color = Color32.op_Implicit(mColors[mColors.Count - 1]);
						i += num10 - 1;
						continue;
					}
				}
				if (!flag)
				{
					BMSymbol bMSymbol = ((!flag2) ? null : MatchSymbol(text, i, length));
					if (bMSymbol == null)
					{
						BMGlyph glyph = mFont.GetGlyph(c);
						if (glyph == null)
						{
							continue;
						}
						if (num6 != 0)
						{
							num4 += glyph.GetKerning(num6);
						}
						if (c == ' ')
						{
							num4 += mSpacingX + glyph.advance;
							num6 = c;
							continue;
						}
						zero.x = val.x * (float)(num4 + glyph.offsetX);
						zero.y = (0f - val.y) * (float)(num5 + glyph.offsetY);
						zero2.x = zero.x + val.x * (float)glyph.width;
						zero2.y = zero.y - val.y * (float)glyph.height;
						zero3.x = mUVRect.xMin + num8 * (float)glyph.x;
						zero3.y = mUVRect.yMax - num9 * (float)glyph.y;
						zero4.x = zero3.x + num8 * (float)glyph.width;
						zero4.y = zero3.y - num9 * (float)glyph.height;
						num4 += mSpacingX + glyph.advance;
						num6 = c;
						if (glyph.channel == 0 || glyph.channel == 15)
						{
							for (int j = 0; j < 4; j++)
							{
								cols.Add(color);
							}
						}
						else
						{
							Color val3 = Color32.op_Implicit(color);
							val3 *= 0.49f;
							switch (glyph.channel)
							{
							case 1:
								val3.b += 0.51f;
								break;
							case 2:
								val3.g += 0.51f;
								break;
							case 4:
								val3.r += 0.51f;
								break;
							case 8:
								val3.a += 0.51f;
								break;
							}
							for (int k = 0; k < 4; k++)
							{
								cols.Add(Color32.op_Implicit(val3));
							}
						}
					}
					else
					{
						zero.x = val.x * (float)(num4 + bMSymbol.offsetX);
						zero.y = (0f - val.y) * (float)(num5 + bMSymbol.offsetY);
						zero2.x = zero.x + val.x * (float)bMSymbol.width;
						zero2.y = zero.y - val.y * (float)bMSymbol.height;
						Rect val4 = bMSymbol.uvRect;
						zero3.x = val4.xMin;
						zero3.y = val4.yMax;
						zero4.x = val4.xMax;
						zero4.y = val4.yMin;
						num4 += mSpacingX + bMSymbol.advance;
						i += bMSymbol.length - 1;
						num6 = 0;
						if (symbolStyle == SymbolStyle.Colored)
						{
							for (int l = 0; l < 4; l++)
							{
								cols.Add(color);
							}
						}
						else
						{
							Color32 item = Color32.op_Implicit(Color.white);
							item.a = color.a;
							for (int m = 0; m < 4; m++)
							{
								cols.Add(item);
							}
						}
					}
					verts.Add(new Vector3(zero2.x, zero.y));
					verts.Add(new Vector3(zero2.x, zero2.y));
					verts.Add(new Vector3(zero.x, zero2.y));
					verts.Add(new Vector3(zero.x, zero.y));
					uvs.Add(new Vector2(zero4.x, zero3.y));
					uvs.Add(new Vector2(zero4.x, zero4.y));
					uvs.Add(new Vector2(zero3.x, zero4.y));
					uvs.Add(new Vector2(zero3.x, zero3.y));
				}
				else if (mDynamicFont.GetCharacterInfo(c, ref mChar, mDynamicFontSize, mDynamicFontStyle))
				{
					zero.x = val.x * ((float)num4 + mChar.vert.xMin);
					zero.y = (0f - val.y) * ((float)num5 - mChar.vert.yMax + mDynamicFontOffset);
					zero2.x = zero.x + val.x * mChar.vert.width;
					zero2.y = zero.y - val.y * mChar.vert.height;
					zero3.x = mChar.uv.xMin;
					zero3.y = mChar.uv.yMin;
					zero4.x = mChar.uv.xMax;
					zero4.y = mChar.uv.yMax;
					num4 += mSpacingX + (int)mChar.width;
					for (int n = 0; n < 4; n++)
					{
						cols.Add(color);
					}
					if (mChar.flipped)
					{
						uvs.Add(new Vector2(zero3.x, zero4.y));
						uvs.Add(new Vector2(zero3.x, zero3.y));
						uvs.Add(new Vector2(zero4.x, zero3.y));
						uvs.Add(new Vector2(zero4.x, zero4.y));
					}
					else
					{
						uvs.Add(new Vector2(zero4.x, zero3.y));
						uvs.Add(new Vector2(zero3.x, zero3.y));
						uvs.Add(new Vector2(zero3.x, zero4.y));
						uvs.Add(new Vector2(zero4.x, zero4.y));
					}
					verts.Add(new Vector3(zero2.x, zero.y));
					verts.Add(new Vector3(zero.x, zero.y));
					verts.Add(new Vector3(zero.x, zero2.y));
					verts.Add(new Vector3(zero2.x, zero2.y));
				}
			}
			if (alignment != Alignment.Left && num2 < verts.size)
			{
				Align(verts, num2, alignment, num4, lineWidth);
				num2 = verts.size;
			}
		}
	}

	private BMSymbol GetSymbol(string sequence, bool createIfMissing)
	{
		int i = 0;
		for (int count = mSymbols.Count; i < count; i++)
		{
			BMSymbol bMSymbol = mSymbols[i];
			if (bMSymbol.sequence == sequence)
			{
				return bMSymbol;
			}
		}
		if (createIfMissing)
		{
			BMSymbol bMSymbol2 = new BMSymbol();
			bMSymbol2.sequence = sequence;
			mSymbols.Add(bMSymbol2);
			return bMSymbol2;
		}
		return null;
	}

	private BMSymbol MatchSymbol(string text, int offset, int textLength)
	{
		int count = mSymbols.Count;
		if (count == 0)
		{
			return null;
		}
		textLength -= offset;
		for (int i = 0; i < count; i++)
		{
			BMSymbol bMSymbol = mSymbols[i];
			int length = bMSymbol.length;
			if (length == 0 || textLength < length)
			{
				continue;
			}
			bool flag = true;
			for (int j = 0; j < length; j++)
			{
				if (text[offset + j] != bMSymbol.sequence[j])
				{
					flag = false;
					break;
				}
			}
			if (flag && bMSymbol.Validate(atlas))
			{
				return bMSymbol;
			}
		}
		return null;
	}

	public void AddSymbol(string sequence, string spriteName)
	{
		BMSymbol symbol = GetSymbol(sequence, createIfMissing: true);
		symbol.spriteName = spriteName;
		MarkAsDirty();
	}

	public void RemoveSymbol(string sequence)
	{
		BMSymbol symbol = GetSymbol(sequence, createIfMissing: false);
		if (symbol != null)
		{
			symbols.Remove(symbol);
		}
		MarkAsDirty();
	}

	public void RenameSymbol(string before, string after)
	{
		BMSymbol symbol = GetSymbol(before, createIfMissing: false);
		if (symbol != null)
		{
			symbol.sequence = after;
		}
		MarkAsDirty();
	}

	public bool UsesSprite(string s)
	{
		if (!string.IsNullOrEmpty(s))
		{
			if (s.Equals(spriteName))
			{
				return true;
			}
			int i = 0;
			for (int count = symbols.Count; i < count; i++)
			{
				BMSymbol bMSymbol = symbols[i];
				if (s.Equals(bMSymbol.spriteName))
				{
					return true;
				}
			}
		}
		return false;
	}
}
