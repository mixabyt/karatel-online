using System;
using System.Collections;
using UnityEngine;

public class ResourceLoader : MonoBehaviour
{
	public enum eSounds
	{
		BangDirt_1,
		BangGlass_1,
		BangGrass_1,
		BangMetal_1,
		BangSand_1,
		BangStone_1,
		BangWood_1,
		FootDirt_4,
		FootGlass_4,
		FootGrass_4,
		FootMetal_4,
		FootSand_4,
		FootStone_4,
		FootWood_4,
		HitDirt_3,
		HitGlass_3,
		HitGrass_3,
		HitMetal_3,
		HitSand_3,
		HitStone_3,
		HitWood_3,
		WaterIn_1,
		WaterOut_1,
		LoopForest_1,
		WeaponSwish_2,
		BangWater_1,
		FootSnow_4,
		HitSnow_3,
		BangSnow_1,
		FootIce_4,
		HitIce_2,
		BangIce_1,
		LoopSnow_1,
		Jump_1,
		LoopOcean_1,
		FootLava_1,
		SetDecor_1,
		Note2_1,
		LoopDesert_1,
		DoorOpen_1,
		DoorClose_1,
		TorchLoop_1,
		LoopUnderwater_1,
		Treasure_1
	}

	[Serializable]
	public class SoundEffect
	{
		public AudioClip[] audio_clips;
	}

	public static ResourceLoader I;

	public string SOUNDS_URL;

	public SoundEffect[] mSoundEffects;

	public bool SoundsBundleLoaded;

	private void Start()
	{
	}

	private IEnumerator LoadSoundsBundle()
	{
		Debug.Log((object)"StartLoadingSound");
		WWW request = WWW.LoadFromCacheOrDownload("//kopatel-online.ru/2012/Sounds3.unity3d", 2);
		yield return request;
		Debug.Log((object)"FinishLoadingSound");
		mSoundEffects = new SoundEffect[Enum.GetValues(typeof(eSounds)).Length];
		int cur_effect_id = 0;
		foreach (int sound in Enum.GetValues(typeof(eSounds)))
		{
			string[] param = ((eSounds)sound).ToString().Split('_');
			int count_samples = int.Parse(param[1]);
			mSoundEffects[cur_effect_id] = new SoundEffect();
			mSoundEffects[cur_effect_id].audio_clips = new AudioClip[count_samples];
			for (int i = 1; i <= count_samples; i++)
			{
				string file_name = ((count_samples != 1) ? (param[0] + "_" + i) : param[0]);
				mSoundEffects[cur_effect_id].audio_clips[i - 1] = (AudioClip)request.assetBundle.Load(file_name);
			}
			cur_effect_id++;
		}
		SoundsBundleLoaded = true;
	}

	public AudioClip GetClip(eSounds effect)
	{
		int num = mSoundEffects[(int)effect].audio_clips.Length;
		if (num == 1)
		{
			return mSoundEffects[(int)effect].audio_clips[0];
		}
		int num2 = Random.Range(0, num);
		return mSoundEffects[(int)effect].audio_clips[num2];
	}

	public void PlayEffectAtPoint(eSounds effect, AudioSource source, Vector3 pos)
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		int num = mSoundEffects[(int)effect].audio_clips.Length;
		if (num == 1)
		{
			if (pos == Vector3.zero)
			{
				source.PlayOneShot(mSoundEffects[(int)effect].audio_clips[0], ProfileINI.sound_volume * ProfileINI.sound_scale);
			}
			else
			{
				AudioSource.PlayClipAtPoint(mSoundEffects[(int)effect].audio_clips[0], pos, ProfileINI.sound_volume * ProfileINI.sound_scale);
			}
			return;
		}
		int num2 = Random.Range(0, num);
		if (pos == Vector3.zero)
		{
			source.PlayOneShot(mSoundEffects[(int)effect].audio_clips[num2], ProfileINI.sound_volume * ProfileINI.sound_scale);
		}
		else
		{
			AudioSource.PlayClipAtPoint(mSoundEffects[(int)effect].audio_clips[num2], pos, ProfileINI.sound_volume * ProfileINI.sound_scale);
		}
	}

	public void PlayEffect(eSounds effect, AudioSource source)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		PlayEffectAtPoint(effect, source, Vector3.zero);
	}

	private void Awake()
	{
		if ((Object)(object)I == (Object)null)
		{
			I = this;
		}
	}

	private void Update()
	{
	}
}
