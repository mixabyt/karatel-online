using System;
using UnityEngine;

[Serializable]
public class ExplosionAtPoint : MonoBehaviour
{
	public Transform explosionPrefab;

	public float rate;

	public float nextCopy;

	public override void Update()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		if (Input.GetKeyDown("mouse 0"))
		{
			nextCopy = Time.time + rate;
			Ray val = ((Component)this).camera.ScreenPointToRay(Input.mousePosition);
			RaycastHit val2 = default;
			if (Physics.Raycast(val, ref val2))
			{
				Quaternion val3 = Quaternion.FromToRotation(Vector3.up, val2.normal);
				Object.Instantiate((Object)(object)explosionPrefab, val2.point, Quaternion.identity);
			}
		}
	}

	public override void Main()
	{
	}
}
