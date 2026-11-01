using Photon;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class OnClickDestroy : MonoBehaviour
{
	public bool DestroyByRpc;

	private void OnClick()
	{
		if (!DestroyByRpc)
		{
			PhotonNetwork.Destroy(((Component)this).gameObject);
		}
		else
		{
			photonView.RPC("DestroyRpc", PhotonTargets.AllBuffered);
		}
	}

	[RPC]
	public void DestroyRpc()
	{
		Object.Destroy((Object)(object)((Component)this).gameObject);
		PhotonNetwork.UnAllocateViewID(photonView.viewID);
	}
}
