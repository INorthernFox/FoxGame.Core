using System;
using Core.Loggers;
using Core.ResourceManagement.Load.interfaces;
using Core.SceneManagers;
using Core.SceneManagers.Data;
using UniRx;

namespace Core.ResourceManagement
{
    public sealed class AddressableCleanupService : IDisposable
    {
        private readonly IAddressableRegistry _registry;
        private readonly PersonalizedLogger _logger;
        private readonly CompositeDisposable _disposables = new();

        public AddressableCleanupService(
            ISceneManager sceneManager,
            IAddressableRegistry registry,
            IGameLogger logger)
        {
            _registry = registry;
            _logger = new PersonalizedLogger(
                logger,
                IGameLogger.LogSystems.ResourceManager,
                nameof(AddressableCleanupService),
                this);

            sceneManager.OnSceneUnloading
                .Subscribe(OnSceneUnloading)
                .AddTo(_disposables);

            _logger.LogInfo("AddressableCleanupService initialized");
        }

        private void OnSceneUnloading(SceneType sceneType)
        {
            _logger.LogInfo($"Cleaning up addressables for scene {sceneType}");
            _registry.ReleaseAllByScene(sceneType);
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
}
