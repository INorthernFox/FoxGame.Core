using System;

namespace Core.ResourceManagement.Load.interfaces
{
    public interface IAddressableHandle : IDisposable
    {
        object Key { get; }
        bool IsValid { get; }
    }

    public interface IAddressableHandle<out T> : IAddressableHandle
    {
        T Asset { get; }
    }
}
