using UnityEngine;

[AddComponentMenu("Image Effects/Motion Blur (Color Accumulation)")]
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class MotionBlur : ImageEffectBase
{
	public float blurAmount = 0.8f;

	public bool extraBlur;

	private RenderTexture accumTexture;

	protected new void Start()
	{
		if (!SystemInfo.supportsRenderTextures)
		{
			((Behaviour)this).enabled = false;
		}
		else
		{
			base.Start();
		}
	}

	protected new void OnDisable()
	{
		base.OnDisable();
		Object.DestroyImmediate((Object)(object)accumTexture);
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected Obj, but got Unknown
		if ((Object)(object)accumTexture == (Object)null || accumTexture.width != source.width || accumTexture.height != source.height)
		{
			Object.DestroyImmediate((Object)(object)accumTexture);
			accumTexture = new RenderTexture(source.width, source.height, 0);
			((Object)accumTexture).hideFlags = (HideFlags)13;
			Graphics.Blit((Texture)(object)source, accumTexture);
		}
		if (extraBlur)
		{
			RenderTexture temporary = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0);
			Graphics.Blit((Texture)(object)accumTexture, temporary);
			Graphics.Blit((Texture)(object)temporary, accumTexture);
			RenderTexture.ReleaseTemporary(temporary);
		}
		blurAmount = Mathf.Clamp(blurAmount, 0f, 0.92f);
		material.SetTexture("_MainTex", (Texture)(object)accumTexture);
		material.SetFloat("_AccumOrig", 1f - blurAmount);
		Graphics.Blit((Texture)(object)source, accumTexture, material);
		Graphics.Blit((Texture)(object)accumTexture, destination);
	}
}
