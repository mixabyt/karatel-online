internal class Log
{
	public delegate void LogDelegate(string msg);

	private static LogDelegate logger;

	public static void SetLog(LogDelegate ld)
	{
		logger = ld;
	}

	public static void Msg(string msg)
	{
		if (logger != null)
		{
			logger(msg);
		}
	}
}
