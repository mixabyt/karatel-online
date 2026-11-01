using UnityEngine;

namespace hd;

public static class Debug
{
	public static bool EnableLog;

	public static bool EnableLogWarning = true;

	public static bool EnableLogError = true;

	public static void Log(object msg)
	{
		if (EnableLog)
		{
			Debug.Log(msg);
		}
	}

	public static void LogWarning(object msg)
	{
		if (EnableLogWarning)
		{
			Debug.LogWarning(msg);
		}
	}

	public static void LogError(object msg)
	{
		if (EnableLogError)
		{
			Debug.LogError(msg);
		}
	}
}
