using UnityEngine;

public class OnClickInstantiate : MonoBehaviour
{
	public GameObject Prefab;

	public int InstantiateType;

	private string[] InstantiateTypeNames = new string[2] { "Mine", "Scene" };

	public bool showGui;

	private void OnClick()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		if (PhotonNetwork.connectionStateDetailed == PeerState.Joined)
		{
			switch (InstantiateType)
			{
			case 0:
				PhotonNetwork.Instantiate(((Object)Prefab).name, InputToEvent.inputHitPos + new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
				break;
			case 1:
				PhotonNetwork.InstantiateSceneObject(((Object)Prefab).name, InputToEvent.inputHitPos + new Vector3(0f, 5f, 0f), Quaternion.identity, 0, null);
				break;
			}
		}
	}

	private void OnGUI()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (showGui)
		{
			GUILayout.BeginArea(new Rect((float)(Screen.width - 180), 0f, 180f, 50f));
			InstantiateType = GUILayout.Toolbar(InstantiateType, InstantiateTypeNames, new GUILayoutOption[0]);
			GUILayout.EndArea();
		}
	}
}
