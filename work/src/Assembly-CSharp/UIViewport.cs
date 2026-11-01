using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Viewport Camera")]
public class UIViewport : MonoBehaviour
{
	public Camera sourceCamera;

	public Transform topLeft;

	public Transform bottomRight;

	public float fullSize = 1f;

	private Camera mCam;

	private void Start()
	{
		mCam = ((Component)this).camera;
		if ((Object)(object)sourceCamera == (Object)null)
		{
			sourceCamera = Camera.main;
		}
	}

	private void LateUpdate()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)topLeft != (Object)null && (Object)(object)bottomRight != (Object)null)
		{
			Vector3 val = sourceCamera.WorldToScreenPoint(topLeft.position);
			Vector3 val2 = sourceCamera.WorldToScreenPoint(bottomRight.position);
			Rect val3 = new Rect(val.x / (float)Screen.width, val2.y / (float)Screen.height, (val2.x - val.x) / (float)Screen.width, (val.y - val2.y) / (float)Screen.height);
			float num = fullSize * val3.height;
			if (val3 != mCam.rect)
			{
				mCam.rect = val3;
			}
			if (mCam.orthographicSize != num)
			{
				mCam.orthographicSize = num;
			}
		}
	}
}
