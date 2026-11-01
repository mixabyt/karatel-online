using UnityEngine;

public class PlayerNode
{
	public const float InitialLife = 1f;

	public const float InitialShield = 0f;

	public const float LavaLifeDamage = 0.5f;

	public string playerName;

	public string viewer_id;

	public int skin;

	public PhotonPlayer networkPlayer;

	public GameObject avatar;

	internal float Life;

	internal float Shield;

	public PlayerNode()
	{
		Life = 1f;
		Shield = 0f;
	}

	public void DoDamage(float value)
	{
		if (!(Life <= 0f))
		{
			Life = DoShieldDamage(value);
			if (Life < 0f)
			{
				Life = 0f;
			}
			if (Life <= 0f && this == WorldGameObject.INSTANCE.MainPlayerNode)
			{
				WorldGameObject.INSTANCE.OnMainPlayerDead();
				WorldGameObject.INSTANCE.MainPlayer.GetComponent<PhotonView>().RPC("Die", PhotonTargets.All);
			}
		}
	}

	private float DoShieldDamage(float value)
	{
		if (Shield > 0f && Random.value > 0.25f)
		{
			Shield -= value;
			value = 0f;
			if (Shield < 0f)
			{
				value = Mathf.Abs(Shield);
				Shield = 0f;
			}
		}
		return Life - value;
	}

	public void LavaDamage()
	{
		DoDamage(0.5f);
	}
}
