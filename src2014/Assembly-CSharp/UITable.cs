using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/Table")]
public class UITable : MonoBehaviour
{
	public enum Direction
	{
		Down,
		Up
	}

	public delegate void OnReposition();

	public int columns = 1;

	public Direction direction;

	public Vector2 padding = Vector2.zero;

	public bool sorted;

	public bool hideInactive = true;

	public bool repositionNow;

	public bool keepWithinPanel;

	public OnReposition onReposition;

	public bool noXReposition;

	public bool reverse;

	private UIPanel mPanel;

	private UIDraggablePanel mDrag;

	private bool mStarted;

	private List<Transform> mChildren = new List<Transform>();

	public List<Transform> children
	{
		get
		{
			if (mChildren.Count == 0)
			{
				Transform transform = ((Component)this).transform;
				mChildren.Clear();
				for (int i = 0; i < transform.childCount; i++)
				{
					Transform child = transform.GetChild(i);
					if (Object.op_Implicit((Object)(object)child) && Object.op_Implicit((Object)(object)((Component)child).gameObject) && (!hideInactive || NGUITools.GetActive(((Component)child).gameObject)))
					{
						mChildren.Add(child);
					}
				}
				if (sorted)
				{
					mChildren.Sort(SortByTagName);
				}
				if (reverse)
				{
					mChildren.Reverse();
				}
			}
			return mChildren;
		}
	}

	public UITable()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
	}

	public static int SortByName(Transform a, Transform b)
	{
		return string.Compare(((Object)a).name, ((Object)b).name);
	}

	public static int SortByTagName(Transform a, Transform b)
	{
		return string.Compare(((Component)a).tag, ((Component)b).tag);
	}

	private void RepositionVariableSize(List<Transform> children)
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		float num2 = 0f;
		int num3 = ((columns <= 0) ? 1 : (children.Count / columns + 1));
		int num4 = ((columns <= 0) ? children.Count : columns);
		Bounds[,] array = new Bounds[num3, num4];
		Bounds[] array2 = new Bounds[num4];
		Bounds[] array3 = new Bounds[num3];
		int num5 = 0;
		int num6 = 0;
		int i = 0;
		for (int count = children.Count; i < count; i++)
		{
			Transform val = children[i];
			Bounds val2 = NGUIMath.CalculateRelativeWidgetBounds(val);
			Vector3 localScale = val.localScale;
			val2.min = Vector3.Scale(val2.min, localScale);
			val2.max = Vector3.Scale(val2.max, localScale);
			array[num6, num5] = val2;
			array2[num5].Encapsulate(val2);
			array3[num6].Encapsulate(val2);
			if (++num5 >= columns && columns > 0)
			{
				num5 = 0;
				num6++;
			}
		}
		num5 = 0;
		num6 = 0;
		int j = 0;
		for (int count2 = children.Count; j < count2; j++)
		{
			Transform val3 = children[j];
			Bounds val4 = array[num6, num5];
			Bounds val5 = array2[num5];
			Bounds val6 = array3[num6];
			Vector3 localPosition = val3.localPosition;
			if (!noXReposition)
			{
				localPosition.x = num + val4.extents.x - val4.center.x;
				localPosition.x += val4.min.x - val5.min.x + padding.x;
			}
			if (direction == Direction.Down)
			{
				localPosition.y = 0f - num2 - val4.extents.y - val4.center.y;
				localPosition.y += (val4.max.y - val4.min.y - val6.max.y + val6.min.y) * 0.5f - padding.y;
			}
			else
			{
				localPosition.y = num2 + val4.extents.y - val4.center.y;
				localPosition.y += (val4.max.y - val4.min.y - val6.max.y + val6.min.y) * 0.5f - padding.y;
			}
			num += val5.max.x - val5.min.x + padding.x * 2f;
			val3.localPosition = localPosition;
			if (++num5 >= columns && columns > 0)
			{
				num5 = 0;
				num6++;
				num = 0f;
				num2 += val6.size.y + padding.y * 2f;
			}
		}
	}

	public void Reposition()
	{
		if (mStarted)
		{
			Transform transform = ((Component)this).transform;
			mChildren.Clear();
			List<Transform> list = children;
			if (list.Count > 0)
			{
				RepositionVariableSize(list);
			}
			if ((Object)(object)mDrag != (Object)null)
			{
				mDrag.UpdateScrollbars(recalculateBounds: true);
				mDrag.RestrictWithinBounds(instant: true);
			}
			else if ((Object)(object)mPanel != (Object)null)
			{
				mPanel.ConstrainTargetToBounds(transform, immediate: true);
			}
			if (onReposition != null)
			{
				onReposition();
			}
		}
		else
		{
			repositionNow = true;
		}
	}

	private void Start()
	{
		mStarted = true;
		if (keepWithinPanel)
		{
			mPanel = NGUITools.FindInParents<UIPanel>(((Component)this).gameObject);
			mDrag = NGUITools.FindInParents<UIDraggablePanel>(((Component)this).gameObject);
		}
		Reposition();
	}

	private void LateUpdate()
	{
		if (repositionNow)
		{
			repositionNow = false;
			Reposition();
		}
	}
}
