using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
	private class Map
	{
		public string AuthorID;

		public string AuthorSlotID;

		public string MapID;

		public string Name;

		public string Rating;
	}

	private class Comment
	{
		public string Name;

		public string Text;

		public string ID;
	}

	public enum CrosshairAction
	{
		None = 0,
		Take = 1,
		Sit = 2,
		Open = 4,
		Activate = 8,
		Write = 0x10,
		Draw = 0x20,
		Wheel = 0x40,
		Put = 0x80
	}

	private class FavoritServer
	{
		public int admin_id;

		public string server_name;

		public byte slot_id;
	}

	public static MainMenu Instance;

	private int _CurNewsBlock;

	private bool _NewsAutoScroll;

	private string[] _NewsText;

	private Texture2D[] _NewsPicture;

	private string[] _NewsPictureURL;

	private string[] _NewsButtonText;

	private string[] _NewsButtonURL;

	private int _FastGameType = -1;

	private int _FastGameMap;

	private List<Store.StandartMap> _FastGameMaps;

	private int _MapGameType;

	private GameINI.GameType[] _MapGameTypes = new GameINI.GameType[5]
	{
		GameINI.GameType.BUILDING,
		GameINI.GameType.DEATHMATCH,
		GameINI.GameType.TEAM_BATTLE,
		GameINI.GameType.CTF,
		GameINI.GameType.ZOMBIE_VIRUS
	};

	private bool _MapDestroyable = true;

	private bool _MapBonuses = true;

	private int _MapSize;

	private GameINI.MapSize[] _MapSizes = new GameINI.MapSize[2]
	{
		GameINI.MapSize.SMALL,
		GameINI.MapSize.MEDIUM
	};

	private int _MapType;

	private GameINI.MapType[] _MapTypes = new GameINI.MapType[6]
	{
		GameINI.MapType.STANDART,
		GameINI.MapType.FLAT,
		GameINI.MapType.SAND,
		GameINI.MapType.OCEAN,
		GameINI.MapType.ISLAND,
		GameINI.MapType.SNOWLAND
	};

	private int _MapTime;

	private GameINI.MapTime[] _MapTimes = new GameINI.MapTime[3]
	{
		GameINI.MapTime.DAY,
		GameINI.MapTime.NIGHT,
		GameINI.MapTime.SWITCH
	};

	private List<Map> _Maps = new List<Map>();

	private string _GetMapsScript;

	private Dictionary<string, string> _GetMapsFormData;

	private ArrayList _MapComments = new ArrayList();

	private int _MapCommentsStartIndex;

	private int _RecommendedShard;

	private string _CustomMapImageURL;

	private DateTime _BonusGemsEndTime = DateTime.MinValue;

	private int _BonusGemsSeconds = -1;

	private bool _BonusGemsShowSeparator;

	private Store.TabType _InventoryTab = Store.TabType.Decorations;

	private Dictionary<string, List<EntityType>> _CurInventoryItems = new Dictionary<string, List<EntityType>>();

	private Dictionary<string, List<BlockType>> _CurInventoryBlocks = new Dictionary<string, List<BlockType>>();

	private Store.TabType _ShopTab;

	private StorePurchase _CurPurchase = StorePurchase.NONE;

	private Dictionary<string, List<StorePurchase>> _CurShopPurchases = new Dictionary<string, List<StorePurchase>>();

	private bool _ShopPurchaseItemInfo;

	private List<RoomInfo> _ServerRooms = new List<RoomInfo>();

	private RoomInfo _SelectedRoom;

	private bool _ServersShowCTF;

	private bool _ServersShowTeamBattle;

	private bool _ServersShowDeathmatch;

	private bool _ServersShowBuilding;

	private bool _ServersShowZombieVirus;

	private int _SelectedMapID = -2;

	private int _SelectedMapSlot;

	private string _SelectedMapOwner = string.Empty;

	private bool _SelectedMapNotVoted;

	private int _SelectedMapRating;

	private string _SelectedMapAuthor = string.Empty;

	private bool _SpeedEnabled = true;

	private bool _FlyEnabled = true;

	internal bool Flying;

	internal bool CinematicCamera;

	public static bool CleanScreen;

	private Menu _CurMenu;

	private Dictionary<Menu, List<string>> _MenuNodeNames = new Dictionary<Menu, List<string>>();

	private Dictionary<Menu, List<Action<object, object>>> _MenuInitializers = new Dictionary<Menu, List<Action<object, object>>>();

	private Menu[] _NonModalMenus;

	private Menu[] _CanToggleMenus;

	private Action[] _AskMenuActions;

	private Action<string> _InputTextAction;

	private int[] _BonusData;

	private Tablichka _CurPlate;

	private List<Gun> _SelectWeapons = new List<Gun>();

	private FavoritServer[] _FavoritServers;

	public DateTime BonusGemsEndTime
	{
		set
		{
			_BonusGemsEndTime = value;
		}
	}

	public Menu CurMenu => _CurMenu;

	public bool MenuActive => _CurMenu != Menu.None;

	public void RegisterMenus()
	{
		_NonModalMenus = new Menu[2]
		{
			Menu.None,
			Menu.Hud
		};
		_CanToggleMenus = new Menu[19]
		{
			Menu.None,
			Menu.Hud,
			Menu.Start,
			Menu.MyMaps,
			Menu.Settings,
			Menu.Shop,
			Menu.ItemsPack,
			Menu.Servers,
			Menu.TopMaps,
			Menu.Bank,
			Menu.Inventory,
			Menu.TabMenu,
			Menu.Book,
			Menu.GameMenu,
			Menu.Emotions,
			Menu.Profile,
			Menu.SelectWeapon,
			Menu.TeamBattle,
			Menu.Deathmatch
		};
		RegisterMenu(Enum.GetValues(typeof(Menu)) as Menu[], null, (object data, object dataEx) =>
		{
			KGUI.EnableNodes("main_menu_background", Application.loadedLevel == 0);
			if (_CurMenu != Menu.Book && (Object)(object)Book.Current != (Object)null)
			{
				Book.Current.SaveCurTexts();
				Book.Current = null;
			}
		}, () =>
		{
		});
		RegisterMenu(new Menu[7]
		{
			Menu.Start,
			Menu.MyMaps,
			Menu.Settings,
			Menu.Shop,
			Menu.Servers,
			Menu.TopMaps,
			Menu.Bank
		}, "main_menu", (object data, object dataEx) =>
		{
			KGUI.EnableNodes("main_menu", enable: true);
			KGUI.EnableNodes("main_menu.slider_page1", enable: false);
			KGUI.EnableNodes("main_menu.slider_page2", enable: false);
			KGUI.EnableNodes("main_menu.top_menu", Application.loadedLevel == 0);
			KGUI.EnableNodes("main_menu.bottom_menu", Application.loadedLevel == 0);
			KGUI.EnableNodes("main_menu.ibtn_profile", enable: false);
			TimeSpan timeSpan = _BonusGemsEndTime - DateTime.Now;
			KGUI.EnableNodes("main_menu.bonus_gems", timeSpan.TotalSeconds > 0.0);
			if (timeSpan.TotalSeconds > 0.0)
			{
				KGUI.SetControlText("main_menu.bonus_gems.txt_time", timeSpan.Hours + ":" + timeSpan.Minutes);
				_BonusGemsSeconds = (int)timeSpan.TotalSeconds;
			}
			if (_CurMenu == Menu.Start || _CurMenu == Menu.TopMaps || _CurMenu == Menu.Settings || _CurMenu == Menu.Bank || (_CurMenu == Menu.Shop && Application.loadedLevel == 0))
			{
				KGUI.EnableNodes("main_menu.button_back", enable: false);
			}
			if (Application.loadedLevel == 0)
			{
				KGUI.SetControlText("main_menu.txt_gold", string.Empty + ProfileINI.coins);
				KGUI.SetControlText("main_menu.txt_gems", string.Empty + ProfileINI.crystals);
				int num = ProfileINI.experience;
				int num2 = ProfileINI.LevelExp(num);
				int num3 = ProfileINI.ExpForLevel(num2);
				int num4 = ProfileINI.ExpForLevel(num2 + 1);
				((Component)KGUI.FindNode("main_menu.exp_bar_cur")).GetComponent<UISprite>().fillAmount = (float)(num - num3) / (float)(num4 - num3);
			}
			KGUI.SetControlText("main_menu.bottom_menu.txt_level", "lvl " + ProfileINI.level);
			KGUI.SetControlSprite(KGUI.FindNode("main_menu.bottom_menu.level_icon"), "level_" + ProfileINI.level, 0f);
			KGUI.SetControlText("main_menu.bottom_menu.txt_nickname", ProfileINI.email);
			KGUI.SetControlText("main_menu.bottom_menu.button_select_world.txt_title", string.Empty + App.Instance.CurrentShard);
		}, () =>
		{
			KGUI.SetButtonCallback("main_menu.ibtn_play", () =>
			{
				SwitchMenu(Menu.Start);
			});
			KGUI.SetButtonCallback("main_menu.ibtn_shop", () =>
			{
				SwitchMenu(Menu.Shop);
			});
			KGUI.SetButtonCallback("main_menu.ibtn_profile", () =>
			{
			});
			KGUI.SetButtonCallback("main_menu.ibtn_maps", () =>
			{
				SwitchMenu(Menu.TopMaps);
			});
			KGUI.SetButtonCallback("main_menu.ibtn_settings", () =>
			{
				SwitchMenu(Menu.Settings);
			});
			KGUI.SetButtonCallback("main_menu.ibtn_buy", () =>
			{
				SwitchMenu(Menu.Bank);
			});
			KGUI.SetButtonCallback("main_menu.ibtn_back", () =>
			{
				if (_CurMenu == Menu.Start)
				{
					if (((Component)KGUI.FindNode("main_menu_start.page1_select_map")).gameObject.activeInHierarchy)
					{
						KGUI.EnableNodes("main_menu_start.page1_select_map", enable: false);
						KGUI.EnableNodes("main_menu_start.page1_fast_game", enable: true);
						RefreshStartFastGame(0);
					}
					else
					{
						KGUI.EnableNodes("main_menu_start.page1_start", enable: true);
						KGUI.EnableNodes("main_menu_start.page1_fast_game", enable: false);
						KGUI.EnableNodes("main_menu_start.page1_select_map", enable: false);
						KGUI.EnableNodes("main_menu.button_back", enable: false);
					}
				}
				else if (_CurMenu == Menu.Shop && Application.loadedLevel == 1)
				{
					SwitchMenu(Menu.Inventory, _InventoryTab);
				}
				else if (_CurMenu == Menu.Bank && Application.loadedLevel == 1)
				{
					SwitchMenu(Menu.Shop, _ShopTab);
				}
				else if (Application.loadedLevel == 0)
				{
					SwitchMenu(Menu.Start);
				}
				else
				{
					HideMenu();
				}
			});
			KGUI.SetButtonCallback("main_menu.ibtn_select_world", () =>
			{
				HideMenu();
				PhotonNetwork.Disconnect();
			});
			KGUI.SetTooltipCallback("main_menu.bottom_menu.background", () => Localize.GetText("LEVEL_" + ProfileINI.level));
		});
		RegisterMenu(Menu.Hud, "hud", (object data, object dataEx) =>
		{
			SetCrosshairInfo(null, CrosshairAction.None);
			KGUI.EnableNodes("hud.txt_watch_mode_tip", App.Instance.Settings.isWatch);
			KGUI.EnableNodes("hud.txt_respawn_tip", enable: false);
			KGUI.EnableNodes("hud.battle.txt_flag_captured_tip", enable: false);
			KGUI.EnableNodes("hud.gold_cup", enable: false);
			KGUI.EnableNodes("hud.chat.grid_messages.slot_prototype", enable: false);
			KGUI.EnableNodes("hud.kills_list.grid_messages.slot_prototype", enable: false);
			KGUI.EnableNodes("hud.nickname", enable: false);
			KGUI.EnableNodes("hud.speech_bubble", enable: false);
			KGUI.EnableNodes("hud.emotion_image", enable: false);
			KGUI.ResizeGrid("hud.boobles", 0, (GameObject slot, int index) =>
			{
			});
			if (GameType.BattleMode())
			{
				KGUI.EnableNodes("hud.custom_blocks", enable: false);
				KGUI.EnableNodes("hud.txt_battle_more_players_tip", PhotonNetwork.playerList.Length < 4);
				if (!GameType.IsObserving())
				{
					KGUI.EnableNodes("hud.crosshair", enable: false);
					RefreshBattleWeapon();
					((Component)CameraController.Instance.HandsGunAnimations).gameObject.SetActive(true);
				}
				else
				{
					KGUI.EnableNodes("hud.battle", enable: false);
					((Component)CameraController.Instance.HandsGunAnimations).gameObject.SetActive(false);
				}
				KGUI.EnableNodes("hud.cube", enable: false);
				if ((Object)(object)TeamBattle.Instance != (Object)null)
				{
					if (TeamBattle.Instance is Deathmatch)
					{
						KGUI.EnableNodes("hud.battle.flag_blue", enable: false);
						KGUI.EnableNodes("hud.battle.flag_red", enable: false);
						KGUI.EnableNodes("hud.battle.txt_blue_score", enable: false);
						KGUI.EnableNodes("hud.battle.txt_red_score", enable: false);
					}
					else
					{
						KGUI.SetControlText("hud.battle.txt_red_score", TeamBattle.Instance.RedTeamScore + string.Empty);
						KGUI.SetControlText("hud.battle.txt_blue_score", TeamBattle.Instance.BlueTeamScore + string.Empty);
					}
					if (!(TeamBattle.Instance is Ctf))
					{
						KGUI.EnableNodes("hud.battle.flags", enable: false);
					}
					if (TeamBattle.Instance is ZombieVirus)
					{
						KGUI.EnableNodes("hud.battle.flag_red", enable: false);
						KGUI.EnableNodes("hud.battle.flag_blue", enable: false);
						KGUI.EnableNodes("hud.battle.txt_red_score", enable: false);
						KGUI.EnableNodes("hud.battle.txt_blue_score", enable: false);
					}
					else
					{
						KGUI.EnableNodes("hud.battle.zombie_virus", enable: false);
						KGUI.EnableNodes("hud.battle.txt_survivors_count", enable: false);
						KGUI.EnableNodes("hud.battle.txt_zombies_count", enable: false);
					}
				}
			}
			else
			{
				KGUI.EnableNodes("hud.battle", enable: false);
				((Component)CameraController.Instance.HandsGunAnimations).gameObject.SetActive(true);
				if (ProfileINI.GetPurchaseValue(StorePurchase.BLOCK_KIND_FENCE) > 0 || ProfileINI.GetPurchaseValue(StorePurchase.BLOCK_KIND_HALF) > 0 || ProfileINI.GetPurchaseValue(StorePurchase.BLOCK_KIND_DIAGONAL) > 0 || ProfileINI.GetPurchaseValue(StorePurchase.BLOCK_KIND_STAIR) > 0)
				{
					KGUI.EnableNodes("hud.custom_blocks.icon_01", ProfileINI.GetPurchaseValue(StorePurchase.BLOCK_KIND_FENCE) > 0);
					KGUI.EnableNodes("hud.custom_blocks.icon_02", ProfileINI.GetPurchaseValue(StorePurchase.BLOCK_KIND_HALF) > 0);
					KGUI.EnableNodes("hud.custom_blocks.icon_03", ProfileINI.GetPurchaseValue(StorePurchase.BLOCK_KIND_DIAGONAL) > 0);
					KGUI.EnableNodes("hud.custom_blocks.icon_04", ProfileINI.GetPurchaseValue(StorePurchase.BLOCK_KIND_STAIR) > 0);
					((Component)KGUI.FindNode("hud.custom_blocks")).GetComponent<UITable>().Reposition();
					KGUI.EnableNodes("hud.custom_blocks.icon_01_selected", WorldGameObject.Instance.CurrentCommonKind == CommonBlockKind.Fence);
					KGUI.EnableNodes("hud.custom_blocks.icon_02_selected", WorldGameObject.Instance.CurrentCommonKind == CommonBlockKind.Half);
					KGUI.EnableNodes("hud.custom_blocks.icon_03_selected", WorldGameObject.Instance.CurrentCommonKind == CommonBlockKind.Diagonal);
					KGUI.EnableNodes("hud.custom_blocks.icon_04_selected", WorldGameObject.Instance.CurrentCommonKind == CommonBlockKind.Stair);
					KGUI.EnableNodes("hud.custom_blocks.icon_05_selected", WorldGameObject.Instance.CurrentCommonKind == CommonBlockKind.Default);
				}
				else
				{
					KGUI.EnableNodes("hud.custom_blocks", enable: false);
				}
			}
			KGUI.EnableNodes("hud.bonuses", enable: false, onlyChild: true);
			RefreshFlying();
			RefreshAcceleration();
			RefreshDayNight();
			if (CleanScreen)
			{
				((Component)KGUI.FindNode("hud")).gameObject.SetActive(false);
			}
		}, () =>
		{
		});
		RegisterMenu(Menu.Start, "main_menu_start", (object data, object dataEx) =>
		{
			KGUI.SetControlText("main_menu.txt_title", Localize.GetText("START_MENU"));
			KGUI.EnableNodes("main_menu_start.page1_fast_game", enable: false);
			KGUI.EnableNodes("main_menu_start.page1_select_map", enable: false);
			KGUI.EnableNodes("main_menu_start.page2_start.txt_news_loading", _NewsText == null);
			RefreshNews();
		}, () =>
		{
			KGUI.SetButtonCallback("main_menu_start.ibtn_fast_game", () =>
			{
				KGUI.EnableNodes("main_menu_start.page1_start", enable: false);
				KGUI.EnableNodes("main_menu_start.page1_fast_game", enable: true);
				KGUI.EnableNodes("main_menu.button_back", enable: true);
				RefreshStartFastGame(0);
			});
			KGUI.SetButtonCallback("main_menu_start.page1_fast_game.ibtn_next_game", () =>
			{
				RefreshStartFastGame(1);
			});
			KGUI.SetButtonCallback("main_menu_start.page1_fast_game.ibtn_prev_game", () =>
			{
				RefreshStartFastGame(-1);
			});
			KGUI.SetButtonCallback("main_menu_start.page1_fast_game.ibtn_play", () =>
			{
				if (_FastGameType == -1)
				{
					SwitchMenu(Menu.MyMaps);
				}
				else if (_FastGameMaps.Count > 0)
				{
					KGUI.EnableNodes("main_menu_start.page1_fast_game", enable: false);
					KGUI.EnableNodes("main_menu_start.page1_select_map", enable: true);
					_FastGameMap = 0;
					RefreshStartSelectMap(0);
				}
				else
				{
					((MonoBehaviour)this).StartCoroutine(FastConnectToGame(_MapGameTypes[_FastGameType]));
				}
			});
			KGUI.SetButtonCallback("main_menu_start.page1_select_map.ibtn_next_map", () =>
			{
				RefreshStartSelectMap(1);
			});
			KGUI.SetButtonCallback("main_menu_start.page1_select_map.ibtn_prev_map", () =>
			{
				RefreshStartSelectMap(-1);
			});
			KGUI.SetButtonCallback("main_menu_start.page1_select_map.ibtn_play2", () =>
			{
				Store.StandartMap standartMap = _FastGameMaps[_FastGameMap];
				_MapGameType = _MapGameTypes.ToList().IndexOf(standartMap.Game);
				_MapDestroyable = standartMap.Destroyable;
				JoinOrCreateGame(App.Instance.GetProtocol() + standartMap.MapURL, -1, isWatch: false, standartMap.Name, standartMap.Description, string.Empty);
			});
			KGUI.SetButtonCallback("main_menu_start.ibtn_my_maps", () =>
			{
				SwitchMenu(Menu.MyMaps);
			});
			KGUI.SetButtonCallback("main_menu_start.ibtn_servers_list", () =>
			{
				SwitchMenu(Menu.Servers);
			});
			KGUI.SetButtonCallback("main_menu_start.clip_news.screen", () =>
			{
				if (_NewsPictureURL[_CurNewsBlock].Length > 0)
				{
					Application.ExternalCall("OpenUrl", new object[1] { _NewsPictureURL[_CurNewsBlock] });
				}
			});
			KGUI.SetButtonCallback("main_menu_start.clip_news.ibtn_look", () =>
			{
				Application.ExternalCall("OpenUrl", new object[1] { _NewsPictureURL[_CurNewsBlock] });
			});
			KGUI.SetButtonCallback("main_menu_start.clip_news.ibtn_bookmark1", () =>
			{
				_CurNewsBlock = 0;
				_NewsAutoScroll = false;
				RefreshNews();
			});
			KGUI.SetButtonCallback("main_menu_start.clip_news.ibtn_bookmark2", () =>
			{
				_CurNewsBlock = 1;
				_NewsAutoScroll = false;
				RefreshNews();
			});
			KGUI.SetButtonCallback("main_menu_start.clip_news.ibtn_bookmark3", () =>
			{
				_CurNewsBlock = 2;
				_NewsAutoScroll = false;
				RefreshNews();
			});
			KGUI.SetButtonCallback("main_menu_start.clip_news.ibtn_bookmark4", () =>
			{
				_CurNewsBlock = 3;
				_NewsAutoScroll = false;
				RefreshNews();
			});
		});
		RegisterMenu(Menu.MyMaps, "main_menu_my_maps", (object data, object dataEx) =>
		{
			KGUI.SetControlText("main_menu.txt_title", Localize.GetText("MY_MAPS_TITLE1"));
			KGUI.EnableNodes("main_menu_my_maps.page2", enable: false, onlyChild: true);
			KGUI.ResizeGrid("main_menu_my_maps.my_maps_grid", 1 + ProfileINI.GetPurchaseValue(StorePurchase.SLOTS), (GameObject slot, int index) =>
			{
				((Component)slot.transform.Find("txt_title")).GetComponent<UILabel>().text = Localize.GetText("MY_MAPS_MAP") + " " + (index + 1);
			}, "main_menu_my_maps");
		}, () =>
		{
			KGUI.SetButtonCallback("main_menu_my_maps.my_maps_grid.ibtn_select", () =>
			{
				((MonoBehaviour)this).StartCoroutine(LoadSlotMapInfo(VkontakteAPI.INSTANCE._viewerId, KGUI.SlotIndex + 1));
			});
			KGUI.SetButtonCallback("main_menu_my_maps.ibtn_buy_slot", () =>
			{
				((MonoBehaviour)this).StartCoroutine(PurchaseBuy(StorePurchase.SLOTS));
			});
			KGUI.SetButtonCallback("main_menu_my_maps.page2.info.map_size.ibtn_next", () =>
			{
				ChangeMapSize(next: true);
			});
			KGUI.SetButtonCallback("main_menu_my_maps.page2.info.map_size.ibtn_prev", () =>
			{
				ChangeMapSize(next: false);
			});
			KGUI.SetButtonCallback("main_menu_my_maps.page2.info.map_type.ibtn_next", () =>
			{
				ChangeMapType(next: true);
			});
			KGUI.SetButtonCallback("main_menu_my_maps.page2.info.map_type.ibtn_prev", () =>
			{
				ChangeMapType(next: false);
			});
			KGUI.SetButtonCallback("main_menu_my_maps.page2.info.map_time.ibtn_next", () =>
			{
				ChangeMapTime(next: true);
			});
			KGUI.SetButtonCallback("main_menu_my_maps.page2.info.map_time.ibtn_prev", () =>
			{
				ChangeMapTime(next: false);
			});
			KGUI.SetButtonCallback("main_menu_my_maps.page2.info.ibtn_create_map", () =>
			{
				ProfileINI.server_name = KGUI.GetControlText("main_menu_my_maps.page2.info.name.inp_name");
				ProfileINI.server_about = KGUI.GetControlText("main_menu_my_maps.page2.info.description.inp_description");
				string controlText = KGUI.GetControlText("main_menu_my_maps.page2.info.password.inp_password");
				if (ProfileINI.server_name.Length <= 3)
				{
					ShowHint("Название комнаты должно быть больше 3 букв");
				}
				else
				{
					bool flag = false;
					RoomInfo[] roomList = PhotonNetwork.GetRoomList();
					foreach (RoomInfo roomInfo in roomList)
					{
						if ((string)roomInfo.customProperties["player_id"] == VkontakteAPI.INSTANCE._viewerId && (int)roomInfo.customProperties["slot_id"] == _SelectedMapSlot)
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						ShowHint("Нельзя перегенерировать карту пока на ней кто-либо играет");
					}
					else
					{
						bool flag2 = false;
						RoomInfo[] roomList2 = PhotonNetwork.GetRoomList();
						foreach (RoomInfo roomInfo2 in roomList2)
						{
							if ((string)roomInfo2.customProperties["map_name"] == ProfileINI.server_name)
							{
								flag2 = true;
								break;
							}
						}
						if (flag2)
						{
							ShowHint("Комната с таким названием существует");
						}
						else
						{
							InitializeGameINI();
							App.Instance.Settings.server_name = ProfileINI.server_name;
							App.Instance.Settings.server_about = ProfileINI.server_about;
							App.Instance.Settings.gameType = GameINI.GameType.BUILDING;
							App.Instance.Settings.mapSize = GameINI.MapSize.SMALL;
							App.Instance.Settings.mapPopulation = 12;
							App.Instance.Settings.isOnline = true;
							App.Instance.Settings.isServer = true;
							App.Instance.Settings.isServerAdministrator = true;
							App.Instance.Settings.isWatch = false;
							App.Instance.Settings.loadingSavedMap = false;
							App.Instance.Settings.playerID = VkontakteAPI.INSTANCE._viewerId;
							App.Instance.Settings.slotID = _SelectedMapSlot;
							App.Instance.Settings.publicMapID = 0;
							App.Instance.Settings.Password = controlText;
							App.Instance.Settings.mapSize = _MapSizes[_MapSize];
							App.Instance.Settings.mapType = _MapTypes[_MapType];
							App.Instance.Settings.mapTime = _MapTimes[_MapTime];
							HideMenu();
							ShowLoading("LOADING_LEVEL", string.Empty);
							Application.LoadLevel("Game");
						}
					}
				}
			});
			KGUI.SetButtonCallback("main_menu_my_maps.page2.base.ibtn_load", () =>
			{
				ShowMyMap(isGenerate: false, isToPublic: false);
			});
			KGUI.SetButtonCallback("main_menu_my_maps.page2.base.ibtn_generate", () =>
			{
				ShowMyMap(isGenerate: true, isToPublic: false);
			});
			KGUI.SetButtonCallback("main_menu_my_maps.page2.base.ibtn_public", () =>
			{
				ShowMyMap(isGenerate: false, isToPublic: true);
			});
			KGUI.SetButtonCallback("main_menu_my_maps.page2.base.ibtn_look_in_public", () =>
			{
				SwitchMenu(Menu.TopMaps, _SelectedMapID);
			});
			KGUI.SetButtonCallback("main_menu_my_maps.page2.base.ibtn_remove_from_public", () =>
			{
				((MonoBehaviour)this).StartCoroutine(ClosePublicMap(VkontakteAPI.INSTANCE._viewerId, _SelectedMapSlot, _SelectedMapID));
			});
			KGUI.SetButtonCallback("main_menu_my_maps.page2.base.ibtn_save", () =>
			{
				if (_SelectedMapSlot != 0)
				{
					((MonoBehaviour)this).StartCoroutine(World.Instance.SaveWorldToServer(_SelectedMapSlot));
				}
			});
			KGUI.SetButtonCallback("main_menu_my_maps.page2.info.ibtn_load_map", () =>
			{
				if (KGUI.GetControlText("main_menu_my_maps.page2.info.name.inp_name").Length <= 3)
				{
					ShowHint("Название комнаты должно быть больше 3 букв");
				}
				else
				{
					JoinOrCreateGame(VkontakteAPI.INSTANCE._viewerId, _SelectedMapSlot, isWatch: false, KGUI.GetControlText("main_menu_my_maps.page2.info.inp_name"), KGUI.GetControlText("main_menu_my_maps.page2.info.inp_description"), KGUI.GetControlText("main_menu_my_maps.page2.info.inp_password"));
				}
			});
			KGUI.SetButtonCallback("main_menu_my_maps.page2.info.game_type.ibtn_next", () =>
			{
				if (_MapGameType < _MapGameTypes.Length - 1)
				{
					_MapGameType++;
					KGUI.SetControlText("main_menu_my_maps.page2.info.game_type.txt_type", Localize.GetText("GAME_TYPE_" + _MapGameTypes[_MapGameType]));
					KGUI.EnableNodes("main_menu_my_maps.page2.info.destroyable", _MapGameTypes[_MapGameType] != GameINI.GameType.BUILDING);
					KGUI.EnableNodes("main_menu_my_maps.page2.info.bonuses", _MapGameTypes[_MapGameType] != GameINI.GameType.BUILDING);
					((Component)KGUI.FindNode("main_menu_my_maps.page2.info.grid_map_info")).GetComponent<UITable>().Reposition();
					KGUI.ResetScrollBar("main_menu_my_maps.page2.info.clip_map_info");
				}
			});
			KGUI.SetButtonCallback("main_menu_my_maps.page2.info.game_type.ibtn_prev", () =>
			{
				if (_MapGameType > 0)
				{
					_MapGameType--;
					KGUI.SetControlText("main_menu_my_maps.page2.info.game_type.txt_type", Localize.GetText("GAME_TYPE_" + _MapGameTypes[_MapGameType]));
					KGUI.EnableNodes("main_menu_my_maps.page2.info.destroyable", _MapGameTypes[_MapGameType] != GameINI.GameType.BUILDING);
					KGUI.EnableNodes("main_menu_my_maps.page2.info.bonuses", _MapGameTypes[_MapGameType] != GameINI.GameType.BUILDING);
					((Component)KGUI.FindNode("main_menu_my_maps.page2.info.grid_map_info")).GetComponent<UITable>().Reposition();
					KGUI.ResetScrollBar("main_menu_my_maps.page2.info.clip_map_info");
				}
			});
			KGUI.SetCheckboxCallback("main_menu_my_maps.page2.info.destroyable.ckb_destroyable", () =>
			{
				_MapDestroyable = KGUI.CallbackSender.GetComponent<UICheckbox>().isChecked;
			});
			KGUI.SetCheckboxCallback("main_menu_my_maps.page2.info.bonuses.ckb_bonuses", () =>
			{
				_MapBonuses = KGUI.CallbackSender.GetComponent<UICheckbox>().isChecked;
			});
			KGUI.SetButtonCallback("main_menu_my_maps.page2.info.ibtn_public_map", () =>
			{
				string controlText = KGUI.GetControlText("main_menu_my_maps.page2.info.inp_name");
				if (controlText.Length <= 3)
				{
					ShowHint("Название комнаты должно быть больше 3 букв");
				}
				else
				{
					((MonoBehaviour)this).StartCoroutine(PublicMapToServer(controlText, KGUI.GetControlText("main_menu_my_maps.page2.info.inp_description"), _CustomMapImageURL));
				}
			});
			KGUI.SetButtonCallback(new string[2] { "main_menu_my_maps.page2.info.screen", "main_menu_my_maps.page2.info.screen_custom" }, () =>
			{
				ShowInputText("INPUT_TEXT_PICTURE_URL1", "INPUT_TEXT_PICTURE_URL2", (string text) =>
				{
					((MonoBehaviour)this).StartCoroutine(GetCustomMapImage(_SelectedMapID, "main_menu_my_maps.page2.info.screen", text));
				});
			});
		});
		RegisterMenu(Menu.Settings, "main_menu_settings", (object data, object dataEx) =>
		{
			KGUI.SetControlText("main_menu.txt_title", Localize.GetText("SETTINGS_TITLE1"));
			UISlider component = ((Component)KGUI.FindNode("main_menu_settings.sld_hor_mouse_sensivity")).GetComponent<UISlider>();
			UISlider component2 = ((Component)KGUI.FindNode("main_menu_settings.sld_hor_sound_volume")).GetComponent<UISlider>();
			UISlider component3 = ((Component)KGUI.FindNode("main_menu_settings.sld_hor_ambient_volume")).GetComponent<UISlider>();
			component.sliderValue = (ProfileINI.mouse_sens - 1f) / 29f;
			component2.sliderValue = ProfileINI.sound_volume;
			component3.sliderValue = ProfileINI.ambient_volume;
			KGUI.SetControlText("main_menu_settings.mouse_sensivity.txt_percent", (int)(component.sliderValue * 100f) + "%");
			KGUI.SetControlText("main_menu_settings.sound_volume.txt_percent", (int)(component2.sliderValue * 100f) + "%");
			KGUI.SetControlText("main_menu_settings.ambient_volume.txt_percent", (int)(component3.sliderValue * 100f) + "%");
			string text = Localize.GetText("DESTRUCTION_" + ProfileINI.options_destroy switch
			{
				0 => "SIMPLE", 
				1 => "MEDIUM", 
				_ => "EPIC", 
			});
			KGUI.SetControlText("main_menu_settings.destruction.txt_type", text);
			KGUI.SetControlCheckbox("main_menu_settings.ckb_show_bubbles", ProfileINI.showBaloons);
		}, () =>
		{
			KGUI.SetSliderCallback("main_menu_settings.sld_hor_mouse_sensivity", () =>
			{
				UISlider component = KGUI.CallbackSender.GetComponent<UISlider>();
				ProfileINI.mouse_sens = component.sliderValue * 29f + 1f;
				ProfileINI.Save();
				KGUI.SetControlText("main_menu_settings.mouse_sensivity.txt_percent", (int)(component.sliderValue * 100f) + "%");
			});
			KGUI.SetSliderCallback("main_menu_settings.sld_hor_sound_volume", () =>
			{
				UISlider component = KGUI.CallbackSender.GetComponent<UISlider>();
				ProfileINI.sound_volume = component.sliderValue;
				ProfileINI.Save();
				KGUI.SetControlText("main_menu_settings.sound_volume.txt_percent", (int)(component.sliderValue * 100f) + "%");
			});
			KGUI.SetSliderCallback("main_menu_settings.sld_hor_ambient_volume", () =>
			{
				UISlider component = KGUI.CallbackSender.GetComponent<UISlider>();
				ProfileINI.ambient_volume = component.sliderValue;
				ProfileINI.Save();
				KGUI.SetControlText("main_menu_settings.ambient_volume.txt_percent", (int)(component.sliderValue * 100f) + "%");
			});
			KGUI.SetButtonCallback("main_menu_settings.destruction.ibtn_next", () =>
			{
				if (ProfileINI.options_destroy < 2)
				{
					ProfileINI.options_destroy++;
					((MonoBehaviour)this).StartCoroutine(ProfileINI.SaveDestroy());
					string text = Localize.GetText("DESTRUCTION_" + ProfileINI.options_destroy switch
					{
						0 => "SIMPLE", 
						1 => "MEDIUM", 
						_ => "EPIC", 
					});
					KGUI.SetControlText("main_menu_settings.destruction.txt_type", text);
				}
			});
			KGUI.SetButtonCallback("main_menu_settings.destruction.ibtn_prev", () =>
			{
				if (ProfileINI.options_destroy > 0)
				{
					ProfileINI.options_destroy--;
					((MonoBehaviour)this).StartCoroutine(ProfileINI.SaveDestroy());
					string text = Localize.GetText("DESTRUCTION_" + ProfileINI.options_destroy switch
					{
						0 => "SIMPLE", 
						1 => "MEDIUM", 
						_ => "EPIC", 
					});
					KGUI.SetControlText("main_menu_settings.destruction.txt_type", text);
				}
			});
			KGUI.SetCheckboxCallback("main_menu_settings.ckb_show_bubbles", () =>
			{
				UICheckbox component = KGUI.CallbackSender.GetComponent<UICheckbox>();
				ProfileINI.showBaloons = component.isChecked;
				ProfileINI.Save();
			});
		});
		RegisterMenu(Menu.Shop, "main_menu_shop", (object data, object dataEx) =>
		{
			KGUI.SetControlText("main_menu.txt_title", Localize.GetText("SHOP_TITLE1"));
			KGUI.EnableNodes("main_menu_shop.page2_shop", enable: false);
			if (data != null)
			{
				_ShopTab = (Store.TabType)(int)data;
			}
			RefreshShop();
		}, () =>
		{
			List<string> list = new List<string>();
			for (int i = 0; i < Store.TabTypeButtonNames.Length; i++)
			{
				string text = "main_menu_shop" + Store.TabTypeButtonNames[i];
				if ((Object)(object)KGUI.FindNode(text, mayBeNull: true) != (Object)null)
				{
					list.Add(text);
				}
			}
			KGUI.SetButtonCallback(list.ToArray(), () =>
			{
				Store.TabType shopTab = _ShopTab;
				for (int j = 0; j < Store.TabTypeButtonNames.Length; j++)
				{
					if (Store.TabTypeButtonNames[j].Contains(((Object)KGUI.CallbackSender).name))
					{
						_ShopTab = (Store.TabType)j;
						break;
					}
				}
				if (_ShopTab != shopTab)
				{
					RefreshShop();
				}
			});
			KGUI.SetButtonCallback("main_menu_shop.items.ibtn_select", () =>
			{
				ShowPurchaseItem(_CurShopPurchases[_CurShopPurchases.Keys.ElementAt(KGUI.SlotIndex2)][KGUI.SlotIndex]);
			});
			KGUI.SetButtonCallback("main_menu_shop.icons.ibtn_next_items", () =>
			{
				((Component)KGUI.FindNode("main_menu_shop.clip_icons")).GetComponent<UIDraggablePanel>().Scroll(0.4f);
			});
			KGUI.SetButtonCallback("main_menu_shop.icons.ibtn_prev_items", () =>
			{
				((Component)KGUI.FindNode("main_menu_shop.clip_icons")).GetComponent<UIDraggablePanel>().Scroll(-0.4f);
			});
			KGUI.SetButtonCallback("main_menu_shop.page2_shop.ibtn_item_look", () =>
			{
				GetPurchaseContent(out var purchaseEntities, out var purchaseBlocks);
				SetMenu(Menu.ItemsPack, show: true, purchaseEntities, purchaseBlocks);
			});
			KGUI.SetButtonCallback("main_menu_shop.page2_shop.ibtn_item_info", () =>
			{
				_ShopPurchaseItemInfo = true;
				ShowPurchaseItem(_CurPurchase);
			});
			KGUI.SetButtonCallback("main_menu_shop.page2_shop.ibtn_item_back", () =>
			{
				_ShopPurchaseItemInfo = false;
				ShowPurchaseItem(_CurPurchase);
			});
			KGUI.SetButtonCallback("main_menu_shop.page2_shop.ibtn_rotate_left", () =>
			{
			});
			KGUI.SetButtonCallback("main_menu_shop.page2_shop.ibtn_rotate_right", () =>
			{
			});
			KGUI.SetButtonCallback("main_menu_shop.page2_shop.ibtn_buy_one", () =>
			{
				((MonoBehaviour)this).StartCoroutine(PurchaseBuy(_CurPurchase));
			});
			KGUI.SetButtonCallback("main_menu_shop.page2_shop.ibtn_buy_day", () =>
			{
				((MonoBehaviour)this).StartCoroutine(PurchaseBuy(_CurPurchase, 1));
			});
			KGUI.SetButtonCallback("main_menu_shop.page2_shop.ibtn_buy_week", () =>
			{
				((MonoBehaviour)this).StartCoroutine(PurchaseBuy(_CurPurchase, 2));
			});
			KGUI.SetButtonCallback("main_menu_shop.page2_shop.ibtn_buy_forever", () =>
			{
				((MonoBehaviour)this).StartCoroutine(PurchaseBuy(_CurPurchase, 3));
			});
			KGUI.SetButtonCallback("main_menu_shop.page2_shop.ibtn_buy_month", () =>
			{
				((MonoBehaviour)this).StartCoroutine(PurchaseBuy(_CurPurchase, 4));
			});
			KGUI.SetButtonCallback("main_menu_shop.page2_shop.ibtn_equip", () =>
			{
				((MonoBehaviour)this).StartCoroutine(ProfileINI.SetSkin(Store.Purchases[_CurPurchase].Skin));
				ShowPurchaseItem(_CurPurchase);
			});
			KGUI.SetButtonCallback("main_menu_shop.page2_shop.ibtn_use", () =>
			{
				if (_CurPurchase == StorePurchase.CHANGENICK)
				{
					SwitchMenu(Menu.ChangeName);
				}
			});
		});
		RegisterMenu(Menu.ItemsPack, "items_pack", (object data, object dataEx) =>
		{
			List<EntityType> entities = (List<EntityType>)data;
			List<BlockType> blocks = (List<BlockType>)dataEx;
			KGUI.ResizeGrid("items_pack.grid_items", entities.Count + blocks.Count, (GameObject slot, int index) =>
			{
				string spriteName = ((index >= entities.Count) ? Store.Blocks[blocks[index - entities.Count]].SpriteName.Value : Store.Entities[entities[index]].SpriteName.Value);
				((Component)slot.transform.Find("icon")).GetComponent<UISprite>().spriteName = spriteName;
			}, "emotions");
		}, () =>
		{
			KGUI.SetButtonCallback("items_pack.ibtn_close", () =>
			{
				SetMenu(Menu.ItemsPack, show: false);
			});
		});
		RegisterMenu(Menu.Inventory, "inventory", (object data, object dataEx) =>
		{
			if (data != null)
			{
				_InventoryTab = (Store.TabType)(int)data;
			}
			RefreshInventory();
		}, () =>
		{
			List<string> list = new List<string>();
			for (int i = 0; i < Store.TabTypeButtonNames.Length; i++)
			{
				string text = "inventory" + Store.TabTypeButtonNames[i];
				if ((Object)(object)KGUI.FindNode(text, mayBeNull: true) != (Object)null)
				{
					list.Add(text);
				}
			}
			KGUI.SetButtonCallback(list.ToArray(), () =>
			{
				Store.TabType inventoryTab = _InventoryTab;
				for (int j = 0; j < Store.TabTypeButtonNames.Length; j++)
				{
					if (Store.TabTypeButtonNames[j].Contains(((Object)KGUI.CallbackSender).name))
					{
						_InventoryTab = (Store.TabType)j;
						if (_InventoryTab == Store.TabType.Blocks)
						{
							_InventoryTab = Store.TabType.Blocks_Wood;
						}
						break;
					}
				}
				if (_InventoryTab != inventoryTab)
				{
					RefreshInventory();
				}
			});
			KGUI.SetButtonCallback("inventory.items.ibtn_select", () =>
			{
				if (!GameType.BuildDisabled())
				{
					if (!_InventoryTab.ToString().Contains("Blocks"))
					{
						EntityType entityType = _CurInventoryItems[_CurInventoryItems.Keys.ElementAt(KGUI.SlotIndex2)][KGUI.SlotIndex];
						if (entityType != EntityType.RAIL)
						{
							WorldGameObject.Instance.AddPreview(entityType);
							StorePurchase storePurchase = Store.Entities[entityType].Purchase;
							if (storePurchase != StorePurchase.NONE)
							{
								Store.Pay cost = Store.Purchases[storePurchase].Cost;
								if (cost is Store.OnePay && !((Store.OnePay)cost).Once && ProfileINI.CheckPurchaseValue(storePurchase))
								{
									int purchaseValue = ProfileINI.GetPurchaseValue(storePurchase);
									if (purchaseValue > 0)
									{
										ProfileINI.SetPurchaseValue(storePurchase, purchaseValue - 1);
										((MonoBehaviour)this).StartCoroutine(PurchaseUse(storePurchase));
									}
								}
							}
						}
						else
						{
							WorldGameObject.Instance.CurrentBlock = BlockType.Air;
							WorldGameObject.Instance.CurrentBlockEntity = EntityType.RAIL;
							PreviewCubes.Instance.DrawRail();
						}
					}
					else
					{
						BlockType blockType = _CurInventoryBlocks[_CurInventoryBlocks.Keys.ElementAt(KGUI.SlotIndex2)][KGUI.SlotIndex];
						WorldGameObject.Instance.CurrentBlock = blockType;
						WorldGameObject.Instance.CurrentBlockEntity = EntityType.AIR;
						PreviewCubes.Instance.DrawCube();
						PreviewCubes.Instance.Cube.renderer.material.mainTexture = (Texture)(object)WorldData.Instance.BlockTextures[blockType];
					}
					HideMenu();
				}
			});
			KGUI.SetButtonCallback("inventory.icons.ibtn_next_items", () =>
			{
				((Component)KGUI.FindNode("inventory.clip_icons")).GetComponent<UIDraggablePanel>().Scroll(0.4f);
			});
			KGUI.SetButtonCallback("inventory.icons.ibtn_prev_items", () =>
			{
				((Component)KGUI.FindNode("inventory.clip_icons")).GetComponent<UIDraggablePanel>().Scroll(-0.4f);
			});
			KGUI.SetButtonCallback("inventory.ibtn_back", () =>
			{
				if (_InventoryTab.ToString().Contains("Blocks_"))
				{
					_InventoryTab = Store.TabType.Blocks;
					RefreshInventory();
				}
				else
				{
					HideMenu();
				}
			});
			KGUI.SetButtonCallback("inventory.ibtn_shop", () =>
			{
				if (_InventoryTab.ToString().Contains("Blocks_"))
				{
					SwitchMenu(Menu.Shop, Store.TabType.Blocks);
				}
				else
				{
					SwitchMenu(Menu.Shop, _InventoryTab);
				}
			});
		});
		RegisterMenu(Menu.Servers, "main_menu_servers", (object data, object dataEx) =>
		{
			KGUI.SetControlText("main_menu.txt_title", Localize.GetText("SERVERS_TITLE1"));
			KGUI.EnableNodes("main_menu.slider_page1", enable: true);
			KGUI.EnableNodes("main_menu_servers.page2", enable: false);
			RefreshServersList();
		}, () =>
		{
			KGUI.SetInputCallback("main_menu_servers.search.inp_box", () =>
			{
				RefreshServersList();
			});
			KGUI.SetButtonCallback("main_menu_servers.servers_grid.ibtn_select", () =>
			{
				RoomInfo roomInfo = (_SelectedRoom = _ServerRooms[KGUI.SlotIndex]);
				KGUI.EnableNodes("main_menu_servers.page2", enable: true);
				KGUI.SetControlText("main_menu_servers.server_info_name.txt_name", (string)roomInfo.customProperties["map_name"]);
				KGUI.SetControlText("main_menu_servers.server_info_players.txt_players", roomInfo.playerCount + "/" + roomInfo.maxPlayers);
				KGUI.EnableNodes("main_menu_servers.button_info", ((string)roomInfo.customProperties["map_about"]).Length > 0);
			});
			KGUI.SetButtonCallback("main_menu_servers.sorting.ibtn_team_red", () =>
			{
				_ServersShowCTF = !_ServersShowCTF;
				RefreshServersList();
			});
			KGUI.SetButtonCallback("main_menu_servers.sorting.ibtn_gun", () =>
			{
				_ServersShowTeamBattle = !_ServersShowTeamBattle;
				RefreshServersList();
			});
			KGUI.SetButtonCallback("main_menu_servers.sorting.ibtn_skull", () =>
			{
				_ServersShowDeathmatch = !_ServersShowDeathmatch;
				RefreshServersList();
			});
			KGUI.SetButtonCallback("main_menu_servers.sorting.ibtn_mining", () =>
			{
				_ServersShowBuilding = !_ServersShowBuilding;
				RefreshServersList();
			});
			KGUI.SetButtonCallback("main_menu_servers.sorting.ibtn_zombie", () =>
			{
				_ServersShowZombieVirus = !_ServersShowZombieVirus;
				RefreshServersList();
			});
			KGUI.SetButtonCallback("main_menu_servers.ibtn_enter", () =>
			{
				if (_SelectedRoom.customProperties.ContainsKey("password") && ((string)_SelectedRoom.customProperties["password"]).Length > 0)
				{
					ShowInputText("SERVERS_PASSWORD", "SERVERS_ENTER_PASSWORD", (string text) =>
					{
						if ((string)_SelectedRoom.customProperties["password"] == text)
						{
							JoinGame(_SelectedRoom);
						}
						else
						{
							ShowHint("SERVERS_WRONG_PASSWORD");
						}
					});
				}
				else
				{
					JoinGame(_SelectedRoom);
				}
			});
			KGUI.SetButtonCallback("main_menu_servers.ibtn_info", () =>
			{
				ShowHint((string)_SelectedRoom.customProperties["map_about"]);
			});
			KGUI.SetButtonCallback("main_menu_servers.sorting.ibtn_next", () =>
			{
				((Component)KGUI.FindNode("main_menu_servers.sorting.clip_icons")).GetComponent<UIDraggablePanel>().Scroll(0.4f);
			});
			KGUI.SetButtonCallback("main_menu_servers.sorting.ibtn_prev", () =>
			{
				((Component)KGUI.FindNode("main_menu_servers.sorting.clip_icons")).GetComponent<UIDraggablePanel>().Scroll(-0.4f);
			});
		});
		RegisterMenu(Menu.TopMaps, "main_menu_top_maps", (object data, object dataEx) =>
		{
			KGUI.SetControlText("main_menu.txt_title", Localize.GetText("TOP_MAPS_TITLE1"));
			KGUI.EnableNodes("main_menu_top_maps.page2", enable: false);
			KGUI.ResizeGrid("main_menu_top_maps.top_maps_grid", 0, (GameObject slot, int index) =>
			{
			});
			KGUI.EnableNodes("main_menu_top_maps.buttons_gag", enable: false, onlyChild: true);
			if (data == null)
			{
				KGUI.EnableNodes("main_menu_top_maps.buttons_gag.new_maps", enable: true);
				KGUI.SetControlText("main_menu_top_maps.txt_best_period", Localize.GetText("TOP_MAPS_NEW"));
				((MonoBehaviour)this).StartCoroutine(RefreshListOfMapsNew());
			}
			else
			{
				KGUI.SetControlText("main_menu_top_maps.txt_best_period", string.Empty);
				((MonoBehaviour)this).StartCoroutine(LoadTopMapInfo((int)data));
			}
		}, () =>
		{
			KGUI.SetButtonCallback("main_menu_top_maps.ibtn_new_maps", () =>
			{
				KGUI.SetControlText("main_menu_top_maps.txt_best_period", Localize.GetText("TOP_MAPS_NEW"));
				KGUI.EnableNodes("main_menu_top_maps.buttons_gag", enable: false, onlyChild: true);
				KGUI.EnableNodes("main_menu_top_maps.buttons_gag.new_maps", enable: true);
				((MonoBehaviour)this).StartCoroutine(RefreshListOfMapsNew());
			});
			KGUI.SetButtonCallback("main_menu_top_maps.ibtn_best_maps", () =>
			{
				KGUI.SetControlText("main_menu_top_maps.txt_best_period", Localize.GetText("TOP_MAPS_ALL"));
				KGUI.EnableNodes("main_menu_top_maps.buttons_gag", enable: false, onlyChild: true);
				KGUI.EnableNodes("main_menu_top_maps.buttons_gag.best_maps", enable: true);
				((MonoBehaviour)this).StartCoroutine(RefreshListOfMapsBestAllTime());
			});
			KGUI.SetButtonCallback("main_menu_top_maps.ibtn_best_maps_for_day", () =>
			{
				KGUI.SetControlText("main_menu_top_maps.txt_best_period", Localize.GetText("TOP_MAPS_DAY"));
				KGUI.EnableNodes("main_menu_top_maps.buttons_gag", enable: false, onlyChild: true);
				KGUI.EnableNodes("main_menu_top_maps.buttons_gag.best_maps_for_day", enable: true);
				((MonoBehaviour)this).StartCoroutine(RefreshListOfDay());
			});
			KGUI.SetButtonCallback("main_menu_top_maps.ibtn_best_maps_for_week", () =>
			{
				KGUI.SetControlText("main_menu_top_maps.txt_best_period", Localize.GetText("TOP_MAPS_WEEK"));
				KGUI.EnableNodes("main_menu_top_maps.buttons_gag", enable: false, onlyChild: true);
				KGUI.EnableNodes("main_menu_top_maps.buttons_gag.best_maps_for_week", enable: true);
				((MonoBehaviour)this).StartCoroutine(RefreshListOfMapsWeekly());
			});
			KGUI.SetButtonCallback("main_menu_top_maps.ibtn_best_maps_for_month", () =>
			{
				KGUI.SetControlText("main_menu_top_maps.txt_best_period", Localize.GetText("TOP_MAPS_MONTH"));
				KGUI.EnableNodes("main_menu_top_maps.buttons_gag", enable: false, onlyChild: true);
				KGUI.EnableNodes("main_menu_top_maps.buttons_gag.best_maps_for_month", enable: true);
				((MonoBehaviour)this).StartCoroutine(RefreshListOfMapsMonthly());
			});
			KGUI.SetButtonCallback("main_menu_top_maps.ibtn_select", () =>
			{
				if (KGUI.SlotIndex < _Maps.Count)
				{
					((MonoBehaviour)this).StartCoroutine(LoadTopMapInfo(int.Parse(_Maps[KGUI.SlotIndex].MapID)));
				}
				else
				{
					((MonoBehaviour)this).StartCoroutine(LoadMoreMaps());
				}
			});
			KGUI.SetButtonCallback("main_menu_top_maps.ibtn_look", () =>
			{
				JoinOrCreateGame(_SelectedMapOwner, _SelectedMapSlot, isWatch: true, KGUI.GetControlText("main_menu_top_maps.page2.txt_name"), KGUI.GetControlText("main_menu_top_maps.page2.txt_description"), string.Empty);
			});
			KGUI.SetButtonCallback("main_menu_top_maps.ibtn_copy_url", () =>
			{
				string text = null;
				if (App.Instance.CurPlatform == App.Platform.VKONTATE)
				{
					text = "http://vk.com/app2721929#map_id_" + _SelectedMapID;
				}
				else if (App.Instance.CurPlatform == App.Platform.ONDOKLASNIKI)
				{
					text = "http://www.odnoklassniki.ru/game/kopatel-online?map_id_" + _SelectedMapID;
				}
				else if (App.Instance.CurPlatform == App.Platform.MOYMIR)
				{
					text = "http://my.mail.ru/apps/659403?#map_id_" + _SelectedMapID;
				}
				else if (App.Instance.CurPlatform == App.Platform.WEBSITE)
				{
					text = "http://www.kopatel-online.ru/test_server.html#map_id_" + _SelectedMapID;
				}
				if (text != null)
				{
					NGUITools.clipboard = text;
					ShowHint(Localize.GetText("TOP_MAPS_COPY_URL_DONE") + "\n" + text);
				}
				else
				{
					ShowHint("TOP_MAPS_COPY_URL_UNSUPPORTED");
				}
			});
			KGUI.SetButtonCallback("main_menu_top_maps.score.ibtn_vote_yes", () =>
			{
				KGUI.EnableNodes("main_menu_top_maps.score.background_large", enable: false);
				KGUI.EnableNodes("main_menu_top_maps.score.background_small", enable: true);
				KGUI.EnableNodes("main_menu_top_maps.score.button_vote_yes", enable: false);
				int num = int.Parse(KGUI.GetControlText("main_menu_top_maps.txt_score"));
				KGUI.SetControlText("main_menu_top_maps.txt_score", string.Empty + (num + 1));
				_SelectedMapNotVoted = false;
				((MonoBehaviour)this).StartCoroutine(VoteYes(_SelectedMapID));
			});
		});
		RegisterMenu(Menu.Welcome, "welcome", (object data, object dataEx) =>
		{
			KGUI.EnableNodes("welcome.txt_hint", enable: false);
			ProfileINI.LoadLoginInfo(out var login, out var password, out var remember);
			KGUI.SetControlText("welcome.inp_login", login);
			KGUI.SetControlText("welcome.inp_password", password);
			KGUI.SetControlText("welcome.inp_repeat_password", string.Empty);
			KGUI.SetControlCheckbox("welcome.ckb_remember", remember);
			KGUI.EnableNodes("welcome.registration", enable: false);
			KGUI.EnableNodes("welcome.registration_buttons", enable: false);
			KGUI.EnableNodes("welcome.language_ru", Localize.Locale == Localize.LocaleType.RU);
			KGUI.EnableNodes("welcome.language_en", Localize.Locale == Localize.LocaleType.EN);
		}, () =>
		{
			KGUI.SetButtonCallback("welcome.ibtn_registration", () =>
			{
				KGUI.EnableNodes("welcome.play_buttons", enable: false);
				KGUI.EnableNodes("welcome.registration", enable: true);
				KGUI.EnableNodes("welcome.registration_buttons", enable: true);
			});
			KGUI.SetButtonCallback("welcome.ibtn_registration_back", () =>
			{
				KGUI.EnableNodes("welcome.play_buttons", enable: true);
				KGUI.EnableNodes("welcome.registration", enable: false);
				KGUI.EnableNodes("welcome.registration_buttons", enable: false);
			});
			KGUI.SetButtonCallback("welcome.ibtn_enter", () =>
			{
				if (KGUI.GetControlText("welcome.inp_login").Length == 0)
				{
					ShowHint("Заполните поле 'ЭЛЕКТРОННАЯ ПОЧТА'");
				}
				else if (KGUI.GetControlText("welcome.inp_password").Length == 0)
				{
					ShowHint("Заполните поле 'ПАРОЛЬ'");
				}
				else
				{
					((MonoBehaviour)this).StartCoroutine(WebEnterGame(KGUI.GetControlText("welcome.inp_login"), KGUI.GetControlText("welcome.inp_password")));
				}
			});
			KGUI.SetButtonCallback("welcome.ibtn_recover", () =>
			{
				if (KGUI.GetControlText("welcome.inp_login").Length == 0)
				{
					ShowHint("Заполните поле 'ЭЛЕКТРОННАЯ ПОЧТА'");
				}
				else
				{
					((MonoBehaviour)this).StartCoroutine(WebPasswordRecover(KGUI.GetControlText("welcome.inp_login")));
				}
			});
			KGUI.SetButtonCallback("welcome.ibtn_register", () =>
			{
				if (KGUI.GetControlText("welcome.inp_login").Length == 0)
				{
					ShowHint("Заполните поле 'ЭЛЕКТРОННАЯ ПОЧТА'");
				}
				else if (KGUI.GetControlText("welcome.inp_password").Length == 0)
				{
					ShowHint("Заполните поле 'ПАРОЛЬ'");
				}
				else if (KGUI.GetControlText("welcome.inp_repeat_password").Length == 0)
				{
					ShowHint("Заполните поле 'ПОВТОРИТЕ ПАРОЛЬ'");
				}
				else if (KGUI.GetControlText("welcome.inp_password") != KGUI.GetControlText("inp_repeat_password"))
				{
					ShowHint("Пароли не совпадают");
				}
				else
				{
					((MonoBehaviour)this).StartCoroutine(WebRegister(KGUI.GetControlText("welcome.inp_login"), KGUI.GetControlText("welcome.inp_password")));
				}
			});
			KGUI.SetCheckboxCallback("welcome.ckb_remember", () =>
			{
				ProfileINI.LoadLoginInfo(out var login, out var password, out var _);
				ProfileINI.SaveLoginInfo(login, password, KGUI.GetControlCheckbox("welcome.ckb_remember"));
			});
			KGUI.SetButtonCallback("welcome.ibtn_language_en", () =>
			{
				KGUI.SetLocale(Localize.LocaleType.EN);
				KGUI.EnableNodes("welcome.language_ru", Localize.Locale == Localize.LocaleType.RU);
				KGUI.EnableNodes("welcome.language_en", Localize.Locale == Localize.LocaleType.EN);
			});
			KGUI.SetButtonCallback("welcome.ibtn_language_ru", () =>
			{
				KGUI.SetLocale(Localize.LocaleType.RU);
				KGUI.EnableNodes("welcome.language_ru", Localize.Locale == Localize.LocaleType.RU);
				KGUI.EnableNodes("welcome.language_en", Localize.Locale == Localize.LocaleType.EN);
			});
		});
		RegisterMenu(Menu.Loading, "loading", (object data, object dataEx) =>
		{
		}, () =>
		{
		});
		RegisterMenu(Menu.Hint, "hint", (object data, object dataEx) =>
		{
			KGUI.SetControlText("hint.txt_info", Localize.GetText((string)data));
		}, () =>
		{
			KGUI.SetButtonCallback("hint.ibtn_ok", () =>
			{
				HideHint();
			});
		});
		RegisterMenu(Menu.NotEnoughMoney, "not_enough_money", (object data, object dataEx) =>
		{
			bool flag = (bool)((object[])data)[0];
			int num = (int)((object[])data)[1];
			if (flag)
			{
				KGUI.EnableNodes("not_enough_money.icon_gold", enable: false);
				KGUI.SetControlText("not_enough_money.txt_gems", string.Empty + num);
				KGUI.SetControlText("not_enough_money.txt_gold", string.Empty);
			}
			else
			{
				KGUI.EnableNodes("not_enough_money.icon_gems", enable: false);
				KGUI.SetControlText("not_enough_money.txt_gold", string.Empty + num);
				KGUI.SetControlText("not_enough_money.txt_gems", string.Empty);
			}
		}, () =>
		{
			KGUI.SetButtonCallback("not_enough_money.ibtn_ok", () =>
			{
				SetMenu(Menu.NotEnoughMoney, show: false);
			});
			KGUI.SetButtonCallback("not_enough_money.ibtn_buy", () =>
			{
				SetMenu(Menu.NotEnoughMoney, show: false);
				SwitchMenu(Menu.Bank);
			});
		});
		RegisterMenu(Menu.Bonus, "bonus", (object data, object dataEx) =>
		{
			int num = ((int[])data)[0];
			int num2 = ((int[])data)[1];
			int num3 = ((int[])data)[2];
			for (int num4 = 5; num4 > num; num4--)
			{
				KGUI.EnableNodes("bonus.stages.stage" + num4 + "_on", enable: false);
			}
			KGUI.SetControlText("bonus.txt_days", num + "/5 " + Localize.GetText("BONUS_DAYS"));
			KGUI.EnableNodes("bonus.stages.stage4_q", enable: false);
			if (num2 > 0)
			{
				KGUI.SetControlText("bonus.txt_count", string.Empty + num2);
				KGUI.EnableNodes("bonus.gems", enable: false);
			}
			if (num3 > 0)
			{
				KGUI.SetControlText("bonus.txt_count", string.Empty + num3);
				KGUI.EnableNodes("bonus.gold", enable: false);
			}
			SoundManager.Instance.Play(SoundManager.Sound.EverydayBonus);
		}, () =>
		{
			KGUI.SetButtonCallback("bonus.ibtn_ok", () =>
			{
				HideMenu();
				ShowLoading("LOADING_PROFILE", string.Empty);
				((MonoBehaviour)this).StartCoroutine(App.Instance.LoadProfile2(from_loading: true));
			});
		});
		RegisterMenu(Menu.BonusGems, "bonus_gems", (object data, object dataEx) =>
		{
			_BonusData = (int[])data;
			SoundManager.Instance.Play(SoundManager.Sound.EverydayBonus);
		}, () =>
		{
			KGUI.SetButtonCallback("bonus_gems.ibtn_ok", () =>
			{
				SwitchMenu(Menu.Bonus, _BonusData);
			});
		});
		RegisterMenu(Menu.ServerSelect, "server_select", (object data, object dataEx) =>
		{
			int num = App.Instance.PlatformShardCount[(int)App.Instance.CurPlatform];
			int bestServerIndex = -1;
			int bestServerLoad = 0;
			KGUI.ResizeGrid("server_select.grid_servers_list", num, (GameObject slot, int index) =>
			{
				int num3 = (int)(100f * (float)App.Instance.ShardPopulation[index] / (float)App.Instance.MaxShardPopulation);
				if (num3 >= 100)
				{
					slot.GetComponentInChildren<UILabel>().text = Localize.GetText("SERVER_SELECT_SERVER_NAME") + " " + (index + 1) + " " + Localize.GetText("SERVER_SELECT_SERVER_FULL");
				}
				else
				{
					slot.GetComponentInChildren<UILabel>().text = Localize.GetText("SERVER_SELECT_SERVER_NAME") + " " + (index + 1);
				}
				KGUI.EnableNodes("server_select." + index + ".utilization_1", num3 >= 25);
				KGUI.EnableNodes("server_select." + index + ".utilization_2", num3 >= 50);
				KGUI.EnableNodes("server_select." + index + ".utilization_3", num3 >= 75);
				KGUI.EnableNodes("server_select." + index + ".utilization_4", num3 >= 100);
				if (bestServerIndex == -1 || num3 < bestServerLoad)
				{
					bestServerIndex = index;
					bestServerLoad = num3;
				}
			});
			if (bestServerLoad < 100 && num > 1)
			{
				_RecommendedShard = bestServerIndex;
				GameObject gameObject = ((Component)KGUI.FindNode("server_select.recommended_server")).gameObject;
				int num2 = (int)(100f * (float)App.Instance.ShardPopulation[bestServerIndex] / (float)App.Instance.MaxShardPopulation);
				gameObject.GetComponentInChildren<UILabel>().text = "Рекомендуемый мир " + (bestServerIndex + 1);
				KGUI.EnableNodes("server_select.recommended_server.utilization_1", num2 >= 25);
				KGUI.EnableNodes("server_select.recommended_server.utilization_2", num2 >= 50);
				KGUI.EnableNodes("server_select.recommended_server.utilization_3", num2 >= 75);
				KGUI.EnableNodes("server_select.recommended_server.utilization_4", num2 >= 100);
			}
			else
			{
				KGUI.EnableNodes("server_select.recommended_server", enable: false);
			}
		}, () =>
		{
			KGUI.SetButtonCallback(new string[2] { "server_select.ibtn_some_server", "server_select.ibtn_recommended_server" }, () =>
			{
				int num = ((!(((Object)KGUI.CallbackSender).name == "ibtn_some_server")) ? _RecommendedShard : KGUI.SlotIndex);
				int num2 = (int)(100f * (float)App.Instance.ShardPopulation[num] / (float)App.Instance.MaxShardPopulation);
				if (num2 < 100)
				{
					HideMenu();
					ShowLoading("LOADING_SERVER", string.Empty);
					App.Instance.CurrentShard = num + 1;
					PhotonNetwork.autoJoinLobby = true;
					PhotonNetwork.ConnectUsingSettings(App.Instance.CurPlatform.ToString() + App.Instance.GameVersion + "Qshard_" + App.Instance.CurrentShard);
				}
			});
		});
		RegisterMenu(Menu.Bank, "main_menu_bank", (object data, object dataEx) =>
		{
			KGUI.SetControlText("main_menu.txt_title", Localize.GetText("BANK_TITLE"));
			bool flag = DateTime.Now < _BonusGemsEndTime;
			for (int i = 1; i <= 5; i++)
			{
				KGUI.EnableNodes("gems" + i + ".action", flag);
				KGUI.EnableNodes("gold" + i + ".action", enable: false);
				KGUI.EnableNodes("gems" + i + ".txt_action", flag);
				KGUI.EnableNodes("gold" + i + ".txt_action", enable: false);
				UISprite component = ((Component)KGUI.FindNode("gems" + i + ".icon_vk")).GetComponent<UISprite>();
				if (App.Instance.CurPlatform == App.Platform.MOYMIR)
				{
					component.spriteName = "icon_mm";
				}
				else if (App.Instance.CurPlatform == App.Platform.ONDOKLASNIKI)
				{
					component.spriteName = "icon_ok";
				}
				else if (App.Instance.CurPlatform == App.Platform.FACEBOOK)
				{
					component.spriteName = "icon_fb";
				}
				else if (App.Instance.CurPlatform == App.Platform.VKONTATE)
				{
					component.spriteName = "gems1_icon_vk";
				}
				else
				{
					component.spriteName = "icon_ws";
				}
				component.MakePixelPerfect();
				int num = (i - 1) * 3;
				int num2 = ((!flag) ? 1 : 2);
				KGUI.SetControlText("gems" + i + ".txt_cost", "за " + (int)Store.Gems[(int)App.Instance.CurPlatform][num]);
				KGUI.SetControlText("gems" + i + ".txt_count", string.Empty + (int)Store.Gems[(int)App.Instance.CurPlatform][num + 1] * num2 + (((int)Store.Gems[(int)App.Instance.CurPlatform][num + 2] == 0) ? string.Empty : (" + " + (int)Store.Gems[(int)App.Instance.CurPlatform][num + 2] * num2)));
				int num3 = (i - 1) * 3;
				KGUI.SetControlText("gold" + i + ".txt_cost", "за " + (int)Store.Gold[num3]);
				KGUI.SetControlText("gold" + i + ".txt_count", string.Empty + (int)Store.Gold[num3 + 1] + (((int)Store.Gold[num3 + 2] == 0) ? string.Empty : (" + " + (int)Store.Gold[num3 + 2])));
			}
		}, () =>
		{
			for (int i = 1; i <= 5; i++)
			{
				KGUI.SetButtonCallback("main_menu_bank.gems" + i + ".ibtn_buy", () =>
				{
					int num = int.Parse(((Object)KGUI.CallbackSender.transform.parent.parent).name.Substring(4)) - 1;
					int num2 = (int)Store.Gems[(int)App.Instance.CurPlatform][num * 3 + 1] + (int)Store.Gems[(int)App.Instance.CurPlatform][num * 3 + 2];
					int num3 = Store.Gems[(int)App.Instance.CurPlatform][num * 3];
					if (App.Instance.CurPlatform == App.Platform.WEBSITE)
					{
						WebCallPayStation(num2);
					}
					else if (App.Instance.CurPlatform == App.Platform.VKONTATE)
					{
						Application.ExternalCall("ByCryss" + num2, new object[0]);
					}
					else if (App.Instance.CurPlatform == App.Platform.ONDOKLASNIKI)
					{
						Application.ExternalCall("PayCrys", new object[1] { num3 });
					}
					else if (App.Instance.CurPlatform == App.Platform.MOYMIR)
					{
						Application.ExternalCall("Pay", new object[1] { num3 });
					}
					else if (App.Instance.CurPlatform == App.Platform.FACEBOOK)
					{
						FacebookPaymentWindow(num3);
					}
				});
				KGUI.SetButtonCallback("main_menu_bank.gold" + i + ".ibtn_buy", () =>
				{
					int num = int.Parse(((Object)KGUI.CallbackSender.transform.parent.parent).name.Substring(4)) - 1;
					((MonoBehaviour)this).StartCoroutine(SellCrystall(Store.Gold[num * 3]));
				});
			}
		});
		RegisterMenu(Menu.TabMenu, "tab_menu", (object data, object dataEx) =>
		{
			RefreshTabMenu();
		}, () =>
		{
			KGUI.SetCheckboxCallback("tab_menu.ckb_new_players_building", () =>
			{
				ProfileINI.newgamersislook = KGUI.GetControlCheckbox("tab_menu.ckb_new_players_building");
			});
			KGUI.SetCheckboxCallback("tab_menu.ckb_builder", () =>
			{
				PlayerNode playerNode = WorldGameObject.Instance.PlayerList[KGUI.SlotIndex];
				if (!Level.Instance.IsBuilder(playerNode.Name))
				{
					Level.Instance.AddBuilder(playerNode.Name);
				}
				else
				{
					Level.Instance.RemoveBuilder(playerNode.Name);
				}
			});
			KGUI.SetCheckboxCallback("tab_menu.ckb_moderator", () =>
			{
				PlayerNode playerNode = WorldGameObject.Instance.PlayerList[KGUI.SlotIndex];
				if (!Level.Instance.IsModerator(playerNode.Name))
				{
					Level.Instance.AddModerator(playerNode.Name);
				}
				else
				{
					Level.Instance.RemoveModerator(playerNode.Name);
				}
			});
			KGUI.SetButtonCallback("tab_menu.ibtn_ban", () =>
			{
				PlayerNode playerNode = WorldGameObject.Instance.PlayerList[KGUI.SlotIndex];
				if ((Level.Instance.IsAdministrator() || Level.Instance.IsModerator()) && !Level.Instance.IsBanned(playerNode.NetPlayer.name))
				{
					WorldGameObject.Instance.BanPlayer(playerNode);
				}
			});
			KGUI.SetButtonCallback("tab_menu.ibtn_unban", () =>
			{
				string[] banned = Level.Instance.Banned;
				string playerName = banned[KGUI.SlotIndex - WorldGameObject.Instance.PlayerList.Count];
				if ((Level.Instance.IsAdministrator() || Level.Instance.IsModerator()) && Level.Instance.IsBanned(playerName))
				{
					WorldGameObject.Instance.UnbanPlayer(playerName);
				}
			});
			KGUI.SetButtonCallback("tab_menu.top_map_info.score.ibtn_vote_yes", () =>
			{
				KGUI.EnableNodes("tab_menu.top_map_info.score.background_large", enable: false);
				KGUI.EnableNodes("tab_menu.top_map_info.score.background_small", enable: true);
				KGUI.EnableNodes("tab_menu.top_map_info.score.button_vote_yes", enable: false);
				int num = int.Parse(KGUI.GetControlText("tab_menu.top_map_info.txt_score"));
				KGUI.SetControlText("tab_menu.top_map_info.txt_score", string.Empty + (num + 1));
				_SelectedMapNotVoted = false;
				((MonoBehaviour)this).StartCoroutine(VoteYes(_SelectedMapID));
			});
		});
		RegisterMenu(Menu.Book, "book", (object data, object dataEx) =>
		{
			Book.Current.RefreshControls();
		}, () =>
		{
			KGUI.SetButtonCallback("book.ibtn_next", () =>
			{
				Book.Current.NextButton();
			});
			KGUI.SetButtonCallback("book.ibtn_prev", () =>
			{
				Book.Current.PrevButton();
			});
			KGUI.SetButtonCallback("book.ibtn_close", () =>
			{
				Book.Current.CloseButton();
			});
		});
		RegisterMenu(Menu.GameMenu, "game_menu", (object data, object dataEx) =>
		{
			if (!GameType.BattleMode() || TeamBattle.Instance is ZombieVirus)
			{
				KGUI.EnableNodes("game_menu.choose_weapon", enable: false);
			}
			if (GameType.BattleMode())
			{
				KGUI.EnableNodes("game_menu.respawn", enable: false);
			}
			if (App.Instance.Settings.isWatch || GameType.BuildDisabled())
			{
				KGUI.EnableNodes("game_menu.inventory", enable: false);
			}
			if (!Level.Instance.IsAdministrator() || App.Instance.Settings.isWatch || !GameType.CanSaveMap())
			{
				KGUI.EnableNodes("game_menu.save", enable: false);
			}
			if (!App.Instance.Settings.isWatch)
			{
				KGUI.EnableNodes("game_menu.estimate", enable: false);
			}
			((Component)KGUI.FindNode("game_menu.buttons")).GetComponent<UITable>().Reposition();
		}, () =>
		{
			KGUI.SetButtonCallback("game_menu.ibtn_resume", () =>
			{
				HideMenu();
			});
			KGUI.SetButtonCallback("game_menu.ibtn_choose_weapon", () =>
			{
				SwitchMenu(Menu.SelectWeapon, true);
			});
			KGUI.SetButtonCallback("game_menu.ibtn_respawn", () =>
			{
				WorldGameObject.Instance.Respawn();
				HideMenu();
			});
			KGUI.SetButtonCallback("game_menu.ibtn_emotions", () =>
			{
				SwitchMenu(Menu.Emotions);
			});
			KGUI.SetButtonCallback("game_menu.ibtn_inventory", () =>
			{
				WorldGameObject.Instance.ToggleInventory();
			});
			KGUI.SetButtonCallback("game_menu.ibtn_save", () =>
			{
				SwitchMenu(Menu.MyMaps);
			});
			KGUI.SetButtonCallback("game_menu.ibtn_estimate", () =>
			{
			});
			KGUI.SetButtonCallback("game_menu.ibtn_get_game_url", () =>
			{
				string text = null;
				string text2 = App.Instance.CurrentShard.ToString("D2") + PhotonNetwork.room.name;
				if (App.Instance.CurPlatform == App.Platform.VKONTATE)
				{
					text = "http://vk.com/app2721929#game_id_" + text2;
				}
				else if (App.Instance.CurPlatform == App.Platform.ONDOKLASNIKI)
				{
					text = "http://www.odnoklassniki.ru/game/kopatel-online?game_id_" + text2;
				}
				else if (App.Instance.CurPlatform == App.Platform.MOYMIR)
				{
					text = "http://my.mail.ru/apps/659403?#game_id_" + text2;
				}
				else if (App.Instance.CurPlatform == App.Platform.WEBSITE)
				{
					text = "http://www.kopatel-online.ru/test_server.html#game_id_" + text2;
				}
				if (text != null)
				{
					NGUITools.clipboard = text;
					ShowHint(Localize.GetText("GAME_URL_URL_DONE") + "\n" + text);
				}
				else
				{
					ShowHint("GAME_URL_URL_UNSUPPORTED");
				}
			});
			KGUI.SetButtonCallback("game_menu.ibtn_help", () =>
			{
				SwitchMenu(Menu.Tutorial);
			});
			KGUI.SetButtonCallback("game_menu.ibtn_exit", () =>
			{
				if (Level.Instance.IsAdministrator() && PhotonNetwork.room.playerCount > 1 && !App.Instance.Settings.isWatch)
				{
					ShowAskMenu("EXIT_TEXT", "EXIT_DESCRIPTION", new string[3] { "EXIT_SERVER_OFF", "EXIT_STILL_SERVER", "EXIT_RESUME" }, new Action[3]
					{
						() =>
						{
							WorldGameObject.Instance.CloseServerAndExit();
						},
						() =>
						{
							WorldGameObject.Instance.ExitGame();
						},
						() =>
						{
						}
					}, showCloseButton: true);
				}
				else
				{
					WorldGameObject.Instance.ExitGame();
				}
			});
			KGUI.SetButtonCallback("game_menu.ibtn_close", () =>
			{
				HideMenu();
			});
		});
		RegisterMenu(Menu.AskMenu, "ask_menu", (object data, object dataEx) =>
		{
			_AskMenuActions = (Action[])dataEx;
			string[] strData = (string[])data;
			KGUI.SetControlText("ask_menu.txt_title", Localize.GetText(strData[0]));
			KGUI.SetControlText("ask_menu.txt_info", Localize.GetText(strData[1]));
			KGUI.ResizeGrid("ask_menu.buttons", _AskMenuActions.Length, (GameObject slot, int index) =>
			{
				slot.GetComponentInChildren<UILabel>().text = Localize.GetText(strData[index + 2]);
			});
			if (strData[strData.Length - 1] != "X")
			{
				KGUI.EnableNodes("ask_menu.button_close", enable: false);
			}
		}, () =>
		{
			KGUI.SetButtonCallback("ask_menu.ibtn_select", () =>
			{
				SetMenu(Menu.AskMenu, show: false);
				_AskMenuActions[KGUI.SlotIndex]();
			});
			KGUI.SetButtonCallback("ask_menu.ibtn_close", () =>
			{
				SetMenu(Menu.AskMenu, show: false);
			});
		});
		RegisterMenu(Menu.InputText, "input_text", (object data, object dataEx) =>
		{
			_InputTextAction = (Action<string>)dataEx;
			string[] array = (string[])data;
			KGUI.SetControlText("input_text.txt_title", array[0]);
			KGUI.SetControlText("input_text.txt_info", array[1]);
			((Component)KGUI.FindNode("input_text.inp_input")).GetComponent<UIInput>().selected = true;
		}, () =>
		{
			KGUI.SetButtonCallback("input_text.ibtn_ok", () =>
			{
				string controlText = KGUI.GetControlText("input_text.inp_input");
				if (controlText.Length != 0)
				{
					SetMenu(Menu.InputText, show: false);
					_InputTextAction(controlText);
				}
			});
			KGUI.SetButtonCallback("input_text.ibtn_close", () =>
			{
				SetMenu(Menu.InputText, show: false);
			});
		});
		RegisterMenu(Menu.Emotions, "emotions", (object data, object dataEx) =>
		{
			if (ProfileINI.GetPurchaseValue(StorePurchase.EMOTIONS) != 0 && ProfileINI.GetPurchaseValue(StorePurchase.MEM_SMILES) != 0)
			{
				KGUI.EnableNodes("emotions.button_more_emotions", enable: false);
			}
			SpeechBubbles.Emotion[] activeEmotions = SpeechBubbles.GetActiveEmotions();
			KGUI.ResizeGrid("emotions.grid_emotions", activeEmotions.Length, (GameObject slot, int index) =>
			{
				((Component)slot.transform.Find("icon")).GetComponent<UISprite>().spriteName = ((Object)activeEmotions[index].Picture).name;
			}, "emotions");
		}, () =>
		{
			KGUI.SetButtonCallback("emotions.ibtn_select", () =>
			{
				SpeechBubbles.Emotion emotion = SpeechBubbles.GetActiveEmotions()[KGUI.SlotIndex];
				Chat.SendEmotion(((Object)emotion.Picture).name, forAll: true);
				HideMenu();
			});
			KGUI.SetButtonCallback("emotions.ibtn_more_emotions", () =>
			{
				SwitchMenu(Menu.Shop, Store.TabType.Tools);
				if (ProfileINI.GetPurchaseValue(StorePurchase.EMOTIONS) == 0)
				{
					ShowPurchaseItem(StorePurchase.EMOTIONS);
				}
				else
				{
					ShowPurchaseItem(StorePurchase.MEM_SMILES);
				}
			});
			KGUI.SetButtonCallback("emotions.ibtn_close", () =>
			{
				HideMenu();
			});
		});
		RegisterMenu(Menu.ChangeName, "change_name", (object data, object dataEx) =>
		{
			KGUI.EnableNodes("change_name.button_close", enable: false);
			KGUI.EnableNodes("change_name.txt_name_busy", enable: false);
		}, () =>
		{
			KGUI.SetButtonCallback("change_name.ibtn_ok", () =>
			{
				string controlText = KGUI.GetControlText("change_name.inp_name");
				if (controlText.Length < 3)
				{
					ShowHint("NAME_TOO_SHORT");
				}
				else if (controlText.Length > 12)
				{
					ShowHint("NAME_TOO_LONG");
				}
				else
				{
					((MonoBehaviour)this).StartCoroutine(ChangeNameProcess(controlText));
				}
			});
			KGUI.SetButtonCallback("change_name.ibtn_close", () =>
			{
				HideMenu();
			});
		});
		RegisterMenu(Menu.Tutorial, "tutorial", (object data, object dataEx) =>
		{
			KGUI.EnableNodes("tutorial.tutorial1", enable: false, onlyChild: true);
			KGUI.EnableNodes("tutorial.tutorial1.1", enable: true);
		}, () =>
		{
			KGUI.SetButtonCallback("tutorial.ibtn_next", () =>
			{
				int num = 0;
				if (((Component)KGUI.FindNode("tutorial.tutorial1.1")).gameObject.activeInHierarchy)
				{
					num = 1;
				}
				else if (((Component)KGUI.FindNode("tutorial.tutorial1.2")).gameObject.activeInHierarchy)
				{
					num = 2;
				}
				else if (((Component)KGUI.FindNode("tutorial.tutorial1.3")).gameObject.activeInHierarchy)
				{
					num = 3;
				}
				else if (((Component)KGUI.FindNode("tutorial.tutorial1.4")).gameObject.activeInHierarchy)
				{
					num = 4;
				}
				if (num < 4)
				{
					KGUI.EnableNodes("tutorial.tutorial1." + num, enable: false);
					KGUI.EnableNodes("tutorial.tutorial1." + (num + 1), enable: true);
				}
				else
				{
					HideMenu();
				}
			});
		});
		RegisterMenu(Menu.SelectWeapon, "select_weapon", (object data, object dataEx) =>
		{
			if ((Object)(object)WorldGameObject.Instance.MainPlayer == (Object)null)
			{
				HideMenu();
			}
			else
			{
				PlayerNetwork player = WorldGameObject.Instance.MainPlayer.GetComponent<PlayerNetwork>();
				if ((bool)data)
				{
					if (!WorldGameObject.Instance.MainPlayerDead)
					{
						WorldGameObject.Instance.KillPlayer();
					}
					player.SelectWeapon(0);
					for (int i = 0; i < player.MainWeapon.Length; i++)
					{
						player.MainWeapon[i] = null;
					}
				}
				_SelectWeapons.Clear();
				_SelectWeapons.Add(player.Pistol);
				Gun[] guns = player.Guns;
				foreach (Gun gun in guns)
				{
					if ((Object)(object)gun != (Object)(object)player.Knife && (Object)(object)gun != (Object)(object)player.Grenade && ProfileINI.GetPurchaseValue(gun.storePurchase) != 0 && !_SelectWeapons.Contains(gun))
					{
						_SelectWeapons.Add(gun);
					}
				}
				KGUI.ResizeGrid("select_weapon.grid_weapons", _SelectWeapons.Count, (GameObject slot, int index) =>
				{
					((Component)slot.transform.Find("txt_title")).GetComponent<UILabel>().text = string.Empty;
					KGUI.SetControlSprite(slot.transform.Find("icon"), ((Object)_SelectWeapons[index].weaponIconBig).name, 100f);
					((Component)slot.transform.Find("selected")).gameObject.SetActive(player.MainWeapon.Contains(_SelectWeapons[index]));
				}, "select_weapon");
				RefreshBattleWeapon();
			}
		}, () =>
		{
			KGUI.SetButtonCallback("select_weapon.ibtn_buy", () =>
			{
				SwitchMenu(Menu.Shop, Store.TabType.Weapons);
			});
			KGUI.SetButtonCallback("select_weapon.ibtn_fight", () =>
			{
				HideMenu();
				PlayerNetwork component = WorldGameObject.Instance.MainPlayer.GetComponent<PlayerNetwork>();
				if (WorldGameObject.Instance.MainPlayerDead)
				{
					WorldGameObject.Instance.Respawn();
				}
			});
			KGUI.SetButtonCallback("select_weapon.ibtn_close", () =>
			{
				HideMenu();
			});
			KGUI.SetButtonCallback("select_weapon.ibtn_select", () =>
			{
				PlayerNetwork component = WorldGameObject.Instance.MainPlayer.GetComponent<PlayerNetwork>();
				Gun gun = _SelectWeapons[KGUI.SlotIndex];
				if (!component.MainWeapon.Contains(gun))
				{
					for (int i = 0; i < component.MainWeapon.Length; i++)
					{
						if ((Object)(object)component.MainWeapon[i] == (Object)null)
						{
							bs._WorldGameObject.MainPlayer.audio.PlayOneShot(Igor.Instance.takeSound, ProfileINI.sound_volume * ProfileINI.sound_scale);
							component.MainWeapon[i] = gun;
							KGUI.EnableNodes("select_weapon.grid_weapons." + KGUI.SlotIndex + ".selected", enable: true);
							break;
						}
					}
				}
				else
				{
					component.MainWeapon[component.MainWeapon.ToList().IndexOf(gun)] = null;
					KGUI.EnableNodes("select_weapon.grid_weapons." + KGUI.SlotIndex + ".selected", enable: false);
				}
				RefreshBattleWeapon();
			});
		});
		RegisterMenu(Menu.Profile, string.Empty, (object data, object dataEx) =>
		{
		}, () =>
		{
		});
		RegisterMenu(Menu.Plate, "plate", (object data, object dataEx) =>
		{
			_CurPlate = (Tablichka)data;
			string[] array = (string[])dataEx;
			KGUI.SetControlText("plate.inp_line1", array[0]);
			KGUI.SetControlText("plate.inp_line2", array[1]);
			KGUI.SetControlText("plate.inp_line3", array[2]);
			KGUI.SetControlText("plate.inp_line4", array[3]);
		}, () =>
		{
			KGUI.SetButtonCallback("plate.ibtn_ok", () =>
			{
				_CurPlate.UpdateText(KGUI.GetControlText("plate.inp_line1"), KGUI.GetControlText("plate.inp_line2"), KGUI.GetControlText("plate.inp_line3"), KGUI.GetControlText("plate.inp_line4"));
				HideMenu();
			});
		});
		RegisterMenu(Menu.SetPicture, string.Empty, (object data, object dataEx) =>
		{
		}, () =>
		{
		});
		RegisterMenu(Menu.TeamBattle, "team_battle", (object data, object dataEx) =>
		{
			KGUI.EnableNodes("team_battle.button_back", enable: false);
			if (TeamBattle.Instance is ZombieVirus)
			{
				KGUI.SetControlText("team_battle.button_red_team.txt_title", Localize.GetText("TEAM_BATTLE_FOR_ZOMBIES"));
				KGUI.SetControlText("team_battle.button_blue_team.txt_title", Localize.GetText("TEAM_BATTLE_FOR_SURVIVORS"));
				KGUI.SetControlText("team_battle.page1.txt_team", Localize.GetText("TEAM_BATTLE_ZOMBIES"));
				KGUI.SetControlText("team_battle.page2.txt_team", Localize.GetText("TEAM_BATTLE_SURVIVORS"));
			}
			else
			{
				KGUI.SetControlText("team_battle.button_red_team.txt_title", Localize.GetText("TEAM_BATTLE_FOR_RED"));
				KGUI.SetControlText("team_battle.button_blue_team.txt_title", Localize.GetText("TEAM_BATTLE_FOR_BLUE"));
				KGUI.SetControlText("team_battle.page1.txt_team", Localize.GetText("TEAM_BATTLE_RED"));
				KGUI.SetControlText("team_battle.page2.txt_team", Localize.GetText("TEAM_BATTLE_BLUE"));
			}
			TeamBattle.Instance.RefreshTabMenu();
		}, () =>
		{
			KGUI.SetButtonCallback("team_battle.ibtn_observe", () =>
			{
				TeamBattle.Instance.StartPlay(0);
				HideMenu();
			});
			KGUI.SetButtonCallback("team_battle.ibtn_red_team", () =>
			{
				TeamBattle.Instance.StartPlay(1);
				if (!(TeamBattle.Instance is ZombieVirus))
				{
					SwitchMenu(Menu.SelectWeapon, false);
				}
			});
			KGUI.SetButtonCallback("team_battle.ibtn_blue_team", () =>
			{
				TeamBattle.Instance.StartPlay(2);
				SwitchMenu(Menu.SelectWeapon, false);
			});
			KGUI.SetButtonCallback("team_battle.ibtn_back", () =>
			{
				HideMenu();
			});
			KGUI.SetButtonCallback("team_battle.ibtn_close", () =>
			{
				HideMenu();
			});
			KGUI.SetButtonCallback("team_battle.grid_blue_team.ibtn_cheater", () =>
			{
				TeamBattle.Instance.CheckPlayerCheating(2, KGUI.SlotIndex);
			});
			KGUI.SetButtonCallback("team_battle.grid_red_team.ibtn_cheater", () =>
			{
				TeamBattle.Instance.CheckPlayerCheating(1, KGUI.SlotIndex);
			});
			KGUI.SetTooltipCallback("team_battle.grid_blue_team.level_icon", () => Localize.GetText("LEVEL_" + TeamBattle.Instance.GetPlayerLevel(2, KGUI.SlotIndex)));
			KGUI.SetTooltipCallback("team_battle.grid_red_team.level_icon", () => Localize.GetText("LEVEL_" + TeamBattle.Instance.GetPlayerLevel(1, KGUI.SlotIndex)));
		});
		RegisterMenu(Menu.Deathmatch, "deathmatch", (object data, object dataEx) =>
		{
			KGUI.EnableNodes("deathmatch.button_close", enable: false);
			KGUI.EnableNodes("deathmatch.checkbox_chat", enable: false);
			TeamBattle.Instance.RefreshTabMenu();
		}, () =>
		{
			KGUI.SetButtonCallback("deathmatch.ibtn_play", () =>
			{
				TeamBattle.Instance.StartPlay(1);
				SwitchMenu(Menu.SelectWeapon, false);
			});
			KGUI.SetButtonCallback("deathmatch.ibtn_observe", () =>
			{
				TeamBattle.Instance.StartPlay(0);
				HideMenu();
			});
			KGUI.SetButtonCallback("deathmatch.ibtn_close", () =>
			{
				HideMenu();
			});
			KGUI.SetCheckboxCallback("deathmatch.ckb_chat", () =>
			{
			});
			KGUI.SetButtonCallback("deathmatch.grid_players.ibtn_cheater", () =>
			{
				TeamBattle.Instance.CheckPlayerCheating(1, KGUI.SlotIndex);
			});
			KGUI.SetTooltipCallback("deathmatch.grid_players.level_icon", () => Localize.GetText("LEVEL_" + TeamBattle.Instance.GetPlayerLevel(1, KGUI.SlotIndex)));
		});
		RegisterMenu(Menu.BattleResult, "battle_result", (object data, object dataEx) =>
		{
			bool flag = (bool)data;
			int score = (int)dataEx;
			KGUI.EnableNodes("battle_result.button_close", enable: false);
			KGUI.EnableNodes("battle_result.txt_coins", enable: false);
			KGUI.EnableNodes("battle_result.coins_icon", enable: false);
			if (flag)
			{
				KGUI.EnableNodes("battle_result.lose_screen", enable: false);
			}
			else
			{
				KGUI.EnableNodes("battle_result.win_screen", enable: false);
				KGUI.EnableNodes("battle_result.win_title", enable: false);
			}
			if (ProfileINI.GetPurchaseValue(StorePurchase.MORE_EXPERIENCE) != 0)
			{
				KGUI.EnableNodes("battle_result.exp_enabled", enable: false);
				KGUI.EnableNodes("battle_result.exp_premium_disabled", enable: false);
			}
			else
			{
				KGUI.EnableNodes("battle_result.exp_disabled", enable: false);
				KGUI.EnableNodes("battle_result.exp_premium_enabled", enable: false);
			}
			((MonoBehaviour)this).StartCoroutine(BattleResultProcess(score));
		}, () =>
		{
			KGUI.SetButtonCallback("battle_result.ibtn_ok", () =>
			{
				TeamBattle.Instance.RefreshTabMenu(open: true);
			});
			KGUI.SetButtonCallback("battle_result.ibtn_close", () =>
			{
				HideMenu();
			});
		});
		RegisterMenu(Menu.LevelUp, "level_up", (object data, object dataEx) =>
		{
			int level = ProfileINI.level;
			KGUI.SetControlText("level_up.txt_title", Localize.GetText("LEVEL_TITLE") + " " + level);
			KGUI.SetControlText("level_up.txt_level_name", Localize.GetText("LEVEL_" + level));
			KGUI.SetControlSprite(KGUI.FindNode("level_up.level_icon"), "level_" + level, 0f);
			WorldGameObject.Instance.photonView.RPC("SetPlayerLevel", PhotonTargets.OthersBuffered, PhotonNetwork.player, level);
		}, () =>
		{
			KGUI.SetButtonCallback("level_up.ibtn_ok", () =>
			{
				SetMenu(Menu.LevelUp, show: false);
			});
		});
	}

	public void RegisterMenu(Menu menu, string nodeName, Action<object, object> initializer, Action callbacks)
	{
		if (!_MenuNodeNames.ContainsKey(menu))
		{
			_MenuNodeNames[menu] = new List<string>();
		}
		if (nodeName != null)
		{
			_MenuNodeNames[menu].Add(nodeName);
		}
		if (!_MenuInitializers.ContainsKey(menu))
		{
			_MenuInitializers[menu] = new List<Action<object, object>>();
		}
		_MenuInitializers[menu].Add(initializer);
		callbacks?.Invoke();
	}

	public void RegisterMenu(Menu[] menus, string nodeName, Action<object, object> initializer, Action callbacks)
	{
		foreach (Menu menu in menus)
		{
			RegisterMenu(menu, nodeName, initializer, null);
		}
		callbacks();
	}

	public void SwitchMenu(Menu menu, object data = null, object dataEx = null)
	{
		if (!((Component)KGUI.FindNode("loading")).gameObject.activeSelf && menu != _CurMenu)
		{
			SetMenu(_CurMenu, show: false, data, dataEx);
			_CurMenu = menu;
			SetMenu(_CurMenu, show: true, data, dataEx);
		}
	}

	public void ToggleMenu(Menu menu)
	{
		if (!((Component)KGUI.FindNode("loading")).gameObject.activeSelf)
		{
			if (_CurMenu != menu && _CanToggleMenus.Contains(_CurMenu))
			{
				SwitchMenu(menu);
			}
			else if (_CurMenu == menu)
			{
				HideMenu();
			}
		}
	}

	public void SetMenu(Menu menu, bool show, object data = null, object dataEx = null)
	{
		UITooltip.ShowText(null);
		foreach (string item in _MenuNodeNames[menu])
		{
			((Component)KGUI.FindNode(item)).gameObject.SetActive(!show);
			KGUI.EnableNodes(item, show);
		}
		if (!_NonModalMenus.Contains(menu))
		{
			KGUI.SetModal(_MenuNodeNames[menu], show);
		}
		if (!show)
		{
			return;
		}
		foreach (Action<object, object> item2 in _MenuInitializers[menu])
		{
			item2(data, dataEx);
		}
		if (Application.loadedLevel != 1)
		{
			return;
		}
		if (_CurMenu != Menu.None)
		{
			Screen.lockCursor = false;
			WorldGameObject.Instance.DisableMouseLook();
			if ((Object)(object)WorldGameObject.Instance.MainPlayer != (Object)null)
			{
				WorldGameObject.Instance.MainPlayer.GetComponent<PlayerInput>().SetMovement(move: false);
			}
		}
		else
		{
			Screen.lockCursor = true;
			WorldGameObject.Instance.EnableMouseLook();
			if ((Object)(object)WorldGameObject.Instance.MainPlayer != (Object)null)
			{
				WorldGameObject.Instance.MainPlayer.GetComponent<PlayerInput>().SetMovement(move: true);
			}
		}
	}

	public void HideMenu()
	{
		SwitchMenu(Menu.None);
	}

	public void ShowHint(string text, bool asNewMenu = false)
	{
		if (asNewMenu)
		{
			SwitchMenu(Menu.Hint, text);
		}
		else
		{
			SetMenu(Menu.Hint, show: true, text);
		}
	}

	public void HideHint()
	{
		if (CurMenu == Menu.Hint)
		{
			HideMenu();
		}
		else
		{
			SetMenu(Menu.Hint, show: false);
		}
	}

	public void ShowNotEnoughMoney(bool gems, int count)
	{
		SetMenu(Menu.NotEnoughMoney, show: true, new object[2] { gems, count });
	}

	public void ShowLoading(string text, string rawText = "")
	{
		SetMenu(Menu.Loading, show: true);
		SetLoadingText(text, rawText);
	}

	public void HideLoading()
	{
		SetMenu(Menu.Loading, show: false, string.Empty);
	}

	public void SetLoadingText(string text, string rawText = "")
	{
		KGUI.SetControlText("loading.txt_text", Localize.GetText(text) + rawText);
	}

	public void ShowAskMenu(string title, string text, string[] buttonNames, Action[] buttonCallbacks, bool showCloseButton)
	{
		List<string> list = new List<string>();
		list.Add(title);
		list.Add(text);
		list.AddRange(buttonNames);
		list.Add((!showCloseButton) ? string.Empty : "X");
		SetMenu(Menu.AskMenu, show: true, list.ToArray(), buttonCallbacks);
	}

	public void ShowInputText(string title, string text, Action<string> okCallback)
	{
		List<string> list = new List<string>();
		list.Add(Localize.GetText(title));
		list.Add(Localize.GetText(text));
		SetMenu(Menu.InputText, show: true, list.ToArray(), okCallback);
	}

	private void Awake()
	{
		if ((Object)(object)Instance != (Object)null)
		{
			Instance.SetMenu(Menu.Hud, show: false);
			return;
		}
		Instance = this;
		RegisterMenus();
	}

	public IEnumerator LoadNewsBlocks()
	{
		string news_name = "news.txt";
		if (Localize.Locale == Localize.LocaleType.RU)
		{
			news_name = "news.txt";
		}
		if (Localize.Locale == Localize.LocaleType.EN)
		{
			news_name = "news_en.txt";
		}
		WWW news = new WWW(App.Instance.GetProtocol() + "diggerworld.ru/news/" + news_name);
		yield return news;
		string[] newsBlocksRaw = news.text.Split('|');
		List<string[]> newsBlocks = new List<string[]>();
		for (int i = 0; i < newsBlocksRaw.Length; i++)
		{
			string[] newsBlockParams = newsBlocksRaw[i].Split(';');
			if (newsBlockParams.Length >= 5)
			{
				for (int j = 0; j < newsBlockParams.Length; j++)
				{
					newsBlockParams[j] = newsBlockParams[j].Trim();
				}
				newsBlocks.Add(newsBlockParams);
			}
		}
		string[] newsText = new string[newsBlocks.Count];
		Texture2D[] newsPicture = new Texture2D[newsBlocks.Count];
		string[] newsPictureURL = new string[newsBlocks.Count];
		string[] newsButtonText = new string[newsBlocks.Count];
		string[] newsButtonURL = new string[newsBlocks.Count];
		for (int k = 0; k < newsBlocks.Count; k++)
		{
			string[] newsBlockParams2 = newsBlocks[k];
			newsText[k] = newsBlockParams2[0];
			newsPictureURL[k] = newsBlockParams2[2];
			newsButtonText[k] = newsBlockParams2[3];
			newsButtonURL[k] = newsBlockParams2[4];
			WWW www = new WWW(newsBlockParams2[1]);
			yield return www;
			if (www.error == null)
			{
				newsPicture[k] = www.texture;
			}
		}
		_NewsText = newsText;
		_NewsPicture = newsPicture;
		_NewsPictureURL = newsPictureURL;
		_NewsButtonText = newsButtonText;
		_NewsButtonURL = newsButtonURL;
		_NewsAutoScroll = false;
		KGUI.EnableNodes("main_menu_start.page2_start.txt_news_loading", enable: false);
		if (((Component)KGUI.FindNode("main_menu_start.page2_start")).gameObject.activeInHierarchy)
		{
			RefreshNews();
		}
	}

	private void OnLevelWasLoaded()
	{
		KGUI.EnableNodes("main_menu_background", Application.loadedLevel == 0);
	}

	public void RefreshAcceleration()
	{
		bool flag = ProfileINI.GetPurchaseValue(StorePurchase.SPEED) != 0 && !GameType.BattleMode();
		bool flag2 = _SpeedEnabled && flag && !GameType.BattleMode();
		if ((Object)(object)WorldGameObject.Instance.MainPlayer != (Object)null)
		{
			WorldGameObject.Instance.MainPlayer.GetComponent<PlayerInput>().SetSpeedUp(flag2);
		}
		if (flag)
		{
			KGUI.EnableNodes("hud.bonuses.speed", enable: true);
			KGUI.EnableNodes("hud.bonuses.speed.on", flag2);
			KGUI.EnableNodes("hud.bonuses.speed.off", !flag2);
		}
		else
		{
			KGUI.EnableNodes("hud.bonuses.speed", enable: false);
		}
	}

	public void RefreshFlying()
	{
		bool flag = ProfileINI.GetPurchaseValue(StorePurchase.FLY) != 0 && !GameType.BattleMode();
		Flying = (_FlyEnabled && flag) || GameType.IsObserving();
		if ((Object)(object)WorldGameObject.Instance.MainPlayer != (Object)null)
		{
			WorldGameObject.Instance.MainPlayer.SendMessage("SetLadderBody", (object)Flying);
		}
		if (flag)
		{
			KGUI.EnableNodes("hud.bonuses.fly", enable: true);
			KGUI.EnableNodes("hud.bonuses.fly.on", Flying);
			KGUI.EnableNodes("hud.bonuses.fly.txt_tip1", Flying);
			KGUI.EnableNodes("hud.bonuses.fly.txt_tip2", Flying);
			KGUI.EnableNodes("hud.bonuses.fly.off", !Flying);
		}
		else
		{
			KGUI.EnableNodes("hud.bonuses.fly", enable: false);
		}
	}

	public void RefreshDayNight()
	{
		if (false)
		{
			KGUI.EnableNodes("hud.bonuses.day_night", enable: true);
			TimeOfDay.RefreshBonusIcon();
		}
		else
		{
			KGUI.EnableNodes("hud.bonuses.day_night", enable: false);
		}
	}

	private void UpdateSpecialBonus()
	{
		if (!GameType.BattleMode() && !Chat.IsEnabled() && !MenuActive)
		{
			if (Input.GetKeyDown((KeyCode)49) && ProfileINI.GetPurchaseValue(StorePurchase.SPEED) != 0)
			{
				_SpeedEnabled = !_SpeedEnabled;
				RefreshAcceleration();
			}
			if (Input.GetKeyDown((KeyCode)50) && ProfileINI.GetPurchaseValue(StorePurchase.FLY) != 0)
			{
				_FlyEnabled = !_FlyEnabled;
				RefreshFlying();
			}
		}
	}

	public void SetCrosshairInfo(string title, CrosshairAction crosshairAction)
	{
		KGUI.EnableNodes("hud.crosshair.actions", enable: false, onlyChild: true);
		if ((crosshairAction & CrosshairAction.Take) != 0)
		{
			KGUI.EnableNodes("hud.crosshair.actions.take", enable: true);
		}
		if ((crosshairAction & CrosshairAction.Sit) != 0)
		{
			KGUI.EnableNodes("hud.crosshair.actions.sit", enable: true);
		}
		if ((crosshairAction & CrosshairAction.Open) != 0)
		{
			KGUI.EnableNodes("hud.crosshair.actions.open", enable: true);
		}
		if ((crosshairAction & CrosshairAction.Activate) != 0)
		{
			KGUI.EnableNodes("hud.crosshair.actions.activate", enable: true);
		}
		if ((crosshairAction & CrosshairAction.Write) != 0)
		{
			KGUI.EnableNodes("hud.crosshair.actions.write", enable: true);
		}
		if ((crosshairAction & CrosshairAction.Draw) != 0)
		{
			KGUI.EnableNodes("hud.crosshair.actions.draw", enable: true);
		}
		if ((crosshairAction & CrosshairAction.Wheel) != 0)
		{
			KGUI.EnableNodes("hud.crosshair.actions.wheel", enable: true);
		}
		if ((crosshairAction & CrosshairAction.Put) != 0)
		{
			KGUI.EnableNodes("hud.crosshair.actions.put", enable: true);
		}
		((Component)KGUI.FindNode("hud.crosshair.actions")).GetComponent<UITable>().Reposition();
		if (!string.IsNullOrEmpty(title))
		{
			KGUI.EnableNodes("hud.crosshair.txt_title", enable: true);
			KGUI.SetControlText("hud.crosshair.txt_title", Localize.GetText("PURCHASE_" + title));
		}
		else
		{
			KGUI.EnableNodes("hud.crosshair.txt_title", enable: false);
		}
	}

	public void WebCallPayStation(int count)
	{
		if (count != 0)
		{
			Application.ExternalCall("WebCallPayStation", new object[1] { "https://secure.xsolla.com/paystation/index.php?id_theme=34&project=7373&v1=" + VkontakteAPI.INSTANCE._viewerId + "&email=" + ProfileINI.email + "&out=" + count });
		}
		else
		{
			Application.ExternalCall("WebCallPayStation", new object[1] { "https://secure.xsolla.com/paystation/index.php?id_theme=34&project=7373&v1=" + VkontakteAPI.INSTANCE._viewerId + "&email=" + ProfileINI.email });
		}
	}

	private IEnumerator WebEnterGame(string login, string password)
	{
		HideMenu();
		ShowLoading("LOADING_ENTER_TO_GAME", string.Empty);
		yield return 0;
		login = login.ToLower();
		password = password.ToLower();
		int packetID = App.Instance.Rnd.Next();
		WWWForm enterGameForm = new WWWForm();
		enterGameForm.AddField("email", login);
		enterGameForm.AddField("password", password);
		enterGameForm.AddField("random", packetID);
		WWW enterGame = new WWW(App.Instance.GetCurScriptsUrl() + "enter_game.php", enterGameForm);
		yield return enterGame;
		if (enterGame.error == null && enterGame.text != "LOGIN_FAILED")
		{
			string serverText = enterGame.text.Substring(0, enterGame.text.IndexOf('\n'));
			string serverKey = enterGame.text.Substring(enterGame.text.IndexOf('\n') + 1);
			if (serverKey.Trim() == VkontakteAPI.MD52(serverText + "_" + packetID + "_" + VkontakteAPI.INSTANCE.GetSecret()))
			{
				VkontakteAPI.INSTANCE.SetViewerId(serverText.Split('\t')[0]);
				VkontakteAPI.INSTANCE.SetAuthKey(serverText.Split('\t')[1]);
				ProfileINI.email = login.Trim();
				if (KGUI.GetControlCheckbox("welcome.ckb_remember"))
				{
					ProfileINI.SaveLoginInfo(login, password, remember: true);
				}
				else
				{
					ProfileINI.ClearLoginInfo();
				}
				SetLoadingText("LOADING_PROFILE", string.Empty);
				((MonoBehaviour)this).StartCoroutine(App.Instance.LoadProfile());
			}
		}
		else
		{
			HideLoading();
			SwitchMenu(Menu.Welcome);
			if (enterGame.error == null)
			{
				ShowHint("ENTER_FAIL");
			}
			else
			{
				ShowHint(Localize.GetText("ENTER_ERROR") + "\n" + enterGame.error);
			}
		}
	}

	private IEnumerator WebPasswordRecover(string login)
	{
		ShowLoading("LOADING_PASSWORD_RECOVERING", string.Empty);
		login = login.ToLower();
		WWWForm passwordRecoverForm = new WWWForm();
		passwordRecoverForm.AddField("email", login);
		WWW passwordRecover = new WWW(App.Instance.GetCurScriptsUrl() + "password_recover.php", passwordRecoverForm);
		yield return passwordRecover;
		HideLoading();
		if (passwordRecover.error == null)
		{
			if (passwordRecover.text.Trim() == "SEND")
			{
				ShowHint("RECOVER_SEND");
			}
			else if (passwordRecover.text.Trim() == "WRONG_EMAIL")
			{
				ShowHint("RECOVER_WRONG_EMAIL");
			}
			else if (passwordRecover.text.Trim() == "SEND_FAIL")
			{
				ShowHint("RECOVER_SEND_FAIL");
			}
			else if (passwordRecover.text.Trim() == "EMAIL_NOT_FOUND")
			{
				ShowHint("RECOVER_EMAIL_NOT_FOUND");
			}
			else
			{
				ShowHint(Localize.GetText("RECOVER_ERROR") + "\n" + passwordRecover.text);
			}
		}
		else
		{
			ShowHint(Localize.GetText("RECOVER_ERROR") + "\n" + passwordRecover.error);
		}
	}

	private IEnumerator WebRegister(string login, string password)
	{
		ShowLoading("LOADING_PLAYER_REGISTRATION", string.Empty);
		yield return 0;
		login = login.ToLower();
		password = password.ToLower();
		string uniqID = SystemInfo.deviceUniqueIdentifier;
		int packetID = App.Instance.Rnd.Next();
		WWWForm registerForm = new WWWForm();
		registerForm.AddField("email", login);
		registerForm.AddField("password", password);
		registerForm.AddField("uniq_id", uniqID);
		registerForm.AddField("check_hash", VkontakteAPI.MD52(uniqID + "_" + packetID + "_" + VkontakteAPI.INSTANCE.GetSecret()));
		registerForm.AddField("random", packetID);
		WWW register = new WWW(App.Instance.GetCurScriptsUrl() + "register.php", registerForm);
		yield return register;
		HideLoading();
		yield return 0;
		string serverText = register.text.Substring(0, register.text.IndexOf('\n'));
		string serverKey = register.text.Substring(register.text.IndexOf('\n') + 1);
		if (serverKey.Trim() == VkontakteAPI.MD52(serverText + "_" + packetID + "_" + VkontakteAPI.INSTANCE.GetSecret()))
		{
			switch (serverText)
			{
			case "REGISTERED":
				KGUI.EnableNodes("welcome.play_buttons", enable: true);
				KGUI.EnableNodes("welcome.registration", enable: false);
				KGUI.EnableNodes("welcome.registration_buttons", enable: false);
				ShowHint("ACTIVATION_CHECK");
				break;
			case "WRONG_EMAIL":
				ShowHint("REGISTER_WRONG_EMAIL");
				break;
			case "SEND_FAIL":
				ShowHint("REGISTER_SEND_FAIL");
				break;
			case "EMAIL_FAILED":
				ShowHint("REGISTER_EMAIL_FAILED");
				break;
			case "REG_FAILED":
				ShowHint("REGISTER_REG_FAILED");
				break;
			}
		}
	}

	private void ToggleFullScreen()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (Screen.fullScreen)
		{
			Screen.SetResolution(800, 600, false);
		}
		else
		{
			Resolution currentResolution = Screen.currentResolution;
			int width = currentResolution.width;
			Resolution currentResolution2 = Screen.currentResolution;
			Screen.SetResolution(width, currentResolution2.height, true);
		}
		RefreshScreenSize();
	}

	public void RefreshScreenSize()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		Resolution currentResolution = Screen.currentResolution;
		WorldGameObject.Instance.HitEffect.GetComponent<GUITexture>().pixelInset = new Rect((float)(-currentResolution.width / 2), (float)(-currentResolution.height / 2), (float)currentResolution.width, (float)currentResolution.height);
		WorldGameObject.Instance.ZombieEffect.GetComponent<GUITexture>().pixelInset = new Rect((float)(-currentResolution.width / 2), (float)(-currentResolution.height / 2), (float)currentResolution.width, (float)currentResolution.height);
	}

	private IEnumerator MoveToGameScene()
	{
		SetLoadingText("LOADING_LEVEL", string.Empty);
		while (PhotonNetwork.room == null)
		{
			yield return 0;
		}
		PhotonNetwork.isMessageQueueRunning = false;
		Application.LoadLevel("Game");
	}

	private void InitializeGameINI()
	{
		GameINI gameINI = (App.Instance.Settings = new GameINI());
		gameINI.playerID = string.Empty;
		gameINI.slotID = 0;
		gameINI.publicMapID = _SelectedMapID;
		gameINI.gameType = _MapGameTypes[_MapGameType];
		gameINI.gifts = _MapGameTypes[_MapGameType] != GameINI.GameType.BUILDING && _MapBonuses;
		gameINI.destroyable = _MapGameTypes[_MapGameType] == GameINI.GameType.BUILDING || _MapDestroyable;
		gameINI.mapSize = GameINI.MapSize.SMALL;
		gameINI.mapTime = _MapTimes[_MapTime];
		gameINI.mapPopulation = 12;
		gameINI.isOnline = true;
		gameINI.isServer = false;
		gameINI.isServerAdministrator = false;
		gameINI.isWatch = false;
		gameINI.loadingSavedMap = true;
		gameINI.Password = string.Empty;
		PhotonNetwork.playerName = ProfileINI.nickname;
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		bool isSSL = App.Instance.IsSSL;
		App.Instance.IsSSL = false;
		hashtable.Add("server_url", App.Instance.GetCurScriptsUrl());
		App.Instance.IsSSL = isSSL;
		hashtable.Add("player_id", VkontakteAPI.INSTANCE._viewerId);
		PhotonNetwork.SetPlayerCustomProperties(hashtable);
	}

	public void JoinGame(RoomInfo room)
	{
		Debug.Log((object)"JoinGame");
		InitializeGameINI();
		App.Instance.Settings.playerID = (string)room.customProperties["player_id"];
		App.Instance.Settings.slotID = (int)room.customProperties["slot_id"];
		App.Instance.Settings.isWatch = (bool)room.customProperties["is_watch"];
		App.Instance.Settings.server_name = (string)room.customProperties["map_name"];
		App.Instance.Settings.server_about = (string)room.customProperties["map_about"];
		PhotonNetwork.JoinRoom(room.name);
		HideMenu();
		ShowLoading("LOADING_TRY_CONNECT", string.Empty);
	}

	private void OnPhotonJoinRoomFailed()
	{
		Debug.Log((object)"OnPhotonJoinRoomFailed");
		if (Application.loadedLevel == 0)
		{
			Instance.HideLoading();
			SwitchMenu(Menu.MyMaps);
		}
		else
		{
			Instance.ShowLoading("LOADING_LEAVING", string.Empty);
			Application.LoadLevel("Menu");
		}
	}

	private void JoinOrCreateGame(string playerID, int slotID, bool isWatch, string serverName, string serverDesc, string password)
	{
		InitializeGameINI();
		App.Instance.Settings.server_name = serverName;
		App.Instance.Settings.server_about = serverDesc;
		App.Instance.Settings.Password = password;
		App.Instance.Settings.playerID = playerID;
		App.Instance.Settings.slotID = slotID;
		App.Instance.Settings.isWatch = isWatch;
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable.Add("player_id", playerID);
		hashtable.Add("slot_id", slotID);
		PhotonNetwork.JoinRandomRoom(hashtable, 0);
		HideMenu();
		ShowLoading("LOADING_TRY_CONNECT", string.Empty);
	}

	private void OnPhotonRandomJoinFailed()
	{
		Debug.Log((object)"OnPhotonRandomJoinFailed");
		App.Instance.Settings.isServer = true;
		App.Instance.Settings.isServerAdministrator = App.Instance.Settings.slotID != -1;
		Application.LoadLevel("Game");
		ShowLoading("LOADING_LEVEL", string.Empty);
	}

	private void OnJoinedRoom()
	{
		Debug.Log((object)"OnJoinedRoom");
		if ((string)PhotonNetwork.room.customProperties["player_id"] == VkontakteAPI.INSTANCE._viewerId)
		{
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable.Add("map_name", App.Instance.Settings.server_name);
			hashtable.Add("map_about", App.Instance.Settings.server_about);
			hashtable.Add("password", App.Instance.Settings.Password);
			PhotonNetwork.room.SetCustomProperties(hashtable);
		}
		if (!App.Instance.Settings.isServer)
		{
			ProfileINI.server_about = (string)PhotonNetwork.room.customProperties["map_about"];
			((MonoBehaviour)this).StartCoroutine(MoveToGameScene());
		}
	}

	public void CleanScreenMode(bool mode)
	{
		if (GameType.IsObserving())
		{
			return;
		}
		CleanScreen = mode;
		if (CleanScreen)
		{
			if (!CameraController.Instance.IsThirdPerson)
			{
				WorldGameObject.Instance.MainPlayer.GetComponent<SkinManager>().DisableAllHands();
			}
			for (int i = 0; i < WorldGameObject.Instance.PlayerList.Count; i++)
			{
				GameObject avatar = WorldGameObject.Instance.PlayerList[i].Avatar;
				if ((Object)(object)avatar != (Object)null)
				{
					avatar.GetComponent<Nickname>().Hide();
				}
			}
		}
		else
		{
			if (!CameraController.Instance.IsThirdPerson)
			{
				WorldGameObject.Instance.MainPlayer.GetComponent<SkinManager>().SetHand(ProfileINI.GetActualSkin());
			}
			for (int j = 0; j < WorldGameObject.Instance.PlayerList.Count; j++)
			{
				GameObject avatar2 = WorldGameObject.Instance.PlayerList[j].Avatar;
				if ((Object)(object)avatar2 != (Object)null)
				{
					avatar2.GetComponent<Nickname>().Show();
				}
			}
		}
		((Component)KGUI.FindNode("hud")).gameObject.SetActive(!CleanScreen);
	}

	private void Update()
	{
		if (Input.GetKeyDown((KeyCode)293))
		{
			ToggleFullScreen();
		}
		if ((Object)(object)WorldGameObject.Instance != (Object)null && Input.GetKeyDown((KeyCode)114) && !Chat.IsEnabled() && !Instance.MenuActive && !GameType.BattleMode())
		{
			WorldGameObject.Instance.Respawn();
		}
		if (Input.GetKeyDown((KeyCode)290))
		{
			CinematicCamera = !CinematicCamera;
		}
		if (Input.GetKeyDown((KeyCode)292) && Application.loadedLevelName == "Game")
		{
			CleanScreen = !CleanScreen;
			CleanScreenMode(CleanScreen);
		}
		if (Input.GetKeyDown((KeyCode)289))
		{
			GameConsole.show = !GameConsole.show;
		}
		if (Application.loadedLevel == 1)
		{
			UpdateSpecialBonus();
		}
		if (_BonusGemsSeconds != -1 && (CurMenu == Menu.Start || CurMenu == Menu.MyMaps || CurMenu == Menu.Settings || CurMenu == Menu.Shop || CurMenu == Menu.Servers || CurMenu == Menu.TopMaps || CurMenu == Menu.Bank))
		{
			TimeSpan timeSpan = _BonusGemsEndTime - DateTime.Now;
			if (timeSpan.TotalSeconds > 0.0 && _BonusGemsSeconds != (int)timeSpan.TotalSeconds)
			{
				KGUI.SetControlText("main_menu.bonus_gems.txt_time", timeSpan.Hours + ((!_BonusGemsShowSeparator) ? " " : ":") + timeSpan.Minutes);
				_BonusGemsSeconds = (int)timeSpan.TotalSeconds;
				_BonusGemsShowSeparator = !_BonusGemsShowSeparator;
			}
			else if (timeSpan.TotalSeconds <= 0.0)
			{
				_BonusGemsSeconds = -1;
				HideMenu();
				SwitchMenu(Menu.Bank);
			}
		}
	}

	private void RefreshServersList()
	{
		((MonoBehaviour)this).CancelInvoke("RefreshServersList");
		if (!((Component)KGUI.FindNode("main_menu_servers")).gameObject.activeInHierarchy)
		{
			return;
		}
		((MonoBehaviour)this).Invoke("RefreshServersList", 1f);
		_ServerRooms.Clear();
		string controlText = KGUI.GetControlText("main_menu_servers.search.inp_box");
		bool flag = _ServersShowCTF || _ServersShowTeamBattle || _ServersShowDeathmatch || _ServersShowBuilding || _ServersShowZombieVirus;
		RoomInfo[] roomList = PhotonNetwork.GetRoomList();
		foreach (RoomInfo roomInfo in roomList)
		{
			if ((roomInfo.customProperties.ContainsKey("is_watch") && (bool)roomInfo.customProperties["is_watch"]) || (controlText.Length > 0 && ((string)roomInfo.customProperties["map_name"]).IndexOf(controlText, StringComparison.CurrentCultureIgnoreCase) == -1))
			{
				continue;
			}
			if (flag)
			{
				GameINI.GameType gameType = (GameINI.GameType)(int)roomInfo.customProperties["game_type"];
				if ((!_ServersShowCTF && gameType == GameINI.GameType.CTF) || (!_ServersShowTeamBattle && gameType == GameINI.GameType.TEAM_BATTLE) || (!_ServersShowDeathmatch && gameType == GameINI.GameType.DEATHMATCH) || (!_ServersShowBuilding && gameType == GameINI.GameType.BUILDING) || (!_ServersShowZombieVirus && gameType == GameINI.GameType.ZOMBIE_VIRUS))
				{
					continue;
				}
			}
			_ServerRooms.Add(roomInfo);
			if (_ServerRooms.Count >= 100)
			{
				break;
			}
		}
		KGUI.EnableNodes("main_menu_servers.sorting.gun.active", _ServersShowTeamBattle);
		KGUI.EnableNodes("main_menu_servers.sorting.team_red.active", _ServersShowCTF);
		KGUI.EnableNodes("main_menu_servers.sorting.skull.active", _ServersShowDeathmatch);
		KGUI.EnableNodes("main_menu_servers.sorting.mining.active", _ServersShowBuilding);
		KGUI.EnableNodes("main_menu_servers.sorting.zombie.active", _ServersShowZombieVirus);
		KGUI.ResizeGrid("main_menu_servers.servers_grid", _ServerRooms.Count, (GameObject slot, int index) =>
		{
			RoomInfo roomInfo2 = _ServerRooms[index];
			string text = (string)roomInfo2.customProperties["map_name"];
			((Component)slot.transform.Find("txt_title")).GetComponent<UILabel>().text = text;
			string text2 = roomInfo2.playerCount + "/" + roomInfo2.maxPlayers;
			((Component)slot.transform.Find("txt_players")).GetComponent<UILabel>().text = text2;
			GameINI.GameType gameType2 = (GameINI.GameType)(int)roomInfo2.customProperties["game_type"];
			Transform val = slot.transform.Find("icons");
			((Component)val.Find("icon_flag")).gameObject.SetActive(gameType2 == GameINI.GameType.CTF);
			((Component)val.Find("icon_gun")).gameObject.SetActive(gameType2 == GameINI.GameType.TEAM_BATTLE);
			((Component)val.Find("icon_skull")).gameObject.SetActive(gameType2 == GameINI.GameType.DEATHMATCH);
			((Component)val.Find("icon_mining")).gameObject.SetActive(gameType2 == GameINI.GameType.BUILDING);
			((Component)val.Find("icon_zombie")).gameObject.SetActive(gameType2 == GameINI.GameType.ZOMBIE_VIRUS);
			((Component)val.Find("icon_lock")).gameObject.SetActive(roomInfo2.customProperties.ContainsKey("password") && ((string)roomInfo2.customProperties["password"]).Length > 0);
		}, "main_menu_servers");
	}

	private IEnumerator FastConnectToGame(GameINI.GameType gameType)
	{
		yield return 0;
		RoomInfo[] allBattlesRoomList = PhotonNetwork.GetRoomList();
		List<RoomInfo> battlesRoomList = new List<RoomInfo>();
		RoomInfo[] array = allBattlesRoomList;
		foreach (RoomInfo room in array)
		{
			if (room.customProperties.ContainsKey("game_type") && (int)room.customProperties["game_type"] == (int)gameType)
			{
				battlesRoomList.Add(room);
			}
		}
		if (battlesRoomList.Count == 0)
		{
			ShowHint("Нет созданных карт, создайте свою нажав на 'Мои карты'");
			yield break;
		}
		Random r = new Random();
		for (int j = 0; j < battlesRoomList.Count; j++)
		{
			int j2 = r.Next(0, battlesRoomList.Count);
			RoomInfo room2 = battlesRoomList[j2];
			if (((string)room2.customProperties["password"]).Length == 0 && room2.playerCount < room2.maxPlayers)
			{
				JoinGame(room2);
				yield break;
			}
		}
		foreach (Room room3 in battlesRoomList)
		{
			if (((string)room3.customProperties["password"]).Length == 0 && room3.playerCount < room3.maxPlayers)
			{
				JoinGame(room3);
				yield break;
			}
		}
		ShowHint("Все сервера полные или под паролем, создайте свой нажав на 'Мои карты'");
	}

	private IEnumerator GetFavoritServers()
	{
		_FavoritServers = null;
		WWWForm form = new WWWForm();
		form.AddField("viewer_id", VkontakteAPI.INSTANCE._viewerId);
		Random r = new Random();
		form.AddField("random", r.Next());
		WWW php_load = new WWW(App.Instance.GetCurScriptsUrl() + "get_favorit_servers.php", form);
		yield return php_load;
		if (php_load.error != null)
		{
			Debug.LogError((object)php_load.error);
			yield break;
		}
		string[] param = php_load.text.Split('\t');
		List<FavoritServer> favoritServers = new List<FavoritServer>();
		for (int i = 1; i < param.Length; i += 3)
		{
			favoritServers.Add(new FavoritServer
			{
				server_name = param[i],
				admin_id = int.Parse(param[i + 1]),
				slot_id = byte.Parse(param[i + 2])
			});
		}
		_FavoritServers = favoritServers.ToArray();
	}

	public IEnumerator PurchaseUse(StorePurchase purchase)
	{
		Debug.Log((object)("PurchaseUse... " + purchase));
		WWWForm purchaseUseForm = new WWWForm();
		purchaseUseForm.AddField("viewer_id", VkontakteAPI.INSTANCE._viewerId);
		purchaseUseForm.AddField("auth_key", VkontakteAPI.INSTANCE._authKey);
		if (App.Instance.CurPlatform == App.Platform.ONDOKLASNIKI)
		{
			purchaseUseForm.AddField("session_key", VkontakteAPI.INSTANCE._sessionKey);
		}
		purchaseUseForm.AddField("p_ind", (int)purchase);
		Random r = new Random();
		purchaseUseForm.AddField("random", r.Next());
		WWW purchaseUse = new WWW(App.Instance.GetCurScriptsUrl() + "purchase_use.php", purchaseUseForm);
		yield return purchaseUse;
		if (purchaseUse.error != null)
		{
			Debug.Log((object)("Purchase use error: " + purchaseUse.error));
		}
		else if (purchaseUse.text != "OK")
		{
			Debug.Log((object)("Purchase use fail: " + purchaseUse.text));
		}
	}

	private IEnumerator PurchaseBuy(StorePurchase purchase, int dateIndex = 0)
	{
		if (!ProfileINI.CheckPurchaseValue(purchase))
		{
			yield break;
		}
		StorePurchase requiredPurchase = Store.Purchases[purchase].RequiredPurchase;
		if (requiredPurchase != StorePurchase.NONE && ProfileINI.GetPurchaseValue(requiredPurchase) <= 0)
		{
			ShowHint(Localize.GetText("SHOP_LOCKED") + " '" + Localize.GetText("PURCHASE_" + Store.Purchases[requiredPurchase].Name) + "'");
			yield break;
		}
		Store.Pay pay = Store.Purchases[purchase].Cost;
		int count = 0;
		int payCoins = 0;
		int payGems = 0;
		if (pay is Store.OnePay)
		{
			Store.OnePay onePay = (Store.OnePay)pay;
			payCoins = onePay.Coins;
			payGems = onePay.Gems;
			count = onePay.Count;
		}
		else
		{
			Store.TimedPay timedPay = (Store.TimedPay)pay;
			dateIndex = Mathf.Clamp(dateIndex, 1, 4);
			switch (dateIndex)
			{
			case 1:
				payCoins = timedPay.DayCoins;
				payGems = timedPay.DayGems;
				break;
			case 2:
				payCoins = timedPay.WeekCoins;
				payGems = timedPay.WeekGems;
				break;
			case 3:
				payCoins = timedPay.ForeverCoins;
				payGems = timedPay.ForeverGems;
				break;
			default:
				payCoins = timedPay.MonthCoins;
				payGems = timedPay.MonthGems;
				break;
			}
			count = dateIndex;
		}
		if (ProfileINI.coins < payCoins)
		{
			ShowNotEnoughMoney(gems: false, payCoins - ProfileINI.coins);
			yield break;
		}
		if (ProfileINI.crystals < payGems)
		{
			ShowNotEnoughMoney(gems: true, payGems - ProfileINI.crystals);
			yield break;
		}
		WWWForm purchaseBuyForm = new WWWForm();
		purchaseBuyForm.AddField("viewer_id", VkontakteAPI.INSTANCE._viewerId);
		purchaseBuyForm.AddField("auth_key", VkontakteAPI.INSTANCE._authKey);
		if (App.Instance.CurPlatform == App.Platform.ONDOKLASNIKI)
		{
			purchaseBuyForm.AddField("session_key", VkontakteAPI.INSTANCE._sessionKey);
		}
		purchaseBuyForm.AddField("p_ind", (int)purchase);
		purchaseBuyForm.AddField("count", count);
		purchaseBuyForm.AddField("pay_coins", payCoins);
		purchaseBuyForm.AddField("pay_gems", payGems);
		int packetID = App.Instance.Rnd.Next();
		purchaseBuyForm.AddField("random", packetID);
		string yobaKey = App.Instance.GenerateKey(20);
		purchaseBuyForm.AddField("yoba_key", App.Instance.KeyEncrypt(yobaKey));
		string[] array = new string[5];
		int num = (int)purchase;
		array[0] = num.ToString();
		array[1] = "_";
		array[2] = packetID.ToString();
		array[3] = "_";
		array[4] = VkontakteAPI.INSTANCE.GetSecret();
		purchaseBuyForm.AddField("beta_key", VkontakteAPI.MD52(string.Concat(array)));
		ShowLoading("SHOP_BUY_IN_PROGRESS", string.Empty);
		string result = null;
		if ((Object)(object)WorldGameObject.Instance == (Object)null || App.Instance.CurPlatform != App.Platform.VKONTATE)
		{
			WWW purchaseBuy = new WWW(App.Instance.GetCurScriptsUrl() + "purchase_buy.php", purchaseBuyForm);
			yield return purchaseBuy;
			result = ((purchaseBuy.error == null) ? purchaseBuy.text : ("error" + purchaseBuy.error));
		}
		else
		{
			string shopRequest = Encoding.UTF8.GetString(purchaseBuyForm.data);
			yield return ((MonoBehaviour)this).StartCoroutine(WorldGameObject.Instance.PhotonPurchaseBuy(shopRequest, (string r) =>
			{
				result = r;
			}));
		}
		HideLoading();
		if (!result.StartsWith("error"))
		{
			if (result.StartsWith("SUCCESS"))
			{
				string serverText = result.Substring(0, result.IndexOf('\n'));
				string serverKey = result.Substring(result.IndexOf('\n') + 1);
				if (serverKey.Trim() == VkontakteAPI.MD52(serverText + "_" + packetID + "_" + yobaKey))
				{
					yobaKey = string.Empty;
					if (serverText.Trim() == "SUCCESS")
					{
						if (payCoins > 0)
						{
							ProfileINI.coins -= payCoins;
							KGUI.SetControlText("main_menu.txt_gold", string.Empty + ProfileINI.coins);
						}
						if (payGems > 0)
						{
							ProfileINI.crystals -= payGems;
							KGUI.SetControlText("main_menu.txt_gems", string.Empty + ProfileINI.crystals);
						}
						if (pay is Store.OnePay)
						{
							if (!((Store.OnePay)pay).Once)
							{
								ProfileINI.SetPurchaseValue(purchase, ProfileINI.GetPurchaseValue(purchase) + count);
							}
							else
							{
								ProfileINI.SetPurchaseValue(purchase, count);
							}
						}
						else
						{
							ProfileINI.SetPurchaseValue(purchase, count);
							switch (count)
							{
							case 1:
								ProfileINI.SetPurchaseTime(purchase, ProfileINI.server_time.AddDays(1.0));
								break;
							case 2:
								ProfileINI.SetPurchaseTime(purchase, ProfileINI.server_time.AddDays(7.0));
								break;
							case 3:
								ProfileINI.SetPurchaseTime(purchase, new DateTime(9999, 12, 30));
								break;
							}
						}
						if (Application.loadedLevel == 0)
						{
							if (purchase == StorePurchase.SLOTS)
							{
								KGUI.ResizeGrid("main_menu_my_maps.my_maps_grid", 1 + ProfileINI.GetPurchaseValue(StorePurchase.SLOTS), (GameObject slot, int index) =>
								{
									((Component)slot.transform.Find("txt_title")).GetComponent<UILabel>().text = Localize.GetText("MY_MAPS_MAP") + " " + (index + 1);
								}, "main_menu_my_maps");
							}
						}
						else if (Application.loadedLevel == 1)
						{
							switch (purchase)
							{
							case StorePurchase.SPEED:
								RefreshAcceleration();
								break;
							case StorePurchase.FLY:
								RefreshFlying();
								break;
							case StorePurchase.TIME_OF_DAY:
								RefreshDayNight();
								break;
							}
							if (purchase == StorePurchase.BLOCK_KIND_FENCE || purchase == StorePurchase.BLOCK_KIND_HALF || purchase == StorePurchase.BLOCK_KIND_DIAGONAL || purchase == StorePurchase.BLOCK_KIND_STAIR)
							{
								Instance.SetMenu(Menu.Hud, show: true);
							}
						}
						if (_CurPurchase == purchase)
						{
							ShowPurchaseItem(purchase);
						}
						SoundManager.Instance.Play(SoundManager.Sound.ShopBuy);
						if ((Object)(object)WorldGameObject.Instance != (Object)null && App.Instance.CurPlatform != App.Platform.VKONTATE)
						{
							int value = ProfileINI.GetPurchaseValue(purchase);
							string time = ProfileINI.GetPurchaseTime(purchase).ToString();
							WorldGameObject.Instance.photonView.RPC("SetPurchase", PhotonTargets.AllViaServer, (int)purchase, value, time);
						}
					}
					else
					{
						ShowHint("Bad request " + serverText);
					}
				}
				else
				{
					ShowHint("Security error");
				}
			}
			else
			{
				ShowHint(result);
			}
		}
		else
		{
			ShowHint(result.Substring("error".Length));
		}
	}

	private void ShowPurchaseItem(StorePurchase purchase)
	{
		_CurPurchase = purchase;
		Store.PurchaseInfo purchaseInfo = Store.Purchases[purchase];
		string text = Localize.GetText("PURCHASE_" + purchaseInfo.Name);
		string text2 = Localize.GetText("PURCHASE_DESC_" + purchaseInfo.Name, string.Empty);
		Store.Pay cost = purchaseInfo.Cost;
		bool flag = _ShopPurchaseItemInfo && text2.Length > 0;
		KGUI.EnableNodes("main_menu_shop.page2_shop", enable: false);
		KGUI.EnableNodes("main_menu_shop.page2_shop", enable: true);
		KGUI.EnableNodes("main_menu_shop.page2_shop.ibtn_item_upgrade", enable: false);
		KGUI.SetControlText("main_menu_shop.page2_shop.txt_item_name", text);
		KGUI.SetControlText("main_menu_shop.page2_shop.txt_info", string.Empty);
		if (!flag)
		{
			KGUI.EnableNodes("main_menu_shop.page2_shop.ibtn_item_back", enable: false);
			KGUI.EnableNodes("main_menu_shop.page2_shop.txt_info_ext", enable: false);
			if (purchaseInfo.LargeIcon != null)
			{
				KGUI.SetControlSprite(KGUI.FindNode("main_menu_shop.page2_shop.item_preview"), purchaseInfo.LargeIcon, 150f);
				KGUI.EnableNodes("main_menu_shop.page2_shop.item_preview_shadow", enable: false);
			}
			else
			{
				KGUI.SetControlSprite(KGUI.FindNode("main_menu_shop.page2_shop.item_preview"), purchaseInfo.Icon, 60f);
			}
			if (text2.Length == 0)
			{
				KGUI.EnableNodes("main_menu_shop.page2_shop.ibtn_item_info", enable: false);
			}
		}
		else
		{
			KGUI.EnableNodes("main_menu_shop.page2_shop.ibtn_item_info", enable: false);
			KGUI.EnableNodes("main_menu_shop.page2_shop.item_preview", enable: false);
			KGUI.EnableNodes("main_menu_shop.page2_shop.item_preview_shadow", enable: false);
			KGUI.SetControlText("main_menu_shop.page2_shop.txt_info_ext", text2);
		}
		GetPurchaseContent(out var purchaseEntities, out var purchaseBlocks);
		if (purchaseEntities.Count + purchaseBlocks.Count <= 1)
		{
			KGUI.EnableNodes("main_menu_shop.page2_shop.ibtn_item_look", enable: false);
		}
		if (cost is Store.TimedPay)
		{
			Store.TimedPay timedPay = (Store.TimedPay)cost;
			KGUI.EnableNodes("main_menu_shop.page2_shop.buy_one", enable: false);
			if (ProfileINI.GetPurchaseValue(purchase) > 0 && ProfileINI.GetPurchaseTime(purchase).Year > 2000)
			{
				if (ProfileINI.GetPurchaseTime(purchase).Year == 9999)
				{
					KGUI.SetControlText("main_menu_shop.page2_shop.txt_info", Localize.GetText("SHOP_FOREVER_BUY"));
				}
				else
				{
					KGUI.SetControlText("main_menu_shop.page2_shop.txt_info", Localize.GetText("SHOP_ACTIVE_UNTIL") + ProfileINI.GetPurchaseTime(purchase));
				}
				KGUI.EnableNodes("main_menu_shop.page2_shop.buy_day", enable: false);
				KGUI.EnableNodes("main_menu_shop.page2_shop.buy_week", enable: false);
				KGUI.EnableNodes("main_menu_shop.page2_shop.buy_forever", enable: false);
				KGUI.EnableNodes("main_menu_shop.page2_shop.buy_month", enable: false);
			}
			else
			{
				if ((int)timedPay.DayCoins > 0)
				{
					KGUI.EnableNodes("main_menu_shop.page2_shop.buy_day.icon_gems", enable: false);
				}
				else
				{
					KGUI.EnableNodes("main_menu_shop.page2_shop.buy_day.icon_gold", enable: false);
				}
				if ((int)timedPay.WeekCoins > 0)
				{
					KGUI.EnableNodes("main_menu_shop.page2_shop.buy_week.icon_gems", enable: false);
				}
				else
				{
					KGUI.EnableNodes("main_menu_shop.page2_shop.buy_week.icon_gold", enable: false);
				}
				if ((int)timedPay.ForeverCoins > 0)
				{
					KGUI.EnableNodes("main_menu_shop.page2_shop.buy_forever.icon_gems", enable: false);
				}
				else
				{
					KGUI.EnableNodes("main_menu_shop.page2_shop.buy_forever.icon_gold", enable: false);
				}
				if ((int)timedPay.MonthCoins > 0)
				{
					KGUI.EnableNodes("main_menu_shop.page2_shop.buy_month.icon_gems", enable: false);
				}
				else
				{
					KGUI.EnableNodes("main_menu_shop.page2_shop.buy_month.icon_gold", enable: false);
				}
				if ((int)timedPay.DayCoins + (int)timedPay.DayGems > 0)
				{
					KGUI.SetControlText("main_menu_shop.page2_shop.txt_buy_day_price", string.Empty + ((int)timedPay.DayCoins + (int)timedPay.DayGems));
				}
				else
				{
					KGUI.EnableNodes("main_menu_shop.page2_shop.buy_day", enable: false);
				}
				if ((int)timedPay.WeekCoins + (int)timedPay.WeekGems > 0)
				{
					KGUI.SetControlText("main_menu_shop.page2_shop.txt_buy_week_price", string.Empty + ((int)timedPay.WeekCoins + (int)timedPay.WeekGems));
				}
				else
				{
					KGUI.EnableNodes("main_menu_shop.page2_shop.buy_week", enable: false);
				}
				if ((int)timedPay.ForeverCoins + (int)timedPay.ForeverGems > 0)
				{
					KGUI.SetControlText("main_menu_shop.page2_shop.txt_buy_forever_price", string.Empty + ((int)timedPay.ForeverCoins + (int)timedPay.ForeverGems));
				}
				else
				{
					KGUI.EnableNodes("main_menu_shop.page2_shop.buy_forever", enable: false);
				}
				if ((int)timedPay.MonthCoins + (int)timedPay.MonthGems > 0)
				{
					KGUI.SetControlText("main_menu_shop.page2_shop.txt_buy_month_price", string.Empty + ((int)timedPay.MonthCoins + (int)timedPay.MonthGems));
				}
				else
				{
					KGUI.EnableNodes("main_menu_shop.page2_shop.buy_month", enable: false);
				}
			}
		}
		else
		{
			Store.OnePay onePay = (Store.OnePay)cost;
			KGUI.EnableNodes("main_menu_shop.page2_shop.buy_day", enable: false);
			KGUI.EnableNodes("main_menu_shop.page2_shop.buy_week", enable: false);
			KGUI.EnableNodes("main_menu_shop.page2_shop.buy_forever", enable: false);
			KGUI.EnableNodes("main_menu_shop.page2_shop.buy_month", enable: false);
			if ((int)onePay.Coins > 0)
			{
				KGUI.EnableNodes("main_menu_shop.page2_shop.buy_one.icon_gems", enable: false);
			}
			else
			{
				KGUI.EnableNodes("main_menu_shop.page2_shop.buy_one.icon_gold", enable: false);
			}
			if ((bool)onePay.Once)
			{
				if (ProfileINI.GetPurchaseValue(purchase) != 0)
				{
					KGUI.EnableNodes("main_menu_shop.page2_shop.buy_one", enable: false);
					KGUI.SetControlText("main_menu_shop.page2_shop.txt_info", Localize.GetText("SHOP_BOUGHT"));
				}
				else
				{
					KGUI.SetControlText("main_menu_shop.page2_shop.buy_one.txt_price", string.Empty + ((int)onePay.Coins + (int)onePay.Gems));
				}
			}
			else
			{
				KGUI.SetControlText("main_menu_shop.page2_shop.txt_info", Localize.GetText("SHOP_YOU_HAVE") + " " + ProfileINI.GetPurchaseValue(purchase));
				KGUI.SetControlText("main_menu_shop.page2_shop.buy_one.txt_price", string.Empty + ((int)onePay.Coins + (int)onePay.Gems));
			}
		}
		if ((StorePurchase)Store.Purchases[purchase].RequiredPurchase != StorePurchase.NONE && ProfileINI.GetPurchaseValue(Store.Purchases[purchase].RequiredPurchase) <= 0)
		{
			KGUI.SetControlText("main_menu_shop.page2_shop.buy_one.txt_title", Localize.GetText("SHOP_BUY_LOCK"));
		}
		else
		{
			KGUI.SetControlText("main_menu_shop.page2_shop.buy_one.txt_title", Localize.GetText("SHOP_BUY_ONE"));
		}
		if (purchase != StorePurchase.CHANGENICK || ProfileINI.GetPurchaseValue(purchase) <= 0)
		{
			KGUI.EnableNodes("main_menu_shop.page2_shop.use", enable: false);
		}
		if ((Store.TabType)Store.Purchases[purchase].Tab == Store.TabType.Skins)
		{
			bool flag2 = (int)ProfileINI.skin == (int)Store.Purchases[purchase].Skin;
			if (ProfileINI.GetPurchaseValue(purchase) == 0 || flag2)
			{
				KGUI.EnableNodes("main_menu_shop.page2_shop.equip", enable: false);
			}
			if (flag2)
			{
				KGUI.SetControlText("main_menu_shop.page2_shop.txt_info", Localize.GetText("SHOP_EQUIPPED"));
			}
		}
		else
		{
			KGUI.EnableNodes("main_menu_shop.page2_shop.equip", enable: false);
		}
		bool flag3 = false;
		KGUI.EnableNodes("main_menu_shop.page2_shop.skin_preview", enable: false);
		KGUI.EnableNodes("main_menu_shop.page2_shop.pet_preview", enable: false);
		if (!flag)
		{
			if (_ShopTab == Store.TabType.Skins)
			{
				GameObject val = GameObject.Find("SkinPreview");
				if ((Object)(object)val != (Object)null)
				{
					val.GetComponentInChildren<SkinManager>().SetSkin(Store.Purchases[purchase].Skin);
					KGUI.EnableNodes("main_menu_shop.page2_shop.skin_preview", enable: true);
					flag3 = true;
				}
			}
			else if (_ShopTab == Store.TabType.Pets)
			{
				GameObject val2 = GameObject.Find("PetPreview");
				if ((Object)(object)val2 != (Object)null)
				{
					switch (purchase)
					{
					case StorePurchase.FISH_1:
						val2.GetComponentInChildren<SkinManager>().SetSkin(0);
						break;
					case StorePurchase.FISH_2:
						val2.GetComponentInChildren<SkinManager>().SetSkin(1);
						break;
					case StorePurchase.FISH_3:
						val2.GetComponentInChildren<SkinManager>().SetSkin(2);
						break;
					case StorePurchase.CAT:
						val2.GetComponentInChildren<SkinManager>().SetSkin(3);
						break;
					case StorePurchase.DOG:
						val2.GetComponentInChildren<SkinManager>().SetSkin(4);
						break;
					case StorePurchase.CHICKEN:
						val2.GetComponentInChildren<SkinManager>().SetSkin(5);
						break;
					case StorePurchase.BOAR:
						val2.GetComponentInChildren<SkinManager>().SetSkin(6);
						break;
					case StorePurchase.CRAB:
						val2.GetComponentInChildren<SkinManager>().SetSkin(7);
						break;
					}
					KGUI.EnableNodes("main_menu_shop.page2_shop.pet_preview", enable: true);
					flag3 = true;
				}
			}
		}
		if (flag3)
		{
			((MonoBehaviour)this).StopCoroutine("PreviewRotationProcess");
			((MonoBehaviour)this).StartCoroutine("PreviewRotationProcess");
			KGUI.EnableNodes("main_menu_shop.page2_shop.item_preview", enable: false);
			KGUI.EnableNodes("main_menu_shop.page2_shop.item_preview_shadow", enable: false);
		}
		else
		{
			((MonoBehaviour)this).StopCoroutine("PreviewRotationProcess");
			KGUI.EnableNodes("main_menu_shop.page2_shop.ibtn_rotate_left", enable: false);
			KGUI.EnableNodes("main_menu_shop.page2_shop.ibtn_rotate_right", enable: false);
		}
	}

	private void GetPurchaseContent(out List<EntityType> purchaseEntities, out List<BlockType> purchaseBlocks)
	{
		purchaseEntities = new List<EntityType>();
		foreach (KeyValuePair<EntityType, Store.EntityInfo> entity in Store.Entities)
		{
			if ((StorePurchase)entity.Value.Purchase == _CurPurchase)
			{
				purchaseEntities.Add(entity.Key);
			}
		}
		purchaseBlocks = new List<BlockType>();
		foreach (KeyValuePair<BlockType, Store.BlockInfo> block in Store.Blocks)
		{
			if ((StorePurchase)block.Value.Purchase == _CurPurchase)
			{
				purchaseBlocks.Add(block.Key);
			}
		}
	}

	private IEnumerator PreviewRotationProcess()
	{
		UIImageButton left = ((Component)KGUI.FindNode("main_menu_shop.page2_shop.ibtn_rotate_left")).GetComponent<UIImageButton>();
		UIImageButton right = ((Component)KGUI.FindNode("main_menu_shop.page2_shop.ibtn_rotate_right")).GetComponent<UIImageButton>();
		while (((Component)left).gameObject.activeInHierarchy && ((Component)right).gameObject.activeInHierarchy)
		{
			float rot = 0f;
			if (left.target.spriteName == left.pressedSprite)
			{
				rot = -120f;
			}
			else if (right.target.spriteName == right.pressedSprite)
			{
				rot = 120f;
			}
			if (rot != 0f)
			{
				GameObject preview = null;
				if (_ShopTab == Store.TabType.Skins)
				{
					preview = GameObject.Find("SkinPreview");
				}
				else if (_ShopTab == Store.TabType.Pets)
				{
					preview = GameObject.Find("PetPreview");
				}
				if ((Object)(object)preview != (Object)null)
				{
					((Component)preview.GetComponentInChildren<SkinManager>()).transform.Rotate(Vector3.up, rot * Time.deltaTime);
				}
			}
			yield return (object)new WaitForEndOfFrame();
		}
	}

	private IEnumerator SellCrystall(int count)
	{
		Debug.Log((object)"SellCrystall");
		if (ProfileINI.crystals < count)
		{
			ShowNotEnoughMoney(gems: true, count - ProfileINI.crystals);
			yield break;
		}
		WWWForm form = new WWWForm();
		form.AddField("viewer_id", VkontakteAPI.INSTANCE._viewerId);
		form.AddField("auth_key", VkontakteAPI.INSTANCE._authKey);
		if (App.Instance.CurPlatform == App.Platform.ONDOKLASNIKI)
		{
			form.AddField("session_key", VkontakteAPI.INSTANCE._sessionKey);
		}
		form.AddField("count", count);
		Random r = new Random();
		form.AddField("random", r.Next());
		WWW php_load = new WWW(App.Instance.GetCurScriptsUrl() + "sell_crystalls.php", form);
		yield return php_load;
		Debug.Log((object)php_load.text);
		if (php_load.text.Trim() == "SUCCSES")
		{
			ProfileINI.crystals -= count;
			KGUI.SetControlText("main_menu.txt_gems", string.Empty + ProfileINI.crystals);
			ProfileINI.coins += count * 100;
			KGUI.SetControlText("main_menu.txt_gold", string.Empty + ProfileINI.coins);
		}
	}

	public string FindOpenRoom(int mapID)
	{
		RoomInfo[] roomList = PhotonNetwork.GetRoomList();
		for (int i = 0; i < roomList.Length; i++)
		{
			if (roomList[i].customProperties.ContainsKey("public_id"))
			{
				int num = (int)roomList[i].customProperties["public_id"];
				if (num == mapID)
				{
					return (string)roomList[i].customProperties["map_name"];
				}
			}
		}
		return null;
	}

	private IEnumerator SetPregenerateTimer(int slot_id)
	{
		WWWForm form = new WWWForm();
		form.AddField("id", VkontakteAPI.INSTANCE._viewerId);
		form.AddField("map_place", slot_id);
		Random r = new Random();
		form.AddField("random", r.Next());
		yield return (object)new WWW(App.Instance.GetCurScriptsUrl() + "new_map_timer.php", form);
	}

	private IEnumerator PreregenerateSlot(int slot_id)
	{
		_SelectedMapID = -2;
		WWWForm form = new WWWForm();
		form.AddField("id", VkontakteAPI.INSTANCE._viewerId);
		form.AddField("map_place", slot_id);
		Random r = new Random();
		form.AddField("random", r.Next());
		yield return (object)new WWW(App.Instance.GetCurScriptsUrl() + "is_peregenerate_map.php", form);
		_SelectedMapID = 1;
		yield return 0;
	}

	private IEnumerator PostComment(string text, int map_id)
	{
		WWWForm form = new WWWForm();
		form.AddField("id", VkontakteAPI.INSTANCE._viewerId);
		form.AddField("post_name", ProfileINI.nickname);
		form.AddField("post_text", text);
		form.AddField("map_id", map_id);
		Random r = new Random();
		form.AddField("random", r.Next());
		yield return (object)new WWW(App.Instance.GetCurScriptsUrl() + "post_comment.php", form);
		((MonoBehaviour)this).StartCoroutine(GetComments(map_id, _MapCommentsStartIndex));
	}

	private IEnumerator GetComments(int map_id, int _page_numb)
	{
		_MapCommentsStartIndex += _page_numb - _MapCommentsStartIndex;
		_MapComments.Clear();
		WWWForm form = new WWWForm();
		form.AddField("map_id", map_id);
		form.AddField("start_ind", _MapCommentsStartIndex);
		Debug.Log((object)"GetComentStartIndex");
		Debug.Log((object)_MapCommentsStartIndex);
		Random r = new Random();
		form.AddField("random", r.Next());
		WWW php_load = new WWW(App.Instance.GetCurScriptsUrl() + "get_coments.php", form);
		yield return php_load;
		if (php_load.error != null)
		{
			Debug.LogError((object)php_load.error);
			yield break;
		}
		string[] commentsData = php_load.text.Split('\t');
		for (int i = 1; i < commentsData.Length; i += 3)
		{
			Comment comment = new Comment
			{
				ID = commentsData[i],
				Name = commentsData[i + 1],
				Text = commentsData[i + 2]
			};
			_MapComments.Add(comment);
		}
	}

	private IEnumerator PublicMapToServer(string mapName, string mapAbout, string pictureURL)
	{
		KGUI.EnableNodes("main_menu_my_maps.page2", enable: false, onlyChild: true);
		KGUI.EnableNodes("main_menu.slider_page2", enable: false);
		WWWForm form = new WWWForm();
		form.AddField("id", VkontakteAPI.INSTANCE._viewerId);
		form.AddField("slot_id", _SelectedMapSlot);
		form.AddField("name", mapName);
		form.AddField("about", mapAbout);
		form.AddField("url", (pictureURL == null) ? string.Empty : pictureURL);
		form.AddField("auth_key", VkontakteAPI.INSTANCE._authKey);
		if (App.Instance.CurPlatform == App.Platform.ONDOKLASNIKI)
		{
			form.AddField("session_key", VkontakteAPI.INSTANCE._sessionKey);
		}
		Random r = new Random();
		form.AddField("random", r.Next());
		yield return (object)new WWW(App.Instance.GetCurScriptsUrl() + "public_map.php", form);
		yield return ((MonoBehaviour)this).StartCoroutine(LoadSlotMapInfo(VkontakteAPI.INSTANCE._viewerId, _SelectedMapSlot));
	}

	private IEnumerator ClosePublicMap(string veiwer_id, int slot_id, int map_id)
	{
		KGUI.EnableNodes("main_menu_my_maps.page2", enable: false, onlyChild: true);
		KGUI.EnableNodes("main_menu.slider_page2", enable: false);
		WWWForm form = new WWWForm();
		form.AddField("id", veiwer_id);
		form.AddField("slot_id", slot_id);
		form.AddField("map_id", map_id);
		Random r = new Random();
		form.AddField("random", r.Next());
		form.AddField("auth_key", VkontakteAPI.INSTANCE._authKey);
		if (App.Instance.CurPlatform == App.Platform.ONDOKLASNIKI)
		{
			form.AddField("session_key", VkontakteAPI.INSTANCE._sessionKey);
		}
		yield return (object)new WWW(App.Instance.GetCurScriptsUrl() + "close_map.php", form);
		yield return ((MonoBehaviour)this).StartCoroutine(LoadSlotMapInfo(veiwer_id, slot_id));
	}

	private IEnumerator LoadSlotMapInfo(string viewerID, int slotID)
	{
		KGUI.EnableNodes("main_menu_my_maps.page2", enable: false, onlyChild: true);
		KGUI.EnableNodes("main_menu.slider_page2", enable: false);
		WWWForm getSlotInfoForm = new WWWForm();
		getSlotInfoForm.AddField("id", viewerID);
		Random r = new Random();
		getSlotInfoForm.AddField("random", r.Next());
		getSlotInfoForm.AddField("map_place", slotID);
		WWW getSlotInfo = new WWW(App.Instance.GetCurScriptsUrl() + "get_slot_info.php", getSlotInfoForm);
		yield return getSlotInfo;
		_SelectedMapOwner = viewerID;
		_SelectedMapSlot = slotID;
		_SelectedMapID = 0;
		_SelectedMapNotVoted = false;
		_SelectedMapRating = 0;
		_SelectedMapAuthor = string.Empty;
		if (getSlotInfo.text == "NOT AVAILABLE SLOT")
		{
			ShowHint("Слота не существет");
			_SelectedMapSlot = 0;
			yield break;
		}
		if (getSlotInfo.text != "EMPTY SLOT")
		{
			_SelectedMapID = Convert.ToInt32(getSlotInfo.text.Trim());
			if (_SelectedMapID < 0)
			{
				ShowHint("Слот заблокирован");
				_SelectedMapSlot = 0;
				_SelectedMapID = 0;
				yield break;
			}
		}
		if (Application.loadedLevel == 0)
		{
			if (_SelectedMapID != 0)
			{
				KGUI.EnableNodes("main_menu_my_maps.page2.base", enable: true);
				KGUI.EnableNodes("main_menu_my_maps.page2.base.save", enable: false);
				if (_SelectedMapID == 1)
				{
					KGUI.EnableNodes("main_menu_my_maps.page2.base.load.button_look_in_public", enable: false);
					KGUI.EnableNodes("main_menu_my_maps.page2.base.load.button_remove_from_public", enable: false);
				}
				else
				{
					KGUI.EnableNodes("main_menu_my_maps.page2.base.load.button_generate", enable: false);
					KGUI.EnableNodes("main_menu_my_maps.page2.base.load.button_public", enable: false);
				}
				((Component)KGUI.FindNode("main_menu_my_maps.page2.base.load")).GetComponent<UITable>().Reposition();
			}
			else
			{
				ShowMyMap(isGenerate: true, isToPublic: false);
			}
		}
		else
		{
			KGUI.EnableNodes("main_menu_my_maps.page2.base", enable: true);
			KGUI.EnableNodes("main_menu_my_maps.page2.base.load", enable: false);
			KGUI.EnableNodes("main_menu_my_maps.page2.base.save.txt_empty", enable: false);
			KGUI.EnableNodes("main_menu_my_maps.page2.base.save.txt_rewrite", enable: false);
			if (_SelectedMapID >= 1)
			{
				KGUI.EnableNodes("main_menu_my_maps.page2.base.save.txt_rewrite", enable: true);
			}
			else
			{
				KGUI.EnableNodes("main_menu_my_maps.page2.base.save.txt_empty", enable: true);
			}
		}
	}

	private void ShowMyMap(bool isGenerate, bool isToPublic)
	{
		KGUI.EnableNodes("main_menu_my_maps.page2", enable: false, onlyChild: true);
		KGUI.EnableNodes("main_menu_my_maps.page2.info", enable: true);
		if (isGenerate)
		{
			((Component)KGUI.FindNode("main_menu_my_maps.page2.info.map_time")).tag = "Sort03";
			KGUI.SetControlText("main_menu_my_maps.page2.info.map_size.txt_type", Localize.GetText("MAP_SIZE_" + _MapSizes[_MapSize]));
			KGUI.SetControlText("main_menu_my_maps.page2.info.map_type.txt_type", Localize.GetText("MAP_TYPE_" + _MapTypes[_MapType]));
		}
		else
		{
			((Component)KGUI.FindNode("main_menu_my_maps.page2.info.map_time")).tag = "Sort09";
			KGUI.EnableNodes("main_menu_my_maps.page2.info.map_size", enable: false);
			KGUI.EnableNodes("main_menu_my_maps.page2.info.map_type", enable: false);
		}
		if (isToPublic)
		{
			KGUI.EnableNodes("main_menu_my_maps.page2.info.destroyable", enable: false);
			KGUI.EnableNodes("main_menu_my_maps.page2.info.game_type", enable: false);
			KGUI.EnableNodes("main_menu_my_maps.page2.info.password", enable: false);
			KGUI.EnableNodes("main_menu_my_maps.page2.info.bonuses", enable: false);
		}
		else
		{
			KGUI.SetControlText("main_menu_my_maps.page2.info.map_time.txt_type", Localize.GetText("MAP_TIME_" + _MapTimes[_MapTime]));
			KGUI.SetControlCheckbox("main_menu_my_maps.page2.info.destroyable.ckb_destroyable", _MapDestroyable);
			KGUI.SetControlCheckbox("main_menu_my_maps.page2.info.bonuses.ckb_bonuses", _MapBonuses);
			KGUI.EnableNodes("main_menu_my_maps.page2.info.destroyable", _MapGameTypes[_MapGameType] != GameINI.GameType.BUILDING);
			KGUI.EnableNodes("main_menu_my_maps.page2.info.bonuses", _MapGameTypes[_MapGameType] != GameINI.GameType.BUILDING);
			KGUI.SetControlText("main_menu_my_maps.page2.info.game_type.txt_type", Localize.GetText("GAME_TYPE_" + _MapGameTypes[_MapGameType]));
			KGUI.EnableNodes("main_menu_my_maps.page2.info.screen", enable: false);
		}
		_CustomMapImageURL = null;
		KGUI.EnableNodes("main_menu_my_maps.page2.info.screen_custom", enable: false);
		KGUI.EnableNodes("main_menu_my_maps.page2.info.screen_loading", enable: false);
		KGUI.EnableNodes("main_menu_my_maps.page2.info.button_load_map", !isGenerate && !isToPublic);
		KGUI.EnableNodes("main_menu_my_maps.page2.info.button_public_map", isToPublic);
		KGUI.EnableNodes("main_menu_my_maps.page2.info.button_create_map", isGenerate);
		((Component)KGUI.FindNode("main_menu_my_maps.page2.info.clip_map_info.grid_map_info")).GetComponent<UITable>().Reposition();
		KGUI.ResetScrollBar("main_menu_my_maps.page2.info.clip_map_info");
	}

	public IEnumerator LoadTopMapInfo(int mapID)
	{
		KGUI.EnableNodes("main_menu_top_maps.page2", enable: false);
		_MapCommentsStartIndex = 0;
		WWWForm getMapInfoForm = new WWWForm();
		getMapInfoForm.AddField("map_id", mapID);
		Random r = new Random();
		getMapInfoForm.AddField("random", r.Next());
		WWW getMapInfo = new WWW(App.Instance.GetCurScriptsUrl() + "get_map_info.php", getMapInfoForm);
		yield return getMapInfo;
		if (getMapInfo.error != null)
		{
			Debug.LogError((object)getMapInfo.error);
		}
		else if (((Component)KGUI.FindNode("main_menu_top_maps")).gameObject.activeInHierarchy)
		{
			string[] info = getMapInfo.text.Split('\t');
			_SelectedMapOwner = info[7];
			_SelectedMapSlot = int.Parse(info[8]);
			_SelectedMapID = mapID;
			_SelectedMapNotVoted = false;
			_SelectedMapRating = int.Parse(info[6]);
			_SelectedMapAuthor = info[3];
			KGUI.EnableNodes("main_menu_top_maps.page2", enable: true);
			KGUI.EnableNodes("main_menu_top_maps.score.background_large", enable: false);
			KGUI.EnableNodes("main_menu_top_maps.score.button_vote_yes", enable: false);
			KGUI.SetControlText("main_menu_top_maps.txt_author", _SelectedMapAuthor);
			KGUI.SetControlText("main_menu_top_maps.txt_score", string.Empty + _SelectedMapRating);
			KGUI.SetControlText("main_menu_top_maps.txt_name", info[0]);
			KGUI.SetControlText("main_menu_top_maps.txt_description", info[1]);
			KGUI.ResetScrollBar("main_menu_top_maps.page2.clip_map_info");
			((MonoBehaviour)this).StartCoroutine(GetCustomMapImage(mapID, "main_menu_top_maps.screen", info[2]));
			((MonoBehaviour)this).StartCoroutine(GetVoteData(mapID));
		}
	}

	private IEnumerator GetVoteData(int mapID)
	{
		WWWForm getVoteForm = new WWWForm();
		getVoteForm.AddField("id", VkontakteAPI.INSTANCE._viewerId);
		getVoteForm.AddField("map_id", mapID);
		Random r = new Random();
		getVoteForm.AddField("random", r.Next());
		WWW getVote = new WWW(App.Instance.GetCurScriptsUrl() + "is_voute_by_map.php", getVoteForm);
		yield return getVote;
		if (mapID != _SelectedMapID)
		{
			yield break;
		}
		if (getVote.error == null)
		{
			if (getVote.text.Trim() == "NOT_VOTE")
			{
				_SelectedMapNotVoted = true;
				KGUI.EnableNodes("main_menu_top_maps.score.background_large", enable: true);
				KGUI.EnableNodes("main_menu_top_maps.score.background_small", enable: false);
				KGUI.EnableNodes("main_menu_top_maps.score.button_vote_yes", enable: true);
			}
			else if (getVote.text.Trim() == "YES")
			{
				Debug.Log((object)"Vote: Yes");
			}
			else if (getVote.text.Trim() == "NO")
			{
				Debug.Log((object)"Vote: No");
			}
			else
			{
				Debug.LogError((object)getVote.text);
			}
		}
		else
		{
			Debug.LogError((object)getVote.error);
		}
	}

	private IEnumerator VoteYes(int mapID)
	{
		WWWForm voteYesForm = new WWWForm();
		voteYesForm.AddField("id", VkontakteAPI.INSTANCE._viewerId);
		voteYesForm.AddField("map_id", mapID);
		voteYesForm.AddField("auth_key", VkontakteAPI.INSTANCE._authKey);
		if (App.Instance.CurPlatform == App.Platform.ONDOKLASNIKI)
		{
			voteYesForm.AddField("session_key", VkontakteAPI.INSTANCE._sessionKey);
		}
		Random r = new Random();
		voteYesForm.AddField("random", r.Next());
		yield return (object)new WWW(App.Instance.GetCurScriptsUrl() + "voute_yes.php", voteYesForm);
	}

	private IEnumerator VoteYesNot(int mapID)
	{
		WWWForm voteYesNotForm = new WWWForm();
		voteYesNotForm.AddField("id", VkontakteAPI.INSTANCE._viewerId);
		voteYesNotForm.AddField("map_id", mapID);
		Random r = new Random();
		voteYesNotForm.AddField("random", r.Next());
		voteYesNotForm.AddField("auth_key", VkontakteAPI.INSTANCE._authKey);
		if (App.Instance.CurPlatform == App.Platform.ONDOKLASNIKI)
		{
			voteYesNotForm.AddField("session_key", VkontakteAPI.INSTANCE._sessionKey);
		}
		yield return (object)new WWW(App.Instance.GetCurScriptsUrl() + "voute_yes_not.php", voteYesNotForm);
	}

	private IEnumerator VoteNoNot(int mapID)
	{
		WWWForm voteNoNotForm = new WWWForm();
		voteNoNotForm.AddField("id", VkontakteAPI.INSTANCE._viewerId);
		voteNoNotForm.AddField("map_id", mapID);
		Random r = new Random();
		voteNoNotForm.AddField("random", r.Next());
		yield return (object)new WWW(App.Instance.GetCurScriptsUrl() + "voute_no_not.php", voteNoNotForm);
	}

	private IEnumerator GetCustomMapImage(int mapID, string controlName, string imageURL)
	{
		UITable table = ((Component)KGUI.FindNode(controlName).parent).GetComponent<UITable>();
		UISprite screen = ((Component)KGUI.FindNode(controlName)).GetComponent<UISprite>();
		UITexture screenCustom = ((Component)KGUI.FindNode(controlName + "_custom")).GetComponent<UITexture>();
		GameObject screenLoading = ((Component)KGUI.FindNode(controlName + "_loading")).gameObject;
		if (!((Component)screen).gameObject.activeInHierarchy || ((Component)screenCustom).gameObject.activeInHierarchy || screenLoading.gameObject.activeInHierarchy)
		{
			((Component)screen).gameObject.SetActive(true);
			((Component)screenCustom).gameObject.SetActive(false);
			screenLoading.SetActive(false);
			if ((Object)(object)table != (Object)null)
			{
				table.repositionNow = true;
			}
		}
		if (imageURL.Length == 0)
		{
			yield break;
		}
		((Component)screen).gameObject.SetActive(false);
		screenLoading.SetActive(true);
		if ((Object)(object)table != (Object)null)
		{
			table.repositionNow = true;
		}
		WWW www = new WWW(imageURL);
		yield return www;
		if (mapID == _SelectedMapID)
		{
			screenLoading.SetActive(false);
			if (www.error == null)
			{
				((Component)screenCustom).gameObject.SetActive(true);
				screenCustom.mainTexture = (Texture)(object)www.texture;
				_CustomMapImageURL = imageURL;
			}
			else
			{
				Debug.LogError((object)www.error);
				((Component)screen).gameObject.SetActive(true);
			}
			if ((Object)(object)table != (Object)null)
			{
				table.repositionNow = true;
			}
		}
	}

	private IEnumerator RefreshListOfMapsBestAllTime()
	{
		yield return ((MonoBehaviour)this).StartCoroutine(RefreshListOfMapsEx("get_all_time_map"));
	}

	private IEnumerator RefreshListOfMapsMonthly()
	{
		yield return ((MonoBehaviour)this).StartCoroutine(RefreshListOfMapsEx("get_monthly_map"));
	}

	private IEnumerator RefreshListOfDay()
	{
		yield return ((MonoBehaviour)this).StartCoroutine(RefreshListOfMapsEx("get_day_map"));
	}

	private IEnumerator RefreshListOfMapsWeekly()
	{
		yield return ((MonoBehaviour)this).StartCoroutine(RefreshListOfMapsEx("get_weekly_map"));
	}

	private IEnumerator RefreshListOfMapsNew()
	{
		yield return ((MonoBehaviour)this).StartCoroutine(RefreshListOfMapsEx("get_latest_map"));
	}

	private IEnumerator RefreshListOfMapsEx(string script)
	{
		_GetMapsScript = script;
		_GetMapsFormData = new Dictionary<string, string>
		{
			{ "page", "0" },
			{
				"random",
				string.Empty + new Random().Next()
			}
		};
		WWWForm getMapsForm = new WWWForm();
		foreach (KeyValuePair<string, string> data in _GetMapsFormData)
		{
			getMapsForm.AddField(data.Key, data.Value);
		}
		WWW getMaps = new WWW(App.Instance.GetCurScriptsUrl() + script + ".php", getMapsForm);
		yield return getMaps;
		if (getMaps.error == null)
		{
			((MonoBehaviour)this).StartCoroutine(RefreshListOfMaps(getMaps.text, append: false));
		}
		else
		{
			Debug.LogError((object)getMaps.error);
		}
	}

	private IEnumerator SearchMaps(int searchType, bool searchSortType, string searchMapName, string searchMapAuthor)
	{
		_GetMapsScript = "search_maps";
		_GetMapsFormData = new Dictionary<string, string>
		{
			{
				"text",
				(searchType != 0) ? searchMapAuthor : searchMapName
			},
			{
				"sortby",
				(!searchSortType) ? "public_date" : "rating"
			},
			{
				"type",
				string.Empty + searchType
			},
			{ "page", "0" },
			{
				"random",
				string.Empty + new Random().Next()
			}
		};
		WWWForm getMapsForm = new WWWForm();
		foreach (KeyValuePair<string, string> data in _GetMapsFormData)
		{
			getMapsForm.AddField(data.Key, data.Value);
		}
		WWW getMaps = new WWW(App.Instance.GetCurScriptsUrl() + "search_maps.php", getMapsForm);
		yield return getMaps;
		if (getMaps.error == null)
		{
			((MonoBehaviour)this).StartCoroutine(RefreshListOfMaps(getMaps.text, append: false));
		}
		else
		{
			Debug.LogError((object)getMaps.error);
		}
	}

	private IEnumerator LoadMoreMaps()
	{
		_GetMapsFormData["random"] = string.Empty + new Random().Next();
		_GetMapsFormData["page"] = string.Empty + (int.Parse(_GetMapsFormData["page"]) + 1);
		WWWForm getMapsForm = new WWWForm();
		foreach (KeyValuePair<string, string> data in _GetMapsFormData)
		{
			getMapsForm.AddField(data.Key, data.Value);
		}
		WWW getMaps = new WWW(App.Instance.GetCurScriptsUrl() + _GetMapsScript + ".php", getMapsForm);
		yield return getMaps;
		if (getMaps.error == null)
		{
			yield return ((MonoBehaviour)this).StartCoroutine(RefreshListOfMaps(getMaps.text, append: true));
		}
		else
		{
			Debug.LogError((object)getMaps.error);
		}
	}

	public IEnumerator RefreshListOfMaps(string text, bool append)
	{
		if (!append)
		{
			_Maps.Clear();
		}
		if (text.Length == 0)
		{
			yield break;
		}
		string[] mapsData = text.Split('\t');
		for (int i = 1; i < mapsData.Length; i += 5)
		{
			Map map = new Map
			{
				MapID = mapsData[i],
				AuthorID = mapsData[i + 1],
				AuthorSlotID = mapsData[i + 2],
				Name = mapsData[i + 3],
				Rating = mapsData[i + 4]
			};
			_Maps.Add(map);
		}
		bool moreMaps = true;
		KGUI.ResizeGrid("main_menu_top_maps.top_maps_grid", _Maps.Count + (moreMaps ? 1 : 0), (GameObject slot, int index) =>
		{
			if (index < _Maps.Count)
			{
				Map map2 = _Maps[index];
				slot.GetComponentInChildren<UILabel>().text = map2.Name + " [* " + map2.Rating + "]";
			}
			else
			{
				slot.GetComponentInChildren<UILabel>().text = Localize.GetText("TOP_MAPS_MORE_MAPS");
			}
		});
		if (!append && _Maps.Count > 0 && CurMenu == Menu.TopMaps)
		{
			yield return ((MonoBehaviour)this).StartCoroutine(LoadTopMapInfo(int.Parse(_Maps[0].MapID)));
		}
	}

	private IEnumerator ChangeNameProcess(string name)
	{
		ShowLoading("LOADING_NAME_CHANGING", string.Empty);
		KGUI.EnableNodes("change_name.txt_name_busy", enable: false);
		WWWForm setNameForm = new WWWForm();
		Random r = new Random();
		setNameForm.AddField("random", r.Next());
		setNameForm.AddField("id", VkontakteAPI.INSTANCE._viewerId);
		setNameForm.AddField("auth_key", VkontakteAPI.INSTANCE._authKey);
		if (App.Instance.CurPlatform == App.Platform.ONDOKLASNIKI)
		{
			setNameForm.AddField("session_key", VkontakteAPI.INSTANCE._sessionKey);
		}
		setNameForm.AddField("name", name);
		WWW setName = new WWW(App.Instance.GetCurScriptsUrl() + "set_name.php", setNameForm);
		yield return setName;
		if (setName.error == null)
		{
			if (setName.text == "SET_NAME")
			{
				ProfileINI.SetPurchaseValue(StorePurchase.CHANGENICK, ProfileINI.GetPurchaseValue(StorePurchase.CHANGENICK) - 1);
				((MonoBehaviour)this).StartCoroutine(PurchaseUse(StorePurchase.CHANGENICK));
				ProfileINI.Save();
				if (!PhotonNetwork.connected)
				{
					App.Instance.ConnectToShard(random: true);
				}
				else
				{
					SwitchMenu(Menu.Start);
				}
			}
			else
			{
				KGUI.EnableNodes("change_name.txt_name_busy", enable: true);
			}
		}
		else
		{
			Debug.LogError((object)setName.error);
		}
		HideLoading();
	}

	public void RefreshTabMenu()
	{
		if ((Object)(object)TeamBattle.Instance != (Object)null)
		{
			TeamBattle.Instance.RefreshTabMenu();
		}
		if (!((Component)KGUI.FindNode("tab_menu")).gameObject.activeInHierarchy)
		{
			return;
		}
		bool isWatch = App.Instance.Settings.isWatch;
		bool isAdministrator = Level.Instance.IsAdministrator() && !isWatch;
		bool isModerator = Level.Instance.IsModerator() && !isWatch;
		string text = (string)PhotonNetwork.room.customProperties["map_name"];
		KGUI.SetControlText("tab_menu.txt_title", text);
		if (isAdministrator)
		{
			KGUI.SetControlCheckbox("tab_menu.ckb_new_players_building", ProfileINI.newgamersislook);
		}
		else
		{
			KGUI.EnableNodes("tab_menu.administration", enable: false);
		}
		if (isWatch)
		{
			KGUI.SetControlText("tab_menu.top_map_info.txt_author", _SelectedMapAuthor);
			KGUI.EnableNodes("tab_menu.top_map_info.score.background_large", _SelectedMapNotVoted);
			KGUI.EnableNodes("tab_menu.top_map_info.score.background_small", !_SelectedMapNotVoted);
			KGUI.EnableNodes("tab_menu.top_map_info.score.button_vote_yes", _SelectedMapNotVoted);
			KGUI.SetControlText("tab_menu.top_map_info.score.txt_score", string.Empty + _SelectedMapRating);
		}
		else
		{
			KGUI.EnableNodes("tab_menu.top_map_info", enable: false);
		}
		string[] banned = Level.Instance.Banned;
		KGUI.ResizeGrid("tab_menu.grid_players", WorldGameObject.Instance.PlayerList.Count + banned.Length, (GameObject slot, int index) =>
		{
			string text2;
			bool flag;
			if (index < WorldGameObject.Instance.PlayerList.Count)
			{
				PlayerNode playerNode = WorldGameObject.Instance.PlayerList[index];
				text2 = playerNode.NetPlayer.name;
				flag = playerNode.NetPlayer == PhotonNetwork.player;
			}
			else
			{
				text2 = banned[index - WorldGameObject.Instance.PlayerList.Count];
				flag = false;
			}
			bool flag2 = Level.Instance.IsAdministrator(text2) && !isWatch;
			bool flag3 = Level.Instance.IsModerator(text2) && !isWatch;
			bool isChecked = Level.Instance.IsBuilder(text2) && !isWatch;
			bool flag4 = Level.Instance.IsBanned(text2);
			Transform transform = slot.transform;
			((Component)transform.Find("txt_name")).GetComponent<UILabel>().text = text2;
			((Component)transform.Find("txt_right")).GetComponent<UILabel>().text = ((!flag) ? string.Empty : Localize.GetText("TAB_MENU_YOU"));
			Transform val = transform.Find("checkbox");
			((Component)val).gameObject.SetActive(true);
			if (!flag2 && !flag4)
			{
				((Component)val.Find("administrator")).gameObject.SetActive(false);
				((Component)val.Find("banned")).gameObject.SetActive(false);
				UICheckbox componentInChildren = ((Component)val.Find("ckb_moderator")).GetComponentInChildren<UICheckbox>();
				UICheckbox componentInChildren2 = ((Component)val.Find("ckb_builder")).GetComponentInChildren<UICheckbox>();
				componentInChildren.isChecked = flag3;
				componentInChildren2.isChecked = isChecked;
				((Behaviour)componentInChildren).enabled = isAdministrator;
				((Behaviour)componentInChildren2).enabled = isAdministrator || isModerator;
			}
			else
			{
				((Component)val.Find("ckb_moderator")).gameObject.SetActive(false);
				((Component)val.Find("ckb_builder")).gameObject.SetActive(false);
				((Component)val.Find("administrator")).gameObject.SetActive(flag2);
				((Component)val.Find("banned")).gameObject.SetActive(flag4);
			}
			Transform val2 = transform.Find("button_ban");
			Transform val3 = transform.Find("button_unban");
			if (((isAdministrator && !flag2) || (isModerator && !flag2 && !flag3)) && !flag)
			{
				if (flag4)
				{
					((Component)val2).gameObject.SetActive(false);
				}
				else
				{
					((Component)val3).gameObject.SetActive(false);
				}
			}
			else
			{
				((Component)val2).gameObject.SetActive(false);
				((Component)val3).gameObject.SetActive(false);
			}
			Transform val4 = transform.Find("level");
			((Component)val4).gameObject.SetActive(false);
		}, "tab_menu");
	}

	public void RefreshShop()
	{
		if (!((Component)KGUI.FindNode("main_menu_shop")).gameObject.activeInHierarchy)
		{
			return;
		}
		KGUI.SetControlText("main_menu_shop.icons.txt_title", Localize.GetText("STORE_" + _ShopTab.ToString().ToUpper()));
		string[] tabTypeButtonNames = Store.TabTypeButtonNames;
		foreach (string text in tabTypeButtonNames)
		{
			if ((Object)(object)KGUI.FindNode("main_menu_shop" + text, mayBeNull: true) != (Object)null)
			{
				KGUI.EnableNodes("main_menu_shop" + text, !text.Contains("ibtn_blocks_"));
			}
		}
		((Component)KGUI.FindNode("main_menu_shop.grid_icons")).GetComponent<UITable>().Reposition();
		_CurShopPurchases = new Dictionary<string, List<StorePurchase>>();
		foreach (KeyValuePair<StorePurchase, Store.PurchaseInfo> purchase in Store.Purchases)
		{
			if (!purchase.Value.Disabled && (Store.TabType)purchase.Value.Tab == _ShopTab)
			{
				if (!_CurShopPurchases.ContainsKey(purchase.Value.Category))
				{
					_CurShopPurchases.Add(purchase.Value.Category, new List<StorePurchase>());
				}
				_CurShopPurchases[purchase.Value.Category].Add(purchase.Key);
			}
		}
		KGUI.ResizeGrid("main_menu_shop.grid_items_categories", _CurShopPurchases.Count, (GameObject slot, int index) =>
		{
			List<StorePurchase> purchases = _CurShopPurchases[_CurShopPurchases.Keys.ElementAt(index)];
			KGUI.ResizeGrid("main_menu_shop.grid_items_categories." + index + ".grid_items", purchases.Count, (GameObject val, int index2) =>
			{
				KGUI.SetControlSprite(val.transform.Find("item_image"), Store.Purchases[purchases[index2]].Icon, 60f);
			});
			if (_CurShopPurchases.Count <= 1)
			{
				((Component)slot.transform.Find("bookmark")).gameObject.SetActive(false);
			}
			else
			{
				((Component)slot.transform.Find("bookmark").Find("txt_title")).GetComponent<UILabel>().text = Localize.GetText(Store.Purchases[purchases[0]].Category);
			}
		}, "main_menu_shop_" + _ShopTab);
		((MonoBehaviour)this).StartCoroutine(RefreshCategoriesProcess("main_menu_shop.grid_items_categories"));
	}

	public IEnumerator RefreshCategoriesProcess(string nodeName)
	{
		yield return 0;
		((Component)KGUI.FindNode(nodeName)).GetComponent<UITable>().repositionNow = true;
	}

	public void RefreshInventory()
	{
		if (!((Component)KGUI.FindNode("inventory")).gameObject.activeInHierarchy)
		{
			return;
		}
		KGUI.SetControlText("inventory.icons.txt_title", Localize.GetText("STORE_" + _InventoryTab.ToString().ToUpper()));
		string[] tabTypeButtonNames = Store.TabTypeButtonNames;
		foreach (string text in tabTypeButtonNames)
		{
			if ((Object)(object)KGUI.FindNode("inventory" + text, mayBeNull: true) != (Object)null)
			{
				if (_InventoryTab.ToString().Contains("Blocks_"))
				{
					KGUI.EnableNodes("inventory" + text, text.Contains("ibtn_blocks_"));
				}
				else
				{
					KGUI.EnableNodes("inventory" + text, !text.Contains("ibtn_blocks_") && !text.Contains("ibtn_maps") && !text.Contains("ibtn_skins") && !text.Contains("ibtn_weapons") && !text.Contains("ibtn_tools") && !text.Contains("ibtn_bonuses"));
				}
			}
		}
		((Component)KGUI.FindNode("inventory.grid_icons")).GetComponent<UITable>().Reposition();
		if (!_InventoryTab.ToString().Contains("Blocks"))
		{
			_CurInventoryItems.Clear();
			foreach (KeyValuePair<EntityType, Store.EntityInfo> entity in Store.Entities)
			{
				if ((Store.TabType)entity.Value.Tab == _InventoryTab && GetInventoryItemCount(entity.Key) != 0 && (entity.Value.Validator == null || entity.Value.Validator()))
				{
					if (!_CurInventoryItems.ContainsKey(entity.Value.Category))
					{
						_CurInventoryItems.Add(entity.Value.Category, new List<EntityType>());
					}
					_CurInventoryItems[entity.Value.Category].Add(entity.Key);
				}
			}
			KGUI.ResizeGrid("inventory.grid_items_categories", _CurInventoryItems.Count, (GameObject slot, int index) =>
			{
				List<EntityType> items = _CurInventoryItems[_CurInventoryItems.Keys.ElementAt(index)];
				KGUI.ResizeGrid("inventory.grid_items_categories." + index + ".grid_items", items.Count, (GameObject val, int index2) =>
				{
					EntityType entityType = items[index2];
					int inventoryItemCount = GetInventoryItemCount(entityType);
					KGUI.SetControlSprite(val.transform.Find("item_image"), Store.Entities[entityType].SpriteName, 60f);
					((Component)val.transform.Find("txt_count")).GetComponent<UILabel>().text = string.Empty;
					if (inventoryItemCount != -1)
					{
						Store.Pay cost = Store.Purchases[Store.Entities[entityType].Purchase].Cost;
						if (cost is Store.OnePay && !((Store.OnePay)cost).Once)
						{
							((Component)val.transform.Find("txt_count")).GetComponent<UILabel>().text = string.Empty + inventoryItemCount;
						}
					}
				});
				if (_CurInventoryItems.Count <= 1)
				{
					((Component)slot.transform.Find("bookmark")).gameObject.SetActive(false);
				}
				else
				{
					((Component)slot.transform.Find("bookmark").Find("txt_title")).GetComponent<UILabel>().text = Localize.GetText(Store.Entities[items[0]].Category);
				}
			}, "inventory_" + _InventoryTab);
			((MonoBehaviour)this).StartCoroutine(RefreshCategoriesProcess("inventory.grid_items_categories"));
			return;
		}
		_CurInventoryBlocks.Clear();
		foreach (KeyValuePair<BlockType, Store.BlockInfo> block in Store.Blocks)
		{
			if ((Store.TabType)block.Value.Tab == _InventoryTab && GetInventoryBlockCount(block.Key) != 0 && (block.Value.Validator == null || block.Value.Validator()))
			{
				if (!_CurInventoryBlocks.ContainsKey(block.Value.Category))
				{
					_CurInventoryBlocks.Add(block.Value.Category, new List<BlockType>());
				}
				_CurInventoryBlocks[block.Value.Category].Add(block.Key);
			}
		}
		KGUI.ResizeGrid("inventory.grid_items_categories", _CurInventoryBlocks.Count, (GameObject slot, int index) =>
		{
			List<BlockType> blocks = _CurInventoryBlocks[_CurInventoryBlocks.Keys.ElementAt(index)];
			KGUI.ResizeGrid("inventory.grid_items_categories." + index + ".grid_items", blocks.Count, (GameObject val, int index2) =>
			{
				BlockType key = blocks[index2];
				KGUI.SetControlSprite(val.transform.Find("item_image"), Store.Blocks[key].SpriteName, 60f);
				((Component)val.transform.Find("txt_count")).GetComponent<UILabel>().text = string.Empty;
			});
			if (_CurInventoryBlocks.Count <= 1)
			{
				((Component)slot.transform.Find("bookmark")).gameObject.SetActive(false);
			}
			else
			{
				((Component)slot.transform.Find("bookmark").Find("txt_title")).GetComponent<UILabel>().text = Localize.GetText(Store.Blocks[blocks[0]].Category);
			}
		}, "inventory_" + _InventoryTab);
		((MonoBehaviour)this).StartCoroutine(RefreshCategoriesProcess("inventory.grid_items_categories"));
	}

	private int GetInventoryItemCount(EntityType entityType)
	{
		Store.EntityInfo entityInfo = Store.Entities[entityType];
		if ((StorePurchase)entityInfo.Purchase == StorePurchase.NONE)
		{
			return -1;
		}
		return Mathf.Max(ProfileINI.GetPurchaseValue(entityInfo.Purchase), 0);
	}

	private int GetInventoryBlockCount(BlockType blockType)
	{
		Store.BlockInfo blockInfo = Store.Blocks[blockType];
		if ((StorePurchase)blockInfo.Purchase == StorePurchase.NONE)
		{
			return -1;
		}
		return Mathf.Max(ProfileINI.GetPurchaseValue(blockInfo.Purchase), 0);
	}

	public void RefreshBattleWeapon()
	{
		PlayerNetwork player = WorldGameObject.Instance.MainPlayer.GetComponent<PlayerNetwork>();
		List<Gun> weapons = new List<Gun>();
		weapons.Add(player.Knife);
		weapons.AddRange(player.MainWeapon);
		if (player.Grenade.bullets > 0)
		{
			weapons.Add(player.Grenade);
		}
		KGUI.EnableNodes("hud.battle.bullets", enable: false, onlyChild: true);
		KGUI.EnableNodes("hud.battle.txt_bullets", enable: false);
		KGUI.ResizeGrid("hud.battle.grid_weapons", weapons.Count, (GameObject slot, int index) =>
		{
			Gun gun = weapons[index];
			KGUI.EnableNodes("hud.battle.grid_weapons." + index + ".numbers", enable: false, onlyChild: true);
			KGUI.EnableNodes("hud.battle.grid_weapons." + index + ".numbers." + (index + 1), enable: true);
			KGUI.EnableNodes("hud.battle.grid_weapons." + index + ".weapons", enable: false, onlyChild: true);
			if ((Object)(object)gun != (Object)null)
			{
				KGUI.EnableNodes("hud.battle.grid_weapons." + index + ".weapons." + ((Object)gun).name, enable: true);
			}
			if ((Object)(object)gun != (Object)null && (Object)(object)player.CurGun == (Object)(object)gun)
			{
				if (!gun.melee)
				{
					KGUI.EnableNodes("hud.battle.bullets." + ((!((Object)(object)gun.bulletIcon != (Object)null)) ? "bullet_2" : ((Object)gun.bulletIcon).name), enable: true);
					KGUI.EnableNodes("hud.battle.txt_bullets", enable: true);
					KGUI.SetControlText("hud.battle.txt_bullets", gun.bullets + "/" + gun.bulletsLeft);
				}
			}
			else
			{
				KGUI.EnableNodes("hud.battle.grid_weapons." + index + ".selected", enable: false);
			}
		});
		if (WorldGameObject.Instance.MainPlayerNode.IsZombie)
		{
			KGUI.EnableNodes("hud.battle.grid_weapons", enable: false);
		}
	}

	private void ChangeMapSize(bool next)
	{
		int newMapSize = _MapSize;
		KeyValuePair<StorePurchase, Store.PurchaseInfo> keyValuePair;
		do
		{
			newMapSize += (next ? 1 : (-1));
			if (newMapSize < 0 || newMapSize >= _MapSizes.Length)
			{
				return;
			}
			keyValuePair = Store.Purchases.FirstOrDefault((KeyValuePair<StorePurchase, Store.PurchaseInfo> p) => p.Value.Data != null && p.Value.Data.Value is GameINI.MapSize && (int)p.Value.Data.Value == (int)_MapSizes[newMapSize]);
		}
		while (keyValuePair.Value != null && ProfileINI.GetPurchaseValue(keyValuePair.Key) <= 0);
		_MapSize = newMapSize;
		KGUI.SetControlText("main_menu_my_maps.page2.info.map_size.txt_type", Localize.GetText("MAP_SIZE_" + _MapSizes[_MapSize]));
	}

	private void ChangeMapType(bool next)
	{
		int newMapType = _MapType;
		KeyValuePair<StorePurchase, Store.PurchaseInfo> keyValuePair;
		do
		{
			newMapType += (next ? 1 : (-1));
			if (newMapType < 0 || newMapType >= _MapTypes.Length)
			{
				return;
			}
			keyValuePair = Store.Purchases.FirstOrDefault((KeyValuePair<StorePurchase, Store.PurchaseInfo> p) => p.Value.Data != null && p.Value.Data.Value is GameINI.MapType && (int)p.Value.Data.Value == (int)_MapTypes[newMapType]);
		}
		while (keyValuePair.Value != null && ProfileINI.GetPurchaseValue(keyValuePair.Key) <= 0);
		_MapType = newMapType;
		KGUI.SetControlText("main_menu_my_maps.page2.info.map_type.txt_type", Localize.GetText("MAP_TYPE_" + _MapTypes[_MapType]));
	}

	private void ChangeMapTime(bool next)
	{
		int num = _MapTime + (next ? 1 : (-1));
		if (num >= 0 && num < _MapTimes.Length)
		{
			_MapTime = num;
			KGUI.SetControlText("main_menu_my_maps.page2.info.map_time.txt_type", Localize.GetText("MAP_TIME_" + _MapTimes[_MapTime]));
		}
	}

	private void RefreshNews()
	{
		KGUI.EnableNodes("main_menu_start.clip_news", enable: false);
		if (_NewsText == null || _NewsText.Length <= 0)
		{
			return;
		}
		KGUI.EnableNodes("main_menu_start.clip_news", enable: true);
		KGUI.SetControlText("main_menu_start.clip_news.label", _NewsText[_CurNewsBlock]);
		((Component)KGUI.FindNode("main_menu_start.clip_news.screen")).GetComponent<UITexture>().mainTexture = (Texture)(object)_NewsPicture[_CurNewsBlock];
		((Component)KGUI.FindNode("main_menu_start.clip_news.screen")).GetComponent<UITexture>().MakePixelPerfect();
		if (_NewsButtonText[_CurNewsBlock].Length > 0)
		{
			KGUI.SetControlText("main_menu_start.clip_news.look_button.txt_title", _NewsButtonText[_CurNewsBlock]);
		}
		else
		{
			KGUI.EnableNodes("main_menu_start.clip_news.look_button", enable: false);
		}
		for (int i = 0; i < 4; i++)
		{
			if (i != _CurNewsBlock)
			{
				KGUI.EnableNodes("main_menu_start.clip_news.bookmark" + (i + 1) + ".selected", enable: false);
			}
		}
		for (int j = 0; j < 4; j++)
		{
			if (j >= _NewsText.Length)
			{
				KGUI.EnableNodes("main_menu_start.clip_news.bookmark" + (j + 1), enable: false);
			}
		}
		((MonoBehaviour)this).CancelInvoke("NextNews");
		if (_NewsAutoScroll)
		{
			((MonoBehaviour)this).Invoke("NextNews", 5f);
		}
	}

	private void NextNews()
	{
		if (_NewsText != null && _NewsText.Length > 0)
		{
			_CurNewsBlock++;
			if (_CurNewsBlock >= _NewsText.Length)
			{
				_CurNewsBlock = 0;
			}
			RefreshNews();
		}
	}

	public void FacebookPaymentWindow(int price)
	{
		Debug.Log((object)"FacebookPaymentWindow");
		Debug.Log((object)price);
		Application.ExternalEval("FB.ui({\r\n\t\t\t\tmethod: 'pay',\r\n\t\t\t\taction: 'purchaseitem',\r\n\t\t\t\tproduct: 'http://diggerworld.ru/facebook/gooods/crys_" + price + ".html'\r\n\t\t\t},OnItemBuy);\r\n\t\t\t\t\tfunction OnItemBuy(data) {\r\n                    if (data.signed_request) {\r\n                     UnitySend('PaymentFB', '' + data.signed_request);\r\n                    }\r\n                }\r\n                ");
	}

	private void RefreshStartFastGame(int scroll)
	{
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Expected Obj, but got Unknown
		_FastGameType += scroll;
		_FastGameType = Mathf.Clamp(_FastGameType, -1, _MapGameTypes.Length - 1);
		_FastGameMaps = new List<Store.StandartMap>();
		if (_FastGameType != -1)
		{
			SecuredValue<Store.StandartMap>[] standartMaps = Store.StandartMaps;
			foreach (SecuredValue<Store.StandartMap> securedValue in standartMaps)
			{
				if ((GameINI.GameType)securedValue.Value.Game == _MapGameTypes[_FastGameType])
				{
					_FastGameMaps.Add(securedValue.Value);
				}
			}
		}
		KGUI.SetControlText("main_menu_start.page1_fast_game.button_play.txt_title", Localize.GetText((_FastGameMaps.Count != 0) ? "START_NEXT" : "START_PLAY"));
		KGUI.EnableNodes("main_menu_start.page1_fast_game.game_types", enable: false);
		KGUI.EnableNodes("main_menu_start.page1_fast_game.game_types", enable: true);
		foreach (Transform item in KGUI.FindNode("main_menu_start.page1_fast_game.game_types"))
		{
			Transform val = item;
			((Component)val).gameObject.SetActive(_FastGameType != -1 && ((Object)val).name == _MapGameTypes[_FastGameType].ToString().ToLower());
		}
		if (_FastGameType == -1)
		{
			KGUI.EnableNodes("main_menu_start.page1_fast_game.game_types.random", enable: true);
		}
		if (_FastGameType != -1)
		{
			KGUI.SetControlText("main_menu_start.page1_fast_game.txt_game_type", Localize.GetText("GAME_TYPE_" + _MapGameTypes[_FastGameType]));
			KGUI.SetControlText("main_menu_start.page1_fast_game.txt_description", Localize.GetText("START_DESCRIPTION_" + _MapGameTypes[_FastGameType]));
		}
		else
		{
			KGUI.SetControlText("main_menu_start.page1_fast_game.txt_game_type", Localize.GetText("GAME_TYPE_RANDOM"));
			KGUI.SetControlText("main_menu_start.page1_fast_game.txt_description", Localize.GetText("START_DESCRIPTION_RANDOM"));
		}
	}

	private void RefreshStartSelectMap(int scroll)
	{
		_FastGameMap += scroll;
		_FastGameMap = Mathf.Clamp(_FastGameMap, 0, _FastGameMaps.Count - 1);
		Store.StandartMap standartMap = _FastGameMaps[_FastGameMap];
		KGUI.SetControlText("main_menu_start.page1_select_map.txt_map_name", Localize.GetText(standartMap.Name));
		KGUI.SetControlText("main_menu_start.page1_select_map.txt_map_description", Localize.GetText(standartMap.Description));
		KGUI.SetControlSprite(KGUI.FindNode("main_menu_start.page1_select_map.map_image"), standartMap.SpriteName, 0f);
	}

	private IEnumerator BattleResultProcess(int score)
	{
		bool isPremium = ProfileINI.GetPurchaseValue(StorePurchase.MORE_EXPERIENCE) != 0;
		int premiumScore = 0;
		if (isPremium)
		{
			premiumScore = score * 50 / 100;
		}
		int curExp = ProfileINI.experience;
		int oldExp = curExp;
		ProfileINI.experience = (int)ProfileINI.experience + (score + premiumScore);
		int actualExp = ProfileINI.experience;
		if (ProfileINI.LevelExp(oldExp) != ProfileINI.LevelExp(actualExp))
		{
			SetMenu(Menu.LevelUp, show: true);
		}
		while (true)
		{
			int level = ProfileINI.LevelExp(curExp);
			int expForCurLevel = ProfileINI.ExpForLevel(level);
			int expForNextLevel = ProfileINI.ExpForLevel(level + 1);
			KGUI.SetControlText("battle_result.txt_exp", string.Empty + Mathf.Min(curExp - oldExp, score));
			KGUI.SetControlText("battle_result.txt_exp_premium", string.Empty + Mathf.Max(curExp - oldExp - score, 0));
			KGUI.SetControlText("battle_result.txt_level_exp", curExp + " / " + expForNextLevel);
			KGUI.SetControlSprite(KGUI.FindNode("battle_result.cur_level"), "level_" + level, 0f);
			KGUI.SetControlSprite(KGUI.FindNode("battle_result.next_level"), "level_" + Mathf.Min(level + 1, 30), 0f);
			((Component)KGUI.FindNode("battle_result.bar_cur")).GetComponent<UISprite>().fillAmount = (float)(curExp - expForCurLevel) / (float)(expForNextLevel - expForCurLevel);
			if (curExp == actualExp)
			{
				break;
			}
			yield return (object)new WaitForSeconds(0.05f);
			curExp += level;
			if (curExp > actualExp)
			{
				curExp = actualExp;
			}
		}
	}
}
