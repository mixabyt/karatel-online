using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockDoor : EntityBase
{
	public int LockID;

	public GameObject LockerModel;

	public GameObject InnerModel;

	private bool _Opened;

	public override void OnButtonF(string playerName)
	{
		if ((Object)(object)LockerModel != (Object)null && !Level.Instance.IsAdministrator() && !Level.Instance.IsModerator())
		{
			List<LockKey> list = new List<LockKey>((LockKey[])(object)Object.FindObjectsOfType(typeof(LockKey)));
			int count = list.FindAll((LockKey i) => i.LockID == LockID && (Object)(object)i.KeyModel != (Object)null).Count;
			if (count > 0)
			{
				Chat.SendInfo(Localize.GetText("LOCK_REMANING_KEYS") + ((count <= 1) ? string.Empty : (count + " ")) + Localize.GetText("PURCHASE_" + list[0].Name), forAll: false);
			}
			else
			{
				((MonoBehaviour)this).StartCoroutine(UnlockDoorProcess());
			}
		}
		else if (!_Opened)
		{
			OpenDoor();
		}
		else
		{
			CloseDoor();
		}
	}

	private IEnumerator UnlockDoorProcess()
	{
		float a = 1f;
		while (a > 0f)
		{
			a = Mathf.MoveTowards(a, 0f, Time.deltaTime);
			MaterialExt.SetTransparent(LockerModel, a);
			yield return (object)new WaitForEndOfFrame();
		}
		yield return (object)new WaitForSeconds(0.5f);
		Object.Destroy((Object)(object)LockerModel);
		LockerModel = null;
		OpenDoor();
	}

	private void OpenDoor()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		iTween.RotateTo(InnerModel, iTween.Hash("rotation", (object)new Vector3(-90f, 90f, 0f), "time", 1f, "islocal", true, "easetype", iTween.EaseType.bounce));
		_Opened = true;
		InnerModel.collider.isTrigger = true;
		SoundManager.Instance.Play(SoundManager.Sound.DoorOpen, ((Component)this).audio);
	}

	private void CloseDoor()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		iTween.RotateTo(InnerModel, iTween.Hash("rotation", (object)new Vector3(-90f, 0f, 0f), "time", 1f, "islocal", true, "easetype", iTween.EaseType.bounce));
		_Opened = false;
		InnerModel.collider.isTrigger = false;
		SoundManager.Instance.Play(SoundManager.Sound.DoorClose, ((Component)this).audio);
	}
}
