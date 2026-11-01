using Photon;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class MoveByKeys : MonoBehaviour
{
	public float speed = 10f;

	private void Update()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		if (Input.GetKey((KeyCode)97))
		{
			Transform transform = ((Component)this).transform;
			transform.position += Vector3.left * (speed * Time.deltaTime);
		}
		if (Input.GetKey((KeyCode)100))
		{
			Transform transform2 = ((Component)this).transform;
			transform2.position += Vector3.right * (speed * Time.deltaTime);
		}
		if (Input.GetKey((KeyCode)119))
		{
			Transform transform3 = ((Component)this).transform;
			transform3.position += Vector3.forward * (speed * Time.deltaTime);
		}
		if (Input.GetKey((KeyCode)115))
		{
			Transform transform4 = ((Component)this).transform;
			transform4.position += Vector3.back * (speed * Time.deltaTime);
		}
	}

	private void Start()
	{
		((Behaviour)this).enabled = photonView.isMine;
	}
}
