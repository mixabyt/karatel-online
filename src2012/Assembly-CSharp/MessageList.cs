using System.Collections.Generic;
using UnityEngine;

public class MessageList : MonoBehaviour
{
	public GUIText messagePrefab;

	public float lineSize = 20f;

	public int layerTag;

	public bool open;

	private List<GUIText> messages = new List<GUIText>();

	private void Awake()
	{
		if (!Object.op_Implicit((Object)(object)messagePrefab))
		{
			((Behaviour)this).enabled = false;
			Debug.Log((object)"Must set the GUIText prefab for MessageList");
		}
	}

	public void ShowAllMessage()
	{
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		open = true;
		for (int i = 0; i < messages.Count; i++)
		{
			if (!Object.op_Implicit((Object)(object)messages[i]))
			{
				messages.RemoveAt(i--);
				continue;
			}
			if (!((Component)messages[i]).gameObject.GetComponent<TimedFadeText>().isFaded)
			{
				((Component)messages[i]).gameObject.GetComponent<TimedFadeText>().isFaded = true;
			}
			Color color = messages[i].material.color;
			color.a = 1f;
			messages[i].material.color = color;
			GUITexture component = ((Component)messages[i]).GetComponent<GUITexture>();
			if ((Object)(object)component != (Object)null)
			{
				component.color = new Color(component.color.r, component.color.g, component.color.b, color.a);
			}
		}
	}

	public void HideAllMessage()
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		open = false;
		for (int i = 0; i < messages.Count; i++)
		{
			if (!Object.op_Implicit((Object)(object)messages[i]))
			{
				messages.RemoveAt(i--);
				continue;
			}
			Color color = messages[i].material.color;
			color.a = 0f;
			messages[i].material.color = color;
			GUITexture component = ((Component)messages[i]).GetComponent<GUITexture>();
			if ((Object)(object)component != (Object)null)
			{
				component.color = new Color(component.color.r, component.color.g, component.color.b, color.a);
			}
		}
	}

	public void AddMessage(string text, Texture picture, Color color)
	{
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < messages.Count; i++)
		{
			if (!Object.op_Implicit((Object)(object)messages[i]))
			{
				messages.RemoveAt(i--);
				continue;
			}
			messages[i].pixelOffset = messages[i].pixelOffset + new Vector2(0f, lineSize);
			GUITexture component = ((Component)messages[i]).GetComponent<GUITexture>();
			if ((Object)(object)component != (Object)null)
			{
				float x = messages[i].pixelOffset.x;
				Rect screenRect = ((GUIElement)messages[i]).GetScreenRect();
				component.pixelInset = new Rect(x + screenRect.width + 4f, messages[i].pixelOffset.y - 16f, 16f, 16f);
			}
		}
		Object val = Object.Instantiate((Object)(object)messagePrefab, ((Component)messagePrefab).transform.position, ((Component)messagePrefab).transform.rotation);
		GUIText val2 = (GUIText)(object)((val is GUIText) ? val : null);
		val2.text = text;
		val2.material.color = color;
		((Component)val2).gameObject.layer = layerTag;
		messages.Add(val2);
		if (open)
		{
			((Component)val2).gameObject.GetComponent<TimedFadeText>().isFaded = true;
		}
		if ((Object)(object)picture != (Object)null)
		{
			GUITexture val3 = ((Component)val2).gameObject.AddComponent<GUITexture>();
			val3.texture = picture;
			float x2 = val2.pixelOffset.x;
			Rect screenRect2 = ((GUIElement)val2).GetScreenRect();
			val3.pixelInset = new Rect(x2 + screenRect2.width + 4f, val2.pixelOffset.y - 16f, 16f, 16f);
		}
	}
}
