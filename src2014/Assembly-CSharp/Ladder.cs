using UnityEngine;

public class Ladder : EntityBase
{
	private bool players_stay;

	public void OnTriggerExit(Collider other)
	{
		if ((Object)(object)((Component)other).gameObject == (Object)(object)WorldGameObject.Instance.MainPlayer)
		{
			WorldGameObject.Instance.MainPlayer.SendMessage("SetLadderBody", (object)false);
			players_stay = false;
			WorldGameObject.Instance.player_on_ladder = false;
			MainMenu.Instance.RefreshFlying();
		}
	}

	protected override void Destruction()
	{
		if (players_stay)
		{
			WorldGameObject.Instance.MainPlayer.SendMessage("SetLadderBody", (object)false);
			WorldGameObject.Instance.player_on_ladder = false;
			MainMenu.Instance.RefreshFlying();
		}
	}

	public void OnTriggerStay(Collider other)
	{
		if ((Object)(object)((Component)other).gameObject == (Object)(object)WorldGameObject.Instance.MainPlayer)
		{
			WorldGameObject.Instance.MainPlayer.SendMessage("SetLadderBody", (object)true);
			players_stay = true;
			WorldGameObject.Instance.player_on_ladder = true;
			WorldGameObject.Instance.last_ladder_delta = 0f;
		}
	}
}
