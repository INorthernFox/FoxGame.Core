using Core.ResourceManagement.Load.Data;
using UnityEngine;
using Zenject;

namespace Core.DI.Projects
{
    public class AddressablesInstaller : MonoBehaviour
    {
        public AdressablesPathsConfig AdressablesPathsConfig;

        public void InstallBindings(DiContainer container)
        {
            container.Bind<AdressablesPathsConfig>().FromInstance(AdressablesPathsConfig).AsSingle();
        }
    }
}
