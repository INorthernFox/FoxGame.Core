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
        private readonly PersonalizedLogger _logger;

        protected AddressableService(IGameLogger logger) =>
            _logger = new PersonalizedLogger(logger, IGameLogger.LogSystems.ResourceManager, nameof(AddressableService), this);

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
            if (key == null)
            {
                _logger.LogError("Addressable key cannot be null");
                return Result.Fail<T>("Addressable key cannot be null.");
            }

            try
            {
                if (_handles.TryGetValue(key, out var cachedHandle))
                {
                    if (!cachedHandle.IsValid())
                    {
                        _handles.Remove(key);
                    }
                    else
                    {
                        var typedCachedHandle = cachedHandle.Convert<T>();
                        return typedCachedHandle.IsDone
                            ? typedCachedHandle.Result
                            : await typedCachedHandle.Task;
                    }
                }

                var handle = Addressables.LoadAssetAsync<T>(key);
                _handles[key] = handle;

                return await handle.Task;
            }
            catch (Exception exception)
            {
                _logger.LogError($"Failed to load addressable asset with key {key}: {exception}");
                return Result.Fail<T>($"Failed to load addressable asset with key {key}: {exception}");
            }
        }
    }
}