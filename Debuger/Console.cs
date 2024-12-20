using DataKeeper.Generic;
using UnityEngine;

namespace DataKeeper.Debuger
{
    public static class Console
    {
        public static readonly ReactiveList<LogData> ReactiveLogData = new ReactiveList<LogData>();
        
        private static bool _isStarted = false;

        
        public static void Start()
        {
            if(_isStarted) return;
            _isStarted = true;
            
            Clear();
            Application.logMessageReceived += LogMessageReceived;
        }

        public static void Stop()
        {
            _isStarted = false;
            Application.logMessageReceived -= LogMessageReceived;
        }

        public static void Clear()
        {
            ReactiveLogData.Clear();
        }

        private static void LogMessageReceived(string condition, string stacktrace, LogType type)
        {
            ReactiveLogData.Add(new LogData(type, condition, stacktrace));
        }
        
        public static Color GetLogColor(LogData data)
        {
            return data.Type switch
            {
                LogType.Error => Color.red,
                LogType.Assert => Color.blue,
                LogType.Warning => Color.yellow,
                LogType.Log => Color.cyan,
                LogType.Exception => Color.magenta,
                _ => Color.black
            };
        }
    }

    public class LogData
    {
        public LogType Type { get; private set; }
        public string Condition { get; private set; }
        public string Stacktrace { get; private set; }
        
        public LogData(LogType type, string condition, string stacktrace)
        {
            Type = type;
            Condition = condition;
            Stacktrace = stacktrace;
        }
    };
}
