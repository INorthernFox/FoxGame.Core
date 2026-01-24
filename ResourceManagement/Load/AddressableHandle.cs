using System;
using Core.ResourceManagement.Load.interfaces;

namespace Core.ResourceManagement.Load
{
    public sealed class AddressableHandle<T> : IAddressableHandle<T>
    {
        private readonly AddressableService _service;
        private bool _disposed;

        public object Key { get; }
        public T Asset { get; }
        public bool IsValid => !_disposed && Asset != null;

        public AddressableHandle(object key, T asset, AddressableService service)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Asset = asset;
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            _service.Release(Key);
        }
    }
}
