using System;
using NSpeex;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class VoiceChatPlayer : MonoBehaviour
{
	private float lastTime;

	private double played;

	private double received;

	private int index;

	private float[] data;

	private float playDelay;

	private bool shouldPlay;

	private float lastRecvTime;

	private SpeexDecoder speexDec = new SpeexDecoder(BandMode.Narrow);

	[SerializeField]
	private int playbackDelay = 2;

	public float LastRecvTime => lastRecvTime;

	private void Start()
	{
		int num = VoiceChatSettings.Instance.Frequency * 10;
		((Component)this).audio.loop = true;
		((Component)this).audio.clip = AudioClip.Create("VoiceChat", num, 1, VoiceChatSettings.Instance.Frequency, false, false);
		data = new float[num];
		if (VoiceChatSettings.Instance.LocalDebug)
		{
			VoiceChatRecorder.Instance.NewSample += OnNewSample;
		}
	}

	private void Update()
	{
		if (((Component)this).audio.isPlaying)
		{
			if (lastTime > ((Component)this).audio.time)
			{
				played += ((Component)this).audio.clip.length;
			}
			lastTime = ((Component)this).audio.time;
			if (played + (double)((Component)this).audio.time >= received)
			{
				Stop();
				shouldPlay = false;
			}
		}
		else if (shouldPlay)
		{
			playDelay -= Time.deltaTime;
			if (playDelay <= 0f)
			{
				((Component)this).audio.Play();
			}
		}
	}

	private void Stop()
	{
		((Component)this).audio.Stop();
		((Component)this).audio.time = 0f;
		index = 0;
		played = 0.0;
		received = 0.0;
		lastTime = 0f;
	}

	public void OnNewSample(VoiceChatPacket packet)
	{
		lastRecvTime = Time.time;
		float[] array = null;
		int num = VoiceChatUtils.Decompress(speexDec, packet, out array);
		received += VoiceChatSettings.Instance.SampleTime;
		Array.Copy(array, 0, data, index, num);
		index += num;
		if (index >= ((Component)this).audio.clip.samples)
		{
			index = 0;
		}
		((Component)this).audio.clip.SetData(data, 0);
		if (!((Component)this).audio.isPlaying)
		{
			shouldPlay = true;
			if (playDelay <= 0f)
			{
				playDelay = (float)VoiceChatSettings.Instance.SampleTime * (float)playbackDelay;
			}
		}
		VoiceChatFloatPool.Instance.Return(array);
	}
}
