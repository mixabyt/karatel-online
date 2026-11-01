using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GA_Debug
{
	public bool SubmitErrors;

	public int MaxErrorCount;

	public bool SubmitErrorStackTrace;

	public bool SubmitErrorSystemInfo;

	private int _errorCount;

	private static bool _showLogOnGUI;

	public List<string> Messages;

	public void HandleLog(string logString, string stackTrace, LogType type)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Invalid comparison between Unknown and I4
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		if (_showLogOnGUI)
		{
			if (Messages == null)
			{
				Messages = new List<string>();
			}
			Messages.Add(logString);
		}
		if (!SubmitErrors || _errorCount >= MaxErrorCount || (int)type == 3)
		{
			return;
		}
		_errorCount++;
		string text = logString.Replace('"', '\'').Replace('\n', ' ').Replace('\r', ' ');
		string text2 = stackTrace.Replace('"', '\'').Replace('\n', ' ').Replace('\r', ' ');
		string eventName = "Exception";
		if (SubmitErrorStackTrace)
		{
			SubmitError(eventName, text + " " + text2, type);
		}
		else
		{
			SubmitError(eventName, null, type);
		}
		if (!SubmitErrorSystemInfo)
		{
			return;
		}
		List<Hashtable> genericInfo = GA.API.GenericInfo.GetGenericInfo(text);
		foreach (Hashtable item in genericInfo)
		{
			GA_Queue.AddItem(item, GA_Submit.CategoryType.GA_Error, stack: false);
		}
	}

	public void SubmitError(string eventName, string message, LogType type)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected I4, but got Unknown
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3.zero;
		if ((Object)(object)GA.SettingsGA.TrackTarget != (Object)null)
		{
			val = GA.SettingsGA.TrackTarget.position;
		}
		GA_Error.SeverityType severity = GA_Error.SeverityType.info;
		switch ((int)type)
		{
		case 1:
			severity = GA_Error.SeverityType.info;
			break;
		case 0:
			severity = GA_Error.SeverityType.error;
			break;
		case 4:
			severity = GA_Error.SeverityType.critical;
			break;
		case 3:
			severity = GA_Error.SeverityType.debug;
			break;
		case 2:
			severity = GA_Error.SeverityType.warning;
			break;
		}
		GA.API.Error.NewErrorEvent(severity, message, val.x, val.y, val.z);
	}

	public static void EnabledLog()
	{
		_showLogOnGUI = true;
	}
}
