using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Store
{
	public enum TabType
	{
		Blocks,
		Blocks_Wood,
		Blocks_Stone,
		Blocks_Nature,
		Blocks_Colored,
		Blocks_Glass,
		Blocks_Cell,
		Blocks_Tile,
		Blocks_Sets,
		Pets,
		Maps,
		Skins,
		Plants,
		Weapons,
		Decorations,
		Tools,
		Bonuses,
		Untagged
	}

	public class EntityInfo
	{
		public SecuredValue<StorePurchase> Purchase;

		public SecuredValue<string> SpriteName;

		public SecuredValue<TabType> Tab;

		public SecuredValue<string> Category = string.Empty;

		public Func<bool> Validator;
	}

	public class BlockInfo
	{
		public SecuredValue<StorePurchase> Purchase;

		public SecuredValue<string> SpriteName;

		public SecuredValue<TabType> Tab;

		public SecuredValue<string> Category = string.Empty;

		public Func<bool> Validator;
	}

	public class PurchaseInfo
	{
		public bool Disabled;

		public Pay Cost;

		public SecuredValue<string> Name;

		public SecuredValue<string> Icon;

		public SecuredValue<string> LargeIcon;

		public SecuredValue<TabType> Tab;

		public SecuredValue<string> Category = string.Empty;

		public SecuredValue<int> Skin;

		public SecuredValue<object> Data;

		public SecuredValue<StorePurchase> RequiredPurchase = StorePurchase.NONE;
	}

	public class Pay
	{
	}

	public class OnePay : Pay
	{
		public SecuredValue<int> Coins;

		public SecuredValue<int> Gems;

		public SecuredValue<bool> Once;

		public SecuredValue<int> Count;

		public OnePay(int coins, int gems, bool once, int count = 1)
		{
			Coins = coins;
			Gems = gems;
			Once = once;
			Count = count;
			if (coins == 0 == (gems == 0) || coins < 0 || gems < 0 || count <= 0 || (once && count != 1))
			{
				Debug.LogError((object)"Invalid pay data");
			}
		}
	}

	public class TimedPay : Pay
	{
		public SecuredValue<int> DayCoins;

		public SecuredValue<int> WeekCoins;

		public SecuredValue<int> ForeverCoins;

		public SecuredValue<int> MonthCoins;

		public SecuredValue<int> DayGems;

		public SecuredValue<int> WeekGems;

		public SecuredValue<int> ForeverGems;

		public SecuredValue<int> MonthGems;

		public TimedPay(int dayCoins, int weekCoins, int foreverCoins, int dayGems, int weekGems, int foreverGems, int monthCoins = 0, int monthGems = 0)
		{
			DayCoins = dayCoins;
			WeekCoins = weekCoins;
			MonthCoins = monthCoins;
			ForeverCoins = foreverCoins;
			DayGems = dayGems;
			WeekGems = weekGems;
			MonthGems = monthGems;
			ForeverGems = foreverGems;
			if ((dayCoins != 0 && dayGems != 0) || (weekCoins != 0 && weekGems != 0) || (foreverCoins != 0 && foreverGems != 0) || (monthCoins != 0 && monthGems != 0) || dayCoins < 0 || weekCoins < 0 || foreverCoins < 0 || monthCoins < 0 || dayGems < 0 || weekGems < 0 || foreverGems < 0 || monthGems < 0 || (monthCoins != 0 && foreverCoins != 0) || (monthGems != 0 && foreverGems != 0))
			{
				Debug.LogError((object)"Invalid pay data");
			}
		}
	}

	public class StandartMap
	{
		public SecuredValue<GameINI.GameType> Game;

		public SecuredValue<bool> Destroyable;

		public SecuredValue<string> Name;

		public SecuredValue<string> Description;

		public SecuredValue<string> SpriteName;

		public SecuredValue<string> MapURL;
	}

	public static string[] TabTypeButtonNames;

	public static Dictionary<EntityType, EntityInfo> Entities;

	public static Dictionary<BlockType, BlockInfo> Blocks;

	public static Dictionary<StorePurchase, PurchaseInfo> Purchases;

	public static SecuredValue<int>[][] Gems;

	public static SecuredValue<int>[] Gold;

	public static SecuredValue<StandartMap>[] StandartMaps;

	static Store()
	{
		TabTypeButtonNames = new string[17]
		{
			".icons.ibtn_blocks", ".icons.ibtn_blocks_wood", ".icons.ibtn_blocks_stone", ".icons.ibtn_blocks_nature", ".icons.ibtn_blocks_colored", ".icons.ibtn_blocks_glass", ".icons.ibtn_blocks_cell", ".icons.ibtn_blocks_tile", ".icons.ibtn_blocks_sets", ".icons.ibtn_pets",
			".icons.ibtn_maps", ".icons.ibtn_skins", ".icons.ibtn_plants", ".icons.ibtn_weapons", ".icons.ibtn_decorations", ".icons.ibtn_tools", ".icons.ibtn_bonuses"
		};
		Entities = new Dictionary<EntityType, EntityInfo>
		{
			{
				EntityType.FISH_1,
				new EntityInfo
				{
					Purchase = StorePurchase.FISH_1,
					SpriteName = "fish_1",
					Tab = TabType.Pets
				}
			},
			{
				EntityType.FISH_2,
				new EntityInfo
				{
					Purchase = StorePurchase.FISH_2,
					SpriteName = "fish_2",
					Tab = TabType.Pets
				}
			},
			{
				EntityType.FISH_3,
				new EntityInfo
				{
					Purchase = StorePurchase.FISH_3,
					SpriteName = "fish_3",
					Tab = TabType.Pets
				}
			},
			{
				EntityType.CAT,
				new EntityInfo
				{
					Purchase = StorePurchase.CAT,
					SpriteName = "cat",
					Tab = TabType.Pets
				}
			},
			{
				EntityType.DOG,
				new EntityInfo
				{
					Purchase = StorePurchase.DOG,
					SpriteName = "dog",
					Tab = TabType.Pets
				}
			},
			{
				EntityType.CHICKEN,
				new EntityInfo
				{
					Purchase = StorePurchase.CHICKEN,
					SpriteName = "chicken_pet",
					Tab = TabType.Pets
				}
			},
			{
				EntityType.BOAR,
				new EntityInfo
				{
					Purchase = StorePurchase.BOAR,
					SpriteName = "pet_01_icon",
					Tab = TabType.Pets
				}
			},
			{
				EntityType.CRAB,
				new EntityInfo
				{
					Purchase = StorePurchase.CRAB,
					SpriteName = "pet_02_icon",
					Tab = TabType.Pets
				}
			},
			{
				EntityType.FLOWER_BLUE,
				new EntityInfo
				{
					Purchase = StorePurchase.FLOWER,
					SpriteName = "flower_bell_blue",
					Tab = TabType.Plants
				}
			},
			{
				EntityType.FLOWER_RED,
				new EntityInfo
				{
					Purchase = StorePurchase.FLOWER,
					SpriteName = "flower_bell_red",
					Tab = TabType.Plants
				}
			},
			{
				EntityType.FLOWER_WHITE,
				new EntityInfo
				{
					Purchase = StorePurchase.FLOWER,
					SpriteName = "flower_bell_white",
					Tab = TabType.Plants
				}
			},
			{
				EntityType.FLOWER_YELLOW,
				new EntityInfo
				{
					Purchase = StorePurchase.FLOWER,
					SpriteName = "flower_bell_yellow",
					Tab = TabType.Plants
				}
			},
			{
				EntityType.FLOWER_PERPLE,
				new EntityInfo
				{
					Purchase = StorePurchase.FLOWER2,
					SpriteName = "flower_purple",
					Tab = TabType.Plants
				}
			},
			{
				EntityType.BLUEBERY,
				new EntityInfo
				{
					Purchase = StorePurchase.FLOWER2,
					SpriteName = "blueberry",
					Tab = TabType.Plants
				}
			},
			{
				EntityType.STROBERY,
				new EntityInfo
				{
					Purchase = StorePurchase.FLOWER2,
					SpriteName = "strawberry",
					Tab = TabType.Plants
				}
			},
			{
				EntityType.CAMMOLINE,
				new EntityInfo
				{
					Purchase = StorePurchase.FLOWER2,
					SpriteName = "cammoline",
					Tab = TabType.Plants
				}
			},
			{
				EntityType.BELLSMALL,
				new EntityInfo
				{
					Purchase = StorePurchase.FLOWER2,
					SpriteName = "flower_bell_small",
					Tab = TabType.Plants
				}
			},
			{
				EntityType.FERN,
				new EntityInfo
				{
					Purchase = StorePurchase.FLOWER3,
					SpriteName = "Fern",
					Tab = TabType.Plants
				}
			},
			{
				EntityType.WEAT,
				new EntityInfo
				{
					Purchase = StorePurchase.FLOWER3,
					SpriteName = "wheet",
					Tab = TabType.Plants
				}
			},
			{
				EntityType.SHROOM1,
				new EntityInfo
				{
					Purchase = StorePurchase.FLOWER3,
					SpriteName = "shrum1",
					Tab = TabType.Plants
				}
			},
			{
				EntityType.SHROOM2,
				new EntityInfo
				{
					Purchase = StorePurchase.FLOWER3,
					SpriteName = "shrum2",
					Tab = TabType.Plants
				}
			},
			{
				EntityType.GRASS,
				new EntityInfo
				{
					Purchase = StorePurchase.FLOWER3,
					SpriteName = "grass",
					Tab = TabType.Plants
				}
			},
			{
				EntityType.GIFT_AMMO,
				new EntityInfo
				{
					Purchase = StorePurchase.BONUS_WEAPONS,
					SpriteName = "ico_box_ammo_rifle",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.GIFT_ARMOR,
				new EntityInfo
				{
					Purchase = StorePurchase.BONUS_ARMOR,
					SpriteName = "ico_box_ammo_armor",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.GIFT_GRENADE,
				new EntityInfo
				{
					Purchase = StorePurchase.BOUNS_GRANADE,
					SpriteName = "ico_box_ammo_grenade",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.GIFT_LIFE,
				new EntityInfo
				{
					Purchase = StorePurchase.BONUS_LIFE,
					SpriteName = "ico_box_ammo_health",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.TNT,
				new EntityInfo
				{
					Purchase = StorePurchase.DINAMIT,
					SpriteName = "TNT_s",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.ATOMBOMB,
				new EntityInfo
				{
					Purchase = StorePurchase.ATOMBOMB,
					SpriteName = "BOMB_s",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.BOAT,
				new EntityInfo
				{
					Purchase = StorePurchase.BOAT,
					SpriteName = "boat",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.TROLLEY,
				new EntityInfo
				{
					Purchase = StorePurchase.TROLLEY,
					SpriteName = "lorry",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.RAIL,
				new EntityInfo
				{
					Purchase = StorePurchase.RAIL,
					SpriteName = "rails",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.SPAWN_ARROW,
				new EntityInfo
				{
					Purchase = StorePurchase.SPAWN_POINT,
					SpriteName = "checkpoint_flag_icon_01",
					Tab = TabType.Decorations,
					Validator = () => Level.Instance.IsAdministrator()
				}
			},
			{
				EntityType.TEAM_SPAWN_ARROW_BLUE,
				new EntityInfo
				{
					Purchase = StorePurchase.TEAM_SPAWN_POINT,
					SpriteName = "spawn_arrow_blue",
					Tab = TabType.Decorations,
					Validator = () => Level.Instance.IsAdministrator()
				}
			},
			{
				EntityType.TEAM_SPAWN_ARROW_RED,
				new EntityInfo
				{
					Purchase = StorePurchase.TEAM_SPAWN_POINT,
					SpriteName = "spawn_arrow_red",
					Tab = TabType.Decorations,
					Validator = () => Level.Instance.IsAdministrator()
				}
			},
			{
				EntityType.GOLD_KUBOK,
				new EntityInfo
				{
					Purchase = StorePurchase.GOLD_KUBOK,
					SpriteName = "gold_cubok",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.CHAIR,
				new EntityInfo
				{
					Purchase = StorePurchase.STOOL,
					SpriteName = "chair_wood",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.CHAIR_STONE,
				new EntityInfo
				{
					Purchase = StorePurchase.CHAIR_STONE,
					SpriteName = "chair_stone",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.TABLE,
				new EntityInfo
				{
					Purchase = StorePurchase.TABLE,
					SpriteName = "table_02_wood",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.TABLE_STONE,
				new EntityInfo
				{
					Purchase = StorePurchase.TABLE_STONE,
					SpriteName = "table_02_stone",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.BIG_TABLE,
				new EntityInfo
				{
					Purchase = StorePurchase.BIG_TABLE,
					SpriteName = "table_01_wood",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.BUCKET,
				new EntityInfo
				{
					Purchase = StorePurchase.BACKET,
					SpriteName = "bucket",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.BASKET,
				new EntityInfo
				{
					Purchase = StorePurchase.BASKET,
					SpriteName = "basket",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.BARREL,
				new EntityInfo
				{
					Purchase = StorePurchase.BARREL,
					SpriteName = "barrel",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.FENCE_H,
				new EntityInfo
				{
					Purchase = StorePurchase.FENCE,
					SpriteName = "fence_(X)_wood",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.FENCE_I,
				new EntityInfo
				{
					Purchase = StorePurchase.FENCE,
					SpriteName = "fence_(tile)_wood",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.FENCE_L,
				new EntityInfo
				{
					Purchase = StorePurchase.FENCE,
					SpriteName = "fence_(corner)_wood",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.FENCE_T,
				new EntityInfo
				{
					Purchase = StorePurchase.FENCE,
					SpriteName = "fence_(T)_wood",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.FENCE_STONE_X,
				new EntityInfo
				{
					Purchase = StorePurchase.FENCE_STONE,
					SpriteName = "fence_(X)_stone",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.FENCE_STONE_I,
				new EntityInfo
				{
					Purchase = StorePurchase.FENCE_STONE,
					SpriteName = "fence_(tile)_stone",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.FENCE_STONE_L,
				new EntityInfo
				{
					Purchase = StorePurchase.FENCE_STONE,
					SpriteName = "fence_(corner)_stone",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.FENCE_STONE_T,
				new EntityInfo
				{
					Purchase = StorePurchase.FENCE_STONE,
					SpriteName = "fence_(T)_stone",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.CHEST,
				new EntityInfo
				{
					Purchase = StorePurchase.CHEST,
					SpriteName = "chest_02",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.CHEST_STONE,
				new EntityInfo
				{
					Purchase = StorePurchase.CHEST_STONE,
					SpriteName = "chest_01",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.SIGN,
				new EntityInfo
				{
					Purchase = StorePurchase.SIGN,
					SpriteName = "sign",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.BED,
				new EntityInfo
				{
					Purchase = StorePurchase.BED,
					SpriteName = "bed_wood",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.BED_STONE,
				new EntityInfo
				{
					Purchase = StorePurchase.BED_STONE,
					SpriteName = "bed_stone",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.CUBOARDS,
				new EntityInfo
				{
					Purchase = StorePurchase.CUPBOARDS,
					SpriteName = "case_wood",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.CUPBOARDL,
				new EntityInfo
				{
					Purchase = StorePurchase.CUPBOARDL,
					SpriteName = "big_case_wood",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.STAIRSW,
				new EntityInfo
				{
					Purchase = StorePurchase.STAIRSW,
					SpriteName = "stairs_w",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.STAIRSC,
				new EntityInfo
				{
					Purchase = StorePurchase.STAIRSB,
					SpriteName = "stairs_s",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.DOORW,
				new EntityInfo
				{
					Purchase = StorePurchase.DOORW,
					SpriteName = "door_wood",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.DOORM,
				new EntityInfo
				{
					Purchase = StorePurchase.DOORM,
					SpriteName = "door_wood",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.TABLICHKAF,
				new EntityInfo
				{
					Purchase = StorePurchase.TABLICHKA,
					SpriteName = "tablet_01",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.TABLICHKAW,
				new EntityInfo
				{
					Purchase = StorePurchase.TABLICHKAW,
					SpriteName = "tablet_02",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.TORCH,
				new EntityInfo
				{
					Purchase = StorePurchase.TORCHW,
					SpriteName = "torch_01_wood",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.TORCH_STONE,
				new EntityInfo
				{
					Purchase = StorePurchase.TORCH_STONE,
					SpriteName = "torch_02_stone",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.TORCH_FLOOR,
				new EntityInfo
				{
					Purchase = StorePurchase.TORCHF,
					SpriteName = "torch_02_wood",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.TORCH_FLOOR_STONE,
				new EntityInfo
				{
					Purchase = StorePurchase.TORCH_FLOOR_STONE,
					SpriteName = "torch_02_stone",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.LADDER,
				new EntityInfo
				{
					Purchase = StorePurchase.LADDER,
					SpriteName = "ladder",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.PAPER,
				new EntityInfo
				{
					Purchase = StorePurchase.PAPER,
					SpriteName = "paper_icon",
					Tab = TabType.Decorations,
					Validator = () => Level.Instance.IsAdministrator()
				}
			},
			{
				EntityType.PICTURE1_1,
				new EntityInfo
				{
					Purchase = StorePurchase.PICTURE1_1,
					SpriteName = "frame_icon1_1",
					Tab = TabType.Decorations,
					Validator = () => Level.Instance.IsAdministrator()
				}
			},
			{
				EntityType.PICTURE1_2,
				new EntityInfo
				{
					Purchase = StorePurchase.PICTURE1_2,
					SpriteName = "frame_icon1_2",
					Tab = TabType.Decorations,
					Validator = () => Level.Instance.IsAdministrator()
				}
			},
			{
				EntityType.PICTURE2_1,
				new EntityInfo
				{
					Purchase = StorePurchase.PICTURE2_1,
					SpriteName = "frame_icon2_1",
					Tab = TabType.Decorations,
					Validator = () => Level.Instance.IsAdministrator()
				}
			},
			{
				EntityType.PICTURE3_2,
				new EntityInfo
				{
					Purchase = StorePurchase.PICTURE3_2,
					SpriteName = "frame_icon3_2",
					Tab = TabType.Decorations,
					Validator = () => Level.Instance.IsAdministrator()
				}
			},
			{
				EntityType.BOOK,
				new EntityInfo
				{
					Purchase = StorePurchase.BOOK,
					SpriteName = "object_icon_01",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.BENCH_WOOD,
				new EntityInfo
				{
					Purchase = StorePurchase.BENCH_WOOD,
					SpriteName = "bench_wood",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.BENCH_STONE,
				new EntityInfo
				{
					Purchase = StorePurchase.BENCH_STONE,
					SpriteName = "bench_stone",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.TOMBSTONE_1,
				new EntityInfo
				{
					Purchase = StorePurchase.TOMBSTONE_1,
					SpriteName = "tombstone_01 (1)",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.TOMBSTONE_2,
				new EntityInfo
				{
					Purchase = StorePurchase.TOMBSTONE_2,
					SpriteName = "tombstone_02",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.FOOD_CAKE,
				new EntityInfo
				{
					Purchase = StorePurchase.FOOD_CAKE,
					SpriteName = "cake",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.FOOD_WOOD_PLATE,
				new EntityInfo
				{
					Purchase = StorePurchase.FOOD_WOOD_PLATE,
					SpriteName = "wood_plate",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.FOOD_SPOON,
				new EntityInfo
				{
					Purchase = StorePurchase.FOOD_SPOON,
					SpriteName = "spoon",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.FOOD_FORK,
				new EntityInfo
				{
					Purchase = StorePurchase.FOOD_FORK,
					SpriteName = "fork",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.FOOD_MUG,
				new EntityInfo
				{
					Purchase = StorePurchase.FOOD_MUG,
					SpriteName = "mug",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.FOOD_JUG,
				new EntityInfo
				{
					Purchase = StorePurchase.FOOD_JUG,
					SpriteName = "jug",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.FOOD_METAL_PLATE,
				new EntityInfo
				{
					Purchase = StorePurchase.FOOD_METAL_PLATE,
					SpriteName = "metal_plate",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.FOOD_BREAD,
				new EntityInfo
				{
					Purchase = StorePurchase.FOOD_BREAD,
					SpriteName = "bread",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.FOOD_CHEES,
				new EntityInfo
				{
					Purchase = StorePurchase.FOOD_CHEES,
					SpriteName = "cheese",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.FOOD_CHIKEN,
				new EntityInfo
				{
					Purchase = StorePurchase.FOOD_CHIKEN,
					SpriteName = "chicken",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.FOOD_SVININA,
				new EntityInfo
				{
					Purchase = StorePurchase.FOOD_SVININA,
					SpriteName = "pork",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.FOOD_FISH,
				new EntityInfo
				{
					Purchase = StorePurchase.FOOD_FISH,
					SpriteName = "fish",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.FOOD_STEIK,
				new EntityInfo
				{
					Purchase = StorePurchase.FOOD_STEIK,
					SpriteName = "meat",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.FOOD_ORANGE,
				new EntityInfo
				{
					Purchase = StorePurchase.FOOD_ORANGE,
					SpriteName = "orange",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.FOOD_PIZZA,
				new EntityInfo
				{
					Purchase = StorePurchase.FOOD_PIZZA,
					SpriteName = "pizza",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.FOOD_COLA,
				new EntityInfo
				{
					Purchase = StorePurchase.FOOD_COLA,
					SpriteName = "cola",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.FOOD_BURGER,
				new EntityInfo
				{
					Purchase = StorePurchase.FOOD_BURGER,
					SpriteName = "burger",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.FOOD_PIROG,
				new EntityInfo
				{
					Purchase = StorePurchase.FOOD_PIROG,
					SpriteName = "cheesecake",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.TEAM_DOOR_RED,
				new EntityInfo
				{
					Purchase = StorePurchase.TEAMDOORS,
					SpriteName = "double-doors_team_01_icon_01",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.TEAM_DOOR_BLUE,
				new EntityInfo
				{
					Purchase = StorePurchase.TEAMDOORS,
					SpriteName = "double-doors_team_02_icon_01",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.FLAG_RED,
				new EntityInfo
				{
					Purchase = StorePurchase.FLAG,
					SpriteName = "flag_01",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.FLAG_BLUE,
				new EntityInfo
				{
					Purchase = StorePurchase.FLAG,
					SpriteName = "flag_02",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.PAINTING_01,
				new EntityInfo
				{
					Purchase = StorePurchase.PAINTING_01,
					SpriteName = "painting_01",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.PAINTING_02,
				new EntityInfo
				{
					Purchase = StorePurchase.PAINTING_02,
					SpriteName = "painting_02",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.PAINTING_03,
				new EntityInfo
				{
					Purchase = StorePurchase.PAINTING_03,
					SpriteName = "painting_03",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.PAINTING_04,
				new EntityInfo
				{
					Purchase = StorePurchase.PAINTING_04,
					SpriteName = "painting_04",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.PAINTING_05,
				new EntityInfo
				{
					Purchase = StorePurchase.PAINTING_05,
					SpriteName = "painting_05",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.PAINTING_06,
				new EntityInfo
				{
					Purchase = StorePurchase.PAINTING_06,
					SpriteName = "painting_06",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.PAINTING_07,
				new EntityInfo
				{
					Purchase = StorePurchase.PAINTING_07,
					SpriteName = "painting_07",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.PAINTING_08,
				new EntityInfo
				{
					Purchase = StorePurchase.PAINTING_08,
					SpriteName = "painting_08",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.PAINTING_09,
				new EntityInfo
				{
					Purchase = StorePurchase.PAINTING_09,
					SpriteName = "painting_09",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.PAINTING_10,
				new EntityInfo
				{
					Purchase = StorePurchase.PAINTING_10,
					SpriteName = "painting_10",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.PAINTING_12,
				new EntityInfo
				{
					Purchase = StorePurchase.PAINTING_12,
					SpriteName = "painting_12",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.PAINTING_13,
				new EntityInfo
				{
					Purchase = StorePurchase.PAINTING_13,
					SpriteName = "painting_13",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.LOCK_DOOR_RED,
				new EntityInfo
				{
					Purchase = StorePurchase.LOCK_DOOR,
					SpriteName = "door_red",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.LOCK_DOOR_YELLOW,
				new EntityInfo
				{
					Purchase = StorePurchase.LOCK_DOOR,
					SpriteName = "door_yellow",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.LOCK_DOOR_BLUE,
				new EntityInfo
				{
					Purchase = StorePurchase.LOCK_DOOR,
					SpriteName = "door_blue",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.LOCK_DOOR_GREEN,
				new EntityInfo
				{
					Purchase = StorePurchase.LOCK_DOOR,
					SpriteName = "door_green",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.LOCK_DOOR_WHITE,
				new EntityInfo
				{
					Purchase = StorePurchase.LOCK_DOOR,
					SpriteName = "door_white",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.LOCK_KEY_RED,
				new EntityInfo
				{
					Purchase = StorePurchase.LOCK_KEY,
					SpriteName = "key_red",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.LOCK_KEY_YELLOW,
				new EntityInfo
				{
					Purchase = StorePurchase.LOCK_KEY,
					SpriteName = "key_yellow",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.LOCK_KEY_BLUE,
				new EntityInfo
				{
					Purchase = StorePurchase.LOCK_KEY,
					SpriteName = "key_blue",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.LOCK_KEY_GREEN,
				new EntityInfo
				{
					Purchase = StorePurchase.LOCK_KEY,
					SpriteName = "key_green",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.LOCK_KEY_WHITE,
				new EntityInfo
				{
					Purchase = StorePurchase.LOCK_KEY,
					SpriteName = "key_white",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.MILITARY_ARMORY,
				new EntityInfo
				{
					Purchase = StorePurchase.MILITARY_ARMORY,
					SpriteName = "m_armory",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.MILITARY_BARREL,
				new EntityInfo
				{
					Purchase = StorePurchase.MILITARY_BARREL,
					SpriteName = "m_barrel",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.MILITARY_BED,
				new EntityInfo
				{
					Purchase = StorePurchase.MILITARY_BED,
					SpriteName = "m_bed",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.MILITARY_CAM,
				new EntityInfo
				{
					Purchase = StorePurchase.MILITARY_CAM,
					SpriteName = "m_cam",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.MILITARY_CASE_BIG,
				new EntityInfo
				{
					Purchase = StorePurchase.MILITARY_CASE_BIG,
					SpriteName = "m_case_big",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.MILITARY_CASE_SMALL,
				new EntityInfo
				{
					Purchase = StorePurchase.MILITARY_CASE_SMALL,
					SpriteName = "m_case_small",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.MILITARY_CHAIR,
				new EntityInfo
				{
					Purchase = StorePurchase.MILITARY_CHAIR,
					SpriteName = "m_chair",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.MILITARY_CHEST,
				new EntityInfo
				{
					Purchase = StorePurchase.MILITARY_CHEST,
					SpriteName = "m_chest",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.MILITARY_CONSOLE_01,
				new EntityInfo
				{
					Purchase = StorePurchase.MILITARY_CONSOLE_01,
					SpriteName = "m_console_01",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.MILITARY_CONSOLE_02,
				new EntityInfo
				{
					Purchase = StorePurchase.MILITARY_CONSOLE_02,
					SpriteName = "m_console_02",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.MILITARY_CONSOLE_03,
				new EntityInfo
				{
					Purchase = StorePurchase.MILITARY_CONSOLE_03,
					SpriteName = "m_console_03",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.MILITARY_DOOR,
				new EntityInfo
				{
					Purchase = StorePurchase.MILITARY_DOOR,
					SpriteName = "m_door",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.MILITARY_LAMP,
				new EntityInfo
				{
					Purchase = StorePurchase.MILITARY_LAMP,
					SpriteName = "m_lamp",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.MILITARY_LANTERN,
				new EntityInfo
				{
					Purchase = StorePurchase.MILITARY_LANTERN,
					SpriteName = "m_lantern",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.MILITARY_MUG,
				new EntityInfo
				{
					Purchase = StorePurchase.MILITARY_MUG,
					SpriteName = "m_mug",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.MILITARY_PC,
				new EntityInfo
				{
					Purchase = StorePurchase.MILITARY_PC,
					SpriteName = "m_PC",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.MILITARY_TABLE_BIG,
				new EntityInfo
				{
					Purchase = StorePurchase.MILITARY_TABLE_BIG,
					SpriteName = "m_table_big",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.MILITARY_TABLE_SMALL,
				new EntityInfo
				{
					Purchase = StorePurchase.MILITARY_TABLE_SMALL,
					SpriteName = "m_table_small",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.MILITARY_FENCE_L,
				new EntityInfo
				{
					Purchase = StorePurchase.MILITARY_FENCE,
					SpriteName = "m_fence_(corner)",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.MILITARY_FENCE_END,
				new EntityInfo
				{
					Purchase = StorePurchase.MILITARY_FENCE,
					SpriteName = "m_fence_(end)",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.MILITARY_FENCE_T,
				new EntityInfo
				{
					Purchase = StorePurchase.MILITARY_FENCE,
					SpriteName = "m_fence_(T)",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.MILITARY_FENCE_I,
				new EntityInfo
				{
					Purchase = StorePurchase.MILITARY_FENCE,
					SpriteName = "m_fence_(tile)",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.MILITARY_FENCE_X,
				new EntityInfo
				{
					Purchase = StorePurchase.MILITARY_FENCE,
					SpriteName = "m_fence_(X)",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.MILITARY_BENCH,
				new EntityInfo
				{
					Purchase = StorePurchase.MILITARY_BENCH,
					SpriteName = "m_bench",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.MILITARY_BOX_01,
				new EntityInfo
				{
					Purchase = StorePurchase.MILITARY_BOX_01,
					SpriteName = "m_box_01",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.MILITARY_BOX_02,
				new EntityInfo
				{
					Purchase = StorePurchase.MILITARY_BOX_02,
					SpriteName = "m_box_02",
					Tab = TabType.Decorations
				}
			},
			{
				EntityType.MILITARY_BRIEFING_BOARD,
				new EntityInfo
				{
					Purchase = StorePurchase.MILITARY_BRIEFING_BOARD,
					SpriteName = "m_briefing_board",
					Tab = TabType.Decorations
				}
			}
		};
		Blocks = new Dictionary<BlockType, BlockInfo>
		{
			{
				BlockType.WoodPlank,
				new BlockInfo
				{
					Purchase = StorePurchase.NONE,
					SpriteName = "WoodPlank1",
					Tab = TabType.Blocks_Wood
				}
			},
			{
				BlockType.WoodPlank2,
				new BlockInfo
				{
					Purchase = StorePurchase.NONE,
					SpriteName = "WoodPlank2",
					Tab = TabType.Blocks_Wood
				}
			},
			{
				BlockType.WoodPlank3,
				new BlockInfo
				{
					Purchase = StorePurchase.NONE,
					SpriteName = "woodPlank3",
					Tab = TabType.Blocks_Wood
				}
			},
			{
				BlockType.WoodPlank4,
				new BlockInfo
				{
					Purchase = StorePurchase.NONE,
					SpriteName = "WoodPlank4",
					Tab = TabType.Blocks_Wood
				}
			},
			{
				BlockType.WoodPlank5,
				new BlockInfo
				{
					Purchase = StorePurchase.NONE,
					SpriteName = "WoodPlank5",
					Tab = TabType.Blocks_Wood
				}
			},
			{
				BlockType.Crates,
				new BlockInfo
				{
					Purchase = StorePurchase.CUBES4,
					SpriteName = "crateц",
					Tab = TabType.Blocks_Wood
				}
			},
			{
				BlockType.Wood8,
				new BlockInfo
				{
					Purchase = StorePurchase.CUBES4,
					SpriteName = "wood8",
					Tab = TabType.Blocks_Wood
				}
			},
			{
				BlockType.Wood9,
				new BlockInfo
				{
					Purchase = StorePurchase.CUBES4,
					SpriteName = "wood9",
					Tab = TabType.Blocks_Wood
				}
			},
			{
				BlockType.Wood10,
				new BlockInfo
				{
					Purchase = StorePurchase.CUBES4,
					SpriteName = "wood10",
					Tab = TabType.Blocks_Wood
				}
			},
			{
				BlockType.Wood11,
				new BlockInfo
				{
					Purchase = StorePurchase.CUBES4,
					SpriteName = "wood11",
					Tab = TabType.Blocks_Wood
				}
			},
			{
				BlockType.Brick,
				new BlockInfo
				{
					Purchase = StorePurchase.NONE,
					SpriteName = "Brick1",
					Tab = TabType.Blocks_Stone
				}
			},
			{
				BlockType.Brick2,
				new BlockInfo
				{
					Purchase = StorePurchase.NONE,
					SpriteName = "Brick2",
					Tab = TabType.Blocks_Stone
				}
			},
			{
				BlockType.Brick3,
				new BlockInfo
				{
					Purchase = StorePurchase.NONE,
					SpriteName = "Brick3",
					Tab = TabType.Blocks_Stone
				}
			},
			{
				BlockType.Brick4,
				new BlockInfo
				{
					Purchase = StorePurchase.NONE,
					SpriteName = "Brick4",
					Tab = TabType.Blocks_Stone
				}
			},
			{
				BlockType.Brick6,
				new BlockInfo
				{
					Purchase = StorePurchase.NONE,
					SpriteName = "Brick6",
					Tab = TabType.Blocks_Stone
				}
			},
			{
				BlockType.Brick7,
				new BlockInfo
				{
					Purchase = StorePurchase.CUBES1,
					SpriteName = "Brick7",
					Tab = TabType.Blocks_Stone
				}
			},
			{
				BlockType.Stone10,
				new BlockInfo
				{
					Purchase = StorePurchase.CUBES1,
					SpriteName = "stone10",
					Tab = TabType.Blocks_Stone
				}
			},
			{
				BlockType.Stone10_1,
				new BlockInfo
				{
					Purchase = StorePurchase.CUBES1,
					SpriteName = "stone10_1",
					Tab = TabType.Blocks_Stone
				}
			},
			{
				BlockType.Stone9_2,
				new BlockInfo
				{
					Purchase = StorePurchase.CUBES1,
					SpriteName = "stone9_2",
					Tab = TabType.Blocks_Stone
				}
			},
			{
				BlockType.Stone14,
				new BlockInfo
				{
					Purchase = StorePurchase.CUBES1,
					SpriteName = "stone14",
					Tab = TabType.Blocks_Stone
				}
			},
			{
				BlockType.Stone14_1,
				new BlockInfo
				{
					Purchase = StorePurchase.CUBES1,
					SpriteName = "stone14_1",
					Tab = TabType.Blocks_Stone
				}
			},
			{
				BlockType.Stone14_2,
				new BlockInfo
				{
					Purchase = StorePurchase.CUBES1,
					SpriteName = "stone14_2",
					Tab = TabType.Blocks_Stone
				}
			},
			{
				BlockType.Stone14_4,
				new BlockInfo
				{
					Purchase = StorePurchase.CUBES1,
					SpriteName = "stone14_4",
					Tab = TabType.Blocks_Stone
				}
			},
			{
				BlockType.Stone15,
				new BlockInfo
				{
					Purchase = StorePurchase.CUBES1,
					SpriteName = "stone15",
					Tab = TabType.Blocks_Stone
				}
			},
			{
				BlockType.Stone15_1,
				new BlockInfo
				{
					Purchase = StorePurchase.CUBES1,
					SpriteName = "stone15_1",
					Tab = TabType.Blocks_Stone
				}
			},
			{
				BlockType.Stone15_2,
				new BlockInfo
				{
					Purchase = StorePurchase.CUBES2,
					SpriteName = "stone15_2",
					Tab = TabType.Blocks_Stone
				}
			},
			{
				BlockType.Stone16,
				new BlockInfo
				{
					Purchase = StorePurchase.CUBES2,
					SpriteName = "stone16",
					Tab = TabType.Blocks_Stone
				}
			},
			{
				BlockType.Stone16_2,
				new BlockInfo
				{
					Purchase = StorePurchase.CUBES2,
					SpriteName = "stone16_2",
					Tab = TabType.Blocks_Stone
				}
			},
			{
				BlockType.Stone16_3,
				new BlockInfo
				{
					Purchase = StorePurchase.CUBES2,
					SpriteName = "stone16_3",
					Tab = TabType.Blocks_Stone
				}
			},
			{
				BlockType.Stone18,
				new BlockInfo
				{
					Purchase = StorePurchase.CUBES2,
					SpriteName = "stone18",
					Tab = TabType.Blocks_Stone
				}
			},
			{
				BlockType.Stone18_1,
				new BlockInfo
				{
					Purchase = StorePurchase.CUBES2,
					SpriteName = "stone18_1",
					Tab = TabType.Blocks_Stone
				}
			},
			{
				BlockType.Stone18_2,
				new BlockInfo
				{
					Purchase = StorePurchase.CUBES2,
					SpriteName = "stone18_2",
					Tab = TabType.Blocks_Stone
				}
			},
			{
				BlockType.Stone17,
				new BlockInfo
				{
					Purchase = StorePurchase.CUBES2,
					SpriteName = "stone17",
					Tab = TabType.Blocks_Stone
				}
			},
			{
				BlockType.Stone17_1,
				new BlockInfo
				{
					Purchase = StorePurchase.CUBES2,
					SpriteName = "stone17_1",
					Tab = TabType.Blocks_Stone
				}
			},
			{
				BlockType.Stone17_2,
				new BlockInfo
				{
					Purchase = StorePurchase.CUBES2,
					SpriteName = "stone17_2",
					Tab = TabType.Blocks_Stone
				}
			},
			{
				BlockType.Colored1,
				new BlockInfo
				{
					Purchase = StorePurchase.CUBES3,
					SpriteName = "colored1",
					Tab = TabType.Blocks_Colored
				}
			},
			{
				BlockType.Colored2,
				new BlockInfo
				{
					Purchase = StorePurchase.CUBES3,
					SpriteName = "colored2",
					Tab = TabType.Blocks_Colored
				}
			},
			{
				BlockType.Colored3,
				new BlockInfo
				{
					Purchase = StorePurchase.CUBES3,
					SpriteName = "colored3",
					Tab = TabType.Blocks_Colored
				}
			},
			{
				BlockType.Colored4,
				new BlockInfo
				{
					Purchase = StorePurchase.CUBES3,
					SpriteName = "colored4",
					Tab = TabType.Blocks_Colored
				}
			},
			{
				BlockType.Colored5,
				new BlockInfo
				{
					Purchase = StorePurchase.CUBES3,
					SpriteName = "colored5",
					Tab = TabType.Blocks_Colored
				}
			},
			{
				BlockType.Colored6,
				new BlockInfo
				{
					Purchase = StorePurchase.CUBES3,
					SpriteName = "colored6",
					Tab = TabType.Blocks_Colored
				}
			},
			{
				BlockType.Colored7,
				new BlockInfo
				{
					Purchase = StorePurchase.CUBES3,
					SpriteName = "colored7",
					Tab = TabType.Blocks_Colored
				}
			},
			{
				BlockType.Colored8,
				new BlockInfo
				{
					Purchase = StorePurchase.CUBES3,
					SpriteName = "colored8",
					Tab = TabType.Blocks_Colored
				}
			},
			{
				BlockType.Colored9,
				new BlockInfo
				{
					Purchase = StorePurchase.CUBES3,
					SpriteName = "colored9",
					Tab = TabType.Blocks_Colored
				}
			},
			{
				BlockType.Wood,
				new BlockInfo
				{
					Purchase = StorePurchase.NONE,
					SpriteName = "bark_new1",
					Tab = TabType.Blocks_Nature
				}
			},
			{
				BlockType.TopSoil,
				new BlockInfo
				{
					Purchase = StorePurchase.NONE,
					SpriteName = "grass-dirt_new1",
					Tab = TabType.Blocks_Nature
				}
			},
			{
				BlockType.Leaves,
				new BlockInfo
				{
					Purchase = StorePurchase.NONE,
					SpriteName = "leaf_new",
					Tab = TabType.Blocks_Nature
				}
			},
			{
				BlockType.Dirt,
				new BlockInfo
				{
					Purchase = StorePurchase.NONE,
					SpriteName = "dirt2",
					Tab = TabType.Blocks_Nature
				}
			},
			{
				BlockType.Stone,
				new BlockInfo
				{
					Purchase = StorePurchase.NONE,
					SpriteName = "rock_new",
					Tab = TabType.Blocks_Nature
				}
			},
			{
				BlockType.Sand,
				new BlockInfo
				{
					Purchase = StorePurchase.NONE,
					SpriteName = "sand",
					Tab = TabType.Blocks_Nature
				}
			},
			{
				BlockType.Water,
				new BlockInfo
				{
					Purchase = StorePurchase.WATER,
					SpriteName = "water",
					Tab = TabType.Blocks_Nature
				}
			},
			{
				BlockType.Lava,
				new BlockInfo
				{
					Purchase = StorePurchase.LAVA,
					SpriteName = "Лава3",
					Tab = TabType.Blocks_Nature
				}
			},
			{
				BlockType.SnowDirt,
				new BlockInfo
				{
					Purchase = StorePurchase.NONE,
					SpriteName = "snow_dirt",
					Tab = TabType.Blocks_Nature,
					Validator = () => App.Instance.Settings.mapType == GameINI.MapType.SNOWLAND
				}
			},
			{
				BlockType.Snow,
				new BlockInfo
				{
					Purchase = StorePurchase.NONE,
					SpriteName = "snow",
					Tab = TabType.Blocks_Nature,
					Validator = () => App.Instance.Settings.mapType == GameINI.MapType.SNOWLAND
				}
			},
			{
				BlockType.Ice,
				new BlockInfo
				{
					Purchase = StorePurchase.ICE,
					SpriteName = "ice",
					Tab = TabType.Blocks_Nature
				}
			},
			{
				BlockType.Gum,
				new BlockInfo
				{
					Purchase = StorePurchase.CUBE_BATUTE,
					SpriteName = "gum2",
					Tab = TabType.Blocks_Nature
				}
			},
			{
				BlockType.TeamBlue,
				new BlockInfo
				{
					Purchase = StorePurchase.TEAMCUBES,
					SpriteName = "brick_blue",
					Tab = TabType.Blocks_Nature
				}
			},
			{
				BlockType.TeamRed,
				new BlockInfo
				{
					Purchase = StorePurchase.TEAMCUBES,
					SpriteName = "brick_red",
					Tab = TabType.Blocks_Nature
				}
			},
			{
				BlockType.Glass1,
				new BlockInfo
				{
					Purchase = StorePurchase.GLASSCUBES,
					SpriteName = "glass wood",
					Tab = TabType.Blocks_Glass
				}
			},
			{
				BlockType.Glass2,
				new BlockInfo
				{
					Purchase = StorePurchase.GLASSCUBES,
					SpriteName = "glass metall",
					Tab = TabType.Blocks_Glass
				}
			},
			{
				BlockType.Glass3,
				new BlockInfo
				{
					Purchase = StorePurchase.GLASSCUBES,
					SpriteName = "colored_glass7",
					Tab = TabType.Blocks_Glass
				}
			},
			{
				BlockType.Glass4,
				new BlockInfo
				{
					Purchase = StorePurchase.GLASSCUBES,
					SpriteName = "colored_glass6",
					Tab = TabType.Blocks_Glass
				}
			},
			{
				BlockType.Glass5,
				new BlockInfo
				{
					Purchase = StorePurchase.GLASSCUBES,
					SpriteName = "colored_glass5",
					Tab = TabType.Blocks_Glass
				}
			},
			{
				BlockType.Glass6,
				new BlockInfo
				{
					Purchase = StorePurchase.GLASSCUBES,
					SpriteName = "colored_glass4",
					Tab = TabType.Blocks_Glass
				}
			},
			{
				BlockType.Glass7,
				new BlockInfo
				{
					Purchase = StorePurchase.GLASSCUBES,
					SpriteName = "colored_glass3",
					Tab = TabType.Blocks_Glass
				}
			},
			{
				BlockType.Glass8,
				new BlockInfo
				{
					Purchase = StorePurchase.GLASSCUBES,
					SpriteName = "colored_glass2",
					Tab = TabType.Blocks_Glass
				}
			},
			{
				BlockType.Glass9,
				new BlockInfo
				{
					Purchase = StorePurchase.GLASSCUBES,
					SpriteName = "colored_glass1",
					Tab = TabType.Blocks_Glass
				}
			},
			{
				BlockType.Reshetka1,
				new BlockInfo
				{
					Purchase = StorePurchase.RESHETKACUBES,
					SpriteName = "cell1",
					Tab = TabType.Blocks_Cell
				}
			},
			{
				BlockType.Reshetka2,
				new BlockInfo
				{
					Purchase = StorePurchase.RESHETKACUBES,
					SpriteName = "cell2",
					Tab = TabType.Blocks_Cell
				}
			},
			{
				BlockType.Reshetka3,
				new BlockInfo
				{
					Purchase = StorePurchase.RESHETKACUBES,
					SpriteName = "cell3",
					Tab = TabType.Blocks_Cell
				}
			},
			{
				BlockType.Reshetka4,
				new BlockInfo
				{
					Purchase = StorePurchase.RESHETKACUBES,
					SpriteName = "cell4",
					Tab = TabType.Blocks_Cell
				}
			},
			{
				BlockType.Reshetka5,
				new BlockInfo
				{
					Purchase = StorePurchase.RESHETKACUBES,
					SpriteName = "cell5",
					Tab = TabType.Blocks_Cell
				}
			},
			{
				BlockType.Tile1,
				new BlockInfo
				{
					Purchase = StorePurchase.TILE_CUBES,
					SpriteName = "tile_01",
					Tab = TabType.Blocks_Tile
				}
			},
			{
				BlockType.Tile2,
				new BlockInfo
				{
					Purchase = StorePurchase.TILE_CUBES,
					SpriteName = "tile_02",
					Tab = TabType.Blocks_Tile
				}
			},
			{
				BlockType.Tile3,
				new BlockInfo
				{
					Purchase = StorePurchase.TILE_CUBES,
					SpriteName = "tile_03",
					Tab = TabType.Blocks_Tile
				}
			},
			{
				BlockType.Tile4,
				new BlockInfo
				{
					Purchase = StorePurchase.TILE_CUBES,
					SpriteName = "tile_04",
					Tab = TabType.Blocks_Tile
				}
			},
			{
				BlockType.Tile5,
				new BlockInfo
				{
					Purchase = StorePurchase.TILE_CUBES,
					SpriteName = "tile_05",
					Tab = TabType.Blocks_Tile
				}
			},
			{
				BlockType.Tile7,
				new BlockInfo
				{
					Purchase = StorePurchase.TILE_CUBES,
					SpriteName = "tile_07",
					Tab = TabType.Blocks_Tile
				}
			},
			{
				BlockType.Tile9,
				new BlockInfo
				{
					Purchase = StorePurchase.TILE_CUBES,
					SpriteName = "tile_09",
					Tab = TabType.Blocks_Tile
				}
			},
			{
				BlockType.Castle01_Wall,
				new BlockInfo
				{
					Purchase = StorePurchase.CASTLE_CUBES,
					SpriteName = "castle_texture_01_wall",
					Tab = TabType.Blocks_Sets,
					Category = "CASTLE_CUBES"
				}
			},
			{
				BlockType.Castle02_Wall,
				new BlockInfo
				{
					Purchase = StorePurchase.CASTLE_CUBES,
					SpriteName = "castle_texture_02_wall",
					Tab = TabType.Blocks_Sets,
					Category = "CASTLE_CUBES"
				}
			},
			{
				BlockType.Castle03_Floor,
				new BlockInfo
				{
					Purchase = StorePurchase.CASTLE_CUBES,
					SpriteName = "castle_texture_03_floor",
					Tab = TabType.Blocks_Sets,
					Category = "CASTLE_CUBES"
				}
			},
			{
				BlockType.Castle04_Floor,
				new BlockInfo
				{
					Purchase = StorePurchase.CASTLE_CUBES,
					SpriteName = "castle_texture_04_floor",
					Tab = TabType.Blocks_Sets,
					Category = "CASTLE_CUBES"
				}
			},
			{
				BlockType.Castle05_Floor,
				new BlockInfo
				{
					Purchase = StorePurchase.CASTLE_CUBES,
					SpriteName = "castle_texture_05_floor",
					Tab = TabType.Blocks_Sets,
					Category = "CASTLE_CUBES"
				}
			},
			{
				BlockType.Castle06,
				new BlockInfo
				{
					Purchase = StorePurchase.CASTLE_CUBES,
					SpriteName = "castle_texture_06",
					Tab = TabType.Blocks_Sets,
					Category = "CASTLE_CUBES"
				}
			},
			{
				BlockType.Castle07_Grate,
				new BlockInfo
				{
					Purchase = StorePurchase.CASTLE_CUBES,
					SpriteName = "castle_texture_07_grate",
					Tab = TabType.Blocks_Sets,
					Category = "CASTLE_CUBES"
				}
			},
			{
				BlockType.Castle08_Wall,
				new BlockInfo
				{
					Purchase = StorePurchase.CASTLE_CUBES,
					SpriteName = "castle_texture_08_wall",
					Tab = TabType.Blocks_Sets,
					Category = "CASTLE_CUBES"
				}
			},
			{
				BlockType.Castle09,
				new BlockInfo
				{
					Purchase = StorePurchase.CASTLE_CUBES,
					SpriteName = "castle_texture_09",
					Tab = TabType.Blocks_Sets,
					Category = "CASTLE_CUBES"
				}
			},
			{
				BlockType.Dungeon01_Wall,
				new BlockInfo
				{
					Purchase = StorePurchase.DUNGEON_CUBES,
					SpriteName = "dungeon_texture_01_wall",
					Tab = TabType.Blocks_Sets,
					Category = "DUNGEON_CUBES"
				}
			},
			{
				BlockType.Dungeon02_Floor,
				new BlockInfo
				{
					Purchase = StorePurchase.DUNGEON_CUBES,
					SpriteName = "dungeon_texture_02_floor",
					Tab = TabType.Blocks_Sets,
					Category = "DUNGEON_CUBES"
				}
			},
			{
				BlockType.Dungeon03,
				new BlockInfo
				{
					Purchase = StorePurchase.DUNGEON_CUBES,
					SpriteName = "dungeon_texture_03",
					Tab = TabType.Blocks_Sets,
					Category = "DUNGEON_CUBES"
				}
			},
			{
				BlockType.Dungeon04_Wall,
				new BlockInfo
				{
					Purchase = StorePurchase.DUNGEON_CUBES,
					SpriteName = "dungeon_texture_04_wall",
					Tab = TabType.Blocks_Sets,
					Category = "DUNGEON_CUBES"
				}
			},
			{
				BlockType.Dungeon05_FloorElement,
				new BlockInfo
				{
					Purchase = StorePurchase.DUNGEON_CUBES,
					SpriteName = "dungeon_texture_05_floor_element",
					Tab = TabType.Blocks_Sets,
					Category = "DUNGEON_CUBES"
				}
			},
			{
				BlockType.Dungeon06_WallBottom,
				new BlockInfo
				{
					Purchase = StorePurchase.DUNGEON_CUBES,
					SpriteName = "dungeon_texture_06_wall_bottom",
					Tab = TabType.Blocks_Sets,
					Category = "DUNGEON_CUBES"
				}
			},
			{
				BlockType.Dungeon07_WallElement,
				new BlockInfo
				{
					Purchase = StorePurchase.DUNGEON_CUBES,
					SpriteName = "dungeon_texture_07_wall_element",
					Tab = TabType.Blocks_Sets,
					Category = "DUNGEON_CUBES"
				}
			},
			{
				BlockType.Dungeon08_WoodBar,
				new BlockInfo
				{
					Purchase = StorePurchase.DUNGEON_CUBES,
					SpriteName = "dungeon_texture_08_wood_bar",
					Tab = TabType.Blocks_Sets,
					Category = "DUNGEON_CUBES"
				}
			},
			{
				BlockType.Dungeon09_WoodBox,
				new BlockInfo
				{
					Purchase = StorePurchase.DUNGEON_CUBES,
					SpriteName = "dungeon_texture_09_wood_box",
					Tab = TabType.Blocks_Sets,
					Category = "DUNGEON_CUBES"
				}
			},
			{
				BlockType.Dungeon10_Grate,
				new BlockInfo
				{
					Purchase = StorePurchase.DUNGEON_CUBES,
					SpriteName = "dungeon_texture_10_grate",
					Tab = TabType.Blocks_Sets,
					Category = "DUNGEON_CUBES"
				}
			},
			{
				BlockType.Dungeon11_WallElement,
				new BlockInfo
				{
					Purchase = StorePurchase.DUNGEON_CUBES,
					SpriteName = "dungeon_texture_11_wall_element",
					Tab = TabType.Blocks_Sets,
					Category = "DUNGEON_CUBES"
				}
			},
			{
				BlockType.Dungeon12_Floor,
				new BlockInfo
				{
					Purchase = StorePurchase.DUNGEON_CUBES,
					SpriteName = "dungeon_texture_12_floor",
					Tab = TabType.Blocks_Sets,
					Category = "DUNGEON_CUBES"
				}
			},
			{
				BlockType.Dungeon13_Floor,
				new BlockInfo
				{
					Purchase = StorePurchase.DUNGEON_CUBES,
					SpriteName = "dungeon_texture_13_floor",
					Tab = TabType.Blocks_Sets,
					Category = "DUNGEON_CUBES"
				}
			},
			{
				BlockType.Fortress01_Floor,
				new BlockInfo
				{
					Purchase = StorePurchase.FORTRESS_CUBES,
					SpriteName = "fortress_texture_01_floor",
					Tab = TabType.Blocks_Sets,
					Category = "FORTRESS_CUBES"
				}
			},
			{
				BlockType.Fortress02_Wall,
				new BlockInfo
				{
					Purchase = StorePurchase.FORTRESS_CUBES,
					SpriteName = "fortress_texture_02_wall",
					Tab = TabType.Blocks_Sets,
					Category = "FORTRESS_CUBES"
				}
			},
			{
				BlockType.Fortress03_Floor,
				new BlockInfo
				{
					Purchase = StorePurchase.FORTRESS_CUBES,
					SpriteName = "fortress_texture_03_floor",
					Tab = TabType.Blocks_Sets,
					Category = "FORTRESS_CUBES"
				}
			},
			{
				BlockType.Fortress04_Wall,
				new BlockInfo
				{
					Purchase = StorePurchase.FORTRESS_CUBES,
					SpriteName = "fortress_texture_04_wall",
					Tab = TabType.Blocks_Sets,
					Category = "FORTRESS_CUBES"
				}
			},
			{
				BlockType.Fortress05_Wall,
				new BlockInfo
				{
					Purchase = StorePurchase.FORTRESS_CUBES,
					SpriteName = "fortress_texture_05_wall",
					Tab = TabType.Blocks_Sets,
					Category = "FORTRESS_CUBES"
				}
			},
			{
				BlockType.Fortress06_Wall,
				new BlockInfo
				{
					Purchase = StorePurchase.FORTRESS_CUBES,
					SpriteName = "fortress_texture_06_wall",
					Tab = TabType.Blocks_Sets,
					Category = "FORTRESS_CUBES"
				}
			},
			{
				BlockType.Fortress07_Floor,
				new BlockInfo
				{
					Purchase = StorePurchase.FORTRESS_CUBES,
					SpriteName = "fortress_texture_07_floor",
					Tab = TabType.Blocks_Sets,
					Category = "FORTRESS_CUBES"
				}
			},
			{
				BlockType.Fortress08_Grate,
				new BlockInfo
				{
					Purchase = StorePurchase.FORTRESS_CUBES,
					SpriteName = "fortress_texture_08_grate",
					Tab = TabType.Blocks_Sets,
					Category = "FORTRESS_CUBES"
				}
			},
			{
				BlockType.Fortress09_WoodBox,
				new BlockInfo
				{
					Purchase = StorePurchase.FORTRESS_CUBES,
					SpriteName = "fortress_texture_09_wood_box",
					Tab = TabType.Blocks_Sets,
					Category = "FORTRESS_CUBES"
				}
			},
			{
				BlockType.Fortress10_WoodBar,
				new BlockInfo
				{
					Purchase = StorePurchase.FORTRESS_CUBES,
					SpriteName = "fortress_texture_10_wood_bar",
					Tab = TabType.Blocks_Sets,
					Category = "FORTRESS_CUBES"
				}
			},
			{
				BlockType.Library01_WallBottom,
				new BlockInfo
				{
					Purchase = StorePurchase.LIBRARY_CUBES,
					SpriteName = "library_texture_01_wall_bottom",
					Tab = TabType.Blocks_Sets,
					Category = "LIBRARY_CUBES"
				}
			},
			{
				BlockType.Library02_Floor,
				new BlockInfo
				{
					Purchase = StorePurchase.LIBRARY_CUBES,
					SpriteName = "library_texture_02_floor",
					Tab = TabType.Blocks_Sets,
					Category = "LIBRARY_CUBES"
				}
			},
			{
				BlockType.Library03_Floor,
				new BlockInfo
				{
					Purchase = StorePurchase.LIBRARY_CUBES,
					SpriteName = "library_texture_03_floor",
					Tab = TabType.Blocks_Sets,
					Category = "LIBRARY_CUBES"
				}
			},
			{
				BlockType.Library04_Wall,
				new BlockInfo
				{
					Purchase = StorePurchase.LIBRARY_CUBES,
					SpriteName = "library_texture_04_wall",
					Tab = TabType.Blocks_Sets,
					Category = "LIBRARY_CUBES"
				}
			},
			{
				BlockType.Library05_WallElement,
				new BlockInfo
				{
					Purchase = StorePurchase.LIBRARY_CUBES,
					SpriteName = "library_texture_05_wall_element",
					Tab = TabType.Blocks_Sets,
					Category = "LIBRARY_CUBES"
				}
			},
			{
				BlockType.Library06_BookShelf,
				new BlockInfo
				{
					Purchase = StorePurchase.LIBRARY_CUBES,
					SpriteName = "library_texture_06_book_shelf",
					Tab = TabType.Blocks_Sets,
					Category = "LIBRARY_CUBES"
				}
			},
			{
				BlockType.Library07,
				new BlockInfo
				{
					Purchase = StorePurchase.LIBRARY_CUBES,
					SpriteName = "library_texture_07",
					Tab = TabType.Blocks_Sets,
					Category = "LIBRARY_CUBES"
				}
			},
			{
				BlockType.Tavern01_WallBottom,
				new BlockInfo
				{
					Purchase = StorePurchase.TAVERN_CUBES,
					SpriteName = "tavern_texture_01_wall_bottom",
					Tab = TabType.Blocks_Sets,
					Category = "TAVERN_CUBES"
				}
			},
			{
				BlockType.Tavern02_Wall,
				new BlockInfo
				{
					Purchase = StorePurchase.TAVERN_CUBES,
					SpriteName = "tavern_texture_02_wall",
					Tab = TabType.Blocks_Sets,
					Category = "TAVERN_CUBES"
				}
			},
			{
				BlockType.Tavern03_WallElement,
				new BlockInfo
				{
					Purchase = StorePurchase.TAVERN_CUBES,
					SpriteName = "tavern_texture_03_wall_element",
					Tab = TabType.Blocks_Sets,
					Category = "TAVERN_CUBES"
				}
			},
			{
				BlockType.Tavern04_WoodBox,
				new BlockInfo
				{
					Purchase = StorePurchase.TAVERN_CUBES,
					SpriteName = "tavern_texture_04_wood_box",
					Tab = TabType.Blocks_Sets,
					Category = "TAVERN_CUBES"
				}
			},
			{
				BlockType.Tavern05_Floor,
				new BlockInfo
				{
					Purchase = StorePurchase.TAVERN_CUBES,
					SpriteName = "tavern_texture_05_floor",
					Tab = TabType.Blocks_Sets,
					Category = "TAVERN_CUBES"
				}
			},
			{
				BlockType.Tavern06_Wall,
				new BlockInfo
				{
					Purchase = StorePurchase.TAVERN_CUBES,
					SpriteName = "tavern_texture_06_wall",
					Tab = TabType.Blocks_Sets,
					Category = "TAVERN_CUBES"
				}
			},
			{
				BlockType.Tavern07_Wall,
				new BlockInfo
				{
					Purchase = StorePurchase.TAVERN_CUBES,
					SpriteName = "tavern_texture_07_wall",
					Tab = TabType.Blocks_Sets,
					Category = "TAVERN_CUBES"
				}
			},
			{
				BlockType.Tavern08_FloorElement,
				new BlockInfo
				{
					Purchase = StorePurchase.TAVERN_CUBES,
					SpriteName = "tavern_texture_08_floor_element",
					Tab = TabType.Blocks_Sets,
					Category = "TAVERN_CUBES"
				}
			},
			{
				BlockType.Tavern09_WoodBar,
				new BlockInfo
				{
					Purchase = StorePurchase.TAVERN_CUBES,
					SpriteName = "tavern_texture_09_wood_bar",
					Tab = TabType.Blocks_Sets,
					Category = "TAVERN_CUBES"
				}
			},
			{
				BlockType.Tavern10_WallBottom,
				new BlockInfo
				{
					Purchase = StorePurchase.TAVERN_CUBES,
					SpriteName = "tavern_texture_10_wall_bottom",
					Tab = TabType.Blocks_Sets,
					Category = "TAVERN_CUBES"
				}
			},
			{
				BlockType.Tavern11_WallBottom,
				new BlockInfo
				{
					Purchase = StorePurchase.TAVERN_CUBES,
					SpriteName = "tavern_texture_11_wall_bottom",
					Tab = TabType.Blocks_Sets,
					Category = "TAVERN_CUBES"
				}
			},
			{
				BlockType.Military01,
				new BlockInfo
				{
					Purchase = StorePurchase.MILITARY_CUBES,
					SpriteName = "military_01",
					Tab = TabType.Blocks_Sets,
					Category = "MILITARY_CUBES"
				}
			},
			{
				BlockType.Military02,
				new BlockInfo
				{
					Purchase = StorePurchase.MILITARY_CUBES,
					SpriteName = "military_02",
					Tab = TabType.Blocks_Sets,
					Category = "MILITARY_CUBES"
				}
			},
			{
				BlockType.Military03,
				new BlockInfo
				{
					Purchase = StorePurchase.MILITARY_CUBES,
					SpriteName = "military_03",
					Tab = TabType.Blocks_Sets,
					Category = "MILITARY_CUBES"
				}
			},
			{
				BlockType.Military04,
				new BlockInfo
				{
					Purchase = StorePurchase.MILITARY_CUBES,
					SpriteName = "military_04",
					Tab = TabType.Blocks_Sets,
					Category = "MILITARY_CUBES"
				}
			},
			{
				BlockType.Military05,
				new BlockInfo
				{
					Purchase = StorePurchase.MILITARY_CUBES,
					SpriteName = "military_05",
					Tab = TabType.Blocks_Sets,
					Category = "MILITARY_CUBES"
				}
			},
			{
				BlockType.Military06,
				new BlockInfo
				{
					Purchase = StorePurchase.MILITARY_CUBES,
					SpriteName = "military_06",
					Tab = TabType.Blocks_Sets,
					Category = "MILITARY_CUBES"
				}
			},
			{
				BlockType.Military07,
				new BlockInfo
				{
					Purchase = StorePurchase.MILITARY_CUBES,
					SpriteName = "military_07",
					Tab = TabType.Blocks_Sets,
					Category = "MILITARY_CUBES"
				}
			},
			{
				BlockType.Military08,
				new BlockInfo
				{
					Purchase = StorePurchase.MILITARY_CUBES,
					SpriteName = "military_08",
					Tab = TabType.Blocks_Sets,
					Category = "MILITARY_CUBES"
				}
			},
			{
				BlockType.Military09,
				new BlockInfo
				{
					Purchase = StorePurchase.MILITARY_CUBES,
					SpriteName = "military_09",
					Tab = TabType.Blocks_Sets,
					Category = "MILITARY_CUBES"
				}
			},
			{
				BlockType.Military10,
				new BlockInfo
				{
					Purchase = StorePurchase.MILITARY_CUBES,
					SpriteName = "military_10",
					Tab = TabType.Blocks_Sets,
					Category = "MILITARY_CUBES"
				}
			},
			{
				BlockType.Military11,
				new BlockInfo
				{
					Purchase = StorePurchase.MILITARY_CUBES,
					SpriteName = "military_11",
					Tab = TabType.Blocks_Sets,
					Category = "MILITARY_CUBES"
				}
			},
			{
				BlockType.Military12,
				new BlockInfo
				{
					Purchase = StorePurchase.MILITARY_CUBES,
					SpriteName = "military_12",
					Tab = TabType.Blocks_Sets,
					Category = "MILITARY_CUBES"
				}
			}
		};
		Purchases = new Dictionary<StorePurchase, PurchaseInfo>
		{
			{
				StorePurchase.SLOTS,
				new PurchaseInfo
				{
					Cost = new OnePay(0, 20, once: false),
					Name = "SLOTS",
					Tab = TabType.Untagged,
					Icon = string.Empty
				}
			},
			{
				StorePurchase.FISH_1,
				new PurchaseInfo
				{
					Cost = new OnePay(300, 0, once: false),
					Name = "FISH1",
					Tab = TabType.Pets,
					Icon = "fish_1"
				}
			},
			{
				StorePurchase.FISH_2,
				new PurchaseInfo
				{
					Cost = new OnePay(300, 0, once: false),
					Name = "FISH2",
					Tab = TabType.Pets,
					Icon = "fish_2"
				}
			},
			{
				StorePurchase.FISH_3,
				new PurchaseInfo
				{
					Cost = new OnePay(300, 0, once: false),
					Name = "FISH3",
					Tab = TabType.Pets,
					Icon = "fish_3"
				}
			},
			{
				StorePurchase.CAT,
				new PurchaseInfo
				{
					Cost = new OnePay(500, 0, once: false),
					Name = "CAT",
					Tab = TabType.Pets,
					Icon = "cat"
				}
			},
			{
				StorePurchase.DOG,
				new PurchaseInfo
				{
					Cost = new OnePay(600, 0, once: false),
					Name = "DOG",
					Tab = TabType.Pets,
					Icon = "dog"
				}
			},
			{
				StorePurchase.CHICKEN,
				new PurchaseInfo
				{
					Cost = new OnePay(400, 0, once: false),
					Name = "CHIKEN",
					Tab = TabType.Pets,
					Icon = "chicken_pet"
				}
			},
			{
				StorePurchase.CRAB,
				new PurchaseInfo
				{
					Cost = new OnePay(0, 1, once: false),
					Name = "CRAB",
					Tab = TabType.Pets,
					Icon = "pet_02_icon"
				}
			},
			{
				StorePurchase.BOAR,
				new PurchaseInfo
				{
					Cost = new OnePay(0, 3, once: false),
					Name = "BOAR",
					Tab = TabType.Pets,
					Icon = "pet_01_icon"
				}
			},
			{
				StorePurchase.MEDIUM_MAP,
				new PurchaseInfo
				{
					Cost = new OnePay(0, 1, once: true),
					Name = "MIDDLE_MAP",
					Tab = TabType.Maps,
					Icon = "icons_medium_map_002d_fin",
					Data = GameINI.MapSize.MEDIUM
				}
			},
			{
				StorePurchase.FLAT_MAP,
				new PurchaseInfo
				{
					Cost = new OnePay(600, 0, once: true),
					Name = "FLAT_MAP",
					Tab = TabType.Maps,
					Icon = "icons_terrain_plaint_003_fin",
					LargeIcon = "FlatPreview",
					Data = GameINI.MapType.FLAT
				}
			},
			{
				StorePurchase.SAND_MAP,
				new PurchaseInfo
				{
					Cost = new OnePay(800, 0, once: true),
					Name = "DESERT",
					Tab = TabType.Maps,
					Icon = "icons_terrain_desert_004_fin",
					LargeIcon = "SandPreview",
					Data = GameINI.MapType.SAND
				}
			},
			{
				StorePurchase.OCEAN_MAP,
				new PurchaseInfo
				{
					Cost = new OnePay(1000, 0, once: true),
					Name = "OCEAN",
					Tab = TabType.Maps,
					Icon = "icons_terrain_ocean_002_fin",
					LargeIcon = "OceanPreview",
					Data = GameINI.MapType.OCEAN
				}
			},
			{
				StorePurchase.ISLAND_MAP,
				new PurchaseInfo
				{
					Cost = new OnePay(0, 10, once: true),
					Name = "ISLANDS",
					Tab = TabType.Maps,
					Icon = "icons_terrain_islands_003",
					LargeIcon = "IslandsPreview",
					Data = GameINI.MapType.ISLAND
				}
			},
			{
				StorePurchase.SNOW_MAP,
				new PurchaseInfo
				{
					Cost = new OnePay(0, 15, once: true),
					Name = "WINTER",
					Tab = TabType.Maps,
					Icon = "icons_terrain_snow_wip_003",
					LargeIcon = "SnowPreview",
					Data = GameINI.MapType.SNOWLAND
				}
			},
			{
				StorePurchase.STANDART_SKIN,
				new PurchaseInfo
				{
					Cost = new OnePay(120, 0, once: true),
					Name = "NORMAL_SKIN",
					Tab = TabType.Skins,
					Icon = "icons_digger",
					Category = "SKIN_BASE",
					Skin = 0
				}
			},
			{
				StorePurchase.GIRL_SKIN_1,
				new PurchaseInfo
				{
					Cost = new OnePay(0, 5, once: true),
					Name = "GIRL_SKIN_1",
					Tab = TabType.Skins,
					Icon = "girl_skin_02",
					Category = "SKIN_BASE",
					Skin = 17
				}
			},
			{
				StorePurchase.GIRL_SKIN_2,
				new PurchaseInfo
				{
					Cost = new OnePay(0, 10, once: true),
					Name = "GIRL_SKIN_2",
					Tab = TabType.Skins,
					Icon = "girl_skin_01",
					Category = "SKIN_BASE",
					Skin = 18
				}
			},
			{
				StorePurchase.SMELTER_SKIN,
				new PurchaseInfo
				{
					Cost = new OnePay(2000, 0, once: true),
					Name = "SMELTER_SKIN",
					Tab = TabType.Skins,
					Icon = "smelter_skin",
					Category = "SKIN_BASE",
					Skin = 24
				}
			},
			{
				StorePurchase.COOK_SKIN,
				new PurchaseInfo
				{
					Cost = new OnePay(0, 15, once: true),
					Name = "COOK_SKIN",
					Tab = TabType.Skins,
					Icon = "cook_skin",
					Category = "SKIN_BASE",
					Skin = 21
				}
			},
			{
				StorePurchase.ARCHER_SKIN,
				new PurchaseInfo
				{
					Cost = new OnePay(3000, 0, once: true),
					Name = "ARCHER_SKIN",
					Tab = TabType.Skins,
					Icon = "archer_skin",
					Category = "SKIN_BASE",
					Skin = 19
				}
			},
			{
				StorePurchase.KNIGHT_SKIN_0,
				new PurchaseInfo
				{
					Cost = new OnePay(1500, 0, once: true),
					Name = "KNIGHT0",
					Tab = TabType.Skins,
					Icon = "icons_knight0",
					Category = "SKIN_MIDDLE_AGES",
					Skin = 13
				}
			},
			{
				StorePurchase.KNIGHT_SKIN_1,
				new PurchaseInfo
				{
					Cost = new OnePay(2500, 0, once: true),
					Name = "KNIGHT",
					Tab = TabType.Skins,
					Icon = "icons_knght_2lvl",
					Category = "SKIN_MIDDLE_AGES",
					Skin = 2,
					RequiredPurchase = StorePurchase.KNIGHT_SKIN_0
				}
			},
			{
				StorePurchase.KNIGHT_SKIN_2,
				new PurchaseInfo
				{
					Cost = new OnePay(0, 30, once: true),
					Name = "EPIC_KNIGHT",
					Tab = TabType.Skins,
					Icon = "icons_knght_3lvl",
					Category = "SKIN_MIDDLE_AGES",
					Skin = 3,
					RequiredPurchase = StorePurchase.KNIGHT_SKIN_1
				}
			},
			{
				StorePurchase.PIRATE_SKIN_1,
				new PurchaseInfo
				{
					Cost = new OnePay(2000, 0, once: true),
					Name = "PIRATE",
					Tab = TabType.Skins,
					Icon = "icons_pirate_1lvl_sktch_004",
					Category = "SKIN_MIDDLE_AGES",
					Skin = 1
				}
			},
			{
				StorePurchase.PIRATE_SKIN_2,
				new PurchaseInfo
				{
					Cost = new OnePay(2500, 0, once: true),
					Name = "CAPTAIN_PIRATE",
					Tab = TabType.Skins,
					Icon = "icons_pirate_2lvl_001",
					Category = "SKIN_MIDDLE_AGES",
					Skin = 4,
					RequiredPurchase = StorePurchase.PIRATE_SKIN_1
				}
			},
			{
				StorePurchase.VIKING_SKIN_1,
				new PurchaseInfo
				{
					Cost = new OnePay(0, 25, once: true),
					Name = "VIKING",
					Tab = TabType.Skins,
					Icon = "icons_viking_001",
					Category = "SKIN_MIDDLE_AGES",
					Skin = 5
				}
			},
			{
				StorePurchase.ZOMBIE_SKIN,
				new PurchaseInfo
				{
					Cost = new OnePay(3000, 0, once: true),
					Name = "ZOMBIE",
					Tab = TabType.Skins,
					Icon = "icons_zombie",
					Category = "SKIN_MIDDLE_AGES",
					Skin = 6
				}
			},
			{
				StorePurchase.SKELETON_SKIN,
				new PurchaseInfo
				{
					Cost = new OnePay(3500, 0, once: true),
					Name = "SKELETON",
					Tab = TabType.Skins,
					Icon = "icons_skeleton",
					Category = "SKIN_MIDDLE_AGES",
					Skin = 14,
					RequiredPurchase = StorePurchase.ZOMBIE_SKIN
				}
			},
			{
				StorePurchase.DEATH_KNIGHT_SKIN,
				new PurchaseInfo
				{
					Cost = new OnePay(0, 40, once: true),
					Name = "DEATH_KNIGHT",
					Tab = TabType.Skins,
					Icon = "unit_icon_(c)",
					Category = "SKIN_MIDDLE_AGES",
					Skin = 7,
					RequiredPurchase = StorePurchase.SKELETON_SKIN
				}
			},
			{
				StorePurchase.SOVIET,
				new PurchaseInfo
				{
					Cost = new OnePay(2000, 0, once: true),
					Name = "SOVIET_SOLDIER",
					Tab = TabType.Skins,
					Icon = "soviet",
					Category = "SKIN_WORLD_WAR_II",
					Skin = 9
				}
			},
			{
				StorePurchase.AMERICAN,
				new PurchaseInfo
				{
					Cost = new OnePay(3000, 0, once: true),
					Name = "US_SOLDIER",
					Tab = TabType.Skins,
					Icon = "american",
					Category = "SKIN_WORLD_WAR_II",
					Skin = 11
				}
			},
			{
				StorePurchase.GERMAN,
				new PurchaseInfo
				{
					Cost = new OnePay(0, 30, once: true),
					Name = "GERMAN_OFICER",
					Tab = TabType.Skins,
					Icon = "german_22",
					Category = "SKIN_WORLD_WAR_II",
					Skin = 12
				}
			},
			{
				StorePurchase.SOLDIER,
				new PurchaseInfo
				{
					Cost = new OnePay(3500, 0, once: true),
					Name = "STALKER",
					Tab = TabType.Skins,
					Icon = "unit",
					Category = "SKIN_MODERNITY",
					Skin = 8
				}
			},
			{
				StorePurchase.SWAT,
				new PurchaseInfo
				{
					Cost = new OnePay(0, 35, once: true),
					Name = "SWAT",
					Tab = TabType.Skins,
					Icon = "swat",
					Category = "SKIN_MODERNITY",
					Skin = 10
				}
			},
			{
				StorePurchase.TERRORIST_SKIN,
				new PurchaseInfo
				{
					Cost = new OnePay(3500, 0, once: true),
					Name = "TERRORIST_SKIN",
					Tab = TabType.Skins,
					Icon = "terrorist_skin",
					Category = "SKIN_MODERNITY",
					Skin = 23
				}
			},
			{
				StorePurchase.DARK_STALKER_SKIN,
				new PurchaseInfo
				{
					Cost = new OnePay(0, 30, once: true),
					Name = "DARK_STALKER_SKIN",
					Tab = TabType.Skins,
					Icon = "SAS_skin",
					Category = "SKIN_MODERNITY",
					Skin = 22
				}
			},
			{
				StorePurchase.MERCENARY_SKIN,
				new PurchaseInfo
				{
					Cost = new OnePay(3200, 0, once: true),
					Name = "MERCENARY_SKIN",
					Tab = TabType.Skins,
					Icon = "mercenary_skin",
					Category = "SKIN_MODERNITY",
					Skin = 20
				}
			},
			{
				StorePurchase.IRON_KO_SKIN,
				new PurchaseInfo
				{
					Cost = new OnePay(4000, 0, once: true),
					Name = "IRON_KO",
					Tab = TabType.Skins,
					Icon = "iron_ko_icon",
					Category = "SKIN_SUPERHEROES",
					Skin = 15
				}
			},
			{
				StorePurchase.DARK_KO_SKIN,
				new PurchaseInfo
				{
					Cost = new OnePay(0, 50, once: true),
					Name = "DARK_KO",
					Tab = TabType.Skins,
					Icon = "icons_dark_ko",
					Category = "SKIN_SUPERHEROES",
					Skin = 16
				}
			},
			{
				StorePurchase.FLOWER,
				new PurchaseInfo
				{
					Cost = new TimedPay(50, 150, 0, 0, 0, 0),
					Name = "FLOWERS1",
					Tab = TabType.Plants,
					Icon = "plants3"
				}
			},
			{
				StorePurchase.FLOWER2,
				new PurchaseInfo
				{
					Cost = new TimedPay(70, 250, 0, 0, 0, 0),
					Name = "FLOWERS2",
					Tab = TabType.Plants,
					Icon = "plants2"
				}
			},
			{
				StorePurchase.FLOWER3,
				new PurchaseInfo
				{
					Cost = new TimedPay(100, 350, 0, 0, 0, 0),
					Name = "FLOWERS3",
					Tab = TabType.Plants,
					Icon = "plants1"
				}
			},
			{
				StorePurchase.BONUS_LIFE,
				new PurchaseInfo
				{
					Cost = new OnePay(10, 0, once: false),
					Name = "LIFE",
					Tab = TabType.Bonuses,
					Icon = "ico_box_ammo_health"
				}
			},
			{
				StorePurchase.BONUS_ARMOR,
				new PurchaseInfo
				{
					Cost = new OnePay(10, 0, once: false),
					Name = "ARMOR",
					Tab = TabType.Bonuses,
					Icon = "ico_box_ammo_armor"
				}
			},
			{
				StorePurchase.BONUS_WEAPONS,
				new PurchaseInfo
				{
					Cost = new OnePay(10, 0, once: false),
					Name = "WEAPONS",
					Tab = TabType.Bonuses,
					Icon = "ico_box_ammo_rifle"
				}
			},
			{
				StorePurchase.BOUNS_GRANADE,
				new PurchaseInfo
				{
					Cost = new OnePay(10, 0, once: false),
					Name = "GRENADES",
					Tab = TabType.Bonuses,
					Icon = "ico_box_ammo_grenade"
				}
			},
			{
				StorePurchase.BLOCK_KIND_FENCE,
				new PurchaseInfo
				{
					Cost = new OnePay(0, 5, once: true),
					Name = "BLOCK_KIND_FENCE",
					Tab = TabType.Blocks,
					Icon = "column_icon",
					Category = "BLOCK_FORMS"
				}
			},
			{
				StorePurchase.BLOCK_KIND_HALF,
				new PurchaseInfo
				{
					Cost = new OnePay(0, 10, once: true),
					Name = "BLOCK_KIND_HALF",
					Tab = TabType.Blocks,
					Icon = "half-block_icon",
					Category = "BLOCK_FORMS"
				}
			},
			{
				StorePurchase.BLOCK_KIND_DIAGONAL,
				new PurchaseInfo
				{
					Cost = new OnePay(0, 15, once: true),
					Name = "BLOCK_KIND_DIAGONAL",
					Tab = TabType.Blocks,
					Icon = "ramp_icon",
					Category = "BLOCK_FORMS"
				}
			},
			{
				StorePurchase.BLOCK_KIND_STAIR,
				new PurchaseInfo
				{
					Cost = new OnePay(0, 20, once: true),
					Name = "BLOCK_KIND_STAIR",
					Tab = TabType.Blocks,
					Icon = "stairs_icon",
					Category = "BLOCK_FORMS"
				}
			},
			{
				StorePurchase.CUBES4,
				new PurchaseInfo
				{
					Cost = new TimedPay(70, 250, 500, 0, 0, 0),
					Name = "WOODEN_BLOCKS",
					Tab = TabType.Blocks,
					Icon = "block_wood",
					Category = "BLOCK_TYPES"
				}
			},
			{
				StorePurchase.CUBES1,
				new PurchaseInfo
				{
					Cost = new TimedPay(100, 350, 700, 0, 0, 0),
					Name = "STONE_BLOCKS",
					Tab = TabType.Blocks,
					Icon = "block_stones_01",
					Category = "BLOCK_TYPES"
				}
			},
			{
				StorePurchase.CUBES2,
				new PurchaseInfo
				{
					Cost = new TimedPay(100, 350, 700, 0, 0, 0),
					Name = "STONE_BLOCKS",
					Tab = TabType.Blocks,
					Icon = "block_stones_02",
					Category = "BLOCK_TYPES"
				}
			},
			{
				StorePurchase.CUBES3,
				new PurchaseInfo
				{
					Cost = new TimedPay(150, 500, 1000, 0, 0, 0),
					Name = "COLORED_BLOCKS",
					Tab = TabType.Blocks,
					Icon = "block_colored",
					Category = "BLOCK_TYPES"
				}
			},
			{
				StorePurchase.GLASSCUBES,
				new PurchaseInfo
				{
					Cost = new TimedPay(180, 600, 1200, 0, 0, 0),
					Name = "GLASS_BLOCKS",
					Tab = TabType.Blocks,
					Icon = "block_glass",
					Category = "BLOCK_TYPES"
				}
			},
			{
				StorePurchase.RESHETKACUBES,
				new PurchaseInfo
				{
					Cost = new TimedPay(180, 600, 1200, 0, 0, 0),
					Name = "GRATES_BLOCKS",
					Tab = TabType.Blocks,
					Icon = "block_cell",
					Category = "BLOCK_TYPES"
				}
			},
			{
				StorePurchase.TILE_CUBES,
				new PurchaseInfo
				{
					Cost = new TimedPay(180, 600, 1200, 0, 0, 0),
					Name = "TILE_CUBES",
					Tab = TabType.Blocks,
					Icon = "block_tile",
					Category = "BLOCK_TYPES"
				}
			},
			{
				StorePurchase.ICE,
				new PurchaseInfo
				{
					Cost = new TimedPay(220, 800, 1600, 0, 0, 0),
					Name = "ICE_BLOCK",
					Tab = TabType.Blocks,
					Icon = "block_ice",
					Category = "BLOCK_TYPES"
				}
			},
			{
				StorePurchase.LAVA,
				new PurchaseInfo
				{
					Cost = new TimedPay(220, 800, 1600, 0, 0, 0),
					Name = "LAVA_BLOCK",
					Tab = TabType.Blocks,
					Icon = "block_lava",
					Category = "BLOCK_TYPES"
				}
			},
			{
				StorePurchase.WATER,
				new PurchaseInfo
				{
					Cost = new TimedPay(320, 1100, 1600, 0, 0, 0),
					Name = "WATER_BLOCK",
					Tab = TabType.Blocks,
					Icon = "block_water",
					Category = "BLOCK_TYPES"
				}
			},
			{
				StorePurchase.CUBE_BATUTE,
				new PurchaseInfo
				{
					Cost = new TimedPay(350, 1300, 3000, 0, 0, 0),
					Name = "TRAMPOLINE_BLOCK",
					Tab = TabType.Blocks,
					Icon = "block_trampoline",
					Category = "BLOCK_TYPES"
				}
			},
			{
				StorePurchase.TEAMCUBES,
				new PurchaseInfo
				{
					Cost = new TimedPay(400, 1500, 4000, 0, 0, 0),
					Name = "TEAM_BLOCKS",
					Tab = TabType.Blocks,
					Icon = "block_team",
					Category = "BLOCK_TYPES"
				}
			},
			{
				StorePurchase.TAVERN_CUBES,
				new PurchaseInfo
				{
					Cost = new OnePay(0, 5, once: true),
					Name = "TAVERN_CUBES",
					Tab = TabType.Blocks,
					Icon = "block_set_tavern",
					Category = "BLOCK_SETS"
				}
			},
			{
				StorePurchase.LIBRARY_CUBES,
				new PurchaseInfo
				{
					Cost = new OnePay(0, 7, once: true),
					Name = "LIBRARY_CUBES",
					Tab = TabType.Blocks,
					Icon = "block_set_library",
					Category = "BLOCK_SETS"
				}
			},
			{
				StorePurchase.DUNGEON_CUBES,
				new PurchaseInfo
				{
					Cost = new OnePay(0, 9, once: true),
					Name = "DUNGEON_CUBES",
					Tab = TabType.Blocks,
					Icon = "block_set_dungeon",
					Category = "BLOCK_SETS"
				}
			},
			{
				StorePurchase.FORTRESS_CUBES,
				new PurchaseInfo
				{
					Cost = new OnePay(0, 12, once: true),
					Name = "FORTRESS_CUBES",
					Tab = TabType.Blocks,
					Icon = "block_set_fortress",
					Category = "BLOCK_SETS"
				}
			},
			{
				StorePurchase.CASTLE_CUBES,
				new PurchaseInfo
				{
					Cost = new OnePay(0, 15, once: true),
					Name = "CASTLE_CUBES",
					Tab = TabType.Blocks,
					Icon = "block_set_castle",
					Category = "BLOCK_SETS"
				}
			},
			{
				StorePurchase.MILITARY_CUBES,
				new PurchaseInfo
				{
					Cost = new OnePay(0, 18, once: true),
					Name = "MILITARY_CUBES",
					Tab = TabType.Blocks,
					Icon = "block_set_military",
					Category = "BLOCK_SETS"
				}
			},
			{
				StorePurchase.MORE_EXPERIENCE,
				new PurchaseInfo
				{
					Cost = new TimedPay(0, 0, 0, 10, 30, 0, 0, 90),
					Name = "MORE_EXPERIENCE",
					Tab = TabType.Weapons,
					Icon = "premium_icon",
					LargeIcon = "premium_art_01",
					Category = "WEAPON_OTHER"
				}
			},
			{
				StorePurchase.WEAPON_GLOK,
				new PurchaseInfo
				{
					Disabled = true,
					Cost = new OnePay(500, 0, once: true),
					Name = "GLOCK",
					Tab = TabType.Weapons,
					Icon = "G18"
				}
			},
			{
				StorePurchase.WEAPON_MP5,
				new PurchaseInfo
				{
					Cost = new OnePay(1300, 0, once: true),
					Name = "MP5",
					Tab = TabType.Weapons,
					Icon = "mp5",
					Category = "WEAPON_TODAY"
				}
			},
			{
				StorePurchase.WEAPON_AK47,
				new PurchaseInfo
				{
					Cost = new OnePay(1900, 0, once: true),
					Name = "AK47",
					Tab = TabType.Weapons,
					Icon = "ak47",
					Category = "WEAPON_TODAY"
				}
			},
			{
				StorePurchase.WEAPON_M16,
				new PurchaseInfo
				{
					Cost = new OnePay(1900, 0, once: true),
					Name = "M4A",
					Tab = TabType.Weapons,
					Icon = "m16",
					Category = "WEAPON_TODAY"
				}
			},
			{
				StorePurchase.WEAPON_RIFLE,
				new PurchaseInfo
				{
					Cost = new OnePay(2300, 0, once: true),
					Name = "RIFLE",
					Tab = TabType.Weapons,
					Icon = "rifle",
					Category = "WEAPON_TODAY"
				}
			},
			{
				StorePurchase.WEAPON_SNIPER_RIFLE,
				new PurchaseInfo
				{
					Cost = new OnePay(0, 10, once: true),
					Name = "SNIPER_RIFLE",
					Tab = TabType.Weapons,
					Icon = "sniper_rifle",
					Category = "WEAPON_TODAY"
				}
			},
			{
				StorePurchase.WEAPON_M1014,
				new PurchaseInfo
				{
					Cost = new OnePay(2000, 0, once: true),
					Name = "SHOTGUN",
					Tab = TabType.Weapons,
					Icon = "m1014",
					Category = "WEAPON_TODAY"
				}
			},
			{
				StorePurchase.WEAPON_GRENADE_LAUNCHER,
				new PurchaseInfo
				{
					Cost = new OnePay(0, 15, once: true),
					Name = "GRENADE_GUN",
					Tab = TabType.Weapons,
					Icon = "grenade-launcher",
					Category = "WEAPON_TODAY"
				}
			},
			{
				StorePurchase.WEAPON_MP40,
				new PurchaseInfo
				{
					Cost = new OnePay(0, 5, once: true),
					Name = "MP40",
					Tab = TabType.Weapons,
					Icon = "big_mp40",
					Category = "WEAPON_WW2"
				}
			},
			{
				StorePurchase.WEAPON_PPSH,
				new PurchaseInfo
				{
					Cost = new OnePay(0, 10, once: true),
					Name = "PPSH",
					Tab = TabType.Weapons,
					Icon = "big_ppsh",
					Category = "WEAPON_WW2"
				}
			},
			{
				StorePurchase.WEAPON_THOMPSON,
				new PurchaseInfo
				{
					Cost = new OnePay(0, 15, once: true),
					Name = "THOMPSON",
					Tab = TabType.Weapons,
					Icon = "big_thompson",
					Category = "WEAPON_WW2"
				}
			},
			{
				StorePurchase.CHANGENICK,
				new PurchaseInfo
				{
					Disabled = true,
					Cost = new OnePay(1, 0, once: false),
					Name = "CHANGENICK",
					Tab = TabType.Tools,
					Icon = "кирка+"
				}
			},
			{
				StorePurchase.KIRCKA,
				new PurchaseInfo
				{
					Cost = new TimedPay(500, 1200, 0, 0, 0, 10),
					Name = "IMPROVED_PICK",
					Tab = TabType.Tools,
					Icon = "кирка+"
				}
			},
			{
				StorePurchase.SPEED,
				new PurchaseInfo
				{
					Cost = new TimedPay(500, 1200, 0, 0, 0, 10),
					Name = "SPEED_POTION",
					Tab = TabType.Tools,
					Icon = "icon speed shop 100x75"
				}
			},
			{
				StorePurchase.FLY,
				new PurchaseInfo
				{
					Cost = new TimedPay(500, 1200, 0, 0, 0, 10),
					Name = "FLIGHT_POTION",
					Tab = TabType.Tools,
					Icon = "icon flight shop 100x75"
				}
			},
			{
				StorePurchase.DINAMIT,
				new PurchaseInfo
				{
					Cost = new OnePay(100, 0, once: false),
					Name = "DYNAMITE",
					Tab = TabType.Tools,
					Icon = "TNT_s_menu"
				}
			},
			{
				StorePurchase.ATOMBOMB,
				new PurchaseInfo
				{
					Cost = new OnePay(300, 0, once: false),
					Name = "AIR_BOMB",
					Tab = TabType.Tools,
					Icon = "BOMB_s_menu_01"
				}
			},
			{
				StorePurchase.SPAWN_POINT,
				new PurchaseInfo
				{
					Cost = new OnePay(500, 0, once: false),
					Name = "RESPAWN",
					Tab = TabType.Tools,
					Icon = "checkpoint_flag_icon_02"
				}
			},
			{
				StorePurchase.TEAM_SPAWN_POINT,
				new PurchaseInfo
				{
					Cost = new OnePay(100, 0, once: false),
					Name = "TEAM_RESPAWN",
					Tab = TabType.Tools,
					Icon = "comand_spawn"
				}
			},
			{
				StorePurchase.FLAG,
				new PurchaseInfo
				{
					Cost = new OnePay(500, 0, once: false, 2),
					Name = "FLAG",
					Tab = TabType.Tools,
					Icon = "flags_01_icon_02"
				}
			},
			{
				StorePurchase.BOAT,
				new PurchaseInfo
				{
					Cost = new OnePay(750, 0, once: false),
					Name = "BLOAT",
					Tab = TabType.Tools,
					Icon = "boat_store"
				}
			},
			{
				StorePurchase.EMOTIONS,
				new PurchaseInfo
				{
					Cost = new OnePay(1000, 0, once: true),
					Name = "ADV_SMILES",
					Tab = TabType.Tools,
					Icon = "ico7"
				}
			},
			{
				StorePurchase.MEM_SMILES,
				new PurchaseInfo
				{
					Cost = new OnePay(0, 5, once: true),
					Name = "MEM_FACES",
					Tab = TabType.Tools,
					Icon = "icon"
				}
			},
			{
				StorePurchase.RAIL,
				new PurchaseInfo
				{
					Cost = new OnePay(100, 0, once: false, 12),
					Name = "RAILS",
					Tab = TabType.Tools,
					Icon = "rails_store"
				}
			},
			{
				StorePurchase.TROLLEY,
				new PurchaseInfo
				{
					Cost = new OnePay(900, 0, once: false),
					Name = "TROLLEY",
					Tab = TabType.Tools,
					Icon = "vagonetka_store"
				}
			},
			{
				StorePurchase.TIME_OF_DAY,
				new PurchaseInfo
				{
					Disabled = true,
					Cost = new TimedPay(500, 1200, 0, 0, 0, 0),
					Name = "DAY_NIGHT",
					Tab = TabType.Tools,
					Icon = "day_night"
				}
			},
			{
				StorePurchase.GOLD_KUBOK,
				new PurchaseInfo
				{
					Cost = new OnePay(300, 0, once: false),
					Name = "GOLD_CUP",
					Tab = TabType.Tools,
					Icon = "icon_cup_shop_100x75_001"
				}
			},
			{
				StorePurchase.TEAMDOORS,
				new PurchaseInfo
				{
					Cost = new OnePay(600, 0, once: false),
					Name = "TEAM_DOORS",
					Tab = TabType.Tools,
					Icon = "double-doors_team_02_icon_02"
				}
			},
			{
				StorePurchase.LOCK_DOOR,
				new PurchaseInfo
				{
					Cost = new OnePay(300, 0, once: false),
					Name = "LOCK_DOOR",
					Tab = TabType.Tools,
					Icon = "icon_shop_doors"
				}
			},
			{
				StorePurchase.LOCK_KEY,
				new PurchaseInfo
				{
					Cost = new OnePay(50, 0, once: false),
					Name = "LOCK_KEY",
					Tab = TabType.Tools,
					Icon = "icon_shop_keys"
				}
			},
			{
				StorePurchase.STOOL,
				new PurchaseInfo
				{
					Cost = new OnePay(10, 0, once: false),
					Name = "WOODEN_CHAIR",
					Tab = TabType.Decorations,
					Icon = "chair_wood",
					Category = "DECOR_MIDLE_AGE"
				}
			},
			{
				StorePurchase.CHAIR_STONE,
				new PurchaseInfo
				{
					Cost = new OnePay(30, 0, once: false),
					Name = "STONE_CHAIR",
					Tab = TabType.Decorations,
					Icon = "chair_stone",
					Category = "DECOR_MIDLE_AGE"
				}
			},
			{
				StorePurchase.BED,
				new PurchaseInfo
				{
					Cost = new OnePay(30, 0, once: false),
					Name = "SIMPLE_BED",
					Tab = TabType.Decorations,
					Icon = "bed_wood",
					Category = "DECOR_MIDLE_AGE"
				}
			},
			{
				StorePurchase.BED_STONE,
				new PurchaseInfo
				{
					Cost = new OnePay(90, 0, once: false),
					Name = "STONE_BED",
					Tab = TabType.Decorations,
					Icon = "bed_stone",
					Category = "DECOR_MIDLE_AGE"
				}
			},
			{
				StorePurchase.FENCE,
				new PurchaseInfo
				{
					Cost = new OnePay(10, 0, once: false),
					Name = "FENCE",
					Tab = TabType.Decorations,
					Icon = "fence_(tile)_wood",
					Category = "DECOR_MIDLE_AGE"
				}
			},
			{
				StorePurchase.FENCE_STONE,
				new PurchaseInfo
				{
					Cost = new OnePay(30, 0, once: false),
					Name = "STONE_FENCE",
					Tab = TabType.Decorations,
					Icon = "fence_(tile)_stone",
					Category = "DECOR_MIDLE_AGE"
				}
			},
			{
				StorePurchase.BACKET,
				new PurchaseInfo
				{
					Cost = new OnePay(10, 0, once: false),
					Name = "BUCKET",
					Tab = TabType.Decorations,
					Icon = "bucket",
					Category = "DECOR_MIDLE_AGE"
				}
			},
			{
				StorePurchase.BASKET,
				new PurchaseInfo
				{
					Cost = new OnePay(10, 0, once: false),
					Name = "BASKET",
					Tab = TabType.Decorations,
					Icon = "basket",
					Category = "DECOR_MIDLE_AGE"
				}
			},
			{
				StorePurchase.DOORW,
				new PurchaseInfo
				{
					Cost = new OnePay(30, 0, once: false),
					Name = "DOOR",
					Tab = TabType.Decorations,
					Icon = "door_wood",
					Category = "DECOR_MIDLE_AGE"
				}
			},
			{
				StorePurchase.TABLE,
				new PurchaseInfo
				{
					Cost = new OnePay(10, 0, once: false),
					Name = "WOODEN_TABLE",
					Tab = TabType.Decorations,
					Icon = "table_02_wood",
					Category = "DECOR_MIDLE_AGE"
				}
			},
			{
				StorePurchase.TABLE_STONE,
				new PurchaseInfo
				{
					Cost = new OnePay(30, 0, once: false),
					Name = "STONE_TABLE",
					Tab = TabType.Decorations,
					Icon = "table_02_stone",
					Category = "DECOR_MIDLE_AGE"
				}
			},
			{
				StorePurchase.BIG_TABLE,
				new PurchaseInfo
				{
					Cost = new OnePay(30, 0, once: false),
					Name = "BIG_TABLE",
					Tab = TabType.Decorations,
					Icon = "table_01_wood",
					Category = "DECOR_MIDLE_AGE"
				}
			},
			{
				StorePurchase.CHEST,
				new PurchaseInfo
				{
					Cost = new OnePay(30, 0, once: false),
					Name = "CHEST",
					Tab = TabType.Decorations,
					Icon = "chest_02",
					Category = "DECOR_MIDLE_AGE"
				}
			},
			{
				StorePurchase.CHEST_STONE,
				new PurchaseInfo
				{
					Cost = new OnePay(90, 0, once: false),
					Name = "STONE_CHEST",
					Tab = TabType.Decorations,
					Icon = "chest_01",
					Category = "DECOR_MIDLE_AGE"
				}
			},
			{
				StorePurchase.BARREL,
				new PurchaseInfo
				{
					Cost = new OnePay(15, 0, once: false),
					Name = "BARREL",
					Tab = TabType.Decorations,
					Icon = "barrel",
					Category = "DECOR_MIDLE_AGE"
				}
			},
			{
				StorePurchase.SIGN,
				new PurchaseInfo
				{
					Cost = new OnePay(15, 0, once: false),
					Name = "SIGNPOST",
					Tab = TabType.Decorations,
					Icon = "sign",
					Category = "DECOR_MIDLE_AGE"
				}
			},
			{
				StorePurchase.CUPBOARDS,
				new PurchaseInfo
				{
					Cost = new OnePay(30, 0, once: false),
					Name = "NIGHTSTAND",
					Tab = TabType.Decorations,
					Icon = "case_wood",
					Category = "DECOR_MIDLE_AGE"
				}
			},
			{
				StorePurchase.CUPBOARDL,
				new PurchaseInfo
				{
					Cost = new OnePay(30, 0, once: false),
					Name = "CUPBOARD",
					Tab = TabType.Decorations,
					Icon = "big_case_wood",
					Category = "DECOR_MIDLE_AGE"
				}
			},
			{
				StorePurchase.STAIRSW,
				new PurchaseInfo
				{
					Disabled = true,
					Cost = new OnePay(15, 0, once: false),
					Name = "STAIRS",
					Tab = TabType.Decorations,
					Icon = "stairs_w",
					Category = "DECOR_MIDLE_AGE"
				}
			},
			{
				StorePurchase.STAIRSB,
				new PurchaseInfo
				{
					Disabled = true,
					Cost = new OnePay(30, 0, once: false),
					Name = "STAIRS",
					Tab = TabType.Decorations,
					Icon = "stairs_s",
					Category = "DECOR_MIDLE_AGE"
				}
			},
			{
				StorePurchase.TORCHW,
				new PurchaseInfo
				{
					Cost = new OnePay(30, 0, once: false),
					Name = "WALL_TORCH",
					Tab = TabType.Decorations,
					Icon = "torch_01_wood",
					Category = "DECOR_MIDLE_AGE"
				}
			},
			{
				StorePurchase.TORCH_STONE,
				new PurchaseInfo
				{
					Cost = new OnePay(90, 0, once: false),
					Name = "STONE_WALL_TORCH",
					Tab = TabType.Decorations,
					Icon = "torch_01_stone",
					Category = "DECOR_MIDLE_AGE"
				}
			},
			{
				StorePurchase.TORCHF,
				new PurchaseInfo
				{
					Cost = new OnePay(30, 0, once: false),
					Name = "FLOOR_TORCH",
					Tab = TabType.Decorations,
					Icon = "torch_02_wood",
					Category = "DECOR_MIDLE_AGE"
				}
			},
			{
				StorePurchase.TORCH_FLOOR_STONE,
				new PurchaseInfo
				{
					Cost = new OnePay(90, 0, once: false),
					Name = "STONE_FLOOR_TORCH",
					Tab = TabType.Decorations,
					Icon = "torch_02_stone",
					Category = "DECOR_MIDLE_AGE"
				}
			},
			{
				StorePurchase.TABLICHKA,
				new PurchaseInfo
				{
					Cost = new OnePay(30, 0, once: false),
					Name = "FLOOR_PLATE",
					Tab = TabType.Decorations,
					Icon = "tablet_01",
					Category = "DECOR_MIDLE_AGE"
				}
			},
			{
				StorePurchase.TABLICHKAW,
				new PurchaseInfo
				{
					Cost = new OnePay(30, 0, once: false),
					Name = "WALL_PLATE",
					Tab = TabType.Decorations,
					Icon = "tablet_02",
					Category = "DECOR_MIDLE_AGE"
				}
			},
			{
				StorePurchase.BOOK,
				new PurchaseInfo
				{
					Cost = new OnePay(200, 0, once: false),
					Name = "BOOK",
					Tab = TabType.Decorations,
					Icon = "book_console_wood",
					Category = "DECOR_MIDLE_AGE"
				}
			},
			{
				StorePurchase.BENCH_WOOD,
				new PurchaseInfo
				{
					Cost = new OnePay(30, 0, once: false),
					Name = "WOODEN_BENCH",
					Tab = TabType.Decorations,
					Icon = "bench_wood",
					Category = "DECOR_MIDLE_AGE"
				}
			},
			{
				StorePurchase.BENCH_STONE,
				new PurchaseInfo
				{
					Cost = new OnePay(90, 0, once: false),
					Name = "STONE_BENCH",
					Tab = TabType.Decorations,
					Icon = "bench_stone",
					Category = "DECOR_MIDLE_AGE"
				}
			},
			{
				StorePurchase.LADDER,
				new PurchaseInfo
				{
					Cost = new OnePay(20, 0, once: false),
					Name = "LADDER",
					Tab = TabType.Decorations,
					Icon = "ladder",
					Category = "DECOR_MIDLE_AGE"
				}
			},
			{
				StorePurchase.TOMBSTONE_1,
				new PurchaseInfo
				{
					Cost = new OnePay(60, 0, once: false),
					Name = "TOMB",
					Tab = TabType.Decorations,
					Icon = "tombstone_01 (1)",
					Category = "DECOR_MIDLE_AGE"
				}
			},
			{
				StorePurchase.TOMBSTONE_2,
				new PurchaseInfo
				{
					Cost = new OnePay(60, 0, once: false),
					Name = "TOMB",
					Tab = TabType.Decorations,
					Icon = "tombstone_02",
					Category = "DECOR_MIDLE_AGE"
				}
			},
			{
				StorePurchase.FOOD_CAKE,
				new PurchaseInfo
				{
					Cost = new OnePay(0, 1, once: false),
					Name = "CAKE",
					Tab = TabType.Decorations,
					Icon = "cake",
					Category = "DECOR_FOOD"
				}
			},
			{
				StorePurchase.FOOD_WOOD_PLATE,
				new PurchaseInfo
				{
					Cost = new OnePay(20, 0, once: false),
					Name = "WOODEN_PLATE",
					Tab = TabType.Decorations,
					Icon = "wood_plate",
					Category = "DECOR_FOOD"
				}
			},
			{
				StorePurchase.FOOD_SPOON,
				new PurchaseInfo
				{
					Cost = new OnePay(10, 0, once: false),
					Name = "SPOON",
					Tab = TabType.Decorations,
					Icon = "spoon",
					Category = "DECOR_FOOD"
				}
			},
			{
				StorePurchase.FOOD_FORK,
				new PurchaseInfo
				{
					Cost = new OnePay(10, 0, once: false),
					Name = "FORCK",
					Tab = TabType.Decorations,
					Icon = "fork",
					Category = "DECOR_FOOD"
				}
			},
			{
				StorePurchase.FOOD_MUG,
				new PurchaseInfo
				{
					Cost = new OnePay(20, 0, once: false),
					Name = "MUG",
					Tab = TabType.Decorations,
					Icon = "mug",
					Category = "DECOR_FOOD"
				}
			},
			{
				StorePurchase.FOOD_JUG,
				new PurchaseInfo
				{
					Cost = new OnePay(25, 0, once: false),
					Name = "JUG",
					Tab = TabType.Decorations,
					Icon = "jug",
					Category = "DECOR_FOOD"
				}
			},
			{
				StorePurchase.FOOD_METAL_PLATE,
				new PurchaseInfo
				{
					Cost = new OnePay(30, 0, once: false),
					Name = "METAL_PLATE",
					Tab = TabType.Decorations,
					Icon = "metal_plate",
					Category = "DECOR_FOOD"
				}
			},
			{
				StorePurchase.FOOD_BREAD,
				new PurchaseInfo
				{
					Cost = new OnePay(10, 0, once: false),
					Name = "BREAD",
					Tab = TabType.Decorations,
					Icon = "bread",
					Category = "DECOR_FOOD"
				}
			},
			{
				StorePurchase.FOOD_CHEES,
				new PurchaseInfo
				{
					Cost = new OnePay(15, 0, once: false),
					Name = "CHEESE",
					Tab = TabType.Decorations,
					Icon = "cheese",
					Category = "DECOR_FOOD"
				}
			},
			{
				StorePurchase.FOOD_CHIKEN,
				new PurchaseInfo
				{
					Cost = new OnePay(30, 0, once: false),
					Name = "CHIKEN",
					Tab = TabType.Decorations,
					Icon = "chicken",
					Category = "DECOR_FOOD"
				}
			},
			{
				StorePurchase.FOOD_SVININA,
				new PurchaseInfo
				{
					Cost = new OnePay(50, 0, once: false),
					Name = "PIG",
					Tab = TabType.Decorations,
					Icon = "pork",
					Category = "DECOR_FOOD"
				}
			},
			{
				StorePurchase.FOOD_FISH,
				new PurchaseInfo
				{
					Cost = new OnePay(15, 0, once: false),
					Name = "FISH",
					Tab = TabType.Decorations,
					Icon = "fish",
					Category = "DECOR_FOOD"
				}
			},
			{
				StorePurchase.FOOD_STEIK,
				new PurchaseInfo
				{
					Cost = new OnePay(25, 0, once: false),
					Name = "STEAK",
					Tab = TabType.Decorations,
					Icon = "meat",
					Category = "DECOR_FOOD"
				}
			},
			{
				StorePurchase.FOOD_ORANGE,
				new PurchaseInfo
				{
					Cost = new OnePay(10, 0, once: false),
					Name = "ORANGE",
					Tab = TabType.Decorations,
					Icon = "orange",
					Category = "DECOR_FOOD"
				}
			},
			{
				StorePurchase.FOOD_PIZZA,
				new PurchaseInfo
				{
					Cost = new OnePay(50, 0, once: false),
					Name = "PIZZA",
					Tab = TabType.Decorations,
					Icon = "pizza",
					Category = "DECOR_FOOD"
				}
			},
			{
				StorePurchase.FOOD_COLA,
				new PurchaseInfo
				{
					Cost = new OnePay(20, 0, once: false),
					Name = "COLA",
					Tab = TabType.Decorations,
					Icon = "cola",
					Category = "DECOR_FOOD"
				}
			},
			{
				StorePurchase.FOOD_BURGER,
				new PurchaseInfo
				{
					Cost = new OnePay(25, 0, once: false),
					Name = "HAMBURGER",
					Tab = TabType.Decorations,
					Icon = "burger",
					Category = "DECOR_FOOD"
				}
			},
			{
				StorePurchase.FOOD_PIROG,
				new PurchaseInfo
				{
					Cost = new OnePay(20, 0, once: false),
					Name = "PIE",
					Tab = TabType.Decorations,
					Icon = "cheesecake",
					Category = "DECOR_FOOD"
				}
			},
			{
				StorePurchase.PICTURE1_1,
				new PurchaseInfo
				{
					Disabled = true,
					Cost = new OnePay(20, 0, once: false),
					Name = "PIE",
					Tab = TabType.Decorations,
					Icon = "picture_07"
				}
			},
			{
				StorePurchase.PAINTING_07,
				new PurchaseInfo
				{
					Cost = new OnePay(50, 0, once: false),
					Name = "PAINTING_07",
					Tab = TabType.Decorations,
					Icon = "painting_07",
					Category = "DECOR_PAINTINGS"
				}
			},
			{
				StorePurchase.PAINTING_08,
				new PurchaseInfo
				{
					Cost = new OnePay(50, 0, once: false),
					Name = "PAINTING_08",
					Tab = TabType.Decorations,
					Icon = "painting_08",
					Category = "DECOR_PAINTINGS"
				}
			},
			{
				StorePurchase.PAINTING_09,
				new PurchaseInfo
				{
					Cost = new OnePay(50, 0, once: false),
					Name = "PAINTING_09",
					Tab = TabType.Decorations,
					Icon = "painting_09",
					Category = "DECOR_PAINTINGS"
				}
			},
			{
				StorePurchase.PAINTING_10,
				new PurchaseInfo
				{
					Cost = new OnePay(50, 0, once: false),
					Name = "PAINTING_10",
					Tab = TabType.Decorations,
					Icon = "painting_10",
					Category = "DECOR_PAINTINGS"
				}
			},
			{
				StorePurchase.PAINTING_03,
				new PurchaseInfo
				{
					Cost = new OnePay(75, 0, once: false),
					Name = "PAINTING_03",
					Tab = TabType.Decorations,
					Icon = "painting_03",
					Category = "DECOR_PAINTINGS"
				}
			},
			{
				StorePurchase.PAINTING_04,
				new PurchaseInfo
				{
					Cost = new OnePay(75, 0, once: false),
					Name = "PAINTING_04",
					Tab = TabType.Decorations,
					Icon = "painting_04",
					Category = "DECOR_PAINTINGS"
				}
			},
			{
				StorePurchase.PAINTING_05,
				new PurchaseInfo
				{
					Cost = new OnePay(75, 0, once: false),
					Name = "PAINTING_05",
					Tab = TabType.Decorations,
					Icon = "painting_05",
					Category = "DECOR_PAINTINGS"
				}
			},
			{
				StorePurchase.PAINTING_06,
				new PurchaseInfo
				{
					Cost = new OnePay(75, 0, once: false),
					Name = "PAINTING_06",
					Tab = TabType.Decorations,
					Icon = "painting_06",
					Category = "DECOR_PAINTINGS"
				}
			},
			{
				StorePurchase.PAINTING_12,
				new PurchaseInfo
				{
					Cost = new OnePay(100, 0, once: false),
					Name = "PAINTING_12",
					Tab = TabType.Decorations,
					Icon = "painting_12",
					Category = "DECOR_PAINTINGS"
				}
			},
			{
				StorePurchase.PAINTING_13,
				new PurchaseInfo
				{
					Cost = new OnePay(100, 0, once: false),
					Name = "PAINTING_13",
					Tab = TabType.Decorations,
					Icon = "painting_13",
					Category = "DECOR_PAINTINGS"
				}
			},
			{
				StorePurchase.PAINTING_01,
				new PurchaseInfo
				{
					Cost = new OnePay(200, 0, once: false),
					Name = "PAINTING_01",
					Tab = TabType.Decorations,
					Icon = "painting_01",
					Category = "DECOR_PAINTINGS"
				}
			},
			{
				StorePurchase.PAINTING_02,
				new PurchaseInfo
				{
					Cost = new OnePay(200, 0, once: false),
					Name = "PAINTING_02",
					Tab = TabType.Decorations,
					Icon = "painting_02",
					Category = "DECOR_PAINTINGS"
				}
			},
			{
				StorePurchase.MILITARY_ARMORY,
				new PurchaseInfo
				{
					Cost = new OnePay(75, 0, once: false),
					Name = "MILITARY_ARMORY",
					Tab = TabType.Decorations,
					Icon = "m_armory",
					Category = "DECOR_MILITARY"
				}
			},
			{
				StorePurchase.MILITARY_BARREL,
				new PurchaseInfo
				{
					Cost = new OnePay(30, 0, once: false),
					Name = "MILITARY_BARREL",
					Tab = TabType.Decorations,
					Icon = "m_barrel",
					Category = "DECOR_MILITARY"
				}
			},
			{
				StorePurchase.MILITARY_BED,
				new PurchaseInfo
				{
					Cost = new OnePay(60, 0, once: false),
					Name = "MILITARY_BED",
					Tab = TabType.Decorations,
					Icon = "m_bed",
					Category = "DECOR_MILITARY"
				}
			},
			{
				StorePurchase.MILITARY_CAM,
				new PurchaseInfo
				{
					Cost = new OnePay(100, 0, once: false),
					Name = "MILITARY_CAM",
					Tab = TabType.Decorations,
					Icon = "m_cam",
					Category = "DECOR_MILITARY"
				}
			},
			{
				StorePurchase.MILITARY_CASE_BIG,
				new PurchaseInfo
				{
					Cost = new OnePay(90, 0, once: false),
					Name = "MILITARY_CASE_BIG",
					Tab = TabType.Decorations,
					Icon = "m_case_big",
					Category = "DECOR_MILITARY"
				}
			},
			{
				StorePurchase.MILITARY_CASE_SMALL,
				new PurchaseInfo
				{
					Cost = new OnePay(60, 0, once: false),
					Name = "MILITARY_CASE_SMALL",
					Tab = TabType.Decorations,
					Icon = "m_case_small",
					Category = "DECOR_MILITARY"
				}
			},
			{
				StorePurchase.MILITARY_CHAIR,
				new PurchaseInfo
				{
					Cost = new OnePay(20, 0, once: false),
					Name = "MILITARY_CHAIR",
					Tab = TabType.Decorations,
					Icon = "m_chair",
					Category = "DECOR_MILITARY"
				}
			},
			{
				StorePurchase.MILITARY_CHEST,
				new PurchaseInfo
				{
					Cost = new OnePay(60, 0, once: false),
					Name = "MILITARY_CHEST",
					Tab = TabType.Decorations,
					Icon = "m_chest",
					Category = "DECOR_MILITARY"
				}
			},
			{
				StorePurchase.MILITARY_CONSOLE_01,
				new PurchaseInfo
				{
					Cost = new OnePay(50, 0, once: false),
					Name = "MILITARY_CONSOLE_01",
					Tab = TabType.Decorations,
					Icon = "m_console_01",
					Category = "DECOR_MILITARY"
				}
			},
			{
				StorePurchase.MILITARY_CONSOLE_02,
				new PurchaseInfo
				{
					Cost = new OnePay(50, 0, once: false),
					Name = "MILITARY_CONSOLE_02",
					Tab = TabType.Decorations,
					Icon = "m_console_02",
					Category = "DECOR_MILITARY"
				}
			},
			{
				StorePurchase.MILITARY_CONSOLE_03,
				new PurchaseInfo
				{
					Cost = new OnePay(50, 0, once: false),
					Name = "MILITARY_CONSOLE_03",
					Tab = TabType.Decorations,
					Icon = "m_console_03",
					Category = "DECOR_MILITARY"
				}
			},
			{
				StorePurchase.MILITARY_DOOR,
				new PurchaseInfo
				{
					Cost = new OnePay(60, 0, once: false),
					Name = "MILITARY_DOOR",
					Tab = TabType.Decorations,
					Icon = "m_door",
					Category = "DECOR_MILITARY"
				}
			},
			{
				StorePurchase.MILITARY_LAMP,
				new PurchaseInfo
				{
					Cost = new OnePay(60, 0, once: false),
					Name = "MILITARY_LAMP",
					Tab = TabType.Decorations,
					Icon = "m_lamp",
					Category = "DECOR_MILITARY"
				}
			},
			{
				StorePurchase.MILITARY_LANTERN,
				new PurchaseInfo
				{
					Cost = new OnePay(60, 0, once: false),
					Name = "MILITARY_LANTERN",
					Tab = TabType.Decorations,
					Icon = "m_lantern",
					Category = "DECOR_MILITARY"
				}
			},
			{
				StorePurchase.MILITARY_MUG,
				new PurchaseInfo
				{
					Cost = new OnePay(40, 0, once: false),
					Name = "MILITARY_MUG",
					Tab = TabType.Decorations,
					Icon = "m_mug",
					Category = "DECOR_MILITARY"
				}
			},
			{
				StorePurchase.MILITARY_PC,
				new PurchaseInfo
				{
					Cost = new OnePay(100, 0, once: false),
					Name = "MILITARY_PC",
					Tab = TabType.Decorations,
					Icon = "m_PC",
					Category = "DECOR_MILITARY"
				}
			},
			{
				StorePurchase.MILITARY_TABLE_BIG,
				new PurchaseInfo
				{
					Cost = new OnePay(60, 0, once: false),
					Name = "MILITARY_TABLE_BIG",
					Tab = TabType.Decorations,
					Icon = "m_table_big",
					Category = "DECOR_MILITARY"
				}
			},
			{
				StorePurchase.MILITARY_TABLE_SMALL,
				new PurchaseInfo
				{
					Cost = new OnePay(20, 0, once: false),
					Name = "MILITARY_TABLE_SMALL",
					Tab = TabType.Decorations,
					Icon = "m_table_small",
					Category = "DECOR_MILITARY"
				}
			},
			{
				StorePurchase.MILITARY_FENCE,
				new PurchaseInfo
				{
					Cost = new OnePay(60, 0, once: false),
					Name = "MILITARY_FENCE",
					Tab = TabType.Decorations,
					Icon = "m_fence_(X)",
					Category = "DECOR_MILITARY"
				}
			},
			{
				StorePurchase.MILITARY_BENCH,
				new PurchaseInfo
				{
					Cost = new OnePay(60, 0, once: false),
					Name = "MILITARY_BENCH",
					Tab = TabType.Decorations,
					Icon = "m_bench",
					Category = "DECOR_MILITARY"
				}
			},
			{
				StorePurchase.MILITARY_BOX_01,
				new PurchaseInfo
				{
					Cost = new OnePay(30, 0, once: false),
					Name = "MILITARY_BOX_01",
					Tab = TabType.Decorations,
					Icon = "m_box_01",
					Category = "DECOR_MILITARY"
				}
			},
			{
				StorePurchase.MILITARY_BOX_02,
				new PurchaseInfo
				{
					Cost = new OnePay(30, 0, once: false),
					Name = "MILITARY_BOX_02",
					Tab = TabType.Decorations,
					Icon = "m_box_02",
					Category = "DECOR_MILITARY"
				}
			},
			{
				StorePurchase.MILITARY_BRIEFING_BOARD,
				new PurchaseInfo
				{
					Cost = new OnePay(150, 0, once: false),
					Name = "MILITARY_BRIEFING_BOARD",
					Tab = TabType.Decorations,
					Icon = "m_briefing_board",
					Category = "DECOR_MILITARY"
				}
			}
		};
		Gems = new SecuredValue<int>[6][]
		{
			new SecuredValue<int>[15]
			{
				3, 6, 0, 5, 10, 5, 10, 20, 10, 20,
				40, 20, 40, 80, 40
			},
			new SecuredValue<int>[15]
			{
				15, 6, 0, 38, 10, 5, 75, 20, 10, 150,
				40, 20, 300, 80, 40
			},
			new SecuredValue<int>[15]
			{
				25, 6, 0, 50, 12, 0, 100, 24, 0, 250,
				60, 5, 500, 120, 15
			},
			new SecuredValue<int>[15]
			{
				25, 6, 0, 40, 10, 5, 80, 20, 10, 160,
				40, 20, 220, 80, 40
			},
			new SecuredValue<int>[15]
			{
				1, 6, 0, 4, 24, 5, 10, 60, 12, 20,
				120, 30, 50, 260, 70
			},
			new SecuredValue<int>[15]
			{
				1, 6, 0, 4, 24, 5, 10, 60, 12, 20,
				120, 30, 50, 260, 70
			}
		};
		Gold = new SecuredValue<int>[15]
		{
			1, 100, 0, 3, 300, 0, 5, 500, 0, 10,
			1000, 0, 30, 3000, 0
		};
		StandartMaps = new SecuredValue<StandartMap>[16]
		{
			new StandartMap
			{
				Game = GameINI.GameType.DEATHMATCH,
				Destroyable = false,
				Name = "Заправка",
				Description = string.Empty,
				SpriteName = "sandoil",
				MapURL = "diggerworld.ru/standart_maps/zapravka.map"
			},
			new StandartMap
			{
				Game = GameINI.GameType.DEATHMATCH,
				Destroyable = false,
				Name = "Космодром",
				Description = string.Empty,
				SpriteName = "cosmo",
				MapURL = "diggerworld.ru/standart_maps/dm_cosmodrom.map"
			},
			new StandartMap
			{
				Game = GameINI.GameType.DEATHMATCH,
				Destroyable = false,
				Name = "Город в пустыне",
				Description = string.Empty,
				SpriteName = "sand",
				MapURL = "diggerworld.ru/standart_maps/dm_sand.map"
			},
			new StandartMap
			{
				Game = GameINI.GameType.TEAM_BATTLE,
				Destroyable = false,
				Name = "Бабочка Смерти",
				Description = string.Empty,
				SpriteName = "babochka",
				MapURL = "diggerworld.ru/standart_maps/tm_babochka.map"
			},
			new StandartMap
			{
				Game = GameINI.GameType.TEAM_BATTLE,
				Destroyable = false,
				Name = "Поместье",
				Description = string.Empty,
				SpriteName = "mantion",
				MapURL = "diggerworld.ru/standart_maps/tm_mantion.map"
			},
			new StandartMap
			{
				Game = GameINI.GameType.TEAM_BATTLE,
				Destroyable = false,
				Name = "Лесопилка",
				Description = string.Empty,
				SpriteName = "lesopilka",
				MapURL = "diggerworld.ru/standart_maps/tm_lesopilka.map"
			},
			new StandartMap
			{
				Game = GameINI.GameType.TEAM_BATTLE,
				Destroyable = false,
				Name = "Тюрьма 2",
				Description = string.Empty,
				SpriteName = "sector13",
				MapURL = "diggerworld.ru/standart_maps/tm_ifon_prison.map"
			},
			new StandartMap
			{
				Game = GameINI.GameType.TEAM_BATTLE,
				Destroyable = false,
				Name = "Новый мост",
				Description = string.Empty,
				SpriteName = "most2",
				MapURL = "diggerworld.ru/standart_maps/tm_most2.map"
			},
			new StandartMap
			{
				Game = GameINI.GameType.TEAM_BATTLE,
				Destroyable = false,
				Name = "Замок",
				Description = string.Empty,
				SpriteName = "zamok",
				MapURL = "diggerworld.ru/standart_maps/tm_zamok.map"
			},
			new StandartMap
			{
				Game = GameINI.GameType.TEAM_BATTLE,
				Destroyable = false,
				Name = "Тюрьма",
				Description = string.Empty,
				SpriteName = "Prisonsmall",
				MapURL = "diggerworld.ru/standart_maps/tm_prizon.map"
			},
			new StandartMap
			{
				Game = GameINI.GameType.TEAM_BATTLE,
				Destroyable = false,
				Name = "Битва в пустыне",
				Description = string.Empty,
				SpriteName = "DessertWarsmall",
				MapURL = "diggerworld.ru/standart_maps/team_dessertwars.map"
			},
			new StandartMap
			{
				Game = GameINI.GameType.CTF,
				Destroyable = false,
				Name = "Тюрьма 2",
				Description = string.Empty,
				SpriteName = "sector13",
				MapURL = "diggerworld.ru/standart_maps/ctf_ifon_prison.map"
			},
			new StandartMap
			{
				Game = GameINI.GameType.CTF,
				Destroyable = false,
				Name = "Разрушенный мост",
				Description = string.Empty,
				SpriteName = "most1",
				MapURL = "diggerworld.ru/standart_maps/ctf_most2.map"
			},
			new StandartMap
			{
				Game = GameINI.GameType.CTF,
				Destroyable = false,
				Name = "Тюрьма под водой",
				Description = string.Empty,
				SpriteName = "atlantida",
				MapURL = "diggerworld.ru/standart_maps/ctf_atlantida.map"
			},
			new StandartMap
			{
				Game = GameINI.GameType.ZOMBIE_VIRUS,
				Destroyable = false,
				Name = "Заправка",
				Description = string.Empty,
				SpriteName = "sandoil",
				MapURL = "diggerworld.ru/standart_maps/zm_zapravka.map"
			},
			new StandartMap
			{
				Game = GameINI.GameType.ZOMBIE_VIRUS,
				Destroyable = false,
				Name = "Город в пустыне",
				Description = string.Empty,
				SpriteName = "sand",
				MapURL = "diggerworld.ru/standart_maps/zm_sand.map"
			}
		};
		string path = "..\\KopatelOnlineServer\\Server\\";
		if (Application.isEditor && Directory.Exists(path))
		{
			GeneratePurchasesForServer(path);
		}
		string path2 = "..\\KopatelOnlinePhoton\\src-server\\Plugins\\Custom\\Qmax\\Qmax.Kopatel.HivePlugin\\";
		if (Application.isEditor && Directory.Exists(path2))
		{
			GenerateDataForPhotonServer(path2);
		}
	}

	private static void GeneratePurchasesForServer(string path)
	{
		StreamWriter streamWriter = new StreamWriter(path + "purchase_list.php");
		streamWriter.WriteLine("<?php");
		streamWriter.WriteLine("$purchases = array(");
		foreach (KeyValuePair<StorePurchase, PurchaseInfo> purchase in Purchases)
		{
			if (!purchase.Value.Disabled)
			{
				Pay cost = purchase.Value.Cost;
				string text = "\t" + (int)purchase.Key + " => array(";
				if (cost is OnePay)
				{
					OnePay onePay = (OnePay)cost;
					string text2 = text;
					text = text2 + "'one', " + (int)onePay.Coins + ", " + (int)onePay.Gems + ", " + ((bool)onePay.Once).ToString().ToLower() + ", " + (int)onePay.Count;
				}
				else
				{
					TimedPay timedPay = (TimedPay)cost;
					string text2 = text;
					text = text2 + "'timed', " + (int)timedPay.DayCoins + ", " + (int)timedPay.WeekCoins + ", " + (int)timedPay.ForeverCoins + ", " + (int)timedPay.MonthCoins + ", " + (int)timedPay.DayGems + ", " + (int)timedPay.WeekGems + ", " + (int)timedPay.ForeverGems + ", " + (int)timedPay.MonthGems;
				}
				text = text + "), // " + purchase.Key;
				streamWriter.WriteLine(text);
			}
		}
		streamWriter.WriteLine(");");
		streamWriter.WriteLine("?>");
		streamWriter.Close();
	}

	private static void GenerateDataForPhotonServer(string path)
	{
		StreamWriter streamWriter = new StreamWriter(path + "Data.cs");
		streamWriter.WriteLine("using System;\r\nusing System.Collections.Generic;\r\n\r\nnamespace Qmax.Kopatel\r\n{");
		streamWriter.WriteLine("\tpublic class Data");
		streamWriter.WriteLine("\t{");
		streamWriter.WriteLine("\t\tstatic public string[] RpcList = new string[]");
		streamWriter.WriteLine("\t\t{");
		foreach (string rpc in PhotonNetwork.PhotonServerSettings.RpcList)
		{
			streamWriter.WriteLine("\t\t\t\"" + rpc + "\",");
		}
		streamWriter.WriteLine("\t\t};");
		streamWriter.WriteLine("\t};");
		streamWriter.WriteLine();
		streamWriter.WriteLine("\tpublic class Purchases\r\n\t{");
		streamWriter.WriteLine("\t\tstatic public int None = " + 5 + ";");
		streamWriter.WriteLine("\t\tstatic public int MoreExperience = " + 197 + ";");
		streamWriter.WriteLine(string.Empty);
		streamWriter.WriteLine("\t\tstatic public Dictionary<int, string> EntityNames = new Dictionary<int, string>()");
		streamWriter.WriteLine("\t\t{");
		string[] names = Enum.GetNames(typeof(EntityType));
		Array values = Enum.GetValues(typeof(EntityType));
		for (int i = 0; i < names.Length; i++)
		{
			streamWriter.WriteLine("\t\t\t{ " + (int)values.GetValue(i) + ", \"" + names[i] + "\" },");
		}
		streamWriter.WriteLine("\t\t};");
		streamWriter.WriteLine(string.Empty);
		streamWriter.WriteLine("\t\tstatic public Dictionary<int, string> BlockNames = new Dictionary<int, string>()");
		streamWriter.WriteLine("\t\t{");
		string[] names2 = Enum.GetNames(typeof(BlockType));
		Array values2 = Enum.GetValues(typeof(BlockType));
		for (int j = 0; j < names2.Length; j++)
		{
			streamWriter.WriteLine("\t\t\t{ " + (int)(byte)values2.GetValue(j) + ", \"" + names2[j] + "\" },");
		}
		streamWriter.WriteLine("\t\t};");
		streamWriter.WriteLine(string.Empty);
		streamWriter.WriteLine("\t\tstatic public Dictionary<int, string> PurchaseNames = new Dictionary<int, string>()");
		streamWriter.WriteLine("\t\t{");
		string[] names3 = Enum.GetNames(typeof(StorePurchase));
		Array values3 = Enum.GetValues(typeof(StorePurchase));
		for (int k = 0; k < names3.Length; k++)
		{
			streamWriter.WriteLine("\t\t\t{ " + (int)values3.GetValue(k) + ", \"" + names3[k] + "\" },");
		}
		streamWriter.WriteLine("\t\t};");
		streamWriter.WriteLine(string.Empty);
		streamWriter.WriteLine("\t\tstatic public Dictionary<int, int> EntityToPurchase = new Dictionary<int, int>()");
		streamWriter.WriteLine("\t\t{");
		foreach (KeyValuePair<EntityType, EntityInfo> entity in Entities)
		{
			streamWriter.WriteLine("\t\t\t{ " + (int)entity.Key + ", " + (int)entity.Value.Purchase.Value + " },");
		}
		streamWriter.WriteLine("\t\t};");
		streamWriter.WriteLine(string.Empty);
		streamWriter.WriteLine("\t\tstatic public Dictionary<int, int> BlockToPurchase = new Dictionary<int, int>()");
		streamWriter.WriteLine("\t\t{");
		foreach (KeyValuePair<BlockType, BlockInfo> block in Blocks)
		{
			streamWriter.WriteLine("\t\t\t{ " + (int)block.Key + ", " + (int)block.Value.Purchase.Value + " },");
		}
		streamWriter.WriteLine("\t\t};");
		streamWriter.WriteLine(string.Empty);
		streamWriter.WriteLine("\t\tstatic public Dictionary<int, int> BlockKindToPurchase = new Dictionary<int, int>()");
		streamWriter.WriteLine("\t\t{");
		streamWriter.WriteLine("\t\t\t{ " + 1 + ", " + 158 + " },");
		streamWriter.WriteLine("\t\t\t{ " + 2 + ", " + 157 + " },");
		streamWriter.WriteLine("\t\t\t{ " + 3 + ", " + 159 + " },");
		streamWriter.WriteLine("\t\t\t{ " + 4 + ", " + 159 + " },");
		streamWriter.WriteLine("\t\t\t{ " + 5 + ", " + 159 + " },");
		streamWriter.WriteLine("\t\t\t{ " + 6 + ", " + 159 + " },");
		streamWriter.WriteLine("\t\t\t{ " + 7 + ", " + 160 + " },");
		streamWriter.WriteLine("\t\t\t{ " + 8 + ", " + 160 + " },");
		streamWriter.WriteLine("\t\t\t{ " + 9 + ", " + 160 + " },");
		streamWriter.WriteLine("\t\t\t{ " + 10 + ", " + 160 + " },");
		streamWriter.WriteLine("\t\t};");
		streamWriter.WriteLine(string.Empty);
		streamWriter.WriteLine("\t\tstatic public HashSet<int> OnePayPurchases = new HashSet<int>()");
		streamWriter.WriteLine("\t\t{");
		foreach (KeyValuePair<StorePurchase, PurchaseInfo> purchase in Purchases)
		{
			if (purchase.Value.Cost is OnePay)
			{
				streamWriter.WriteLine("\t\t\t" + (int)purchase.Key + ",");
			}
		}
		streamWriter.WriteLine("\t\t};");
		streamWriter.WriteLine(string.Empty);
		streamWriter.WriteLine("\t\tstatic public HashSet<int> TimedPurchases = new HashSet<int>()");
		streamWriter.WriteLine("\t\t{");
		foreach (KeyValuePair<StorePurchase, PurchaseInfo> purchase2 in Purchases)
		{
			if (purchase2.Value.Cost is TimedPay)
			{
				streamWriter.WriteLine("\t\t\t" + (int)purchase2.Key + ",");
			}
		}
		streamWriter.WriteLine("\t\t};");
		streamWriter.WriteLine(string.Empty);
		streamWriter.WriteLine("\t\tstatic public HashSet<int> OncePayPurchases = new HashSet<int>()");
		streamWriter.WriteLine("\t\t{");
		foreach (KeyValuePair<StorePurchase, PurchaseInfo> purchase3 in Purchases)
		{
			if (purchase3.Value.Cost is OnePay && (bool)((OnePay)purchase3.Value.Cost).Once)
			{
				streamWriter.WriteLine("\t\t\t" + (int)purchase3.Key + ",");
			}
		}
		streamWriter.WriteLine("\t\t};");
		streamWriter.WriteLine("\t}\r\n}");
		streamWriter.Close();
	}
}
