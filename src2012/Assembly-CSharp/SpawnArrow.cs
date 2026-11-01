using UnityEngine;

public class SpawnArrow : EntityBase
{
	public int TeamIndex => (Type != EntityType.SPAWN_ARROW) ? ((Type == EntityType.TEAM_SPAWN_ARROW_RED) ? 1 : 2) : 0;

	protected override void PreviewCreation(object[] data)
	{
		RefreshColor();
	}

	protected override void Creation(object[] data)
	{
		RefreshColor();
	}

	public void RefreshColor()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		Color teamColor = TeamBattle.GetTeamColor(TeamIndex);
		MaterialExt.SetColor(((Component)this).gameObject, teamColor.r, teamColor.g, teamColor.b);
	}

	public override void OnLeftMouseHit(string playerName)
	{
		if ((Object)(object)TeamBattle.Instance != (Object)null)
		{
			ChatClient.instance.Info("Нельзя разрушать точки спавна");
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
			ChatClient.instance.Info("Во время командного боя точку возрождения передвигать нельзя");
		}
		else if (Level.Instance.IsAdministrator())
		{
			WorldGameObject.INSTANCE.TakeEnity(((Component)this).gameObject);
		}
		else
		{
			ChatClient.instance.Info("Точку возрождения может передвигать только администратор карты");
		}
	}
}
