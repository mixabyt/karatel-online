using UnityEngine;

[AddComponentMenu("Detonator/Smoke")]
[RequireComponent(typeof(Detonator))]
public class DetonatorSmoke : DetonatorComponent
{
	private const float _baseSize = 1f;

	private const float _baseDuration = 8f;

	private const float _baseDamping = 0.1300004f;

	private Color _baseColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

	private float _scaledDuration;

	private GameObject _smokeA;

	private DetonatorBurstEmitter _smokeAEmitter;

	public Material smokeAMaterial;

	private GameObject _smokeB;

	private DetonatorBurstEmitter _smokeBEmitter;

	public Material smokeBMaterial;

	public bool drawSmokeA = true;

	public bool drawSmokeB = true;

	public DetonatorSmoke()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
	}

	public override void Init()
	{
		FillMaterials(wipe: false);
		BuildSmokeA();
		BuildSmokeB();
	}

	public void FillMaterials(bool wipe)
	{
		if (!Object.op_Implicit((Object)(object)smokeAMaterial) || wipe)
		{
			smokeAMaterial = MyDetonator().smokeAMaterial;
		}
		if (!Object.op_Implicit((Object)(object)smokeBMaterial) || wipe)
		{
			smokeBMaterial = MyDetonator().smokeBMaterial;
		}
	}

	public void BuildSmokeA()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected Obj, but got Unknown
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		_smokeA = new GameObject("SmokeA");
		_smokeAEmitter = (DetonatorBurstEmitter)(object)_smokeA.AddComponent("DetonatorBurstEmitter");
		_smokeA.transform.parent = ((Component)this).transform;
		_smokeA.transform.localPosition = localPosition;
		_smokeA.transform.localRotation = Quaternion.identity;
		_smokeAEmitter.material = smokeAMaterial;
		_smokeAEmitter.exponentialGrowth = false;
		_smokeAEmitter.sizeGrow = 0.095f;
		_smokeAEmitter.useWorldSpace = MyDetonator().useWorldSpace;
		_smokeAEmitter.upwardsBias = MyDetonator().upwardsBias;
	}

	public void UpdateSmokeA()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		_smokeA.transform.localPosition = Vector3.Scale(localPosition, new Vector3(size, size, size));
		_smokeA.transform.LookAt(((Component)Camera.main).transform);
		_smokeA.transform.localPosition = -(Vector3.forward * -1.5f);
		_smokeAEmitter.color = color;
		_smokeAEmitter.duration = duration * 0.5f;
		_smokeAEmitter.durationVariation = 0f;
		_smokeAEmitter.timeScale = timeScale;
		_smokeAEmitter.count = 4f;
		_smokeAEmitter.particleSize = 25f;
		_smokeAEmitter.sizeVariation = 3f;
		_smokeAEmitter.velocity = velocity;
		_smokeAEmitter.startRadius = 10f;
		_smokeAEmitter.size = size;
		_smokeAEmitter.useExplicitColorAnimation = true;
		_smokeAEmitter.explodeDelayMin = explodeDelayMin;
		_smokeAEmitter.explodeDelayMax = explodeDelayMax;
		Color val = new Color(0.2f, 0.2f, 0.2f, 0.4f);
		Color val2 = new Color(0.2f, 0.2f, 0.2f, 0.7f);
		Color val3 = new Color(0.2f, 0.2f, 0.2f, 0.4f);
		Color val4 = new Color(0.2f, 0.2f, 0.2f, 0f);
		_smokeAEmitter.colorAnimation[0] = val;
		_smokeAEmitter.colorAnimation[1] = val2;
		_smokeAEmitter.colorAnimation[2] = val2;
		_smokeAEmitter.colorAnimation[3] = val3;
		_smokeAEmitter.colorAnimation[4] = val4;
	}

	public void BuildSmokeB()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected Obj, but got Unknown
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		_smokeB = new GameObject("SmokeB");
		_smokeBEmitter = (DetonatorBurstEmitter)(object)_smokeB.AddComponent("DetonatorBurstEmitter");
		_smokeB.transform.parent = ((Component)this).transform;
		_smokeB.transform.localPosition = localPosition;
		_smokeB.transform.localRotation = Quaternion.identity;
		_smokeBEmitter.material = smokeBMaterial;
		_smokeBEmitter.exponentialGrowth = false;
		_smokeBEmitter.sizeGrow = 0.095f;
		_smokeBEmitter.useWorldSpace = MyDetonator().useWorldSpace;
		_smokeBEmitter.upwardsBias = MyDetonator().upwardsBias;
	}

	public void UpdateSmokeB()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		_smokeB.transform.localPosition = Vector3.Scale(localPosition, new Vector3(size, size, size));
		_smokeB.transform.LookAt(((Component)Camera.main).transform);
		_smokeB.transform.localPosition = -(Vector3.forward * -1f);
		_smokeBEmitter.color = color;
		_smokeBEmitter.duration = duration * 0.5f;
		_smokeBEmitter.durationVariation = 0f;
		_smokeBEmitter.count = 2f;
		_smokeBEmitter.particleSize = 25f;
		_smokeBEmitter.sizeVariation = 3f;
		_smokeBEmitter.velocity = velocity;
		_smokeBEmitter.startRadius = 10f;
		_smokeBEmitter.size = size;
		_smokeBEmitter.useExplicitColorAnimation = true;
		_smokeBEmitter.explodeDelayMin = explodeDelayMin;
		_smokeBEmitter.explodeDelayMax = explodeDelayMax;
		Color val = new Color(0.2f, 0.2f, 0.2f, 0.4f);
		Color val2 = new Color(0.2f, 0.2f, 0.2f, 0.7f);
		Color val3 = new Color(0.2f, 0.2f, 0.2f, 0.4f);
		Color val4 = new Color(0.2f, 0.2f, 0.2f, 0f);
		_smokeBEmitter.colorAnimation[0] = val;
		_smokeBEmitter.colorAnimation[1] = val2;
		_smokeBEmitter.colorAnimation[2] = val2;
		_smokeBEmitter.colorAnimation[3] = val3;
		_smokeBEmitter.colorAnimation[4] = val4;
	}

	public void Reset()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		FillMaterials(wipe: true);
		on = true;
		size = 1f;
		duration = 8f;
		explodeDelayMin = 0f;
		explodeDelayMax = 0f;
		color = _baseColor;
		velocity = new Vector3(3f, 3f, 3f);
	}

	public override void Explode()
	{
		if (!(detailThreshold > detail) && on)
		{
			UpdateSmokeA();
			UpdateSmokeB();
			if (drawSmokeA)
			{
				_smokeAEmitter.Explode();
			}
			if (drawSmokeB)
			{
				_smokeBEmitter.Explode();
			}
		}
	}
}
