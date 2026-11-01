using System.Collections.Generic;
using Photon;
using UnityEngine;

public class AI : MonoBehaviour
{
	public const float FollowDistance = 3f;

	public const float FollowSpeedUpDistance = 15f;

	public const float RotationSmoothDamping = 6f;

	public const float PlayersMonitoringDistance = 8f;

	public const float FidgetInterval = 2f;

	public float Speed = 4f;

	public int Damage = 1;

	public bool Aggressive;

	public float AggressiveDistance = 10f;

	public float AggressiveBackDistance = 20f;

	private GameObject _AggressiveTarget;

	private Vector3 _AggressiveHomePosition = Vector3.zero;

	public bool Patrolling;

	public float PatrollingDistance;

	public float PatrollingIdle = 5f;

	private Vector3 _PatrollingCenter = Vector3.zero;

	private bool _PatrollingIdleActive;

	public bool Follow;

	private GameObject _FollowTarget;

	public bool PlayersMonitoring = true;

	private GameObject _PlayersMonitoringTarget;

	private CharacterController _CharacterController;

	private NetworkSyncAnimation _NetworkAnim;

	private float _IdleTime;

	private float _FidgetNextTime;

	private float _StunTime;

	private List<Vector3> _Path;

	private GameObject _PathObject;

	private int _PathFailCounter;

	private bool _PathNeedJump;

	private List<Vector3> _PendingPath;

	private float _JumpForce;

	private float _SpeedMultiplier = 1f;

	private Vector3 _PunchForce = Vector3.zero;

	private bool IsStunned => Time.time < _StunTime;

	public AI()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
	}

	private void Awake()
	{
		_CharacterController = ((Component)this).GetComponent<CharacterController>();
		_NetworkAnim = ((Component)this).GetComponent<NetworkSyncAnimation>();
		((Component)this).transform.Translate(0f, 0.1f, 0f, (Space)0);
	}

	private void Start()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		_AggressiveHomePosition = ((Component)this).transform.position;
		_PatrollingCenter = ((Component)this).transform.position;
	}

	public void Punch(Vector3 force, float duration)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		_ = photonView;
		if (1 == 0)
		{
			return;
		}
		ClearPath();
		_StunTime = Time.time + duration;
		_IdleTime = 0f;
		_PunchForce = force;
		_PendingPath = null;
		int num = Random.Range(3, 6);
		for (int i = 0; i < num; i++)
		{
			if (_PendingPath == null)
			{
				List<Vector3> list = Pathfinder.FindRandom(((Component)this).transform.position, _PatrollingCenter, PatrollingDistance);
				if (list == null)
				{
					break;
				}
				_PendingPath = new List<Vector3>(list);
			}
			else
			{
				List<Vector3> list2 = Pathfinder.FindRandom(_PendingPath[_PendingPath.Count - 1], _PatrollingCenter, PatrollingDistance);
				if (list2 == null)
				{
					break;
				}
				_PendingPath.AddRange(list2);
			}
		}
	}

	private void Update()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0721: Unknown result type (might be due to invalid IL or missing references)
		//IL_0726: Unknown result type (might be due to invalid IL or missing references)
		//IL_073b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0740: Unknown result type (might be due to invalid IL or missing references)
		//IL_0749: Unknown result type (might be due to invalid IL or missing references)
		//IL_074e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b01: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b0b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b10: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0abb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0adb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae0: Unknown result type (might be due to invalid IL or missing references)
		//IL_079c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0788: Unknown result type (might be due to invalid IL or missing references)
		//IL_0866: Unknown result type (might be due to invalid IL or missing references)
		//IL_0871: Unknown result type (might be due to invalid IL or missing references)
		//IL_0876: Unknown result type (might be due to invalid IL or missing references)
		//IL_087b: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0805: Unknown result type (might be due to invalid IL or missing references)
		//IL_080a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0819: Unknown result type (might be due to invalid IL or missing references)
		//IL_081e: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0507: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_0615: Unknown result type (might be due to invalid IL or missing references)
		//IL_0620: Unknown result type (might be due to invalid IL or missing references)
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_0652: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_09aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a48: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		_ = photonView;
		if (1 == 0 || !WorldGameObject.INSTANCE.IsWorldGenerated)
		{
			return;
		}
		if (((Component)this).transform.position.y < 0f)
		{
			((Component)this).transform.position = WorldGameObject.INSTANCE.GenerateSpawnPoint();
		}
		if (_Path != null && _CharacterController.isGrounded)
		{
			ValidatePath();
		}
		if (Time.time < _IdleTime)
		{
			if ((Object)(object)_NetworkAnim != (Object)null && Object.op_Implicit((Object)(object)_NetworkAnim.Anim) && !IsIdlePlaying() && !_NetworkAnim.Anim.IsPlaying("fidget"))
			{
				if ((TrackedReference)(object)_NetworkAnim.Anim["fidget"] != (TrackedReference)null && Time.time >= _FidgetNextTime)
				{
					_FidgetNextTime = Time.time + _NetworkAnim.Anim["fidget"].length + Random.Range(2f, 4f);
					_NetworkAnim.PlayAnimation(NetworkSyncAnimation.AnimState.fidget);
				}
				else
				{
					PlayIdle();
				}
			}
		}
		else if (_Path == null && _PendingPath != null && _CharacterController.isGrounded && !IsStunned)
		{
			_Path = _PendingPath;
			_PendingPath = null;
		}
		else if (_Path == null && _CharacterController.isGrounded && !IsStunned)
		{
			if (Aggressive && (Object)(object)_AggressiveTarget == (Object)null)
			{
				Collider[] array = Physics.OverlapSphere(((Component)this).transform.position, AggressiveDistance);
				List<GameObject> list = new List<GameObject>();
				Collider[] array2 = array;
				foreach (Collider val in array2)
				{
					if (!((Object)(object)((Component)val).gameObject == (Object)(object)((Component)this).gameObject) && (Object)(object)((Component)val).GetComponent<PlayerNetwork>() != (Object)null)
					{
						list.Add(((Component)val).gameObject);
					}
				}
				if (list.Count > 0)
				{
					GameObject val2 = list[Random.Range(0, list.Count)];
					_AggressiveHomePosition = val2.transform.position;
					_AggressiveTarget = val2;
				}
			}
			if (Follow && (Object)(object)_FollowTarget == (Object)null)
			{
				_FollowTarget = WorldGameObject.INSTANCE.MainPlayer;
			}
			if (Aggressive && (Object)(object)_AggressiveTarget != (Object)null)
			{
				float num = Vector3.Distance(((Component)this).transform.position, _AggressiveTarget.transform.position);
				if (num <= 2f)
				{
					RotateTo(_AggressiveTarget.transform.position);
					if ((Object)(object)_NetworkAnim != (Object)null && (Object)(object)_NetworkAnim.Anim != (Object)null && !_NetworkAnim.Anim.IsPlaying("dig"))
					{
						if ((TrackedReference)(object)_NetworkAnim.Anim["dig"] != (TrackedReference)null)
						{
							_NetworkAnim.PlayAnimation(NetworkSyncAnimation.AnimState.dig);
						}
						EntityBase component = _AggressiveTarget.GetComponent<EntityBase>();
						if ((Object)(object)component != (Object)null)
						{
							for (int j = 0; j < Damage; j++)
							{
								component.OnLeftMouseHit(string.Empty);
							}
							if (component.Life <= 0)
							{
								_AggressiveTarget = null;
							}
						}
						if ((Object)(object)_AggressiveTarget == (Object)(object)WorldGameObject.INSTANCE.MainPlayer)
						{
							WorldGameObject.INSTANCE.PunchPlayer(PhotonNetwork.player, ((Component)this).transform.forward);
							if (WorldGameObject.INSTANCE.MainPlayerNode.Life <= 0f)
							{
								_AggressiveTarget = null;
							}
						}
					}
				}
				else if (num > AggressiveDistance + 1f || Vector3.Distance(((Component)this).transform.position, _AggressiveHomePosition) > AggressiveBackDistance)
				{
					_AggressiveTarget = null;
					MoveToPosition(null, _AggressiveHomePosition, 1f, 0f);
				}
				else
				{
					MoveToPosition(_AggressiveTarget, Vector3.zero, 1f, 1f);
				}
			}
			else if (Follow && (Object)(object)_FollowTarget != (Object)null)
			{
				float num2 = Vector3.Distance(((Component)this).transform.position, _FollowTarget.transform.position);
				if (num2 <= 3f)
				{
					RotateTo(_FollowTarget.transform.position);
					PlayIdle();
				}
				else
				{
					float num3 = ((!(num2 > 15f)) ? 0f : (num2 - 15f));
					MoveToPosition(_FollowTarget, Vector3.zero, 1f + num3, 3f);
				}
			}
			else if (Patrolling)
			{
				if (!_PatrollingIdleActive)
				{
					_PatrollingIdleActive = true;
					_IdleTime = Time.time + Random.Range(PatrollingIdle, PatrollingIdle * 2f);
				}
				else
				{
					_PatrollingIdleActive = false;
					_Path = Pathfinder.FindRandom(((Component)this).transform.position, _PatrollingCenter, PatrollingDistance);
					if (_Path == null)
					{
						_IdleTime = Time.time + 1f;
					}
				}
			}
			else
			{
				if (PlayersMonitoring)
				{
					if ((Object)(object)_PlayersMonitoringTarget != (Object)null && Vector3.Distance(_PlayersMonitoringTarget.transform.position, ((Component)this).transform.position) > 9f)
					{
						_PlayersMonitoringTarget = null;
					}
					if ((Object)(object)_PlayersMonitoringTarget == (Object)null)
					{
						Collider[] array3 = Physics.OverlapSphere(((Component)this).transform.position, 8f);
						Collider[] array4 = array3;
						foreach (Collider val3 in array4)
						{
							if ((Object)(object)((Component)val3).gameObject != (Object)(object)((Component)this).gameObject && (Object)(object)((Component)val3).GetComponent<PlayerNetwork>() != (Object)null)
							{
								_PlayersMonitoringTarget = ((Component)val3).gameObject;
								break;
							}
						}
					}
					if ((Object)(object)_PlayersMonitoringTarget != (Object)null)
					{
						RotateTo(_PlayersMonitoringTarget.transform.position);
					}
					else
					{
						_IdleTime = Time.time + 1f;
					}
				}
				else
				{
					_IdleTime = Time.time + 1f;
				}
				PlayIdle();
			}
		}
		if (_JumpForce > Physics.gravity.y)
		{
			_JumpForce = Mathf.MoveTowards(_JumpForce, Physics.gravity.y, (0f - Physics.gravity.y) * Time.deltaTime);
		}
		if (_Path != null)
		{
			for (int l = 0; l < _Path.Count; l++)
			{
				Debug.DrawLine((l != 0) ? _Path[l - 1] : ((Component)this).transform.position, _Path[l], Color.red);
			}
			float num4 = Vector3.Distance(((Component)this).transform.position, _Path[0]);
			if (_CharacterController.isGrounded)
			{
				if (_Path[0].y > ((Component)this).transform.position.y + 0.2f || _PathNeedJump)
				{
					_JumpForce = 4.5f;
					_PathNeedJump = false;
				}
				else
				{
					_JumpForce = -0.1f;
				}
			}
			Vector3 val4 = _Path[0] - ((Component)this).transform.position;
			val4.Normalize();
			val4.y = _JumpForce;
			val4.x *= Speed * _SpeedMultiplier;
			val4.z *= Speed * _SpeedMultiplier;
			_CharacterController.Move(val4 * Time.deltaTime);
			RotateTo(_Path[0]);
			string text = ((!_CharacterController.isGrounded) ? "jump" : "run");
			NetworkSyncAnimation.AnimState anim = ((!_CharacterController.isGrounded) ? NetworkSyncAnimation.AnimState.jump : NetworkSyncAnimation.AnimState.run);
			if ((Object)(object)_NetworkAnim != (Object)null && (Object)(object)_NetworkAnim.Anim != (Object)null && (TrackedReference)(object)_NetworkAnim.Anim[text] != (TrackedReference)null && !_NetworkAnim.Anim.IsPlaying(text))
			{
				_NetworkAnim.PlayAnimation(anim);
			}
			if (!_CharacterController.isGrounded)
			{
				return;
			}
			if (Vector3.Distance(((Component)this).transform.position, _Path[0]) < 0.1f)
			{
				_PathFailCounter = 0;
				_PathNeedJump = _Path.Count >= 2 && Vector3.Distance(_Path[0], _Path[1]) > 1.9f;
				_Path.RemoveAt(0);
				if (_Path.Count == 0)
				{
					ClearPath();
				}
			}
			else if (Vector3.Distance(((Component)this).transform.position, _Path[0]) >= num4 && ++_PathFailCounter >= 10)
			{
				ClearPath();
				_IdleTime = Time.time + 1f;
			}
		}
		else
		{
			if (_CharacterController.isGrounded)
			{
				_JumpForce = -0.1f;
			}
			if (IsStunned && _PunchForce != Vector3.zero)
			{
				_CharacterController.Move(_PunchForce * Time.deltaTime);
			}
			else
			{
				_CharacterController.Move(new Vector3(0f, _JumpForce, 0f) * Time.deltaTime);
			}
		}
	}

	private void MoveToPosition(GameObject targetObject, Vector3 targetPosition, float speedMultiplier, float cutPath)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		_Path = Pathfinder.Find(((Component)this).transform.position, (!((Object)(object)targetObject != (Object)null)) ? targetPosition : targetObject.transform.position);
		int num = (int)cutPath;
		if (_Path != null && num > 0)
		{
			if (_Path.Count > num)
			{
				_Path.RemoveRange(_Path.Count - num, num);
			}
			else
			{
				ClearPath();
			}
		}
		if (_Path == null)
		{
			_IdleTime = Time.time + 1f;
		}
		_SpeedMultiplier = speedMultiplier;
		_PathObject = targetObject;
	}

	private void ValidatePath()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)_PathObject != (Object)null && Vector3.Distance(_Path[_Path.Count - 1], _PathObject.transform.position) > 0.1f)
		{
			ClearPath();
		}
	}

	private void RotateTo(Vector3 position)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = new Vector3(position.x, ((Component)this).transform.position.y, position.z) - ((Component)this).transform.position;
		if (val != Vector3.zero)
		{
			Quaternion val2 = Quaternion.LookRotation(val);
			((Component)this).transform.rotation = Quaternion.Slerp(((Component)this).transform.rotation, val2, Time.deltaTime * 6f);
		}
	}

	private void ClearPath()
	{
		_Path = null;
		_PathNeedJump = false;
		_PathFailCounter = 0;
	}

	private void PlayIdle()
	{
		if (!((Object)(object)_NetworkAnim == (Object)null) && !((Object)(object)_NetworkAnim.Anim == (Object)null) && !IsIdlePlaying())
		{
			if ((TrackedReference)(object)_NetworkAnim.Anim["idle2"] != (TrackedReference)null && Random.value > 0.5f)
			{
				_NetworkAnim.PlayAnimation(NetworkSyncAnimation.AnimState.idle2);
			}
			else if ((TrackedReference)(object)_NetworkAnim.Anim["idle"] != (TrackedReference)null)
			{
				_NetworkAnim.PlayAnimation(NetworkSyncAnimation.AnimState.idle);
			}
		}
	}

	private bool IsIdlePlaying()
	{
		return _NetworkAnim.Anim.IsPlaying("idle") || _NetworkAnim.Anim.IsPlaying("idle2");
	}
}
