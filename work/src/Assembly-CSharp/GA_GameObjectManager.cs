using System.Collections;
using UnityEngine;

public class GA_GameObjectManager : MonoBehaviour
{
	private void Awake()
	{
		Object.DontDestroyOnLoad((Object)(object)((Component)this).gameObject);
	}

	public void RunCoroutine(IEnumerator routine)
	{
		((MonoBehaviour)this).StartCoroutine(routine);
	}

	public void OnApplicationQuit()
	{
	}
}
