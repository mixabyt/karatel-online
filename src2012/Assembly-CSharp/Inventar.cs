using UnityEngine;

public class Inventar : MonoBehaviour
{
	public class Element
	{
		public eObjectTypes Type;

		public BlockType Block;

		public EntityType Entity;

		public int count;

		public Element()
		{
			Type = eObjectTypes.NOT_DEFINED;
			Block = BlockType.Air;
			Entity = EntityType.AIR;
			count = 0;
		}
	}

	private class Cell
	{
		public BlockType Type;

		public int count;

		public Cell()
		{
			Type = BlockType.Air;
			count = 0;
		}
	}

	public bool showMenu;

	public Texture2D ImgSelect;

	public Texture2D ImgBack;

	public float backX;

	public float backY;

	public float invX;

	public float invY;

	public float invDX;

	public float invDY;

	public float invLastDY;

	public float invSize;

	public static Inventar INSTANCE;

	public float fastCellX;

	public float fastCellY;

	public int cellWidth;

	public GUIStyle CellStyle;

	public GUIStyle ToolTipStyle;

	private Element[,] Inventory = new Element[9, 4];

	private Element mouseElement = new Element();

	private bool bOnMouse;

	public int SelectId;

	private void CopyToMouse(Element element)
	{
		mouseElement.Type = element.Type;
		mouseElement.Block = element.Block;
		mouseElement.Entity = element.Entity;
		mouseElement.count = element.count;
	}

	private void CopyFromMouse(Element element)
	{
		element.Type = mouseElement.Type;
		element.Block = mouseElement.Block;
		element.Entity = mouseElement.Entity;
		element.count = mouseElement.count;
	}

	private void Reset(Element element)
	{
		element.Type = eObjectTypes.NOT_DEFINED;
		element.Block = BlockType.Air;
		element.Entity = EntityType.AIR;
		element.count = 0;
	}

	private string GetName(Element element)
	{
		if (element.Type == eObjectTypes.BLOCK)
		{
			return WorldGameObject.INSTANCE.BlockParametrs[(uint)element.Block].name;
		}
		if (element.Type == eObjectTypes.ENTITY)
		{
			return WorldGameObject.INSTANCE.EnityParametrs[(int)element.Entity].name;
		}
		return string.Empty;
	}

	private Texture2D GetIcon(Element element)
	{
		if (element.Type == eObjectTypes.BLOCK)
		{
			return WorldGameObject.INSTANCE.BlockParametrs[(uint)element.Block].icon;
		}
		if (element.Type == eObjectTypes.ENTITY)
		{
			return WorldGameObject.INSTANCE.EnityParametrs[(int)element.Entity].icon;
		}
		return null;
	}

	private void Awake()
	{
		INSTANCE = this;
		for (int i = 0; i < 9; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				Inventory[i, j] = new Element();
			}
		}
	}

	private void OnGUI()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Expected Obj, but got Unknown
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		if (!showMenu)
		{
			return;
		}
		GUI.Label(new Rect(backX, backY, (float)((Texture)ImgBack).width, (float)((Texture)ImgBack).height), (Texture)(object)ImgBack);
		float num = invX;
		float num2 = invY;
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 9; j++)
			{
				if (GUI.Button(new Rect(num, num2, invSize, invSize), new GUIContent((Texture)(object)GetIcon(Inventory[j, i]), GetName(Inventory[j, i])), CellStyle))
				{
					if (!bOnMouse)
					{
						if (Inventory[j, i].count > 0)
						{
							CopyToMouse(Inventory[j, i]);
							Reset(Inventory[j, i]);
							bOnMouse = true;
						}
					}
					else if (Inventory[j, i].Type == eObjectTypes.NOT_DEFINED)
					{
						CopyFromMouse(Inventory[j, i]);
						bOnMouse = false;
					}
				}
				if (Inventory[j, i].count > 0)
				{
					GUI.Label(new Rect(num, num2, invSize, invSize), Inventory[j, i].count.ToString(), ToolTipStyle);
				}
				num += invSize;
			}
			num2 = ((i != 2) ? (num2 + invSize) : (num2 + (invSize + invLastDY)));
			num = invX;
		}
		if (bOnMouse)
		{
			Rect val = new Rect(Input.mousePosition.x, 600f - Input.mousePosition.y + 20f, invSize, invSize);
			GUI.Label(val, (Texture)(object)GetIcon(mouseElement));
			if (mouseElement.count > 0)
			{
				GUI.Label(val, mouseElement.count.ToString(), ToolTipStyle);
			}
		}
		Rect val2 = new Rect(Input.mousePosition.x, 600f - Input.mousePosition.y + 20f, 200f, 200f);
		GUI.Label(val2, GUI.tooltip, ToolTipStyle);
		float num3 = fastCellX;
		for (int k = 0; k < 9; k++)
		{
			GUI.Label(new Rect(num3 + 4f, fastCellY + 4f, (float)(cellWidth - 8), (float)(cellWidth - 8)), (Texture)(object)GetIcon(Inventory[k, 3]));
			if (Inventory[k, 3].count > 0)
			{
				GUI.Label(new Rect(num3, fastCellY, (float)cellWidth, (float)cellWidth), Inventory[k, 3].count.ToString());
			}
			if (SelectId == k)
			{
				GUI.Label(new Rect(num3, fastCellY, (float)cellWidth, (float)cellWidth), (Texture)(object)ImgSelect);
			}
			num3 += (float)cellWidth;
		}
	}

	public Element PopCurElement()
	{
		Element element = new Element();
		Reset(element);
		if (Inventory[SelectId, 3].Type == eObjectTypes.NOT_DEFINED)
		{
			return element;
		}
		if (Inventory[SelectId, 3].Type == eObjectTypes.BLOCK)
		{
			element.Type = eObjectTypes.BLOCK;
			element.Block = Inventory[SelectId, 3].Block;
			Inventory[SelectId, 3].count--;
			if (Inventory[SelectId, 3].count <= 0)
			{
				Reset(Inventory[SelectId, 3]);
			}
		}
		if (Inventory[SelectId, 3].Type == eObjectTypes.ENTITY)
		{
			element.Type = eObjectTypes.ENTITY;
			element.Entity = Inventory[SelectId, 3].Entity;
			Inventory[SelectId, 3].count--;
			if (Inventory[SelectId, 3].count <= 0)
			{
				Reset(Inventory[SelectId, 3]);
			}
		}
		return element;
	}

	public bool PutEntityToInventory(EntityType type, int count)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (type == EntityType.AIR)
		{
			return false;
		}
		Vector2 val = FindCellWithType(type);
		if (val == new Vector2(-1f, -1f))
		{
			val = FindEmptyCell();
		}
		if (val == new Vector2(-1f, -1f))
		{
			return false;
		}
		AddEntityToCell(val, type, count);
		return true;
	}

	public bool PutBlockToInventory(BlockType type, int count)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (type == BlockType.Air)
		{
			return false;
		}
		Vector2 val = FindCellWithType(type);
		if (val == new Vector2(-1f, -1f))
		{
			val = FindEmptyCell();
		}
		if (val == new Vector2(-1f, -1f))
		{
			return false;
		}
		AddBlockToCell(val, type, count);
		return true;
	}

	private void AddEntityToCell(Vector2 ind_cell, EntityType type, int count)
	{
		Inventory[(int)ind_cell.x, (int)ind_cell.y].Type = eObjectTypes.ENTITY;
		Inventory[(int)ind_cell.x, (int)ind_cell.y].Entity = type;
		Inventory[(int)ind_cell.x, (int)ind_cell.y].count += count;
	}

	private void AddBlockToCell(Vector2 ind_cell, BlockType type, int count)
	{
		Inventory[(int)ind_cell.x, (int)ind_cell.y].Type = eObjectTypes.BLOCK;
		Inventory[(int)ind_cell.x, (int)ind_cell.y].Block = type;
		Inventory[(int)ind_cell.x, (int)ind_cell.y].count += count;
	}

	private Vector2 FindEmptyCell()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 9; i++)
		{
			if (Inventory[i, 3].Type == eObjectTypes.NOT_DEFINED)
			{
				return new Vector2((float)i, 3f);
			}
		}
		for (int j = 0; j < 4; j++)
		{
			for (int k = 0; k < 9; k++)
			{
				if (Inventory[k, j].Type == eObjectTypes.NOT_DEFINED)
				{
					return new Vector2((float)k, (float)j);
				}
			}
		}
		return new Vector2(-1f, -1f);
	}

	private Vector2 FindCellWithType(BlockType type)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 9; i++)
		{
			if (Inventory[i, 3].Type == eObjectTypes.BLOCK && Inventory[i, 3].Block == type)
			{
				return new Vector2((float)i, 3f);
			}
		}
		for (int j = 0; j < 4; j++)
		{
			for (int k = 0; k < 9; k++)
			{
				if (Inventory[k, j].Type == eObjectTypes.BLOCK && Inventory[k, j].Block == type)
				{
					return new Vector2((float)k, (float)j);
				}
			}
		}
		return new Vector2(-1f, -1f);
	}

	private Vector2 FindCellWithType(EntityType type)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 9; i++)
		{
			if (Inventory[i, 3].Type == eObjectTypes.ENTITY && Inventory[i, 3].Entity == type)
			{
				return new Vector2((float)i, 3f);
			}
		}
		for (int j = 0; j < 4; j++)
		{
			for (int k = 0; k < 9; k++)
			{
				if (Inventory[k, j].Type == eObjectTypes.ENTITY && Inventory[k, j].Entity == type)
				{
					return new Vector2((float)k, (float)j);
				}
			}
		}
		return new Vector2(-1f, -1f);
	}

	private void Update()
	{
		if (WorldGameObject.INSTANCE.bPreviewMagazinItem)
		{
			return;
		}
		if (Input.GetAxis("Mouse ScrollWheel") < 0f)
		{
			SelectId--;
			if (SelectId < 0)
			{
				SelectId = 0;
			}
		}
		if (Input.GetAxis("Mouse ScrollWheel") > 0f)
		{
			SelectId++;
			if (SelectId >= 8)
			{
				SelectId = 8;
			}
		}
	}
}
