using UnityEngine;

public class NewMap : MonoBehaviour
{
	public static NewMap INSTANCE;

	public bool showMenu = true;

	private void Awake()
	{
		INSTANCE = this;
	}

	private void OnGUI()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		if (showMenu)
		{
			GUI.Box(new Rect((float)(Screen.width / 2 - 250), (float)(Screen.height / 2 - 100), 500f, 200f), "Новая карта");
			GUILayout.BeginArea(new Rect((float)(Screen.width / 2 - 200), (float)(Screen.height / 2 - 100), 400f, 200f));
			GUILayout.Space(30f);
			if (GUILayout.Button("Очистить карту - 1 голос", new GUILayoutOption[0]))
			{
				showMenu = false;
				WorldGameObject.INSTANCE.CreateNewMap();
			}
			if (GUILayout.Button("Установить среднию карту  - 5  голосов", new GUILayoutOption[0]))
			{
				showMenu = false;
			}
			GUILayout.Button("Установить большую карту  - 8 голосов", new GUILayoutOption[0]);
			GUILayout.Button("Установить огромную карту - 12 голосов", new GUILayoutOption[0]);
			GUILayout.EndArea();
		}
	}
}
