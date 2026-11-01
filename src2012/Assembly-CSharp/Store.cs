using System.Collections.Generic;
using UnityEngine;

public class Store : MonoBehaviour
{
	private enum StoreWindow
	{
		DECORATIONS,
		PLANTS,
		MOBS,
		BLOCKS_WOOD,
		BLOCKS_STONE,
		BLOCKS_NATURE,
		BLOCKS_COLORED,
		BLOCKS_GLASS,
		BLOCKS_RESHETKI
	}

	public GUISkin guiSkin;

	public GUISkin guiBox;

	public static Store INSTANCE;

	public Texture2D[] vBlockWood;

	public BlockType[] vBlockWoodId;

	public Texture2D[] vBlockStone;

	public BlockType[] vBlockStoneId;

	public Texture2D[] vBlockColored;

	public BlockType[] vBlockColoredId;

	public Texture2D[] vBlockNature;

	public BlockType[] vBlockNatureId;

	public Texture2D[] vBlockGlass;

	public BlockType[] vBlockGlassId;

	public Texture2D[] vBlockReshetki;

	public BlockType[] vBlockReshetkiId;

	public Texture2D[] vDecorItem;

	public EntityType[] vDecorId;

	public Texture2D[] vMobsItem;

	public EntityType[] vMobsId;

	public Texture2D[] vTreesItem;

	public Texture2D[] vPlantItem;

	public EntityType[] vPlantId;

	public Texture2D[] vToolsItem;

	private Dictionary<int, int> _EntitiesIndicies;

	private Dictionary<int, int> _BlocksIndicies;

	private int _IndiciesOffset;

	private Vector2 scrollPosition;

	public bool showMenu = true;

	private int mSeek;

	private StoreWindow curWindow = StoreWindow.BLOCKS_NATURE;

	public int count_start_x;

	public int count_start_y;

	public int count_dx;

	public int count_dy;

	private int count_pos_x;

	private int count_pos_y;

	private void CreateSecuredLists()
	{
		_IndiciesOffset = Random.Range(776543, 32845112);
		_EntitiesIndicies = new Dictionary<int, int>();
		for (int i = 0; i < vDecorId.Length; i++)
		{
			_EntitiesIndicies.Add((int)(vDecorId[i] + _IndiciesOffset), i + _IndiciesOffset);
		}
		for (int j = 0; j < vMobsId.Length; j++)
		{
			_EntitiesIndicies.Add((int)(vMobsId[j] + _IndiciesOffset), j + _IndiciesOffset);
		}
		for (int k = 0; k < vPlantId.Length; k++)
		{
			_EntitiesIndicies.Add((int)(vPlantId[k] + _IndiciesOffset), k + _IndiciesOffset);
		}
		_BlocksIndicies = new Dictionary<int, int>();
		for (int l = 0; l < vBlockWoodId.Length; l++)
		{
			_BlocksIndicies.Add((int)vBlockWoodId[l] + _IndiciesOffset, l + _IndiciesOffset);
		}
		for (int m = 0; m < vBlockStoneId.Length; m++)
		{
			_BlocksIndicies.Add((int)vBlockStoneId[m] + _IndiciesOffset, m + _IndiciesOffset);
		}
		for (int n = 0; n < vBlockColoredId.Length; n++)
		{
			_BlocksIndicies.Add((int)vBlockColoredId[n] + _IndiciesOffset, n + _IndiciesOffset);
		}
		for (int num = 0; num < vBlockNatureId.Length; num++)
		{
			_BlocksIndicies.Add((int)vBlockNatureId[num] + _IndiciesOffset, num + _IndiciesOffset);
		}
		for (int num2 = 0; num2 < vBlockGlassId.Length; num2++)
		{
			_BlocksIndicies.Add((int)vBlockGlassId[num2] + _IndiciesOffset, num2 + _IndiciesOffset);
		}
		for (int num3 = 0; num3 < vBlockReshetkiId.Length; num3++)
		{
			_BlocksIndicies.Add((int)vBlockReshetkiId[num3] + _IndiciesOffset, num3 + _IndiciesOffset);
		}
	}

	private void CheckSecuredEntity(EntityType entityType, int index)
	{
		if (!_EntitiesIndicies.TryGetValue((int)(entityType + _IndiciesOffset), out var value) || index + _IndiciesOffset != value)
		{
			PhotonNetwork.Disconnect();
		}
	}

	private void CheckSecuredBlock(BlockType blockType, int index)
	{
		if (!_BlocksIndicies.TryGetValue((int)blockType + _IndiciesOffset, out var value) || index + _IndiciesOffset != value)
		{
			PhotonNetwork.Disconnect();
		}
	}

	private EntityType GetCurItemId(int ind)
	{
		EntityType entityType = EntityType.AIR;
		if (curWindow == StoreWindow.DECORATIONS)
		{
			entityType = vDecorId[ind];
		}
		if (curWindow == StoreWindow.PLANTS)
		{
			entityType = vPlantId[ind];
		}
		if (curWindow == StoreWindow.MOBS)
		{
			entityType = vMobsId[ind];
		}
		CheckSecuredEntity(entityType, ind);
		return entityType;
	}

	private BlockType GetCurBlockId(int ind)
	{
		BlockType blockType = BlockType.Air;
		if (curWindow == StoreWindow.BLOCKS_WOOD)
		{
			blockType = vBlockWoodId[ind];
		}
		if (curWindow == StoreWindow.BLOCKS_COLORED)
		{
			blockType = vBlockColoredId[ind];
		}
		if (curWindow == StoreWindow.BLOCKS_STONE)
		{
			blockType = vBlockStoneId[ind];
		}
		if (curWindow == StoreWindow.BLOCKS_NATURE)
		{
			blockType = vBlockNatureId[ind];
		}
		if (curWindow == StoreWindow.BLOCKS_GLASS)
		{
			blockType = vBlockGlassId[ind];
		}
		if (curWindow == StoreWindow.BLOCKS_RESHETKI)
		{
			blockType = vBlockReshetkiId[ind];
		}
		CheckSecuredBlock(blockType, ind);
		return blockType;
	}

	private Texture2D GetCurItemSprite(int ind)
	{
		if (curWindow == StoreWindow.DECORATIONS)
		{
			return vDecorItem[ind];
		}
		if (curWindow == StoreWindow.PLANTS)
		{
			return vPlantItem[ind];
		}
		if (curWindow == StoreWindow.MOBS)
		{
			return vMobsItem[ind];
		}
		if (curWindow == StoreWindow.BLOCKS_WOOD)
		{
			return vBlockWood[ind];
		}
		if (curWindow == StoreWindow.BLOCKS_STONE)
		{
			return vBlockStone[ind];
		}
		if (curWindow == StoreWindow.BLOCKS_COLORED)
		{
			return vBlockColored[ind];
		}
		if (curWindow == StoreWindow.BLOCKS_NATURE)
		{
			return vBlockNature[ind];
		}
		if (curWindow == StoreWindow.BLOCKS_GLASS)
		{
			return vBlockGlass[ind];
		}
		if (curWindow == StoreWindow.BLOCKS_RESHETKI)
		{
			return vBlockReshetki[ind];
		}
		return null;
	}

	private int GetCurItemLength()
	{
		if (curWindow == StoreWindow.DECORATIONS)
		{
			return vDecorItem.Length;
		}
		if (curWindow == StoreWindow.PLANTS)
		{
			return vPlantItem.Length;
		}
		if (curWindow == StoreWindow.MOBS)
		{
			return vMobsItem.Length;
		}
		if (curWindow == StoreWindow.BLOCKS_WOOD)
		{
			return vBlockWood.Length;
		}
		if (curWindow == StoreWindow.BLOCKS_STONE)
		{
			return vBlockStone.Length;
		}
		if (curWindow == StoreWindow.BLOCKS_COLORED)
		{
			return vBlockColored.Length;
		}
		if (curWindow == StoreWindow.BLOCKS_NATURE)
		{
			return vBlockNature.Length;
		}
		if (curWindow == StoreWindow.BLOCKS_GLASS)
		{
			return vBlockGlass.Length;
		}
		if (curWindow == StoreWindow.BLOCKS_RESHETKI)
		{
			return vBlockReshetki.Length;
		}
		return 0;
	}

	private void Awake()
	{
		INSTANCE = this;
		CreateSecuredLists();
	}

	private void Update()
	{
		if (Input.GetKeyDown((KeyCode)27))
		{
			showMenu = false;
		}
	}

	private void OnGUI()
	{
		if (showMenu)
		{
			GuiStore(15);
		}
	}

	private bool CurWindowIsBlock()
	{
		if (curWindow == StoreWindow.BLOCKS_NATURE || curWindow == StoreWindow.BLOCKS_WOOD || curWindow == StoreWindow.BLOCKS_STONE || curWindow == StoreWindow.BLOCKS_COLORED || curWindow == StoreWindow.BLOCKS_GLASS || curWindow == StoreWindow.BLOCKS_RESHETKI)
		{
			return true;
		}
		return false;
	}

	private void GuiStore(int id)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0873: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)guiBox != (Object)null)
		{
			GUI.skin = guiBox;
		}
		GUI.Box(new Rect((float)(Screen.width / 2 - 200), (float)(Screen.height / 2 - 200), 400f, 400f), "Инвентарь");
		if ((Object)(object)guiSkin != (Object)null)
		{
			GUI.skin = guiSkin;
		}
		GUILayout.BeginArea(new Rect((float)(Screen.width / 2 - 200), (float)(Screen.height / 2 - 200), 400f, 400f));
		GUILayout.Space(20f);
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		GUILayout.Space(10f);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		if (GUILayout.Button("Блоки", new GUILayoutOption[0]))
		{
			SetWindow(StoreWindow.BLOCKS_WOOD);
		}
		if (GUILayout.Button("Предметы", new GUILayoutOption[0]))
		{
			SetWindow(StoreWindow.DECORATIONS);
		}
		if (GUILayout.Button("Животные", new GUILayoutOption[0]))
		{
			SetWindow(StoreWindow.MOBS);
		}
		if (GUILayout.Button("Растения", new GUILayoutOption[0]))
		{
			SetWindow(StoreWindow.PLANTS);
		}
		GUILayout.EndHorizontal();
		if (CurWindowIsBlock())
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (GUILayout.Button("Природные", new GUILayoutOption[0]))
			{
				SetWindow(StoreWindow.BLOCKS_NATURE);
			}
			if (GUILayout.Button("Дерево", new GUILayoutOption[0]))
			{
				SetWindow(StoreWindow.BLOCKS_WOOD);
			}
			if (GUILayout.Button("Камень", new GUILayoutOption[0]))
			{
				SetWindow(StoreWindow.BLOCKS_STONE);
			}
			if (ProfileINI.GetPurchesValue(StorePurchase.CUBES3) != 0 && GUILayout.Button("Цветные", new GUILayoutOption[0]))
			{
				SetWindow(StoreWindow.BLOCKS_COLORED);
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (ProfileINI.GetPurchesValue(StorePurchase.GLASSCUBES) != 0 && GUILayout.Button("Стекла", new GUILayoutOption[1] { GUILayout.MaxWidth(100f) }))
			{
				SetWindow(StoreWindow.BLOCKS_GLASS);
			}
			if (ProfileINI.GetPurchesValue(StorePurchase.RESHETKACUBES) != 0 && GUILayout.Button("Решетки", new GUILayoutOption[1] { GUILayout.MaxWidth(100f) }))
			{
				SetWindow(StoreWindow.BLOCKS_RESHETKI);
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.Space(10f);
		if (curWindow == StoreWindow.DECORATIONS)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(10f);
			GUILayout.Label("Мебель можно купить в магазине", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
			if (GUILayout.Button("Купить", new GUILayoutOption[0]))
			{
				showMenu = false;
				MainMenu.INSTANCE.OpenMebelMenu();
			}
			GUILayout.EndHorizontal();
		}
		if (curWindow == StoreWindow.MOBS)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(10f);
			GUILayout.Label("Животных можно купить в магазине", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
			if (GUILayout.Button("Купить", new GUILayoutOption[0]))
			{
				showMenu = false;
				MainMenu.INSTANCE.OpenMobsMenu();
			}
			GUILayout.EndHorizontal();
		}
		if (curWindow == StoreWindow.PLANTS)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(10f);
			GUILayout.Label("Цветы можно купить в магазине", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
			if (GUILayout.Button("Купить", new GUILayoutOption[0]))
			{
				showMenu = false;
				MainMenu.INSTANCE.OpenPlantsMenu();
			}
			GUILayout.EndHorizontal();
		}
		int num = 0;
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, new GUILayoutOption[1] { GUILayout.MinHeight(300f) });
		count_pos_x = count_start_x;
		count_pos_y = count_start_y;
		while (num < GetCurItemLength())
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			count_pos_x = count_start_x;
			for (int i = 0; i < 4; i++)
			{
				if (num < GetCurItemLength())
				{
					GUILayoutOption[] array = new GUILayoutOption[2]
					{
						GUILayout.MaxWidth(90f),
						GUILayout.MaxHeight(90f)
					};
					if ((ProfileINI.GetPurchesValue(StorePurchase.CUBES3) == 0 && curWindow == StoreWindow.BLOCKS_COLORED) || (ProfileINI.GetPurchesValue(StorePurchase.CUBES2) == 0 && curWindow == StoreWindow.BLOCKS_STONE && num >= 15) || (ProfileINI.GetPurchesValue(StorePurchase.CUBES4) == 0 && curWindow == StoreWindow.BLOCKS_WOOD && num > 4) || (ProfileINI.GetPurchesValue(StorePurchase.FLOWER) == 0 && curWindow == StoreWindow.PLANTS && num >= 0 && num < 4) || (ProfileINI.GetPurchesValue(StorePurchase.FLOWER2) == 0 && curWindow == StoreWindow.PLANTS && num >= 4 && num < 9) || (ProfileINI.GetPurchesValue(StorePurchase.FLOWER3) == 0 && curWindow == StoreWindow.PLANTS && num >= 9) || (ProfileINI.GetPurchesValue(StorePurchase.WATER) == 0 && curWindow == StoreWindow.BLOCKS_NATURE && num == 6) || (ProfileINI.GetPurchesValue(StorePurchase.ICE) == 0 && curWindow == StoreWindow.BLOCKS_NATURE && num == 10) || (ProfileINI.GetPurchesValue(StorePurchase.LAVA) == 0 && curWindow == StoreWindow.BLOCKS_NATURE && num == 7) || (App.instance.gameIni.mapType != GameINI.MapType.SNOWLAND && curWindow == StoreWindow.BLOCKS_NATURE && (num == 8 || num == 9)) || (ProfileINI.GetPurchesValue(StorePurchase.CUBES1) == 0 && curWindow == StoreWindow.BLOCKS_STONE && num > 4 && num < 15) || ((curWindow == StoreWindow.DECORATIONS || curWindow == StoreWindow.MOBS) && ProfileINI.GetPurchesValue(MainMenu.INSTANCE.entityPurches[(int)GetCurItemId(num)]) <= 0))
					{
						i--;
					}
					else
					{
						if (GUILayout.Button((Texture)(object)GetCurItemSprite(num), GUIStyle.op_Implicit("CubesButton"), array))
						{
							if (CurWindowIsBlock())
							{
								Screen.lockCursor = true;
								WorldGameObject.INSTANCE.EnableMouseLook();
								showMenu = false;
								WorldGameObject.INSTANCE.currentBlock = GetCurBlockId(num);
								WorldGameObject.INSTANCE.currentBlockEntity = EntityType.AIR;
								PreviewCubes.Instance.DrawCube();
								PreviewCubes.Instance.Cube.renderer.material.mainTexture = (Texture)(object)WorldData.Instance.BlockTextures[WorldGameObject.INSTANCE.currentBlock];
							}
							else if (GetCurItemId(num) == EntityType.RAIL)
							{
								Screen.lockCursor = true;
								WorldGameObject.INSTANCE.EnableMouseLook();
								showMenu = false;
								WorldGameObject.INSTANCE.currentBlock = BlockType.Air;
								WorldGameObject.INSTANCE.currentBlockEntity = EntityType.RAIL;
								PreviewCubes.Instance.DrawRail();
							}
							else
							{
								if ((GetCurItemId(num) == EntityType.PICTURE2_1 || GetCurItemId(num) == EntityType.PAPER || GetCurItemId(num) == EntityType.PICTURE1_1 || GetCurItemId(num) == EntityType.PICTURE1_2 || GetCurItemId(num) == EntityType.PICTURE3_2) && !Level.Instance.IsAdministrator())
								{
									ChatClient.instance.Warning("Картины можно ставить только на своей карте");
									Screen.lockCursor = true;
									WorldGameObject.INSTANCE.EnableMouseLook();
									showMenu = false;
									return;
								}
								if ((GetCurItemId(num) == EntityType.SPAWN_ARROW || GetCurItemId(num) == EntityType.TEAM_SPAWN_ARROW_RED || GetCurItemId(num) == EntityType.TEAM_SPAWN_ARROW_BLUE) && !Level.Instance.IsAdministrator())
								{
									ChatClient.instance.Warning("Точку возрождения можно устанавливать только на своей карте");
									Screen.lockCursor = true;
									WorldGameObject.INSTANCE.EnableMouseLook();
									showMenu = false;
									return;
								}
								WorldGameObject.INSTANCE.AddPreview(GetCurItemId(num));
								Screen.lockCursor = true;
								WorldGameObject.INSTANCE.EnableMouseLook();
								showMenu = false;
								if (curWindow == StoreWindow.DECORATIONS || curWindow == StoreWindow.MOBS)
								{
									StorePurchase purches = MainMenu.INSTANCE.entityPurches[(int)GetCurItemId(num)];
									if (ProfileINI.CheckPurchesValue(purches))
									{
										int purchesValue = ProfileINI.GetPurchesValue(purches);
										ProfileINI.SetPurchesValue(purches, purchesValue - 1);
										((MonoBehaviour)MainMenu.INSTANCE).StartCoroutine(MainMenu.INSTANCE.UsePurches(purches));
									}
									else
									{
										App.instance.OnCheatDetected("decor_cheat");
									}
								}
							}
						}
						if (curWindow == StoreWindow.DECORATIONS || curWindow == StoreWindow.MOBS)
						{
							StorePurchase purches2 = MainMenu.INSTANCE.entityPurches[(int)GetCurItemId(num)];
							int purchesValue2 = ProfileINI.GetPurchesValue(purches2);
							GUI.Label(new Rect((float)count_pos_x, (float)count_pos_y, 100f, 100f), purchesValue2.ToString());
						}
						count_pos_x += count_dx;
					}
				}
				num++;
			}
			count_pos_y += count_dy;
			GUILayout.EndHorizontal();
		}
		GUILayout.EndScrollView();
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	private bool isButtonNext()
	{
		return GetCurItemLength() - mSeek > 12;
	}

	private bool isButtonLast()
	{
		return mSeek > 0;
	}

	private void SetWindow(StoreWindow window)
	{
		mSeek = 0;
		curWindow = window;
	}
}
