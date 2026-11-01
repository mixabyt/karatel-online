using System;
using System.Linq;
using Exocortex.DSP;
using UnityEngine;

public class VoiceChatRecorder : MonoBehaviour
{
	private static VoiceChatRecorder instance;

	[SerializeField]
	private KeyCode toggleToTalkKey = (KeyCode)111;

	[SerializeField]
	private KeyCode pushToTalkKey = (KeyCode)112;

	[SerializeField]
	private bool autoDetectSpeaking;

	[SerializeField]
	private int autoDetectIndex = 4;

	[SerializeField]
	private float forceTransmitTime = 2f;

	private int previousPosition;

	private int sampleIndex;

	private string device;

	private AudioClip clip;

	private bool transmitToggled;

	private bool recording;

	private float forceTransmit;

	private int recordFrequency;

	private int recordSampleSize;

	private int targetFrequency;

	private int targetSampleSize;

	private float[] fftBuffer;

	private float[] sampleBuffer;

	private VoiceChatCircularBuffer<float[]> previousSampleBuffer = new VoiceChatCircularBuffer<float[]>(5);

	public static VoiceChatRecorder Instance
	{
		get
		{
			if ((Object)(object)instance == (Object)null)
			{
				instance = Object.FindObjectOfType(typeof(VoiceChatRecorder)) as VoiceChatRecorder;
			}
			return instance;
		}
	}

	public KeyCode PushToTalkKey
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return pushToTalkKey;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			pushToTalkKey = value;
		}
	}

	public KeyCode ToggleToTalkKey
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return toggleToTalkKey;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			toggleToTalkKey = value;
		}
	}

	public bool AutoDetectSpeech
	{
		get
		{
			return autoDetectSpeaking;
		}
		set
		{
			autoDetectSpeaking = value;
		}
	}

	public int NetworkId { get; set; }

	public string Device
	{
		get
		{
			return device;
		}
		set
		{
			if (value != null && !Microphone.devices.Contains(value))
			{
				Debug.LogError((object)(value + " is not a valid microphone device"));
			}
			else
			{
				device = value;
			}
		}
	}

	public bool HasDefaultDevice => device == null;

	public bool HasSpecificDevice => device != null;

	public bool IsTransmitting
	{
		get
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			return transmitToggled || forceTransmit > 0f || Input.GetKey(pushToTalkKey);
		}
	}

	public bool IsRecording => recording;

	public string[] AvailableDevices => Microphone.devices;

	public event Action<VoiceChatPacket> NewSample;

	public VoiceChatRecorder()
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
	}

	private void Start()
	{
		if ((Object)(object)instance != (Object)null && (Object)(object)instance != (Object)(object)this)
		{
			Object.Destroy((Object)(object)this);
			Debug.LogError((object)"Only one instance of VoiceChatRecorder can exist");
		}
		else
		{
			NetworkId = -1;
			instance = this;
		}
	}

	private void OnEnable()
	{
		if ((Object)(object)instance != (Object)null && (Object)(object)instance != (Object)(object)this)
		{
			Object.Destroy((Object)(object)this);
			Debug.LogError((object)"Only one instance of VoiceChatRecorder can exist");
		}
		else
		{
			instance = this;
		}
	}

	private void OnDisable()
	{
		instance = null;
	}

	private void OnDestroy()
	{
		instance = null;
	}

	private void Update()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		pushToTalkKey = (KeyCode)((!Chat.IsEnabled()) ? 112 : 0);
		if (!recording)
		{
			return;
		}
		forceTransmit -= Time.deltaTime;
		if (Input.GetKeyUp(toggleToTalkKey))
		{
			transmitToggled = !transmitToggled;
		}
		bool transmit = transmitToggled || Input.GetKey(pushToTalkKey);
		int position = Microphone.GetPosition(Device);
		if (position < previousPosition)
		{
			while (sampleIndex < recordFrequency)
			{
				ReadSample(transmit);
			}
			sampleIndex = 0;
		}
		previousPosition = position;
		while (sampleIndex + recordSampleSize <= position)
		{
			ReadSample(transmit);
		}
	}

	private void Resample(float[] src, float[] dst)
	{
		if (src.Length == dst.Length)
		{
			Array.Copy(src, 0, dst, 0, src.Length);
			return;
		}
		float num = 1f / (float)dst.Length;
		for (int i = 0; i < dst.Length; i++)
		{
			float num2 = num * (float)i * (float)src.Length;
			dst[i] = src[(int)num2];
		}
	}

	private void ReadSample(bool transmit)
	{
		clip.GetData(sampleBuffer, sampleIndex);
		float[] array = VoiceChatFloatPool.Instance.Get();
		Resample(sampleBuffer, array);
		sampleIndex += recordSampleSize;
		float num = float.MinValue;
		int num2 = -1;
		if (autoDetectSpeaking && !transmit)
		{
			for (int i = 0; i < fftBuffer.Length; i++)
			{
				fftBuffer[i] = 0f;
			}
			Array.Copy(array, 0, fftBuffer, 0, array.Length);
			Fourier.FFT(fftBuffer, fftBuffer.Length / 2, FourierDirection.Forward);
			for (int j = 0; j < fftBuffer.Length; j++)
			{
				if (fftBuffer[j] > num)
				{
					num = fftBuffer[j];
					num2 = j;
				}
			}
		}
		if (NewSample != null && (transmit || forceTransmit > 0f || num2 >= autoDetectIndex))
		{
			if (num2 >= autoDetectIndex)
			{
				if (forceTransmit <= 0f)
				{
					while (previousSampleBuffer.Count > 0)
					{
						TransmitBuffer(previousSampleBuffer.Remove());
					}
				}
				forceTransmit = forceTransmitTime;
			}
			TransmitBuffer(array);
		}
		else
		{
			if (previousSampleBuffer.Count == previousSampleBuffer.Capacity)
			{
				VoiceChatFloatPool.Instance.Return(previousSampleBuffer.Remove());
			}
			previousSampleBuffer.Add(array);
		}
	}

	private void TransmitBuffer(float[] buffer)
	{
		VoiceChatPacket obj = VoiceChatUtils.Compress(buffer);
		obj.NetworkId = NetworkId;
		NewSample(obj);
	}

	public bool StartRecording()
	{
		if (NetworkId == -1 && !VoiceChatSettings.Instance.LocalDebug)
		{
			Debug.LogError((object)"NetworkId is -1");
			return false;
		}
		if (recording)
		{
			Debug.LogError((object)"Already recording");
			return false;
		}
		targetFrequency = VoiceChatSettings.Instance.Frequency;
		targetSampleSize = VoiceChatSettings.Instance.SampleSize;
		int num = default;
		int num2 = default;
		Microphone.GetDeviceCaps(Device, ref num, ref num2);
		recordFrequency = ((num != 0 || num2 != 0) ? num2 : 44100);
		recordSampleSize = recordFrequency / (targetFrequency / targetSampleSize);
		clip = Microphone.Start(Device, true, 1, recordFrequency);
		sampleBuffer = new float[recordSampleSize];
		fftBuffer = new float[VoiceChatUtils.ClosestPowerOfTwo(targetSampleSize)];
		recording = true;
		return recording;
	}

	public void StopRecording()
	{
		clip = null;
		recording = false;
	}
}
