using UnityEngine;

[AddComponentMenu("NGUI/Tween/Volume")]
public class TweenVolume : UITweener
{
	public float from;

	public float to = 1f;

	private AudioSource mSource;

	public AudioSource audioSource
	{
		get
		{
			if ((Object)(object)mSource == (Object)null)
			{
				mSource = ((Component)this).audio;
				if ((Object)(object)mSource == (Object)null)
				{
					mSource = ((Component)this).GetComponentInChildren<AudioSource>();
					if ((Object)(object)mSource == (Object)null)
					{
						Debug.LogError((object)"TweenVolume needs an AudioSource to work with", (Object)(object)this);
						((Behaviour)this).enabled = false;
					}
				}
			}
			return mSource;
		}
	}

	public float volume
	{
		get
		{
			return audioSource.volume;
		}
		set
		{
			audioSource.volume = value;
		}
	}

	protected override void OnUpdate(float factor, bool isFinished)
	{
		volume = from * (1f - factor) + to * factor;
		((Behaviour)mSource).enabled = mSource.volume > 0.01f;
	}

	public static TweenVolume Begin(GameObject go, float duration, float targetVolume)
	{
		TweenVolume tweenVolume = UITweener.Begin<TweenVolume>(go, duration);
		tweenVolume.from = tweenVolume.volume;
		tweenVolume.to = targetVolume;
		if (duration <= 0f)
		{
			tweenVolume.Sample(1f, isFinished: true);
			((Behaviour)tweenVolume).enabled = false;
		}
		return tweenVolume;
	}
}
