using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Photon;
using UnityEngine;

public class bs : MonoBehaviour
{
	public static float immportalTime = 3f;

	public static StringBuilder stringBuilder = new StringBuilder();

	public static int rndhack = new Random().Next(50, 1000);

	private Dictionary<string, int> hackDictInt = new Dictionary<string, int>();

	private Dictionary<string, float> hackDictfloat = new Dictionary<string, float>();

	public static Igor _Igor => Igor.Instance;

	public static WorldGameObject _WorldGameObject => WorldGameObject.Instance;

	public static MainMenu _MainMenu => MainMenu.Instance;

	public static World _World => World.Instance;

	public Vector3 pos
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return ((Component)this).transform.position;
		}
	}

	public Quaternion rot
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return ((Component)this).transform.rotation;
		}
	}

	public static IEnumerator AddUpdate(Func<bool> y)
	{
		yield return null;
		while (y())
		{
			yield return null;
		}
	}

	public static bool DebugKey(KeyCode key)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (_Igor.debug && Input.GetKey((KeyCode)304) && Input.GetKeyDown(key))
		{
			return true;
		}
		return false;
	}

	public static IEnumerator AddMethod(Func<bool> y, Action a)
	{
		yield return null;
		while (!y())
		{
			yield return null;
		}
		a();
	}

	public static IEnumerator AddMethod(float sec, Action a)
	{
		yield return (object)new WaitForSeconds(sec);
		a();
	}

	public static IEnumerator AddMethod(Action a)
	{
		yield return null;
		a();
	}

	public static IEnumerable<Transform> GetTransforms(Transform ts)
	{
		yield return ts;
		foreach (Transform t2 in ts)
		{
			Transform t = t2;
			foreach (Transform transform in GetTransforms(t))
			{
				yield return transform;
			}
		}
	}

	public static T Log<T>(T s)
	{
		stringBuilder.AppendLine(s.ToString());
		return s;
	}

	public Vector3 ZeroY(Vector3 v)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		v.y = 0f;
		return v;
	}

	public static void Log(params object[] s)
	{
		foreach (object obj in s)
		{
			stringBuilder.Append(string.Concat(obj, "\t"));
		}
		stringBuilder.AppendLine();
	}

	public void PlayOneShot(AudioClip audioClip)
	{
		((Component)this).audio.PlayOneShot(audioClip, ProfileINI.sound_volume * ProfileINI.sound_scale);
	}

	public int GetInt(string s, int def = 0)
	{
		if (!hackDictInt.TryGetValue(s, out var value))
		{
			return def;
		}
		return value - rndhack;
	}

	public void SetInt(string s, int value)
	{
		hackDictInt[s] = value + rndhack;
	}

	public float Getfloat(string s, float def = 0f)
	{
		if (!hackDictfloat.TryGetValue(s, out var value))
		{
			return def;
		}
		return value - (float)rndhack;
	}

	public void Setfloat(string s, float value)
	{
		hackDictfloat[s] = value + (float)rndhack;
	}
}
