using Core.GameSettings.Providers;
using UnityEngine;
using Zenject;

namespace Core.DI.Projects
{
    public class GeneralGameSettingInstaller : MonoBehaviour
    {
        public void InstallBindings(DiContainer container)
        {
            container.Bind<GeneralGameSettingProvider>().AsSingle();
        }
    }
}