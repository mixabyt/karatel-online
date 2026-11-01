using UnityEngine;

[RequireComponent(typeof(Detonator))]
public class DetonatorCloudRing : DetonatorComponent
{
	private float _baseSize = 1f;

	private float _baseDuration = 5f;

	private Vector3 _baseVelocity = new Vector3(155f, 5f, 155f);

	private Color _baseColor = Color.white;

	private Vector3 _baseForce = new Vector3(0.162f, 2.56f, 0f);

	private GameObject _cloudRing;

	private DetonatorBurstEmitter _cloudRingEmitter;

	public Material cloudRingMaterial;

	public DetonatorCloudRing()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
	}

	public override void Init()
	{
		FillMaterials(wipe: false);
		BuildCloudRing();
	}

	public void FillMaterials(bool wipe)
	{
		if (!Object.op_Implicit((Object)(object)cloudRingMaterial) || wipe)
		{
			cloudRingMaterial = MyDetonator().smokeBMaterial;
		}
	}

	public void BuildCloudRing()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected Obj, but got Unknown
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		_cloudRing = new GameObject("CloudRing");
		_cloudRingEmitter = (DetonatorBurstEmitter)(object)_cloudRing.AddComponent("DetonatorBurstEmitter");
		_cloudRing.transform.parent = ((Component)this).transform;
		_cloudRing.transform.localPosition = localPosition;
		_cloudRingEmitter.material = cloudRingMaterial;
		_cloudRingEmitter.useExplicitColorAnimation = true;
	}

	public void UpdateCloudRing()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		_cloudRing.transform.localPosition = Vector3.Scale(localPosition, new Vector3(size, size, size));
		_cloudRingEmitter.color = color;
		_cloudRingEmitter.duration = duration;
		_cloudRingEmitter.durationVariation = duration / 4f;
		_cloudRingEmitter.count = (int)(detail * 50f);
		_cloudRingEmitter.particleSize = 10f;
		_cloudRingEmitter.sizeVariation = 2f;
		_cloudRingEmitter.velocity = velocity;
		_cloudRingEmitter.startRadius = 3f;
		_cloudRingEmitter.size = size;
		_cloudRingEmitter.force = force;
		_cloudRingEmitter.explodeDelayMin = explodeDelayMin;
		_cloudRingEmitter.explodeDelayMax = explodeDelayMax;
		Color val = Color.Lerp(color, new Color(0.2f, 0.2f, 0.2f, 0.6f), 0.5f);
		Color val2 = new Color(0.2f, 0.2f, 0.2f, 0.5f);
		Color val3 = new Color(0.2f, 0.2f, 0.2f, 0.3f);
		Color val4 = new Color(0.2f, 0.2f, 0.2f, 0f);
		_cloudRingEmitter.colorAnimation[0] = val;
		_cloudRingEmitter.colorAnimation[1] = val2;
		_cloudRingEmitter.colorAnimation[2] = val2;
		_cloudRingEmitter.colorAnimation[3] = val3;
		_cloudRingEmitter.colorAnimation[4] = val4;
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
			UpdateCloudRing();
			_cloudRingEmitter.Explode();
		}
	}
}
