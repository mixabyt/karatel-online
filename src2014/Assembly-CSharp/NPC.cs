using System.Collections;
using UnityEngine;

public class NPC : EntityBase
{
	public const float ShowLifeBarDistance = 10f;

	public const float LifeBarWidth = 150f;

	public const float LifeBarHeight = 15f;

	public const float DeathTransparentTime = 2f;

	public const int PunchChancePercent = 33;

	public Texture RectGreenTexture;

	public Texture RectRedTexture;

	protected byte _InitialLife;

	private float _CurLifeValue = 1f;

	private float _CurLifeAlpha;

	private CharacterController _CharacterController;

	private AI _AI;

	private Vector3 _Position = Vector3.zero;

	private Quaternion _Rotation = Quaternion.identity;

	public NPC()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
	}

	protected override void Creation(object[] data)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		SkinManager component = ((Component)this).GetComponent<SkinManager>();
		if ((Object)(object)component != (Object)null)
		{
			component.SetSkin((data != null) ? ((int)data[0]) : 0);
		}
		_InitialLife = Life;
		_CharacterController = ((Component)this).GetComponent<CharacterController>();
		_AI = ((Component)this).GetComponent<AI>();
		_Position = ((Component)this).transform.position;
		_Rotation = ((Component)this).transform.rotation;
	}

	protected override void PreviewCreation(object[] data)
	{
		SkinManager component = ((Component)this).GetComponent<SkinManager>();
		if ((Object)(object)component != (Object)null)
		{
			component.SetSkin((data != null) ? ((int)data[0]) : 0);
		}
		Object.Destroy((Object)(object)((Component)this).GetComponent<AI>());
	}

	protected override void Updating()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if (!photonView.isMine)
		{
			((Component)this).transform.position = Vector3.Lerp(((Component)this).transform.position, _Position, Time.deltaTime * 5f);
			((Component)this).transform.rotation = Quaternion.Lerp(((Component)this).transform.rotation, _Rotation, Time.deltaTime * 5f);
		}
	}

	private void OnGUI()
	{
	}

	public override void OnLeftMouseHit(string playerName)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (Level.Instance.IsAdministrator())
		{
			photonView.RPC("Hit", PhotonTargets.All, ((Component)CameraController.RaycastCamera).transform.forward);
		}
		else
		{
			photonView.RPC("Punch", PhotonTargets.All, ((Component)CameraController.RaycastCamera).transform.forward);
		}
	}

	[RPC]
	private void Punch(Vector3 cameraDir)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (PhotonNetwork.isMasterClient)
		{
			cameraDir.y = 0.2f;
			_AI.Punch(cameraDir.normalized * 10f, 0.2f);
		}
	}

	[RPC]
	private void Hit(Vector3 cameraDir, PhotonMessageInfo info)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if (Life <= 0)
		{
			return;
		}
		Life--;
		if (!PhotonNetwork.isMasterClient)
		{
			return;
		}
		if (Random.Range(0, 100) < 33)
		{
			_AI.Punch(cameraDir.normalized * 10f, 0.2f);
		}
		if (Life <= 0)
		{
			photonView.RPC("Dying", PhotonTargets.All);
			if ((Object)(object)Survival.Instance != (Object)null)
			{
				Survival.Instance.OnKillEnemy(info.sender);
			}
		}
	}

	private void HitRedness()
	{
		SkinManager component = ((Component)this).GetComponent<SkinManager>();
		if ((Object)(object)component != (Object)null)
		{
			component.Redness();
		}
	}

	[RPC]
	private void Dying()
	{
		((MonoBehaviour)this).StartCoroutine(DyingProcess());
	}

	private IEnumerator DyingProcess()
	{
		Object.Destroy((Object)(object)_AI);
		SkinManager skinManager = ((Component)this).GetComponent<SkinManager>();
		if ((Object)(object)skinManager != (Object)null)
		{
			NetworkSyncAnimation networkAnim = ((Component)this).GetComponent<NetworkSyncAnimation>();
			if (PhotonNetwork.isMasterClient && (Object)(object)networkAnim != (Object)null && (Object)(object)networkAnim.Anim != (Object)null && (TrackedReference)(object)networkAnim.Anim["death"] != (TrackedReference)null)
			{
				networkAnim.PlayAnimation(NetworkSyncAnimation.AnimState.death);
				yield return (object)new WaitForSeconds(networkAnim.Anim["death"].length);
			}
			if ((Object)(object)_CharacterController != (Object)null)
			{
				Object.Destroy((Object)(object)_CharacterController);
				_CharacterController = null;
			}
			float startTime = Time.time;
			while (true)
			{
				float alpha = 1f - (Time.time - startTime) / 2f;
				if (alpha < 0f)
				{
					break;
				}
				skinManager.SetTransparent(alpha);
				yield return (object)new WaitForSeconds(0.01f);
			}
		}
		if (PhotonNetwork.isMasterClient)
		{
			SelfDelete();
		}
	}

	protected virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (stream.isWriting)
		{
			stream.SendNext(((Component)this).transform.position);
			stream.SendNext(((Component)this).transform.rotation);
			stream.SendNext(Life);
		}
		else
		{
			_Position = (Vector3)stream.ReceiveNext();
			_Rotation = (Quaternion)stream.ReceiveNext();
			Life = (byte)stream.ReceiveNext();
		}
	}
}
