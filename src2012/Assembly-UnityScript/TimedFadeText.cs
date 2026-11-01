using System;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(GUIText))]
public class TimedFadeText : MonoBehaviour
{
	public float liveTime;

	public float fadeTime;

	private float time;

	private bool isFading;

	private bool isFaded;

	private float startAlpha;

	public TimedFadeText()
	{
		liveTime = 5f;
		fadeTime = 2f;
		startAlpha = 1f;
	}

	public override void Start()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)((Component)this).guiText.material))
		{
			((Behaviour)this).enabled = false;
		}
		startAlpha = ((Component)this).guiText.material.color.a;
	}

	public override void Update()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		time += Time.deltaTime;
		if (!isFaded)
		{
			if (isFading)
			{
				if (!(time < fadeTime))
				{
					isFaded = true;
				}
				float a = CalculateAlpha();
				Color color = ((Component)this).guiText.material.color;
				float num = (color.a = a);
				Color val = (((Component)this).guiText.material.color = color);
			}
			else if (!(time < liveTime))
			{
				isFading = true;
				time = 0f;
			}
		}
		if (!isFaded)
		{
		}
	}

	public override float CalculateAlpha()
	{
		float num = Mathf.Clamp01((fadeTime - time) / fadeTime);
		return Mathf.SmoothStep(0f, startAlpha, num);
	}

	public override void Main()
	{
	}
}
