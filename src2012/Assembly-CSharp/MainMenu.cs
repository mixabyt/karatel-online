using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class MainMenu : bs
{
	public enum Windows
	{
		Name,
		Game,
		GameSave,
		MapCard,
		RoomUrl,
		Main,
		Loading,
		Battles,
		Options,
		ByCrystal,
		Store,
		TutorialImages,
		PublicMap,
		Saves,
		Profile,
		GameStore,
		EveryDayBonus,
		DrawTablichka,
		SendMoney,
		FailedConnect,
		ChooseServer,
		SetPicture,
		WelcomeScreen,
		DeathMenu,
		BanMenu,
		ServerWasClosed,
		ExitMenu,
		Voice,
		SelectWeapon
	}

	public enum MAP_LIST_TYPE
	{
		Last,
		Day,
		Week,
		Month,
		Best,
		Search
	}

	private class Map
	{
		public string autor_id;

		public string autor_slot_id;

		public string mapId;

		public string name;

		public string rating;
	}

	private class Comment
	{
		public string name;

		public string text;

		public string id;
	}

	private enum WelcomeScreenWindow
	{
		WELCOME,
		REGISTER,
		PASSWORD_RECOVER
	}

	private enum BatllesLeftMenu
	{
		UNKNOWN,
		BUILDING,
		MY_MAPS,
		MAIN,
		FAST_CONNECT
	}

	private enum BatllesServersMenu
	{
		UNKNOWN,
		ALL_SERVERS,
		FAVORIT
	}

	public enum BatllesRightMenu
	{
		UNKNOWN,
		NEW_BATTLE,
		EXISTS_BATTLE,
		SAVED_BATTLE,
		MY_MAPS,
		NEWS
	}

	private class FavoritSavers
	{
		public int admin_id;

		public string server_name;

		public byte slot_id;
	}

	private enum StoreLeftWindow
	{
		BLOCKS,
		MEBEL,
		PLANTS,
		OTHER,
		INSTRUMENTS,
		SKINS,
		MAPS,
		MOBS,
		WEAPONS,
		BONUS
	}

	public class SelectBtn
	{
		private List<string> texts = new List<string>();

		public string name;

		public int select_id;

		public void AddItem(string name)
		{
			texts.Add(name);
		}

		public void Draw()
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label(name, GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
			GUILayout.Space(40f);
			GUILayout.Label(texts[select_id], GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
			if (GUILayout.Button("<", GUIStyle.op_Implicit("ShortButton"), new GUILayoutOption[0]))
			{
				select_id--;
				if (select_id < 0)
				{
					select_id = 0;
				}
				ProfileINI.options_destroy = select_id;
				((MonoBehaviour)INSTANCE).StartCoroutine(ProfileINI.SaveDestroy());
			}
			if (GUILayout.Button(">", GUIStyle.op_Implicit("ShortButton"), new GUILayoutOption[0]))
			{
				select_id++;
				if (select_id >= texts.Count)
				{
					select_id = texts.Count - 1;
				}
				ProfileINI.options_destroy = select_id;
				((MonoBehaviour)INSTANCE).StartCoroutine(ProfileINI.SaveDestroy());
			}
			GUILayout.EndHorizontal();
		}
	}

	public enum MapWindows
	{
		MY_MAPS,
		BEST_MAPS_ALL_TIME,
		BEST_MAPS_DAY,
		BEST_MAPS_WEEK,
		BEST_MAPS_MONTH,
		NEW_MAPS,
		SEARCH_MAPS,
		SEARCH_NAME,
		SEARCH_AUTHOR,
		SEARCH_RESULT
	}

	private enum eVouteData
	{
		VOUTE_UNKNOWN,
		VOUTE_YES,
		VOUTE_NO,
		VOUTE_NOT
	}

	public Texture2D[] button_textures;

	public string loading_text = "загружаю";

	public static MainMenu INSTANCE;

	public Texture2D standart_map_img;

	public Texture2D player_map_img;

	public RenderTexture player_preview;

	public RenderTexture mob_preview;

	[NonSerialized]
	public GameObject preview_obj;

	[NonSerialized]
	public GameObject mobs_obj;

	public GUISkin guiSkin;

	public GUISkin boxSkin;

	public Texture2D crystalImg;

	public Texture2D coinsImg;

	public Texture2D coinsImgBig;

	public Texture2D vkImg;

	public Texture2D mmImg;

	public Texture2D LockServerIcon;

	public GUISkin PlateSkin;

	public Texture2D[] helpImages;

	public Texture2D[] storeIcons;

	public string[] storeItemName;

	public string[] storeItemText;

	public StoreImgList[] storeImgList;

	public int[] purchesImgInd;

	public int[] clotheInd;

	public StoreItemType[] purchesType;

	public Texture2D[] diffImg;

	public Texture2D[] MapPreview;

	public StorePurchase[] entityPurches;

	public Dictionary<EntityType, StorePurchase> purchesEntityDictionary;

	public ValuePay[] ItemCosts;

	private bool destroyable = true;

	private bool gifts = true;

	private ArrayList Maps = new ArrayList();

	private ArrayList Comments = new ArrayList();

	public Windows curWindow;

	public Windows lastWindow;

	public bool showMenu = true;

	private string MapPopulation = "12";

	private string Password = string.Empty;

	private int _GameType;

	private string[] _GameTypes = new string[3] { "Строительство", "Война", "Командный бой" };

	private int cur_map_page;

	private int count_page = 1;

	private int comments_start_ind;

	private int count_comments;

	private int max_page_view;

	private string search_map_name = string.Empty;

	private string search_map_author = string.Empty;

	private int search_type;

	private bool sort_type = true;

	private bool sort_type2;

	private bool show_search_error;

	private MAP_LIST_TYPE cur_map_list_view = MAP_LIST_TYPE.Day;

	private GUIStyle listStyle = new GUIStyle();

	private string news_text = string.Empty;

	private string news_url = string.Empty;

	private Texture2D news_img;

	public Texture2D[] FastBarImages;

	public Texture2D[] FastBarNumbers;

	private bool _SpeedEnabled = true;

	private bool _FlyEnabled = true;

	[NonSerialized]
	public bool is_cursor_text;

	[NonSerialized]
	public string cursor_text = string.Empty;

	[NonSerialized]
	public bool cursor_take;

	[NonSerialized]
	public bool cursor_sit;

	[NonSerialized]
	public bool cursor_open;

	[NonSerialized]
	public bool cursor_activate;

	[NonSerialized]
	public bool cursor_write;

	[NonSerialized]
	public bool cursor_draw;

	[NonSerialized]
	public bool cursor_wheel;

	[NonSerialized]
	public bool cursor_put;

	public bool draw_map_card_open_room;

	public PlayerNode send_money_player;

	private string send_money_error = string.Empty;

	private string coins_to_send = "0";

	private int int_coins;

	public string welcome_email = string.Empty;

	public string welcome_password = string.Empty;

	private string welcome_password_repeat = string.Empty;

	public bool welcome_remember = true;

	public string password_recover_text = string.Empty;

	private string welcome_error = string.Empty;

	private bool welcome_start_enter;

	private WelcomeScreenWindow curWelcomWindow;

	private string _ConnectPassword = string.Empty;

	private Vector2 _BattlesScroll = default;

	private bool _BattleExistsShowInfo;

	private Vector2 news_scroll = default;

	private List<string> map_types_names = new List<string>();

	private int map_type_select_id;

	private List<string> map_size_names = new List<string>();

	private int map_size_select_id;

	private string _NewGameWarning = string.Empty;

	public bool cinematic_camera;

	private bool enable_clean_screen_mode;

	private BatllesLeftMenu battlesLeftMenu = BatllesLeftMenu.MAIN;

	private BatllesServersMenu battlesServersMenu = BatllesServersMenu.ALL_SERVERS;

	public BatllesRightMenu battlesRightMenu = BatllesRightMenu.NEWS;

	private string _BattlesMapName = string.Empty;

	private Vector2 _BattlesListScroll = new Vector2(0f, 0f);

	private string _FastConnectWarning = string.Empty;

	private int all_player_count;

	private string _ServerNameToFind = "Поиск сервера по названию";

	private FavoritSavers[] favorit_servers;

	private StoreLeftWindow curStoreLeftWindow;

	private Vector2 slider = default;

	private StorePurchase _CurrentPurchase = StorePurchase.NONE;

	private string by_warning = string.Empty;

	private bool start_by_from_coins;

	private Vector2 optionsListScroll;

	private int ind1_count = 6;

	private int ind2_count = 15;

	private int ind3_count = 30;

	private int ind4_count = 60;

	private int ind5_count = 120;

	private int ind1_voute = 3;

	private int ind2_voute = 5;

	private int ind3_voute = 10;

	private int ind4_voute = 20;

	private int ind5_voute = 40;

	[NonSerialized]
	public int count_gifts = 234;

	private float crystall_to_sell;

	public int last_ind;

	public SelectBtn options_destroy = new SelectBtn();

	private MapWindows mapWindows = MapWindows.BEST_MAPS_ALL_TIME;

	private int _SelectedMapID = -2;

	private int _SelectedSlot = -1;

	private string _PublicMapOwnerID = "0";

	private int _PublicMapOwnerSlot;

	private int _PublicMapRating;

	private Vector2 publicMapsScroll;

	private string[] publicMapsList;

	private int publicMapsListSelect;

	private int publicMapsListId;

	private Vector2 my_maps_croll;

	private bool draw_public_form;

	private string post_text = string.Empty;

	private bool draw_comment_form;

	private Vector2 coments_scroll;

	private string public_map_name = string.Empty;

	private string public_map_about = string.Empty;

	private string public_map_url = string.Empty;

	private string public_map_authorname = string.Empty;

	private int close_public_map_count;

	private bool regen_fail;

	private eVouteData vouteData;

	private int curent_page;

	public string picture_url = string.Empty;

	public Picture curent_picture;

	internal string[] PlateLines = new string[4];

	internal Tablichka CurrentPlate;

	private string name_info_text = "Введите свой ник:";

	private bool check_name;

	private WWW php_load;

	private Vector2 _WeaponScrollView;

	private bool _HaveWeapons;

	public bool SpeedEnabled => _SpeedEnabled && ProfileINI.GetPurchesValue(StorePurchase.SPEED) != 0 && !bs._Igor.enableGuns;

	public bool FlyEnabled => _FlyEnabled && ProfileINI.GetPurchesValue(StorePurchase.FLY) != 0 && !bs._Igor.enableGuns;

	public MainMenu()
	{
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Expected Obj, but got Unknown
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
	}

	private void Awake()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected Obj, but got Unknown
		if (!Object.op_Implicit((Object)(object)INSTANCE))
		{
			INSTANCE = this;
		}
		curWindow = Windows.Loading;
		listStyle.normal.textColor = Color.white;
		GUIStyleState onHover = listStyle.onHover;
		Texture2D background = new Texture2D(2, 2);
		listStyle.hover.background = background;
		onHover.background = background;
		RectOffset padding = listStyle.padding;
		int num = 4;
		listStyle.padding.bottom = num;
		num = num;
		listStyle.padding.top = num;
		num = num;
		listStyle.padding.right = num;
		padding.left = num;
		options_destroy.name = "Разрушения";
		options_destroy.AddItem("Простые");
		options_destroy.AddItem("Средние");
		options_destroy.AddItem("Эпичные");
		map_size_names.Add("Маленькая");
		map_size_names.Add("Средняя");
		((MonoBehaviour)this).StartCoroutine(LoadNewsBlock());
	}

	[DebuggerHidden]
	private IEnumerator LoadNewsBlock()
	{
		// ILSpy could not decompile this. Please report the exception below,
		// along with the assembly it came from, at https://github.com/icsharpcode/ILSpy/issues/new
		// System.BadImageFormatException: Invalid method header: 0x13 0x16
		//    at System.Reflection.Metadata.MethodBodyBlock.Create(BlobReader reader)
		//    at System.Reflection.Metadata.PEReaderExtensions.GetMethodBody(PEReader peReader, Int32 relativeVirtualAddress)
		//    at ICSharpCode.Decompiler.Metadata.PEFile.GetMethodBody(Int32 rva) in /_/ICSharpCode.Decompiler/Metadata/PEFile.cs:line 72
		//    at ICSharpCode.Decompiler.IL.ControlFlow.YieldReturnDecompiler.CreateILAst(MethodDefinitionHandle method, ILTransformContext context) in /_/ICSharpCode.Decompiler/IL/ControlFlow/YieldReturnDecompiler.cs:line 499
		//    at ICSharpCode.Decompiler.IL.ControlFlow.YieldReturnDecompiler.AnalyzeMoveNext(ILFunction function) in /_/ICSharpCode.Decompiler/IL/ControlFlow/YieldReturnDecompiler.cs:line 654
		//    at ICSharpCode.Decompiler.IL.ControlFlow.YieldReturnDecompiler.Run(ILFunction function, ILTransformContext context) in /_/ICSharpCode.Decompiler/IL/ControlFlow/YieldReturnDecompiler.cs:line 152
		//    at ICSharpCode.Decompiler.IL.ILFunction.RunTransforms(IEnumerable`1 transforms, ILTransformContext context) in /_/ICSharpCode.Decompiler/IL/Instructions/ILFunction.cs:line 421
		//    at ICSharpCode.Decompiler.CSharp.CSharpDecompiler.DecompileBody(IMethod method, EntityDeclaration entityDecl, DecompileRun decompileRun, ITypeResolveContext decompilationContext, ExtensionInfo extensionInfo) in /_/ICSharpCode.Decompiler/CSharp/CSharpDecompiler.cs:line 2416
	}

	private string RSAEncrypt(string message)
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
			Debug.Log((object)("Problem with RSA:\n" + ex.Message.ToString()));
			return string.Empty;
		}
	}

	private string RSADecrypt(string cryptedMessage)
	{
		try
		{
			string xmlString = "<RSAKeyValue><Modulus>t3A/x4DoMsQNVdIWe0rYmBy98tIQz+2/M5eiqL7IxS2h7L3mHLZVOHeTzDWbBGLHIx8la1oWUXPGUr/31Mgbvuc8PDXW0Oh011LWUYYPgaFLKqwvnS7T8ehNYLNkHxEeiKrtMPHSp0n6IfChKAvUZbdPs9IjAr1kO2lVNJ5Ickc=</Modulus><Exponent>AQAB</Exponent><P>6Ww86GpbXesa3Ja43qW4f41C9Wvga+1NfhLHL7eAVk54ktXL1hnFcMUAVEkZ6gc5eSc0FErJ9LeRVZl+L8EA3w==</P><Q>yS5afQ2STt2M12W0X4+WjIL5eFcEIT1mCbuwxKfC+uyu9YwPGmyZ31qblGQB1v2BRx5Y4+nPutGnq97giw2zmQ==</Q><DP>dSpfaEn6kqnwUclWAmwsr9m4QnhjrcjvnNjQIqN5R9kbrJikOFO3R2ObRrNqzo3Ry35iJc1kkfGSgeUJ8e5vAQ==</DP><DQ>BrBe29eOrMaa/zJne+HnTIaySrkB7yT+doZiCzfUuMkTzCA7BFLmDtIRaC6nDQHiwX2V0QYeHQUJgSJo7tamyQ==</DQ><InverseQ>oR1DCzYfxCp47qvgv8NfR11/CbjCkWhB+IJNbA2NTsYfidMyP8l9Z1XQXhNWquwuWvI9fe8kF8Vb0MCC4i/cbA==</InverseQ><D>L9ihaiIfbo5ghBAGOSZulE565QkRxYpXotdcX/NTXsrmP1/Q05BjbhUSgeNBicVR98E5+2RGCfzOr+rOc7u2MIW328tV1KNN6CXg44AyU5tFAbtalFXmVRTKRV79n2vPnRhri4VPJj7vVqJgb4xb2lYRWcziOJialWgN2+Q3buE=</D></RSAKeyValue>";
			RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
			rSACryptoServiceProvider.FromXmlString(xmlString);
			byte[] bytes = rSACryptoServiceProvider.Decrypt(Convert.FromBase64String(cryptedMessage), fOAEP: true);
			return Encoding.UTF8.GetString(bytes);
		}
		catch (Exception ex)
		{
			Debug.Log((object)("Problem with RSA:\n" + ex.Message.ToString()));
			return string.Empty;
		}
	}

	public void OnMenuReloaded()
	{
		if (ProfileINI.was_banned)
		{
			curWindow = Windows.BanMenu;
			ProfileINI.was_banned = false;
		}
		else if (ProfileINI.server_was_closed)
		{
			curWindow = Windows.ServerWasClosed;
			ProfileINI.server_was_closed = false;
		}
		else
		{
			curWindow = Windows.Battles;
		}
		if (battlesLeftMenu == BatllesLeftMenu.MY_MAPS && battlesRightMenu == BatllesRightMenu.SAVED_BATTLE)
		{
			battlesRightMenu = BatllesRightMenu.MY_MAPS;
		}
	}

	public void DisableBonuses()
	{
		_SpeedEnabled = false;
		SpeedBonus();
		_FlyEnabled = false;
		FlyBonus();
	}

	private void DrawSpecialBonus()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		if (bs._Igor.enableGuns)
		{
			return;
		}
		if (ProfileINI.GetPurchesValue(StorePurchase.SPEED) != 0)
		{
			GUI.Label(new Rect((float)(Screen.width / 2 - 107), (float)(Screen.height - 75), 68f, 68f), (Texture)(object)FastBarImages[(!SpeedEnabled) ? 1u : 0u]);
			GUI.Label(new Rect((float)(Screen.width / 2 - 107), (float)(Screen.height - 75), 26f, 26f), (Texture)(object)FastBarNumbers[0]);
		}
		if (ProfileINI.GetPurchesValue(StorePurchase.FLY) != 0)
		{
			if (_FlyEnabled)
			{
				GUILayout.BeginArea(new Rect((float)(Screen.width / 2 - 200), (float)(Screen.height - 160), 400f, 200f));
				GUILayout.Label("Пробел - взлет", GUIStyle.op_Implicit("CursorTextSmall"), new GUILayoutOption[0]);
				GUILayout.Label("Левый Шифт - приземление", GUIStyle.op_Implicit("CursorTextSmall"), new GUILayoutOption[0]);
				GUILayout.EndArea();
			}
			GUI.Label(new Rect((float)(Screen.width / 2 - 34), (float)(Screen.height - 75), 68f, 68f), (Texture)(object)FastBarImages[(!FlyEnabled) ? 3 : 2]);
			GUI.Label(new Rect((float)(Screen.width / 2 - 34), (float)(Screen.height - 75), 26f, 26f), (Texture)(object)FastBarNumbers[1]);
		}
		if (ProfileINI.GetPurchesValue(StorePurchase.TIME_OF_DAY) != 0 && !App.instance.gameIni.isWatch && Level.Instance.IsAdministrator())
		{
			if (TimeOfDay.NormalizedTime == 0f || TimeOfDay.NormalizedTime == 1f)
			{
				GUI.Label(new Rect((float)(Screen.width / 2 + 39), (float)(Screen.height - 75), 68f, 68f), (Texture)(object)FastBarImages[(!(TimeOfDay.NormalizedTime < 0.5f)) ? 5 : 4]);
			}
			else
			{
				Matrix4x4 matrix = GUI.matrix;
				GUIUtility.RotateAroundPivot(TimeOfDay.NormalizedTime * 360f, new Vector2((float)(Screen.width / 2 + 39 + 34), (float)(Screen.height - 75 + 34)));
				GUI.Label(new Rect((float)(Screen.width / 2 + 39), (float)(Screen.height - 75), 68f, 68f), (Texture)(object)FastBarImages[(!(TimeOfDay.NormalizedTime < 0.5f)) ? 5 : 4]);
				GUI.matrix = matrix;
			}
			GUI.Label(new Rect((float)(Screen.width / 2 + 39), (float)(Screen.height - 75), 26f, 26f), (Texture)(object)FastBarNumbers[2]);
		}
	}

	public void SpeedBonus()
	{
		bool flag = SpeedEnabled && ProfileINI.GetPurchesValue(StorePurchase.SPEED) != 0;
		if ((Object)(object)WorldGameObject.INSTANCE.MainPlayer != (Object)null)
		{
			WorldGameObject.INSTANCE.MainPlayer.SendMessage("SetSpeedEffect", (object)flag);
		}
	}

	public void FlyBonus()
	{
		bool flag = (FlyEnabled && ProfileINI.GetPurchesValue(StorePurchase.FLY) != 0) || ((Object)(object)TeamBattle.Instance != (Object)null && TeamBattle.Instance.IsObserving());
		if ((Object)(object)WorldGameObject.INSTANCE.MainPlayer != (Object)null)
		{
			WorldGameObject.INSTANCE.MainPlayer.SendMessage("SetLadderBody", (object)flag);
		}
	}

	public void UpdateSpecialBonus()
	{
		if (!bs._Igor.enableGuns)
		{
			if (Input.GetKeyDown((KeyCode)49) && ProfileINI.GetPurchesValue(StorePurchase.SPEED) != 0)
			{
				_SpeedEnabled = !_SpeedEnabled;
				SpeedBonus();
			}
			if (Input.GetKeyDown((KeyCode)50) && ProfileINI.GetPurchesValue(StorePurchase.FLY) != 0)
			{
				_FlyEnabled = !_FlyEnabled;
				FlyBonus();
			}
			if (Input.GetKeyDown((KeyCode)51) && ProfileINI.GetPurchesValue(StorePurchase.TIME_OF_DAY) != 0 && !App.instance.gameIni.isWatch && Level.Instance.IsAdministrator())
			{
				TimeOfDay.SwitchDayNightForAll();
			}
		}
	}

	private void OnGUI()
	{
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Expected Obj, but got Unknown
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		FixMenuControl(10000);
		if ((Object)(object)guiSkin != (Object)null)
		{
			GUI.skin = guiSkin;
		}
		if (!enable_clean_screen_mode && Application.loadedLevelName == "Game" && App.instance.gameIni.isWatch && WorldGameObject.INSTANCE.IsWorldGenerated)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(250f);
			GUILayout.Label(" [Режим просмотра : TAB - информация о карте] ", GUIStyle.op_Implicit("ProsmotrLabel"), new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
		}
		if (Application.loadedLevelName == "Game" && is_cursor_text)
		{
			GUILayout.BeginArea(new Rect((float)(Screen.width / 2 - 200), (float)(Screen.height / 2 - 100), 400f, 200f));
			GUILayout.Label(cursor_text, GUIStyle.op_Implicit("CursorText"), new GUILayoutOption[0]);
			if (cursor_write)
			{
				GUILayout.Label("[F] написать", GUIStyle.op_Implicit("CursorTextSmall"), new GUILayoutOption[0]);
			}
			if (cursor_take)
			{
				if (WorldGameObject.INSTANCE.curSelItem == EntityType.RAIL)
				{
					GUILayout.Label("[E] разрушить", GUIStyle.op_Implicit("CursorTextSmall"), new GUILayoutOption[0]);
				}
				else
				{
					GUILayout.Label("[E] взять", GUIStyle.op_Implicit("CursorTextSmall"), new GUILayoutOption[0]);
				}
			}
			if (cursor_activate)
			{
				GUILayout.Label("[F] активировать", GUIStyle.op_Implicit("CursorTextSmall"), new GUILayoutOption[0]);
			}
			if (cursor_sit)
			{
				if (WorldGameObject.INSTANCE.curSelItem == EntityType.BED || WorldGameObject.INSTANCE.curSelItem == EntityType.BED_STONE)
				{
					GUILayout.Label("[F] лежать", GUIStyle.op_Implicit("CursorTextSmall"), new GUILayoutOption[0]);
				}
				else if (WorldGameObject.INSTANCE.curSelItem == EntityType.CHAIR || WorldGameObject.INSTANCE.curSelItem == EntityType.CHAIR_STONE || WorldGameObject.INSTANCE.curSelItem == EntityType.BENCH_WOOD || WorldGameObject.INSTANCE.curSelItem == EntityType.BENCH_STONE)
				{
					GUILayout.Label("[F] сидеть", GUIStyle.op_Implicit("CursorTextSmall"), new GUILayoutOption[0]);
				}
				else
				{
					GUILayout.Label("[F] сесть/выйти", GUIStyle.op_Implicit("CursorTextSmall"), new GUILayoutOption[0]);
				}
			}
			if (cursor_draw)
			{
				GUILayout.Label("[F] нарисовать", GUIStyle.op_Implicit("CursorTextSmall"), new GUILayoutOption[0]);
			}
			if (cursor_open)
			{
				GUILayout.Label("[F] октрыть/закрыть", GUIStyle.op_Implicit("CursorTextSmall"), new GUILayoutOption[0]);
			}
			if (cursor_put)
			{
				GUILayout.Label("[E] установить", GUIStyle.op_Implicit("CursorTextSmall"), new GUILayoutOption[0]);
			}
			if (cursor_wheel)
			{
				GUILayout.Label("[Колесико мышки] вращать", GUIStyle.op_Implicit("CursorTextSmall"), new GUILayoutOption[0]);
			}
			GUILayout.EndArea();
		}
		if (!enable_clean_screen_mode && Application.loadedLevelName == "Game" && WorldGameObject.INSTANCE.IsWorldGenerated)
		{
			DrawSpecialBonus();
		}
		if (showMenu)
		{
			if (curWindow == Windows.Loading)
			{
				GUILayout.Window(8, new Rect((float)(Screen.width / 2 - 200), (float)(Screen.height / 2 - 100), 400f, 200f), (WindowFunction)LoadingMenu, "Загрузка...", new GUILayoutOption[0]);
			}
			else if (curWindow == Windows.WelcomeScreen)
			{
				DrawWelcomeScreen();
			}
			else if (curWindow == Windows.RoomUrl)
			{
				DrawRoomUrlMenu();
			}
			else if (curWindow == Windows.ChooseServer)
			{
				DrawChooseServer();
			}
			else if (curWindow == Windows.FailedConnect)
			{
				DrawFailedConnect();
			}
			else if (curWindow == Windows.SendMoney)
			{
				DrawSendMoney();
			}
			else if (curWindow == Windows.ServerWasClosed)
			{
				DrawServerWasClosed();
			}
			else if (curWindow == Windows.BanMenu)
			{
				DrawBanMenu();
			}
			else if (curWindow == Windows.ExitMenu)
			{
				DrawExitMenu();
			}
			else if (curWindow == Windows.DeathMenu)
			{
				DrawDeathMenu();
			}
			else if (curWindow == Windows.Game)
			{
				DrawInGameMenu();
			}
			else if (curWindow == Windows.MapCard)
			{
				DrawMapCard();
			}
			else if (curWindow == Windows.GameStore)
			{
				DrawInGameStore();
			}
			else if (curWindow == Windows.EveryDayBonus)
			{
				DrawEveryDayBonus();
			}
			else if (curWindow == Windows.GameSave)
			{
				DrawInGameSave();
			}
			else if (curWindow == Windows.TutorialImages)
			{
				DrawTutorial();
			}
			else if (curWindow == Windows.DrawTablichka)
			{
				DrawTablichkaMenu();
			}
			else if (curWindow == Windows.SetPicture)
			{
				SetPictureMenu();
			}
			else if (curWindow == Windows.Name)
			{
				NameMenu();
			}
			else if (curWindow == Windows.Voice)
			{
				VoiceMenu();
			}
			else if (curWindow == Windows.SelectWeapon)
			{
				WeaponMenu();
			}
			else
			{
				GUIMenu(0);
			}
		}
	}

	private void DrawMapCard()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)boxSkin != (Object)null)
		{
			GUI.skin = boxSkin;
		}
		GUI.Box(new Rect(250f, 70f, 360f, 450f), "Карта");
		if ((Object)(object)guiSkin != (Object)null)
		{
			GUI.skin = guiSkin;
		}
		GUILayout.BeginArea(new Rect(260f, 70f, 340f, 450f));
		if (!draw_map_card_open_room)
		{
			DrawSaveMapInfo();
		}
		else
		{
			GUILayout.Space(30f);
			DrawBattlesExistsMenu();
		}
		GUILayout.EndArea();
	}

	private void DrawChooseServer()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)boxSkin != (Object)null)
		{
			GUI.skin = boxSkin;
		}
		GUI.Box(new Rect((float)(Screen.width / 2 - 220), (float)(Screen.height / 2 - 150), 440f, 410f), "Выбор сервера");
		if ((Object)(object)guiSkin != (Object)null)
		{
			GUI.skin = guiSkin;
		}
		GUILayout.BeginArea(new Rect((float)(Screen.width / 2 - 200), (float)(Screen.height / 2 - 150), 400f, 410f));
		GUILayout.Space(40f);
		GUILayout.Label("Разница между мирами только в составе игроков находящихся в них.", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		for (int i = 0; i < App.instance.realy_shard_used[(int)App.instance.platform]; i++)
		{
			int num = (int)(100f * (float)App.instance.shard_count[i] / (float)App.instance.max_shard_population);
			if (num >= 100)
			{
				GUILayout.Button("'Мир " + (i + 1) + "' заполнен ", new GUILayoutOption[0]);
			}
			else if (GUILayout.Button("'Мир " + (i + 1) + "' загруженность : " + num.ToString() + " %", new GUILayoutOption[0]) || (bs._Igor.autojoin && PhotonNetwork.connectionState == ConnectionState.Disconnected))
			{
				App.instance.go_to_menu_on_conected = true;
				PhotonNetwork.autoJoinLobby = true;
				PhotonNetwork.ConnectUsingSettings(App.instance.platform.ToString() + App.instance.game_verison + "Qshard_" + (i + 1));
			}
		}
		GUILayout.EndArea();
	}

	private void DrawFailedConnect()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)boxSkin != (Object)null)
		{
			GUI.skin = boxSkin;
		}
		GUI.Box(new Rect((float)(Screen.width / 2 - 220), (float)(Screen.height / 2 - 50), 440f, 200f), "Проблема!");
		if ((Object)(object)guiSkin != (Object)null)
		{
			GUI.skin = guiSkin;
		}
		GUILayout.BeginArea(new Rect((float)(Screen.width / 2 - 200), (float)(Screen.height / 2 - 50), 400f, 200f));
		GUILayout.Space(60f);
		if (Level.Instance.IsAdministrator() && !App.instance.gameIni.isWatch && GameTypeUtils.CanSaveMap())
		{
			GUILayout.Label("Связь с сервером потеряна. Чтобы не потерять вашу карту сохраните ее, затем обновите страницу с игрой", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
			if (GUILayout.Button("Сохранить карту", new GUILayoutOption[0]))
			{
				OpenSaveMenu();
			}
		}
		else
		{
			GUILayout.Label("Связь с сервером потеряна, обновите страницу и зайдите в игру снова", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		}
		GUILayout.EndArea();
	}

	private void DrawSendMoney()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)boxSkin != (Object)null)
		{
			GUI.skin = boxSkin;
		}
		GUI.Box(new Rect((float)(Screen.width / 2 - 220), (float)(Screen.height / 2 - 50), 440f, 200f), "Перевод");
		if ((Object)(object)guiSkin != (Object)null)
		{
			GUI.skin = guiSkin;
		}
		GUILayout.BeginArea(new Rect((float)(Screen.width / 2 - 200), (float)(Screen.height / 2 - 50), 400f, 200f));
		GUILayout.Space(40f);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("Ваши деньги :", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		GUILayout.Label((Texture)(object)coinsImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[0]);
		GUILayout.Label(ProfileINI.coins.ToString(), GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[0]);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("Отправить игроку :", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		GUILayout.Label(send_money_player.playerName, GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label((Texture)(object)coinsImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[0]);
		coins_to_send = GUILayout.TextField(coins_to_send, new GUILayoutOption[0]);
		if (int.TryParse(coins_to_send, out int_coins))
		{
			int_coins = Mathf.Clamp(int_coins, 0, ProfileINI.coins);
			coins_to_send = int_coins.ToString();
		}
		else
		{
			coins_to_send = "0";
		}
		GUILayout.EndHorizontal();
		if (send_money_error.Length > 0)
		{
			GUILayout.Label(send_money_error, GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		}
		if (GUILayout.Button("Отправить", new GUILayoutOption[0]))
		{
			((MonoBehaviour)this).StartCoroutine(SendMoneyToPlayer(send_money_player.viewer_id, int_coins));
		}
		if (GUILayout.Button("Отмена", new GUILayoutOption[0]))
		{
			showMenu = false;
		}
		GUILayout.EndArea();
	}

	private IEnumerator SendMoneyToPlayer(string player_id, int count)
	{
		Debug.Log((object)player_id);
		Debug.Log((object)count);
		WWWForm form = new WWWForm();
		form.AddField("viewer_id", VkontakteAPI.INSTANCE._viewerId);
		form.AddField("player_id", player_id);
		form.AddField("count", count);
		form.AddField("auth_key", VkontakteAPI.INSTANCE._authKey);
		if (App.instance.platform == App.ePlatform.ONDOKLASNIKI)
		{
			form.AddField("session_key", VkontakteAPI.INSTANCE._sessionKey);
		}
		Random r = new Random();
		form.AddField("random", r.Next());
		WWW php_load = new WWW(App.instance.GetCurScriptsUrl() + "send_money_to_player.php", form);
		yield return php_load;
		Debug.Log((object)php_load.text);
		if (php_load.text.Trim() == "SUCCESS")
		{
			ProfileINI.coins -= count;
			coins_to_send = "0";
			WorldGameObject.INSTANCE.SendMoneyToPlayer(send_money_player, int_coins);
		}
		else
		{
			send_money_error = "Ошибка при переводе, перевод не зачислен...";
		}
	}

	private void DrawWelcomeScreen()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)boxSkin != (Object)null)
		{
			GUI.skin = boxSkin;
		}
		GUI.Box(new Rect((float)(Screen.width / 2 - 220), (float)(Screen.height / 2 - 115), 440f, 230f), "Добро пожаловать в Копатель онлайн!");
		if ((Object)(object)guiSkin != (Object)null)
		{
			GUI.skin = guiSkin;
		}
		GUILayout.BeginArea(new Rect((float)(Screen.width / 2 - 200), (float)(Screen.height / 2 - 115), 400f, 230f));
		GUILayout.Space(40f);
		if (curWelcomWindow == WelcomeScreenWindow.WELCOME)
		{
			GUILayout.Label("Электронная почта", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
			welcome_email = GUILayout.TextField(welcome_email, new GUILayoutOption[0]);
			GUILayout.Label("Пароль", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
			welcome_password = GUILayout.PasswordField(welcome_password, "*"[0], 25, new GUILayoutOption[0]);
			welcome_remember = GUILayout.Toggle(welcome_remember, "Запомнить", new GUILayoutOption[0]);
			if (welcome_error.Length > 0)
			{
				GUILayout.Label(welcome_error, GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
			}
			if (welcome_error == "Такого сочетания почты и пароля не найденно" && GUILayout.Button("Забыли пароль?", new GUILayoutOption[0]))
			{
				curWelcomWindow = WelcomeScreenWindow.PASSWORD_RECOVER;
				password_recover_text = string.Empty;
			}
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (GUILayout.Button("Регистрация", new GUILayoutOption[1] { GUILayout.MaxWidth(150f) }))
			{
				curWelcomWindow = WelcomeScreenWindow.REGISTER;
				welcome_error = string.Empty;
				welcome_password = string.Empty;
				welcome_password_repeat = string.Empty;
				welcome_email = string.Empty;
			}
			if (!welcome_start_enter && (GUILayout.Button("Войти", new GUILayoutOption[0]) || bs._Igor.autojoin))
			{
				((MonoBehaviour)this).StartCoroutine(WebEnterGame(welcome_email, welcome_password));
			}
			GUILayout.EndHorizontal();
		}
		if (curWelcomWindow == WelcomeScreenWindow.PASSWORD_RECOVER)
		{
			GUILayout.Label("Введите почту на которую регистрировался аккаунт", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
			welcome_email = GUILayout.TextField(welcome_email, new GUILayoutOption[0]);
			if (password_recover_text.Length > 0)
			{
				GUILayout.Label(password_recover_text, GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
			}
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (GUILayout.Button("Назад", new GUILayoutOption[0]))
			{
				curWelcomWindow = WelcomeScreenWindow.WELCOME;
				welcome_error = string.Empty;
				welcome_password = string.Empty;
				welcome_password_repeat = string.Empty;
				welcome_email = string.Empty;
			}
			if (GUILayout.Button("Востановить пароль", new GUILayoutOption[1] { GUILayout.MaxWidth(200f) }))
			{
				((MonoBehaviour)this).StartCoroutine(WebPasswordRecover(welcome_email));
			}
			GUILayout.EndHorizontal();
		}
		if (curWelcomWindow == WelcomeScreenWindow.REGISTER)
		{
			GUILayout.Label("Электронная почта", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
			welcome_email = GUILayout.TextField(welcome_email, new GUILayoutOption[0]);
			GUILayout.Label("Пароль", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
			welcome_password = GUILayout.PasswordField(welcome_password, "*"[0], 25, new GUILayoutOption[0]);
			GUILayout.Label("Повторите пароль", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
			welcome_password_repeat = GUILayout.PasswordField(welcome_password_repeat, "*"[0], 25, new GUILayoutOption[0]);
			if (welcome_error.Length > 0)
			{
				GUILayout.Label(welcome_error, GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
			}
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (GUILayout.Button("Назад", new GUILayoutOption[0]))
			{
				curWelcomWindow = WelcomeScreenWindow.WELCOME;
				welcome_error = string.Empty;
				welcome_password = string.Empty;
				welcome_password_repeat = string.Empty;
				welcome_email = string.Empty;
			}
			if (GUILayout.Button("Зарегестрироваться", new GUILayoutOption[1] { GUILayout.MaxWidth(200f) }))
			{
				if (welcome_password != welcome_password_repeat)
				{
					welcome_error = "Пароли не совпадают";
				}
				else if (welcome_email.Length < 3)
				{
					welcome_error = "Заполните поле 'Электронная почта'";
				}
				else if (welcome_password.Length == 0)
				{
					welcome_error = "Заполните поле 'Пароль'";
				}
				else
				{
					((MonoBehaviour)this).StartCoroutine(WebRegister(welcome_email, welcome_password));
				}
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.EndArea();
	}

	private void WebCallPayStation(int count)
	{
		if (count != 0)
		{
			Application.ExternalCall("OpenUrl", new object[1] { "https://kopatel-online.ru/2012/api/save_map_to_slot.php?source=unity2012&v=" + VkontakteAPI.INSTANCE._viewerId + "&email=" + ProfileINI.email + "&out=" + count });
		}
		else
		{
			Application.ExternalCall("OpenUrl", new object[1] { "https://kopatel-online.ru/2012/api/save_map_to_slot.php?source=unity2012&v=" + VkontakteAPI.INSTANCE._viewerId + "&email=" + ProfileINI.email });
		}
	}

	private IEnumerator WebEnterGame(string email, string password)
	{
		welcome_start_enter = true;
		email = email.ToLower();
		password = password.ToLower();
		int packet_id = App.instance.random.Next();
		WWWForm form = new WWWForm();
		form.AddField("email", email);
		form.AddField("password", password);
		form.AddField("random", packet_id);
		WWW php_load = new WWW(App.instance.GetCurScriptsUrl() + "enter_game.php", form);
		yield return php_load;
		welcome_start_enter = false;
		Debug.Log((object)php_load.text);
		if (php_load.text == "LOGIN_FAILED")
		{
			welcome_error = "Такого сачетания почты и пароля не найденно";
			yield break;
		}
		string server_text = php_load.text.Substring(0, php_load.text.IndexOf('\n'));
		string server_key = php_load.text.Substring(php_load.text.IndexOf('\n') + 1);
		server_key.Trim();
		VkontakteAPI.MD52(server_text + "_" + packet_id + "_" + VkontakteAPI.INSTANCE.GetSecret());
		if (true)
		{
			VkontakteAPI.INSTANCE.SetViewerId(server_text.Split('|')[0]);
			VkontakteAPI.INSTANCE.SetAuthKey(server_text.Split('|')[1]);
			ProfileINI.email = email.Trim();
			if (welcome_remember)
			{
				ProfileINI.SaveLoginInfo();
			}
			else
			{
				ProfileINI.ClearLoginInfo();
			}
			curWindow = Windows.Loading;
			INSTANCE.loading_text = "Загрузка профайла...";
			((MonoBehaviour)this).StartCoroutine(App.instance.LoadProfile());
		}
	}

	private IEnumerator WebPasswordRecover(string email)
	{
		email = email.ToLower();
		WWWForm form = new WWWForm();
		form.AddField("email", email);
		WWW php_load = new WWW(App.instance.GetCurScriptsUrl() + "password_recover.php", form);
		yield return php_load;
		if (php_load.text.Trim() == "SEND")
		{
			password_recover_text = "Ваш пароль выслан на почту";
		}
		if (php_load.text.Trim() == "EMAIL_NOT_FOUND")
		{
			password_recover_text = "На данную почту не зарегестирован аккаунт";
		}
	}

	private IEnumerator WebRegister(string email, string password)
	{
		email = email.ToLower();
		password = password.ToLower();
		string uniq_id = SystemInfo.deviceUniqueIdentifier;
		int packet_id = App.instance.random.Next();
		WWWForm form = new WWWForm();
		form.AddField("email", email);
		form.AddField("password", password);
		form.AddField("uniq_id", uniq_id);
		form.AddField("check_hash", VkontakteAPI.MD52(uniq_id + "_" + packet_id + "_" + VkontakteAPI.INSTANCE.GetSecret()));
		form.AddField("random", packet_id);
		WWW php_load = new WWW(App.instance.GetCurScriptsUrl() + "register.php", form);
		yield return php_load;
		string server_text = php_load.text.Substring(0, php_load.text.IndexOf('\n'));
		string server_key = php_load.text.Substring(php_load.text.IndexOf('\n') + 1);
		server_key.Trim();
		VkontakteAPI.MD52(server_text + "_" + packet_id + "_" + VkontakteAPI.INSTANCE.GetSecret());
		if (true)
		{
			if (server_text == "REGISTERED")
			{
				curWelcomWindow = WelcomeScreenWindow.WELCOME;
				welcome_password = string.Empty;
				((MonoBehaviour)this).StartCoroutine(WebEnterGame(email, password));
			}
			if (server_text == "EMAIL_FAILED")
			{
				welcome_error = "Аккаунт на эту почту уже зарегестрирован";
			}
			if (server_text == "REG_FAILED")
			{
				welcome_error = "С вашего компьютера уже зарегестрирован аккаунт";
			}
		}
	}

	private void DrawRoomUrlMenu()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)boxSkin != (Object)null)
		{
			GUI.skin = boxSkin;
		}
		GUI.Box(new Rect((float)(Screen.width / 2 - 220), (float)(Screen.height / 2 - 50), 440f, 100f), "Ссылка");
		if ((Object)(object)guiSkin != (Object)null)
		{
			GUI.skin = guiSkin;
		}
		GUILayout.BeginArea(new Rect((float)(Screen.width / 2 - 200), (float)(Screen.height / 2 - 50), 400f, 100f));
		GUILayout.Space(40f);
		GUILayout.TextField("https://kopatel-online.ru/2012/api/" + PhotonNetwork.room.name.Replace(' ', '_'), new GUILayoutOption[0]);
		if (GUILayout.Button("Ок", new GUILayoutOption[0]))
		{
			curWindow = Windows.Game;
			showMenu = false;
		}
		GUILayout.EndArea();
	}

	private void DrawServerWasClosed()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)boxSkin != (Object)null)
		{
			GUI.skin = boxSkin;
		}
		GUI.Box(new Rect((float)(Screen.width / 2 - 220), (float)(Screen.height / 2 - 65), 440f, 130f), "Выход");
		if ((Object)(object)guiSkin != (Object)null)
		{
			GUI.skin = guiSkin;
		}
		GUILayout.BeginArea(new Rect((float)(Screen.width / 2 - 200), (float)(Screen.height / 2 - 65), 400f, 130f));
		GUILayout.Space(40f);
		GUILayout.Label("Сервер на котором вы играли был закрыт администратором", new GUILayoutOption[0]);
		if (GUILayout.Button("Ок", new GUILayoutOption[0]))
		{
			curWindow = Windows.Battles;
		}
		GUILayout.EndArea();
	}

	private void DrawExitMenu()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)boxSkin != (Object)null)
		{
			GUI.skin = boxSkin;
		}
		GUI.Box(new Rect((float)(Screen.width / 2 - 220), (float)(Screen.height / 2 - 65), 440f, 200f), "Выход");
		if ((Object)(object)guiSkin != (Object)null)
		{
			GUI.skin = guiSkin;
		}
		GUILayout.BeginArea(new Rect((float)(Screen.width / 2 - 200), (float)(Screen.height / 2 - 65), 400f, 200f));
		GUILayout.Space(35f);
		GUILayout.Label("Подсказка!", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		GUILayout.Label("Вы можете оставить сервер включенным на время вашего отсутсвия. Зажав TAB в игре, вы можете назначить модератора, который будет следить за сервером пока вас нет", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		if (GUILayout.Button("Cервер выключить", new GUILayoutOption[0]))
		{
			WorldGameObject.INSTANCE.CloseServerAndExit();
		}
		if (GUILayout.Button("Cервер пусть работает", new GUILayoutOption[0]))
		{
			WorldGameObject.INSTANCE.ExitGame();
		}
		if (GUILayout.Button("Вернутся в игру", new GUILayoutOption[0]))
		{
			WorldGameObject.INSTANCE.EnableMouseLook();
			Screen.lockCursor = true;
			showMenu = false;
		}
		GUILayout.EndArea();
	}

	private void DrawBanMenu()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)boxSkin != (Object)null)
		{
			GUI.skin = boxSkin;
		}
		GUI.Box(new Rect((float)(Screen.width / 2 - 220), (float)(Screen.height / 2 - 65), 440f, 130f), "Бан!");
		if ((Object)(object)guiSkin != (Object)null)
		{
			GUI.skin = guiSkin;
		}
		GUILayout.BeginArea(new Rect((float)(Screen.width / 2 - 200), (float)(Screen.height / 2 - 65), 400f, 130f));
		GUILayout.Space(40f);
		GUILayout.Label("Вы получили бан на этом сервере", new GUILayoutOption[0]);
		if (GUILayout.Button("Ок", new GUILayoutOption[0]))
		{
			curWindow = Windows.Battles;
		}
		GUILayout.EndArea();
	}

	private void DrawDeathMenu()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)boxSkin != (Object)null)
		{
			GUI.skin = boxSkin;
		}
		GUI.Box(new Rect((float)(Screen.width / 2 - 220), (float)(Screen.height / 2 - 115), 440f, 230f), "Смерть!");
		if ((Object)(object)guiSkin != (Object)null)
		{
			GUI.skin = guiSkin;
		}
		GUILayout.BeginArea(new Rect((float)(Screen.width / 2 - 200), (float)(Screen.height / 2 - 115), 400f, 230f));
		GUILayout.Space(40f);
		GUILayout.Label("Вы сгорели в лаве!!!", new GUILayoutOption[0]);
		GUILayout.Label("В следующий раз будьте внимательны и не наступайте на лавовые блоки.", new GUILayoutOption[0]);
		if (GUILayout.Button("Возродится", new GUILayoutOption[0]))
		{
			CleanScreenMode(mode: false);
			WorldGameObject.INSTANCE.Respawn();
		}
		GUILayout.EndArea();
	}

	private void DrawInGameMenu()
	{
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		bool flag = Level.Instance.IsAdministrator();
		int num = (((!App.instance.gameIni.isWatch && !GameTypeUtils.BuildDisabled()) ? 1 : 0) + (((flag && !App.instance.gameIni.isWatch && GameTypeUtils.CanSaveMap()) || App.instance.gameIni.isWatch) ? 1 : 0)) * 27;
		if ((Object)(object)boxSkin != (Object)null)
		{
			GUI.skin = boxSkin;
		}
		GUI.Box(new Rect((float)(Screen.width / 2 - 120), (float)(Screen.height / 2 - 100), 280f, (float)(170 + num)), "Меню");
		if ((Object)(object)guiSkin != (Object)null)
		{
			GUI.skin = guiSkin;
		}
		GUILayout.BeginArea(new Rect((float)(Screen.width / 2 - 110), (float)(Screen.height / 2 - 130), 260f, (float)(210 + num)));
		GUILayout.Space(70f);
		if (GUILayout.Button("Продолжить", new GUILayoutOption[0]))
		{
			WorldGameObject.INSTANCE.EnableMouseLook();
			Screen.lockCursor = true;
			showMenu = false;
		}
		if (bs._Igor.enableGuns && GUILayout.Button("Выбор Оружия", new GUILayoutOption[0]))
		{
			ShowWeaponWindow();
		}
		if (!bs._Igor.enableGuns && GUILayout.Button("Возродиться [R]", new GUILayoutOption[0]))
		{
			WorldGameObject.INSTANCE.Respawn();
		}
		if (GUILayout.Button("Эмоции [Q]", new GUILayoutOption[0]))
		{
			SpeechBubbles.ShowMenu = true;
		}
		if (!App.instance.gameIni.isWatch && !GameTypeUtils.BuildDisabled() && GUILayout.Button("Инвентарь [С]", new GUILayoutOption[0]))
		{
			showMenu = false;
			WorldGameObject.INSTANCE.PressCubeMenu();
		}
		if (flag && !App.instance.gameIni.isWatch && GameTypeUtils.CanSaveMap())
		{
			if (GUILayout.Button("Сохранить [F10]", new GUILayoutOption[0]))
			{
				OpenSaveMenu();
			}
		}
		else if (App.instance.gameIni.isWatch && GUILayout.Button("Оценить [TAB]", new GUILayoutOption[0]))
		{
			curWindow = Windows.MapCard;
		}
		if (GUILayout.Button("Помощь", new GUILayoutOption[0]))
		{
			curWindow = Windows.TutorialImages;
			curent_page = 0;
		}
		if (GUILayout.Button("Выйти...", new GUILayoutOption[0]))
		{
			if (flag && PhotonNetwork.room.playerCount > 1 && !App.instance.gameIni.isWatch)
			{
				curWindow = Windows.ExitMenu;
			}
			else
			{
				WorldGameObject.INSTANCE.ExitGame();
			}
		}
		GUILayout.EndArea();
	}

	public void OpenMobsMenu()
	{
		showMenu = true;
		curWindow = Windows.GameStore;
		curStoreLeftWindow = StoreLeftWindow.MOBS;
	}

	public void OpenEmotionsMenu()
	{
		showMenu = true;
		curWindow = Windows.GameStore;
		curStoreLeftWindow = StoreLeftWindow.INSTRUMENTS;
		_CurrentPurchase = StorePurchase.EMOTIONS;
	}

	public void OpenMEMEmotionsMenu()
	{
		showMenu = true;
		curWindow = Windows.GameStore;
		curStoreLeftWindow = StoreLeftWindow.INSTRUMENTS;
		_CurrentPurchase = StorePurchase.MEM_SMILES;
	}

	public void OpenMebelMenu()
	{
		showMenu = true;
		curWindow = Windows.GameStore;
		curStoreLeftWindow = StoreLeftWindow.MEBEL;
	}

	public void OpenPlantsMenu()
	{
		showMenu = true;
		curWindow = Windows.GameStore;
		curStoreLeftWindow = StoreLeftWindow.PLANTS;
	}

	public void OpenSaveMenu()
	{
		showMenu = true;
		mapWindows = MapWindows.MY_MAPS;
		_SelectedMapID = -3;
		curWindow = Windows.GameSave;
	}

	private void FixMenuControl(int _cur)
	{
		if (_cur >= 500)
		{
			if ((_cur == 0 && curWindow == Windows.Battles) || (_cur == 1 && curWindow == Windows.Store) || (_cur == 2 && curWindow == Windows.Saves) || (_cur == 3 && curWindow == Windows.Options) || (_cur == 4 && curStoreLeftWindow == StoreLeftWindow.BLOCKS) || (_cur == 5 && curStoreLeftWindow == StoreLeftWindow.MEBEL) || (_cur == 6 && curStoreLeftWindow == StoreLeftWindow.PLANTS) || (_cur == 7 && curStoreLeftWindow == StoreLeftWindow.OTHER) || (_cur == 8 && curStoreLeftWindow == StoreLeftWindow.INSTRUMENTS) || (_cur == 9 && curStoreLeftWindow == StoreLeftWindow.SKINS) || (_cur == 10 && curStoreLeftWindow == StoreLeftWindow.MAPS) || (_cur == 11 && (mapWindows == MapWindows.BEST_MAPS_DAY || mapWindows == MapWindows.BEST_MAPS_WEEK || mapWindows == MapWindows.BEST_MAPS_MONTH)) || (_cur == 12 && mapWindows == MapWindows.NEW_MAPS) || (_cur == 13 && (mapWindows == MapWindows.SEARCH_MAPS || mapWindows == MapWindows.SEARCH_AUTHOR || mapWindows == MapWindows.SEARCH_NAME || mapWindows == MapWindows.SEARCH_RESULT)) || (_cur == 14 && mapWindows == MapWindows.BEST_MAPS_DAY) || (_cur == 15 && mapWindows == MapWindows.BEST_MAPS_WEEK) || (_cur == 16 && mapWindows == MapWindows.BEST_MAPS_MONTH))
			{
				GUIStyleState normal = GUI.skin.button.normal;
				Texture2D background = button_textures[2];
				GUI.skin.button.active.background = background;
				normal.background = background;
				GUIStyleState hover = GUI.skin.button.hover;
				background = button_textures[3];
				GUI.skin.button.onNormal.background = background;
				background = background;
				GUI.skin.button.onHover.background = background;
				background = background;
				GUI.skin.button.onFocused.background = background;
				background = background;
				GUI.skin.button.onActive.background = background;
				hover.background = background;
			}
			else
			{
				GUIStyleState normal2 = GUI.skin.button.normal;
				Texture2D background = button_textures[0];
				GUI.skin.button.active.background = background;
				normal2.background = background;
				GUIStyleState hover2 = GUI.skin.button.hover;
				background = button_textures[1];
				GUI.skin.button.onNormal.background = background;
				background = background;
				GUI.skin.button.onHover.background = background;
				background = background;
				GUI.skin.button.onFocused.background = background;
				background = background;
				GUI.skin.button.onActive.background = background;
				hover2.background = background;
			}
		}
	}

	private void DrawTopMenu()
	{
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		FixMenuControl(0);
		if (GUILayout.Button("Играть", new GUILayoutOption[0]))
		{
			_BattleExistsShowInfo = false;
			by_warning = string.Empty;
			_NewGameWarning = string.Empty;
			ProfileINI.Save();
			curWindow = Windows.Battles;
			battlesLeftMenu = BatllesLeftMenu.MAIN;
			battlesRightMenu = BatllesRightMenu.NEWS;
			_FastConnectWarning = string.Empty;
		}
		FixMenuControl(1);
		if (GUILayout.Button("$ Магазин $", new GUILayoutOption[0]))
		{
			by_warning = string.Empty;
			ProfileINI.Save();
			curWindow = Windows.Store;
			curStoreLeftWindow = StoreLeftWindow.BLOCKS;
			_CurrentPurchase = StorePurchase.NONE;
		}
		FixMenuControl(2);
		if (GUILayout.Button("Топ карт", new GUILayoutOption[0]))
		{
			by_warning = string.Empty;
			ProfileINI.Save();
			curWindow = Windows.Saves;
			mapWindows = MapWindows.BEST_MAPS_DAY;
			_SelectedMapID = -2;
			cur_map_page = 0;
			((MonoBehaviour)this).StartCoroutine(RefreshListOfDay(cur_map_page));
		}
		FixMenuControl(3);
		if (GUILayout.Button("Настройки", new GUILayoutOption[0]))
		{
			by_warning = string.Empty;
			ProfileINI.Save();
			curWindow = Windows.Options;
		}
		FixMenuControl(10000);
		GUILayout.Space(40f);
		DrawMoney(draw_btn: true);
		GUILayout.EndHorizontal();
	}

	private void toggleFullScreen()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (Screen.fullScreen)
		{
			Screen.SetResolution(800, 600, false);
			return;
		}
		Resolution currentResolution = Screen.currentResolution;
		int width = currentResolution.width;
		Resolution currentResolution2 = Screen.currentResolution;
		Screen.SetResolution(width, currentResolution2.height, true);
	}

	public void DrawMoney(bool draw_btn)
	{
		GUILayout.Label((Texture)(object)coinsImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[0]);
		GUILayout.Label(ProfileINI.coins.ToString(), GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[0]);
		GUILayout.Label((Texture)(object)crystalImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[0]);
		GUILayout.Label(ProfileINI.crystals.ToString(), GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[0]);
		if (draw_btn && GUILayout.Button("купить", new GUILayoutOption[0]))
		{
			by_warning = string.Empty;
			lastWindow = curWindow;
			curWindow = Windows.ByCrystal;
		}
	}

	private void DrawBattlesExistsMenu()
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		RoomInfo roomInfo = null;
		RoomInfo[] roomList = PhotonNetwork.GetRoomList();
		foreach (RoomInfo roomInfo2 in roomList)
		{
			if ((string)roomInfo2.customProperties["map_name"] == _BattlesMapName)
			{
				roomInfo = roomInfo2;
				break;
			}
		}
		if (roomInfo == null)
		{
			return;
		}
		_BattlesScroll = GUILayout.BeginScrollView(_BattlesScroll, new GUILayoutOption[0]);
		if (curWindow == Windows.Saves)
		{
			GUILayout.Space(10f);
			GUILayout.Label("Карта в сети!", new GUILayoutOption[0]);
		}
		else
		{
			GUILayout.Space(30f);
		}
		if (roomInfo.customProperties.ContainsKey("map_name"))
		{
			GUILayout.Label((string)roomInfo.customProperties["map_name"], GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		}
		if (roomInfo.customProperties.ContainsKey("game_type"))
		{
			GUILayout.Label("Тип игры: " + _GameTypes[(int)roomInfo.customProperties["game_type"]], GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		}
		if (roomInfo.customProperties.ContainsKey("map_about"))
		{
			GUILayout.Label((string)roomInfo.customProperties["map_about"], GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		}
		if (roomInfo.customProperties.ContainsKey("rating"))
		{
			GUILayout.Label("Рейтинг " + (byte)roomInfo.customProperties["rating"], GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		}
		int playerCount = roomInfo.playerCount;
		int maxPlayers = roomInfo.maxPlayers;
		GUILayout.Label("Играют: " + playerCount + " из " + maxPlayers, GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		string text = string.Empty;
		if (roomInfo.customProperties.ContainsKey("password"))
		{
			text = (string)roomInfo.customProperties["password"];
		}
		if (text.Length > 0)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label("Пароль: ", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
			_ConnectPassword = GUILayout.TextField(_ConnectPassword, new GUILayoutOption[1] { GUILayout.MaxWidth(150f) });
			GUILayout.EndHorizontal();
		}
		GUILayout.Label(_NewGameWarning, GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		if (playerCount < maxPlayers)
		{
			if (GUILayout.Button("Зайти в игру", new GUILayoutOption[0]))
			{
				if (text.Length == 0 || text == _ConnectPassword)
				{
					JoinGame(roomInfo);
				}
				else
				{
					_NewGameWarning = "Пароль не верный";
				}
			}
		}
		else
		{
			GUILayout.Label("Сервер полный", new GUILayoutOption[0]);
		}
		if (roomInfo.customProperties.ContainsKey("public_id") && (int)roomInfo.customProperties["public_id"] > 1)
		{
			if (curWindow == Windows.Saves)
			{
				_BattleExistsShowInfo = true;
			}
			if (_BattleExistsShowInfo)
			{
				if (curWindow != Windows.Saves && GUILayout.Button("Скрыть информацию", new GUILayoutOption[0]))
				{
					_BattleExistsShowInfo = false;
				}
				if (curWindow == Windows.Saves)
				{
					GUILayout.Space(10f);
				}
				else
				{
					GUILayout.Space(30f);
				}
				if (_SelectedMapID == -2)
				{
					GUILayout.Label("Загружаю информацию...", new GUILayoutOption[0]);
				}
				if (_SelectedMapID > 1)
				{
					GUILayout.Label("Автор карты : " + public_map_authorname, GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
					GUILayout.Label("Очки      : " + _PublicMapRating, GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
					GUILayout.Label(string.Empty, GUIStyle.op_Implicit("Divider"), new GUILayoutOption[0]);
					GUILayoutOption[] array = new GUILayoutOption[4]
					{
						GUILayout.MinWidth(308f),
						GUILayout.MaxWidth(308f),
						GUILayout.MinHeight(136f),
						GUILayout.MaxHeight(136f)
					};
					if ((Object)(object)player_map_img != (Object)null)
					{
						GUILayout.Label((Texture)(object)player_map_img, array);
					}
					else
					{
						GUILayout.Label((Texture)(object)standart_map_img, array);
					}
					GUILayout.Label(string.Empty, GUIStyle.op_Implicit("Divider"), new GUILayoutOption[0]);
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
					GUILayout.Label("Название карты:", new GUILayoutOption[0]);
					GUILayout.TextField(public_map_name, new GUILayoutOption[1] { GUILayout.MaxWidth(200f) });
					GUILayout.EndHorizontal();
					if (App.instance.platform == App.ePlatform.VKONTATE)
					{
						GUILayout.BeginHorizontal(new GUILayoutOption[0]);
						GUILayout.Label("Ссылка на карту:", new GUILayoutOption[0]);
						GUILayout.TextField("http://vkontakte.ru/app2721929#map_id_" + _SelectedMapID, new GUILayoutOption[1] { GUILayout.MaxWidth(200f) });
						GUILayout.EndHorizontal();
					}
					GUILayout.Label("Описание карты:", new GUILayoutOption[0]);
					GUILayout.TextArea(public_map_about, new GUILayoutOption[1] { GUILayout.MaxHeight(100f) });
					GUILayout.Label(string.Empty, GUIStyle.op_Implicit("Divider"), new GUILayoutOption[0]);
					GUILayout.Label("Коментарии :", new GUILayoutOption[0]);
					for (int j = 0; j < Comments.Count; j++)
					{
						GUILayout.Label((Comments[j] as Comment).name, GUIStyle.op_Implicit("CommentName"), new GUILayoutOption[0]);
						GUILayout.BeginHorizontal(new GUILayoutOption[0]);
						GUILayout.Space(20f);
						GUILayout.Label((Comments[j] as Comment).text, new GUILayoutOption[0]);
						GUILayout.EndHorizontal();
					}
					if (count_comments > 1)
					{
						GUILayout.BeginHorizontal(new GUILayoutOption[0]);
						if (comments_start_ind <= 0)
						{
							GUI.enabled = false;
						}
						if (GUILayout.Button(" <<< ", new GUILayoutOption[0]))
						{
							((MonoBehaviour)this).StartCoroutine(GetComments(_SelectedMapID, 0));
						}
						if (GUILayout.Button(" < ", new GUILayoutOption[0]))
						{
							((MonoBehaviour)this).StartCoroutine(GetComments(_SelectedMapID, comments_start_ind - 1));
						}
						GUI.enabled = true;
						if (count_comments > 0)
						{
							GUILayout.Label("  " + (comments_start_ind + 1) + "/" + count_comments + "  ", new GUILayoutOption[0]);
						}
						else
						{
							GUILayout.Label("  " + (comments_start_ind + 1) + "/" + (count_comments + 1) + "  ", new GUILayoutOption[0]);
						}
						if (comments_start_ind + 1 >= count_comments)
						{
							GUI.enabled = false;
						}
						if (GUILayout.Button(" > ", new GUILayoutOption[0]))
						{
							((MonoBehaviour)this).StartCoroutine(GetComments(_SelectedMapID, comments_start_ind + 1));
						}
						if (GUILayout.Button(" >>> ", new GUILayoutOption[0]))
						{
							((MonoBehaviour)this).StartCoroutine(GetComments(_SelectedMapID, count_comments - 1));
						}
						GUI.enabled = true;
						GUILayout.EndHorizontal();
					}
					if (draw_comment_form)
					{
						GUILayout.Label("Текст комментария:", new GUILayoutOption[0]);
						post_text = GUILayout.TextArea(post_text, new GUILayoutOption[1] { GUILayout.MaxHeight(100f) });
						if (GUILayout.Button("Отправить", new GUILayoutOption[0]) && post_text.Length > 3)
						{
							((MonoBehaviour)this).StartCoroutine(PostComment(post_text, _SelectedMapID));
							post_text = string.Empty;
							draw_comment_form = false;
						}
					}
					else if (GUILayout.Button("Коментировать", new GUILayoutOption[0]))
					{
						draw_comment_form = true;
					}
				}
			}
			else if (GUILayout.Button("Информация", new GUILayoutOption[0]))
			{
				_BattleExistsShowInfo = true;
			}
		}
		GUILayout.EndScrollView();
	}

	public IEnumerator MoveToGameScene()
	{
		while (PhotonNetwork.room == null)
		{
			yield return 0;
		}
		PhotonNetwork.isMessageQueueRunning = false;
		loading_text = "Загрузка уровня...";
		Application.LoadLevel("Game");
	}

	private void OnMasterClientSwitched(PhotonPlayer player)
	{
	}

	private void OnPhotonCreateRoomFailed()
	{
		Debug.Log((object)"OnPhotonCreateGameFailed");
	}

	private void OnJoinedLobby()
	{
		Debug.Log((object)"OnJoinedLobby");
	}

	private void OnLeftLobby()
	{
		Debug.Log((object)"OnLeftLobby");
	}

	private void OnCreatedRoom()
	{
		Debug.Log((object)"OnCreatedRoom");
	}

	private void OnReceivedRoomList()
	{
		all_player_count = PhotonNetwork.countOfPlayersInRooms;
		if (App.instance.waiting_room_recive)
		{
			App.instance.waiting_room_recive = false;
			App.instance.CheckDirectLink();
		}
		_FastConnectWarning = string.Empty;
	}

	private void OnReceivedRoomListUpdate()
	{
		all_player_count = PhotonNetwork.countOfPlayers;
		_FastConnectWarning = string.Empty;
	}

	private void OnPhotonPlayerDisconnected(PhotonPlayer player)
	{
		if (PhotonNetwork.isMasterClient)
		{
			WorldGameObject.INSTANCE.OnPhotonPlayerDisconneced(player);
		}
	}

	private void DrawBattlesSavedMenu()
	{
		GUILayout.Space(30f);
		GUILayout.Label("Карта : " + _SelectedSlot, new GUILayoutOption[0]);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("Игровой режим: ", new GUILayoutOption[0]);
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		GUILayout.Label(_GameTypes[_GameType], new GUILayoutOption[0]);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		if (GUILayout.Button("<", GUIStyle.op_Implicit("ShortButton"), new GUILayoutOption[0]))
		{
			_GameType = Mathf.Clamp(--_GameType, 0, _GameTypes.Length - 1);
		}
		if (GUILayout.Button(">", GUIStyle.op_Implicit("ShortButton"), new GUILayoutOption[0]))
		{
			_GameType = Mathf.Clamp(++_GameType, 0, _GameTypes.Length - 1);
		}
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		if (_GameType > 0)
		{
			destroyable = GUILayout.Toggle(destroyable, "Разрушаемый ландшафт", new GUILayoutOption[0]);
		}
		else
		{
			destroyable = true;
		}
		if (_GameType > 0)
		{
			gifts = GUILayout.Toggle(gifts, "Падающие бонусы", new GUILayoutOption[0]);
		}
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("Название: ", new GUILayoutOption[0]);
		ProfileINI.server_name = GUILayout.TextField(ProfileINI.server_name, new GUILayoutOption[1] { GUILayout.MaxWidth(150f) });
		ProfileINI.server_name = Utils.MaxStringLength(ProfileINI.server_name, 18);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("Описание: ", new GUILayoutOption[0]);
		ProfileINI.server_about = GUILayout.TextArea(ProfileINI.server_about, new GUILayoutOption[1] { GUILayout.MaxWidth(150f) });
		ProfileINI.server_about = Utils.MaxStringLength(ProfileINI.server_about, 250);
		GUILayout.EndHorizontal();
		GUILayout.Space(20f);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("Пароль (если надо): ", new GUILayoutOption[0]);
		Password = GUILayout.TextField(Password, new GUILayoutOption[1] { GUILayout.MaxWidth(150f) });
		GUILayout.EndHorizontal();
		GUILayout.Label(_NewGameWarning, GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		if (!GUILayout.Button("Загрузить", new GUILayoutOption[0]))
		{
			return;
		}
		if (ProfileINI.server_name.Length < 3)
		{
			_NewGameWarning = "Название комнаты должно быть больше 3 букв!";
			return;
		}
		bool flag = false;
		RoomInfo[] roomList = PhotonNetwork.GetRoomList();
		foreach (RoomInfo roomInfo in roomList)
		{
			if ((string)roomInfo.customProperties["map_name"] == ProfileINI.server_name && (!((string)roomInfo.customProperties["player_id"] == VkontakteAPI.INSTANCE._viewerId) || (int)roomInfo.customProperties["slot_id"] != _SelectedSlot))
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			_NewGameWarning = string.Empty;
			JoinOrCreateGame(VkontakteAPI.INSTANCE._viewerId, _SelectedSlot, isWatch: false);
		}
		else
		{
			_NewGameWarning = "Комната с таким названием существует.";
		}
	}

	private void InitializeGameINI()
	{
		GameINI gameINI = (App.instance.gameIni = new GameINI());
		gameINI.playerID = string.Empty;
		gameINI.slotID = 0;
		gameINI.publicMapID = _SelectedMapID;
		gameINI.gameType = (GameINI.GameType)_GameType;
		gameINI.gifts = gifts;
		gameINI.destroyable = destroyable;
		gameINI.mapSize = GameINI.MapSize.SMALL;
		gameINI.mapPopulation = byte.Parse(MapPopulation);
		gameINI.isOnline = true;
		gameINI.isServer = false;
		gameINI.isWatch = false;
		gameINI.loadingSavedMap = true;
		gameINI.Password = Password;
	}

	private void JoinGame(RoomInfo room)
	{
		Debug.Log((object)"JoinGame");
		InitializeGameINI();
		App.instance.gameIni.playerID = (string)room.customProperties["player_id"];
		App.instance.gameIni.slotID = (int)room.customProperties["slot_id"];
		App.instance.gameIni.isWatch = (bool)room.customProperties["is_watch"];
		App.instance.gameIni.server_name = (string)room.customProperties["map_name"];
		App.instance.gameIni.server_about = (string)room.customProperties["map_about"];
		PhotonNetwork.playerName = ProfileINI.nickname;
		PhotonNetwork.JoinRoom(room);
		curWindow = Windows.Loading;
		loading_text = "Попытка присоединиться...";
	}

	private void OnPhotonJoinRoomFailed()
	{
		Debug.Log((object)"OnPhotonJoinRoomFailed");
		curWindow = Windows.Battles;
	}

	private void JoinOrCreateGame(string playerID, int slotID, bool isWatch)
	{
		Debug.Log((object)"JoinOrCreateGame");
		InitializeGameINI();
		App.instance.gameIni.server_name = ProfileINI.server_name;
		App.instance.gameIni.server_about = ProfileINI.server_about;
		App.instance.gameIni.playerID = playerID;
		App.instance.gameIni.slotID = slotID;
		App.instance.gameIni.isWatch = isWatch;
		PhotonNetwork.playerName = ProfileINI.nickname;
		App.instance.gameIni.isServer = true;
		Application.LoadLevel("Game");
	}

	private void OnPhotonRandomJoinFailed()
	{
		Debug.Log((object)"OnPhotonRandomJoinFailed");
		App.instance.gameIni.isServer = true;
		Application.LoadLevel("Game");
		curWindow = Windows.Loading;
		loading_text = "Загрузка уровня...";
	}

	private void OnJoinedRoom()
	{
		Debug.Log((object)"OnJoinedRoom");
		if ((string)PhotonNetwork.room.customProperties["player_id"] == VkontakteAPI.INSTANCE._viewerId)
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("map_name", App.instance.gameIni.server_name);
			hashtable.Add("map_about", App.instance.gameIni.server_about);
			hashtable.Add("password", App.instance.gameIni.Password);
			PhotonNetwork.room.SetCustomProperties(hashtable);
		}
		if (!App.instance.gameIni.isServer)
		{
			ProfileINI.server_about = (string)PhotonNetwork.room.customProperties["map_about"];
			((MonoBehaviour)this).StartCoroutine(MoveToGameScene());
		}
	}

	private void DrawNews()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		GUILayoutOption[] array = new GUILayoutOption[2]
		{
			GUILayout.MaxWidth(90f),
			GUILayout.MaxHeight(90f)
		};
		news_scroll = GUILayout.BeginScrollView(news_scroll, new GUILayoutOption[0]);
		GUILayout.Space(35f);
		if (App.instance.platform == App.ePlatform.VKONTATE && news_text.Length > 3)
		{
			GUILayout.Label(news_text, GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
			if (GUILayout.Button((Texture)(object)news_img, GUI.skin.label, new GUILayoutOption[0]))
			{
				Application.ExternalCall("OpenUrl", new object[1] { news_url });
			}
		}
		GUILayout.Label(string.Empty, GUIStyle.op_Implicit("Divider"), new GUILayoutOption[0]);
		GUILayout.Label("Обновление", GUIStyle.op_Implicit("CommentName"), new GUILayoutOption[0]);
		GUILayout.Label("- Переработаны старые декорации и добавлено некоторое количество новых!", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		GUILayout.Label("- На стульях можно сидеть!", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		GUILayout.Label("- На кроватях можно лежать!", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		GUILayout.Label("- Модераторы на карте теперь могут переносить декорации", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		if (GUILayout.Button("Посмотреть новые декорации...", new GUILayoutOption[0]))
		{
			by_warning = string.Empty;
			curWindow = Windows.Store;
			curStoreLeftWindow = StoreLeftWindow.MEBEL;
		}
		GUILayout.Label(string.Empty, GUIStyle.op_Implicit("Divider"), new GUILayoutOption[0]);
		GUILayout.Label("Обновление", GUIStyle.op_Implicit("CommentName"), new GUILayoutOption[0]);
		GUILayout.Label("- Новый скин Сталкер!", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		GUILayout.Label("- Книги", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		GUILayout.Label("- Возможность назначать модераторов на карту", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		if (GUILayout.Button((Texture)(object)storeIcons[73], GUIStyle.op_Implicit("StoreBtn"), array))
		{
			by_warning = string.Empty;
			_CurrentPurchase = StorePurchase.SOLDIER;
			curWindow = Windows.Store;
			curStoreLeftWindow = StoreLeftWindow.SKINS;
			int num = 73;
			preview_obj.GetComponent<SkinManager>().SetSkin(clotheInd[num]);
		}
		if (GUILayout.Button((Texture)(object)storeIcons[74], GUIStyle.op_Implicit("StoreBtn"), array))
		{
			by_warning = string.Empty;
			_CurrentPurchase = StorePurchase.BOOK;
			curWindow = Windows.Store;
			curStoreLeftWindow = StoreLeftWindow.INSTRUMENTS;
		}
		GUILayout.EndHorizontal();
		if (GUILayout.Button("Подробней об обновлении", new GUILayoutOption[0]))
		{
			Application.ExternalCall("OpenUrl", new object[1] { "https://kopatel-online.ru/2012/api/worlds/" });
		}
		GUILayout.Label(string.Empty, GUIStyle.op_Implicit("Divider"), new GUILayoutOption[0]);
		GUILayout.Label("Обновление", GUIStyle.op_Implicit("CommentName"), new GUILayoutOption[0]);
		GUILayout.Label("- Добавленно 3 новых животных, собака, кошка и курица!", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		GUILayout.Label("- Способность позволяющая менять день ночь на карте", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		GUILayout.Label("- Лава теперь светится в темноте!", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		GUILayout.Label("- Новый скин, Рыцарь смерти!", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		GUILayout.Label("- Добавлены мем смайлики", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		if (GUILayout.Button((Texture)(object)storeIcons[68], GUIStyle.op_Implicit("StoreBtn"), array))
		{
			by_warning = string.Empty;
			_CurrentPurchase = StorePurchase.CAT;
			curWindow = Windows.Store;
			curStoreLeftWindow = StoreLeftWindow.MOBS;
			if ((Object)(object)mobs_obj != (Object)null)
			{
				mobs_obj.GetComponent<SkinManager>().SetSkin(3);
			}
		}
		if (GUILayout.Button((Texture)(object)storeIcons[69], GUIStyle.op_Implicit("StoreBtn"), array))
		{
			by_warning = string.Empty;
			_CurrentPurchase = StorePurchase.DOG;
			curWindow = Windows.Store;
			curStoreLeftWindow = StoreLeftWindow.MOBS;
			if ((Object)(object)mobs_obj != (Object)null)
			{
				mobs_obj.GetComponent<SkinManager>().SetSkin(4);
			}
		}
		if (GUILayout.Button((Texture)(object)storeIcons[70], GUIStyle.op_Implicit("StoreBtn"), array))
		{
			by_warning = string.Empty;
			_CurrentPurchase = StorePurchase.CHICKEN;
			curWindow = Windows.Store;
			curStoreLeftWindow = StoreLeftWindow.MOBS;
			if ((Object)(object)mobs_obj != (Object)null)
			{
				mobs_obj.GetComponent<SkinManager>().SetSkin(5);
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		if (GUILayout.Button((Texture)(object)storeIcons[71], GUIStyle.op_Implicit("StoreBtn"), array))
		{
			by_warning = string.Empty;
			_CurrentPurchase = StorePurchase.DEATH_KNIGHT_SKIN;
			curWindow = Windows.Store;
			curStoreLeftWindow = StoreLeftWindow.SKINS;
			int num2 = 71;
			preview_obj.GetComponent<SkinManager>().SetSkin(clotheInd[num2]);
		}
		if (GUILayout.Button((Texture)(object)storeIcons[67], GUIStyle.op_Implicit("StoreBtn"), array))
		{
			by_warning = string.Empty;
			_CurrentPurchase = StorePurchase.TIME_OF_DAY;
			curWindow = Windows.Store;
			curStoreLeftWindow = StoreLeftWindow.INSTRUMENTS;
		}
		if (GUILayout.Button((Texture)(object)storeIcons[72], GUIStyle.op_Implicit("StoreBtn"), array))
		{
			by_warning = string.Empty;
			_CurrentPurchase = StorePurchase.MEM_SMILES;
			curWindow = Windows.Store;
			curStoreLeftWindow = StoreLeftWindow.INSTRUMENTS;
		}
		GUILayout.EndHorizontal();
		if (GUILayout.Button("Видео обзор обновления...", new GUILayoutOption[0]))
		{
			Application.ExternalCall("OpenUrl", new object[1] { "http://www.youtube.com/watch?v=Wv_0mbWSUlQ" });
		}
		GUILayout.Label(string.Empty, GUIStyle.op_Implicit("Divider"), new GUILayoutOption[0]);
		GUILayout.Label("Новости", GUIStyle.op_Implicit("CommentName"), new GUILayoutOption[0]);
		GUILayout.Label("- Напоминаем что у игры есть группа в которой происходит много интересного!", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		GUILayout.Label("- Например сейчас проходит конкурс на лучшую американскую горку", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		GUILayout.Label("- Также в группе выходят отличные видео обзоры топовых карт", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		if (GUILayout.Button("О конкурсе...", new GUILayoutOption[0]))
		{
			Application.ExternalCall("OpenUrl", new object[1] { "http://vk.com/kopatel_official?w=wall-32017129_24960" });
		}
		if (GUILayout.Button("Видео обзор топовых карт", new GUILayoutOption[0]))
		{
			Application.ExternalCall("OpenUrl", new object[1] { "http://youtu.be/G_JyWuax4go" });
		}
		GUILayout.Label(string.Empty, GUIStyle.op_Implicit("Divider"), new GUILayoutOption[0]);
		GUILayout.Label("Обновление", GUIStyle.op_Implicit("CommentName"), new GUILayoutOption[0]);
		GUILayout.Label("- В игру добавлены рельсы и вагонетки!", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		if (GUILayout.Button((Texture)(object)storeIcons[65], GUIStyle.op_Implicit("StoreBtn"), array))
		{
			by_warning = string.Empty;
			_CurrentPurchase = StorePurchase.RAIL;
			curWindow = Windows.Store;
			curStoreLeftWindow = StoreLeftWindow.INSTRUMENTS;
		}
		if (GUILayout.Button((Texture)(object)storeIcons[66], GUIStyle.op_Implicit("StoreBtn"), array))
		{
			by_warning = string.Empty;
			_CurrentPurchase = StorePurchase.TROLLEY;
			curWindow = Windows.Store;
			curStoreLeftWindow = StoreLeftWindow.INSTRUMENTS;
		}
		GUILayout.EndHorizontal();
		GUILayout.Label(string.Empty, GUIStyle.op_Implicit("Divider"), new GUILayoutOption[0]);
		GUILayout.Label("Обновление", GUIStyle.op_Implicit("CommentName"), new GUILayoutOption[0]);
		GUILayout.Label("- Тексты которые пишут игроки, теперь появляется над ними!", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		GUILayout.Label("- В настройках можно включить/выключить их отображение", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		GUILayout.Label("- Добавлены эмоции! В игре доступны через кнопку Q, либо в меню на ESC", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		GUILayout.Label("- Добавлен расширенный пак на +12 эмоций", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		GUILayout.Label("- Увеличен лимит предметов на карте в 2 раза", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		GUILayout.Label("- Добавлена анимация смерти", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		if (GUILayout.Button((Texture)(object)storeIcons[64], GUIStyle.op_Implicit("StoreBtn"), array))
		{
			by_warning = string.Empty;
			_CurrentPurchase = StorePurchase.EMOTIONS;
			curWindow = Windows.Store;
			curStoreLeftWindow = StoreLeftWindow.OTHER;
		}
		GUILayout.EndScrollView();
	}

	private void DrawBattlesNewMenu()
	{
		GUILayout.Space(30f);
		if (ProfileINI.GetPurchesValue(StorePurchase.MEDIUM_MAP) > 0)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label("Размер карты:", new GUILayoutOption[1] { GUILayout.MinWidth(100f) });
			GUILayout.Label(map_size_names[map_size_select_id], new GUILayoutOption[0]);
			if (GUILayout.Button("<", GUIStyle.op_Implicit("ShorBtnCh"), new GUILayoutOption[0]))
			{
				map_size_select_id--;
				if (map_size_select_id < 0)
				{
					map_size_select_id = 0;
				}
			}
			if (GUILayout.Button(">", GUIStyle.op_Implicit("ShorBtnCh"), new GUILayoutOption[0]))
			{
				map_size_select_id++;
				if (map_size_select_id > 1)
				{
					map_size_select_id = 1;
				}
			}
			GUILayout.EndHorizontal();
		}
		else
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label("Размер карты: ", new GUILayoutOption[1] { GUILayout.MinWidth(100f) });
			GUILayout.Label("Маленькая", new GUILayoutOption[1] { GUILayout.MaxWidth(150f) });
			GUILayout.EndHorizontal();
			map_size_select_id = 0;
		}
		if (map_types_names.Count > 1)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label("Тип карты:", new GUILayoutOption[1] { GUILayout.MinWidth(100f) });
			GUILayout.Label(map_types_names[map_type_select_id], new GUILayoutOption[0]);
			if (GUILayout.Button("<", GUIStyle.op_Implicit("ShorBtnCh"), new GUILayoutOption[0]))
			{
				map_type_select_id--;
				if (map_type_select_id < 0)
				{
					map_type_select_id = 0;
				}
			}
			if (GUILayout.Button(">", GUIStyle.op_Implicit("ShorBtnCh"), new GUILayoutOption[0]))
			{
				map_type_select_id++;
				if (map_type_select_id > map_types_names.Count - 1)
				{
					map_type_select_id = map_types_names.Count - 1;
				}
			}
			GUILayout.EndHorizontal();
		}
		else
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label("Тип карты: ", new GUILayoutOption[1] { GUILayout.MinWidth(100f) });
			GUILayout.Label("Стандартная", new GUILayoutOption[1] { GUILayout.MaxWidth(150f) });
			GUILayout.EndHorizontal();
			map_type_select_id = 0;
		}
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("Название: ", new GUILayoutOption[0]);
		ProfileINI.server_name = GUILayout.TextField(ProfileINI.server_name, new GUILayoutOption[1] { GUILayout.MaxWidth(150f) });
		ProfileINI.server_name = Utils.MaxStringLength(ProfileINI.server_name, 16);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("Описание: ", new GUILayoutOption[0]);
		ProfileINI.server_about = GUILayout.TextArea(ProfileINI.server_about, new GUILayoutOption[1] { GUILayout.MaxWidth(150f) });
		ProfileINI.server_about = Utils.MaxStringLength(ProfileINI.server_about, 250);
		GUILayout.EndHorizontal();
		GUILayout.Space(20f);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("Пароль (если надо): ", new GUILayoutOption[0]);
		Password = GUILayout.TextField(Password, new GUILayoutOption[1] { GUILayout.MaxWidth(150f) });
		GUILayout.EndHorizontal();
		GUILayout.Label(_NewGameWarning, GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		if (!GUILayout.Button("Создать", new GUILayoutOption[0]) && !bs._Igor.autoJoin2)
		{
			return;
		}
		if (ProfileINI.server_name.Length < 3)
		{
			_NewGameWarning = "Название комнаты должно быть больше 3 букв!";
			return;
		}
		bool flag = false;
		RoomInfo[] roomList = PhotonNetwork.GetRoomList();
		foreach (RoomInfo roomInfo in roomList)
		{
			if ((string)roomInfo.customProperties["player_id"] == VkontakteAPI.INSTANCE._viewerId && (int)roomInfo.customProperties["slot_id"] == _SelectedSlot)
			{
				flag = true;
				break;
			}
		}
		bool flag2 = false;
		if (!flag)
		{
			RoomInfo[] roomList2 = PhotonNetwork.GetRoomList();
			foreach (RoomInfo roomInfo2 in roomList2)
			{
				if ((string)roomInfo2.customProperties["map_name"] == ProfileINI.server_name)
				{
					flag2 = true;
					break;
				}
			}
		}
		if (!flag && !flag2)
		{
			_NewGameWarning = string.Empty;
			battlesRightMenu = BatllesRightMenu.UNKNOWN;
			App.instance.gameIni = new GameINI();
			App.instance.gameIni.server_name = ProfileINI.server_name;
			App.instance.gameIni.server_about = ProfileINI.server_about;
			App.instance.gameIni.gameType = GameINI.GameType.BUILDING;
			App.instance.gameIni.mapSize = GameINI.MapSize.SMALL;
			App.instance.gameIni.mapPopulation = byte.Parse(MapPopulation);
			App.instance.gameIni.isOnline = true;
			App.instance.gameIni.isServer = true;
			App.instance.gameIni.isWatch = false;
			App.instance.gameIni.loadingSavedMap = false;
			App.instance.gameIni.playerID = VkontakteAPI.INSTANCE._viewerId;
			App.instance.gameIni.slotID = _SelectedSlot;
			App.instance.gameIni.publicMapID = 0;
			App.instance.gameIni.Password = Password;
			if (map_size_select_id == 0)
			{
				App.instance.gameIni.mapSize = GameINI.MapSize.SMALL;
			}
			if (map_size_select_id == 1)
			{
				App.instance.gameIni.mapSize = GameINI.MapSize.MEDIUM;
			}
			if (map_types_names[map_type_select_id] == "Стандартная")
			{
				App.instance.gameIni.mapType = GameINI.MapType.STANDART;
			}
			if (map_types_names[map_type_select_id] == "Плоская")
			{
				App.instance.gameIni.mapType = GameINI.MapType.FLAT;
			}
			if (map_types_names[map_type_select_id] == "Океан")
			{
				App.instance.gameIni.mapType = GameINI.MapType.OCEAN;
			}
			if (map_types_names[map_type_select_id] == "Пустыня")
			{
				App.instance.gameIni.mapType = GameINI.MapType.SAND;
			}
			if (map_types_names[map_type_select_id] == "Острова")
			{
				App.instance.gameIni.mapType = GameINI.MapType.ISLAND;
			}
			if (map_types_names[map_type_select_id] == "Зима")
			{
				App.instance.gameIni.mapType = GameINI.MapType.SNOWLAND;
			}
			PhotonNetwork.playerName = ProfileINI.nickname;
			Application.LoadLevel("Game");
			curWindow = Windows.Loading;
			loading_text = "Загрузка уровня...";
		}
		else if (flag)
		{
			_NewGameWarning = "Нельзя перегенерировать карту пока на ней кто-либо играет.";
		}
		else
		{
			_NewGameWarning = "Комната с таким названием существует.";
		}
	}

	private void ToBattlesMenu()
	{
		curWindow = Windows.Battles;
	}

	private void ToLastMenu()
	{
		curWindow = lastWindow;
	}

	public void CleanScreenMode(bool mode)
	{
		if ((Object)(object)TeamBattle.Instance != (Object)null && TeamBattle.Instance.IsObserving())
		{
			return;
		}
		enable_clean_screen_mode = mode;
		if (enable_clean_screen_mode)
		{
			WorldGameObject.INSTANCE.GUI.SetActiveRecursively(false);
			if (!CameraController.Instance.IsThirdPerson)
			{
				WorldGameObject.INSTANCE.MainPlayer.GetComponent<SkinManager>().DisableAllHands();
			}
			for (int i = 0; i < WorldGameObject.INSTANCE.playerList.Count; i++)
			{
				GameObject avatar = WorldGameObject.INSTANCE.playerList[i].avatar;
				if ((Object)(object)avatar != (Object)null)
				{
					avatar.GetComponent<SetLabelOnMessage>().Hide();
				}
			}
			return;
		}
		if (!CameraController.Instance.IsThirdPerson)
		{
			WorldGameObject.INSTANCE.MainPlayer.GetComponent<SkinManager>().SetHand(ProfileINI.skin);
		}
		WorldGameObject.INSTANCE.GUI.SetActiveRecursively(true);
		for (int j = 0; j < WorldGameObject.INSTANCE.playerList.Count; j++)
		{
			GameObject avatar2 = WorldGameObject.INSTANCE.playerList[j].avatar;
			if ((Object)(object)avatar2 != (Object)null)
			{
				avatar2.GetComponent<SetLabelOnMessage>().Show();
			}
		}
	}

	private void Update()
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		bs.Log("Can Destroy " + bs._MainMenu.destroyable);
		if (Input.GetKeyDown((KeyCode)293))
		{
			toggleFullScreen();
			if (Application.loadedLevelName == "Game")
			{
				GUITexture component = GameObject.Find("hit_effect").GetComponent<GUITexture>();
				Resolution currentResolution = Screen.currentResolution;
				float num = -currentResolution.width / 2;
				Resolution currentResolution2 = Screen.currentResolution;
				float num2 = -currentResolution2.height / 2;
				Resolution currentResolution3 = Screen.currentResolution;
				float num3 = currentResolution3.width;
				Resolution currentResolution4 = Screen.currentResolution;
				component.pixelInset = new Rect(num, num2, num3, (float)currentResolution4.height);
			}
		}
		if (Input.GetKeyDown((KeyCode)114) && !ChatClient.isEnabled && !bs._Igor.enableGuns)
		{
			WorldGameObject.INSTANCE.Respawn();
		}
		if (Input.GetKeyDown((KeyCode)290))
		{
			cinematic_camera = !cinematic_camera;
		}
		if (Input.GetKeyDown((KeyCode)292) && Application.loadedLevelName == "Game")
		{
			enable_clean_screen_mode = !enable_clean_screen_mode;
			CleanScreenMode(enable_clean_screen_mode);
		}
		if (Input.GetKeyDown((KeyCode)289))
		{
			GameConsoleWithScrollView.show = !GameConsoleWithScrollView.show;
		}
		UpdateSpecialBonus();
		if (Application.loadedLevelName == "Menu")
		{
			if (GameConsoleWithScrollView.show)
			{
				showMenu = false;
			}
			else
			{
				showMenu = true;
			}
			if (Input.GetKeyDown((KeyCode)27) && curWindow == Windows.MapCard)
			{
				curWindow = Windows.Battles;
			}
		}
	}

	private void DrawBattlesList()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Expected Obj, but got Unknown
		_BattlesListScroll = GUILayout.BeginScrollView(_BattlesListScroll, new GUILayoutOption[0]);
		RoomInfo[] roomList = PhotonNetwork.GetRoomList();
		foreach (RoomInfo roomInfo in roomList)
		{
			if (roomInfo.customProperties.ContainsKey("is_watch") && (bool)roomInfo.customProperties["is_watch"])
			{
				continue;
			}
			string text = string.Concat(roomInfo.customProperties["map_name"], " ", roomInfo.playerCount, "/", roomInfo.maxPlayers);
			if (!(_ServerNameToFind == "Поиск сервера по названию") && _ServerNameToFind.Length != 0 && text.ToLower().IndexOf(_ServerNameToFind.ToLower()) == -1)
			{
				continue;
			}
			Texture2D val = null;
			if (roomInfo.customProperties.ContainsKey("password") && ((string)roomInfo.customProperties["password"]).Length > 0)
			{
				val = LockServerIcon;
			}
			if (GUILayout.Button(new GUIContent(text, (Texture)(object)val), new GUILayoutOption[2]
			{
				GUILayout.MaxWidth(280f),
				GUILayout.MinHeight(50f)
			}))
			{
				_NewGameWarning = string.Empty;
				battlesRightMenu = BatllesRightMenu.EXISTS_BATTLE;
				_BattlesMapName = (string)roomInfo.customProperties["map_name"];
				if (roomInfo.customProperties.ContainsKey("public_id") && (int)roomInfo.customProperties["public_id"] > 1)
				{
					((MonoBehaviour)this).StartCoroutine(LoadMapInfo((int)roomInfo.customProperties["public_id"]));
				}
			}
		}
		GUILayout.EndScrollView();
	}

	private void DrawFavoritServers()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (favorit_servers == null)
		{
			GUILayout.Label("Загрузка списка...", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
			return;
		}
		_BattlesListScroll = GUILayout.BeginScrollView(_BattlesListScroll, new GUILayoutOption[0]);
		FavoritSavers[] array = favorit_servers;
		foreach (FavoritSavers favoritSavers in array)
		{
			string server_name = favoritSavers.server_name;
			if (GUILayout.Button(server_name, new GUILayoutOption[2]
			{
				GUILayout.MaxWidth(280f),
				GUILayout.MinHeight(50f)
			}))
			{
			}
		}
		GUILayout.EndScrollView();
	}

	private void CreateGame()
	{
		battlesRightMenu = BatllesRightMenu.NEW_BATTLE;
		map_types_names.Clear();
		map_types_names.Add("Стандартная");
		map_type_select_id = 0;
		if (ProfileINI.GetPurchesValue(StorePurchase.FLAT_MAP) > 0)
		{
			map_types_names.Add("Плоская");
		}
		if (ProfileINI.GetPurchesValue(StorePurchase.SAND_MAP) > 0)
		{
			map_types_names.Add("Пустыня");
		}
		if (ProfileINI.GetPurchesValue(StorePurchase.OCEAN_MAP) > 0)
		{
			map_types_names.Add("Океан");
		}
		if (ProfileINI.GetPurchesValue(StorePurchase.ISLAND_MAP) > 0)
		{
			map_types_names.Add("Острова");
		}
		if (ProfileINI.GetPurchesValue(StorePurchase.SNOW_MAP) > 0)
		{
			map_types_names.Add("Зима");
		}
	}

	private void FastConnectToGame(GameINI.GameType gameType)
	{
		RoomInfo[] roomList = PhotonNetwork.GetRoomList();
		List<RoomInfo> list = new List<RoomInfo>();
		RoomInfo[] array = roomList;
		foreach (RoomInfo roomInfo in array)
		{
			if (roomInfo.customProperties.Contains("game_type") && (int)roomInfo.customProperties["game_type"] == (int)gameType)
			{
				list.Add(roomInfo);
			}
		}
		if (list.Count == 0)
		{
			_FastConnectWarning = "Нет созданных карт, создайте свою нажав на 'Мои карты'";
			return;
		}
		Random random = new Random();
		for (int j = 0; j < list.Count; j++)
		{
			int index = random.Next(0, list.Count);
			RoomInfo roomInfo2 = list[index];
			if (((string)roomInfo2.customProperties["password"]).Length == 0 && roomInfo2.playerCount < roomInfo2.maxPlayers)
			{
				JoinGame(roomInfo2);
				return;
			}
		}
		foreach (Room item in list)
		{
			if (((string)item.customProperties["password"]).Length == 0 && item.playerCount < item.maxPlayers)
			{
				JoinGame(item);
				return;
			}
		}
		_FastConnectWarning = "Все сервера полные или под паролем, создайте свой нажав на 'Мои карты' ";
	}

	private void DrawBattles()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0533: Unknown result type (might be due to invalid IL or missing references)
		//IL_050b: Unknown result type (might be due to invalid IL or missing references)
		//IL_056e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0591: Unknown result type (might be due to invalid IL or missing references)
		if (all_player_count == 0)
		{
			GUI.Box(new Rect(0f, 70f, 340f, 460f), "Поиск серверов...");
		}
		else
		{
			GUI.Box(new Rect(0f, 70f, 340f, 460f), "Меню");
		}
		GUILayout.BeginArea(new Rect(10f, 70f, 320f, 450f));
		GUILayout.Space(30f);
		if (battlesLeftMenu != BatllesLeftMenu.MAIN && GUILayout.Button("<-Назад", new GUILayoutOption[1] { GUILayout.MaxWidth(100f) }))
		{
			_FastConnectWarning = string.Empty;
			battlesLeftMenu = BatllesLeftMenu.MAIN;
			battlesRightMenu = BatllesRightMenu.NEWS;
		}
		if (battlesLeftMenu == BatllesLeftMenu.MAIN)
		{
			GUILayout.Space(20f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(50f);
			if (GUILayout.Button("Быстрая игра", GUIStyle.op_Implicit("MainBtn3"), new GUILayoutOption[1] { GUILayout.MinWidth(400f) }))
			{
				battlesLeftMenu = BatllesLeftMenu.FAST_CONNECT;
			}
			GUILayout.EndHorizontal();
			if (_FastConnectWarning.Length > 0)
			{
				GUILayout.Space(50f);
				GUILayout.Label(_FastConnectWarning, GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
			}
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(50f);
			if (GUILayout.Button("Мои карты", GUIStyle.op_Implicit("MainBtn1"), new GUILayoutOption[1] { GUILayout.MinWidth(400f) }) || bs._Igor.autoJoin2)
			{
				battlesLeftMenu = BatllesLeftMenu.MY_MAPS;
				battlesRightMenu = BatllesRightMenu.UNKNOWN;
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(0f);
			if (GUILayout.Button("Список серверов", new GUILayoutOption[1] { GUILayout.MinHeight(50f) }))
			{
				battlesLeftMenu = BatllesLeftMenu.BUILDING;
			}
			GUILayout.EndHorizontal();
			if (App.instance.platform == App.ePlatform.ONDOKLASNIKI)
			{
				GUILayout.Space(120f);
				if (GUILayout.Button("Позвать друзей", new GUILayoutOption[1] { GUILayout.MinHeight(50f) }))
				{
					Application.ExternalCall("InviteFriend", new object[0]);
				}
			}
		}
		if (battlesLeftMenu == BatllesLeftMenu.BUILDING)
		{
			if (battlesServersMenu == BatllesServersMenu.ALL_SERVERS)
			{
				DrawBattlesList();
				GUI.SetNextControlName("FindServer");
				_ServerNameToFind = GUILayout.TextField(_ServerNameToFind, new GUILayoutOption[0]);
				if (Input.GetKeyDown((KeyCode)113) || Input.GetKeyDown((KeyCode)119) || Input.GetKeyDown((KeyCode)114) || Input.GetKeyDown((KeyCode)116) || Input.GetKeyDown((KeyCode)121) || Input.GetKeyDown((KeyCode)117) || Input.GetKeyDown((KeyCode)105) || Input.GetKeyDown((KeyCode)111) || Input.GetKeyDown((KeyCode)112) || Input.GetKeyDown((KeyCode)97) || Input.GetKeyDown((KeyCode)115) || Input.GetKeyDown((KeyCode)100) || Input.GetKeyDown((KeyCode)102) || Input.GetKeyDown((KeyCode)103) || Input.GetKeyDown((KeyCode)104) || Input.GetKeyDown((KeyCode)106) || Input.GetKeyDown((KeyCode)107) || Input.GetKeyDown((KeyCode)108) || Input.GetKeyDown((KeyCode)122) || Input.GetKeyDown((KeyCode)120) || Input.GetKeyDown((KeyCode)99) || Input.GetKeyDown((KeyCode)118) || Input.GetKeyDown((KeyCode)98) || Input.GetKeyDown((KeyCode)110) || Input.GetKeyDown((KeyCode)109))
				{
					GUI.FocusControl("FindServer");
				}
			}
			if (battlesServersMenu == BatllesServersMenu.FAVORIT)
			{
				DrawFavoritServers();
			}
		}
		if (battlesLeftMenu == BatllesLeftMenu.MY_MAPS)
		{
			DrawMyMaps();
		}
		if (battlesLeftMenu == BatllesLeftMenu.FAST_CONNECT)
		{
			GUILayout.Space(20f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(50f);
			if (GUILayout.Button("Строительство", GUIStyle.op_Implicit("MainBtn3"), new GUILayoutOption[1] { GUILayout.MinWidth(400f) }))
			{
				FastConnectToGame(GameINI.GameType.BUILDING);
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(50f);
			if (GUILayout.Button("Война", GUIStyle.op_Implicit("MainBtn3"), new GUILayoutOption[1] { GUILayout.MinWidth(400f) }))
			{
				FastConnectToGame(GameINI.GameType.DEATHMATCH);
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(50f);
			if (GUILayout.Button("Командный бой", GUIStyle.op_Implicit("MainBtn3"), new GUILayoutOption[1] { GUILayout.MinWidth(400f) }))
			{
				FastConnectToGame(GameINI.GameType.TEAM_BATTLE);
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.EndArea();
		if (battlesRightMenu == BatllesRightMenu.NEWS)
		{
			GUI.Box(new Rect(350f, 70f, 340f, 460f), "Новости");
		}
		else
		{
			GUI.Box(new Rect(350f, 70f, 340f, 460f), "Описание");
		}
		if (battlesRightMenu == BatllesRightMenu.EXISTS_BATTLE || battlesRightMenu == BatllesRightMenu.MY_MAPS)
		{
			GUILayout.BeginArea(new Rect(360f, 70f, 340f, 450f));
		}
		else
		{
			GUILayout.BeginArea(new Rect(360f, 70f, 320f, 450f));
		}
		if (battlesRightMenu == BatllesRightMenu.NEWS)
		{
			DrawNews();
		}
		if (battlesRightMenu == BatllesRightMenu.NEW_BATTLE)
		{
			DrawBattlesNewMenu();
		}
		if (battlesRightMenu == BatllesRightMenu.EXISTS_BATTLE)
		{
			DrawBattlesExistsMenu();
		}
		if (battlesRightMenu == BatllesRightMenu.SAVED_BATTLE)
		{
			DrawBattlesSavedMenu();
		}
		if (battlesRightMenu == BatllesRightMenu.MY_MAPS)
		{
			DrawSaveMapInfo();
		}
		GUILayout.EndArea();
	}

	private IEnumerator GetFavoritServers()
	{
		favorit_servers = null;
		WWWForm form = new WWWForm();
		form.AddField("viewer_id", VkontakteAPI.INSTANCE._viewerId);
		Random r = new Random();
		form.AddField("random", r.Next());
		WWW php_load = new WWW(App.instance.GetCurScriptsUrl() + "get_favorit_servers.php", form);
		yield return php_load;
		Debug.Log((object)php_load.text);
		string[] servers = php_load.text.Split('|');
		favorit_servers = new FavoritSavers[servers.Length - 1];
		for (int i = 1; i < servers.Length; i++)
		{
			string[] param = servers[i].Split(',');
			favorit_servers[i - 1] = new FavoritSavers();
			favorit_servers[i - 1].server_name = param[0];
			favorit_servers[i - 1].admin_id = int.Parse(param[1]);
			favorit_servers[i - 1].slot_id = byte.Parse(param[2]);
		}
	}

	private void DrawInGameStore()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)boxSkin != (Object)null)
		{
			GUI.skin = boxSkin;
		}
		GUI.Box(new Rect((float)(Screen.width / 2 - 370), (float)(Screen.height / 2 - 270), 740f, 540f), "Магазин");
		if ((Object)(object)guiSkin != (Object)null)
		{
			GUI.skin = guiSkin;
		}
		GUILayout.BeginArea(new Rect((float)(Screen.width / 2 - 350), (float)(Screen.height / 2 - 270), 700f, 540f));
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		GUILayout.Space(30f);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		DrawMoney(draw_btn: false);
		if (bs._Igor.enableGuns)
		{
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Закрыть", new GUILayoutOption[0]))
			{
				curWindow = Windows.SelectWeapon;
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(20f);
		GUI.Box(new Rect(0f, 70f, 440f, 460f), "Магазин");
		GUILayout.BeginArea(new Rect(10f, 70f, 420f, 450f));
		GUILayout.Space(30f);
		DrawStoreLeft();
		GUILayout.EndArea();
		GUI.Box(new Rect(450f, 70f, 240f, 460f), "Описание");
		GUILayout.BeginArea(new Rect(460f, 70f, 220f, 450f));
		DrawStoreRight();
		GUILayout.EndArea();
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	private void DrawStore()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		GUI.Box(new Rect(0f, 70f, 440f, 460f), "Магазин");
		GUILayout.BeginArea(new Rect(10f, 70f, 420f, 450f));
		GUILayout.Space(30f);
		DrawStoreLeft();
		GUILayout.EndArea();
		GUI.Box(new Rect(450f, 70f, 240f, 460f), "Описание");
		GUILayout.BeginArea(new Rect(460f, 70f, 220f, 450f));
		DrawStoreRight();
		GUILayout.EndArea();
	}

	private void DrawStoreLeft()
	{
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		GUILayoutOption[] array = new GUILayoutOption[2]
		{
			GUILayout.MaxWidth(90f),
			GUILayout.MaxHeight(90f)
		};
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		FixMenuControl(4);
		if (GUILayout.Button("Блоки", new GUILayoutOption[0]))
		{
			curStoreLeftWindow = StoreLeftWindow.BLOCKS;
		}
		FixMenuControl(5);
		if (GUILayout.Button("Декорации", new GUILayoutOption[0]))
		{
			curStoreLeftWindow = StoreLeftWindow.MEBEL;
		}
		FixMenuControl(6);
		if (GUILayout.Button("Растения", new GUILayoutOption[0]))
		{
			curStoreLeftWindow = StoreLeftWindow.PLANTS;
		}
		if (GUILayout.Button("Животные", new GUILayoutOption[0]))
		{
			curStoreLeftWindow = StoreLeftWindow.MOBS;
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		FixMenuControl(8);
		if (GUILayout.Button("Инструменты", new GUILayoutOption[1] { GUILayout.MaxWidth(150f) }))
		{
			curStoreLeftWindow = StoreLeftWindow.INSTRUMENTS;
		}
		if (Application.loadedLevelName == "Menu")
		{
			FixMenuControl(9);
			if (GUILayout.Button("Одежда", new GUILayoutOption[1] { GUILayout.MaxWidth(150f) }))
			{
				curStoreLeftWindow = StoreLeftWindow.SKINS;
				_CurrentPurchase = StorePurchase.NONE;
			}
		}
		FixMenuControl(10);
		if (GUILayout.Button("Карты", new GUILayoutOption[1] { GUILayout.MaxWidth(150f) }))
		{
			curStoreLeftWindow = StoreLeftWindow.MAPS;
			_CurrentPurchase = StorePurchase.NONE;
		}
		FixMenuControl(7);
		if (GUILayout.Button("Разное", new GUILayoutOption[0]))
		{
			curStoreLeftWindow = StoreLeftWindow.OTHER;
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		if (GUILayout.Button("Оружие", new GUILayoutOption[1] { GUILayout.MaxWidth(150f) }))
		{
			curStoreLeftWindow = StoreLeftWindow.WEAPONS;
			_CurrentPurchase = StorePurchase.NONE;
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		slider = GUILayout.BeginScrollView(slider, new GUILayoutOption[0]);
		if (curStoreLeftWindow == StoreLeftWindow.MOBS)
		{
			StorePurchase[] array2 = new StorePurchase[6]
			{
				StorePurchase.FISH_1,
				StorePurchase.FISH_2,
				StorePurchase.FISH_3,
				StorePurchase.CAT,
				StorePurchase.DOG,
				StorePurchase.CHICKEN
			};
			int[] array3 = new int[6] { 59, 60, 61, 68, 69, 70 };
			for (int i = 0; i < array2.Length; i++)
			{
				if (i % 4 == 0)
				{
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				}
				if (GUILayout.Button((Texture)(object)storeIcons[array3[i]], GUIStyle.op_Implicit("StoreBtn"), array))
				{
					by_warning = string.Empty;
					_CurrentPurchase = array2[i];
					if ((Object)(object)mobs_obj != (Object)null)
					{
						mobs_obj.GetComponent<SkinManager>().SetSkin(i);
					}
				}
				if (i % 4 == 3 || i == array2.Length - 1)
				{
					GUILayout.EndHorizontal();
				}
			}
		}
		if (curStoreLeftWindow == StoreLeftWindow.SKINS)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (GUILayout.Button((Texture)(object)storeIcons[33], GUIStyle.op_Implicit("StoreBtn"), array))
			{
				by_warning = string.Empty;
				_CurrentPurchase = StorePurchase.STANDART_SKIN;
				int num = (int)(_CurrentPurchase - 5 - 1);
				preview_obj.GetComponent<SkinManager>().SetSkin(clotheInd[num]);
			}
			if (GUILayout.Button((Texture)(object)storeIcons[38], GUIStyle.op_Implicit("StoreBtn"), array))
			{
				by_warning = string.Empty;
				_CurrentPurchase = StorePurchase.PIRATE_SKIN_1;
				int num2 = (int)(_CurrentPurchase - 5 - 1);
				preview_obj.GetComponent<SkinManager>().SetSkin(clotheInd[num2]);
			}
			if (GUILayout.Button((Texture)(object)storeIcons[40], GUIStyle.op_Implicit("StoreBtn"), array))
			{
				by_warning = string.Empty;
				_CurrentPurchase = StorePurchase.KNIGHT_SKIN_1;
				int num3 = (int)(_CurrentPurchase - 5 - 1);
				preview_obj.GetComponent<SkinManager>().SetSkin(clotheInd[num3]);
			}
			if (GUILayout.Button((Texture)(object)storeIcons[46], GUIStyle.op_Implicit("StoreBtn"), array))
			{
				by_warning = string.Empty;
				_CurrentPurchase = StorePurchase.KNIGHT_SKIN_2;
				int num4 = (int)(_CurrentPurchase - 5 - 1);
				preview_obj.GetComponent<SkinManager>().SetSkin(clotheInd[num4]);
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (GUILayout.Button((Texture)(object)storeIcons[54], GUIStyle.op_Implicit("StoreBtn"), array))
			{
				by_warning = string.Empty;
				_CurrentPurchase = StorePurchase.PIRATE_SKIN_2;
				int num5 = (int)(_CurrentPurchase - 5 - 1);
				preview_obj.GetComponent<SkinManager>().SetSkin(clotheInd[num5]);
			}
			if (GUILayout.Button((Texture)(object)storeIcons[58], GUIStyle.op_Implicit("StoreBtn"), array))
			{
				by_warning = string.Empty;
				_CurrentPurchase = StorePurchase.VIKING_SKIN_1;
				int num6 = (int)(_CurrentPurchase - 5 - 1);
				preview_obj.GetComponent<SkinManager>().SetSkin(clotheInd[num6]);
			}
			if (GUILayout.Button((Texture)(object)storeIcons[62], GUIStyle.op_Implicit("StoreBtn"), array))
			{
				by_warning = string.Empty;
				_CurrentPurchase = StorePurchase.ZOMBIE_SKIN;
				int num7 = (int)(_CurrentPurchase - 5 - 1);
				preview_obj.GetComponent<SkinManager>().SetSkin(clotheInd[num7]);
			}
			if (GUILayout.Button((Texture)(object)storeIcons[71], GUIStyle.op_Implicit("StoreBtn"), array))
			{
				by_warning = string.Empty;
				_CurrentPurchase = StorePurchase.DEATH_KNIGHT_SKIN;
				int num8 = (int)(_CurrentPurchase - 5 - 1);
				preview_obj.GetComponent<SkinManager>().SetSkin(clotheInd[num8]);
			}
			GUILayout.EndHorizontal();
			if (GUILayout.Button((Texture)(object)storeIcons[73], GUIStyle.op_Implicit("StoreBtn"), array))
			{
				by_warning = string.Empty;
				_CurrentPurchase = StorePurchase.SOLDIER;
				int num9 = (int)(_CurrentPurchase - 5 - 1);
				preview_obj.GetComponent<SkinManager>().SetSkin(clotheInd[num9]);
			}
		}
		if (curStoreLeftWindow == StoreLeftWindow.INSTRUMENTS)
		{
			StorePurchase[] array4 = new StorePurchase[14]
			{
				StorePurchase.KIRCKA,
				StorePurchase.SPEED,
				StorePurchase.FLY,
				StorePurchase.DINAMIT,
				StorePurchase.ATOMBOMB,
				StorePurchase.SPAWN_POINT,
				StorePurchase.TEAM_SPAWN_POINT,
				StorePurchase.BOAT,
				StorePurchase.EMOTIONS,
				StorePurchase.MEM_SMILES,
				StorePurchase.RAIL,
				StorePurchase.TROLLEY,
				StorePurchase.TIME_OF_DAY,
				StorePurchase.GOLD_KUBOK
			};
			for (int j = 0; j < array4.Length; j++)
			{
				if (j % 4 == 0)
				{
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				}
				if (GUILayout.Button((Texture)(object)storeIcons[(int)(array4[j] - 6)], GUIStyle.op_Implicit("StoreBtn"), array))
				{
					by_warning = string.Empty;
					_CurrentPurchase = array4[j];
				}
				if (j % 4 == 3)
				{
					GUILayout.EndHorizontal();
				}
			}
			if (array4.Length % 4 != 0)
			{
				GUILayout.EndHorizontal();
			}
		}
		if (curStoreLeftWindow == StoreLeftWindow.OTHER)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (GUILayout.Button((Texture)(object)storeIcons[0], GUIStyle.op_Implicit("StoreBtn"), array))
			{
				by_warning = string.Empty;
				_CurrentPurchase = StorePurchase.SLOTS;
			}
			if (GUILayout.Button((Texture)(object)storeIcons[27], GUIStyle.op_Implicit("StoreBtn"), array))
			{
				by_warning = string.Empty;
				_CurrentPurchase = StorePurchase.CHANGENICK;
			}
			GUILayout.EndHorizontal();
		}
		if (curStoreLeftWindow == StoreLeftWindow.MAPS)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (GUILayout.Button((Texture)(object)storeIcons[47], GUIStyle.op_Implicit("StoreBtn"), array))
			{
				by_warning = string.Empty;
				_CurrentPurchase = StorePurchase.MEDIUM_MAP;
			}
			if (GUILayout.Button((Texture)(object)storeIcons[48], GUIStyle.op_Implicit("StoreBtn"), array))
			{
				by_warning = string.Empty;
				_CurrentPurchase = StorePurchase.FLAT_MAP;
			}
			if (GUILayout.Button((Texture)(object)storeIcons[49], GUIStyle.op_Implicit("StoreBtn"), array))
			{
				by_warning = string.Empty;
				_CurrentPurchase = StorePurchase.SAND_MAP;
			}
			if (GUILayout.Button((Texture)(object)storeIcons[50], GUIStyle.op_Implicit("StoreBtn"), array))
			{
				by_warning = string.Empty;
				_CurrentPurchase = StorePurchase.OCEAN_MAP;
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (GUILayout.Button((Texture)(object)storeIcons[51], GUIStyle.op_Implicit("StoreBtn"), array))
			{
				by_warning = string.Empty;
				_CurrentPurchase = StorePurchase.ISLAND_MAP;
			}
			if (GUILayout.Button((Texture)(object)storeIcons[52], GUIStyle.op_Implicit("StoreBtn"), array))
			{
				by_warning = string.Empty;
				_CurrentPurchase = StorePurchase.SNOW_MAP;
			}
			GUILayout.EndHorizontal();
		}
		if (curStoreLeftWindow == StoreLeftWindow.MEBEL)
		{
			StorePurchase[] array5 = new StorePurchase[32]
			{
				StorePurchase.STOOL,
				StorePurchase.CHAIR_STONE,
				StorePurchase.BED,
				StorePurchase.BED_STONE,
				StorePurchase.FENCE,
				StorePurchase.FENCE_STONE,
				StorePurchase.BACKET,
				StorePurchase.BASKET,
				StorePurchase.DOORW,
				StorePurchase.TABLE,
				StorePurchase.TABLE_STONE,
				StorePurchase.BIG_TABLE,
				StorePurchase.CHEST,
				StorePurchase.CHEST_STONE,
				StorePurchase.BARREL,
				StorePurchase.SIGN,
				StorePurchase.CUPBOARDS,
				StorePurchase.CUPBOARDL,
				StorePurchase.STAIRSW,
				StorePurchase.STAIRSB,
				StorePurchase.TORCHW,
				StorePurchase.TORCH_STONE,
				StorePurchase.TORCHF,
				StorePurchase.TORCH_FLOOR_STONE,
				StorePurchase.TABLICHKA,
				StorePurchase.TABLICHKAW,
				StorePurchase.BOOK,
				StorePurchase.BENCH_WOOD,
				StorePurchase.BENCH_STONE,
				StorePurchase.LADDER,
				StorePurchase.TOMBSTONE_1,
				StorePurchase.TOMBSTONE_2
			};
			for (int k = 0; k < array5.Length; k++)
			{
				if (k % 4 == 0)
				{
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				}
				if (GUILayout.Button((Texture)(object)storeIcons[(int)(array5[k] - 6)], GUIStyle.op_Implicit("StoreBtn"), array))
				{
					by_warning = string.Empty;
					_CurrentPurchase = array5[k];
				}
				if (k % 4 == 3)
				{
					GUILayout.EndHorizontal();
				}
			}
			if (array5.Length % 4 != 0)
			{
				GUILayout.EndHorizontal();
			}
		}
		if (curStoreLeftWindow == StoreLeftWindow.WEAPONS)
		{
			StorePurchase[] array6 = new StorePurchase[8]
			{
				StorePurchase.WEAPON_GLOK,
				StorePurchase.WEAPON_MP5,
				StorePurchase.WEAPON_AK47,
				StorePurchase.WEAPON_M16,
				StorePurchase.WEAPON_RIFLE,
				StorePurchase.WEAPON_SNIPER_RIFLE,
				StorePurchase.WEAPON_M1014,
				StorePurchase.WEAPON_GRENADE_LAUNCHER
			};
			for (int l = 0; l < array6.Length; l++)
			{
				if (l % 4 == 0)
				{
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				}
				if (GUILayout.Button((Texture)(object)storeIcons[(int)(array6[l] - 6)], GUIStyle.op_Implicit("StoreBtn"), array))
				{
					by_warning = string.Empty;
					_CurrentPurchase = array6[l];
				}
				if (l % 4 == 3)
				{
					GUILayout.EndHorizontal();
				}
			}
			if (array6.Length % 4 != 0)
			{
				GUILayout.EndHorizontal();
			}
		}
		if (curStoreLeftWindow == StoreLeftWindow.BONUS)
		{
			StorePurchase[] array7 = new StorePurchase[4]
			{
				StorePurchase.BONUS_LIFE,
				StorePurchase.BONUS_ARMOR,
				StorePurchase.BONUS_WEAPONS,
				StorePurchase.BOUNS_GRANADE
			};
			for (int m = 0; m < array7.Length; m++)
			{
				if (m % 4 == 0)
				{
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				}
				if (GUILayout.Button((Texture)(object)storeIcons[(int)(array7[m] - 6)], GUIStyle.op_Implicit("StoreBtn"), array))
				{
					by_warning = string.Empty;
					_CurrentPurchase = array7[m];
				}
				if (m % 4 == 3)
				{
					GUILayout.EndHorizontal();
				}
			}
			if (array7.Length % 4 != 0)
			{
				GUILayout.EndHorizontal();
			}
		}
		if (curStoreLeftWindow == StoreLeftWindow.PLANTS)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (GUILayout.Button((Texture)(object)storeIcons[4], GUIStyle.op_Implicit("StoreBtn"), array))
			{
				by_warning = string.Empty;
				_CurrentPurchase = StorePurchase.FLOWER;
			}
			if (GUILayout.Button((Texture)(object)storeIcons[12], GUIStyle.op_Implicit("StoreBtn"), array))
			{
				by_warning = string.Empty;
				_CurrentPurchase = StorePurchase.FLOWER2;
			}
			if (GUILayout.Button((Texture)(object)storeIcons[13], GUIStyle.op_Implicit("StoreBtn"), array))
			{
				by_warning = string.Empty;
				_CurrentPurchase = StorePurchase.FLOWER3;
			}
			GUILayout.EndHorizontal();
		}
		if (curStoreLeftWindow == StoreLeftWindow.BLOCKS)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (GUILayout.Button((Texture)(object)storeIcons[14], GUIStyle.op_Implicit("StoreBtn"), array))
			{
				by_warning = string.Empty;
				_CurrentPurchase = StorePurchase.CUBES4;
			}
			if (GUILayout.Button((Texture)(object)storeIcons[1], GUIStyle.op_Implicit("StoreBtn"), array))
			{
				by_warning = string.Empty;
				_CurrentPurchase = StorePurchase.CUBES1;
			}
			if (GUILayout.Button((Texture)(object)storeIcons[2], GUIStyle.op_Implicit("StoreBtn"), array))
			{
				by_warning = string.Empty;
				_CurrentPurchase = StorePurchase.CUBES2;
			}
			if (GUILayout.Button((Texture)(object)storeIcons[3], GUIStyle.op_Implicit("StoreBtn"), array))
			{
				by_warning = string.Empty;
				_CurrentPurchase = StorePurchase.CUBES3;
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (GUILayout.Button((Texture)(object)storeIcons[29], GUIStyle.op_Implicit("StoreBtn"), array))
			{
				by_warning = string.Empty;
				_CurrentPurchase = StorePurchase.GLASSCUBES;
			}
			if (GUILayout.Button((Texture)(object)storeIcons[30], GUIStyle.op_Implicit("StoreBtn"), array))
			{
				by_warning = string.Empty;
				_CurrentPurchase = StorePurchase.RESHETKACUBES;
			}
			if (GUILayout.Button((Texture)(object)storeIcons[53], GUIStyle.op_Implicit("StoreBtn"), array))
			{
				by_warning = string.Empty;
				_CurrentPurchase = StorePurchase.ICE;
			}
			if (GUILayout.Button((Texture)(object)storeIcons[55], GUIStyle.op_Implicit("StoreBtn"), array))
			{
				by_warning = string.Empty;
				_CurrentPurchase = StorePurchase.LAVA;
			}
			GUILayout.EndHorizontal();
			if (GUILayout.Button((Texture)(object)storeIcons[34], GUIStyle.op_Implicit("StoreBtn"), array))
			{
				by_warning = string.Empty;
				_CurrentPurchase = StorePurchase.WATER;
			}
		}
		GUILayout.EndScrollView();
	}

	private void DrawStoreRight()
	{
		//IL_0571: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04df: Unknown result type (might be due to invalid IL or missing references)
		GUILayout.Space(30f);
		int num = (int)(_CurrentPurchase - 5 - 1);
		if (num < 0)
		{
			return;
		}
		int one_pay_coins = ItemCosts[num].one_pay_coins;
		GUILayout.Label(storeItemName[num], new GUILayoutOption[0]);
		if (storeItemText[num].Length > 0)
		{
			GUILayout.Label(string.Empty, GUIStyle.op_Implicit("Divider"), new GUILayoutOption[0]);
			GUILayout.Label(storeItemText[num], GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		}
		if (_CurrentPurchase == StorePurchase.FLY || _CurrentPurchase == StorePurchase.SPEED)
		{
			GUILayout.Label(string.Empty, new GUILayoutOption[0]);
			GUILayout.Label("!Цифрами на клавиатуре можно активировать либо отключить способность!", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		}
		if (_CurrentPurchase == StorePurchase.MEDIUM_MAP)
		{
			GUILayout.Label(string.Empty, new GUILayoutOption[0]);
			GUILayout.Label("!Если у вас заняты все слоты, нажмите ПЕРЕГЕНЕРИРОВАТЬ один из слотов, чтобы создать карту нового размера!", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		}
		if (_CurrentPurchase == StorePurchase.FLAT_MAP)
		{
			GUILayout.Label((Texture)(object)MapPreview[0], new GUILayoutOption[0]);
			GUILayout.Label("!Если у вас заняты все слоты, нажмите ПЕРЕГЕНЕРИРОВАТЬ один из слотов, чтобы создать плоскую карту!", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		}
		if (_CurrentPurchase == StorePurchase.SAND_MAP)
		{
			GUILayout.Label((Texture)(object)MapPreview[1], new GUILayoutOption[0]);
			GUILayout.Label("!Если у вас заняты все слоты, нажмите ПЕРЕГЕНЕРИРОВАТЬ один из слотов, чтобы создать карту с пустыней!", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		}
		if (_CurrentPurchase == StorePurchase.OCEAN_MAP)
		{
			GUILayout.Label((Texture)(object)MapPreview[2], new GUILayoutOption[0]);
			GUILayout.Label("!Если у вас заняты все слоты, нажмите ПЕРЕГЕНЕРИРОВАТЬ один из слотов, чтобы создать карту с океаном!", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		}
		if (_CurrentPurchase == StorePurchase.ISLAND_MAP)
		{
			GUILayout.Label((Texture)(object)MapPreview[3], new GUILayoutOption[0]);
			GUILayout.Label("!Если у вас заняты все слоты, нажмите ПЕРЕГЕНЕРИРОВАТЬ один из слотов, чтобы создать карту c островами!", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		}
		if (_CurrentPurchase == StorePurchase.SNOW_MAP)
		{
			GUILayout.Label((Texture)(object)MapPreview[4], new GUILayoutOption[0]);
			GUILayout.Label("!Если у вас заняты все слоты, нажмите ПЕРЕГЕНЕРИРОВАТЬ один из слотов, чтобы создать снежную карту!", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		}
		GUILayout.Label(string.Empty, GUIStyle.op_Implicit("Divider"), new GUILayoutOption[0]);
		if (purchesImgInd[num] >= 0)
		{
			if (storeImgList[purchesImgInd[num]].img_list.Length == 1)
			{
				GUILayout.Label((Texture)(object)storeImgList[purchesImgInd[num]].img_list[0], new GUILayoutOption[0]);
			}
			if (storeImgList[purchesImgInd[num]].img_list.Length == 4)
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				for (int i = 0; i < storeImgList[purchesImgInd[num]].img_list.Length; i++)
				{
					GUILayout.Label((Texture)(object)storeImgList[purchesImgInd[num]].img_list[i], new GUILayoutOption[1] { GUILayout.MaxWidth(50f) });
				}
				GUILayout.EndHorizontal();
			}
			if (storeImgList[purchesImgInd[num]].img_list.Length > 4)
			{
				GUILayoutOption[] array = new GUILayoutOption[2]
				{
					GUILayout.MaxWidth(40f),
					GUILayout.MaxHeight(40f)
				};
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				for (int j = 0; j < 5; j++)
				{
					GUILayout.Label((Texture)(object)storeImgList[purchesImgInd[num]].img_list[j], GUIStyle.op_Implicit("StoreCubeSamples"), array);
				}
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				for (int k = 5; k < storeImgList[purchesImgInd[num]].img_list.Length; k++)
				{
					GUILayout.Label((Texture)(object)storeImgList[purchesImgInd[num]].img_list[k], GUIStyle.op_Implicit("StoreCubeSamples"), array);
				}
				if (storeImgList[purchesImgInd[num]].img_list.Length == 9)
				{
					GUILayout.Space(45f);
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.Label(string.Empty, GUIStyle.op_Implicit("Divider"), new GUILayoutOption[0]);
		}
		if (purchesType[num] == StoreItemType.MOB && Application.loadedLevelName == "Menu")
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(20f);
			GUILayout.Label((Texture)(object)mob_preview, new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (GUILayout.RepeatButton("<---", new GUILayoutOption[0]))
			{
				mobs_obj.transform.RotateAround(Vector3.up, 0.02f);
			}
			if (GUILayout.RepeatButton("--->", new GUILayoutOption[0]))
			{
				mobs_obj.transform.RotateAround(Vector3.up, -0.02f);
			}
			GUILayout.EndHorizontal();
			GUILayout.Label(string.Empty, GUIStyle.op_Implicit("Divider"), new GUILayoutOption[0]);
		}
		if (purchesType[num] == StoreItemType.SKINS)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(20f);
			GUILayout.Label((Texture)(object)player_preview, new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (GUILayout.RepeatButton("<---", new GUILayoutOption[0]))
			{
				preview_obj.transform.RotateAround(Vector3.up, 0.02f);
			}
			if (GUILayout.RepeatButton("--->", new GUILayoutOption[0]))
			{
				preview_obj.transform.RotateAround(Vector3.up, -0.02f);
			}
			GUILayout.EndHorizontal();
			GUILayout.Label(string.Empty, GUIStyle.op_Implicit("Divider"), new GUILayoutOption[0]);
		}
		if ((purchesType[num] == StoreItemType.ENTITY || purchesType[num] == StoreItemType.MOB) && !ItemCosts[num].is_one_by)
		{
			GUILayout.Label("У вас : " + ProfileINI.GetPurchesValue(_CurrentPurchase), GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		}
		if (by_warning.Length > 0)
		{
			GUILayout.Label(by_warning, GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
			if (Application.loadedLevelName == "Menu" && GUILayout.Button("Купить кристаллы!", new GUILayoutOption[0]))
			{
				curWindow = Windows.ByCrystal;
			}
		}
		if (ProfileINI.GetPurchesValue(_CurrentPurchase) == 0 || (purchesType[num] == StoreItemType.ENTITY && !ItemCosts[num].is_one_by) || purchesType[num] == StoreItemType.MOB)
		{
			if (_CurrentPurchase == StorePurchase.KNIGHT_SKIN_2 && ProfileINI.GetPurchesValue(StorePurchase.KNIGHT_SKIN_1) == 0)
			{
				GUILayout.Label("Чтобы купить этот образ, у вас должен быть куплен образ 'Рыцарь'", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
				return;
			}
			if (_CurrentPurchase == StorePurchase.PIRATE_SKIN_2 && ProfileINI.GetPurchesValue(StorePurchase.PIRATE_SKIN_1) == 0)
			{
				GUILayout.Label("Чтобы купить этот образ, у вас должен быть куплен образ 'Пират'", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
				return;
			}
			if (_CurrentPurchase == StorePurchase.DEATH_KNIGHT_SKIN && ProfileINI.GetPurchesValue(StorePurchase.ZOMBIE_SKIN) == 0)
			{
				GUILayout.Label("Чтобы купить этот образ, у вас должен быть куплен образ 'Зомби'", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
				return;
			}
			GUILayout.Label("Цена :", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
			if (ItemCosts[num].is_time_pay && !ItemCosts[num].is_crystall)
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label((Texture)(object)coinsImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[0]);
				GUILayout.Label(ItemCosts[num].one_day.ToString(), GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[0]);
				if (GUILayout.Button("на день", new GUILayoutOption[0]))
				{
					if (ProfileINI.coins < ItemCosts[num].one_day)
					{
						by_warning = "Не хватает монет, новые монеты можно купить на кристаллы, либо получить ежедневным бонусом";
					}
					else
					{
						((MonoBehaviour)this).StartCoroutine(ByFromStoreCoins(_CurrentPurchase, ItemCosts[num].one_day, 1, 1));
					}
				}
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label((Texture)(object)coinsImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[0]);
				GUILayout.Label(ItemCosts[num].one_week.ToString(), GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[0]);
				if (GUILayout.Button("на неделю", new GUILayoutOption[0]))
				{
					if (ProfileINI.coins < ItemCosts[num].one_week)
					{
						by_warning = "Не хватает монет, новые монеты можно купить на кристаллы, либо получить ежедневным бонусом";
					}
					else
					{
						((MonoBehaviour)this).StartCoroutine(ByFromStoreCoins(_CurrentPurchase, ItemCosts[num].one_week, 1, 2));
					}
				}
				GUILayout.EndHorizontal();
				if (ItemCosts[num].one_mounth > 0)
				{
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
					GUILayout.Label((Texture)(object)coinsImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[0]);
					GUILayout.Label(ItemCosts[num].one_mounth.ToString(), GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[0]);
					if (GUILayout.Button("на месяц", new GUILayoutOption[0]))
					{
						if (ProfileINI.coins < ItemCosts[num].one_mounth)
						{
							by_warning = "Не хватает монет, новые монеты можно купить на кристаллы, либо получить ежедневным бонусом";
						}
						else
						{
							((MonoBehaviour)this).StartCoroutine(ByFromStoreCoins(_CurrentPurchase, ItemCosts[num].one_mounth, 1, 4));
						}
					}
					GUILayout.EndHorizontal();
				}
				if (ItemCosts[num].all_time <= 0)
				{
					return;
				}
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label((Texture)(object)coinsImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[0]);
				GUILayout.Label(ItemCosts[num].all_time.ToString(), GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[0]);
				if (GUILayout.Button("навсегда", new GUILayoutOption[0]))
				{
					if (ProfileINI.coins < ItemCosts[num].all_time)
					{
						by_warning = "Не хватает монет, новые монеты можно купить на кристаллы, либо получить ежедневным бонусом";
					}
					else
					{
						((MonoBehaviour)this).StartCoroutine(ByFromStoreCoins(_CurrentPurchase, ItemCosts[num].all_time, 1, 3));
					}
				}
				GUILayout.EndHorizontal();
				return;
			}
			if (ItemCosts[num].is_time_pay && ItemCosts[num].is_crystall)
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label((Texture)(object)crystalImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[0]);
				GUILayout.Label(ItemCosts[num].one_day.ToString(), GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[0]);
				if (GUILayout.Button("на день", new GUILayoutOption[0]))
				{
					if (ProfileINI.crystals < ItemCosts[num].one_day)
					{
						by_warning = "Не хватает кристаллов, новые кристаллы можно купить в магизне";
					}
					else
					{
						((MonoBehaviour)this).StartCoroutine(ByFromStoreCoins(_CurrentPurchase, ItemCosts[num].one_day, 1, 1));
					}
				}
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label((Texture)(object)crystalImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[0]);
				GUILayout.Label(ItemCosts[num].one_week.ToString(), GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[0]);
				if (GUILayout.Button("на неделю", new GUILayoutOption[0]))
				{
					if (ProfileINI.crystals < ItemCosts[num].one_week)
					{
						by_warning = "Не хватает кристаллов, новые кристаллы можно купить в магизне";
					}
					else
					{
						((MonoBehaviour)this).StartCoroutine(ByFromStoreCoins(_CurrentPurchase, ItemCosts[num].one_week, 1, 2));
					}
				}
				GUILayout.EndHorizontal();
				if (ItemCosts[num].one_mounth > 0)
				{
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
					GUILayout.Label((Texture)(object)crystalImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[0]);
					GUILayout.Label(ItemCosts[num].one_mounth.ToString(), GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[0]);
					if (GUILayout.Button("на месяц", new GUILayoutOption[0]))
					{
						if (ProfileINI.crystals < ItemCosts[num].one_mounth)
						{
							by_warning = "Не хватает кристаллов, новые кристаллы можно купить в магизне";
						}
						else
						{
							((MonoBehaviour)this).StartCoroutine(ByFromStoreCoins(_CurrentPurchase, ItemCosts[num].one_mounth, 1, 4));
						}
					}
					GUILayout.EndHorizontal();
				}
				if (ItemCosts[num].all_time <= 0)
				{
					return;
				}
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label((Texture)(object)crystalImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[0]);
				GUILayout.Label(ItemCosts[num].all_time.ToString(), GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[0]);
				if (GUILayout.Button("навсегда", new GUILayoutOption[0]))
				{
					if (ProfileINI.crystals < ItemCosts[num].all_time)
					{
						by_warning = "Не хватает кристаллов, новые кристаллы можно купить в магизне";
					}
					else
					{
						((MonoBehaviour)this).StartCoroutine(ByFromStoreCoins(_CurrentPurchase, ItemCosts[num].all_time, 1, 3));
					}
				}
				GUILayout.EndHorizontal();
				return;
			}
			if (!ItemCosts[num].is_crystall)
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label((Texture)(object)coinsImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[0]);
				GUILayout.Label(one_pay_coins.ToString(), GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[0]);
				if (GUILayout.Button("Купить", new GUILayoutOption[1] { GUILayout.MaxWidth(100f) }))
				{
					if (ProfileINI.coins < one_pay_coins)
					{
						by_warning = string.Empty;
					}
					else
					{
						int count = 1;
						if (_CurrentPurchase == StorePurchase.RAIL)
						{
							count = 20;
						}
						((MonoBehaviour)this).StartCoroutine(ByFromStoreCoins(_CurrentPurchase, one_pay_coins, count, 0));
					}
				}
				GUILayout.EndHorizontal();
				return;
			}
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label((Texture)(object)crystalImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[0]);
			GUILayout.Label(ItemCosts[num].crystal.ToString(), GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[0]);
			if (GUILayout.Button("Купить", new GUILayoutOption[1] { GUILayout.MaxWidth(100f) }))
			{
				if (ProfileINI.crystals < ItemCosts[num].crystal)
				{
					by_warning = "Не хватает кристаллов, новые кристаллы можно приобрести в магазине";
				}
				else if (_CurrentPurchase == StorePurchase.SLOTS)
				{
					((MonoBehaviour)this).StartCoroutine(ByFromStoreCrystal(_CurrentPurchase, ItemCosts[num].crystal, ProfileINI.GetPurchesValue(StorePurchase.SLOTS) + 2));
				}
				else
				{
					((MonoBehaviour)this).StartCoroutine(ByFromStoreCrystal(_CurrentPurchase, ItemCosts[num].crystal, 1));
				}
			}
			GUILayout.EndHorizontal();
			return;
		}
		GUILayout.Label("Куплено", new GUILayoutOption[0]);
		if (purchesType[num] == StoreItemType.SKINS)
		{
			if ((int)ProfileINI.skin == clotheInd[num])
			{
				GUILayout.Label("Это ваш текущий образ", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
			}
			else if (GUILayout.Button("Установить", new GUILayoutOption[0]))
			{
				((MonoBehaviour)this).StartCoroutine(ProfileINI.SetSkin(clotheInd[num]));
			}
		}
		else if (ProfileINI.GetPurchesTime(_CurrentPurchase).Year == 9999)
		{
			GUILayout.Label("Навсегда", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		}
		else if (ProfileINI.GetPurchesTime(_CurrentPurchase).Year > 2000)
		{
			GUILayout.Label("Действует до:", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
			GUILayout.Label(ProfileINI.GetPurchesTime(_CurrentPurchase).ToString(), GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		}
	}

	public IEnumerator UsePurches(StorePurchase purches)
	{
		WWWForm form = new WWWForm();
		form.AddField("viewer_id", VkontakteAPI.INSTANCE._viewerId);
		form.AddField("auth_key", VkontakteAPI.INSTANCE._authKey);
		if (App.instance.platform == App.ePlatform.ONDOKLASNIKI)
		{
			form.AddField("session_key", VkontakteAPI.INSTANCE._sessionKey);
		}
		form.AddField("p_ind", (int)purches);
		Random r = new Random();
		form.AddField("random", r.Next());
		yield return (object)new WWW(App.instance.GetCurScriptsUrl() + "use_purches.php", form);
	}

	private IEnumerator ByFromStoreCrystal(StorePurchase purches, int cost, int count)
	{
		if (!ProfileINI.CheckPurchesValue(purches))
		{
			yield break;
		}
		int packet_id = App.instance.random.Next();
		WWWForm form = new WWWForm();
		form.AddField("viewer_id", VkontakteAPI.INSTANCE._viewerId);
		form.AddField("auth_key", VkontakteAPI.INSTANCE._authKey);
		if (App.instance.platform == App.ePlatform.ONDOKLASNIKI)
		{
			form.AddField("session_key", VkontakteAPI.INSTANCE._sessionKey);
		}
		form.AddField("p_ind", (int)purches);
		form.AddField("count", count);
		form.AddField("random", packet_id);
		form.AddField("beta_key", cost.ToString());
		WWW php_load = new WWW(App.instance.GetCurScriptsUrl() + "by_from_store_crystal.php", form);
		yield return php_load;
		string server_text = php_load.text.Substring(0, php_load.text.IndexOf('\n')).Trim();
		string server_key = php_load.text.Substring(php_load.text.IndexOf('\n') + 1);
		if (server_key.Trim() == VkontakteAPI.MD52(server_text + "_" + packet_id + "_" + VkontakteAPI.INSTANCE.GetSecret()) && server_text.Trim() == "SUCCESS")
		{
			switch (purches)
			{
			case StorePurchase.SLOTS:
				ProfileINI.IncPurchesValue(purches, 1);
				break;
			case StorePurchase.NONE:
				curWindow = Windows.Battles;
				CreateGame();
				break;
			case StorePurchase.CHANGENICK:
				curWindow = Windows.Name;
				break;
			default:
				ProfileINI.IncPurchesValue(purches, count);
				break;
			}
			ProfileINI.crystals -= cost;
		}
	}

	private IEnumerator ByFromStoreCoins(StorePurchase purches, int cost, int count, int delta_ind)
	{
		if (!start_by_from_coins)
		{
			start_by_from_coins = true;
			if (ProfileINI.CheckPurchesValue(purches))
			{
				int packet_id = App.instance.random.Next();
				WWWForm form = new WWWForm();
				form.AddField("viewer_id", VkontakteAPI.INSTANCE._viewerId);
				form.AddField("auth_key", VkontakteAPI.INSTANCE._authKey);
				if (App.instance.platform == App.ePlatform.ONDOKLASNIKI)
				{
					form.AddField("session_key", VkontakteAPI.INSTANCE._sessionKey);
				}
				form.AddField("p_ind", (int)purches);
				form.AddField("count", count);
				form.AddField("date_delta", delta_ind);
				form.AddField("random", packet_id);
				string yoba_key = App.instance.GenerateKey(20);
				form.AddField("yoba_key", App.instance.KeyEncrypt(yoba_key));
				form.AddField("beta_key", cost.ToString());
				WWW php_load = new WWW(App.instance.GetCurScriptsUrl() + "by_from_store_coins.php", form);
				yield return php_load;
				string server_text = php_load.text.Substring(0, php_load.text.IndexOf('\n'));
				string server_key = php_load.text.Substring(php_load.text.IndexOf('\n') + 1);
				if (server_key.Trim() == VkontakteAPI.MD52(server_text + "_" + packet_id + "_" + yoba_key))
				{
					yoba_key = string.Empty;
					if (server_text.Trim() == "SUCCESS")
					{
						ProfileINI.IncPurchesValue(purches, count);
						ProfileINI.coins -= cost;
						if (delta_ind == 1)
						{
							ProfileINI.SetPurchesTime(purches, ProfileINI.server_time.AddDays(1.0));
						}
						if (delta_ind == 2)
						{
							ProfileINI.SetPurchesTime(purches, ProfileINI.server_time.AddDays(7.0));
						}
						if (delta_ind == 4)
						{
							ProfileINI.SetPurchesTime(purches, ProfileINI.server_time.AddDays(30.0));
						}
						if (delta_ind == 3)
						{
							DateTime time = new DateTime(9999, 12, 30);
							ProfileINI.SetPurchesTime(purches, time);
						}
						if (purches == StorePurchase.SPEED && Application.loadedLevelName == "Game")
						{
							SpeedBonus();
						}
						if (purches == StorePurchase.FLY && Application.loadedLevelName == "Game")
						{
							FlyBonus();
						}
					}
				}
			}
		}
		start_by_from_coins = false;
	}

	private void DrawByCrystal()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a9e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1015: Unknown result type (might be due to invalid IL or missing references)
		//IL_1038: Unknown result type (might be due to invalid IL or missing references)
		//IL_171b: Unknown result type (might be due to invalid IL or missing references)
		//IL_173e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0559: Unknown result type (might be due to invalid IL or missing references)
		//IL_057c: Unknown result type (might be due to invalid IL or missing references)
		//IL_156d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1590: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e34: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e57: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c87: Unknown result type (might be due to invalid IL or missing references)
		//IL_1caa: Unknown result type (might be due to invalid IL or missing references)
		if (App.instance.platform == App.ePlatform.ONDOKLASNIKI)
		{
			GUI.Box(new Rect(0f, 70f, 340f, 460f), string.Empty);
			GUILayout.BeginArea(new Rect(10f, 70f, 320f, 450f));
			GUILayout.Space(10f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(40f);
			GUILayout.Label("Купить золото", GUIStyle.op_Implicit("CrystallBy"), new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			GUILayout.Space(20f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label("700", GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[1] { GUILayout.MaxWidth(100f) });
			GUILayout.Label((Texture)(object)coinsImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[1] { GUILayout.MaxWidth(30f) });
			GUILayout.Label(" за ", new GUILayoutOption[0]);
			GUILayout.Label("25 OK", GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[0]);
			if (GUILayout.Button("Купить", new GUILayoutOption[1] { GUILayout.MaxWidth(100f) }))
			{
				Application.ExternalCall("PayGold", new object[1] { 25 });
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label("1400", GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[1] { GUILayout.MaxWidth(100f) });
			GUILayout.Label((Texture)(object)coinsImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[1] { GUILayout.MaxWidth(30f) });
			GUILayout.Label(" за ", new GUILayoutOption[0]);
			GUILayout.Label("50 OK", GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[0]);
			if (GUILayout.Button("Купить", new GUILayoutOption[1] { GUILayout.MaxWidth(100f) }))
			{
				Application.ExternalCall("PayGold", new object[1] { 50 });
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label("2800", GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[1] { GUILayout.MaxWidth(100f) });
			GUILayout.Label((Texture)(object)coinsImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[1] { GUILayout.MaxWidth(30f) });
			GUILayout.Label(" за ", new GUILayoutOption[0]);
			GUILayout.Label("100 OK", GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[0]);
			if (GUILayout.Button("Купить", new GUILayoutOption[1] { GUILayout.MaxWidth(100f) }))
			{
				Application.ExternalCall("PayGold", new object[1] { 100 });
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label("7000(+500 бонус)", GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[1] { GUILayout.MaxWidth(100f) });
			GUILayout.Label((Texture)(object)coinsImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[1] { GUILayout.MaxWidth(30f) });
			GUILayout.Label(" за ", new GUILayoutOption[0]);
			GUILayout.Label("250 OK", GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[0]);
			if (GUILayout.Button("Купить", new GUILayoutOption[1] { GUILayout.MaxWidth(100f) }))
			{
				Application.ExternalCall("PayGold", new object[1] { 250 });
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label("14000(+1500 бонус)", GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[1] { GUILayout.MaxWidth(100f) });
			GUILayout.Label((Texture)(object)coinsImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[1] { GUILayout.MaxWidth(30f) });
			GUILayout.Label(" за ", new GUILayoutOption[0]);
			GUILayout.Label("500 OK", GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[0]);
			if (GUILayout.Button("Купить", new GUILayoutOption[1] { GUILayout.MaxWidth(100f) }))
			{
				Application.ExternalCall("PayGold", new object[1] { 500 });
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label("28000(+4500 бонус)", GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[1] { GUILayout.MaxWidth(100f) });
			GUILayout.Label((Texture)(object)coinsImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[1] { GUILayout.MaxWidth(30f) });
			GUILayout.Label(" за ", new GUILayoutOption[0]);
			GUILayout.Label("1000 OK", GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[0]);
			if (GUILayout.Button("Купить", new GUILayoutOption[1] { GUILayout.MaxWidth(100f) }))
			{
				Application.ExternalCall("PayGold", new object[1] { 1000 });
			}
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
			GUI.Box(new Rect(350f, 70f, 340f, 460f), string.Empty);
			GUILayout.BeginArea(new Rect(360f, 70f, 320f, 450f));
			GUILayout.Space(10f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(40f);
			GUILayout.Label("Купить кристаллы", GUIStyle.op_Implicit("CrystallBy"), new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			GUILayout.Space(20f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label("6", GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[1] { GUILayout.MaxWidth(100f) });
			GUILayout.Label((Texture)(object)crystalImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[1] { GUILayout.MaxWidth(30f) });
			GUILayout.Label(" за ", new GUILayoutOption[0]);
			GUILayout.Label("25 OK", GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[0]);
			if (GUILayout.Button("Купить", new GUILayoutOption[1] { GUILayout.MaxWidth(100f) }))
			{
				Application.ExternalCall("PayCrys", new object[1] { 25 });
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label("12", GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[1] { GUILayout.MaxWidth(100f) });
			GUILayout.Label((Texture)(object)crystalImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[1] { GUILayout.MaxWidth(30f) });
			GUILayout.Label(" за ", new GUILayoutOption[0]);
			GUILayout.Label("50 OK", GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[0]);
			if (GUILayout.Button("Купить", new GUILayoutOption[1] { GUILayout.MaxWidth(100f) }))
			{
				Application.ExternalCall("PayCrys", new object[1] { 50 });
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label("24", GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[1] { GUILayout.MaxWidth(100f) });
			GUILayout.Label((Texture)(object)crystalImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[1] { GUILayout.MaxWidth(30f) });
			GUILayout.Label(" за ", new GUILayoutOption[0]);
			GUILayout.Label("100 OK", GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[0]);
			if (GUILayout.Button("Купить", new GUILayoutOption[1] { GUILayout.MaxWidth(100f) }))
			{
				Application.ExternalCall("PayCrys", new object[1] { 100 });
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label("60(+5 бонус)", GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[1] { GUILayout.MaxWidth(100f) });
			GUILayout.Label((Texture)(object)crystalImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[1] { GUILayout.MaxWidth(30f) });
			GUILayout.Label(" за ", new GUILayoutOption[0]);
			GUILayout.Label("250 OK", GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[0]);
			if (GUILayout.Button("Купить", new GUILayoutOption[1] { GUILayout.MaxWidth(100f) }))
			{
				Application.ExternalCall("PayCrys", new object[1] { 250 });
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label("120(+15 бонус)", GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[1] { GUILayout.MaxWidth(100f) });
			GUILayout.Label((Texture)(object)crystalImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[1] { GUILayout.MaxWidth(30f) });
			GUILayout.Label(" за ", new GUILayoutOption[0]);
			GUILayout.Label("500 OK", GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[0]);
			if (GUILayout.Button("Купить", new GUILayoutOption[1] { GUILayout.MaxWidth(100f) }))
			{
				Application.ExternalCall("PayCrys", new object[1] { 500 });
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label("240(+45 бонус)", GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[1] { GUILayout.MaxWidth(100f) });
			GUILayout.Label((Texture)(object)crystalImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[1] { GUILayout.MaxWidth(30f) });
			GUILayout.Label(" за ", new GUILayoutOption[0]);
			GUILayout.Label("1000 OK", GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[0]);
			if (GUILayout.Button("Купить", new GUILayoutOption[1] { GUILayout.MaxWidth(100f) }))
			{
				Application.ExternalCall("PayCrys", new object[1] { 1000 });
			}
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}
		if (App.instance.platform == App.ePlatform.WEBSITE)
		{
			GUI.Box(new Rect(0f, 70f, 340f, 460f), string.Empty);
			GUILayout.BeginArea(new Rect(10f, 70f, 320f, 450f));
			GUILayout.Space(10f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(40f);
			GUILayout.Label("Купить кристаллы", GUIStyle.op_Implicit("CrystallBy"), new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			GUILayout.Space(40f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label(ind1_count.ToString(), GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[1] { GUILayout.MinWidth(30f) });
			GUILayout.Label((Texture)(object)crystalImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[1] { GUILayout.MinWidth(150f) });
			if (GUILayout.Button("Купить", new GUILayoutOption[1] { GUILayout.MaxWidth(100f) }))
			{
				WebCallPayStation(ind1_count);
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label(ind2_count.ToString(), GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[1] { GUILayout.MinWidth(30f) });
			GUILayout.Label((Texture)(object)crystalImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[1] { GUILayout.MinWidth(150f) });
			if (GUILayout.Button("Купить", new GUILayoutOption[1] { GUILayout.MaxWidth(100f) }))
			{
				WebCallPayStation(ind2_count);
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label(ind3_count.ToString(), GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[1] { GUILayout.MinWidth(30f) });
			GUILayout.Label((Texture)(object)crystalImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[1] { GUILayout.MinWidth(150f) });
			if (GUILayout.Button("Купить", new GUILayoutOption[1] { GUILayout.MaxWidth(100f) }))
			{
				WebCallPayStation(ind3_count);
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label(ind4_count.ToString(), GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[1] { GUILayout.MinWidth(30f) });
			GUILayout.Label((Texture)(object)crystalImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[1] { GUILayout.MinWidth(150f) });
			if (GUILayout.Button("Купить", new GUILayoutOption[1] { GUILayout.MaxWidth(100f) }))
			{
				WebCallPayStation(ind4_count);
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label(ind5_count.ToString(), GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[1] { GUILayout.MinWidth(30f) });
			GUILayout.Label((Texture)(object)crystalImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[1] { GUILayout.MinWidth(150f) });
			if (GUILayout.Button("Купить", new GUILayoutOption[1] { GUILayout.MaxWidth(100f) }))
			{
				WebCallPayStation(ind5_count);
			}
			GUILayout.EndHorizontal();
			if (GUILayout.Button("Приобрести произвольное число", new GUILayoutOption[1] { GUILayout.MinHeight(50f) }))
			{
				WebCallPayStation(0);
			}
			GUILayout.EndArea();
			GUI.Box(new Rect(350f, 70f, 340f, 460f), string.Empty);
			GUILayout.BeginArea(new Rect(360f, 70f, 320f, 450f));
			GUILayout.Space(10f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(40f);
			GUILayout.Label("Купить золото", GUIStyle.op_Implicit("CrystallBy"), new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			GUILayout.Space(40f);
			GUILayout.Label("Золото, меняется за кристаллы.", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
			GUILayout.Label("1 кристалл = 100 золотых монет", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
			if (ProfileINI.crystals > 0)
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label((Texture)(object)crystalImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[0]);
				GUILayout.Label(((int)crystall_to_sell).ToString(), GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[0]);
				GUILayout.Space(180f);
				GUILayout.Label(((int)crystall_to_sell * 100).ToString(), GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[0]);
				GUILayout.Label((Texture)(object)coinsImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[0]);
				GUILayout.EndHorizontal();
				crystall_to_sell = GUILayout.HorizontalSlider(crystall_to_sell, 0f, (float)ProfileINI.crystals, new GUILayoutOption[0]);
				if (GUILayout.Button("Купить", new GUILayoutOption[0]))
				{
					((MonoBehaviour)this).StartCoroutine(SellCrystall((int)crystall_to_sell));
				}
			}
			else
			{
				GUILayout.Label("Недостаточно кристалов для покупки золота", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
			}
			GUILayout.EndArea();
		}
		if (App.instance.platform == App.ePlatform.VKONTATE)
		{
			GUI.Box(new Rect(0f, 70f, 340f, 460f), string.Empty);
			GUILayout.BeginArea(new Rect(10f, 70f, 320f, 450f));
			GUILayout.Space(10f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(40f);
			GUILayout.Label("Купить кристаллы", GUIStyle.op_Implicit("CrystallBy"), new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			GUILayout.Space(30f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label(ind1_count.ToString(), GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[1] { GUILayout.MinWidth(115f) });
			GUILayout.Label((Texture)(object)crystalImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[1] { GUILayout.MaxWidth(30f) });
			GUILayout.Label(" за ", new GUILayoutOption[0]);
			GUILayout.Label(ind1_voute.ToString(), GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[1] { GUILayout.MinWidth(20f) });
			GUILayout.Label((Texture)(object)vkImg, new GUILayoutOption[1] { GUILayout.MaxWidth(20f) });
			if (GUILayout.Button("Купить", new GUILayoutOption[1] { GUILayout.MaxWidth(100f) }))
			{
				Application.ExternalCall("ByCryss6", new object[0]);
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label("10 (+ 5 бонус)", GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[1] { GUILayout.MinWidth(115f) });
			GUILayout.Label((Texture)(object)crystalImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[1] { GUILayout.MaxWidth(30f) });
			GUILayout.Label(" за ", new GUILayoutOption[0]);
			GUILayout.Label(ind2_voute.ToString(), GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[1] { GUILayout.MinWidth(20f) });
			GUILayout.Label((Texture)(object)vkImg, new GUILayoutOption[1] { GUILayout.MaxWidth(20f) });
			if (GUILayout.Button("Купить", new GUILayoutOption[1] { GUILayout.MaxWidth(100f) }))
			{
				Application.ExternalCall("ByCryss15", new object[0]);
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label("20 (+ 10 бонус)", GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[1] { GUILayout.MinWidth(115f) });
			GUILayout.Label((Texture)(object)crystalImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[1] { GUILayout.MaxWidth(30f) });
			GUILayout.Label(" за ", new GUILayoutOption[0]);
			GUILayout.Label(ind3_voute.ToString(), GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[1] { GUILayout.MinWidth(20f) });
			GUILayout.Label((Texture)(object)vkImg, new GUILayoutOption[1] { GUILayout.MaxWidth(20f) });
			if (GUILayout.Button("Купить", new GUILayoutOption[1] { GUILayout.MaxWidth(100f) }))
			{
				Application.ExternalCall("ByCryss30", new object[0]);
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label("40 (+ 20 бонус)", GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[1] { GUILayout.MinWidth(115f) });
			GUILayout.Label((Texture)(object)crystalImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[1] { GUILayout.MaxWidth(30f) });
			GUILayout.Label(" за ", new GUILayoutOption[0]);
			GUILayout.Label(ind4_voute.ToString(), GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[1] { GUILayout.MinWidth(20f) });
			GUILayout.Label((Texture)(object)vkImg, new GUILayoutOption[1] { GUILayout.MaxWidth(20f) });
			if (GUILayout.Button("Купить", new GUILayoutOption[1] { GUILayout.MaxWidth(100f) }))
			{
				Application.ExternalCall("ByCryss60", new object[0]);
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label("80 (+ 40 бонус)", GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[1] { GUILayout.MinWidth(115f) });
			GUILayout.Label((Texture)(object)crystalImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[1] { GUILayout.MaxWidth(30f) });
			GUILayout.Label(" за ", new GUILayoutOption[0]);
			GUILayout.Label(ind5_voute.ToString(), GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[1] { GUILayout.MinWidth(20f) });
			GUILayout.Label((Texture)(object)vkImg, new GUILayoutOption[1] { GUILayout.MaxWidth(20f) });
			if (GUILayout.Button("Купить", new GUILayoutOption[1] { GUILayout.MaxWidth(100f) }))
			{
				Application.ExternalCall("ByCryss120", new object[0]);
			}
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
			GUI.Box(new Rect(350f, 70f, 340f, 460f), string.Empty);
			GUILayout.BeginArea(new Rect(360f, 70f, 320f, 450f));
			GUILayout.Space(10f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(40f);
			GUILayout.Label("Купить золото", GUIStyle.op_Implicit("CrystallBy"), new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			GUILayout.Space(40f);
			if (ProfileINI.crystals > 0)
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label((Texture)(object)crystalImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[0]);
				GUILayout.Label(((int)crystall_to_sell).ToString(), GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[0]);
				GUILayout.Space(180f);
				GUILayout.Label(((int)crystall_to_sell * 100).ToString(), GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[0]);
				GUILayout.Label((Texture)(object)coinsImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[0]);
				GUILayout.EndHorizontal();
				crystall_to_sell = GUILayout.HorizontalSlider(crystall_to_sell, 0f, (float)ProfileINI.crystals, new GUILayoutOption[0]);
				if (GUILayout.Button("Купить", new GUILayoutOption[0]))
				{
					((MonoBehaviour)this).StartCoroutine(SellCrystall((int)crystall_to_sell));
				}
			}
			else
			{
				GUILayout.Label("Недостаточно кристалов для покупки золота", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
			}
			GUILayout.EndArea();
		}
		if (App.instance.platform != App.ePlatform.MOYMIR)
		{
			return;
		}
		GUI.Box(new Rect(0f, 70f, 340f, 460f), string.Empty);
		GUILayout.BeginArea(new Rect(10f, 70f, 320f, 450f));
		GUILayout.Space(10f);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Space(40f);
		GUILayout.Label("Купить кристаллы", GUIStyle.op_Implicit("CrystallBy"), new GUILayoutOption[0]);
		GUILayout.EndHorizontal();
		GUILayout.Space(30f);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("6", GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[1] { GUILayout.MinWidth(80f) });
		GUILayout.Label((Texture)(object)crystalImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[1] { GUILayout.MaxWidth(30f) });
		GUILayout.Label(" за ", new GUILayoutOption[0]);
		GUILayout.Label("25", GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[1] { GUILayout.MinWidth(40f) });
		GUILayout.Label((Texture)(object)mmImg, new GUILayoutOption[1] { GUILayout.MaxWidth(20f) });
		if (GUILayout.Button("Купить", new GUILayoutOption[1] { GUILayout.MaxWidth(100f) }))
		{
			Application.ExternalCall("Pay", new object[1] { 25 });
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("10 + 5 бонус", GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[1] { GUILayout.MinWidth(80f) });
		GUILayout.Label((Texture)(object)crystalImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[1] { GUILayout.MaxWidth(30f) });
		GUILayout.Label(" за ", new GUILayoutOption[0]);
		GUILayout.Label("40", GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[1] { GUILayout.MinWidth(40f) });
		GUILayout.Label((Texture)(object)mmImg, new GUILayoutOption[1] { GUILayout.MaxWidth(20f) });
		if (GUILayout.Button("Купить", new GUILayoutOption[1] { GUILayout.MaxWidth(100f) }))
		{
			Application.ExternalCall("Pay", new object[1] { 40 });
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("20 + 10 бонус", GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[1] { GUILayout.MinWidth(80f) });
		GUILayout.Label((Texture)(object)crystalImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[1] { GUILayout.MaxWidth(30f) });
		GUILayout.Label(" за ", new GUILayoutOption[0]);
		GUILayout.Label("80", GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[1] { GUILayout.MinWidth(40f) });
		GUILayout.Label((Texture)(object)mmImg, new GUILayoutOption[1] { GUILayout.MaxWidth(20f) });
		if (GUILayout.Button("Купить", new GUILayoutOption[1] { GUILayout.MaxWidth(100f) }))
		{
			Application.ExternalCall("Pay", new object[1] { 80 });
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("40 + 20 бонус", GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[1] { GUILayout.MinWidth(80f) });
		GUILayout.Label((Texture)(object)crystalImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[1] { GUILayout.MaxWidth(30f) });
		GUILayout.Label(" за ", new GUILayoutOption[0]);
		GUILayout.Label("160", GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[1] { GUILayout.MinWidth(40f) });
		GUILayout.Label((Texture)(object)mmImg, new GUILayoutOption[1] { GUILayout.MaxWidth(20f) });
		if (GUILayout.Button("Купить", new GUILayoutOption[1] { GUILayout.MaxWidth(100f) }))
		{
			Application.ExternalCall("Pay", new object[1] { 160 });
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("80 + 40 бонус", GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[1] { GUILayout.MinWidth(80f) });
		GUILayout.Label((Texture)(object)crystalImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[1] { GUILayout.MaxWidth(30f) });
		GUILayout.Label(" за ", new GUILayoutOption[0]);
		GUILayout.Label("220", GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[1] { GUILayout.MinWidth(40f) });
		GUILayout.Label((Texture)(object)mmImg, new GUILayoutOption[1] { GUILayout.MaxWidth(20f) });
		if (GUILayout.Button("Купить", new GUILayoutOption[1] { GUILayout.MaxWidth(100f) }))
		{
			Application.ExternalCall("Pay", new object[1] { 220 });
		}
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
		GUI.Box(new Rect(350f, 70f, 340f, 460f), string.Empty);
		GUILayout.BeginArea(new Rect(360f, 70f, 320f, 450f));
		GUILayout.Space(10f);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Space(40f);
		GUILayout.Label("Купить золото", GUIStyle.op_Implicit("CrystallBy"), new GUILayoutOption[0]);
		GUILayout.EndHorizontal();
		GUILayout.Space(40f);
		if (ProfileINI.crystals > 0)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label((Texture)(object)crystalImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[0]);
			GUILayout.Label(((int)crystall_to_sell).ToString(), GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[0]);
			GUILayout.Space(180f);
			GUILayout.Label(((int)crystall_to_sell * 100).ToString(), GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[0]);
			GUILayout.Label((Texture)(object)coinsImg, GUIStyle.op_Implicit("CrystallImg"), new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			crystall_to_sell = GUILayout.HorizontalSlider(crystall_to_sell, 0f, (float)ProfileINI.crystals, new GUILayoutOption[0]);
			if (GUILayout.Button("Купить", new GUILayoutOption[0]))
			{
				((MonoBehaviour)this).StartCoroutine(SellCrystall((int)crystall_to_sell));
			}
		}
		else
		{
			GUILayout.Label("Недостаточно кристалов для покупки золота", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		}
		GUILayout.EndArea();
	}

	public IEnumerator SellCrystall(int count)
	{
		WWWForm form = new WWWForm();
		form.AddField("viewer_id", VkontakteAPI.INSTANCE._viewerId);
		form.AddField("auth_key", VkontakteAPI.INSTANCE._authKey);
		if (App.instance.platform == App.ePlatform.ONDOKLASNIKI)
		{
			form.AddField("session_key", VkontakteAPI.INSTANCE._sessionKey);
		}
		form.AddField("count", count);
		Random r = new Random();
		form.AddField("random", r.Next());
		WWW php_load = new WWW(App.instance.GetCurScriptsUrl() + "sell_crystalls.php", form);
		yield return php_load;
		MonoBehaviour.print((object)php_load.text);
		if (php_load.text.Trim() == "SUCCSES")
		{
			ProfileINI.crystals -= count;
			ProfileINI.coins += count * 100;
		}
	}

	public IEnumerator ByCrystalls(int ind)
	{
		last_ind = ind;
		WWWForm form = new WWWForm();
		form.AddField("viewer_id", VkontakteAPI.INSTANCE._viewerId);
		form.AddField("auth_key", VkontakteAPI.INSTANCE._authKey);
		form.AddField("ind", ind);
		Random r = new Random();
		form.AddField("random", r.Next());
		WWW php_load = new WWW(App.instance.GetCurScriptsUrl() + "by_crystalls.php", form);
		yield return php_load;
		Debug.Log((object)php_load.text);
		if (php_load.text.Trim() == "SUCCESS")
		{
			if (ind == 1)
			{
				ProfileINI.crystals += ind1_count;
			}
			if (ind == 2)
			{
				ProfileINI.crystals += ind2_count;
			}
			if (ind == 3)
			{
				ProfileINI.crystals += ind3_count;
			}
			if (ind == 4)
			{
				ProfileINI.crystals += ind4_count;
			}
			if (ind == 5)
			{
				ProfileINI.crystals += ind5_count;
			}
		}
		if (php_load.text.Trim() == "NOT_MONEY")
		{
			if (ind == 1)
			{
				Application.ExternalCall("ShowPaymentBox", new object[1] { 3 });
			}
			if (ind == 2)
			{
				Application.ExternalCall("ShowPaymentBox", new object[1] { 5 });
			}
			if (ind == 3)
			{
				Application.ExternalCall("ShowPaymentBox", new object[1] { 10 });
			}
			if (ind == 4)
			{
				Application.ExternalCall("ShowPaymentBox", new object[1] { 20 });
			}
			if (ind == 5)
			{
				Application.ExternalCall("ShowPaymentBox", new object[1] { 40 });
			}
		}
	}

	private void DrawOptions()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		GUI.Box(new Rect(0f, 70f, 340f, 450f), "Настройки");
		GUILayout.BeginArea(new Rect(10f, 70f, 320f, 450f));
		optionsListScroll = GUILayout.BeginScrollView(optionsListScroll, new GUILayoutOption[0]);
		GUILayout.Space(30f);
		GUILayout.Space(10f);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("Чуствительность мыши", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[1] { GUILayout.MinWidth(150f) });
		ProfileINI.mouse_sens = GUILayout.HorizontalSlider(ProfileINI.mouse_sens, 1f, 30f, new GUILayoutOption[1] { GUILayout.MaxWidth(150f) });
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("Громкость звуков", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[1] { GUILayout.MinWidth(150f) });
		ProfileINI.sound_volume = GUILayout.HorizontalSlider(ProfileINI.sound_volume, 0f, 1f, new GUILayoutOption[1] { GUILayout.MaxWidth(150f) });
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("Громкость природы", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[1] { GUILayout.MinWidth(150f) });
		ProfileINI.ambient_volume = GUILayout.HorizontalSlider(ProfileINI.ambient_volume, 0f, 1f, new GUILayoutOption[1] { GUILayout.MaxWidth(150f) });
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		options_destroy.Draw();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("Показывать фразы над игроками", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[1] { GUILayout.MinWidth(150f) });
		ProfileINI.showBaloons = GUILayout.Toggle(ProfileINI.showBaloons, string.Empty, new GUILayoutOption[1] { GUILayout.MaxWidth(150f) });
		GUILayout.EndHorizontal();
		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}

	private void DrawMapList()
	{
		if (Application.loadedLevelName == "Menu")
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (GUILayout.Button("Лучшие", new GUILayoutOption[0]))
			{
				mapWindows = MapWindows.BEST_MAPS_DAY;
				_SelectedMapID = -2;
				cur_map_page = 0;
				((MonoBehaviour)this).StartCoroutine(RefreshListOfDay(cur_map_page));
			}
			if (GUILayout.Button("Новые", new GUILayoutOption[0]))
			{
				mapWindows = MapWindows.NEW_MAPS;
				_SelectedMapID = -2;
				cur_map_page = 0;
				((MonoBehaviour)this).StartCoroutine(RefreshListOfMapsNew(cur_map_page));
			}
			GUILayout.EndHorizontal();
		}
		if (mapWindows == MapWindows.BEST_MAPS_WEEK || mapWindows == MapWindows.BEST_MAPS_MONTH || mapWindows == MapWindows.BEST_MAPS_ALL_TIME || mapWindows == MapWindows.BEST_MAPS_DAY)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (GUILayout.Button("За день", new GUILayoutOption[0]))
			{
				_SelectedMapID = -2;
				cur_map_page = 0;
				((MonoBehaviour)this).StartCoroutine(RefreshListOfDay(cur_map_page));
				mapWindows = MapWindows.BEST_MAPS_DAY;
			}
			if (GUILayout.Button("Неделя", new GUILayoutOption[0]))
			{
				_SelectedMapID = -2;
				cur_map_page = 0;
				((MonoBehaviour)this).StartCoroutine(RefreshListOfMapsWeekly(cur_map_page));
				mapWindows = MapWindows.BEST_MAPS_WEEK;
			}
			if (GUILayout.Button("Месяц", new GUILayoutOption[0]))
			{
				_SelectedMapID = -2;
				cur_map_page = 0;
				((MonoBehaviour)this).StartCoroutine(RefreshListOfMapsMonthly(cur_map_page));
				mapWindows = MapWindows.BEST_MAPS_MONTH;
			}
			GUILayout.EndHorizontal();
		}
		if (mapWindows == MapWindows.SEARCH_MAPS || mapWindows == MapWindows.SEARCH_NAME || mapWindows == MapWindows.SEARCH_AUTHOR || mapWindows == MapWindows.SEARCH_RESULT)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (GUILayout.Button("По названию", new GUILayoutOption[0]))
			{
				_SelectedMapID = -2;
				cur_map_page = 0;
				mapWindows = MapWindows.SEARCH_NAME;
			}
			if (GUILayout.Button("По автору", new GUILayoutOption[0]))
			{
				_SelectedMapID = -2;
				cur_map_page = 0;
				mapWindows = MapWindows.SEARCH_AUTHOR;
			}
			GUILayout.EndHorizontal();
		}
		if (mapWindows == MapWindows.SEARCH_RESULT)
		{
			GUILayout.Label("Результаты поиска:", new GUILayoutOption[0]);
			if (count_page == -1)
			{
				GUILayout.Label("По Вашему запросу ничего не найдено.", new GUILayoutOption[0]);
			}
		}
		if (mapWindows == MapWindows.NEW_MAPS)
		{
			GUILayout.Label("[Обновляется раз в минуту]", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
			GUILayout.Label("Последние опубликованные", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		}
		if (mapWindows == MapWindows.BEST_MAPS_WEEK)
		{
			GUILayout.Label("[Обновляется раз в минуту]", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
			GUILayout.Label("Лучшие карты этой недели", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		}
		if (mapWindows == MapWindows.BEST_MAPS_MONTH)
		{
			GUILayout.Label("[Обновляется раз в минуту]", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
			GUILayout.Label("Лучшие карты этого месяца", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		}
		if (mapWindows == MapWindows.BEST_MAPS_DAY)
		{
			GUILayout.Label("[Обновляется раз в минуту]", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
			GUILayout.Label("Лучшие карты этого дня", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		}
		if (mapWindows == MapWindows.BEST_MAPS_ALL_TIME)
		{
			GUILayout.Label("Лучшие карты за все время,добавляются модератором", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		}
		if (mapWindows == MapWindows.NEW_MAPS || mapWindows == MapWindows.BEST_MAPS_WEEK || mapWindows == MapWindows.BEST_MAPS_MONTH || mapWindows == MapWindows.BEST_MAPS_ALL_TIME || mapWindows == MapWindows.BEST_MAPS_DAY || mapWindows == MapWindows.SEARCH_RESULT)
		{
			DrawListOfPublicMaps();
		}
		if (mapWindows == MapWindows.SEARCH_NAME && mapWindows != MapWindows.SEARCH_RESULT)
		{
			search_type = 0;
			GUILayout.Label("Название карты (Мин. 3 симв.):", new GUILayoutOption[0]);
			search_map_name = GUILayout.TextField(search_map_name, new GUILayoutOption[0]);
			GUILayout.Label("Сортировать по:", new GUILayoutOption[0]);
			sort_type = GUILayout.Toggle(sort_type, "По рейтингу", new GUILayoutOption[0]);
			if (sort_type)
			{
				sort_type2 = false;
			}
			else if (!sort_type2)
			{
				sort_type = true;
			}
			sort_type2 = GUILayout.Toggle(sort_type2, "По дате", new GUILayoutOption[0]);
			if (sort_type2)
			{
				sort_type = false;
			}
			else if (!sort_type)
			{
				sort_type2 = true;
			}
			if (GUILayout.Button("Искать", new GUILayoutOption[0]))
			{
				if (search_map_name.Length < 3)
				{
					show_search_error = true;
				}
				else
				{
					show_search_error = false;
					mapWindows = MapWindows.SEARCH_RESULT;
					((MonoBehaviour)this).StartCoroutine(SearchMaps(0));
				}
			}
		}
		if (mapWindows == MapWindows.SEARCH_AUTHOR && mapWindows != MapWindows.SEARCH_RESULT)
		{
			search_type = 1;
			GUILayout.Label("Автор карты (Мин. 3 симв.):", new GUILayoutOption[0]);
			search_map_author = GUILayout.TextField(search_map_author, new GUILayoutOption[0]);
			GUILayout.Label("Сортировать по:", new GUILayoutOption[0]);
			sort_type = GUILayout.Toggle(sort_type, "По рейтингу", new GUILayoutOption[0]);
			if (sort_type)
			{
				sort_type2 = false;
			}
			else if (!sort_type2)
			{
				sort_type = true;
			}
			sort_type2 = GUILayout.Toggle(sort_type2, "По дате", new GUILayoutOption[0]);
			if (sort_type2)
			{
				sort_type = false;
			}
			else if (!sort_type)
			{
				sort_type2 = true;
			}
			if (GUILayout.Button("Искать", new GUILayoutOption[0]))
			{
				if (search_map_author.Length < 3)
				{
					show_search_error = true;
				}
				else
				{
					show_search_error = false;
					mapWindows = MapWindows.SEARCH_RESULT;
					((MonoBehaviour)this).StartCoroutine(SearchMaps(0));
				}
			}
		}
		if (show_search_error)
		{
			GUILayout.Label("Ошибка: нужно ввести не менее 3 символов!", new GUILayoutOption[0]);
		}
	}

	public bool FindOpenRoom(int map_id)
	{
		RoomInfo[] roomList = PhotonNetwork.GetRoomList();
		for (int i = 0; i < roomList.Length; i++)
		{
			if (roomList[i].customProperties.ContainsKey("public_id"))
			{
				int num = (int)roomList[i].customProperties["public_id"];
				if (num == map_id)
				{
					_BattlesMapName = (string)roomList[i].customProperties["map_name"];
					return true;
				}
			}
		}
		return false;
	}

	private void DrawListOfPublicMaps()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (publicMapsList == null)
		{
			return;
		}
		publicMapsScroll = GUILayout.BeginScrollView(publicMapsScroll, new GUILayoutOption[0]);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		publicMapsListSelect = GUILayout.SelectionGrid(publicMapsListSelect, publicMapsList, 1, new GUILayoutOption[1] { GUILayout.MaxWidth(280f) });
		if (publicMapsListSelect != publicMapsListId)
		{
			publicMapsListId = publicMapsListSelect;
			int map_id = Convert.ToInt32((Maps[publicMapsListId] as Map).mapId);
			_PublicMapOwnerID = (Maps[publicMapsListId] as Map).autor_id;
			_PublicMapOwnerSlot = Convert.ToInt32((Maps[publicMapsListId] as Map).autor_slot_id);
			_SelectedMapID = -2;
			draw_map_card_open_room = false;
			((MonoBehaviour)this).StartCoroutine(LoadMapInfo(map_id));
		}
		GUILayout.EndHorizontal();
		GUILayout.EndScrollView();
		if (count_page < max_page_view - 1)
		{
			if (cur_map_page + 1 <= count_page && GUILayout.Button("Загрузить ещё карты", new GUILayoutOption[0]))
			{
				((MonoBehaviour)this).StartCoroutine(LoadMoreMaps(cur_map_page + 1));
			}
		}
		else if (cur_map_page + 1 <= max_page_view - 1 && GUILayout.Button("Загрузить ещё карты", new GUILayoutOption[0]))
		{
			((MonoBehaviour)this).StartCoroutine(LoadMoreMaps(cur_map_page + 1));
		}
	}

	private void DrawMyMaps()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		GUILayout.Space(20f);
		my_maps_croll = GUILayout.BeginScrollView(my_maps_croll, new GUILayoutOption[0]);
		if (_SelectedSlot == 1)
		{
			GUIStyleState normal = GUI.skin.button.normal;
			Texture2D background = button_textures[4];
			GUI.skin.button.active.background = background;
			normal.background = background;
			GUIStyleState hover = GUI.skin.button.hover;
			background = button_textures[5];
			GUI.skin.button.onNormal.background = background;
			background = background;
			GUI.skin.button.onHover.background = background;
			background = background;
			GUI.skin.button.onFocused.background = background;
			background = background;
			GUI.skin.button.onActive.background = background;
			hover.background = background;
		}
		else
		{
			GUIStyleState normal2 = GUI.skin.button.normal;
			Texture2D background = button_textures[0];
			GUI.skin.button.active.background = background;
			normal2.background = background;
			GUIStyleState hover2 = GUI.skin.button.hover;
			background = button_textures[1];
			GUI.skin.button.onNormal.background = background;
			background = background;
			GUI.skin.button.onHover.background = background;
			background = background;
			GUI.skin.button.onFocused.background = background;
			background = background;
			GUI.skin.button.onActive.background = background;
			hover2.background = background;
		}
		if (GUILayout.Button("Карта 1", new GUILayoutOption[1] { GUILayout.MaxHeight(50f) }) || (bs._Igor.autoJoin2 && _SelectedSlot != 1))
		{
			by_warning = string.Empty;
			_SelectedSlot = 1;
			_SelectedMapID = -2;
			battlesRightMenu = BatllesRightMenu.MY_MAPS;
			((MonoBehaviour)this).StartCoroutine(LoadSlotMapInfo(VkontakteAPI.INSTANCE._viewerId, _SelectedSlot));
		}
		for (int i = 0; i < ProfileINI.GetPurchesValue(StorePurchase.SLOTS); i++)
		{
			if (_SelectedSlot == i + 2)
			{
				GUIStyleState normal3 = GUI.skin.button.normal;
				Texture2D background = button_textures[4];
				GUI.skin.button.active.background = background;
				normal3.background = background;
				GUIStyleState hover3 = GUI.skin.button.hover;
				background = button_textures[5];
				GUI.skin.button.onNormal.background = background;
				background = background;
				GUI.skin.button.onHover.background = background;
				background = background;
				GUI.skin.button.onFocused.background = background;
				background = background;
				GUI.skin.button.onActive.background = background;
				hover3.background = background;
			}
			else
			{
				GUIStyleState normal4 = GUI.skin.button.normal;
				Texture2D background = button_textures[0];
				GUI.skin.button.active.background = background;
				normal4.background = background;
				GUIStyleState hover4 = GUI.skin.button.hover;
				background = button_textures[1];
				GUI.skin.button.onNormal.background = background;
				background = background;
				GUI.skin.button.onHover.background = background;
				background = background;
				GUI.skin.button.onFocused.background = background;
				background = background;
				GUI.skin.button.onActive.background = background;
				hover4.background = background;
			}
			if (GUILayout.Button("Карта " + (i + 2), new GUILayoutOption[1] { GUILayout.MaxHeight(50f) }))
			{
				by_warning = string.Empty;
				_SelectedSlot = 1 + i + 1;
				_SelectedMapID = -2;
				battlesRightMenu = BatllesRightMenu.MY_MAPS;
				((MonoBehaviour)this).StartCoroutine(LoadSlotMapInfo(VkontakteAPI.INSTANCE._viewerId, _SelectedSlot));
			}
		}
		FixMenuControl(1000);
		GUILayout.Label(string.Empty, GUIStyle.op_Implicit("Divider"), new GUILayoutOption[0]);
		if (by_warning.Length > 0)
		{
			GUILayout.Label(by_warning, GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
			if (Application.loadedLevelName == "Menu" && GUILayout.Button("Купить кристаллы", new GUILayoutOption[0]))
			{
				by_warning = string.Empty;
				curWindow = Windows.ByCrystal;
			}
		}
		else if (GUILayout.Button("Купить слот... [20 кристалов]", new GUILayoutOption[1] { GUILayout.MaxHeight(50f) }))
		{
			if (ProfileINI.crystals >= 20)
			{
				((MonoBehaviour)this).StartCoroutine(ByFromStoreCrystal(StorePurchase.SLOTS, 20, ProfileINI.GetPurchesValue(StorePurchase.SLOTS) + 2));
			}
			else
			{
				by_warning = "Нехватает кристалов, вы можете приобрести их в магазине";
			}
		}
		GUILayout.EndScrollView();
	}

	private void DrawSaveMapInfo()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		GUILayout.Space(30f);
		coments_scroll = GUILayout.BeginScrollView(coments_scroll, new GUILayoutOption[0]);
		if (_SelectedMapID == -2)
		{
			GUILayout.Label("Загружаю информацию...", new GUILayoutOption[0]);
		}
		if (_SelectedMapID == -1)
		{
			GUILayout.Label("Слот заблокирован, вы можете купить его.", new GUILayoutOption[0]);
			if (GUILayout.Button("Купить слот", new GUILayoutOption[0]))
			{
				curWindow = Windows.Store;
				_CurrentPurchase = StorePurchase.SLOTS;
			}
		}
		if (_SelectedMapID == 0)
		{
			GUILayout.Label("Свободный слот", new GUILayoutOption[0]);
			if (Application.loadedLevelName == "Game")
			{
				if (GUILayout.Button("Сохранить", new GUILayoutOption[0]))
				{
					((MonoBehaviour)this).StartCoroutine(World.Instance.SaveWorldToServer(_SelectedSlot));
				}
			}
			else if (GUILayout.Button("Создать карту", new GUILayoutOption[0]))
			{
				curWindow = Windows.Battles;
				CreateGame();
			}
		}
		if (_SelectedMapID > 0)
		{
			RoomInfo roomInfo = null;
			RoomInfo[] roomList = PhotonNetwork.GetRoomList();
			foreach (RoomInfo roomInfo2 in roomList)
			{
				if ((string)roomInfo2.customProperties["player_id"] == VkontakteAPI.INSTANCE._viewerId && (int)roomInfo2.customProperties["slot_id"] == _SelectedMapID)
				{
					roomInfo = roomInfo2;
					break;
				}
			}
			if (roomInfo == null || (mapWindows == MapWindows.MY_MAPS && Application.loadedLevelName == "Game"))
			{
				if (_SelectedMapID == 1)
				{
					if (!regen_fail)
					{
						GUILayout.Label("Тут есть карта", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
					}
					else
					{
						GUILayout.Label("Перегенерировать карту можно только 1 раз в день!", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
					}
				}
				else
				{
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
					GUILayout.Label("Карта опубликована", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
					if (vouteData == eVouteData.VOUTE_NOT && GUILayout.Button("Голос за", new GUILayoutOption[0]))
					{
						((MonoBehaviour)this).StartCoroutine(VouteYes(_SelectedMapID));
					}
					if (vouteData == eVouteData.VOUTE_YES)
					{
						GUILayout.Label("Вы за!", new GUILayoutOption[0]);
						if (GUILayout.Button("Отменить", new GUILayoutOption[0]))
						{
							((MonoBehaviour)this).StartCoroutine(VouteYesNot(_SelectedMapID));
						}
					}
					if (vouteData == eVouteData.VOUTE_NO)
					{
						GUILayout.Label("Вы против!", new GUILayoutOption[0]);
						if (GUILayout.Button("Отменить", new GUILayoutOption[0]))
						{
							((MonoBehaviour)this).StartCoroutine(VouteNoNot(_SelectedMapID));
						}
					}
					GUILayout.EndHorizontal();
					GUILayout.Label("Автор карты : " + public_map_authorname, GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
					GUILayout.Label("Очки      : " + _PublicMapRating, GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
					GUILayout.Label(string.Empty, GUIStyle.op_Implicit("Divider"), new GUILayoutOption[0]);
					GUILayoutOption[] array = new GUILayoutOption[4]
					{
						GUILayout.MinWidth(308f),
						GUILayout.MaxWidth(308f),
						GUILayout.MinHeight(136f),
						GUILayout.MaxHeight(136f)
					};
					GUILayout.Label((Texture)(object)((!((Object)(object)player_map_img != (Object)null)) ? standart_map_img : player_map_img), array);
				}
				GUILayout.Label(string.Empty, GUIStyle.op_Implicit("Divider"), new GUILayoutOption[0]);
				if (curWindow == Windows.Battles)
				{
					if (Application.loadedLevelName == "Menu" && GUILayout.Button("Загрузить...", new GUILayoutOption[0]))
					{
						if (draw_public_form)
						{
							App.instance.gameIni.server_name = public_map_name;
						}
						battlesRightMenu = BatllesRightMenu.SAVED_BATTLE;
					}
					if (_SelectedMapID > 1)
					{
						if (close_public_map_count == 1)
						{
							GUILayout.Label("Уверены?", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
						}
						if (GUILayout.Button("Снять с публикации", new GUILayoutOption[0]))
						{
							if (close_public_map_count == 1)
							{
								close_public_map_count = 0;
								draw_public_form = false;
								((MonoBehaviour)this).StartCoroutine(ClosePublicMap(VkontakteAPI.INSTANCE._viewerId, _SelectedSlot, _SelectedMapID));
							}
							else
							{
								close_public_map_count++;
							}
						}
					}
				}
				else if (Application.loadedLevelName == "Menu" && GUILayout.Button("Посмотреть", new GUILayoutOption[0]))
				{
					JoinOrCreateGame(_PublicMapOwnerID, _PublicMapOwnerSlot, isWatch: true);
				}
				if (mapWindows == MapWindows.MY_MAPS && Application.loadedLevelName == "Game" && !App.instance.gameIni.isWatch && GUILayout.Button("Сохранить", new GUILayoutOption[0]))
				{
					((MonoBehaviour)this).StartCoroutine(World.Instance.SaveWorldToServer(_SelectedSlot));
				}
				if (_SelectedMapID == 1)
				{
					if (Application.loadedLevelName == "Menu" && (GUILayout.Button("Перегенерировать...", new GUILayoutOption[0]) || bs._Igor.autoJoin2))
					{
						((MonoBehaviour)this).StartCoroutine(PreregenerateSlot(_SelectedSlot));
					}
					if (draw_public_form)
					{
						GUILayout.BeginHorizontal(new GUILayoutOption[0]);
						GUILayout.Label("Название карты:", new GUILayoutOption[0]);
						public_map_name = GUILayout.TextField(public_map_name, new GUILayoutOption[1] { GUILayout.MaxWidth(200f) });
						public_map_name = Utils.MaxStringLength(public_map_name, 16);
						GUILayout.EndHorizontal();
						GUILayout.BeginHorizontal(new GUILayoutOption[0]);
						GUILayout.Label("URL картинки:", new GUILayoutOption[0]);
						public_map_url = GUILayout.TextField(public_map_url, new GUILayoutOption[1] { GUILayout.MaxWidth(200f) });
						GUILayout.EndHorizontal();
						GUILayout.Label("Описание карты:", new GUILayoutOption[0]);
						public_map_about = GUILayout.TextArea(public_map_about, new GUILayoutOption[1] { GUILayout.MaxHeight(100f) });
						if (GUILayout.Button("Опубликовать", new GUILayoutOption[0]))
						{
							((MonoBehaviour)this).StartCoroutine(PublicMapToServer());
						}
					}
					else if (Application.loadedLevelName == "Menu" && battlesLeftMenu == BatllesLeftMenu.MY_MAPS && curWindow == Windows.Battles && GUILayout.Button("Опубликовать в топ карт", new GUILayoutOption[0]))
					{
						public_map_name = string.Empty;
						draw_public_form = true;
					}
				}
				if (_SelectedMapID > 1)
				{
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
					GUILayout.Label("Название карты:", new GUILayoutOption[0]);
					GUILayout.TextField(public_map_name, new GUILayoutOption[1] { GUILayout.MaxWidth(200f) });
					GUILayout.EndHorizontal();
					if (App.instance.platform == App.ePlatform.VKONTATE)
					{
						GUILayout.BeginHorizontal(new GUILayoutOption[0]);
						GUILayout.Label("Ссылка на карту:", new GUILayoutOption[0]);
						GUILayout.TextField("http://vkontakte.ru/app2721929#map_id_" + _SelectedMapID, new GUILayoutOption[1] { GUILayout.MaxWidth(200f) });
						GUILayout.EndHorizontal();
					}
					if (App.instance.platform == App.ePlatform.WEBSITE)
					{
						GUILayout.BeginHorizontal(new GUILayoutOption[0]);
						GUILayout.Label("Ссылка на карту:", new GUILayoutOption[0]);
						GUILayout.TextField("http://kopatel-online.ru/#map_id_" + _SelectedMapID, new GUILayoutOption[1] { GUILayout.MaxWidth(200f) });
						GUILayout.EndHorizontal();
					}
					if (App.instance.platform == App.ePlatform.ONDOKLASNIKI)
					{
						GUILayout.BeginHorizontal(new GUILayoutOption[0]);
						GUILayout.Label("Ссылка на карту:", new GUILayoutOption[0]);
						GUILayout.TextField("http://www.odnoklassniki.ru/game/kopatel-online?map_id_" + _SelectedMapID + "e", new GUILayoutOption[1] { GUILayout.MaxWidth(200f) });
						GUILayout.EndHorizontal();
					}
					if (App.instance.platform == App.ePlatform.MOYMIR)
					{
						GUILayout.BeginHorizontal(new GUILayoutOption[0]);
						GUILayout.Label("Ссылка на карту:", new GUILayoutOption[0]);
						GUILayout.TextField("http://my.mail.ru/apps/659403?#map_id_" + _SelectedMapID + "e", new GUILayoutOption[1] { GUILayout.MaxWidth(200f) });
						GUILayout.EndHorizontal();
					}
					GUILayout.Label("Описание карты:", new GUILayoutOption[0]);
					GUILayout.TextArea(public_map_about, new GUILayoutOption[1] { GUILayout.MaxHeight(100f) });
					GUILayout.Label(string.Empty, GUIStyle.op_Implicit("Divider"), new GUILayoutOption[0]);
					GUILayout.Label("Коментарии :", new GUILayoutOption[0]);
					for (int j = 0; j < Comments.Count; j++)
					{
						GUILayout.Label((Comments[j] as Comment).name, GUIStyle.op_Implicit("CommentName"), new GUILayoutOption[0]);
						GUILayout.BeginHorizontal(new GUILayoutOption[0]);
						GUILayout.Space(20f);
						GUILayout.Label((Comments[j] as Comment).text, new GUILayoutOption[0]);
						GUILayout.EndHorizontal();
					}
					if (count_comments > 1)
					{
						GUILayout.BeginHorizontal(new GUILayoutOption[0]);
						if (comments_start_ind <= 0)
						{
							GUI.enabled = false;
						}
						if (GUILayout.Button(" <<< ", new GUILayoutOption[0]))
						{
							((MonoBehaviour)this).StartCoroutine(GetComments(_SelectedMapID, 0));
						}
						if (GUILayout.Button(" < ", new GUILayoutOption[0]))
						{
							((MonoBehaviour)this).StartCoroutine(GetComments(_SelectedMapID, comments_start_ind - 1));
						}
						GUI.enabled = true;
						if (count_comments > 0)
						{
							GUILayout.Label("  " + (comments_start_ind + 1) + "/" + count_comments + "  ", new GUILayoutOption[0]);
						}
						else
						{
							GUILayout.Label("  " + (comments_start_ind + 1) + "/" + (count_comments + 1) + "  ", new GUILayoutOption[0]);
						}
						if (comments_start_ind + 1 >= count_comments)
						{
							GUI.enabled = false;
						}
						if (GUILayout.Button(" > ", new GUILayoutOption[0]))
						{
							((MonoBehaviour)this).StartCoroutine(GetComments(_SelectedMapID, comments_start_ind + 1));
						}
						if (GUILayout.Button(" >>> ", new GUILayoutOption[0]))
						{
							((MonoBehaviour)this).StartCoroutine(GetComments(_SelectedMapID, count_comments - 1));
						}
						GUI.enabled = true;
						GUILayout.EndHorizontal();
					}
					if (draw_comment_form)
					{
						GUILayout.Label("Текст комментария:", new GUILayoutOption[0]);
						post_text = GUILayout.TextArea(post_text, new GUILayoutOption[1] { GUILayout.MaxHeight(100f) });
						if (GUILayout.Button("Отправить", new GUILayoutOption[0]) && post_text.Length > 3)
						{
							((MonoBehaviour)this).StartCoroutine(PostComment(post_text, _SelectedMapID));
							post_text = string.Empty;
							draw_comment_form = false;
						}
					}
					else if (GUILayout.Button("Коментировать", new GUILayoutOption[0]))
					{
						draw_comment_form = true;
					}
				}
			}
			else
			{
				GUILayout.Label("Карта активна", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
				GUILayout.Label(string.Empty, GUIStyle.op_Implicit("Divider"), new GUILayoutOption[0]);
				if (GUILayout.Button("Загрузить...", new GUILayoutOption[0]))
				{
					JoinOrCreateGame(VkontakteAPI.INSTANCE._viewerId, _SelectedSlot, isWatch: false);
				}
			}
		}
		GUILayout.EndScrollView();
	}

	public IEnumerator SetPregenerateTimer(int slot_id)
	{
		WWWForm form = new WWWForm();
		form.AddField("id", VkontakteAPI.INSTANCE._viewerId);
		form.AddField("map_place", slot_id);
		Random r = new Random();
		form.AddField("random", r.Next());
		yield return (object)new WWW(App.instance.GetCurScriptsUrl() + "new_map_timer.php", form);
	}

	public IEnumerator PreregenerateSlot(int slot_id)
	{
		_SelectedMapID = -2;
		WWWForm form = new WWWForm();
		form.AddField("id", VkontakteAPI.INSTANCE._viewerId);
		form.AddField("map_place", slot_id);
		Random r = new Random();
		form.AddField("random", r.Next());
		yield return (object)new WWW(App.instance.GetCurScriptsUrl() + "is_peregenerate_map.php", form);
		_SelectedMapID = 1;
		curWindow = Windows.Battles;
		CreateGame();
		yield return 0;
	}

	public IEnumerator ClosePublicMap(string veiwer_id, int slot_id, int map_id)
	{
		WWWForm form = new WWWForm();
		form.AddField("id", veiwer_id);
		form.AddField("slot_id", slot_id);
		form.AddField("map_id", map_id);
		Random r = new Random();
		form.AddField("random", r.Next());
		form.AddField("auth_key", VkontakteAPI.INSTANCE._authKey);
		if (App.instance.platform == App.ePlatform.ONDOKLASNIKI)
		{
			form.AddField("session_key", VkontakteAPI.INSTANCE._sessionKey);
		}
		WWW php_load = new WWW(App.instance.GetCurScriptsUrl() + "close_map.php", form);
		yield return php_load;
		Debug.Log((object)php_load.text);
		yield return ((MonoBehaviour)this).StartCoroutine(LoadSlotMapInfo(veiwer_id, slot_id));
	}

	public IEnumerator PostComment(string text, int map_id)
	{
		WWWForm form = new WWWForm();
		form.AddField("id", VkontakteAPI.INSTANCE._viewerId);
		form.AddField("post_name", ProfileINI.nickname);
		form.AddField("post_text", text);
		form.AddField("map_id", map_id);
		Random r = new Random();
		form.AddField("random", r.Next());
		yield return (object)new WWW(App.instance.GetCurScriptsUrl() + "post_comment.php", form);
		((MonoBehaviour)this).StartCoroutine(GetComments(map_id, comments_start_ind));
	}

	public IEnumerator GetComments(int map_id, int _page_numb)
	{
		comments_start_ind += _page_numb - comments_start_ind;
		Comments.Clear();
		WWWForm form = new WWWForm();
		form.AddField("map_id", map_id);
		form.AddField("start_ind", comments_start_ind);
		Debug.Log((object)"GetComentStartIndex");
		Debug.Log((object)comments_start_ind);
		Random r = new Random();
		form.AddField("random", r.Next());
		WWW php_load = new WWW(App.instance.GetCurScriptsUrl() + "get_coments.php", form);
		yield return php_load;
		int ind = php_load.text.IndexOf('|');
		count_comments = Convert.ToInt32(php_load.text.Substring(0, ind));
		string text = php_load.text.Remove(0, ind + 1);
		string[] comments = text.Split('|');
		for (int i = 1; i < comments.Length; i++)
		{
			string[] param = comments[i].Split('~');
			Comment comment = new Comment
			{
				id = param[0],
				name = param[1],
				text = param[2]
			};
			Comments.Add(comment);
		}
		Debug.Log((object)("CL " + count_comments));
	}

	public IEnumerator PublicMapToServer()
	{
		_SelectedMapID = -2;
		WWWForm form = new WWWForm();
		form.AddField("id", VkontakteAPI.INSTANCE._viewerId);
		form.AddField("slot_id", _SelectedSlot);
		form.AddField("name", public_map_name);
		form.AddField("about", public_map_about);
		form.AddField("url", public_map_url);
		form.AddField("auth_key", VkontakteAPI.INSTANCE._authKey);
		if (App.instance.platform == App.ePlatform.ONDOKLASNIKI)
		{
			form.AddField("session_key", VkontakteAPI.INSTANCE._sessionKey);
		}
		Random r = new Random();
		form.AddField("random", r.Next());
		WWW php_load = new WWW(App.instance.GetCurScriptsUrl() + "public_map.php", form);
		yield return php_load;
		Debug.Log((object)"public_map");
		Debug.Log((object)php_load.text);
		yield return ((MonoBehaviour)this).StartCoroutine(LoadSlotMapInfo(VkontakteAPI.INSTANCE._viewerId, _SelectedSlot));
	}

	public IEnumerator LoadSlotMapInfo(string viewer_id, int slot_id)
	{
		regen_fail = false;
		WWWForm form = new WWWForm();
		form.AddField("id", viewer_id);
		Random r = new Random();
		form.AddField("random", r.Next());
		form.AddField("map_place", slot_id);
		WWW php_load = new WWW(App.instance.GetCurScriptsUrl() + "get_slot_info.php", form);
		yield return php_load;
		if (php_load.text == "NOT AVAILABLE SLOT")
		{
			_SelectedMapID = -1;
			yield break;
		}
		if (php_load.text == "EMPTY SLOT")
		{
			_SelectedMapID = 0;
			yield break;
		}
		Debug.Log((object)("M " + _SelectedMapID));
		_SelectedMapID = Convert.ToInt32(php_load.text.Trim());
		if (_SelectedMapID > 1)
		{
			yield return ((MonoBehaviour)this).StartCoroutine(LoadMapInfo(_SelectedMapID));
		}
	}

	public IEnumerator LoadMapInfo(int map_id)
	{
		player_map_img = null;
		comments_start_ind = 0;
		draw_comment_form = false;
		post_text = string.Empty;
		vouteData = eVouteData.VOUTE_UNKNOWN;
		WWWForm form2 = new WWWForm();
		form2.AddField("map_id", map_id);
		Random r = new Random();
		form2.AddField("random", r.Next());
		WWW php_load2 = new WWW(App.instance.GetCurScriptsUrl() + "get_map_info.php", form2);
		yield return php_load2;
		MonoBehaviour.print((object)php_load2.text);
		string[] param = php_load2.text.Split('|');
		public_map_name = param[0];
		public_map_about = param[1];
		public_map_url = param[2];
		public_map_authorname = param[3];
		Convert.ToInt32(param[4]);
		Convert.ToInt32(param[5]);
		_PublicMapRating = Convert.ToInt32(param[6]);
		_PublicMapOwnerID = param[7];
		_PublicMapOwnerSlot = Convert.ToInt32(param[8]);
		_SelectedMapID = map_id;
		((MonoBehaviour)this).StartCoroutine("GetMapImg", (object)public_map_url);
		((MonoBehaviour)this).StartCoroutine(GetComments(map_id, comments_start_ind));
		((MonoBehaviour)this).StartCoroutine("GetVouteData", (object)map_id);
	}

	public IEnumerator GetVouteData(int map_id)
	{
		WWWForm form = new WWWForm();
		form.AddField("id", VkontakteAPI.INSTANCE._viewerId);
		form.AddField("map_id", map_id);
		Random r = new Random();
		form.AddField("random", r.Next());
		WWW php_load = new WWW(App.instance.GetCurScriptsUrl() + "is_voute_by_map.php", form);
		yield return php_load;
		if (php_load.text.Trim() == "NOT_VOTE")
		{
			vouteData = eVouteData.VOUTE_NOT;
		}
		if (php_load.text.Trim() == "YES")
		{
			vouteData = eVouteData.VOUTE_YES;
		}
		if (php_load.text.Trim() == "NO")
		{
			vouteData = eVouteData.VOUTE_NO;
		}
	}

	public IEnumerator VouteYes(int map_id)
	{
		vouteData = eVouteData.VOUTE_YES;
		WWWForm form = new WWWForm();
		form.AddField("id", VkontakteAPI.INSTANCE._viewerId);
		form.AddField("map_id", map_id);
		form.AddField("auth_key", VkontakteAPI.INSTANCE._authKey);
		if (App.instance.platform == App.ePlatform.ONDOKLASNIKI)
		{
			form.AddField("session_key", VkontakteAPI.INSTANCE._sessionKey);
		}
		Random r = new Random();
		form.AddField("random", r.Next());
		yield return (object)new WWW(App.instance.GetCurScriptsUrl() + "voute_yes.php", form);
	}

	public IEnumerator VouteYesNot(int map_id)
	{
		vouteData = eVouteData.VOUTE_UNKNOWN;
		WWWForm form = new WWWForm();
		form.AddField("id", VkontakteAPI.INSTANCE._viewerId);
		form.AddField("map_id", map_id);
		Random r = new Random();
		form.AddField("random", r.Next());
		form.AddField("auth_key", VkontakteAPI.INSTANCE._authKey);
		if (App.instance.platform == App.ePlatform.ONDOKLASNIKI)
		{
			form.AddField("session_key", VkontakteAPI.INSTANCE._sessionKey);
		}
		yield return (object)new WWW(App.instance.GetCurScriptsUrl() + "voute_yes_not.php", form);
		vouteData = eVouteData.VOUTE_NOT;
	}

	public IEnumerator VouteNoNot(int map_id)
	{
		vouteData = eVouteData.VOUTE_UNKNOWN;
		WWWForm form = new WWWForm();
		form.AddField("id", VkontakteAPI.INSTANCE._viewerId);
		form.AddField("map_id", map_id);
		Random r = new Random();
		form.AddField("random", r.Next());
		yield return (object)new WWW(App.instance.GetCurScriptsUrl() + "voute_no_not.php", form);
		vouteData = eVouteData.VOUTE_NOT;
	}

	public IEnumerator GetMapImg(string url)
	{
		if (url.Length > 0)
		{
			WWW www = new WWW(url);
			yield return www;
			if (www.error == null)
			{
				player_map_img = www.texture;
			}
		}
	}

	private void DrawTutorial()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)boxSkin != (Object)null)
		{
			GUI.skin = boxSkin;
		}
		GUI.Box(new Rect((float)(Screen.width / 2 - 370), (float)(Screen.height / 2 - 270), 740f, 540f), "Туториал");
		if ((Object)(object)guiSkin != (Object)null)
		{
			GUI.skin = guiSkin;
		}
		GUI.Label(new Rect(90f, 50f, 624f, 458f), (Texture)(object)helpImages[curent_page]);
		if (curent_page > 0 && GUI.Button(new Rect(50f, 500f, 200f, 50f), "Назад"))
		{
			curent_page--;
		}
		if (curent_page < helpImages.Length - 1)
		{
			if (GUI.Button(new Rect(550f, 500f, 200f, 50f), "Далее"))
			{
				curent_page++;
			}
		}
		else if (GUI.Button(new Rect(550f, 500f, 200f, 50f), "Играть!"))
		{
			showMenu = false;
			WorldGameObject.INSTANCE.EnableMouseLook();
			Screen.lockCursor = true;
			curWindow = Windows.Game;
			ProfileINI.tutorial_watch = 1;
			Debug.Log((object)"2");
			ProfileINI.Save();
		}
	}

	private void DrawInGameSave()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)boxSkin != (Object)null)
		{
			GUI.skin = boxSkin;
		}
		GUI.Box(new Rect((float)(Screen.width / 2 - 370), (float)(Screen.height / 2 - 270), 740f, 540f), "Меню");
		if ((Object)(object)guiSkin != (Object)null)
		{
			GUI.skin = guiSkin;
		}
		GUILayout.BeginArea(new Rect((float)(Screen.width / 2 - 350), (float)(Screen.height / 2 - 270), 700f, 540f));
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		GUILayout.Space(30f);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		DrawMoney(draw_btn: false);
		GUILayout.EndHorizontal();
		GUILayout.Space(20f);
		GUI.Box(new Rect(0f, 70f, 340f, 450f), "Карты");
		GUILayout.BeginArea(new Rect(10f, 70f, 320f, 450f));
		GUILayout.Space(30f);
		DrawMyMaps();
		GUILayout.EndArea();
		GUI.Box(new Rect(350f, 70f, 360f, 450f), "Описание");
		GUILayout.BeginArea(new Rect(360f, 70f, 340f, 450f));
		DrawSaveMapInfo();
		GUILayout.EndArea();
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	private void DrawSaves()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		GUI.Box(new Rect(0f, 70f, 340f, 450f), "Карты");
		GUILayout.BeginArea(new Rect(10f, 70f, 320f, 450f));
		GUILayout.Space(30f);
		DrawMapList();
		GUILayout.EndArea();
		GUI.Box(new Rect(350f, 70f, 360f, 450f), "Описание");
		GUILayout.BeginArea(new Rect(360f, 70f, 340f, 450f));
		if (!draw_map_card_open_room)
		{
			DrawSaveMapInfo();
		}
		else
		{
			DrawBattlesExistsMenu();
		}
		GUILayout.EndArea();
	}

	private void GUIMenu(int id)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)boxSkin != (Object)null)
		{
			GUI.skin = boxSkin;
		}
		GUI.Box(new Rect((float)(Screen.width / 2 - 370), (float)(Screen.height / 2 - 270), 740f, 540f), "Меню");
		if ((Object)(object)guiSkin != (Object)null)
		{
			GUI.skin = guiSkin;
		}
		GUILayout.BeginArea(new Rect((float)(Screen.width / 2 - 350), (float)(Screen.height / 2 - 270), 700f, 540f));
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		GUILayout.Space(30f);
		DrawTopMenu();
		GUILayout.Space(20f);
		if (curWindow == Windows.Battles)
		{
			DrawBattles();
		}
		if (curWindow == Windows.Options)
		{
			DrawOptions();
		}
		if (curWindow == Windows.Store)
		{
			DrawStore();
		}
		if (curWindow == Windows.ByCrystal)
		{
			DrawByCrystal();
		}
		if (curWindow == Windows.Saves)
		{
			DrawSaves();
		}
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	private void LoadingMenu(int id)
	{
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		GUILayout.Space(30f);
		if (Application.loadedLevelName == "Game" && ProfileINI.server_name.Length > 0)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(20f);
			GUILayout.Label("Сервер : ", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
			GUILayout.Label(App.instance.gameIni.server_name, GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
		}
		else
		{
			GUILayout.Space(20f);
		}
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Space(20f);
		GUILayout.Label(loading_text, new GUILayoutOption[0]);
		GUILayout.Space(10f);
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
	}

	private void DrawEveryDayBonus()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)boxSkin != (Object)null)
		{
			GUI.skin = boxSkin;
		}
		GUI.Box(new Rect((float)(Screen.width / 2 - 150), (float)(Screen.height / 2 - 150), 300f, 250f), "Ежедневный бонус!");
		if ((Object)(object)guiSkin != (Object)null)
		{
			GUI.skin = guiSkin;
		}
		GUILayout.BeginArea(new Rect((float)(Screen.width / 2 - 140), (float)(Screen.height / 2 - 150), 270f, 250f));
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		GUILayout.Space(45f);
		GUILayout.Label("  Поздравляем, ваш ежедневный бонус :", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		GUILayout.Label("               100 золотых монет!", GUIStyle.op_Implicit("LegendaryText"), new GUILayoutOption[0]);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Space(100f);
		GUILayout.Label((Texture)(object)coinsImgBig, new GUILayoutOption[0]);
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		if (GUILayout.Button("ОК", new GUILayoutOption[0]))
		{
			curWindow = Windows.Loading;
			((MonoBehaviour)this).StartCoroutine(App.instance.LoadProfile2(from_loading: true));
		}
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	private void SetPictureMenu()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)boxSkin != (Object)null)
		{
			GUI.skin = boxSkin;
		}
		GUI.Box(new Rect((float)(Screen.width / 2 - 170), (float)(Screen.height / 2 - 110), 340f, 140f), "Картина");
		if ((Object)(object)guiSkin != (Object)null)
		{
			GUI.skin = guiSkin;
		}
		GUILayout.BeginArea(new Rect((float)(Screen.width / 2 - 160), (float)(Screen.height / 2 - 110), 320f, 140f));
		GUILayout.Space(40f);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("URL изображения", new GUILayoutOption[0]);
		GUILayout.Space(80f);
		GUILayout.Button("?", new GUILayoutOption[0]);
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		picture_url = GUILayout.TextField(picture_url, new GUILayoutOption[1] { GUILayout.MinWidth(180f) });
		if (GUILayout.Button("Очистить", new GUILayoutOption[0]))
		{
			picture_url = string.Empty;
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		if (GUILayout.Button("Установить изображение", new GUILayoutOption[0]))
		{
			curent_picture.SynhNetworkPicture(picture_url);
			WorldGameObject.INSTANCE.EnableMouseLook();
			Screen.lockCursor = true;
			showMenu = false;
		}
		GUILayout.EndArea();
	}

	private void DrawTablichkaMenu()
	{
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		GUISkin skin = GUI.skin;
		GUI.skin = PlateSkin;
		int width = ((Texture)GUI.skin.box.normal.background).width;
		int height = ((Texture)GUI.skin.box.normal.background).height;
		int width2 = ((Texture)GUI.skin.button.normal.background).width;
		int height2 = ((Texture)GUI.skin.button.normal.background).height;
		GUI.Box(new Rect((float)(Screen.width / 2 - width / 2), (float)(Screen.height / 2 - height / 2), (float)width, (float)height), string.Empty);
		for (int i = 0; i < PlateLines.Length; i++)
		{
			PlateLines[i] = GUI.TextField(new Rect((float)(Screen.width / 2 - width / 2 + 230), (float)(Screen.height / 2 - height / 2 + 170 + i * 70), 300f, 70f), PlateLines[i], 11);
		}
		if (GUI.Button(new Rect((float)(Screen.width / 2 - width / 2 + 350), (float)(Screen.height / 2 - height / 2 + 520), (float)width2, (float)height2), string.Empty))
		{
			CurrentPlate.UpdateText(PlateLines[0], PlateLines[1], PlateLines[2], PlateLines[3]);
			WorldGameObject.INSTANCE.EnableMouseLook();
			Screen.lockCursor = true;
			showMenu = false;
		}
		GUI.skin = skin;
	}

	private void NameMenu()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)boxSkin != (Object)null)
		{
			GUI.skin = boxSkin;
		}
		GUI.Box(new Rect((float)(Screen.width / 2 - 150), (float)(Screen.height / 2 - 100), 300f, 120f), "Имя");
		if ((Object)(object)guiSkin != (Object)null)
		{
			GUI.skin = guiSkin;
		}
		GUILayout.BeginArea(new Rect((float)(Screen.width / 2 - 150), (float)(Screen.height / 2 - 100), 300f, 120f));
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		GUILayout.Space(25f);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Space(20f);
		GUILayout.Label(name_info_text, new GUILayoutOption[0]);
		GUILayout.Space(10f);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Space(10f);
		ProfileINI.nickname = GUILayout.TextField(ProfileINI.nickname, new GUILayoutOption[0]);
		if (ProfileINI.nickname.Length > 12)
		{
			ProfileINI.nickname = ProfileINI.nickname.Substring(0, 12);
		}
		GUILayout.Space(10f);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Space(10f);
		if (ProfileINI.nickname.Length >= 3 && !check_name && GUILayout.Button("Продолжить", new GUILayoutOption[0]))
		{
			((MonoBehaviour)this).StartCoroutine(CheckName(ProfileINI.nickname));
		}
		GUILayout.Space(10f);
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	public IEnumerator CheckName(string name)
	{
		check_name = true;
		WWWForm form = new WWWForm();
		Random r = new Random();
		form.AddField("random", r.Next());
		form.AddField("id", VkontakteAPI.INSTANCE._viewerId);
		form.AddField("auth_key", VkontakteAPI.INSTANCE._authKey);
		if (App.instance.platform == App.ePlatform.ONDOKLASNIKI)
		{
			form.AddField("session_key", VkontakteAPI.INSTANCE._sessionKey);
		}
		form.AddField("name", name);
		WWW php_load = new WWW(App.instance.GetCurScriptsUrl() + "set_name.php", form);
		yield return php_load;
		if (php_load.text == "SET_NAME")
		{
			ProfileINI.Save();
			check_name = false;
			if (VkontakteAPI.INSTANCE._mapID > 1)
			{
				App.instance.ConnectToRandomServer();
			}
			else
			{
				App.instance.OpenChooseShardMenu();
			}
		}
		else
		{
			name_info_text = "Ник занят, попробуйте другой";
			check_name = false;
		}
	}

	public IEnumerator RefreshListOfMaps(string _text)
	{
		int ind = _text.IndexOf('|');
		count_page = Convert.ToInt32(_text.Substring(0, ind));
		string text = _text.Remove(0, ind + 1);
		ind = text.IndexOf('|');
		max_page_view = Convert.ToInt32(text.Substring(0, ind));
		text = text.Remove(0, ind + 1);
		string[] maps = text.Split('|');
		Maps.Clear();
		publicMapsList = new string[maps.Length - 1];
		for (int i = 1; i < maps.Length; i++)
		{
			string[] param = maps[i].Split(',');
			Map map = new Map
			{
				mapId = param[0],
				autor_id = param[1],
				autor_slot_id = param[2],
				name = param[3],
				rating = param[4]
			};
			Maps.Add(map);
			publicMapsList[i - 1] = map.name + " [* " + map.rating + "]";
		}
		publicMapsListSelect = 0;
		if (Maps.Count > 0)
		{
			int map_id = Convert.ToInt32((Maps[0] as Map).mapId);
			yield return ((MonoBehaviour)this).StartCoroutine(LoadMapInfo(map_id));
		}
		else
		{
			_SelectedMapID = -3;
		}
	}

	public IEnumerator RefreshListOfMapsBestAllTime(int _page_numb)
	{
		cur_map_page += _page_numb - cur_map_page;
		cur_map_list_view = MAP_LIST_TYPE.Best;
		WWWForm form = new WWWForm();
		Random r = new Random();
		form.AddField("page", cur_map_page);
		form.AddField("random", r.Next());
		WWW php_load = new WWW(App.instance.GetCurScriptsUrl() + "get_all_time_map.php", form);
		yield return php_load;
		((MonoBehaviour)this).StartCoroutine(RefreshListOfMaps(php_load.text));
	}

	public IEnumerator RefreshListOfMapsMonthly(int _page_numb)
	{
		cur_map_page += _page_numb - cur_map_page;
		cur_map_list_view = MAP_LIST_TYPE.Month;
		WWWForm form = new WWWForm();
		Random r = new Random();
		form.AddField("page", cur_map_page);
		form.AddField("random", r.Next());
		WWW php_load = new WWW(App.instance.GetCurScriptsUrl() + "get_monthly_map.php", form);
		yield return php_load;
		((MonoBehaviour)this).StartCoroutine(RefreshListOfMaps(php_load.text));
	}

	public IEnumerator RefreshListOfDay(int _page_numb)
	{
		cur_map_page += _page_numb - cur_map_page;
		cur_map_list_view = MAP_LIST_TYPE.Day;
		WWWForm form = new WWWForm();
		Random r = new Random();
		form.AddField("page", cur_map_page);
		form.AddField("random", r.Next());
		WWW php_load = new WWW(App.instance.GetCurScriptsUrl() + "get_day_map.php", form);
		yield return php_load;
		Debug.Log((object)"RefreshListOfDay");
		Debug.Log((object)php_load.text);
		((MonoBehaviour)this).StartCoroutine(RefreshListOfMaps(php_load.text));
	}

	public IEnumerator RefreshListOfMapsWeekly(int _page_numb)
	{
		cur_map_page += _page_numb - cur_map_page;
		cur_map_list_view = MAP_LIST_TYPE.Week;
		WWWForm form = new WWWForm();
		Random r = new Random();
		form.AddField("page", cur_map_page);
		form.AddField("random", r.Next());
		WWW php_load = new WWW(App.instance.GetCurScriptsUrl() + "get_weekly_map.php", form);
		yield return php_load;
		Debug.Log((object)php_load.text);
		((MonoBehaviour)this).StartCoroutine(RefreshListOfMaps(php_load.text));
	}

	public IEnumerator RefreshListOfMapsNew(int _page_numb)
	{
		cur_map_page += _page_numb - cur_map_page;
		cur_map_list_view = MAP_LIST_TYPE.Last;
		WWWForm form = new WWWForm();
		Random r = new Random();
		form.AddField("page", cur_map_page);
		form.AddField("random", r.Next());
		WWW php_load = new WWW(App.instance.GetCurScriptsUrl() + "get_latest_map.php", form);
		yield return php_load;
		Debug.Log((object)php_load.text);
		((MonoBehaviour)this).StartCoroutine(RefreshListOfMaps(php_load.text));
	}

	public IEnumerator SearchMaps(int _page_numb)
	{
		cur_map_page += _page_numb - cur_map_page;
		cur_map_list_view = MAP_LIST_TYPE.Search;
		WWWForm form = new WWWForm();
		Random r = new Random();
		if (search_type == 0)
		{
			form.AddField("text", search_map_name);
		}
		else
		{
			form.AddField("text", search_map_author);
		}
		if (sort_type)
		{
			form.AddField("sortby", "rating");
		}
		else
		{
			form.AddField("sortby", "public_date");
		}
		form.AddField("type", search_type);
		form.AddField("page", cur_map_page);
		form.AddField("random", r.Next());
		WWW php_load = new WWW(App.instance.GetCurScriptsUrl() + "search_maps.php", form);
		yield return php_load;
		Debug.Log((object)php_load.text);
		((MonoBehaviour)this).StartCoroutine(RefreshListOfMaps(php_load.text));
	}

	public IEnumerator LoadMoreMaps(int _page_numb)
	{
		cur_map_page += _page_numb - cur_map_page;
		WWWForm form = new WWWForm();
		Random r = new Random();
		form.AddField("page", cur_map_page);
		form.AddField("random", r.Next());
		switch (cur_map_list_view)
		{
		case MAP_LIST_TYPE.Best:
			php_load = new WWW(App.instance.GetCurScriptsUrl() + "get_all_time_map.php", form);
			break;
		case MAP_LIST_TYPE.Day:
			php_load = new WWW(App.instance.GetCurScriptsUrl() + "get_day_map.php", form);
			break;
		case MAP_LIST_TYPE.Last:
			php_load = new WWW(App.instance.GetCurScriptsUrl() + "get_latest_map.php", form);
			break;
		case MAP_LIST_TYPE.Month:
			php_load = new WWW(App.instance.GetCurScriptsUrl() + "get_monthly_map.php", form);
			break;
		case MAP_LIST_TYPE.Week:
			php_load = new WWW(App.instance.GetCurScriptsUrl() + "get_weekly_map.php", form);
			break;
		case MAP_LIST_TYPE.Search:
			if (search_type == 0)
			{
				form.AddField("text", search_map_name);
			}
			else
			{
				form.AddField("text", search_map_author);
			}
			if (sort_type)
			{
				form.AddField("sortby", "rating");
			}
			else
			{
				form.AddField("sortby", "public_date");
			}
			form.AddField("type", search_type);
			php_load = new WWW(App.instance.GetCurScriptsUrl() + "search_maps.php", form);
			break;
		}
		yield return php_load;
		((MonoBehaviour)this).StartCoroutine(RefreshWithNewMaps(php_load.text));
	}

	public IEnumerator RefreshWithNewMaps(string _text)
	{
		int ind = _text.IndexOf('|');
		count_page = Convert.ToInt32(_text.Substring(0, ind));
		string text = _text.Remove(0, ind + 1);
		ind = text.IndexOf('|');
		max_page_view = Convert.ToInt32(text.Substring(0, ind));
		text = text.Remove(0, ind + 1);
		string[] maps = text.Split('|');
		string[] tmpl = new string[publicMapsList.Length];
		for (int i = 0; i < publicMapsList.Length; i++)
		{
			tmpl[i] = publicMapsList[i];
		}
		publicMapsList = new string[tmpl.Length + maps.Length - 1];
		for (int j = 0; j < tmpl.Length; j++)
		{
			publicMapsList[j] = tmpl[j];
		}
		for (int k = 1; k < maps.Length; k++)
		{
			string[] param = maps[k].Split(',');
			Map map = new Map
			{
				mapId = param[0],
				autor_id = param[1],
				autor_slot_id = param[2],
				name = param[3],
				rating = param[4]
			};
			Maps.Add(map);
			publicMapsList[tmpl.Length + k - 1] = map.name + " [* " + map.rating + "]";
		}
		publicMapsListSelect = 0;
		if (Maps.Count > 0)
		{
			int map_id = Convert.ToInt32((Maps[0] as Map).mapId);
			yield return ((MonoBehaviour)this).StartCoroutine(LoadMapInfo(map_id));
		}
		else
		{
			_SelectedMapID = -3;
		}
	}

	public void ShowPopup()
	{
		showMenu = true;
		curWindow = Windows.Voice;
		bs._WorldGameObject.DisableMouseLook();
		Screen.lockCursor = false;
	}

	public void ShowWeaponWindow()
	{
		if ((Object)(object)WorldGameObject.INSTANCE.MainPlayer != (Object)null)
		{
			PlayerNetwork component = WorldGameObject.INSTANCE.MainPlayer.GetComponent<PlayerNetwork>();
			component.SelectWeapon(0);
			component.MainWeapon[0] = (component.MainWeapon[1] = null);
		}
		if (!bs._WorldGameObject.FirstSelect)
		{
			KillPlayer();
		}
		bs._WorldGameObject.FirstSelect = false;
		showMenu = true;
		curWindow = Windows.SelectWeapon;
		bs._WorldGameObject.DisableMouseLook();
		Screen.lockCursor = false;
	}

	public static void KillPlayer()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (bs._WorldGameObject.MainPlayerNode.Life >= 0f)
		{
			bs._WorldGameObject.PunchPlayer(PhotonNetwork.player, Vector3.zero, 100f);
		}
	}

	private void WeaponMenu()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)boxSkin != (Object)null)
		{
			GUI.skin = boxSkin;
		}
		GUI.Box(new Rect((float)(Screen.width / 2 - 220), (float)(Screen.height / 2 - 105), 440f, 250f), string.Empty);
		if ((Object)(object)guiSkin != (Object)null)
		{
			GUI.skin = guiSkin;
		}
		GUILayout.BeginArea(new Rect((float)(Screen.width / 2 - 200), (float)(Screen.height / 2 - 105), 400f, 250f));
		GUILayout.Space(35f);
		PlayerNetwork component = bs._WorldGameObject.MainPlayer.GetComponent<PlayerNetwork>();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("Приобрести оружие можно в магазине", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		if (GUILayout.Button("Купить", new GUILayoutOption[0]))
		{
			curWindow = Windows.GameStore;
			curStoreLeftWindow = StoreLeftWindow.WEAPONS;
		}
		GUILayout.EndHorizontal();
		GUILayout.Label("Выберете оружее", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		_WeaponScrollView = GUILayout.BeginScrollView(_WeaponScrollView, new GUILayoutOption[0]);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		int num = 0;
		_HaveWeapons = false;
		Gun[] guns = component.Guns;
		foreach (Gun gun in guns)
		{
			if (new Gun[2] { component.Grenade, component.Knife }.Contains(gun) || component.MainWeapon.Contains(gun) || ProfileINI.GetPurchesValue(gun.storePurchase) == 0)
			{
				continue;
			}
			_HaveWeapons = true;
			if ((Object)(object)gun.weaponIconBig == (Object)null)
			{
				MonoBehaviour.print((object)((Object)gun).name);
			}
			if (GUILayout.Button((Texture)(object)gun.weaponIconBig, new GUILayoutOption[1] { GUILayout.ExpandWidth(true) }))
			{
				for (int j = 0; j < 2; j++)
				{
					if ((Object)(object)component.MainWeapon[j] == (Object)null)
					{
						bs._WorldGameObject.MainPlayer.audio.PlayOneShot(bs._Igor.takeSound, ProfileINI.sound_volume * ProfileINI.sound_scale);
						component.MainWeapon[j] = gun;
						break;
					}
				}
			}
			num++;
			if (num % 3 == 0)
			{
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.EndScrollView();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("В бой!", new GUILayoutOption[0]))
		{
			WorldGameObject.INSTANCE.EnableMouseLook();
			Screen.lockCursor = true;
			showMenu = false;
		}
		GUILayout.EndArea();
	}

	private void VoiceMenu()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)boxSkin != (Object)null)
		{
			GUI.skin = boxSkin;
		}
		GUI.Box(new Rect((float)(Screen.width / 2 - 220), (float)(Screen.height / 2 - 65), 440f, 200f), string.Empty);
		if ((Object)(object)guiSkin != (Object)null)
		{
			GUI.skin = guiSkin;
		}
		GUILayout.BeginArea(new Rect((float)(Screen.width / 2 - 200), (float)(Screen.height / 2 - 65), 400f, 200f));
		GUILayout.Space(35f);
		GUILayout.Label("Нажмите ок в следуйщем окне чтобы активировать микрофон", new GUILayoutOption[0]);
		if (GUILayout.Button("Далее", new GUILayoutOption[0]))
		{
			WorldGameObject.INSTANCE.EnableMouseLook();
			showMenu = false;
		}
		GUILayout.EndArea();
	}
}
