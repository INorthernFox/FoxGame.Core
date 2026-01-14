using System.Threading.Tasks;
using Core.Loggers;
using FluentResults;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Core.UI.Factories
{
    public abstract class BaseUICanvasFactory<TCanvas, TView>
        where TCanvas : BaseUICanvas
        where TView : BaseUICanvasViewWithModel<TCanvas>
    {
        private readonly UICanvasAssetsConfig _assetsConfig;
        private readonly UICanvasViewLoader _viewLoader;
        private readonly UIForegroundSortingService _sortingService;
        private readonly UICanvasRepository _canvasRepository;
        private readonly IGameLogger _logger;

        protected abstract UICanvasType CanvasType { get; }

        protected BaseUICanvasFactory(
            UICanvasAssetsConfig assetsConfig,
            UICanvasViewLoader viewLoader,
            UIForegroundSortingService sortingService,
            UICanvasRepository canvasRepository,
            IGameLogger logger)
        {
            _assetsConfig = assetsConfig;
            _viewLoader = viewLoader;
            _sortingService = sortingService;
            _canvasRepository = canvasRepository;
            _logger = logger;
        }

        public async Task<Result<UICanvasContainer<TCanvas, TView>>> CreateAsync(string id, Transform parent = null)
        {
            if (CanvasType == UICanvasType.None)
                return Result.Fail<UICanvasContainer<TCanvas, TView>>("Use CreateAsync with fileName for None canvas type");

            Result<AssetReferenceGameObject> getAssetResult = _assetsConfig.GetAsset(CanvasType);

            if (getAssetResult.IsFailed)
                return Result.Fail<UICanvasContainer<TCanvas, TView>>(getAssetResult.Errors);

            return await LoadAndInstantiate(id, getAssetResult.Value, parent);
        }

        public async Task<Result<UICanvasContainer<TCanvas, TView>>> CreateAsync(string id, string fileName, Transform parent = null)
        {
            Result<BaseUICanvasView> loadResult = await _viewLoader.LoadAsync(fileName);

            return loadResult.IsFailed 
                ? Result.Fail<UICanvasContainer<TCanvas, TView>>(loadResult.Errors) 
                : Instantiate(id, loadResult.Value, parent);
        }

        private async Task<Result<UICanvasContainer<TCanvas, TView>>> LoadAndInstantiate(string id, AssetReferenceGameObject assetRef, Transform parent)
        {
            Result<BaseUICanvasView> loadResult = await _viewLoader.LoadAsync(assetRef);

            return loadResult.IsFailed 
                ? Result.Fail<UICanvasContainer<TCanvas, TView>>(loadResult.Errors) 
                : Instantiate(id, loadResult.Value, parent);
        }

        private Result<UICanvasContainer<TCanvas, TView>> Instantiate(string id, BaseUICanvasView prefab, Transform parent)
        {
            if (prefab is not TView viewPrefab)
                return Result.Fail<UICanvasContainer<TCanvas, TView>>($"Loaded prefab is not {typeof(TView).Name}");

            TView view = Object.Instantiate(viewPrefab, parent);

            TCanvas model = CreateModel(id);
            view.Initialize(model, _sortingService, _logger);
            _canvasRepository.Add(model);

            return Result.Ok(new UICanvasContainer<TCanvas, TView>(model, view));
        }

        protected abstract TCanvas CreateModel(string id);
    }
}
