using Core.Loggers;
using Core.Loggers.Dev;
using UnityEngine;
using Zenject;

namespace Core.DI.Projects
{
    public class LoggerInstaller : MonoBehaviour
    {
        public DevGameLoggerSettings Settings;
        
        public  void InstallBindings(DiContainer container)
        {
            DevGameLogger logger = new(Settings);
            container.BindInterfacesTo<DevGameLogger>().FromInstance(logger).AsSingle();
        }
    }
}