using UnityEngine;

public class Hud : bs
{
	public GUIText Health;

	public GUIText Shield;

	public GUIText BulletText;

	public GUIText BlueTeam;

	public GUIText RedTeam;

	public GUITexture BlueTeamt;

	public GUITexture RedTeamt;

	public GUIText Time;

	public GUITexture[] WeaponBG;

	public GUITexture[] MainWeapon = new GUITexture[2];

	public GUITexture[] Grenade;

	public GUITexture BulletIcon;

	public Texture2D Background;

	public Texture2D BackgroundNoAmmo;

	public Texture2D BackgroundHollow;

	public Texture2D defBulletIcon;

	public void Update()
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)bs._WorldGameObject.MainPlayer == (Object)null)
		{
			return;
		}
		GUIText blueTeam = BlueTeam;
		bool flag = !bs._Igor.DeathMatch;
		((Behaviour)RedTeamt).enabled = flag;
		flag = flag;
		((Behaviour)RedTeam).enabled = flag;
		flag = flag;
		((Behaviour)BlueTeamt).enabled = flag;
		((Behaviour)blueTeam).enabled = flag;
		PlayerNetwork component = bs._WorldGameObject.MainPlayer.GetComponent<PlayerNetwork>();
		for (int i = 0; i < MainWeapon.Length; i++)
		{
			if (((GUIElement)MainWeapon[i]).HitTest(Input.mousePosition) && Input.GetMouseButton(0) && bs._MainMenu.showMenu && bs._MainMenu.curWindow == MainMenu.Windows.SelectWeapon)
			{
				component.MainWeapon[i] = null;
				MainWeapon[i].texture = null;
			}
		}
		for (int j = 0; j < MainWeapon.Length; j++)
		{
			Gun gun = component.MainWeapon[j];
			if ((Object)(object)gun != (Object)null)
			{
				MainWeapon[j].texture = (Texture)(object)((gun.bullets <= 0) ? gun.weaponIconGrey : gun.weaponIcon);
			}
			else
			{
				MainWeapon[j].texture = null;
			}
		}
		GUITexture[] grenade = Grenade;
		foreach (GUITexture val in grenade)
		{
			((Behaviour)val).enabled = component.Grenade.bullets > 0;
		}
	}
}
