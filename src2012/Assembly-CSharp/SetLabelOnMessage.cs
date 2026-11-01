using System.Collections;
using UnityEngine;

public class SetLabelOnMessage : MonoBehaviour
{
	public GameObject prefabLabel;

	public float maxDistance = 500f;

	public Vector3 offset = Vector3.up;

	public bool clampToScreen;

	public float clampBorderSize = 0.05f;

	private Transform target;

	private Camera cam;

	private GUIText label;

	private Transform labelTransform;

	private Transform camTransform;

	private bool _RaycastFail;

	public SetLabelOnMessage()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
	}

	public void SetLabel(string text)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		label = ((GameObject)Object.Instantiate((Object)(object)prefabLabel, Vector3.zero, Quaternion.identity)).guiText;
		label.text = text;
		target = ((Component)this).GetComponent<PlayerNetwork>().SpinBone.transform;
		cam = Camera.main;
		labelTransform = ((Component)label).transform;
		camTransform = ((Component)cam).transform;
	}

	public void SetColor(Color color)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)label != (Object)null)
		{
			float a = label.material.color.a;
			label.material.color = new Color(color.r, color.g, color.b, a);
		}
	}

	public void Hide()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)label != (Object)null)
		{
			Color color = label.material.color;
			color.a = 0f;
			label.material.color = color;
		}
	}

	public void Show()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)label != (Object)null)
		{
			Color color = label.material.color;
			color.a = 1f;
			label.material.color = color;
		}
	}

	private void OnDisable()
	{
		if ((Object)(object)label != (Object)null)
		{
			Object.DestroyImmediate((Object)(object)label);
			label = null;
		}
	}

	private void LateUpdate()
	{
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		cam = Camera.main;
		camTransform = ((Component)cam).transform;
		if (!((Object)(object)target == (Object)null) && !((Object)(object)label == (Object)null) && !((Object)(object)cam == (Object)null))
		{
			Vector3 val2;
			if (clampToScreen)
			{
				Vector3 val = camTransform.InverseTransformPoint(target.position);
				val.z = Mathf.Max(val.z, 1f);
				val2 = cam.WorldToViewportPoint(camTransform.TransformPoint(val + offset));
				val2 = new Vector3(Mathf.Clamp(val2.x, clampBorderSize, 1f - clampBorderSize), Mathf.Clamp(val2.y, clampBorderSize, 1f - clampBorderSize), val2.z);
			}
			else
			{
				val2 = cam.WorldToViewportPoint(target.position + offset);
			}
			labelTransform.position = val2;
			((Behaviour)label).enabled = val2.z > 0f && val2.z <= maxDistance && !_RaycastFail;
		}
	}

	public void SetRaycast(bool enable)
	{
		((MonoBehaviour)this).StopCoroutine("RaycastPlayerProcess");
		if (enable)
		{
			((MonoBehaviour)this).StartCoroutine("RaycastPlayerProcess");
		}
		else
		{
			_RaycastFail = false;
		}
	}

	private IEnumerator RaycastPlayerProcess()
	{
		while (true)
		{
			RaycastPlayer();
			yield return (object)new WaitForSeconds(0.5f);
		}
	}

	private void RaycastPlayer()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		_RaycastFail = true;
		if ((Object)(object)CameraController.Instance == (Object)null)
		{
			return;
		}
		if ((Object)(object)((Component)this).gameObject == (Object)(object)WorldGameObject.INSTANCE.MainPlayer)
		{
			_RaycastFail = false;
			return;
		}
		Vector3 position = ((Component)CameraController.RaycastCamera).transform.position;
		PlayerNetwork component = ((Component)this).gameObject.GetComponent<PlayerNetwork>();
		Vector3 val;
		if ((Object)(object)component != (Object)null && (Object)(object)component.HeadBone != (Object)null && component.HeadBone.transform.GetChildCount() > 0)
		{
			val = ((Component)component.HeadBone.transform.GetChild(0)).transform.position;
		}
		else if ((Object)(object)component != (Object)null && (Object)(object)component.HeadBone != (Object)null)
		{
			val = component.HeadBone.transform.position;
		}
		else
		{
			Bounds bounds = ((Component)this).gameObject.collider.bounds;
			val = bounds.center;
		}
		float num = Vector3.Distance(position, val);
		if (!(num > maxDistance))
		{
			int num2 = 1 << LayerMask.NameToLayer("Terrain");
			Vector3 val2 = val - position;
			if (!Physics.Raycast(position, val2.normalized, num, num2))
			{
				_RaycastFail = false;
			}
		}
	}
}
