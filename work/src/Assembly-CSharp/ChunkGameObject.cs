using UnityEngine;

public class ChunkGameObject : MonoBehaviour
{
	private MeshFilter _MeshFilter;

	private MeshCollider _MeshCollider;

	private MeshRenderer _MeshRenderer;

	private void Awake()
	{
		_MeshFilter = ((Component)this).gameObject.GetComponent<MeshFilter>();
		if ((Object)(object)_MeshFilter == (Object)null)
		{
			_MeshFilter = ((Component)this).gameObject.AddComponent<MeshFilter>();
		}
		_MeshCollider = ((Component)this).gameObject.GetComponent<MeshCollider>();
		if ((Object)(object)_MeshCollider == (Object)null)
		{
			_MeshCollider = ((Component)this).gameObject.AddComponent<MeshCollider>();
		}
		_MeshRenderer = ((Component)this).gameObject.GetComponent<MeshRenderer>();
		if ((Object)(object)((Renderer)_MeshRenderer).material.mainTexture == (Object)null)
		{
			((Renderer)_MeshRenderer).material.mainTexture = (Texture)(object)WorldGameObject.Instance.WorldTextureAtlas;
		}
		((Renderer)_MeshRenderer).material.SetFloat("_TimeOfDay", TimeOfDay.NormalizedTime);
	}

	public void CreateFromChunk(Chunk.MeshData data)
	{
		PMeshData pMeshData = PMeshData.AcquireFor(data);
		pMeshData.ApplyTo(_MeshFilter.mesh);
		_MeshCollider.sharedMesh = null;
		_MeshCollider.sharedMesh = _MeshFilter.mesh;
		_MeshFilter.mesh.RecalculateNormals();
		data.m_Vertices.Clear();
		data.m_Uvs.Clear();
		data.m_Colors.Clear();
		data.m_Indices.Clear();
	}
}
