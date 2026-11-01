using UnityEngine;

public class TabMenu : MonoBehaviour
{
	public static TabMenu Instance;

	public GUISkin guiSkin;

	public GUISkin boxSkin;

	public bool showMenu;

	private Vector2 _PlayersScroll;

	private void Awake()
	{
		Instance = this;
	}

	private void OnGUI()
	{
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		if (!showMenu || (Object)(object)TeamBattle.Instance != (Object)null || (Object)(object)Survival.Instance != (Object)null)
		{
			return;
		}
		bool flag = Level.Instance.IsAdministrator();
		bool flag2 = Level.Instance.IsModerator();
		string text = (string)PhotonNetwork.room.customProperties["map_name"];
		string text2 = (string)PhotonNetwork.room.customProperties["map_about"];
		if ((Object)(object)boxSkin != (Object)null)
		{
			GUI.skin = boxSkin;
		}
		GUI.Box(new Rect((float)(Screen.width / 2 - 270), (float)(Screen.height / 2 - 200), 540f, 400f), "Сервер : " + text);
		if ((Object)(object)guiSkin != (Object)null)
		{
			GUI.skin = guiSkin;
		}
		GUILayout.BeginArea(new Rect((float)(Screen.width / 2 - 250), (float)(Screen.height / 2 - 200), 500f, 400f));
		GUILayout.Space(40f);
		GUILayout.Label(text2, GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
		if (flag)
		{
			GUILayout.Label(string.Empty, GUIStyle.op_Implicit("Divider"), new GUILayoutOption[0]);
			ProfileINI.newgamersislook = GUILayout.Toggle(ProfileINI.newgamersislook, "Новые игроки не могут изменять карту", new GUILayoutOption[0]);
			GUILayout.Label(string.Empty, GUIStyle.op_Implicit("Divider"), new GUILayoutOption[0]);
		}
		GUILayout.Space(20f);
		_PlayersScroll = GUILayout.BeginScrollView(_PlayersScroll, new GUILayoutOption[0]);
		foreach (PlayerNode player in WorldGameObject.INSTANCE.playerList)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			bool flag3 = Level.Instance.IsAdministrator(player.playerName);
			bool flag4 = Level.Instance.IsModerator(player.playerName);
			string text3 = player.playerName;
			if (flag3)
			{
				text3 += " (A)";
			}
			if (flag4)
			{
				text3 += " (M)";
			}
			GUILayout.Label(text3, GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[1] { GUILayout.MinWidth(120f) });
			if (player.viewer_id != VkontakteAPI.INSTANCE._viewerId)
			{
				if (flag && !flag3)
				{
					if (!flag4)
					{
						if (GUILayout.Button("+M", new GUILayoutOption[1] { GUILayout.MaxWidth(180f) }))
						{
							Level.Instance.AddModerator(player.playerName);
							if (!Level.Instance.IsBuilder(player.playerName))
							{
								Level.Instance.AddBuilder(player.playerName);
							}
						}
					}
					else if (GUILayout.Button("-M", new GUILayoutOption[1] { GUILayout.MaxWidth(180f) }))
					{
						Level.Instance.RemoveModerator(player.playerName);
					}
				}
				if ((flag || flag2) && !flag3 && !flag4)
				{
					if (GUILayout.Button("Забанить", new GUILayoutOption[1] { GUILayout.MaxWidth(180f) }))
					{
						WorldGameObject.INSTANCE.BanPlayer(player);
						return;
					}
					if (!Level.Instance.IsBuilder(player.playerName))
					{
						if (GUILayout.Button("Разрешить строить", new GUILayoutOption[1] { GUILayout.MaxWidth(180f) }))
						{
							WorldGameObject.INSTANCE.PlayerCanBuild(player, canBuild: true);
						}
					}
					else if (GUILayout.Button("Запретить строить", new GUILayoutOption[1] { GUILayout.MaxWidth(180f) }))
					{
						WorldGameObject.INSTANCE.PlayerCanBuild(player, canBuild: false);
					}
				}
			}
			else
			{
				GUILayout.Label("<- Это вы", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
			}
			GUILayout.EndHorizontal();
		}
		if (flag || flag2)
		{
			GUILayout.Label(string.Empty, GUIStyle.op_Implicit("Divider"), new GUILayoutOption[0]);
			GUILayout.Label("Забаненные :", GUIStyle.op_Implicit("PlainText"), new GUILayoutOption[0]);
			string[] banned = Level.Instance.Banned;
			foreach (string text4 in banned)
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label(text4, new GUILayoutOption[0]);
				if (GUILayout.Button("Разбанить", new GUILayoutOption[0]))
				{
					WorldGameObject.INSTANCE.UnBanPlayer(text4);
				}
				GUILayout.EndHorizontal();
			}
		}
		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}
}
