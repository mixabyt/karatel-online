using System.Collections.Generic;
using UnityEngine;

public class VoiceChatNetworkProxy : MonoBehaviour
{
	private static int networkIdCounter;

	private int assignedNetworkId = -1;

	private VoiceChatPlayer player;

	private Queue<VoiceChatPacket> packets = new Queue<VoiceChatPacket>(16);

	private void Start()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		if (((Component)this).networkView.isMine)
		{
			VoiceChatRecorder.Instance.NewSample += OnNewSample;
		}
		if (Network.isServer)
		{
			assignedNetworkId = ++networkIdCounter;
			((Component)this).networkView.RPC("SetNetworkId", ((Component)this).networkView.owner, new object[1] { assignedNetworkId });
		}
		if (Network.isClient && !((Component)this).networkView.isMine)
		{
			((Component)this).gameObject.AddComponent<AudioSource>();
			player = ((Component)this).gameObject.AddComponent<VoiceChatPlayer>();
		}
	}

	private void OnNewSample(VoiceChatPacket packet)
	{
		packets.Enqueue(packet);
	}

	[RPC]
	private void SetNetworkId(int networkId)
	{
		VoiceChatRecorder.Instance.NetworkId = networkId;
	}

	private void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		int count = packets.Count;
		if (stream.isWriting)
		{
			stream.Serialize(ref count);
			while (packets.Count > 0)
			{
				VoiceChatPacket packet = packets.Dequeue();
				stream.WritePacket(packet);
				if (packet.Data.Length == VoiceChatSettings.Instance.SampleSize)
				{
					VoiceChatBytePool.Instance.Return(packet.Data);
				}
			}
			return;
		}
		if (Network.isServer)
		{
			stream.Serialize(ref count);
			for (int i = 0; i < count; i++)
			{
				packets.Enqueue(stream.ReadPacket());
				if (Network.connections.Length < 2)
				{
					packets.Dequeue();
				}
			}
			return;
		}
		stream.Serialize(ref count);
		for (int j = 0; j < count; j++)
		{
			VoiceChatPacket packet2 = stream.ReadPacket();
			if ((Object)(object)player != (Object)null)
			{
				player.OnNewSample(packet2);
			}
		}
	}
}
