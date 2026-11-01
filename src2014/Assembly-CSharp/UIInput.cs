using UnityEngine;

[AddComponentMenu("NGUI/UI/Input (Basic)")]
public class UIInput : MonoBehaviour
{
	public enum KeyboardType
	{
		Default,
		ASCIICapable,
		NumbersAndPunctuation,
		URL,
		NumberPad,
		PhonePad,
		NamePhonePad,
		EmailAddress
	}

	public delegate char Validator(string currentText, char nextChar);

	public delegate void OnSubmit(string inputString);

	public static UIInput current;

	public UILabel label;

	public int maxChars;

	public string caratChar = "|";

	public string firstChar = string.Empty;

	public Validator validator;

	public KeyboardType type;

	public bool isPassword;

	public bool autoCorrect;

	public bool useLabelTextAtStart;

	public Color activeColor = Color.white;

	public GameObject selectOnTab;

	public GameObject eventReceiver;

	public string functionName = "OnSubmit";

	public OnSubmit onSubmit;

	private string mText = string.Empty;

	private string mDefaultText = string.Empty;

	private Color mDefaultColor = Color.white;

	private UIWidget.Pivot mPivot = UIWidget.Pivot.Left;

	private float mPosition;

	private string mLastIME = string.Empty;

	private bool mDoInit = true;

	public virtual string text
	{
		get
		{
			if (mDoInit)
			{
				Init();
			}
			return mText;
		}
		set
		{
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			if (mDoInit)
			{
				Init();
			}
			mText = value;
			if ((Object)(object)label != (Object)null)
			{
				if (string.IsNullOrEmpty(value))
				{
					value = mDefaultText;
				}
				label.supportEncoding = false;
				label.text = ((!selected) ? value : (firstChar + value + caratChar));
				label.showLastPasswordChar = selected;
				label.color = ((!selected && !(value != mDefaultText)) ? mDefaultColor : activeColor);
			}
		}
	}

	public bool selected
	{
		get
		{
			return (Object)(object)UICamera.selectedObject == (Object)(object)((Component)this).gameObject;
		}
		set
		{
			if (!value && (Object)(object)UICamera.selectedObject == (Object)(object)((Component)this).gameObject)
			{
				UICamera.selectedObject = null;
			}
			else if (value)
			{
				UICamera.selectedObject = ((Component)this).gameObject;
			}
		}
	}

	public string defaultText
	{
		get
		{
			return mDefaultText;
		}
		set
		{
			if (label.text == mDefaultText)
			{
				label.text = value;
			}
			mDefaultText = value;
		}
	}

	public UIInput()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
	}

	protected void Init()
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		if (!mDoInit)
		{
			return;
		}
		mDoInit = false;
		if ((Object)(object)label == (Object)null)
		{
			label = ((Component)this).GetComponentInChildren<UILabel>();
		}
		if ((Object)(object)label != (Object)null)
		{
			if (useLabelTextAtStart)
			{
				mText = label.text;
			}
			mDefaultText = label.text;
			mDefaultColor = label.color;
			label.supportEncoding = false;
			label.password = isPassword;
			mPivot = label.pivot;
			mPosition = label.cachedTransform.localPosition.x;
		}
		else
		{
			((Behaviour)this).enabled = false;
		}
	}

	private void OnEnable()
	{
		if (UICamera.IsHighlighted(((Component)this).gameObject))
		{
			OnSelect(isSelected: true);
		}
	}

	private void OnDisable()
	{
		if (UICamera.IsHighlighted(((Component)this).gameObject))
		{
			OnSelect(isSelected: false);
		}
	}

	private void OnSelect(bool isSelected)
	{
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		if (mDoInit)
		{
			Init();
		}
		if (!((Object)(object)label != (Object)null) || !((Behaviour)this).enabled || !NGUITools.GetActive(((Component)this).gameObject))
		{
			return;
		}
		if (isSelected)
		{
			mText = ((useLabelTextAtStart || !(label.text == mDefaultText)) ? label.text : string.Empty);
			label.color = activeColor;
			if (isPassword)
			{
				label.password = true;
			}
			Input.imeCompositionMode = (IMECompositionMode)1;
			Transform cachedTransform = label.cachedTransform;
			Vector3 val = Vector2.op_Implicit(label.pivotOffset);
			val.y += label.relativeSize.y;
			val = cachedTransform.TransformPoint(val);
			Input.compositionCursorPos = Vector2.op_Implicit(UICamera.currentCamera.WorldToScreenPoint(val));
			UpdateLabel();
			return;
		}
		if (string.IsNullOrEmpty(mText))
		{
			label.text = mDefaultText;
			label.color = mDefaultColor;
			if (isPassword)
			{
				label.password = false;
			}
		}
		else
		{
			label.text = mText;
		}
		label.showLastPasswordChar = false;
		Input.imeCompositionMode = (IMECompositionMode)2;
		RestoreLabel();
	}

	private void Update()
	{
		if (selected)
		{
			if ((Object)(object)selectOnTab != (Object)null && Input.GetKeyDown((KeyCode)9))
			{
				UICamera.selectedObject = selectOnTab;
			}
			if (Input.GetKeyDown((KeyCode)118) && (Input.GetKey((KeyCode)306) || Input.GetKey((KeyCode)305)))
			{
				Append(NGUITools.clipboard);
			}
			if (Application.isEditor && Input.GetKeyDown((KeyCode)277))
			{
				Append(NGUITools.clipboard);
			}
			if (mLastIME != Input.compositionString)
			{
				mLastIME = Input.compositionString;
				UpdateLabel();
			}
		}
	}

	private void OnInput(string input)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Invalid comparison between Unknown and I4
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Invalid comparison between Unknown and I4
		if (mDoInit)
		{
			Init();
		}
		if (selected && ((Behaviour)this).enabled && NGUITools.GetActive(((Component)this).gameObject) && (int)Application.platform != 11 && (int)Application.platform != 8)
		{
			Append(input);
		}
	}

	private void Append(string input)
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Invalid comparison between Unknown and I4
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Invalid comparison between Unknown and I4
		int i = 0;
		for (int length = input.Length; i < length; i++)
		{
			char c = input[i];
			if (c == '\b')
			{
				if (mText.Length > 0)
				{
					mText = mText.Substring(0, mText.Length - 1);
					((Component)this).SendMessage("OnInputChanged", (object)this, (SendMessageOptions)1);
				}
			}
			else if (c == '\r' || c == '\n')
			{
				if (onSubmit != null && ((int)UICamera.current.submitKey0 == 13 || (int)UICamera.current.submitKey1 == 13) && (!label.multiLine || (!Input.GetKey((KeyCode)306) && !Input.GetKey((KeyCode)305))))
				{
					current = this;
					if (onSubmit != null)
					{
						onSubmit(mText);
					}
					if ((Object)(object)eventReceiver == (Object)null)
					{
						eventReceiver = ((Component)this).gameObject;
					}
					eventReceiver.SendMessage(functionName, (object)mText, (SendMessageOptions)1);
					current = null;
					selected = false;
					return;
				}
				if (validator != null)
				{
					c = validator(mText, c);
				}
				if (c == '\0')
				{
					continue;
				}
				if (c == '\n' || c == '\r')
				{
					if (label.multiLine)
					{
						mText += "\n";
					}
				}
				else
				{
					mText += c;
				}
				((Component)this).SendMessage("OnInputChanged", (object)this, (SendMessageOptions)1);
			}
			else if (c >= ' ')
			{
				if (validator != null)
				{
					c = validator(mText, c);
				}
				if (c != 0)
				{
					mText += c;
					((Component)this).SendMessage("OnInputChanged", (object)this, (SendMessageOptions)1);
				}
			}
		}
		UpdateLabel();
	}

	private void UpdateLabel()
	{
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		if (mDoInit)
		{
			Init();
		}
		if (maxChars > 0 && mText.Length > maxChars)
		{
			mText = mText.Substring(0, maxChars);
		}
		if (!((Object)(object)label.font != (Object)null))
		{
			return;
		}
		string text;
		if (isPassword && selected)
		{
			text = firstChar;
			int i = 0;
			for (int length = mText.Length; i < length; i++)
			{
				text += "*";
			}
			text = text + Input.compositionString + caratChar;
		}
		else
		{
			text = ((!selected) ? mText : (firstChar + mText + Input.compositionString + caratChar));
		}
		label.supportEncoding = false;
		if (!label.shrinkToFit)
		{
			if (label.multiLine)
			{
				text = label.font.WrapText(text, (float)label.lineWidth / label.cachedTransform.localScale.x, 0, encoding: false, UIFont.SymbolStyle.None);
			}
			else
			{
				string endOfLineThatFits = label.font.GetEndOfLineThatFits(text, (float)label.lineWidth / label.cachedTransform.localScale.x, encoding: false, UIFont.SymbolStyle.None);
				if (endOfLineThatFits != text)
				{
					text = endOfLineThatFits;
					Vector3 localPosition = label.cachedTransform.localPosition;
					localPosition.x = mPosition + (float)label.lineWidth;
					if (mPivot == UIWidget.Pivot.Left)
					{
						label.pivot = UIWidget.Pivot.Right;
					}
					else if (mPivot == UIWidget.Pivot.TopLeft)
					{
						label.pivot = UIWidget.Pivot.TopRight;
					}
					else if (mPivot == UIWidget.Pivot.BottomLeft)
					{
						label.pivot = UIWidget.Pivot.BottomRight;
					}
					label.cachedTransform.localPosition = localPosition;
				}
				else
				{
					RestoreLabel();
				}
			}
		}
		label.text = text;
		label.showLastPasswordChar = selected;
	}

	private void RestoreLabel()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)label != (Object)null)
		{
			label.pivot = mPivot;
			Vector3 localPosition = label.cachedTransform.localPosition;
			localPosition.x = mPosition;
			label.cachedTransform.localPosition = localPosition;
		}
	}
}
