using System;
using UnityEngine;

namespace Core.Loggers.Dev
{
    [Serializable]
    public class LogSystemSettings
    {
        [HideInInspector] public string Name;
        public IGameLogger.LogSystems Type;
        public IGameLogger.LogLevel Level;
        public Color Color = Color.white;
    }
}