using System.Collections;
using UnityEngine;

public class SkinManager : MonoBehaviour
{
	public SkinElement[] Skins;

	public SkinElement[] Hands;

	private static Shader _TransparentShader;

	private float _CurAlpha = 1f;

	private Animation _Anim;

	public Animation Anim => _Anim;

	private void Awake()
	{
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
			for (int j = 0; j < Skins[i].objs.Length; j++)
			{
				Skins[i].objs[j].SetActiveRecursively(false);
			}
		}
		_Anim = null;
	}

	public void DisableAllHands()
	{
		for (int i = 0; i < Hands.Length; i++)
		{
			for (int j = 0; j < Hands[i].objs.Length; j++)
			{
				Hands[i].objs[j].SetActiveRecursively(false);
			}
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
				for (int j = 0; j < Hands[i].objs.Length; j++)
				{
					Hands[i].objs[j].SetActiveRecursively(true);
					if ((Object)(object)Hands[i].objs[j].animation != (Object)null)
					{
						_Anim = Hands[i].objs[j].animation;
					}
				}
			}
			else
			{
				for (int k = 0; k < Hands[i].objs.Length; k++)
				{
					Hands[i].objs[k].SetActiveRecursively(false);
				}
			}
		}
	}

	public void SetSkin(int ind)
	{
		_Anim = ((Component)this).animation;
		for (int i = 0; i < Skins.Length; i++)
		{
			if (i == ind)
			{
				for (int j = 0; j < Skins[i].objs.Length; j++)
				{
					Skins[i].objs[j].SetActiveRecursively(true);
					if ((Object)(object)Skins[i].objs[j].animation != (Object)null)
					{
						_Anim = Skins[i].objs[j].animation;
					}
				}
			}
			else
			{
				for (int k = 0; k < Skins[i].objs.Length; k++)
				{
					Skins[i].objs[k].SetActiveRecursively(false);
				}
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
}
