using System.Reflection;
using Photon;
using UnityEngine;

[AddComponentMenu("Miscellaneous/Photon View &v")]
public class PhotonView : MonoBehaviour
{
	public int subId;

	public int ownerId;

	public int group;

	protected internal bool mixedModeIsReliable;

	public int prefixBackup = -1;

	private object[] instantiationDataField;

	protected internal object[] lastOnSerializeDataSent;

	protected internal object[] lastOnSerializeDataReceived;

	public Component observed;

	public ViewSynchronization synchronization;

	public OnSerializeTransform onSerializeTransformOption = OnSerializeTransform.PositionAndRotation;

	public OnSerializeRigidBody onSerializeRigidBodyOption = OnSerializeRigidBody.All;

	public int instantiationId;

	public int isForcedSending;

	private bool didAwake;

	protected internal bool destroyedByPhotonNetworkOrQuit;

	private MethodInfo OnSerializeMethodInfo;

	private bool failedToFindOnSerialize;

	public int prefix
	{
		get
		{
			if (prefixBackup == -1 && PhotonNetwork.networkingPeer != null)
			{
				prefixBackup = PhotonNetwork.networkingPeer.currentLevelPrefix;
			}
			return prefixBackup;
		}
		set
		{
			prefixBackup = value;
		}
	}

	public object[] instantiationData
	{
		get
		{
			if (!didAwake)
			{
				instantiationDataField = PhotonNetwork.networkingPeer.FetchInstantiationData(instantiationId);
			}
			return instantiationDataField;
		}
		set
		{
			instantiationDataField = value;
		}
	}

	public int viewID
	{
		get
		{
			return ownerId * PhotonNetwork.MAX_VIEW_IDS + subId;
		}
		set
		{
			bool flag = didAwake && subId == 0;
			ownerId = value / PhotonNetwork.MAX_VIEW_IDS;
			subId = value % PhotonNetwork.MAX_VIEW_IDS;
			if (flag)
			{
				PhotonNetwork.networkingPeer.RegisterPhotonView(this);
			}
		}
	}

	public bool isSceneView => ownerId == 0;

	public PhotonPlayer owner => PhotonPlayer.Find(ownerId);

	public int OwnerActorNr => ownerId;

	public bool isMine
	{
		get
		{
			if (isForcedSending == -1)
			{
				return false;
			}
			if (isForcedSending == 1)
			{
				return true;
			}
			return ownerId == PhotonNetwork.player.ID || (isSceneView && PhotonNetwork.isMasterClient);
		}
	}

	public void Awake()
	{
		PhotonNetwork.networkingPeer.RegisterPhotonView(this);
		instantiationDataField = PhotonNetwork.networkingPeer.FetchInstantiationData(instantiationId);
		didAwake = true;
	}

	public void OnApplicationQuit()
	{
		destroyedByPhotonNetworkOrQuit = true;
	}

	public void OnDestroy()
	{
		if (!destroyedByPhotonNetworkOrQuit)
		{
			PhotonNetwork.networkingPeer.LocalCleanPhotonView(this);
		}
		if (!destroyedByPhotonNetworkOrQuit && !Application.isLoadingLevel)
		{
			if (instantiationId > 0)
			{
				Debug.LogError((object)string.Concat("OnDestroy() seems to be called without PhotonNetwork.Destroy()?! GameObject: ", ((Component)this).gameObject, " Application.isLoadingLevel: ", Application.isLoadingLevel));
			}
			else if (viewID <= 0)
			{
				Debug.LogWarning((object)$"OnDestroy manually allocated PhotonView {this}. The viewID is 0. Was it ever (manually) set?");
			}
			else if (isMine && !PhotonNetwork.manuallyAllocatedViewIds.Contains(viewID))
			{
				Debug.LogWarning((object)$"OnDestroy manually allocated PhotonView {this}. The viewID is local (isMine) but not in manuallyAllocatedViewIds list. Use UnAllocateViewID() after you destroyed the PV.");
			}
		}
		if (PhotonNetwork.networkingPeer.instantiatedObjects.ContainsKey(instantiationId))
		{
			GameObject val = PhotonNetwork.networkingPeer.instantiatedObjects[instantiationId];
			bool flag = (Object)(object)val == (Object)(object)((Component)this).gameObject;
			if (flag)
			{
				Debug.LogWarning((object)string.Format("OnDestroy for PhotonView {0} but GO is still in instantiatedObjects. instantiationId: {1}. Use PhotonNetwork.Destroy(). {2} Identical with this: {3} PN.Destroyed called for this PV: {4}", this, instantiationId, (!Application.isLoadingLevel) ? string.Empty : "Loading new scene caused this.", flag, destroyedByPhotonNetworkOrQuit));
			}
		}
	}

	protected internal void ExecuteOnSerialize(PhotonStream pStream, PhotonMessageInfo info)
	{
		if (failedToFindOnSerialize)
		{
			return;
		}
		if ((object)OnSerializeMethodInfo == null)
		{
			Component val = observed;
			if (!NetworkingPeer.GetMethod((MonoBehaviour)(object)((val is MonoBehaviour) ? val : null), PhotonNetworkingMessage.OnPhotonSerializeView.ToString(), out OnSerializeMethodInfo))
			{
				Debug.LogError((object)("The observed monobehaviour (" + ((Object)observed).name + ") of this PhotonView does not implement OnPhotonSerializeView()!"));
				failedToFindOnSerialize = true;
				return;
			}
		}
		OnSerializeMethodInfo.Invoke(observed, new object[2] { pStream, info });
	}

	public void RPC(string methodName, PhotonTargets target, params object[] parameters)
	{
		if (PhotonNetwork.networkingPeer.hasSwitchedMC && target == PhotonTargets.MasterClient)
		{
			PhotonNetwork.RPC(this, methodName, PhotonNetwork.masterClient, parameters);
		}
		else
		{
			PhotonNetwork.RPC(this, methodName, target, parameters);
		}
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

	public override string ToString()
	{
		return string.Format("View ({3}){0} on {1} {2}", viewID, (!((Object)(object)((Component)this).gameObject != (Object)null)) ? "GO==null" : ((Object)((Component)this).gameObject).name, (!isSceneView) ? string.Empty : "(scene)", prefix);
	}
}
