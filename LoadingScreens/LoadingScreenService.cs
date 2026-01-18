using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Loggers;
using FluentResults;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace Core.LoadingScreens
{
    public class LoadingScreenService
    {
        private const string LogKey = nameof(LoadingScreenService);

        private readonly LoadingScreenLoader _loadingScreenLoader;
        private readonly LoadingScreenConfig _loadingScreenConfig;
        private readonly IGameLogger _logger;
        private LoadingScreenRoot _root;

        private readonly Dictionary<LoadingScreenType, LoadingScreen> _screens = new();

        private static IGameLogger.LogSystems LogSystem => IGameLogger.LogSystems.UIWindow;

        public LoadingScreenService(
            LoadingScreenLoader loadingScreenLoader,
            LoadingScreenConfig loadingScreenConfig,
            IGameLogger logger)
        {
            _loadingScreenLoader = loadingScreenLoader;
            _loadingScreenConfig = loadingScreenConfig;
            _logger = logger;
        }

        public void Initialize()
        {
            _root = new GameObject("LoadingScreens").AddComponent<LoadingScreenRoot>();
        }

        public async Task<Result> Preload(params LoadingScreenType[] types)
        {
            _logger.LogInfo(LogSystem, $"Preloading {types.Length} loading screen(s)", LogKey, this);

            List<AssetReference> refs = new(types.Length);

            foreach(LoadingScreenType type in types)
            {
                Result<AssetReferenceGameObject> getRef = _loadingScreenConfig.GetData(type);

                if(getRef.IsFailed)
                {
                    _logger.LogError(LogSystem, $"Failed to get asset reference for loading screen type {type}", LogKey, this);
                    return Result.Fail(getRef.Errors);
                }

                refs.Add(getRef.Value);
            }

            Task<Result<LoadingScreen>>[] tasks = refs
                .Select(r => _loadingScreenLoader.LoadAsync(r))
                .ToArray();

            Result<LoadingScreen>[] results;

            try
            {
                results = await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(LogSystem, $"Exception during loading screen preload: {ex.Message}", LogKey, this);
                return Result.Fail(new ExceptionalError(ex));
            }

            List<Result<LoadingScreen>> failed = results.Where(r => r.IsFailed).ToList();

            if(failed.Count <= 0)
            {
                _logger.LogInfo(LogSystem, $"Successfully preloaded {types.Length} loading screen(s)", LogKey, this);
                return Result.Ok();
            }

            foreach (var failedResult in failed)
            {
                _logger.LogError(LogSystem, $"Failed to preload loading screen: {string.Join(", ", failedResult.Errors)}", LogKey, this);
            }

            List<IError> mergedErrors = failed.SelectMany(r => r.Errors).ToList();
            return Result.Fail(mergedErrors);
        }

        public async Task<Result> Show(LoadingScreenType type, string description = "")
        {
            if(!_screens.TryGetValue(type, out LoadingScreen loadingScreen))
            {
                Result<LoadingScreen> loadScreenResult = await Load(type);

                if(loadScreenResult.IsFailed)
                {
                    _logger.LogError(LogSystem, $"Failed to load loading screen {type}: {string.Join(", ", loadScreenResult.Errors)}", LogKey, this);
                    return Result.Fail(loadScreenResult.Errors);
                }

                loadingScreen = CreateNewScreen(type, loadScreenResult.Value);
            }

            loadingScreen.Show(description);
            _logger.LogInfo(LogSystem, $"Showing loading screen {type}", LogKey, this);
            return Result.Ok();
        }

        public Result Hide(LoadingScreenType type)
        {
            if(!_screens.TryGetValue(type, out LoadingScreen loadingScreen) || !loadingScreen.State)
            {
                _logger.LogWarning(LogSystem, $"Cannot hide loading screen {type}: not found or not visible", LogKey, this);
                return Result.Fail($"There are no suitable loading screens for type {type}.");
            }

            loadingScreen.Hide();
            _logger.LogInfo(LogSystem, $"Hiding loading screen {type}", LogKey, this);
            return Result.Ok();
        }

        private async Task<Result<LoadingScreen>> Load(LoadingScreenType type)
        {
            Result<AssetReferenceGameObject> getReferenceResult = _loadingScreenConfig.GetData(type);

            if(getReferenceResult.IsFailed)
                return Result.Fail(getReferenceResult.Errors);

            return await _loadingScreenLoader.LoadAsync(getReferenceResult.Value);
        }

        private LoadingScreen CreateNewScreen(LoadingScreenType type, LoadingScreen prefab)
        {
            LoadingScreen loadingScreen = Object.Instantiate(prefab, _root.transform);
            loadingScreen.Initialize(type);
            _screens.Add(type, loadingScreen);
            return loadingScreen;
        }
    }

}