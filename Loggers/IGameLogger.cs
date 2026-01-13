using System;

namespace Core.Loggers
{
    public interface IGameLogger
    {
        public void LogError(LogSystems logSystems, string msg, string key, object parent = null);
        public void LogWarning(LogSystems logSystems, string msg, string key, object parent = null);
        public void LogInfo(LogSystems logSystems, string msg, string key, object parent = null);
        public void Log(LogSystems logSystems, LogLevel level, string msg, string key, object parent = null);

        [Flags]
        public enum LogLevel
        {
            Non = 0,
            Error = 1 << 0,
            Warning = 1 << 1,
            Info = 1 << 2,
            All     = ~0    
        }
        
        public enum LogSystems
        {
            SceneManager = 0,
            GameStateMachine = 1,
            ResourceManager = 2,
            UIWindow = 3,
        }
    }

}
