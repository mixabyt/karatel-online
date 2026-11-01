using UnityEngine;

public class VoiceChatServerUi : MonoBehaviour
{
	private void OnGUI()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		int num = Screen.width / 2;
		int num2 = Screen.height / 2;
		GUI.Label(new Rect((float)(num - 50), (float)(num2 - 10), 100f, 20f), "Server Running");
	}
}
