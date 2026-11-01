using System;
using System.Collections.Generic;
using MonoForks.System.Net;
using MonoForks.System.Windows.Interop;

namespace MonoForks.System.Windows.Browser.Net;

internal abstract class BaseDomainPolicy : ICrossDomainPolicy
{
	public class Headers
	{
		private class PrefixComparer : IEqualityComparer<string>
		{
			public bool Equals(string x, string y)
			{
				int num = x.Length - 1;
				if (x.Length > 0 && x[num] == '*')
				{
					return string.Compare(x, 0, y, 0, num, StringComparison.OrdinalIgnoreCase) == 0;
				}
				return string.Compare(x, y, StringComparison.OrdinalIgnoreCase) == 0;
			}

			public int GetHashCode(string obj)
			{
				return obj?.GetHashCode() ?? 0;
			}
		}

		private static PrefixComparer pc = new PrefixComparer();

		private List<string> list;

		public bool AllowAllHeaders { get; private set; }

		public bool IsAllowed(string[] headers)
		{
			if (AllowAllHeaders)
			{
				return true;
			}
			if (headers == null || headers.Length == 0)
			{
				return true;
			}
			foreach (string y in headers)
			{
				bool flag = false;
				foreach (string item in list)
				{
					if (pc.Equals(item, y))
					{
						flag = true;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		public void SetHeaders(string raw)
		{
			if (raw == "*")
			{
				AllowAllHeaders = true;
				list = null;
			}
			else if (raw != null)
			{
				string[] array = raw.Split(',');
				list = new List<string>(array.Length + 1);
				list.Add("Content-Type");
				for (int i = 0; i < array.Length; i++)
				{
					string text = array[i].Trim();
					if (!string.IsNullOrEmpty(text))
					{
						list.Add(text);
					}
				}
			}
			else
			{
				AllowAllHeaders = false;
				list = new List<string>(1);
				list.Add("Content-Type");
			}
		}
	}

	private static string root;

	public static Uri ApplicationUri => PluginHost.RootUri;

	public static string ApplicationRoot
	{
		get
		{
			if (root == null)
			{
				root = CrossDomainPolicyManager.GetRoot(ApplicationUri);
			}
			return root;
		}
	}

	public bool IsAllowed(WebRequest request)
	{
		Dictionary<string, string>.KeyCollection keys = request.Headers.Keys;
		string[] array = new string[keys.Count];
		keys.CopyTo(array, 0);
		return IsAllowed(request.RequestUri, array);
	}

	public abstract bool IsAllowed(Uri uri, params string[] headerKeys);
}
