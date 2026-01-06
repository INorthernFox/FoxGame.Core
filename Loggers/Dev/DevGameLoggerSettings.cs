using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Loggers.Dev
{
    [CreateAssetMenu(fileName = "DevGameLoggerSettings", menuName = "Game Settings/Dev/Logger/Settings", order = 0)]
    public class DevGameLoggerSettings : ScriptableObject
    {
        [SerializeField]
        private IGameLogger.LogLevel _globalEnabledLevels =
            IGameLogger.LogLevel.Error
            | IGameLogger.LogLevel.Warning;

        [SerializeField]
        private LogSystemSettings[] _enabledSystemsType;
        private Dictionary<IGameLogger.LogSystems, IGameLogger.LogLevel> _systems;
        private Dictionary<IGameLogger.LogSystems, Color> _colors;

        public bool IsEnabledLevel(IGameLogger.LogLevel level) => (_globalEnabledLevels & level) != 0;
        public bool IsEnabledSystem(IGameLogger.LogSystems systemType, IGameLogger.LogLevel level)
        {
            _systems ??= BuildSystemsDictionary();
            _colors ??= BuildColorsDictionary();
            return _systems.TryGetValue(systemType, out IGameLogger.LogLevel levels) && (levels & level) != 0;
        }

        public Color GetColor(IGameLogger.LogSystems systemType)
        {
            _colors ??= BuildColorsDictionary();
            return _colors.TryGetValue(systemType, out Color color) ? color : Color.white;
        }

        private Dictionary<IGameLogger.LogSystems, IGameLogger.LogLevel> BuildSystemsDictionary() =>
            _enabledSystemsType.ToDictionary(entry => entry.Type, entry => entry.Level);

        private Dictionary<IGameLogger.LogSystems, Color> BuildColorsDictionary() =>
            _enabledSystemsType.ToDictionary(entry => entry.Type, entry => entry.Color);
        
        private void OnValidate()
        {
            if(Application.isPlaying)
                return;

            for( int i = 0; i < _enabledSystemsType.Length; i++ )
            {
                LogSystemSettings settings = _enabledSystemsType[i];
                settings.Name = settings.Type.ToString();
                _enabledSystemsType[i] = settings;
            }
        }
    }
}