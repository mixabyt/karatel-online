using UnityEngine;

namespace MonoForks.System.Windows.Interop;

public class PluginHost
{
	public static Uri SourceUri => new Uri(UnityCrossDomainHelper.GetWebSecurityHostUri());

	public static Uri RootUri => new Uri(GetRoot(SourceUri));

	private static string GetRoot(Uri uri)
	{
		if ((uri.Scheme == "http" && uri.Port == 80) || (uri.Scheme == "https" && uri.Port == 443) || uri.Port == -1)
		{
			return $"{uri.Scheme}://{uri.DnsSafeHost}";
		}
		return $"{uri.Scheme}://{uri.DnsSafeHost}:{uri.Port}";
	}
}
