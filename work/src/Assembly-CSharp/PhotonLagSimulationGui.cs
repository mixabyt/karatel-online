using ExitGames.Client.Photon;
using UnityEngine;

public class PhotonLagSimulationGui : MonoBehaviour
{
	public Rect WindowRect = new Rect(0f, 100f, 120f, 100f);

	public int WindowId = 101;

	public bool Visible = true;

	public PhotonPeer Peer { get; set; }

	public PhotonLagSimulationGui()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
	}

	public void Start()
	{
		Peer = PhotonNetwork.networkingPeer;
	}

	public void OnGUI()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Expected Obj, but got Unknown
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected Obj, but got Unknown
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (Visible)
		{
			if (Peer == null)
			{
				WindowRect = GUILayout.Window(WindowId, WindowRect, (WindowFunction)NetSimHasNoPeerWindow, "Netw. Sim.", new GUILayoutOption[0]);
			}
			else
			{
				WindowRect = GUILayout.Window(WindowId, WindowRect, (WindowFunction)NetSimWindow, "Netw. Sim.", new GUILayoutOption[0]);
			}
		}
	}

	private void NetSimHasNoPeerWindow(int windowId)
	{
		GUILayout.Label("No peer to communicate with. ", new GUILayoutOption[0]);
	}

	private void NetSimWindow(int windowId)
	{
		GUILayout.Label($"Rtt:{Peer.RoundTripTime,4} +/-{Peer.RoundTripTimeVariance,3}", new GUILayoutOption[0]);
		bool isSimulationEnabled = Peer.IsSimulationEnabled;
		bool flag = GUILayout.Toggle(isSimulationEnabled, "Simulate", new GUILayoutOption[0]);
		if (flag != isSimulationEnabled)
		{
			Peer.IsSimulationEnabled = flag;
		}
		float num = Peer.NetworkSimulationSettings.IncomingLag;
		GUILayout.Label("Lag " + num, new GUILayoutOption[0]);
		num = GUILayout.HorizontalSlider(num, 0f, 500f, new GUILayoutOption[0]);
		Peer.NetworkSimulationSettings.IncomingLag = (int)num;
		Peer.NetworkSimulationSettings.OutgoingLag = (int)num;
		float num2 = Peer.NetworkSimulationSettings.IncomingJitter;
		GUILayout.Label("Jit " + num2, new GUILayoutOption[0]);
		num2 = GUILayout.HorizontalSlider(num2, 0f, 100f, new GUILayoutOption[0]);
		Peer.NetworkSimulationSettings.IncomingJitter = (int)num2;
		Peer.NetworkSimulationSettings.OutgoingJitter = (int)num2;
		float num3 = Peer.NetworkSimulationSettings.IncomingLossPercentage;
		GUILayout.Label("Loss " + num3, new GUILayoutOption[0]);
		num3 = GUILayout.HorizontalSlider(num3, 0f, 10f, new GUILayoutOption[0]);
		Peer.NetworkSimulationSettings.IncomingLossPercentage = (int)num3;
		Peer.NetworkSimulationSettings.OutgoingLossPercentage = (int)num3;
		if (GUI.changed)
		{
			WindowRect.height = 100f;
		}
		GUI.DragWindow();
	}
}
