using UnityEngine;

public class TrolleyPlayer : MonoBehaviour
{
	private static int _TerrainLayer = LayerMask.NameToLayer("Terrain");

	public void Initialize(Trolley trolley, GameObject player)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		CapsuleCollider val = ((Component)this).gameObject.AddComponent<CapsuleCollider>();
		((Collider)val).isTrigger = true;
		((Component)this).gameObject.AddComponent<Rigidbody>();
		((Component)this).rigidbody.isKinematic = true;
		CharacterController component = player.GetComponent<CharacterController>();
		val.height = component.height;
		val.radius = component.radius;
		val.center = component.center;
		((Component)this).transform.position = ((Component)component).transform.position;
		((Component)this).transform.rotation = ((Component)component).transform.rotation;
		((Component)this).transform.localScale = ((Component)component).transform.localScale;
		((Component)this).transform.parent = ((Component)trolley.Model).transform;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (((Component)other).gameObject.layer == _TerrainLayer)
		{
			Trolley component = ((Component)((Component)this).transform.root).GetComponent<Trolley>();
			if ((Object)(object)component.Player != (Object)null)
			{
				component.PlayerExit();
			}
		}
	}
}
