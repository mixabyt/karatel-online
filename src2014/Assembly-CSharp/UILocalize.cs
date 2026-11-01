using UnityEngine;

[RequireComponent(typeof(UIWidget))]
[AddComponentMenu("NGUI/UI/Localize")]
public class UILocalize : MonoBehaviour
{
	public string key;

	private string mLanguage;

	private bool mStarted;

	private void OnLocalize(Localization loc)
	{
		if (mLanguage != loc.currentLanguage)
		{
			Localize();
		}
	}

	private void OnEnable()
	{
		if (mStarted && (Object)(object)Localization.instance != (Object)null)
		{
			Localize();
		}
	}

	private void Start()
	{
		mStarted = true;
		if ((Object)(object)Localization.instance != (Object)null)
		{
			Localize();
		}
	}

	public void Localize()
	{
		Localization instance = Localization.instance;
		UIWidget component = ((Component)this).GetComponent<UIWidget>();
		UILabel uILabel = component as UILabel;
		UISprite uISprite = component as UISprite;
		if (string.IsNullOrEmpty(mLanguage) && string.IsNullOrEmpty(key) && (Object)(object)uILabel != (Object)null)
		{
			key = uILabel.text;
		}
		string text = ((!string.IsNullOrEmpty(key)) ? instance.Get(key) : string.Empty);
		if ((Object)(object)uILabel != (Object)null)
		{
			UIInput uIInput = NGUITools.FindInParents<UIInput>(((Component)uILabel).gameObject);
			if ((Object)(object)uIInput != (Object)null && (Object)(object)uIInput.label == (Object)(object)uILabel)
			{
				uIInput.defaultText = text;
			}
			else
			{
				uILabel.text = text;
			}
		}
		else if ((Object)(object)uISprite != (Object)null)
		{
			uISprite.spriteName = text;
			uISprite.MakePixelPerfect();
		}
		mLanguage = instance.currentLanguage;
	}
}
