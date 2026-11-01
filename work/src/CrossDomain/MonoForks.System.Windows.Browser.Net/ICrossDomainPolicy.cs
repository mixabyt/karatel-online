using MonoForks.System.Net;

namespace MonoForks.System.Windows.Browser.Net;

internal interface ICrossDomainPolicy
{
	bool IsAllowed(WebRequest request);
}
