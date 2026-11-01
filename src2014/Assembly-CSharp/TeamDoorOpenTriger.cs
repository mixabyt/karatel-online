using UnityEngine;

public class TeamDoorOpenTriger : MonoBehaviour
{
	public GameObject doors;

	private void OnTriggerEnter(Collider other)
	{
		if (!PhotonNetwork.isMasterClient)
		{
			return;
		}
		PhotonView component = ((Component)other).gameObject.GetComponent<PhotonView>();
		if ((Object)(object)component == (Object)null)
		{
			return;
		}
		if ((Object)(object)TeamBattle.Instance != (Object)null)
		{
			if (App.Instance.Settings.gameType == GameINI.GameType.DEATHMATCH)
			{
				doors.GetComponent<TeamDoor>().OpenDoorR();
				return;
			}
			int playerTeam = TeamBattle.Instance.GetPlayerTeam(component.owner.name);
			if (playerTeam != 0 && playerTeam == doors.GetComponent<TeamDoor>().team)
			{
				doors.GetComponent<TeamDoor>().OpenDoorR();
			}
		}
		else
		{
			doors.GetComponent<TeamDoor>().OpenDoorR();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (PhotonNetwork.isMasterClient)
		{
			PhotonView component = ((Component)other).gameObject.GetComponent<PhotonView>();
			if (!((Object)(object)component == (Object)null))
			{
				doors.GetComponent<TeamDoor>().CloseDoorR();
			}
		}
	}
}
