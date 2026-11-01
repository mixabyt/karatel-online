using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileINI
{
	public static string nickname;

	public static string email;

	public static string server_name;

	public static string server_about;

	private static SecuredValue<int> m_skin;

	public static float mouse_sens;

	public static float sound_volume;

	public static float sound_scale = 1f;

	public static float ambient_volume;

	public static int tutorial_watch;

	public static int options_destroy;

	public static int crystals;

	public static int camera_type;

	public static int coins;

	public static bool newgamersislook;

	public static bool was_banned = false;

	public static bool server_was_closed = false;

	public static DateTime server_time;

	public static bool showBaloons = true;

	public static bool showSelfBaloons = true;

	public static Dictionary<StorePurchase, SecuredValue<int>> purchesList = new Dictionary<StorePurchase, SecuredValue<int>>();

	public static Dictionary<StorePurchase, DateTime> purchesTime = new Dictionary<StorePurchase, DateTime>();

	public static SecuredValue<int> skin
	{
		get
		{
			return m_skin;
		}
		set
		{
			m_skin = value;
		}
	}

	public static void SaveLoginInfo()
	{
		PlayerPrefs.SetString("welcome_email", MainMenu.INSTANCE.welcome_email);
		PlayerPrefs.SetString("welcome_password", MainMenu.INSTANCE.welcome_password);
		PlayerPrefs.SetInt("welcome_remember", MainMenu.INSTANCE.welcome_remember ? 1 : 0);
	}

	public static void ClearLoginInfo()
	{
		PlayerPrefs.SetString("welcome_email", string.Empty);
		PlayerPrefs.SetString("welcome_password", string.Empty);
		PlayerPrefs.SetInt("welcome_remember", MainMenu.INSTANCE.welcome_remember ? 1 : 0);
	}

	public static void LoadLoginInfo()
	{
		MainMenu.INSTANCE.welcome_email = string.Empty;
		MainMenu.INSTANCE.welcome_password = string.Empty;
		MainMenu.INSTANCE.welcome_remember = false;
	}

	public static void Save()
	{
		PlayerPrefs.SetString("last_server_name", server_name);
		PlayerPrefs.SetString("last_server_about", server_about);
		Utils.PlayerPrefsSetBool("new_gamers_is_look", newgamersislook);
		PlayerPrefs.SetInt("camera_type", camera_type);
		PlayerPrefs.SetFloat("mouse_sens", mouse_sens);
		PlayerPrefs.SetInt("tutorial_watch", tutorial_watch);
		PlayerPrefs.SetInt("tutorial_watch", tutorial_watch);
		PlayerPrefs.SetFloat("sound_volume", sound_volume);
		PlayerPrefs.SetFloat("ambient_volume", ambient_volume);
		PlayerPrefs.SetInt("showBaloons", showBaloons ? 1 : 0);
		PlayerPrefs.SetInt("showSelfBaloons", showSelfBaloons ? 1 : 0);
	}

	public static IEnumerator SaveDestroy()
	{
		WWWForm form = new WWWForm();
		form.AddField("id", VkontakteAPI.INSTANCE._viewerId);
		form.AddField("auth_key", VkontakteAPI.INSTANCE._authKey);
		if (App.instance.platform == App.ePlatform.ONDOKLASNIKI)
		{
			form.AddField("session_key", VkontakteAPI.INSTANCE._sessionKey);
		}
		form.AddField("ind", options_destroy);
		Random r = new Random();
		form.AddField("random", r.Next());
		yield return (object)new WWW(App.instance.GetCurScriptsUrl() + "set_destroy.php", form);
	}

	public static void LoadFromPrefs()
	{
		if (PlayerPrefs.HasKey("sound_volume"))
		{
			mouse_sens = PlayerPrefs.GetFloat("mouse_sens");
		}
		else
		{
			mouse_sens = 4f;
		}
		tutorial_watch = PlayerPrefs.GetInt("tutorial_watch");
		if (PlayerPrefs.HasKey("sound_volume"))
		{
			sound_volume = PlayerPrefs.GetFloat("sound_volume");
		}
		else
		{
			sound_volume = 0.7f;
		}
		if (PlayerPrefs.HasKey("ambient_volume"))
		{
			ambient_volume = PlayerPrefs.GetFloat("ambient_volume");
		}
		else
		{
			ambient_volume = 0.7f;
		}
		server_name = PlayerPrefs.GetString("last_server_name");
		server_about = PlayerPrefs.GetString("last_server_about");
		camera_type = PlayerPrefs.GetInt("camera_type");
		newgamersislook = Utils.PlayerPrefsGetBool("new_gamers_is_look", defaultValue: false);
		showBaloons = PlayerPrefs.GetInt("showBaloons", 1) != 0;
		showSelfBaloons = PlayerPrefs.GetInt("showSelfBaloons", 1) != 0;
		if (mouse_sens == 0f)
		{
			mouse_sens = 4f;
		}
		SetPurchesValue(StorePurchase.STANDART_SKIN, 1);
		SetPurchesTime(StorePurchase.STANDART_SKIN, new DateTime(2200, 1, 1));
	}

	public static void Reset()
	{
		mouse_sens = 4f;
		nickname = string.Empty;
		tutorial_watch = 0;
		crystals = 5;
		coins = 300;
		options_destroy = 1;
		sound_volume = 0.7f;
		ambient_volume = 0.7f;
		camera_type = 1;
		showBaloons = true;
		showSelfBaloons = true;
		MainMenu.INSTANCE.options_destroy.select_id = options_destroy;
		SetPurchesValue(StorePurchase.STANDART_SKIN, 1);
		SetPurchesTime(StorePurchase.STANDART_SKIN, new DateTime(2200, 1, 1));
	}

	public static int GetPurchesValue(StorePurchase purches)
	{
		purchesList.TryGetValue(purches, out var value);
		if (value == null)
		{
			return 0;
		}
		return value;
	}

	public static bool CheckPurchesValue(StorePurchase purches)
	{
		purchesList.TryGetValue(purches, out var value);
		return value?.IsSecured() ?? true;
	}

	public static IEnumerator SetSkin(int ind)
	{
		skin = ind;
		MainMenu.INSTANCE.preview_obj.GetComponent<SkinManager>().SetSkin(skin);
		int packet_id = App.instance.random.Next();
		WWWForm form = new WWWForm();
		form.AddField("random", packet_id);
		form.AddField("skin", (int)skin);
		form.AddField("id", VkontakteAPI.INSTANCE._viewerId);
		form.AddField("auth_key", VkontakteAPI.INSTANCE._authKey);
		if (App.instance.platform == App.ePlatform.ONDOKLASNIKI)
		{
			form.AddField("session_key", VkontakteAPI.INSTANCE._sessionKey);
		}
		yield return (object)new WWW(App.instance.GetCurScriptsUrl() + "set_skin.php", form);
	}

	public static DateTime GetPurchesTime(StorePurchase purches)
	{
		purchesTime.TryGetValue(purches, out var value);
		return value;
	}

	public static void SetPurchesValue(StorePurchase purches, int value)
	{
		purchesList[purches] = value;
	}

	public static void SetPurchesTime(StorePurchase purches, DateTime time)
	{
		purchesTime[purches] = time;
	}

	public static void IncPurchesValue(StorePurchase purches, int count)
	{
		int purchesValue = GetPurchesValue(purches);
		SetPurchesValue(purches, purchesValue + count);
	}

	public static void DecPurchesValue(StorePurchase purches, int count)
	{
		int purchesValue = GetPurchesValue(purches);
		SetPurchesValue(purches, purchesValue - count);
	}
}
