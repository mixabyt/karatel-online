using UnityEngine;

[RequireComponent(typeof(UIWidget))]
[ExecuteInEditMode]
public class AnimatedColor : MonoBehaviour
{
	public Color color = Color.white;

	private UIWidget mWidget;

	public AnimatedColor()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
	}

	private void Awake()
	{
		mWidget = ((Component)this).GetComponent<UIWidget>();
	}

	private void Update()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		mWidget.color = color;
	}
}
