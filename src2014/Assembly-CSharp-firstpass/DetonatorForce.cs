using UnityEngine;

[RequireComponent(typeof(Detonator))]
[AddComponentMenu("Detonator/Force")]
public class DetonatorForce : DetonatorComponent
{
	private float _baseRadius = 50f;

	private float _basePower = 4000f;

	private float _scaledRange;

	private float _scaledIntensity;

	private bool _delayedExplosionStarted;

	private float _explodeDelay;

	public float radius;

	public float power;

	public GameObject fireObject;

	public float fireObjectLife;

	private Collider[] _colliders;

	private GameObject _tempFireObject;

	private Vector3 _explosionPosition;

	public override void Init()
	{
	}

	private void Update()
	{
		if (_delayedExplosionStarted)
		{
			_explodeDelay -= Time.deltaTime;
			if (_explodeDelay <= 0f)
			{
				Explode();
			}
		}
	}

	public override void Explode()
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		if (!on || detailThreshold > detail)
		{
			return;
		}
		if (!_delayedExplosionStarted)
		{
			_explodeDelay = explodeDelayMin + Random.value * (explodeDelayMax - explodeDelayMin);
		}
		if (_explodeDelay <= 0f)
		{
			_explosionPosition = ((Component)this).transform.position;
			_colliders = Physics.OverlapSphere(_explosionPosition, radius);
			Collider[] colliders = _colliders;
			foreach (Collider val in colliders)
			{
				if (!Object.op_Implicit((Object)(object)val) || !Object.op_Implicit((Object)(object)((Component)val).rigidbody))
				{
					continue;
				}
				((Component)val).rigidbody.AddExplosionForce(power * size, _explosionPosition, radius * size, 4f * MyDetonator().upwardsBias * size);
				((Component)this).SendMessage("OnDetonatorForceHit", (object)null, (SendMessageOptions)1);
				if (Object.op_Implicit((Object)(object)fireObject))
				{
					if (Object.op_Implicit((Object)(object)((Component)val).transform.Find(((Object)fireObject).name + "(Clone)")))
					{
						return;
					}
					Object val2 = Object.Instantiate((Object)(object)fireObject, ((Component)this).transform.position, ((Component)this).transform.rotation);
					_tempFireObject = (GameObject)(object)((val2 is GameObject) ? val2 : null);
					_tempFireObject.transform.parent = ((Component)val).transform;
					_tempFireObject.transform.localPosition = new Vector3(0f, 0f, 0f);
					if (Object.op_Implicit((Object)(object)_tempFireObject.particleEmitter))
					{
						_tempFireObject.particleEmitter.emit = true;
						Object.Destroy((Object)(object)_tempFireObject, fireObjectLife);
					}
				}
			}
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
		radius = _baseRadius;
		power = _basePower;
	}
}
