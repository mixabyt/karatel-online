using UnityEngine;

public class ComboBox
{
	private static bool forceToUnShow;

	private static int useControlID = -1;

	private bool isClickedComboButton;

	private int selectedItemIndex;

	public int List(Rect rect, string buttonText, GUIContent[] listContent, GUIStyle listStyle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected Obj, but got Unknown
		return List(rect, new GUIContent(buttonText), listContent, GUIStyle.op_Implicit("button"), GUIStyle.op_Implicit("box"), listStyle);
	}

	public int List(Rect rect, GUIContent buttonContent, GUIContent[] listContent, GUIStyle listStyle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return List(rect, buttonContent, listContent, GUIStyle.op_Implicit("button"), GUIStyle.op_Implicit("box"), listStyle);
	}

	public int List(Rect rect, string buttonText, GUIContent[] listContent, GUIStyle buttonStyle, GUIStyle boxStyle, GUIStyle listStyle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected Obj, but got Unknown
		return List(rect, new GUIContent(buttonText), listContent, buttonStyle, boxStyle, listStyle);
	}

	public int List(Rect rect, GUIContent buttonContent, GUIContent[] listContent, GUIStyle buttonStyle, GUIStyle boxStyle, GUIStyle listStyle)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Invalid comparison between Unknown and I4
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		if (forceToUnShow)
		{
			forceToUnShow = false;
			isClickedComboButton = false;
		}
		bool flag = false;
		int controlID = GUIUtility.GetControlID((FocusType)2);
		EventType typeForControl = Event.current.GetTypeForControl(controlID);
		if ((int)typeForControl == 1 && isClickedComboButton)
		{
			flag = true;
		}
		if (GUI.Button(rect, buttonContent, buttonStyle))
		{
			if (useControlID == -1)
			{
				useControlID = controlID;
				isClickedComboButton = false;
			}
			if (useControlID != controlID)
			{
				forceToUnShow = true;
				useControlID = controlID;
			}
			isClickedComboButton = true;
		}
		if (isClickedComboButton)
		{
			Rect val = new Rect(rect.x, rect.y + listStyle.CalcHeight(listContent[0], 1f), rect.width, listStyle.CalcHeight(listContent[0], 1f) * (float)listContent.Length);
			GUI.Box(val, string.Empty, boxStyle);
			int num = GUI.SelectionGrid(val, selectedItemIndex, listContent, 1, listStyle);
			if (num != selectedItemIndex)
			{
				selectedItemIndex = num;
			}
		}
		if (flag)
		{
			isClickedComboButton = false;
		}
		return GetSelectedItemIndex();
	}

	public int GetSelectedItemIndex()
	{
		return selectedItemIndex;
	}

	public bool IsShowingList()
	{
		return isClickedComboButton;
	}
}
