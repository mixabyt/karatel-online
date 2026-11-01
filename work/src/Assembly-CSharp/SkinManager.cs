using System.Collections;
using UnityEngine;

public class SkinManager : MonoBehaviour
{
	public static string[] SkinNames = new string[58]
	{
		"Digger", "Digger_Hand", "Pirate", "Pirate_Hands", "Kinght", "Knight_Hands", "EpicKnight", "EpicKnight_Hands", "Pirate2", "Pirate2_Hands",
		"Viking", "Viking_Hands", "Zombie", "Zombie_Hands", "Death_Knight", "DeathKnight_Hands", "Soldier02", "Soldier_Hands", "Soviet", "Soviet_Hands",
		"Swat", "Swat_Hands", "Soldier03", "Soviet_Hands", "German_officer", "German_officer_Hands", "Knight01", "Knight_Hand", "Skeleton", "Skelet_Hand",
		"IronMan", "Ironman_Hands", "Batman", "Batman_Hands", "Digger_girl01", "DiggerGirl01_Hands", "Digger_girl02", "DiggerGirl02_Hands", "Archer", "Archer_Hands",
		" Terrorist", "Terrorist_Hands", "Cook", "Cook_Hands", "DarkStalker", "DarkStalker_Hands", "mercenary", "Mercenary_Hands", "Smelter", "Smelter_Hands",
		"Root", "Standard", "German", "German_Hands", "Soldier", "Standard", "Standard", "Standard"
	};

	public GameObject[] Skins;

	public GameObject[] Hands;

	private static Shader _TransparentShader = null;

	private float _CurAlpha = 1f;

	private Animation _Anim;

	private int _CurSkin = -1;

	public Animation Anim => _Anim;

	public int CurSkin => _CurSkin;

	private void Awake()
	{
		UpdateTextures();
		DisableAllSkins();
		DisableAllHands();
		if ((Object)(object)_TransparentShader == (Object)null)
		{
			_TransparentShader = Shader.Find("Transparent/VertexLit with Z");
		}
		TimeOfDay.Affect(((Component)this).gameObject);
	}

	public void DisableAllSkins()
	{
		for (int i = 0; i < Skins.Length; i++)
		{
			Skins[i].SetActive(false);
		}
		_CurSkin = -1;
		_Anim = null;
	}

	public void DisableAllHands()
	{
		for (int i = 0; i < Hands.Length; i++)
		{
			Hands[i].SetActive(false);
		}
		_Anim = null;
	}

	public void SetHand(int ind)
	{
		_Anim = ((Component)this).animation;
		for (int i = 0; i < Hands.Length; i++)
		{
			if (i == ind)
			{
				Hands[i].SetActive(true);
				if ((Object)(object)Hands[i].animation != (Object)null)
				{
					_Anim = Hands[i].animation;
				}
			}
			else
			{
				Hands[i].SetActive(false);
			}
		}
	}

	public void SetSkin(int ind)
	{
		_CurSkin = ind;
		_Anim = ((Component)this).animation;
		for (int i = 0; i < Skins.Length; i++)
		{
			if (i == ind)
			{
				Skins[i].SetActive(true);
				if ((Object)(object)Skins[i].animation != (Object)null)
				{
					_Anim = Skins[i].animation;
				}
			}
			else
			{
				Skins[i].SetActive(false);
			}
		}
	}

	public void SetTransparent(float alpha)
	{
		alpha = Mathf.Clamp01(alpha);
		if ((_CurAlpha < 1f && alpha == 1f) || (_CurAlpha == 1f && alpha < 1f))
		{
			MaterialExt.SetShader(((Component)this).gameObject, (!(alpha < 1f)) ? null : _TransparentShader);
		}
		_CurAlpha = alpha;
		MaterialExt.SetTransparent(((Component)this).gameObject, _CurAlpha);
	}

	public void AffectColor(float r, float g, float b)
	{
		MaterialExt.AffectColor(((Component)this).gameObject, r, g, b);
	}

	public void Redness()
	{
		((MonoBehaviour)this).StopCoroutine("RednessProcess");
		AffectColor(0f, 0f, 0f);
		((MonoBehaviour)this).StartCoroutine("RednessProcess");
	}

	private IEnumerator RednessProcess()
	{
		float duration = 0.25f;
		float startTime = Time.time;
		while (Time.time - startTime < duration)
		{
			AffectColor((Time.time - startTime) / duration, 0f, 0f);
			yield return (object)new WaitForSeconds(0.05f);
		}
		startTime = Time.time;
		while (Time.time - startTime < duration)
		{
			AffectColor(1f - (Time.time - startTime) / duration, 0f, 0f);
			yield return (object)new WaitForSeconds(0.05f);
		}
		AffectColor(0f, 0f, 0f);
	}

	public void UpdateTextures()
	{
		Utils.UpdateTextures(((Component)this).transform.Find("characters"), ContentUpdater.CharacterTextures);
		Utils.UpdateTextures(((Component)this).transform.Find("recoil/Main Camera/hands"), ContentUpdater.CharacterTextures);
		Utils.UpdateTextures(((Component)this).transform.Find("recoil/Main Camera/anims/anims"), ContentUpdater.WeaponTextures);
		Utils.UpdateTextures(((Component)this).transform.Find("Root/Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand/gunModels"), ContentUpdater.WeaponTextures);
	}
}
