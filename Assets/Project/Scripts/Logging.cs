public static class Logging
{
    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void Log(object message) => UnityEngine.Debug.Log(message);

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    internal static void LogWarning(object message) => UnityEngine.Debug.LogWarning(message);

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    internal static void LogError(object message) => UnityEngine.Debug.LogError(message);
}