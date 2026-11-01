using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holder : EntityBase
{
	public static Holder Active;

	public Transform[] AttachPoints;

	public NetworkSyncAnimation.AnimState StayAnimation;

	private bool _IsCameraThirdPerson;

	private bool _IsCameraFreeLook;

	private bool[] _BusyAttachPoints;

	private int _BusyAttachPointIndex;

	protected override void Awake()
	{
		base.Awake();
		_BusyAttachPoints = new bool[AttachPoints.Length];
	}

	public override void OnButtonF(string playerName)
	{
		if ((Object)(object)Active == (Object)null)
		{
			PlayerEnter();
		}
	}

	protected override void Destruction()
	{
		if ((Object)(object)this == (Object)(object)Active)
		{
			PlayerExit(move: true);
		}
	}

	private void PlayerEnter()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		GameObject mainPlayer = WorldGameObject.Instance.MainPlayer;
		Transform attachPoint = GetAttachPoint(mainPlayer.transform.position);
		if (!((Object)(object)attachPoint == (Object)null))
		{
			_IsCameraThirdPerson = CameraController.Instance.IsThirdPerson;
			_IsCameraFreeLook = CameraController.Instance.IsFreeLook;
			if (!_IsCameraThirdPerson || !_IsCameraFreeLook)
			{
				CameraController.Instance.SetThirdPerson(freeLook: true, 0f, saveSettings: false);
			}
			CameraController.Instance.DisableCameraSwitching = true;
			if ((Object)(object)mainPlayer.GetComponent<PlayerMotor>() != (Object)null)
			{
				((Behaviour)mainPlayer.GetComponent<PlayerMotor>()).enabled = false;
			}
			if ((Object)(object)mainPlayer.GetComponent<PlayerInput>() != (Object)null)
			{
				mainPlayer.GetComponent<PlayerInput>().SetMovement(move: false);
			}
			mainPlayer.transform.position = attachPoint.position;
			mainPlayer.GetComponent<MouseLook>().RotationX = attachPoint.eulerAngles.y;
			mainPlayer.GetComponent<NetworkSyncAnimation>().PlayAnimation(StayAnimation);
			Active = this;
			((MonoBehaviour)this).StartCoroutine(PlayerMonitoringProcess());
		}
	}

	private void PlayerExit(bool move)
	{
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		CameraController.Instance.DisableCameraSwitching = false;
		if (_IsCameraThirdPerson)
		{
			CameraController.Instance.SetThirdPerson(_IsCameraFreeLook);
		}
		else
		{
			CameraController.Instance.SetFirstPerson();
		}
		GameObject mainPlayer = WorldGameObject.Instance.MainPlayer;
		if ((Object)(object)mainPlayer.GetComponent<PlayerMotor>() != (Object)null)
		{
			((Behaviour)mainPlayer.GetComponent<PlayerMotor>()).enabled = true;
		}
		if ((Object)(object)mainPlayer.GetComponent<PlayerInput>() != (Object)null)
		{
			mainPlayer.GetComponent<PlayerInput>().SetMovement(move: true);
		}
		if (move)
		{
			Transform transform = mainPlayer.transform;
			Bounds bounds = ((Component)this).renderer.bounds;
			transform.position = bounds.center;
		}
		_BusyAttachPoints[_BusyAttachPointIndex] = false;
		photonView.RPC("SetBusyPoint", PhotonTargets.Others, _BusyAttachPointIndex, false);
		Active = null;
		((MonoBehaviour)this).StopAllCoroutines();
	}

	private IEnumerator PlayerMonitoringProcess()
	{
		Vector3 pos = WorldGameObject.Instance.MainPlayer.transform.position;
		while (!((Object)(object)WorldGameObject.Instance.MainPlayer == (Object)null) && WorldGameObject.Instance.MainPlayer.GetComponent<NetworkSyncAnimation>().CurrentAnimation == StayAnimation && !(pos != WorldGameObject.Instance.MainPlayer.transform.position))
		{
			yield return (object)new WaitForEndOfFrame();
		}
		PlayerExit(pos == WorldGameObject.Instance.MainPlayer.transform.position);
	}

	private Transform GetAttachPoint(Vector3 pos)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		List<Transform> list = new List<Transform>(AttachPoints);
		List<Transform> list2 = new List<Transform>(AttachPoints);
		list2.Sort((Transform a, Transform b) =>
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			return (int)((Vector3.Distance(a.position, pos) - Vector3.Distance(b.position, pos)) * 1000f);
		});
		for (int num = 0; num < list2.Count; num++)
		{
			Transform val = list2[num];
			int num2 = list.IndexOf(val);
			if (!_BusyAttachPoints[num2])
			{
				_BusyAttachPointIndex = num2;
				_BusyAttachPoints[_BusyAttachPointIndex] = true;
				photonView.RPC("SetBusyPoint", PhotonTargets.Others, _BusyAttachPointIndex, true);
				return val;
			}
		}
		return null;
	}

	[RPC]
	private void SetBusyPoint(int index, bool busy)
	{
		_BusyAttachPoints[index] = busy;
	}
}
