using System.Collections;
using UnityEngine;

public class Gift : EntityBase
{
	public const float RespawnTime = 60f;

	private bool _CanRespawn = true;

	protected override void Awake()
	{
		base.Awake();
		((Component)this).gameObject.AddComponent<Rigidbody>();
		((Component)this).rigidbody.drag = 1f;
		((Component)this).rigidbody.isKinematic = true;
	}

	protected override void Creation(object[] data)
	{
		((Component)this).rigidbody.isKinematic = false;
	}

	public override object[] GetData()
	{
		return new object[1] { _CanRespawn };
	}

	protected override void Updating()
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		if (!GameType.BattleMode() || GameType.IsObserving())
		{
			return;
		}
		if (photonView.isMine && ((Component)this).transform.position.y < -200f)
		{
			PhotonNetwork.Destroy(((Component)this).gameObject);
		}
		Vector3 position = WorldGameObject.Instance.MainPlayer.transform.position;
		PlayerNetwork component = WorldGameObject.Instance.MainPlayer.GetComponent<PlayerNetwork>();
		Vector3 val = ((Component)this).transform.position - position;
		if (!(val.magnitude < 2f))
		{
			return;
		}
		SoundManager.Instance.PlayAtPoint(SoundManager.Sound.TakeGift, ((Component)this).transform.position);
		if (Type != EntityType.GIFT_ARMOR && Type != EntityType.GIFT_LIFE)
		{
			if (Type == EntityType.GIFT_AMMO)
			{
				component.CurGun.bulletsLeft = component.CurGun.bulletsMax;
			}
			else if (Type == EntityType.GIFT_GRENADE)
			{
				component.Grenade.bullets += 3;
			}
		}
		if (_CanRespawn)
		{
			((MonoBehaviour)WorldGameObject.Instance).StartCoroutine(RespawnGift());
		}
		else
		{
			SelfDelete();
		}
	}

	[RPC]
	public void Hide()
	{
		((Component)this).gameObject.SetActive(false);
	}

	[RPC]
	public void Show()
	{
		((Component)this).gameObject.SetActive(true);
	}

	private IEnumerator RespawnGift()
	{
		photonView.RPC("Hide", PhotonTargets.AllBuffered);
		yield return (object)new WaitForSeconds(60f);
		photonView.RPC("Show", PhotonTargets.AllBuffered);
	}
}
