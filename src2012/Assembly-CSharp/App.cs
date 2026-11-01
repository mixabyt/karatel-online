using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Photon;
using UnityEngine;

[Obfuscation(Exclude = true, ApplyToMembers = true)]
public class App : MonoBehaviour
{
	public enum ePlatform
	{
		VKONTATE,
		WEBSITE,
		ONDOKLASNIKI,
		MOYMIR
	}

	public static App instance;

	public GameINI gameIni;

	public Random random = new Random();

	public ePlatform platform;

	public bool web_test;

	public string[] server_script_urls;

	public string[] server_saves_urls;

	public string[] server_app_ids;

	public int[] shard_count;

	public int[] realy_shard_used;

	public int max_shard_population;

	public string game_verison;

	private int cur_shard_test = 1;

	[NonSerialized]
	public bool go_to_menu_on_conected;

	private bool connect_to_random_shard;

	public int curent_shard_ind;

	private float waiting_shard_population_time;

	private bool is_waiting_shard_population;

	private float last_realtime;

	private DateTime last_datetime;

	[NonSerialized]
	public bool waiting_room_recive;

	private DateTime olddt;

	public string GetCurScriptsUrl()
	{
		return "https://kopatel-online.ru/2012/api/";
	}

	public string GetCurSavesUrl()
	{
		return "https://kopatel-online.ru/2012/api/";
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad((Object)(object)((Component)this).gameObject);
		if ((Object)(object)instance == (Object)null)
		{
			instance = this;
			return;
		}
		if (ProfileINI.was_banned)
		{
			MainMenu.INSTANCE.curWindow = MainMenu.Windows.BanMenu;
			ProfileINI.was_banned = false;
		}
		else if (ProfileINI.server_was_closed)
		{
			MainMenu.INSTANCE.curWindow = MainMenu.Windows.ServerWasClosed;
			ProfileINI.server_was_closed = false;
		}
		else
		{
			MainMenu.INSTANCE.curWindow = MainMenu.Windows.Battles;
		}
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}

	public string GenerateKey(int length)
	{
		Random random = new Random();
		char[] array = new char[62]
		{
			'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J',
			'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T',
			'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd',
			'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n',
			'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x',
			'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7',
			'8', '9'
		};
		string text = string.Empty;
		for (int i = 0; i < length; i++)
		{
			text += array[random.Next(0, 35)];
		}
		return text;
	}

	public string KeyEncrypt(string message)
	{
		try
		{
			string xmlString = "<RSAKeyValue><Modulus>t3A/x4DoMsQNVdIWe0rYmBy98tIQz+2/M5eiqL7IxS2h7L3mHLZVOHeTzDWbBGLHIx8la1oWUXPGUr/31Mgbvuc8PDXW0Oh011LWUYYPgaFLKqwvnS7T8ehNYLNkHxEeiKrtMPHSp0n6IfChKAvUZbdPs9IjAr1kO2lVNJ5Ickc=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
			RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
			rSACryptoServiceProvider.FromXmlString(xmlString);
			return Convert.ToBase64String(rSACryptoServiceProvider.Encrypt(Encoding.UTF8.GetBytes(message), fOAEP: true));
		}
		catch (Exception ex)
		{
			Debug.Log((object)("Problem with Encrypt:\n" + ex.Message.ToString()));
			return string.Empty;
		}
	}

	private void Start()
	{
		MainMenu.INSTANCE.curWindow = MainMenu.Windows.WelcomeScreen;
		ProfileINI.LoadLoginInfo();
	}

	public void OpenChooseShardMenu()
	{
		ConnectToRandomServer();
	}

	public void ConnectToRandomServer()
	{
		MainMenu.INSTANCE.curWindow = (MainMenu.Windows)29;
	}

	public void WaitingShardPopulation()
	{
		if (cur_shard_test <= 0)
		{
			return;
		}
		waiting_shard_population_time += Time.deltaTime;
		if (waiting_shard_population_time < 5f)
		{
			if (PhotonNetwork.countOfRooms > 0)
			{
				shard_count[cur_shard_test - 1] = PhotonNetwork.countOfRooms;
				PhotonNetwork.Disconnect();
				cur_shard_test++;
				is_waiting_shard_population = false;
			}
		}
		else
		{
			shard_count[cur_shard_test - 1] = PhotonNetwork.countOfRooms;
			PhotonNetwork.Disconnect();
			cur_shard_test++;
			is_waiting_shard_population = false;
		}
	}

	public void OnCheatDetected(string name)
	{
		if (Application.loadedLevelName == "Game")
		{
			WorldGameObject.INSTANCE.ExitGame();
		}
	}

	private IEnumerator SpeedHackDetected()
	{
		while (true)
		{
			last_realtime = Time.time;
			last_datetime = DateTime.Now;
			yield return (object)new WaitForSeconds(5f);
			float good = (DateTime.Now - last_datetime).Seconds;
			float current = Time.time - last_realtime;
			Debug.Log((object)good);
			Debug.Log((object)current);
			if (Math.Abs(good - current) > 0.5f)
			{
				OnCheatDetected("speed_hack");
			}
		}
	}

	private void Update()
	{
		if (gameIni != null && PhotonNetwork.room != null)
		{
			bs.Log(gameIni.gameType);
			gameIni.gameType = (GameINI.GameType)(int)PhotonNetwork.room.customProperties["game_type"];
		}
		ProfileINI.server_time = ProfileINI.server_time.AddSeconds(Time.deltaTime);
		if (is_waiting_shard_population)
		{
			WaitingShardPopulation();
		}
	}

	public void OnConnectedToMaster()
	{
		is_waiting_shard_population = true;
		waiting_shard_population_time = 0f;
	}

	public void OnConnectedToPhoton()
	{
		if (connect_to_random_shard)
		{
			CheckDirectLink();
		}
		if (go_to_menu_on_conected)
		{
			MainMenu.INSTANCE.curWindow = MainMenu.Windows.Battles;
			MainMenu.INSTANCE.battlesRightMenu = MainMenu.BatllesRightMenu.NEWS;
		}
	}

	private void OnDisconnectedFromPhoton()
	{
	}

	public void OnFailedToConnectToPhoton()
	{
	}

	public void ShowGame()
	{
		Screen.SetResolution(800, 600, false);
	}

	public void HideGame()
	{
		Screen.SetResolution(1, 1, false);
	}

	public void OnBallanceChanged()
	{
		((MonoBehaviour)this).StartCoroutine(LoadProfile2(from_loading: false));
	}

	public void OnBallanceChangedOK(string text)
	{
		((MonoBehaviour)this).StartCoroutine(LoadProfile2(from_loading: false));
	}

	public void OnBallanceChangedMM()
	{
		((MonoBehaviour)this).StartCoroutine(LoadProfile2(from_loading: false));
	}

	public void CheckDirectLink()
	{
		Debug.Log((object)"StartCheckDirectLink");
		RoomInfo[] roomList = PhotonNetwork.GetRoomList();
		if (VkontakteAPI.INSTANCE._mapID > 1)
		{
			Debug.Log((object)"DirectLinkTrue");
			if (roomList.Length > 0)
			{
				waiting_room_recive = false;
				MainMenu.INSTANCE.curWindow = MainMenu.Windows.MapCard;
				if (MainMenu.INSTANCE.FindOpenRoom(VkontakteAPI.INSTANCE._mapID))
				{
					MainMenu.INSTANCE.draw_map_card_open_room = true;
					((MonoBehaviour)this).StartCoroutine(MainMenu.INSTANCE.LoadMapInfo(VkontakteAPI.INSTANCE._mapID));
				}
				else
				{
					MainMenu.INSTANCE.draw_map_card_open_room = false;
					((MonoBehaviour)this).StartCoroutine(MainMenu.INSTANCE.LoadMapInfo(VkontakteAPI.INSTANCE._mapID));
				}
			}
			else
			{
				Debug.Log((object)"WaitingForServerListToFindMap");
				waiting_room_recive = true;
			}
		}
		else
		{
			Debug.Log((object)"OpenGameMenu");
			MainMenu.INSTANCE.curWindow = MainMenu.Windows.Battles;
			MainMenu.INSTANCE.battlesRightMenu = MainMenu.BatllesRightMenu.NEWS;
		}
	}

	public IEnumerator LoadProfile2(bool from_loading)
	{
		WWWForm form = new WWWForm();
		form.AddField("id", VkontakteAPI.INSTANCE._viewerId);
		form.AddField("auth_key", VkontakteAPI.INSTANCE._authKey);
		if (instance.platform == ePlatform.ONDOKLASNIKI)
		{
			form.AddField("session_key", VkontakteAPI.INSTANCE._sessionKey);
		}
		Random r = new Random();
		int randomI = r.Next();
		int serverRND = 0;
		string social_name = string.Empty;
		string player_id = string.Empty;
		form.AddField("random", randomI);
		string yoba_key = GenerateKey(20);
		form.AddField("yoba_key", KeyEncrypt(yoba_key));
		WWW php_load = new WWW(instance.GetCurScriptsUrl() + "get_profile.php", form);
		yield return php_load;
		string server_text = php_load.text.Substring(0, php_load.text.IndexOf('\n'));
		string server_key = php_load.text.Substring(php_load.text.IndexOf('\n') + 1);
		_ = server_key;
		VkontakteAPI.MD52(server_text + "_" + yoba_key);
		if (1 == 0)
		{
			yield break;
		}
		yoba_key = string.Empty;
		if (server_text.Substring(0, 13) == "Saved Profile")
		{
			string profile_params = server_text.Substring(15);
			string[] param_vector = profile_params.Split(',');
			for (int i = 0; i < param_vector.Length; i++)
			{
				string[] pare = param_vector[i].Split('=');
				if (pare[0].Trim() == "rnd")
				{
					int.TryParse(pare[1].Trim(), out serverRND);
					if (serverRND == 0)
					{
						serverRND = -4123;
					}
				}
				if (pare[0].Trim() == "soc")
				{
					social_name = pare[1].Trim();
				}
				if (pare[0].Trim() == "player")
				{
					player_id = pare[1].Trim();
				}
				if (pare[0].Trim() == "name")
				{
					ProfileINI.nickname = pare[1].Trim();
				}
				if (pare[0].Trim() == "crystal")
				{
					ProfileINI.crystals = Convert.ToInt32(pare[1].Trim());
				}
				if (pare[0].Trim() == "coins")
				{
					ProfileINI.coins = Convert.ToInt32(pare[1].Trim());
				}
				if (pare[0].Trim() == "destroy")
				{
					ProfileINI.options_destroy = Convert.ToInt32(pare[1].Trim());
					MainMenu.INSTANCE.options_destroy.select_id = ProfileINI.options_destroy;
				}
				if (pare[0].Trim() == "st")
				{
					ProfileINI.server_time = Utils.StringToDateTime(pare[1].Trim());
				}
				if (pare[0].Trim() == "skin")
				{
					ProfileINI.skin = int.Parse(pare[1].Trim());
				}
				if (pare[0].Length <= 3 || !(pare[0].Substring(0, 3) == "inv"))
				{
					continue;
				}
				string[] inv_param = pare[1].Split('|');
				StorePurchase pe = (StorePurchase)Convert.ToInt32(pare[0].Substring(3));
				int pi = Convert.ToInt32(inv_param[0]);
				ProfileINI.SetPurchesValue(pe, pi);
				if (pi > 0)
				{
					int ind = (int)(pe - 5 - 1);
					if (ind < MainMenu.INSTANCE.ItemCosts.Length && MainMenu.INSTANCE.ItemCosts[ind].is_time_pay)
					{
						DateTime time_end = Utils.StringToDateTime(inv_param[1]);
						ProfileINI.SetPurchesTime(pe, time_end);
					}
				}
			}
			bool profile_is_valid = true;
			if (serverRND != randomI)
			{
				profile_is_valid = false;
			}
			if (social_name == "vk" && platform != ePlatform.VKONTATE)
			{
				profile_is_valid = false;
			}
			if (social_name == "ws" && platform != ePlatform.WEBSITE)
			{
				profile_is_valid = false;
			}
			if (social_name == "ok" && platform != ePlatform.ONDOKLASNIKI)
			{
				profile_is_valid = false;
			}
			if (social_name == "mm" && platform != ePlatform.MOYMIR)
			{
				profile_is_valid = false;
			}
			if (social_name != "vk" && social_name != "ws" && social_name != "ok" && social_name != "mm")
			{
				profile_is_valid = false;
			}
			if (VkontakteAPI.INSTANCE._viewerId != player_id)
			{
				profile_is_valid = false;
			}
			if (1 == 0)
			{
				yield break;
			}
			if (ProfileINI.nickname == string.Empty || ProfileINI.nickname == "Копатель")
			{
				MainMenu.INSTANCE.curWindow = MainMenu.Windows.Name;
			}
			else if (from_loading)
			{
				ProfileINI.LoadFromPrefs();
				CheckTimePurches();
				Debug.Log((object)VkontakteAPI.INSTANCE._mapID);
				if (VkontakteAPI.INSTANCE._mapID > 1)
				{
					ConnectToRandomServer();
				}
				else
				{
					OpenChooseShardMenu();
				}
			}
		}
		else
		{
			ProfileINI.Reset();
			ProfileINI.Save();
			MainMenu.INSTANCE.curWindow = MainMenu.Windows.Name;
		}
	}

	public void CheckTimePurches()
	{
		foreach (int value in Enum.GetValues(typeof(StorePurchase)))
		{
			if (value != 5)
			{
				int num = value - 5 - 1;
				if (num < MainMenu.INSTANCE.ItemCosts.Length && MainMenu.INSTANCE.ItemCosts[num].is_time_pay && ProfileINI.GetPurchesValue((StorePurchase)value) > 0 && ProfileINI.GetPurchesTime((StorePurchase)value) < ProfileINI.server_time)
				{
					ProfileINI.SetPurchesValue((StorePurchase)value, 0);
					((MonoBehaviour)this).StartCoroutine(MainMenu.INSTANCE.UsePurches((StorePurchase)value));
				}
			}
		}
	}

	[DebuggerHidden]
	public IEnumerator LoadProfile()
	{
		return LoadProfile2(from_loading: true);
	}
}
