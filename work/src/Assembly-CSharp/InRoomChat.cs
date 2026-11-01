using System.Collections.Generic;
using Photon;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class InRoomChat : MonoBehaviour
{
	public Rect GuiRect = new Rect(0f, 0f, 250f, 300f);

	public bool IsVisible = true;

	public bool AlignBottom;

	public List<string> messages = new List<string>();

	private string inputLine = string.Empty;

	private Vector2 scrollPos = Vector2.zero;

	public static readonly string ChatRPC = "Chat";

	public InRoomChat()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
	}

	public void Start()
	{
		if (AlignBottom)
		{
			GuiRect.y = (float)Screen.height - GuiRect.height;
		}
	}

	public void OnGUI()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Invalid comparison between Unknown and I4
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Invalid comparison between Unknown and I4
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Invalid comparison between Unknown and I4
		if (!IsVisible || PhotonNetwork.connectionStateDetailed != PeerState.Joined)
		{
			return;
		}
		if ((int)Event.current.type == 4 && ((int)Event.current.keyCode == 271 || (int)Event.current.keyCode == 13))
		{
			if (!string.IsNullOrEmpty(inputLine))
			{
				photonView.RPC("Chat", PhotonTargets.All, inputLine);
				inputLine = string.Empty;
				GUI.FocusControl(string.Empty);
				return;
			}
			GUI.FocusControl("ChatInput");
		}
		GUI.SetNextControlName(string.Empty);
		GUILayout.BeginArea(GuiRect);
		scrollPos = GUILayout.BeginScrollView(scrollPos, new GUILayoutOption[0]);
		GUILayout.FlexibleSpace();
		for (int num = messages.Count - 1; num >= 0; num--)
		{
			GUILayout.Label(messages[num], new GUILayoutOption[0]);
		}
		GUILayout.EndScrollView();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUI.SetNextControlName("ChatInput");
		inputLine = GUILayout.TextField(inputLine, new GUILayoutOption[0]);
		if (GUILayout.Button("Send", new GUILayoutOption[1] { GUILayout.ExpandWidth(false) }))
		{
			photonView.RPC("Chat", PhotonTargets.All, inputLine);
			inputLine = string.Empty;
			GUI.FocusControl(string.Empty);
		}
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}

	[RPC]
	public void Chat(string newLine, PhotonMessageInfo mi)
	{
		string text = "anonymous";
		if (mi != null && mi.sender != null)
		{
			text = (string.IsNullOrEmpty(mi.sender.name) ? ("player " + mi.sender.ID) : mi.sender.name);
		}
		messages.Add(text + ": " + newLine);
	}

	public void AddLine(string newLine)
	{
		messages.Add(newLine);
	}
}
