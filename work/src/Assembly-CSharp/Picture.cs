using System.Collections;
using UnityEngine;

public class Picture : EntityBase
{
	public GameObject picture;

	public Texture2D default_texture;

	public string picture_url;

	protected override void PreviewCreation(object[] data)
	{
		if (data != null)
		{
			SendPictureToOther((string)data[0]);
		}
	}

	protected override void Creation(object[] data)
	{
		picture.renderer.material.mainTexture = (Texture)(object)default_texture;
		if (data != null)
		{
			SendPictureToOther((string)data[0]);
		}
		if (!PhotonNetwork.isMasterClient)
		{
			photonView.RPC("GetCurPicture", PhotonTargets.MasterClient);
		}
	}

	public override object[] GetData()
	{
		return new object[1] { picture_url };
	}

	public override void OnButtonF(string player_name)
	{
		if (player_name == Creator)
		{
			MainMenu.Instance.SwitchMenu(Menu.SetPicture);
		}
		else
		{
			Chat.SendInfo(Localize.GetText("CANT_EDIT_PICTURES"), forAll: false);
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
