using System.Collections;
using UnityEngine;

public class GA_Gui : MonoBehaviour
{
	public enum WindowType
	{
		None,
		MessageTypeWindow,
		FeedbackWindow,
		BugWindow
	}

	[HideInInspector]
	public bool GuiEnabled;

	[HideInInspector]
	public bool GuiAllowScreenshot;

	private WindowType _windowType;

	private bool _popUpError;

	private bool _takeScreenshot;

	private Texture2D _screenshot;

	private Rect _messageTypeWindowRect;

	private Rect _feedbackWindowRect;

	private Rect _bugWindowRect;

	private string _topic = string.Empty;

	private string _details = string.Empty;

	private Texture2D keyTexture;

	private void Start()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		_messageTypeWindowRect = new Rect((float)(Screen.width / 2 - 200), (float)(Screen.height / 2 - 75), 400f, 150f);
		if (GuiAllowScreenshot)
		{
			_feedbackWindowRect = new Rect((float)(Screen.width / 2 - 200), (float)(Screen.height / 2 - 170), 400f, 340f);
			_bugWindowRect = new Rect((float)(Screen.width / 2 - 200), (float)(Screen.height / 2 - 170), 400f, 340f);
		}
		else
		{
			_feedbackWindowRect = new Rect((float)(Screen.width / 2 - 200), (float)(Screen.height / 2 - 150), 400f, 300f);
			_bugWindowRect = new Rect((float)(Screen.width / 2 - 200), (float)(Screen.height / 2 - 150), 400f, 300f);
		}
	}

	private void OnGUI()
	{
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Expected Obj, but got Unknown
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Expected Obj, but got Unknown
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Expected Obj, but got Unknown
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		Texture2D background = GUI.skin.window.onNormal.background;
		GUI.skin.window.onNormal.background = GUI.skin.window.normal.background;
		if (GuiEnabled || _windowType != WindowType.None)
		{
			switch (_windowType)
			{
			case WindowType.None:
				if (GUI.Button(new Rect((float)(Screen.width - 55), (float)(Screen.height - 55), 50f, 50f), (Texture)(object)GA.SettingsGA.Logo))
				{
					_windowType = WindowType.MessageTypeWindow;
				}
				break;
			case WindowType.MessageTypeWindow:
				_messageTypeWindowRect = GUI.Window(0, _messageTypeWindowRect, (WindowFunction)MessageTypeWindow, string.Empty);
				break;
			case WindowType.FeedbackWindow:
				if (_takeScreenshot)
				{
					((MonoBehaviour)this).StartCoroutine(TakeScreenshot());
				}
				else
				{
					_feedbackWindowRect = GUI.Window(0, _feedbackWindowRect, (WindowFunction)FeedbackWindow, string.Empty);
				}
				break;
			case WindowType.BugWindow:
				if (_takeScreenshot)
				{
					((MonoBehaviour)this).StartCoroutine(TakeScreenshot());
				}
				else
				{
					_bugWindowRect = GUI.Window(0, _bugWindowRect, (WindowFunction)BugWindow, string.Empty);
				}
				break;
			}
		}
		GUI.skin.window.onNormal.background = background;
	}

	private void MessageTypeWindow(int windowID)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		GUI.Label(new Rect(10f, 15f, 380f, 50f), "Help improve this game by submitting feedback/bug reports");
		if (GUI.Button(new Rect(10f, 50f, 185f, 90f), "Иваникус"))
		{
			_windowType = WindowType.FeedbackWindow;
			GUI.FocusControl("TopicField");
		}
		if (GUI.Button(new Rect(205f, 50f, 185f, 90f), "Bug Report"))
		{
			_windowType = WindowType.BugWindow;
			GUI.FocusControl("TopicField");
		}
	}

	private void FeedbackWindow(int windowID)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		GUI.Label(new Rect(10f, 15f, 380f, 50f), "Submit feedback");
		GUI.Label(new Rect(10f, 50f, 380f, 20f), "Topic*");
		GUI.SetNextControlName("TopicField");
		_topic = GUI.TextField(new Rect(10f, 70f, 380f, 20f), _topic, 50);
		GUI.Label(new Rect(10f, 100f, 380f, 20f), "Details*");
		_details = GUI.TextArea(new Rect(10f, 120f, 380f, 130f), _details, 400);
		if (GuiAllowScreenshot)
		{
			num = 40;
			if (GUI.Button(new Rect(10f, 260f, 130f, 25f), "Take Screenshot"))
			{
				_takeScreenshot = true;
			}
			if ((Object)(object)_screenshot != (Object)null)
			{
				GUI.Label(new Rect(192f, 262f, 198f, 20f), "Screenshot added.");
				GUI.DrawTexture(new Rect(150f, 256f, 32f, 32f), (Texture)(object)_screenshot);
			}
			else
			{
				GUI.Label(new Rect(150f, 262f, 240f, 20f), "Screenshot not added.");
			}
		}
		if (GUI.Button(new Rect(10f, (float)(260 + num), 185f, 30f), "Cancel"))
		{
			_topic = string.Empty;
			_details = string.Empty;
			_windowType = WindowType.None;
			_screenshot = null;
		}
		if (GUI.Button(new Rect(205f, (float)(260 + num), 185f, 30f), "Submit") && _topic.Length > 0 && _details.Length > 0)
		{
			Vector3 val = Vector3.zero;
			if ((Object)(object)GA.SettingsGA.TrackTarget != (Object)null)
			{
				val = GA.SettingsGA.TrackTarget.position;
			}
			GA.API.Error.NewEvent(GA_Error.SeverityType.info, "Feedback [" + _topic + "]: " + _details, val.x, val.y, val.z);
			_topic = string.Empty;
			_details = string.Empty;
			_windowType = WindowType.None;
			_screenshot = null;
		}
	}

	private void BugWindow(int windowID)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		if (_popUpError)
		{
			GUI.Label(new Rect(10f, 10f, 385f, 50f), "Oops! It looks like an error just occurred! Please fill out this form with as many details as possible (what you were doing, etc.).");
		}
		else
		{
			GUI.Label(new Rect(10f, 15f, 380f, 50f), "Submit bug report");
		}
		GUI.Label(new Rect(10f, 50f, 380f, 20f), "Topic*");
		GUI.SetNextControlName("TopicField");
		_topic = GUI.TextField(new Rect(10f, 70f, 380f, 20f), _topic, 50);
		GUI.Label(new Rect(10f, 100f, 380f, 20f), "Details*");
		_details = GUI.TextArea(new Rect(10f, 120f, 380f, 130f), _details, 400);
		if (GuiAllowScreenshot)
		{
			num = 40;
			if (GUI.Button(new Rect(10f, 260f, 130f, 25f), "Take Screenshot"))
			{
				_takeScreenshot = true;
			}
			if ((Object)(object)_screenshot != (Object)null)
			{
				GUI.Label(new Rect(192f, 262f, 198f, 20f), "Screenshot added.");
				GUI.DrawTexture(new Rect(150f, 256f, 32f, 32f), (Texture)(object)_screenshot);
			}
			else
			{
				GUI.Label(new Rect(150f, 262f, 240f, 20f), "Screenshot not added.");
			}
		}
		if (GUI.Button(new Rect(10f, (float)(260 + num), 185f, 30f), "Cancel"))
		{
			_topic = string.Empty;
			_details = string.Empty;
			_windowType = WindowType.None;
			_popUpError = false;
			_screenshot = null;
		}
		if (GUI.Button(new Rect(205f, (float)(260 + num), 185f, 30f), "Submit") && _topic.Length > 0 && _details.Length > 0)
		{
			Vector3 trackPosition = Vector3.zero;
			if ((Object)(object)GA.SettingsGA.TrackTarget != (Object)null)
			{
				trackPosition = GA.SettingsGA.TrackTarget.position;
			}
			if (_popUpError)
			{
				GA.API.Error.NewEvent(GA_Error.SeverityType.info, "Crash Report [" + _topic + "]: " + _details, trackPosition);
			}
			else
			{
				GA.API.Error.NewEvent(GA_Error.SeverityType.info, "Bug Report [" + _topic + "]: " + _details, trackPosition);
			}
			_topic = string.Empty;
			_details = string.Empty;
			_windowType = WindowType.None;
			_popUpError = false;
			_screenshot = null;
		}
	}

	private IEnumerator TakeScreenshot()
	{
		yield return (object)new WaitForEndOfFrame();
		_screenshot = new Texture2D(Screen.width, Screen.height, (TextureFormat)3, false);
		_screenshot.ReadPixels(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), 0, 0);
		_screenshot.Apply();
		_takeScreenshot = false;
	}

	public void OpenBugGUI()
	{
		_windowType = WindowType.BugWindow;
		_popUpError = true;
		if (GuiAllowScreenshot)
		{
			_takeScreenshot = true;
		}
	}
}
