using System.Collections;
using UnityEngine;

public class GUISaveBtn : MonoBehaviour
{
	private void Update()
	{
	}

	private void OnMouseEnter()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		iTween.ScaleTo(((Component)this).gameObject, new Vector3(0.02f, 0.02f, 0.02f), 0.5f);
	}

	private void OnMouseExit()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		iTween.ScaleTo(((Component)this).gameObject, new Vector3(0f, 0f, 0f), 0.5f);
	}

	private void OnMouseDown()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		Hashtable hashtable = new Hashtable();
		hashtable.Add("scale", (object)new Vector3(-0.01f, -0.01f, -0.01f));
		hashtable.Add("time", 0.1f);
		hashtable.Add("easetype", iTween.EaseType.linear);
		iTween.ScaleTo(((Component)this).gameObject, hashtable);
		Hashtable hashtable2 = new Hashtable();
		hashtable2.Add("scale", (object)new Vector3(0f, 0f, 0f));
		hashtable2.Add("time", 0.1f);
		hashtable2.Add("delay", 0.1f);
		hashtable2.Add("easetype", iTween.EaseType.linear);
		iTween.ScaleTo(((Component)this).gameObject, hashtable2);
	}
}
