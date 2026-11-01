using UnityEngine;

public class MenuPreviewObjInit : MonoBehaviour
{
	private void Start()
	{
		MainMenu.INSTANCE.preview_obj = ((Component)this).gameObject;
	}
}
