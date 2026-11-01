using UnityEngine;

public class MobsPreviewObj : MonoBehaviour
{
	private void Start()
	{
		MainMenu.INSTANCE.mobs_obj = ((Component)this).gameObject;
	}
}
