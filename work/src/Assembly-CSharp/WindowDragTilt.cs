using UnityEngine;

[AddComponentMenu("NGUI/Examples/Window Drag Tilt")]
public class WindowDragTilt : MonoBehaviour
{
	public int updateOrder;

	public float degrees = 30f;

	private Vector3 mLastPos;

	private Transform mTrans;

	private float mAngle;

	private bool mInit = true;

	private void Start()
	{
		UpdateManager.AddCoroutine((MonoBehaviour)(object)this, updateOrder, CoroutineUpdate);
	}

	private void OnEnable()
	{
		mInit = true;
	}

	private void CoroutineUpdate(float delta)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (mInit)
		{
			mInit = false;
			mTrans = ((Component)this).transform;
			mLastPos = mTrans.position;
		}
		Vector3 val = mTrans.position - mLastPos;
		mLastPos = mTrans.position;
		mAngle += val.x * degrees;
		mAngle = NGUIMath.SpringLerp(mAngle, 0f, 20f, delta);
		mTrans.localRotation = Quaternion.Euler(0f, 0f, 0f - mAngle);
	}
}
