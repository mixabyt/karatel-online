using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Saved Option")]
public class UISavedOption : MonoBehaviour
{
	public string keyName;

	private UIPopupList mList;

	private UICheckbox mCheck;

	private string key => (!string.IsNullOrEmpty(keyName)) ? keyName : ("NGUI State: " + ((Object)this).name);

	private void Awake()
	{
		mList = ((Component)this).GetComponent<UIPopupList>();
		mCheck = ((Component)this).GetComponent<UICheckbox>();
		if ((Object)(object)mList != (Object)null)
		{
			UIPopupList uIPopupList = mList;
			uIPopupList.onSelectionChange = (UIPopupList.OnSelectionChange)Delegate.Combine(uIPopupList.onSelectionChange, new UIPopupList.OnSelectionChange(SaveSelection));
		}
		if ((Object)(object)mCheck != (Object)null)
		{
			UICheckbox uICheckbox = mCheck;
			uICheckbox.onStateChange = (UICheckbox.OnStateChange)Delegate.Combine(uICheckbox.onStateChange, new UICheckbox.OnStateChange(SaveState));
		}
	}

	private void OnDestroy()
	{
		if ((Object)(object)mCheck != (Object)null)
		{
			UICheckbox uICheckbox = mCheck;
			uICheckbox.onStateChange = (UICheckbox.OnStateChange)Delegate.Remove(uICheckbox.onStateChange, new UICheckbox.OnStateChange(SaveState));
		}
		if ((Object)(object)mList != (Object)null)
		{
			UIPopupList uIPopupList = mList;
			uIPopupList.onSelectionChange = (UIPopupList.OnSelectionChange)Delegate.Remove(uIPopupList.onSelectionChange, new UIPopupList.OnSelectionChange(SaveSelection));
		}
	}

	private void OnEnable()
	{
		if ((Object)(object)mList != (Object)null)
		{
			string text = PlayerPrefs.GetString(key);
			if (!string.IsNullOrEmpty(text))
			{
				mList.selection = text;
			}
			return;
		}
		if ((Object)(object)mCheck != (Object)null)
		{
			mCheck.isChecked = PlayerPrefs.GetInt(key, 1) != 0;
			return;
		}
		string text2 = PlayerPrefs.GetString(key);
		UICheckbox[] componentsInChildren = ((Component)this).GetComponentsInChildren<UICheckbox>(true);
		int i = 0;
		for (int num = componentsInChildren.Length; i < num; i++)
		{
			UICheckbox uICheckbox = componentsInChildren[i];
			uICheckbox.isChecked = ((Object)uICheckbox).name == text2;
		}
	}

	private void OnDisable()
	{
		if (!((Object)(object)mCheck == (Object)null) || !((Object)(object)mList == (Object)null))
		{
			return;
		}
		UICheckbox[] componentsInChildren = ((Component)this).GetComponentsInChildren<UICheckbox>(true);
		int i = 0;
		for (int num = componentsInChildren.Length; i < num; i++)
		{
			UICheckbox uICheckbox = componentsInChildren[i];
			if (uICheckbox.isChecked)
			{
				SaveSelection(((Object)uICheckbox).name);
				break;
			}
		}
	}

	private void SaveSelection(string selection)
	{
		PlayerPrefs.SetString(key, selection);
	}

	private void SaveState(bool state)
	{
		PlayerPrefs.SetInt(key, state ? 1 : 0);
	}
}
