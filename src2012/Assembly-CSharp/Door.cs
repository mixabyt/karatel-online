using UnityEngine;

public class Door : EntityBase
{
	public GameObject InnerModel;

	private bool _Opened;

	public override void OnButtonF(string player_name)
	{
		if (!_Opened)
		{
			photonView.RPC("OpenDoor", PhotonTargets.All);
		}
		else
		{
			photonView.RPC("CloseDoor", PhotonTargets.All);
		}
	}

	[RPC]
	public void OpenDoor()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		iTween.RotateTo(InnerModel, iTween.Hash("rotation", (object)new Vector3(0f, 90f, 0f), "time", 1f, "islocal", true, "easetype", iTween.EaseType.bounce));
		_Opened = true;
		InnerModel.collider.isTrigger = true;
		ResourceLoader.I.PlayEffect(ResourceLoader.eSounds.DoorOpen_1, ((Component)this).audio);
	}

	[RPC]
	public void CloseDoor()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		iTween.RotateTo(InnerModel, iTween.Hash("rotation", (object)new Vector3(0f, 0f, 0f), "time", 1f, "islocal", true, "easetype", iTween.EaseType.bounce));
		_Opened = false;
		InnerModel.collider.isTrigger = false;
		ResourceLoader.I.PlayEffect(ResourceLoader.eSounds.DoorClose_1, ((Component)this).audio);
	}
}
