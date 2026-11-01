using System.Collections;
using UnityEngine;

public class CubeSparcs : MonoBehaviour
{
	public GameObject[] parts;

	public PhysicMaterial material;

	private void Start()
	{
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < parts.Length; i++)
		{
			float num = Random.Range(-0.4f, 0.4f);
			float num2 = Random.Range(-0.4f, 0.4f);
			float num3 = Random.Range(-0.4f, 0.4f);
			MeshCollider val = parts[i].AddComponent<MeshCollider>();
			val.convex = true;
			((Collider)val).material = material;
			Rigidbody val2 = parts[i].AddComponent<Rigidbody>();
			val2.AddForce(new Vector3(num * 300f, num2 * 300f, num3 * 300f));
			parts[i].layer = LayerMask.NameToLayer("ParticleCubesDestroy");
		}
		AffectColor();
		((MonoBehaviour)this).StartCoroutine(FadeProcess());
	}

	public void SetTexture(Texture2D texture)
	{
		for (int i = 0; i < parts.Length; i++)
		{
			parts[i].renderer.material.mainTexture = (Texture)(object)texture;
		}
	}

	private IEnumerator FadeProcess()
	{
		yield return (object)new WaitForSeconds(1f);
		float alpha = 1f;
		float startTime = Time.time;
		while (alpha > 0f)
		{
			alpha = Mathf.Clamp01(1f - (Time.time - startTime));
			MaterialExt.SetTransparent(((Component)this).gameObject, alpha);
			AffectColor();
			yield return (object)new WaitForSeconds(0.01f);
		}
		yield return (object)new WaitForSeconds(1f);
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}

	private void AffectColor()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		int x = (int)((Component)this).transform.position.x;
		int y = (int)((Component)this).transform.position.z;
		int z = (int)((Component)this).transform.position.y;
		if (WorldData.Instance.IsValidBlock(x, y, z))
		{
			byte blockLight = World.Instance.Lighting.GetBlockLight(x, y, z);
			float num = (float)(blockLight >> 4) * (1f - TimeOfDay.NormalizedTime);
			float num2 = blockLight & 4;
			float num3 = Mathf.Max(num, num2) / (float)World.Instance.Lighting.LightingSteps;
			float num4 = (1f - num3) * -0.4f;
			MaterialExt.AffectColor(((Component)this).gameObject, num4, num4, num4);
		}
	}
}
