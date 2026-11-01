using UnityEngine;

public class Gold : EntityBase
{
	public override void OnButtonE(string player_name)
	{
		((MonoBehaviour)WorldGameObject.INSTANCE).StartCoroutine(WorldGameObject.INSTANCE.AddMoney());
		SelfDelete();
	}

	public override void OnLeftMouseHit(string player_name)
	{
	}
}
