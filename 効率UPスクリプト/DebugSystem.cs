public static class DebugSystem
{
#if !(UNITY_EDITOR || DEVELOPMENT_BUILD)
	[System.Diagnostics.Conditional("MY_DEBUG_SYSTEM")]
#endif
	public static void Log(string msg)
	{
		UnityEngine.Debug.Log(msg);
	}
}
