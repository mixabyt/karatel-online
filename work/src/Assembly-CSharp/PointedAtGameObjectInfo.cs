using UnityEngine;

[RequireComponent(typeof(InputToEvent))]
public class PointedAtGameObjectInfo : MonoBehaviour
{
	private void OnGUI()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)InputToEvent.goPointedAt != (Object)null)
		{
			PhotonView photonView = InputToEvent.goPointedAt.GetPhotonView();
			if ((Object)(object)photonView != (Object)null)
			{
				GUI.Label(new Rect(Input.mousePosition.x + 5f, (float)Screen.height - Input.mousePosition.y - 15f, 300f, 30f), string.Format("ViewID {0} InstID {1} Lvl {2} {3}", photonView.viewID, photonView.instantiationId, photonView.prefix, photonView.isSceneView ? "scene" : ((!photonView.isMine) ? ("owner: " + photonView.ownerId) : "mine")));
			}
		}
	}
}
