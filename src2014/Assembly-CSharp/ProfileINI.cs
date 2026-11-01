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

	public static SecuredValue<int> skin;

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

	public static SecuredValue<int> experience;

	public static Dictionary<StorePurchase, SecuredValue<int>> purchasesList = new Dictionary<StorePurchase, SecuredValue<int>>();

	public static Dictionary<StorePurchase, DateTime> purchasesTime = new Dictionary<StorePurchase, DateTime>();

	public static int level => LevelExp(experience);

	public static int LevelExp(int exp)
	{
		return Mathf.Clamp(Mathf.FloorToInt(Mathf.Log((float)(exp / 25), 2f)) + 1, 1, 30);
	}

	public static int ExpForLevel(int level)
	{
		return (level > 1) ? Mathf.RoundToInt(Mathf.Pow(2f, (float)(level - 1)) * 25f) : 0;
	}

	public static int GetActualSkin()
	{
		return 0;
	}

	public static void SaveLoginInfo(string login, string password, bool remember)
	{
		PlayerPrefs.SetString("welcome_email", login);
		PlayerPrefs.SetString("welcome_password", password);
		PlayerPrefs.SetInt("welcome_remember", remember ? 1 : 0);
	}

	public static void ClearLoginInfo()
	{
		PlayerPrefs.SetString("welcome_email", string.Empty);
		PlayerPrefs.SetString("welcome_password", string.Empty);
		PlayerPrefs.SetInt("welcome_remember", 0);
	}

	public static void LoadLoginInfo(out string login, out string password, out bool remember)
	{
		login = PlayerPrefs.GetString("welcome_email");
		password = PlayerPrefs.GetString("welcome_password");
		remember = PlayerPrefs.GetInt("welcome_remember") == 1;
		if (bs._Igor.autoChangeUser && login != "test1")
		{
			login = (bs._Igor.secondUser ? "kriger.1999@mail.ru" : "blakstarvip@mail.ru");
			password = (bs._Igor.secondUser ? "cfif1999" : "a9x8bn66");
			SaveLoginInfo(login, password, remember);
		}
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
		if (App.Instance.CurPlatform == App.Platform.ONDOKLASNIKI)
		{
			form.AddField("session_key", VkontakteAPI.INSTANCE._sessionKey);
		}
		form.AddField("ind", options_destroy);
		Random r = new Random();
		form.AddField("random", r.Next());
		yield return (object)new WWW(App.Instance.GetCurScriptsUrl() + "set_destroy.php", form);
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
		SetPurchaseValue(StorePurchase.STANDART_SKIN, 1);
		SetPurchaseTime(StorePurchase.STANDART_SKIN, new DateTime(2200, 1, 1));
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
		SetPurchaseValue(StorePurchase.STANDART_SKIN, 1);
		SetPurchaseTime(StorePurchase.STANDART_SKIN, new DateTime(2200, 1, 1));
	}

	public static int GetPurchaseValue(StorePurchase purchase)
	{
		purchasesList.TryGetValue(purchase, out var value);
		if (value == null)
		{
			return 0;
		}
		return value;
	}

	public static bool CheckPurchaseValue(StorePurchase purchase)
	{
		purchasesList.TryGetValue(purchase, out var value);
		return value?.IsSecured() ?? true;
	}

	public static IEnumerator SetSkin(int ind)
	{
		skin = ind;
		int packet_id = App.Instance.Rnd.Next();
		WWWForm form = new WWWForm();
		form.AddField("random", packet_id);
		form.AddField("skin", (int)skin);
		form.AddField("id", VkontakteAPI.INSTANCE._viewerId);
		form.AddField("auth_key", VkontakteAPI.INSTANCE._authKey);
		if (App.Instance.CurPlatform == App.Platform.ONDOKLASNIKI)
		{
			form.AddField("session_key", VkontakteAPI.INSTANCE._sessionKey);
		}
		yield return (object)new WWW(App.Instance.GetCurScriptsUrl() + "set_skin.php", form);
	}

	public static DateTime GetPurchaseTime(StorePurchase purchase)
	{
		purchasesTime.TryGetValue(purchase, out var value);
		return value;
	}

	public static void SetPurchaseValue(StorePurchase purchase, int value)
	{
		purchasesList[purchase] = value;
	}

	public static void SetPurchaseTime(StorePurchase purchase, DateTime time)
	{
		purchasesTime[purchase] = time;
	}
}
