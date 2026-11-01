using UnityEngine;

public class AviaBomb : EntityBase
{
	public GameObject DetonateEffect;

	public GameObject BobmLamp;

	public bool is_start_detonating;

	private bool is_start_activate;

	public override void OnButtonE(string player_name)
	{
		if (Creator == player_name || Level.Instance.IsAdministrator())
		{
			if (!is_start_activate)
			{
				WorldGameObject.Instance.TakeEnity(((Component)this).gameObject);
			}
		}
		else
		{
			Localize.GetText(ProfileINI.nickname + Localize.GetText("CANT_TAKE_AND_DESTROY_ITEMS"));
		}
	}

	public override void OnButtonF(string player_name)
	{
		if (Level.Instance.IsAdministrator() || Level.Instance.IsModerator())
		{
			photonView.RPC("DetonateRPC", PhotonTargets.MasterClient);
			photonView.RPC("DetonateShowToOther", PhotonTargets.All);
		}
	}

	[RPC]
	private void DetonateShowToOther()
	{
		((Renderer)BobmLamp.GetComponent<MeshRenderer>()).enabled = true;
		is_start_activate = true;
	}

	[RPC]
	private void DetonateRPC()
	{
		if (!is_start_detonating)
		{
			((MonoBehaviour)this).Invoke("Detonate", 5f);
			is_start_detonating = true;
		}
	}

	public void ChainDetonate()
	{
		if (!is_start_detonating)
		{
			((MonoBehaviour)this).Invoke("Detonate", 0.6f);
			is_start_detonating = true;
		}
	}

	protected override void Destruction()
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		if (is_start_activate)
		{
			if (!WorldGameObject.Instance.IsWorldGenerated)
			{
				DestroyAreaAt((int)((Component)this).gameObject.transform.position.x, (int)((Component)this).gameObject.transform.position.z, (int)((Component)this).gameObject.transform.position.y, 8, is_world_generated: false);
				return;
			}
			Object.Instantiate((Object)(object)DetonateEffect, ((Component)this).gameObject.transform.position, Quaternion.identity);
			DestroyAreaAt((int)((Component)this).gameObject.transform.position.x, (int)((Component)this).gameObject.transform.position.z, (int)((Component)this).gameObject.transform.position.y, 8, is_world_generated: true);
		}
	}

	public void Detonate()
	{
		SelfDelete();
		ChainReaction();
	}

	public void ChainReaction()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		foreach (EntityBase entity in EntityBase.Entities)
		{
			if ((Object)(object)entity == (Object)(object)this)
			{
				continue;
			}
			if (entity.Type == EntityType.TNT)
			{
				Vector3 val = ((Component)this).gameObject.transform.position - ((Component)entity).gameObject.transform.position;
				if (val.sqrMagnitude < 400f)
				{
					((Component)entity).GetComponent<TNT>().ChainDetonate();
				}
			}
			else if (entity.Type == EntityType.ATOMBOMB)
			{
				Vector3 val2 = ((Component)this).gameObject.transform.position - ((Component)entity).gameObject.transform.position;
				if (val2.sqrMagnitude < 400f)
				{
					((Component)entity).GetComponent<AviaBomb>().ChainDetonate();
				}
			}
			else
			{
				Vector3 val3 = ((Component)this).gameObject.transform.position - ((Component)entity).gameObject.transform.position;
				if (val3.sqrMagnitude < 400f)
				{
					entity.SelfDelete();
				}
			}
		}
	}

	public void DestroyAreaAt(int blockX, int blockY, int blockZ, int radius, bool is_world_generated)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		for (int i = blockX - radius; i <= blockX + radius; i++)
		{
			for (int j = blockY - radius; j <= blockY + radius; j++)
			{
				for (int k = blockZ - radius; k <= blockZ + radius; k++)
				{
					if (!(Vector3.Distance(new Vector3((float)blockX, (float)blockY, (float)blockZ), new Vector3((float)i, (float)j, (float)k)) <= (float)radius))
					{
						continue;
					}
					if (is_world_generated)
					{
						WorldData.Instance.SetBlockTypeWithRegeneration(i, j, k, BlockType.Air);
						if (Level.Instance.IsAdministrator())
						{
							World.Instance.CurLevelDelta.Add(BlockType.Air, i, j, k);
						}
					}
					else
					{
						WorldGameObject.Instance.LevelDeltaLoading.Add(BlockType.Air, i, j, k);
					}
				}
			}
		}
		World.Instance.Lighting.RecalculateLightingAroundBlock(blockX, blockY, blockZ, radius, lightChanged: true);
	}
}
