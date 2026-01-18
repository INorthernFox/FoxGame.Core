using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Loggers;
using FluentResults;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.ResourceManagement.Load
{
    public class AddressableService
    {
        private readonly Dictionary<object, AsyncOperationHandle> _handles = new();
        private readonly IGameLogger _logger;

        private IGameLogger.LogSystems LogSystem =>
            IGameLogger.LogSystems.ResourceManager;

        protected AddressableService(IGameLogger logger)
        {
            _logger = logger;
        }

        public Result Unload(string key)
        {
            if(!_handles.TryGetValue(key, out AsyncOperationHandle handle))
            {
                return Result.Ok();
            }

            if(handle.IsValid())
            {
                Addressables.Release(handle);
            }

            _handles.Remove(key);
            return Result.Ok();
        }

        public Result UnloadAll()
        {
            foreach(AsyncOperationHandle handle in _handles.Values.Where(handle => handle.IsValid()))
            {
                Addressables.Release(handle);
            }

            _handles.Clear();
            return Result.Ok();
        }

        protected async Task<Result<T>> LoadAssetAsync<T>(object key)
        {
            if(key == null)
            {
                _logger.LogError(LogSystem, "Addressable key cannot be null", nameof(LoadAssetAsync), this);
                return Result.Fail<T>("Addressable key cannot be null.");
            }

            try
            {
                if(_handles.TryGetValue(key, out AsyncOperationHandle cachedHandle))
                {
                    if(!cachedHandle.IsValid())
                    {
                        _handles.Remove(key);
                    }
                    else
                    {
                        AsyncOperationHandle<T> typedCachedHandle = cachedHandle.Convert<T>();
                        if(typedCachedHandle.IsDone)
                        {
                            return typedCachedHandle.Result;
                        }

                        return await typedCachedHandle.Task;
                    }
                }

                AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(key);
                _handles[key] = handle;

                return await handle.Task;
            }
            catch (Exception exception)
            {
                _logger.LogError(LogSystem, $"Failed to load addressable asset with key {key}: {exception}", nameof(LoadAssetAsync), this);
                return Result.Fail<T>($"Failed to load addressable asset with key {key}: {exception}");
            }
        }
    }
}