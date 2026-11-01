using System;
using System.Collections.Generic;
using Photon;
using UnityEngine;

public class EntityBase : MonoBehaviour
{
	public static List<EntityBase> Entities = new List<EntityBase>();

	public static int LayerEntity = LayerMask.NameToLayer("Entity");

	public static int LayerSmallDecor = LayerMask.NameToLayer("SmallDecor");

	public static int LayerStorePreview = LayerMask.NameToLayer("StorePreview");

	[NonSerialized]
	public string Name;

	[NonSerialized]
	public EntityType Type;

	[NonSerialized]
	public string Creator;

	[NonSerialized]
	public byte Life = 4;

	protected float _ShakeTime;

	protected Vector3 _ShakeOffset = Vector3.zero;

	public int BlockX
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return (int)((Component)this).transform.position.x;
		}
	}

	public int BlockY
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return (int)((Component)this).transform.position.z;
		}
	}

	public int BlockZ
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return (int)((Component)this).transform.position.y;
		}
	}

	public bool IsPreview => photonView.instantiationData == null;

	public EntityBase()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
	}

	protected virtual void Awake()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected Obj, but got Unknown
		UpdateTextures();
		foreach (Transform item in ((Component)this).transform)
		{
			Transform node = item;
			FindColliders(node);
		}
	}

	protected virtual void Creation(object[] data)
	{
	}

	protected virtual void PreviewCreation(object[] data)
	{
	}

	private void Start()
	{
		if (!IsPreview)
		{
			if ((Object)(object)WorldGameObject.Instance != (Object)null && WorldGameObject.Instance.IsWorldGenerated)
			{
				InitializeInternal();
			}
			else
			{
				Entities.Add(this);
			}
		}
	}

	private void InitializeInternal()
	{
		FixWorldBounds();
		TimeOfDay.Affect(((Component)this).gameObject);
		Type = (EntityType)(int)photonView.instantiationData[0];
		Name = WorldGameObject.Instance.EnityParametrs[(int)Type].name;
		Creator = (string)photonView.instantiationData[1];
		if (WorldGameObject.Instance.EnityParametrs[(int)Type].small)
		{
			Utils.SetLayerRecursively(((Component)this).gameObject, LayerSmallDecor);
		}
		else
		{
			Utils.SetLayerRecursively(((Component)this).gameObject, LayerEntity);
		}
		object[] array = null;
		if (photonView.instantiationData.Length > 3)
		{
			array = new object[photonView.instantiationData.Length - 3];
			Array.Copy(photonView.instantiationData, 3, array, 0, array.Length);
		}
		Entities.Add(this);
		Creation(array);
	}

	public void InitializePreview(EntityType type, object[] data)
	{
		Type = type;
		Name = WorldGameObject.Instance.EnityParametrs[(int)Type].name;
		Utils.SetLayerRecursively(((Component)this).gameObject, LayerStorePreview);
		PreviewCreation(data);
	}

	public static void OnWorldGenerated()
	{
		EntityBase[] array = Entities.ToArray();
		Entities.Clear();
		EntityBase[] array2 = array;
		foreach (EntityBase entityBase in array2)
		{
			entityBase.InitializeInternal();
		}
	}

	protected virtual void Updating()
	{
	}

	protected virtual void PreviewUpdating()
	{
	}

	private void Update()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		if (!WorldGameObject.Instance.IsWorldGenerated)
		{
			return;
		}
		if (!IsPreview)
		{
			Updating();
		}
		else
		{
			PreviewUpdating();
		}
		if (_ShakeTime != 0f)
		{
			Transform transform = ((Component)this).transform;
			transform.position -= _ShakeOffset;
			if (Time.time >= _ShakeTime)
			{
				_ShakeTime = 0f;
				_ShakeOffset = Vector3.zero;
			}
			else
			{
				_ShakeOffset = new Vector3(Random.Range(-0.05f, 0.05f), Random.Range(-0.05f, 0.05f), Random.Range(-0.01f, 0.01f));
				Transform transform2 = ((Component)this).transform;
				transform2.position += _ShakeOffset;
			}
		}
	}

	protected virtual void Destruction()
	{
	}

	protected virtual void PreviewDestruction()
	{
	}

	public virtual void SelfDelete()
	{
		WorldGameObject.Instance.photonView.RPC("DeleteEntityNetworkAll", PhotonTargets.All, photonView.viewID);
		WorldGameObject.Instance.photonView.RPC("DeleteEntityNetworkMasterClient", PhotonTargets.MasterClient, photonView.viewID);
	}

	private void OnDestroy()
	{
		if (IsPreview)
		{
			PreviewDestruction();
		}
	}

	public virtual void OnLeftMouseHit(string playerName)
	{
		if (Life == 0)
		{
			return;
		}
		if (Creator == playerName || Level.Instance.IsAdministrator())
		{
			Life--;
			if (Life > 0)
			{
				_ShakeTime = Time.time + 0.5f;
			}
			else
			{
				SelfDelete();
			}
		}
		else
		{
			Chat.SendInfo(ProfileINI.nickname + Localize.GetText("CANT_DESTROY_ITEMS"), forAll: false);
		}
	}

	public virtual void OnButtonE(string playerName)
	{
		if (!((Object)(object)TeamBattle.Instance != (Object)null))
		{
			if (Creator == playerName || Level.Instance.IsAdministrator() || Level.Instance.IsModerator())
			{
				WorldGameObject.Instance.TakeEnity(((Component)this).gameObject);
			}
			else
			{
				Chat.SendInfo(ProfileINI.nickname + Localize.GetText("CANT_TAKE_AND_DESTROY_ITEMS"), forAll: false);
			}
		}
	}

	public virtual void OnButtonF(string playerName)
	{
	}

	protected void FixWorldBounds()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = ((Component)this).transform.position;
		if (position.x < 0f)
		{
			((Component)this).transform.position = new Vector3(0f, ((Component)this).transform.position.y, ((Component)this).transform.position.z);
		}
		else if (position.x >= (float)WorldData.Instance.WidthInBlocks)
		{
			((Component)this).transform.position = new Vector3((float)(WorldData.Instance.WidthInBlocks - 1), ((Component)this).transform.position.y, ((Component)this).transform.position.z);
		}
		if (position.z < 0f)
		{
			((Component)this).transform.position = new Vector3(((Component)this).transform.position.x, ((Component)this).transform.position.y, 0f);
		}
		else if (position.z >= (float)WorldData.Instance.HeightInBlocks)
		{
			((Component)this).transform.position = new Vector3(((Component)this).transform.position.x, ((Component)this).transform.position.y, (float)(WorldData.Instance.HeightInBlocks - 1));
		}
		if (position.y < 0f)
		{
			((Component)this).transform.position = new Vector3(((Component)this).transform.position.x, 0f, ((Component)this).transform.position.z);
		}
		else if (position.y >= (float)WorldData.Instance.DepthInBlocks)
		{
			((Component)this).transform.position = new Vector3(((Component)this).transform.position.x, (float)(WorldData.Instance.DepthInBlocks - 1), ((Component)this).transform.position.z);
		}
	}

	public static EntityBase FindEntity(GameObject go)
	{
		EntityBase entityBase = go.GetComponent<EntityBase>();
		if ((Object)(object)entityBase == (Object)null)
		{
			EntityBaseChild component = go.GetComponent<EntityBaseChild>();
			if ((Object)(object)component != (Object)null)
			{
				entityBase = component.Parent;
			}
		}
		return entityBase;
	}

	public virtual object[] GetData()
	{
		return null;
	}

	public void UpdateTextures()
	{
		Utils.UpdateTextures(((Component)this).transform, ContentUpdater.EntitiesTextures);
	}

	private void FindColliders(Transform node)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected Obj, but got Unknown
		if ((Object)(object)((Component)node).collider != (Object)null)
		{
			if ((Object)(object)((Component)node).GetComponent<EntityBaseChild>() == (Object)null)
			{
				((Component)node).gameObject.AddComponent<EntityBaseChild>();
			}
			((Component)node).GetComponent<EntityBaseChild>().Parent = this;
		}
		foreach (Transform item in node)
		{
			Transform node2 = item;
			FindColliders(node2);
		}
	}
}
