using System;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Vignette and Chromatic Aberration")]
[ExecuteInEditMode]
public class Vignetting : PostEffectsBase
{
	public float intensity;

	public float chromaticAberration;

	public float blur;

	public float blurSpread;

	public Shader vignetteShader;

	private Material vignetteMaterial;

	public Shader separableBlurShader;

	private Material separableBlurMaterial;

	public Shader chromAberrationShader;

	private Material chromAberrationMaterial;

	public Vignetting()
	{
		intensity = 0.375f;
		chromaticAberration = 0.2f;
		blur = 0.1f;
		blurSpread = 1.5f;
	}

	public override void Start()
	{
		CreateMaterials();
		CheckSupport(needDepth: false);
	}

	public override void CreateMaterials()
	{
		vignetteMaterial = CheckShaderAndCreateMaterial(vignetteShader, vignetteMaterial);
		separableBlurMaterial = CheckShaderAndCreateMaterial(separableBlurShader, separableBlurMaterial);
		chromAberrationMaterial = CheckShaderAndCreateMaterial(chromAberrationShader, chromAberrationMaterial);
	}

	public override void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		CreateMaterials();
		float num = 1f * (float)source.width / (1f * (float)source.height);
		float num2 = 2f / 1024f;
		RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height, 0);
		RenderTexture temporary2 = RenderTexture.GetTemporary((int)((float)source.width / 2f), (int)((float)source.height / 2f), 0);
		RenderTexture temporary3 = RenderTexture.GetTemporary((int)((float)source.width / 4f), (int)((float)source.height / 4f), 0);
		RenderTexture temporary4 = RenderTexture.GetTemporary((int)((float)source.width / 4f), (int)((float)source.height / 4f), 0);
		Graphics.Blit((Texture)(object)source, temporary2, chromAberrationMaterial, 0);
		Graphics.Blit((Texture)(object)temporary2, temporary3);
		for (int i = 0; i < 2; i++)
		{
			separableBlurMaterial.SetVector("offsets", new Vector4(0f, blurSpread * num2, 0f, 0f));
			Graphics.Blit((Texture)(object)temporary3, temporary4, separableBlurMaterial);
			separableBlurMaterial.SetVector("offsets", new Vector4(blurSpread * num2 / num, 0f, 0f, 0f));
			Graphics.Blit((Texture)(object)temporary4, temporary3, separableBlurMaterial);
		}
		vignetteMaterial.SetFloat("_Intensity", intensity);
		vignetteMaterial.SetFloat("_Blur", blur);
		vignetteMaterial.SetTexture("_VignetteTex", (Texture)(object)temporary3);
		Graphics.Blit((Texture)(object)source, temporary, vignetteMaterial);
		chromAberrationMaterial.SetFloat("_ChromaticAberration", chromaticAberration);
		Graphics.Blit((Texture)(object)temporary, destination, chromAberrationMaterial, 1);
		RenderTexture.ReleaseTemporary(temporary);
		RenderTexture.ReleaseTemporary(temporary2);
		RenderTexture.ReleaseTemporary(temporary3);
		RenderTexture.ReleaseTemporary(temporary4);
	}

	public override void Main()
	{
	}
}
