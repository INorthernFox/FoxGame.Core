using System.Collections.Generic;
using Core.Loggers;

namespace Core.FileEditor.Tests.EditMode.Mocks
{
    public sealed class MockGameLogger : IGameLogger
    {
        public List<LogEntry> LogEntries { get; } = new List<LogEntry>();

        public void LogError(IGameLogger.LogSystems logSystems, string msg, string key, object parent = null)
        {
            Log(logSystems, IGameLogger.LogLevel.Error, msg, key, parent);
        }

        public void LogWarning(IGameLogger.LogSystems logSystems, string msg, string key, object parent = null)
        {
            Log(logSystems, IGameLogger.LogLevel.Warning, msg, key, parent);
        }

        public void LogInfo(IGameLogger.LogSystems logSystems, string msg, string key, object parent = null)
        {
            Log(logSystems, IGameLogger.LogLevel.Info, msg, key, parent);
        }

        public void Log(IGameLogger.LogSystems logSystems, IGameLogger.LogLevel level, string msg, string key, object parent = null)
        {
            LogEntries.Add(new LogEntry(logSystems, level, msg, key, parent));
        }

        public void Clear()
        {
            LogEntries.Clear();
        }

        public bool HasError(string messageContains = null)
        {
            return HasLogOfLevel(IGameLogger.LogLevel.Error, messageContains);
        }

        public bool HasWarning(string messageContains = null)
        {
            return HasLogOfLevel(IGameLogger.LogLevel.Warning, messageContains);
        }

        public bool HasInfo(string messageContains = null)
        {
            return HasLogOfLevel(IGameLogger.LogLevel.Info, messageContains);
        }

        private bool HasLogOfLevel(IGameLogger.LogLevel level, string messageContains)
        {
            foreach (var entry in LogEntries)
            {
                if (entry.Level == level)
                {
                    if (string.IsNullOrEmpty(messageContains) || entry.Message.Contains(messageContains))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public sealed class LogEntry
        {
            public IGameLogger.LogSystems System { get; }
            public IGameLogger.LogLevel Level { get; }
            public string Message { get; }
            public string Key { get; }
            public object Parent { get; }

            public LogEntry(IGameLogger.LogSystems system, IGameLogger.LogLevel level, string message, string key, object parent)
            {
                System = system;
                Level = level;
                Message = message;
                Key = key;
                Parent = parent;
            }
        }
    }
}
