using System.Collections;
using System.Collections.Generic;
using Photon;
using UnityEngine;

public class TimeOfDay : MonoBehaviour
{
	public const float SwitchSpeed = 1f;

	public static Color DayFogColor = new Color(0.8f, 0.84f, 0.96f);

	public static Color NightFogColor = new Color(0.1f, 0.1f, 0.1f);

	public static Color WaterFogColor = new Color(0f, 0.4f, 0.7f, 0.6f);

	private static TimeOfDay _Instance;

	private float _NormalizedTime;

	private float _NormalizedTimeTarget;

	private Color _FogColor = DayFogColor;

	private Material _Skybox;

	private List<GameObject> _ColoredObjects = new List<GameObject>();

	public static float NormalizedTime => _Instance._NormalizedTime;

	public static Color FogColor
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			return _Instance._FogColor;
		}
	}

	public TimeOfDay()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
	}

	static TimeOfDay()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
	}

	private void Awake()
	{
		_Instance = this;
		_Skybox = RenderSettings.skybox;
		InitializeTime(0f);
	}

	private void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
	{
		if (PhotonNetwork.isMasterClient)
		{
			photonView.RPC("InitializeTimeInternal", newPlayer, _NormalizedTimeTarget);
		}
	}

	public static void InitializeTime(float normalizedTime)
	{
		if (!((Object)(object)_Instance == (Object)null))
		{
			_Instance.InitializeTimeInternal(normalizedTime);
		}
	}

	[RPC]
	private void InitializeTimeInternal(float normalizedTime)
	{
		((MonoBehaviour)this).StopAllCoroutines();
		_NormalizedTimeTarget = normalizedTime;
		SetNormalizedTime(normalizedTime);
	}

	public static void SwitchDayNightForAll()
	{
		if (!((Object)(object)_Instance == (Object)null))
		{
			_Instance.photonView.RPC("SwitchDayNight", PhotonTargets.All);
		}
	}

	[RPC]
	private void SwitchDayNight()
	{
		((MonoBehaviour)this).StopAllCoroutines();
		_NormalizedTimeTarget = ((_NormalizedTimeTarget == 0f) ? 1 : 0);
		((MonoBehaviour)this).StartCoroutine(ChangeTimeOfDayProcess());
	}

	public static void Affect(GameObject go)
	{
		if (!((Object)(object)_Instance == (Object)null))
		{
			float num = _Instance._NormalizedTime * -0.4f;
			MaterialExt.AffectColor(go, num, num, num);
			_Instance._ColoredObjects.Add(go);
		}
	}

	private IEnumerator ChangeTimeOfDayProcess()
	{
		while (_NormalizedTime != _NormalizedTimeTarget)
		{
			SetNormalizedTime(Mathf.MoveTowards(_NormalizedTime, _NormalizedTimeTarget, 0.05f));
			yield return (object)new WaitForSeconds(0.05f);
		}
	}

	private void SetNormalizedTime(float value)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		_NormalizedTime = value;
		_Skybox.SetFloat("_Blend", _NormalizedTime);
		_FogColor = Color.Lerp(DayFogColor, NightFogColor, _NormalizedTime);
		if ((Object)(object)RenderSettings.skybox != (Object)null)
		{
			RenderSettings.fogColor = _FogColor;
		}
		if (WorldGameObject.INSTANCE.ChunkObjects != null)
		{
			ChunkGameObject[,,,] chunkObjects = WorldGameObject.INSTANCE.ChunkObjects;
			int length = chunkObjects.GetLength(0);
			int length2 = chunkObjects.GetLength(1);
			int length3 = chunkObjects.GetLength(2);
			int length4 = chunkObjects.GetLength(3);
			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < length2; j++)
				{
					for (int k = 0; k < length3; k++)
					{
						for (int l = 0; l < length4; l++)
						{
							ChunkGameObject chunkGameObject = chunkObjects[i, j, k, l];
							if ((Object)(object)chunkGameObject != (Object)null)
							{
								((Renderer)((Component)chunkGameObject).GetComponent<MeshRenderer>()).material.SetFloat("_TimeOfDay", _NormalizedTime);
							}
						}
					}
				}
			}
		}
		float num = _NormalizedTime * -0.4f;
		for (int m = 0; m < _ColoredObjects.Count; m++)
		{
			if ((Object)(object)_ColoredObjects[m] != (Object)null)
			{
				MaterialExt.AffectColor(_ColoredObjects[m], num, num, num);
			}
			else
			{
				_ColoredObjects.RemoveAt(m--);
			}
		}
	}
}
