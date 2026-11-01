// Compiled against the web player's own mscorlib 2.0.5.0 and UnityEngine.dll, then copied
// into Assembly-CSharp by the patcher. Keep it to plain string code: no generics, lambdas or
// foreach, so the copied IL only needs mscorlib and UnityEngine references.
using UnityEngine;

public static class KopatelPatch
{
	// Hosts the 2014 client was built against. URL-style strings ("host/path/") get our
	// "host:port"; the Photon self-hosted IPs are matched whole and get the bare host.
	public static readonly string[] WebHosts = new string[]
	{
		"vk.diggerworld.ru", "vk2.diggerworld.ru", "server1.diggerworld.ru", "www.diggerworld.ru",
		"diggerworld.ru", "62.75.224.107", "188.138.112.218"
	};

	public static readonly string[] PhotonHosts = new string[] { "176.60.181.71", "178.63.48.23" };

	private static string _hostPort;

	// "host:port" of the server the .unity3d came from, e.g. "127.0.0.1:8080".
	public static string HostPort()
	{
		if (_hostPort == null)
		{
			string url = Application.absoluteURL;
			if (url == null)
			{
				url = string.Empty;
			}
			int start = url.IndexOf("://");
			start = (start < 0) ? 0 : start + 3;
			int end = url.IndexOf('/', start);
			_hostPort = (end < 0) ? url.Substring(start) : url.Substring(start, end - start);
			if (_hostPort.Length == 0)
			{
				_hostPort = "127.0.0.1:8080";
			}
		}
		return _hostPort;
	}

	public static string HostOnly()
	{
		string hostPort = HostPort();
		int colon = hostPort.LastIndexOf(':');
		return (colon < 0) ? hostPort : hostPort.Substring(0, colon);
	}

	// Fetch the socket policy (TCP 843) on the main thread before Photon connects from its own,
	// and say in the player log whether it worked: without it every Photon connect fails with
	// "no valid crossdomain policy". (The player's policy client writes to System.Console first;
	// if the plugin process was started with broken std handles, that throws and the policy is
	// denied without any connection - start the game through the launcher, not a console.)
	public static void PrefetchPolicy()
	{
		bool ok = Security.PrefetchSocketPolicy(HostOnly(), 843, 5000);
		Debug.Log("KopatelPatch: socket policy from " + HostOnly() + ":843 -> " + (ok ? "ok" : "FAILED"));
	}

	// Replaces MainMenu.ToggleFullScreen (F12). The player's own full screen always ends on Esc,
	// which the game needs for its menu, so ask the page to switch the browser window instead.
	public static void ToggleFullScreen()
	{
		Application.ExternalCall("ToggleFullScreen", new object[0]);
	}

	public static string Host(string s)
	{
		if (s == null)
		{
			return s;
		}
		for (int i = 0; i < PhotonHosts.Length; i++)
		{
			if (s == PhotonHosts[i])
			{
				return HostOnly();
			}
		}
		string scheme = string.Empty;
		string rest = s;
		if (rest.StartsWith("http://"))
		{
			scheme = "http://";
			rest = rest.Substring(7);
		}
		else if (rest.StartsWith("https://"))
		{
			scheme = "http://";
			rest = rest.Substring(8);
		}
		for (int i = 0; i < WebHosts.Length; i++)
		{
			string host = WebHosts[i];
			if (rest == host || rest.StartsWith(host + "/"))
			{
				return scheme + HostPort() + rest.Substring(host.Length);
			}
		}
		return s;
	}
}
