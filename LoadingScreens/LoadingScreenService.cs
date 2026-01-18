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
        private readonly LoadingScreenLoader _loadingScreenLoader;
        private readonly LoadingScreenConfig _loadingScreenConfig;
        private readonly PersonalizedLogger _logger;
        private readonly Dictionary<LoadingScreenType, LoadingScreen> _screens = new();

        private LoadingScreenRoot _root;

        public LoadingScreenService(
            LoadingScreenLoader loadingScreenLoader,
            LoadingScreenConfig loadingScreenConfig,
            IGameLogger logger)
        {
            _loadingScreenLoader = loadingScreenLoader;
            _loadingScreenConfig = loadingScreenConfig;
            _logger = new PersonalizedLogger(logger, IGameLogger.LogSystems.UIWindow, nameof(LoadingScreenService), this);
        }

        public void Initialize()
        {
            _root = new GameObject("LoadingScreens").AddComponent<LoadingScreenRoot>();
        }

        public async Task<Result> Preload(params LoadingScreenType[] types)
        {
            _logger.LogInfo($"Preloading {types.Length} loading screen(s)");

            var refs = new List<AssetReference>(types.Length);

            foreach (var type in types)
            {
                var getRef = _loadingScreenConfig.GetData(type);

                if (getRef.IsFailed)
                {
                    _logger.LogError($"Failed to get asset reference for loading screen type {type}");
                    return Result.Fail(getRef.Errors);
                }

                refs.Add(getRef.Value);
            }

            var tasks = refs.Select(r => _loadingScreenLoader.LoadAsync(r)).ToArray();

            Result<LoadingScreen>[] results;

            try
            {
                results = await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception during loading screen preload: {ex.Message}");
                return Result.Fail(new ExceptionalError(ex));
            }

            var failed = results.Where(r => r.IsFailed).ToList();

            if (failed.Count <= 0)
            {
                _logger.LogInfo($"Successfully preloaded {types.Length} loading screen(s)");
                return Result.Ok();
            }

            foreach (var failedResult in failed)
            {
                _logger.LogError($"Failed to preload loading screen: {string.Join(", ", failedResult.Errors)}");
            }

            var mergedErrors = failed.SelectMany(r => r.Errors).ToList();
            return Result.Fail(mergedErrors);
        }

        public async Task<Result> Show(LoadingScreenType type, string description = "")
        {
            if (!_screens.TryGetValue(type, out var loadingScreen))
            {
                var loadScreenResult = await Load(type);

                if (loadScreenResult.IsFailed)
                {
                    _logger.LogError($"Failed to load loading screen {type}: {string.Join(", ", loadScreenResult.Errors)}");
                    return Result.Fail(loadScreenResult.Errors);
                }

                loadingScreen = CreateNewScreen(type, loadScreenResult.Value);
            }

            loadingScreen.Show(description);
            _logger.LogInfo($"Showing loading screen {type}");
            return Result.Ok();
        }

        public Result Hide(LoadingScreenType type)
        {
            if (!_screens.TryGetValue(type, out var loadingScreen) || !loadingScreen.State)
            {
                _logger.LogWarning($"Cannot hide loading screen {type}: not found or not visible");
                return Result.Fail($"There are no suitable loading screens for type {type}.");
            }

            loadingScreen.Hide();
            _logger.LogInfo($"Hiding loading screen {type}");
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