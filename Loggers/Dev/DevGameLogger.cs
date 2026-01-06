using System;
using System.Text;
using UnityEngine;

namespace Core.Loggers.Dev
{
    public class DevGameLogger : IGameLogger
    {
        private readonly DevGameLoggerSettings _settings;

        public DevGameLogger(DevGameLoggerSettings settings)
        {
            _settings = settings;
        }

        public void LogError(IGameLogger.LogSystems logSystems, string msg, string key, object parent = null) =>
            Log(logSystems, IGameLogger.LogLevel.Error, msg, key, parent);

        public void LogWarning(IGameLogger.LogSystems logSystems, string msg, string key, object parent = null) =>
            Log(logSystems, IGameLogger.LogLevel.Warning, msg, key, parent);

        public void LogInfo(IGameLogger.LogSystems logSystems, string msg, string key, object parent = null) =>
            Log(logSystems, IGameLogger.LogLevel.Info, msg, key, parent);

        public void Log(IGameLogger.LogSystems logSystems, IGameLogger.LogLevel level, string msg, string key, object parent = null)
        {
            if(!_settings.IsEnabledLevel(level) || !_settings.IsEnabledSystem(logSystems, level))
                return;

            Color color = _settings.GetColor(logSystems);
            string startText = $"[{Paint(level.ToString().ToUpper(), color)}]-[{Paint(logSystems.ToString(), color)}]";
            StringBuilder result = new( startText);
      
            result.Append(" =>  ");
            result.Append(msg);
            result.Append($"\n[{Paint(key, color)}]");
            
            if(parent != null)
                result.Append($"-[{Paint(parent.GetType().Name, color)}]");
            
            switch(level)
            {
                case IGameLogger.LogLevel.Error:
                    Debug.LogError(result.ToString());
                    break;
                case IGameLogger.LogLevel.Warning:
                    Debug.LogWarning(result.ToString());
                    break;
                case IGameLogger.LogLevel.Info:
                    Debug.Log(result.ToString());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }

        private string Paint(string msg, Color color) =>
            $"<color=#{(byte) (color.r * 255f):X2}{(byte) (color.g * 255f):X2}{(byte) (color.b * 255f):X2}>{msg}</color>";
    }

}