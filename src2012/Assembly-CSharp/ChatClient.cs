using Photon;
using UnityEngine;

public class ChatClient : MonoBehaviour
{
	public static bool isEnabled;

	public static ChatClient instance;

	public string prefix = "> ";

	public string cursor = "_";

	public MessageList messageList;

	private string input = string.Empty;

	private bool showCursor = true;

	private void Awake()
	{
		instance = this;
		((MonoBehaviour)this).InvokeRepeating("BlinkCursor", 0.5f, 0.5f);
	}

	private void BlinkCursor()
	{
		showCursor = !showCursor;
	}

	private void Update()
	{
		if (MainMenu.INSTANCE.showMenu || Book.IsAnyOpened)
		{
			return;
		}
		string inputString = Input.inputString;
		foreach (char c in inputString)
		{
			switch (c)
			{
			case '\b':
				if (isEnabled && input.Length != 0)
				{
					input = input.Substring(0, input.Length - 1);
				}
				break;
			case '\n':
			case '\r':
				WorldGameObject.INSTANCE.MainPlayer.SendMessage("SetMovement", (object)isEnabled);
				if (isEnabled)
				{
					messageList.HideAllMessage();
					if (input.Length > 0)
					{
						SendChatMessage(ProfileINI.nickname + ": " + input);
					}
					input = string.Empty;
				}
				else
				{
					messageList.ShowAllMessage();
				}
				isEnabled = !isEnabled;
				break;
			default:
				if (isEnabled && input.Length < 100)
				{
					input += c;
				}
				break;
			}
		}
		((Behaviour)((Component)this).guiText).enabled = isEnabled;
		((Component)this).guiText.text = prefix + input + ((!showCursor) ? string.Empty : cursor);
	}

	public void SendChatMessage(string message)
	{
		photonView.RPC("Chat", PhotonTargets.All, message, bs._Igor.spectate);
	}

	public void SendInfoMessage(string message)
	{
		if (!bs._Igor.spectate)
		{
			photonView.RPC("Info", PhotonTargets.All, message);
		}
	}

	public void SendWarningMessage(string message)
	{
		photonView.RPC("Warning", PhotonTargets.All, message);
	}

	public void SendEmote(string emoteName)
	{
		if (!bs._Igor.spectate)
		{
			photonView.RPC("Emote", PhotonTargets.All, ProfileINI.nickname, emoteName);
		}
	}

	[RPC]
	private void Chat(string message, bool spectate)
	{
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		int num = message.IndexOf(": ");
		if (num != -1 && !spectate)
		{
			PlayerNode playerNode = WorldGameObject.INSTANCE.FindPlayerByName(message.Substring(0, num));
			if (playerNode != null && playerNode != WorldGameObject.INSTANCE.MainPlayerNode && (Object)(object)playerNode.avatar != (Object)null)
			{
				SpeechBubbles.AddText(playerNode.avatar, message.Substring(num + 2, message.Length - num - 2));
			}
			if (playerNode != null && playerNode == WorldGameObject.INSTANCE.MainPlayerNode)
			{
				SpeechBubbles.AddText(WorldGameObject.INSTANCE.MainPlayer, message.Substring(num + 2, message.Length - num - 2));
			}
		}
		messageList.AddMessage(message, null, Color.white);
	}

	[RPC]
	public void Info(string message)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		messageList.AddMessage(message, null, Color.yellow);
	}

	[RPC]
	public void Warning(string message)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)WorldGameObject.INSTANCE != (Object)null && (Object)(object)WorldGameObject.INSTANCE.MainPlayer != (Object)null)
		{
			ResourceLoader.I.PlayEffect(ResourceLoader.eSounds.Note2_1, WorldGameObject.INSTANCE.MainPlayer.audio);
		}
		messageList.AddMessage(message, null, Color.red);
	}

	[RPC]
	private void Emote(string playerName, string emoteName)
	{
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		SpeechBubbles.Emote emote = SpeechBubbles.GetEmote(emoteName);
		if (emote != null)
		{
			PlayerNode playerNode = WorldGameObject.INSTANCE.FindPlayerByName(playerName);
			if (playerNode != null && playerNode != WorldGameObject.INSTANCE.MainPlayerNode && (Object)(object)playerNode.avatar != (Object)null)
			{
				SpeechBubbles.AddPicture(playerNode.avatar, emote.Picture);
			}
			if (playerNode != null && playerNode == WorldGameObject.INSTANCE.MainPlayerNode)
			{
				SpeechBubbles.AddPicture(WorldGameObject.INSTANCE.MainPlayer, emote.Picture);
			}
			if (!string.IsNullOrEmpty(emote.Text))
			{
				messageList.AddMessage(playerName + ": **" + emote.Text + "** ", null, Color.white);
			}
		}
	}
}
