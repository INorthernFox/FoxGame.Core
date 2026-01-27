using Core.ConfigProviders;
using Core.ConfigProviders.GeneralConfigs;
using UnityEngine;
using Zenject;

namespace Core.DI.Projects
{
    public class ConfigsInstaller : MonoBehaviour
    {
        public void InstallBindings(DiContainer container)
        {
            container.Bind<IConfigsService>()
                .To<ConfigsService>()
                .AsSingle();

            container.Bind<IConfigsProvider>()
                .To<GeneralConfigsProvider>()
                .AsSingle();
        }
    }
}