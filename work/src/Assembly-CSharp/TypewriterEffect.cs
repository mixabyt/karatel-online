using UnityEngine;

[RequireComponent(typeof(UILabel))]
[AddComponentMenu("NGUI/Examples/Typewriter Effect")]
public class TypewriterEffect : MonoBehaviour
{
	public int charsPerSecond = 40;

	private UILabel mLabel;

	private string mText;

	private int mOffset;

	private float mNextChar;

	private void Update()
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)mLabel == (Object)null)
		{
			mLabel = ((Component)this).GetComponent<UILabel>();
			mLabel.supportEncoding = false;
			mLabel.symbolStyle = UIFont.SymbolStyle.None;
			mText = mLabel.font.WrapText(mLabel.text, (float)mLabel.lineWidth / mLabel.cachedTransform.localScale.x, mLabel.maxLineCount, encoding: false, UIFont.SymbolStyle.None);
		}
		if (mOffset < mText.Length)
		{
			if (mNextChar <= Time.time)
			{
				charsPerSecond = Mathf.Max(1, charsPerSecond);
				float num = 1f / (float)charsPerSecond;
				char c = mText[mOffset];
				if (c == '.' || c == '\n' || c == '!' || c == '?')
				{
					num *= 4f;
				}
				mNextChar = Time.time + num;
				mLabel.text = mText.Substring(0, ++mOffset);
			}
		}
		else
		{
			Object.Destroy((Object)(object)this);
		}
	}
}
