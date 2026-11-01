using UnityEngine;

[AddComponentMenu("NGUI/Examples/Slider Colors")]
[RequireComponent(typeof(UISlider))]
[ExecuteInEditMode]
public class UISliderColors : MonoBehaviour
{
	public UISprite sprite;

	public Color[] colors = new Color[3]
	{
		Color.red,
		Color.yellow,
		Color.green
	};

	private UISlider mSlider;

	public UISliderColors()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
	}

	private void Start()
	{
		mSlider = ((Component)this).GetComponent<UISlider>();
		Update();
	}

	private void Update()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)sprite == (Object)null || colors.Length == 0)
		{
			return;
		}
		float sliderValue = mSlider.sliderValue;
		sliderValue *= (float)(colors.Length - 1);
		int num = Mathf.FloorToInt(sliderValue);
		Color color = colors[0];
		if (num >= 0)
		{
			if (num + 1 >= colors.Length)
			{
				color = ((num >= colors.Length) ? colors[colors.Length - 1] : colors[num]);
			}
			else
			{
				float num2 = sliderValue - (float)num;
				color = Color.Lerp(colors[num], colors[num + 1], num2);
			}
		}
		color.a = sprite.color.a;
		sprite.color = color;
	}
}
