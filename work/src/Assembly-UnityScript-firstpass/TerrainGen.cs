using System;
using UnityEngine;

[Serializable]
public class TerrainGen : MonoBehaviour
{
	public GameObject prefab;

	public override void Start()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		for (int i = -10; i < 10; i++)
		{
			for (int j = -10; j < 10; j++)
			{
				for (int k = -10; k <= 0; k++)
				{
					Object.Instantiate((Object)(object)prefab, new Vector3((float)j, (float)k, (float)i), Quaternion.identity);
				}
			}
		}
	}

	public override void Main()
	{
	}
}
