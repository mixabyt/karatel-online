using MonoForks.System.Net;

namespace MonoForks.System.Windows.Browser.Net;

internal sealed class SiteOfOriginPolicy : ICrossDomainPolicy
{
	public bool IsAllowed(WebRequest request)
	{
		return true;
	}

	public static bool HasSameOrigin(Uri a, Uri b)
	{
		return a.Scheme == b.Scheme && a.DnsSafeHost == b.DnsSafeHost && (a.Port == -1 || b.Port == -1 || a.Port == b.Port);
	}
}
