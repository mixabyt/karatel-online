using UnityEngine;

public class MaterialExt : MonoBehaviour
{
	private Material[] _OriginalMaterials;

	private bool[] _HaveColorProperty;

	public static void AffectColor(GameObject go, float r, float g, float b)
	{
		AffectNode(go.transform, 0, r, g, b, 0f);
	}

	public static void SetColor(GameObject go, float r, float g, float b)
	{
		AffectNode(go.transform, 1, r, g, b, 0f);
	}

	public static void SetTransparent(GameObject go, float a)
	{
		AffectNode(go.transform, 2, 0f, 0f, 0f, a);
	}

	public static void SetShader(GameObject go, Shader shader)
	{
		SetShaderInternal(go.transform, shader);
	}

	private static void AffectNode(Transform node, int affectType, float r, float g, float b, float a)
	{
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Expected Obj, but got Unknown
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)((Component)node).renderer != (Object)null)
		{
			MaterialExt materialExt = GetMaterialExt(node);
			Material[] materials = ((Component)node).renderer.materials;
			for (int i = 0; i < materialExt._OriginalMaterials.Length; i++)
			{
				if (materialExt._HaveColorProperty[i])
				{
					switch (affectType)
					{
					case 0:
					{
						Color color2 = materialExt._OriginalMaterials[i].color;
						materials[i].color = new Color(Mathf.Clamp01(color2.r + r), Mathf.Clamp01(color2.g + g), Mathf.Clamp01(color2.b + b), materials[i].color.a);
						break;
					}
					case 1:
						materials[i].color = new Color(r, g, b, materials[i].color.a);
						break;
					default:
					{
						Color color = ((Component)materialExt).renderer.materials[i].color;
						materials[i].color = new Color(color.r, color.g, color.b, a);
						break;
					}
					}
				}
			}
			((Component)node).renderer.materials = materials;
		}
		foreach (Transform item in node)
		{
			Transform node2 = item;
			AffectNode(node2, affectType, r, g, b, a);
		}
	}

	private static void SetShaderInternal(Transform node, Shader shader)
	{
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected Obj, but got Unknown
		if ((Object)(object)((Component)node).renderer != (Object)null)
		{
			MaterialExt materialExt = GetMaterialExt(node);
			Material[] materials = ((Component)materialExt).renderer.materials;
			for (int i = 0; i < materialExt._OriginalMaterials.Length; i++)
			{
				if ((Object)(object)materialExt._OriginalMaterials[i] != (Object)null)
				{
					materials[i].shader = ((!((Object)(object)shader != (Object)null)) ? materialExt._OriginalMaterials[i].shader : shader);
				}
			}
			((Component)node).renderer.materials = materials;
		}
		foreach (Transform item in node)
		{
			Transform node2 = item;
			SetShaderInternal(node2, shader);
		}
	}

	private static MaterialExt GetMaterialExt(Transform node)
	{
		MaterialExt materialExt = ((Component)node).GetComponent<MaterialExt>();
		if ((Object)(object)materialExt == (Object)null)
		{
			materialExt = ((Component)node).gameObject.AddComponent<MaterialExt>();
			materialExt._OriginalMaterials = new Material[((Component)node).renderer.sharedMaterials.Length];
			materialExt._HaveColorProperty = new bool[materialExt._OriginalMaterials.Length];
			for (int i = 0; i < materialExt._OriginalMaterials.Length; i++)
			{
				if ((Object)(object)((Component)node).renderer.sharedMaterials[i] != (Object)null)
				{
					Material[] originalMaterials = materialExt._OriginalMaterials;
					int num = i;
					Object val = Object.Instantiate((Object)(object)((Component)node).renderer.sharedMaterials[i]);
					originalMaterials[num] = (Material)(object)((val is Material) ? val : null);
					materialExt._HaveColorProperty[i] = materialExt._OriginalMaterials[i].HasProperty("_Color");
				}
			}
		}
		return materialExt;
	}
}
