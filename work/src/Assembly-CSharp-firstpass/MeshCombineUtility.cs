using UnityEngine;

public class MeshCombineUtility
{
	public struct MeshInstance
	{
		public Mesh mesh;

		public int subMeshIndex;

		public Matrix4x4 transform;
	}

	public static Mesh Combine(MeshInstance[] combines, bool generateStrips)
	{
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0514: Unknown result type (might be due to invalid IL or missing references)
		//IL_051b: Expected Obj, but got Unknown
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < combines.Length; i++)
		{
			MeshInstance meshInstance = combines[i];
			if (!Object.op_Implicit((Object)(object)meshInstance.mesh))
			{
				continue;
			}
			num += meshInstance.mesh.vertexCount;
			if (!generateStrips)
			{
				continue;
			}
			int num4 = meshInstance.mesh.GetTriangles(meshInstance.subMeshIndex).Length;
			if (num4 != 0)
			{
				if (num3 != 0)
				{
					num3 = (((num3 & 1) != 1) ? (num3 + 2) : (num3 + 3));
				}
				num3 += num4;
			}
			else
			{
				generateStrips = false;
			}
		}
		if (!generateStrips)
		{
			for (int j = 0; j < combines.Length; j++)
			{
				MeshInstance meshInstance2 = combines[j];
				if (Object.op_Implicit((Object)(object)meshInstance2.mesh))
				{
					num2 += meshInstance2.mesh.GetTriangles(meshInstance2.subMeshIndex).Length;
				}
			}
		}
		Vector3[] array = new Vector3[num];
		Vector3[] array2 = new Vector3[num];
		Vector4[] array3 = new Vector4[num];
		Vector2[] array4 = new Vector2[num];
		Vector2[] array5 = new Vector2[num];
		Color[] array6 = new Color[num];
		int[] array7 = new int[num2];
		int[] array8 = new int[num3];
		int offset = 0;
		for (int k = 0; k < combines.Length; k++)
		{
			MeshInstance meshInstance3 = combines[k];
			if (Object.op_Implicit((Object)(object)meshInstance3.mesh))
			{
				Copy(meshInstance3.mesh.vertexCount, meshInstance3.mesh.vertices, array, ref offset, meshInstance3.transform);
			}
		}
		offset = 0;
		for (int l = 0; l < combines.Length; l++)
		{
			MeshInstance meshInstance4 = combines[l];
			if (Object.op_Implicit((Object)(object)meshInstance4.mesh))
			{
				Matrix4x4 transform = meshInstance4.transform;
				Matrix4x4 inverse = transform.inverse;
				transform = inverse.transpose;
				CopyNormal(meshInstance4.mesh.vertexCount, meshInstance4.mesh.normals, array2, ref offset, transform);
			}
		}
		offset = 0;
		for (int m = 0; m < combines.Length; m++)
		{
			MeshInstance meshInstance5 = combines[m];
			if (Object.op_Implicit((Object)(object)meshInstance5.mesh))
			{
				Matrix4x4 transform2 = meshInstance5.transform;
				Matrix4x4 inverse2 = transform2.inverse;
				transform2 = inverse2.transpose;
				CopyTangents(meshInstance5.mesh.vertexCount, meshInstance5.mesh.tangents, array3, ref offset, transform2);
			}
		}
		offset = 0;
		for (int n = 0; n < combines.Length; n++)
		{
			MeshInstance meshInstance6 = combines[n];
			if (Object.op_Implicit((Object)(object)meshInstance6.mesh))
			{
				Copy(meshInstance6.mesh.vertexCount, meshInstance6.mesh.uv, array4, ref offset);
			}
		}
		offset = 0;
		for (int num5 = 0; num5 < combines.Length; num5++)
		{
			MeshInstance meshInstance7 = combines[num5];
			if (Object.op_Implicit((Object)(object)meshInstance7.mesh))
			{
				Copy(meshInstance7.mesh.vertexCount, meshInstance7.mesh.uv1, array5, ref offset);
			}
		}
		offset = 0;
		for (int num6 = 0; num6 < combines.Length; num6++)
		{
			MeshInstance meshInstance8 = combines[num6];
			if (Object.op_Implicit((Object)(object)meshInstance8.mesh))
			{
				CopyColors(meshInstance8.mesh.vertexCount, meshInstance8.mesh.colors, array6, ref offset);
			}
		}
		int num7 = 0;
		int num8 = 0;
		int num9 = 0;
		for (int num10 = 0; num10 < combines.Length; num10++)
		{
			MeshInstance meshInstance9 = combines[num10];
			if (!Object.op_Implicit((Object)(object)meshInstance9.mesh))
			{
				continue;
			}
			if (generateStrips)
			{
				int[] triangles = meshInstance9.mesh.GetTriangles(meshInstance9.subMeshIndex);
				if (num8 != 0)
				{
					if ((num8 & 1) == 1)
					{
						array8[num8] = array8[num8 - 1];
						array8[num8 + 1] = triangles[0] + num9;
						array8[num8 + 2] = triangles[0] + num9;
						num8 += 3;
					}
					else
					{
						array8[num8] = array8[num8 - 1];
						array8[num8 + 1] = triangles[0] + num9;
						num8 += 2;
					}
				}
				for (int num11 = 0; num11 < triangles.Length; num11++)
				{
					array8[num11 + num8] = triangles[num11] + num9;
				}
				num8 += triangles.Length;
			}
			else
			{
				int[] triangles2 = meshInstance9.mesh.GetTriangles(meshInstance9.subMeshIndex);
				for (int num12 = 0; num12 < triangles2.Length; num12++)
				{
					array7[num12 + num7] = triangles2[num12] + num9;
				}
				num7 += triangles2.Length;
			}
			num9 += meshInstance9.mesh.vertexCount;
		}
		Mesh val = new Mesh();
		((Object)val).name = "Combined Mesh";
		val.vertices = array;
		val.normals = array2;
		val.colors = array6;
		val.uv = array4;
		val.uv1 = array5;
		val.tangents = array3;
		if (generateStrips)
		{
			val.SetTriangles(array8, 0);
		}
		else
		{
			val.triangles = array7;
		}
		return val;
	}

	private static void Copy(int vertexcount, Vector3[] src, Vector3[] dst, ref int offset, Matrix4x4 transform)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < src.Length; i++)
		{
			ref Vector3 reference = ref dst[i + offset];
			reference = transform.MultiplyPoint(src[i]);
		}
		offset += vertexcount;
	}

	private static void CopyNormal(int vertexcount, Vector3[] src, Vector3[] dst, ref int offset, Matrix4x4 transform)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < src.Length; i++)
		{
			ref Vector3 reference = ref dst[i + offset];
			Vector3 val = transform.MultiplyVector(src[i]);
			reference = val.normalized;
		}
		offset += vertexcount;
	}

	private static void Copy(int vertexcount, Vector2[] src, Vector2[] dst, ref int offset)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < src.Length; i++)
		{
			ref Vector2 reference = ref dst[i + offset];
			reference = src[i];
		}
		offset += vertexcount;
	}

	private static void CopyColors(int vertexcount, Color[] src, Color[] dst, ref int offset)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < src.Length; i++)
		{
			ref Color reference = ref dst[i + offset];
			reference = src[i];
		}
		offset += vertexcount;
	}

	private static void CopyTangents(int vertexcount, Vector4[] src, Vector4[] dst, ref int offset, Matrix4x4 transform)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < src.Length; i++)
		{
			Vector4 val = src[i];
			Vector3 val2 = new Vector3(val.x, val.y, val.z);
			Vector3 val3 = transform.MultiplyVector(val2);
			val2 = val3.normalized;
			ref Vector4 reference = ref dst[i + offset];
			reference = new Vector4(val2.x, val2.y, val2.z, val.w);
		}
		offset += vertexcount;
	}
}
