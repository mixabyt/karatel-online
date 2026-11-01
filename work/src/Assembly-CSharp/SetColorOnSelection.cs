using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(UIWidget))]
[AddComponentMenu("NGUI/Examples/Set Color on Selection")]
public class SetColorOnSelection : MonoBehaviour
{
	private UIWidget mWidget;

	private void OnSelectionChange(string val)
	{
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)mWidget == (Object)null)
		{
			mWidget = ((Component)this).GetComponent<UIWidget>();
		}
		switch (val)
		{
		case "White":
			mWidget.color = Color.white;
			break;
		case "Red":
			mWidget.color = Color.red;
			break;
		case "Green":
			mWidget.color = Color.green;
			break;
		case "Blue":
			mWidget.color = Color.blue;
			break;
		case "Yellow":
			mWidget.color = Color.yellow;
			break;
		case "Cyan":
			mWidget.color = Color.cyan;
			break;
		case "Magenta":
			mWidget.color = Color.magenta;
			break;
		}
	}
}
