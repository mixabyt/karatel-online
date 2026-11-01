using UnityEngine;

[RequireComponent(typeof(Detonator))]
[AddComponentMenu("Detonator/Shockwave")]
public class DetonatorShockwave : DetonatorComponent
{
	private float _baseSize = 1f;

	private float _baseDuration = 0.25f;

	private Vector3 _baseVelocity = new Vector3(0f, 0f, 0f);

	private Color _baseColor = Color.white;

	private GameObject _shockwave;

	private DetonatorBurstEmitter _shockwaveEmitter;

	public Material shockwaveMaterial;

	public ParticleRenderMode renderMode;

	public DetonatorShockwave()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
	}

	public override void Init()
	{
		FillMaterials(wipe: false);
		BuildShockwave();
	}

	public void FillMaterials(bool wipe)
	{
		if (!Object.op_Implicit((Object)(object)shockwaveMaterial) || wipe)
		{
			shockwaveMaterial = MyDetonator().shockwaveMaterial;
		}
	}

	public void BuildShockwave()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected Obj, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		_shockwave = new GameObject("Shockwave");
		_shockwaveEmitter = (DetonatorBurstEmitter)(object)_shockwave.AddComponent("DetonatorBurstEmitter");
		_shockwave.transform.parent = ((Component)this).transform;
		_shockwave.transform.localRotation = Quaternion.identity;
		_shockwave.transform.localPosition = localPosition;
		_shockwaveEmitter.material = shockwaveMaterial;
		_shockwaveEmitter.exponentialGrowth = false;
		_shockwaveEmitter.useWorldSpace = MyDetonator().useWorldSpace;
	}

	public void UpdateShockwave()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		_shockwave.transform.localPosition = Vector3.Scale(localPosition, new Vector3(size, size, size));
		_shockwaveEmitter.color = color;
		_shockwaveEmitter.duration = duration;
		_shockwaveEmitter.durationVariation = duration * 0.1f;
		_shockwaveEmitter.count = 1f;
		_shockwaveEmitter.detail = 1f;
		_shockwaveEmitter.particleSize = 25f;
		_shockwaveEmitter.sizeVariation = 0f;
		_shockwaveEmitter.velocity = new Vector3(0f, 0f, 0f);
		_shockwaveEmitter.startRadius = 0f;
		_shockwaveEmitter.sizeGrow = 202f;
		_shockwaveEmitter.size = size;
		_shockwaveEmitter.explodeDelayMin = explodeDelayMin;
		_shockwaveEmitter.explodeDelayMax = explodeDelayMax;
		_shockwaveEmitter.renderMode = renderMode;
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
		if (on)
		{
			UpdateShockwave();
			_shockwaveEmitter.Explode();
		}
	}
}
