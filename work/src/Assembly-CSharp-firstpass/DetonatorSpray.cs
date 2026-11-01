using UnityEngine;

[AddComponentMenu("Detonator/Object Spray")]
[RequireComponent(typeof(Detonator))]
public class DetonatorSpray : DetonatorComponent
{
	public GameObject sprayObject;

	public int count = 10;

	public float startingRadius;

	public float minScale = 1f;

	public float maxScale = 1f;

	private bool _delayedExplosionStarted;

	private float _explodeDelay;

	private Vector3 _explosionPosition;

	private float _tmpScale;

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
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		if (!_delayedExplosionStarted)
		{
			_explodeDelay = explodeDelayMin + Random.value * (explodeDelayMax - explodeDelayMin);
		}
		if (_explodeDelay <= 0f)
		{
			int num = (int)(detail * (float)count);
			for (int i = 0; i < num; i++)
			{
				Vector3 val = Random.onUnitSphere * (startingRadius * size);
				Vector3 val2 = new Vector3(velocity.x * size, velocity.y * size, velocity.z * size);
				Object val3 = Object.Instantiate((Object)(object)sprayObject, ((Component)this).transform.position + val, ((Component)this).transform.rotation);
				GameObject val4 = (GameObject)(object)((val3 is GameObject) ? val3 : null);
				val4.transform.parent = ((Component)this).transform;
				_tmpScale = minScale + Random.value * (maxScale - minScale);
				_tmpScale *= size;
				val4.transform.localScale = new Vector3(_tmpScale, _tmpScale, _tmpScale);
				val4.rigidbody.velocity = Vector3.Scale(val.normalized, val2);
				Object.Destroy((Object)(object)val4, duration * timeScale);
				_delayedExplosionStarted = false;
				_explodeDelay = 0f;
			}
		}
		else
		{
			_delayedExplosionStarted = true;
		}
	}

	public void Reset()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		velocity = new Vector3(15f, 15f, 15f);
	}
}
