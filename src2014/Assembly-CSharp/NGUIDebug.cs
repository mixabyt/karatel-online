using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Internal/Debug")]
public class NGUIDebug : MonoBehaviour
{
	private static List<string> mLines = new List<string>();

	private static NGUIDebug mInstance = null;

	public static void Log(string text)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected Obj, but got Unknown
		if (Application.isPlaying)
		{
			if (mLines.Count > 20)
			{
				mLines.RemoveAt(0);
			}
			mLines.Add(text);
			if ((Object)(object)mInstance == (Object)null)
			{
				GameObject val = new GameObject("_NGUI Debug");
				mInstance = val.AddComponent<NGUIDebug>();
				Object.DontDestroyOnLoad((Object)(object)val);
			}
		}
		else
		{
			Debug.Log((object)text);
		}
	}

	public static void DrawBounds(Bounds b)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		Vector3 center = b.center;
		Vector3 val = b.center - b.extents;
		Vector3 val2 = b.center + b.extents;
		Debug.DrawLine(new Vector3(val.x, val.y, center.z), new Vector3(val2.x, val.y, center.z), Color.red);
		Debug.DrawLine(new Vector3(val.x, val.y, center.z), new Vector3(val.x, val2.y, center.z), Color.red);
		Debug.DrawLine(new Vector3(val2.x, val.y, center.z), new Vector3(val2.x, val2.y, center.z), Color.red);
		Debug.DrawLine(new Vector3(val.x, val2.y, center.z), new Vector3(val2.x, val2.y, center.z), Color.red);
	}

	private void OnGUI()
	{
		int i = 0;
		for (int count = mLines.Count; i < count; i++)
		{
			GUILayout.Label(mLines[i], new GUILayoutOption[0]);
		}
	}
}
