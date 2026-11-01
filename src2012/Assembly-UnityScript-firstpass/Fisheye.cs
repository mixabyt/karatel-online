using System;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Fisheye")]
[ExecuteInEditMode]
public class Fisheye : PostEffectsBase
{
	public float strengthX;

	public float strengthY;

	public Shader fishEyeShader;

	private Material fisheyeMaterial;

	public Fisheye()
	{
		strengthX = 0.05f;
		strengthY = 0.05f;
	}

	public override void CreateMaterials()
	{
		fisheyeMaterial = CheckShaderAndCreateMaterial(fishEyeShader, fisheyeMaterial);
	}

	public override void Start()
	{
		CreateMaterials();
		CheckSupport(needDepth: false);
	}

	public override void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		CreateMaterials();
		float num = 5f / 32f;
		float num2 = (float)source.width * 1f / ((float)source.height * 1f);
		fisheyeMaterial.SetVector("intensity", new Vector4(strengthX * num2 * num, strengthY * num, strengthX * num2 * num, strengthY * num));
		Graphics.Blit((Texture)(object)source, destination, fisheyeMaterial);
	}

	public override void Main()
	{
	}
}
