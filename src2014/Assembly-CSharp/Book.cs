using System.Collections.Generic;
using UnityEngine;

public class Book : EntityBase
{
	public static Book Current;

	private List<string> _Texts = new List<string>();

	private int _CurPage;

	private bool _EditorMode => Creator == ProfileINI.nickname;

	public override void OnButtonF(string playerName)
	{
		base.OnButtonF(playerName);
		Current = this;
		MainMenu.Instance.SwitchMenu(Menu.Book);
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

	public void RefreshControls()
	{
		for (int i = 0; i < 2; i++)
		{
			int num = _CurPage + i;
			string text = "-" + (num + 1).ToString("d2") + "-";
			if (i == 0)
			{
				KGUI.SetControlText("book.txt_page_num_left", text);
			}
			else
			{
				KGUI.SetControlText("book.txt_page_num_right", text);
			}
			string text2 = ((num >= _Texts.Count) ? string.Empty : _Texts[num]);
			if (i == 0)
			{
				KGUI.SetControlText("book.inp_page_left", text2);
			}
			else
			{
				KGUI.SetControlText("book.inp_page_right", text2);
			}
		}
		KGUI.EnableNodes("book.ibtn_next", (!_EditorMode && _CurPage < _Texts.Count - 2) || (_EditorMode && _CurPage <= _Texts.Count));
		KGUI.EnableNodes("book.ibtn_prev", _CurPage >= 2);
	}

	public void NextButton()
	{
		SaveCurTexts();
		_CurPage += 2;
		RefreshControls();
	}

	public void PrevButton()
	{
		SaveCurTexts();
		_CurPage -= 2;
		RefreshControls();
	}

	public void CloseButton()
	{
		SaveCurTexts();
		Current = null;
		MainMenu.Instance.HideMenu();
	}

	public void SaveCurTexts()
	{
		if (_EditorMode)
		{
			photonView.RPC("UpdateText", PhotonTargets.All, _CurPage, KGUI.GetControlText("book.inp_page_left"));
			photonView.RPC("UpdateText", PhotonTargets.All, _CurPage + 1, KGUI.GetControlText("book.inp_page_right"));
		}
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
		for (int num = _Texts.Count - 1; num >= 0; num--)
		{
			photonView.RPC("UpdateText", info.sender, num, _Texts[num]);
		}
	}
}
