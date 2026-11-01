using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class PhotonNetworkScript : MonoBehaviour
{
	private void Start()
	{
		Debug.Log((object)"START ");
	}

	private void Update()
	{
		Debug.Log((object)"UPDATE ");
	}
}
