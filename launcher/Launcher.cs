// One-file launcher for the 2012 client: a portable Firefox 51 with Unity Web Player,
// opened on the game page of a chosen server. Built by build_launcher.py (C# 5, .NET Framework 4.5).
using System;
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

[assembly: AssemblyTitle("Копатель Онлайн 2012")]
[assembly: AssemblyProduct("Копатель Онлайн 2012")]

static class Launcher
{
    static readonly string Home = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "KopatelOnline2012");
    static readonly string FirefoxDir = Path.Combine(Home, "firefox");
    static readonly string ProfileDir = Path.Combine(Home, "profile");
    static readonly string ServerFile = Path.Combine(Home, "server.txt");

    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new LauncherForm());
    }

    public static string Resource(string name)
    {
        using (var s = Assembly.GetExecutingAssembly().GetManifestResourceStream(name))
        using (var r = new StreamReader(s, Encoding.UTF8))
            return r.ReadToEnd().Trim();
    }

    public static string LoadServer()
    {
        try { if (File.Exists(ServerFile)) return File.ReadAllText(ServerFile).Trim(); }
        catch (IOException) { }
        return Resource("default_server.txt");
    }

    // "host", "host:port" or a full URL -> the game page URL. The server's default HTTP port is 8080.
    public static string GameUrl(string address)
    {
        address = address.Trim();
        if (address.Length == 0) return null;
        if (!address.Contains("://"))
        {
            string host = address.Split('/')[0];
            bool hasPort = host.StartsWith("[") ? host.Contains("]:") : host.Contains(":");
            address = "http://" + (hasPort ? address : host + ":8080" + address.Substring(host.Length));
        }
        Uri uri;
        if (!Uri.TryCreate(address, UriKind.Absolute, out uri) || (uri.Scheme != "http" && uri.Scheme != "https"))
            return null;
        return uri.AbsoluteUri;
    }

    public static void Prepare(string address, Action<string, int> progress)
    {
        Directory.CreateDirectory(Home);
        File.WriteAllText(ServerFile, address);

        string payloadId = Resource("payload_id.txt");
        string stamp = Path.Combine(FirefoxDir, "kop-payload.txt");
        string webPlayer = WebPlayerDir();
        bool needFirefox = !File.Exists(stamp) || File.ReadAllText(stamp).Trim() != payloadId;
        bool needPlayer = !File.Exists(Path.Combine(webPlayer, "loader", "npUnity3D32.dll"));
        if (needFirefox || needPlayer)
        {
            if (needFirefox && Directory.Exists(FirefoxDir)) Directory.Delete(FirefoxDir, true);
            using (var s = Assembly.GetExecutingAssembly().GetManifestResourceStream("payload.zip"))
            using (var zip = new ZipArchive(s, ZipArchiveMode.Read))
            {
                var entries = zip.Entries.Where(e =>
                    (needFirefox && e.FullName.StartsWith("firefox/")) ||
                    (needPlayer && e.FullName.StartsWith("webplayer/"))).ToList();
                for (int i = 0; i < entries.Count; i++)
                {
                    var e = entries[i];
                    if (e.Name.Length == 0) continue;
                    string rel = e.FullName.Substring(e.FullName.IndexOf('/') + 1).Replace('/', '\\');
                    string target = Path.Combine(e.FullName.StartsWith("firefox/") ? FirefoxDir : webPlayer, rel);
                    Directory.CreateDirectory(Path.GetDirectoryName(target));
                    e.ExtractToFile(target, true);
                    progress("Розпакування…", (i + 1) * 100 / entries.Count);
                }
            }
            if (needPlayer) RegisterWebPlayer(webPlayer);
            if (needFirefox) File.WriteAllText(stamp, payloadId);
        }

        Directory.CreateDirectory(Path.Combine(ProfileDir, "chrome"));
        File.WriteAllText(Path.Combine(ProfileDir, "user.js"),
            Resource("user.js").Replace("%GAME_URL%", GameUrl(address)), new UTF8Encoding(false));
        File.WriteAllText(Path.Combine(ProfileDir, "chrome", "userChrome.css"),
            Resource("userChrome.css"), new UTF8Encoding(false));
    }

    // Unity Web Player lives where its own installer puts it, so an existing install is reused as is.
    static string WebPlayerDir()
    {
        using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Unity\WebPlayer"))
        {
            var dir = key == null ? null : key.GetValue("Directory") as string;
            if (!string.IsNullOrEmpty(dir)) return dir;
        }
        string profile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        return Path.Combine(profile, @"AppData\LocalLow\Unity\WebPlayer");
    }

    static void RegisterWebPlayer(string dir)
    {
        using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Unity\WebPlayer"))
        {
            if (key.GetValue("Directory") == null) key.SetValue("Directory", dir);
            if (key.GetValue("UnityWebPlayerReleaseChannel") == null) key.SetValue("UnityWebPlayerReleaseChannel", "Stable");
        }
    }

    public static Process RunningGame()
    {
        string exe = Path.Combine(FirefoxDir, "firefox.exe");
        foreach (var p in Process.GetProcessesByName("firefox"))
        {
            try { if (string.Equals(p.MainModule.FileName, exe, StringComparison.OrdinalIgnoreCase)) return p; }
            catch (Exception) { } // 64-bit or elevated Firefox processes cannot be inspected
        }
        return null;
    }

    public static void Start(string address)
    {
        var psi = new ProcessStartInfo(Path.Combine(FirefoxDir, "firefox.exe"),
            "-no-remote -profile \"" + ProfileDir + "\" \"" + GameUrl(address) + "\"");
        psi.UseShellExecute = false;
        psi.EnvironmentVariables["MOZ_PLUGIN_PATH"] = Path.Combine(WebPlayerDir(), "loader");
        psi.EnvironmentVariables["MOZ_CRASHREPORTER_DISABLE"] = "1";
        Process.Start(psi);
    }
}

class LauncherForm : Form
{
    readonly TextBox address = new TextBox();
    readonly Button play = new Button();
    readonly ProgressBar bar = new ProgressBar();
    readonly Label status = new Label();

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

        var label = new Label { Text = "Адреса сервера (IP або IP:порт):", Location = new Point(16, 16), AutoSize = true };
        address.Location = new Point(16, 40);
        address.Width = 388;
        address.Text = Launcher.LoadServer();
        play.Text = "Грати";
        play.Location = new Point(304, 76);
        play.Size = new Size(100, 32);
        play.Click += delegate { Play(); };
        status.Location = new Point(16, 84);
        status.AutoSize = true;
        bar.Location = new Point(16, 120);
        bar.Size = new Size(388, 14);
        bar.Visible = false;
        Controls.AddRange(new Control[] { label, address, play, status, bar });
        AcceptButton = play;
    }

    void Play()
    {
        string addr = address.Text.Trim();
        if (Launcher.GameUrl(addr) == null)
        {
            MessageBox.Show(this, "Вкажіть адресу сервера, наприклад 192.168.0.10 або 1.2.3.4:8080.", Text);
            return;
        }
        var running = Launcher.RunningGame();
        if (running != null)
        {
            if (MessageBox.Show(this, "Гра вже запущена. Закрити її й запустити заново?", Text,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            running.Kill();
            running.WaitForExit(5000);
        }
        play.Enabled = address.Enabled = false;
        status.Text = "Підготовка…";
        new Thread(delegate()
        {
            try
            {
                Launcher.Prepare(addr, (text, percent) => BeginInvoke((Action)delegate
                {
                    status.Text = text;
                    bar.Visible = true;
                    bar.Value = percent;
                }));
                Launcher.Start(addr);
                BeginInvoke((Action)Close);
            }
            catch (Exception ex)
            {
                BeginInvoke((Action)delegate
                {
                    MessageBox.Show(this, "Не вдалося запустити гру:\n" + ex.Message, Text,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    play.Enabled = address.Enabled = true;
                    status.Text = "";
                    bar.Visible = false;
                });
            }
        }) { IsBackground = true }.Start();
    }
}
