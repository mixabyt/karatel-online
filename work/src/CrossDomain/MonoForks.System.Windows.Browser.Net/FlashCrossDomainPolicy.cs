using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MonoForks.Mono.Xml;

namespace MonoForks.System.Windows.Browser.Net;

internal class FlashCrossDomainPolicy : BaseDomainPolicy
{
	public class AllowAccessFrom
	{
		public string Domain { get; set; }

		public bool AllowAnyPort { get; set; }

		public int[] ToPorts { get; set; }

		public bool Secure { get; set; }

		public AllowAccessFrom()
		{
			Secure = true;
		}

		public bool IsAllowed(Uri uri, string[] headerKeys)
		{
			Log.Msg(string.Concat("Checking if ", uri, " is a valid domain"));
			if (!CheckDomain(uri))
			{
				return false;
			}
			if (!AllowAnyPort && ToPorts != null && Array.IndexOf(ToPorts, uri.Port) < 0)
			{
				Log.Msg("requested port: " + uri.Port + " is not allowed by specified portrange");
				return false;
			}
			if (!Secure)
			{
				return true;
			}
			if (BaseDomainPolicy.ApplicationUri.Scheme == Uri.UriSchemeHttps)
			{
				return uri.Scheme == Uri.UriSchemeHttps;
			}
			Log.Msg("All requirements met, the request is approved");
			return true;
		}

		public bool IsSocketConnectionAllowed(int port, int policyport)
		{
			if (policyport > 1024 && port < 1024)
			{
				return false;
			}
			bool flag = false;
			if (AllowAnyPort)
			{
				flag = true;
			}
			if (ToPorts != null)
			{
				int[] toPorts = ToPorts;
				foreach (int num in toPorts)
				{
					if (num == port)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return Domain == "*";
		}

		private bool CheckDomain(Uri uri)
		{
			Log.Msg("Checking request-host: " + uri.Host + " against valid domain: " + Domain);
			if (Domain == "*")
			{
				return true;
			}
			if (BaseDomainPolicy.ApplicationUri.Host == Domain)
			{
				return true;
			}
			if (Domain[0] != '*')
			{
				return false;
			}
			string value = Domain.Substring(1, Domain.Length - 1);
			if (uri.Host.EndsWith(value))
			{
				return true;
			}
			return false;
		}
	}

	public class AllowHttpRequestHeadersFrom
	{
		public string Domain { get; set; }

		public bool AllowAllHeaders { get; set; }

		public Headers Headers { get; private set; }

		public bool Secure { get; set; }

		public AllowHttpRequestHeadersFrom()
		{
			Headers = new Headers();
		}

		public bool IsRejected(Uri uri, string[] headerKeys)
		{
			if (Headers.IsAllowed(headerKeys))
			{
				return false;
			}
			if (!Secure)
			{
				return true;
			}
			if (BaseDomainPolicy.ApplicationUri.Scheme == Uri.UriSchemeHttps)
			{
				return uri.Scheme == Uri.UriSchemeHttps;
			}
			return true;
		}
	}

	private class Reader : MiniParser.IReader
	{
		private Stream stream;

		public Reader(Stream stream)
		{
			this.stream = stream;
		}

		public int Read()
		{
			return stream.ReadByte();
		}
	}

	private class Handler : MiniParser.IHandler
	{
		private string current;

		private Stack stack = new Stack();

		private FlashCrossDomainPolicy cdp;

		public Handler(FlashCrossDomainPolicy cdp)
		{
			this.cdp = cdp;
		}

		public void OnStartElement(string name, MiniParser.IAttrList attrs)
		{
			Log.Msg("Parsing: " + name);
			if (current == null)
			{
				if (name != "cross-domain-policy")
				{
					throw new Exception("Expected root element to be <cross-domain-policy>. found " + name + " instead");
				}
			}
			else
			{
				if (!(current == "cross-domain-policy"))
				{
					throw new Exception("Invalid element " + name);
				}
				switch (name)
				{
				case "site-control":
					cdp.SiteControl = attrs.GetValue("permitted-cross-domain-policies");
					break;
				case "allow-access-from":
				{
					AllowAccessFrom allowAccessFrom = new AllowAccessFrom();
					allowAccessFrom.Domain = attrs.GetValue("domain");
					allowAccessFrom.Secure = ReadBooleanAttribute(attrs, "secure");
					AllowAccessFrom allowAccessFrom2 = allowAccessFrom;
					string value = attrs.GetValue("to-ports");
					if (value == "*")
					{
						allowAccessFrom2.AllowAnyPort = true;
					}
					else if (value != null)
					{
						ArrayList arrayList = new ArrayList();
						string[] array = value.Split(',');
						string[] array2 = array;
						foreach (string text in array2)
						{
							int result3;
							if (text.Contains("-"))
							{
								string[] array3 = text.Split('-');
								if (array3.Length == 2 && int.TryParse(array3[0], out var result) && int.TryParse(array3[1], out var result2))
								{
									for (int j = result; j != result2; j++)
									{
										arrayList.Add(j);
									}
								}
							}
							else if (int.TryParse(text, out result3))
							{
								arrayList.Add(result3);
							}
						}
						allowAccessFrom2.ToPorts = (int[])arrayList.ToArray(typeof(int));
					}
					cdp.AllowedAccesses.Add(allowAccessFrom2);
					break;
				}
				case "allow-http-request-headers-from":
				{
					AllowHttpRequestHeadersFrom allowHttpRequestHeadersFrom = new AllowHttpRequestHeadersFrom();
					allowHttpRequestHeadersFrom.Domain = attrs.GetValue("domain");
					allowHttpRequestHeadersFrom.Secure = ReadBooleanAttribute(attrs, "secure");
					AllowHttpRequestHeadersFrom allowHttpRequestHeadersFrom2 = allowHttpRequestHeadersFrom;
					allowHttpRequestHeadersFrom2.Headers.SetHeaders(attrs.GetValue("headers"));
					cdp.AllowedHttpRequestHeaders.Add(allowHttpRequestHeadersFrom2);
					break;
				}
				}
			}
			stack.Push(name);
			current = name;
			Log.Msg(name);
			int length = attrs.Length;
			for (int k = 0; k < length; k++)
			{
				Log.Msg("  " + attrs.GetName(k) + ": " + attrs.GetValue(k));
			}
		}

		public void OnEndElement(string name)
		{
			stack.Pop();
			current = ((stack.Count <= 0) ? null : ((string)stack.Peek()));
		}

		public void OnStartParsing(MiniParser parser)
		{
		}

		public void OnChars(string ch)
		{
		}

		public void OnEndParsing(MiniParser parser)
		{
		}
	}

	private string site_control;

	public static FlashCrossDomainPolicy DenyPolicy = new FlashCrossDomainPolicy();

	public int PolicyPort { get; set; }

	public List<AllowAccessFrom> AllowedAccesses { get; private set; }

	public List<AllowHttpRequestHeadersFrom> AllowedHttpRequestHeaders { get; private set; }

	public string SiteControl
	{
		get
		{
			return (!string.IsNullOrEmpty(site_control)) ? site_control : "all";
		}
		set
		{
			site_control = value;
		}
	}

	public FlashCrossDomainPolicy()
	{
		AllowedAccesses = new List<AllowAccessFrom>();
		AllowedHttpRequestHeaders = new List<AllowHttpRequestHeadersFrom>();
		PolicyPort = 843;
	}

	public bool IsSocketConnectionAllowed(int port)
	{
		foreach (AllowAccessFrom allowedAccess in AllowedAccesses)
		{
			if (allowedAccess.IsSocketConnectionAllowed(port, PolicyPort))
			{
				return true;
			}
		}
		return false;
	}

	public override bool IsAllowed(Uri uri, string[] headerKeys)
	{
		switch (SiteControl)
		{
		default:
			Log.Msg("rejected because SiteControl does not have a valid value");
			return false;
		case "all":
		case "master-only":
		case "by-ftp-filename":
		{
			bool flag = false;
			if (AllowedAccesses.Count > 0)
			{
				foreach (AllowAccessFrom allowedAccess in AllowedAccesses)
				{
					if (allowedAccess.IsAllowed(uri, headerKeys))
					{
						flag = true;
					}
				}
			}
			if (!flag)
			{
				Log.Msg("Rejected because there was no AllowedAcces entry in the crossdomain file allowing this request.");
				return false;
			}
			if (AllowedHttpRequestHeaders.Count > 0)
			{
				foreach (AllowHttpRequestHeadersFrom allowedHttpRequestHeader in AllowedHttpRequestHeaders)
				{
					if (allowedHttpRequestHeader.IsRejected(uri, headerKeys))
					{
						return false;
					}
				}
			}
			return true;
		}
		}
	}

	public static bool ReadBooleanAttribute(MiniParser.IAttrList attrs, string attribute)
	{
		switch (attrs.GetValue(attribute))
		{
		default:
		{
			int num;
			if (num == 1)
			{
				return false;
			}
			throw new Exception("Invalid boolean attribute: " + attribute);
		}
		case "true":
		case null:
			return true;
		}
	}

	public static FlashCrossDomainPolicy FromStream(Stream originalStream)
	{
		Log.Msg("received policy");
		MemoryStream memoryStream = StripTrailing0(originalStream);
		if (memoryStream.Length == 0L)
		{
			throw new ArgumentException("Policy can't be constructed from empty stream.");
		}
		FlashCrossDomainPolicy flashCrossDomainPolicy = new FlashCrossDomainPolicy();
		Handler handler = new Handler(flashCrossDomainPolicy);
		Reader reader = new Reader(memoryStream);
		MiniParser miniParser = new MiniParser();
		miniParser.Parse(reader, handler);
		if (flashCrossDomainPolicy.AllowedHttpRequestHeaders.Count == 0)
		{
			AllowHttpRequestHeadersFrom allowHttpRequestHeadersFrom = new AllowHttpRequestHeadersFrom();
			allowHttpRequestHeadersFrom.Domain = "*";
			allowHttpRequestHeadersFrom.Secure = true;
			AllowHttpRequestHeadersFrom allowHttpRequestHeadersFrom2 = allowHttpRequestHeadersFrom;
			allowHttpRequestHeadersFrom2.Headers.SetHeaders(null);
			flashCrossDomainPolicy.AllowedHttpRequestHeaders.Add(allowHttpRequestHeadersFrom2);
		}
		Log.Msg("done parsing policy");
		return flashCrossDomainPolicy;
	}

	private static MemoryStream StripTrailing0(Stream stream)
	{
		MemoryStream memoryStream = new MemoryStream();
		while (true)
		{
			int num = stream.ReadByte();
			if (num == 0 || num == -1)
			{
				break;
			}
			memoryStream.WriteByte((byte)num);
		}
		memoryStream.Seek(0L, SeekOrigin.Begin);
		return memoryStream;
	}
}
