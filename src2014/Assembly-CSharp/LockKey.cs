using UnityEngine;

public class LockKey : EntityBase
{
	public int LockID;

	public GameObject KeyModel;

	private void OnTriggerEnter(Collider other)
	{
		if (!IsPreview && WorldGameObject.Instance.IsWorldGenerated && !((Object)(object)((Component)other).gameObject != (Object)(object)WorldGameObject.Instance.MainPlayer) && !Level.Instance.IsAdministrator() && !Level.Instance.IsModerator())
		{
			TakeKey();
		}
	}

	public new virtual void OnButtonE(string playerName)
	{
		if (Creator == playerName || Level.Instance.IsAdministrator() || Level.Instance.IsModerator())
		{
			WorldGameObject.Instance.TakeEnity(((Component)this).gameObject);
		}
		else
		{
			TakeKey();
		}
	}

	private void TakeKey()
	{
		Object.Destroy((Object)(object)KeyModel);
		Object.Destroy((Object)(object)((Component)this).collider);
		Chat.SendInfo(PhotonNetwork.playerName + Localize.GetText("LOCK_FIND_KEY") + Localize.GetText("PURCHASE_" + Name), forAll: true);
	}
}
