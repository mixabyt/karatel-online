using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

[Obfuscation(Exclude = true, ApplyToMembers = true)]
public class VkontakteAPI : MonoBehaviour
{
	public struct RequestParameter(string name, string value)
	{
		public string Name = name;

		public string Value = value;

		public override string ToString()
		{
			return Name + "=" + Value;
		}
	}

	public static VkontakteAPI INSTANCE;

	public string VK_API_SECRET = string.Empty;

	public string OK_API_SECRET = string.Empty;

	public bool TestMode = true;

	public string Version = "2.0";

	[NonSerialized]
	public int _mapID;

	[NonSerialized]
	public string _gameID = string.Empty;

	public string _roomId = string.Empty;

	private string _apiUrl = string.Empty;

	private string _GameUrl = string.Empty;

	public string _viewerId = string.Empty;

	public string _authKey = string.Empty;

	[NonSerialized]
	public string _sessionKey = string.Empty;

	private string _appId = string.Empty;

	private bool _isInit;

	public bool IsInit => _isInit;

	private void Awake()
	{
		Object.DontDestroyOnLoad((Object)(object)((Component)this).gameObject);
		if ((Object)(object)INSTANCE == (Object)null)
		{
			INSTANCE = this;
		}
		else
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
	}

	private void Start()
	{
	}

	public string GetSecret()
	{
		return Crypto.DHJKVS("EAAAABYXupiVGpvNWUt0v1V1Q2iD+tP2HYFVYKVlxqP3nrijFud0DcHZEEUQLhPbBGVvxQ==", "5mXfvdih");
	}

	public void SetApiUrl(string url)
	{
		_apiUrl = url + ((!url.EndsWith("?")) ? "?" : string.Empty);
	}

	public void SetUrl(string url)
	{
		_GameUrl = url;
		if (App.Instance.CurPlatform == App.Platform.ONDOKLASNIKI || App.Instance.CurPlatform == App.Platform.MOYMIR)
		{
			_mapID = GetUrlInt(url, "map_id_", "&");
			_gameID = GetUrlString(url, "game_id_", "&");
		}
		else
		{
			_mapID = GetUrlInt(url, "map_id_", string.Empty);
			_gameID = GetUrlString(url, "game_id_", string.Empty);
		}
	}

	private string GetUrlString(string url, string param, string end = "")
	{
		int num = url.IndexOf(param);
		if (num != -1)
		{
			if (end == string.Empty)
			{
				return url.Substring(num + param.Length);
			}
			int num2 = url.IndexOf(end, num);
			return url.Substring(num + param.Length, num2 - (num + param.Length));
		}
		return string.Empty;
	}

	private int GetUrlInt(string url, string param, string end = "")
	{
		int result = 0;
		if (int.TryParse(GetUrlString(url, param, end), out result))
		{
			return result;
		}
		return 0;
	}

	public void SetAppId(string appId)
	{
		_appId = appId;
	}

	public void SetSessionKey(string key)
	{
		_sessionKey = key;
	}

	public void SetAuthKey(string key)
	{
		_authKey = key;
	}

	public void SetViewerId(string viewerId)
	{
		_viewerId = viewerId;
	}

	public void SetSocial(string social)
	{
		switch (social)
		{
		case "VK":
			App.Instance.CurPlatform = App.Platform.VKONTATE;
			break;
		case "OK":
			App.Instance.CurPlatform = App.Platform.ONDOKLASNIKI;
			break;
		case "MM":
			App.Instance.CurPlatform = App.Platform.MOYMIR;
			break;
		case "FB":
			App.Instance.CurPlatform = App.Platform.FACEBOOK;
			break;
		}
	}

	public void SetLocalize(string lang)
	{
		if (lang == "RU")
		{
			Localize.Locale = Localize.LocaleType.RU;
			KGUI.SetLocale(Localize.LocaleType.RU);
			KGUI.EnableNodes("welcome.language_ru", Localize.Locale == Localize.LocaleType.RU);
			KGUI.EnableNodes("welcome.language_en", Localize.Locale == Localize.LocaleType.EN);
		}
		if (lang == "EN")
		{
			Localize.Locale = Localize.LocaleType.EN;
			KGUI.SetLocale(Localize.LocaleType.EN);
			KGUI.EnableNodes("welcome.language_ru", Localize.Locale == Localize.LocaleType.RU);
			KGUI.EnableNodes("welcome.language_en", Localize.Locale == Localize.LocaleType.EN);
		}
	}

	public void PaymentFB(string data)
	{
		Debug.Log((object)"PaymentFB");
		if (data.Length >= 0)
		{
			ServerPaymentFB(data);
		}
	}

	public void ServerPaymentFB(string data)
	{
		Debug.Log((object)"ServerPaymentFB");
		Debug.Log((object)"StartReloadingProfile");
		((MonoBehaviour)this).StartCoroutine(App.Instance.LoadProfile2(from_loading: false));
	}

	public void SetSSL(string ssl)
	{
		Debug.Log((object)"SetSSL");
		Debug.Log((object)ssl);
		if (ssl == "1")
		{
			App.Instance.IsSSL = true;
		}
		else
		{
			App.Instance.IsSSL = false;
		}
	}

	public void ApiInitialized(int a)
	{
		Debug.Log((object)"ApiInitialized");
		_isInit = true;
		if (App.Instance.CurPlatform == App.Platform.MOYMIR)
		{
			MainMenu.Instance.SetLoadingText("LOADING_PROFILE", string.Empty);
			((MonoBehaviour)this).StartCoroutine(App.Instance.LoadProfile());
		}
		else if (App.Instance.CurPlatform == App.Platform.VKONTATE)
		{
			if (MD52(_appId + '_' + _viewerId + '_' + VK_API_SECRET) == _authKey)
			{
				MainMenu.Instance.SetLoadingText("LOADING_PROFILE", string.Empty);
				((MonoBehaviour)this).StartCoroutine(App.Instance.LoadProfile());
			}
			else
			{
				MainMenu.Instance.SetLoadingText("LOADING_SECURITY_ERROR", string.Empty);
			}
		}
		else if (App.Instance.CurPlatform == App.Platform.ONDOKLASNIKI)
		{
			if (MD52(_viewerId + _sessionKey + OK_API_SECRET) == _authKey)
			{
				MainMenu.Instance.SetLoadingText("LOADING_PROFILE", string.Empty);
				((MonoBehaviour)this).StartCoroutine(App.Instance.LoadProfile());
			}
			else
			{
				MainMenu.Instance.SetLoadingText("LOADING_SECURITY_ERROR", string.Empty);
			}
		}
		else if (App.Instance.CurPlatform == App.Platform.WEBSITE)
		{
			MainMenu.Instance.SwitchMenu(Menu.Welcome);
		}
		else if (App.Instance.CurPlatform == App.Platform.FACEBOOK)
		{
			MainMenu.Instance.SetLoadingText("LOADING_PROFILE", string.Empty);
			((MonoBehaviour)this).StartCoroutine(App.Instance.LoadProfile());
		}
	}

	public static string Md5(string value)
	{
		MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
		byte[] bytes = Encoding.ASCII.GetBytes(value);
		bytes = mD5CryptoServiceProvider.ComputeHash(bytes);
		string text = string.Empty;
		for (int i = 0; i < bytes.Length; i++)
		{
			text += bytes[i].ToString("x2").ToLower();
		}
		return text;
	}

	public static string MD52(string password)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(password);
		try
		{
			MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
			byte[] array = mD5CryptoServiceProvider.ComputeHash(bytes);
			string text = string.Empty;
			byte[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				byte b = array2[i];
				text = ((b >= 16) ? (text + b.ToString("x")) : (text + "0" + b.ToString("x")));
			}
			return text;
		}
		catch
		{
			throw;
		}
	}

	public string CreateRequestUrl(RequestParameter[] parameters)
	{
		List<RequestParameter> list = new List<RequestParameter>();
		list.Add(new RequestParameter("api_id", _appId));
		list.Add(new RequestParameter("v", Version));
		if (TestMode)
		{
			list.Add(new RequestParameter("test_mode", "1"));
		}
		list.AddRange(parameters);
		List<string> list2 = new List<string>();
		foreach (RequestParameter item in list)
		{
			list2.Add(item.Name);
		}
		list2.Sort();
		List<RequestParameter> list3 = new List<RequestParameter>();
		foreach (string item2 in list2)
		{
			foreach (RequestParameter item3 in list)
			{
				if (item3.Name == item2)
				{
					list3.Add(item3);
					break;
				}
			}
		}
		list = list3;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(_viewerId);
		foreach (RequestParameter item4 in list)
		{
			stringBuilder.Append(item4.ToString());
		}
		stringBuilder.Append(GetSecret());
		string value = Md5(stringBuilder.ToString());
		stringBuilder = new StringBuilder();
		stringBuilder.Append(_apiUrl);
		foreach (RequestParameter item5 in list)
		{
			stringBuilder.Append(item5.ToString());
			stringBuilder.Append("&");
		}
		stringBuilder.Append("sig=");
		stringBuilder.Append(value);
		return stringBuilder.ToString();
	}
}
