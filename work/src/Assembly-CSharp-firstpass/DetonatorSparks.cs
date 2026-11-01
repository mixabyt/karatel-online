using UnityEngine;

[RequireComponent(typeof(Detonator))]
[AddComponentMenu("Detonator/Sparks")]
public class DetonatorSparks : DetonatorComponent
{
	private float _baseSize = 1f;

	private float _baseDuration = 4f;

	private Vector3 _baseVelocity = new Vector3(155f, 155f, 155f);

	private Color _baseColor = Color.white;

	private Vector3 _baseForce = Physics.gravity;

	private float _scaledDuration;

	private GameObject _sparks;

	private DetonatorBurstEmitter _sparksEmitter;

	public Material sparksMaterial;

	public DetonatorSparks()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
	}

	public override void Init()
	{
		FillMaterials(wipe: false);
		BuildSparks();
	}

	public void FillMaterials(bool wipe)
	{
		if (!Object.op_Implicit((Object)(object)sparksMaterial) || wipe)
		{
			sparksMaterial = MyDetonator().sparksMaterial;
		}
	}

	public void BuildSparks()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected Obj, but got Unknown
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		_sparks = new GameObject("Sparks");
		_sparksEmitter = (DetonatorBurstEmitter)(object)_sparks.AddComponent("DetonatorBurstEmitter");
		_sparks.transform.parent = ((Component)this).transform;
		_sparks.transform.localPosition = localPosition;
		_sparks.transform.localRotation = Quaternion.identity;
		_sparksEmitter.material = sparksMaterial;
		_sparksEmitter.force = Physics.gravity / 3f;
		_sparksEmitter.useExplicitColorAnimation = false;
		_sparksEmitter.useWorldSpace = MyDetonator().useWorldSpace;
		_sparksEmitter.upwardsBias = MyDetonator().upwardsBias;
	}

	public void UpdateSparks()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		_scaledDuration = duration * timeScale;
		_sparksEmitter.color = color;
		_sparksEmitter.duration = _scaledDuration / 4f;
		_sparksEmitter.durationVariation = _scaledDuration;
		_sparksEmitter.count = (int)(detail * 50f);
		_sparksEmitter.particleSize = 0.5f;
		_sparksEmitter.sizeVariation = 0.25f;
		if (_sparksEmitter.upwardsBias > 0f)
		{
			_sparksEmitter.velocity = new Vector3(velocity.x / Mathf.Log(_sparksEmitter.upwardsBias), velocity.y * Mathf.Log(_sparksEmitter.upwardsBias), velocity.z / Mathf.Log(_sparksEmitter.upwardsBias));
		}
		else
		{
			_sparksEmitter.velocity = velocity;
		}
		_sparksEmitter.startRadius = 0f;
		_sparksEmitter.size = size;
		_sparksEmitter.explodeDelayMin = explodeDelayMin;
		_sparksEmitter.explodeDelayMax = explodeDelayMax;
	}

	public void Reset()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		FillMaterials(wipe: true);
		on = true;
		size = _baseSize;
		duration = _baseDuration;
		explodeDelayMin = 0f;
		explodeDelayMax = 0f;
		color = _baseColor;
		velocity = _baseVelocity;
		force = _baseForce;
	}

	public override void Explode()
	{
		if (on)
		{
			UpdateSparks();
			_sparksEmitter.Explode();
		}
	}
}
