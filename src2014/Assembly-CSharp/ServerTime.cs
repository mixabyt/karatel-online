using System;
using UnityEngine;

public class ServerTime : MonoBehaviour
{
	private void OnGUI()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		GUILayout.BeginArea(new Rect((float)(Screen.width / 2 - 100), 0f, 200f, 30f));
		GUILayout.Label($"Time Offset: {PhotonNetwork.networkingPeer.ServerTimeInMilliSeconds - Environment.TickCount}", new GUILayoutOption[0]);
		if (GUILayout.Button("fetch", new GUILayoutOption[0]))
		{
			PhotonNetwork.FetchServerTimestamp();
		}
		GUILayout.EndArea();
	}
}
