using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Checkbox Controlled Object")]
public class UICheckboxControlledObject : MonoBehaviour
{
	public GameObject target;

	public bool inverse;

	private void OnEnable()
	{
		UICheckbox component = ((Component)this).GetComponent<UICheckbox>();
		if ((Object)(object)component != (Object)null)
		{
			OnActivate(component.isChecked);
		}
	}

	private void OnActivate(bool isActive)
	{
		if ((Object)(object)target != (Object)null)
		{
			NGUITools.SetActive(target, (!inverse) ? isActive : (!isActive));
			UIPanel uIPanel = NGUITools.FindInParents<UIPanel>(target);
			if ((Object)(object)uIPanel != (Object)null)
			{
				uIPanel.Refresh();
			}
		}
	}
}
