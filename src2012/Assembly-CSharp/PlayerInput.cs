using UnityEngine;

public class PlayerInput : MonoBehaviour
{
	private PlayerMotor _Motor;

	private CharacterController _Controller;

	private SecuredValue<bool> _EnableMovement;

	private SecuredValue<bool> _UnderWater;

	private SecuredValue<float> _WaterTime;

	private SecuredValue<int> _WaterSpeed;

	private SecuredValue<float> _WaterVelocity;

	private SecuredValue<bool> _Ladder;

	private SecuredValue<int> _LaderSpeed;

	private SecuredValue<bool> _OnIce;

	private SecuredValue<bool> _DirectMovement;

	private SecuredValue<Vector3> _CurVelocity;

	private void Awake()
	{
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		_Motor = ((Component)this).GetComponent<PlayerMotor>();
		_Controller = ((Component)this).GetComponent<CharacterController>();
		_EnableMovement = true;
		_UnderWater = false;
		_WaterTime = 0f;
		_WaterSpeed = 3;
		_WaterVelocity = 0f;
		_Ladder = false;
		_LaderSpeed = 4;
		_OnIce = false;
		_DirectMovement = false;
		_CurVelocity = Vector3.zero;
	}

	private void SetDirectMovement(bool enabled)
	{
		_DirectMovement = enabled;
	}

	public void SetMovement(bool move)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		_EnableMovement = move;
		if (!move)
		{
			_Motor.InputMoveDirection = new Vector3(0f, 0f, 0f);
			_Motor.InputJump = false;
		}
	}

	private void SetSpeedEffect(bool enable)
	{
		if (enable)
		{
			_Motor.Movement.MaxForwardSpeed = 13f;
			_Motor.Movement.MaxSidewaysSpeed = 13f;
			_Motor.Movement.MaxBackwardsSpeed = 13f;
			_WaterSpeed = 8;
			_LaderSpeed = 12;
		}
		else
		{
			_Motor.Movement.MaxForwardSpeed = 5f;
			_Motor.Movement.MaxSidewaysSpeed = 5f;
			_Motor.Movement.MaxBackwardsSpeed = 5f;
			_WaterSpeed = 3;
			_LaderSpeed = 5;
		}
	}

	private void SetIceBody(bool ice)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (ice && !_OnIce)
		{
			_CurVelocity = _Motor.Movement.Velocity * 0.2f;
		}
		_OnIce = ice;
	}

	private void SetUnderWaterBody(bool[] water_data)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (water_data[0])
		{
			if (!_UnderWater)
			{
				_Motor.Movement.Gravity = 0f;
				_Motor.Movement.Velocity = Vector3.zero;
				_Motor.Movement.FrameVelocity = Vector3.zero;
				_Motor.InputMoveDirection = Vector3.zero;
				_UnderWater = water_data[0];
				_WaterTime = Time.time + 1f;
			}
			else if (water_data[1])
			{
				_WaterTime = Time.time + 0.5f;
			}
		}
		else if (!water_data[0] && (bool)_UnderWater && !_Ladder)
		{
			_Motor.Movement.Gravity = 40f;
			_UnderWater = water_data[0];
		}
	}

	private void SetLadderBody(bool _ladder)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		_Ladder = _ladder;
		if ((bool)_Ladder)
		{
			_UnderWater = false;
			_Motor.Movement.Gravity = 0f;
			_Motor.Movement.Velocity = Vector3.zero;
			_Motor.Movement.FrameVelocity = Vector3.zero;
			_Motor.InputMoveDirection = Vector3.zero;
		}
		else
		{
			_Motor.Movement.Gravity = 40f;
		}
	}

	private void Update()
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		if (!_EnableMovement)
		{
			return;
		}
		Vector3 val = (_DirectMovement ? new Vector3(0f, 0f, Mathf.Max(Mathf.Abs(Input.GetAxis("Horizontal")), Mathf.Abs(Input.GetAxis("Vertical")))) : new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")));
		if (val != Vector3.zero)
		{
			float magnitude = val.magnitude;
			val /= magnitude;
			magnitude = Mathf.Min(1f, magnitude);
			magnitude *= magnitude;
			val *= magnitude;
		}
		float num = 0f;
		if ((bool)_Ladder)
		{
			val.x *= (float)(int)_LaderSpeed;
			val.z *= (float)(int)_LaderSpeed;
			if (Input.GetKey("space"))
			{
				num = 1f;
			}
			if (Input.GetKey("left shift"))
			{
				num = -1f;
			}
			val.y = (float)(int)_LaderSpeed * num;
			_Motor.InputJump = Input.GetButton("Jump");
			_Controller.Move(((Component)this).transform.rotation * val * Time.deltaTime);
		}
		else if ((bool)_UnderWater)
		{
			_Motor.Movement.Gravity = 0f;
			val.x *= (float)(int)_WaterSpeed;
			val.z *= (float)(int)_WaterSpeed;
			if (Input.GetKey("space") && Time.time >= (float)_WaterTime)
			{
				_WaterVelocity = Mathf.MoveTowards((float)_WaterVelocity, 1f, Time.deltaTime * 15f);
				if ((float)_WaterVelocity > 0f)
				{
					num = _WaterVelocity;
				}
			}
			else
			{
				_WaterVelocity = Mathf.MoveTowards((float)_WaterVelocity, -1f, Time.deltaTime * 15f);
			}
			if (Input.GetKey("left shift"))
			{
				num = -1f;
			}
			val.y = (float)(int)_WaterSpeed * num;
			_Motor.InputJump = Input.GetButton("Jump");
			if ((float)_WaterVelocity < 0f)
			{
				val.y += (float)_WaterVelocity;
			}
			_Controller.Move(((Component)this).transform.rotation * val * Time.deltaTime);
		}
		else if ((bool)_OnIce)
		{
			Vector3 val2 = ((Component)this).transform.TransformDirection(val);
			_CurVelocity = Vector3.Lerp((Vector3)_CurVelocity, val2, 1.25f * Time.deltaTime);
			_Motor.InputMoveDirection = _CurVelocity;
			_Motor.InputJump = Input.GetButton("Jump");
		}
		else
		{
			_Motor.InputMoveDirection = ((Component)this).transform.rotation * val;
			_Motor.InputJump = Input.GetButton("Jump");
		}
	}
}
