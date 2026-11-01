using System;
using UnityEngine;

[Serializable]
[AddComponentMenu("Camera-Control/Smooth Look At")]
public class SmoothLookAt : MonoBehaviour
{
	public Transform target;

	public float damping;

	public bool smooth;

	public SmoothLookAt()
	{
		damping = 6f;
		smooth = true;
	}

	public override void LateUpdate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)target))
		{
			if (smooth)
			{
				Quaternion val = Quaternion.LookRotation(target.position - ((Component)this).transform.position);
				((Component)this).transform.rotation = Quaternion.Slerp(((Component)this).transform.rotation, val, Time.deltaTime * damping);
			}
			else
			{
				((Component)this).transform.LookAt(target);
			}
		}
	}

	public override void Start()
	{
		if (Object.op_Implicit((Object)(object)((Component)this).rigidbody))
		{
			((Component)this).rigidbody.freezeRotation = true;
		}
	}

	public override void Main()
	{
	}
}
