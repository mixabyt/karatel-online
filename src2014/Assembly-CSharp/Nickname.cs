using System.Collections;
using UnityEngine;

public class Nickname : MonoBehaviour
{
	public const float MaxDistance = 500f;

	private GameObject _GUIObject;

	private UILabel _Text;

	private Transform _Level;

	private bool _Show;

	private Transform _Target;

	private bool _RaycastFail;

	public void SetPlayer(PlayerNode player)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected Obj, but got Unknown
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)_GUIObject == (Object)null)
		{
			GameObject gameObject = ((Component)KGUI.FindNode("hud.nicknames.nickname")).gameObject;
			GameObject val = (GameObject)Object.Instantiate((Object)(object)gameObject);
			Utils.SetActiveRecursively(val, enable: true);
			val.transform.parent = gameObject.transform.parent;
			val.transform.localScale = gameObject.transform.localScale;
			val.transform.localPosition = gameObject.transform.localPosition;
			val.transform.localRotation = gameObject.transform.localRotation;
			_GUIObject = val;
			_Text = ((Component)_GUIObject.transform.Find("txt_text")).GetComponent<UILabel>();
			_Level = _GUIObject.transform.Find("level_icon");
			if (!GameType.BattleMode())
			{
				Object.Destroy((Object)(object)((Component)_Level).gameObject);
				_Level = null;
			}
			_GUIObject.SetActive(true);
			_Show = true;
		}
		_Text.text = player.Name;
		SetLevel(player.Level);
		_Target = ((Component)this).GetComponent<PlayerNetwork>().SpinBone.transform;
	}

	public void SetLevel(int level)
	{
		if ((Object)(object)_Level != (Object)null)
		{
			KGUI.SetControlSprite(_Level, "level_" + level, 0f);
		}
	}

	public void SetColor(Color color)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)_Text != (Object)null)
		{
			float a = _Text.color.a;
			_Text.color = new Color(color.r, color.g, color.b, a);
		}
	}

	public void Hide()
	{
		if ((Object)(object)_GUIObject != (Object)null)
		{
			_GUIObject.SetActive(false);
			_Show = false;
		}
	}

	public void Show()
	{
		if ((Object)(object)_GUIObject != (Object)null)
		{
			_GUIObject.SetActive(true);
			_Show = true;
		}
	}

	private void OnDestroy()
	{
		if ((Object)(object)_GUIObject != (Object)null)
		{
			Object.Destroy((Object)(object)_GUIObject);
		}
	}

	private void LateUpdate()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		if (_Show && (Object)(object)_GUIObject != (Object)null && (Object)(object)_Target != (Object)null && (Object)(object)CameraController.Instance != (Object)null)
		{
			Vector3 pos = CameraController.RaycastCamera.WorldToScreenPoint(((Component)_Target).transform.position + Vector3.up * 1.2f);
			bool flag = pos.z > 0f && pos.z <= 500f && !_RaycastFail;
			if (_GUIObject.activeSelf != flag)
			{
				_GUIObject.SetActive(flag);
			}
			if (flag)
			{
				_GUIObject.transform.localPosition = KGUI.ScreenToGUIPoint(pos);
			}
		}
		else if ((Object)(object)_GUIObject != (Object)null && _GUIObject.activeSelf)
		{
			_GUIObject.SetActive(false);
		}
	}

	public void SetRaycast(bool enable)
	{
		((MonoBehaviour)this).StopCoroutine("RaycastPlayerProcess");
		if (enable)
		{
			((MonoBehaviour)this).StartCoroutine("RaycastPlayerProcess");
		}
		else
		{
			_RaycastFail = false;
		}
	}

	private IEnumerator RaycastPlayerProcess()
	{
		while (true)
		{
			RaycastPlayer();
			yield return (object)new WaitForSeconds(0.5f);
		}
	}

	private void RaycastPlayer()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		_RaycastFail = true;
		if ((Object)(object)CameraController.Instance == (Object)null)
		{
			return;
		}
		if ((Object)(object)((Component)this).gameObject == (Object)(object)WorldGameObject.Instance.MainPlayer)
		{
			_RaycastFail = false;
			return;
		}
		Vector3 position = ((Component)CameraController.RaycastCamera).transform.position;
		PlayerNetwork component = ((Component)this).gameObject.GetComponent<PlayerNetwork>();
		Vector3 val;
		if ((Object)(object)component != (Object)null && (Object)(object)component.HeadBone != (Object)null && component.HeadBone.transform.childCount > 0)
		{
			val = ((Component)component.HeadBone.transform.GetChild(0)).transform.position;
		}
		else if ((Object)(object)component != (Object)null && (Object)(object)component.HeadBone != (Object)null)
		{
			val = component.HeadBone.transform.position;
		}
		else
		{
			Bounds bounds = ((Component)this).gameObject.collider.bounds;
			val = bounds.center;
		}
		float num = Vector3.Distance(position, val);
		if (!(num > 500f))
		{
			int num2 = 1 << LayerMask.NameToLayer("Terrain");
			Vector3 val2 = val - position;
			if (!Physics.Raycast(position, val2.normalized, num, num2))
			{
				_RaycastFail = false;
			}
		}
	}
}
