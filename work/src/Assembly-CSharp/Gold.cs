using UnityEngine;

public class Gold : EntityBase
{
	public override void OnButtonE(string player_name)
	{
		((MonoBehaviour)WorldGameObject.Instance).StartCoroutine(WorldGameObject.Instance.AddMoney());
		SelfDelete();
	}

	public override void OnLeftMouseHit(string player_name)
	{
	}
}
