using UnityEngine;

public class VoiceChatUi : MonoBehaviour
{
	private void Start()
	{
		Application.RequestUserAuthorization((UserAuthorization)2);
	}

	private void OnGUI()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected Obj, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		GUILayout.Window(1, new Rect(10f, 10f, (float)(Screen.width - 20), (float)(Screen.height - 20)), (WindowFunction)Window, string.Empty, GUIStyle.none, new GUILayoutOption[0]);
	}

	private void Window(int id)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		GUI.Box(new Rect(0f, 0f, (float)(Screen.width - 20), (float)(Screen.height - 20)), string.Empty);
		if (VoiceChatRecorder.Instance.IsRecording)
		{
			GUILayout.Label(VoiceChatRecorder.Instance.Device, new GUILayoutOption[0]);
			if (GUILayout.Button("Stop Recording", new GUILayoutOption[0]))
			{
				VoiceChatRecorder.Instance.StopRecording();
			}
		}
		else
		{
			GUILayout.Label("Select microphone to start recording", new GUILayoutOption[0]);
			string[] availableDevices = VoiceChatRecorder.Instance.AvailableDevices;
			foreach (string text in availableDevices)
			{
				if (GUILayout.Button(text, new GUILayoutOption[0]))
				{
					VoiceChatRecorder.Instance.Device = text;
					VoiceChatRecorder.Instance.StartRecording();
				}
			}
		}
		if (VoiceChatRecorder.Instance.Device != null)
		{
			GUILayout.Label("Push-to-talk key: " + VoiceChatRecorder.Instance.PushToTalkKey, new GUILayoutOption[0]);
			GUILayout.Label("Toggle-to-talk key: " + VoiceChatRecorder.Instance.ToggleToTalkKey, new GUILayoutOption[0]);
			GUILayout.Label("Auto detect speech: " + ((!VoiceChatRecorder.Instance.AutoDetectSpeech) ? "Off" : "On"), new GUILayoutOption[0]);
			if (GUILayout.Button("Toggle Auto Detect", new GUILayoutOption[0]))
			{
				VoiceChatRecorder.Instance.AutoDetectSpeech = !VoiceChatRecorder.Instance.AutoDetectSpeech;
			}
			GUI.color = ((!VoiceChatRecorder.Instance.IsTransmitting) ? Color.red : Color.green);
			GUILayout.Label((!VoiceChatRecorder.Instance.IsTransmitting) ? "Silent" : "Transmitting", new GUILayoutOption[0]);
		}
	}
}
