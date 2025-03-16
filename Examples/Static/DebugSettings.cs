using DataKeeper.Attributes;

[StaticClassInspector("Debug")]
public static class DebugSettings
{
    public static bool EnableLogs = true;
    public static bool DrawGizmos = true;
    public static LogLevel CurrentLogLevel = LogLevel.Warning;
    
    public enum LogLevel
    {
        Info,
        Warning,
        Error
    }
}