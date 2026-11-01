using System;
using UnityEngine;

public class Craft : MonoBehaviour
{
	[Serializable]
	public class CraftScroll
	{
		[Serializable]
		public class Element
		{
			public eObjectTypes Type;

			public BlockType Block;

			public EntityType Entity;

			public int count;
		}

		public Element Result;

		public Element[] Components;
	}

	public static Craft INSTANCE;

	public bool showMenu = true;

	private Vector2 scrollPosition;

	public CraftScroll[] Scrolls;

	private void Awake()
	{
		INSTANCE = this;
	}

	private void Start()
	{
	}

	private string GetName(CraftScroll.Element element)
	{
		if (element.Type == eObjectTypes.BLOCK)
		{
			return WorldGameObject.INSTANCE.BlockParametrs[(uint)element.Block].name;
		}
		return string.Empty;
	}

	private Texture2D GetIcon(CraftScroll.Element element)
	{
		if (element.Type == eObjectTypes.BLOCK)
		{
			return WorldGameObject.INSTANCE.BlockParametrs[(uint)element.Block].icon;
		}
		return null;
	}

	private string GetId(CraftScroll.Element element)
	{
		if (element.Type == eObjectTypes.BLOCK)
		{
			return Enum.GetName(typeof(BlockType), (int)element.Block);
		}
		return null;
	}

	private void OnGUI()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Expected Obj, but got Unknown
		if (!showMenu)
		{
			return;
		}
		GUI.Box(new Rect((float)(Screen.width / 2 - 200), (float)(Screen.height / 2 - 200), 400f, 400f), "Крафт");
		GUILayout.BeginArea(new Rect((float)(Screen.width / 2 - 200), (float)(Screen.height / 2 - 200), 400f, 400f));
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, new GUILayoutOption[1] { GUILayout.MinHeight(250f) });
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		GUILayout.Space(40f);
		int num = 0;
		while (num < Scrolls.Length)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			for (int i = 0; i < 4; i++)
			{
				if (num < Scrolls.Length)
				{
					GUILayoutOption[] array = new GUILayoutOption[2]
					{
						GUILayout.MaxWidth(90f),
						GUILayout.MaxHeight(90f)
					};
					GUILayout.Button(new GUIContent((Texture)(object)GetIcon(Scrolls[num].Result), num.ToString()), array);
				}
				num++;
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.EndVertical();
		GUILayout.EndScrollView();
		GUILayout.EndArea();
		if (GUI.tooltip.Length != 0)
		{
			Rect val = new Rect(Input.mousePosition.x, 600f - Input.mousePosition.y, 200f, 200f);
			int num2 = Convert.ToInt32(GUI.tooltip);
			GUI.Box(val, GetName(Scrolls[num2].Result));
			GUI.Label(new Rect(val.x + 10f, val.y + 20f, 200f, 200f), "Компоненты :");
			GUI.Label(new Rect(val.x + 10f, val.y + 40f, 64f, 64f), GetName(Scrolls[num2].Components[0]));
			GUI.Label(new Rect(val.x + 10f, val.y + 60f, 64f, 64f), (Texture)(object)GetIcon(Scrolls[num2].Components[0]));
			GUI.Label(new Rect(val.x + 10f, val.y + 60f, 64f, 64f), "0/" + Scrolls[num2].Components[0].count);
		}
	}
}
