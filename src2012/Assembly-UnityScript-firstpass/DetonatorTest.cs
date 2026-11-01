using System;
using Boo.Lang.Runtime;
using UnityEngine;
using UnityScript.Lang;

[Serializable]
public class DetonatorTest : MonoBehaviour
{
	public GameObject currentDetonator;

	private int _currentExpIdx;

	private bool buttonClicked;

	public GameObject[] detonatorPrefabs;

	public float explosionLife;

	public float timeScale;

	public float detailLevel;

	public GameObject wall;

	private GameObject _currentWall;

	private int _spawnWallTime;

	private object _guiRect;

	private bool toggleBool;

	private Rect checkRect;

	public DetonatorTest()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		_currentExpIdx = -1;
		explosionLife = 10f;
		timeScale = 1f;
		detailLevel = 1f;
		_spawnWallTime = -1000;
		checkRect = new Rect(0f, 0f, 260f, 180f);
	}

	public override void Start()
	{
		SpawnWall();
		if (!Object.op_Implicit((Object)(object)currentDetonator))
		{
			NextExplosion();
		}
		else
		{
			_currentExpIdx = 0;
		}
	}

	public override void OnGUI()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		_guiRect = (object)new Rect(7f, (float)(Screen.height - 180), 250f, 200f);
		GUILayout.BeginArea((Rect)_guiRect);
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		string name = ((Object)currentDetonator).name;
		if (GUILayout.Button(name + " (Click For Next)", new GUILayoutOption[0]))
		{
			NextExplosion();
		}
		if (GUILayout.Button("Rebuild Wall", new GUILayoutOption[0]))
		{
			SpawnWall();
		}
		if (GUILayout.Button("Camera Far", new GUILayoutOption[0]))
		{
			((Component)Camera.main).transform.position = new Vector3(0f, 0f, -7f);
			((Component)Camera.main).transform.eulerAngles = new Vector3(13.5f, 0f, 0f);
		}
		if (GUILayout.Button("Camera Near", new GUILayoutOption[0]))
		{
			((Component)Camera.main).transform.position = new Vector3(0f, -8.664466f, 31.38269f);
			((Component)Camera.main).transform.eulerAngles = new Vector3(1.213462f, 0f, 0f);
		}
		GUILayout.Label("Time Scale", new GUILayoutOption[0]);
		timeScale = GUILayout.HorizontalSlider(timeScale, 0f, 1f, new GUILayoutOption[0]);
		GUILayout.Label("Detail Level (re-explode after change)", new GUILayoutOption[0]);
		detailLevel = GUILayout.HorizontalSlider(detailLevel, 0f, 1f, new GUILayoutOption[0]);
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	public override void NextExplosion()
	{
		if (_currentExpIdx >= detonatorPrefabs.Length - 1)
		{
			_currentExpIdx = 0;
		}
		else
		{
			_currentExpIdx++;
		}
		currentDetonator = detonatorPrefabs[_currentExpIdx];
	}

	public override void SpawnWall()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected Obj, but got Unknown
		if (Object.op_Implicit((Object)(object)_currentWall))
		{
			Object.Destroy((Object)(object)_currentWall);
		}
		_currentWall = (GameObject)Object.Instantiate((Object)(object)wall, new Vector3(-7f, -12f, 48f), Quaternion.identity);
		_spawnWallTime = (int)Time.time;
	}

	public override void Update()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		_guiRect = (object)new Rect(7f, (float)(Screen.height - 150), 250f, 200f);
		if (!(Time.time + (float)_spawnWallTime <= 0.5f))
		{
			if (!checkRect.Contains(Input.mousePosition) && Input.GetMouseButtonDown(0))
			{
				SpawnExplosion();
			}
			Time.timeScale = timeScale;
		}
	}

	public override void SpawnExplosion()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected Obj, but got Unknown
		Ray val = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit val2 = default;
		GameObject val4 = default;
		if (Physics.Raycast(val, ref val2, 1000f))
		{
			object value = RuntimeServices.InvokeBinaryOperator("op_Division", UnityRuntimeServices.GetProperty(currentDetonator.GetComponent("Detonator"), "size"), 3);
			Vector3 val3 = val2.point + Vector3.Scale(val2.normal, new Vector3(RuntimeServices.UnboxSingle(value), RuntimeServices.UnboxSingle(value), RuntimeServices.UnboxSingle(value)));
			val4 = (GameObject)Object.Instantiate((Object)(object)currentDetonator, val3, Quaternion.identity);
			RuntimeServices.SetProperty(val4.GetComponent("Detonator"), "detail", detailLevel);
		}
		Object.Destroy((Object)(object)val4, explosionLife);
	}

	public override void Main()
	{
	}
}
