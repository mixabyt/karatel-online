using ExitGames.Client.Photon;
using UnityEngine;

public class PhotonStatsGui : MonoBehaviour
{
	public bool statsWindowOn = true;

	public bool statsOn = true;

	public bool healthStatsVisible;

	public bool trafficStatsOn;

	public bool buttonsOn;

	public Rect statsRect = new Rect(0f, 100f, 200f, 50f);

	public int WindowId = 100;

	public PhotonStatsGui()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
	}

	public void Start()
	{
		statsRect.x = (float)Screen.width - statsRect.width;
	}

	public void Update()
	{
		if (Input.GetKeyDown((KeyCode)9) && Input.GetKey((KeyCode)304))
		{
			statsWindowOn = !statsWindowOn;
			statsOn = true;
		}
	}

	public void OnGUI()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected Obj, but got Unknown
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (PhotonNetwork.networkingPeer.TrafficStatsEnabled != statsOn)
		{
			PhotonNetwork.networkingPeer.TrafficStatsEnabled = statsOn;
		}
		if (statsWindowOn)
		{
			statsRect = GUILayout.Window(WindowId, statsRect, (WindowFunction)TrafficStatsWindow, "Messages (shift+tab)", new GUILayoutOption[0]);
		}
	}

	public void TrafficStatsWindow(int windowID)
	{
		bool flag = false;
		TrafficStatsGameLevel trafficStatsGameLevel = PhotonNetwork.networkingPeer.TrafficStatsGameLevel;
		long num = PhotonNetwork.networkingPeer.TrafficStatsElapsedMs / 1000;
		if (num == 0L)
		{
			num = 1L;
		}
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		buttonsOn = GUILayout.Toggle(buttonsOn, "buttons", new GUILayoutOption[0]);
		healthStatsVisible = GUILayout.Toggle(healthStatsVisible, "health", new GUILayoutOption[0]);
		trafficStatsOn = GUILayout.Toggle(trafficStatsOn, "traffic", new GUILayoutOption[0]);
		GUILayout.EndHorizontal();
		string text = $"Out|In|Sum:\t{trafficStatsGameLevel.TotalOutgoingMessageCount,4} | {trafficStatsGameLevel.TotalIncomingMessageCount,4} | {trafficStatsGameLevel.TotalMessageCount,4}";
		string text2 = $"{num}sec average:";
		string text3 = $"Out|In|Sum:\t{trafficStatsGameLevel.TotalOutgoingMessageCount / num,4} | {trafficStatsGameLevel.TotalIncomingMessageCount / num,4} | {trafficStatsGameLevel.TotalMessageCount / num,4}";
		GUILayout.Label(text, new GUILayoutOption[0]);
		GUILayout.Label(text2, new GUILayoutOption[0]);
		GUILayout.Label(text3, new GUILayoutOption[0]);
		if (buttonsOn)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			statsOn = GUILayout.Toggle(statsOn, "stats on", new GUILayoutOption[0]);
			if (GUILayout.Button("Reset", new GUILayoutOption[0]))
			{
				PhotonNetwork.networkingPeer.TrafficStatsReset();
				PhotonNetwork.networkingPeer.TrafficStatsEnabled = true;
			}
			flag = GUILayout.Button("To Log", new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
		}
		string text4 = string.Empty;
		string text5 = string.Empty;
		if (trafficStatsOn)
		{
			text4 = "Incoming: " + PhotonNetwork.networkingPeer.TrafficStatsIncoming.ToString();
			text5 = "Outgoing: " + PhotonNetwork.networkingPeer.TrafficStatsOutgoing.ToString();
			GUILayout.Label(text4, new GUILayoutOption[0]);
			GUILayout.Label(text5, new GUILayoutOption[0]);
		}
		string text6 = string.Empty;
		if (healthStatsVisible)
		{
			text6 = string.Format("ping: {6}[+/-{7}]ms\nlongest delta between\nsend: {0,4}ms disp: {1,4}ms\nlongest time for:\nev({3}):{2,3}ms op({5}):{4,3}ms", trafficStatsGameLevel.LongestDeltaBetweenSending, trafficStatsGameLevel.LongestDeltaBetweenDispatching, trafficStatsGameLevel.LongestEventCallback, trafficStatsGameLevel.LongestEventCallbackCode, trafficStatsGameLevel.LongestOpResponseCallback, trafficStatsGameLevel.LongestOpResponseCallbackOpCode, PhotonNetwork.networkingPeer.RoundTripTime, PhotonNetwork.networkingPeer.RoundTripTimeVariance);
			GUILayout.Label(text6, new GUILayoutOption[0]);
		}
		if (flag)
		{
			string text7 = $"{text}\n{text2}\n{text3}\n{text4}\n{text5}\n{text6}";
			Debug.Log((object)text7);
		}
		if (GUI.changed)
		{
			statsRect.height = 100f;
		}
		GUI.DragWindow();
	}
}
