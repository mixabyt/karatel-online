using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Drag Panel Contents")]
[ExecuteInEditMode]
public class UIDragPanelContents : MonoBehaviour
{
	public UIDraggablePanel draggablePanel;

	[HideInInspector]
	[SerializeField]
	private UIPanel panel;

	private void Awake()
	{
		if (!((Object)(object)panel != (Object)null))
		{
			return;
		}
		if ((Object)(object)draggablePanel == (Object)null)
		{
			draggablePanel = ((Component)panel).GetComponent<UIDraggablePanel>();
			if ((Object)(object)draggablePanel == (Object)null)
			{
				draggablePanel = ((Component)panel).gameObject.AddComponent<UIDraggablePanel>();
			}
		}
		panel = null;
	}

	private void Start()
	{
		if ((Object)(object)draggablePanel == (Object)null)
		{
			draggablePanel = NGUITools.FindInParents<UIDraggablePanel>(((Component)this).gameObject);
		}
	}

	private void OnPress(bool pressed)
	{
		if (((Behaviour)this).enabled && NGUITools.GetActive(((Component)this).gameObject) && (Object)(object)draggablePanel != (Object)null)
		{
			draggablePanel.Press(pressed);
		}
	}

	private void OnDrag(Vector2 delta)
	{
		if (((Behaviour)this).enabled && NGUITools.GetActive(((Component)this).gameObject) && (Object)(object)draggablePanel != (Object)null)
		{
			draggablePanel.Drag();
		}
	}

	private void OnScroll(float delta)
	{
		if (((Behaviour)this).enabled && NGUITools.GetActive(((Component)this).gameObject) && (Object)(object)draggablePanel != (Object)null)
		{
			draggablePanel.Scroll(delta);
		}
	}
}
