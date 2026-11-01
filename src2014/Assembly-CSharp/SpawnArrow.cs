using UnityEngine;

public class SpawnArrow : EntityBase
{
	public static SpawnArrow CurSpawn;

	public int TeamIndex => (Type != EntityType.SPAWN_ARROW) ? ((Type == EntityType.TEAM_SPAWN_ARROW_RED) ? 1 : 2) : 0;

	protected override void Creation(object[] data)
	{
		if (Type == EntityType.SPAWN_ARROW)
		{
			if ((Object)(object)CurSpawn == (Object)null)
			{
				CurSpawn = this;
			}
			if (PhotonNetwork.isMasterClient && App.Instance.Settings.gameType != GameINI.GameType.BUILDING)
			{
				SelfDelete();
			}
		}
	}

	protected override void Destruction()
	{
		if (!((Object)(object)CurSpawn == (Object)(object)this))
		{
			return;
		}
		CurSpawn = null;
		SpawnArrow[] array = Object.FindObjectsOfType(typeof(SpawnArrow)) as SpawnArrow[];
		SpawnArrow[] array2 = array;
		foreach (SpawnArrow spawnArrow in array2)
		{
			if (spawnArrow.Type == EntityType.SPAWN_ARROW)
			{
				CurSpawn = spawnArrow;
				break;
			}
		}
	}

	protected override void Updating()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		if (Type == EntityType.SPAWN_ARROW && (Object)(object)CurSpawn != (Object)(object)this && (Object)(object)WorldGameObject.Instance.MainPlayerEyes != (Object)null)
		{
			Bounds bounds = ((Component)this).collider.bounds;
			if (bounds.Contains(((Component)WorldGameObject.Instance.MainPlayerEyes).transform.position))
			{
				CurSpawn = this;
				SoundManager.Instance.PlayAtPoint(SoundManager.Sound.CtfFlagCaptured, ((Component)WorldGameObject.Instance.MainPlayerEyes).transform.position);
				Chat.SendInfo(Localize.GetText("CHECKPOINT"), forAll: false);
			}
		}
	}

	public override void OnLeftMouseHit(string playerName)
	{
		if ((Object)(object)TeamBattle.Instance != (Object)null)
		{
			Chat.SendInfo(Localize.GetText("CANT_DESTROY_SPWAN"), forAll: false);
		}
		else
		{
			base.OnLeftMouseHit(playerName);
		}
	}

	public override void OnButtonE(string player_name)
	{
		if ((Object)(object)TeamBattle.Instance != (Object)null)
		{
			Chat.SendInfo(Localize.GetText("CANT_MOVE_SPWAN"), forAll: false);
		}
		else if (Level.Instance.IsAdministrator())
		{
			WorldGameObject.Instance.TakeEnity(((Component)this).gameObject);
		}
		else
		{
			Chat.SendInfo(Localize.GetText("CANT_MOVE_SPWAN_ADMIN"), forAll: false);
		}
	}
}
