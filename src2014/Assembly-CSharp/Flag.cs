using System.Linq;
using UnityEngine;

public class Flag : EntityBase
{
	private Vector3 _StartPos;

	private Quaternion _StartRot;

	private Transform _PoleNode;

	private Flag _OtherFlag;

	private PlayerNetwork _Owner;

	private bool _CapturedTip;

	private bool _MarkerEnabled;

	private Vector3 _MarkerPos;

	private Vector3 _Position = Vector3.zero;

	private Quaternion _Rotation = Quaternion.identity;

	public bool IsBlue => Type == EntityType.FLAG_BLUE;

	private bool _Captured => (Object)(object)_Owner != (Object)null;

	private bool _CapturedByMe => (Object)(object)_Owner != (Object)null && (Object)(object)((Component)_Owner).gameObject == (Object)(object)WorldGameObject.Instance.MainPlayer;

	private string _MarkerName => (!IsBlue) ? "hud.battle.flags.red" : "hud.battle.flags.blue";

	public Flag()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
	}

	protected override void Creation(object[] data)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		_StartPos = ((Component)this).transform.position;
		_StartRot = ((Component)this).transform.rotation;
		_Position = ((Component)this).transform.position;
		_Rotation = ((Component)this).transform.rotation;
		_PoleNode = ((Component)this).transform.Find("pole");
		_OtherFlag = Object.FindObjectsOfType(typeof(Flag)).Cast<Flag>().FirstOrDefault((Flag a) => (Object)(object)a != (Object)(object)this);
		_MarkerEnabled = false;
		KGUI.EnableNodes(_MarkerName, enable: true);
		KGUI.EnableNodes(_MarkerName, enable: false);
	}

	protected override void Updating()
	{
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)TeamBattle.Instance == (Object)null || !(TeamBattle.Instance is Ctf))
		{
			if (GameType.BattleMode())
			{
				SelfDelete();
			}
			return;
		}
		if ((Object)(object)((Component)_PoleNode).rigidbody == (Object)null)
		{
			((Component)_PoleNode).gameObject.AddComponent<Rigidbody>().constraints = (RigidbodyConstraints)112;
		}
		((Component)_PoleNode).rigidbody.isKinematic = (Object)(object)_Owner != (Object)null || !photonView.isMine;
		PlayerNetwork component = WorldGameObject.Instance.MainPlayer.GetComponent<PlayerNetwork>();
		bool mainPlayerDead = WorldGameObject.Instance.MainPlayerDead;
		if (!component.IsObserver && Vector3.Distance(component.pos, _PoleNode.position) < 3f && !_Captured && !mainPlayerDead)
		{
			if (component.IsBlueTeam == IsBlue && Vector3.Distance(_StartPos, _PoleNode.position) > 3f)
			{
				photonView.RPC("ResetPos", PhotonTargets.All, false);
			}
			if (component.IsBlueTeam != IsBlue)
			{
				photonView.RPC("SetOwner", PhotonTargets.All, component.photonView.viewID);
			}
		}
		if (_CapturedByMe && (mainPlayerDead || component.IsObserver || component.IsBlueTeam == IsBlue))
		{
			photonView.RPC("DropFlag", PhotonTargets.All, _Owner.FlagPlaceHolder.position);
		}
		if (photonView.isMine && Vector3.Distance(_PoleNode.position, _OtherFlag._StartPos) < 3f && !_OtherFlag._Captured)
		{
			int teamIndex = ((!_OtherFlag.IsBlue) ? 1 : 2);
			TeamBattle.Instance.AddTeamScore(teamIndex, 1);
			TeamBattle.Instance.ModifyPlayerData(teamIndex, _Owner.photonView.owner.name, 30, 0, 0);
			photonView.RPC("ResetPos", PhotonTargets.All, true);
		}
		if ((Object)(object)_Owner != (Object)null)
		{
			_PoleNode.position = _Owner.FlagPlaceHolder.position;
			_PoleNode.rotation = _Owner.FlagPlaceHolder.rotation;
			((Component)_PoleNode).gameObject.SetActive(!((Object)(object)_Owner == (Object)(object)component) || CameraController.Instance.IsThirdPerson);
		}
		else if (!photonView.isMine)
		{
			((Component)this).transform.position = Vector3.Lerp(((Component)this).transform.position, _Position, Time.deltaTime * 5f);
			((Component)this).transform.rotation = Quaternion.Lerp(((Component)this).transform.rotation, _Rotation, Time.deltaTime * 5f);
		}
		else
		{
			FixWorldBounds();
		}
		bool flag = false;
		if ((Object)(object)_Owner == (Object)null)
		{
			Vector3 position = _PoleNode.position;
			Vector3 up = _PoleNode.up;
			Bounds bounds = ((Component)_PoleNode).collider.bounds;
			Vector3 val = position + up * bounds.size.y * 1.25f;
			Vector3 val2 = CameraController.RaycastCamera.WorldToScreenPoint(val);
			if (_MarkerPos != val2)
			{
				_MarkerPos = val2;
				KGUI.FindNode(_MarkerName).localPosition = Utils.ScreenToNGUIPos(val2);
			}
			if (_MarkerPos.z >= 0f)
			{
				flag = true;
			}
		}
		if (_MarkerEnabled != flag)
		{
			_MarkerEnabled = flag;
			KGUI.EnableNodes(_MarkerName, flag);
		}
		if (_CapturedByMe)
		{
			if (!_CapturedTip)
			{
				_CapturedTip = true;
				KGUI.EnableNodes("hud.battle.txt_flag_captured_tip", enable: true);
			}
			((Component)KGUI.FindNode("hud.battle.txt_flag_captured_tip")).GetComponent<UIWidget>().alpha = 0.3f + Mathf.PingPong(Time.time, 0.7f);
		}
		else if (_CapturedTip)
		{
			_CapturedTip = false;
			KGUI.EnableNodes("hud.battle.txt_flag_captured_tip", enable: false);
		}
	}

	private void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
	{
		if (PhotonNetwork.isMasterClient && (Object)(object)_Owner != (Object)null)
		{
			photonView.RPC("SetOwner", newPlayer, _Owner.photonView.viewID);
		}
	}

	[RPC]
	public void ResetPos(bool delivered)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		_Owner = null;
		((Component)_PoleNode).gameObject.SetActive(true);
		_PoleNode.position = _StartPos;
		_PoleNode.rotation = _StartRot;
		CtfMessage((!delivered) ? "RETURNED" : "DELIVERED");
	}

	[RPC]
	public void DropFlag(Vector3 pos)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		_Owner = null;
		((Component)_PoleNode).gameObject.SetActive(true);
		_PoleNode.position = pos;
		_PoleNode.rotation = Quaternion.Euler(0f, _PoleNode.eulerAngles.y, 0f);
		CtfMessage("DROPPED");
	}

	[RPC]
	public void SetOwner(int playerPhotonViewID)
	{
		PhotonView photonView = PhotonView.Find(playerPhotonViewID);
		_Owner = ((Component)photonView).GetComponent<PlayerNetwork>();
		((Component)_PoleNode).gameObject.SetActive(true);
		CtfMessage("CAPTURED");
	}

	private void CtfMessage(string message)
	{
		int playerTeam = TeamBattle.Instance.GetPlayerTeam();
		string text;
		if (playerTeam == 1 || playerTeam == 2)
		{
			bool flag = (playerTeam == 1 && !IsBlue) || (playerTeam == 2 && IsBlue);
			text = ((!flag) ? "OTHER" : "OUR");
			if (message == "CAPTURED")
			{
				SoundManager.Instance.Play((!flag) ? SoundManager.Sound.CtfFlagCaptured : SoundManager.Sound.CtfFlagStolen);
			}
			else if (message == "DELIVERED")
			{
				SoundManager.Instance.Play(SoundManager.Sound.CtfFlagDelivered);
			}
		}
		else
		{
			text = (IsBlue ? "BLUE" : "RED");
			if (message == "CAPTURED")
			{
				SoundManager.Instance.Play(SoundManager.Sound.CtfFlagCaptured);
			}
			else if (message == "DELIVERED")
			{
				SoundManager.Instance.Play(SoundManager.Sound.CtfFlagDelivered);
			}
		}
		Chat.SendInfo(Localize.GetText("CTF_FLAG_" + message + "_" + text), forAll: false);
	}

	public override void OnLeftMouseHit(string playerName)
	{
		if (!((Object)(object)TeamBattle.Instance != (Object)null))
		{
			base.OnLeftMouseHit(playerName);
		}
	}

	public override void OnButtonE(string playerName)
	{
		if (!((Object)(object)TeamBattle.Instance != (Object)null))
		{
			base.OnButtonE(playerName);
		}
	}

	protected virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (stream.isWriting)
		{
			stream.SendNext(((Component)this).transform.position);
			stream.SendNext(((Component)this).transform.rotation);
		}
		else
		{
			_Position = (Vector3)stream.ReceiveNext();
			_Rotation = (Quaternion)stream.ReceiveNext();
		}
	}
}
