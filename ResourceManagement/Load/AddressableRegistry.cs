using System.Collections.Generic;
using Core.ResourceManagement.Load.interfaces;
using Core.SceneManagers.Data;

namespace Core.ResourceManagement.Load
{
    public sealed class AddressableRegistry : IAddressableRegistry
    {
        private readonly HashSet<AddressableService> _services = new();

        public IReadOnlyCollection<AddressableService> RegisteredServices => _services;

        public void Register(AddressableService service)
        {
            if (service != null)
            {
                _services.Add(service);
            }
        }

        public void Unregister(AddressableService service)
        {
            if (service != null)
            {
                _services.Remove(service);
            }
        }

        public void ReleaseAllByScene(SceneType scene)
        {
            foreach (var service in _services)
            {
                service.ReleaseByScene(scene);
            }
        }
    }
}
