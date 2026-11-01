using System;
using UnityEngine;

public class sin_movement : MonoBehaviour
{
	public float amplitude;

	public float frequency;

	private Vector3 dir = new Vector3(0f, 1f, 0f);

	public sin_movement()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
	}

	private void Start()
	{
	}

	private void Update()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		Transform transform = ((Component)this).transform;
		transform.position += amplitude * (Mathf.Sin((float)Math.PI * 2f * frequency * Time.time) - Mathf.Sin((float)Math.PI * 2f * frequency * (Time.time - Time.deltaTime))) * dir;
	}
}
