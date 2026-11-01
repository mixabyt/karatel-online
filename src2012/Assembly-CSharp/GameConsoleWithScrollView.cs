using System.Text;
using UnityEngine;

public class GameConsoleWithScrollView : MonoBehaviour
{
	public bool show_output = true;

	public bool show_stack;

	public static bool show;

	public static GameConsoleWithScrollView I;

	private int error_count;

	private StringBuilder strb = new StringBuilder();

	private Rect pos_rect = new Rect(50f, 75f, 400f, 400f);

	private Rect view_rect = new Rect(0f, 0f, 400f, 30000f);

	private Vector2 scroll_pos;

	public GameConsoleWithScrollView()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
	}

	private void Awake()
	{
		I = this;
		strb.AppendLine("SYSTEM INFO:");
		strb.AppendLine("deviceModel " + SystemInfo.deviceModel);
		strb.AppendLine("deviceName " + SystemInfo.deviceName);
		strb.AppendLine("graphicsDeviceID " + SystemInfo.graphicsDeviceID);
		strb.AppendLine("graphicsDeviceName " + SystemInfo.graphicsDeviceName);
		strb.AppendLine("graphicsDeviceVendor " + SystemInfo.graphicsDeviceVendor);
		strb.AppendLine("graphicsDeviceVersion " + SystemInfo.graphicsDeviceVersion);
		strb.AppendLine("graphicsShaderLevel " + SystemInfo.graphicsShaderLevel);
		strb.AppendLine("operatingSystem " + SystemInfo.operatingSystem);
		strb.AppendLine("processorCount " + SystemInfo.processorCount);
		strb.AppendLine("processorType " + SystemInfo.processorType);
		strb.AppendLine("systemMemorySize " + SystemInfo.systemMemorySize);
		strb.AppendLine(string.Empty);
		strb.AppendLine("CONSOLE:");
	}

	private void OnEnable()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected Obj, but got Unknown
		Application.RegisterLogCallback((LogCallback)HandleLog);
	}

	private void OnDisable()
	{
		Application.RegisterLogCallback((LogCallback)null);
	}

	private void HandleLog(string logString, string stackTrace, LogType type)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Invalid comparison between Unknown and I4
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		if ((int)type == 4 || (int)type == 0)
		{
			error_count++;
		}
		if (show_output || show_stack)
		{
			strb.Append("\n");
			if (show_output)
			{
				strb.AppendLine(logString);
			}
			if (show_stack || (int)type == 4 || (int)type == 0)
			{
				strb.AppendLine(stackTrace);
			}
		}
	}

	public void OnGUI()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (show)
		{
			scroll_pos = GUI.BeginScrollView(pos_rect, scroll_pos, view_rect);
			GUI.TextArea(new Rect(0f, 0f, view_rect.width - 50f, view_rect.height), strb.ToString());
			GUI.EndScrollView();
		}
	}
}
