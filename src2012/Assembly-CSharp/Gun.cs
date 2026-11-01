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

	public int ArrayId;

	public bool pistol;

	public GameObject hands;

	public GameObject weapon;

	public Bullet bullet;

	public bool melee;

	public SecuredValue<float> shootInterval = 0.1f;

	public SecuredValue<int> bulletsMax = 90;

	public SecuredValue<int> bulletsLeft = 0;

	public SecuredValue<int> bulletsMag = 30;

	public SecuredValue<float> RecoilUp = 0.03f;

	public SecuredValue<float> RecoilRand = 0.015f;

	public SecuredValue<float> RecoilRecover = 3f;

	public SecuredValue<float> CrouchRecoilFactor = 0.5f;

	public SecuredValue<float> accuracy = 0f;

	public SecuredValue<int> shootBullets = 1;

	public SecuredValue<float> damage = 0.1f;

	public SecuredValue<float> bulletSpeed = 100f;

	public SecuredValue<float> reloadSpeed = 1f;

	public SecuredValue<float> HeadHit = 2f;

	public SecuredValue<float> BulletGravity = 0f;

	public int ragdollForce = 1;

	public AudioClip shootSound;

	public AudioClip reloadSound;

	public SecuredValue<bool> sniper = false;

	public SecuredValue<bool> bazuka = false;

	public SecuredValue<bool> grenade = false;

	public SecuredValue<float> expDist = 5f;

	public float explosionTime = 10f;

	public Vector3 GrenadeThrowForce;

	internal Vector3 handsPos;

	internal CameraController playerCC;

	internal PlayerNetwork pl;

	private static int rand = new Random().Next(-1000, 1000);

	internal int m_bullets = rand;

	private Transform MuzzleFlashThird;

	private Transform MuzzleFlashFps;

	internal int bullets
	{
		get
		{
			return m_bullets - rand;
		}
		set
		{
			m_bullets = value + rand;
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
		EnableVeapon2(state: false);
	}

	public void EnableWeapon(bool enable)
	{
		if (enable)
		{
			if ((Object)(object)weapon != (Object)null)
			{
				MuzzleFlashThird = (pl.MuzzleFlashThird = weapon.transform.Find("muzzleFlash"));
			}
			if ((Object)(object)hands != (Object)null)
			{
				MuzzleFlashFps = (pl.MuzzleFlashFps = hands.transform.Find("muzzleFlash"));
			}
		}
		DisableMuzleFlash();
		EnableVeapon2(enable);
	}

	private void EnableVeapon2(bool state)
	{
		ShowWep(state);
		weapon.SetActiveRecursively(false);
	}

	public void ShowWep(bool state)
	{
		foreach (GameObject item in ((IEnumerable<GameObject>)new GameObject[2] { hands, weapon }).Where((Func<GameObject, bool>)((GameObject a) => (Object)(object)a != (Object)null)))
		{
			foreach (Transform item2 in from a in bs.GetTransforms(item.transform)
				where (Object)(object)((Component)a).renderer != (Object)null
				select a)
			{
				((Component)item2).renderer.enabled = state;
			}
		}
	}

	public void DisableMuzleFlash()
	{
		foreach (Transform item in ((IEnumerable<Transform>)new Transform[2] { MuzzleFlashFps, MuzzleFlashThird }).Where((Func<Transform, bool>)((Transform a) => (Object)(object)a != (Object)null)))
		{
			((Component)item).renderer.enabled = false;
		}
	}
}
