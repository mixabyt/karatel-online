using MonoForks.System.Net;

namespace MonoForks.System.Windows.Browser.Net;

internal sealed class PolicyDownloadPolicy : ICrossDomainPolicy
{
	public bool IsAllowed(WebRequest request)
	{
		return IsLocalPathPolicy(request.RequestUri);
	}

	public static bool IsLocalPathPolicy(Uri uri)
	{
		string localPath = uri.LocalPath;
		if (string.CompareOrdinal(localPath, "/clientaccesspolicy.xml") == 0)
		{
			return true;
		}
		if (string.CompareOrdinal(localPath, "/crossdomain.xml") == 0)
		{
			return true;
		}
		return false;
	}
}
