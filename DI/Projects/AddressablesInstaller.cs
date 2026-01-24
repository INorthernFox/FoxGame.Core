using Core.ResourceManagement;
using Core.ResourceManagement.Load;
using Core.ResourceManagement.Load.Data;
using Core.ResourceManagement.Load.interfaces;
using UnityEngine;
using Zenject;

namespace Core.DI.Projects
{
    public class AddressablesInstaller : MonoBehaviour
    {
        [SerializeField] private AdressablesPathsConfig _adressablesPathsConfig;
        [SerializeField] private AddressableRetryConfig _retryConfig;

        public void InstallBindings(DiContainer container)
        {
            container.Bind<AdressablesPathsConfig>().FromInstance(_adressablesPathsConfig).AsSingle();

            if (_retryConfig != null)
            {
                container.Bind<AddressableRetryConfig>().FromInstance(_retryConfig).AsSingle();
            }

            container.Bind<IAddressableRegistry>().To<AddressableRegistry>().AsSingle();
            container.Bind<AddressableCleanupService>().AsSingle().NonLazy();
        }
    }
}
