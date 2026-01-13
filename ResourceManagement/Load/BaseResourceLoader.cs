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
            IGameLogger logger) :
            base(logger)
        {
            _paths = paths;
        }

        public async Task<Result<T>> LoadAsync(string key)
        {
            Result<string> pathResult = _paths.GetPath(ResourceType, key);

            if(pathResult.IsFailed)
                return Result.Fail(pathResult.Errors);

            return await Load(pathResult.Value);
        }

        public async Task<Result<T>> LoadAsync(AssetReference key) =>
            await Load(key);
        
        protected virtual async  Task<Result<T>> Load(object key)
        {
            Result<T> loadResult = await LoadAssetAsync<T>(key);

            return loadResult.IsFailed
                ? Result.Fail<T>(loadResult.Errors)
                : Result.Ok(loadResult.Value);
        }

    }

}