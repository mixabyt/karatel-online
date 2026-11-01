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

	private Transform _CrosshairNode;

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

	private float _LastEmotion;

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

	public static Camera RaycastCamera => (!((Object)(object)Instance != (Object)null)) ? null : (Instance._IsThirdPerson ? Instance._ThirdPersonCamera : Instance._FirstPersonCamera);

	public static float RaycastDistance => (!((Object)(object)Instance != (Object)null)) ? 0f : Instance._RaycastDistance.Value;

	public Gun CurGun => _PlayerNetwork.CurGun;

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
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		if (!photonView.isMine)
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
		_CrosshairNode = KGUI.FindNode("hud.battle.crosshair");
		((Behaviour)_ThirdPersonCamera).enabled = false;
		Vector3 val = ((Component)_ThirdPersonCamera).transform.position - ((Component)_FirstPersonCamera).transform.position;
		_ThirdPersonCameraRange = val.magnitude;
		_ThirdPersonCameraInitialPosition = ((Component)_ThirdPersonCamera).transform.localPosition;
		_ThirdPersonCameraInitialRotation = ((Component)_ThirdPersonCamera).transform.localRotation;
		_HandsAnimation["hit"].speed = 2f;
		_HandsAnimation["build"].speed = 2f;
		_SkinManager.SetSkin(-1);
		_SkinManager.SetHand(ProfileINI.GetActualSkin());
		_StartTime = Time.time;
	}

	private void Start()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_CameraRecoilOldPos = CameraRecoil.localPosition;
		SetSavedCamera();
		if (PhotonNetwork.isMasterClient && GameType.BattleMode() && App.Instance.Settings.gifts)
		{
			bs._WorldGameObject.CreateGifts();
		}
		if (photonView.owner.isLocal)
		{
			Reset();
		}
	}

	public void SetSavedCamera()
	{
		if (GameType.BattleMode())
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
			_SkinManager.SetHand(ProfileINI.GetActualSkin());
			((Component)_PlayerNetwork.GunModels).gameObject.SetActive(false);
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
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
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
		_SkinManager.SetSkin(ProfileINI.GetActualSkin());
		_SkinManager.SetHand(-1);
		((Component)_PlayerNetwork.GunModels).gameObject.SetActive(true);
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
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a0f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a14: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a15: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a23: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a60: Unknown result type (might be due to invalid IL or missing references)
		//IL_088e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0893: Unknown result type (might be due to invalid IL or missing references)
		//IL_0895: Unknown result type (might be due to invalid IL or missing references)
		//IL_089a: Unknown result type (might be due to invalid IL or missing references)
		//IL_089f: Unknown result type (might be due to invalid IL or missing references)
		//IL_08aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0601: Unknown result type (might be due to invalid IL or missing references)
		//IL_0606: Unknown result type (might be due to invalid IL or missing references)
		//IL_060b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0622: Unknown result type (might be due to invalid IL or missing references)
		//IL_0629: Unknown result type (might be due to invalid IL or missing references)
		//IL_0634: Unknown result type (might be due to invalid IL or missing references)
		//IL_0639: Unknown result type (might be due to invalid IL or missing references)
		//IL_063e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a84: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a8b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a90: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a95: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a9a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0914: Unknown result type (might be due to invalid IL or missing references)
		//IL_0919: Unknown result type (might be due to invalid IL or missing references)
		//IL_0693: Unknown result type (might be due to invalid IL or missing references)
		//IL_0acf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_097e: Unknown result type (might be due to invalid IL or missing references)
		//IL_097f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0996: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0747: Unknown result type (might be due to invalid IL or missing references)
		//IL_074c: Unknown result type (might be due to invalid IL or missing references)
		//IL_075b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0760: Unknown result type (might be due to invalid IL or missing references)
		//IL_0764: Unknown result type (might be due to invalid IL or missing references)
		//IL_0769: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0704: Unknown result type (might be due to invalid IL or missing references)
		//IL_0709: Unknown result type (might be due to invalid IL or missing references)
		//IL_070e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0727: Unknown result type (might be due to invalid IL or missing references)
		//IL_0734: Unknown result type (might be due to invalid IL or missing references)
		//IL_0739: Unknown result type (might be due to invalid IL or missing references)
		//IL_073e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0743: Unknown result type (might be due to invalid IL or missing references)
		bs.Log("Weapon " + ((Object)CurGun).name);
		if (bs.DebugKey((KeyCode)97))
		{
			_PlayerNetwork.Grenade.bullets = 2;
		}
		if (bs.DebugKey((KeyCode)115))
		{
			Scope = false;
		}
		if (bs.DebugKey((KeyCode)119))
		{
			((Component)this).transform.position = Object.FindObjectsOfType(typeof(PlayerNetwork)).Cast<PlayerNetwork>().First((PlayerNetwork a) => (Object)(object)((Component)a).GetComponent<CameraController>() == (Object)null)
				.pos;
		}
		if (bs.DebugKey((KeyCode)101))
		{
			for (int num = 0; num < 20; num++)
			{
				bs._WorldGameObject.CreateGifts(dontcheck: true);
			}
		}
		if (bs.DebugKey((KeyCode)113))
		{
			_PlayerNetwork.Grenade.bullets = 3;
		}
		if (bs.DebugKey((KeyCode)102))
		{
			bs._WorldGameObject.PunchPlayer(photonView.owner, Vector3.zero, 100f, CurGun.arrayId);
		}
		if (!_DisableInput && !_DisableCameraSwitching && (!GameType.BattleMode() || bs._Igor.debug))
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
			if (axis != 0f && (Object)(object)WorldGameObject.Instance.EntityPreview == (Object)null && !MainMenu.Instance.MenuActive)
			{
				_ThirdPersonCameraRange += (0f - axis) * 10f;
				_ThirdPersonCameraRange = Mathf.Clamp(_ThirdPersonCameraRange, 1.5f, 6f);
			}
			bool flag = !_DisableInput && !Chat.IsEnabled() && !MainMenu.Instance.MenuActive && (Input.GetAxis("Vertical") != 0f || Input.GetAxis("Horizontal") != 0f);
			Vector3 val = ((!bs._WorldGameObject.MainPlayerDead) ? ((Component)_FirstPersonCamera).transform.position : ((Component)this).GetComponent<PlayerNetwork>().Standart.position);
			if (!_DisableInput && !MainMenu.Instance.MenuActive && (Input.GetKey((KeyCode)323) || Input.GetKey((KeyCode)324)))
			{
				bool flag2 = false;
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
					flag2 = true;
				}
				if (_IsFreeLook && !flag2)
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
			else if (flag && !_IsFreeLook && ((Collider)_CharacterController).enabled)
			{
				bool flag3 = Input.GetAxis("Vertical") > 0f;
				bool flag4 = Input.GetAxis("Vertical") < 0f;
				bool flag5 = Input.GetAxis("Horizontal") < 0f;
				bool flag6 = Input.GetAxis("Horizontal") > 0f;
				if (flag3 && flag5)
				{
					if (!IsPlaying("runR"))
					{
						CrossFade("runR");
						_NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.run);
					}
				}
				else if (flag3 && flag6)
				{
					if (!IsPlaying("runL"))
					{
						CrossFade("runL");
						_NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.run);
					}
				}
				else if (flag3)
				{
					if (!IsPlaying("run"))
					{
						CrossFade("run");
						_NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.run);
					}
				}
				else if (flag4)
				{
					if (!IsPlaying("back"))
					{
						CrossFade("back");
						_NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.back);
					}
				}
				else if (flag5)
				{
					if (!IsPlaying("strafL"))
					{
						CrossFade("strafL");
						_NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.strafL);
					}
				}
				else if (flag6 && !IsPlaying("strafR"))
				{
					CrossFade("strafR");
					_NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.strafR);
				}
			}
			else if (flag && _IsFreeLook && ((Collider)_CharacterController).enabled)
			{
				Vector3 val4 = val - ((Component)_ThirdPersonCamera).transform.position;
				val4 = new Vector3(val4.x, 0f, val4.z);
				Quaternion val5 = Quaternion.LookRotation(val4.normalized, ((Component)this).transform.up);
				bool flag7 = Input.GetAxis("Vertical") > 0f;
				bool flag8 = Input.GetAxis("Vertical") < 0f;
				bool flag9 = Input.GetAxis("Horizontal") < 0f;
				bool flag10 = Input.GetAxis("Horizontal") > 0f;
				if (flag9)
				{
					val5 *= Quaternion.AngleAxis((float)(flag7 ? (-45) : ((!flag8) ? (-90) : (-135))), ((Component)this).transform.up);
				}
				if (flag10)
				{
					val5 *= Quaternion.AngleAxis((float)(flag7 ? 45 : ((!flag8) ? 90 : 135)), ((Component)this).transform.up);
				}
				if (!flag7 && flag9 == flag10 && flag8)
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
				else if (!IsPlaying("idle") && (Object)(object)Holder.Active == (Object)null)
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
				if (!MainMenu.Instance.MenuActive)
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
				bool flag11 = !Input.GetKey((KeyCode)323) && !Input.GetKey((KeyCode)324);
				if (flag11 != _FreeLookDirectMovement)
				{
					_FreeLookDirectMovement = flag11;
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
		else if (!_DisableInput && !Chat.IsEnabled() && !MainMenu.Instance.MenuActive && !GameType.IsObserving())
		{
			_IsMove = false;
			if (((Collider)_CharacterController).enabled)
			{
				_IsMove = true;
				bool flag12 = Input.GetAxis("Vertical") > 0f;
				bool flag13 = Input.GetAxis("Vertical") < 0f;
				bool flag14 = Input.GetAxis("Horizontal") < 0f;
				bool flag15 = Input.GetAxis("Horizontal") > 0f;
				if (flag12)
				{
					_NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.run);
				}
				else if (flag13)
				{
					_NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.back);
				}
				else if (flag14)
				{
					_NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.strafL);
				}
				else if (flag15)
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
			if (GameType.BattleMode())
			{
				UpdateWeapon();
			}
		}
		if (Scope && (bool)CurGun.sniper)
		{
			_FirstPersonCamera.fieldOfView = 20f;
		}
		if (GameType.BattleMode())
		{
			bool flag16 = !CurGun.melee;
			if (_PlayerNetwork.Crouch && !flag16)
			{
				photonView.RPC("SetCrouch", PhotonTargets.All, false);
			}
			else if (flag16 && (Input.GetKeyDown((KeyCode)304) || Input.GetKeyUp((KeyCode)304)))
			{
				photonView.RPC("SetCrouch", PhotonTargets.All, Input.GetKeyDown((KeyCode)304));
			}
		}
		if (_CharacterController.isGrounded)
		{
			_LastTimeGrounded = Time.time;
		}
		if (!Chat.IsEnabled() && !MainMenu.Instance.MenuActive && Input.GetKey(VoiceChatRecorder.Instance.PushToTalkKey))
		{
			if (!_MicInited)
			{
				((MonoBehaviour)this).StartCoroutine(InitMicrophone());
			}
			else if (Time.time - _LastEmotion > 3f)
			{
				_LastEmotion = Time.time;
				Chat.SendEmotion("voiceChat", forAll: true);
			}
		}
	}

	public void OnControllerColliderHit(ControllerColliderHit hit)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		if (_CharacterController.velocity.y < -15f && GameType.BattleMode() && Time.time - _StartTime > 3f && Time.time - _LastTimeGrounded > 1f)
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
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0508: Unknown result type (might be due to invalid IL or missing references)
		//IL_050e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_0518: Unknown result type (might be due to invalid IL or missing references)
		//IL_052e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0534: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_055d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_0553: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0484: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0493: Unknown result type (might be due to invalid IL or missing references)
		//IL_0497: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0562: Unknown result type (might be due to invalid IL or missing references)
		//IL_0572: Unknown result type (might be due to invalid IL or missing references)
		//IL_057e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0583: Unknown result type (might be due to invalid IL or missing references)
		//IL_0599: Unknown result type (might be due to invalid IL or missing references)
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
		if (!CurGun.melee && !IsAction() && !HandsGunAnimations.IsPlaying("shoot") && (Input.GetAxis("Vertical") != 0f || Input.GetAxis("Horizontal") != 0f))
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
		bool flag2 = Scope && (bool)CurGun.sniper;
		if (((Component)SniperScope).renderer.enabled != flag2)
		{
			((Component)SniperScope).renderer.enabled = flag2;
		}
		if (((Component)_CrosshairNode).gameObject.activeInHierarchy != !CurGun.sniper)
		{
			((Component)_CrosshairNode).gameObject.SetActive(!CurGun.sniper);
		}
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
		SetRecoil(Vector3.Lerp(_Recoil, Vector3.zero, Time.deltaTime * CurGun.RecoilRecover));
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
			else if (!IsAction() && GameType.BattleMode())
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
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		if ((CurGun.pistol || (bool)CurGun.grenade) && (!Input.GetKeyDown((KeyCode)323) || (bool)CurGun.grenade) && (!Input.GetKeyUp((KeyCode)323) || !CurGun.grenade))
		{
			return;
		}
		SecuredValue<bool> securedValue = Time.time - _PlayerNetwork.LastTimeShoot > (float)CurGun.shootInterval;
		SecuredValue<bool> securedValue2 = CurGun.bullets > 0;
		if ((bool)securedValue && (bool)securedValue2 && Time.time - _PlayerNetwork.LastTimeShoot > (float)CurGun.shootInterval)
		{
			Play("shoot");
			CurGun.bullets--;
			SetRecoil(_Recoil + (Random.insideUnitSphere * CurGun.RecoilRand + Vector3.up * CurGun.RecoilUp * (float)((!_PlayerNetwork.Crouch) ? ((SecuredValue<float>)1f) : CurGun.CrouchRecoilFactor)));
			if (CurGun.bullets <= 0 && CurGun.bulletsLeft > 0)
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
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		_CurGunID = i;
		Gun gun = _AvaibleWeapons[_CurGunID];
		if ((Object)(object)gun != (Object)null && (gun.melee || (allowNoBullets && !gun.grenade) || gun.bulletsLeft > 0 || gun.bullets > 0 || gun.bulletsLeft > 0))
		{
			_PlayerNetwork.SelectWeapon(gun.arrayId);
			if ((Object)(object)CurGun.hands != (Object)null && (Object)(object)CurGun.hands.animation != (Object)null)
			{
				CurGun.hands.transform.localPosition = CurGun.handsPos;
			}
			Scope = false;
			if (!CurGun.melee)
			{
				Play("draw");
			}
			MainMenu.Instance.RefreshBattleWeapon();
			return true;
		}
		return false;
	}

	private IEnumerator InitMicrophone()
	{
		_MicInited = true;
		VoiceChatRecorder.Instance.NetworkId = photonView.owner.ID;
		MainMenu.Instance.ShowHint("HINT_ALLOW_MICROPHONE", asNewMenu: true);
		while (MainMenu.Instance.CurMenu == Menu.Hint)
		{
			yield return 0;
		}
		MainMenu.Instance.SwitchMenu(Menu.External);
		yield return Application.RequestUserAuthorization((UserAuthorization)2);
		MainMenu.Instance.SwitchMenu(Menu.None);
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
			Debug.Log((object)"No microphone found");
		}
		VoiceChatRecorder.Instance.StartRecording();
		VoiceChatRecorder.Instance.NewSample += Instance_NewSample;
		Debug.Log((object)"Microphone inited");
	}

	private void Instance_NewSample(VoiceChatPacket obj)
	{
		photonView.RPC("SendAudio", PhotonTargets.Others, obj.Data, obj.Length, PhotonNetwork.player.ID);
	}

	private void Play(string clip)
	{
		HandsGunAnimations.Stop();
		HandsGunAnimations.Play(clip);
		HandsGunAnimations.Sample();
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
			CurGun.bulletsLeft -= (int)CurGun.bulletsMag - CurGun.bullets;
			CurGun.bullets = CurGun.bulletsMag;
			if (CurGun.bulletsLeft < 0)
			{
				CurGun.bullets += CurGun.bulletsLeft;
				CurGun.bulletsLeft = 0;
			}
		}));
	}

	private Animation GetHandsAnimation()
	{
		return ((Component)((Component)_FirstPersonCamera).transform.Find("hands")).GetComponentInChildren<Animation>();
	}

	private void HitEvent()
	{
		WorldGameObject.Instance.OnPlayerHitTerrain();
	}

	private void BuildEvent()
	{
		if (!GameType.BlocksBuildDisabled())
		{
			WorldGameObject.Instance.OnBuildTerrain();
		}
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
		SelectWeapon(_CurGunID, allowNoBullets: true);
		Scope = false;
	}

	private void SetRecoil(Vector3 recoil)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		if (!(_Recoil == recoil))
		{
			_Recoil = recoil;
			float num = 6f + _Recoil.magnitude * 100f;
			for (int i = 1; i <= 4; i++)
			{
				float num2 = i switch
				{
					2 => 1f, 
					4 => -1f, 
					_ => 0f, 
				} * num;
				float num3 = i switch
				{
					1 => 1f, 
					3 => -1f, 
					_ => 0f, 
				} * num;
				((Component)KGUI.FindNode("hud.battle.crosshair." + i)).transform.localPosition = new Vector3(num2, num3, 0f);
			}
		}
	}
}
