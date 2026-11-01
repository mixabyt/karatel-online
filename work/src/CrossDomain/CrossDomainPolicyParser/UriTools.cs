using MonoForks.System;

namespace CrossDomainPolicyParser;

internal class UriTools
{
	public static Uri MakeUri(string gameurl, string url)
	{
		if (!url.ToLower().StartsWith("http://") && !url.ToLower().StartsWith("https://") && !url.ToLower().StartsWith("file://"))
		{
			url = GetBaseUrl(gameurl) + "/" + url;
		}
		Log.Msg("About to parse url: " + url);
		return new Uri(url);
	}

	private static string GetBaseUrl(string url)
	{
		return url.Substring(0, url.LastIndexOf('/'));
	}
}
