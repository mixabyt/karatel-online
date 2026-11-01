using UnityEngine;

[RequireComponent(typeof(Detonator))]
[AddComponentMenu("Detonator/Heatwave (Pro Only)")]
public class DetonatorHeatwave : DetonatorComponent
{
	private GameObject _heatwave;

	private float s;

	private float _startSize;

	private float _maxSize;

	private float _baseDuration = 0.25f;

	private bool _delayedExplosionStarted;

	private float _explodeDelay;

	public float zOffset = 0.5f;

	public float distortion = 64f;

	private float _elapsedTime;

	private float _normalizedTime;

	public Material heatwaveMaterial;

	private Material _material;

	public override void Init()
	{
	}

	private void Update()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		if (_delayedExplosionStarted)
		{
			_explodeDelay -= Time.deltaTime;
			if (_explodeDelay <= 0f)
			{
				Explode();
			}
		}
		if (Object.op_Implicit((Object)(object)_heatwave))
		{
			_heatwave.transform.rotation = Quaternion.FromToRotation(Vector3.up, ((Component)Camera.main).transform.position - _heatwave.transform.position);
			_heatwave.transform.localPosition = localPosition + Vector3.forward * zOffset;
			_elapsedTime += Time.deltaTime;
			_normalizedTime = _elapsedTime / duration;
			s = Mathf.Lerp(_startSize, _maxSize, _normalizedTime);
			_heatwave.renderer.material.SetFloat("_BumpAmt", (1f - _normalizedTime) * distortion);
			_heatwave.gameObject.transform.localScale = new Vector3(s, s, s);
			if (_elapsedTime > duration)
			{
				Object.Destroy((Object)(object)_heatwave.gameObject);
			}
		}
	}

	public override void Explode()
	{
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Expected Obj, but got Unknown
		if (!SystemInfo.supportsImageEffects || detailThreshold > detail || !on)
		{
			return;
		}
		if (!_delayedExplosionStarted)
		{
			_explodeDelay = explodeDelayMin + Random.value * (explodeDelayMax - explodeDelayMin);
		}
		if (_explodeDelay <= 0f)
		{
			_startSize = 0f;
			_maxSize = size * 10f;
			_material = new Material(Shader.Find("HeatDistort"));
			_heatwave = GameObject.CreatePrimitive((PrimitiveType)4);
			Object.Destroy((Object)(object)_heatwave.GetComponent(typeof(MeshCollider)));
			if (!Object.op_Implicit((Object)(object)heatwaveMaterial))
			{
				heatwaveMaterial = MyDetonator().heatwaveMaterial;
			}
			_material.CopyPropertiesFromMaterial(heatwaveMaterial);
			_heatwave.renderer.material = _material;
			_heatwave.transform.parent = ((Component)this).transform;
			_delayedExplosionStarted = false;
			_explodeDelay = 0f;
		}
		else
		{
			_delayedExplosionStarted = true;
		}
	}

	public void Reset()
	{
		duration = _baseDuration;
	}
}
