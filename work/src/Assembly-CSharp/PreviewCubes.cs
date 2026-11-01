using UnityEngine;

public class PreviewCubes : MonoBehaviour
{
	public static PreviewCubes Instance;

	public GameObject Cube;

	public GameObject Rail;

	private GameObject _CurModel;

	private GUIStyle _RailsCountStyle;

	private void Awake()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected Obj, but got Unknown
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		Instance = this;
		_RailsCountStyle = new GUIStyle();
		_RailsCountStyle.fontSize = 40;
		_RailsCountStyle.normal.textColor = Color.white;
		_RailsCountStyle.alignment = (TextAnchor)4;
		DrawCube();
	}

	private void Update()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)_CurModel != (Object)null)
		{
			_CurModel.transform.Rotate(Vector3.up * Time.deltaTime * 20f);
		}
	}

	private void OnGUI()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		GUI.depth = 100;
		if ((Object)(object)_CurModel == (Object)(object)Rail)
		{
			GUI.Label(new Rect(20f, (float)(Screen.height - 115), 100f, 100f), ProfileINI.GetPurchaseValue(StorePurchase.RAIL).ToString(), _RailsCountStyle);
		}
	}

	public void DrawCube()
	{
		if (!((Object)(object)_CurModel == (Object)(object)Cube))
		{
			_CurModel = Cube;
			Cube.renderer.enabled = true;
			Rail.renderer.enabled = false;
		}
	}

	public void DrawRail()
	{
		if (!((Object)(object)_CurModel == (Object)(object)Rail))
		{
			_CurModel = Rail;
			Rail.renderer.enabled = true;
			Cube.renderer.enabled = false;
		}
	}
}
