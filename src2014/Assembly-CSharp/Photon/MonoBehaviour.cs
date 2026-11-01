using UnityEngine;

namespace Photon;

public class MonoBehaviour : MonoBehaviour
{
	public PhotonView photonView => PhotonView.Get((Component)(object)this);

	public PhotonView networkView
	{
		get
		{
			Debug.LogWarning((object)"Why are you still using networkView? should be PhotonView?");
			return PhotonView.Get((Component)(object)this);
		}
	}
}
