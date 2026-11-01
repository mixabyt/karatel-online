using System;
using UnityEngine;

public class TimedFadeText : MonoBehaviour
{
	public float liveTime = 5f;

	public float fadeTime = 2f;

	public GUIText text;

	private float time;

	[NonSerialized]
	public bool isFading;

	[NonSerialized]
	public bool isFaded;

	private float startAlpha = 1f;

	private void Start()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)((Component)this).guiText.material))
		{
			((Behaviour)this).enabled = false;
		}
		startAlpha = ((Component)this).guiText.material.color.a;
	}

	private void Update()
	{
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		time += Time.deltaTime;
		if (!isFaded)
		{
			if (isFading)
			{
				if (time >= fadeTime)
				{
					isFaded = true;
				}
				Color color = ((Component)this).guiText.material.color;
				color.a = CalculateAlpha();
				((Component)this).guiText.material.color = color;
				if ((Object)(object)((Component)this).guiTexture != (Object)null)
				{
					((Component)this).guiTexture.color = new Color(((Component)this).guiTexture.color.r, ((Component)this).guiTexture.color.g, ((Component)this).guiTexture.color.b, color.a);
				}
			}
			else if (time >= liveTime)
			{
				isFading = true;
				time = 0f;
			}
		}
		if (isFaded && text.pixelOffset.y > -4f)
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
	}

	private float CalculateAlpha()
	{
		float num = Mathf.Clamp01((fadeTime - time) / fadeTime);
		return Mathf.SmoothStep(0f, startAlpha, num);
	}
}
