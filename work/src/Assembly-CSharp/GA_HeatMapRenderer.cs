using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GA_HeatMapRenderer : MonoBehaviour
{
	public enum RenderModel
	{
		Transparent,
		TransparentOverlay
	}

	public RenderModel CurrentRenderModel = RenderModel.TransparentOverlay;

	public Color MinColor = Color.yellow;

	public Color MaxColor = Color.red;

	public float RangeMin;

	public float RangeMax = 1f;

	public float MaxRadius = 1f;

	public bool ShowValueLabels;

	public GA_Histogram Histogram;

	public GA_Histogram.HistogramScale HistogramScale;

	public GA_HeatMapDataFilterBase datafilter;

	public GameObject BillBoard;

	private List<GameObject> DestroyList;

	public GA_HeatMapRenderer()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
	}

	private void OnEnable()
	{
		datafilter = ((Component)this).GetComponent<GA_HeatMapDataFilterBase>();
	}

	public void OnDataUpdate()
	{
		if (DestroyList == null)
		{
			DestroyList = new List<GameObject>();
		}
		for (int i = 0; i < ((Component)this).transform.childCount; i++)
		{
			GameObject gameObject = ((Component)((Component)this).transform.GetChild(i)).gameObject;
			if (!DestroyList.Contains(gameObject) && (Object)(object)gameObject.GetComponent<GA_HeatmapData>() == (Object)null && ((Object)gameObject).name.Contains("GA_"))
			{
				DestroyList.Add(gameObject);
			}
		}
		RenderModelChanged();
		if ((Object)(object)datafilter == (Object)null)
		{
			datafilter = ((Component)this).GetComponent<GA_HeatMapDataFilterBase>();
		}
		if ((Object)(object)datafilter != (Object)null)
		{
			List<GA_DataPoint> data = datafilter.GetData();
			RecalculateHistogram();
			if (data == null || data.Count == 0)
			{
				GA.Log("GameAnalytics: No data to create heatmap. Returning.");
			}
			else
			{
				createBillboards(data);
			}
		}
		else
		{
			GA.Log("GameAnalytics: GA_HeatMapDataFilterBase component missing.");
		}
	}

	private void createBillboards(List<GA_DataPoint> data)
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected Obj, but got Unknown
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Expected Obj, but got Unknown
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		GA.Log("GameAnalytics: Creating mesh billboards for heapmap");
		if (data == null)
		{
			return;
		}
		int num = data.Count;
		int num2 = 0;
		while (num > 0)
		{
			int num3 = num;
			int num4 = (int)Mathf.Floor(21666.666f);
			if (num3 > num4)
			{
				num3 = num4;
			}
			BillBoard = new GameObject("GA_Billboards-" + num2 + "-" + (num2 + num3), new Type[2]
			{
				typeof(MeshFilter),
				typeof(MeshRenderer)
			});
			BillBoard.transform.parent = ((Component)this).transform;
			BillBoard.transform.localPosition = Vector3.zero;
			Vector3[] array = new Vector3[num3 * 3];
			Vector3[] array2 = new Vector3[num3 * 3];
			Vector2[] array3 = new Vector2[num3 * 3];
			Color[] array4 = new Color[num3 * 3];
			int[] array5 = new int[num3 * 3];
			for (int i = 0; i < num3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					ref Vector3 reference = ref array[3 * i + j];
					reference = data[num2].position;
					ref Color reference2 = ref array4[3 * i + j];
					reference2 = new Color(data[num2].density, 0f, 0f, 1f);
					float num5 = (float)j * (float)Math.PI * 2f / 3f;
					ref Vector2 reference3 = ref array3[3 * i + j];
					reference3 = new Vector2(Mathf.Cos(num5), Mathf.Sin(num5));
					ref Vector3 reference4 = ref array2[3 * i + j];
					reference4 = Vector3.zero;
				}
				array5[i * 3] = i * 3;
				array5[i * 3 + 1] = i * 3 + 2;
				array5[i * 3 + 2] = i * 3 + 1;
				num2++;
			}
			MeshFilter component = BillBoard.GetComponent<MeshFilter>();
			Mesh val = new Mesh();
			val.vertices = array;
			val.normals = array2;
			val.uv = array3;
			val.colors = array4;
			val.triangles = array5;
			component.sharedMesh = val;
			component.sharedMesh.RecalculateBounds();
			num -= num3;
			SetMaterialVariables();
		}
	}

	public void SetMaterialVariables()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected Obj, but got Unknown
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		MeshRenderer[] componentsInChildren = ((Component)this).gameObject.GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer val in componentsInChildren)
		{
			if (((Object)((Component)val).gameObject).name.Contains("GA_Billboards"))
			{
				if ((Object)(object)((Renderer)val).sharedMaterial == (Object)null)
				{
					((Renderer)val).sharedMaterial = new Material(Shader.Find("Custom/GA_HeatMapSolid"));
				}
				RenderModel currentRenderModel = CurrentRenderModel;
				if (currentRenderModel == RenderModel.TransparentOverlay)
				{
					((Renderer)val).sharedMaterial.shader = Shader.Find("Custom/GA_HeatMapTransparentOverlay");
				}
				else
				{
					((Renderer)val).sharedMaterial.shader = Shader.Find("Custom/GA_HeatMapTransparent");
				}
				((Renderer)val).sharedMaterial.SetColor("_Cold", MinColor);
				((Renderer)val).sharedMaterial.SetColor("_Warm", MaxColor);
				((Renderer)val).sharedMaterial.SetFloat("_RangeMin", RangeMin);
				((Renderer)val).sharedMaterial.SetFloat("_RangeWidth", RangeMax - RangeMin);
				((Renderer)val).sharedMaterial.SetFloat("_MaxRadius", MaxRadius);
				float num = (0f - Histogram.RealDataMin) / (Histogram.RealDataMax - Histogram.RealDataMin);
				((Renderer)val).sharedMaterial.SetFloat("_RangeZero", num);
				((Renderer)val).sharedMaterial.SetColor("_ColZero", Color.white);
			}
		}
	}

	public void RecalculateHistogram()
	{
		if ((Object)(object)datafilter != (Object)null)
		{
			List<GA_DataPoint> data = datafilter.GetData();
			if (data != null)
			{
				Histogram.Recalculate(data, 40, HistogramScale);
			}
		}
	}

	public void RenderModelChanged()
	{
		SetMaterialVariables();
	}

	public void OnScaleChanged()
	{
		RecalculateHistogram();
	}

	private void Update()
	{
		if (DestroyList == null)
		{
			return;
		}
		foreach (GameObject destroy in DestroyList)
		{
			if (Application.isPlaying)
			{
				Object.Destroy((Object)(object)destroy);
			}
		}
	}
}
