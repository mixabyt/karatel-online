using System;
using System.Collections;
using UnityEngine;

public class GA
{
	public class GA_API
	{
		public GA_Error Error = new GA_Error();

		public GA_Design Design = new GA_Design();

		public GA_Business Business = new GA_Business();

		public GA_GenericInfo GenericInfo = new GA_GenericInfo();

		public GA_Debug Debugging = new GA_Debug();

		public GA_Archive Archive = new GA_Archive();

		public GA_Request Request = new GA_Request();

		public GA_Submit Submit = new GA_Submit();

		public GA_User User = new GA_User();
	}

	public static GA_GameObjectManager _GA_controller;

	private static GA_Settings _settings;

	private static GA_API api = new GA_API();

	public static GA_Settings SettingsGA
	{
		get
		{
			if ((Object)(object)_settings == (Object)null)
			{
				InitAPI();
			}
			return _settings;
		}
		private set
		{
			_settings = value;
		}
	}

	public static GA_GameObjectManager GA_controller
	{
		get
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected Obj, but got Unknown
			if ((Object)(object)_GA_controller == (Object)null)
			{
				GameObject val = new GameObject("GA_Controller");
				_GA_controller = val.AddComponent<GA_GameObjectManager>();
			}
			return _GA_controller;
		}
		private set
		{
			_GA_controller = value;
		}
	}

	public static GA_API API
	{
		get
		{
			if ((Object)(object)SettingsGA == (Object)null)
			{
				InitAPI();
			}
			return api;
		}
		private set
		{
		}
	}

	private static void InitAPI()
	{
		try
		{
			_settings = (GA_Settings)(object)Resources.Load("GameAnalytics/GA_Settings", typeof(GA_Settings));
			InitializeQueue();
		}
		catch (Exception ex)
		{
			Debug.Log((object)("Error getting GA_Settings in InitAPI: " + ex.Message));
		}
	}

	private static void InitializeQueue()
	{
		SettingsGA.SetKeys(SettingsGA.GameKey, SettingsGA.SecretKey);
		if (Application.isPlaying)
		{
			if (API.GenericInfo.UserID == string.Empty && !SettingsGA.CustomUserID)
			{
				Debug.LogWarning((object)"GA UserID not set. No data will be sent.");
			}
			else
			{
				RunCoroutine(SettingsGA.CheckInternetConnectivity(startQueue: true));
			}
		}
	}

	public static void RunCoroutine(IEnumerator routine)
	{
		RunCoroutine(routine, () => true);
	}

	public static void RunCoroutine(IEnumerator routine, Func<bool> done)
	{
		if (Application.isPlaying || !Application.isEditor)
		{
			GA_controller.RunCoroutine(routine);
		}
	}

	public static void Log(object msg, bool addEvent)
	{
		if (SettingsGA.DebugMode || (addEvent && SettingsGA.DebugAddEvent))
		{
			Debug.Log(msg);
		}
	}

	public static void Log(object msg)
	{
		if (SettingsGA.DebugMode)
		{
			Debug.Log(msg);
		}
	}

	public static void LogWarning(object msg)
	{
		Debug.LogWarning(msg);
	}

	public static void LogError(object msg)
	{
		Debug.LogError(msg);
	}
}
