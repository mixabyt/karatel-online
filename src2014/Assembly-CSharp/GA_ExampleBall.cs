using UnityEngine;

public class GA_ExampleBall : MonoBehaviour
{
	public float Speed = 1f;

	private void Start()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).rigidbody.velocity = new Vector3(Random.value * 0.2f - 0.1f, -1f, 0f) * Speed;
	}

	private void Update()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).rigidbody.AddForce(Vector3.down * 0.0001f);
		Rigidbody rigidbody = ((Component)this).rigidbody;
		Vector3 velocity = ((Component)this).rigidbody.velocity;
		rigidbody.velocity = velocity.normalized * Speed;
	}
}
