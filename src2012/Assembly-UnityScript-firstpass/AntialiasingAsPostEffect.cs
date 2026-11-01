using System;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Antialiasing (Image based)")]
[ExecuteInEditMode]
public class AntialiasingAsPostEffect : PostEffectsBase
{
	public AAMode mode;

	public bool showGeneratedNormals;

	public float offsetScale;

	public float blurRadius;

	public bool dlaaSharp;

	public Shader ssaaShader;

	private Material ssaa;

	public Shader dlaaShader;

	private Material dlaa;

	public Shader nfaaShader;

	private Material nfaa;

	public Shader shaderFXAAPreset2;

	private Material materialFXAAPreset2;

	public Shader shaderFXAAPreset3;

	private Material materialFXAAPreset3;

	public Shader shaderFXAAII;

	private Material materialFXAAII;

	public AntialiasingAsPostEffect()
	{
		mode = AAMode.FXAA2;
		offsetScale = 0.2f;
		blurRadius = 18f;
	}

	public override void CreateMaterials()
	{
		materialFXAAPreset2 = CheckShaderAndCreateMaterial(shaderFXAAPreset2, materialFXAAPreset2);
		materialFXAAPreset3 = CheckShaderAndCreateMaterial(shaderFXAAPreset3, materialFXAAPreset3);
		materialFXAAII = CheckShaderAndCreateMaterial(shaderFXAAII, materialFXAAII);
		nfaa = CheckShaderAndCreateMaterial(nfaaShader, nfaa);
		ssaa = CheckShaderAndCreateMaterial(ssaaShader, ssaa);
		dlaa = CheckShaderAndCreateMaterial(dlaaShader, dlaa);
	}

	public override void Start()
	{
		CreateMaterials();
		CheckSupport(needDepth: false);
	}

	public override void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		CreateMaterials();
		if (mode < AAMode.NFAA)
		{
			Material val = null;
			if (mode == AAMode.FXAA1PresetB)
			{
				val = materialFXAAPreset3;
			}
			else
			{
				val = ((mode != AAMode.FXAA1PresetA) ? materialFXAAII : materialFXAAPreset2);
			}
			if (mode == AAMode.FXAA1PresetA)
			{
				((Texture)source).anisoLevel = 4;
			}
			Graphics.Blit((Texture)(object)source, destination, val);
			if (mode == AAMode.FXAA1PresetA)
			{
				((Texture)source).anisoLevel = 0;
			}
		}
		else if (mode == AAMode.SSAA)
		{
			Graphics.Blit((Texture)(object)source, destination, ssaa);
		}
		else if (mode == AAMode.DLAA)
		{
			((Texture)source).anisoLevel = 0;
			RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height);
			Graphics.Blit((Texture)(object)source, temporary, dlaa, 0);
			Graphics.Blit((Texture)(object)temporary, destination, dlaa, (!dlaaSharp) ? 1 : 2);
			RenderTexture.ReleaseTemporary(temporary);
		}
		else if (mode == AAMode.NFAA)
		{
			((Texture)source).anisoLevel = 0;
			nfaa.SetFloat("_OffsetScale", offsetScale);
			nfaa.SetFloat("_BlurRadius", blurRadius);
			Graphics.Blit((Texture)(object)source, destination, nfaa, showGeneratedNormals ? 1 : 0);
		}
		else
		{
			Graphics.Blit((Texture)(object)source, destination);
		}
	}

	public override void Main()
	{
	}
}
