using System.Threading.Tasks;
using Core.Loggers;
using Core.ResourceManagement.Load.Data;
using Core.ResourceManagement.Load.interfaces;
using FluentResults;
using UnityEngine.AddressableAssets;

namespace Core.ResourceManagement.Load
{
    public abstract class BaseResourceLoader<T> : AddressableService, IResourceLoader<T>
    {
        private readonly AdressablesPathsConfig _paths;

        protected abstract ResourceType ResourceType { get; }

        protected BaseResourceLoader(
            AdressablesPathsConfig paths,
            IGameLogger logger,
            IAddressableRegistry registry = null,
            AddressableRetryConfig retryConfig = null) :
            base(logger, retryConfig)
        {
            _paths = paths;
            registry?.Register(this);
        }

        public async Task<Result<T>> LoadAsync(string key)
        {
            Result<string> pathResult = _paths.GetPath(ResourceType, key);

            if (pathResult.IsFailed)
                return Result.Fail(pathResult.Errors);

            return await Load(pathResult.Value);
        }

        public async Task<Result<T>> LoadAsync(AssetReference key) =>
            await Load(key);

        public async Task<Result<IAddressableHandle<T>>> LoadWithHandleAsync(string key)
        {
            Result<string> pathResult = _paths.GetPath(ResourceType, key);

            if (pathResult.IsFailed)
                return Result.Fail(pathResult.Errors);

            return await LoadWithHandle(pathResult.Value);
        }

        public async Task<Result<IAddressableHandle<T>>> LoadWithHandleAsync(AssetReference key) =>
            await LoadWithHandle(key);

        protected virtual async Task<Result<T>> Load(object key)
        {
            Result<T> loadResult = await LoadAssetAsync<T>(key);

            return loadResult.IsFailed
                ? Result.Fail<T>(loadResult.Errors)
                : Result.Ok(loadResult.Value);
        }

        protected virtual async Task<Result<IAddressableHandle<T>>> LoadWithHandle(object key)
        {
            Result<T> loadResult = await LoadAssetAsync<T>(key);

            if (loadResult.IsFailed)
                return Result.Fail(loadResult.Errors);

            var handle = new AddressableHandle<T>(key, loadResult.Value, this);
            return Result.Ok<IAddressableHandle<T>>(handle);
        }
    }
}
