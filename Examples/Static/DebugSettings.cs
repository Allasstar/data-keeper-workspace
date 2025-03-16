using System.Collections.Generic;
using DataKeeper.Attributes;

[StaticClassInspector("Debug")]
public static class DebugSettings
{
    public static bool EnableLogs = true;
    public static bool DrawGizmos = true;
    public static LogLevel CurrentLogLevel = LogLevel.Warning;
    
    public static List<string> TestStringList = new List<string>();
    public static List<int> TestintList = new List<int>();
    
    public enum LogLevel
    {
        Info,
        Warning,
        Error
    }
}