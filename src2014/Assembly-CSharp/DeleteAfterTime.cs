using UnityEngine;

public class DeleteAfterTime : MonoBehaviour
{
	public float life_time = 1f;

	private void Start()
	{
		((MonoBehaviour)this).Invoke("DeleteInst", life_time);
	}

	private void Update()
	{
	}

	private void DeleteInst()
	{
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}
}
