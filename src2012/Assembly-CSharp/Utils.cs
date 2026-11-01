using System;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.BZip2;
using UnityEngine;

internal class Utils
{
	public static string MaxStringLength(string str, int length)
	{
		if (str.Length > length)
		{
			return str.Substring(0, length);
		}
		return str;
	}

	public static DateTime StringToDateTime(string str)
	{
		int num = Convert.ToInt32(str.Substring(0, 4));
		int num2 = Convert.ToInt32(str.Substring(5, 2));
		int num3 = Convert.ToInt32(str.Substring(8, 2));
		int hour = Convert.ToInt32(str.Substring(11, 2));
		int minute = Convert.ToInt32(str.Substring(14, 2));
		return new DateTime(Mathf.Clamp(num, 1, 9999), Mathf.Clamp(num2, 1, 12), Mathf.Clamp(num3, 1, 31), hour, minute, 0);
	}

	public static byte[] UnzipByte(byte[] compbytes, int new_length)
	{
		MemoryStream memoryStream = null;
		BZip2InputStream bZip2InputStream = null;
		try
		{
			memoryStream = new MemoryStream(compbytes);
			bZip2InputStream = new BZip2InputStream(memoryStream);
			byte[] array = new byte[new_length];
			bZip2InputStream.Read(array, 0, array.Length);
			bZip2InputStream.Close();
			memoryStream.Close();
			return array;
		}
		finally
		{
			bZip2InputStream?.Dispose();
			memoryStream?.Dispose();
		}
	}

	public static byte[] ZipByte(byte[] buffer)
	{
		MemoryStream memoryStream = null;
		BZip2OutputStream bZip2OutputStream = null;
		try
		{
			memoryStream = new MemoryStream();
			bZip2OutputStream = new BZip2OutputStream(memoryStream);
			bZip2OutputStream.Write(buffer, 0, buffer.Length);
			bZip2OutputStream.Close();
			byte[] result = memoryStream.ToArray();
			memoryStream.Close();
			return result;
		}
		finally
		{
			bZip2OutputStream?.Dispose();
			memoryStream?.Dispose();
		}
	}

	public static string ZipString(string sBuffer)
	{
		MemoryStream memoryStream = null;
		BZip2OutputStream bZip2OutputStream = null;
		try
		{
			memoryStream = new MemoryStream();
			int length = sBuffer.Length;
			using BinaryWriter binaryWriter = new BinaryWriter(memoryStream, Encoding.ASCII);
			binaryWriter.Write(length);
			bZip2OutputStream = new BZip2OutputStream(memoryStream);
			bZip2OutputStream.Write(Encoding.ASCII.GetBytes(sBuffer), 0, sBuffer.Length);
			bZip2OutputStream.Close();
			string result = Convert.ToBase64String(memoryStream.ToArray());
			memoryStream.Close();
			binaryWriter.Close();
			return result;
		}
		finally
		{
			bZip2OutputStream?.Dispose();
			memoryStream?.Dispose();
		}
	}

	public static string UnzipString(string compbytes)
	{
		MemoryStream memoryStream = null;
		BZip2InputStream bZip2InputStream = null;
		try
		{
			memoryStream = new MemoryStream(Convert.FromBase64String(compbytes));
			using BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.ASCII);
			int num = binaryReader.ReadInt32();
			bZip2InputStream = new BZip2InputStream(memoryStream);
			byte[] array = new byte[num];
			bZip2InputStream.Read(array, 0, array.Length);
			bZip2InputStream.Close();
			memoryStream.Close();
			string result = Encoding.ASCII.GetString(array);
			binaryReader.Close();
			return result;
		}
		finally
		{
			bZip2InputStream?.Dispose();
			memoryStream?.Dispose();
		}
	}

	public static void SetLayerRecursively(GameObject obj, int newLayer)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected Obj, but got Unknown
		if ((Object)null == (Object)(object)obj)
		{
			return;
		}
		obj.layer = newLayer;
		foreach (Transform item in obj.transform)
		{
			Transform val = item;
			if (!((Object)null == (Object)(object)val))
			{
				SetLayerRecursively(((Component)val).gameObject, newLayer);
			}
		}
	}

	public static void PlayerPrefsSetBool(string name, bool value)
	{
		PlayerPrefs.SetInt(name, value ? 1 : 0);
	}

	public static bool PlayerPrefsGetBool(string name)
	{
		return PlayerPrefs.GetInt(name) == 1;
	}

	public static bool PlayerPrefsGetBool(string name, bool defaultValue)
	{
		if (PlayerPrefs.HasKey(name))
		{
			return PlayerPrefsGetBool(name);
		}
		return defaultValue;
	}
}
