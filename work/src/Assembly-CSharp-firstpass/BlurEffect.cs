using UnityEngine;

[AddComponentMenu("Image Effects/Blur")]
[ExecuteInEditMode]
public class BlurEffect : MonoBehaviour
{
	public int iterations = 3;

	public float blurSpread = 0.6f;

	private static string blurMatString = "Shader \"BlurConeTap\" {\n\tProperties { _MainTex (\"\", any) = \"\" {} }\n\tSubShader {\n\t\tPass {\n\t\t\tZTest Always Cull Off ZWrite Off Fog { Mode Off }\n\t\t\tSetTexture [_MainTex] {constantColor (0,0,0,0.25) combine texture * constant alpha}\n\t\t\tSetTexture [_MainTex] {constantColor (0,0,0,0.25) combine texture * constant + previous}\n\t\t\tSetTexture [_MainTex] {constantColor (0,0,0,0.25) combine texture * constant + previous}\n\t\t\tSetTexture [_MainTex] {constantColor (0,0,0,0.25) combine texture * constant + previous}\n\t\t}\n\t}\n\tFallback off\n}";

	private static Material m_Material;

	protected static Material material
	{
		get
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Expected Obj, but got Unknown
			if ((Object)(object)m_Material == (Object)null)
			{
				m_Material = new Material(blurMatString);
				((Object)m_Material).hideFlags = (HideFlags)13;
				((Object)m_Material.shader).hideFlags = (HideFlags)13;
			}
			return m_Material;
		}
	}

	protected void OnDisable()
	{
		if (Object.op_Implicit((Object)(object)m_Material))
		{
			Object.DestroyImmediate((Object)(object)m_Material.shader);
			Object.DestroyImmediate((Object)(object)m_Material);
		}
	}

	protected void Start()
	{
		if (!SystemInfo.supportsImageEffects)
		{
			((Behaviour)this).enabled = false;
		}
		else if (!material.shader.isSupported)
		{
			((Behaviour)this).enabled = false;
		}
	}

	public void FourTapCone(RenderTexture source, RenderTexture dest, int iteration)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		float num = 0.5f + (float)iteration * blurSpread;
		Graphics.BlitMultiTap((Texture)(object)source, dest, material, new Vector2[4]
		{
			new Vector2(0f - num, 0f - num),
			new Vector2(0f - num, num),
			new Vector2(num, num),
			new Vector2(num, 0f - num)
		});
	}

	private void DownSample4x(RenderTexture source, RenderTexture dest)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		float num = 1f;
		Graphics.BlitMultiTap((Texture)(object)source, dest, material, new Vector2[4]
		{
			new Vector2(0f - num, 0f - num),
			new Vector2(0f - num, num),
			new Vector2(num, num),
			new Vector2(num, 0f - num)
		});
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		RenderTexture temporary = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0);
		RenderTexture temporary2 = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0);
		DownSample4x(source, temporary);
		bool flag = true;
		for (int i = 0; i < iterations; i++)
		{
			if (flag)
			{
				FourTapCone(temporary, temporary2, i);
			}
			else
			{
				FourTapCone(temporary2, temporary, i);
			}
			flag = !flag;
		}
		if (flag)
		{
			Graphics.Blit((Texture)(object)temporary, destination);
		}
		else
		{
			Graphics.Blit((Texture)(object)temporary2, destination);
		}
		RenderTexture.ReleaseTemporary(temporary);
		RenderTexture.ReleaseTemporary(temporary2);
	}
}
