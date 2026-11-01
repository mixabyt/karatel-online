using System;
using UnityEngine;

[Serializable]
public class CameraCullLayers : MonoBehaviour
{
	public override void Start()
	{
		float[] array = new float[32];
		array[8] = 20f;
		array[12] = 40f;
		((Component)this).camera.layerCullDistances = array;
	}

	public override void Main()
	{
	}
}
