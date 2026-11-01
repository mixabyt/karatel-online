using UnityEngine;

[AddComponentMenu("Game/UI/Button Key Binding")]
public class UIButtonKeyBinding : MonoBehaviour
{
	public KeyCode keyCode;

	private void Update()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (!UICamera.inputHasFocus && (int)keyCode != 0)
		{
			if (Input.GetKeyDown(keyCode))
			{
				((Component)this).SendMessage("OnPress", (object)true, (SendMessageOptions)1);
			}
			if (Input.GetKeyUp(keyCode))
			{
				((Component)this).SendMessage("OnPress", (object)false, (SendMessageOptions)1);
				((Component)this).SendMessage("OnClick", (SendMessageOptions)1);
			}
		}
	}
}
