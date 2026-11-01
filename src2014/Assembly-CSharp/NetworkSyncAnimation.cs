using System;
using Photon;
using UnityEngine;

public class NetworkSyncAnimation : MonoBehaviour
{
	public enum AnimState
	{
		idle,
		idle2,
		run,
		jump,
		strafL,
		strafR,
		back,
		dig,
		build,
		death,
		fidget,
		sit,
		lie
	}

	private SkinManager _SkinManager;

	private AnimState _CurrentAnimation;

	private AnimState _LastAnimation;

	public Animation Anim => (!((Object)(object)_SkinManager != (Object)null)) ? ((Component)this).animation : _SkinManager.Anim;

	public AnimState CurrentAnimation => _CurrentAnimation;

	private void Start()
	{
		_SkinManager = ((Component)this).GetComponent<SkinManager>();
	}

	public void PlayAnimation(AnimState anim, bool crossFade = true)
	{
		if ((Object)(object)Anim != (Object)null)
		{
			string name = Enum.GetName(typeof(AnimState), anim);
			if (crossFade)
			{
				Anim.CrossFade(name);
			}
			else
			{
				Anim.Play(name);
			}
		}
		SyncAnimation(anim);
	}

	public void SyncAnimation(AnimState anim)
	{
		_CurrentAnimation = anim;
	}

	private void Update()
	{
		if (photonView.isMine || !((Object)(object)Anim != (Object)null) || !((Object)(object)((Component)this).GetComponent<PlayerNetwork>() != (Object)null))
		{
			return;
		}
		string name = Enum.GetName(typeof(AnimState), _CurrentAnimation);
		name = ((Component)this).GetComponent<PlayerNetwork>().GetPrefix(Anim, name);
		if (!Anim.IsPlaying(name) && (_CurrentAnimation != AnimState.death || _CurrentAnimation != _LastAnimation))
		{
			if (_CurrentAnimation == AnimState.dig || _CurrentAnimation == AnimState.build)
			{
				Anim.Play(name);
			}
			else
			{
				Anim.CrossFade(name);
			}
			_LastAnimation = _CurrentAnimation;
		}
	}

	private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext((byte)_CurrentAnimation);
		}
		else
		{
			_CurrentAnimation = (AnimState)(byte)stream.ReceiveNext();
		}
	}
}
