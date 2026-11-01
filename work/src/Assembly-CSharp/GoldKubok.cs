using UnityEngine;

public class GoldKubok : EntityBase
{
	public GameObject Cubok;

	public GameObject partical;

	protected override void Creation(object[] data)
	{
		WorldGameObject.Instance.KubokCountInGame++;
		WorldGameObject.Instance.UpdateKubokText();
	}

	public override void OnLeftMouseHit(string player_name)
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		if (!App.Instance.Settings.isWatch)
		{
			if (Level.Instance.CanBuild)
			{
				if (Creator == player_name || Level.Instance.IsAdministrator())
				{
					Life--;
					if (Life > 0)
					{
						iTween.ShakePosition(((Component)this).gameObject, new Vector3(0.05f, 0.05f, 0.01f), 0.5f);
						SoundManager.Instance.Play(SoundManager.Sound.HitMetal, WorldGameObject.Instance.MainPlayer.gameObject.audio);
					}
					else
					{
						SelfDelete();
						SoundManager.Instance.Play(SoundManager.Sound.HitMetal, WorldGameObject.Instance.MainPlayer.gameObject.audio);
					}
				}
				else
				{
					Chat.SendInfo(ProfileINI.nickname + Localize.GetText("CANT_TAKE_AND_DESTROY_ITEMS"), forAll: false);
				}
			}
			else
			{
				Chat.SendWarning(Localize.GetText("CANT_EDIT_MAP"), forAll: false);
			}
		}
		else
		{
			Chat.SendWarning(Localize.GetText("CANT_EDIT_MAP_IN_WATCH"), forAll: false);
		}
	}

	protected override void Destruction()
	{
		WorldGameObject.Instance.KubokCountInGame--;
		WorldGameObject.Instance.UpdateKubokText();
		if (!Level.Instance.IsAdministrator() && !WorldGameObject.Instance.FindAllKubok && WorldGameObject.Instance.KubokFindInGame >= WorldGameObject.Instance.KubokCountInGame)
		{
			WorldGameObject.Instance.FindAllKubok = true;
		}
	}

	public override void OnButtonE(string player_name)
	{
		if (Level.Instance.IsAdministrator() && !App.Instance.Settings.isWatch)
		{
			WorldGameObject.Instance.TakeEnity(((Component)this).gameObject);
			return;
		}
		SoundManager.Instance.Play(SoundManager.Sound.Treasure, WorldGameObject.Instance.MainPlayer.gameObject.audio);
		iTween.FadeTo(((Component)this).gameObject, 0f, 0.5f);
		partical.GetComponent<ParticleEmitter>().emit = false;
		Object.Destroy((Object)(object)((Component)this).gameObject.GetComponent<BoxCollider>());
		WorldGameObject.Instance.KubokFindInGame++;
		WorldGameObject.Instance.UpdateKubokText();
		if (!WorldGameObject.Instance.FindAllKubok)
		{
			if (WorldGameObject.Instance.KubokFindInGame < WorldGameObject.Instance.KubokCountInGame)
			{
				Chat.SendInfo(Localize.GetText("CUP_PLAYER") + ProfileINI.nickname + Localize.GetText("CUP_PLAYER_FIND") + " [" + WorldGameObject.Instance.KubokFindInGame + "]", forAll: true);
			}
			else
			{
				Chat.SendInfo(Localize.GetText("CUP_PLAYER") + ProfileINI.nickname + Localize.GetText("CUP_PLAYER_FIND_ALL"), forAll: true);
				WorldGameObject.Instance.FindAllKubok = true;
			}
		}
	}
}
