using System.Collections;
using UnityEngine;

public class GA_ExampleTarget : MonoBehaviour
{
	public float SpinSpeed = 0.5f;

	private bool _hit;

	private void Start()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).renderer.material.color = Color.yellow;
	}

	private void Update()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (!_hit)
		{
			((Component)this).rigidbody.AddTorque(Vector3.forward * SpinSpeed);
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (((Object)collision.gameObject).name.Equals("Ball") && !_hit)
		{
			GA_ExampleHighScore.AddScore(10, ((Object)((Component)this).gameObject).name, collision.transform.position);
			((MonoBehaviour)this).StartCoroutine(FlashColor(0.5f));
		}
	}

	private IEnumerator FlashColor(float duration)
	{
		_hit = true;
		((Component)this).renderer.material.color = Color.green;
		yield return (object)new WaitForSeconds(duration);
		_hit = false;
		((Component)this).renderer.material.color = Color.yellow;
	}
}
