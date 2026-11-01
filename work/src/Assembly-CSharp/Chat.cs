using System.Collections.Generic;
using Photon;
using UnityEngine;

public class Chat : MonoBehaviour
{
	private class Message
	{
		public string Text;

		public float StartTime;

		public UILabel Label;
	}

	public const int MaxMessages = 15;

	public const float DisappearDelay = 5f;

	public const float DisappearTime = 1f;

	private static Chat _Instance;

	private List<Message> _Messages = new List<Message>();

	private GameObject _Grid;

	private UIInput _Input;

	private bool _IgnoreNextEnter;

	private void Awake()
	{
		_Instance = this;
		_Grid = ((Component)KGUI.FindNode("hud.chat.grid_messages")).gameObject;
		_Input = ((Component)KGUI.FindNode("hud.chat.inp_message")).GetComponent<UIInput>();
		_Input.onSubmit = OnSubmit;
		KGUI.ResizeGrid("hud.chat.grid_messages", 0, (GameObject slot, int index) =>
		{
		});
	}

	private void Update()
	{
		if ((Input.GetKeyUp((KeyCode)13) || Input.GetKeyUp((KeyCode)271)) && !MainMenu.Instance.MenuActive)
		{
			if (!_IgnoreNextEnter && !_Input.selected)
			{
				WorldGameObject.Instance.MainPlayer.GetComponent<PlayerInput>().StopMovement();
				_Input.selected = true;
				ActivateGrid(activate: true);
				RefreshAlpha();
			}
			_IgnoreNextEnter = false;
		}
		if (_Grid.activeInHierarchy)
		{
			RefreshAlpha();
		}
		if (!_Input.selected && _Input.text.Length > 0)
		{
			_Input.text = string.Empty;
		}
	}

	public static bool IsEnabled()
	{
		if ((Object)(object)_Instance == (Object)null)
		{
			return false;
		}
		return _Instance._Input.selected;
	}

	public static void SendText(string message, bool forAll)
	{
		if (!((Object)(object)_Instance == (Object)null))
		{
			if (forAll)
			{
				_Instance.photonView.RPC("Text", PhotonTargets.All, message, GameType.IsObserving());
			}
			else
			{
				_Instance.Text(message, GameType.IsObserving());
			}
		}
	}

	public static void SendInfo(string message, bool forAll)
	{
		if (!((Object)(object)_Instance == (Object)null) && !GameType.IsObserving())
		{
			if (forAll)
			{
				_Instance.photonView.RPC("Info", PhotonTargets.All, message);
			}
			else
			{
				_Instance.Info(message);
			}
		}
	}

	public static void SendWarning(string message, bool forAll)
	{
		if (!((Object)(object)_Instance == (Object)null))
		{
			if (forAll)
			{
				_Instance.photonView.RPC("Warning", PhotonTargets.All, message);
			}
			else
			{
				_Instance.Warning(message);
			}
		}
	}

	public static void SendEmotion(string emotionName, bool forAll)
	{
		if (!((Object)(object)_Instance == (Object)null) && !GameType.IsObserving())
		{
			if (forAll)
			{
				_Instance.photonView.RPC("Emotion", PhotonTargets.All, ProfileINI.nickname, emotionName);
			}
			else
			{
				_Instance.Emotion(ProfileINI.nickname, emotionName);
			}
		}
	}

	[RPC]
	private void Text(string message, bool spectate)
	{
		int num = message.IndexOf(": ");
		if (num != -1 && !spectate)
		{
			PlayerNode playerNode = WorldGameObject.Instance.FindPlayerByName(message.Substring(0, num));
			if (playerNode != null && playerNode != WorldGameObject.Instance.MainPlayerNode && (Object)(object)playerNode.Avatar != (Object)null)
			{
				SpeechBubbles.AddText(playerNode.Avatar, message.Substring(num + 2, message.Length - num - 2));
			}
			if (playerNode != null && playerNode == WorldGameObject.Instance.MainPlayerNode)
			{
				SpeechBubbles.AddText(WorldGameObject.Instance.MainPlayer, message.Substring(num + 2, message.Length - num - 2));
			}
		}
		AddMessage("[FFFFFF]" + message);
	}

	[RPC]
	private void Info(string message)
	{
		AddMessage("[FFFF00]" + message);
	}

	[RPC]
	private void Warning(string message)
	{
		if ((Object)(object)WorldGameObject.Instance != (Object)null && (Object)(object)WorldGameObject.Instance.MainPlayer != (Object)null)
		{
			SoundManager.Instance.Play(SoundManager.Sound.Note2, WorldGameObject.Instance.MainPlayer.audio);
		}
		AddMessage("[FF0000]" + message);
	}

	[RPC]
	private void Emotion(string playerName, string emotionName)
	{
		SpeechBubbles.Emotion emotion = SpeechBubbles.GetEmotion(emotionName);
		if (emotion != null)
		{
			PlayerNode playerNode = WorldGameObject.Instance.FindPlayerByName(playerName);
			if (playerNode != null && playerNode != WorldGameObject.Instance.MainPlayerNode && (Object)(object)playerNode.Avatar != (Object)null)
			{
				SpeechBubbles.AddEmotion(playerNode.Avatar, ((Object)emotion.Picture).name);
			}
			if (playerNode != null && playerNode == WorldGameObject.Instance.MainPlayerNode)
			{
				SpeechBubbles.AddEmotion(WorldGameObject.Instance.MainPlayer, ((Object)emotion.Picture).name);
			}
			if (!string.IsNullOrEmpty(emotion.Text) && emotion.Text != "voiceChat")
			{
				AddMessage("[FFFFFF]" + playerName + ": **" + Localize.GetText(emotion.Text) + "**");
			}
		}
	}

	private void OnSubmit(string inputText)
	{
		inputText = NGUITools.StripSymbols(inputText);
		if (!string.IsNullOrEmpty(inputText))
		{
			inputText = App.Instance.AntiMatSystem.GetGoodText(inputText);
			SendText(ProfileINI.nickname + ": " + inputText, forAll: true);
			_Input.text = string.Empty;
			_Input.selected = false;
		}
		_IgnoreNextEnter = true;
	}

	private void AddMessage(string text)
	{
		_Messages.Add(new Message
		{
			Text = text,
			StartTime = Time.time
		});
		while (_Messages.Count > 15)
		{
			_Messages.RemoveAt(0);
		}
		ActivateGrid(activate: true);
		KGUI.ResizeGrid("hud.chat.grid_messages", _Messages.Count, (GameObject slot, int index) =>
		{
			Message message = _Messages[_Messages.Count - index - 1];
			UILabel componentInChildren = slot.GetComponentInChildren<UILabel>();
			componentInChildren.text = message.Text;
			message.Label = componentInChildren;
		});
		RefreshAlpha();
	}

	private void RefreshAlpha()
	{
		float num = 0f;
		foreach (Message message in _Messages)
		{
			float num2 = 1f;
			if (!_Input.selected)
			{
				num2 = Mathf.Clamp01(1f - (Time.time - message.StartTime - 5f) / 1f);
			}
			if ((Object)(object)message.Label != (Object)null)
			{
				message.Label.alpha = num2;
			}
			num = Mathf.Max(num, num2);
		}
		if (num == 0f)
		{
			ActivateGrid(activate: false);
		}
	}

	private void ActivateGrid(bool activate)
	{
		_Grid.SetActive(activate);
		if (activate)
		{
			((Component)_Grid.transform.Find("slot_prototype")).gameObject.SetActive(false);
		}
	}
}
