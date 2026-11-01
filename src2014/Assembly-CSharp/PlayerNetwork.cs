using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerNetwork : bs
{
	public Transform Standart;

	public GameObject PlayerCamera;

	public GameObject HeadBone;

	public GameObject SpinBone;

	public Transform Head;

	internal bool InVehicle;

	internal bool InTrolley;

	private BlockType _CurDownCube;

	private bool _ForcedSound;

	private Vector3 _CorrectPlayerPos = Vector3.zero;

	private Quaternion _CorrectPlayerRot = Quaternion.identity;

	private bool _CameraRotEnabled;

	private float _CameraRotX;

	private float _CurrentHeadX;

	internal Vector3 HitDir = Vector3.zero;

	internal Vector3 HitPos;

	internal float HitTime;

	internal SecuredValue<int> CurGunIdEx = new SecuredValue<int>();

	internal Transform MuzzleFlashFirst;

	internal Transform MuzzleFlashThird;

	internal float LastTimeShoot;

	internal float ResetTime;

	private SkinManager _SkinManager;

	private CharacterController _CharacterController;

	private VoiceChatPlayer _VoiceChatPlayer;

	private Vector3 _StandartOldPos;

	private Quaternion _StandartOldRot;

	private int _KilledByGun = -1;

	private int _OldTeam;

	public Transform RHand;

	public Transform GunModels;

	public Transform FlagPlaceHolder;

	public Gun[] Guns;

	public float CrouchCamOffset;

	internal float QDamageTime = float.MinValue;

	internal PhotonPlayer Killer;

	internal Gun[] MainWeapon = new Gun[2];

	internal bool Crouch;

	public Gun Ak47;

	public Gun M16;

	public Gun Mp5;

	public Gun Shotgun;

	public Gun Sniper;

	public Gun GrenadeLauncher;

	public Gun Grenade;

	public Gun Pistol;

	public Gun Knife;

	public Gun Riffle;

	public Gun Ppsh;

	public Gun Mp40;

	public Gun Tompson;

	public static int MultiKill;

	public GameObject TransformHandR;

	public GameObject TransformHandL;

	public int PlayerTeam => ((Object)(object)TeamBattle.Instance != (Object)null) ? TeamBattle.Instance.GetPlayerTeam(PlayerName) : 0;

	public bool IsRedTeam => PlayerTeam == 1;

	public bool IsBlueTeam => PlayerTeam == 2;

	public bool IsObserver => !IsRedTeam && !IsBlueTeam;

	public string PlayerName => photonView.owner.name;

	internal int CurGunId
	{
		get
		{
			return CurGunIdEx;
		}
		set
		{
			CurGunIdEx = value;
		}
	}

	public Gun CurGun => Guns[(CurGunId != -1) ? CurGunId : 0];

	public PlayerNetwork()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
	}

	public void InitWeapons()
	{
		Pistol.pistol = (Knife.melee = (Sniper.sniper = (GrenadeLauncher.bazuka = (Grenade.grenade = true))));
		Gun knife = Knife;
		Gun knife2 = Knife;
		Gun knife3 = Knife;
		int num = (Grenade.bulletsMag = (Grenade.bulletsMax = 0));
		Grenade.bullets = num;
		num = num;
		Grenade.bulletsLeft = num;
		num = (knife2.bulletsMag = (knife3.bulletsMax = num));
		Knife.bullets = num;
		knife.bulletsLeft = num;
		Pistol.accuracy = 0.01f;
		Pistol.damage = 0.2f;
		Mp5.damage = 0.07f;
		Mp5.shootInterval = 0.08f;
		Ppsh.bulletsMag = 71;
		Ppsh.RecoilUp = 0.01f;
		Ppsh.RecoilRand = 0.01f;
		Ppsh.shootInterval = 0.09f;
		Ppsh.bulletsMax = 142;
		Ppsh.damage = 0.08f;
		Mp40.bulletsMag = 32;
		Mp40.RecoilUp = 0.02f;
		Mp40.RecoilRand = 0.015f;
		Tompson.bulletsMag = 30;
		Tompson.RecoilUp = 0.01f;
		Tompson.RecoilRand = 0.01f;
		Sniper.damage = 0.75f;
		Sniper.bulletsMax = 7;
		Sniper.bulletSpeed = (Riffle.bulletSpeed = 300f);
		Riffle.damage = 0.7f;
		Riffle.shootInterval = (Sniper.shootInterval = 1f);
		Shotgun.shootBullets = 5;
		Shotgun.damage = 0.3f;
		Shotgun.RecoilRand = 0.01f;
		Shotgun.RecoilUp = 0.1f;
		Shotgun.shootInterval = 1f;
		Shotgun.accuracy = 0.1f;
		Grenade.damage = 0.5f;
		GrenadeLauncher.reloadSpeed = 0.5f;
		GrenadeLauncher.BulletGravity = 0.5f;
		GrenadeLauncher.expDist = 6f;
		GrenadeLauncher.shootInterval = (Grenade.shootInterval = 0.5f);
		GrenadeLauncher.damage = 0.9f;
		GrenadeLauncher.shootInterval = 1f;
		GrenadeLauncher.bulletsMag = 1;
		GrenadeLauncher.bulletsMax = 6;
		Gun[] guns = Guns;
		foreach (Gun gun in guns)
		{
			gun.Reset();
		}
	}

	public string GetPrefix(Animation bodyAnimation, string s)
	{
		string text = s + ((!CurGun.pistol) ? string.Empty : "_pi") + ((!CurGun.rifle) ? string.Empty : "_rf") + ((!Crouch) ? string.Empty : "_cr");
		if ((TrackedReference)(object)bodyAnimation[text] != (TrackedReference)null)
		{
			return text;
		}
		if (Crouch)
		{
			text = s + ((!CurGun.pistol) ? string.Empty : "_pi") + ((!CurGun.rifle) ? string.Empty : "_rf");
			if ((TrackedReference)(object)bodyAnimation[text] != (TrackedReference)null)
			{
				return text;
			}
		}
		return s;
	}

	private void Awake()
	{
		_CharacterController = ((Component)this).GetComponent<CharacterController>();
	}

	private void Start()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected Obj, but got Unknown
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Expected Obj, but got Unknown
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		InitWeapons();
		if (photonView.isMine)
		{
			foreach (Transform item in ((Component)this).transform)
			{
				Transform val = item;
				if (Object.op_Implicit((Object)(object)((Component)val).GetComponent<SkinnedMeshRenderer>()))
				{
					((Renderer)((Component)val).GetComponent<SkinnedMeshRenderer>()).enabled = false;
				}
			}
			((Component)this).GetComponent<SkinManager>().DisableAllSkins();
			((Component)GunModels).gameObject.SetActive(false);
			((Component)this).GetComponent<SkinManager>().SetHand(ProfileINI.GetActualSkin());
		}
		else
		{
			if ((Object)(object)WorldGameObject.Instance.MainPlayer != (Object)null)
			{
				PlayerNode player = WorldGameObject.Instance.FindPlayer(photonView.owner);
				((Component)this).GetComponent<Nickname>().SetPlayer(player);
				int skin = WorldGameObject.Instance.FindPlayer(photonView.owner).Skin;
				((Component)this).GetComponent<SkinManager>().SetSkin(skin);
			}
			Object.Destroy((Object)(object)((Component)this).GetComponent<PlayerInput>());
			Object.Destroy((Object)(object)((Component)this).GetComponent<MouseLook>());
			Object.Destroy((Object)(object)((Component)this).GetComponent<PlayerMotor>());
			Object.Destroy((Object)(object)((Component)this).GetComponent<CameraController>());
			GameObject gameObject = ((Component)this).gameObject;
			((Object)gameObject).name = ((Object)gameObject).name + "Remote";
			foreach (Transform item2 in ((Component)this).transform)
			{
				Transform val2 = item2;
				if (((Object)val2).name == "recoil")
				{
					Object.Destroy((Object)(object)((Component)val2).gameObject);
				}
			}
		}
		WorldGameObject.Instance.FindPlayer(photonView.owner).Avatar = ((Component)this).gameObject;
		_VoiceChatPlayer = ((Component)this).GetComponent<VoiceChatPlayer>();
		((Component)this).gameObject.layer = LayerMask.NameToLayer("Players");
		_SkinManager = ((Component)this).GetComponentInChildren<SkinManager>();
		if (!photonView.isMine)
		{
			photonView.RPC("OnPlConnected", photonView.owner, PhotonNetwork.player);
		}
		else
		{
			OnLoaded(ProfileINI.GetActualSkin());
		}
		_StandartOldPos = Standart.localPosition;
		_StandartOldRot = Standart.localRotation;
		Reset(die: false);
		for (int i = 0; i < Guns.Length; i++)
		{
			Guns[i].arrayId = i;
		}
		if (GameType.BattleMode())
		{
			((Component)this).gameObject.GetComponent<SkinManager>().DisableAllSkins();
			((Component)this).gameObject.GetComponent<Nickname>().Hide();
		}
		if (photonView.isMine)
		{
			SoundFoots();
		}
	}

	[RPC]
	public void OnLoaded(int skin)
	{
		Guns[0].EnableWeapon(enable: false);
		RefreshNativeWeapon(skin);
		Guns[0].EnableWeapon(enable: true);
	}

	private static Transform FindWeapon(GameObject b)
	{
		return bs.GetTransforms(b.transform).FirstOrDefault((Transform a) => new string[6] { "_knife", "_sword", "_axe", "handle", "_pick", "_bat" }.Any((string c) => ((Object)a).name.EndsWith(c, StringComparison.OrdinalIgnoreCase)));
	}

	public void RefreshNativeWeapon(int skin)
	{
		Transform val = FindWeapon(_SkinManager.Skins[skin]);
		if ((Object)(object)val != (Object)null)
		{
			Guns[0].weapon = ((Component)val).gameObject;
		}
		if (photonView.isMine)
		{
			val = _SkinManager.Hands[skin].transform.GetChild(0);
			if ((Object)(object)val != (Object)null)
			{
				Guns[0].hands = ((Component)val.parent).gameObject;
			}
		}
	}

	[RPC]
	public void OnPlConnected(PhotonPlayer player)
	{
		photonView.RPC("OnLoaded", player, ProfileINI.GetActualSkin());
		photonView.RPC("EnableWeapon", player, CurGunId);
	}

	public void SelectWeapon(int id)
	{
		photonView.RPC("EnableWeapon", PhotonTargets.All, id);
	}

	[RPC]
	public void EnableWeapon(int id)
	{
		if (CurGunId != -1)
		{
			Guns[CurGunId].EnableWeapon(enable: false);
		}
		CurGunId = id;
		if (id != -1)
		{
			Guns[id].EnableWeapon(enable: true);
		}
	}

	[RPC]
	public void Shoot(Vector3 pos, Vector3 dir, float throwSpeed, bool qdamage, int gunid)
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		LastTimeShoot = Time.time;
		PlayOneShot((!((Object)(object)CurGun.shootSound == (Object)null)) ? CurGun.shootSound : bs._Igor.shootSound);
		for (int i = 0; i < (int)CurGun.shootBullets; i++)
		{
			Bullet bullet = (Bullet)(object)Object.Instantiate((Object)(object)(CurGun.bullet ?? bs._Igor.bullet), pos, Quaternion.LookRotation(dir + (float)CurGun.accuracy * Random.insideUnitSphere));
			bullet.gun = Guns[gunid];
			bullet.throwFactorSpeed = throwSpeed;
			bullet.Update2(Random.Range(0f, 0.05f));
			if (!qdamage)
			{
				continue;
			}
			Renderer[] componentsInChildren = ((Component)bullet).GetComponentsInChildren<Renderer>();
			foreach (Renderer val in componentsInChildren)
			{
				if (((Object)val.material.shader).name != "Diffuse")
				{
					val.material.SetColor("_TintColor", Color.red);
				}
			}
		}
	}

	[RPC]
	public void SendAudio(byte[] data, int length, int id)
	{
		if (TeamBattle.Instance.EnableVoiceChat && (Object)(object)_VoiceChatPlayer != (Object)null)
		{
			_VoiceChatPlayer.OnNewSample(new VoiceChatPacket
			{
				Compression = VoiceChatCompression.Speex,
				Data = data,
				Length = length,
				NetworkId = id
			});
		}
	}

	private void SoundFoots()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)WorldGameObject.Instance.MainPlayer != (Object)null && !Crouch)
		{
			Vector3 position = WorldGameObject.Instance.MainPlayer.transform.position;
			_CurDownCube = WorldData.Instance.GetBlockType((int)position.x, (int)position.z, (int)(position.y - 0.1f));
			if (_CurDownCube != BlockType.Air && _CurDownCube != BlockType.Water && _CurDownCube != BlockType.Lava && (_ForcedSound || Input.GetAxis("Vertical") != 0f || Input.GetAxis("Horizontal") != 0f))
			{
				_ForcedSound = false;
				if (!WorldGameObject.Instance.MainPlayerDead && !GameType.IsObserving())
				{
					WorldGameObject.Instance.photonView.RPC("SoundRPC", PhotonTargets.Others, (int)WorldGameObject.Instance.BlockParametrs[(uint)_CurDownCube].FootEffect, WorldGameObject.Instance.MainPlayer.gameObject.transform.position);
					SoundManager.Instance.Play(WorldGameObject.Instance.BlockParametrs[(uint)_CurDownCube].FootEffect, ((Component)this).audio);
				}
			}
		}
		((MonoBehaviour)this).Invoke("SoundFoots", 0.35f);
	}

	private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		if (stream.isWriting)
		{
			stream.SendNext(((Component)this).transform.position);
			stream.SendNext(((Component)this).transform.rotation);
			stream.SendNext(!WorldGameObject.Instance.MainPlayerDead && !InTrolley && (Object)(object)Holder.Active == (Object)null);
			stream.SendNext(PlayerCamera.transform.eulerAngles.x);
		}
		else
		{
			_CorrectPlayerPos = (Vector3)stream.ReceiveNext();
			_CorrectPlayerRot = (Quaternion)stream.ReceiveNext();
			_CameraRotEnabled = (bool)stream.ReceiveNext();
			_CameraRotX = (float)stream.ReceiveNext();
		}
	}

	private void Update()
	{
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		if (_OldTeam != PlayerTeam && photonView.isMine)
		{
			((MonoBehaviour)this).StartCoroutine(OnPlayerSelectedTeam());
		}
		_OldTeam = PlayerTeam;
		if (bs.DebugKey((KeyCode)116))
		{
			SetTransparent();
		}
		foreach (Transform item in ((IEnumerable<Transform>)new Transform[2] { MuzzleFlashFirst, MuzzleFlashThird }).Where((Func<Transform, bool>)((Transform a) => (Object)(object)a != (Object)null)))
		{
			((Component)item).renderer.enabled = Time.time - LastTimeShoot < 0.05f;
		}
		if (!photonView.isMine)
		{
			if (!InVehicle)
			{
				((Component)this).transform.position = Vector3.Lerp(((Component)this).transform.position, _CorrectPlayerPos, Time.deltaTime * 5f);
			}
			((Component)this).transform.rotation = Quaternion.Lerp(((Component)this).transform.rotation, _CorrectPlayerRot, Time.deltaTime * 5f);
		}
		if (photonView.isMine && (Object)(object)WorldGameObject.Instance.MainPlayer != (Object)null && _CurDownCube == BlockType.Air)
		{
			Camera mainPlayerEyes = WorldGameObject.Instance.MainPlayerEyes;
			_CurDownCube = WorldData.Instance.GetBlockType((int)((Component)mainPlayerEyes).transform.position.x, (int)((Component)mainPlayerEyes).transform.position.z, (int)((Component)mainPlayerEyes).transform.position.y - 2);
			if (_CurDownCube != BlockType.Air)
			{
				((MonoBehaviour)this).CancelInvoke();
				_ForcedSound = true;
				SoundFoots();
			}
		}
	}

	private void LateUpdate()
	{
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		if (!photonView.isMine && _CameraRotEnabled)
		{
			if (_CameraRotX < 360f && _CameraRotX >= 270f)
			{
				_CameraRotX -= 360f;
			}
			_CurrentHeadX = Mathf.Lerp(_CurrentHeadX, (0f - _CameraRotX) / 3f - 90f, Time.deltaTime * 5f);
			if (!CurGun.rifle)
			{
				Transform transform = SpinBone.transform;
				Quaternion rotation = SpinBone.transform.rotation;
				float x = rotation.eulerAngles.x;
				Quaternion rotation2 = SpinBone.transform.rotation;
				transform.rotation = Quaternion.Euler(x, rotation2.eulerAngles.y, _CurrentHeadX + ((!Crouch) ? 0f : CrouchCamOffset));
			}
			Vector3 eulerAngles = HeadBone.transform.root.eulerAngles;
			HeadBone.transform.rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y - 90f, _CurrentHeadX);
		}
	}

	[RPC]
	public void Die()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		Reset(die: true);
		if (HitDir != Vector3.zero && _KilledByGun != -1)
		{
			Rigidbody[] componentsInChildren = ((Component)Standart).GetComponentsInChildren<Rigidbody>();
			foreach (Rigidbody val in componentsInChildren)
			{
				val.AddForceAtPosition(HitDir, HitPos);
			}
		}
		if (Killer != null && Killer.ID == PhotonNetwork.player.ID)
		{
			MultiKill = 1;
			if ((Object)(object)TeamBattle.Instance != (Object)null)
			{
				TeamBattle.Instance.OnDead(Killer.name, photonView.owner.name, MultiKill);
			}
		}
		if ((Object)(object)((Component)this).GetComponent<CameraController>() != (Object)null)
		{
			MultiKill = 0;
		}
		if (Killer != null)
		{
			string name = bs._WorldGameObject.FindPlayer(photonView.owner).Name;
			string name2 = bs._WorldGameObject.FindPlayer(Killer).Name;
			if ((Object)(object)TeamBattle.Instance != (Object)null && _KilledByGun != -1)
			{
				KillsList.AddKill(name2, name, _KilledByGun);
			}
		}
	}

	[RPC]
	public void HitBy(PhotonPlayer pl, int gunId)
	{
		Killer = pl;
		_KilledByGun = gunId;
	}

	[RPC]
	public void OnHit(Vector3 dir, Vector3 pos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		HitDir = dir;
		HitPos = pos;
		HitTime = Time.time;
	}

	public void DisableMuzzleflash()
	{
		foreach (Transform item in ((IEnumerable<Transform>)new Transform[2] { MuzzleFlashFirst, MuzzleFlashThird }).Where((Func<Transform, bool>)((Transform a) => (Object)(object)a != (Object)null)))
		{
			((Component)item).renderer.enabled = false;
		}
	}

	public IEnumerator OnPlayerSelectedTeam()
	{
		if ((Object)(object)TeamBattle.Instance != (Object)null && TeamBattle.Instance.IsObserving(photonView.owner.name))
		{
			SelectWeapon(-1);
		}
		yield return null;
	}

	public void Reset(bool die)
	{
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Expected Obj, but got Unknown
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)((Component)this).GetComponent<CameraController>() == (Object)null && !die)
		{
			SetTransparent();
		}
		ResetTime = Time.time;
		Collider[] componentsInChildren = ((Component)Standart).GetComponentsInChildren<Collider>();
		foreach (Collider val in componentsInChildren)
		{
			((Component)val).gameObject.layer = LayerMask.NameToLayer("Players");
			val.enabled = die;
			if ((Object)(object)((Component)val).rigidbody != (Object)null)
			{
				((Component)val).rigidbody.isKinematic = !die;
			}
		}
		if ((Object)(object)CurGun.weapon != (Object)null && die)
		{
			DisableMuzzleflash();
			Transform transform = CurGun.weapon.transform;
			GameObject val2 = (GameObject)Object.Instantiate((Object)(object)CurGun.weapon, transform.position, transform.rotation);
			val2.transform.localScale = transform.lossyScale;
			val2.AddComponent<BoxCollider>();
			val2.AddComponent<Rigidbody>();
			val2.rigidbody.collisionDetectionMode = (CollisionDetectionMode)1;
			val2.transform.parent = null;
			Object.Destroy((Object)(object)val2, 5f);
			CurGun.EnableWeapon(enable: false);
		}
		((Collider)_CharacterController).enabled = !die;
		((Behaviour)((Component)this).animation).enabled = !die;
		if (!die)
		{
			Standart.localPosition = _StandartOldPos;
			Standart.localRotation = _StandartOldRot;
		}
		if ((Object)(object)CurGun.weapon != (Object)null && !die)
		{
			SelectWeapon(CurGunId);
		}
	}

	private void SetTransparent()
	{
		_SkinManager.SetTransparent(0.5f);
		((MonoBehaviour)this).StartCoroutine(bs.AddMethod(3f, () =>
		{
			_SkinManager.SetTransparent(bs.immportalTime);
		}));
	}

	[RPC]
	public void SetCrouch(bool b)
	{
		Crouch = b;
	}

	[RPC]
	public void SetPos(Vector3 p)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.position = p;
	}
}
