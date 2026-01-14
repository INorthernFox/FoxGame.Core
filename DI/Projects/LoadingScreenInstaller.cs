using Core.LoadingScreens;
using UnityEngine;
using Zenject;

namespace Core.DI.Projects
{
    public class LoadingScreenInstaller : MonoBehaviour
    {
        public LoadingScreenConfig LoadingScreenConfig;

        public void InstallBindings(DiContainer container)
        {
            container.Bind<LoadingScreenConfig>().FromInstance(LoadingScreenConfig).AsSingle();
            container.Bind<LoadingScreenLoader>().AsSingle();
            container.Bind<LoadingScreenService>().AsSingle();
        }
    }
}
