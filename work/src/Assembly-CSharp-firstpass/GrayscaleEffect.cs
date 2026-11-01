using UnityEngine;

[AddComponentMenu("Image Effects/Grayscale")]
[ExecuteInEditMode]
public class GrayscaleEffect : ImageEffectBase
{
	public Texture textureRamp;

	public float rampOffset;

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		material.SetTexture("_RampTex", textureRamp);
		material.SetFloat("_RampOffset", rampOffset);
		Graphics.Blit((Texture)(object)source, destination, material);
	}
}
