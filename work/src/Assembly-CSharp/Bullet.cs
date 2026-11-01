using UnityEngine;

public class Bullet : bs
{
	internal Gun gun;

	internal float throwFactorSpeed;

	private void Start()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		Object.Destroy((Object)(object)((Component)this).gameObject, gun.explosionTime);
		if ((bool)gun.grenade)
		{
			((Component)this).transform.Translate(gun.GrenadeThrowForce.normalized);
			((Component)this).rigidbody.velocity = ((Component)this).transform.rotation * gun.GrenadeThrowForce * Mathf.Clamp01(throwFactorSpeed);
			((Component)this).rigidbody.angularVelocity = Random.insideUnitSphere;
		}
		if ((bool)gun.bazuka)
		{
			((Component)this).audio.volume = ProfileINI.sound_volume * ProfileINI.sound_scale;
			((Component)this).audio.Play();
		}
	}

	private void OnDestroy()
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		if ((!gun.grenade && !gun.bazuka) || !((Object)(object)gun.playerCC != (Object)null))
		{
			return;
		}
		bs._WorldGameObject.photonView.RPC("Explosion", PhotonTargets.All, pos);
		Object[] array = Object.FindObjectsOfType(typeof(PlayerNetwork));
		for (int i = 0; i < array.Length; i++)
		{
			PlayerNetwork playerNetwork = (PlayerNetwork)(object)array[i];
			Vector3 val = playerNetwork.pos - pos;
			if (val.magnitude < (float)gun.expDist && (TeamBattle.Instance.IsOpposite(playerNetwork.PlayerName) || (Object)(object)((Component)playerNetwork).GetComponent<CameraController>() != (Object)null))
			{
				PhotonView component = ((Component)playerNetwork).GetComponent<PhotonView>();
				object[] array2 = new object[2];
				Vector3 val2 = playerNetwork.pos - pos;
				array2[0] = val2.normalized * 1000f * (float)gun.ragdollForce;
				array2[1] = pos;
				component.RPC("OnHit", PhotonTargets.All, array2);
				bs._WorldGameObject.PunchPlayer(playerNetwork.photonView.owner, playerNetwork.pos - pos, gun.damage, gun.arrayId);
			}
		}
		Vector3 val3 = ((Component)this).transform.position + ((!gun.bazuka) ? Vector3.down : Vector3.zero);
		int num = 1;
		int num2 = (int)val3.x;
		int num3 = (int)val3.z;
		int num4 = (int)val3.y;
		if (App.Instance.Settings.destroyable && num2 >= 0 && num3 >= 0 && num4 >= 0 && num2 < WorldData.Instance.WidthInBlocks && num3 < WorldData.Instance.HeightInBlocks && num4 < WorldData.Instance.DepthInBlocks)
		{
			bs._WorldGameObject.photonView.RPC("RemoveBlocksAt", PhotonTargets.All, num2, num3, num4, num);
		}
	}

	private void Update()
	{
		Update2(Time.deltaTime);
	}

	private void OnCollisionEnter(Collision collision)
	{
		PlayOneShot(bs._Igor.grenadeHit);
	}

	public void Update2(float deltaTime)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		if ((bool)gun.grenade)
		{
			return;
		}
		if ((float)gun.BulletGravity > 0f)
		{
			Transform transform = ((Component)this).transform;
			transform.forward += Vector3.down * (float)gun.BulletGravity * deltaTime;
		}
		Vector3 val = ((Component)this).transform.forward * (float)gun.bulletSpeed * deltaTime;
		Debug.DrawRay(((Component)this).transform.position, val.normalized);
		RaycastHit val2 = default;
		if (Physics.Raycast(((Component)this).transform.position, val, ref val2, val.magnitude + 2f, (1 << LayerMask.NameToLayer("Terrain")) | (1 << LayerMask.NameToLayer("Players"))) && ((Component)val2.collider).tag != "Water")
		{
			if ((Object)(object)gun.playerCC != (Object)null && !gun.bazuka)
			{
				PlayerNode playerNode = bs._WorldGameObject.FindPlayerByAvatar(((Component)val2.collider).gameObject);
				if (playerNode != null)
				{
					if (TeamBattle.Instance.IsOpposite(playerNode.NetPlayer.name))
					{
						SecuredValue<float> securedValue = gun.damage;
						Vector3 val3 = val2.point - playerNode.Avatar.GetComponent<PlayerNetwork>().Head.position;
						if (val3.magnitude < 0.5f)
						{
							Vector3 val4 = val2.point - playerNode.Avatar.GetComponent<PlayerNetwork>().Head.position;
							MonoBehaviour.print((object)("headHit " + val4.magnitude));
							securedValue = (float)securedValue * (float)gun.HeadHit;
						}
						playerNode.Avatar.GetComponent<PhotonView>().RPC("OnHit", PhotonTargets.All, val.normalized * 1000f * (float)gun.ragdollForce, val2.point);
						bs._WorldGameObject.PunchPlayer(playerNode.NetPlayer, val.normalized, securedValue, gun.arrayId);
					}
				}
				else
				{
					Object.Instantiate((Object)(object)bs._Igor.concerne, val2.point, Quaternion.LookRotation(val2.normal));
					Vector3 val5 = val2.point - val2.normal * 0.1f;
					IntVect targetLocation = new IntVect((int)val5.x, (int)val5.z, (int)val5.y);
					BlockType blockType = WorldData.Instance.GetBlockType(targetLocation.X, targetLocation.Y, targetLocation.Z);
					if (blockType != BlockType.Air && blockType != BlockType.Water)
					{
						World.Instance.Dig2(targetLocation, val2.point, (float)gun.damage * 2f);
					}
				}
			}
			AudioSource.PlayClipAtPoint(bs._Igor.hitSound[Random.Range(0, bs._Igor.hitSound.Length)], val2.point, ProfileINI.sound_volume * ProfileINI.sound_scale);
			((Component)this).transform.position = val2.point;
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
		else
		{
			Transform transform2 = ((Component)this).transform;
			transform2.position += val;
		}
	}
}
