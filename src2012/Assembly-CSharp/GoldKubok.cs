using UnityEngine;

public class GoldKubok : EntityBase
{
	public GameObject Cubok;

	public GameObject partical;

	protected override void Creation(object[] data)
	{
		WorldGameObject.INSTANCE.kubok_count_in_game++;
		WorldGameObject.INSTANCE.UpdateKubokText();
	}

	public override void OnLeftMouseHit(string player_name)
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		if (!App.instance.gameIni.isWatch)
		{
			if (Level.Instance.CanBuild)
			{
				if (Creator == player_name || Level.Instance.IsAdministrator())
				{
					Life--;
					if (Life > 0)
					{
						iTween.ShakePosition(((Component)this).gameObject, new Vector3(0.05f, 0.05f, 0.01f), 0.5f);
						ResourceLoader.I.PlayEffect(ResourceLoader.eSounds.HitMetal_3, WorldGameObject.INSTANCE.MainPlayer.gameObject.audio);
					}
					else
					{
						SelfDelete();
						ResourceLoader.I.PlayEffect(ResourceLoader.eSounds.HitMetal_3, WorldGameObject.INSTANCE.MainPlayer.gameObject.audio);
					}
				}
				else
				{
					ChatClient.instance.Info(ProfileINI.nickname + ", чужие вещи брать и разрушать запрещено");
				}
			}
			else
			{
				ChatClient.instance.Warning("Изменять карту нельзя, обратитесь к администратору карты, чтобы получить возможность строить");
			}
		}
		else
		{
			ChatClient.instance.SendWarningMessage("В режиме просмотра изменять карту нельзя");
		}
	}

	protected override void Destruction()
	{
		WorldGameObject.INSTANCE.kubok_count_in_game--;
		WorldGameObject.INSTANCE.UpdateKubokText();
		if (!Level.Instance.IsAdministrator() && !WorldGameObject.INSTANCE.find_all_kubok && WorldGameObject.INSTANCE.kubok_find_in_game >= WorldGameObject.INSTANCE.kubok_count_in_game)
		{
			WorldGameObject.INSTANCE.find_all_kubok = true;
		}
	}

	public override void OnButtonE(string player_name)
	{
		if (Level.Instance.IsAdministrator() && !App.instance.gameIni.isWatch)
		{
			WorldGameObject.INSTANCE.TakeEnity(((Component)this).gameObject);
			return;
		}
		ResourceLoader.I.PlayEffect(ResourceLoader.eSounds.Treasure_1, WorldGameObject.INSTANCE.MainPlayer.gameObject.audio);
		iTween.FadeTo(((Component)this).gameObject, 0f, 0.5f);
		partical.GetComponent<ParticleEmitter>().emit = false;
		Object.Destroy((Object)(object)((Component)this).gameObject.GetComponent<BoxCollider>());
		WorldGameObject.INSTANCE.kubok_find_in_game++;
		WorldGameObject.INSTANCE.UpdateKubokText();
		if (!WorldGameObject.INSTANCE.find_all_kubok)
		{
			if (WorldGameObject.INSTANCE.kubok_find_in_game < WorldGameObject.INSTANCE.kubok_count_in_game)
			{
				ChatClient.instance.SendInfoMessage("Игрок " + ProfileINI.nickname + " нашел кубок![" + WorldGameObject.INSTANCE.kubok_find_in_game + "]");
			}
			else
			{
				ChatClient.instance.SendInfoMessage("Игрок " + ProfileINI.nickname + " нашел все кубки!!!");
				WorldGameObject.INSTANCE.find_all_kubok = true;
			}
		}
	}
}
