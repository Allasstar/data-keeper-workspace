using System;
using System.Collections.Generic;
using DataKeeper.Extensions;
using DataKeeper.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace DataKeeper.Debuger
{
    public class DebugCanvas : MonoBehaviour
    {
        [SerializeField] private LogButton _logButtonTemplate;
        [SerializeField] private Transform _logContainer;

        private ObjectPool<LogButton> _logButtonPool;
        private List<LogButton> _logButtonUI = new List<LogButton>();

        private List<LogType> _logTypeFilter = new List<LogType>() { LogType.Log, LogType.Warning, LogType.Error, LogType.Assert, LogType.Exception };
        private string _logFilter = "";
        
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            _logButtonPool = new ObjectPool<LogButton>(CreateFunc, ActionOnGet, ActionOnRelease);
        }

        private void ActionOnRelease(LogButton logButton)
        {
            logButton.gameObject.SetActive(false);
        }

        private void ActionOnGet(LogButton logButton)
        {
            logButton.transform.SetAsLastSibling();
            logButton.Reset();
            logButton.transform.localScale = Vector3.one;
        }

        private LogButton CreateFunc()
        {
            var logButton = Instantiate(_logButtonTemplate);
            logButton.transform.parent = _logContainer;
            logButton.transform.localScale = Vector3.one;
            logButton.transform.SetAsLastSibling();

            return logButton;
        }

        private void OnEnable()
        {
            Console.ReactiveLogData.AddListener(Log);
        }

        private void OnDisable()
        {
            Console.ReactiveLogData.RemoveListener(Log);
        }
        
        private void Log(int index, LogData data, ListChangedEvent change)
        {
            if (change == ListChangedEvent.Cleared)
            {
                ClearUI();
                return;
            }
            
            if(data == null || change == ListChangedEvent.Removed) return;

            var logButton = _logButtonPool.Get();
            _logButtonUI.Add(logButton);

            var color = Console.GetLogColor(data);

            logButton.SetLog(data.Type, color, data.Condition, Time.time.ToString(), data.Stacktrace);
            logButton.gameObject.SetActive(IsNotFiltered(logButton));
        }

        private void ClearUI()
        {
            foreach (var logButton in _logButtonUI)
            {
                _logButtonPool.Release(logButton);
            }
            
            _logButtonUI.Clear();
        }

        public void ClearLogs()
        {
            Console.ReactiveLogData.Clear();
        }

        public void OnLogFilter(bool isOn)
        {
            var logType = LogType.Log;

            if (isOn)
            {
                if (!_logTypeFilter.Contains(logType))
                {
                    _logTypeFilter.Add(logType);
                }
            }
            else
            {
                if (_logTypeFilter.Contains(logType))
                {
                    _logTypeFilter.Remove(logType);
                }
            }
            
            Filter();
        }
        
        public void OnWarningFilter(bool isOn)
        {
            var logType = LogType.Warning;
            
            if (isOn)
            {
                if (!_logTypeFilter.Contains(logType))
                {
                    _logTypeFilter.Add(logType);
                }
            }
            else
            {
                if (_logTypeFilter.Contains(logType))
                {
                    _logTypeFilter.Remove(logType);
                }
            }
            
            Filter();
        }
        
        public void OnErrorFilter(bool isOn)
        {
            var logType = LogType.Error;
            
            if (isOn)
            {
                if (!_logTypeFilter.Contains(logType))
                {
                    _logTypeFilter.Add(logType);
                }
            }
            else
            {
                if (_logTypeFilter.Contains(logType))
                {
                    _logTypeFilter.Remove(logType);
                }
            }
            
            Filter();
        }

        public void OnStringFilter(string logFilter)
        {
            _logFilter = logFilter;
            Filter();
        }

        private void Filter()
        {
            foreach (var logButton in _logButtonUI)
            {
                logButton.SetGameObjectActive(IsNotFiltered(logButton));
            }
        }

        private bool IsNotFiltered(LogButton logButton)
        {
            return _logTypeFilter.Contains(logButton.LogType) 
                   && (_logFilter == "" || logButton.LogText.Contains(_logFilter, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
