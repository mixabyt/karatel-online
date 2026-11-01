using System.Collections.Generic;
using UnityEngine;

public class KindTest : MonoBehaviour
{
	private void Awake()
	{
	}

	private void Update()
	{
		if ((!Input.GetKeyDown((KeyCode)122) && !Input.GetKeyDown((KeyCode)120)) || Chat.IsEnabled() || MainMenu.Instance.MenuActive)
		{
			return;
		}
		List<CommonBlockKind> list = new List<CommonBlockKind>();
		if (ProfileINI.GetPurchaseValue(StorePurchase.BLOCK_KIND_FENCE) > 0)
		{
			list.Add(CommonBlockKind.Fence);
		}
		if (ProfileINI.GetPurchaseValue(StorePurchase.BLOCK_KIND_HALF) > 0)
		{
			list.Add(CommonBlockKind.Half);
		}
		if (ProfileINI.GetPurchaseValue(StorePurchase.BLOCK_KIND_DIAGONAL) > 0)
		{
			list.Add(CommonBlockKind.Diagonal);
		}
		if (ProfileINI.GetPurchaseValue(StorePurchase.BLOCK_KIND_STAIR) > 0)
		{
			list.Add(CommonBlockKind.Stair);
		}
		list.Add(CommonBlockKind.Default);
		int num = list.IndexOf(WorldGameObject.Instance.CurrentCommonKind);
		if (num != -1)
		{
			if (Input.GetKeyDown((KeyCode)122) && num > 0)
			{
				num--;
			}
			else if (Input.GetKeyDown((KeyCode)120) && num < list.Count - 1)
			{
				num++;
			}
		}
		else
		{
			num = list.Count - 1;
		}
		WorldGameObject.Instance.CurrentCommonKind = list[num];
		KGUI.EnableNodes("hud.custom_blocks.icon_01_selected", WorldGameObject.Instance.CurrentCommonKind == CommonBlockKind.Fence);
		KGUI.EnableNodes("hud.custom_blocks.icon_02_selected", WorldGameObject.Instance.CurrentCommonKind == CommonBlockKind.Half);
		KGUI.EnableNodes("hud.custom_blocks.icon_03_selected", WorldGameObject.Instance.CurrentCommonKind == CommonBlockKind.Diagonal);
		KGUI.EnableNodes("hud.custom_blocks.icon_04_selected", WorldGameObject.Instance.CurrentCommonKind == CommonBlockKind.Stair);
		KGUI.EnableNodes("hud.custom_blocks.icon_05_selected", WorldGameObject.Instance.CurrentCommonKind == CommonBlockKind.Default);
	}
}
