using UnityEngine;

public class Ladder : EntityBase
{
	private bool players_stay;

	public void OnTriggerExit(Collider other)
	{
		if ((Object)(object)((Component)other).gameObject == (Object)(object)WorldGameObject.INSTANCE.MainPlayer)
		{
			WorldGameObject.INSTANCE.MainPlayer.SendMessage("SetLadderBody", (object)false);
			players_stay = false;
			WorldGameObject.INSTANCE.player_on_ladder = false;
			MainMenu.INSTANCE.FlyBonus();
		}
	}

	protected override void Destruction()
	{
		if (players_stay)
		{
			WorldGameObject.INSTANCE.MainPlayer.SendMessage("SetLadderBody", (object)false);
			WorldGameObject.INSTANCE.player_on_ladder = false;
			MainMenu.INSTANCE.FlyBonus();
		}
	}

	public void OnTriggerStay(Collider other)
	{
		if ((Object)(object)((Component)other).gameObject == (Object)(object)WorldGameObject.INSTANCE.MainPlayer)
		{
			WorldGameObject.INSTANCE.MainPlayer.SendMessage("SetLadderBody", (object)true);
			players_stay = true;
			WorldGameObject.INSTANCE.player_on_ladder = true;
			WorldGameObject.INSTANCE.last_ladder_delta = 0f;
		}
	}
}
