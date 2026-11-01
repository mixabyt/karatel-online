using UnityEngine;

[AddComponentMenu("NGUI/Examples/Pan With Mouse")]
public class PanWithMouse : IgnoreTimeScale
{
	public Vector2 degrees = new Vector2(5f, 3f);

	public float range = 1f;

	private Transform mTrans;

	private Quaternion mStart;

	private Vector2 mRot = Vector2.zero;

	public PanWithMouse()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
	}

	private void Start()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		mTrans = ((Component)this).transform;
		mStart = mTrans.localRotation;
	}

	private void Update()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		float num = UpdateRealTimeDelta();
		Vector3 mousePosition = Input.mousePosition;
		float num2 = (float)Screen.width * 0.5f;
		float num3 = (float)Screen.height * 0.5f;
		if (range < 0.1f)
		{
			range = 0.1f;
		}
		float num4 = Mathf.Clamp((mousePosition.x - num2) / num2 / range, -1f, 1f);
		float num5 = Mathf.Clamp((mousePosition.y - num3) / num3 / range, -1f, 1f);
		mRot = Vector2.Lerp(mRot, new Vector2(num4, num5), num * 5f);
		mTrans.localRotation = mStart * Quaternion.Euler((0f - mRot.y) * degrees.y, mRot.x * degrees.x, 0f);
	}
}
