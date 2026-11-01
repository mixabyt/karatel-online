using System.Collections.Generic;
using UnityEngine;

public class Book : EntityBase
{
	public GUISkin BookSkin;

	private List<string> _Texts = new List<string>();

	private List<string> _BaseTexts;

	private int _CurPage;

	private bool _Opened;

	private static bool _IsAnyOpened;

	public static bool IsAnyOpened => _IsAnyOpened;

	public override void OnButtonF(string playerName)
	{
		base.OnButtonF(playerName);
		EnterExit();
	}

	protected override void Creation(object[] data)
	{
		if (data != null)
		{
			_Texts.Clear();
			for (int i = 0; i < data.Length; i++)
			{
				_Texts.Add((string)data[i]);
			}
		}
		if (!PhotonNetwork.isMasterClient)
		{
			photonView.RPC("GetTexts", PhotonTargets.MasterClient);
		}
	}

	public override object[] GetData()
	{
		List<object> list = new List<object>();
		foreach (string text in _Texts)
		{
			list.Add(text);
		}
		return list.ToArray();
	}

	private void EnterExit()
	{
		_Opened = !_Opened;
		_IsAnyOpened = _Opened;
		Screen.lockCursor = !_Opened;
		WorldGameObject.INSTANCE.MainPlayer.GetComponent<CameraController>().DisableInput = _Opened;
		if (_Opened)
		{
			ChatClient.isEnabled = false;
		}
		GUI.FocusControl("BookBox");
	}

	private void OnGUI()
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Expected Obj, but got Unknown
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		if (!_Opened)
		{
			return;
		}
		bool flag = Creator == ProfileINI.nickname;
		GUI.depth = 0;
		GUI.skin = BookSkin;
		GUI.SetNextControlName("BookBox");
		GUI.Box(new Rect((float)(Screen.width / 2 - 380), (float)(Screen.height / 2 - 300), 760f, 600f), string.Empty);
		Rect[] array = new Rect[2]
		{
			new Rect((float)(Screen.width / 2 - 380 + 50), (float)(Screen.height / 2 - 300 + 96), 270f, 292f),
			new Rect((float)(Screen.width / 2 - 380 + 441), (float)(Screen.height / 2 - 300 + 96), 270f, 292f)
		};
		Rect[] array2 = new Rect[3]
		{
			new Rect((float)(Screen.width / 2 - 380 + 35), (float)(Screen.height / 2 - 300 + 376), 51f, 52f),
			new Rect((float)(Screen.width / 2 - 380 + 680), (float)(Screen.height / 2 - 300 + 376), 51f, 52f),
			new Rect((float)(Screen.width / 2 - 380 + 680), (float)(Screen.height / 2 - 300 + 50), 50f, 50f)
		};
		for (int i = 0; i < 2; i++)
		{
			Rect val = array[i];
			int num = _CurPage + i;
			GUI.Label(new Rect(val.center.x - 20f, val.yMax + 10f, 50f, 40f), "-" + (num + 1).ToString("d2") + "-");
			string text = ((num >= _Texts.Count) ? string.Empty : _Texts[num]);
			if (flag)
			{
				string text2 = GUI.TextArea(val, text);
				if (BookSkin.textArea.CalcHeight(new GUIContent(text2), val.width) <= val.height && text2 != text)
				{
					if (_BaseTexts == null)
					{
						_BaseTexts = new List<string>(_Texts);
					}
					UpdateText(num, text2);
				}
			}
			else
			{
				GUI.Label(val, text);
			}
		}
		if (_CurPage >= 2 && GUI.Button(array2[0], string.Empty, BookSkin.GetStyle("buttonLeft")))
		{
			SaveEditedTexts();
			_CurPage -= 2;
			GUI.FocusControl("BookBox");
		}
		if (((!flag && _CurPage < _Texts.Count - 2) || (flag && _CurPage <= _Texts.Count - 2)) && GUI.Button(array2[1], string.Empty, BookSkin.GetStyle("buttonRight")))
		{
			SaveEditedTexts();
			_CurPage += 2;
			GUI.FocusControl("BookBox");
		}
		if (GUI.Button(array2[2], string.Empty, BookSkin.GetStyle("buttonClose")))
		{
			SaveEditedTexts();
			EnterExit();
		}
	}

	private void SaveEditedTexts()
	{
		if (_BaseTexts == null)
		{
			return;
		}
		for (int num = Mathf.Max(_BaseTexts.Count, _Texts.Count); num >= 0; num--)
		{
			if (num >= _Texts.Count || num >= _BaseTexts.Count || _BaseTexts[num] != _Texts[num])
			{
				photonView.RPC("UpdateText", PhotonTargets.Others, num, (num >= _Texts.Count) ? string.Empty : _Texts[num]);
			}
		}
		_BaseTexts = null;
	}

	[RPC]
	private void UpdateText(int index, string text)
	{
		while (index >= _Texts.Count)
		{
			_Texts.Add(string.Empty);
		}
		_Texts[index] = text;
		while (index >= 0 && index == _Texts.Count - 1 && _Texts[index] == string.Empty)
		{
			_Texts.RemoveAt(_Texts.Count - 1);
			index--;
		}
	}

	[RPC]
	private void GetTexts(PhotonMessageInfo info)
	{
		List<string> list = ((_BaseTexts == null) ? _Texts : _BaseTexts);
		for (int num = list.Count - 1; num >= 0; num--)
		{
			photonView.RPC("UpdateText", info.sender, num, list[num]);
		}
	}
}
