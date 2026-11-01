using UnityEngine;

[AddComponentMenu("NGUI/UI/Image Button")]
[ExecuteInEditMode]
public class UIImageButton : MonoBehaviour
{
	public UISprite target;

	public string normalSprite;

	public string hoverSprite;

	public string pressedSprite;

	public string disabledSprite;

	public bool isEnabled
	{
		get
		{
			Collider collider = ((Component)this).collider;
			return Object.op_Implicit((Object)(object)collider) && collider.enabled;
		}
		set
		{
			Collider collider = ((Component)this).collider;
			if (Object.op_Implicit((Object)(object)collider) && collider.enabled != value)
			{
				collider.enabled = value;
				UpdateImage();
			}
		}
	}

	private void Awake()
	{
		if ((Object)(object)target == (Object)null)
		{
			target = ((Component)this).GetComponentInChildren<UISprite>();
		}
	}

	private void OnEnable()
	{
		UpdateImage();
	}

	private void UpdateImage()
	{
		if ((Object)(object)target != (Object)null)
		{
			if (isEnabled)
			{
				target.spriteName = ((!UICamera.IsHighlighted(((Component)this).gameObject)) ? normalSprite : hoverSprite);
			}
			else
			{
				target.spriteName = disabledSprite;
			}
			target.MakePixelPerfect();
		}
	}

	private void OnHover(bool isOver)
	{
		if (isEnabled && (Object)(object)target != (Object)null)
		{
			target.spriteName = ((!isOver) ? normalSprite : hoverSprite);
			target.MakePixelPerfect();
		}
	}

	private void OnPress(bool pressed)
	{
		if (pressed)
		{
			target.spriteName = pressedSprite;
			target.MakePixelPerfect();
		}
		else
		{
			UpdateImage();
		}
	}
}
