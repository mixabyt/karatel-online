using Photon;
using UnityEngine;

[AddComponentMenu("Miscellaneous/Photon View")]
public class PhotonView : MonoBehaviour
{
	[SerializeField]
	private int sceneViewID;

	[SerializeField]
	private PhotonViewID ID = new PhotonViewID(0, null);

	public Component observed;

	public ViewSynchronization synchronization;

	public int group;

	public short prefix = -1;

	public object[] instantiationData;

	protected internal object[] lastOnSerializeDataSent;

	protected internal object[] lastOnSerializeDataReceived;

	public OnSerializeTransform onSerializeTransformOption = OnSerializeTransform.PositionAndRotation;

	public OnSerializeRigidBody onSerializeRigidBodyOption = OnSerializeRigidBody.All;

	private bool registeredPhotonView;

	public int isForcedSending;

	private bool ranSetup;

	public PhotonViewID viewID
	{
		get
		{
			if (!ranSetup)
			{
				Setup();
			}
			if (ID.ID < 1 && sceneViewID > 0)
			{
				ID = new PhotonViewID(sceneViewID, null);
			}
			return ID;
		}
		set
		{
			if (!ranSetup)
			{
				Setup();
			}
			if (registeredPhotonView && PhotonNetwork.networkingPeer != null)
			{
				PhotonNetwork.networkingPeer.RemovePhotonView(this, mayFail: true);
			}
			ID = value;
			if (PhotonNetwork.networkingPeer != null)
			{
				PhotonNetwork.networkingPeer.RegisterPhotonView(this);
				registeredPhotonView = true;
			}
		}
	}

	public bool isSceneView => sceneViewID > 0 || (ID.owner == null && ID.ID > 0 && ID.ID < PhotonNetwork.MAX_VIEW_IDS);

	public PhotonPlayer owner
	{
		get
		{
			if (!ranSetup)
			{
				Setup();
			}
			return viewID.owner;
		}
	}

	public bool isMine
	{
		get
		{
			if (!ranSetup)
			{
				Setup();
			}
			if (isForcedSending == -1)
			{
				return false;
			}
			if (isForcedSending == 1)
			{
				return true;
			}
			return owner == PhotonNetwork.player || (isSceneView && PhotonNetwork.isMasterClient);
		}
	}

	public override string ToString()
	{
		return string.Format("View {0} on {1} {2}", ID.ID, ((Object)((Component)this).gameObject).name, (!isSceneView) ? string.Empty : "(scene)");
	}

	public void Awake()
	{
		Setup();
	}

	private void Setup()
	{
		if (!Application.isPlaying || ranSetup)
		{
			return;
		}
		ranSetup = true;
		if (isSceneView)
		{
			if (PhotonNetwork.networkingPeer.PhotonViewSetup_FindMatchingRoot(((Component)this).gameObject))
			{
				sceneViewID = 0;
				return;
			}
			if (sceneViewID < 1)
			{
				Debug.LogError((object)("SceneView " + sceneViewID));
			}
			ID = new PhotonViewID(sceneViewID, null);
			registeredPhotonView = true;
			PhotonNetwork.networkingPeer.RegisterPhotonView(this);
		}
		else if (!PhotonNetwork.networkingPeer.PhotonViewSetup_FindMatchingRoot(((Component)this).gameObject) && PhotonNetwork.logLevel != PhotonLogLevel.ErrorsOnly)
		{
			Debug.LogWarning((object)("Warning: Did not find the root of a PhotonView. This is only OK if you used GameObject.Instantiate to instantiate this prefab. Object: " + ((Object)this).name));
		}
	}

	private void OnDestroy()
	{
		PhotonNetwork.networkingPeer.RemovePhotonView(this, mayFail: true);
	}

	public void RPC(string methodName, PhotonTargets target, params object[] parameters)
	{
		PhotonNetwork.RPC(this, methodName, target, parameters);
	}

	public void RPC(string methodName, PhotonPlayer targetPlayer, params object[] parameters)
	{
		PhotonNetwork.RPC(this, methodName, targetPlayer, parameters);
	}

	public static PhotonView Get(Component component)
	{
		return component.GetComponent<PhotonView>();
	}

	public static PhotonView Get(GameObject gameObj)
	{
		return gameObj.GetComponent<PhotonView>();
	}

	public static PhotonView Find(int viewID)
	{
		return PhotonNetwork.networkingPeer.GetPhotonView(viewID);
	}
}
