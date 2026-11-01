using System;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Edge Detection (Geometry)")]
[ExecuteInEditMode]
public class EdgeDetectEffectNormals : PostEffectsBase
{
	public EdgeDetectMode mode;

	public float sensitivityDepth;

	public float sensitivityNormals;

	public float edgesOnly;

	public Color edgesOnlyBgColor;

	public Shader edgeDetectShader;

	private Material edgeDetectMaterial;

	public EdgeDetectEffectNormals()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		mode = EdgeDetectMode.Thin;
		sensitivityDepth = 1f;
		sensitivityNormals = 1f;
		edgesOnlyBgColor = Color.white;
	}

	public override void CreateMaterials()
	{
		edgeDetectMaterial = CheckShaderAndCreateMaterial(edgeDetectShader, edgeDetectMaterial);
	}

	public override void Start()
	{
		CreateMaterials();
		CheckSupport(needDepth: true);
	}

	public override void OnEnable()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).camera.depthTextureMode = (DepthTextureMode)(((Component)this).camera.depthTextureMode | 2);
	}

	public override void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		CreateMaterials();
		Vector2 val = new Vector2(sensitivityDepth, sensitivityNormals);
		((Texture)source).filterMode = (FilterMode)0;
		edgeDetectMaterial.SetVector("sensitivity", new Vector4(val.x, val.y, 1f, val.y));
		edgeDetectMaterial.SetFloat("_BgFade", edgesOnly);
		Vector4 val2 = Color.op_Implicit(edgesOnlyBgColor);
		edgeDetectMaterial.SetVector("_BgColor", val2);
		if (mode == EdgeDetectMode.Thin)
		{
			Graphics.Blit((Texture)(object)source, destination, edgeDetectMaterial, 0);
		}
		else
		{
			Graphics.Blit((Texture)(object)source, destination, edgeDetectMaterial, 1);
		}
	}

	public override void Main()
	{
	}
}
