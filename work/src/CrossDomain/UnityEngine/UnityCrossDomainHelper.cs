using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security;
using CrossDomainPolicyParser;
using MonoForks.Mono.Xml;
using MonoForks.System;
using MonoForks.System.Net;
using MonoForks.System.Windows.Browser.Net;

namespace UnityEngine;

public class UnityCrossDomainHelper
{
	public enum SecurityPolicy
	{
		DontKnowYet,
		AllowAccess,
		DenyAccess
	}

	private interface IPolicyProvider
	{
		Stream GetPolicy(string url);
	}

	private class WWWPolicyProvider : IPolicyProvider
	{
		private Dictionary<string, WWW> policyDownloads = new Dictionary<string, WWW>();

		public void ClearCache()
		{
			policyDownloads.Clear();
		}

		public Stream GetPolicy(string policyurl)
		{
			WWW value = null;
			policyDownloads.TryGetValue(policyurl, out value);
			if (value != null)
			{
				if (!value.isDone)
				{
					return null;
				}
				bool flag = value.error == null;
				if (!flag)
				{
					throw new InvalidOperationException("Unable to download policy");
				}
				if (flag)
				{
					Log.Msg("Download had OK statuscode");
					Log.Msg("Received the following crossdomain.xml");
					Log.Msg("----------");
					Log.Msg(value.text);
					Log.Msg("----------");
					return new MemoryStream(value.bytes);
				}
			}
			WWW value2 = new WWW(policyurl);
			policyDownloads.Add(policyurl, value2);
			return null;
		}
	}

	private class WebRequestPolicyProvider : IPolicyProvider
	{
		private MethodInfo methodinfo;

		public WebRequestPolicyProvider(MethodInfo mi)
		{
			methodinfo = mi;
		}

		[SecuritySafeCritical]
		public Stream GetPolicy(string policy_url)
		{
			string text = Environment.GetEnvironmentVariable("UNITY_PROXYSERVER");
			if (string.IsNullOrEmpty(text))
			{
				text = null;
			}
			object obj = methodinfo.Invoke(null, new object[2] { policy_url, text });
			return (Stream)obj;
		}
	}

	public delegate string GetWebSecurityHostUriDelegate();

	private static GetWebSecurityHostUriDelegate getWebSecurityHostUriDelegate = DefaultGetWebSecurityHostUri;

	private static WWWPolicyProvider wwwPolicyProvider = new WWWPolicyProvider();

	public static string GetWebSecurityHostUri()
	{
		return getWebSecurityHostUriDelegate();
	}

	internal static void SetWebSecurityHostUriDelegate(GetWebSecurityHostUriDelegate d)
	{
		getWebSecurityHostUriDelegate = d;
	}

	private static string DefaultGetWebSecurityHostUri()
	{
		return Application.webSecurityHostUrl;
	}

	public static void ClearCache()
	{
		wwwPolicyProvider.ClearCache();
		CrossDomainPolicyManager.ClearCache();
	}

	public static SecurityPolicy GetSecurityPolicy(string requesturi_string)
	{
		return GetSecurityPolicy(requesturi_string, wwwPolicyProvider);
	}

	public static bool GetSecurityPolicyForDotNetWebRequest(string requesturi_string, MethodInfo policyProvidingMethod)
	{
		WebRequestPolicyProvider policyProvider = new WebRequestPolicyProvider(policyProvidingMethod);
		return GetSecurityPolicy(requesturi_string, policyProvider) == SecurityPolicy.AllowAccess;
	}

	private static SecurityPolicy GetSecurityPolicy(string requesturi_string, IPolicyProvider policyProvider)
	{
		Uri uri = UriTools.MakeUri(Application.webSecurityHostUrl, requesturi_string);
		if (uri.Scheme == "file")
		{
			if (Application.isEditor)
			{
				return SecurityPolicy.AllowAccess;
			}
			Uri uri2 = new Uri(Application.webSecurityHostUrl);
			if (Application.isWebPlayer && uri2.Scheme == "file")
			{
				return SecurityPolicy.AllowAccess;
			}
			return SecurityPolicy.DenyAccess;
		}
		ICrossDomainPolicy cachedWebPolicy = CrossDomainPolicyManager.GetCachedWebPolicy(uri);
		if (cachedWebPolicy != null)
		{
			WebRequest request = new WebRequest(uri, new Dictionary<string, string>());
			return cachedWebPolicy.IsAllowed(request) ? SecurityPolicy.AllowAccess : SecurityPolicy.DenyAccess;
		}
		if (ShouldEnableLogging())
		{
			Log.SetLog(Console.WriteLine);
		}
		Log.Msg("Determining crossdomain.xml location for request: " + uri);
		Uri flashPolicyUri = CrossDomainPolicyManager.GetFlashPolicyUri(uri);
		try
		{
			Stream policy = policyProvider.GetPolicy(flashPolicyUri.ToString());
			if (policy == null)
			{
				return SecurityPolicy.DontKnowYet;
			}
			CrossDomainPolicyManager.BuildFlashPolicy(statuscodeOK: true, flashPolicyUri, policy, new Dictionary<string, string>());
		}
		catch (InvalidOperationException)
		{
			return SecurityPolicy.DenyAccess;
		}
		catch (MiniParser.XMLError xMLError)
		{
			Debug.Log($"Error reading crossdomain policy: {xMLError.Message}");
			return SecurityPolicy.DenyAccess;
		}
		return GetSecurityPolicy(requesturi_string, policyProvider);
	}

	[SecuritySafeCritical]
	private static bool ShouldEnableLogging()
	{
		return Environment.GetEnvironmentVariable("ENABLE_CROSSDOMAIN_LOGGING") == "1";
	}

	public static bool CheckSocketEndPoint(string connecting_to_ip, int port)
	{
		Log.Msg("CheckSocketEndpoint called for " + connecting_to_ip + " with port: " + port);
		if (!Application.webSecurityEnabled)
		{
			return true;
		}
		bool flag = CrossDomainPolicyManager.CheckSocketEndPoint(connecting_to_ip, port);
		Log.Msg("CheckSocketENdpoint returns :" + flag);
		return flag;
	}

	public static bool PrefetchSocketPolicy(string ip, int policyport, int timeout)
	{
		if (!Application.webSecurityEnabled)
		{
			return false;
		}
		FlashCrossDomainPolicy flashCrossDomainPolicy = CrossDomainPolicyManager.FlashCrossDomainPolicyFor(ip, policyport, timeout);
		return flashCrossDomainPolicy != FlashCrossDomainPolicy.DenyPolicy;
	}
}
