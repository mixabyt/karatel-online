using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Label")]
public class UILabel : UIWidget
{
	public enum Effect
	{
		None,
		Shadow,
		Outline
	}

	[SerializeField]
	[HideInInspector]
	private UIFont mFont;

	[HideInInspector]
	[SerializeField]
	private string mText = string.Empty;

	[HideInInspector]
	[SerializeField]
	private int mMaxLineWidth;

	[HideInInspector]
	[SerializeField]
	private bool mEncoding = true;

	[SerializeField]
	[HideInInspector]
	private int mMaxLineCount;

	[HideInInspector]
	[SerializeField]
	private bool mPassword;

	[HideInInspector]
	[SerializeField]
	private bool mShowLastChar;

	[HideInInspector]
	[SerializeField]
	private Effect mEffectStyle;

	[SerializeField]
	[HideInInspector]
	private Color mEffectColor = Color.black;

	[SerializeField]
	[HideInInspector]
	private UIFont.SymbolStyle mSymbols = UIFont.SymbolStyle.Uncolored;

	[HideInInspector]
	[SerializeField]
	private Vector2 mEffectDistance = Vector2.one;

	[HideInInspector]
	[SerializeField]
	private bool mShrinkToFit;

	[SerializeField]
	[HideInInspector]
	private float mLineWidth;

	[SerializeField]
	[HideInInspector]
	private bool mMultiline = true;

	private bool mShouldBeProcessed = true;

	private string mProcessedText;

	private Vector3 mLastScale = Vector3.one;

	private string mLastText = string.Empty;

	private int mLastWidth;

	private bool mLastEncoding = true;

	private int mLastCount;

	private bool mLastPass;

	private bool mLastShow;

	private Effect mLastEffect;

	private Vector2 mSize = Vector2.zero;

	private bool mPremultiply;

	private bool hasChanged
	{
		get
		{
			return mShouldBeProcessed || mLastText != text || mLastWidth != mMaxLineWidth || mLastEncoding != mEncoding || mLastCount != mMaxLineCount || mLastPass != mPassword || mLastShow != mShowLastChar || mLastEffect != mEffectStyle;
		}
		set
		{
			if (value)
			{
				mChanged = true;
				mShouldBeProcessed = true;
				return;
			}
			mShouldBeProcessed = false;
			mLastText = text;
			mLastWidth = mMaxLineWidth;
			mLastEncoding = mEncoding;
			mLastCount = mMaxLineCount;
			mLastPass = mPassword;
			mLastShow = mShowLastChar;
			mLastEffect = mEffectStyle;
		}
	}

	public UIFont font
	{
		get
		{
			return mFont;
		}
		set
		{
			if ((Object)(object)mFont != (Object)(object)value)
			{
				mFont = value;
				material = ((!((Object)(object)mFont != (Object)null)) ? null : mFont.material);
				mChanged = true;
				hasChanged = true;
				MarkAsChanged();
			}
		}
	}

	public string text
	{
		get
		{
			return mText;
		}
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				if (!string.IsNullOrEmpty(mText))
				{
					mText = string.Empty;
				}
				hasChanged = true;
			}
			else if (mText != value)
			{
				mText = value;
				hasChanged = true;
				if (shrinkToFit)
				{
					MakePixelPerfect();
				}
			}
		}
	}

	public bool supportEncoding
	{
		get
		{
			return mEncoding;
		}
		set
		{
			if (mEncoding != value)
			{
				mEncoding = value;
				hasChanged = true;
				if (value)
				{
					mPassword = false;
				}
			}
		}
	}

	public UIFont.SymbolStyle symbolStyle
	{
		get
		{
			return mSymbols;
		}
		set
		{
			if (mSymbols != value)
			{
				mSymbols = value;
				hasChanged = true;
			}
		}
	}

	public int lineWidth
	{
		get
		{
			return mMaxLineWidth;
		}
		set
		{
			if (mMaxLineWidth != value)
			{
				mMaxLineWidth = value;
				hasChanged = true;
				if (shrinkToFit)
				{
					MakePixelPerfect();
				}
			}
		}
	}

	public bool multiLine
	{
		get
		{
			return mMaxLineCount != 1;
		}
		set
		{
			if (mMaxLineCount != 1 != value)
			{
				mMaxLineCount = ((!value) ? 1 : 0);
				hasChanged = true;
				if (value)
				{
					mPassword = false;
				}
			}
		}
	}

	public int maxLineCount
	{
		get
		{
			return mMaxLineCount;
		}
		set
		{
			if (mMaxLineCount != value)
			{
				mMaxLineCount = Mathf.Max(value, 0);
				hasChanged = true;
				if (value == 1)
				{
					mPassword = false;
				}
			}
		}
	}

	public bool password
	{
		get
		{
			return mPassword;
		}
		set
		{
			if (mPassword != value)
			{
				if (value)
				{
					mMaxLineCount = 1;
					mEncoding = false;
				}
				mPassword = value;
				hasChanged = true;
			}
		}
	}

	public bool showLastPasswordChar
	{
		get
		{
			return mShowLastChar;
		}
		set
		{
			if (mShowLastChar != value)
			{
				mShowLastChar = value;
				hasChanged = true;
			}
		}
	}

	public Effect effectStyle
	{
		get
		{
			return mEffectStyle;
		}
		set
		{
			if (mEffectStyle != value)
			{
				mEffectStyle = value;
				hasChanged = true;
			}
		}
	}

	public Color effectColor
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mEffectColor;
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			if (!mEffectColor.Equals((object)value))
			{
				mEffectColor = value;
				if (mEffectStyle != Effect.None)
				{
					hasChanged = true;
				}
			}
		}
	}

	public Vector2 effectDistance
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mEffectDistance;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (mEffectDistance != value)
			{
				mEffectDistance = value;
				hasChanged = true;
			}
		}
	}

	public bool shrinkToFit
	{
		get
		{
			return mShrinkToFit;
		}
		set
		{
			if (mShrinkToFit != value)
			{
				mShrinkToFit = value;
				hasChanged = true;
			}
		}
	}

	public string processedText
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			if (mLastScale != cachedTransform.localScale)
			{
				mLastScale = cachedTransform.localScale;
				mShouldBeProcessed = true;
			}
			if (hasChanged)
			{
				ProcessText();
			}
			return mProcessedText;
		}
	}

	public override Material material
	{
		get
		{
			Material val = base.material;
			if ((Object)(object)val == (Object)null)
			{
				val = (material = ((!((Object)(object)mFont != (Object)null)) ? null : mFont.material));
			}
			return val;
		}
	}

	public override Vector2 relativeSize
	{
		get
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)mFont == (Object)null)
			{
				return Vector2.op_Implicit(Vector3.one);
			}
			if (hasChanged)
			{
				ProcessText();
			}
			return mSize;
		}
	}

	public UILabel()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
	}

	protected override void OnStart()
	{
		if (mLineWidth > 0f)
		{
			mMaxLineWidth = Mathf.RoundToInt(mLineWidth);
			mLineWidth = 0f;
		}
		if (!mMultiline)
		{
			mMaxLineCount = 1;
			mMultiline = true;
		}
		mPremultiply = (Object)(object)font != (Object)null && (Object)(object)font.material != (Object)null && ((Object)font.material.shader).name.Contains("Premultiplied");
	}

	public override void MarkAsChanged()
	{
		hasChanged = true;
		base.MarkAsChanged();
	}

	private void ProcessText()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		mChanged = true;
		hasChanged = false;
		mLastText = mText;
		float num = Mathf.Abs(cachedTransform.localScale.x);
		float num2 = mFont.size * mMaxLineCount;
		if (num > 0f)
		{
			while (true)
			{
				if (mPassword)
				{
					mProcessedText = string.Empty;
					if (mShowLastChar)
					{
						int i = 0;
						for (int num3 = mText.Length - 1; i < num3; i++)
						{
							mProcessedText += "*";
						}
						if (mText.Length > 0)
						{
							mProcessedText += mText[mText.Length - 1];
						}
					}
					else
					{
						int j = 0;
						for (int length = mText.Length; j < length; j++)
						{
							mProcessedText += "*";
						}
					}
					mProcessedText = mFont.WrapText(mProcessedText, (float)mMaxLineWidth / num, mMaxLineCount, encoding: false, UIFont.SymbolStyle.None);
				}
				else if (mMaxLineWidth > 0)
				{
					mProcessedText = mFont.WrapText(mText, (float)mMaxLineWidth / num, (!mShrinkToFit) ? mMaxLineCount : 0, mEncoding, mSymbols);
				}
				else if (!mShrinkToFit && mMaxLineCount > 0)
				{
					mProcessedText = mFont.WrapText(mText, 100000f, mMaxLineCount, mEncoding, mSymbols);
				}
				else
				{
					mProcessedText = mText;
				}
				mSize = (string.IsNullOrEmpty(mProcessedText) ? Vector2.one : mFont.CalculatePrintedSize(mProcessedText, mEncoding, mSymbols));
				if (!mShrinkToFit)
				{
					break;
				}
				if (mMaxLineCount > 0 && mSize.y * num > num2)
				{
					num = Mathf.Round(num - 1f);
					if (num > 1f)
					{
						continue;
					}
				}
				if (mMaxLineWidth > 0)
				{
					float num4 = (float)mMaxLineWidth / num;
					float num5 = ((!(mSize.x * num > num4)) ? num : (num4 / mSize.x * num));
					num = Mathf.Min(num5, num);
				}
				num = Mathf.Round(num);
				cachedTransform.localScale = new Vector3(num, num, 1f);
				break;
			}
			mSize.x = Mathf.Max(mSize.x, (!(num > 0f)) ? 1f : ((float)lineWidth / num));
		}
		else
		{
			mSize.x = 1f;
			num = mFont.size;
			cachedTransform.localScale = new Vector3(0.01f, 0.01f, 1f);
			mProcessedText = string.Empty;
		}
		mSize.y = Mathf.Max(mSize.y, 1f);
	}

	public override void MakePixelPerfect()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)mFont != (Object)null)
		{
			float pixelSize = font.pixelSize;
			Vector3 localScale = cachedTransform.localScale;
			localScale.x = (float)mFont.size * pixelSize;
			localScale.y = localScale.x;
			localScale.z = 1f;
			Vector3 localPosition = cachedTransform.localPosition;
			localPosition.x = Mathf.CeilToInt(localPosition.x / pixelSize * 4f) >> 2;
			localPosition.y = Mathf.CeilToInt(localPosition.y / pixelSize * 4f) >> 2;
			localPosition.z = Mathf.RoundToInt(localPosition.z);
			localPosition.x *= pixelSize;
			localPosition.y *= pixelSize;
			cachedTransform.localPosition = localPosition;
			cachedTransform.localScale = localScale;
			if (shrinkToFit)
			{
				ProcessText();
			}
		}
		else
		{
			base.MakePixelPerfect();
		}
	}

	private void ApplyShadow(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols, int start, int end, float x, float y)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		Color val = mEffectColor;
		val.a *= alpha * mPanel.alpha;
		Color32 val2 = Color32.op_Implicit((!font.premultipliedAlpha) ? val : NGUITools.ApplyPMA(val));
		for (int i = start; i < end; i++)
		{
			verts.Add(verts.buffer[i]);
			uvs.Add(uvs.buffer[i]);
			cols.Add(cols.buffer[i]);
			Vector3 val3 = verts.buffer[i];
			val3.x += x;
			val3.y += y;
			verts.buffer[i] = val3;
			cols.buffer[i] = val2;
		}
	}

	public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)mFont == (Object)null)
		{
			return;
		}
		Pivot pivot = base.pivot;
		int size = verts.size;
		Color val = color;
		val.a *= mPanel.alpha;
		if (font.premultipliedAlpha)
		{
			val = NGUITools.ApplyPMA(val);
		}
		switch (pivot)
		{
		case Pivot.TopLeft:
		case Pivot.Left:
		case Pivot.BottomLeft:
			mFont.Print(processedText, Color32.op_Implicit(val), verts, uvs, cols, mEncoding, mSymbols, UIFont.Alignment.Left, 0, mPremultiply);
			break;
		case Pivot.TopRight:
		case Pivot.Right:
		case Pivot.BottomRight:
			mFont.Print(processedText, Color32.op_Implicit(val), verts, uvs, cols, mEncoding, mSymbols, UIFont.Alignment.Right, Mathf.RoundToInt(relativeSize.x * (float)mFont.size), mPremultiply);
			break;
		default:
			mFont.Print(processedText, Color32.op_Implicit(val), verts, uvs, cols, mEncoding, mSymbols, UIFont.Alignment.Center, Mathf.RoundToInt(relativeSize.x * (float)mFont.size), mPremultiply);
			break;
		}
		if (effectStyle == Effect.None)
		{
			return;
		}
		int size2 = verts.size;
		float num = 1f / (float)mFont.size;
		float num2 = num * mEffectDistance.x;
		float num3 = num * mEffectDistance.y;
		ApplyShadow(verts, uvs, cols, size, size2, num2, 0f - num3);
		if (effectStyle == Effect.Outline)
		{
			size = size2;
			size2 = verts.size;
			ApplyShadow(verts, uvs, cols, size, size2, 0f - num2, num3);
			size = size2;
			size2 = verts.size;
			ApplyShadow(verts, uvs, cols, size, size2, num2, num3);
			size = size2;
			size2 = verts.size;
			ApplyShadow(verts, uvs, cols, size, size2, 0f - num2, 0f - num3);
			if (mEffectDistance.x > 1f && mEffectDistance.y > 1f)
			{
				size = size2;
				size2 = verts.size;
				ApplyShadow(verts, uvs, cols, size, size2, 0f, num3);
				size = size2;
				size2 = verts.size;
				ApplyShadow(verts, uvs, cols, size, size2, 0f, 0f - num3);
				size = size2;
				size2 = verts.size;
				ApplyShadow(verts, uvs, cols, size, size2, num2, 0f);
				size = size2;
				size2 = verts.size;
				ApplyShadow(verts, uvs, cols, size, size2, 0f - num2, 0f);
			}
		}
	}
}
