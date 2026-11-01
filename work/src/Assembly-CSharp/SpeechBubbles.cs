using System;
using System.Collections.Generic;
using UnityEngine;

public class SpeechBubbles : MonoBehaviour
{
	[Serializable]
	public class Emotion
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

		public float Alpha = 1f;

		public float CurAlpha = 1f;

		public string Text;

		public string EmotionName;

		public float Scale = 1f;

		public GameObject GUIObject;

		public Transform[] GUIObjectTransforms;

		public Vector3[] GUIObjectBasePositions;

		public Vector3[] GUIObjectBaseScales;
	}

	public float RaycastDistance = 50f;

	public float RaycastPeriod = 1f;

	public float BaseLifeTime = 5f;

	public float SymbolLifeTime = 0.2f;

	public float FadeTime = 1f;

	public Emotion[] Emotions;

	private static SpeechBubbles _Instance;

	private List<SpeechBubble> _SpeechBubbles = new List<SpeechBubble>();

	private GameObject _GUIObjectPrototype;

	private GameObject _EmotionImage;

	public static void AddText(GameObject author, string text)
	{
		if ((Object)(object)_Instance != (Object)null && ProfileINI.showBaloons && (ProfileINI.showSelfBaloons || (Object)(object)author != (Object)(object)WorldGameObject.Instance.MainPlayer))
		{
			_Instance.Add(author, text, null);
		}
	}

	public static void AddEmotion(GameObject author, string emotion)
	{
		if ((Object)(object)_Instance != (Object)null)
		{
			_Instance.Add(author, null, emotion);
		}
	}

	private void Add(GameObject author, string text, string emotion)
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected Obj, but got Unknown
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < _SpeechBubbles.Count; i++)
		{
			if ((Object)(object)_SpeechBubbles[i].Author == (Object)(object)author)
			{
				Object.Destroy((Object)(object)_SpeechBubbles[i].GUIObject);
				_SpeechBubbles.RemoveAt(i);
				break;
			}
		}
		if ((Object)(object)author == (Object)(object)WorldGameObject.Instance.MainPlayer && _EmotionImage.activeInHierarchy)
		{
			Utils.SetActiveRecursively(_EmotionImage, enable: false);
		}
		GameObject val = (GameObject)Object.Instantiate((Object)(object)_GUIObjectPrototype);
		Utils.SetActiveRecursively(val, enable: true);
		UILabel component = ((Component)val.transform.Find("txt_text")).GetComponent<UILabel>();
		UISprite component2 = ((Component)val.transform.Find("bubble")).GetComponent<UISprite>();
		UISprite component3 = ((Component)val.transform.Find("bubble_cap")).GetComponent<UISprite>();
		Transform val2 = val.transform.Find("emotion_image");
		if (text != null)
		{
			component.text = text;
			text = component.processedText;
			component.text = text;
			float y = ((Component)component).transform.localPosition.y;
			y += component.font.CalculatePrintedSize(text, component.supportEncoding, component.symbolStyle).y * ((Component)component).transform.localScale.y;
			component2.fillAmount = y / ((Component)component2).transform.localScale.y;
			Vector3 localPosition = ((Component)component3).transform.localPosition;
			((Component)component3).transform.localPosition = new Vector3(localPosition.x, y, localPosition.z);
			Object.Destroy((Object)(object)((Component)val2).gameObject);
		}
		else
		{
			KGUI.SetControlSprite(val2, emotion, 256f);
			Object.Destroy((Object)(object)((Component)component).gameObject);
			Object.Destroy((Object)(object)((Component)component2).gameObject);
			Object.Destroy((Object)(object)((Component)component3).gameObject);
		}
		val.transform.parent = _GUIObjectPrototype.transform.parent;
		val.transform.localScale = _GUIObjectPrototype.transform.localScale;
		val.transform.localPosition = _GUIObjectPrototype.transform.localPosition;
		val.transform.localRotation = _GUIObjectPrototype.transform.localRotation;
		SpeechBubble speechBubble = new SpeechBubble();
		speechBubble.Author = author;
		speechBubble.Text = text;
		speechBubble.EmotionName = ((text != null) ? null : emotion);
		speechBubble.StartTime = Time.time;
		speechBubble.LifeTime = BaseLifeTime + ((text == null) ? 0f : (SymbolLifeTime * (float)text.Length));
		speechBubble.GUIObject = val;
		if (text != null)
		{
			speechBubble.GUIObjectTransforms = new Transform[3]
			{
				((Component)component2).transform,
				((Component)component3).transform,
				((Component)component).transform
			};
			speechBubble.GUIObjectBasePositions = new Vector3[3]
			{
				((Component)component2).transform.localPosition,
				((Component)component3).transform.localPosition,
				((Component)component).transform.localPosition
			};
			speechBubble.GUIObjectBaseScales = new Vector3[3]
			{
				((Component)component2).transform.localScale,
				((Component)component3).transform.localScale,
				((Component)component).transform.localScale
			};
		}
		_SpeechBubbles.Add(speechBubble);
		RefreshBubbleView(speechBubble);
		RaycastAuthor(speechBubble);
	}

	public static Emotion GetEmotion(string emotionName)
	{
		if ((Object)(object)_Instance != (Object)null)
		{
			Emotion[] emotions = _Instance.Emotions;
			foreach (Emotion emotion in emotions)
			{
				if (((Object)emotion.Picture).name == emotionName)
				{
					return emotion;
				}
			}
		}
		return null;
	}

	public static Emotion[] GetActiveEmotions()
	{
		if ((Object)(object)_Instance != (Object)null)
		{
			List<Emotion> list = new List<Emotion>();
			for (int i = 0; i < _Instance.Emotions.Length; i++)
			{
				if ((i <= 5 || i >= 18 || ProfileINI.GetPurchaseValue(StorePurchase.EMOTIONS) != 0) && (i < 18 || ProfileINI.GetPurchaseValue(StorePurchase.MEM_SMILES) != 0) && !(_Instance.Emotions[i].Text == "voiceChat"))
				{
					list.Add(_Instance.Emotions[i]);
				}
			}
			return list.ToArray();
		}
		return new Emotion[0];
	}

	private void Awake()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected Obj, but got Unknown
		_Instance = this;
		_GUIObjectPrototype = ((Component)KGUI.FindNode("hud.speech_bubbles.speech_bubble")).gameObject;
		_EmotionImage = ((Component)KGUI.FindNode("hud.emotion_image")).gameObject;
		foreach (Transform item in KGUI.FindNode("hud.speech_bubbles"))
		{
			Transform val = item;
			if (((Object)val).name != "speech_bubble")
			{
				Object.Destroy((Object)(object)((Component)val).gameObject);
			}
		}
	}

	private void Update()
	{
		bool flag = false;
		for (int num = _SpeechBubbles.Count - 1; num >= 0; num--)
		{
			SpeechBubble speechBubble = _SpeechBubbles[num];
			if ((Object)(object)speechBubble.Author == (Object)null || (Object)(object)speechBubble.Author.collider == (Object)null || Time.time - speechBubble.StartTime >= speechBubble.LifeTime)
			{
				Object.Destroy((Object)(object)speechBubble.GUIObject);
				_SpeechBubbles.RemoveAt(num);
			}
			else
			{
				if (Time.time - speechBubble.StartTime >= speechBubble.LifeTime - FadeTime)
				{
					speechBubble.Alpha = Mathf.MoveTowards(speechBubble.Alpha, 0f, Time.deltaTime * FadeTime);
				}
				else if (speechBubble.Alpha < 1f)
				{
					speechBubble.Alpha = Mathf.MoveTowards(speechBubble.Alpha, 1f, Time.deltaTime * FadeTime);
				}
				if (!((Object)(object)speechBubble.Author == (Object)(object)WorldGameObject.Instance.MainPlayer) || CameraController.Instance.IsThirdPerson)
				{
					if (Time.time - speechBubble.RaycastLastTime >= RaycastPeriod)
					{
						RaycastAuthor(speechBubble);
					}
					if (!speechBubble.RaycastSuccesfull)
					{
						if (speechBubble.GUIObject.activeInHierarchy)
						{
							speechBubble.GUIObject.SetActive(false);
						}
					}
					else
					{
						RefreshBubbleView(speechBubble);
					}
				}
				else
				{
					if (speechBubble.EmotionName != null)
					{
						flag = true;
						if (!_EmotionImage.activeInHierarchy)
						{
							Utils.SetActiveRecursively(_EmotionImage, enable: true);
							KGUI.SetControlSprite(_EmotionImage.transform, speechBubble.EmotionName, 128f);
							_EmotionImage.GetComponentInChildren<UIWidget>().alpha = speechBubble.Alpha;
							speechBubble.CurAlpha = speechBubble.Alpha;
						}
						if (speechBubble.Alpha != speechBubble.CurAlpha)
						{
							speechBubble.CurAlpha = speechBubble.Alpha;
							_EmotionImage.GetComponentInChildren<UIWidget>().alpha = speechBubble.Alpha;
						}
					}
					if (speechBubble.GUIObject.activeInHierarchy)
					{
						speechBubble.GUIObject.SetActive(false);
					}
				}
			}
		}
		if (!flag && _EmotionImage.activeInHierarchy)
		{
			Utils.SetActiveRecursively(_EmotionImage, enable: false);
		}
	}

	private void RefreshBubbleView(SpeechBubble bubble)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)CameraController.RaycastCamera == (Object)null)
		{
			if (bubble.GUIObject.activeInHierarchy)
			{
				bubble.GUIObject.SetActive(false);
			}
			return;
		}
		Vector3 position = bubble.Author.transform.position;
		if ((Object)(object)bubble.Author.collider != (Object)null && bubble.Author.collider.enabled)
		{
			float y = position.y;
			Bounds bounds = bubble.Author.collider.bounds;
			position.y = y + (bounds.size.y + 0.1f);
		}
		else
		{
			position.y += 1.88f;
		}
		position = CameraController.RaycastCamera.WorldToScreenPoint(position);
		if (position.z <= 0f)
		{
			if (bubble.GUIObject.activeInHierarchy)
			{
				bubble.GUIObject.SetActive(false);
			}
			return;
		}
		if (!bubble.GUIObject.activeInHierarchy)
		{
			bubble.GUIObject.SetActive(true);
		}
		float num = Vector3.Distance(((Component)CameraController.RaycastCamera).transform.position, bubble.Author.transform.position);
		float num2 = 1f - Mathf.Clamp01(num / RaycastDistance) / 2f;
		if (bubble.EmotionName != null)
		{
			num2 /= 2f;
		}
		num2 = Mathf.Round(num2 * 100f) / 100f;
		position = KGUI.ScreenToGUIPoint(position);
		position.z = 0f - num / RaycastDistance;
		position = NGUITools.Round(position);
		if (bubble.GUIObject.transform.localPosition != position)
		{
			bubble.GUIObject.transform.localPosition = position;
		}
		if (bubble.Scale != num2)
		{
			bubble.Scale = num2;
			if (bubble.GUIObjectTransforms != null)
			{
				for (int i = 0; i < bubble.GUIObjectTransforms.Length; i++)
				{
					Vector3 localPosition = bubble.GUIObjectBasePositions[i] * num2;
					localPosition.x = Mathf.Floor(localPosition.x);
					localPosition.y = Mathf.Floor(localPosition.y);
					bubble.GUIObjectTransforms[i].localPosition = localPosition;
					localPosition = bubble.GUIObjectBaseScales[i] * num2;
					if (i != bubble.GUIObjectTransforms.Length - 1)
					{
						localPosition.x = Mathf.Floor(localPosition.x);
						localPosition.y = Mathf.Floor(localPosition.y);
					}
					bubble.GUIObjectTransforms[i].localScale = localPosition;
				}
			}
			else
			{
				bubble.GUIObject.transform.localScale = new Vector3(num2, num2, 1f);
			}
		}
		if (bubble.Alpha != bubble.CurAlpha)
		{
			bubble.CurAlpha = bubble.Alpha;
			UIWidget[] componentsInChildren = bubble.GUIObject.GetComponentsInChildren<UIWidget>();
			foreach (UIWidget uIWidget in componentsInChildren)
			{
				uIWidget.alpha = bubble.Alpha;
			}
		}
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
		if ((Object)(object)bubble.Author == (Object)(object)WorldGameObject.Instance.MainPlayer)
		{
			bubble.RaycastSuccesfull = true;
			return;
		}
		Vector3 position = ((Component)CameraController.RaycastCamera).transform.position;
		PlayerNetwork component = bubble.Author.GetComponent<PlayerNetwork>();
		Vector3 val;
		if ((Object)(object)component != (Object)null && (Object)(object)component.HeadBone != (Object)null && component.HeadBone.transform.childCount > 0)
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
