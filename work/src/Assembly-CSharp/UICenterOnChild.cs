using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Center On Child")]
public class UICenterOnChild : MonoBehaviour
{
	public float springStrength = 8f;

	public SpringPanel.OnFinished onFinished;

	private UIDraggablePanel mDrag;

	private GameObject mCenteredObject;

	public GameObject centeredObject => mCenteredObject;

	private void OnEnable()
	{
		Recenter();
	}

	private void OnDragFinished()
	{
		if (((Behaviour)this).enabled)
		{
			Recenter();
		}
	}

	public void Recenter()
	{
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)mDrag == (Object)null)
		{
			mDrag = NGUITools.FindInParents<UIDraggablePanel>(((Component)this).gameObject);
			if ((Object)(object)mDrag == (Object)null)
			{
				Debug.LogWarning((object)string.Concat(((object)this).GetType(), " requires ", typeof(UIDraggablePanel), " on a parent object in order to work"), (Object)(object)this);
				((Behaviour)this).enabled = false;
				return;
			}
			mDrag.onDragFinished = OnDragFinished;
			if ((Object)(object)mDrag.horizontalScrollBar != (Object)null)
			{
				mDrag.horizontalScrollBar.onDragFinished = OnDragFinished;
			}
		}
		if ((Object)(object)mDrag.panel == (Object)null)
		{
			return;
		}
		Vector4 clipRange = mDrag.panel.clipRange;
		Transform cachedTransform = mDrag.panel.cachedTransform;
		Vector3 localPosition = cachedTransform.localPosition;
		localPosition.x += clipRange.x;
		localPosition.y += clipRange.y;
		localPosition = cachedTransform.parent.TransformPoint(localPosition);
		Vector3 val = localPosition - mDrag.currentMomentum * (mDrag.momentumAmount * 0.1f);
		mDrag.currentMomentum = Vector3.zero;
		float num = float.MaxValue;
		Transform val2 = null;
		Transform transform = ((Component)this).transform;
		int i = 0;
		for (int childCount = transform.childCount; i < childCount; i++)
		{
			Transform child = transform.GetChild(i);
			float num2 = Vector3.SqrMagnitude(child.position - val);
			if (num2 < num)
			{
				num = num2;
				val2 = child;
			}
		}
		if ((Object)(object)val2 != (Object)null)
		{
			mCenteredObject = ((Component)val2).gameObject;
			Vector3 val3 = cachedTransform.InverseTransformPoint(val2.position);
			Vector3 val4 = cachedTransform.InverseTransformPoint(localPosition);
			Vector3 val5 = val3 - val4;
			if (mDrag.scale.x == 0f)
			{
				val5.x = 0f;
			}
			if (mDrag.scale.y == 0f)
			{
				val5.y = 0f;
			}
			if (mDrag.scale.z == 0f)
			{
				val5.z = 0f;
			}
			SpringPanel.Begin(((Component)mDrag).gameObject, cachedTransform.localPosition - val5, springStrength).onFinished = onFinished;
		}
		else
		{
			mCenteredObject = null;
		}
	}
}
