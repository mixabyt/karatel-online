using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

[assembly: AssemblyTitle("Копатель Онлайн 2012")]
[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: AssemblyProduct("Копатель Онлайн 2012")]
[assembly: AssemblyVersion("0.0.0.0")]
internal static class Launcher
{
	private static readonly string Home = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "KopatelOnline2012");

	private static readonly string FirefoxDir = Path.Combine(Home, "firefox");

	private static readonly string ProfileDir = Path.Combine(Home, "profile");

	private static readonly string ServerFile = Path.Combine(Home, "server.txt");

	[STAThread]
	private static void Main()
	{
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(defaultValue: false);
		Application.Run(new LauncherForm());
	}

	public static string Resource(string name)
	{
		using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
		using StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);
		return streamReader.ReadToEnd().Trim();
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

	public static string GameUrl(string address)
	{
		address = address.Trim();
		if (address.Length == 0)
		{
			return null;
		}
		if (!address.Contains("://"))
		{
			string text = address.Split('/')[0];
			bool flag = (text.StartsWith("[") ? text.Contains("]:") : text.Contains(":"));
			address = "http://" + (flag ? address : (text + ":8080" + address.Substring(text.Length)));
		}
		if (!Uri.TryCreate(address, UriKind.Absolute, out var result) || (result.Scheme != "http" && result.Scheme != "https"))
		{
			return null;
		}
		return result.AbsoluteUri;
	}

	public static void Prepare(string address, Action<string, int> progress)
	{
		Directory.CreateDirectory(Home);
		File.WriteAllText(ServerFile, address);
		string text = Resource("payload_id.txt");
		string path = Path.Combine(FirefoxDir, "kop-payload.txt");
		string text2 = WebPlayerDir();
		bool needFirefox = !File.Exists(path) || File.ReadAllText(path).Trim() != text;
		bool needPlayer = !File.Exists(Path.Combine(text2, "loader", "npUnity3D32.dll"));
		if (needFirefox || needPlayer)
		{
			if (needFirefox && Directory.Exists(FirefoxDir))
			{
				Directory.Delete(FirefoxDir, recursive: true);
			}
			using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("payload.zip"))
			{
				using ZipArchive zipArchive = new ZipArchive(stream, ZipArchiveMode.Read);
				List<ZipArchiveEntry> list = zipArchive.Entries.Where((ZipArchiveEntry e) => (needFirefox && e.FullName.StartsWith("firefox/")) || (needPlayer && e.FullName.StartsWith("webplayer/"))).ToList();
				for (int num = 0; num < list.Count; num++)
				{
					ZipArchiveEntry zipArchiveEntry = list[num];
					if (zipArchiveEntry.Name.Length != 0)
					{
						string path2 = zipArchiveEntry.FullName.Substring(zipArchiveEntry.FullName.IndexOf('/') + 1).Replace('/', '\\');
						string text3 = Path.Combine(zipArchiveEntry.FullName.StartsWith("firefox/") ? FirefoxDir : text2, path2);
						Directory.CreateDirectory(Path.GetDirectoryName(text3));
						zipArchiveEntry.ExtractToFile(text3, overwrite: true);
						progress("Розпакування…", (num + 1) * 100 / list.Count);
					}
				}
			}
			if (needPlayer)
			{
				RegisterWebPlayer(text2);
			}
			if (needFirefox)
			{
				File.WriteAllText(path, text);
			}
		}
		Directory.CreateDirectory(Path.Combine(ProfileDir, "chrome"));
		File.WriteAllText(Path.Combine(ProfileDir, "user.js"), Resource("user.js").Replace("%GAME_URL%", GameUrl(address)), new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
		File.WriteAllText(Path.Combine(ProfileDir, "chrome", "userChrome.css"), Resource("userChrome.css"), new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
	}

	private static string WebPlayerDir()
	{
		using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\Unity\\WebPlayer"))
		{
			string text = ((registryKey == null) ? null : (registryKey.GetValue("Directory") as string));
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
		}
		string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
		return Path.Combine(folderPath, "AppData\\LocalLow\\Unity\\WebPlayer");
	}

	private static void RegisterWebPlayer(string dir)
	{
		using RegistryKey registryKey = Registry.CurrentUser.CreateSubKey("Software\\Unity\\WebPlayer");
		if (registryKey.GetValue("Directory") == null)
		{
			registryKey.SetValue("Directory", dir);
		}
		if (registryKey.GetValue("UnityWebPlayerReleaseChannel") == null)
		{
			registryKey.SetValue("UnityWebPlayerReleaseChannel", "Stable");
		}
	}

	public static Process RunningGame()
	{
		string b = Path.Combine(FirefoxDir, "firefox.exe");
		Process[] processesByName = Process.GetProcessesByName("firefox");
		foreach (Process process in processesByName)
		{
			try
			{
				if (string.Equals(process.MainModule.FileName, b, StringComparison.OrdinalIgnoreCase))
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

	public static void Start(string address)
	{
		ProcessStartInfo processStartInfo = new ProcessStartInfo(Path.Combine(FirefoxDir, "firefox.exe"), "-no-remote -profile \"" + ProfileDir + "\" \"" + GameUrl(address) + "\"");
		processStartInfo.UseShellExecute = false;
		processStartInfo.EnvironmentVariables["MOZ_PLUGIN_PATH"] = Path.Combine(WebPlayerDir(), "loader");
		processStartInfo.EnvironmentVariables["MOZ_CRASHREPORTER_DISABLE"] = "1";
		Process.Start(processStartInfo);
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
		Text = "Копатель Онлайн 2012";
		Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
		FormBorderStyle = FormBorderStyle.FixedDialog;
		MaximizeBox = false;
		StartPosition = FormStartPosition.CenterScreen;
		AutoScaleMode = AutoScaleMode.Font;
		Font = new Font("Segoe UI", 9.75f);
		ClientSize = new Size(420, 150);
		Label label = new Label
		{
			Text = "Адреса сервера (IP або IP:порт):",
			Location = new Point(16, 16),
			AutoSize = true
		};
		address.Location = new Point(16, 40);
		address.Width = 388;
		address.Text = Launcher.LoadServer();
		play.Text = "Грати";
		play.Location = new Point(304, 76);
		play.Size = new Size(100, 32);
		Button button = play;
		EventHandler value = (object param0, EventArgs param1) =>
		{
			Play();
		};
		button.Click += value;
		status.Location = new Point(16, 84);
		status.AutoSize = true;
		bar.Location = new Point(16, 120);
		bar.Size = new Size(388, 14);
		bar.Visible = false;
		Controls.AddRange(new Control[5] { label, address, play, status, bar });
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
		Process process = Launcher.RunningGame();
		if (process != null)
		{
			if (MessageBox.Show(this, "Гра вже запущена. Закрити її й запустити заново?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return;
			}
			process.Kill();
			process.WaitForExit(5000);
		}
		Button button = play;
		bool enabled = (address.Enabled = false);
		button.Enabled = enabled;
		status.Text = "Підготовка…";
		Thread thread = new Thread(() =>
		{
			try
			{
				Launcher.Prepare(addr, (string text, int percent) =>
				{
					BeginInvoke((Action)(() =>
					{
						status.Text = text;
						bar.Visible = true;
						bar.Value = percent;
					}));
				});
				Launcher.Start(addr);
				BeginInvoke(new Action(Close));
			}
			catch (Exception ex)
			{
				Exception ex2 = ex;
				BeginInvoke((Action)(() =>
				{
					MessageBox.Show(this, "Не вдалося запустити гру:\n" + ex2.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Hand);
					Button button2 = play;
					bool enabled2 = (address.Enabled = true);
					button2.Enabled = enabled2;
					status.Text = "";
					bar.Visible = false;
				}));
			}
		});
		thread.IsBackground = true;
		thread.Start();
	}
}
