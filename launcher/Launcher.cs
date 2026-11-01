// KopatelOnline2014.exe: asks for the server address, unpacks the bundled Firefox 51 and Unity Web
// Player on first start, and opens the server's game page in that Firefox.
// Built by build.ps1 with the .NET Framework csc (C# 5), so no SDK is needed.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

[assembly: AssemblyTitle("Копатель Онлайн 2014")]
[assembly: AssemblyProduct("Копатель Онлайн 2014")]

internal static class Launcher
{
	private static readonly string Home = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "KopatelOnline2014");
	private static readonly string FirefoxDir = Path.Combine(Home, "firefox");
	private static readonly string ProfileDir = Path.Combine(Home, "profile");
	private static readonly string ServerFile = Path.Combine(Home, "server.txt");

	[STAThread]
	private static void Main()
	{
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);
		Application.Run(new LauncherForm());
	}

	public static string Resource(string name)
	{
		using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name))
		using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
		{
			return reader.ReadToEnd().Trim();
		}
	}

	public static string LoadServer()
	{
		try
		{
			if (File.Exists(ServerFile))
			{
				return File.ReadAllText(ServerFile).Trim();
			}
		}
		catch (IOException)
		{
		}
		return Resource("default_server.txt");
	}

	// "1.2.3.4" -> "http://1.2.3.4:8080/", "host:9000" -> "http://host:9000/"; null if unusable.
	public static string GameUrl(string address)
	{
		address = address.Trim();
		if (address.Length == 0)
		{
			return null;
		}
		if (!address.Contains("://"))
		{
			string host = address.Split('/')[0];
			bool hasPort = host.StartsWith("[") ? host.Contains("]:") : host.Contains(":");
			address = "http://" + (hasPort ? address : host + ":8080" + address.Substring(host.Length));
		}
		Uri uri;
		if (!Uri.TryCreate(address, UriKind.Absolute, out uri) || (uri.Scheme != "http" && uri.Scheme != "https"))
		{
			return null;
		}
		return uri.AbsoluteUri;
	}

	public static void Prepare(string address, Action<string, int> progress)
	{
		Directory.CreateDirectory(Home);
		File.WriteAllText(ServerFile, address);
		string payloadId = Resource("payload_id.txt");
		string payloadMark = Path.Combine(FirefoxDir, "kop-payload.txt");
		string playerDir = WebPlayerDir();
		bool needFirefox = !File.Exists(payloadMark) || File.ReadAllText(payloadMark).Trim() != payloadId;
		bool needPlayer = !File.Exists(Path.Combine(playerDir, "loader", "npUnity3D32.dll"));
		if (needFirefox || needPlayer)
		{
			if (needFirefox && Directory.Exists(FirefoxDir))
			{
				Directory.Delete(FirefoxDir, true);
			}
			using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("payload.zip"))
			using (ZipArchive zip = new ZipArchive(stream, ZipArchiveMode.Read))
			{
				List<ZipArchiveEntry> entries = zip.Entries.Where(e =>
					(needFirefox && e.FullName.StartsWith("firefox/")) || (needPlayer && e.FullName.StartsWith("webplayer/"))).ToList();
				for (int i = 0; i < entries.Count; i++)
				{
					ZipArchiveEntry entry = entries[i];
					if (entry.Name.Length == 0)
					{
						continue;
					}
					string rel = entry.FullName.Substring(entry.FullName.IndexOf('/') + 1).Replace('/', '\\');
					string dst = Path.Combine(entry.FullName.StartsWith("firefox/") ? FirefoxDir : playerDir, rel);
					Directory.CreateDirectory(Path.GetDirectoryName(dst));
					entry.ExtractToFile(dst, true);
					progress("Розпакування…", (i + 1) * 100 / entries.Count);
				}
			}
			if (needPlayer)
			{
				RegisterWebPlayer(playerDir);
			}
			if (needFirefox)
			{
				File.WriteAllText(payloadMark, payloadId);
			}
		}
		UTF8Encoding utf8 = new UTF8Encoding(false);
		Directory.CreateDirectory(Path.Combine(ProfileDir, "chrome"));
		File.WriteAllText(Path.Combine(ProfileDir, "user.js"), Resource("user.js").Replace("%GAME_URL%", GameUrl(address)), utf8);
		File.WriteAllText(Path.Combine(ProfileDir, "chrome", "userChrome.css"), Resource("userChrome.css"), utf8);
		// kopatel.cfg (via autoconfig) turns the browser into a game window: F11-style full screen
		// that Esc does not leave, no browser shortcuts, menus, tabs or other pages.
		Directory.CreateDirectory(Path.Combine(FirefoxDir, "defaults", "pref"));
		File.WriteAllText(Path.Combine(FirefoxDir, "defaults", "pref", "autoconfig.js"), Resource("autoconfig.js") + "\n", utf8);
		File.WriteAllText(Path.Combine(FirefoxDir, "kopatel.cfg"), Resource("kopatel.cfg") + "\n", utf8);
	}

	// The Unity Web Player lives per user; reuse an existing install (e.g. from the 2012 launcher).
	private static string WebPlayerDir()
	{
		using (RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Unity\\WebPlayer"))
		{
			string dir = key == null ? null : key.GetValue("Directory") as string;
			if (!string.IsNullOrEmpty(dir))
			{
				return dir;
			}
		}
		return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData\\LocalLow\\Unity\\WebPlayer");
	}

	private static void RegisterWebPlayer(string dir)
	{
		using (RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\Unity\\WebPlayer"))
		{
			if (key.GetValue("Directory") == null)
			{
				key.SetValue("Directory", dir);
			}
			if (key.GetValue("UnityWebPlayerReleaseChannel") == null)
			{
				key.SetValue("UnityWebPlayerReleaseChannel", "Stable");
			}
		}
	}

	public static Process RunningGame()
	{
		string exe = Path.Combine(FirefoxDir, "firefox.exe");
		foreach (Process process in Process.GetProcessesByName("firefox"))
		{
			try
			{
				if (string.Equals(process.MainModule.FileName, exe, StringComparison.OrdinalIgnoreCase))
				{
					return process;
				}
			}
			catch (Exception)
			{
			}
		}
		return null;
	}

	public static Process Start(string address)
	{
		ProcessStartInfo info = new ProcessStartInfo(Path.Combine(FirefoxDir, "firefox.exe"),
			"-no-remote -profile \"" + ProfileDir + "\" \"" + GameUrl(address) + "\"");
		info.UseShellExecute = false;
		info.EnvironmentVariables["MOZ_PLUGIN_PATH"] = Path.Combine(WebPlayerDir(), "loader");
		info.EnvironmentVariables["MOZ_CRASHREPORTER_DISABLE"] = "1";
		return Process.Start(info);
	}
}

// While the game window is in front: Esc becomes ` and F11 becomes F12. The Unity Web Player
// always releases the mouse on Esc and only a click takes it back, while ` opens and closes the
// game's menu just like Esc but keeps the mouse; F12 is the game's full-screen key (handled by the
// browser, see kopatel.cfg), which the plugin would otherwise never let F11 reach.
internal static class GameKeys
{
	private const int WH_KEYBOARD_LL = 13;
	private const int WM_KEYDOWN = 0x100, WM_KEYUP = 0x101, WM_SYSKEYDOWN = 0x104, WM_SYSKEYUP = 0x105;
	private const int KEYEVENTF_KEYUP = 0x2;
	// Marks the keys we inject, so the hook lets them through (other injected input, macros or
	// remote desktops, is remapped like a real keyboard).
	private static readonly IntPtr OwnInput = (IntPtr)0x4B4F50;
	private const byte VK_ESCAPE = 0x1B, VK_OEM_3 = 0xC0, VK_F11 = 0x7A, VK_F12 = 0x7B;

	private delegate IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam);

	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
	private struct KbdLlHookStruct
	{
		public int vkCode, scanCode, flags, time;
		public IntPtr extraInfo;
	}

	[System.Runtime.InteropServices.DllImport("user32.dll")]
	private static extern IntPtr SetWindowsHookEx(int id, HookProc proc, IntPtr module, int threadId);
	[System.Runtime.InteropServices.DllImport("user32.dll")]
	private static extern bool UnhookWindowsHookEx(IntPtr hook);
	[System.Runtime.InteropServices.DllImport("user32.dll")]
	private static extern IntPtr CallNextHookEx(IntPtr hook, int code, IntPtr wParam, IntPtr lParam);
	[System.Runtime.InteropServices.DllImport("user32.dll")]
	private static extern IntPtr GetForegroundWindow();
	[System.Runtime.InteropServices.DllImport("user32.dll")]
	private static extern int GetWindowThreadProcessId(IntPtr window, out int processId);
	[System.Runtime.InteropServices.DllImport("user32.dll")]
	private static extern void keybd_event(byte vk, byte scan, int flags, IntPtr extraInfo);
	[System.Runtime.InteropServices.DllImport("kernel32.dll")]
	private static extern IntPtr GetModuleHandle(string name);

	private static HookProc proc;  // kept alive for the hook's lifetime
	private static IntPtr hook;
	private static int gamePid;

	// Call on a thread with a message loop (the UI thread).
	public static void Install(Process game)
	{
		gamePid = game.Id;
		proc = Hook;
		hook = SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(null), 0);
	}

	public static void Uninstall()
	{
		if (hook != IntPtr.Zero)
		{
			UnhookWindowsHookEx(hook);
			hook = IntPtr.Zero;
		}
	}

	[System.Runtime.InteropServices.DllImport("user32.dll")]
	private static extern IntPtr FindWindowEx(IntPtr parent, IntPtr after, string className, string title);
	[System.Runtime.InteropServices.DllImport("user32.dll")]
	private static extern bool AttachThreadInput(int from, int to, bool attach);
	[System.Runtime.InteropServices.DllImport("user32.dll")]
	private static extern IntPtr SetFocus(IntPtr window);
	[System.Runtime.InteropServices.DllImport("kernel32.dll")]
	private static extern int GetCurrentThreadId();

	private static System.Threading.Timer focusTimer;

	// Switching full screen resizes the window and the keyboard focus stays with Firefox, so WASD
	// would not reach the game until a click. Hand the focus back to the plugin's own window.
	private static void FocusPluginSoon()
	{
		focusTimer = new System.Threading.Timer(state => FocusPlugin(), null, 700, System.Threading.Timeout.Infinite);
	}

	private static void FocusPlugin()
	{
		try
		{
			IntPtr top = Process.GetProcessById(gamePid).MainWindowHandle;
			IntPtr plugin = FindDeep(top, "Unity.WebPlayer");
			if (plugin == IntPtr.Zero)
			{
				return;
			}
			int unused;
			int target = GetWindowThreadProcessId(plugin, out unused);
			int self = GetCurrentThreadId();
			AttachThreadInput(self, target, true);
			SetFocus(plugin);
			AttachThreadInput(self, target, false);
		}
		catch (Exception)
		{
		}
	}

	private static IntPtr FindDeep(IntPtr parent, string className)
	{
		IntPtr child = IntPtr.Zero;
		while ((child = FindWindowEx(parent, child, null, null)) != IntPtr.Zero)
		{
			System.Text.StringBuilder name = new System.Text.StringBuilder(64);
			GetClassName(child, name, name.Capacity);
			if (name.ToString() == className)
			{
				return child;
			}
			IntPtr found = FindDeep(child, className);
			if (found != IntPtr.Zero)
			{
				return found;
			}
		}
		return IntPtr.Zero;
	}

	[System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
	private static extern int GetClassName(IntPtr window, System.Text.StringBuilder name, int size);

	private static bool GameInFront()
	{
		int pid;
		GetWindowThreadProcessId(GetForegroundWindow(), out pid);
		return pid == gamePid;
	}

	private static IntPtr Hook(int code, IntPtr wParam, IntPtr lParam)
	{
		if (code >= 0)
		{
			KbdLlHookStruct key = (KbdLlHookStruct)System.Runtime.InteropServices.Marshal.PtrToStructure(lParam, typeof(KbdLlHookStruct));
			int msg = wParam.ToInt32();
			bool down = msg == WM_KEYDOWN || msg == WM_SYSKEYDOWN;
			bool up = msg == WM_KEYUP || msg == WM_SYSKEYUP;
			byte replacement = key.vkCode == VK_ESCAPE ? VK_OEM_3 : key.vkCode == VK_F11 ? VK_F12 : (byte)0;
			if (replacement != 0 && (down || up) && key.extraInfo != OwnInput && GameInFront())
			{
				keybd_event(replacement, 0, up ? KEYEVENTF_KEYUP : 0, OwnInput);
				if (up && replacement == VK_F12)
				{
					FocusPluginSoon();
				}
				return (IntPtr)1;
			}
			if (key.vkCode == VK_F12 && up && key.extraInfo != OwnInput && GameInFront())
			{
				FocusPluginSoon();
			}
		}
		return CallNextHookEx(hook, code, wParam, lParam);
	}
}

internal class LauncherForm : Form
{
	private readonly TextBox address = new TextBox();
	private readonly Button play = new Button();
	private readonly ProgressBar bar = new ProgressBar();
	private readonly Label status = new Label();

	public LauncherForm()
	{
		Text = "Копатель Онлайн 2014";
		Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
		FormBorderStyle = FormBorderStyle.FixedDialog;
		MaximizeBox = false;
		StartPosition = FormStartPosition.CenterScreen;
		AutoScaleMode = AutoScaleMode.Font;
		Font = new Font("Segoe UI", 9.75f);
		ClientSize = new Size(420, 150);
		Label label = new Label { Text = "Адреса сервера (IP або IP:порт):", Location = new Point(16, 16), AutoSize = true };
		address.Location = new Point(16, 40);
		address.Width = 388;
		address.Text = Launcher.LoadServer();
		play.Text = "Грати";
		play.Location = new Point(304, 76);
		play.Size = new Size(100, 32);
		play.Click += (sender, e) => Play();
		status.Location = new Point(16, 84);
		status.AutoSize = true;
		bar.Location = new Point(16, 120);
		bar.Size = new Size(388, 14);
		bar.Visible = false;
		Controls.AddRange(new Control[] { label, address, play, status, bar });
		AcceptButton = play;
	}

	private void Play()
	{
		string addr = address.Text.Trim();
		if (Launcher.GameUrl(addr) == null)
		{
			MessageBox.Show(this, "Вкажіть адресу сервера, наприклад 192.168.0.10 або 1.2.3.4:8080.", Text);
			return;
		}
		Process running = Launcher.RunningGame();
		if (running != null)
		{
			if (MessageBox.Show(this, "Гра вже запущена. Закрити її й запустити заново?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return;
			}
			running.Kill();
			running.WaitForExit(5000);
		}
		play.Enabled = address.Enabled = false;
		status.Text = "Підготовка…";
		Thread thread = new Thread(() =>
		{
			try
			{
				Launcher.Prepare(addr, (text, percent) => BeginInvoke((Action)(() =>
				{
					status.Text = text;
					bar.Visible = true;
					bar.Value = percent;
				})));
				Process game = Launcher.Start(addr);
				// Stay in the background for the game's lifetime to remap its keys (GameKeys).
				BeginInvoke((Action)(() => RunInBackground(game)));
			}
			catch (Exception ex)
			{
				BeginInvoke((Action)(() =>
				{
					MessageBox.Show(this, "Не вдалося запустити гру:\n" + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
					play.Enabled = address.Enabled = true;
					status.Text = "";
					bar.Visible = false;
				}));
			}
		});
		thread.IsBackground = true;
		thread.Start();
	}

	private void RunInBackground(Process game)
	{
		Hide();
		ShowInTaskbar = false;
		GameKeys.Install(game);
		System.Windows.Forms.Timer watch = new System.Windows.Forms.Timer { Interval = 1000 };
		watch.Tick += (sender, e) =>
		{
			bool exited;
			try
			{
				exited = game.HasExited;
			}
			catch (Exception)
			{
				exited = true;
			}
			if (exited)
			{
				watch.Stop();
				GameKeys.Uninstall();
				Close();
			}
		};
		watch.Start();
	}
}
