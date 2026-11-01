using UnityEngine;

[RequireComponent(typeof(Detonator))]
[AddComponentMenu("Detonator/Light")]
public class DetonatorLight : DetonatorComponent
{
	private float _baseIntensity = 1f;

	private Color _baseColor = Color.white;

	private float _scaledDuration;

	private float _explodeTime = -1000f;

	private GameObject _light;

	private Light _lightComponent;

	public float intensity;

	private float _reduceAmount;

	public DetonatorLight()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
	}

	public override void Init()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected Obj, but got Unknown
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected Obj, but got Unknown
		_light = new GameObject("Light");
		_light.transform.parent = ((Component)this).transform;
		_light.transform.localPosition = localPosition;
		_lightComponent = (Light)_light.AddComponent("Light");
		_lightComponent.type = (LightType)2;
		((Behaviour)_lightComponent).enabled = false;
	}

	private void Update()
	{
		if (_explodeTime + _scaledDuration > Time.time && _lightComponent.intensity > 0f)
		{
			_reduceAmount = intensity * (Time.deltaTime / _scaledDuration);
			Light lightComponent = _lightComponent;
			lightComponent.intensity -= _reduceAmount;
		}
		else if (Object.op_Implicit((Object)(object)_lightComponent))
		{
			((Behaviour)_lightComponent).enabled = false;
		}
	}

	public override void Explode()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (!(detailThreshold > detail))
		{
			_lightComponent.color = color;
			_lightComponent.range = size * 50f;
			_scaledDuration = duration * timeScale;
			((Behaviour)_lightComponent).enabled = true;
			_lightComponent.intensity = intensity;
			_explodeTime = Time.time;
		}
	}

	public void Reset()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		color = _baseColor;
		intensity = _baseIntensity;
	}
}
