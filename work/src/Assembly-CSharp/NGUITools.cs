using System;
using System.Collections.Generic;
using UnityEngine;

public static class NGUITools
{
	private static AudioListener mListener;

	private static bool mLoaded = false;

	private static float mGlobalVolume = 1f;

	private static Color mInvisible = new Color(0f, 0f, 0f, 0f);

	public static float soundVolume
	{
		get
		{
			if (!mLoaded)
			{
				mLoaded = true;
				mGlobalVolume = PlayerPrefs.GetFloat("Sound", 1f);
			}
			return mGlobalVolume;
		}
		set
		{
			if (mGlobalVolume != value)
			{
				mLoaded = true;
				mGlobalVolume = value;
				PlayerPrefs.SetFloat("Sound", value);
			}
		}
	}

	public static bool fileAccess
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Invalid comparison between Unknown and I4
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Invalid comparison between Unknown and I4
			return (int)Application.platform != 5 && (int)Application.platform != 3;
		}
	}

	public static string clipboard
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected Obj, but got Unknown
			TextEditor val = new TextEditor();
			val.Paste();
			val.SelectAll();
			return val.SelectedText;
		}
		set
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected Obj, but got Unknown
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Expected Obj, but got Unknown
			TextEditor val = new TextEditor();
			val.content = new GUIContent(value);
			val.SelectAll();
			val.Copy();
		}
	}

	static NGUITools()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
	}

	public static AudioSource PlaySound(AudioClip clip)
	{
		return PlaySound(clip, 1f, 1f);
	}

	public static AudioSource PlaySound(AudioClip clip, float volume)
	{
		return PlaySound(clip, volume, 1f);
	}

	public static AudioSource PlaySound(AudioClip clip, float volume, float pitch)
	{
		volume *= soundVolume;
		if ((Object)(object)clip != (Object)null && volume > 0.01f)
		{
			if ((Object)(object)mListener == (Object)null)
			{
				Object val = Object.FindObjectOfType(typeof(AudioListener));
				mListener = (AudioListener)(object)((val is AudioListener) ? val : null);
				if ((Object)(object)mListener == (Object)null)
				{
					Camera val2 = Camera.main;
					if ((Object)(object)val2 == (Object)null)
					{
						Object val3 = Object.FindObjectOfType(typeof(Camera));
						val2 = (Camera)(object)((val3 is Camera) ? val3 : null);
					}
					if ((Object)(object)val2 != (Object)null)
					{
						mListener = ((Component)val2).gameObject.AddComponent<AudioListener>();
					}
				}
			}
			if ((Object)(object)mListener != (Object)null && ((Behaviour)mListener).enabled && GetActive(((Component)mListener).gameObject))
			{
				AudioSource val4 = ((Component)mListener).audio;
				if ((Object)(object)val4 == (Object)null)
				{
					val4 = ((Component)mListener).gameObject.AddComponent<AudioSource>();
				}
				val4.pitch = pitch;
				val4.PlayOneShot(clip, volume);
				return val4;
			}
		}
		return null;
	}

	public static WWW OpenURL(string url)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Expected Obj, but got Unknown
		WWW result = null;
		try
		{
			result = new WWW(url);
		}
		catch (Exception ex)
		{
			Debug.LogError((object)ex.Message);
		}
		return result;
	}

	public static WWW OpenURL(string url, WWWForm form)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected Obj, but got Unknown
		if (form == null)
		{
			return OpenURL(url);
		}
		WWW result = null;
		try
		{
			result = new WWW(url, form);
		}
		catch (Exception ex)
		{
			Debug.LogError((object)((ex == null) ? "<null>" : ex.Message));
		}
		return result;
	}

	public static int RandomRange(int min, int max)
	{
		if (min == max)
		{
			return min;
		}
		return Random.Range(min, max + 1);
	}

	public static string GetHierarchy(GameObject obj)
	{
		string text = ((Object)obj).name;
		while ((Object)(object)obj.transform.parent != (Object)null)
		{
			obj = ((Component)obj.transform.parent).gameObject;
			text = ((Object)obj).name + "/" + text;
		}
		return "\"" + text + "\"";
	}

	public static Color ParseColor(string text, int offset)
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		int num = (NGUIMath.HexToDecimal(text[offset]) << 4) | NGUIMath.HexToDecimal(text[offset + 1]);
		int num2 = (NGUIMath.HexToDecimal(text[offset + 2]) << 4) | NGUIMath.HexToDecimal(text[offset + 3]);
		int num3 = (NGUIMath.HexToDecimal(text[offset + 4]) << 4) | NGUIMath.HexToDecimal(text[offset + 5]);
		float num4 = 1f / 255f;
		return new Color(num4 * (float)num, num4 * (float)num2, num4 * (float)num3);
	}

	public static string EncodeColor(Color c)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		int num = 0xFFFFFF & (NGUIMath.ColorToInt(c) >> 8);
		return NGUIMath.DecimalToHex(num);
	}

	public static int ParseSymbol(string text, int index, List<Color> colors, bool premultiply)
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		int length = text.Length;
		if (index + 2 < length)
		{
			if (text[index + 1] == '-')
			{
				if (text[index + 2] == ']')
				{
					if (colors != null && colors.Count > 1)
					{
						colors.RemoveAt(colors.Count - 1);
					}
					return 3;
				}
			}
			else if (index + 7 < length && text[index + 7] == ']')
			{
				if (colors != null)
				{
					Color val = ParseColor(text, index + 1);
					if (EncodeColor(val) != text.Substring(index + 1, 6).ToUpper())
					{
						return 0;
					}
					val.a = colors[colors.Count - 1].a;
					if (premultiply && val.a != 1f)
					{
						val = Color.Lerp(mInvisible, val, val.a);
					}
					colors.Add(val);
				}
				return 8;
			}
		}
		return 0;
	}

	public static string StripSymbols(string text)
	{
		if (text != null)
		{
			int num = 0;
			int length = text.Length;
			while (num < length)
			{
				char c = text[num];
				if (c == '[')
				{
					int num2 = ParseSymbol(text, num, null, premultiply: false);
					if (num2 > 0)
					{
						text = text.Remove(num, num2);
						length = text.Length;
						continue;
					}
				}
				num++;
			}
		}
		return text;
	}

	public static T[] FindActive<T>() where T : Component
	{
		return Object.FindObjectsOfType(typeof(T)) as T[];
	}

	public static Camera FindCameraForLayer(int layer)
	{
		int num = 1 << layer;
		Camera[] array = NGUITools.FindActive<Camera>();
		int i = 0;
		for (int num2 = array.Length; i < num2; i++)
		{
			Camera val = array[i];
			if ((val.cullingMask & num) != 0)
			{
				return val;
			}
		}
		return null;
	}

	public static BoxCollider AddWidgetCollider(GameObject go)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)go != (Object)null)
		{
			Collider component = go.GetComponent<Collider>();
			BoxCollider val = (BoxCollider)(object)((component is BoxCollider) ? component : null);
			if ((Object)(object)val == (Object)null)
			{
				if ((Object)(object)component != (Object)null)
				{
					if (Application.isPlaying)
					{
						Object.Destroy((Object)(object)component);
					}
					else
					{
						Object.DestroyImmediate((Object)(object)component);
					}
				}
				val = go.AddComponent<BoxCollider>();
			}
			int num = CalculateNextDepth(go);
			Bounds val2 = NGUIMath.CalculateRelativeWidgetBounds(go.transform);
			((Collider)val).isTrigger = true;
			val.center = val2.center + Vector3.back * ((float)num * 0.25f);
			val.size = new Vector3(val2.size.x, val2.size.y, 0f);
			return val;
		}
		return null;
	}

	public static string GetName<T>() where T : Component
	{
		string text = typeof(T).ToString();
		if (text.StartsWith("UI"))
		{
			text = text.Substring(2);
		}
		else if (text.StartsWith("UnityEngine."))
		{
			text = text.Substring(12);
		}
		return text;
	}

	public static GameObject AddChild(GameObject parent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected Obj, but got Unknown
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject();
		if ((Object)(object)parent != (Object)null)
		{
			Transform transform = val.transform;
			transform.parent = parent.transform;
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.localScale = Vector3.one;
			val.layer = parent.layer;
		}
		return val;
	}

	public static GameObject AddChild(GameObject parent, GameObject prefab)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		Object val = Object.Instantiate((Object)(object)prefab);
		GameObject val2 = (GameObject)(object)((val is GameObject) ? val : null);
		if ((Object)(object)val2 != (Object)null && (Object)(object)parent != (Object)null)
		{
			Transform transform = val2.transform;
			transform.parent = parent.transform;
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.localScale = Vector3.one;
			val2.layer = parent.layer;
		}
		return val2;
	}

	public static int CalculateNextDepth(GameObject go)
	{
		int num = -1;
		UIWidget[] componentsInChildren = go.GetComponentsInChildren<UIWidget>();
		int i = 0;
		for (int num2 = componentsInChildren.Length; i < num2; i++)
		{
			num = Mathf.Max(num, componentsInChildren[i].depth);
		}
		return num + 1;
	}

	public static T AddChild<T>(GameObject parent) where T : Component
	{
		GameObject val = AddChild(parent);
		((Object)val).name = GetName<T>();
		return val.AddComponent<T>();
	}

	public static T AddWidget<T>(GameObject go) where T : UIWidget
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		int depth = CalculateNextDepth(go);
		T result = NGUITools.AddChild<T>(go);
		result.depth = depth;
		Transform transform = ((Component)result).transform;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		transform.localScale = new Vector3(100f, 100f, 1f);
		((Component)result).gameObject.layer = go.layer;
		return result;
	}

	public static UISprite AddSprite(GameObject go, UIAtlas atlas, string spriteName)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		UIAtlas.Sprite sprite = ((!((Object)(object)atlas != (Object)null)) ? null : atlas.GetSprite(spriteName));
		UISprite uISprite = AddWidget<UISprite>(go);
		uISprite.type = ((sprite != null && !(sprite.inner == sprite.outer)) ? UISprite.Type.Sliced : UISprite.Type.Simple);
		uISprite.atlas = atlas;
		uISprite.spriteName = spriteName;
		return uISprite;
	}

	public static GameObject GetRoot(GameObject go)
	{
		Transform val = go.transform;
		while (true)
		{
			Transform parent = val.parent;
			if ((Object)(object)parent == (Object)null)
			{
				break;
			}
			val = parent;
		}
		return ((Component)val).gameObject;
	}

	public static T FindInParents<T>(GameObject go) where T : Component
	{
		if ((Object)(object)go == (Object)null)
		{
			return (T)(object)null;
		}
		object obj = go.GetComponent<T>();
		if (obj == null)
		{
			Transform parent = go.transform.parent;
			while ((Object)(object)parent != (Object)null && obj == null)
			{
				obj = ((Component)parent).gameObject.GetComponent<T>();
				parent = parent.parent;
			}
		}
		return (T)obj;
	}

	public static void Destroy(Object obj)
	{
		if (!(obj != (Object)null))
		{
			return;
		}
		if (Application.isPlaying)
		{
			if (obj is GameObject)
			{
				GameObject val = (GameObject)(object)((obj is GameObject) ? obj : null);
				val.transform.parent = null;
			}
			Object.Destroy(obj);
		}
		else
		{
			Object.DestroyImmediate(obj);
		}
	}

	public static void DestroyImmediate(Object obj)
	{
		if (obj != (Object)null)
		{
			if (Application.isEditor)
			{
				Object.DestroyImmediate(obj);
			}
			else
			{
				Object.Destroy(obj);
			}
		}
	}

	public static void Broadcast(string funcName)
	{
		GameObject[] array = Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];
		int i = 0;
		for (int num = array.Length; i < num; i++)
		{
			array[i].SendMessage(funcName, (SendMessageOptions)1);
		}
	}

	public static void Broadcast(string funcName, object param)
	{
		GameObject[] array = Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];
		int i = 0;
		for (int num = array.Length; i < num; i++)
		{
			array[i].SendMessage(funcName, param, (SendMessageOptions)1);
		}
	}

	public static bool IsChild(Transform parent, Transform child)
	{
		if ((Object)(object)parent == (Object)null || (Object)(object)child == (Object)null)
		{
			return false;
		}
		while ((Object)(object)child != (Object)null)
		{
			if ((Object)(object)child == (Object)(object)parent)
			{
				return true;
			}
			child = child.parent;
		}
		return false;
	}

	private static void Activate(Transform t)
	{
		SetActiveSelf(((Component)t).gameObject, state: true);
		int i = 0;
		for (int childCount = t.childCount; i < childCount; i++)
		{
			Transform child = t.GetChild(i);
			if (((Component)child).gameObject.activeSelf)
			{
				return;
			}
		}
		int j = 0;
		for (int childCount2 = t.childCount; j < childCount2; j++)
		{
			Transform child2 = t.GetChild(j);
			Activate(child2);
		}
	}

	private static void Deactivate(Transform t)
	{
		SetActiveSelf(((Component)t).gameObject, state: false);
	}

	public static void SetActive(GameObject go, bool state)
	{
		if (state)
		{
			Activate(go.transform);
		}
		else
		{
			Deactivate(go.transform);
		}
	}

	public static void SetActiveChildren(GameObject go, bool state)
	{
		Transform transform = go.transform;
		if (state)
		{
			int i = 0;
			for (int childCount = transform.childCount; i < childCount; i++)
			{
				Transform child = transform.GetChild(i);
				Activate(child);
			}
		}
		else
		{
			int j = 0;
			for (int childCount2 = transform.childCount; j < childCount2; j++)
			{
				Transform child2 = transform.GetChild(j);
				Deactivate(child2);
			}
		}
	}

	public static bool GetActive(GameObject go)
	{
		return Object.op_Implicit((Object)(object)go) && go.activeInHierarchy;
	}

	public static void SetActiveSelf(GameObject go, bool state)
	{
		go.SetActive(state);
	}

	public static void SetLayer(GameObject go, int layer)
	{
		go.layer = layer;
		Transform transform = go.transform;
		int i = 0;
		for (int childCount = transform.childCount; i < childCount; i++)
		{
			Transform child = transform.GetChild(i);
			SetLayer(((Component)child).gameObject, layer);
		}
	}

	public static Vector3 Round(Vector3 v)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		v.x = Mathf.Round(v.x);
		v.y = Mathf.Round(v.y);
		v.z = Mathf.Round(v.z);
		return v;
	}

	public static void MakePixelPerfect(Transform t)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		UIWidget component = ((Component)t).GetComponent<UIWidget>();
		if ((Object)(object)component != (Object)null)
		{
			component.MakePixelPerfect();
			return;
		}
		t.localPosition = Round(t.localPosition);
		t.localScale = Round(t.localScale);
		int i = 0;
		for (int childCount = t.childCount; i < childCount; i++)
		{
			MakePixelPerfect(t.GetChild(i));
		}
	}

	public static bool Save(string fileName, byte[] bytes)
	{
		return false;
	}

	public static byte[] Load(string fileName)
	{
		return null;
	}

	public static Color ApplyPMA(Color c)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (c.a != 1f)
		{
			c.r *= c.a;
			c.g *= c.a;
			c.b *= c.a;
		}
		return c;
	}

	public static void MarkParentAsChanged(GameObject go)
	{
		UIWidget[] componentsInChildren = go.GetComponentsInChildren<UIWidget>();
		int i = 0;
		for (int num = componentsInChildren.Length; i < num; i++)
		{
			componentsInChildren[i].ParentHasChanged();
		}
	}
}
