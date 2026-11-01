using UnityEngine;

[AddComponentMenu("NGUI/Examples/Drag and Drop Surface")]
public class DragDropSurface : MonoBehaviour
{
	public bool rotatePlacedObject;

	private void OnDrop(GameObject go)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		DragDropItem component = go.GetComponent<DragDropItem>();
		if ((Object)(object)component != (Object)null)
		{
			GameObject val = NGUITools.AddChild(((Component)this).gameObject, component.prefab);
			Transform transform = val.transform;
			transform.position = UICamera.lastHit.point;
			if (rotatePlacedObject)
			{
				transform.rotation = Quaternion.LookRotation(UICamera.lastHit.normal) * Quaternion.Euler(90f, 0f, 0f);
			}
			Object.Destroy((Object)(object)go);
		}
	}
}
