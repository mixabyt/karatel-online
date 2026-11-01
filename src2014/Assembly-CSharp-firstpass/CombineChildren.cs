using System;
using System.Collections;
using UnityEngine;

[AddComponentMenu("Mesh/Combine Children")]
public class CombineChildren : MonoBehaviour
{
	public bool generateTriangleStrips = true;

	private void Start()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected Obj, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Expected Obj, but got Unknown
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Expected Obj, but got Unknown
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Expected Obj, but got Unknown
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Expected Obj, but got Unknown
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Expected Obj, but got Unknown
		Component[] componentsInChildren = ((Component)this).GetComponentsInChildren(typeof(MeshFilter));
		Matrix4x4 worldToLocalMatrix = ((Component)this).transform.worldToLocalMatrix;
		Hashtable hashtable = new Hashtable();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			MeshFilter val = (MeshFilter)componentsInChildren[i];
			Renderer renderer = componentsInChildren[i].renderer;
			MeshCombineUtility.MeshInstance meshInstance = new MeshCombineUtility.MeshInstance
			{
				mesh = val.sharedMesh
			};
			if (!((Object)(object)renderer != (Object)null) || !renderer.enabled || !((Object)(object)meshInstance.mesh != (Object)null))
			{
				continue;
			}
			meshInstance.transform = worldToLocalMatrix * ((Component)val).transform.localToWorldMatrix;
			Material[] sharedMaterials = renderer.sharedMaterials;
			for (int j = 0; j < sharedMaterials.Length; j++)
			{
				meshInstance.subMeshIndex = Math.Min(j, meshInstance.mesh.subMeshCount - 1);
				ArrayList arrayList = (ArrayList)hashtable[sharedMaterials[j]];
				if (arrayList != null)
				{
					arrayList.Add(meshInstance);
					continue;
				}
				arrayList = new ArrayList();
				arrayList.Add(meshInstance);
				hashtable.Add(sharedMaterials[j], arrayList);
			}
			renderer.enabled = false;
		}
		foreach (DictionaryEntry item in hashtable)
		{
			ArrayList arrayList2 = (ArrayList)item.Value;
			MeshCombineUtility.MeshInstance[] combines = (MeshCombineUtility.MeshInstance[])arrayList2.ToArray(typeof(MeshCombineUtility.MeshInstance));
			if (hashtable.Count == 1)
			{
				if ((Object)(object)((Component)this).GetComponent(typeof(MeshFilter)) == (Object)null)
				{
					((Component)this).gameObject.AddComponent(typeof(MeshFilter));
				}
				if (!Object.op_Implicit((Object)(object)((Component)this).GetComponent("MeshRenderer")))
				{
					((Component)this).gameObject.AddComponent("MeshRenderer");
				}
				MeshFilter val2 = (MeshFilter)((Component)this).GetComponent(typeof(MeshFilter));
				val2.mesh = MeshCombineUtility.Combine(combines, generateTriangleStrips);
				((Component)this).renderer.material = (Material)item.Key;
				((Component)this).renderer.enabled = true;
			}
			else
			{
				GameObject val3 = new GameObject("Combined mesh");
				val3.transform.parent = ((Component)this).transform;
				val3.transform.localScale = Vector3.one;
				val3.transform.localRotation = Quaternion.identity;
				val3.transform.localPosition = Vector3.zero;
				val3.AddComponent(typeof(MeshFilter));
				val3.AddComponent("MeshRenderer");
				val3.renderer.material = (Material)item.Key;
				MeshFilter val4 = (MeshFilter)val3.GetComponent(typeof(MeshFilter));
				val4.mesh = MeshCombineUtility.Combine(combines, generateTriangleStrips);
			}
		}
	}
}
