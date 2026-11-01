using UnityEngine;

public class GA_ExampleMoveWithPlayer : MonoBehaviour
{
	public GameObject Player;

	private void Update()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)Player != (Object)null)
		{
			((Component)this).transform.position = Player.transform.position + new Vector3(0f, 0.5f, 0f);
		}
	}
}
