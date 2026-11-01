using System;
using Boo.Lang.Runtime;
using UnityEngine;
using UnityScript.Lang;

[Serializable]
[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
	public AnimationClip idleAnimation;

	public AnimationClip walkAnimation;

	public AnimationClip runAnimation;

	public AnimationClip jumpPoseAnimation;

	public float walkMaxAnimationSpeed;

	public float trotMaxAnimationSpeed;

	public float runMaxAnimationSpeed;

	public float jumpAnimationSpeed;

	public float landAnimationSpeed;

	private object _animation;

	private CharacterState _characterState;

	public float walkSpeed;

	public float trotSpeed;

	public float runSpeed;

	public float inAirControlAcceleration;

	public float jumpHeight;

	public float gravity;

	public float speedSmoothing;

	public float rotateSpeed;

	public float trotAfterSeconds;

	public bool canJump;

	private float jumpRepeatTime;

	private float jumpTimeout;

	private float groundedTimeout;

	private float lockCameraTimer;

	private Vector3 moveDirection;

	private float verticalSpeed;

	private float moveSpeed;

	private CollisionFlags collisionFlags;

	private bool jumping;

	private bool jumpingReachedApex;

	private bool movingBack;

	private bool isMoving;

	private float walkTimeStart;

	private float lastJumpButtonTime;

	private float lastJumpTime;

	private float lastJumpStartHeight;

	private Vector3 inAirVelocity;

	private float lastGroundedTime;

	private bool isControllable;

	public ThirdPersonController()
	{
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		walkMaxAnimationSpeed = 0.75f;
		trotMaxAnimationSpeed = 1f;
		runMaxAnimationSpeed = 1f;
		jumpAnimationSpeed = 1.15f;
		landAnimationSpeed = 1f;
		walkSpeed = 2f;
		trotSpeed = 4f;
		runSpeed = 6f;
		inAirControlAcceleration = 3f;
		jumpHeight = 0.5f;
		gravity = 20f;
		speedSmoothing = 10f;
		rotateSpeed = 500f;
		trotAfterSeconds = 3f;
		canJump = true;
		jumpRepeatTime = 0.05f;
		jumpTimeout = 0.15f;
		groundedTimeout = 0.25f;
		moveDirection = Vector3.zero;
		lastJumpButtonTime = -10f;
		lastJumpTime = -1f;
		inAirVelocity = Vector3.zero;
		isControllable = true;
	}

	public override void Awake()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected Obj, but got Unknown
		moveDirection = ((Component)this).transform.TransformDirection(Vector3.forward);
		_animation = (object)(Animation)((Component)this).GetComponent(typeof(Animation));
		if (!RuntimeServices.ToBool(_animation))
		{
			Debug.Log((object)"The character you would like to control doesn't have animations. Moving her might look weird.");
		}
		if (!Object.op_Implicit((Object)(object)idleAnimation))
		{
			_animation = null;
			Debug.Log((object)"No idle animation found. Turning off animations.");
		}
		if (!Object.op_Implicit((Object)(object)walkAnimation))
		{
			_animation = null;
			Debug.Log((object)"No walk animation found. Turning off animations.");
		}
		if (!Object.op_Implicit((Object)(object)runAnimation))
		{
			_animation = null;
			Debug.Log((object)"No run animation found. Turning off animations.");
		}
		if (!Object.op_Implicit((Object)(object)jumpPoseAnimation) && canJump)
		{
			_animation = null;
			Debug.Log((object)"No jump animation found and the character has canJump enabled. Turning off animations.");
		}
	}

	public override void UpdateSmoothedMovementDirection()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		Transform transform = ((Component)Camera.main).transform;
		bool flag = IsGrounded();
		Vector3 val = transform.TransformDirection(Vector3.forward);
		val.y = 0f;
		val = val.normalized;
		Vector3 val2 = new Vector3(val.z, 0f, 0f - val.x);
		float axisRaw = Input.GetAxisRaw("Vertical");
		float axisRaw2 = Input.GetAxisRaw("Horizontal");
		if (!(axisRaw >= -0.2f))
		{
			movingBack = true;
		}
		else
		{
			movingBack = false;
		}
		bool flag2 = isMoving;
		bool flag3 = Mathf.Abs(axisRaw2) > 0.1f;
		if (!flag3)
		{
			flag3 = Mathf.Abs(axisRaw) > 0.1f;
		}
		isMoving = flag3;
		Vector3 val3 = axisRaw2 * val2 + axisRaw * val;
		if (flag)
		{
			lockCameraTimer += Time.deltaTime;
			if (isMoving != flag2)
			{
				lockCameraTimer = 0f;
			}
			if (val3 != Vector3.zero)
			{
				if (!(moveSpeed >= walkSpeed * 0.9f) && flag)
				{
					moveDirection = val3.normalized;
				}
				else
				{
					moveDirection = Vector3.RotateTowards(moveDirection, val3, rotateSpeed * ((float)Math.PI / 180f) * Time.deltaTime, 1000f);
					moveDirection = moveDirection.normalized;
				}
			}
			float num = speedSmoothing * Time.deltaTime;
			float num2 = Mathf.Min(val3.magnitude, 1f);
			_characterState = CharacterState.Idle;
			if (Input.GetKey((KeyCode)304) | Input.GetKey((KeyCode)303))
			{
				num2 *= runSpeed;
				_characterState = CharacterState.Running;
			}
			else if (!(Time.time - trotAfterSeconds <= walkTimeStart))
			{
				num2 *= trotSpeed;
				_characterState = CharacterState.Trotting;
			}
			else
			{
				num2 *= walkSpeed;
				_characterState = CharacterState.Walking;
			}
			moveSpeed = Mathf.Lerp(moveSpeed, num2, num);
			if (!(moveSpeed >= walkSpeed * 0.3f))
			{
				walkTimeStart = Time.time;
			}
		}
		else
		{
			if (jumping)
			{
				lockCameraTimer = 0f;
			}
			if (isMoving)
			{
				inAirVelocity += val3.normalized * Time.deltaTime * inAirControlAcceleration;
			}
		}
	}

	public override void ApplyJumping()
	{
		if (lastJumpTime + jumpRepeatTime <= Time.time && IsGrounded() && canJump && !(Time.time >= lastJumpButtonTime + jumpTimeout))
		{
			verticalSpeed = CalculateJumpVerticalSpeed(jumpHeight);
			((Component)this).SendMessage("DidJump", (SendMessageOptions)1);
		}
	}

	public override void ApplyGravity()
	{
		if (isControllable)
		{
			bool button = Input.GetButton("Jump");
			if (jumping && !jumpingReachedApex && !(verticalSpeed > 0f))
			{
				jumpingReachedApex = true;
				((Component)this).SendMessage("DidJumpReachApex", (SendMessageOptions)1);
			}
			if (IsGrounded())
			{
				verticalSpeed = 0f;
			}
			else
			{
				verticalSpeed -= gravity * Time.deltaTime;
			}
		}
	}

	public override float CalculateJumpVerticalSpeed(float targetJumpHeight)
	{
		return Mathf.Sqrt(2f * targetJumpHeight * gravity);
	}

	public override void DidJump()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		jumping = true;
		jumpingReachedApex = false;
		lastJumpTime = Time.time;
		lastJumpStartHeight = ((Component)this).transform.position.y;
		lastJumpButtonTime = -10f;
		_characterState = CharacterState.Jumping;
	}

	public override void Update()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Expected Obj, but got Unknown
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_044d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		if (!isControllable)
		{
			Input.ResetInputAxes();
		}
		if (Input.GetButtonDown("Jump"))
		{
			lastJumpButtonTime = Time.time;
		}
		UpdateSmoothedMovementDirection();
		ApplyGravity();
		ApplyJumping();
		Vector3 val = moveDirection * moveSpeed + new Vector3(0f, verticalSpeed, 0f) + inAirVelocity;
		val *= Time.deltaTime;
		CharacterController val2 = (CharacterController)((Component)this).GetComponent(typeof(CharacterController));
		collisionFlags = val2.Move(val);
		if (RuntimeServices.ToBool(_animation))
		{
			if (_characterState == CharacterState.Jumping)
			{
				if (!jumpingReachedApex)
				{
					RuntimeServices.SetProperty(RuntimeServices.GetSlice(this, "_animation", new object[1] { ((Object)jumpPoseAnimation).name }), "speed", jumpAnimationSpeed);
					RuntimeServices.SetProperty(RuntimeServices.GetSlice(this, "_animation", new object[1] { ((Object)jumpPoseAnimation).name }), "wrapMode", (object)(WrapMode)8);
					UnityRuntimeServices.Invoke(_animation, "CrossFade", new object[1] { ((Object)jumpPoseAnimation).name }, typeof(MonoBehaviour));
				}
				else
				{
					RuntimeServices.SetProperty(RuntimeServices.GetSlice(this, "_animation", new object[1] { ((Object)jumpPoseAnimation).name }), "speed", 0f - landAnimationSpeed);
					RuntimeServices.SetProperty(RuntimeServices.GetSlice(this, "_animation", new object[1] { ((Object)jumpPoseAnimation).name }), "wrapMode", (object)(WrapMode)8);
					UnityRuntimeServices.Invoke(_animation, "CrossFade", new object[1] { ((Object)jumpPoseAnimation).name }, typeof(MonoBehaviour));
				}
			}
			else
			{
				Vector3 velocity = val2.velocity;
				if (!(velocity.sqrMagnitude >= 0.1f))
				{
					UnityRuntimeServices.Invoke(_animation, "CrossFade", new object[1] { ((Object)idleAnimation).name }, typeof(MonoBehaviour));
				}
				else if (_characterState == CharacterState.Running)
				{
					object slice = RuntimeServices.GetSlice(this, "_animation", new object[1] { ((Object)runAnimation).name });
					Vector3 velocity2 = val2.velocity;
					RuntimeServices.SetProperty(slice, "speed", Mathf.Clamp(velocity2.magnitude, 0f, runMaxAnimationSpeed));
					UnityRuntimeServices.Invoke(_animation, "CrossFade", new object[1] { ((Object)runAnimation).name }, typeof(MonoBehaviour));
				}
				else if (_characterState == CharacterState.Trotting)
				{
					object slice2 = RuntimeServices.GetSlice(this, "_animation", new object[1] { ((Object)walkAnimation).name });
					Vector3 velocity3 = val2.velocity;
					RuntimeServices.SetProperty(slice2, "speed", Mathf.Clamp(velocity3.magnitude, 0f, trotMaxAnimationSpeed));
					UnityRuntimeServices.Invoke(_animation, "CrossFade", new object[1] { ((Object)walkAnimation).name }, typeof(MonoBehaviour));
				}
				else if (_characterState == CharacterState.Walking)
				{
					object slice3 = RuntimeServices.GetSlice(this, "_animation", new object[1] { ((Object)walkAnimation).name });
					Vector3 velocity4 = val2.velocity;
					RuntimeServices.SetProperty(slice3, "speed", Mathf.Clamp(velocity4.magnitude, 0f, walkMaxAnimationSpeed));
					UnityRuntimeServices.Invoke(_animation, "CrossFade", new object[1] { ((Object)walkAnimation).name }, typeof(MonoBehaviour));
				}
			}
		}
		if (IsGrounded())
		{
			((Component)this).transform.rotation = Quaternion.LookRotation(moveDirection);
		}
		else
		{
			Vector3 val3 = val;
			val3.y = 0f;
			if (!(val3.sqrMagnitude <= 0.001f))
			{
				((Component)this).transform.rotation = Quaternion.LookRotation(val3);
			}
		}
		if (IsGrounded())
		{
			lastGroundedTime = Time.time;
			inAirVelocity = Vector3.zero;
			if (jumping)
			{
				jumping = false;
				((Component)this).SendMessage("DidLand", (SendMessageOptions)1);
			}
		}
	}

	public override void OnControllerColliderHit(ControllerColliderHit hit)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if (hit.moveDirection.y <= 0.01f)
		{
		}
	}

	public override float GetSpeed()
	{
		return moveSpeed;
	}

	public override bool IsJumping()
	{
		return jumping;
	}

	public override bool IsGrounded()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Invalid comparison between Unknown and I4
		return (collisionFlags & 4) != 0;
	}

	public override Vector3 GetDirection()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return moveDirection;
	}

	public override bool IsMovingBackwards()
	{
		return movingBack;
	}

	public override float GetLockCameraTimer()
	{
		return lockCameraTimer;
	}

	public override bool IsMoving()
	{
		return Mathf.Abs(Input.GetAxisRaw("Vertical")) + Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.5f;
	}

	public override bool HasJumpReachedApex()
	{
		return jumpingReachedApex;
	}

	public override bool IsGroundedWithTimeout()
	{
		return lastGroundedTime + groundedTimeout > Time.time;
	}

	public override void Reset()
	{
		((Component)this).gameObject.tag = "Player";
	}

	public override void Main()
	{
	}
}
