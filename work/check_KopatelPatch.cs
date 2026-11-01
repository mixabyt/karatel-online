using UnityEngine;

public static class KopatelPatch
{
	public static readonly string[] WebHosts = new string[7] { "vk.diggerworld.ru", "vk2.diggerworld.ru", "server1.diggerworld.ru", "www.diggerworld.ru", "diggerworld.ru", "62.75.224.107", "188.138.112.218" };

	public static readonly string[] PhotonHosts = new string[2] { "176.60.181.71", "178.63.48.23" };

	private static string _hostPort;

	public static string HostPort()
	{
		if (_hostPort == null)
		{
			string text = Application.absoluteURL;
			if (text == null)
			{
				text = string.Empty;
			}
			int num = text.IndexOf("://");
			num = ((num >= 0) ? (num + 3) : 0);
			int num2 = text.IndexOf('/', num);
			_hostPort = ((num2 < 0) ? text.Substring(num) : text.Substring(num, num2 - num));
			if (_hostPort.Length == 0)
			{
				_hostPort = "127.0.0.1:8080";
			}
		}
		return _hostPort;
	}

	public static string HostOnly()
	{
		string text = HostPort();
		int num = text.LastIndexOf(':');
		if (num >= 0)
		{
			return text.Substring(0, num);
		}
		return text;
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
		string text = string.Empty;
		string text2 = s;
		if (text2.StartsWith("http://"))
		{
			text = "http://";
			text2 = text2.Substring(7);
		}
		else if (text2.StartsWith("https://"))
		{
			text = "http://";
			text2 = text2.Substring(8);
		}
		for (int j = 0; j < WebHosts.Length; j++)
		{
			string text3 = WebHosts[j];
			if (text2 == text3 || text2.StartsWith(text3 + "/"))
			{
				return text + HostPort() + text2.Substring(text3.Length);
			}
		}
		return s;
	}
}
