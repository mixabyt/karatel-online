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

	private SecuredValue<bool> _SpeedUp;

	private SecuredValue<float> _Modifier;

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
		_SpeedUp = false;
		_Modifier = 1f;
	}

	private void SetDirectMovement(bool enabled)
	{
		_DirectMovement = enabled;
	}

	public void SetMovement(bool move)
	{
		_EnableMovement = move;
		if (!move)
		{
			StopMovement();
		}
	}

	public void StopMovement()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		_Motor.InputMoveDirection = new Vector3(0f, 0f, 0f);
		_Motor.InputJump = false;
	}

	public void SetSpeedUp(bool speedUp)
	{
		_SpeedUp = speedUp;
		if ((bool)_SpeedUp)
		{
			_Motor.Movement.MaxForwardSpeed = 13f * (float)_Modifier;
			_Motor.Movement.MaxSidewaysSpeed = 13f * (float)_Modifier;
			_Motor.Movement.MaxBackwardsSpeed = 13f * (float)_Modifier;
			_WaterSpeed = Mathf.RoundToInt(8f * (float)_Modifier);
			_LaderSpeed = Mathf.RoundToInt(12f * (float)_Modifier);
		}
		else
		{
			_Motor.Movement.MaxForwardSpeed = 5f * (float)_Modifier;
			_Motor.Movement.MaxSidewaysSpeed = 5f * (float)_Modifier;
			_Motor.Movement.MaxBackwardsSpeed = 5f * (float)_Modifier;
			_WaterSpeed = Mathf.RoundToInt(3f * (float)_Modifier);
			_LaderSpeed = Mathf.RoundToInt(5f * (float)_Modifier);
		}
	}

	public void SetSpeedModifier(float modifier)
	{
		_Modifier = modifier;
		SetSpeedUp(_SpeedUp);
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
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		if (!_EnableMovement || Chat.IsEnabled())
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
