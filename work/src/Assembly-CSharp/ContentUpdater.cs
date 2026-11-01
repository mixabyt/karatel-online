using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentUpdater : MonoBehaviour
{
	public static List<Texture2D> WorldTextures = new List<Texture2D>();

	public static List<Texture2D> CharacterTextures = new List<Texture2D>();

	public static List<Texture2D> WeaponTextures = new List<Texture2D>();

	public static List<Texture2D> EntitiesTextures = new List<Texture2D>();

	public void Start()
	{
		LoadContent("WorldTextures2", 1, WorldTextures, () =>
		{
			if ((Object)(object)WorldGameObject.Instance != (Object)null && (Object)(object)WorldGameObject.Instance.WorldTextureAtlas != (Object)null)
			{
				WorldGameObject.Instance.InitializeTextures();
			}
		});
		LoadContent("CharacterTextures3", 1, CharacterTextures, () =>
		{
			Object[] array = Object.FindObjectsOfType(typeof(SkinManager));
			for (int i = 0; i < array.Length; i++)
			{
				SkinManager skinManager = (SkinManager)(object)array[i];
				skinManager.UpdateTextures();
			}
		});
		LoadContent("WeaponTextures", 2, WeaponTextures, () =>
		{
			Object[] array = Object.FindObjectsOfType(typeof(SkinManager));
			for (int i = 0; i < array.Length; i++)
			{
				SkinManager skinManager = (SkinManager)(object)array[i];
				skinManager.UpdateTextures();
			}
		});
		LoadContent("EntitiesTextures3", 1, EntitiesTextures, () =>
		{
			Object[] array = Object.FindObjectsOfType(typeof(EntityBase));
			for (int i = 0; i < array.Length; i++)
			{
				EntityBase entityBase = (EntityBase)(object)array[i];
				entityBase.UpdateTextures();
			}
		});
	}

	private void LoadContent<T>(string packName, int version, List<T> resultArray, Action loadCallback)
	{
		((MonoBehaviour)this).StartCoroutine(LoadContentProcess(packName, version, resultArray, loadCallback));
	}

	private IEnumerator LoadContentProcess<T>(string packName, int version, List<T> resultArray, Action loadCallback)
	{
		WWW request = WWW.LoadFromCacheOrDownload(App.Instance.GetContentUrl() + packName + ".unity3d", version);
		yield return request;
		if (request.error != null)
		{
			Debug.Log((object)("Load asset bundle fail: " + request.error));
			yield return (object)new WaitForSeconds(30f);
			((MonoBehaviour)this).StartCoroutine(LoadContentProcess(packName, version, resultArray, loadCallback));
			yield break;
		}
		if ((Object)(object)request.assetBundle == (Object)null)
		{
			Debug.Log((object)"Loaded file not contains asset bundle!");
			yield break;
		}
		Object[] assets = request.assetBundle.LoadAll();
		resultArray.Clear();
		for (int i = 0; i < assets.Length; i++)
		{
			resultArray.Add((T)(object)assets[i]);
		}
		loadCallback();
	}
}
