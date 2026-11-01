using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Internal/Localization")]
public class Localization : MonoBehaviour
{
	private static Localization mInstance;

	public string startingLanguage = "English";

	public TextAsset[] languages;

	private Dictionary<string, string> mDictionary = new Dictionary<string, string>();

	private string mLanguage;

	public static bool isActive => (Object)(object)mInstance != (Object)null;

	public static Localization instance
	{
		get
		{
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Expected Obj, but got Unknown
			if ((Object)(object)mInstance == (Object)null)
			{
				mInstance = Object.FindObjectOfType(typeof(Localization)) as Localization;
				if ((Object)(object)mInstance == (Object)null)
				{
					GameObject val = new GameObject("_Localization");
					Object.DontDestroyOnLoad((Object)(object)val);
					mInstance = val.AddComponent<Localization>();
				}
			}
			return mInstance;
		}
	}

	public string currentLanguage
	{
		get
		{
			return mLanguage;
		}
		set
		{
			if (!(mLanguage != value))
			{
				return;
			}
			startingLanguage = value;
			if (!string.IsNullOrEmpty(value))
			{
				if (languages != null)
				{
					int i = 0;
					for (int num = languages.Length; i < num; i++)
					{
						TextAsset val = languages[i];
						if ((Object)(object)val != (Object)null && ((Object)val).name == value)
						{
							Load(val);
							return;
						}
					}
				}
				Object val2 = Resources.Load(value, typeof(TextAsset));
				TextAsset val3 = (TextAsset)(object)((val2 is TextAsset) ? val2 : null);
				if ((Object)(object)val3 != (Object)null)
				{
					Load(val3);
					return;
				}
			}
			mDictionary.Clear();
			PlayerPrefs.DeleteKey("Language");
		}
	}

	private void Awake()
	{
		if ((Object)(object)mInstance == (Object)null)
		{
			mInstance = this;
			Object.DontDestroyOnLoad((Object)(object)((Component)this).gameObject);
			currentLanguage = PlayerPrefs.GetString("Language", startingLanguage);
			if (string.IsNullOrEmpty(mLanguage) && languages != null && languages.Length > 0)
			{
				currentLanguage = ((Object)languages[0]).name;
			}
		}
		else
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
	}

	private void OnEnable()
	{
		if ((Object)(object)mInstance == (Object)null)
		{
			mInstance = this;
		}
	}

	private void OnDestroy()
	{
		if ((Object)(object)mInstance == (Object)(object)this)
		{
			mInstance = null;
		}
	}

	private void Load(TextAsset asset)
	{
		mLanguage = ((Object)asset).name;
		PlayerPrefs.SetString("Language", mLanguage);
		ByteReader byteReader = new ByteReader(asset);
		mDictionary = byteReader.ReadDictionary();
		UIRoot.Broadcast("OnLocalize", this);
	}

	public string Get(string key)
	{
		string value;
		return (!mDictionary.TryGetValue(key, out value)) ? key : value;
	}

	public static string Localize(string key)
	{
		return (!((Object)(object)instance != (Object)null)) ? key : instance.Get(key);
	}
}
