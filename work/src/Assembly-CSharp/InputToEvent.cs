using UnityEngine;

public class InputToEvent : MonoBehaviour
{
	private GameObject lastGo;

	public static Vector3 inputHitPos;

	public bool DetectPointedAtGameObject;

	public static GameObject goPointedAt { get; private set; }

	private void Update()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Invalid comparison between Unknown and I4
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		if (DetectPointedAtGameObject)
		{
			goPointedAt = RaycastObject(Vector2.op_Implicit(Input.mousePosition));
		}
		if (Input.touchCount > 0)
		{
			Touch touch = Input.GetTouch(0);
			if ((int)touch.phase == 0)
			{
				Press(touch.position);
			}
			else if ((int)touch.phase == 3)
			{
				Release(touch.position);
			}
		}
		else
		{
			if (Input.GetMouseButtonDown(0))
			{
				Press(Vector2.op_Implicit(Input.mousePosition));
			}
			if (Input.GetMouseButtonUp(0))
			{
				Release(Vector2.op_Implicit(Input.mousePosition));
			}
		}
	}

	private void Press(Vector2 screenPos)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		lastGo = RaycastObject(screenPos);
		if ((Object)(object)lastGo != (Object)null)
		{
			lastGo.SendMessage("OnPress", (SendMessageOptions)1);
		}
	}

	private void Release(Vector2 screenPos)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)lastGo != (Object)null)
		{
			GameObject val = RaycastObject(screenPos);
			if ((Object)(object)val == (Object)(object)lastGo)
			{
				lastGo.SendMessage("OnClick", (SendMessageOptions)1);
			}
			lastGo.SendMessage("OnRelease", (SendMessageOptions)1);
			lastGo = null;
		}
	}

	private GameObject RaycastObject(Vector2 screenPos)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		RaycastHit val = default;
		if (Physics.Raycast(((Component)this).camera.ScreenPointToRay(Vector2.op_Implicit(screenPos)), ref val, 200f))
		{
			inputHitPos = val.point;
			return ((Component)val.collider).gameObject;
		}
		return null;
	}
}
