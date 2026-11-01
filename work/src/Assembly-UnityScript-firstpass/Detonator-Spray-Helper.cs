using System;
using UnityEngine;

[Serializable]
public class Detonator_0020Spray_0020Helper : MonoBehaviour
{
	public float startTimeMin;

	public float startTimeMax;

	public float stopTimeMin;

	public float stopTimeMax;

	public Material firstMaterial;

	public Material secondMaterial;

	private float startTime;

	private float stopTime;

	private float spawnTime;

	private bool isReallyOn;

	public Detonator_0020Spray_0020Helper()
	{
		stopTimeMin = 10f;
		stopTimeMax = 10f;
	}

	public override void Start()
	{
		isReallyOn = ((Component)this).particleEmitter.emit;
		((Component)this).particleEmitter.emit = false;
		spawnTime = Time.time;
		startTime = Random.value * (startTimeMax - startTimeMin) + startTimeMin + Time.time;
		stopTime = Random.value * (stopTimeMax - stopTimeMin) + stopTimeMin + Time.time;
		if (!(Random.value <= 0.5f))
		{
			((Component)this).renderer.material = firstMaterial;
		}
		else
		{
			((Component)this).renderer.material = secondMaterial;
		}
	}

	public override void FixedUpdate()
	{
		if (!(Time.time <= startTime))
		{
			((Component)this).particleEmitter.emit = isReallyOn;
		}
		if (!(Time.time <= stopTime))
		{
			((Component)this).particleEmitter.emit = false;
		}
	}

	public override void Main()
	{
	}
}
