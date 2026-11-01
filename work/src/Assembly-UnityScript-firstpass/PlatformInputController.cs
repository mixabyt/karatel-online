using System;
using UnityEngine;

[Serializable]
[AddComponentMenu("Character/Platform Input Controller")]
[RequireComponent(typeof(CharacterMotor))]
public class PlatformInputController : MonoBehaviour
{
	public bool autoRotate;

	public float maxRotationSpeed;

	private CharacterMotor motor;

	public PlatformInputController()
	{
		autoRotate = true;
		maxRotationSpeed = 360f;
	}

	public override void Awake()
	{
		motor = (CharacterMotor)(object)((Component)this).GetComponent(typeof(CharacterMotor));
	}

	public override void Update()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f);
		if (val != Vector3.zero)
		{
			float magnitude = val.magnitude;
			val /= magnitude;
			magnitude = Mathf.Min(1f, magnitude);
			magnitude *= magnitude;
			val *= magnitude;
		}
		val = ((Component)Camera.main).transform.rotation * val;
		Quaternion val2 = Quaternion.FromToRotation(-((Component)Camera.main).transform.forward, ((Component)this).transform.up);
		val = val2 * val;
		motor.inputMoveDirection = val;
		motor.inputJump = Input.GetButton("Jump");
		if (autoRotate && !(val.sqrMagnitude <= 0.01f))
		{
			Vector3 v = ConstantSlerp(((Component)this).transform.forward, val, maxRotationSpeed * Time.deltaTime);
			v = ProjectOntoPlane(v, ((Component)this).transform.up);
			((Component)this).transform.rotation = Quaternion.LookRotation(v, ((Component)this).transform.up);
		}
	}

	public override Vector3 ProjectOntoPlane(Vector3 v, Vector3 normal)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return v - Vector3.Project(v, normal);
	}

	public override Vector3 ConstantSlerp(Vector3 from, Vector3 to, float angle)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		float num = Mathf.Min(1f, angle / Vector3.Angle(from, to));
		return Vector3.Slerp(from, to, num);
	}

	public override void Main()
	{
	}
}
