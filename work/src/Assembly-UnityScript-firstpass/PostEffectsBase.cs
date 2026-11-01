using System;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class PostEffectsBase : MonoBehaviour
{
	public override Material CheckShaderAndCreateMaterial(Shader s, Material m2Create)
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Expected Obj, but got Unknown
		object result;
		if (Object.op_Implicit((Object)(object)m2Create) && (Object)(object)m2Create.shader == (Object)(object)s)
		{
			result = m2Create;
		}
		else if (!Object.op_Implicit((Object)(object)s))
		{
			Debug.Log((object)("Missing shader in " + ((Object)this).ToString()));
			((Behaviour)this).enabled = false;
			result = null;
		}
		else if (!s.isSupported)
		{
			NotSupported();
			Debug.LogError((object)("The shader " + ((Object)s).ToString() + " on effect " + ((Object)this).ToString() + " is not supported on this platform!"));
			result = null;
		}
		else
		{
			m2Create = new Material(s);
			((Object)m2Create).hideFlags = (HideFlags)4;
			result = ((!Object.op_Implicit((Object)(object)m2Create)) ? null : m2Create);
		}
		return (Material)result;
	}

	public override Material CreateMaterial(Shader s, Material m2Create)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected Obj, but got Unknown
		object result;
		if (Object.op_Implicit((Object)(object)m2Create) && (Object)(object)m2Create.shader == (Object)(object)s)
		{
			result = m2Create;
		}
		else if (!Object.op_Implicit((Object)(object)s))
		{
			Debug.Log((object)("Missing shader in " + ((Object)this).ToString()));
			result = null;
		}
		else if (!s.isSupported)
		{
			result = null;
		}
		else
		{
			m2Create = new Material(s);
			((Object)m2Create).hideFlags = (HideFlags)4;
			result = ((!Object.op_Implicit((Object)(object)m2Create)) ? null : m2Create);
		}
		return (Material)result;
	}

	public override bool CheckSupport()
	{
		return CheckSupport(needDepth: false);
	}

	public override bool CheckSupport(bool needDepth)
	{
		int result;
		if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures)
		{
			NotSupported();
			result = 0;
		}
		else if (needDepth && !SystemInfo.SupportsRenderTextureFormat((RenderTextureFormat)1))
		{
			NotSupported();
			result = 0;
		}
		else
		{
			result = 1;
		}
		return (byte)result != 0;
	}

	public override bool CheckShader(Shader s)
	{
		Debug.Log((object)("The shader " + ((Object)s).ToString() + " on effect " + ((Object)this).ToString() + " is not part of the Unity 3.2 effects suite anymore. For best performance and quality, please ensure you are using the latest Standard Assets Image Effects (Pro only) package."));
		int result;
		if (!s.isSupported)
		{
			NotSupported();
			result = 0;
		}
		else
		{
			result = 0;
		}
		return (byte)result != 0;
	}

	public override void NotSupported()
	{
		Debug.LogError((object)("The image effect " + ((Object)this).ToString() + "is not supported on this platform!"));
		((Behaviour)this).enabled = false;
	}

	public override void DrawBorder(RenderTexture dest, Material material)
	{
		float num = default;
		float num2 = default;
		float num3 = default;
		float num4 = default;
		RenderTexture.active = dest;
		bool flag = true;
		GL.PushMatrix();
		GL.LoadOrtho();
		for (int i = 0; i < material.passCount; i++)
		{
			material.SetPass(i);
			float num5 = default;
			float num6 = default;
			if (flag)
			{
				num5 = 1f;
				num6 = 0f;
			}
			else
			{
				num5 = 0f;
				num6 = 1f;
			}
			num = 0f;
			num2 = 0f + 1f / ((float)dest.width * 1f);
			num3 = 0f;
			num4 = 1f;
			GL.Begin(7);
			GL.TexCoord2(0f, num5);
			GL.Vertex3(num, num3, 0.1f);
			GL.TexCoord2(1f, num5);
			GL.Vertex3(num2, num3, 0.1f);
			GL.TexCoord2(1f, num6);
			GL.Vertex3(num2, num4, 0.1f);
			GL.TexCoord2(0f, num6);
			GL.Vertex3(num, num4, 0.1f);
			num = 1f - 1f / ((float)dest.width * 1f);
			num2 = 1f;
			num3 = 0f;
			num4 = 1f;
			GL.TexCoord2(0f, num5);
			GL.Vertex3(num, num3, 0.1f);
			GL.TexCoord2(1f, num5);
			GL.Vertex3(num2, num3, 0.1f);
			GL.TexCoord2(1f, num6);
			GL.Vertex3(num2, num4, 0.1f);
			GL.TexCoord2(0f, num6);
			GL.Vertex3(num, num4, 0.1f);
			num = 0f;
			num2 = 1f;
			num3 = 0f;
			num4 = 0f + 1f / ((float)dest.height * 1f);
			GL.TexCoord2(0f, num5);
			GL.Vertex3(num, num3, 0.1f);
			GL.TexCoord2(1f, num5);
			GL.Vertex3(num2, num3, 0.1f);
			GL.TexCoord2(1f, num6);
			GL.Vertex3(num2, num4, 0.1f);
			GL.TexCoord2(0f, num6);
			GL.Vertex3(num, num4, 0.1f);
			num = 0f;
			num2 = 1f;
			num3 = 1f - 1f / ((float)dest.height * 1f);
			num4 = 1f;
			GL.TexCoord2(0f, num5);
			GL.Vertex3(num, num3, 0.1f);
			GL.TexCoord2(1f, num5);
			GL.Vertex3(num2, num3, 0.1f);
			GL.TexCoord2(1f, num6);
			GL.Vertex3(num2, num4, 0.1f);
			GL.TexCoord2(0f, num6);
			GL.Vertex3(num, num4, 0.1f);
			GL.End();
		}
		GL.PopMatrix();
	}

	public override void Main()
	{
	}
}
