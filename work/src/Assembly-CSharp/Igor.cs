using System.Text;
using UnityEngine;

public class Igor : bs
{
	public static Igor Instance;

	public bool debug;

	public AudioClip shootSound;

	public AudioClip[] hitSound;

	public AudioClip[] damageSound;

	public AudioClip grenadeHit;

	public AudioClip fallSound;

	public AudioClip qdamageShoot;

	public AudioClip takeSound;

	public GameObject concerne;

	public GameObject hole;

	public GameObject explosion;

	public Bullet bullet;

	public AudioClip reload;

	public GameObject damageText;

	public bool autojoin;

	public bool autoJoin2;

	public bool autoChangeUser;

	public bool secondUser;

	internal bool enableLog = true;

	public bool teamBattle;

	public bool debugVersion;

	public Texture2D black;

	public bool Deathmatch => TeamBattle.Instance is Deathmatch;

	public string playerName => PhotonNetwork.player.name;

	private void Awake()
	{
		if (!Debug.isDebugBuild)
		{
			debug = false;
		}
		if (!debug)
		{
			debug = (teamBattle = (enableLog = (autojoin = (autoJoin2 = false))));
		}
		Instance = this;
		if (debug)
		{
			QualitySettings.vSyncCount = 1;
		}
	}

	private void Update()
	{
		((Behaviour)((Component)this).guiText).enabled = enableLog;
		((Component)this).guiText.text = bs.stringBuilder.ToString();
		bs.stringBuilder = new StringBuilder();
	}
}
