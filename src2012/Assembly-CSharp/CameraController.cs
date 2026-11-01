using System.Collections;
using System.Linq;
using UnityEngine;

public class CameraController : bs
{
	public const float RaycastDefaultDistance = 4f;

	public const float ThirdPersonMouseLookMinY = -70f;

	public const float ThirdPersonMouseLookMaxY = 50f;

	public const float ThirdPersonCameraRangeMin = 1.5f;

	public const float ThirdPersonCameraRangeMax = 6f;

	public static CameraController Instance;

	private Camera _FirstPersonCamera;

	private Camera _ThirdPersonCamera;

	private SkinManager _SkinManager;

	private Animation _BodyAnimation;

	private Animation _HandsAnimation;

	public Animation HandsGunAnimations;

	private NetworkSyncAnimation _NetworkSyncAnimation;

	private MouseLook _MouseLookHor;

	private MouseLook _MouseLookVer;

	private float _ThirdPersonCameraRange;

	private bool _IsThirdPerson;

	private bool _IsFreeLook;

	private SecuredValue<float> _RaycastDistance = new SecuredValue<float>(4f);

	private Vector3 _ThirdPersonCameraInitialPosition = Vector3.zero;

	private Quaternion _ThirdPersonCameraInitialRotation = Quaternion.identity;

	private Vector3 _FreeLookLastPosition = Vector3.zero;

	private bool _FreeLookDirectMovement;

	private float _ThirdPersonCurAlpha = 1f;

	private CharacterController _CharacterController;

	private bool _DisableInput;

	private bool _DisableCameraSwitching;

	public Transform CameraRecoil;

	public Transform SniperScope;

	public Transform ShootPoint;

	public Transform Damage;

	private Vector3 _Recoil;

	public bool Scope;

	private PlayerNetwork _PlayerNetwork;

	private Vector3 _CameraRecoilOldPos;

	private float _MouseDownTime;

	private float _LastEmote;

	private bool _MicInited;

	private bool _IsMove;

	private float _StartTime;

	private Vector3 _OldVel;

	private int _CurGunID;

	private float _LastTimeGrounded;

	public bool IsThirdPerson => _IsThirdPerson;

	public bool IsFreeLook => _IsFreeLook;

	public bool DisableInput
	{
		get
		{
			return _DisableInput || !Screen.lockCursor;
		}
		set
		{
			_DisableInput = value;
			bool disableInput = value || (_IsThirdPerson && _IsFreeLook);
			_MouseLookVer.disableInput = (_MouseLookHor.disableInput = disableInput);
		}
	}

	public bool DisableCameraSwitching
	{
		get
		{
			return _DisableCameraSwitching;
		}
		set
		{
			_DisableCameraSwitching = value;
		}
	}

	public static Camera RaycastCamera => Instance._IsThirdPerson ? Instance._ThirdPersonCamera : Instance._FirstPersonCamera;

	public static float RaycastDistance => Instance._RaycastDistance;

	public Gun CurGun => _PlayerNetwork.CurGun;

	public Vector3 Recoil
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _Recoil;
		}
	}

	private Gun[] _AvaibleWeapons => new Gun[4]
	{
		_PlayerNetwork.Knife,
		_PlayerNetwork.MainWeapon[0],
		_PlayerNetwork.MainWeapon[1],
		_PlayerNetwork.Grenade
	};

	public CameraController()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
	}

	private void Awake()
	{
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		_ = photonView;
		if (1 == 0)
		{
			Object.Destroy((Object)(object)this);
			return;
		}
		Instance = this;
		_FirstPersonCamera = ((Component)((Component)this).transform.FindChild("recoil/Main Camera")).GetComponent<Camera>();
		_ThirdPersonCamera = ((Component)((Component)_FirstPersonCamera).transform.FindChild("Camera Third Person")).GetComponent<Camera>();
		_SkinManager = ((Component)this).GetComponentInChildren<SkinManager>();
		_MouseLookHor = ((Component)this).GetComponent<MouseLook>();
		_MouseLookVer = ((Component)_FirstPersonCamera).GetComponent<MouseLook>();
		_BodyAnimation = ((Component)this).GetComponent<Animation>();
		_HandsAnimation = GetHandsAnimation();
		_CharacterController = ((Component)this).GetComponent<CharacterController>();
		_NetworkSyncAnimation = ((Component)this).GetComponent<NetworkSyncAnimation>();
		_PlayerNetwork = ((Component)this).GetComponent<PlayerNetwork>();
		((Behaviour)_ThirdPersonCamera).enabled = false;
		Vector3 val = ((Component)_ThirdPersonCamera).transform.position - ((Component)_FirstPersonCamera).transform.position;
		_ThirdPersonCameraRange = val.magnitude;
		_ThirdPersonCameraInitialPosition = ((Component)_ThirdPersonCamera).transform.localPosition;
		_ThirdPersonCameraInitialRotation = ((Component)_ThirdPersonCamera).transform.localRotation;
		_HandsAnimation["hit"].speed = 2f;
		_HandsAnimation["build"].speed = 2f;
		_SkinManager.SetSkin(-1);
		_SkinManager.SetHand(ProfileINI.skin);
		_StartTime = Time.time;
	}

	private void Start()
	{
		Reset();
	}

	public void SetSavedCamera()
	{
		if (bs._Igor.enableGuns)
		{
			SetFirstPerson();
		}
		else
		{
			((MonoBehaviour)this).StartCoroutine(SetSavedCameraProcess());
		}
	}

	private IEnumerator SetSavedCameraProcess()
	{
		yield return 0;
		if (ProfileINI.camera_type == 1)
		{
			SetFirstPerson();
		}
		if (ProfileINI.camera_type == 2)
		{
			SetThirdPerson(freeLook: false);
		}
		if (ProfileINI.camera_type == 3)
		{
			SetThirdPerson(freeLook: true);
		}
	}

	public void SetFirstPerson(bool saveSettings = true)
	{
		if (_IsThirdPerson)
		{
			if (saveSettings)
			{
				ProfileINI.camera_type = 1;
				ProfileINI.Save();
			}
			_IsThirdPerson = false;
			_SkinManager.SetSkin(-1);
			_SkinManager.SetHand(ProfileINI.skin);
			((Component)_PlayerNetwork.GunModels).gameObject.SetActiveRecursively(false);
			((Behaviour)_FirstPersonCamera).enabled = true;
			((Behaviour)_ThirdPersonCamera).enabled = false;
			_MouseLookHor.disableInput = false;
			_MouseLookVer.disableInput = false;
			((Component)_ThirdPersonCamera).transform.parent = ((Component)_FirstPersonCamera).transform;
			_MouseLookVer.minimumY = -90f;
			_MouseLookVer.maximumY = 90f;
			_RaycastDistance = 4f;
			((Component)this).gameObject.SendMessage("SetDirectMovement", (object)false);
			_SkinManager.SetTransparent(1f);
		}
	}

	public void SetThirdPerson(bool freeLook, float customAngle = 0f, bool saveSettings = true)
	{
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		if (_IsThirdPerson && _IsFreeLook == freeLook)
		{
			return;
		}
		if (saveSettings)
		{
			if (!freeLook)
			{
				ProfileINI.camera_type = 2;
				ProfileINI.Save();
			}
			else
			{
				ProfileINI.camera_type = 3;
				ProfileINI.Save();
			}
		}
		_IsThirdPerson = true;
		_IsFreeLook = freeLook;
		((Behaviour)_FirstPersonCamera).enabled = false;
		((Behaviour)_ThirdPersonCamera).enabled = true;
		_SkinManager.SetSkin(ProfileINI.skin);
		_SkinManager.SetHand(-1);
		((Component)_PlayerNetwork.GunModels).gameObject.SetActiveRecursively(true);
		((Component)_ThirdPersonCamera).transform.parent = ((Component)_FirstPersonCamera).transform;
		((Component)_ThirdPersonCamera).transform.localPosition = _ThirdPersonCameraInitialPosition;
		if (customAngle == 0f)
		{
			((Component)_ThirdPersonCamera).transform.localRotation = _ThirdPersonCameraInitialRotation;
		}
		else
		{
			Vector3 eulerAngles = _ThirdPersonCameraInitialRotation.eulerAngles;
			((Component)_ThirdPersonCamera).transform.localRotation = Quaternion.Euler(customAngle, eulerAngles.y, eulerAngles.z);
		}
		if (_IsFreeLook)
		{
			((Component)_ThirdPersonCamera).transform.parent = null;
		}
		Vector3 val = ((Component)_ThirdPersonCamera).transform.position - ((Component)_FirstPersonCamera).transform.position;
		_RaycastDistance = 4f + val.magnitude;
		_MouseLookHor.disableInput = _IsFreeLook;
		_MouseLookVer.disableInput = _IsFreeLook;
		_FreeLookDirectMovement = _IsFreeLook;
		((Component)this).gameObject.SendMessage("SetDirectMovement", (object)_FreeLookDirectMovement);
		if (!_IsFreeLook)
		{
			_MouseLookVer.minimumY = -70f;
			_MouseLookVer.maximumY = 50f;
		}
		else
		{
			_MouseLookVer.RotationY = 0f;
			_FreeLookLastPosition = ((Component)_FirstPersonCamera).transform.position;
		}
		Transform transform = ((Component)_ThirdPersonCamera).transform;
		Vector3 position = ((Component)_FirstPersonCamera).transform.position;
		Vector3 val2 = ((Component)_ThirdPersonCamera).transform.position - ((Component)_FirstPersonCamera).transform.position;
		transform.position = position + val2.normalized * 0.5f;
	}

	private void LateUpdate()
	{
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a61: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a66: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a6a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a6f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aad: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08da: Unknown result type (might be due to invalid IL or missing references)
		//IL_08dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0673: Unknown result type (might be due to invalid IL or missing references)
		//IL_0680: Unknown result type (might be due to invalid IL or missing references)
		//IL_0685: Unknown result type (might be due to invalid IL or missing references)
		//IL_068a: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0acd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ade: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae8: Unknown result type (might be due to invalid IL or missing references)
		//IL_095d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0962: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b1d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b1f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b23: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b28: Unknown result type (might be due to invalid IL or missing references)
		//IL_072c: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_079e: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0714: Unknown result type (might be due to invalid IL or missing references)
		//IL_0719: Unknown result type (might be due to invalid IL or missing references)
		//IL_071e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0723: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_0484: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0493: Unknown result type (might be due to invalid IL or missing references)
		//IL_0497: Unknown result type (might be due to invalid IL or missing references)
		//IL_049c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0756: Unknown result type (might be due to invalid IL or missing references)
		//IL_075b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0760: Unknown result type (might be due to invalid IL or missing references)
		//IL_0765: Unknown result type (might be due to invalid IL or missing references)
		//IL_077e: Unknown result type (might be due to invalid IL or missing references)
		//IL_078b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0790: Unknown result type (might be due to invalid IL or missing references)
		//IL_0795: Unknown result type (might be due to invalid IL or missing references)
		//IL_079a: Unknown result type (might be due to invalid IL or missing references)
		if (bs.DebugKey((KeyCode)97))
		{
			_PlayerNetwork.Grenade.bullets = 2;
		}
		if (bs.DebugKey((KeyCode)115))
		{
			Scope = false;
		}
		bs.Log("Weapon " + ((Object)CurGun).name);
		if (bs.DebugKey((KeyCode)119))
		{
			((Component)this).transform.position = Object.FindObjectsOfType(typeof(PlayerNetwork)).Cast<PlayerNetwork>().First((PlayerNetwork a) => (Object)(object)((Component)a).GetComponent<CameraController>() == (Object)null)
				.pos;
		}
		if (bs._Igor.enableGuns && ((Component)HandsGunAnimations).gameObject.active != !bs._Igor.spectate)
		{
			((Component)HandsGunAnimations).gameObject.SetActiveRecursively(!bs._Igor.spectate);
		}
		bool flag = bs._Igor.enableGuns && (Object)(object)bs._TeamBattle != (Object)null && bs._TeamBattle._TeamSelected && !bs._Igor.spectate;
		if (((Component)bs._Hud).gameObject.active != flag)
		{
			((Component)bs._Hud).gameObject.SetActiveRecursively(flag);
		}
		if (bs.DebugKey((KeyCode)101))
		{
			for (int num = 0; num < 20; num++)
			{
				bs._WorldGameObject.CreateGifts();
			}
		}
		if (bs.DebugKey((KeyCode)113))
		{
			_PlayerNetwork.Grenade.bullets = 3;
		}
		if (bs.DebugKey((KeyCode)102))
		{
			bs._WorldGameObject.PunchPlayer(photonView.owner, Vector3.zero, 100f, CurGun.ArrayId);
		}
		if (!_DisableInput && !_DisableCameraSwitching && (!bs._Igor.enableGuns || bs._Igor.debug))
		{
			if (Input.GetKeyUp((KeyCode)282))
			{
				SetFirstPerson();
			}
			else if (Input.GetKeyUp((KeyCode)283))
			{
				SetThirdPerson(freeLook: false);
			}
			else if (Input.GetKeyUp((KeyCode)284))
			{
				SetThirdPerson(freeLook: true);
			}
		}
		if (_IsThirdPerson)
		{
			float axis = Input.GetAxis("Mouse ScrollWheel");
			if (axis != 0f && !WorldGameObject.INSTANCE.bPreviewMagazinItem && !WorldGameObject.INSTANCE.IsMenuOpen())
			{
				_ThirdPersonCameraRange += (0f - axis) * 10f;
				_ThirdPersonCameraRange = Mathf.Clamp(_ThirdPersonCameraRange, 1.5f, 6f);
			}
			bool flag2 = !_DisableInput && !ChatClient.isEnabled && (Input.GetKey((KeyCode)119) || Input.GetKey((KeyCode)115) || Input.GetKey((KeyCode)97) || Input.GetKey((KeyCode)100));
			Vector3 val = ((!bs._WorldGameObject.IsMainPlayerDead) ? ((Component)_FirstPersonCamera).transform.position : ((Component)this).GetComponent<PlayerNetwork>().Standart.position);
			if (!_DisableInput && !WorldGameObject.INSTANCE.IsMenuOpen() && (Input.GetKey((KeyCode)323) || Input.GetKey((KeyCode)324)))
			{
				bool flag3 = false;
				if (Input.GetKey((KeyCode)323) && !IsPlaying("dig") && !IsPlaying("build"))
				{
					CrossFade("dig");
					_NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.dig);
					((MonoBehaviour)this).Invoke("HitEvent", 0.2f);
				}
				else if (Input.GetKey((KeyCode)324) && !IsPlaying("build") && !IsPlaying("dig"))
				{
					CrossFade("build");
					_NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.build);
					((MonoBehaviour)this).Invoke("BuildEvent", 0.2f);
				}
				else
				{
					flag3 = true;
				}
				if (_IsFreeLook && !flag3)
				{
					Vector3 val2 = val - ((Component)_ThirdPersonCamera).transform.position;
					val2 = new Vector3(val2.x, 0f, val2.z);
					MouseLook mouseLookHor = _MouseLookHor;
					Quaternion val3 = Quaternion.LookRotation(val2.normalized, Vector3.up);
					float y = val3.eulerAngles.y;
					Quaternion originalRotation = _MouseLookHor.OriginalRotation;
					mouseLookHor.RotationX = y - originalRotation.eulerAngles.y;
				}
			}
			else if (flag2 && !_IsFreeLook && ((Collider)_CharacterController).enabled)
			{
				bool key = Input.GetKey((KeyCode)119);
				bool key2 = Input.GetKey((KeyCode)115);
				bool key3 = Input.GetKey((KeyCode)97);
				bool key4 = Input.GetKey((KeyCode)100);
				if (key && key3)
				{
					if (!IsPlaying("runR"))
					{
						CrossFade("runR");
						_NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.run);
					}
				}
				else if (key && key4)
				{
					if (!IsPlaying("runL"))
					{
						CrossFade("runL");
						_NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.run);
					}
				}
				else if (key)
				{
					if (!IsPlaying("run"))
					{
						CrossFade("run");
						_NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.run);
					}
				}
				else if (key2)
				{
					if (!IsPlaying("back"))
					{
						CrossFade("back");
						_NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.back);
					}
				}
				else if (key3)
				{
					if (!IsPlaying("strafL"))
					{
						CrossFade("strafL");
						_NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.strafL);
					}
				}
				else if (key4 && !IsPlaying("strafR"))
				{
					CrossFade("strafR");
					_NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.strafR);
				}
			}
			else if (flag2 && _IsFreeLook && ((Collider)_CharacterController).enabled)
			{
				Vector3 val4 = val - ((Component)_ThirdPersonCamera).transform.position;
				val4 = new Vector3(val4.x, 0f, val4.z);
				Quaternion val5 = Quaternion.LookRotation(val4.normalized, ((Component)this).transform.up);
				bool key5 = Input.GetKey((KeyCode)119);
				bool key6 = Input.GetKey((KeyCode)115);
				bool key7 = Input.GetKey((KeyCode)97);
				bool key8 = Input.GetKey((KeyCode)100);
				if (key7)
				{
					val5 *= Quaternion.AngleAxis((float)(key5 ? (-45) : ((!key6) ? (-90) : (-135))), ((Component)this).transform.up);
				}
				if (key8)
				{
					val5 *= Quaternion.AngleAxis((float)(key5 ? 45 : ((!key6) ? 90 : 135)), ((Component)this).transform.up);
				}
				if (!key5 && key7 == key8 && key6)
				{
					val5 *= Quaternion.AngleAxis(180f, ((Component)this).transform.up);
				}
				float y2 = val5.eulerAngles.y;
				Quaternion originalRotation2 = _MouseLookHor.OriginalRotation;
				float num2 = y2 - originalRotation2.eulerAngles.y;
				_MouseLookHor.RotationX = Mathf.MoveTowardsAngle(_MouseLookHor.RotationX, num2, Time.deltaTime * 420f);
				if (!IsPlaying("run"))
				{
					CrossFade("run");
					_NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.run);
				}
			}
			else if (!IsPlaying("dig") && !IsPlaying("build") && !_DisableInput)
			{
				if (((Collider)_CharacterController).enabled && !_CharacterController.isGrounded)
				{
					if (!IsPlaying("jump"))
					{
						CrossFade("jump");
						_NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.jump);
					}
				}
				else if (!IsPlaying("idle"))
				{
					CrossFade("idle");
					_NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.idle);
				}
			}
			if (_IsFreeLook)
			{
				Transform transform = ((Component)_ThirdPersonCamera).transform;
				transform.position += val - _FreeLookLastPosition;
				_FreeLookLastPosition = val;
				float num3 = 0f;
				float num4 = 0f;
				if (!WorldGameObject.INSTANCE.IsMenuOpen())
				{
					num3 = Input.GetAxis("Mouse X") * ProfileINI.mouse_sens * ((Component)this).GetComponent<MouseLook>().ScopeFactor;
					num4 = Input.GetAxis("Mouse Y") * ProfileINI.mouse_sens * ((Component)this).GetComponent<MouseLook>().ScopeFactor;
				}
				float x = ((Component)_ThirdPersonCamera).transform.eulerAngles.x;
				if (x < 80f && num4 < x - 75f)
				{
					num4 = x - 75f;
				}
				else if (x > 295f && num4 > x - 300f)
				{
					num4 = x - 300f;
				}
				((Component)_ThirdPersonCamera).transform.RotateAround(val, Vector3.up, num3);
				((Component)_ThirdPersonCamera).transform.RotateAround(val, -((Component)_ThirdPersonCamera).transform.right, num4);
				bool flag4 = !Input.GetKey((KeyCode)323) && !Input.GetKey((KeyCode)324);
				if (flag4 != _FreeLookDirectMovement)
				{
					_FreeLookDirectMovement = flag4;
					((Component)this).gameObject.SendMessage("SetDirectMovement", (object)_FreeLookDirectMovement);
				}
			}
			Vector3 val6 = ((Component)_ThirdPersonCamera).transform.position - val;
			Vector3 normalized = val6.normalized;
			float magnitude = val6.magnitude;
			int num5 = (1 << LayerMask.NameToLayer("Entity")) | (1 << LayerMask.NameToLayer("SmallDecor")) | (1 << LayerMask.NameToLayer("Terrain"));
			RaycastHit val7 = default;
			if (Physics.Raycast(val, normalized, ref val7, _ThirdPersonCameraRange + 0.5f, num5))
			{
				Vector3 val8 = val - (val7.point - normalized * 0.5f);
				magnitude = val8.magnitude;
			}
			else
			{
				magnitude = Mathf.MoveTowards(magnitude, _ThirdPersonCameraRange, Time.deltaTime * 5f);
			}
			((Component)_ThirdPersonCamera).transform.position = val + normalized * magnitude;
			_RaycastDistance = 4f + magnitude * 0.9f;
			float num6 = ((!(magnitude < 1.2f)) ? 1f : 0.3f);
			if (_ThirdPersonCurAlpha != num6)
			{
				_ThirdPersonCurAlpha = Mathf.MoveTowards(_ThirdPersonCurAlpha, num6, Time.deltaTime * 3f);
				_SkinManager.SetTransparent(_ThirdPersonCurAlpha);
			}
		}
		else if (!_DisableInput && !WorldGameObject.INSTANCE.IsMenuOpen() && (!((Object)(object)TeamBattle.Instance != (Object)null) || !TeamBattle.Instance.IsObserving()))
		{
			_IsMove = false;
			if (((Collider)_CharacterController).enabled)
			{
				_IsMove = true;
				bool key9 = Input.GetKey((KeyCode)119);
				bool key10 = Input.GetKey((KeyCode)115);
				bool key11 = Input.GetKey((KeyCode)97);
				bool key12 = Input.GetKey((KeyCode)100);
				if (key9)
				{
					_NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.run);
				}
				else if (key10)
				{
					_NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.back);
				}
				else if (key11)
				{
					_NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.strafL);
				}
				else if (key12)
				{
					_NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.strafR);
				}
				else if (((Collider)_CharacterController).enabled && !_CharacterController.isGrounded)
				{
					_NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.jump);
				}
				else
				{
					_IsMove = false;
				}
			}
			UpdateMouseButton();
			if (bs._Igor.enableGuns)
			{
				UpdateWeapon();
			}
		}
		if (Scope && (bool)CurGun.sniper)
		{
			_FirstPersonCamera.fieldOfView = 20f;
		}
		if (bs._Igor.enableGuns)
		{
			UpdateHud();
		}
		if (bs._Igor.enableGuns && bs._Igor.enableCrouch && (Input.GetKeyDown((KeyCode)306) || Input.GetKeyUp((KeyCode)306)))
		{
			photonView.RPC("SetCrouch", PhotonTargets.All, Input.GetKeyDown((KeyCode)306));
		}
		if (_CharacterController.isGrounded)
		{
			_LastTimeGrounded = Time.time;
		}
	}

	public void OnControllerColliderHit(ControllerColliderHit hit)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		if (_CharacterController.velocity.y < -15f && bs._Igor.enableGuns && Time.time - _StartTime > 3f && Time.time - _LastTimeGrounded > 1f)
		{
			Debug.Log((object)_CharacterController.velocity.y);
			PlayOneShot(bs._Igor.fallSound);
			bs._WorldGameObject.PunchPlayer(photonView.owner, Vector3.zero, 0.1f);
		}
	}

	private void CrossFade(string runr)
	{
		_BodyAnimation.CrossFade(_PlayerNetwork.GetPrefix(_BodyAnimation, runr));
	}

	private bool IsPlaying(string runr)
	{
		return _BodyAnimation.IsPlaying(_PlayerNetwork.GetPrefix(_BodyAnimation, runr));
	}

	private bool IsAction()
	{
		return HandsGunAnimations.IsPlaying("reload") || HandsGunAnimations.IsPlaying("draw");
	}

	private void UpdateWeapon()
	{
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_050a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0464: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_050f: Unknown result type (might be due to invalid IL or missing references)
		//IL_051f: Unknown result type (might be due to invalid IL or missing references)
		//IL_052b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0530: Unknown result type (might be due to invalid IL or missing references)
		//IL_054b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0550: Unknown result type (might be due to invalid IL or missing references)
		//IL_0564: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b3: Unknown result type (might be due to invalid IL or missing references)
		bs.Log("UpdatingWeapon");
		float axis = Input.GetAxis("Mouse ScrollWheel");
		if (axis != 0f)
		{
			SwitchWeapon(axis);
		}
		for (int i = 0; i < 4; i++)
		{
			if (Input.GetKeyDown((KeyCode)(49 + i)))
			{
				SelectWeapon(i, allowNoBullets: true);
			}
		}
		if (Scope && !CurGun.sniper)
		{
			((Component)_FirstPersonCamera).animation.CrossFade("aim");
		}
		else
		{
			((Component)_FirstPersonCamera).animation.CrossFade("idle");
		}
		bool flag = Time.time - _PlayerNetwork.HitTime < 1f;
		((Component)Damage).renderer.enabled = flag;
		if (flag)
		{
			((Component)Damage).renderer.material.color = Color.white * Mathf.Max(0f, _PlayerNetwork.HitTime + 1f - Time.time);
			Transform parent = Damage.parent;
			Vector3 v = ZeroY(-((Component)this).transform.forward);
			Vector3 val = ZeroY(_PlayerNetwork.HitDir);
			parent.localEulerAngles = new Vector3(0f, 0f, AngleSigned(v, val.normalized, Vector3.down));
		}
		if (!CurGun.melee && !IsAction() && !HandsGunAnimations.IsPlaying("shoot") && (Input.GetKey((KeyCode)119) || Input.GetKey((KeyCode)115) || Input.GetKey((KeyCode)97) || Input.GetKey((KeyCode)100)))
		{
			HandsGunAnimations.Play("walk");
		}
		if (Input.GetKeyDown((KeyCode)324) && !CurGun.melee && !CurGun.grenade && !CurGun.sniper)
		{
			Scope = !Scope;
		}
		if (Input.GetKeyDown((KeyCode)324) && (bool)CurGun.sniper && !CurGun.hands.animation.isPlaying)
		{
			AnimationState val2 = CurGun.hands.animation["snipe"];
			val2.time = ((!Scope) ? 0f : val2.length);
			val2.speed = ((!Scope) ? 1.5f : (-1.5f));
			CurGun.hands.renderer.enabled = true;
			CurGun.hands.animation.Play();
			if (Scope)
			{
				Scope = !Scope;
			}
			else
			{
				((MonoBehaviour)this).StartCoroutine(bs.AddMethod(() => !CurGun.hands.animation.isPlaying, () =>
				{
					if ((bool)CurGun.sniper)
					{
						Scope = !Scope;
					}
					CurGun.hands.renderer.enabled = false;
				}));
			}
		}
		((Component)SniperScope).renderer.enabled = Scope && (bool)CurGun.sniper;
		if (Input.GetKeyDown((KeyCode)114) && CurGun.bullets != (int)CurGun.bulletsMag && (int)CurGun.bulletsMax > 0 && !HandsGunAnimations.IsPlaying("reload"))
		{
			Reload();
		}
		if (Input.GetKeyDown((KeyCode)323))
		{
			_MouseDownTime = Time.time;
		}
		if ((bool)CurGun.grenade)
		{
			if (Input.GetKey((KeyCode)323) && CurGun.bullets > 0)
			{
				Transform transform = CurGun.hands.transform;
				Vector3 handsPos = CurGun.handsPos;
				Vector3 val3 = Vector3.right + Vector3.back;
				transform.localPosition = handsPos + val3.normalized * Mathf.Clamp01(Time.time - _MouseDownTime) * 0.5f;
			}
			else
			{
				CurGun.hands.transform.localPosition = CurGun.handsPos;
			}
		}
		if (!_IsMove)
		{
			_NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.idle);
		}
		CameraRecoil.localRotation = Quaternion.LookRotation(Vector3.forward + _Recoil);
		CameraRecoil.localPosition = Vector3.Lerp(CameraRecoil.localPosition, _CameraRecoilOldPos + ((!_PlayerNetwork.Crouch) ? Vector3.zero : (Vector3.down * 0.3f)), Time.deltaTime * 10f);
		_Recoil = Vector3.Lerp(_Recoil, Vector3.zero, Time.deltaTime * (float)CurGun.RecoilRecover);
		if (!ChatClient.isEnabled && Input.GetKey(VoiceChatRecorder.Instance.PushToTalkKey) && Time.time - _LastEmote > 3f)
		{
			_LastEmote = Time.time;
			ChatClient.instance.SendEmote("voiceChat");
		}
		if (!_MicInited && Input.GetKeyDown(VoiceChatRecorder.Instance.PushToTalkKey))
		{
			((MonoBehaviour)this).StartCoroutine(InitMicrophone());
		}
	}

	private void UpdateHud()
	{
		bs._Hud.BulletText.text = CurGun.bullets + "/" + (int)CurGun.bulletsLeft;
		int num = (int)(bs._WorldGameObject.MainPlayerNode.Life * 100f);
		bs._Hud.Health.text = num + string.Empty;
		bs._Hud.Shield.text = (int)(bs._WorldGameObject.MainPlayerNode.Shield * 100f) + string.Empty;
		bs._Hud.BlueTeam.text = TeamBattle.Instance.BlueTeamScore + string.Empty;
		bs._Hud.RedTeam.text = TeamBattle.Instance.RedTeamScore + string.Empty;
	}

	private void UpdateMouseButton()
	{
		if (CurGun.bullets <= 0 && (bool)CurGun.grenade)
		{
			SwitchWeapon(1f);
		}
		if (Input.GetKey((KeyCode)323) || (Input.GetKeyUp((KeyCode)323) && (bool)CurGun.grenade))
		{
			if (CurGun.melee)
			{
				if (!_HandsAnimation.IsPlaying("hit") && !_HandsAnimation.IsPlaying("build"))
				{
					_HandsAnimation.Play("hit");
					_NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.dig);
					((MonoBehaviour)this).Invoke("HitEvent", 0.2f);
				}
			}
			else if (!IsAction() && bs._Igor.enableGuns)
			{
				UpdateShooting();
			}
		}
		else if (Input.GetKey((KeyCode)324) && !_HandsAnimation.IsPlaying("hit") && !_HandsAnimation.IsPlaying("build") && CurGun.melee)
		{
			_HandsAnimation.Play("build");
			_NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.build);
			((MonoBehaviour)this).Invoke("BuildEvent", 0.2f);
		}
		else if (!_HandsAnimation.IsPlaying("idle") && (Object)(object)Holder.Active == (Object)null)
		{
			if (CurGun.melee)
			{
				_HandsAnimation.PlayQueued("idle");
				_NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.idle);
			}
		}
		else if (!_IsMove)
		{
			_NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.idle);
		}
	}

	private void UpdateShooting()
	{
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		if (((!CurGun.pistol && !CurGun.grenade) || (Input.GetKeyDown((KeyCode)323) && !CurGun.grenade) || (Input.GetKeyUp((KeyCode)323) && (bool)CurGun.grenade)) && Time.time - _PlayerNetwork.LastTimeShoot > (float)CurGun.shootInterval && CurGun.bullets > 0)
		{
			Play("shoot");
			CurGun.bullets--;
			_Recoil += Random.insideUnitSphere * (float)CurGun.RecoilRand + Vector3.up * (float)CurGun.RecoilUp * (float)((!_PlayerNetwork.Crouch) ? ((SecuredValue<float>)1f) : CurGun.CrouchRecoilFactor);
			if (CurGun.bullets <= 0 && (int)CurGun.bulletsLeft > 0)
			{
				Reload();
			}
			_PlayerNetwork.LastTimeShoot = Time.time;
			RaycastHit val = default;
			Vector3 val3;
			if (Physics.Raycast(_FirstPersonCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)), ref val, 10000f))
			{
				Vector3 val2 = val.point - CurGun.hands.transform.position;
				val3 = val2.normalized;
			}
			else
			{
				val3 = ((Component)_FirstPersonCamera).transform.forward;
			}
			bool flag = _PlayerNetwork.QDamageTime > Time.time;
			_PlayerNetwork.Shoot(_PlayerNetwork.CurGun.hands.transform.position, val3, Time.time - _MouseDownTime, flag, _PlayerNetwork.CurGunId);
			if ((bool)_PlayerNetwork.CurGun.grenade)
			{
				Play("draw");
			}
			photonView.RPC("Shoot", PhotonTargets.Others, ShootPoint.position, val3, Time.time - _MouseDownTime, flag, _PlayerNetwork.CurGunId);
		}
	}

	private void SwitchWeapon(float d)
	{
		for (int i = 0; i < 10; i++)
		{
			_CurGunID = Mod(_CurGunID + ((!(d > 0f)) ? 1 : (-1)), _AvaibleWeapons.Length);
			if (SelectWeapon(_CurGunID))
			{
				break;
			}
		}
	}

	private bool SelectWeapon(int i, bool allowNoBullets = false)
	{
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		_CurGunID = i;
		Gun gun = _AvaibleWeapons[_CurGunID];
		if ((Object)(object)gun != (Object)null && (gun.melee || (allowNoBullets && !gun.grenade) || (int)gun.bulletsLeft > 0 || gun.bullets > 0 || (int)gun.bulletsLeft > 0))
		{
			_PlayerNetwork.SelectWeapon(gun.ArrayId);
			if ((Object)(object)CurGun.hands != (Object)null && (Object)(object)CurGun.hands.animation != (Object)null)
			{
				CurGun.hands.transform.localPosition = CurGun.handsPos;
			}
			Scope = false;
			if (!CurGun.melee)
			{
				Play("draw");
			}
			GUITexture[] weaponBG = bs._Hud.WeaponBG;
			foreach (GUITexture val in weaponBG)
			{
				val.texture = (Texture)(object)bs._Hud.Background;
			}
			bs._Hud.WeaponBG[_CurGunID].texture = (Texture)(object)bs._Hud.BackgroundHollow;
			bs._Hud.BulletIcon.texture = (Texture)(object)((!((Object)(object)CurGun.bulletIcon == (Object)null)) ? CurGun.bulletIcon : bs._Hud.defBulletIcon);
			return true;
		}
		return false;
	}

	private IEnumerator InitMicrophone()
	{
		_MicInited = true;
		VoiceChatRecorder.Instance.NetworkId = photonView.owner.ID;
		MainMenu.INSTANCE.ShowPopup();
		while (MainMenu.INSTANCE.showMenu)
		{
			yield return null;
		}
		bs._WorldGameObject.InitingMicrophone = true;
		yield return Application.RequestUserAuthorization((UserAuthorization)2);
		bs._WorldGameObject.InitingMicrophone = false;
		if (!Application.HasUserAuthorization((UserAuthorization)2))
		{
			_MicInited = false;
			yield break;
		}
		if (VoiceChatRecorder.Instance.AvailableDevices.Length > 0)
		{
			VoiceChatRecorder.Instance.Device = VoiceChatRecorder.Instance.AvailableDevices[0];
		}
		else
		{
			MonoBehaviour.print((object)"No Microphone Found ");
		}
		VoiceChatRecorder.Instance.StartRecording();
		VoiceChatRecorder.Instance.NewSample += Instance_NewSample;
		MonoBehaviour.print((object)"Mic Inited");
	}

	private void Instance_NewSample(VoiceChatPacket obj)
	{
		photonView.RPC("SendAudio", PhotonTargets.Others, obj.Data, obj.Length, PhotonNetwork.player.ID);
	}

	private void Play(string clip)
	{
		HandsGunAnimations.Stop();
		HandsGunAnimations.Play(clip);
	}

	private void Reload()
	{
		if (!CurGun.sniper)
		{
			Scope = false;
		}
		HandsGunAnimations["reload"].speed = CurGun.reloadSpeed;
		Play("reload");
		PlayOneShot((!((Object)(object)CurGun.reloadSound == (Object)null)) ? CurGun.reloadSound : bs._Igor.reload);
		((MonoBehaviour)this).StartCoroutine(bs.AddMethod(() => !HandsGunAnimations.IsPlaying("reload"), () =>
		{
			Gun curGun = CurGun;
			curGun.bulletsLeft = (int)curGun.bulletsLeft - ((int)CurGun.bulletsMag - CurGun.bullets);
			MonoBehaviour.print((object)"reload");
			CurGun.bullets = CurGun.bulletsMag;
			if ((int)CurGun.bulletsLeft < 0)
			{
				CurGun.bullets += CurGun.bulletsLeft;
				CurGun.bulletsLeft = 0;
			}
		}));
	}

	private Animation GetHandsAnimation()
	{
		return ((Component)((Component)_FirstPersonCamera).transform.Find("hand_lastD4")).GetComponentInChildren<Animation>();
	}

	private void HitEvent()
	{
		WorldGameObject.INSTANCE.OnPlayerHitTerrain();
	}

	private void BuildEvent()
	{
		WorldGameObject.INSTANCE.OnBuildTerrain();
	}

	public void JoltScreen()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		Camera val = ((!IsThirdPerson) ? _FirstPersonCamera : _ThirdPersonCamera);
		float num = ((Random.Range(0, 2) != 0) ? (-30) : 30);
		iTween.PunchRotation(((Component)val).gameObject, new Vector3(0f, 0f, num), 1f);
	}

	private int Mod(int a, int n)
	{
		return (a % n + n) % n;
	}

	private float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return Mathf.Atan2(Vector3.Dot(n, Vector3.Cross(v1, v2)), Vector3.Dot(v1, v2)) * 57.29578f;
	}

	private void PrintAnims()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected Obj, but got Unknown
		foreach (AnimationState item in ((Component)this).animation)
		{
			AnimationState val = item;
			bs.Log(val.name + " " + val.layer, val.enabled, val.time);
		}
	}

	public void Reset()
	{
		MonoBehaviour.print((object)("PlayerCC Reset " + _PlayerNetwork.PlayerName));
		SelectWeapon(_CurGunID, allowNoBullets: true);
		Scope = false;
	}
}
