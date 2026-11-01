using System;
using System.Collections;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
	public class PlayerMotorMovement
	{
		public SecuredValue<float> MaxForwardSpeed = 5f;

		public SecuredValue<float> MaxSidewaysSpeed = 5f;

		public SecuredValue<float> MaxBackwardsSpeed = 5f;

		public AnimationCurve SlopeSpeedMultiplier = new AnimationCurve(new Keyframe[3]
		{
			new Keyframe(-90f, 1f),
			new Keyframe(0f, 1f),
			new Keyframe(90f, 0f)
		});

		public SecuredValue<float> Gravity = 40f;

		public SecuredValue<float> MaxFallSpeed = 40f;

		public CollisionFlags CollisionFlags;

		public Vector3 Velocity;

		public Vector3 FrameVelocity = Vector3.zero;

		public Vector3 HitPoint = Vector3.zero;

		public Vector3 LastHitPoint = new Vector3(float.PositiveInfinity, 0f, 0f);

		public PlayerMotorMovement()
		{
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Expected Obj, but got Unknown
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		}
	}

	public enum MovementTransferOnJump
	{
		None,
		InitTransfer,
		PermaTransfer,
		PermaLocked
	}

	public class PlayerMotorJumping
	{
		public bool Enabled = true;

		public bool Jumping;

		public bool HoldingJumpButton;

		public float LastStartTime;

		public float LastButtonDownTime = -100f;

		public Vector3 JumpDir = Vector3.up;

		public PlayerMotorJumping()
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		}
	}

	public class PlayerMotorMovingPlatform
	{
		public bool Enabled = true;

		public MovementTransferOnJump MovementTransfer = MovementTransferOnJump.PermaTransfer;

		public Transform HitPlatform;

		public Transform ActivePlatform;

		public Vector3 ActiveLocalPoint;

		public Vector3 ActiveGlobalPoint;

		public Quaternion ActiveLocalRotation;

		public Quaternion ActiveGlobalRotation;

		public Matrix4x4 LastMatrix;

		public Vector3 PlatformVelocity;

		public bool NewPlatform;
	}

	public class PlayerMotorSliding
	{
		public bool Enabled = true;
	}

	[NonSerialized]
	public bool CanControl = true;

	[NonSerialized]
	public SecuredValue<Vector3> InputMoveDirection = Vector3.zero;

	[NonSerialized]
	public SecuredValue<bool> InputJump = false;

	[NonSerialized]
	public SecuredValue<float> JumpModifier = 1f;

	[NonSerialized]
	public PlayerMotorMovement Movement = new PlayerMotorMovement();

	[NonSerialized]
	public PlayerMotorJumping Jumping = new PlayerMotorJumping();

	[NonSerialized]
	public PlayerMotorMovingPlatform MovingPlatform = new PlayerMotorMovingPlatform();

	[NonSerialized]
	public PlayerMotorSliding Sliding = new PlayerMotorSliding();

	[NonSerialized]
	public bool Grounded = true;

	[NonSerialized]
	public bool IsNeedJump;

	[NonSerialized]
	public bool JumpSeria;

	[NonSerialized]
	public float LastBouncVel;

	[NonSerialized]
	public Vector3 GroundNormal = Vector3.zero;

	private Vector3 _LastGroundNormal = Vector3.zero;

	private Transform _Tr;

	private CharacterController _Controller;

	internal SecuredValue<float> CrouchSpeed = 0f;

	public PlayerMotor()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
	}

	private void Awake()
	{
		_Controller = ((Component)this).GetComponent<CharacterController>();
		_Tr = ((Component)this).transform;
	}

	private void Update()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0539: Unknown result type (might be due to invalid IL or missing references)
		//IL_053e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_054e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0575: Unknown result type (might be due to invalid IL or missing references)
		//IL_057a: Unknown result type (might be due to invalid IL or missing references)
		//IL_057f: Unknown result type (might be due to invalid IL or missing references)
		//IL_059b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		Vector3 velocity = Movement.Velocity;
		velocity = ApplyInputVelocityChange(velocity);
		velocity = ApplyGravityAndJumping(velocity);
		Vector3 zero = Vector3.zero;
		if (MoveWithPlatform())
		{
			Vector3 val = MovingPlatform.ActivePlatform.TransformPoint(MovingPlatform.ActiveLocalPoint);
			zero = val - MovingPlatform.ActiveGlobalPoint;
			if (zero != Vector3.zero)
			{
				_Controller.Move(zero);
			}
			Quaternion val2 = MovingPlatform.ActivePlatform.rotation * MovingPlatform.ActiveLocalRotation;
			Quaternion val3 = val2 * Quaternion.Inverse(MovingPlatform.ActiveGlobalRotation);
			float y = val3.eulerAngles.y;
			if (y != 0f)
			{
				_Tr.Rotate(0f, y, 0f);
			}
		}
		Vector3 position = _Tr.position;
		Vector3 val4 = velocity * Time.deltaTime;
		float stepOffset = _Controller.stepOffset;
		Vector3 val5 = new Vector3(val4.x, 0f, val4.z);
		float num = Mathf.Max(stepOffset, val5.magnitude);
		if (Grounded)
		{
			val4 -= num * Vector3.up;
		}
		MovingPlatform.HitPlatform = null;
		GroundNormal = Vector3.zero;
		if (((Collider)_Controller).enabled)
		{
			Movement.CollisionFlags = _Controller.Move(val4);
		}
		Movement.LastHitPoint = Movement.HitPoint;
		_LastGroundNormal = GroundNormal;
		if (MovingPlatform.Enabled && (Object)(object)MovingPlatform.ActivePlatform != (Object)(object)MovingPlatform.HitPlatform && (Object)(object)MovingPlatform.HitPlatform != (Object)null)
		{
			MovingPlatform.ActivePlatform = MovingPlatform.HitPlatform;
			MovingPlatform.LastMatrix = MovingPlatform.HitPlatform.localToWorldMatrix;
			MovingPlatform.NewPlatform = true;
		}
		Vector3 val6 = new Vector3(velocity.x, 0f, velocity.z);
		Movement.Velocity = (_Tr.position - position) / Time.deltaTime;
		Vector3 val7 = new Vector3(Movement.Velocity.x, 0f, Movement.Velocity.z);
		if (val6 == Vector3.zero)
		{
			Movement.Velocity = new Vector3(0f, Movement.Velocity.y, 0f);
		}
		else
		{
			float num2 = Vector3.Dot(val7, val6) / val6.sqrMagnitude;
			Movement.Velocity = val6 * Mathf.Clamp01(num2) + Movement.Velocity.y * Vector3.up;
		}
		if (Movement.Velocity.y < velocity.y - 0.001f)
		{
			if (Movement.Velocity.y < 0f)
			{
				Movement.Velocity.y = velocity.y;
			}
			else
			{
				Jumping.HoldingJumpButton = false;
			}
		}
		if (Grounded && !IsGroundedTest())
		{
			Grounded = false;
			if (MovingPlatform.Enabled && (MovingPlatform.MovementTransfer == MovementTransferOnJump.InitTransfer || MovingPlatform.MovementTransfer == MovementTransferOnJump.PermaTransfer))
			{
				Movement.FrameVelocity = MovingPlatform.PlatformVelocity;
				PlayerMotorMovement movement = Movement;
				movement.Velocity += MovingPlatform.PlatformVelocity;
			}
			((Component)this).SendMessage("OnFall", (SendMessageOptions)1);
			Transform tr = _Tr;
			tr.position += num * Vector3.up;
		}
		else if (!Grounded && IsGroundedTest())
		{
			Grounded = true;
			Jumping.Jumping = false;
			if (MovingPlatform.Enabled)
			{
				((MonoBehaviour)this).StartCoroutine(SubtractNewPlatformVelocity());
			}
			((Component)this).SendMessage("OnLand", (SendMessageOptions)1);
			BlockType blockType = WorldData.Instance.GetBlockType((int)((Component)WorldGameObject.Instance.MainPlayerEyes).transform.position.x, (int)((Component)WorldGameObject.Instance.MainPlayerEyes).transform.position.z, (int)((Component)WorldGameObject.Instance.MainPlayerEyes).transform.position.y - 2);
			if (blockType == BlockType.Gum)
			{
				IsNeedJump = true;
			}
			else
			{
				JumpSeria = false;
			}
		}
		if (MoveWithPlatform())
		{
			MovingPlatform.ActiveGlobalPoint = _Tr.position + Vector3.up * (_Controller.center.y - _Controller.height * 0.5f + _Controller.radius);
			MovingPlatform.ActiveLocalPoint = MovingPlatform.ActivePlatform.InverseTransformPoint(MovingPlatform.ActiveGlobalPoint);
			MovingPlatform.ActiveGlobalRotation = _Tr.rotation;
			MovingPlatform.ActiveLocalRotation = Quaternion.Inverse(MovingPlatform.ActivePlatform.rotation) * MovingPlatform.ActiveGlobalRotation;
		}
	}

	private void FixedUpdate()
	{
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		if (!MovingPlatform.Enabled)
		{
			return;
		}
		if ((Object)(object)MovingPlatform.ActivePlatform != (Object)null)
		{
			if (!MovingPlatform.NewPlatform)
			{
				PlayerMotorMovingPlatform movingPlatform = MovingPlatform;
				Matrix4x4 localToWorldMatrix = MovingPlatform.ActivePlatform.localToWorldMatrix;
				movingPlatform.PlatformVelocity = (localToWorldMatrix.MultiplyPoint3x4(MovingPlatform.ActiveLocalPoint) - MovingPlatform.LastMatrix.MultiplyPoint3x4(MovingPlatform.ActiveLocalPoint)) / Time.deltaTime;
			}
			MovingPlatform.LastMatrix = MovingPlatform.ActivePlatform.localToWorldMatrix;
			MovingPlatform.NewPlatform = false;
		}
		else
		{
			MovingPlatform.PlatformVelocity = Vector3.zero;
		}
	}

	private Vector3 ApplyInputVelocityChange(Vector3 velocity)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		if (!CanControl)
		{
			InputMoveDirection = Vector3.zero;
		}
		Vector3 normalized;
		if (Grounded && TooSteep())
		{
			Vector3 val = new Vector3(GroundNormal.x, 0f, GroundNormal.z);
			normalized = val.normalized;
			Vector3 val2 = Vector3.Project((Vector3)InputMoveDirection, normalized);
			normalized = normalized + val2 * 0.4f + ((Vector3)InputMoveDirection - val2) * 1f;
			normalized *= 15f;
		}
		else
		{
			normalized = GetDesiredHorizontalVelocity();
		}
		if (MovingPlatform.Enabled && MovingPlatform.MovementTransfer == MovementTransferOnJump.PermaTransfer)
		{
			normalized += Movement.FrameVelocity;
			normalized.y = 0f;
		}
		if (Grounded)
		{
			normalized = AdjustGroundVelocityToNormal(normalized, GroundNormal);
		}
		else
		{
			velocity.y = 0f;
		}
		float num = GetMaxAcceleration(Grounded) * Time.deltaTime;
		Vector3 val3 = normalized - velocity;
		if (val3.sqrMagnitude > num * num)
		{
			val3 = val3.normalized * num;
		}
		if (Grounded || CanControl)
		{
			velocity += val3;
		}
		if (Grounded)
		{
			velocity.y = Mathf.Min(velocity.y, 0f);
		}
		return velocity;
	}

	private Vector3 ApplyGravityAndJumping(Vector3 velocity)
	{
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		if (!InputJump || !CanControl)
		{
			Jumping.HoldingJumpButton = false;
			Jumping.LastButtonDownTime = -100f;
		}
		if ((bool)InputJump && Jumping.LastButtonDownTime < 0f && CanControl)
		{
			Jumping.LastButtonDownTime = Time.time;
		}
		if (Grounded)
		{
			velocity.y = Mathf.Min(0f, velocity.y) - (float)Movement.Gravity * Time.deltaTime;
		}
		else
		{
			velocity.y = Movement.Velocity.y - (float)Movement.Gravity * Time.deltaTime;
			velocity.y = Mathf.Max(velocity.y, 0f - (float)Movement.MaxFallSpeed);
		}
		if (Grounded)
		{
			if ((Jumping.Enabled && CanControl && Time.time - Jumping.LastButtonDownTime < 0.2f) || IsNeedJump)
			{
				Grounded = false;
				Jumping.Jumping = true;
				Jumping.LastStartTime = Time.time;
				Jumping.LastButtonDownTime = -100f;
				Jumping.HoldingJumpButton = true;
				if (TooSteep())
				{
					Jumping.JumpDir = Vector3.Slerp(Vector3.up, GroundNormal, 0.5f);
				}
				else
				{
					Jumping.JumpDir = Vector3.Slerp(Vector3.up, GroundNormal, 0f);
				}
				if (IsNeedJump)
				{
					float num = Mathf.Abs(velocity.y);
					num = ((!JumpSeria) ? 2f : (LastBouncVel + 1.5f));
					Debug.Log((object)num);
					IsNeedJump = false;
					velocity.y = 0f;
					velocity += Jumping.JumpDir * CalculateJumpVerticalSpeed(num);
					LastBouncVel = num;
					JumpSeria = true;
				}
				else
				{
					velocity.y = 0f;
					velocity += Jumping.JumpDir * CalculateJumpVerticalSpeed(1f);
				}
				if (MovingPlatform.Enabled && (MovingPlatform.MovementTransfer == MovementTransferOnJump.InitTransfer || MovingPlatform.MovementTransfer == MovementTransferOnJump.PermaTransfer))
				{
					Movement.FrameVelocity = MovingPlatform.PlatformVelocity;
					velocity += MovingPlatform.PlatformVelocity;
				}
				((Component)this).SendMessage("OnJump", (SendMessageOptions)1);
			}
			else
			{
				Jumping.HoldingJumpButton = false;
			}
		}
		return velocity;
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		if (hit.normal.y > 0f && hit.normal.y > GroundNormal.y && hit.moveDirection.y < 0f)
		{
			Vector3 val = hit.point - Movement.LastHitPoint;
			if (val.sqrMagnitude > 0.001f || _LastGroundNormal == Vector3.zero)
			{
				GroundNormal = hit.normal;
			}
			else
			{
				GroundNormal = _LastGroundNormal;
			}
			MovingPlatform.HitPlatform = ((Component)hit.collider).transform;
			Movement.HitPoint = hit.point;
			Movement.FrameVelocity = Vector3.zero;
		}
	}

	private IEnumerator SubtractNewPlatformVelocity()
	{
		if (!MovingPlatform.Enabled || (MovingPlatform.MovementTransfer != MovementTransferOnJump.InitTransfer && MovingPlatform.MovementTransfer != MovementTransferOnJump.PermaTransfer))
		{
			yield break;
		}
		if (MovingPlatform.NewPlatform)
		{
			Transform platform = MovingPlatform.ActivePlatform;
			yield return (object)new WaitForFixedUpdate();
			yield return (object)new WaitForFixedUpdate();
			if (Grounded && (Object)(object)platform == (Object)(object)MovingPlatform.ActivePlatform)
			{
				yield return 1;
			}
		}
		PlayerMotorMovement movement = Movement;
		movement.Velocity -= MovingPlatform.PlatformVelocity;
	}

	private bool MoveWithPlatform()
	{
		return MovingPlatform.Enabled && (Grounded || MovingPlatform.MovementTransfer == MovementTransferOnJump.PermaLocked) && (Object)(object)MovingPlatform.ActivePlatform != (Object)null;
	}

	private Vector3 GetDesiredHorizontalVelocity()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = _Tr.InverseTransformDirection((Vector3)InputMoveDirection);
		float num = MaxSpeedInDirection(val);
		if (Grounded)
		{
			float num2 = Mathf.Asin(Movement.Velocity.normalized.y) * 57.29578f;
			num *= Movement.SlopeSpeedMultiplier.Evaluate(num2);
		}
		return _Tr.TransformDirection(val * num);
	}

	private Vector3 AdjustGroundVelocityToNormal(Vector3 hVelocity, Vector3 groundNormal)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3.Cross(Vector3.up, hVelocity);
		Vector3 val2 = Vector3.Cross(val, groundNormal);
		return val2.normalized * hVelocity.magnitude;
	}

	private bool IsGroundedTest()
	{
		return GroundNormal.y > 0.01f;
	}

	private float GetMaxAcceleration(bool grounded)
	{
		if (grounded)
		{
			return 30f;
		}
		return 10f;
	}

	private float CalculateJumpVerticalSpeed(float targetJumpHeight)
	{
		return Mathf.Sqrt(2f * targetJumpHeight * (float)Movement.Gravity * (float)JumpModifier);
	}

	private bool IsJumping()
	{
		return Jumping.Jumping;
	}

	private bool IsSliding()
	{
		return Grounded && Sliding.Enabled && TooSteep();
	}

	private bool IsTouchingCeiling()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Invalid comparison between Unknown and I4
		return (Movement.CollisionFlags & 2) != 0;
	}

	private bool IsGrounded()
	{
		return Grounded;
	}

	private bool TooSteep()
	{
		return GroundNormal.y <= Mathf.Cos(_Controller.slopeLimit * ((float)Math.PI / 180f));
	}

	private Vector3 GetDirection()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return InputMoveDirection;
	}

	private void SetControllable(bool controllable)
	{
		CanControl = controllable;
	}

	private float MaxSpeedInDirection(Vector3 desiredMovementDirection)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		if (desiredMovementDirection == Vector3.zero)
		{
			return 0f;
		}
		float num = ((!((Component)this).GetComponent<PlayerNetwork>().Crouch) ? 1f : 0.3f);
		float num2 = (float)((!(desiredMovementDirection.z > 0f)) ? Movement.MaxBackwardsSpeed : Movement.MaxForwardSpeed) / (float)Movement.MaxSidewaysSpeed;
		Vector3 val = new Vector3(desiredMovementDirection.x, 0f, desiredMovementDirection.z / num2);
		Vector3 normalized = val.normalized;
		Vector3 val2 = new Vector3(normalized.x, 0f, normalized.z * num2);
		float num3 = val2.magnitude * (float)Movement.MaxSidewaysSpeed;
		return num3 * num;
	}

	private void SetVelocity(Vector3 velocity)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Grounded = false;
		Movement.Velocity = velocity;
		Movement.FrameVelocity = Vector3.zero;
		((Component)this).SendMessage("OnExternalVelocity");
	}
}
