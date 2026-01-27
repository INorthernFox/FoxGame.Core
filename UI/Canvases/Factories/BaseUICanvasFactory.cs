using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Loggers;
using Core.ResourceManagement.Load.interfaces;
using FluentResults;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Core.UI.Canvases.Factories
{
    public abstract class BaseUICanvasFactory<TCanvas, TView>
        where TCanvas : BaseUICanvas
        where TView : BaseUICanvasViewWithModel<TCanvas>
    {
        private readonly UICanvasAssetsConfig _assetsConfig;
        private readonly UICanvasViewLoader _viewLoader;
        private readonly UIForegroundSortingService _sortingService;
        private readonly UICanvasRepository _canvasRepository;
        private readonly IGameLogger _baseLogger;
        private readonly Dictionary<string, IAddressableHandle<BaseUICanvasView>> _handles = new();

        private PersonalizedLogger _logger;

        protected abstract UICanvasType CanvasType { get; }

        protected PersonalizedLogger Logger => _logger ??= CreateLogger();

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
            _baseLogger = logger;
        }

        private PersonalizedLogger CreateLogger() =>
            new(_baseLogger, IGameLogger.LogSystems.UIWindow, GetType().Name, this);

        public async Task<Result<UICanvasContainer<TCanvas, TView>>> CreateAsync(string id, Transform parent = null)
        {
            if (CanvasType == UICanvasType.None)
                return Result.Fail<UICanvasContainer<TCanvas, TView>>("Use CreateAsync with fileName for None canvas type");

            Result<AssetReferenceGameObject> getAssetResult = _assetsConfig.GetAsset(CanvasType);

            if (getAssetResult.IsFailed)
                return Result.Fail<UICanvasContainer<TCanvas, TView>>(getAssetResult.Errors);

            return await LoadAndInstantiateWithHandle(id, getAssetResult.Value, parent);
        }

        public async Task<Result<UICanvasContainer<TCanvas, TView>>> CreateAsync(string id, string fileName, Transform parent = null)
        {
            Result<IAddressableHandle<BaseUICanvasView>> loadResult = await _viewLoader.LoadWithHandleAsync(fileName);

            if (loadResult.IsFailed)
                return Result.Fail<UICanvasContainer<TCanvas, TView>>(loadResult.Errors);

            return InstantiateWithHandle(id, loadResult.Value, parent);
        }

        private async Task<Result<UICanvasContainer<TCanvas, TView>>> LoadAndInstantiateWithHandle(
            string id,
            AssetReferenceGameObject assetRef,
            Transform parent)
        {
            Result<IAddressableHandle<BaseUICanvasView>> loadResult = await _viewLoader.LoadWithHandleAsync(assetRef);

            if (loadResult.IsFailed)
                return Result.Fail<UICanvasContainer<TCanvas, TView>>(loadResult.Errors);

            return InstantiateWithHandle(id, loadResult.Value, parent);
        }

        private Result<UICanvasContainer<TCanvas, TView>> InstantiateWithHandle(
            string id,
            IAddressableHandle<BaseUICanvasView> handle,
            Transform parent)
        {
            if (handle.Asset is not TView viewPrefab)
            {
                handle.Dispose();
                return Result.Fail<UICanvasContainer<TCanvas, TView>>($"Loaded prefab is not {typeof(TView).Name}");
            }

            var view = Object.Instantiate(viewPrefab, parent);
            var model = CreateModel(id);

            view.Initialize(model, _sortingService, _baseLogger);
            _canvasRepository.Add(model);

            _handles[id] = handle;

            model.OnDispose.Subscribe(_ => OnCanvasDisposed(id));

            return Result.Ok(new UICanvasContainer<TCanvas, TView>(model, view));
        }

        private void OnCanvasDisposed(string id)
        {
            if (_handles.TryGetValue(id, out var handle))
            {
                handle.Dispose();
                _handles.Remove(id);
                Logger.LogInfo($"Disposed handle for canvas {id}");
            }
        }

        protected abstract TCanvas CreateModel(string id);
    }
}
