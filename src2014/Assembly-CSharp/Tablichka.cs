using UnityEngine;

public class Tablichka : EntityBase
{
	public GameObject Line1;

	public GameObject Line2;

	public GameObject Line3;

	public GameObject Line4;

	protected override void PreviewCreation(object[] data)
	{
		if (data != null)
		{
			SetText((string)data[0], (string)data[1], (string)data[2], (string)data[3]);
		}
	}

	protected override void Creation(object[] data)
	{
		if (data != null)
		{
			SetText((string)data[0], (string)data[1], (string)data[2], (string)data[3]);
		}
		if (!PhotonNetwork.isMasterClient)
		{
			photonView.RPC("GetActualText", PhotonTargets.MasterClient);
		}
	}

	public override object[] GetData()
	{
		return new object[4]
		{
			Line1.GetComponent<TextMesh>().text,
			Line2.GetComponent<TextMesh>().text,
			Line3.GetComponent<TextMesh>().text,
			Line4.GetComponent<TextMesh>().text
		};
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
			MainMenu.Instance.SwitchMenu(Menu.Plate, this, new string[4]
			{
				Line1.GetComponent<TextMesh>().text,
				Line2.GetComponent<TextMesh>().text,
				Line3.GetComponent<TextMesh>().text,
				Line4.GetComponent<TextMesh>().text
			});
		}
		else
		{
			Chat.SendInfo(Localize.GetText("CANT_EDIT_PLATE"), forAll: false);
		}
	}

	[RPC]
	private void SetText(string s1, string s2, string s3, string s4)
	{
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		Line1.GetComponent<TextMesh>().characterSize = 0.4f;
		Line2.GetComponent<TextMesh>().characterSize = 0.4f;
		Line3.GetComponent<TextMesh>().characterSize = 0.4f;
		Line4.GetComponent<TextMesh>().characterSize = 0.4f;
		Line1.GetComponent<TextMesh>().text = s1;
		Line2.GetComponent<TextMesh>().text = s2;
		Line3.GetComponent<TextMesh>().text = s3;
		Line4.GetComponent<TextMesh>().text = s4;
		while (true)
		{
			Bounds bounds = ((Renderer)Line1.GetComponent<MeshRenderer>()).bounds;
			if (!(bounds.extents.z > 0.3f))
			{
				Bounds bounds2 = ((Renderer)Line1.GetComponent<MeshRenderer>()).bounds;
				if (!(bounds2.extents.x > 0.3f))
				{
					break;
				}
			}
			TextMesh component = Line1.GetComponent<TextMesh>();
			component.characterSize *= 0.9f;
		}
		while (true)
		{
			Bounds bounds3 = ((Renderer)Line2.GetComponent<MeshRenderer>()).bounds;
			if (!(bounds3.extents.z > 0.3f))
			{
				Bounds bounds4 = ((Renderer)Line2.GetComponent<MeshRenderer>()).bounds;
				if (!(bounds4.extents.x > 0.3f))
				{
					break;
				}
			}
			TextMesh component2 = Line2.GetComponent<TextMesh>();
			component2.characterSize *= 0.9f;
		}
		while (true)
		{
			Bounds bounds5 = ((Renderer)Line3.GetComponent<MeshRenderer>()).bounds;
			if (!(bounds5.extents.z > 0.3f))
			{
				Bounds bounds6 = ((Renderer)Line3.GetComponent<MeshRenderer>()).bounds;
				if (!(bounds6.extents.x > 0.3f))
				{
					break;
				}
			}
			TextMesh component3 = Line3.GetComponent<TextMesh>();
			component3.characterSize *= 0.9f;
		}
		while (true)
		{
			Bounds bounds7 = ((Renderer)Line4.GetComponent<MeshRenderer>()).bounds;
			if (!(bounds7.extents.z > 0.3f))
			{
				Bounds bounds8 = ((Renderer)Line4.GetComponent<MeshRenderer>()).bounds;
				if (!(bounds8.extents.x > 0.3f))
				{
					break;
				}
			}
			TextMesh component4 = Line4.GetComponent<TextMesh>();
			component4.characterSize *= 0.9f;
		}
	}

	public void UpdateText(string s1, string s2, string s3, string s4)
	{
		photonView.RPC("SetText", PhotonTargets.All, s1, s2, s3, s4);
	}
}
