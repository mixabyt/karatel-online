using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
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

	public static string RSAEncrypt(string message)
	{
		try
		{
			string xmlString = "<RSAKeyValue><Modulus>t3A/x4DoMsQNVdIWe0rYmBy98tIQz+2/M5eiqL7IxS2h7L3mHLZVOHeTzDWbBGLHIx8la1oWUXPGUr/31Mgbvuc8PDXW0Oh011LWUYYPgaFLKqwvnS7T8ehNYLNkHxEeiKrtMPHSp0n6IfChKAvUZbdPs9IjAr1kO2lVNJ5Ickc=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
			RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
			rSACryptoServiceProvider.FromXmlString(xmlString);
			return Convert.ToBase64String(rSACryptoServiceProvider.Encrypt(Encoding.UTF8.GetBytes(message), fOAEP: true));
		}
		catch (Exception ex)
		{
			Debug.Log((object)("Problem with RSA:\n" + ex.Message.ToString()));
			return string.Empty;
		}
	}

	public static string RSADecrypt(string cryptedMessage)
	{
		try
		{
			string xmlString = "<RSAKeyValue><Modulus>t3A/x4DoMsQNVdIWe0rYmBy98tIQz+2/M5eiqL7IxS2h7L3mHLZVOHeTzDWbBGLHIx8la1oWUXPGUr/31Mgbvuc8PDXW0Oh011LWUYYPgaFLKqwvnS7T8ehNYLNkHxEeiKrtMPHSp0n6IfChKAvUZbdPs9IjAr1kO2lVNJ5Ickc=</Modulus><Exponent>AQAB</Exponent><P>6Ww86GpbXesa3Ja43qW4f41C9Wvga+1NfhLHL7eAVk54ktXL1hnFcMUAVEkZ6gc5eSc0FErJ9LeRVZl+L8EA3w==</P><Q>yS5afQ2STt2M12W0X4+WjIL5eFcEIT1mCbuwxKfC+uyu9YwPGmyZ31qblGQB1v2BRx5Y4+nPutGnq97giw2zmQ==</Q><DP>dSpfaEn6kqnwUclWAmwsr9m4QnhjrcjvnNjQIqN5R9kbrJikOFO3R2ObRrNqzo3Ry35iJc1kkfGSgeUJ8e5vAQ==</DP><DQ>BrBe29eOrMaa/zJne+HnTIaySrkB7yT+doZiCzfUuMkTzCA7BFLmDtIRaC6nDQHiwX2V0QYeHQUJgSJo7tamyQ==</DQ><InverseQ>oR1DCzYfxCp47qvgv8NfR11/CbjCkWhB+IJNbA2NTsYfidMyP8l9Z1XQXhNWquwuWvI9fe8kF8Vb0MCC4i/cbA==</InverseQ><D>L9ihaiIfbo5ghBAGOSZulE565QkRxYpXotdcX/NTXsrmP1/Q05BjbhUSgeNBicVR98E5+2RGCfzOr+rOc7u2MIW328tV1KNN6CXg44AyU5tFAbtalFXmVRTKRV79n2vPnRhri4VPJj7vVqJgb4xb2lYRWcziOJialWgN2+Q3buE=</D></RSAKeyValue>";
			RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
			rSACryptoServiceProvider.FromXmlString(xmlString);
			byte[] bytes = rSACryptoServiceProvider.Decrypt(Convert.FromBase64String(cryptedMessage), fOAEP: true);
			return Encoding.UTF8.GetString(bytes);
		}
		catch (Exception ex)
		{
			Debug.Log((object)("Problem with RSA:\n" + ex.Message.ToString()));
			return string.Empty;
		}
	}

	public static Vector3 ScreenToNGUIPos(Vector3 pos)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		float num = 1f / UIRoot.list[0].pixelSizeAdjustment;
		return new Vector3((pos.x - (float)(Screen.width / 2)) / num, (pos.y - (float)(Screen.height / 2)) / num, 1f);
	}

	public static void Shuffle(IList list)
	{
		RNGCryptoServiceProvider rNGCryptoServiceProvider = new RNGCryptoServiceProvider();
		int num = list.Count;
		while (num > 1)
		{
			byte[] array = new byte[1];
			do
			{
				rNGCryptoServiceProvider.GetBytes(array);
			}
			while (array[0] >= num * (255 / num));
			int index = array[0] % num;
			num--;
			object value = list[index];
			list[index] = list[num];
			list[num] = value;
		}
	}

	public static void SetActiveRecursively(GameObject go, bool enable)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected Obj, but got Unknown
		go.SetActive(enable);
		foreach (Transform item in go.transform)
		{
			Transform val = item;
			SetActiveRecursively(((Component)val).gameObject, enable);
		}
	}

	public static void SetActiveRecursively(Transform node, bool enable)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected Obj, but got Unknown
		((Component)node).gameObject.SetActive(enable);
		foreach (Transform item in node)
		{
			Transform node2 = item;
			SetActiveRecursively(node2, enable);
		}
	}

	public static void UpdateTextures(Transform node, List<Texture2D> textures)
	{
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Expected Obj, but got Unknown
		if ((Object)(object)node == (Object)null)
		{
			return;
		}
		if ((Object)(object)((Component)node).renderer != (Object)null)
		{
			foreach (Texture2D texture in textures)
			{
				Material[] materials = ((Component)node).renderer.materials;
				foreach (Material val in materials)
				{
					if ((Object)(object)val.mainTexture != (Object)null && ((Object)val.mainTexture).name == ((Object)texture).name)
					{
						val.mainTexture = (Texture)(object)texture;
					}
				}
			}
		}
		foreach (Transform item in node)
		{
			Transform node2 = item;
			UpdateTextures(node2, textures);
		}
	}
}
