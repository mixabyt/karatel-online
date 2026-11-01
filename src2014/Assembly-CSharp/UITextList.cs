using System.Collections.Generic;
using System.Text;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Text List")]
public class UITextList : MonoBehaviour
{
	public enum Style
	{
		Text,
		Chat
	}

	protected class Paragraph
	{
		public string text;

		public string[] lines;
	}

	public Style style;

	public UILabel textLabel;

	public float maxWidth;

	public float maxHeight;

	public int maxEntries = 50;

	public bool supportScrollWheel = true;

	protected char[] mSeparator = new char[1] { '\n' };

	protected List<Paragraph> mParagraphs = new List<Paragraph>();

	protected float mScroll;

	protected bool mSelected;

	protected int mTotalLines;

	public void Clear()
	{
		mParagraphs.Clear();
		UpdateVisibleText();
	}

	public void Add(string text)
	{
		Add(text, updateVisible: true);
	}

	protected void Add(string text, bool updateVisible)
	{
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		Paragraph paragraph = null;
		if (mParagraphs.Count < maxEntries)
		{
			paragraph = new Paragraph();
		}
		else
		{
			paragraph = mParagraphs[0];
			mParagraphs.RemoveAt(0);
		}
		paragraph.text = text;
		mParagraphs.Add(paragraph);
		if ((Object)(object)textLabel != (Object)null && (Object)(object)textLabel.font != (Object)null)
		{
			paragraph.lines = textLabel.font.WrapText(paragraph.text, maxWidth / ((Component)textLabel).transform.localScale.y, textLabel.maxLineCount, textLabel.supportEncoding, textLabel.symbolStyle).Split(mSeparator);
			mTotalLines = 0;
			int i = 0;
			for (int count = mParagraphs.Count; i < count; i++)
			{
				mTotalLines += mParagraphs[i].lines.Length;
			}
		}
		if (updateVisible)
		{
			UpdateVisibleText();
		}
	}

	private void Awake()
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)textLabel == (Object)null)
		{
			textLabel = ((Component)this).GetComponentInChildren<UILabel>();
		}
		if ((Object)(object)textLabel != (Object)null)
		{
			textLabel.lineWidth = 0;
		}
		Collider collider = ((Component)this).collider;
		if ((Object)(object)collider != (Object)null)
		{
			if (maxHeight <= 0f)
			{
				Bounds bounds = collider.bounds;
				maxHeight = bounds.size.y / ((Component)this).transform.lossyScale.y;
			}
			if (maxWidth <= 0f)
			{
				Bounds bounds2 = collider.bounds;
				maxWidth = bounds2.size.x / ((Component)this).transform.lossyScale.x;
			}
		}
	}

	private void OnSelect(bool selected)
	{
		mSelected = selected;
	}

	protected void UpdateVisibleText()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)textLabel != (Object)null))
		{
			return;
		}
		UIFont font = textLabel.font;
		if (!((Object)(object)font != (Object)null))
		{
			return;
		}
		int num = 0;
		int num2 = ((!(maxHeight > 0f)) ? 100000 : Mathf.FloorToInt(maxHeight / textLabel.cachedTransform.localScale.y));
		int num3 = Mathf.RoundToInt(mScroll);
		if (num2 + num3 > mTotalLines)
		{
			num3 = Mathf.Max(0, mTotalLines - num2);
			mScroll = num3;
		}
		if (style == Style.Chat)
		{
			num3 = Mathf.Max(0, mTotalLines - num2 - num3);
		}
		StringBuilder stringBuilder = new StringBuilder();
		int i = 0;
		for (int count = mParagraphs.Count; i < count; i++)
		{
			Paragraph paragraph = mParagraphs[i];
			int j = 0;
			for (int num4 = paragraph.lines.Length; j < num4; j++)
			{
				string value = paragraph.lines[j];
				if (num3 > 0)
				{
					num3--;
					continue;
				}
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append("\n");
				}
				stringBuilder.Append(value);
				num++;
				if (num >= num2)
				{
					break;
				}
			}
			if (num >= num2)
			{
				break;
			}
		}
		textLabel.text = stringBuilder.ToString();
	}

	private void OnScroll(float val)
	{
		if (mSelected && supportScrollWheel)
		{
			val *= ((style != Style.Chat) ? (-10f) : 10f);
			mScroll = Mathf.Max(0f, mScroll + val);
			UpdateVisibleText();
		}
	}
}
