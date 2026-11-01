using System.Collections;
using UnityEngine;

public class Picture : EntityBase
{
	public GameObject picture;

	public Texture2D default_texture;

	public string picture_url;

	protected override void Creation(object[] data)
	{
		picture.renderer.material.mainTexture = (Texture)(object)default_texture;
		if (photonView.instantiationData != null)
		{
			SendPictureToOther((string)photonView.instantiationData[2]);
		}
		if (!PhotonNetwork.isMasterClient)
		{
			photonView.RPC("GetCurPicture", PhotonTargets.MasterClient);
		}
	}

	public override void OnButtonF(string player_name)
	{
		if (player_name == Creator)
		{
			MainMenu.INSTANCE.curent_picture = this;
			MainMenu.INSTANCE.picture_url = picture_url;
			MainMenu.INSTANCE.curWindow = MainMenu.Windows.SetPicture;
			MainMenu.INSTANCE.showMenu = true;
			WorldGameObject.INSTANCE.DisableMouseLook();
			Screen.lockCursor = false;
		}
		else
		{
			ChatClient.instance.Info("Чужие картины изменять запрещено");
		}
	}

	[RPC]
	public void SendPictureToOther(string url)
	{
		picture_url = url;
		if (picture_url.Length > 0)
		{
			((MonoBehaviour)this).StartCoroutine(LoadPictureFromUrl(picture_url));
		}
		else
		{
			picture.renderer.material.mainTexture = (Texture)(object)default_texture;
		}
	}

	private IEnumerator LoadPictureFromUrl(string url)
	{
		WWW www = new WWW(url);
		yield return www;
		if (www.error == null)
		{
			picture.renderer.material.mainTexture = (Texture)(object)www.texture;
		}
		else
		{
			picture.renderer.material.mainTexture = (Texture)(object)default_texture;
		}
	}

	public void SynhNetworkPicture(string url)
	{
		photonView.RPC("SendPictureToOther", PhotonTargets.All, url);
	}

	[RPC]
	public void GetCurPicture(PhotonMessageInfo info)
	{
		photonView.RPC("SendPictureToOther", info.sender, picture_url);
	}
}
