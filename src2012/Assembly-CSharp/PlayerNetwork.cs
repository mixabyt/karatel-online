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

	internal SecuredValue<int> curGunId = new SecuredValue<int>();

	internal Transform MuzzleFlashFps;

	internal Transform MuzzleFlashThird;

	internal float LastTimeShoot;

	internal float ResetTime;

	private SkinManager _SkinManager;

	private CharacterController _CharacterController;

	private VoiceChatPlayer _VoiceChatPlayer;

	private Vector3 _StandartOldPos;

	private Quaternion _StandartOldRot;

	private int _KilledByGun = -1;

	public Transform RHand;

	public Transform GunModels;

	public Transform HandModels;

	public Gun[] Guns;

	public float CrouchCamOffset;

	internal float QDamageTime = float.MinValue;

	internal PhotonPlayer KilledBy;

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

	public static int MultiKill;

	private int oldTeam;

	public int PlayerTeam => ((Object)(object)bs._TeamBattle != (Object)null) ? bs._TeamBattle.GetPlayerTeam(PlayerName) : 0;

	public string PlayerName => photonView.owner.name;

	internal int CurGunId
	{
		get
		{
			return curGunId;
		}
		set
		{
			curGunId = value;
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
		Gun grenade = Grenade;
		int num = (Grenade.bulletsMag = (Grenade.bulletsMax = 0));
		Grenade.bullets = num;
		num = (knife2.bulletsMag = (knife3.bulletsMax = (grenade.bulletsLeft = num)));
		Knife.bullets = num;
		knife.bulletsLeft = num;
		Pistol.accuracy = 0.01f;
		Pistol.damage = 0.3f;
		Mp5.damage = 0.07f;
		Mp5.shootInterval = 0.08f;
		Sniper.damage = 0.9f;
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

	public string GetPrefix(Animation _BodyAnimation, string s)
	{
		string text = s + ((!CurGun.melee) ? "_pi" : string.Empty) + ((!Crouch) ? string.Empty : "_cr");
		if ((TrackedReference)(object)_BodyAnimation[text] != (TrackedReference)null)
		{
			return text;
		}
		text = s + ((!CurGun.melee) ? "_pi" : string.Empty);
		if ((TrackedReference)(object)_BodyAnimation[text] != (TrackedReference)null)
		{
			return text;
		}
		return s;
	}

	private void Start()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected Obj, but got Unknown
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Expected Obj, but got Unknown
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		InitWeapons();
		_ = photonView;
		if (true)
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
			((Component)GunModels).gameObject.SetActiveRecursively(false);
			((Component)this).GetComponent<SkinManager>().SetHand(ProfileINI.skin);
		}
		else
		{
			_ = WorldGameObject.INSTANCE.MainPlayer;
			_ = null;
			if (false)
			{
				string playerName = WorldGameObject.INSTANCE.FindPlayer(photonView.owner).playerName;
				((Component)this).GetComponent<SetLabelOnMessage>().SetLabel(playerName);
				int skin = WorldGameObject.INSTANCE.FindPlayer(photonView.owner).skin;
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
		WorldGameObject.INSTANCE.FindPlayer(photonView.owner).avatar = ((Component)this).gameObject;
		_VoiceChatPlayer = ((Component)this).GetComponent<VoiceChatPlayer>();
		((Component)this).gameObject.layer = LayerMask.NameToLayer("Players");
		_SkinManager = ((Component)this).GetComponentInChildren<SkinManager>();
		_ = photonView;
		if (1 == 0)
		{
			photonView.RPC("OnPlConnected", photonView.owner, PhotonNetwork.player);
		}
		else
		{
			OnLoaded(ProfileINI.skin);
		}
		_CharacterController = ((Component)this).GetComponent<CharacterController>();
		_StandartOldPos = Standart.localPosition;
		_StandartOldRot = Standart.localRotation;
		Reset(die: false);
		for (int i = 0; i < Guns.Length; i++)
		{
			Guns[i].ArrayId = i;
		}
	}

	private static Transform FindWeapon(GameObject b)
	{
		return bs.GetTransforms(b.transform).FirstOrDefault((Transform a) => new string[4] { "_knife", "_sword", "_axe", "handle" }.Any((string c) => ((Object)a).name.EndsWith(c, StringComparison.OrdinalIgnoreCase)));
	}

	[RPC]
	public void OnLoaded(int skin)
	{
		MonoBehaviour.print((object)("OnLoaded " + PlayerName));
		GameObject[] objs = _SkinManager.Skins[skin].objs;
		foreach (GameObject b in objs)
		{
			Transform val = FindWeapon(b);
			if ((Object)(object)val != (Object)null)
			{
				Guns[0].weapon = ((Component)val).gameObject;
			}
		}
		_ = photonView;
		if (true)
		{
			GameObject[] objs2 = _SkinManager.Hands[skin].objs;
			foreach (GameObject b2 in objs2)
			{
				Transform val2 = FindWeapon(b2);
				if ((Object)(object)val2 != (Object)null)
				{
					Guns[0].hands = ((Component)val2.parent).gameObject;
				}
			}
		}
		Gun[] guns = Guns;
		foreach (Gun gun in guns)
		{
			gun.EnableWeapon(enable: false);
		}
		Guns[0].EnableWeapon(enable: true);
	}

	[RPC]
	public void OnPlConnected(PhotonPlayer player)
	{
		photonView.RPC("OnLoaded", player, (int)ProfileINI.skin);
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
		MonoBehaviour.print((object)"receive Audio");
		if (TeamBattle.Instance.EnableVoiceChat)
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
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = WorldGameObject.INSTANCE.MainPlayer.transform.position;
		_CurDownCube = WorldData.Instance.GetBlockType((int)position.x, (int)position.z, (int)(position.y - 0.1f));
		if (_CurDownCube != BlockType.Air && _CurDownCube != BlockType.Water && _CurDownCube != BlockType.Lava && (_ForcedSound || Input.GetKey((KeyCode)119) || Input.GetKey((KeyCode)115) || Input.GetKey((KeyCode)97) || Input.GetKey((KeyCode)100)))
		{
			_ForcedSound = false;
			if (!WorldGameObject.INSTANCE.IsMainPlayerDead)
			{
				WorldGameObject.INSTANCE.photonView.RPC("SoundRPC", PhotonTargets.Others, (int)WorldGameObject.INSTANCE.BlockParametrs[(uint)_CurDownCube].FootEffect, WorldGameObject.INSTANCE.MainPlayer.gameObject.transform.position);
				ResourceLoader.I.PlayEffect(WorldGameObject.INSTANCE.BlockParametrs[(uint)_CurDownCube].FootEffect, ((Component)this).audio);
			}
		}
		((MonoBehaviour)this).Invoke("SoundFoots", 0.35f * (float)((!Crouch) ? 1 : 2));
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
			stream.SendNext(!WorldGameObject.INSTANCE.IsMainPlayerDead && !InTrolley && (Object)(object)Holder.Active == (Object)null);
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
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)bs._TeamBattle != (Object)null)
		{
			if (oldTeam != PlayerTeam)
			{
				_ = photonView;
				if (true)
				{
					((MonoBehaviour)this).StartCoroutine(OnPlayerSelectedTeam());
				}
			}
			oldTeam = PlayerTeam;
		}
		if (bs.DebugKey((KeyCode)116))
		{
			SetTransparent();
		}
		foreach (Transform item in ((IEnumerable<Transform>)new Transform[2] { MuzzleFlashFps, MuzzleFlashThird }).Where((Func<Transform, bool>)((Transform a) => (Object)(object)a != (Object)null)))
		{
			((Component)item).renderer.enabled = Time.time - LastTimeShoot < 0.05f;
		}
		_ = photonView;
		if (1 == 0)
		{
			if (!InVehicle)
			{
				((Component)this).transform.position = Vector3.Lerp(((Component)this).transform.position, _CorrectPlayerPos, Time.deltaTime * 5f);
			}
			((Component)this).transform.rotation = Quaternion.Lerp(((Component)this).transform.rotation, _CorrectPlayerRot, Time.deltaTime * 5f);
		}
		if ((Object)(object)WorldGameObject.INSTANCE.MainPlayer != (Object)null && _CurDownCube == BlockType.Air)
		{
			Camera mainPlayerEyes = WorldGameObject.INSTANCE.MainPlayerEyes;
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
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		_ = photonView;
		if (1 == 0 && _CameraRotEnabled)
		{
			if (_CameraRotX < 360f && _CameraRotX >= 270f)
			{
				_CameraRotX -= 360f;
			}
			_CurrentHeadX = Mathf.Lerp(_CurrentHeadX, (0f - _CameraRotX) / 3f - 90f + ((!Crouch) ? 0f : CrouchCamOffset), Time.deltaTime * 5f);
			Transform transform = HeadBone.transform;
			Quaternion rotation = HeadBone.transform.rotation;
			Vector3 eulerAngles = rotation.eulerAngles;
			float x = eulerAngles.x;
			Quaternion rotation2 = HeadBone.transform.rotation;
			Vector3 eulerAngles2 = rotation2.eulerAngles;
			transform.rotation = Quaternion.Euler(x, eulerAngles2.y, _CurrentHeadX);
			Transform transform2 = SpinBone.transform;
			Quaternion rotation3 = SpinBone.transform.rotation;
			Vector3 eulerAngles3 = rotation3.eulerAngles;
			float x2 = eulerAngles3.x;
			Quaternion rotation4 = SpinBone.transform.rotation;
			Vector3 eulerAngles4 = rotation4.eulerAngles;
			transform2.rotation = Quaternion.Euler(x2, eulerAngles4.y, _CurrentHeadX);
		}
	}

	[RPC]
	public void Die()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Expected Obj, but got Unknown
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
		if (KilledBy != null && KilledBy.ID == PhotonNetwork.player.ID)
		{
			Transform transform = ((Component)Camera.main).transform;
			GameObject val2 = (GameObject)Object.Instantiate((Object)(object)Igor.Instance.damageText, transform.position, transform.rotation);
			val2.transform.parent = ((Component)transform).transform;
			MultiKill++;
			if ((Object)(object)bs._TeamBattle != (Object)null)
			{
				bs._TeamBattle.OnPlayerDead2(KilledBy.name, photonView.owner.name, MultiKill);
			}
			val2.GetComponentInChildren<TextMesh>().text = MultiKill.ToString();
			Object.Destroy((Object)(object)val2, 10f);
		}
		if ((Object)(object)((Component)this).GetComponent<CameraController>() != (Object)null)
		{
			MultiKill = 0;
		}
		if (KilledBy == null)
		{
			return;
		}
		string playerName = bs._WorldGameObject.FindPlayer(photonView.owner).playerName;
		string playerName2 = bs._WorldGameObject.FindPlayer(KilledBy).playerName;
		if ((Object)(object)bs._TeamBattle != (Object)null && _KilledByGun != -1)
		{
			if (bs._TeamBattle.PlayerKilledPlayer.Count == 0)
			{
				bs._TeamBattle.AddedTime = Time.time;
			}
			bs._TeamBattle.PlayerKilledPlayer.Add(new PlayerKilledPlayer
			{
				Died = playerName,
				KilledBy = playerName2,
				GunID = _KilledByGun
			});
			if (bs._TeamBattle.PlayerKilledPlayer.Count > 3)
			{
				bs._TeamBattle.PlayerKilledPlayer.RemoveAt(0);
			}
		}
	}

	[RPC]
	public void HitBy(PhotonPlayer pl, int gunId)
	{
		KilledBy = pl;
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
		foreach (Transform item in ((IEnumerable<Transform>)new Transform[2] { MuzzleFlashFps, MuzzleFlashThird }).Where((Func<Transform, bool>)((Transform a) => (Object)(object)a != (Object)null)))
		{
			((Component)item).renderer.enabled = false;
		}
	}

	public IEnumerator OnPlayerSelectedTeam()
	{
		Debug.Log((object)("OnTeam Changed " + PlayerName + " " + PlayerTeam));
		if ((Object)(object)bs._TeamBattle != (Object)null && bs._TeamBattle.IsObserving(photonView.owner.name))
		{
			SelectWeapon(-1);
		}
		yield return null;
	}

	public void Reset(bool die)
	{
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Expected Obj, but got Unknown
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)((Component)this).GetComponent<CameraController>() == (Object)null && !die)
		{
			SetTransparent();
		}
		ResetTime = Time.time;
		Debug.Log((object)("PlayerNetwork " + ((!die) ? "reset" : "die") + " " + PlayerName));
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
}
