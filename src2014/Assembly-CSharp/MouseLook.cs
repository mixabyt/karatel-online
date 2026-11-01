using UnityEngine;

[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour
{
	public enum RotationAxes
	{
		MouseXAndY,
		MouseX,
		MouseY
	}

	public bool disableInput;

	public RotationAxes axes;

	public float minimumX = -360f;

	public float maximumX = 360f;

	public float minimumY = -60f;

	public float maximumY = 60f;

	public float sinematicLerp = 1f;

	public bool invertX;

	public bool invertY;

	private float rotationX;

	private float rotationY;

	private Quaternion originalRotation;

	public float RotationX
	{
		get
		{
			return rotationX;
		}
		set
		{
			rotationX = value;
			bool flag = disableInput;
			disableInput = true;
			Update();
			disableInput = flag;
		}
	}

	public float RotationY
	{
		get
		{
			return rotationY;
		}
		set
		{
			rotationY = value;
			bool flag = disableInput;
			disableInput = true;
			Update();
			disableInput = flag;
		}
	}

	public Quaternion OriginalRotation
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return originalRotation;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			originalRotation = value;
			Update();
		}
	}

	public float ScopeFactor
	{
		get
		{
			CameraController component = ((Component)((Component)this).transform.root).GetComponent<CameraController>();
			return (!component.Scope || !component.CurGun.sniper) ? 1f : 0.3f;
		}
	}

	private void Update()
	{
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		if (!Screen.lockCursor)
		{
			return;
		}
		if (axes == RotationAxes.MouseXAndY)
		{
			if (!disableInput)
			{
				rotationX += Input.GetAxis("Mouse X") * ProfileINI.mouse_sens * ((!invertX) ? 1f : (-1f)) * ScopeFactor;
				rotationY += Input.GetAxis("Mouse Y") * ProfileINI.mouse_sens * ((!invertY) ? 1f : (-1f)) * ScopeFactor;
				rotationX = ClampAngle(rotationX, minimumX, maximumX);
				rotationY = ClampAngle(rotationY, minimumY, maximumY);
			}
			Quaternion val = Quaternion.AngleAxis(rotationX, Vector3.up);
			Quaternion val2 = Quaternion.AngleAxis(rotationY, Vector3.left);
			if (MainMenu.Instance.CinematicCamera)
			{
				((Component)this).transform.localRotation = Quaternion.Lerp(((Component)this).transform.localRotation, originalRotation * val * val2, Time.deltaTime * sinematicLerp);
			}
			else
			{
				((Component)this).transform.localRotation = originalRotation * val * val2;
			}
		}
		else if (axes == RotationAxes.MouseX)
		{
			if (!disableInput)
			{
				rotationX += Input.GetAxis("Mouse X") * ProfileINI.mouse_sens * ((!invertX) ? 1f : (-1f)) * ScopeFactor;
				rotationX = ClampAngle(rotationX, minimumX, maximumX);
			}
			Quaternion val3 = Quaternion.AngleAxis(rotationX, Vector3.up);
			if (MainMenu.Instance.CinematicCamera)
			{
				((Component)this).transform.localRotation = Quaternion.Lerp(((Component)this).transform.localRotation, originalRotation * val3, Time.deltaTime * sinematicLerp);
			}
			else
			{
				((Component)this).transform.localRotation = originalRotation * val3;
			}
		}
		else
		{
			if (!disableInput)
			{
				rotationY += Input.GetAxis("Mouse Y") * ProfileINI.mouse_sens * ((!invertY) ? 1f : (-1f)) * ScopeFactor;
				rotationY = ClampAngle(rotationY, minimumY, maximumY);
			}
			Quaternion val4 = Quaternion.AngleAxis(rotationY, Vector3.left);
			if (MainMenu.Instance.CinematicCamera)
			{
				((Component)this).transform.localRotation = Quaternion.Lerp(((Component)this).transform.localRotation, originalRotation * val4, Time.deltaTime * sinematicLerp);
			}
			else
			{
				((Component)this).transform.localRotation = originalRotation * val4;
			}
		}
	}

	private void Start()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)((Component)this).rigidbody))
		{
			((Component)this).rigidbody.freezeRotation = true;
		}
		originalRotation = ((Component)this).transform.localRotation;
	}

	public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}
}
