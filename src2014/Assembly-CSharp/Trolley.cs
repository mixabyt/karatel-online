using System.Collections.Generic;
using UnityEngine;

public class Trolley : EntityBase
{
	public const float MaxSpeed = 12f;

	public const float InitialPushSpeed = 3.3f;

	public const float AccelerateSpeed = 1.5f;

	public const float BreakSpeed = 2.5f;

	public const float FrictionSpeed = 0.3f;

	public const float DownhillSpeed = 1f;

	public const float UpSlowDownSpeed = 1.5f;

	public const float ItemsCrashSpeed = 6f;

	public const float FreeFallDisruptionSpeed = 1f;

	public const float FreeFallMultiplierSpeed = 1f;

	public const float FreeFallCeilingSpeed = 1f;

	private static List<Trolley> _AttachedTrolleys = new List<Trolley>();

	private static List<Trolley> _DetachedTrolleys = new List<Trolley>();

	public Transform Model;

	public Collider MainCollider;

	public Transform PlayerPosition;

	public BoxCollider RailConnectCollider;

	public Transform[] Wheels;

	private float _CurSpeed;

	private float _MoveDir = 1f;

	private Rail _CurRail;

	private Rail _CurRailExt;

	private Rail _NextRail;

	private bool _ReverseMove;

	private Transform _PlayerCamera;

	private Vector3 _PlayerCameraOffset = Vector3.zero;

	private bool _PlayerCameraThirdPerson;

	private bool _PlayerCameraFreeLook;

	private MouseLook _MouseLookHor;

	private MouseLook _MouseLookVer;

	private float _RotationSpeedFactor = 1f;

	private Vector3 _CurRotation = Vector3.zero;

	private bool _FreeFall;

	private float _FreeFallTime;

	private Rail _FreeFallRail;

	private Vector3 _CameraOffsetPosition = Vector3.up;

	private float _CameraOffsetRotationX;

	private int _LastPlaneIndex;

	private GameObject _Player;

	private Collider _IgnoredCollider;

	private Vector3 _Position = Vector3.zero;

	private Quaternion _Rotation = Quaternion.identity;

	private Vector3 _PositionModel = Vector3.zero;

	private float _WheelsSpeed;

	public GameObject Player => _Player;

	public Trolley()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
	}

	public void OnManualMove()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Stop();
		StartFreeFall(Vector3.down, 1f);
	}

	public void TryAttach(Rail toRail = null, Collider toCollider = null)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)_CurRail != (Object)null))
		{
			if ((Object)(object)toRail != (Object)null)
			{
				_CurRail = toRail;
			}
			else if ((Object)(object)toCollider != (Object)null)
			{
				_CurRail = Rail.Find(toCollider);
			}
			else
			{
				_CurRail = Rail.Find(BlockX, BlockY, (int)(((Component)this).transform.position.y + 0.2f));
			}
			_CurRailExt = null;
			_NextRail = null;
			RefreshRotation();
			_DetachedTrolleys.Remove(this);
			if ((Object)(object)_CurRail != (Object)null)
			{
				_AttachedTrolleys.Add(this);
			}
			else
			{
				_DetachedTrolleys.Add(this);
			}
		}
	}

	public static void OnRailAttached(Rail rail)
	{
		int blockX = rail.BlockX;
		int blockY = rail.BlockY;
		int blockZ = rail.BlockZ;
		Trolley[] array = _DetachedTrolleys.ToArray();
		foreach (Trolley trolley in array)
		{
			if (!((Object)(object)trolley == (Object)null) && !((Object)(object)trolley.photonView == (Object)null) && trolley.photonView.isMine && !trolley._FreeFall && trolley.BlockX == blockX && trolley.BlockY == blockY && trolley.BlockZ == blockZ)
			{
				_DetachedTrolleys.Remove(trolley);
				_AttachedTrolleys.Add(trolley);
				trolley._CurRail = rail;
			}
		}
	}

	public static void OnRailDetached(Rail rail)
	{
		Trolley[] array = _AttachedTrolleys.ToArray();
		foreach (Trolley trolley in array)
		{
			if (!((Object)(object)trolley == (Object)null) && !((Object)(object)trolley.photonView == (Object)null) && trolley.photonView.isMine && ((Object)(object)trolley._CurRail == (Object)(object)rail || (Object)(object)trolley._CurRailExt == (Object)(object)rail || (Object)(object)trolley._NextRail == (Object)(object)rail))
			{
				trolley.Stop();
				trolley._CurRail = null;
				trolley._CurRailExt = null;
				_AttachedTrolleys.Remove(trolley);
				_DetachedTrolleys.Add(trolley);
			}
		}
	}

	public static void KickPlayer(GameObject player)
	{
		if ((Object)(object)player == (Object)null)
		{
			return;
		}
		bool flag = false;
		foreach (Trolley attachedTrolley in _AttachedTrolleys)
		{
			if ((Object)(object)attachedTrolley._Player == (Object)(object)player)
			{
				attachedTrolley.PlayerExit();
				flag = true;
				break;
			}
		}
		if (flag)
		{
			return;
		}
		foreach (Trolley detachedTrolley in _DetachedTrolleys)
		{
			if ((Object)(object)detachedTrolley._Player == (Object)(object)player)
			{
				detachedTrolley.PlayerExit();
				break;
			}
		}
	}

	protected override void Creation(object[] data)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		_Position = ((Component)this).transform.position;
		_Rotation = ((Component)this).transform.rotation;
		TryAttach();
	}

	protected override void Destruction()
	{
		_AttachedTrolleys.Remove(this);
		_DetachedTrolleys.Remove(this);
		_CurRail = null;
		_CurRailExt = null;
		_NextRail = null;
		if ((Object)(object)_Player != (Object)null)
		{
			PlayerExit();
		}
	}

	private void Push(Vector3 direction, float speed)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)_CurRail == (Object)null)
		{
			return;
		}
		if ((Object)(object)_NextRail != (Object)null)
		{
			Rail next = _CurRail.GetNext(direction, 0f);
			if ((Object)(object)next == (Object)(object)_NextRail)
			{
				if (_CurSpeed < speed)
				{
					_CurSpeed = speed;
				}
				return;
			}
			Stop();
		}
		_NextRail = _CurRail.GetNext(direction, 0f);
		if ((Object)(object)_NextRail != (Object)null)
		{
			if ((Object)(object)_CurRailExt != (Object)null && (Object)(object)_CurRailExt != (Object)(object)_NextRail)
			{
				_NextRail = _CurRail;
				_CurRail = _CurRailExt;
			}
			_CurRailExt = null;
			_CurSpeed = speed;
			Vector3 val = -((Component)this).transform.forward;
			Vector3 val2 = ((Component)_NextRail).transform.position - ((Component)_CurRail).transform.position;
			if (Vector3.Dot(val, val2.normalized) < 0f)
			{
				_MoveDir = -1f;
			}
			else
			{
				_MoveDir = 1f;
			}
			RefreshRotation();
		}
	}

	private void Stop()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)_NextRail == (Object)null))
		{
			if (Vector3.Distance(((Component)this).transform.position, ((Component)_NextRail).transform.position) < Vector3.Distance(((Component)this).transform.position, ((Component)_CurRail).transform.position))
			{
				Rail curRail = _CurRail;
				_CurRail = _NextRail;
				_NextRail = curRail;
			}
			_CurRailExt = ((!(((Component)_CurRail).transform.position != ((Component)_NextRail).transform.position)) ? null : _NextRail);
			_CurRailExt = _NextRail;
			_NextRail = null;
		}
	}

	private void RefreshRotation()
	{
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07de: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ef: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)_NextRail == (Object)null)
		{
			return;
		}
		float num = _CurRail.GetRotationToRail(_NextRail) - ((!(_MoveDir < 0f)) ? 0f : 180f);
		if (_CurRotation.y != num)
		{
			if (Mathf.Abs(Mathf.DeltaAngle(_CurRotation.y, num)) < 100f)
			{
				_CurRotation.y = Mathf.MoveTowardsAngle(_CurRotation.y, num, 720f * Time.deltaTime);
			}
			else
			{
				_CurRotation.y = num;
			}
		}
		((Component)Model).transform.localPosition = Vector3.zero;
		_RotationSpeedFactor = 1f;
		int blockX = _CurRail.BlockX;
		int blockY = _CurRail.BlockY;
		int blockZ = _CurRail.BlockZ;
		Rail.SetPlaneBlock(_CurRail.PlaneIndex, ref blockX, ref blockY, ref blockZ);
		int blockX2 = _NextRail.BlockX;
		int blockY2 = _NextRail.BlockY;
		int blockZ2 = _NextRail.BlockZ;
		Rail.SetPlaneBlock(_CurRail.PlaneIndex, ref blockX2, ref blockY2, ref blockZ2);
		if (_CurRail.IsTransition && _NextRail.IsTransition)
		{
			_CurRotation.x = ((blockZ2 <= blockZ) ? (-45f) : 45f);
			Model.Translate(0f, 0.35f, 0f, (Space)1);
		}
		else if (_CurRail.IsTransition || _NextRail.IsTransition)
		{
			float num2 = 1f - Vector3.Distance(((Component)_NextRail).transform.position, ((Component)this).transform.position) / Vector3.Distance(((Component)_CurRail).transform.position, ((Component)_NextRail).transform.position);
			num2 = Mathf.Clamp(num2, 0f, 1f);
			if (blockZ2 > blockZ || (_NextRail.IsTransition && blockZ == blockZ2) || (!_NextRail.IsTransition && blockZ2 < blockZ))
			{
				if (!_CurRail.IsTransition)
				{
					if (_CurRail.PlaneIndex == _NextRail.PlaneIndex)
					{
						_CurRotation.x = 45f * num2;
						Model.Translate(0f, 0.35f * num2, 0f, (Space)1);
					}
					else if (num2 < 0.5f)
					{
						num2 *= 2f;
						_CurRotation.x = 45f * num2;
						Model.Translate(0f, 0f, 0f, (Space)1);
					}
					else
					{
						num2 = (num2 - 0.5f) * 2f;
						_CurRotation.x = 45f;
						Model.Translate(0f, 0.35f * num2, 0f, (Space)1);
					}
				}
				else if (!_NextRail.IsTransition && blockZ2 < blockZ)
				{
					num2 = 1f - num2;
					if (num2 < 0.5f)
					{
						num2 = 1f - num2 * 2f;
						_CurRotation.x = 0f;
						Model.Translate(0f, 0.2f - 0.2f * num2, 0f, (Space)1);
					}
					else if (num2 < 0.75f)
					{
						_RotationSpeedFactor = 0.5f;
						num2 = (num2 - 0.5f) * 4f;
						_CurRotation.x = 45f * num2;
						Model.Translate(0f, 0.35f - 0.15f * (1f - num2), 0f, (Space)1);
					}
					else
					{
						_CurRotation.x = 45f;
						Model.Translate(0f, 0.35f, 0f, (Space)1);
					}
					ref Vector3 curRotation = ref _CurRotation;
					curRotation.x -= 90f;
				}
				else if (blockX - blockX2 != 0 || blockY - blockY2 != 0)
				{
					num2 = 1f - num2;
					if (num2 < 0.5f)
					{
						_CurRotation.x = 0f;
						Model.Translate(0f, 0.5f * num2 * 2f, 0f, (Space)1);
					}
					else if (num2 < 0.75f)
					{
						_RotationSpeedFactor = 0.5f;
						num2 = (num2 - 0.5f) * 4f;
						_CurRotation.x = 45f * num2;
						Model.Translate(0f, 0.5f - 0.15f * num2, 0f, (Space)1);
					}
					else
					{
						_CurRotation.x = 45f;
						Model.Translate(0f, 0.35f, 0f, (Space)1);
					}
				}
				else if (num2 < 0.5f)
				{
					_CurRotation.x = 45f;
					Model.Translate(0f, 0.35f - 0.35f * num2 * 2f, 0f, (Space)1);
				}
				else
				{
					num2 = (num2 - 0.5f) * 2f;
					_CurRotation.x = 45f + 45f * num2;
				}
			}
			else if (!_CurRail.IsTransition)
			{
				if (num2 < 0.5f)
				{
					_CurRotation.x = 0f;
					if (_CurRail.PlaneIndex == _NextRail.PlaneIndex)
					{
						Model.Translate(0f, 0.5f * num2 * 2f, 0f, (Space)1);
					}
					else
					{
						Model.Translate(0f, 0.2f * num2 * 2f, 0f, (Space)1);
					}
				}
				else if (num2 < 0.75f)
				{
					_RotationSpeedFactor = 0.75f;
					num2 = (num2 - 0.5f) * 4f;
					_CurRotation.x = 360f - 45f * num2;
					if (_CurRail.PlaneIndex == _NextRail.PlaneIndex)
					{
						Model.Translate(0f, 0.5f - 0.15f * num2, 0f, (Space)1);
					}
					else
					{
						Model.Translate(0f, 0.2f + 0.15f * num2, 0f, (Space)1);
					}
				}
				else
				{
					_CurRotation.x = -45f;
					Model.Translate(0f, 0.35f, 0f, (Space)1);
				}
			}
			else
			{
				num2 = 1f - num2;
				_CurRotation.x = -45f * num2;
				Model.Translate(0f, 0.35f * num2, 0f, (Space)1);
			}
		}
		else
		{
			_CurRotation.x = 0f;
		}
		ref Vector3 curRotation2 = ref _CurRotation;
		curRotation2.x *= _MoveDir;
		((Component)this).transform.rotation = Quaternion.Euler(Rail.PlaneRotation[_CurRail.PlaneIndex]);
		((Component)this).transform.Rotate(_CurRotation, (Space)1);
		if ((Object)(object)_Player != (Object)null)
		{
			PlayerFixPosition();
		}
	}

	protected override void Updating()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0525: Unknown result type (might be due to invalid IL or missing references)
		//IL_0535: Unknown result type (might be due to invalid IL or missing references)
		//IL_053a: Unknown result type (might be due to invalid IL or missing references)
		//IL_053f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_0548: Unknown result type (might be due to invalid IL or missing references)
		//IL_0551: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0631: Unknown result type (might be due to invalid IL or missing references)
		//IL_0641: Unknown result type (might be due to invalid IL or missing references)
		//IL_0659: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0743: Unknown result type (might be due to invalid IL or missing references)
		//IL_0753: Unknown result type (might be due to invalid IL or missing references)
		//IL_0758: Unknown result type (might be due to invalid IL or missing references)
		//IL_075d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0761: Unknown result type (might be due to invalid IL or missing references)
		//IL_0766: Unknown result type (might be due to invalid IL or missing references)
		//IL_077b: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ab: Unknown result type (might be due to invalid IL or missing references)
		if (!photonView.isMine)
		{
			((Component)this).transform.position = Vector3.Lerp(((Component)this).transform.position, _Position, Time.deltaTime * 5f);
			((Component)this).transform.rotation = Quaternion.Lerp(((Component)this).transform.rotation, _Rotation, Time.deltaTime * 5f);
			((Component)Model).transform.localPosition = Vector3.Lerp(((Component)Model).transform.localPosition, _PositionModel, Time.deltaTime * 5f);
			if ((Object)(object)_Player != (Object)null)
			{
				PlayerFixPosition();
			}
			Transform[] wheels = Wheels;
			foreach (Transform val in wheels)
			{
				val.Rotate(0f, _WheelsSpeed * Time.deltaTime, 0f, (Space)1);
			}
			return;
		}
		if (_FreeFall)
		{
			if ((Object)(object)_Player != (Object)null)
			{
				PlayerFixPosition();
			}
			if (((Component)this).rigidbody.velocity == Vector3.zero)
			{
				StopFreeFall();
				TryAttach(null, (Collider)(object)RailConnectCollider);
			}
			FixWorldBounds();
			return;
		}
		if ((Object)(object)_CurRail != (Object)null)
		{
			Debug.DrawLine(((Component)this).transform.position + Vector3.up, ((Component)_CurRail).transform.position, Color.red);
		}
		if ((Object)(object)_CurRailExt != (Object)null)
		{
			Debug.DrawLine(((Component)this).transform.position + Vector3.up, ((Component)_CurRailExt).transform.position, Color.green);
		}
		if ((Object)(object)_NextRail != (Object)null)
		{
			Debug.DrawLine(((Component)this).transform.position + Vector3.up, ((Component)_NextRail).transform.position, Color.blue);
		}
		if ((Object)(object)_Player != (Object)null && (Object)(object)_Player == (Object)(object)WorldGameObject.Instance.MainPlayer && (Object)(object)_CurRail != (Object)null)
		{
			if ((Object)(object)_NextRail != (Object)null)
			{
				bool flag = ((!_ReverseMove) ? (-1f) : 1f) * _MoveDir > 0f;
				if ((flag && Input.GetAxis("Vertical") > 0f) || (!flag && Input.GetAxis("Vertical") < 0f))
				{
					_CurSpeed = Mathf.MoveTowards(_CurSpeed, 0f, -1.5f * Time.deltaTime);
				}
				if ((flag && Input.GetAxis("Vertical") < 0f) || (!flag && Input.GetAxis("Vertical") > 0f))
				{
					_CurSpeed = Mathf.MoveTowards(_CurSpeed, 0f, 2.5f * Time.deltaTime);
				}
			}
			if ((Object)(object)_NextRail == (Object)null || _CurSpeed == 0f)
			{
				if (Input.GetAxis("Vertical") > 0f)
				{
					Push(((Component)this).transform.forward * ((!_ReverseMove) ? 1f : (-1f)), 1f);
				}
				else if (Input.GetAxis("Vertical") < 0f)
				{
					Push(-((Component)this).transform.forward * ((!_ReverseMove) ? 1f : (-1f)), 1f);
				}
			}
		}
		if (!((Object)(object)_NextRail != (Object)null))
		{
			return;
		}
		Quaternion rotation = ((Component)this).transform.rotation;
		float num = rotation.eulerAngles.x;
		if (num != 0f)
		{
			if (_MoveDir < 0f)
			{
				num = 360f - num;
			}
			if (num < 180f)
			{
				_CurSpeed = Mathf.MoveTowards(_CurSpeed, 0f, Mathf.Max(num / 90f * 1.5f, 0.3f) * Time.deltaTime);
			}
			else
			{
				_CurSpeed = Mathf.MoveTowards(_CurSpeed, 0f, (0f - (360f - num) / 90f) * 1f * Time.deltaTime);
			}
		}
		else
		{
			_CurSpeed = Mathf.MoveTowards(_CurSpeed, 0f, 0.3f * Time.deltaTime);
		}
		if (_CurSpeed <= 1f && (_CurRail.PlaneIndex == 1 || ((Object)(object)_CurRailExt != (Object)null && _CurRailExt.PlaneIndex == 1)))
		{
			Vector3 val2 = ((Component)_NextRail).transform.position - ((Component)_CurRail).transform.position;
			Vector3 normalized = val2.normalized;
			Stop();
			StartFreeFall(normalized, _CurSpeed * 1f);
		}
		else if (_CurSpeed == 0f)
		{
			Stop();
			Rail down = _CurRail.GetDown();
			if ((Object)(object)down == (Object)null && (Object)(object)_CurRailExt != (Object)null)
			{
				down = _CurRailExt.GetDown();
			}
			if ((Object)(object)down != (Object)null)
			{
				Vector3 val3 = ((Component)down).transform.position - ((Component)_CurRail).transform.position;
				Push(val3.normalized, 0.01f);
			}
		}
		if (!((Object)(object)_NextRail != (Object)null))
		{
			return;
		}
		if (_CurSpeed > 12f)
		{
			_CurSpeed = 12f;
		}
		((Component)this).transform.position = Vector3.MoveTowards(((Component)this).transform.position, ((Component)_NextRail).transform.position, _CurSpeed * Time.deltaTime * _RotationSpeedFactor);
		Transform[] wheels2 = Wheels;
		foreach (Transform val4 in wheels2)
		{
			val4.Rotate(0f, -220f * _CurSpeed * _MoveDir * Time.deltaTime, 0f, (Space)1);
		}
		if (((Component)this).transform.position == ((Component)_NextRail).transform.position)
		{
			float closestAngle = 0f;
			if ((Object)(object)_Player != (Object)null && (Object)(object)_Player == (Object)(object)WorldGameObject.Instance.MainPlayer)
			{
				if (Input.GetKey((KeyCode)97))
				{
					closestAngle = 90f;
				}
				else if (Input.GetKey((KeyCode)100))
				{
					closestAngle = 270f;
				}
			}
			Vector3 val5 = ((Component)_NextRail).transform.position - ((Component)_CurRail).transform.position;
			Vector3 normalized2 = val5.normalized;
			_CurRail = _NextRail;
			_NextRail = _NextRail.GetNext(normalized2, closestAngle);
			if ((Object)(object)_NextRail == (Object)null && _CurSpeed >= 1f)
			{
				StartFreeFall(normalized2, _CurSpeed * 1f);
			}
		}
		RefreshRotation();
		FixWorldBounds();
	}

	protected override void PreviewUpdating()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = (Vector3)((!((Object)(object)Rail.Find(BlockX, BlockY, BlockZ) != (Object)null)) ? new Vector3(0f, -0.1f, 0f) : Vector3.zero);
		Model.localPosition = Vector3.MoveTowards(Model.localPosition, val, Time.deltaTime * 0.5f);
		((Component)this).transform.localRotation = Quaternion.RotateTowards(((Component)this).transform.localRotation, Quaternion.identity, Time.deltaTime * 360f);
	}

	private void OnTriggerEnter(Collider other)
	{
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		if (IsPreview || !photonView.isMine)
		{
			return;
		}
		if ((Object)(object)_CurRail == (Object)null)
		{
			if (_FreeFall)
			{
				EntityBase entityBase = EntityBase.FindEntity(((Component)other).gameObject);
				if (entityBase is Rail && ((Object)(object)entityBase != (Object)(object)_FreeFallRail || _FreeFallTime + 2f < Time.time))
				{
					Vector3 velocity = ((Component)this).rigidbody.velocity;
					StopFreeFall();
					TryAttach(entityBase as Rail);
					Push(velocity.normalized, velocity.magnitude / 2f);
				}
			}
			return;
		}
		CharacterController component = ((Component)other).gameObject.GetComponent<CharacterController>();
		if ((Object)(object)component != (Object)null)
		{
			Vector3 val = ((Component)this).transform.position - ((Component)component).transform.position;
			Push(val.normalized, 3.3f);
			return;
		}
		EntityBase entityBase2 = EntityBase.FindEntity(((Component)other).gameObject);
		if (!((Object)(object)entityBase2 != (Object)null) || entityBase2 is Rail)
		{
			return;
		}
		if (entityBase2 is Trolley)
		{
			Trolley trolley = entityBase2 as Trolley;
			Vector3 val2 = ((Component)this).transform.position - ((Component)trolley).transform.position;
			Vector3 normalized = val2.normalized;
			Push(normalized, 1f);
		}
		else if (_CurSpeed < 6f)
		{
			float speed = _CurSpeed / 2f;
			Vector3 val4;
			if ((Object)(object)_NextRail != (Object)null)
			{
				Vector3 val3 = ((Component)_NextRail).transform.position - ((Component)_CurRail).transform.position;
				val4 = val3.normalized;
			}
			else
			{
				val4 = ((Component)this).transform.forward;
			}
			Vector3 val5 = val4;
			Stop();
			Push(-val5, speed);
		}
		else
		{
			entityBase2.SelfDelete();
		}
	}

	public override void OnButtonF(string playerName)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)_CurRail == (Object)null && (((Component)this).transform.eulerAngles.x != 0f || ((Component)this).transform.eulerAngles.z != 0f))
		{
			return;
		}
		GameObject mainPlayer = WorldGameObject.Instance.MainPlayer;
		if ((Object)(object)_Player != (Object)null)
		{
			if (!((Object)(object)_Player != (Object)(object)mainPlayer))
			{
				PlayerExit();
			}
			return;
		}
		KickPlayer(mainPlayer);
		if ((Object)(object)_CurRail == (Object)null)
		{
			TryAttach();
		}
		PlayerEnter(mainPlayer);
	}

	protected void PlayerEnter(GameObject player, bool others = false)
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Expected Obj, but got Unknown
		_Player = player;
		_Player.GetComponent<PlayerNetwork>().InVehicle = true;
		_Player.GetComponent<PlayerNetwork>().InTrolley = true;
		if ((Object)(object)_Player == (Object)(object)WorldGameObject.Instance.MainPlayer)
		{
			_PlayerCamera = _Player.transform.Find("recoil/Main Camera");
			_PlayerCameraOffset = ((Component)_PlayerCamera).transform.localPosition;
			_MouseLookHor = _Player.GetComponent<MouseLook>();
			_MouseLookVer = ((Component)_PlayerCamera).GetComponent<MouseLook>();
			CameraController component = _Player.GetComponent<CameraController>();
			_PlayerCameraThirdPerson = component.IsThirdPerson;
			_PlayerCameraFreeLook = component.IsFreeLook;
			if (!component.IsThirdPerson || !component.IsFreeLook)
			{
				component.SetThirdPerson(freeLook: true);
			}
			component.DisableCameraSwitching = true;
			((Component)_MouseLookHor).transform.rotation = ((Component)Model).transform.rotation;
			_MouseLookHor.OriginalRotation = ((Component)_MouseLookHor).transform.localRotation;
			_ReverseMove = Vector3.Dot(((Component)CameraController.RaycastCamera).transform.forward, ((Component)this).transform.forward) < 0f;
			_MouseLookHor.RotationX = (_ReverseMove ? 180 : 0);
		}
		if ((Object)(object)_Player.GetComponent<CharacterController>() != (Object)null)
		{
			((Collider)_Player.GetComponent<CharacterController>()).enabled = false;
		}
		if ((Object)(object)_Player.GetComponent<PlayerMotor>() != (Object)null)
		{
			((Behaviour)_Player.GetComponent<PlayerMotor>()).enabled = false;
		}
		if ((Object)(object)_Player.GetComponent<PlayerInput>() != (Object)null)
		{
			_Player.GetComponent<PlayerInput>().SetMovement(move: false);
		}
		PlayerFixPosition();
		if ((Object)(object)_Player == (Object)(object)WorldGameObject.Instance.MainPlayer)
		{
			GameObject val = new GameObject("trolley_player");
			TrolleyPlayer trolleyPlayer = val.AddComponent<TrolleyPlayer>();
			trolleyPlayer.Initialize(this, _Player);
		}
		if (!others)
		{
			photonView.isForcedSending = 1;
			photonView.RPC("PlayerEnterOthers", PhotonTargets.Others, PhotonNetwork.player);
		}
	}

	[RPC]
	protected void PlayerEnterOthers(PhotonPlayer owner)
	{
		photonView.isForcedSending = -1;
		Stop();
		if ((Object)(object)_CurRail != (Object)null)
		{
			_AttachedTrolleys.Remove(this);
		}
		else
		{
			_DetachedTrolleys.Remove(this);
		}
		_CurRail = null;
		_CurRailExt = null;
		_NextRail = null;
		PlayerEnter(WorldGameObject.Instance.FindPlayer(owner).Avatar, others: true);
	}

	public void PlayerExit(bool others = false)
	{
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)_Player.GetComponent<CharacterController>() != (Object)null)
		{
			((Collider)_Player.GetComponent<CharacterController>()).enabled = true;
		}
		if ((Object)(object)_Player.GetComponent<PlayerMotor>() != (Object)null)
		{
			((Behaviour)_Player.GetComponent<PlayerMotor>()).enabled = true;
		}
		if ((Object)(object)_Player.GetComponent<PlayerInput>() != (Object)null)
		{
			_Player.GetComponent<PlayerInput>().SetMovement(move: true);
		}
		if ((Object)(object)_Player == (Object)(object)WorldGameObject.Instance.MainPlayer)
		{
			_MouseLookHor.invertX = false;
			_MouseLookHor.OriginalRotation = Quaternion.identity;
			Vector3 val = ((Component)this).transform.forward * ((!_ReverseMove) ? 1f : (-1f));
			val = new Vector3(val.x, 0f, val.z);
			MouseLook mouseLookHor = _MouseLookHor;
			Quaternion val2 = Quaternion.LookRotation(val.normalized, Vector3.up);
			mouseLookHor.RotationX = val2.eulerAngles.y;
			if ((Object)(object)_PlayerCamera != (Object)null)
			{
				_MouseLookVer.OriginalRotation = Quaternion.identity;
				((Component)_PlayerCamera).transform.localPosition = _PlayerCameraOffset;
			}
			_PlayerCamera = null;
			_MouseLookHor = null;
			_MouseLookVer = null;
			CameraController component = _Player.GetComponent<CameraController>();
			if (!_PlayerCameraThirdPerson)
			{
				component.SetFirstPerson();
			}
			else if (!_PlayerCameraFreeLook)
			{
				component.SetThirdPerson(freeLook: false);
			}
			component.DisableCameraSwitching = false;
			Object.Destroy((Object)(object)((Component)((Component)Model).GetComponentInChildren<TrolleyPlayer>()).gameObject);
		}
		if ((Object)(object)_IgnoredCollider != (Object)null)
		{
			Physics.IgnoreCollision(MainCollider, _IgnoredCollider, false);
			_IgnoredCollider = null;
		}
		Transform transform = _Player.transform;
		transform.position += ((Component)this).transform.up * 1.2f;
		if (_FreeFall)
		{
			Physics.IgnoreCollision(MainCollider, _Player.collider, true);
			_IgnoredCollider = _Player.collider;
		}
		_Player.GetComponent<PlayerNetwork>().InVehicle = false;
		_Player.GetComponent<PlayerNetwork>().InTrolley = false;
		_Player = null;
		if (!others)
		{
			photonView.RPC("PlayerExitOthers", PhotonTargets.Others);
		}
	}

	[RPC]
	protected void PlayerExitOthers()
	{
		PlayerExit(others: true);
	}

	protected virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (stream.isWriting)
		{
			stream.SendNext(((Component)this).transform.position);
			stream.SendNext(((Component)this).transform.rotation);
			stream.SendNext(((Component)Model).transform.localPosition);
			stream.SendNext(-220f * _CurSpeed * _MoveDir);
		}
		else
		{
			_Position = (Vector3)stream.ReceiveNext();
			_Rotation = (Quaternion)stream.ReceiveNext();
			_PositionModel = (Vector3)stream.ReceiveNext();
			_WheelsSpeed = (float)stream.ReceiveNext();
		}
	}

	private void PlayerFixPosition()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		_Player.transform.position = PlayerPosition.position;
		if (photonView.isMine && (Object)(object)_Player == (Object)(object)WorldGameObject.Instance.MainPlayer)
		{
			if (CameraController.Instance.IsThirdPerson)
			{
				((Component)_MouseLookHor).transform.rotation = ((Component)Model).transform.rotation;
				_MouseLookHor.OriginalRotation = ((Component)_MouseLookHor).transform.localRotation;
			}
			else if (_MouseLookHor.OriginalRotation != Quaternion.identity)
			{
				_MouseLookHor.OriginalRotation = Quaternion.identity;
			}
			int num = ((!((Object)(object)_CurRail != (Object)null)) ? _LastPlaneIndex : _CurRail.PlaneIndex);
			if ((Object)(object)_CurRailExt != (Object)null && _CurRailExt.PlaneIndex < num)
			{
				num = _CurRailExt.PlaneIndex;
			}
			if ((Object)(object)_NextRail != (Object)null && _NextRail.PlaneIndex < num)
			{
				num = _NextRail.PlaneIndex;
			}
			_LastPlaneIndex = num;
			Vector3 val = num switch
			{
				1 => Vector3.down, 
				2 => Vector3.right, 
				3 => Vector3.left, 
				4 => Vector3.forward, 
				5 => Vector3.back, 
				_ => Vector3.up, 
			};
			_CameraOffsetPosition = Vector3.Lerp(_CameraOffsetPosition, val, Time.deltaTime * 5f);
			((Component)_PlayerCamera).transform.position = PlayerPosition.position + _CameraOffsetPosition;
			if (((Object)(object)_CurRail != (Object)null && _CurRail.PlaneIndex == 1) || ((Object)(object)_CurRailExt != (Object)null && _CurRailExt.PlaneIndex == 1) || ((Object)(object)_NextRail != (Object)null && _NextRail.PlaneIndex == 1))
			{
				_CameraOffsetRotationX = Mathf.MoveTowardsAngle(_CameraOffsetRotationX, 180f, Time.deltaTime * 360f);
				_MouseLookHor.invertX = true;
			}
			else
			{
				_CameraOffsetRotationX = Mathf.MoveTowardsAngle(_CameraOffsetRotationX, 0f, Time.deltaTime * 360f);
				_MouseLookHor.invertX = false;
			}
			_MouseLookVer.OriginalRotation = Quaternion.Euler(0f - _CameraOffsetRotationX, 0f, 0f);
		}
	}

	private void StartFreeFall(Vector3 direction, float speed)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		_FreeFall = true;
		_FreeFallTime = Time.time;
		_FreeFallRail = _CurRail;
		((Component)MainCollider).gameObject.layer = LayerMask.NameToLayer("Default");
		((Component)this).rigidbody.useGravity = true;
		((Component)this).rigidbody.isKinematic = false;
		((Component)this).rigidbody.velocity = direction * speed;
		Collider collider = ((Component)this).collider;
		SwapColliders((BoxCollider)(object)((collider is BoxCollider) ? collider : null), RailConnectCollider);
		if ((Object)(object)_CurRail != (Object)null)
		{
			_CurRail = null;
			_CurRailExt = null;
			_NextRail = null;
			_AttachedTrolleys.Remove(this);
			_DetachedTrolleys.Add(this);
		}
	}

	private void StopFreeFall()
	{
		_FreeFall = false;
		_FreeFallRail = null;
		((Component)MainCollider).gameObject.layer = LayerMask.NameToLayer("Trolley");
		((Component)this).rigidbody.useGravity = false;
		((Component)this).rigidbody.isKinematic = true;
		Collider collider = ((Component)this).collider;
		SwapColliders((BoxCollider)(object)((collider is BoxCollider) ? collider : null), RailConnectCollider);
		if ((Object)(object)_IgnoredCollider != (Object)null)
		{
			Physics.IgnoreCollision(MainCollider, _IgnoredCollider, false);
			_IgnoredCollider = null;
		}
	}

	private void SwapColliders(BoxCollider collider1, BoxCollider collider2)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		Vector3 center = collider1.center;
		Vector3 size = collider1.size;
		collider1.center = collider2.center;
		collider1.size = collider2.size;
		collider2.center = center;
		collider2.size = size;
	}
}
