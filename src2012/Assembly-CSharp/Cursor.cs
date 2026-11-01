using UnityEngine;

public class Cursor : bs
{
	private const float d = 3f;

	private const float len = 5f;

	internal CameraController cc;

	private Vector3[] _list = new Vector3[8]
	{
		Vector3.left * 3f,
		Vector3.left * 8f,
		Vector3.up * 3f,
		Vector3.up * 8f,
		Vector3.right * 3f,
		Vector3.right * 8f,
		Vector3.down * 3f,
		Vector3.down * 8f
	};

	private static Material green;

	public Cursor()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
	}

	private void Start()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		cc = ((Component)((Component)this).transform.root).GetComponent<CameraController>();
		green = MakeMaterial(new Color(0f, 0f, 1f, 1f));
	}

	private static Material MakeMaterial(Color c)
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Expected Obj, but got Unknown
		Material val = new Material("Shader \"Lines/Colored Blended\" {SubShader { Pass {     Blend SrcAlpha OneMinusSrcAlpha    ZWrite Off Cull Off Fog { Mode Off }    Color(" + c.r + "," + c.b + "," + c.g + "," + c.a + ") } } }");
		((Object)val).hideFlags = (HideFlags)13;
		((Object)val.shader).hideFlags = (HideFlags)13;
		return val;
	}

	public void OnPostRender()
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		if (!cc.CurGun.sniper && bs._Igor.enableGuns && !bs._Igor.spectate)
		{
			green.SetPass(0);
			GL.LoadOrtho();
			GL.Begin(1);
			Vector3 val = new Vector3((float)Screen.width, (float)Screen.height) / 2f;
			val.z = 0.7f;
			Vector3[] list = _list;
			for (int i = 0; i < list.Length; i++)
			{
				Vector3 val2 = list[i];
				Vector3 val3 = val + new Vector3(val2.x, val2.y);
				Vector3 val4 = val3;
				Vector3 normalized = val2.normalized;
				Vector3 recoil = cc.Recoil;
				val3 = val4 + normalized * (1f + recoil.magnitude * 100f);
				val3.x /= (float)Screen.width;
				val3.y /= (float)Screen.height;
				GL.Vertex(val3);
			}
			GL.End();
		}
	}
}
