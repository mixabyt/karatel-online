using System;
using UnityEngine;

[Serializable]
public class BMSymbol
{
	public string sequence;

	public string spriteName;

	private UIAtlas.Sprite mSprite;

	private bool mIsValid;

	private int mLength;

	private int mOffsetX;

	private int mOffsetY;

	private int mWidth;

	private int mHeight;

	private int mAdvance;

	private Rect mUV;

	public int length
	{
		get
		{
			if (mLength == 0)
			{
				mLength = sequence.Length;
			}
			return mLength;
		}
	}

	public int offsetX => mOffsetX;

	public int offsetY => mOffsetY;

	public int width => mWidth;

	public int height => mHeight;

	public int advance => mAdvance;

	public Rect uvRect
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mUV;
		}
	}

	public void MarkAsDirty()
	{
		mIsValid = false;
	}

	public bool Validate(UIAtlas atlas)
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)atlas == (Object)null)
		{
			return false;
		}
		if (!mIsValid)
		{
			if (string.IsNullOrEmpty(spriteName))
			{
				return false;
			}
			mSprite = ((!((Object)(object)atlas != (Object)null)) ? null : atlas.GetSprite(spriteName));
			if (mSprite != null)
			{
				Texture texture = atlas.texture;
				if ((Object)(object)texture == (Object)null)
				{
					mSprite = null;
				}
				else
				{
					Rect rect = (mUV = mSprite.outer);
					if (atlas.coordinates == UIAtlas.Coordinates.Pixels)
					{
						mUV = NGUIMath.ConvertToTexCoords(mUV, texture.width, texture.height);
					}
					else
					{
						rect = NGUIMath.ConvertToPixels(rect, texture.width, texture.height, round: true);
					}
					mOffsetX = Mathf.RoundToInt(mSprite.paddingLeft * rect.width);
					mOffsetY = Mathf.RoundToInt(mSprite.paddingTop * rect.width);
					mWidth = Mathf.RoundToInt(rect.width);
					mHeight = Mathf.RoundToInt(rect.height);
					mAdvance = Mathf.RoundToInt(rect.width + (mSprite.paddingRight + mSprite.paddingLeft) * rect.width);
					mIsValid = true;
				}
			}
		}
		return mSprite != null;
	}
}
