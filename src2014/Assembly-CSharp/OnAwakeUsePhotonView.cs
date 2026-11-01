using Photon;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class OnAwakeUsePhotonView : MonoBehaviour
{
	private void Awake()
	{
		if (photonView.isMine)
		{
			photonView.RPC("OnAwakeRPC", PhotonTargets.All);
		}
	}

	private void Start()
	{
		if (photonView.isMine)
		{
			photonView.RPC("OnAwakeRPC", PhotonTargets.All, (byte)1);
		}
	}

	[RPC]
	public void OnAwakeRPC()
	{
		Debug.Log((object)("RPC: 'OnAwakeRPC' PhotonView: " + photonView));
	}

	[RPC]
	public void OnAwakeRPC(byte myParameter)
	{
		Debug.Log((object)("RPC: 'OnAwakeRPC' Parameter: " + myParameter + " PhotonView: " + photonView));
	}
}
