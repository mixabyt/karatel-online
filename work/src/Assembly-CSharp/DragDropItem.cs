using UnityEngine;

[AddComponentMenu("NGUI/Examples/Drag and Drop Item")]
public class DragDropItem : MonoBehaviour
{
	public GameObject prefab;

	private Transform mTrans;

	private bool mIsDragging;

	private bool mSticky;

	private Transform mParent;

	private void UpdateTable()
	{
		UITable uITable = NGUITools.FindInParents<UITable>(((Component)this).gameObject);
		if ((Object)(object)uITable != (Object)null)
		{
			uITable.repositionNow = true;
		}
	}

	private void Drop()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		Collider collider = UICamera.lastHit.collider;
		DragDropContainer dragDropContainer = ((!((Object)(object)collider != (Object)null)) ? null : ((Component)collider).gameObject.GetComponent<DragDropContainer>());
		if ((Object)(object)dragDropContainer != (Object)null)
		{
			mTrans.parent = ((Component)dragDropContainer).transform;
			Vector3 localPosition = mTrans.localPosition;
			localPosition.z = 0f;
			mTrans.localPosition = localPosition;
		}
		else
		{
			mTrans.parent = mParent;
		}
		UpdateTable();
		NGUITools.MarkParentAsChanged(((Component)this).gameObject);
	}

	private void Awake()
	{
		mTrans = ((Component)this).transform;
	}

	private void OnDrag(Vector2 delta)
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		if (((Behaviour)this).enabled && UICamera.currentTouchID > -2)
		{
			if (!mIsDragging)
			{
				mIsDragging = true;
				mParent = mTrans.parent;
				mTrans.parent = DragDropRoot.root;
				Vector3 localPosition = mTrans.localPosition;
				localPosition.z = 0f;
				mTrans.localPosition = localPosition;
				NGUITools.MarkParentAsChanged(((Component)this).gameObject);
			}
			else
			{
				Transform val = mTrans;
				val.localPosition += Vector2.op_Implicit(delta);
			}
		}
	}

	private void OnPress(bool isPressed)
	{
		if (!((Behaviour)this).enabled)
		{
			return;
		}
		if (isPressed)
		{
			if (!UICamera.current.stickyPress)
			{
				mSticky = true;
				UICamera.current.stickyPress = true;
			}
		}
		else if (mSticky)
		{
			mSticky = false;
			UICamera.current.stickyPress = false;
		}
		mIsDragging = false;
		Collider collider = ((Component)this).collider;
		if ((Object)(object)collider != (Object)null)
		{
			collider.enabled = !isPressed;
		}
		if (!isPressed)
		{
			Drop();
		}
	}
}
