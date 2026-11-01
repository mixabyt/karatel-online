using UnityEngine;

public class BillboardScript : MonoBehaviour
{
	private void Update()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ((Component)Camera.mainCamera).transform.position - ((Component)this).transform.position;
		val.x = (val.z = 0f);
		((Component)this).transform.LookAt(((Component)Camera.mainCamera).transform.position - val);
	}
}
