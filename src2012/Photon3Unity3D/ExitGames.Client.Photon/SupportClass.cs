#define DEBUG
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ExitGames.Client.Photon;

public class SupportClass
{
	public class ThreadSafeRandom
	{
		private static Random _r = new Random();

		public static int Next()
		{
			lock (_r)
			{
				return _r.Next();
			}
		}
	}

	public static int GetTickCount()
	{
		return (int)(DateTime.Now.Ticks / 10000);
	}

	public static void WriteStackTrace(Exception throwable, TextWriter stream = null)
	{
		if (stream != null)
		{
			stream.WriteLine(throwable.ToString());
			stream.WriteLine(throwable.StackTrace);
			stream.Flush();
		}
		else
		{
			Debug.WriteLine(throwable.ToString());
			Debug.WriteLine(throwable.StackTrace);
		}
	}

	public static string DictionaryToString(IDictionary dictionary)
	{
		return DictionaryToString(dictionary, includeTypes: true);
	}

	public static string DictionaryToString(IDictionary dictionary, bool includeTypes)
	{
		if (dictionary == null)
		{
			return "null";
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("{");
		foreach (object key in dictionary.Keys)
		{
			if (stringBuilder.Length > 1)
			{
				stringBuilder.Append(", ");
			}
			Type type;
			string text;
			if (dictionary[key] == null)
			{
				type = typeof(object);
				text = "null";
			}
			else
			{
				type = dictionary[key].GetType();
				text = dictionary[key].ToString();
			}
			if (typeof(IDictionary) == type || typeof(Hashtable) == type)
			{
				text = DictionaryToString((IDictionary)dictionary[key]);
			}
			if (typeof(string[]) == type)
			{
				text = string.Format("{{{0}}}", string.Join(",", (string[])dictionary[key]));
			}
			if (includeTypes)
			{
				stringBuilder.AppendFormat("({0}){1}=({2}){3}", key.GetType().Name, key, type.Name, text);
			}
			else
			{
				stringBuilder.AppendFormat("{0}={1}", key, text);
			}
		}
		stringBuilder.Append("}");
		return stringBuilder.ToString();
	}

	[Obsolete("Use DictionaryToString() instead.")]
	public static string HashtableToString(Hashtable hash)
	{
		return DictionaryToString(hash);
	}

	[Obsolete("Use Protocol.Serialize() instead.")]
	public static void NumberToByteArray(byte[] buffer, int index, short number)
	{
		Protocol.Serialize(number, buffer, ref index);
	}

	[Obsolete("Use Protocol.Serialize() instead.")]
	public static void NumberToByteArray(byte[] buffer, int index, int number)
	{
		Protocol.Serialize(number, buffer, ref index);
	}

	public static string ByteArrayToString(byte[] list)
	{
		if (list == null)
		{
			return string.Empty;
		}
		return BitConverter.ToString(list);
	}
}
