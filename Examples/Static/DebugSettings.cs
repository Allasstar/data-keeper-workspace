using System.Collections.Generic;
using DataKeeper.Attributes;

[StaticClassInspector("Debug")]
public static class DebugSettings
{
    private static bool EnableLogsPrivate = true;
    public static bool EnableLogs = true;
    public static bool DrawGizmos = true;
    public static LogLevel CurrentLogLevel = LogLevel.Warning;
    
    public static List<string> TestStringList = new List<string>() {"2", "4", "test"};
    public static List<int> TestIntList = new List<int>() {1, 3, 5, 6};
    public static bool[] TestBoolArray = new bool[4] { false, false, true, false };
    
    public enum LogLevel
    {
        Info,
        Warning,
        Error
    }
}