using UnityEngine;

public class PlayerNode
{
	public const float LavaLifeDamage = 0.4f;

	public string Name;

	public string ViewerID;

	public int Skin;

	public PhotonPlayer NetPlayer;

	public GameObject Avatar;

	public int Level = 1;

	public SecuredValue<float> LifeEx;

	public SecuredValue<float> ShieldEx;

	public float Life
	{
		get
		{
			return LifeEx;
		}
		set
		{
			LifeEx = value;
			KGUI.SetControlText("hud.battle.txt_health", (int)(value * 100f) + string.Empty);
		}
	}

	public float MaxLife => 1f + ((!IsZombie) ? 0f : 0f);

	public float Shield
	{
		get
		{
			return ShieldEx;
		}
		set
		{
			ShieldEx = value;
			KGUI.SetControlText("hud.battle.txt_shield", (int)(value * 100f) + string.Empty);
		}
	}

	public float MaxShield => 1f;

	public float UnarmedDamage => 0.5f * ((!IsZombie) ? 1f : 2f);

	public bool IsZombie => (Object)(object)TeamBattle.Instance != (Object)null && TeamBattle.Instance is ZombieVirus && TeamBattle.Instance.GetPlayerTeam(Name) == 1;

	public PlayerNode()
	{
		Life = MaxLife;
		Shield = 0f;
	}

	public void DoDamage(float value)
	{
		if (Life <= 0f)
		{
			return;
		}
		if (Shield > 0f)
		{
			Shield -= value;
			if (Shield < 0f)
			{
				value = 0f - Shield;
				Shield = 0f;
			}
			else
			{
				value = 0f;
			}
		}
		Life -= value;
		if (Life < 0f)
		{
			Life = 0f;
		}
		if (Life <= 0f && this == WorldGameObject.Instance.MainPlayerNode)
		{
			WorldGameObject.Instance.OnMainPlayerDead();
			WorldGameObject.Instance.MainPlayer.GetComponent<PhotonView>().RPC("Die", PhotonTargets.All);
		}
	}
}
