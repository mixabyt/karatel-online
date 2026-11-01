using UnityEngine;

public class GA_ExampleController : MonoBehaviour
{
	public float Speed = 1f;

	public float MaxVelocityChange = 10f;

	private void Update()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = new Vector3(Input.GetAxis("Horizontal"), 0f, 0f);
		val *= Speed;
		Vector3 velocity = ((Component)this).rigidbody.velocity;
		Vector3 val2 = val - velocity;
		val2.x = Mathf.Clamp(val2.x, 0f - MaxVelocityChange, MaxVelocityChange);
		val2.y = 0f;
		val2.z = 0f;
		((Component)this).rigidbody.AddForce(val2, (ForceMode)2);
	}
}
