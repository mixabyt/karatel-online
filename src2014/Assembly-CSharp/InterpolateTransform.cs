using Photon;
using UnityEngine;

public class InterpolateTransform : MonoBehaviour
{
	internal struct State
	{
		internal double timestamp;

		internal Vector3 pos;

		internal Quaternion rot;
	}

	public double interpolationBackTime = 0.1;

	private State[] m_BufferedState = new State[20];

	private int m_TimestampCount;

	private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		if (stream.isWriting)
		{
			Vector3 obj = ((Component)this).transform.localPosition;
			Quaternion obj2 = ((Component)this).transform.localRotation;
			stream.Serialize(ref obj);
			stream.Serialize(ref obj2);
			return;
		}
		Vector3 obj3 = Vector3.zero;
		Quaternion obj4 = Quaternion.identity;
		stream.Serialize(ref obj3);
		stream.Serialize(ref obj4);
		for (int num = m_BufferedState.Length - 1; num >= 1; num--)
		{
			ref State reference = ref m_BufferedState[num];
			reference = m_BufferedState[num - 1];
		}
		State state = default;
		state.timestamp = info.timestamp;
		state.pos = obj3;
		state.rot = obj4;
		m_BufferedState[0] = state;
		m_TimestampCount = Mathf.Min(m_TimestampCount + 1, m_BufferedState.Length);
		for (int i = 0; i < m_TimestampCount - 1; i++)
		{
			if (m_BufferedState[i].timestamp < m_BufferedState[i + 1].timestamp)
			{
				Debug.Log((object)"State inconsistent");
			}
		}
	}

	private void Update()
	{
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		double time = PhotonNetwork.time;
		double num = time - interpolationBackTime;
		if (m_BufferedState[0].timestamp > num)
		{
			for (int i = 0; i < m_TimestampCount; i++)
			{
				if (m_BufferedState[i].timestamp <= num || i == m_TimestampCount - 1)
				{
					State state = m_BufferedState[Mathf.Max(i - 1, 0)];
					State state2 = m_BufferedState[i];
					double num2 = state.timestamp - state2.timestamp;
					float num3 = 0f;
					if (num2 > 0.0001)
					{
						num3 = (float)((num - state2.timestamp) / num2);
					}
					((Component)this).transform.localPosition = Vector3.Lerp(state2.pos, state.pos, num3);
					((Component)this).transform.localRotation = Quaternion.Slerp(state2.rot, state.rot, num3);
					break;
				}
			}
		}
		else
		{
			State state3 = m_BufferedState[0];
			((Component)this).transform.localPosition = state3.pos;
			((Component)this).transform.localRotation = state3.rot;
		}
	}
}
