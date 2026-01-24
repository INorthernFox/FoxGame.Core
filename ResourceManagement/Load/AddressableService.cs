using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Loggers;
using Core.ResourceManagement.Load.Data;
using Core.SceneManagers.Data;
using FluentResults;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.ResourceManagement.Load
{
    public class AddressableService
    {
        private readonly Dictionary<object, HandleEntry> _handles = new();
        private readonly PersonalizedLogger _logger;
        private readonly AddressableRetryConfig _retryConfig;

        protected AddressableService(IGameLogger logger, AddressableRetryConfig retryConfig = null)
        {
            _logger = new PersonalizedLogger(logger, IGameLogger.LogSystems.ResourceManager, nameof(AddressableService), this);
            _retryConfig = retryConfig;
        }

        public Result Unload(string key)
        {
            if (!_handles.TryGetValue(key, out HandleEntry entry))
            {
                return Result.Ok();
            }

            if (entry.Handle.IsValid())
            {
                Addressables.Release(entry.Handle);
            }

            _handles.Remove(key);
            return Result.Ok();
        }

        public Result UnloadAll()
        {
            foreach (HandleEntry entry in _handles.Values.Where(entry => entry.Handle.IsValid()))
            {
                Addressables.Release(entry.Handle);
            }

            _handles.Clear();
            return Result.Ok();
        }

        public void Acquire(object key)
        {
            if (key == null)
                return;

            if (_handles.TryGetValue(key, out HandleEntry entry))
            {
                _handles[key] = entry.IncrementReference();
                _logger.LogInfo($"Acquired reference for {key}, count: {_handles[key].ReferenceCount}");
            }
        }

        public void Release(object key)
        {
            if (key == null)
                return;

            if (!_handles.TryGetValue(key, out HandleEntry entry))
                return;

            entry = entry.DecrementReference();
            _handles[key] = entry;

            _logger.LogInfo($"Released reference for {key}, count: {entry.ReferenceCount}");

            if (entry.CanRelease)
            {
                if (entry.Handle.IsValid())
                {
                    Addressables.Release(entry.Handle);
                    _logger.LogInfo($"Fully released addressable {key}");
                }
                _handles.Remove(key);
            }
        }

        public void ReleaseByScene(SceneType scene)
        {
            var keysToRelease = _handles
                .Where(kvp => kvp.Value.OwnerScene == scene)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (object key in keysToRelease)
            {
                if (_handles.TryGetValue(key, out HandleEntry entry) && entry.Handle.IsValid())
                {
                    Addressables.Release(entry.Handle);
                    _logger.LogInfo($"Released addressable {key} for scene {scene}");
                }
                _handles.Remove(key);
            }

            _logger.LogInfo($"Released {keysToRelease.Count} addressables for scene {scene}");
        }

        public AddressableMetrics GetMetrics()
        {
            int totalHandles = _handles.Count;
            int totalReferences = _handles.Values.Sum(e => e.ReferenceCount);
            var sceneBreakdown = _handles
                .Where(kvp => kvp.Value.OwnerScene.HasValue)
                .GroupBy(kvp => kvp.Value.OwnerScene.Value)
                .ToDictionary(g => g.Key, g => g.Count());

            return new AddressableMetrics(totalHandles, totalReferences, sceneBreakdown);
        }

        protected async Task<Result<T>> LoadAssetAsync<T>(object key, SceneType? ownerScene = null)
        {
            if (key == null)
            {
                _logger.LogError("Addressable key cannot be null");
                return Result.Fail<T>("Addressable key cannot be null.");
            }

            if (_handles.TryGetValue(key, out var cachedEntry))
            {
                if (!cachedEntry.IsValid)
                {
                    _handles.Remove(key);
                }
                else
                {
                    _handles[key] = cachedEntry.IncrementReference();
                    _logger.LogInfo($"Cache hit for {key}, ref count: {_handles[key].ReferenceCount}");

                    var typedCachedHandle = cachedEntry.Handle.Convert<T>();
                    return typedCachedHandle.IsDone
                        ? typedCachedHandle.Result
                        : await typedCachedHandle.Task;
                }
            }

            return await LoadWithRetry<T>(key, ownerScene);
        }

        private async Task<Result<T>> LoadWithRetry<T>(object key, SceneType? ownerScene)
        {
            int maxAttempts = _retryConfig?.MaxRetryAttempts ?? 1;
            int baseDelayMs = _retryConfig?.BaseDelayMs ?? 100;
            int maxDelayMs = _retryConfig?.MaxDelayMs ?? 2000;
            bool useExponentialBackoff = _retryConfig?.UseExponentialBackoff ?? false;

            Exception lastException = null;

            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    var handle = Addressables.LoadAssetAsync<T>(key);
                    var result = await handle.Task;

                    var entry = new HandleEntry(handle, ownerScene);
                    _handles[key] = entry;

                    _logger.LogInfo($"Loaded addressable {key}, owner scene: {ownerScene}");
                    return Result.Ok(result);
                }
                catch (Exception exception)
                {
                    lastException = exception;
                    _logger.LogWarning($"Failed to load addressable {key} (attempt {attempt}/{maxAttempts}): {exception.Message}");

                    if (attempt < maxAttempts)
                    {
                        int delay = useExponentialBackoff
                            ? Math.Min(baseDelayMs * (int)Math.Pow(2, attempt - 1), maxDelayMs)
                            : baseDelayMs;

                        await Task.Delay(delay);
                    }
                }
            }

            _logger.LogError($"Failed to load addressable asset with key {key} after {maxAttempts} attempts: {lastException}");
            return Result.Fail<T>($"Failed to load addressable asset with key {key}: {lastException}");
        }
    }
}
