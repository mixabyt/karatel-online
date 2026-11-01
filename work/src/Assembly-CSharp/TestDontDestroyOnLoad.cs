using UnityEngine;

public class TestDontDestroyOnLoad : MonoBehaviour
{
	private void Awake()
	{
		Object.DontDestroyOnLoad((Object)(object)((Component)this).gameObject);
	}
}
