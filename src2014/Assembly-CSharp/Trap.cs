using System.Collections.Generic;
using UnityEngine;

public class Trap : EntityBase
{
	private enum State
	{
		None,
		Delay,
		LiftUp,
		Stay,
		LiftDown
	}

	public float DelayTime;

	public float LiftUpTime;

	public float StayTime;

	public float LiftDownTime;

	public float LiftValue;

	public GameObject Stakes;

	public Transform[] StakesPeak;

	private State _State;

	private float _NextStateTime;

	private List<GameObject> _DamagedPlayers = new List<GameObject>();

	private Vector3 _StakesStartPosition = Vector3.zero;

	public Trap()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
	}

	protected override void Updating()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		if (_State == State.None)
		{
			return;
		}
		Vector3 val = _StakesStartPosition - Stakes.transform.position;
		float num = val.magnitude + 0.001f;
		int num2 = 1 << LayerMask.NameToLayer("Players");
		Transform[] stakesPeak = StakesPeak;
		RaycastHit val3 = default;
		foreach (Transform val2 in stakesPeak)
		{
			if (Physics.Raycast(val2.position - ((Component)this).transform.up * num, ((Component)this).transform.up, ref val3, num, num2) && !_DamagedPlayers.Contains(((Component)val3.collider).gameObject))
			{
				GameObject gameObject = ((Component)val3.collider).gameObject;
				_DamagedPlayers.Add(gameObject);
				if ((Object)(object)gameObject == (Object)(object)WorldGameObject.Instance.MainPlayer)
				{
					WorldGameObject.Instance.MainPlayerNode.DoDamage(100f);
				}
				break;
			}
		}
		if (_State == State.LiftUp || _State == State.LiftDown)
		{
			float num3 = Mathf.Clamp01((_NextStateTime - Time.time) / ((_State != State.LiftUp) ? LiftDownTime : LiftUpTime));
			if (_State == State.LiftUp)
			{
				num3 = 1f - num3;
			}
			Stakes.transform.position = _StakesStartPosition + ((Component)this).transform.up * (num3 * LiftValue);
		}
		if (Time.time >= _NextStateTime)
		{
			NextState();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!IsPreview && _State == State.None && ((Component)other).gameObject.layer == LayerMask.NameToLayer("Players"))
		{
			photonView.RPC("StartWork", PhotonTargets.All);
		}
	}

	[RPC]
	private void StartWork()
	{
		if (_State == State.None)
		{
			NextState();
		}
	}

	private void NextState()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		switch (_State)
		{
		case State.None:
			_State = State.Delay;
			_NextStateTime = Time.time + DelayTime;
			_StakesStartPosition = Stakes.transform.position;
			break;
		case State.Delay:
			_State = State.LiftUp;
			_NextStateTime = Time.time + LiftUpTime;
			break;
		case State.LiftUp:
			_State = State.Stay;
			_NextStateTime = Time.time + StayTime;
			break;
		case State.Stay:
			_State = State.LiftDown;
			_NextStateTime = Time.time + LiftDownTime;
			Stakes.transform.position = _StakesStartPosition + ((Component)this).transform.up * LiftValue;
			break;
		case State.LiftDown:
			_State = State.None;
			Stakes.transform.position = _StakesStartPosition;
			_DamagedPlayers.Clear();
			break;
		}
	}
}
