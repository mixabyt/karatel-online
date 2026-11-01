using System;
using System.IO;
using Ionic.Zlib;
using NAudio.Codecs;
using NSpeex;
using UnityEngine;

public static class VoiceChatUtils
{
	private static SpeexEncoder speexEnc = new SpeexEncoder(BandMode.Narrow);

	private static void ToShortArray(this float[] input, short[] output)
	{
		if (output.Length < input.Length)
		{
			throw new ArgumentException("in: " + input.Length + ", out: " + output.Length);
		}
		for (int i = 0; i < input.Length; i++)
		{
			output[i] = (short)Mathf.Clamp((int)(input[i] * 32767f), -32768, 32767);
		}
	}

	private static void ToFloatArray(this short[] input, float[] output, int length)
	{
		if (output.Length < length || input.Length < length)
		{
			throw new ArgumentException();
		}
		for (int i = 0; i < length; i++)
		{
			output[i] = (float)input[i] / 32767f;
		}
	}

	private static byte[] ZlibCompress(byte[] input, int length)
	{
		using MemoryStream memoryStream = new MemoryStream();
		using (ZlibStream zlibStream = new ZlibStream(memoryStream, CompressionMode.Compress, CompressionLevel.BestCompression))
		{
			zlibStream.Write(input, 0, length);
		}
		return memoryStream.ToArray();
	}

	private static byte[] ZlibDecompress(byte[] input, int length)
	{
		using MemoryStream memoryStream = new MemoryStream();
		using (ZlibStream zlibStream = new ZlibStream(memoryStream, CompressionMode.Decompress, CompressionLevel.BestCompression))
		{
			zlibStream.Write(input, 0, length);
		}
		return memoryStream.ToArray();
	}

	private static byte[] ALawCompress(float[] input)
	{
		byte[] array = VoiceChatBytePool.Instance.Get();
		for (int i = 0; i < input.Length; i++)
		{
			int num = (int)(input[i] * 32767f);
			short sample = (short)Mathf.Clamp(num, -32768, 32767);
			array[i] = ALawEncoder.LinearToALawSample(sample);
		}
		return array;
	}

	private static float[] ALawDecompress(byte[] input, int length)
	{
		float[] array = VoiceChatFloatPool.Instance.Get();
		for (int i = 0; i < length; i++)
		{
			short num = ALawDecoder.ALawToLinearSample(input[i]);
			array[i] = (float)num / 32767f;
		}
		return array;
	}

	private static byte[] SpeexCompress(float[] input, out int length)
	{
		short[] array = VoiceChatShortPool.Instance.Get();
		byte[] array2 = VoiceChatBytePool.Instance.Get();
		input.ToShortArray(array);
		length = speexEnc.Encode(array, 0, input.Length, array2, 0, array2.Length);
		VoiceChatShortPool.Instance.Return(array);
		return array2;
	}

	private static float[] SpeexDecompress(SpeexDecoder speexDec, byte[] data, int dataLength)
	{
		float[] array = VoiceChatFloatPool.Instance.Get();
		short[] array2 = VoiceChatShortPool.Instance.Get();
		speexDec.Decode(data, 0, dataLength, array2, 0, lostFrame: false);
		array2.ToFloatArray(array, array2.Length);
		VoiceChatShortPool.Instance.Return(array2);
		return array;
	}

	public static VoiceChatNetworkProxy CreateProxy()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (!Network.isClient)
		{
			Debug.LogError((object)"You're not a client in the unity networking");
			return null;
		}
		Object val = Resources.Load("VoiceChat_NetworkProxy");
		GameObject val2 = (GameObject)(object)((val is GameObject) ? val : null);
		Object val3 = Network.Instantiate((Object)(object)val2, Vector3.zero, Quaternion.identity, 0);
		GameObject val4 = (GameObject)(object)((val3 is GameObject) ? val3 : null);
		return val4.GetComponent<VoiceChatNetworkProxy>();
	}

	public static VoiceChatPacket Compress(float[] sample)
	{
		VoiceChatPacket result = new VoiceChatPacket
		{
			Compression = VoiceChatSettings.Instance.Compression
		};
		switch (result.Compression)
		{
		case VoiceChatCompression.Alaw:
			result.Length = sample.Length;
			result.Data = ALawCompress(sample);
			break;
		case VoiceChatCompression.AlawZlib:
		{
			byte[] array = ALawCompress(sample);
			result.Data = ZlibCompress(array, sample.Length);
			result.Length = result.Data.Length;
			VoiceChatBytePool.Instance.Return(array);
			break;
		}
		case VoiceChatCompression.Speex:
			result.Data = SpeexCompress(sample, out result.Length);
			break;
		}
		return result;
	}

	public static int Decompress(VoiceChatPacket packet, out float[] data)
	{
		return Decompress(null, packet, out data);
	}

	public static int Decompress(SpeexDecoder speexDecoder, VoiceChatPacket packet, out float[] data)
	{
		switch (packet.Compression)
		{
		case VoiceChatCompression.Speex:
			data = SpeexDecompress(speexDecoder, packet.Data, packet.Length);
			return data.Length;
		case VoiceChatCompression.Alaw:
			data = ALawDecompress(packet.Data, packet.Length);
			return packet.Length;
		case VoiceChatCompression.AlawZlib:
		{
			byte[] array = ZlibDecompress(packet.Data, packet.Length);
			data = ALawDecompress(array, array.Length);
			return array.Length;
		}
		default:
			data = new float[0];
			return 0;
		}
	}

	public static int ClosestPowerOfTwo(int value)
	{
		int num;
		for (num = 1; num < value; num <<= 1)
		{
		}
		return num;
	}
}
