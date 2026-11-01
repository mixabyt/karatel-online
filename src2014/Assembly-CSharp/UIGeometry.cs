using UnityEngine;

public class UIGeometry
{
	public BetterList<Vector3> verts = new BetterList<Vector3>();

	public BetterList<Vector2> uvs = new BetterList<Vector2>();

	public BetterList<Color32> cols = new BetterList<Color32>();

	private BetterList<Vector3> mRtpVerts = new BetterList<Vector3>();

	private Vector3 mRtpNormal;

	private Vector4 mRtpTan;

	public bool hasVertices => verts.size > 0;

	public bool hasTransformed => mRtpVerts != null && mRtpVerts.size > 0 && mRtpVerts.size == verts.size;

	public void Clear()
	{
		verts.Clear();
		uvs.Clear();
		cols.Clear();
		mRtpVerts.Clear();
	}

	public void ApplyOffset(Vector3 pivotOffset)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < verts.size; i++)
		{
			ref Vector3 reference = ref verts.buffer[i];
			reference += pivotOffset;
		}
	}

	public void ApplyTransform(Matrix4x4 widgetToPanel)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		if (verts.size > 0)
		{
			mRtpVerts.Clear();
			int i = 0;
			for (int size = verts.size; i < size; i++)
			{
				mRtpVerts.Add(widgetToPanel.MultiplyPoint3x4(verts[i]));
			}
			Vector3 val = widgetToPanel.MultiplyVector(Vector3.back);
			mRtpNormal = val.normalized;
			Vector3 val2 = widgetToPanel.MultiplyVector(Vector3.right);
			Vector3 normalized = val2.normalized;
			mRtpTan = new Vector4(normalized.x, normalized.y, normalized.z, -1f);
		}
		else
		{
			mRtpVerts.Clear();
		}
	}

	public void WriteToBuffers(BetterList<Vector3> v, BetterList<Vector2> u, BetterList<Color32> c, BetterList<Vector3> n, BetterList<Vector4> t)
	{
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		if (mRtpVerts == null || mRtpVerts.size <= 0)
		{
			return;
		}
		if (n == null)
		{
			for (int i = 0; i < mRtpVerts.size; i++)
			{
				v.Add(mRtpVerts.buffer[i]);
				u.Add(uvs.buffer[i]);
				c.Add(cols.buffer[i]);
			}
			return;
		}
		for (int j = 0; j < mRtpVerts.size; j++)
		{
			v.Add(mRtpVerts.buffer[j]);
			u.Add(uvs.buffer[j]);
			c.Add(cols.buffer[j]);
			n.Add(mRtpNormal);
			t.Add(mRtpTan);
		}
	}
}
