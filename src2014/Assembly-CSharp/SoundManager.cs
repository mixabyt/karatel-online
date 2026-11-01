using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	public enum Sound
	{
		BangDirt,
		BangGlass,
		BangGrass,
		BangMetal,
		BangSand,
		BangStone,
		BangWood,
		FootDirt,
		FootGlass,
		FootGrass,
		FootMetal,
		FootSand,
		FootStone,
		FootWood,
		HitDirt,
		HitGlass,
		HitGrass,
		HitMetal,
		HitSand,
		HitStone,
		HitWood,
		WaterIn,
		WaterOut,
		LoopForest,
		WeaponSwish,
		BangWater,
		FootSnow,
		HitSnow,
		BangSnow,
		FootIce,
		HitIce,
		BangIce,
		LoopSnow,
		Jump,
		LoopOcean,
		FootLava,
		SetDecor,
		Note2,
		LoopDesert,
		DoorOpen,
		DoorClose,
		TorchLoop,
		LoopUnderwater,
		Treasure,
		TeamDoorOpen,
		TeamDoorClose,
		MenuClick,
		ShopBuy,
		EverydayBonus,
		BattleWin,
		BattleLose,
		CtfFlagCaptured,
		CtfFlagDelivered,
		CtfFlagStolen,
		ZombieVirusInfection,
		TakeGift,
		ZombieActivity
	}

	[Serializable]
	public class SoundInfo
	{
		public Sound Type;

		public List<AudioClip> AudioClips;
	}

	public static SoundManager Instance;

	public SoundInfo[] Sounds;

	private SoundInfo[] _SortedSounds;

	internal bool SoundsLoaded;

	private void Awake()
	{
		if (!((Object)(object)Instance != (Object)null))
		{
			Instance = this;
		}
	}

	public IEnumerator LoadingSound()
	{
		Sound[] soundTypes = Enum.GetValues(typeof(Sound)) as Sound[];
		_SortedSounds = new SoundInfo[soundTypes.Length];
		for (int i = 0; i < _SortedSounds.Length; i++)
		{
			_SortedSounds[i] = new SoundInfo
			{
				Type = soundTypes[i],
				AudioClips = new List<AudioClip>()
			};
		}
		SoundInfo[] sounds = Sounds;
		foreach (SoundInfo sound in sounds)
		{
			_SortedSounds[(int)sound.Type].AudioClips.AddRange(sound.AudioClips);
		}
		WWW request = WWW.LoadFromCacheOrDownload(App.Instance.GetContentUrl() + "Sounds.unity3d", 1);
		yield return request;
		if (request.error == null)
		{
			for (int k = 0; k < _SortedSounds.Length; k++)
			{
				for (int l = -1; l < 10; l++)
				{
					string fileName = soundTypes[k].ToString();
					if (l >= 0)
					{
						fileName = fileName + "_" + (l + 1);
					}
					if (request.assetBundle.Contains(fileName))
					{
						_SortedSounds[k].AudioClips.Add((AudioClip)request.assetBundle.Load(fileName));
					}
				}
			}
			SoundsLoaded = true;
		}
		else
		{
			Debug.Log((object)("Sound loading error: " + request.error));
		}
	}

	public AudioClip GetClip(Sound sound)
	{
		if (_SortedSounds == null)
		{
			return null;
		}
		List<AudioClip> audioClips = _SortedSounds[(int)sound].AudioClips;
		return (audioClips.Count <= 0) ? null : audioClips[Random.Range(0, audioClips.Count)];
	}

	public void PlayAtPoint(Sound sound, Vector3 pos)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		AudioClip clip = GetClip(sound);
		if ((Object)(object)clip != (Object)null)
		{
			AudioSource.PlayClipAtPoint(clip, pos, ProfileINI.sound_volume * ProfileINI.sound_scale);
		}
	}

	public void Play(Sound sound, AudioSource source = null)
	{
		AudioClip clip = GetClip(sound);
		if ((Object)(object)clip != (Object)null)
		{
			if ((Object)(object)source == (Object)null)
			{
				source = ((Component)this).audio;
			}
			source.PlayOneShot(clip, ProfileINI.sound_volume * ProfileINI.sound_scale);
		}
	}
}
