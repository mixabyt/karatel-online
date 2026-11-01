using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KGUI : MonoBehaviour
{
	public static GameObject CallbackSender;

	public static int SlotIndex;

	public static int SlotIndex2;

	private static KGUI _Instance;

	private static Dictionary<string, Action> _ControlsCallbacks = new Dictionary<string, Action>();

	private static Dictionary<string, Func<string>> _ControlsCallbacksStr = new Dictionary<string, Func<string>>();

	private static List<List<string>> _ModalControls = new List<List<string>>();

	private static string _ScrollGrid = null;

	private static Transform _ScrollGridNode = null;

	private static Dictionary<UILabel, string> _LabelsInitText = new Dictionary<UILabel, string>();

	private void Awake()
	{
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Expected Obj, but got Unknown
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)_Instance != (Object)null)
		{
			return;
		}
		_Instance = this;
		Utils.SetActiveRecursively(FindNode("Anchor"), enable: true);
		List<Transform> list = new List<Transform>();
		list.Add(FindNode("bonus.txt_count"));
		list.Add(FindNode("main_menu_shop.txt_item_name"));
		list.Add(FindNode("hud.chat.label"));
		list.Add(FindNode("hud.chat.txt_text"));
		list.Add(FindNode("hud.kills_list.txt_killer"));
		list.Add(FindNode("hud.kills_list.txt_victim"));
		list.Add(FindNode("hud.speech_bubble.txt_text"));
		list.Add(FindNode("hud.txt_cheating"));
		list.Add(FindNode("main_menu_top_maps.txt_author"));
		list.Add(FindNode("tab_menu.txt_author"));
		list.Add(FindNode("team_battle.page1.txt_team"));
		list.Add(FindNode("team_battle.page2.txt_team"));
		list.Add(FindNode("plate.inp_line1.label"));
		list.Add(FindNode("plate.inp_line2.label"));
		list.Add(FindNode("plate.inp_line3.label"));
		list.Add(FindNode("plate.inp_line4.label"));
		list.Add(FindNode("book.inp_page_left.label"));
		list.Add(FindNode("book.inp_page_right.label"));
		list.Add(FindNode("main_menu_bank.gems1.txt_action"));
		list.Add(FindNode("main_menu_bank.gems2.txt_action"));
		list.Add(FindNode("main_menu_bank.gems3.txt_action"));
		list.Add(FindNode("main_menu_bank.gems4.txt_action"));
		list.Add(FindNode("main_menu_bank.gems5.txt_action"));
		list.Add(FindNode("main_menu_bank.gold1.txt_action"));
		list.Add(FindNode("main_menu_bank.gold2.txt_action"));
		list.Add(FindNode("main_menu_bank.gold3.txt_action"));
		list.Add(FindNode("main_menu_bank.gold4.txt_action"));
		list.Add(FindNode("main_menu_bank.gold5.txt_action"));
		list.Add(FindNode("common.tooltip.text"));
		List<Transform> list2 = list;
		Object[] array = Object.FindObjectsOfType(typeof(UILabel));
		for (int i = 0; i < array.Length; i++)
		{
			UILabel uILabel = (UILabel)(object)array[i];
			_LabelsInitText.Add(uILabel, uILabel.text);
			if (!list2.Contains(((Component)uILabel).transform))
			{
				uILabel.color = new Color(90f / 255f, 90f / 255f, 70f / 255f);
				uILabel.effectStyle = UILabel.Effect.Outline;
				uILabel.effectDistance = new Vector2(2f, 2f);
				uILabel.effectColor = new Color(206f / 255f, 206f / 255f, 144f / 255f);
			}
		}
		Object[] array2 = Object.FindObjectsOfType(typeof(UIInput));
		for (int j = 0; j < array2.Length; j++)
		{
			UIInput uIInput = (UIInput)(object)array2[j];
			if (((Object)uIInput).name[0] != '@')
			{
				uIInput.label.text = string.Empty;
				uIInput.text = string.Empty;
			}
		}
		SetLocale((Localize.LocaleType)PlayerPrefs.GetInt("locale", 0));
		NGUITools.MakePixelPerfect(((Component)_Instance).transform);
		if (!Application.isEditor)
		{
			foreach (Transform item in FindNode("Anchor"))
			{
				Transform val = item;
				Vector3 localPosition = ((Component)val).transform.localPosition;
				localPosition.x -= 0.5f;
				localPosition.y += 0.5f;
				val.localPosition = localPosition;
			}
		}
		EnableNodes("Anchor", enable: false, onlyChild: true);
		EnableNodes("common", enable: true);
		ProfileINI.LoadFromPrefs();
		((Component)this).gameObject.AddComponent<AudioListener>();
		((Component)this).gameObject.AddComponent<AudioSource>();
	}

	private IEnumerator OnLevelWasLoaded()
	{
		if ((Object)(object)this != (Object)(object)_Instance)
		{
			yield break;
		}
		while (true)
		{
			if (Application.loadedLevel == 0)
			{
				((Behaviour)((Component)this).GetComponent<AudioListener>()).enabled = true;
				yield break;
			}
			AudioListener[] audioListeners = Object.FindObjectsOfType(typeof(AudioListener)) as AudioListener[];
			if (audioListeners.Length > 1)
			{
				break;
			}
			yield return 0;
		}
		((Behaviour)((Component)this).GetComponent<AudioListener>()).enabled = false;
	}

	public static void EnableAudioListener(bool enable)
	{
		((Behaviour)((Component)_Instance).GetComponent<AudioListener>()).enabled = enable;
	}

	public static void SetLocale(Localize.LocaleType locale)
	{
		Localize.Locale = locale;
		PlayerPrefs.SetInt("locale", (int)locale);
		foreach (KeyValuePair<UILabel, string> item in _LabelsInitText)
		{
			item.Key.text = Localize.GetText(item.Value, item.Key.text);
		}
	}

	public static void EnableInput(bool enable)
	{
		List<string> list = new List<string>();
		list.Add(string.Empty);
		SetModal(list, !enable);
	}

	public static void SetModal(List<string> controlsName, bool enable)
	{
		if (enable)
		{
			if (_ModalControls.Contains(controlsName))
			{
				_ModalControls.Remove(controlsName);
			}
			_ModalControls.Add(controlsName);
		}
		else
		{
			_ModalControls.Remove(controlsName);
		}
		EnableCollider(((Component)_Instance).transform, null, enable: true);
		if (_ModalControls.Count > 0)
		{
			EnableCollider(((Component)_Instance).transform, _ModalControls[_ModalControls.Count - 1], enable: false);
		}
	}

	private static void EnableCollider(Transform node, List<string> skipNodes, bool enable)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected Obj, but got Unknown
		if (skipNodes != null && skipNodes.Contains(((Object)node).name))
		{
			return;
		}
		if ((Object)(object)((Component)node).collider != (Object)null)
		{
			((Component)node).collider.enabled = enable;
		}
		foreach (Transform item in node)
		{
			Transform node2 = item;
			EnableCollider(node2, skipNodes, enable);
		}
	}

	public static void SetControlText(string nodeName, string text)
	{
		Transform val = FindNode(nodeName);
		UIInput component = ((Component)val).GetComponent<UIInput>();
		if ((Object)(object)component != (Object)null)
		{
			component.text = text;
		}
		else
		{
			((Component)val).GetComponent<UILabel>().text = text;
		}
	}

	public static string GetControlText(string nodeName)
	{
		Transform val = FindNode(nodeName);
		UIInput component = ((Component)val).GetComponent<UIInput>();
		if ((Object)(object)component != (Object)null)
		{
			return component.text;
		}
		return ((Component)val).GetComponentInChildren<UILabel>().text;
	}

	public static void SetControlCheckbox(string nodeName, bool isChecked)
	{
		((Component)FindNode(nodeName)).GetComponent<UICheckbox>().isChecked = isChecked;
	}

	public static bool GetControlCheckbox(string nodeName)
	{
		return ((Component)FindNode(nodeName)).GetComponent<UICheckbox>().isChecked;
	}

	public static void ResizeGrid(string nodeName, int size, Action<GameObject, int> fillContent, string scrollName = null)
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Expected Obj, but got Unknown
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		_ScrollGrid = null;
		Transform val = FindNode(nodeName);
		if (!((Component)val).gameObject.activeInHierarchy)
		{
			return;
		}
		Utils.SetActiveRecursively(val, enable: true);
		Transform val2 = val.Find("slot_prototype");
		bool flag = false;
		int num = val.childCount - 1;
		int num2 = Mathf.Max(size, num);
		for (int i = 0; i < num2; i++)
		{
			if (i < size)
			{
				GameObject val3;
				if (i >= num)
				{
					val3 = (GameObject)Object.Instantiate((Object)(object)((Component)val2).gameObject);
					Object.Destroy((Object)(object)val3.GetComponent<UIPanel>());
					((Object)val3).name = i.ToString();
					val3.transform.parent = val;
					val3.transform.localScale = ((Component)val2).transform.localScale;
					val3.transform.localPosition = ((Component)val2).transform.localPosition;
					val3.transform.localRotation = ((Component)val2).transform.localRotation;
					flag = true;
				}
				else
				{
					val3 = ((Component)val.Find(i.ToString())).gameObject;
				}
				fillContent(val3, i);
			}
			else
			{
				Utils.SetActiveRecursively(val.Find(i.ToString()), enable: false);
			}
		}
		Utils.SetActiveRecursively(val2, enable: false);
		if (flag)
		{
			((Component)val).GetComponent<UITable>().repositionNow = true;
		}
		if ((Object)(object)((Component)val.parent).GetComponent<UIDraggablePanel>() != (Object)null)
		{
			((MonoBehaviour)_Instance).StartCoroutine(ResetScrollBarProcess(((Component)val.parent).GetComponent<UIDraggablePanel>(), scrollName));
		}
	}

	public static void ResetScrollBar(string draggablePanelName, string scrollName = null)
	{
		UIDraggablePanel component = ((Component)FindNode(draggablePanelName)).GetComponent<UIDraggablePanel>();
		((MonoBehaviour)_Instance).StartCoroutine(ResetScrollBarProcess(component, scrollName));
	}

	private static IEnumerator ResetScrollBarProcess(UIDraggablePanel panel, string scrollName = null)
	{
		yield return 0;
		yield return 0;
		float scrollValue = 1f;
		if ((Object)(object)panel.verticalScrollBar != (Object)null)
		{
			scrollValue = panel.verticalScrollBar.sliderValue;
		}
		panel.RestrictWithinBounds(instant: true);
		panel.ResetPosition();
		if ((Object)(object)panel.verticalScrollBar != (Object)null)
		{
			bool showSlider = panel.showScrollBars == UIDraggablePanel.ShowCondition.Always || panel.shouldMoveVertically;
			if (showSlider != ((Component)((Component)panel.verticalScrollBar).transform.parent).gameObject.activeInHierarchy)
			{
				Utils.SetActiveRecursively(((Component)panel.verticalScrollBar).transform.parent, showSlider);
			}
			_ScrollGrid = scrollName;
			_ScrollGridNode = ((Component)panel).transform;
			if (_ScrollGrid != null && ((Component)_ScrollGridNode).gameObject.activeInHierarchy)
			{
				scrollValue = PlayerPrefs.GetFloat("Scroll_" + _ScrollGrid, 1f);
			}
			panel.verticalScrollBarValue = -1f;
			panel.verticalScrollBar.sliderValue = scrollValue;
		}
	}

	public static void EnableNodes(string nodeName, bool enable, bool onlyChild = false)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected Obj, but got Unknown
		Transform val = FindNode(nodeName);
		if (!onlyChild)
		{
			if (((Component)val).gameObject.activeInHierarchy != enable)
			{
				Utils.SetActiveRecursively(val, enable);
				UpdateControls(val);
			}
			return;
		}
		foreach (Transform item in val)
		{
			Transform val2 = item;
			if (((Component)val2).gameObject.activeInHierarchy != enable)
			{
				Utils.SetActiveRecursively(val2, enable);
				UpdateControls(val2);
			}
		}
	}

	private static void UpdateControls(Transform node)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected Obj, but got Unknown
		if (!((Component)node).gameObject.activeInHierarchy)
		{
			return;
		}
		UIWidget[] components = ((Component)node).gameObject.GetComponents<UIWidget>();
		foreach (UIWidget uIWidget in components)
		{
			uIWidget.Update();
		}
		foreach (Transform item in node)
		{
			Transform node2 = item;
			UpdateControls(node2);
		}
	}

	public static Transform FindNode(string nodeName, bool mayBeNull = false)
	{
		if (nodeName == "Anchor")
		{
			return ((Component)_Instance).transform;
		}
		Transform node = ((Component)_Instance).transform;
		while (true)
		{
			int num = nodeName.IndexOf('.');
			if (num == -1)
			{
				break;
			}
			node = FindNodeEx(node, nodeName.Substring(0, num));
			nodeName = nodeName.Substring(num + 1);
		}
		node = FindNodeEx(node, nodeName);
		if ((Object)(object)node == (Object)null && !mayBeNull)
		{
			Debug.LogError((object)("Node " + nodeName + " not found!"));
		}
		return node;
	}

	private static Transform FindNodeEx(Transform node, string nodeName)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected Obj, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected Obj, but got Unknown
		foreach (Transform item in node)
		{
			Transform val = item;
			if (((Object)val).name == nodeName)
			{
				return val;
			}
		}
		foreach (Transform item2 in node)
		{
			Transform node2 = item2;
			Transform val2 = FindNodeEx(node2, nodeName);
			if ((Object)(object)val2 != (Object)null)
			{
				return val2;
			}
		}
		return null;
	}

	public static void SetButtonCallback(string buttonName, Action func)
	{
		Transform val = FindNode(buttonName);
		_ControlsCallbacks.Add(NodeUniqueName(val), func);
		UIButtonMessage uIButtonMessage = ((Component)val).gameObject.AddComponent<UIButtonMessage>();
		uIButtonMessage.target = ((Component)_Instance).gameObject;
		uIButtonMessage.functionName = "OnButtonClick";
		uIButtonMessage.trigger = UIButtonMessage.Trigger.OnClick;
		uIButtonMessage.includeChildren = false;
	}

	public static void SetButtonCallback(string[] buttonNames, Action func)
	{
		foreach (string buttonName in buttonNames)
		{
			SetButtonCallback(buttonName, func);
		}
	}

	private void OnButtonClick(GameObject sender)
	{
		SoundManager.Instance.Play(SoundManager.Sound.MenuClick);
		CallControlCallback(sender);
	}

	public static void SetSliderCallback(string sliderName, Action func)
	{
		Transform val = FindNode(sliderName);
		_ControlsCallbacks.Add(NodeUniqueName(val), func);
		UISlider component = ((Component)val).GetComponent<UISlider>();
		component.eventReceiver = ((Component)_Instance).gameObject;
		component.functionName = "OnSliderChange";
	}

	private void OnSliderChange(float value)
	{
		CallControlCallback(((Component)UISlider.current).gameObject);
	}

	public static void SetCheckboxCallback(string checkboxName, Action func)
	{
		Transform val = FindNode(checkboxName);
		_ControlsCallbacks.Add(NodeUniqueName(val), func);
		UICheckbox component = ((Component)val).GetComponent<UICheckbox>();
		component.eventReceiver = ((Component)_Instance).gameObject;
		component.functionName = "OnCheckboxActivate";
	}

	private void OnCheckboxActivate(bool value)
	{
		CallControlCallback(((Component)UICheckbox.current).gameObject);
	}

	public static void SetInputCallback(string inputName, Action func)
	{
		Transform val = FindNode(inputName);
		_ControlsCallbacks.Add(NodeUniqueName(val), func);
		UIForwardEvents uIForwardEvents = ((Component)val).gameObject.AddComponent<UIForwardEvents>();
		uIForwardEvents.target = ((Component)_Instance).gameObject;
		uIForwardEvents.onInputChanged = true;
	}

	private void OnInputChanged(GameObject from)
	{
		CallControlCallback(from);
	}

	public static void SetTooltipCallback(string tooltipName, Func<string> func)
	{
		Transform val = FindNode(tooltipName);
		_ControlsCallbacksStr.Add(NodeUniqueName(val), func);
		UIForwardEvents uIForwardEvents = ((Component)val).gameObject.AddComponent<UIForwardEvents>();
		uIForwardEvents.target = ((Component)_Instance).gameObject;
		uIForwardEvents.onTooltip = true;
	}

	private void OnTooltipShow(GameObject from)
	{
		string tooltipText = CallControlCallback(from, returnStr: true);
		UITooltip.ShowText(tooltipText);
	}

	private void OnTooltipHide(GameObject from)
	{
		UITooltip.ShowText(null);
	}

	private static string NodeUniqueName(Transform node)
	{
		Transform val = node;
		while (((Object)val.parent).name != "Anchor")
		{
			val = val.parent;
		}
		string text = ((Object)val).name + ((Object)node).name;
		if (((Object)node.parent).name == "slot_prototype" || int.TryParse(((Object)node.parent).name, out var _))
		{
			return text + ((Object)node.parent.parent).name;
		}
		return text + ((Object)node.parent).name;
	}

	private static string CallControlCallback(GameObject sender, bool returnStr = false)
	{
		if ((Object)(object)((Component)_Instance).transform.root != (Object)(object)sender.transform.root)
		{
			return null;
		}
		CallbackSender = sender;
		if (!int.TryParse(((Object)CallbackSender.transform.parent).name, out SlotIndex) && !int.TryParse(((Object)CallbackSender.transform.parent.parent).name, out SlotIndex))
		{
			SlotIndex = -1;
		}
		if (!int.TryParse(((Object)CallbackSender.transform.parent.parent.parent).name, out SlotIndex2))
		{
			SlotIndex2 = -1;
		}
		if (!returnStr)
		{
			_ControlsCallbacks[NodeUniqueName(sender.transform)]();
			return null;
		}
		return _ControlsCallbacksStr[NodeUniqueName(sender.transform)]();
	}

	public static void SetControlSprite(Transform imageNode, string spriteName, float size)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		UISprite component = ((Component)imageNode.GetChild(0)).GetComponent<UISprite>();
		component.spriteName = spriteName;
		component.MakePixelPerfect();
		if (size != 0f)
		{
			Vector3 localScale = ((Component)component).transform.localScale;
			float num = Mathf.Clamp01(Mathf.Min(size / localScale.x, size / localScale.y));
			imageNode.localScale = new Vector3(num, num, 1f);
		}
	}

	public static void SavePanelScroll(UISlider slider)
	{
		if (_ScrollGrid != null && ((Component)_ScrollGridNode).gameObject.activeInHierarchy)
		{
			PlayerPrefs.SetFloat("Scroll_" + _ScrollGrid, slider.sliderValue);
		}
	}

	public static Vector3 ScreenToGUIPoint(Vector3 pos)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		float num = 1f / UIRoot.GetPixelSizeAdjustment(((Component)_Instance).gameObject);
		return new Vector3((pos.x - (float)(Screen.width / 2)) / num, (pos.y - (float)(Screen.height / 2)) / num, 1f);
	}
}
