using System.Collections;
using UnityEngine;

public class Gift : EntityBase
{
	public GiftType giftType;

	public bool Respawn;

	public float respawnTime = 10f;

	protected override void Creation(object[] data)
	{
		if (data != null && data.Length >= 1)
		{
			Respawn = (bool)data[0];
		}
	}

	public override object[] GetData()
	{
		return new object[1] { Respawn };
	}

	[RPC]
	public void Drop()
	{
		Rigidbody val = ((Component)this).gameObject.AddComponent<Rigidbody>();
		val.drag = 1f;
		((MonoBehaviour)this).StartCoroutine(bs.AddMethod(10f, () =>
		{
			Object.Destroy((Object)(object)((Component)this).rigidbody);
		}));
	}

	protected override void Updating()
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (!bs._Igor.enableGuns || bs._Igor.spectate)
		{
			return;
		}
		_ = photonView;
		if (true && ((Component)this).transform.position.y < -200f)
		{
			PhotonNetwork.Destroy(((Component)this).gameObject);
		}
		Vector3 position = bs._WorldGameObject.MainPlayer.transform.position;
		PlayerNetwork component = bs._WorldGameObject.MainPlayer.GetComponent<PlayerNetwork>();
		Vector3 val = ((Component)this).transform.position - position;
		if (val.magnitude < 2f)
		{
			((Component)component).audio.PlayOneShot(bs._Igor.takeSound, ProfileINI.sound_volume * ProfileINI.sound_scale);
			if (giftType == GiftType.armor)
			{
				bs._WorldGameObject.MainPlayerNode.Shield = 1f;
			}
			if (giftType == GiftType.life)
			{
				bs._WorldGameObject.MainPlayerNode.Life = 1f;
			}
			if (giftType == GiftType.bullets)
			{
				component.CurGun.bulletsLeft = component.CurGun.bulletsMax;
			}
			if (giftType == GiftType.grenade)
			{
				component.Grenade.bullets += 3;
			}
			if (Respawn)
			{
				((MonoBehaviour)bs._WorldGameObject).StartCoroutine(RespawnGift());
				return;
			}
			photonView.RPC("Destroy", PhotonTargets.MasterClient);
			Hide();
		}
	}

	[RPC]
	public void Destroy()
	{
		PhotonNetwork.Destroy(((Component)this).gameObject);
	}

	public void LateUpdate()
	{
		if ((Object)(object)((Component)this).rigidbody != (Object)null)
		{
			((Component)this).rigidbody.isKinematic = !WorldGameObject.INSTANCE.IsWorldGenerated;
		}
	}

	[RPC]
	public void Hide()
	{
		((Component)this).gameObject.SetActiveRecursively(false);
	}

	[RPC]
	public void Show()
	{
		((Component)this).gameObject.SetActiveRecursively(true);
	}

	private IEnumerator RespawnGift()
	{
		photonView.RPC("Hide", PhotonTargets.AllBuffered);
		yield return (object)new WaitForSeconds(respawnTime);
		photonView.RPC("Show", PhotonTargets.AllBuffered);
	}
}
