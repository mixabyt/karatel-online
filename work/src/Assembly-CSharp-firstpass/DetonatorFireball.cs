using UnityEngine;

[AddComponentMenu("Detonator/Fireball")]
[RequireComponent(typeof(Detonator))]
public class DetonatorFireball : DetonatorComponent
{
	private float _baseSize = 1f;

	private float _baseDuration = 3f;

	private Color _baseColor = new Color(1f, 0.423f, 0f, 0.5f);

	private float _scaledDuration;

	private GameObject _fireballA;

	private DetonatorBurstEmitter _fireballAEmitter;

	public Material fireballAMaterial;

	private GameObject _fireballB;

	private DetonatorBurstEmitter _fireballBEmitter;

	public Material fireballBMaterial;

	private GameObject _fireShadow;

	private DetonatorBurstEmitter _fireShadowEmitter;

	public Material fireShadowMaterial;

	public bool drawFireballA = true;

	public bool drawFireballB = true;

	public bool drawFireShadow = true;

	private Color _detailAdjustedColor;

	public DetonatorFireball()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
	}

	public override void Init()
	{
		FillMaterials(wipe: false);
		BuildFireballA();
		BuildFireballB();
		BuildFireShadow();
	}

	public void FillMaterials(bool wipe)
	{
		if (!Object.op_Implicit((Object)(object)fireballAMaterial) || wipe)
		{
			fireballAMaterial = MyDetonator().fireballAMaterial;
		}
		if (!Object.op_Implicit((Object)(object)fireballBMaterial) || wipe)
		{
			fireballBMaterial = MyDetonator().fireballBMaterial;
		}
		if (!Object.op_Implicit((Object)(object)fireShadowMaterial) || wipe)
		{
			if ((double)Random.value > 0.5)
			{
				fireShadowMaterial = MyDetonator().smokeAMaterial;
			}
			else
			{
				fireShadowMaterial = MyDetonator().smokeBMaterial;
			}
		}
	}

	public void BuildFireballA()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected Obj, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		_fireballA = new GameObject("FireballA");
		_fireballAEmitter = (DetonatorBurstEmitter)(object)_fireballA.AddComponent("DetonatorBurstEmitter");
		_fireballA.transform.parent = ((Component)this).transform;
		_fireballA.transform.localRotation = Quaternion.identity;
		_fireballAEmitter.material = fireballAMaterial;
		_fireballAEmitter.useWorldSpace = MyDetonator().useWorldSpace;
		_fireballAEmitter.upwardsBias = MyDetonator().upwardsBias;
	}

	public void UpdateFireballA()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		_fireballA.transform.localPosition = Vector3.Scale(localPosition, new Vector3(size, size, size));
		_fireballAEmitter.color = color;
		_fireballAEmitter.duration = duration * 0.5f;
		_fireballAEmitter.durationVariation = duration * 0.5f;
		_fireballAEmitter.count = 2f;
		_fireballAEmitter.timeScale = timeScale;
		_fireballAEmitter.detail = detail;
		_fireballAEmitter.particleSize = 14f;
		_fireballAEmitter.sizeVariation = 3f;
		_fireballAEmitter.velocity = velocity;
		_fireballAEmitter.startRadius = 4f;
		_fireballAEmitter.size = size;
		_fireballAEmitter.useExplicitColorAnimation = true;
		Color val = new Color(1f, 1f, 1f, 0.5f);
		Color val2 = new Color(0.6f, 0.15f, 0.15f, 0.3f);
		Color val3 = new Color(0.1f, 0.2f, 0.45f, 0f);
		ref Color reference = ref _fireballAEmitter.colorAnimation[0];
		reference = Color.Lerp(color, val, 0.8f);
		ref Color reference2 = ref _fireballAEmitter.colorAnimation[1];
		reference2 = Color.Lerp(color, val, 0.5f);
		ref Color reference3 = ref _fireballAEmitter.colorAnimation[2];
		reference3 = color;
		ref Color reference4 = ref _fireballAEmitter.colorAnimation[3];
		reference4 = Color.Lerp(color, val2, 0.7f);
		_fireballAEmitter.colorAnimation[4] = val3;
		_fireballAEmitter.explodeDelayMin = explodeDelayMin;
		_fireballAEmitter.explodeDelayMax = explodeDelayMax;
	}

	public void BuildFireballB()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected Obj, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		_fireballB = new GameObject("FireballB");
		_fireballBEmitter = (DetonatorBurstEmitter)(object)_fireballB.AddComponent("DetonatorBurstEmitter");
		_fireballB.transform.parent = ((Component)this).transform;
		_fireballB.transform.localRotation = Quaternion.identity;
		_fireballBEmitter.material = fireballBMaterial;
		_fireballBEmitter.useWorldSpace = MyDetonator().useWorldSpace;
		_fireballBEmitter.upwardsBias = MyDetonator().upwardsBias;
	}

	public void UpdateFireballB()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		_fireballB.transform.localPosition = Vector3.Scale(localPosition, new Vector3(size, size, size));
		_fireballBEmitter.color = color;
		_fireballBEmitter.duration = duration * 0.5f;
		_fireballBEmitter.durationVariation = duration * 0.5f;
		_fireballBEmitter.count = 2f;
		_fireballBEmitter.timeScale = timeScale;
		_fireballBEmitter.detail = detail;
		_fireballBEmitter.particleSize = 10f;
		_fireballBEmitter.sizeVariation = 6f;
		_fireballBEmitter.velocity = velocity;
		_fireballBEmitter.startRadius = 4f;
		_fireballBEmitter.size = size;
		_fireballBEmitter.useExplicitColorAnimation = true;
		Color val = new Color(1f, 1f, 1f, 0.5f);
		Color val2 = new Color(0.6f, 0.15f, 0.15f, 0.3f);
		Color val3 = new Color(0.1f, 0.2f, 0.45f, 0f);
		ref Color reference = ref _fireballBEmitter.colorAnimation[0];
		reference = Color.Lerp(color, val, 0.8f);
		ref Color reference2 = ref _fireballBEmitter.colorAnimation[1];
		reference2 = Color.Lerp(color, val, 0.5f);
		ref Color reference3 = ref _fireballBEmitter.colorAnimation[2];
		reference3 = color;
		ref Color reference4 = ref _fireballBEmitter.colorAnimation[3];
		reference4 = Color.Lerp(color, val2, 0.7f);
		_fireballBEmitter.colorAnimation[4] = val3;
		_fireballBEmitter.explodeDelayMin = explodeDelayMin;
		_fireballBEmitter.explodeDelayMax = explodeDelayMax;
	}

	public void BuildFireShadow()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected Obj, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		_fireShadow = new GameObject("FireShadow");
		_fireShadowEmitter = (DetonatorBurstEmitter)(object)_fireShadow.AddComponent("DetonatorBurstEmitter");
		_fireShadow.transform.parent = ((Component)this).transform;
		_fireShadow.transform.localRotation = Quaternion.identity;
		_fireShadowEmitter.material = fireShadowMaterial;
		_fireShadowEmitter.useWorldSpace = MyDetonator().useWorldSpace;
		_fireShadowEmitter.upwardsBias = MyDetonator().upwardsBias;
	}

	public void UpdateFireShadow()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		_fireShadow.transform.localPosition = Vector3.Scale(localPosition, new Vector3(size, size, size));
		_fireShadow.transform.LookAt(((Component)Camera.main).transform);
		_fireShadow.transform.localPosition = -(Vector3.forward * 1f);
		_fireShadowEmitter.color = new Color(0.1f, 0.1f, 0.1f, 0.6f);
		_fireShadowEmitter.duration = duration * 0.5f;
		_fireShadowEmitter.durationVariation = duration * 0.5f;
		_fireShadowEmitter.timeScale = timeScale;
		_fireShadowEmitter.detail = 1f;
		_fireShadowEmitter.particleSize = 13f;
		_fireShadowEmitter.velocity = velocity;
		_fireShadowEmitter.sizeVariation = 1f;
		_fireShadowEmitter.count = 4f;
		_fireShadowEmitter.startRadius = 6f;
		_fireShadowEmitter.size = size;
		_fireShadowEmitter.explodeDelayMin = explodeDelayMin;
		_fireShadowEmitter.explodeDelayMax = explodeDelayMax;
	}

	public void Reset()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		FillMaterials(wipe: true);
		on = true;
		size = _baseSize;
		duration = _baseDuration;
		explodeDelayMin = 0f;
		explodeDelayMax = 0f;
		color = _baseColor;
	}

	public override void Explode()
	{
		if (!(detailThreshold > detail) && on)
		{
			UpdateFireballA();
			UpdateFireballB();
			UpdateFireShadow();
			if (drawFireballA)
			{
				_fireballAEmitter.Explode();
			}
			if (drawFireballB)
			{
				_fireballBEmitter.Explode();
			}
			if (drawFireShadow)
			{
				_fireShadowEmitter.Explode();
			}
		}
	}
}
