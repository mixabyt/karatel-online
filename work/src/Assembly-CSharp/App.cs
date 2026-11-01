using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Photon;
using UnityEngine;

[Obfuscation(Exclude = true, ApplyToMembers = true)]
public class App : MonoBehaviour
{
	public enum Platform
	{
		VKONTATE,
		WEBSITE,
		ONDOKLASNIKI,
		MOYMIR,
		FACEBOOK,
		STEAM
	}

	public static App Instance;

	public Platform CurPlatform;

	public bool WebTest;

	public bool LocalTest;

	public bool RemoteTest;

	internal string GameVersion = "159f9";

	internal GameINI Settings;

	internal Random Rnd = new Random();

	internal AntiMat AntiMatSystem = new AntiMat();

	internal bool IsSSL;

	internal string[] _ScriptsURL = new string[5] { "vk.diggerworld.ru/VK2/", "62.75.224.107/Test2/", "188.138.112.218/OK3/", "62.75.224.107/MM3/", "server1.diggerworld.ru/FB/" };

	internal string[] MapSavesULR = new string[5] { "vk2.diggerworld.ru/VK/", "62.75.224.107/WB/", "188.138.112.218/OK/", "62.75.224.107/MM/", "server1.diggerworld.ru/FB/" };

	internal string[] PhotonAppIds = new string[5] { "b773236b-b47e-463c-9b6d-07f3d866fef0", "b773236b-b47e-463c-9b6d-07f3d866fef0", "b773236b-b47e-463c-9b6d-07f3d866fef0", "b773236b-b47e-463c-9b6d-07f3d866fef0", "3b7d2ea1-4137-4e28-8ab7-da27249f0400" };

	internal int[] PlatformShardCount = new int[5] { 13, 1, 5, 3, 1 };

	internal int MaxShardPopulation = 900;

	internal int[] ShardPopulation = new int[14];

	internal int CurrentShard;

	private float _WaitingShardPopulationTime;

	private bool _WaitingShardPopulation;

	private int _CurShardTest;

	private bool _AppQuit;

	private float last_realtime;

	private DateTime last_datetime;

	private DateTime olddt;

	public string GetProtocol()
	{
		if (IsSSL)
		{
			return "https://";
		}
		return "http://";
	}

	public string GetCurScriptsUrl()
	{
		return GetProtocol() + _ScriptsURL[(int)CurPlatform];
	}

	public string GetCurScriptsUrlSmall()
	{
		return _ScriptsURL[(int)CurPlatform];
	}

	public string GetCurSavesUrl()
	{
		return GetProtocol() + MapSavesULR[(int)CurPlatform];
	}

	public string GetContentUrl()
	{
		return GetProtocol() + "diggerworld.ru/content/";
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

	private void Awake()
	{
		Debug.Log((object)ProfileINI.LevelExp(711));
		if ((Object)(object)Instance == (Object)null)
		{
			Instance = this;
			Object.DontDestroyOnLoad((Object)(object)((Component)this).gameObject);
		}
		else if (PhotonNetwork.connected)
		{
			MainMenu.Instance.HideLoading();
			MainMenu.Instance.SwitchMenu(Menu.MyMaps);
			if (ProfileINI.was_banned)
			{
				MainMenu.Instance.ShowHint("HINT_YOU_HAVE_BAN");
				ProfileINI.was_banned = false;
			}
			else if (ProfileINI.server_was_closed)
			{
				MainMenu.Instance.ShowHint("HINT_SERVER_CLOSED");
				ProfileINI.server_was_closed = false;
			}
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
		else
		{
			Screen.lockCursor = false;
			MainMenu.Instance.HideLoading();
			Instance.Start();
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
	}

	private void Start()
	{
		PhotonNetwork.UseNameServer = true;
		if (LocalTest)
		{
			PhotonNetwork.PhotonServerSettings.UseMyServer("176.60.181.71", 5055, "Master");
		}
		else if (RemoteTest)
		{
			PhotonNetwork.PhotonServerSettings.UseMyServer("178.63.48.23", 5055, "Master");
		}
		else if (CurPlatform == Platform.FACEBOOK)
		{
			PhotonNetwork.PhotonServerSettings.UseMyServer("178.63.48.23", 5055, "Master");
		}
		else
		{
			PhotonNetwork.PhotonServerSettings.UseCloud(PhotonAppIds[(int)CurPlatform], 0);
		}
		if (Application.isEditor || (CurPlatform == Platform.WEBSITE && WebTest))
		{
			MainMenu.Instance.SwitchMenu(Menu.Welcome);
		}
		else
		{
			Application.ExternalCall("SendSettingsToPlayer", new object[0]);
		}
	}

	public void ConnectToShard(bool random)
	{
		CurrentShard = 0;
		if (!random)
		{
			_CurShardTest = 1;
			_WaitingShardPopulation = false;
			TestNextShard();
			return;
		}
		if (VkontakteAPI.INSTANCE._gameID == string.Empty)
		{
			CurrentShard = Random.Range(0, PlatformShardCount[(int)CurPlatform]) + 1;
		}
		else
		{
			CurrentShard = int.Parse(VkontakteAPI.INSTANCE._gameID.Substring(0, 2));
		}
		MainMenu.Instance.HideLoading();
		MainMenu.Instance.ShowLoading("LOADING_SERVER", string.Empty);
		PhotonNetwork.autoJoinLobby = true;
		PhotonNetwork.ConnectUsingSettings(CurPlatform.ToString() + GameVersion + "Qshard_" + CurrentShard);
	}

	public void TestNextShard()
	{
		if (_CurShardTest > PlatformShardCount[(int)CurPlatform])
		{
			_CurShardTest = 0;
			MainMenu.Instance.HideLoading();
			MainMenu.Instance.SwitchMenu(Menu.ServerSelect);
			return;
		}
		MainMenu.Instance.ShowLoading("LOADING_GET_SHARDS", " [" + _CurShardTest + "/" + PlatformShardCount[(int)CurPlatform] + "]");
		PhotonNetwork.autoJoinLobby = false;
		PhotonNetwork.ConnectUsingSettings(CurPlatform.ToString() + GameVersion + "Qshard_" + _CurShardTest);
	}

	public void WaitingShardPopulation()
	{
		if (_CurShardTest <= 0)
		{
			return;
		}
		_WaitingShardPopulationTime += Time.deltaTime;
		if (_WaitingShardPopulationTime < 5f)
		{
			if (PhotonNetwork.countOfRooms > 0)
			{
				ShardPopulation[_CurShardTest - 1] = PhotonNetwork.countOfRooms;
				PhotonNetwork.Disconnect();
				_CurShardTest++;
				_WaitingShardPopulation = false;
			}
		}
		else
		{
			ShardPopulation[_CurShardTest - 1] = PhotonNetwork.countOfRooms;
			PhotonNetwork.Disconnect();
			_CurShardTest++;
			_WaitingShardPopulation = false;
		}
	}

	public void OnCheatDetected(string name)
	{
		if (Application.loadedLevelName == "Game")
		{
			WorldGameObject.Instance.ExitGame();
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
		ProfileINI.server_time = ProfileINI.server_time.AddSeconds(Time.deltaTime);
		if (_WaitingShardPopulation)
		{
			WaitingShardPopulation();
		}
	}

	public void OnConnectedToMaster()
	{
		if (_CurShardTest > 0)
		{
			_WaitingShardPopulation = true;
			_WaitingShardPopulationTime = 0f;
		}
	}

	public void OnConnectedToPhoton()
	{
		Debug.Log((object)"OnConnectedToPhoton");
		if (CurrentShard <= 0)
		{
			return;
		}
		if (VkontakteAPI.INSTANCE._gameID == string.Empty)
		{
			MainMenu.Instance.HideLoading();
			MainMenu.Instance.SwitchMenu(Menu.Start);
			if (VkontakteAPI.INSTANCE._mapID > 1)
			{
				int mapID = VkontakteAPI.INSTANCE._mapID;
				VkontakteAPI.INSTANCE._mapID = 0;
				MainMenu.Instance.SwitchMenu(Menu.TopMaps, mapID);
			}
		}
		else
		{
			MainMenu.Instance.SetLoadingText(Localize.GetText("LOADING_WAIT_ROOMS"), string.Empty);
		}
	}

	private void OnReceivedRoomListUpdate()
	{
		if (CurrentShard > 0 && VkontakteAPI.INSTANCE._gameID != string.Empty)
		{
			((MonoBehaviour)this).StartCoroutine(TryConnectToRoomProcess());
		}
	}

	private IEnumerator TryConnectToRoomProcess()
	{
		MainMenu.Instance.SetLoadingText("GAME_URL_FIND_ROOM", string.Empty);
		string roomName = VkontakteAPI.INSTANCE._gameID.Substring(2);
		VkontakteAPI.INSTANCE._gameID = string.Empty;
		RoomInfo foundedRoom = null;
		for (int i = 0; i < 10; i++)
		{
			RoomInfo[] allBattlesRoomList = PhotonNetwork.GetRoomList();
			RoomInfo[] array = allBattlesRoomList;
			foreach (RoomInfo room in array)
			{
				if (room.name == roomName)
				{
					foundedRoom = room;
					break;
				}
			}
			if (foundedRoom != null)
			{
				break;
			}
			yield return (object)new WaitForSeconds(1f);
		}
		MainMenu.Instance.HideLoading();
		MainMenu.Instance.SwitchMenu(Menu.Start);
		if (foundedRoom != null)
		{
			MainMenu.Instance.JoinGame(foundedRoom);
		}
		else
		{
			MainMenu.Instance.ShowHint("GAME_URL_GAME_NOT_FOUND");
		}
	}

	private void OnDisconnectedFromPhoton()
	{
		((MonoBehaviour)this).StartCoroutine(HandleDisconnection());
	}

	private void OnFailedToConnectToPhoton()
	{
		((MonoBehaviour)this).StartCoroutine(HandleDisconnection());
	}

	private void OnApplicationQuit()
	{
		_AppQuit = true;
	}

	private IEnumerator HandleDisconnection()
	{
		yield return 0;
		if (_AppQuit)
		{
			yield break;
		}
		if (_CurShardTest > 0)
		{
			TestNextShard();
			yield break;
		}
		MainMenu.Instance.HideMenu();
		MainMenu.Instance.HideLoading();
		if (Application.loadedLevelName == "Game" && WorldGameObject.Instance.IsWorldGenerated)
		{
			MainMenu.Instance.ShowLoading("LOADING_LEAVING", string.Empty);
			Application.LoadLevel("Menu");
		}
		else if (Application.loadedLevelName == "Menu")
		{
			ConnectToShard(random: false);
		}
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

	public void CheckTimePurchases()
	{
		foreach (KeyValuePair<StorePurchase, Store.PurchaseInfo> purchase in Store.Purchases)
		{
			Store.Pay cost = purchase.Value.Cost;
			if (cost is Store.TimedPay && ProfileINI.GetPurchaseValue(purchase.Key) > 0 && ProfileINI.GetPurchaseTime(purchase.Key) < ProfileINI.server_time)
			{
				ProfileINI.SetPurchaseValue(purchase.Key, 0);
				((MonoBehaviour)this).StartCoroutine(MainMenu.Instance.PurchaseUse(purchase.Key));
			}
		}
	}

	public IEnumerator LoadProfile()
	{
		((MonoBehaviour)this).StartCoroutine(MainMenu.Instance.LoadNewsBlocks());
		((MonoBehaviour)this).StartCoroutine(SoundManager.Instance.LoadingSound());
		Debug.Log((object)"Start loading profile");
		if (VkontakteAPI.INSTANCE._viewerId == "16676491")
		{
			MainMenu.Instance.ShowLoading("LOADING_BANNED", string.Empty);
			yield break;
		}
		Debug.Log((object)"EveryDayBonus...");
		WWWForm everydayBonusForm = new WWWForm();
		everydayBonusForm.AddField("id", VkontakteAPI.INSTANCE._viewerId);
		Random r = new Random();
		everydayBonusForm.AddField("random", r.Next());
		WWW everydayBonus = new WWW(GetCurScriptsUrl() + "everyday_bonus.php", everydayBonusForm);
		yield return everydayBonus;
		if (everydayBonus.error != null)
		{
			Debug.LogError((object)everydayBonus.error);
			yield break;
		}
		int bonusGemsIndex = everydayBonus.text.IndexOf("BONUS_GEMS");
		if (bonusGemsIndex != -1)
		{
			MainMenu.Instance.BonusGemsEndTime = DateTime.Parse(everydayBonus.text.Substring(everydayBonus.text.IndexOf("BONUS_GEMS") + 11));
		}
		if (everydayBonus.text.StartsWith("OK"))
		{
			string[] data = everydayBonus.text.Split(' ');
			int[] bonusData = new int[3]
			{
				int.Parse(data[1]),
				int.Parse(data[2]),
				int.Parse(data[3])
			};
			MainMenu.Instance.HideLoading();
			if (everydayBonus.text.Contains("SHOW_BONUS_GEMS"))
			{
				MainMenu.Instance.SwitchMenu(Menu.BonusGems, bonusData);
			}
			else
			{
				MainMenu.Instance.SwitchMenu(Menu.Bonus, bonusData);
			}
		}
		else
		{
			yield return ((MonoBehaviour)this).StartCoroutine(LoadProfile2(from_loading: true));
		}
	}

	public IEnumerator LoadProfile2(bool from_loading)
	{
		Debug.Log((object)"LoadProfile...");
		WWWForm getProfileForm = new WWWForm();
		getProfileForm.AddField("id", VkontakteAPI.INSTANCE._viewerId);
		getProfileForm.AddField("auth_key", VkontakteAPI.INSTANCE._authKey);
		if (CurPlatform == Platform.ONDOKLASNIKI)
		{
			getProfileForm.AddField("session_key", VkontakteAPI.INSTANCE._sessionKey);
		}
		Random r = new Random();
		int randomI = r.Next();
		int serverRND = 0;
		string social_name = string.Empty;
		string player_id = string.Empty;
		getProfileForm.AddField("random", randomI);
		string yoba_key = GenerateKey(20);
		getProfileForm.AddField("yoba_key", KeyEncrypt(yoba_key));
		WWW getProfile = new WWW(GetCurScriptsUrl() + "get_profile.php", getProfileForm);
		yield return getProfile;
		Debug.Log((object)getProfile.text);
		if ((Object)(object)((Component)this).gameObject.GetComponent<ContentUpdater>() == (Object)null)
		{
			((Component)this).gameObject.AddComponent<ContentUpdater>();
		}
		if (getProfile.text.StartsWith("BANNED"))
		{
			string until = getProfile.text.Substring(getProfile.text.IndexOf("UNTIL") + 6);
			string description = ((!getProfile.text.Contains("CHEATING")) ? (Localize.GetText("BANNED_OTHER") + until) : (Localize.GetText("BANNED_CHEATING") + until));
			MainMenu.Instance.HideLoading();
			MainMenu.Instance.ShowAskMenu("BANNED_TITLE", description, new string[1] { "BANNED_UNBAN" }, new Action[1]
			{
				() =>
				{
					((MonoBehaviour)this).StartCoroutine(UnbanProcess());
				}
			}, showCloseButton: false);
			yield break;
		}
		if (getProfile.text.StartsWith("ACTIVATION"))
		{
			MainMenu.Instance.SetLoadingText("ACTIVATION_REQUIRED", string.Empty);
			yield break;
		}
		string server_text = getProfile.text.Substring(0, getProfile.text.IndexOf('\n'));
		string server_key = getProfile.text.Substring(getProfile.text.IndexOf('\n') + 1);
		if (!(server_key == VkontakteAPI.MD52(server_text + "_" + yoba_key)))
		{
			yield break;
		}
		Debug.Log((object)"Enter parsing profile...");
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
				}
				if (pare[0].Trim() == "st")
				{
					ProfileINI.server_time = Utils.StringToDateTime(pare[1].Trim());
				}
				if (pare[0].Trim() == "skin")
				{
					ProfileINI.skin = int.Parse(pare[1].Trim());
				}
				if (pare[0].Length > 3 && pare[0].Substring(0, 3) == "inv")
				{
					string[] inv_param = pare[1].Split('\t');
					StorePurchase pe = (StorePurchase)Convert.ToInt32(pare[0].Substring(3));
					int pi = Convert.ToInt32(inv_param[0]);
					ProfileINI.SetPurchaseValue(pe, pi);
					if (pi > 0 && Store.Purchases.ContainsKey(pe))
					{
						Store.Pay pay = Store.Purchases[pe].Cost;
						if (pay is Store.TimedPay)
						{
							DateTime time_end = Utils.StringToDateTime(inv_param[1]);
							ProfileINI.SetPurchaseTime(pe, time_end);
						}
					}
				}
				if (pare[0].Trim() == "experience")
				{
					ProfileINI.experience = Convert.ToInt32(pare[1].Trim());
				}
			}
			bool profile_is_valid = true;
			if (serverRND != randomI)
			{
				profile_is_valid = false;
			}
			if (social_name == "fb" && CurPlatform != Platform.FACEBOOK)
			{
				profile_is_valid = false;
			}
			if (social_name == "vk" && CurPlatform != Platform.VKONTATE)
			{
				profile_is_valid = false;
			}
			if (social_name == "ws" && CurPlatform != Platform.WEBSITE)
			{
				profile_is_valid = false;
			}
			if (social_name == "ok" && CurPlatform != Platform.ONDOKLASNIKI)
			{
				profile_is_valid = false;
			}
			if (social_name == "mm" && CurPlatform != Platform.MOYMIR)
			{
				profile_is_valid = false;
			}
			if (social_name != "vk" && social_name != "ws" && social_name != "ok" && social_name != "mm" && social_name != "fb")
			{
				profile_is_valid = false;
			}
			if (VkontakteAPI.INSTANCE._viewerId != player_id)
			{
				profile_is_valid = false;
			}
			if (profile_is_valid)
			{
				if (ProfileINI.nickname == string.Empty || ProfileINI.nickname == "Копатель")
				{
					MainMenu.Instance.HideLoading();
					MainMenu.Instance.SwitchMenu(Menu.ChangeName);
				}
				else if (from_loading)
				{
					ProfileINI.LoadFromPrefs();
					CheckTimePurchases();
					ConnectToShard(random: true);
				}
			}
		}
		else
		{
			ProfileINI.Reset();
			ProfileINI.Save();
			MainMenu.Instance.HideLoading();
			MainMenu.Instance.SwitchMenu(Menu.ChangeName);
		}
	}

	private IEnumerator UnbanProcess()
	{
		if (Application.isEditor)
		{
			WWWForm unbanForm = new WWWForm();
			unbanForm.AddField("viewer_id", VkontakteAPI.INSTANCE._viewerId);
			unbanForm.AddField("auth_key", VkontakteAPI.INSTANCE._authKey);
			WWW unban = new WWW(Instance.GetCurScriptsUrl() + "unban.php", unbanForm);
			yield return unban;
			Debug.Log((object)("Unban result: " + unban.text));
		}
		else
		{
			Application.ExternalCall("ByUnban", new object[0]);
		}
	}
}
