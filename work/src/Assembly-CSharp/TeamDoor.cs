using UnityEngine;

public class TeamDoor : EntityBase
{
	public GameObject LeftDoor;

	public GameObject RightDoor;

	public GameObject GearModel1;

	public GameObject GearModel2;

	public GameObject trigger;

	public int team;

	private bool _Opened;

	public void OpenDoorR()
	{
		photonView.RPC("OpenDoor", PhotonTargets.All);
	}

	public void CloseDoorR()
	{
		photonView.RPC("CloseDoor", PhotonTargets.All);
	}

	public override void OnLeftMouseHit(string playerName)
	{
		if ((Object)(object)TeamBattle.Instance == (Object)null)
		{
			base.OnLeftMouseHit(playerName);
		}
	}

	[RPC]
	public void OpenDoor()
	{
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		iTween.RotateAdd(GearModel1, iTween.Hash("x", 360, "time", 0.25f, "islocal", true, "easetype", iTween.EaseType.easeInQuad));
		iTween.RotateAdd(GearModel2, iTween.Hash("x", 360, "time", 0.25f, "islocal", true, "easetype", iTween.EaseType.easeInQuad));
		iTween.MoveTo(LeftDoor, iTween.Hash("position", (object)new Vector3(0f, 0f, 1f), "time", 0.25f, "islocal", true, "easetype", iTween.EaseType.easeInQuad));
		iTween.MoveTo(RightDoor, iTween.Hash("position", (object)new Vector3(0f, 0f, -1f), "time", 0.25f, "islocal", true, "easetype", iTween.EaseType.easeInQuad));
		_Opened = true;
		SoundManager.Instance.Play(SoundManager.Sound.TeamDoorOpen);
	}

	[RPC]
	public void CloseDoor()
	{
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		if (_Opened)
		{
			iTween.RotateAdd(GearModel1, iTween.Hash("x", -360, "time", 0.25f, "islocal", true, "easetype", iTween.EaseType.easeInQuad));
			iTween.RotateAdd(GearModel2, iTween.Hash("x", -360, "time", 0.25f, "islocal", true, "easetype", iTween.EaseType.easeInQuad));
			if (team == 1)
			{
				iTween.MoveTo(LeftDoor, iTween.Hash("position", (object)new Vector3(0f, 0f, 0.0329404f), "time", 0.25f, "islocal", true, "easetype", iTween.EaseType.easeInQuad));
				iTween.MoveTo(RightDoor, iTween.Hash("position", (object)new Vector3(0f, 0f, 0.0329404f), "time", 0.25f, "islocal", true, "easetype", iTween.EaseType.easeInQuad));
			}
			if (team == 2)
			{
				iTween.MoveTo(LeftDoor, iTween.Hash("position", (object)new Vector3(0f, 0f, 0.1363504f), "time", 0.25f, "islocal", true, "easetype", iTween.EaseType.easeInQuad));
				iTween.MoveTo(RightDoor, iTween.Hash("position", (object)new Vector3(0f, 0f, 0.1363504f), "time", 0.25f, "islocal", true, "easetype", iTween.EaseType.easeInQuad));
			}
			_Opened = false;
			SoundManager.Instance.Play(SoundManager.Sound.TeamDoorClose);
		}
	}
}
