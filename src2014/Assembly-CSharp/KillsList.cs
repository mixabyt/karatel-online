using System.Collections.Generic;
using Photon;
using UnityEngine;

public class KillsList : MonoBehaviour
{
	private class Message
	{
		public float StartTime;

		public string Killer;

		public string Victim;

		public int GunID;

		public UIWidget[] Widgets;
	}

	public const int MaxMessages = 5;

	public const float DisappearDelay = 5f;

	public const float DisappearTime = 1f;

	private static KillsList _Instance;

	private List<Message> _Messages = new List<Message>();

	private GameObject _Grid;

	private void Awake()
	{
		_Instance = this;
		_Grid = ((Component)KGUI.FindNode("hud.kills_list.grid_messages")).gameObject;
		KGUI.ResizeGrid("hud.kills_list.grid_messages", 0, (GameObject slot, int index) =>
		{
		});
	}

	private void Update()
	{
		if (_Grid.activeInHierarchy)
		{
			RefreshAlpha();
		}
	}

	public static void AddKill(string killer, string victim, int gunID)
	{
		if (!((Object)(object)_Instance == (Object)null))
		{
			_Instance.AddKillMessage(killer, victim, gunID);
		}
	}

	private void AddKillMessage(string killer, string victim, int gunID)
	{
		if (!WorldGameObject.Instance.IsWorldGenerated || (Object)(object)WorldGameObject.Instance.MainPlayer == (Object)null)
		{
			return;
		}
		Gun[] guns = WorldGameObject.Instance.MainPlayer.GetComponent<PlayerNetwork>().Guns;
		if (gunID < 0 || gunID >= guns.Length)
		{
			return;
		}
		_Messages.Add(new Message
		{
			StartTime = Time.time,
			Killer = killer,
			Victim = victim,
			GunID = gunID
		});
		while (_Messages.Count > 5)
		{
			_Messages.RemoveAt(0);
		}
		ActivateGrid(activate: true);
		KGUI.ResizeGrid("hud.kills_list.grid_messages", _Messages.Count, (GameObject slot, int index) =>
		{
			Message message = _Messages[_Messages.Count - index - 1];
			message.Widgets = slot.GetComponentsInChildren<UIWidget>(true);
			UIWidget[] widgets = message.Widgets;
			foreach (UIWidget uIWidget in widgets)
			{
				if (((Object)uIWidget).name == "txt_killer")
				{
					((Component)uIWidget).GetComponent<UILabel>().text = message.Killer;
				}
				else if (((Object)uIWidget).name == "txt_victim")
				{
					((Component)uIWidget).GetComponent<UILabel>().text = message.Victim;
				}
				else if (((Object)((Component)uIWidget).transform.parent).name == "gun_image")
				{
					KGUI.SetControlSprite(((Component)uIWidget).transform.parent, "weapons_" + ((Object)guns[(message.GunID != -1) ? message.GunID : 0]).name, 0f);
				}
			}
		});
		RefreshAlpha();
	}

	private void RefreshAlpha()
	{
		float num = 0f;
		foreach (Message message in _Messages)
		{
			float num2 = Mathf.Clamp01(1f - (Time.time - message.StartTime - 5f) / 1f);
			UIWidget[] widgets = message.Widgets;
			foreach (UIWidget uIWidget in widgets)
			{
				if ((Object)(object)uIWidget != (Object)null)
				{
					uIWidget.alpha = num2;
				}
			}
			num = Mathf.Max(num, num2);
		}
		if (num == 0f)
		{
			ActivateGrid(activate: false);
		}
	}

	private void ActivateGrid(bool activate)
	{
		_Grid.SetActive(activate);
		if (activate)
		{
			((Component)_Grid.transform.Find("slot_prototype")).gameObject.SetActive(false);
		}
	}
}
