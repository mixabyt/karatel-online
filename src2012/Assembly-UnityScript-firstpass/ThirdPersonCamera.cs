using System;
using UnityEngine;

[Serializable]
public class ThirdPersonCamera : MonoBehaviour
{
	public Transform cameraTransform;

	private Transform _target;

	public float distance;

	public float height;

	public float angularSmoothLag;

	public float angularMaxSpeed;

	public float heightSmoothLag;

	public float snapSmoothLag;

	public float snapMaxSpeed;

	public float clampHeadPositionScreenSpace;

	public float lockCameraTimeout;

	private Vector3 headOffset;

	private Vector3 centerOffset;

	private float heightVelocity;

	private float angleVelocity;

	private bool snap;

	private ThirdPersonController controller;

	private float targetHeight;

	public ThirdPersonCamera()
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		distance = 7f;
		height = 3f;
		angularSmoothLag = 0.3f;
		angularMaxSpeed = 15f;
		heightSmoothLag = 0.3f;
		snapSmoothLag = 0.2f;
		snapMaxSpeed = 720f;
		clampHeadPositionScreenSpace = 0.75f;
		lockCameraTimeout = 0.2f;
		headOffset = Vector3.zero;
		centerOffset = Vector3.zero;
		targetHeight = 100000f;
	}

	public override void Awake()
	{
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Expected Obj, but got Unknown
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)cameraTransform) && Object.op_Implicit((Object)(object)Camera.main))
		{
			cameraTransform = ((Component)Camera.main).transform;
		}
		if (!Object.op_Implicit((Object)(object)cameraTransform))
		{
			Debug.Log((object)"Please assign a camera to the ThirdPersonCamera script.");
			((Behaviour)this).enabled = false;
		}
		_target = ((Component)this).transform;
		if (Object.op_Implicit((Object)(object)_target))
		{
			controller = (ThirdPersonController)(object)((Component)_target).GetComponent(typeof(ThirdPersonController));
		}
		if (Object.op_Implicit((Object)(object)controller))
		{
			CharacterController val = (CharacterController)((Component)_target).collider;
			Bounds bounds = ((Collider)val).bounds;
			centerOffset = bounds.center - _target.position;
			headOffset = centerOffset;
			ref Vector3 reference = ref headOffset;
			Bounds bounds2 = ((Collider)val).bounds;
			reference.y = bounds2.max.y - _target.position.y;
		}
		else
		{
			Debug.Log((object)"Please assign a target to the camera that has a ThirdPersonController script attached.");
		}
		Cut(_target, centerOffset);
	}

	public override void DebugDrawStuff()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		Debug.DrawLine(_target.position, _target.position + headOffset);
	}

	public override float AngleDistance(float a, float b)
	{
		a = Mathf.Repeat(a, 360f);
		b = Mathf.Repeat(b, 360f);
		return Mathf.Abs(b - a);
	}

	public override void Apply(Transform dummyTarget, Vector3 dummyCenter)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)controller))
		{
			return;
		}
		Vector3 val = _target.position + centerOffset;
		Vector3 headPos = _target.position + headOffset;
		float y = _target.eulerAngles.y;
		float y2 = cameraTransform.eulerAngles.y;
		float num = y;
		if (Input.GetButton("Fire2"))
		{
			snap = true;
		}
		if (snap)
		{
			if (!(AngleDistance(y2, y) >= 3f))
			{
				snap = false;
			}
			y2 = Mathf.SmoothDampAngle(y2, num, ref angleVelocity, snapSmoothLag, snapMaxSpeed);
		}
		else
		{
			if (!(controller.GetLockCameraTimer() >= lockCameraTimeout))
			{
				num = y2;
			}
			if (!(AngleDistance(y2, num) <= 160f) && controller.IsMovingBackwards())
			{
				num += 180f;
			}
			y2 = Mathf.SmoothDampAngle(y2, num, ref angleVelocity, angularSmoothLag, angularMaxSpeed);
		}
		if (controller.IsJumping())
		{
			float num2 = val.y + height;
			if (num2 < targetHeight || !(num2 - targetHeight <= 5f))
			{
				targetHeight = val.y + height;
			}
		}
		else
		{
			targetHeight = val.y + height;
		}
		float y3 = cameraTransform.position.y;
		y3 = Mathf.SmoothDamp(y3, targetHeight, ref heightVelocity, heightSmoothLag);
		Quaternion val2 = Quaternion.Euler(0f, y2, 0f);
		cameraTransform.position = val;
		cameraTransform.position += val2 * Vector3.back * distance;
		float y4 = y3;
		Vector3 position = cameraTransform.position;
		float num3 = (position.y = y4);
		Vector3 val3 = (cameraTransform.position = position);
		SetUpRotation(val, headPos);
	}

	public override void LateUpdate()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Apply(((Component)this).transform, Vector3.zero);
	}

	public override void Cut(Transform dummyTarget, Vector3 dummyCenter)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		float num = heightSmoothLag;
		float num2 = snapMaxSpeed;
		float num3 = snapSmoothLag;
		snapMaxSpeed = 10000f;
		snapSmoothLag = 0.001f;
		heightSmoothLag = 0.001f;
		snap = true;
		Apply(((Component)this).transform, Vector3.zero);
		heightSmoothLag = num;
		snapMaxSpeed = num2;
		snapSmoothLag = num3;
	}

	public override void SetUpRotation(Vector3 centerPos, Vector3 headPos)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = cameraTransform.position;
		Vector3 val = centerPos - position;
		Quaternion val2 = Quaternion.LookRotation(new Vector3(val.x, 0f, val.z));
		Vector3 val3 = Vector3.forward * distance + Vector3.down * height;
		cameraTransform.rotation = val2 * Quaternion.LookRotation(val3);
		Ray val4 = ((Component)cameraTransform).camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1f));
		Ray val5 = ((Component)cameraTransform).camera.ViewportPointToRay(new Vector3(0.5f, clampHeadPositionScreenSpace, 1f));
		Vector3 point = val4.GetPoint(distance);
		Vector3 point2 = val5.GetPoint(distance);
		float num = Vector3.Angle(val4.direction, val5.direction);
		float num2 = num / (point.y - point2.y);
		float num3 = num2 * (point.y - centerPos.y);
		if (!(num3 >= num))
		{
			num3 = 0f;
			return;
		}
		num3 -= num;
		cameraTransform.rotation *= Quaternion.Euler(0f - num3, 0f, 0f);
	}

	public override Vector3 GetCenterOffset()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return centerOffset;
	}

	public override void Main()
	{
	}
}
