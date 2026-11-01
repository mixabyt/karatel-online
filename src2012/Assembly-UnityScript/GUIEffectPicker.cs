using System;
using Boo.Lang.Runtime;
using UnityEngine;
using UnityScript.Lang;

[Serializable]
public class GUIEffectPicker : MonoBehaviour
{
	public Transform[] FX;

	public override void OnGUI()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < Extensions.get_length((System.Array)FX); i++)
		{
			if (GUI.Button(new Rect((float)(120 * i), 0f, 120f, 80f), ((Object)((Component)FX[i]).gameObject).name))
			{
				RuntimeServices.SetProperty(((Component)this).gameObject.GetComponent("ExplosionAtPoint"), "explosionPrefab", FX[i]);
			}
		}
		Time.timeScale = GUI.HorizontalSlider(new Rect(0f, 130f, (float)Screen.width, 10f), Time.timeScale, 0f, 15f);
	}

	public override void Main()
	{
	}
}
