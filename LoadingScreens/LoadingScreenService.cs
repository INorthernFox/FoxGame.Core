using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private LoadingScreenRoot _root;

        private readonly Dictionary<LoadingScreenType, LoadingScreen> _screens = new();

        public LoadingScreenService(
            LoadingScreenLoader loadingScreenLoader,
            LoadingScreenConfig loadingScreenConfig)
        {
            _loadingScreenLoader = loadingScreenLoader;
            _loadingScreenConfig = loadingScreenConfig;
        }

        public void Initialize()
        {
            _root = new GameObject("LoadingScreens").AddComponent<LoadingScreenRoot>();
        }

        public async Task<Result> Preload(params LoadingScreenType[] types)
        {
            List<AssetReference> refs = new(types.Length);

            foreach(LoadingScreenType type in types)
            {
                Result<AssetReferenceGameObject> getRef = _loadingScreenConfig.GetData(type);

                if(getRef.IsFailed)
                    return Result.Fail(getRef.Errors);

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
                return Result.Fail(new ExceptionalError(ex));
            }

            List<Result<LoadingScreen>> failed = results.Where(r => r.IsFailed).ToList();

            if(failed.Count <= 0)
                return Result.Ok();

            List<IError> mergedErrors = failed.SelectMany(r => r.Errors).ToList();
            return Result.Fail(mergedErrors);
        }

        public async Task<Result> Show(LoadingScreenType type, string description = "")
        {
            if(!_screens.TryGetValue(type, out LoadingScreen loadingScreen))
            {
                Result<LoadingScreen> loadScreenResult = await Load(type);

                if(loadScreenResult.IsFailed)
                    return Result.Fail(loadScreenResult.Errors);

                loadingScreen = CreateNewScreen(type, loadScreenResult.Value);
            }

            loadingScreen.Show(description);
            return Result.Ok();
        }

        public Result Hide(LoadingScreenType type)
        {
            if(!_screens.TryGetValue(type, out LoadingScreen loadingScreen) || !loadingScreen.State)
            {
                return Result.Fail("There are no suitable loading screenshots.");
            }

            loadingScreen.Hide();
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