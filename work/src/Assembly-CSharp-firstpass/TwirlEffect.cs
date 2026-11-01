using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Twirl")]
public class TwirlEffect : ImageEffectBase
{
	public Vector2 radius = new Vector2(0.3f, 0.3f);

	public float angle = 50f;

	public Vector2 center = new Vector2(0.5f, 0.5f);

	public TwirlEffect()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		ImageEffects.RenderDistortion(material, source, destination, angle, center, radius);
	}
}
