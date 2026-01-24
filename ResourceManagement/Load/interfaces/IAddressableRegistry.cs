using System.Collections.Generic;
using Core.SceneManagers.Data;

namespace Core.ResourceManagement.Load.interfaces
{
    public interface IAddressableRegistry
    {
        void Register(AddressableService service);
        void Unregister(AddressableService service);
        void ReleaseAllByScene(SceneType scene);
        IReadOnlyCollection<AddressableService> RegisteredServices { get; }
    }
}
