using UnityEditor.PackageManager;

namespace Loggers
{
    public interface ILogger
    {
        public void LogError(string msg, string key, object parent = null);
        public void LogWarning(string msg, string key, object parent = null);
        public void LogInfo(string msg, string key, object parent = null);
        public void Log(LogLevel level, string msg, string key, object parent = null);
        
        public enum LogType
        {
            Non = 0,
            Error = 1,
            Warning = 2,
            Info = 3,
        }
    }
}