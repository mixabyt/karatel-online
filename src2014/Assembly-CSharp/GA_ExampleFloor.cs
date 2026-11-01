using UnityEngine;

public class GA_ExampleFloor : MonoBehaviour
{
	private void Start()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).renderer.material.color = Color.red;
	}

	private void OnCollisionEnter(Collision collision)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (((Object)collision.gameObject).name.Equals("Ball"))
		{
			GA_ExampleHighScore.GameOver(collision.transform.position);
			Object.Destroy((Object)(object)collision.gameObject);
		}
	}
}
