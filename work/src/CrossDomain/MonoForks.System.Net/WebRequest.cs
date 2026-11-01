using System.Collections.Generic;

namespace MonoForks.System.Net;

internal class WebRequest
{
	public Uri RequestUri { get; set; }

	public Dictionary<string, string> Headers { get; set; }

	public WebRequest(Uri requesturi, Dictionary<string, string> headers)
	{
		RequestUri = requesturi;
		Headers = headers;
	}
}
