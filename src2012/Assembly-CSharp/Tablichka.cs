using UnityEngine;

public class Tablichka : EntityBase
{
	public GameObject Line1;

	public GameObject Line2;

	public GameObject Line3;

	public GameObject Line4;

	protected override void Creation(object[] data)
	{
		if (photonView.instantiationData != null)
		{
			SetText((string)photonView.instantiationData[2], (string)photonView.instantiationData[3], (string)photonView.instantiationData[4], (string)photonView.instantiationData[5]);
		}
		if (!PhotonNetwork.isMasterClient)
		{
			photonView.RPC("GetActualText", PhotonTargets.MasterClient);
		}
	}

	[RPC]
	private void GetActualText(PhotonMessageInfo info)
	{
		photonView.RPC("SetText", info.sender, Line1.GetComponent<TextMesh>().text, Line2.GetComponent<TextMesh>().text, Line3.GetComponent<TextMesh>().text, Line4.GetComponent<TextMesh>().text);
	}

	public override void OnButtonF(string playerName)
	{
		if (playerName == Creator)
		{
			MainMenu.INSTANCE.PlateLines[0] = Line1.GetComponent<TextMesh>().text;
			MainMenu.INSTANCE.PlateLines[1] = Line2.GetComponent<TextMesh>().text;
			MainMenu.INSTANCE.PlateLines[2] = Line3.GetComponent<TextMesh>().text;
			MainMenu.INSTANCE.PlateLines[3] = Line4.GetComponent<TextMesh>().text;
			MainMenu.INSTANCE.curWindow = MainMenu.Windows.DrawTablichka;
			MainMenu.INSTANCE.showMenu = true;
			MainMenu.INSTANCE.CurrentPlate = this;
			WorldGameObject.INSTANCE.DisableMouseLook();
			Screen.lockCursor = false;
		}
		else
		{
			ChatClient.instance.Info("Текст чужих табличек изменять запрещено");
		}
	}

	[RPC]
	private void SetText(string s1, string s2, string s3, string s4)
	{
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		Line1.GetComponent<TextMesh>().characterSize = 1f;
		Line2.GetComponent<TextMesh>().characterSize = 1f;
		Line3.GetComponent<TextMesh>().characterSize = 1f;
		Line4.GetComponent<TextMesh>().characterSize = 1f;
		Line1.GetComponent<TextMesh>().text = s1;
		Line2.GetComponent<TextMesh>().text = s2;
		Line3.GetComponent<TextMesh>().text = s3;
		Line4.GetComponent<TextMesh>().text = s4;
		Bounds bounds = ((Renderer)Line1.GetComponent<MeshRenderer>()).bounds;
		if (!(bounds.extents.z > 0.4f))
		{
			Bounds bounds2 = ((Renderer)Line1.GetComponent<MeshRenderer>()).bounds;
			if (!(bounds2.extents.x > 0.4f))
			{
				goto IL_0102;
			}
		}
		Line1.GetComponent<TextMesh>().characterSize = 0.7f;
		goto IL_0102;
		IL_016f:
		Bounds bounds3 = ((Renderer)Line3.GetComponent<MeshRenderer>()).bounds;
		if (!(bounds3.extents.z > 0.4f))
		{
			Bounds bounds4 = ((Renderer)Line3.GetComponent<MeshRenderer>()).bounds;
			if (!(bounds4.extents.x > 0.4f))
			{
				goto IL_01dc;
			}
		}
		Line3.GetComponent<TextMesh>().characterSize = 0.7f;
		goto IL_01dc;
		IL_01dc:
		Bounds bounds5 = ((Renderer)Line4.GetComponent<MeshRenderer>()).bounds;
		if (!(bounds5.extents.z > 0.4f))
		{
			Bounds bounds6 = ((Renderer)Line4.GetComponent<MeshRenderer>()).bounds;
			if (!(bounds6.extents.x > 0.4f))
			{
				return;
			}
		}
		Line4.GetComponent<TextMesh>().characterSize = 0.7f;
		return;
		IL_0102:
		Bounds bounds7 = ((Renderer)Line2.GetComponent<MeshRenderer>()).bounds;
		if (!(bounds7.extents.z > 0.4f))
		{
			Bounds bounds8 = ((Renderer)Line2.GetComponent<MeshRenderer>()).bounds;
			if (!(bounds8.extents.x > 0.4f))
			{
				goto IL_016f;
			}
		}
		Line2.GetComponent<TextMesh>().characterSize = 0.7f;
		goto IL_016f;
	}

	public void UpdateText(string s1, string s2, string s3, string s4)
	{
		photonView.RPC("SetText", PhotonTargets.All, s1, s2, s3, s4);
	}
}
