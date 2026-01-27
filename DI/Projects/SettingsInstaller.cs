using Core.GameSettings;
using Core.GameSettings.Providers;
using UnityEngine;
using Zenject;

namespace Core.DI.Projects
{
    public class SettingsInstaller : MonoBehaviour
    {
        public void InstallBindings(DiContainer container)
        {
            container.Bind<ISettingsService>()
                .To<SettingsService>()
                .AsSingle();
        }
    }
}
