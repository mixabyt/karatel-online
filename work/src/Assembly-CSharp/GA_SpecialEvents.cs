using System.Collections;
using UnityEngine;

public class GA_SpecialEvents : MonoBehaviour
{
	private float _lastLevelStartTime;

	private static int _frameCountAvg;

	private static float _lastUpdateAvg;

	private int _frameCountCrit;

	private float _lastUpdateCrit;

	public void Start()
	{
		SceneChange();
		((MonoBehaviour)this).StartCoroutine(SubmitFPSRoutine());
	}

	private IEnumerator SubmitFPSRoutine()
	{
		while (Application.isPlaying && (Object)(object)GA_SystemTracker.GA_SYSTEMTRACKER != (Object)null && GA_SystemTracker.GA_SYSTEMTRACKER.SubmitFpsCritical)
		{
			SubmitCriticalFPS();
			yield return (object)new WaitForSeconds((float)GA_SystemTracker.GA_SYSTEMTRACKER.FpsCirticalSubmitInterval);
		}
	}

	public void Update()
	{
		if (GA_SystemTracker.GA_SYSTEMTRACKER.SubmitFpsAverage)
		{
			_frameCountAvg++;
		}
		if (GA_SystemTracker.GA_SYSTEMTRACKER.SubmitFpsCritical)
		{
			_frameCountCrit++;
		}
	}

	public void OnLevelWasLoaded()
	{
		SceneChange();
	}

	public void OnApplicationQuit()
	{
	}

	public static void SubmitAverageFPS()
	{
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)GA_SystemTracker.GA_SYSTEMTRACKER != (Object)null) || !GA_SystemTracker.GA_SYSTEMTRACKER.SubmitFpsAverage)
		{
			return;
		}
		float num = Time.time - _lastUpdateAvg;
		if (!(num > 1f))
		{
			return;
		}
		float num2 = (float)_frameCountAvg / num;
		_lastUpdateAvg = Time.time;
		_frameCountAvg = 0;
		if (num2 > 0f)
		{
			if ((Object)(object)GA.SettingsGA.TrackTarget != (Object)null)
			{
				GA.API.Design.NewEvent("GA:AverageFPS", (int)num2, GA.SettingsGA.TrackTarget.position);
			}
			else
			{
				GA.API.Design.NewEvent("GA:AverageFPS", (float)(int)num2);
			}
		}
	}

	public void SubmitCriticalFPS()
	{
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)GA_SystemTracker.GA_SYSTEMTRACKER != (Object)null) || !GA_SystemTracker.GA_SYSTEMTRACKER.SubmitFpsCritical)
		{
			return;
		}
		float num = Time.time - _lastUpdateCrit;
		if (!(num >= 1f))
		{
			return;
		}
		float num2 = (float)_frameCountCrit / num;
		_lastUpdateCrit = Time.time;
		_frameCountCrit = 0;
		if (num2 <= (float)GA_SystemTracker.GA_SYSTEMTRACKER.FpsCriticalThreshold)
		{
			if ((Object)(object)GA.SettingsGA.TrackTarget != (Object)null)
			{
				GA.API.Design.NewEvent("GA:CriticalFPS", (int)num2, GA.SettingsGA.TrackTarget.position);
			}
			else
			{
				GA.API.Design.NewEvent("GA:CriticalFPS", (float)(int)num2);
			}
		}
	}

	private void SceneChange()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (GA_SystemTracker.GA_SYSTEMTRACKER.IncludeSceneChange)
		{
			if ((Object)(object)GA.SettingsGA.TrackTarget != (Object)null)
			{
				GA.API.Design.NewEvent("GA:LevelStarted", Time.time - _lastLevelStartTime, GA.SettingsGA.TrackTarget.position);
			}
			else
			{
				GA.API.Design.NewEvent("GA:LevelStarted", Time.time - _lastLevelStartTime);
			}
		}
		_lastLevelStartTime = Time.time;
	}
}
