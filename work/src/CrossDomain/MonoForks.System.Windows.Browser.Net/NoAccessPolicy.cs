using MonoForks.System.Net;

namespace MonoForks.System.Windows.Browser.Net;

internal sealed class NoAccessPolicy : ICrossDomainPolicy
{
	public bool IsAllowed(WebRequest request)
	{
		return false;
	}
}
