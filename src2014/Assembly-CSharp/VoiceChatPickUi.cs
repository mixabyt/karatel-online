using UnityEngine;

public class VoiceChatPickUi : MonoBehaviour
{
	private void OnGUI()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected Obj, but got Unknown
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		GUILayout.Window(0, new Rect((float)(Screen.width / 2 - 50), (float)(Screen.height / 2 - 50), 100f, 100f), (WindowFunction)Window, string.Empty, GUIStyle.none, new GUILayoutOption[0]);
	}

	private void Window(int id)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		GUI.Box(new Rect(0f, 0f, 100f, 100f), string.Empty);
		if (GUILayout.Button("Start Server", new GUILayoutOption[0]))
		{
			((Behaviour)((Component)this).gameObject.GetComponent<VoiceChatUnityServer>()).enabled = true;
			((Component)this).gameObject.AddComponent<VoiceChatServerUi>();
			Object.Destroy((Object)(object)this);
		}
		if (GUILayout.Button("Start Client", new GUILayoutOption[0]))
		{
			((Behaviour)((Component)this).gameObject.GetComponent<VoiceChatUnityClient>()).enabled = true;
			((Component)this).gameObject.AddComponent<VoiceChatUi>();
			Object.Destroy((Object)(object)this);
		}
	}
}
