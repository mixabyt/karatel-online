using System;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(CharacterMotor))]
[AddComponentMenu("Character/FPS Input Controller")]
public class FPSInputController : MonoBehaviour
{
	private CharacterMotor motor;

	private CharacterController controller;

	private bool enable_movement;

	private bool under_water;

	private float water_time;

	private int water_speed;

	private float water_velocity;

	private int lader_speed;

	private bool ladder;

	private bool on_ice;

	private bool direct_movement;

	public Vector3 curVel;

	public FPSInputController()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		curVel = Vector3.zero;
	}

	public override void Awake()
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected Obj, but got Unknown
		enable_movement = true;
		under_water = false;
		water_time = 0f;
		ladder = false;
		on_ice = false;
		water_speed = 3;
		lader_speed = 4;
		motor = (CharacterMotor)(object)((Component)this).GetComponent(typeof(CharacterMotor));
		controller = (CharacterController)((Component)this).GetComponent(typeof(CharacterController));
		direct_movement = false;
	}

	public override void SetDirectMovement(bool enabled)
	{
		direct_movement = enabled;
	}

	public override void SetMovement(bool move)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		enable_movement = move;
		if (!move)
		{
			motor.inputMoveDirection = new Vector3(0f, 0f, 0f);
			motor.inputJump = false;
		}
	}

	public override void SetSpeedEffect(bool enable)
	{
		if (enable)
		{
			motor.movement.maxForwardSpeed = 13f;
			motor.movement.maxSidewaysSpeed = 13f;
			motor.movement.maxBackwardsSpeed = 13f;
			water_speed = 8;
			lader_speed = 12;
		}
		else
		{
			motor.movement.maxForwardSpeed = 5f;
			motor.movement.maxSidewaysSpeed = 5f;
			motor.movement.maxBackwardsSpeed = 5f;
			water_speed = 3;
			lader_speed = 5;
		}
	}

	public override void SetIceBody(bool ice)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (ice && !on_ice)
		{
			curVel = motor.movement.velocity * 0.2f;
		}
		on_ice = ice;
	}

	public override void SetUnderWaterBody(bool[] water_data)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if (water_data[0])
		{
			if (!under_water)
			{
				motor.movement.gravity = 0f;
				motor.movement.velocity = Vector3.zero;
				motor.movement.frameVelocity = Vector3.zero;
				motor.inputMoveDirection = Vector3.zero;
				under_water = water_data[0];
				water_time = Time.time + 1f;
			}
			else if (water_data[1])
			{
				water_time = Time.time + 0.5f;
			}
		}
		else if (!water_data[0] && under_water && !ladder)
		{
			motor.movement.gravity = 40f;
			under_water = water_data[0];
		}
	}

	public override void SetLadderBody(bool _ladder)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		ladder = _ladder;
		if (ladder)
		{
			under_water = false;
			motor.movement.gravity = 0f;
			motor.movement.velocity = Vector3.zero;
			motor.movement.frameVelocity = Vector3.zero;
			motor.inputMoveDirection = Vector3.zero;
		}
		else
		{
			motor.movement.gravity = 40f;
		}
	}

	public override void Update()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		if (!enable_movement)
		{
			return;
		}
		Vector3 val = Vector3.zero;
		val = (direct_movement ? new Vector3(0f, 0f, Mathf.Max(Mathf.Abs(Input.GetAxis("Horizontal")), Mathf.Abs(Input.GetAxis("Vertical")))) : new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")));
		if (val != Vector3.zero)
		{
			float magnitude = val.magnitude;
			val /= magnitude;
			magnitude = Mathf.Min(1f, magnitude);
			magnitude *= magnitude;
			val *= magnitude;
		}
		if (ladder)
		{
			val.x *= (float)lader_speed;
			val.z *= (float)lader_speed;
			if (Input.GetKey("space"))
			{
				num = 1f;
			}
			if (Input.GetKey("left shift"))
			{
				num = -1f;
			}
			val.y = (float)lader_speed * num;
			motor.inputJump = Input.GetButton("Jump");
			controller.Move(((Component)this).transform.rotation * val * Time.deltaTime);
		}
		else if (under_water)
		{
			motor.movement.gravity = 0f;
			val.x *= (float)water_speed;
			val.z *= (float)water_speed;
			if (Input.GetKey("space") && !(Time.time < water_time))
			{
				water_velocity = Mathf.MoveTowards(water_velocity, 1f, Time.deltaTime * 15f);
				if (!(water_velocity <= 0f))
				{
					num = water_velocity;
				}
			}
			else
			{
				water_velocity = Mathf.MoveTowards(water_velocity, -1f, Time.deltaTime * 15f);
			}
			if (Input.GetKey("left shift"))
			{
				num = -1f;
			}
			val.y = (float)water_speed * num;
			motor.inputJump = Input.GetButton("Jump");
			if (!(water_velocity >= 0f))
			{
				val.y += water_velocity;
			}
			controller.Move(((Component)this).transform.rotation * val * Time.deltaTime);
		}
		else if (on_ice)
		{
			Vector3 val2 = ((Component)this).transform.TransformDirection(val);
			curVel = Vector3.Lerp(curVel, val2, 1.25f * Time.deltaTime);
			motor.inputMoveDirection = curVel;
			motor.inputJump = Input.GetButton("Jump");
		}
		else
		{
			motor.inputMoveDirection = ((Component)this).transform.rotation * val;
			motor.inputJump = Input.GetButton("Jump");
		}
	}

	public override void Main()
	{
	}
}
