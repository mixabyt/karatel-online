using UnityEngine;

public class AnimatedAlpha : MonoBehaviour
{
	public float alpha = 1f;

	private UIWidget mWidget;

	private UIPanel mPanel;

	private void Awake()
	{
		mWidget = ((Component)this).GetComponent<UIWidget>();
		mPanel = ((Component)this).GetComponent<UIPanel>();
		Update();
	}

	private void Update()
	{
		if ((Object)(object)mWidget != (Object)null)
		{
			mWidget.alpha = alpha;
		}
		if ((Object)(object)mPanel != (Object)null)
		{
			mPanel.alpha = alpha;
		}
	}
}
