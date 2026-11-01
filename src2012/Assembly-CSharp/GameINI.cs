public class GameINI
{
	public enum MapSize
	{
		SMALL,
		MEDIUM,
		LARGE,
		HUGE
	}

	public enum MapType
	{
		STANDART,
		FLAT,
		SAND,
		OCEAN,
		ISLAND,
		SNOWLAND
	}

	public enum GameType
	{
		BUILDING,
		DEATHMATCH,
		TEAM_BATTLE,
		SURVIVAL
	}

	public MapSize mapSize;

	public MapType mapType;

	public string playerID;

	public int slotID;

	public bool loadingSavedMap;

	public int publicMapID;

	private GameType _gameType;

	public byte mapPopulation;

	public bool isServer;

	public string Password;

	public bool isWatch;

	public byte ServerRating;

	public string server_name;

	public string server_about;

	public bool isOnline;

	public bool gifts = true;

	public bool destroyable = true;

	internal bool _firstSelect = true;

	public GameType gameType
	{
		get
		{
			return bs._Igor.teamBattle ? GameType.DEATHMATCH : _gameType;
		}
		set
		{
			_gameType = value;
		}
	}
}
