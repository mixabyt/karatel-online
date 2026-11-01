using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Gun : bs
{
	public StorePurchase storePurchase = StorePurchase.WEAPON_GLOK;

	public Texture2D weaponIcon;

	public Texture2D weaponIconGrey;

	public Texture2D weaponIconBig;

	public Texture2D bulletIcon;

	public bool pistol;

	public GameObject hands;

	public GameObject weapon;

	public Bullet bullet;

	public bool melee;

	public bool rifle;

	public int ragdollForce = 1;

	public AudioClip shootSound;

	public AudioClip reloadSound;

	public float explosionTime = 10f;

	public Vector3 GrenadeThrowForce;

	internal int arrayId;

	internal Vector3 handsPos;

	internal CameraController playerCC;

	internal PlayerNetwork pl;

	private static int rand = new Random().Next(-1000, 1000);

	internal int m_bullets = rand;

	internal SecuredValue<float> shootInterval = 0.1f;

	internal SecuredValue<int> bulletsMax = 90;

	internal SecuredValue<int> bulletsLeftEx = 0;

	internal SecuredValue<int> bulletsMag = 30;

	internal SecuredValue<float> CrouchRecoilFactor = 0.5f;

	internal SecuredValue<float> accuracy = 0f;

	internal SecuredValue<int> shootBullets = 1;

	internal SecuredValue<float> damage = 0.1f;

	internal SecuredValue<float> bulletSpeed = 100f;

	internal SecuredValue<float> reloadSpeed = 1f;

	internal SecuredValue<float> HeadHit = 2f;

	internal SecuredValue<float> BulletGravity = 0f;

	internal SecuredValue<bool> sniper = false;

	internal SecuredValue<bool> bazuka = false;

	internal SecuredValue<bool> grenade = false;

	internal SecuredValue<float> expDist = 5f;

	private Transform muzzleFlashThird;

	private Transform muzzleFlashFirst;

	internal int bullets
	{
		get
		{
			return m_bullets - rand;
		}
		set
		{
			m_bullets = value + rand;
			KGUI.SetControlText("hud.battle.txt_bullets", value + "/" + bulletsLeft);
		}
	}

	internal int bulletsLeft
	{
		get
		{
			return bulletsLeftEx;
		}
		set
		{
			bulletsLeftEx = value;
			KGUI.SetControlText("hud.battle.txt_bullets", bullets + "/" + value);
		}
	}

	internal float RecoilUp
	{
		get
		{
			return Getfloat("recup", 0.03f);
		}
		set
		{
			Setfloat("recup", value);
		}
	}

	internal float RecoilRand
	{
		get
		{
			return Getfloat("recRand", 0.015f);
		}
		set
		{
			Setfloat("recRand", value);
		}
	}

	internal float RecoilRecover
	{
		get
		{
			return Getfloat("recRec", 3f);
		}
		set
		{
			Setfloat("recRec", value);
		}
	}

	private void Awake()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		playerCC = ((Component)((Component)this).transform.root).GetComponent<CameraController>();
		pl = ((Component)((Component)this).transform.root).GetComponent<PlayerNetwork>();
		if ((Object)(object)hands != (Object)null)
		{
			handsPos = hands.transform.localPosition;
		}
		Reset();
	}

	public void Reset()
	{
		bullets = bulletsMag;
		bulletsLeft = bulletsMax;
	}

	public void EnableWeapon(bool enable)
	{
		if (enable)
		{
			if ((Object)(object)weapon != (Object)null)
			{
				muzzleFlashThird = (pl.MuzzleFlashThird = weapon.transform.Find("muzzleFlash"));
			}
			if ((Object)(object)hands != (Object)null)
			{
				muzzleFlashFirst = (pl.MuzzleFlashFirst = hands.transform.Find("muzzleFlash"));
			}
		}
		DisableMuzzleFlash();
		foreach (GameObject item in ((IEnumerable<GameObject>)new GameObject[2] { hands, weapon }).Where((Func<GameObject, bool>)((GameObject a) => (Object)(object)a != (Object)null)))
		{
			foreach (Transform item2 in from a in bs.GetTransforms(item.transform)
				where (Object)(object)((Component)a).renderer != (Object)null
				select a)
			{
				((Component)item2).renderer.enabled = enable;
			}
		}
		DisableMuzzleFlash();
		pl.DisableMuzzleflash();
	}

	public void DisableMuzzleFlash()
	{
		if ((Object)(object)muzzleFlashFirst != (Object)null)
		{
			((Component)muzzleFlashFirst).renderer.enabled = false;
		}
		if ((Object)(object)muzzleFlashThird != (Object)null)
		{
			((Component)muzzleFlashThird).renderer.enabled = false;
		}
	}
}
