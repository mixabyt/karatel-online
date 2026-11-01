using UnityEngine;

public class VoiceChatUnityServer : MonoBehaviour
{
	public int Port = 15000;

	public int MaxConnections = 8;

	private void Start()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		Network.InitializeServer(MaxConnections, Port, false);
		Object.Destroy((Object)(object)((Component)this).GetComponent<VoiceChatUnityClient>());
	}

	private void OnPlayerDisconnected(NetworkPlayer player)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}
}
