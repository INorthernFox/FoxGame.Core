using System.Threading.Tasks;
using Core.Loggers;
using Core.ResourceManagement.Load.Data;
using Core.ResourceManagement.Load.interfaces;
using FluentResults;
using UnityEngine;

namespace Core.ResourceManagement.Load
{
    public abstract class ComponentBaseResourceLoader<T> : BaseResourceLoader<T>
        where T : Component
    {
        protected ComponentBaseResourceLoader(
            AdressablesPathsConfig paths,
            IGameLogger logger,
            IAddressableRegistry registry = null,
            AddressableRetryConfig retryConfig = null)
            : base(paths, logger, registry, retryConfig)
        {
        }

        protected override async Task<Result<T>> Load(object key)
        {
            Result<GameObject> loadResult = await LoadAssetAsync<GameObject>(key);

            if (loadResult.IsFailed)
                return Result.Fail<T>(loadResult.Errors);

            if (!loadResult.Value.TryGetComponent(out T component))
                return Result.Fail<T>($"Can't find component {typeof(T).Name}");

            return Result.Ok(component);
        }

        protected override async Task<Result<IAddressableHandle<T>>> LoadWithHandle(object key)
        {
            Result<GameObject> loadResult = await LoadAssetAsync<GameObject>(key);

            if (loadResult.IsFailed)
                return Result.Fail(loadResult.Errors);

            if (!loadResult.Value.TryGetComponent(out T component))
                return Result.Fail<IAddressableHandle<T>>($"Can't find component {typeof(T).Name}");

            var handle = new AddressableHandle<T>(key, component, this);
            return Result.Ok<IAddressableHandle<T>>(handle);
        }
    }
}
