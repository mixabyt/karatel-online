using UnityEngine;

public abstract class DetonatorComponent : MonoBehaviour
{
	public bool on = true;

	public bool detonatorControlled = true;

	[HideInInspector]
	public float startSize = 1f;

	public float size = 1f;

	public float explodeDelayMin;

	public float explodeDelayMax;

	[HideInInspector]
	public float startDuration = 2f;

	public float duration = 2f;

	[HideInInspector]
	public float timeScale = 1f;

	[HideInInspector]
	public float startDetail = 1f;

	public float detail = 1f;

	[HideInInspector]
	public Color startColor = Color.white;

	public Color color = Color.white;

	[HideInInspector]
	public Vector3 startLocalPosition = Vector3.zero;

	public Vector3 localPosition = Vector3.zero;

	[HideInInspector]
	public Vector3 startForce = Vector3.zero;

	public Vector3 force = Vector3.zero;

	[HideInInspector]
	public Vector3 startVelocity = Vector3.zero;

	public Vector3 velocity = Vector3.zero;

	public float detailThreshold;

	protected DetonatorComponent()
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
	}

	public abstract void Explode();

	public abstract void Init();

	public void SetStartValues()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		startSize = size;
		startForce = force;
		startVelocity = velocity;
		startDuration = duration;
		startDetail = detail;
		startColor = color;
		startLocalPosition = localPosition;
	}

	public Detonator MyDetonator()
	{
		return ((Component)this).GetComponent("Detonator") as Detonator;
	}
}
