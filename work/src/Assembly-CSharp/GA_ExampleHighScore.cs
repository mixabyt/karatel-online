using UnityEngine;

public class GA_ExampleHighScore : MonoBehaviour
{
	public GameObject BallPrefab;

	private static bool GAMEOVER;

	private static int SCORE;

	private static int HIGHSCORE;

	private int _genderIndex;

	private int _birthYear;

	private int _friendCount;

	private GUIContent[] genderComboBoxList = new GUIContent[3]
	{
		new GUIContent("-"),
		new GUIContent("Female"),
		new GUIContent("Male")
	};

	private ComboBox genderComboBoxControl = new ComboBox();

	private Vector3 _startingPosition;

	public GA_ExampleHighScore()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected Obj, but got Unknown
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected Obj, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected Obj, but got Unknown
	}

	private void Start()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_startingPosition = ((Component)this).transform.position;
	}

	private void OnGUI()
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected Obj, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (GAMEOVER)
		{
			GUI.Window(0, new Rect((float)(Screen.width / 2 - 100), (float)(Screen.height / 2 - 110), 200f, 220f), (WindowFunction)HighScoreWindow, "GAME OVER");
		}
		GUI.Label(new Rect(10f, (float)(Screen.height - 70), 200f, 20f), "SCORE: " + SCORE);
	}

	public static void GameOver(Vector3 position)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (PlayerPrefs.HasKey("GAExampleScore"))
		{
			HIGHSCORE = PlayerPrefs.GetInt("GAExampleScore", 0);
		}
		GAMEOVER = true;
		GA.API.Design.NewEvent("GameOver", position);
		GA_Queue.ForceSubmit();
	}

	public static void AddScore(int score, string scoreName, Vector3 position)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		SCORE += score;
		GA.API.Design.NewEvent("Score:" + scoreName, 10f, position);
	}

	private void HighScoreWindow(int windowID)
	{
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		if (SCORE > HIGHSCORE)
		{
			GUILayout.Label("OLD HIGHSCORE:", new GUILayoutOption[0]);
		}
		else
		{
			GUILayout.Label("HIGHSCORE:", new GUILayoutOption[0]);
		}
		GUILayout.Label(HIGHSCORE.ToString(), new GUILayoutOption[0]);
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("NEW SCORE:", new GUILayoutOption[0]);
		GUILayout.Label(SCORE.ToString(), new GUILayoutOption[0]);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("BIRTH YEAR:", new GUILayoutOption[0]);
		int result = 0;
		if (int.TryParse(GUILayout.TextField((_birthYear == 0) ? string.Empty : _birthYear.ToString(), new GUILayoutOption[1] { GUILayout.Width(50f) }), out result))
		{
			_birthYear = result;
		}
		else
		{
			_birthYear = 0;
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("FRIEND COUNT:", new GUILayoutOption[0]);
		int result2 = 0;
		if (int.TryParse(GUILayout.TextField((_friendCount == 0) ? string.Empty : _friendCount.ToString(), new GUILayoutOption[1] { GUILayout.Width(50f) }), out result2))
		{
			_friendCount = result2;
		}
		else
		{
			_friendCount = 0;
		}
		GUILayout.EndHorizontal();
		GUILayout.Label("GENDER:", new GUILayoutOption[0]);
		_genderIndex = genderComboBoxControl.List(new Rect(115f, 130f, 75f, 20f), genderComboBoxList[_genderIndex], genderComboBoxList, GUI.skin.button);
		GUILayout.Space(10f);
		GUI.enabled = SCORE > HIGHSCORE;
		if (GUILayout.Button("SAVE", new GUILayoutOption[1] { GUILayout.Width(100f) }))
		{
			HIGHSCORE = SCORE;
			PlayerPrefs.SetInt("GAExampleScore", SCORE);
			PlayerPrefs.Save();
			GA_User.Gender gender = GA_User.Gender.Unknown;
			if (_genderIndex == 1)
			{
				gender = GA_User.Gender.Female;
			}
			else if (_genderIndex == 2)
			{
				gender = GA_User.Gender.Male;
			}
			GA.API.User.NewUser(gender, _birthYear, _friendCount);
			GA_Queue.ForceSubmit();
		}
		GUI.enabled = true;
		if (GUILayout.Button("RESTART", new GUILayoutOption[1] { GUILayout.Width(100f) }))
		{
			GAMEOVER = false;
			SCORE = 0;
			Object val = Object.Instantiate((Object)(object)BallPrefab, new Vector3(0f, 3f, 0f), Quaternion.identity);
			GameObject val2 = (GameObject)(object)((val is GameObject) ? val : null);
			((Object)val2).name = "Ball";
			((Component)this).transform.position = _startingPosition;
			((Component)this).rigidbody.velocity = Vector3.zero;
		}
		if (!genderComboBoxControl.IsShowingList())
		{
			GUI.DrawTexture(new Rect(126f, 156f, 55f, 55f), (Texture)(object)GA.SettingsGA.Logo);
		}
		GUI.FocusWindow(0);
	}
}
