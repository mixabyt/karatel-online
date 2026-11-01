using System;
using System.Collections.Generic;
using UnityEngine;

public class SpeechBubbles : MonoBehaviour
{
	[Serializable]
	public class Emote
	{
		public Texture Picture;

		public string Text;
	}

	private class SpeechBubble
	{
		public GameObject Author;

		public float StartTime;

		public float LifeTime;

		public bool RaycastSuccesfull;

		public float RaycastLastTime;

		public float Fade = 1f;

		public string Text;

		public Texture Picture;
	}

	private const float _BubbleWidth = 256f;

	private const float _BubbleHeight = 512f;

	private const float _BubbleTop = 18f;

	private const float _BubbleTopTex = 9f / 256f;

	private const float _BubbleBottom = 67f;

	private const float _BubbleBottomTex = 134f / 1024f;

	private const float _BubbleMid = 427f;

	private const float _BubbleMidTex = 0.8339844f;

	public float RaycastDistance = 50f;

	public float RaycastPeriod = 1f;

	public float BaseLifeTime = 5f;

	public float SymbolLifeTime = 0.2f;

	public float FadeTime = 1f;

	public Texture BubbleTexture;

	public Font BubbleFont;

	public Emote[] Emotes;

	private static SpeechBubbles _Instance;

	private bool _ShowMenu;

	private List<SpeechBubble> _SpeechBubbles = new List<SpeechBubble>();

	private GUIStyle _TextStyle;

	private Vector2 _MenuScroll = Vector2.zero;

	public static bool ShowMenu
	{
		get
		{
			return (Object)(object)_Instance != (Object)null && _Instance._ShowMenu;
		}
		set
		{
			if ((Object)(object)_Instance != (Object)null)
			{
				_Instance._ShowMenu = value;
				if (_Instance._ShowMenu)
				{
					WorldGameObject.INSTANCE.DisableMouseLook();
					Screen.lockCursor = false;
					Store.INSTANCE.showMenu = false;
					MainMenu.INSTANCE.showMenu = false;
					TabMenu.Instance.showMenu = false;
				}
				else
				{
					WorldGameObject.INSTANCE.EnableMouseLook();
					Screen.lockCursor = true;
				}
			}
		}
	}

	public SpeechBubbles()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
	}

	public static void AddText(GameObject author, string text)
	{
		if ((Object)(object)_Instance != (Object)null && ProfileINI.showBaloons && (ProfileINI.showSelfBaloons || (Object)(object)author != (Object)(object)WorldGameObject.INSTANCE.MainPlayer))
		{
			_Instance.Add(author, text, null);
		}
	}

	public static void AddPicture(GameObject author, Texture picture)
	{
		if ((Object)(object)_Instance != (Object)null)
		{
			_Instance.Add(author, null, picture);
		}
	}

	private void Add(GameObject author, string text, Texture picture)
	{
		for (int i = 0; i < _SpeechBubbles.Count; i++)
		{
			if ((Object)(object)_SpeechBubbles[i].Author == (Object)(object)author)
			{
				_SpeechBubbles.RemoveAt(i);
				break;
			}
		}
		SpeechBubble speechBubble = new SpeechBubble();
		speechBubble.Author = author;
		speechBubble.Text = text;
		speechBubble.Picture = ((text != null) ? null : picture);
		speechBubble.StartTime = Time.time;
		speechBubble.LifeTime = BaseLifeTime + ((text == null) ? 0f : (SymbolLifeTime * (float)text.Length));
		_SpeechBubbles.Add(speechBubble);
		RaycastAuthor(speechBubble);
	}

	public static Emote GetEmote(string emoteName)
	{
		if ((Object)(object)_Instance != (Object)null)
		{
			Emote[] emotes = _Instance.Emotes;
			foreach (Emote emote in emotes)
			{
				if (((Object)emote.Picture).name == emoteName)
				{
					return emote;
				}
			}
		}
		return null;
	}

	private void Awake()
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected Obj, but got Unknown
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)_Instance != (Object)null)
		{
			if ((Object)(object)((Component)this).gameObject != (Object)(object)((Component)_Instance).gameObject)
			{
				Object.Destroy((Object)(object)((Component)_Instance).gameObject);
			}
			else
			{
				Object.Destroy((Object)(object)_Instance);
			}
		}
		_Instance = this;
		_TextStyle = new GUIStyle();
		_TextStyle.alignment = (TextAnchor)4;
		_TextStyle.normal.textColor = Color.black;
		_TextStyle.wordWrap = true;
		_TextStyle.font = BubbleFont;
	}

	private void OnGUI()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Invalid comparison between Unknown and I4
		//IL_0615: Unknown result type (might be due to invalid IL or missing references)
		//IL_066c: Unknown result type (might be due to invalid IL or missing references)
		//IL_069b: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0736: Unknown result type (might be due to invalid IL or missing references)
		//IL_0823: Unknown result type (might be due to invalid IL or missing references)
		//IL_087f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Expected Obj, but got Unknown
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0402: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_0423: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_047d: Unknown result type (might be due to invalid IL or missing references)
		//IL_049c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04be: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0505: Unknown result type (might be due to invalid IL or missing references)
		//IL_0524: Unknown result type (might be due to invalid IL or missing references)
		//IL_055d: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_059c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
		if ((int)Event.current.type == 8)
		{
			GUI.depth = 10;
			return;
		}
		if ((int)Event.current.type == 7 && (Object)(object)CameraController.Instance != (Object)null)
		{
			for (int num = _SpeechBubbles.Count - 1; num >= 0; num--)
			{
				SpeechBubble speechBubble = _SpeechBubbles[num];
				if ((Object)(object)speechBubble.Author == (Object)null || (Object)(object)speechBubble.Author.collider == (Object)null || Time.time - speechBubble.StartTime >= speechBubble.LifeTime)
				{
					_SpeechBubbles.RemoveAt(num);
				}
				else
				{
					if (Time.time - speechBubble.StartTime >= speechBubble.LifeTime - FadeTime)
					{
						speechBubble.Fade = Mathf.MoveTowards(speechBubble.Fade, 0f, Time.deltaTime * FadeTime);
					}
					else if (speechBubble.Fade < 1f)
					{
						speechBubble.Fade = Mathf.MoveTowards(speechBubble.Fade, 1f, Time.deltaTime * FadeTime);
					}
					if ((Object)(object)speechBubble.Author == (Object)(object)WorldGameObject.INSTANCE.MainPlayer && !CameraController.Instance.IsThirdPerson)
					{
						if ((Object)(object)speechBubble.Picture != (Object)null)
						{
							Color color = GUI.color;
							GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, speechBubble.Fade);
							GUI.DrawTexture(new Rect((float)(Screen.width / 2 - 64), 10f, 128f, 128f), speechBubble.Picture);
							GUI.color = color;
						}
					}
					else
					{
						if (Time.time - speechBubble.RaycastLastTime >= RaycastPeriod)
						{
							RaycastAuthor(speechBubble);
						}
						if (speechBubble.RaycastSuccesfull)
						{
							Vector3 position = speechBubble.Author.transform.position;
							if ((Object)(object)speechBubble.Author.collider != (Object)null && speechBubble.Author.collider.enabled)
							{
								float y = position.y;
								Bounds bounds = speechBubble.Author.collider.bounds;
								position.y = y + (bounds.size.y * speechBubble.Author.transform.localScale.y + 0.1f);
							}
							else
							{
								position.y += 1.88f;
							}
							position = CameraController.RaycastCamera.WorldToScreenPoint(position);
							if (!(position.z <= 0f))
							{
								float num2 = 256f;
								float num3 = 512f;
								float num4 = 0f;
								if (speechBubble.Text != null)
								{
									num4 = _TextStyle.CalcHeight(new GUIContent(speechBubble.Text), num2 - 10f);
								}
								else if ((Object)(object)speechBubble.Picture != (Object)null)
								{
									num4 = 256f;
								}
								float num5 = Vector3.Distance(((Component)CameraController.RaycastCamera).transform.position, speechBubble.Author.transform.position);
								float num6 = 0.7f - Mathf.Clamp01(num5 / RaycastDistance) / 2f;
								if ((Object)(object)speechBubble.Picture != (Object)null)
								{
									num6 /= 2f;
								}
								position.x -= num2 / 2f * num6;
								position.y = (float)Screen.height - position.y;
								position.y -= (18f + num4 + 67f) * num6;
								position.x /= num6;
								position.y /= num6;
								Matrix4x4 matrix = GUI.matrix;
								GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(num6, num6, 1f));
								Color color2 = GUI.color;
								if (speechBubble.Fade != 1f)
								{
									GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, speechBubble.Fade);
								}
								if (speechBubble.Text != null)
								{
									GUI.DrawTextureWithTexCoords(new Rect(position.x, position.y, num2, 18f), BubbleTexture, new Rect(0f, 247f / 256f, 1f, 9f / 256f));
									GUI.DrawTextureWithTexCoords(new Rect(position.x, position.y + 18f, num2, num4), BubbleTexture, new Rect(0f, 134f / 1024f, 1f, num4 / num3));
									GUI.DrawTextureWithTexCoords(new Rect(position.x, position.y + 18f + num4, num2, 67f), BubbleTexture, new Rect(0f, 0f, 1f, 134f / 1024f));
								}
								if (speechBubble.Text != null)
								{
									GUI.Label(new Rect(position.x + 5f, position.y + 18f, num2 - 10f, num4), speechBubble.Text, _TextStyle);
								}
								if ((Object)(object)speechBubble.Picture != (Object)null)
								{
									GUI.DrawTexture(new Rect(position.x, position.y + 18f, num2, num4), speechBubble.Picture, (ScaleMode)2);
								}
								GUI.matrix = matrix;
								if (speechBubble.Fade != 1f)
								{
									GUI.color = color2;
								}
							}
						}
					}
				}
			}
		}
		if (!ShowMenu)
		{
			return;
		}
		GUI.skin = Store.INSTANCE.guiBox;
		GUI.Box(new Rect((float)(Screen.width / 2 - 300), (float)(Screen.height / 2 - 200), 562f, 400f), "Эмоции");
		GUI.skin = Store.INSTANCE.guiSkin;
		Rect val = new Rect(0f, 0f, 542f, (float)(138 * (Emotes.Length / 4 + ((Emotes.Length % 4 != 0) ? 1 : 0))));
		_MenuScroll = GUI.BeginScrollView(new Rect((float)(Screen.width / 2 - 300), (float)(Screen.height / 2 - 200 + 30), 562f, 364f), _MenuScroll, val);
		int num7 = 0;
		for (int i = 0; i < Emotes.Length; i++)
		{
			if ((i <= 5 || i >= 18 || ProfileINI.GetPurchesValue(StorePurchase.EMOTIONS) != 0) && (i < 18 || ProfileINI.GetPurchesValue(StorePurchase.MEM_SMILES) != 0))
			{
				if (Emotes[i].Text != "voiceChat" && GUI.Button(new Rect((float)(num7 % 4 * 138), (float)(num7 / 4 * 138), 128f, 128f), Emotes[i].Picture, GUI.skin.label))
				{
					ShowMenu = false;
					ChatClient.instance.SendEmote(((Object)Emotes[i].Picture).name);
				}
				num7++;
			}
		}
		if (ProfileINI.GetPurchesValue(StorePurchase.EMOTIONS) == 0 && ProfileINI.GetPurchesValue(StorePurchase.MEM_SMILES) == 0 && GUI.Button(new Rect(20f, 276f, 512f, 45f), "Еще эмоции...", GUIStyle.op_Implicit("Button")))
		{
			MainMenu.INSTANCE.OpenEmotionsMenu();
			_ShowMenu = false;
		}
		if (ProfileINI.GetPurchesValue(StorePurchase.EMOTIONS) == 0 && ProfileINI.GetPurchesValue(StorePurchase.MEM_SMILES) == 1 && GUI.Button(new Rect(20f, 690f, 512f, 45f), "Еще эмоции...", GUIStyle.op_Implicit("Button")))
		{
			MainMenu.INSTANCE.OpenEmotionsMenu();
			_ShowMenu = false;
		}
		if (ProfileINI.GetPurchesValue(StorePurchase.EMOTIONS) == 1 && ProfileINI.GetPurchesValue(StorePurchase.MEM_SMILES) == 0 && GUI.Button(new Rect(20f, 690f, 512f, 45f), "Еще эмоции...", GUIStyle.op_Implicit("Button")))
		{
			MainMenu.INSTANCE.OpenMEMEmotionsMenu();
			_ShowMenu = false;
		}
		GUI.EndScrollView();
	}

	private void RaycastAuthor(SpeechBubble bubble)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		bubble.RaycastSuccesfull = false;
		bubble.RaycastLastTime = Time.time;
		if ((Object)(object)CameraController.Instance == (Object)null)
		{
			return;
		}
		if ((Object)(object)bubble.Author == (Object)(object)WorldGameObject.INSTANCE.MainPlayer)
		{
			bubble.RaycastSuccesfull = true;
			return;
		}
		Vector3 position = ((Component)CameraController.RaycastCamera).transform.position;
		PlayerNetwork component = bubble.Author.GetComponent<PlayerNetwork>();
		Vector3 val;
		if ((Object)(object)component != (Object)null && (Object)(object)component.HeadBone != (Object)null && component.HeadBone.transform.GetChildCount() > 0)
		{
			val = ((Component)component.HeadBone.transform.GetChild(0)).transform.position;
		}
		else if ((Object)(object)component != (Object)null && (Object)(object)component.HeadBone != (Object)null)
		{
			val = component.HeadBone.transform.position;
		}
		else
		{
			Bounds bounds = bubble.Author.collider.bounds;
			val = bounds.center;
		}
		float num = Vector3.Distance(position, val);
		if (!(num > RaycastDistance))
		{
			int num2 = 1 << LayerMask.NameToLayer("Terrain");
			Vector3 val2 = val - position;
			if (!Physics.Raycast(position, val2.normalized, num, num2))
			{
				bubble.RaycastSuccesfull = true;
			}
		}
	}
}
