using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class ManualPhotonViewAllocator : MonoBehaviour
{
	public GameObject Prefab;

	public void AllocateManualPhotonView()
	{
		PhotonView photonView = ((Component)this).gameObject.GetPhotonView();
		if ((Object)(object)photonView == (Object)null)
		{
			Debug.LogError((object)"Can't do manual instantiation without PhotonView component.");
			return;
		}
		int num = PhotonNetwork.AllocateViewID();
		photonView.RPC("InstantiateRpc", PhotonTargets.AllBuffered, num);
	}

	[RPC]
	public void InstantiateRpc(int viewID)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		Object val = Object.Instantiate((Object)(object)Prefab, InputToEvent.inputHitPos + new Vector3(0f, 5f, 0f), Quaternion.identity);
		GameObject val2 = (GameObject)(object)((val is GameObject) ? val : null);
		val2.GetPhotonView().viewID = viewID;
		OnClickDestroy component = val2.GetComponent<OnClickDestroy>();
		component.DestroyByRpc = true;
	}
}
