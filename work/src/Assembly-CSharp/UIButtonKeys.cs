using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button Keys")]
[RequireComponent(typeof(Collider))]
public class UIButtonKeys : MonoBehaviour
{
	public bool startsSelected;

	public UIButtonKeys selectOnClick;

	public UIButtonKeys selectOnUp;

	public UIButtonKeys selectOnDown;

	public UIButtonKeys selectOnLeft;

	public UIButtonKeys selectOnRight;

	private void OnEnable()
	{
		if (startsSelected && (Object)(object)UICamera.selectedObject == (Object)null)
		{
			if (!NGUITools.GetActive(UICamera.selectedObject))
			{
				UICamera.selectedObject = ((Component)this).gameObject;
			}
			else
			{
				UICamera.Notify(((Component)this).gameObject, "OnHover", true);
			}
		}
	}

	private void OnKey(KeyCode key)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected I4, but got Unknown
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Invalid comparison between Unknown and I4
		if (!((Behaviour)this).enabled || !NGUITools.GetActive(((Component)this).gameObject))
		{
			return;
		}
		switch (key - 273)
		{
		case 3:
			if ((Object)(object)selectOnLeft != (Object)null)
			{
				UICamera.selectedObject = ((Component)selectOnLeft).gameObject;
			}
			return;
		case 2:
			if ((Object)(object)selectOnRight != (Object)null)
			{
				UICamera.selectedObject = ((Component)selectOnRight).gameObject;
			}
			return;
		case 0:
			if ((Object)(object)selectOnUp != (Object)null)
			{
				UICamera.selectedObject = ((Component)selectOnUp).gameObject;
			}
			return;
		case 1:
			if ((Object)(object)selectOnDown != (Object)null)
			{
				UICamera.selectedObject = ((Component)selectOnDown).gameObject;
			}
			return;
		}
		if ((int)key != 9)
		{
			return;
		}
		if (Input.GetKey((KeyCode)304) || Input.GetKey((KeyCode)303))
		{
			if ((Object)(object)selectOnLeft != (Object)null)
			{
				UICamera.selectedObject = ((Component)selectOnLeft).gameObject;
			}
			else if ((Object)(object)selectOnUp != (Object)null)
			{
				UICamera.selectedObject = ((Component)selectOnUp).gameObject;
			}
			else if ((Object)(object)selectOnDown != (Object)null)
			{
				UICamera.selectedObject = ((Component)selectOnDown).gameObject;
			}
			else if ((Object)(object)selectOnRight != (Object)null)
			{
				UICamera.selectedObject = ((Component)selectOnRight).gameObject;
			}
		}
		else if ((Object)(object)selectOnRight != (Object)null)
		{
			UICamera.selectedObject = ((Component)selectOnRight).gameObject;
		}
		else if ((Object)(object)selectOnDown != (Object)null)
		{
			UICamera.selectedObject = ((Component)selectOnDown).gameObject;
		}
		else if ((Object)(object)selectOnUp != (Object)null)
		{
			UICamera.selectedObject = ((Component)selectOnUp).gameObject;
		}
		else if ((Object)(object)selectOnLeft != (Object)null)
		{
			UICamera.selectedObject = ((Component)selectOnLeft).gameObject;
		}
	}

	private void OnClick()
	{
		if (((Behaviour)this).enabled && (Object)(object)selectOnClick != (Object)null)
		{
			UICamera.selectedObject = ((Component)selectOnClick).gameObject;
		}
	}
}
