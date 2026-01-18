namespace Core.Loggers
{
    public class PersonalizedLogger : IGameLogger
    {
        private readonly IGameLogger _background;
        private readonly IGameLogger.LogSystems _logSystems;
        private readonly string _key;
        private readonly object _parent;

        public PersonalizedLogger(
            IGameLogger background,
            IGameLogger.LogSystems logSystems,
            string key,
            object parent = null)
        {
            _background = background;
            _logSystems = logSystems;
            _key = key;
            _parent = parent;

        }

        public void LogError(string msg, string key = null) =>
            LogError(_logSystems, msg, string.IsNullOrEmpty(key) ? _key : key, _parent);

        public void LogWarning(string msg, string key = null) =>
            LogWarning(_logSystems, msg, string.IsNullOrEmpty(key) ? _key : key, _parent);

        public void LogInfo(string msg, string key = null) =>
            LogInfo(_logSystems, msg, string.IsNullOrEmpty(key) ? _key : key, _parent);

        public void Log(IGameLogger.LogLevel level, string msg, string key = null) =>
            Log(_logSystems, level, msg, string.IsNullOrEmpty(key) ? _key : key, _parent);

        public void LogError(IGameLogger.LogSystems logSystems, string msg, string key, object parent = null) =>
            _background.LogError(logSystems, msg, key, parent);

        public void LogWarning(IGameLogger.LogSystems logSystems, string msg, string key, object parent = null) =>
            _background.LogWarning(logSystems, msg, key, parent);

        public void LogInfo(IGameLogger.LogSystems logSystems, string msg, string key, object parent = null) =>
            _background.LogInfo(logSystems, msg, key, parent);

        public void Log(IGameLogger.LogSystems logSystems, IGameLogger.LogLevel level, string msg, string key, object parent = null) =>
            _background.Log(logSystems, level, msg, key, parent);
    }
}