using UnityEngine;

public class VoiceChatUnityClient : MonoBehaviour
{
	private VoiceChatNetworkProxy proxy;

	public int Port = 15000;

	public string Address = "127.0.0.1";

	private void Start()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		Network.Connect(Address, Port);
	}

	private void OnConnectedToServer()
	{
		proxy = VoiceChatUtils.CreateProxy();
	}

	private void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		Object.Destroy((Object)(object)((Component)proxy).gameObject);
	}
}
