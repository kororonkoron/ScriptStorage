using UnityEngine;
public static class DebugSystem
{
#if !(UNITY_EDITOR || DEVELOPMENT_BUILD)
	[System.Diagnostics.Conditional("MY_DEBUG_SYSTEM")]
#endif

	public static void Log(string msg)
	{
		Debug.Log(msg);
	}

	public static void LogFormat(string msg)
	{
		Debug.LogFormat(msg);
	}

	public static void LogWarning(string msg)
	{
		Debug.LogWarning(msg);
	}
	public static void LogWarning(System.Exception e)
	{
		Debug.LogWarning(e);
	}

	public static void LogError(string msg)
	{
		Debug.LogError(msg);
	}
}
