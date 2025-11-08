using UnityEngine;

public class CustomLogger
{
    public static void Log(object message)
    {
        if (IsEditor())
            Debug.Log(message);
    }

    public static void Log(object message, Object context)
    {
        if (IsEditor())
            Debug.Log(message, context);
    }

    public static void LogWarning(object message)
    {
        if (IsEditor())
            Debug.LogWarning(message);
    }

    public static void LogWarning(object message, Object context)
    {
        if (IsEditor())
            Debug.LogWarning(message, context);
    }

    public static void LogError(object message)
    {
        if (IsEditor())
            Debug.LogError(message);
    }

    public static void LogError(object message, Object context)
    {
        if (IsEditor())
            Debug.LogError(message, context);
    }

    private static bool IsEditor()
    {
        return Application.isEditor;
    }
}