using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security;
using MonoForks.System.Windows.Interop;

namespace MonoForks.System.Windows.Browser.Net;

internal static class CrossDomainPolicyManager
{
	public const string ClientAccessPolicyFile = "/clientaccesspolicy.xml";

	public const string CrossDomainFile = "/crossdomain.xml";

	private const int Timeout = 10000;

	private const int PolicyPort = 843;

	private static Dictionary<string, ICrossDomainPolicy> policies = new Dictionary<string, ICrossDomainPolicy>();

	internal static ICrossDomainPolicy PolicyDownloadPolicy = new PolicyDownloadPolicy();

	private static ICrossDomainPolicy site_of_origin_policy = new SiteOfOriginPolicy();

	private static ICrossDomainPolicy no_access_policy = new NoAccessPolicy();

	private static readonly Dictionary<string, FlashCrossDomainPolicy> SocketPoliciesByIp = new Dictionary<string, FlashCrossDomainPolicy>();

	public static string GetRoot(Uri uri)
	{
		if ((uri.Scheme == "http" && uri.Port == 80) || (uri.Scheme == "https" && uri.Port == 443) || uri.Port == -1)
		{
			return $"{uri.Scheme}://{uri.DnsSafeHost}/";
		}
		return $"{uri.Scheme}://{uri.DnsSafeHost}:{uri.Port}/";
	}

	private static Uri GetRootUri(Uri uri)
	{
		return new Uri(GetRoot(uri));
	}

	public static Uri GetSilverlightPolicyUri(Uri uri)
	{
		return new Uri(GetRootUri(uri), "/clientaccesspolicy.xml");
	}

	public static Uri GetFlashPolicyUri(Uri uri)
	{
		return new Uri(GetRootUri(uri), "/crossdomain.xml");
	}

	public static ICrossDomainPolicy GetCachedWebPolicy(Uri uri)
	{
		if (SiteOfOriginPolicy.HasSameOrigin(uri, PluginHost.SourceUri))
		{
			return site_of_origin_policy;
		}
		string text = string.Empty;
		if (!uri.IsDefaultPort)
		{
			text = ":" + uri.Port;
		}
		if (uri.ToString() == uri.Scheme + "://" + uri.Host + text + "/crossdomain.xml")
		{
			return PolicyDownloadPolicy;
		}
		string root = GetRoot(uri);
		ICrossDomainPolicy value = null;
		policies.TryGetValue(root, out value);
		return value;
	}

	private static void AddPolicy(Uri responseUri, ICrossDomainPolicy policy)
	{
		string root = GetRoot(responseUri);
		try
		{
			policies.Add(root, policy);
		}
		catch (ArgumentException)
		{
		}
	}

	public static ICrossDomainPolicy BuildFlashPolicy(bool statuscodeOK, Uri uri, Stream responsestream, Dictionary<string, string> responseheaders)
	{
		ICrossDomainPolicy crossDomainPolicy = null;
		if (statuscodeOK)
		{
			try
			{
				crossDomainPolicy = FlashCrossDomainPolicy.FromStream(responsestream);
			}
			catch (Exception ex)
			{
				Log.Msg($"BuildFlashPolicy caught an exception while parsing {uri}: {ex.Message}");
				throw;
			}
			if (crossDomainPolicy != null)
			{
				Log.Msg("crossdomain.xml was succesfully parsed");
				string value = null;
				responseheaders.TryGetValue("X-Permitted-Cross-Domain-Policies", out value);
				if (!string.IsNullOrEmpty(value))
				{
					(crossDomainPolicy as FlashCrossDomainPolicy).SiteControl = value;
				}
			}
		}
		if (crossDomainPolicy == null)
		{
			crossDomainPolicy = no_access_policy;
		}
		AddPolicy(uri, crossDomainPolicy);
		return crossDomainPolicy;
	}

	public static void ClearCache()
	{
		policies.Clear();
	}

	public static bool CheckSocketEndPoint(string connecting_to_ip, int port)
	{
		return CheckSocketEndPoint(connecting_to_ip, port, 843);
	}

	public static bool CheckSocketEndPoint(string connecting_to_ip, int port, int policyport)
	{
		FlashCrossDomainPolicy flashCrossDomainPolicy = FlashCrossDomainPolicyFor(connecting_to_ip, policyport, 3000);
		return flashCrossDomainPolicy.IsSocketConnectionAllowed(port);
	}

	public static FlashCrossDomainPolicy FlashCrossDomainPolicyFor(string connecting_to_ip, int policyport, int timeout)
	{
		if (SocketPoliciesByIp.TryGetValue(connecting_to_ip, out var value))
		{
			Log.Msg($"Policy for host {connecting_to_ip} found in the cache.");
			return value;
		}
		try
		{
			FlashCrossDomainPolicy flashCrossDomainPolicy = RetrieveFlashCrossDomainPolicyFrom(connecting_to_ip, policyport, timeout);
			flashCrossDomainPolicy.PolicyPort = policyport;
			SocketPoliciesByIp.Add(connecting_to_ip, flashCrossDomainPolicy);
			return flashCrossDomainPolicy;
		}
		catch (Exception arg)
		{
			Log.Msg($"{typeof(CrossDomainPolicyManager).Name} caught an exception while checking endpoint {connecting_to_ip}: {arg}");
			return FlashCrossDomainPolicy.DenyPolicy;
		}
	}

	[SecuritySafeCritical]
	private static FlashCrossDomainPolicy RetrieveFlashCrossDomainPolicyFrom(string host, int port, int timeout)
	{
		Type type = Type.GetType("System.Net.Sockets.SocketPolicyClient, System");
		Stream stream = (Stream)type.InvokeMember("GetPolicyStreamForIP", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, null, new object[3] { host, port, timeout });
		if (stream == null)
		{
			throw new Exception("got back null stream from getpolicystream");
		}
		return FlashCrossDomainPolicy.FromStream(stream);
	}
}
