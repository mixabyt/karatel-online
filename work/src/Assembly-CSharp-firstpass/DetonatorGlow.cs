using UnityEngine;

[AddComponentMenu("Detonator/Glow")]
[RequireComponent(typeof(Detonator))]
public class DetonatorGlow : DetonatorComponent
{
	private float _baseSize = 1f;

	private float _baseDuration = 3f;

	private Vector3 _baseVelocity = new Vector3(0f, 0f, 0f);

	private Color _baseColor = Color.black;

	private float _scaledDuration;

	private GameObject _glow;

	private DetonatorBurstEmitter _glowEmitter;

	public Material glowMaterial;

	public DetonatorGlow()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
	}

	public override void Init()
	{
		FillMaterials(wipe: false);
		BuildGlow();
	}

	public void FillMaterials(bool wipe)
	{
		if (!Object.op_Implicit((Object)(object)glowMaterial) || wipe)
		{
			glowMaterial = MyDetonator().glowMaterial;
		}
	}

	public void BuildGlow()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected Obj, but got Unknown
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		_glow = new GameObject("Glow");
		_glowEmitter = (DetonatorBurstEmitter)(object)_glow.AddComponent("DetonatorBurstEmitter");
		_glow.transform.parent = ((Component)this).transform;
		_glow.transform.localPosition = localPosition;
		_glowEmitter.material = glowMaterial;
		_glowEmitter.exponentialGrowth = false;
		_glowEmitter.useExplicitColorAnimation = true;
		_glowEmitter.useWorldSpace = MyDetonator().useWorldSpace;
	}

	public void UpdateGlow()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		_glow.transform.localPosition = Vector3.Scale(localPosition, new Vector3(size, size, size));
		_glowEmitter.color = color;
		_glowEmitter.duration = duration;
		_glowEmitter.timeScale = timeScale;
		_glowEmitter.count = 1f;
		_glowEmitter.particleSize = 65f;
		_glowEmitter.sizeVariation = 0f;
		_glowEmitter.velocity = new Vector3(0f, 0f, 0f);
		_glowEmitter.startRadius = 0f;
		_glowEmitter.sizeGrow = 0f;
		_glowEmitter.size = size;
		_glowEmitter.explodeDelayMin = explodeDelayMin;
		_glowEmitter.explodeDelayMax = explodeDelayMax;
		Color val = Color.Lerp(color, new Color(0.5f, 0.1f, 0.1f, 1f), 0.5f);
		val.a = 0.9f;
		Color val2 = Color.Lerp(color, new Color(0.6f, 0.3f, 0.3f, 1f), 0.5f);
		val2.a = 0.8f;
		Color val3 = Color.Lerp(color, new Color(0.7f, 0.3f, 0.3f, 1f), 0.5f);
		val3.a = 0.5f;
		Color val4 = Color.Lerp(color, new Color(0.4f, 0.3f, 0.4f, 1f), 0.5f);
		val4.a = 0.2f;
		Color val5 = new Color(0.1f, 0.1f, 0.4f, 0f);
		_glowEmitter.colorAnimation[0] = val;
		_glowEmitter.colorAnimation[1] = val2;
		_glowEmitter.colorAnimation[2] = val3;
		_glowEmitter.colorAnimation[3] = val4;
		_glowEmitter.colorAnimation[4] = val5;
	}

	private void Update()
	{
	}

	public void Reset()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		FillMaterials(wipe: true);
		on = true;
		size = _baseSize;
		duration = _baseDuration;
		explodeDelayMin = 0f;
		explodeDelayMax = 0f;
		color = _baseColor;
		velocity = _baseVelocity;
	}

	public override void Explode()
	{
		if (!(detailThreshold > detail) && on)
		{
			UpdateGlow();
			_glowEmitter.Explode();
		}
	}
}
