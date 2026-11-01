using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using MonoForks.System.Net;

namespace MonoForks.System;

[Serializable]
public class Uri : MarshalByRefObject, ISerializable
{
	private struct UriScheme(string s, string d, int p)
	{
		public string scheme = s;

		public string delimiter = d;

		public int defaultPort = p;
	}

	private bool isUnixFilePath;

	private string source;

	private string scheme = string.Empty;

	private string host = string.Empty;

	private int port = -1;

	private string path = string.Empty;

	private string query = string.Empty;

	private string fragment = string.Empty;

	private string userinfo = string.Empty;

	private bool isUnc;

	private bool isOpaquePart;

	private bool isAbsoluteUri = true;

	private string[] segments;

	private bool userEscaped;

	private string cachedAbsoluteUri;

	private string cachedToString;

	private string cachedLocalPath;

	private int cachedHashCode;

	private static readonly string hexUpperChars = "0123456789ABCDEF";

	public static readonly string SchemeDelimiter = "://";

	public static readonly string UriSchemeFile = "file";

	public static readonly string UriSchemeFtp = "ftp";

	public static readonly string UriSchemeGopher = "gopher";

	public static readonly string UriSchemeHttp = "http";

	public static readonly string UriSchemeHttps = "https";

	public static readonly string UriSchemeMailto = "mailto";

	public static readonly string UriSchemeNews = "news";

	public static readonly string UriSchemeNntp = "nntp";

	private static UriScheme[] schemes = new UriScheme[8]
	{
		new UriScheme(UriSchemeHttp, SchemeDelimiter, 80),
		new UriScheme(UriSchemeHttps, SchemeDelimiter, 443),
		new UriScheme(UriSchemeFtp, SchemeDelimiter, 21),
		new UriScheme(UriSchemeFile, SchemeDelimiter, -1),
		new UriScheme(UriSchemeMailto, ":", 25),
		new UriScheme(UriSchemeNews, ":", 119),
		new UriScheme(UriSchemeNntp, SchemeDelimiter, 119),
		new UriScheme(UriSchemeGopher, SchemeDelimiter, 70)
	};

	public string AbsolutePath => path;

	public string AbsoluteUri
	{
		get
		{
			EnsureAbsoluteUri();
			if (cachedAbsoluteUri == null)
			{
				cachedAbsoluteUri = GetLeftPart(UriPartial.Path);
				if (query.Length > 0)
				{
					cachedAbsoluteUri += query;
				}
				if (fragment.Length > 0)
				{
					cachedAbsoluteUri += fragment;
				}
			}
			return cachedAbsoluteUri;
		}
	}

	public string Authority
	{
		get
		{
			EnsureAbsoluteUri();
			return (GetDefaultPort(Scheme) != port) ? (host + ":" + port) : host;
		}
	}

	public string Fragment
	{
		get
		{
			EnsureAbsoluteUri();
			return fragment;
		}
	}

	public string Host
	{
		get
		{
			EnsureAbsoluteUri();
			return host;
		}
	}

	public UriHostNameType HostNameType
	{
		get
		{
			EnsureAbsoluteUri();
			UriHostNameType uriHostNameType = CheckHostName(Host);
			if (uriHostNameType != UriHostNameType.Unknown)
			{
				return uriHostNameType;
			}
			return UriHostNameType.Basic;
		}
	}

	public bool IsDefaultPort
	{
		get
		{
			EnsureAbsoluteUri();
			return GetDefaultPort(Scheme) == port;
		}
	}

	public bool IsFile
	{
		get
		{
			EnsureAbsoluteUri();
			return Scheme == UriSchemeFile;
		}
	}

	public bool IsLoopback
	{
		get
		{
			EnsureAbsoluteUri();
			if (Host.Length == 0)
			{
				return false;
			}
			if (host == "loopback" || host == "localhost")
			{
				return true;
			}
			if (IPAddress.TryParse(host, out var address) && IPAddress.Loopback.Equals(address))
			{
				return true;
			}
			if (IPv6Address.TryParse(host, out var result) && IPv6Address.IsLoopback(result))
			{
				return true;
			}
			return false;
		}
	}

	public bool IsUnc
	{
		get
		{
			EnsureAbsoluteUri();
			return isUnc;
		}
	}

	public string LocalPath
	{
		get
		{
			EnsureAbsoluteUri();
			if (cachedLocalPath != null)
			{
				return cachedLocalPath;
			}
			if (!IsFile)
			{
				return AbsolutePath;
			}
			bool flag = path.Length > 3 && path[1] == ':' && (path[2] == '\\' || path[2] == '/');
			if (!IsUnc)
			{
				string text = Unescape(path);
				if (flag)
				{
					cachedLocalPath = text.Replace('/', '\\');
				}
				else
				{
					cachedLocalPath = text;
				}
			}
			else if (path.Length > 1 && path[1] == ':')
			{
				cachedLocalPath = Unescape(path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar));
			}
			else if (Path.DirectorySeparatorChar == '\\')
			{
				string text2 = host;
				if (path.Length > 0)
				{
					text2 += path.Replace('/', '\\');
				}
				cachedLocalPath = "\\\\" + Unescape(text2);
			}
			else
			{
				cachedLocalPath = Unescape(path);
			}
			if (cachedLocalPath.Length == 0)
			{
				cachedLocalPath = Path.DirectorySeparatorChar.ToString();
			}
			return cachedLocalPath;
		}
	}

	public string PathAndQuery
	{
		get
		{
			EnsureAbsoluteUri();
			return path + Query;
		}
	}

	public int Port
	{
		get
		{
			EnsureAbsoluteUri();
			return port;
		}
	}

	public string Query
	{
		get
		{
			EnsureAbsoluteUri();
			return query;
		}
	}

	public string Scheme
	{
		get
		{
			EnsureAbsoluteUri();
			return scheme;
		}
	}

	public string[] Segments
	{
		get
		{
			EnsureAbsoluteUri();
			if (segments != null)
			{
				return segments;
			}
			if (path.Length == 0)
			{
				segments = new string[0];
				return segments;
			}
			string[] array = (segments = path.Split('/'));
			bool flag = path.EndsWith("/");
			if (array.Length > 0 && flag)
			{
				string[] array2 = new string[array.Length - 1];
				Array.Copy(array, 0, array2, 0, array.Length - 1);
				array = array2;
			}
			int i = 0;
			if (IsFile && path.Length > 1 && path[1] == ':')
			{
				string[] array3 = new string[array.Length + 1];
				Array.Copy(array, 1, array3, 2, array.Length - 1);
				array = array3;
				array[0] = path.Substring(0, 2);
				array[1] = string.Empty;
				i++;
			}
			for (int num = array.Length; i < num; i++)
			{
				if (i != num - 1 || flag)
				{
					array[i] += '/';
				}
			}
			segments = array;
			return segments;
		}
	}

	public bool UserEscaped => userEscaped;

	public string UserInfo
	{
		get
		{
			EnsureAbsoluteUri();
			return userinfo;
		}
	}

	[MonoTODO("add support for IPv6 address")]
	public string DnsSafeHost
	{
		get
		{
			EnsureAbsoluteUri();
			return Unescape(Host);
		}
	}

	public bool IsAbsoluteUri => isAbsoluteUri;

	internal string OriginalString => (source == null) ? ToString() : source;

	public Uri(string uriString)
		: this(uriString, dontEscape: false)
	{
	}

	protected Uri(SerializationInfo serializationInfo, StreamingContext streamingContext)
		: this(serializationInfo.GetString("AbsoluteUri"), dontEscape: true)
	{
	}

	public Uri(string uriString, bool dontEscape)
	{
		userEscaped = dontEscape;
		source = uriString;
		Parse();
		if (!isAbsoluteUri)
		{
			throw new UriFormatException("Invalid URI: The format of the URI could not be determined.");
		}
	}

	public Uri(Uri baseUri, string relativeUri)
	{
		Merge(baseUri, relativeUri);
	}

	public Uri(Uri baseUri, string relativeUri, bool dontEscape)
	{
		userEscaped = dontEscape;
		Merge(baseUri, relativeUri);
	}

	void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("AbsoluteUri", AbsoluteUri);
	}

	private void Merge(Uri baseUri, string relativeUri)
	{
		if (baseUri == null)
		{
			throw new NullReferenceException("baseUri");
		}
		if (relativeUri.Length >= 2 && relativeUri[0] == '\\' && relativeUri[1] == '\\')
		{
			source = relativeUri;
			Parse();
			return;
		}
		int num = relativeUri.IndexOf(':');
		if (num != -1)
		{
			int num2 = relativeUri.IndexOfAny(new char[3] { '/', '\\', '?' });
			if (num2 > num || num2 < 0)
			{
				if (string.CompareOrdinal(baseUri.Scheme, 0, relativeUri, 0, num) != 0 || !IsPredefinedScheme(baseUri.Scheme) || (relativeUri.Length > num + 1 && relativeUri[num + 1] == '/'))
				{
					source = relativeUri;
					Parse();
					return;
				}
				relativeUri = relativeUri.Substring(num + 1);
			}
		}
		scheme = baseUri.scheme;
		host = baseUri.host;
		port = baseUri.port;
		userinfo = baseUri.userinfo;
		isUnc = baseUri.isUnc;
		isUnixFilePath = baseUri.isUnixFilePath;
		isOpaquePart = baseUri.isOpaquePart;
		if (relativeUri == string.Empty)
		{
			path = baseUri.path;
			query = baseUri.query;
			fragment = baseUri.fragment;
			return;
		}
		num = relativeUri.IndexOf('#');
		if (num != -1)
		{
			if (userEscaped)
			{
				fragment = relativeUri.Substring(num);
			}
			else
			{
				fragment = "#" + EscapeString(relativeUri.Substring(num + 1));
			}
			relativeUri = relativeUri.Substring(0, num);
		}
		num = relativeUri.IndexOf('?');
		if (num != -1)
		{
			query = relativeUri.Substring(num);
			if (!userEscaped)
			{
				query = EscapeString(query);
			}
			relativeUri = relativeUri.Substring(0, num);
		}
		if (relativeUri.Length > 0 && relativeUri[0] == '/')
		{
			if (relativeUri.Length > 1 && relativeUri[1] == '/')
			{
				source = scheme + ':' + relativeUri;
				Parse();
				return;
			}
			path = relativeUri;
			if (!userEscaped)
			{
				path = EscapeString(path);
			}
			return;
		}
		path = baseUri.path;
		if (relativeUri.Length > 0 || query.Length > 0)
		{
			num = path.LastIndexOf('/');
			if (num >= 0)
			{
				path = path.Substring(0, num + 1);
			}
		}
		if (relativeUri.Length == 0)
		{
			return;
		}
		path += relativeUri;
		int startIndex = 0;
		while (true)
		{
			num = path.IndexOf("./", startIndex);
			switch (num)
			{
			case 0:
				path = path.Remove(0, 2);
				break;
			default:
				if (path[num - 1] != '.')
				{
					path = path.Remove(num, 2);
				}
				else
				{
					startIndex = num + 1;
				}
				break;
			case -1:
				if (path.Length > 1 && path[path.Length - 1] == '.' && path[path.Length - 2] == '/')
				{
					path = path.Remove(path.Length - 1, 1);
				}
				startIndex = 0;
				while (true)
				{
					num = path.IndexOf("/../", startIndex);
					switch (num)
					{
					case 0:
						startIndex = 3;
						break;
					default:
					{
						int num3 = path.LastIndexOf('/', num - 1);
						if (num3 == -1)
						{
							startIndex = num + 1;
						}
						else if (path.Substring(num3 + 1, num - num3 - 1) != "..")
						{
							path = path.Remove(num3 + 1, num - num3 + 3);
						}
						else
						{
							startIndex = num + 1;
						}
						break;
					}
					case -1:
						if (path.Length > 3 && path.EndsWith("/.."))
						{
							num = path.LastIndexOf('/', path.Length - 4);
							if (num != -1 && path.Substring(num + 1, path.Length - num - 4) != "..")
							{
								path = path.Remove(num + 1, path.Length - num - 1);
							}
						}
						if (!userEscaped)
						{
							path = EscapeString(path);
						}
						return;
					}
				}
			}
		}
	}

	public static UriHostNameType CheckHostName(string name)
	{
		if (name == null || name.Length == 0)
		{
			return UriHostNameType.Unknown;
		}
		if (IsIPv4Address(name))
		{
			return UriHostNameType.IPv4;
		}
		if (IsDomainAddress(name))
		{
			return UriHostNameType.Dns;
		}
		if (IPv6Address.TryParse(name, out var _))
		{
			return UriHostNameType.IPv6;
		}
		return UriHostNameType.Unknown;
	}

	internal static bool IsIPv4Address(string name)
	{
		string[] array = name.Split('.');
		if (array.Length != 4)
		{
			return false;
		}
		for (int i = 0; i < 4; i++)
		{
			if (array[i].Length == 0)
			{
				return false;
			}
			uint num;
			try
			{
				num = uint.Parse(array[i]);
			}
			catch (Exception)
			{
				return false;
			}
			if (num > 255)
			{
				return false;
			}
		}
		return true;
	}

	internal static bool IsDomainAddress(string name)
	{
		int length = name.Length;
		int num = 0;
		for (int i = 0; i < length; i++)
		{
			char c = name[i];
			if (num == 0)
			{
				if (!char.IsLetterOrDigit(c))
				{
					return false;
				}
			}
			else if (c == '.')
			{
				num = 0;
			}
			else if (!char.IsLetterOrDigit(c) && c != '-' && c != '_')
			{
				return false;
			}
			if (++num == 64)
			{
				return false;
			}
		}
		return true;
	}

	protected virtual void Canonicalize()
	{
	}

	[MonoTODO("Find out what this should do")]
	protected virtual void CheckSecurity()
	{
	}

	public static bool CheckSchemeName(string schemeName)
	{
		if (schemeName == null || schemeName.Length == 0)
		{
			return false;
		}
		if (!IsAlpha(schemeName[0]))
		{
			return false;
		}
		int length = schemeName.Length;
		for (int i = 1; i < length; i++)
		{
			char c = schemeName[i];
			if (!char.IsDigit(c) && !IsAlpha(c) && c != '.' && c != '+' && c != '-')
			{
				return false;
			}
		}
		return true;
	}

	private static bool IsAlpha(char c)
	{
		return char.IsLetter(c);
	}

	public override bool Equals(object comparant)
	{
		if (comparant == null)
		{
			return false;
		}
		Uri uri = comparant as Uri;
		if (uri == null)
		{
			if (!(comparant is string uriString))
			{
				return false;
			}
			uri = new Uri(uriString);
		}
		return InternalEquals(uri);
	}

	private bool InternalEquals(Uri uri)
	{
		CultureInfo invariantCulture = CultureInfo.InvariantCulture;
		return scheme.ToLower(invariantCulture) == uri.scheme.ToLower(invariantCulture) && host.ToLower(invariantCulture) == uri.host.ToLower(invariantCulture) && port == uri.port && query.ToLower(invariantCulture) == uri.query.ToLower(invariantCulture) && path == uri.path;
	}

	public override int GetHashCode()
	{
		if (cachedHashCode == 0)
		{
			CultureInfo invariantCulture = CultureInfo.InvariantCulture;
			if (isAbsoluteUri)
			{
				cachedHashCode = scheme.ToLower(invariantCulture).GetHashCode() ^ host.ToLower(invariantCulture).GetHashCode() ^ port ^ query.ToLower(invariantCulture).GetHashCode() ^ path.GetHashCode();
			}
			else
			{
				cachedHashCode = source.GetHashCode();
			}
		}
		return cachedHashCode;
	}

	public string GetLeftPart(UriPartial part)
	{
		EnsureAbsoluteUri();
		switch (part)
		{
		case UriPartial.Scheme:
			return scheme + GetOpaqueWiseSchemeDelimiter();
		case UriPartial.Authority:
		{
			if (scheme == UriSchemeMailto || scheme == UriSchemeNews)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder2 = new StringBuilder();
			stringBuilder2.Append(scheme);
			stringBuilder2.Append(GetOpaqueWiseSchemeDelimiter());
			if (path.Length > 1 && path[1] == ':' && UriSchemeFile == scheme)
			{
				stringBuilder2.Append('/');
			}
			if (userinfo.Length > 0)
			{
				stringBuilder2.Append(userinfo).Append('@');
			}
			stringBuilder2.Append(host);
			int defaultPort = GetDefaultPort(scheme);
			if (port != -1 && port != defaultPort)
			{
				stringBuilder2.Append(':').Append(port);
			}
			return stringBuilder2.ToString();
		}
		case UriPartial.Path:
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(scheme);
			stringBuilder.Append(GetOpaqueWiseSchemeDelimiter());
			if (path.Length > 1 && path[1] == ':' && UriSchemeFile == scheme)
			{
				stringBuilder.Append('/');
			}
			if (userinfo.Length > 0)
			{
				stringBuilder.Append(userinfo).Append('@');
			}
			stringBuilder.Append(host);
			int defaultPort = GetDefaultPort(scheme);
			if (port != -1 && port != defaultPort)
			{
				stringBuilder.Append(':').Append(port);
			}
			if (path.Length > 0)
			{
				stringBuilder.Append(path);
			}
			return stringBuilder.ToString();
		}
		default:
			return null;
		}
	}

	public static int FromHex(char digit)
	{
		if ('0' <= digit && digit <= '9')
		{
			return digit - 48;
		}
		if ('a' <= digit && digit <= 'f')
		{
			return digit - 97 + 10;
		}
		if ('A' <= digit && digit <= 'F')
		{
			return digit - 65 + 10;
		}
		throw new ArgumentException("digit");
	}

	public static string HexEscape(char character)
	{
		if (character > 'ÿ')
		{
			throw new ArgumentOutOfRangeException("character");
		}
		return "%" + hexUpperChars[(character & 0xF0) >> 4] + hexUpperChars[character & 0xF];
	}

	public static char HexUnescape(string pattern, ref int index)
	{
		if (pattern == null)
		{
			throw new ArgumentException("pattern");
		}
		if (index < 0 || index >= pattern.Length)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		if (!IsHexEncoding(pattern, index))
		{
			return pattern[index++];
		}
		index++;
		int num = FromHex(pattern[index++]);
		int num2 = FromHex(pattern[index++]);
		return (char)((num << 4) | num2);
	}

	public static bool IsHexDigit(char digit)
	{
		return ('0' <= digit && digit <= '9') || ('a' <= digit && digit <= 'f') || ('A' <= digit && digit <= 'F');
	}

	public static bool IsHexEncoding(string pattern, int index)
	{
		if (index + 3 > pattern.Length)
		{
			return false;
		}
		return pattern[index++] == '%' && IsHexDigit(pattern[index++]) && IsHexDigit(pattern[index]);
	}

	public string MakeRelative(Uri toUri)
	{
		if (Scheme != toUri.Scheme || Authority != toUri.Authority)
		{
			return toUri.ToString();
		}
		string text = string.Empty;
		if (path != toUri.path)
		{
			string[] array = Segments;
			string[] array2 = toUri.Segments;
			int i = 0;
			for (int num = Math.Min(array.Length, array2.Length); i < num && !(array[i] != array2[i]); i++)
			{
			}
			for (int j = i + 1; j < array.Length; j++)
			{
				text += "../";
			}
			for (int k = i; k < array2.Length; k++)
			{
				text += array2[k];
			}
		}
		return text;
	}

	private void AppendQueryAndFragment(ref string result)
	{
		if (query.Length > 0)
		{
			string text = ((query[0] != '?') ? Unescape(query, excludeSpecial: false) : ('?' + Unescape(query.Substring(1), excludeSpecial: false)));
			result += text;
		}
		if (fragment.Length > 0)
		{
			result += fragment;
		}
	}

	public override string ToString()
	{
		if (cachedToString != null)
		{
			return cachedToString;
		}
		if (isAbsoluteUri)
		{
			cachedToString = Unescape(GetLeftPart(UriPartial.Path), excludeSpecial: true);
		}
		else
		{
			cachedToString = Unescape(path);
		}
		AppendQueryAndFragment(ref cachedToString);
		return cachedToString;
	}

	protected virtual void Escape()
	{
		path = EscapeString(path);
	}

	protected static string EscapeString(string str)
	{
		return EscapeString(str, escapeReserved: false, escapeHex: true, escapeBrackets: true);
	}

	internal static string EscapeString(string str, bool escapeReserved, bool escapeHex, bool escapeBrackets)
	{
		if (str == null)
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder();
		int length = str.Length;
		for (int i = 0; i < length; i++)
		{
			if (IsHexEncoding(str, i))
			{
				stringBuilder.Append(str.Substring(i, 3));
				i += 2;
				continue;
			}
			byte[] bytes = Encoding.UTF8.GetBytes(new char[1] { str[i] });
			int num = bytes.Length;
			for (int j = 0; j < num; j++)
			{
				char c = (char)bytes[j];
				if (c <= ' ' || c >= '\u007f' || "<>%\"{}|\\^`".IndexOf(c) != -1 || (escapeHex && c == '#') || (escapeBrackets && (c == '[' || c == ']')) || (escapeReserved && ";/?:@&=+$,".IndexOf(c) != -1))
				{
					stringBuilder.Append(HexEscape(c));
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
		}
		return stringBuilder.ToString();
	}

	protected virtual void Parse()
	{
		ParseUri(UriKind.Absolute);
	}

	private void ParseUri(UriKind kind)
	{
		Parse(kind, source);
		if (!userEscaped)
		{
			host = EscapeString(host, escapeReserved: false, escapeHex: true, escapeBrackets: false);
			if (host.Length > 1 && host[0] != '[' && host[host.Length - 1] != ']')
			{
				host = host.ToLower(CultureInfo.InvariantCulture);
			}
			if (path.Length > 0)
			{
				path = EscapeString(path);
			}
		}
	}

	protected virtual string Unescape(string str)
	{
		return Unescape(str, excludeSpecial: false);
	}

	internal static string Unescape(string str, bool excludeSpecial)
	{
		if (str == null)
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder();
		int length = str.Length;
		for (int i = 0; i < length; i++)
		{
			char c = str[i];
			if (c == '%')
			{
				char c2 = HexUnescapeMultiByte(str, ref i, out var surrogate);
				if (excludeSpecial && c2 == '#')
				{
					stringBuilder.Append("%23");
				}
				else if (excludeSpecial && c2 == '%')
				{
					stringBuilder.Append("%25");
				}
				else if (excludeSpecial && c2 == '?')
				{
					stringBuilder.Append("%3F");
				}
				else
				{
					stringBuilder.Append(c2);
					if (surrogate != '\0')
					{
						stringBuilder.Append(surrogate);
					}
				}
				i--;
			}
			else
			{
				stringBuilder.Append(c);
			}
		}
		return stringBuilder.ToString();
	}

	private void ParseAsWindowsUNC(string uriString)
	{
		scheme = UriSchemeFile;
		port = -1;
		fragment = string.Empty;
		query = string.Empty;
		isUnc = true;
		uriString = uriString.TrimStart('\\');
		int num = uriString.IndexOf('\\');
		if (num > 0)
		{
			path = uriString.Substring(num);
			host = uriString.Substring(0, num);
		}
		else
		{
			host = uriString;
			path = string.Empty;
		}
		path = path.Replace("\\", "/");
	}

	private string ParseAsWindowsAbsoluteFilePath(string uriString)
	{
		if (uriString.Length > 2 && uriString[2] != '\\' && uriString[2] != '/')
		{
			return "Relative file path is not allowed.";
		}
		scheme = UriSchemeFile;
		host = string.Empty;
		port = -1;
		path = uriString.Replace("\\", "/");
		fragment = string.Empty;
		query = string.Empty;
		return null;
	}

	private void ParseAsUnixAbsoluteFilePath(string uriString)
	{
		isUnixFilePath = true;
		scheme = UriSchemeFile;
		port = -1;
		fragment = string.Empty;
		query = string.Empty;
		host = string.Empty;
		path = null;
		if (uriString.Length >= 2 && uriString[0] == '/' && uriString[1] == '/')
		{
			uriString = uriString.TrimStart('/');
			path = '/' + uriString;
		}
		if (path == null)
		{
			path = uriString;
		}
	}

	private void Parse(UriKind kind, string uriString)
	{
		if (uriString == null)
		{
			throw new ArgumentNullException("uriString");
		}
		string text = ParseNoExceptions(kind, uriString);
		if (text != null)
		{
			throw new UriFormatException(text);
		}
	}

	private string ParseNoExceptions(UriKind kind, string uriString)
	{
		uriString = uriString.Trim();
		int length = uriString.Length;
		if (length == 0 && (kind == UriKind.Relative || kind == UriKind.RelativeOrAbsolute))
		{
			isAbsoluteUri = false;
			return null;
		}
		if (length <= 1 && kind != UriKind.Relative)
		{
			return "Absolute URI is too short";
		}
		int num = 0;
		num = uriString.IndexOf(':');
		if (num == 0)
		{
			return "Invalid URI: The format of the URI could not be determined.";
		}
		if (num < 0)
		{
			if (uriString[0] == '/' && Path.DirectorySeparatorChar == '/')
			{
				ParseAsUnixAbsoluteFilePath(uriString);
				if (kind == UriKind.Relative)
				{
					isAbsoluteUri = false;
				}
			}
			else if (uriString.Length >= 2 && uriString[0] == '\\' && uriString[1] == '\\')
			{
				ParseAsWindowsUNC(uriString);
			}
			else
			{
				isAbsoluteUri = false;
				path = uriString;
			}
			return null;
		}
		if (num == 1)
		{
			if (!IsAlpha(uriString[0]))
			{
				return "URI scheme must start with a letter.";
			}
			string text = ParseAsWindowsAbsoluteFilePath(uriString);
			if (text != null)
			{
				return text;
			}
			return null;
		}
		scheme = uriString.Substring(0, num).ToLower(CultureInfo.InvariantCulture);
		if (!CheckSchemeName(scheme))
		{
			return Locale.GetText("URI scheme must start with a letter and must consist of one of alphabet, digits, '+', '-' or '.' character.");
		}
		int i = num + 1;
		int num2 = uriString.Length;
		num = uriString.IndexOf('#', i);
		if (!IsUnc && num != -1)
		{
			if (userEscaped)
			{
				fragment = uriString.Substring(num);
			}
			else
			{
				fragment = "#" + EscapeString(uriString.Substring(num + 1));
			}
			num2 = num;
		}
		num = uriString.IndexOf('?', i, num2 - i);
		if (num != -1)
		{
			query = uriString.Substring(num, num2 - num);
			num2 = num;
			if (!userEscaped)
			{
				query = EscapeString(query);
			}
		}
		if (IsPredefinedScheme(scheme) && scheme != UriSchemeMailto && scheme != UriSchemeNews && (num2 - i < 2 || (num2 - i >= 2 && uriString[i] == '/' && uriString[i + 1] != '/')))
		{
			return "Invalid URI: The Authority/Host could not be parsed.";
		}
		bool flag = num2 - i >= 2 && uriString[i] == '/' && uriString[i + 1] == '/';
		bool flag2 = scheme == UriSchemeFile && flag && (num2 - i == 2 || uriString[i + 2] == '/');
		bool flag3 = false;
		if (flag)
		{
			if (kind == UriKind.Relative)
			{
				return "Absolute URI when we expected a relative one";
			}
			if (scheme != UriSchemeMailto && scheme != UriSchemeNews)
			{
				i += 2;
			}
			if (scheme == UriSchemeFile)
			{
				int num3 = 2;
				for (int j = i; j < num2 && uriString[j] == '/'; j++)
				{
					num3++;
				}
				if (num3 >= 4)
				{
					flag2 = false;
					for (; i < num2 && uriString[i] == '/'; i++)
					{
					}
				}
				else if (num3 >= 3)
				{
					i++;
				}
			}
			if (num2 - i > 1 && uriString[i + 1] == ':')
			{
				flag2 = false;
				flag3 = true;
			}
		}
		else if (!IsPredefinedScheme(scheme))
		{
			path = uriString.Substring(i, num2 - i);
			isOpaquePart = true;
			return null;
		}
		if (flag2)
		{
			num = -1;
		}
		else
		{
			num = uriString.IndexOf('/', i, num2 - i);
			if (num == -1 && flag3)
			{
				num = uriString.IndexOf('\\', i, num2 - i);
			}
		}
		if (num == -1)
		{
			if (scheme != UriSchemeMailto && scheme != UriSchemeNews)
			{
				path = "/";
			}
		}
		else
		{
			path = uriString.Substring(num, num2 - num);
			num2 = num;
		}
		num = uriString.IndexOf('@', i, num2 - i);
		if (num != -1)
		{
			userinfo = uriString.Substring(i, num - i);
			i = num + 1;
		}
		port = -1;
		num = ((!flag2) ? uriString.LastIndexOf(':', num2 - 1, num2 - i) : (-1));
		if (num != -1 && num != num2 - 1)
		{
			string text2 = uriString.Substring(num + 1, num2 - (num + 1));
			if (text2.Length > 0 && text2[text2.Length - 1] != ']')
			{
				try
				{
					port = (int)uint.Parse(text2, CultureInfo.InvariantCulture);
					num2 = num;
				}
				catch (Exception)
				{
					return "Invalid URI: Invalid port number";
				}
			}
			else if (port == -1)
			{
				port = GetDefaultPort(scheme);
			}
		}
		else if (port == -1)
		{
			port = GetDefaultPort(scheme);
		}
		uriString = uriString.Substring(i, num2 - i);
		host = uriString;
		if (flag2)
		{
			path = Reduce('/' + uriString);
			host = string.Empty;
		}
		else if (host.Length == 2 && host[1] == ':')
		{
			path = host + path;
			host = string.Empty;
		}
		else if (isUnixFilePath)
		{
			uriString = "//" + uriString;
			host = string.Empty;
		}
		else if (scheme == UriSchemeFile)
		{
			isUnc = true;
		}
		else if (scheme == UriSchemeNews)
		{
			if (host.Length > 0)
			{
				path = host;
				host = string.Empty;
			}
		}
		else if (host.Length == 0 && (scheme == UriSchemeHttp || scheme == UriSchemeGopher || scheme == UriSchemeNntp || scheme == UriSchemeHttps || scheme == UriSchemeFtp))
		{
			return "Invalid URI: The hostname could not be parsed";
		}
		bool flag4 = host.Length > 0 && CheckHostName(host) == UriHostNameType.Unknown;
		if (!flag4 && host.Length > 1 && host[0] == '[' && host[host.Length - 1] == ']')
		{
			if (IPv6Address.TryParse(host, out var result))
			{
				host = "[" + result.ToString(fullLength: true) + "]";
			}
			else
			{
				flag4 = true;
			}
		}
		if (flag4)
		{
			return Locale.GetText("Invalid URI: The hostname could not be parsed. (" + host + ")");
		}
		if (scheme != UriSchemeMailto && scheme != UriSchemeNews && scheme != UriSchemeFile)
		{
			path = Reduce(path);
		}
		return null;
	}

	private static string Reduce(string path)
	{
		if (path == "/")
		{
			return path;
		}
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < path.Length; i++)
		{
			char c = path[i];
			switch (c)
			{
			case '\\':
				stringBuilder.Append('/');
				break;
			case '%':
				if (i < path.Length - 2)
				{
					char c2 = path[i + 1];
					char c3 = char.ToUpper(path[i + 2]);
					if ((c2 == '2' && c3 == 'F') || (c2 == '5' && c3 == 'C'))
					{
						stringBuilder.Append('/');
						i += 2;
					}
					else
					{
						stringBuilder.Append(c);
					}
				}
				else
				{
					stringBuilder.Append(c);
				}
				break;
			default:
				stringBuilder.Append(c);
				break;
			}
		}
		path = stringBuilder.ToString();
		ArrayList arrayList = new ArrayList();
		int num = 0;
		while (num < path.Length)
		{
			int num2 = path.IndexOf('/', num);
			if (num2 == -1)
			{
				num2 = path.Length;
			}
			string text = path.Substring(num, num2 - num);
			num = num2 + 1;
			if (text.Length == 0 || text == ".")
			{
				continue;
			}
			if (text == "..")
			{
				int count = arrayList.Count;
				if (count > 0 && (string)arrayList[count - 1] != "..")
				{
					arrayList.RemoveAt(count - 1);
					continue;
				}
			}
			arrayList.Add(text);
		}
		if (arrayList.Count == 0)
		{
			return "/";
		}
		stringBuilder.Length = 0;
		if (path[0] == '/')
		{
			stringBuilder.Append('/');
		}
		bool flag = true;
		foreach (string item in arrayList)
		{
			if (flag)
			{
				flag = false;
			}
			else
			{
				stringBuilder.Append('/');
			}
			stringBuilder.Append(item);
		}
		if (path.EndsWith("/"))
		{
			stringBuilder.Append('/');
		}
		return stringBuilder.ToString();
	}

	private static char HexUnescapeMultiByte(string pattern, ref int index, out char surrogate)
	{
		surrogate = '\0';
		if (pattern == null)
		{
			throw new ArgumentException("pattern");
		}
		if (index < 0 || index >= pattern.Length)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		if (!IsHexEncoding(pattern, index))
		{
			return pattern[index++];
		}
		int num = index++;
		int num2 = FromHex(pattern[index++]);
		int num3 = FromHex(pattern[index++]);
		int num4 = num2;
		int num5 = 0;
		while ((num4 & 8) == 8)
		{
			num5++;
			num4 <<= 1;
		}
		if (num5 <= 1)
		{
			return (char)((num2 << 4) | num3);
		}
		byte[] array = new byte[num5];
		bool flag = false;
		array[0] = (byte)((num2 << 4) | num3);
		for (int i = 1; i < num5; i++)
		{
			if (!IsHexEncoding(pattern, index++))
			{
				flag = true;
				break;
			}
			int num6 = FromHex(pattern[index++]);
			if ((num6 & 0xC) != 8)
			{
				flag = true;
				break;
			}
			int num7 = FromHex(pattern[index++]);
			array[i] = (byte)((num6 << 4) | num7);
		}
		if (flag)
		{
			index = num + 3;
			return (char)array[0];
		}
		byte b = byte.MaxValue;
		b = (byte)(b >> num5 + 1);
		int num8 = array[0] & b;
		for (int j = 1; j < num5; j++)
		{
			num8 <<= 6;
			num8 |= array[j] & 0x3F;
		}
		if (num8 <= 65535)
		{
			return (char)num8;
		}
		num8 -= 65536;
		surrogate = (char)((num8 & 0x3FF) | 0xDC00);
		return (char)((num8 >> 10) | 0xD800);
	}

	internal static string GetSchemeDelimiter(string scheme)
	{
		for (int i = 0; i < schemes.Length; i++)
		{
			if (schemes[i].scheme == scheme)
			{
				return schemes[i].delimiter;
			}
		}
		return SchemeDelimiter;
	}

	internal static int GetDefaultPort(string scheme)
	{
		for (int i = 0; i < schemes.Length; i++)
		{
			if (schemes[i].scheme == scheme)
			{
				return schemes[i].defaultPort;
			}
		}
		return -1;
	}

	private string GetOpaqueWiseSchemeDelimiter()
	{
		if (isOpaquePart)
		{
			return ":";
		}
		return GetSchemeDelimiter(scheme);
	}

	protected virtual bool IsBadFileSystemCharacter(char ch)
	{
		if (ch < ' ' || (ch < '@' && ch > '9'))
		{
			return true;
		}
		switch ((int)ch)
		{
		case 0:
		case 34:
		case 38:
		case 42:
		case 44:
		case 47:
		case 92:
		case 94:
		case 124:
			return true;
		default:
			return false;
		}
	}

	protected static bool IsExcludedCharacter(char ch)
	{
		switch (ch)
		{
		default:
			return true;
		case '"':
		case '#':
		case '%':
		case '<':
		case '>':
		case '[':
		case '\\':
		case ']':
		case '^':
		case '`':
		case '{':
		case '|':
		case '}':
			return true;
		case '!':
		case '$':
		case '&':
		case '\'':
		case '(':
		case ')':
		case '*':
		case '+':
		case ',':
		case '-':
		case '.':
		case '/':
		case '0':
		case '1':
		case '2':
		case '3':
		case '4':
		case '5':
		case '6':
		case '7':
		case '8':
		case '9':
		case ':':
		case ';':
		case '=':
		case '?':
		case '@':
		case 'A':
		case 'B':
		case 'C':
		case 'D':
		case 'E':
		case 'F':
		case 'G':
		case 'H':
		case 'I':
		case 'J':
		case 'K':
		case 'L':
		case 'M':
		case 'N':
		case 'O':
		case 'P':
		case 'Q':
		case 'R':
		case 'S':
		case 'T':
		case 'U':
		case 'V':
		case 'W':
		case 'X':
		case 'Y':
		case 'Z':
		case '_':
		case 'a':
		case 'b':
		case 'c':
		case 'd':
		case 'e':
		case 'f':
		case 'g':
		case 'h':
		case 'i':
		case 'j':
		case 'k':
		case 'l':
		case 'm':
		case 'n':
		case 'o':
		case 'p':
		case 'q':
		case 'r':
		case 's':
		case 't':
		case 'u':
		case 'v':
		case 'w':
		case 'x':
		case 'y':
		case 'z':
		case '~':
			return false;
		}
	}

	internal static bool MaybeUri(string s)
	{
		int num = s.IndexOf(':');
		if (num == -1)
		{
			return false;
		}
		if (num >= 10)
		{
			return false;
		}
		return IsPredefinedScheme(s.Substring(0, num));
	}

	private static bool IsPredefinedScheme(string scheme)
	{
		switch (scheme)
		{
		case "http":
		case "https":
		case "file":
		case "ftp":
		case "nntp":
		case "gopher":
		case "mailto":
		case "news":
			return true;
		default:
			return false;
		}
	}

	protected virtual bool IsReservedCharacter(char ch)
	{
		if (ch == '$' || ch == '&' || ch == '+' || ch == ',' || ch == '/' || ch == ':' || ch == ';' || ch == '=' || ch == '@')
		{
			return true;
		}
		return false;
	}

	private void EnsureAbsoluteUri()
	{
	}
}
