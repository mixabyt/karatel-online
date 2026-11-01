using UnityEngine;

public class Soccer : MonoBehaviour
{
	public Transform plChar;

	public int threshold;

	public int power;

	private void Start()
	{
	}

	private void Update()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ((Component)this).transform.position - ((Component)plChar).transform.position;
		if (val.magnitude < (float)threshold)
		{
			Rigidbody rigidbody = ((Component)this).rigidbody;
			Vector3 val2 = ((Component)this).transform.position - ((Component)plChar).transform.position;
			rigidbody.AddForce(val2.normalized * (float)power);
		}
	}
}
