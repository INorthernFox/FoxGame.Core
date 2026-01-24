using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Loggers;
using Core.SceneManagers.Data;
using FluentResults;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

namespace Core.SceneManagers
{
    public sealed class SceneManager : ISceneManager
    {
        private readonly ScenePreset _scenePreset;
        private readonly PersonalizedLogger _logger;
        private readonly Dictionary<SceneType, SceneData> _sceneLookup = new();
        private readonly Subject<SceneType> _onSceneLoaded = new();
        private readonly Subject<SceneType> _onSceneUnloading = new();

        public IObservable<SceneType> OnSceneLoaded => _onSceneLoaded;
        public IObservable<SceneType> OnSceneUnloading => _onSceneUnloading;

        public SceneManager(ScenePreset scenePreset, IGameLogger logger)
        {
            _scenePreset = scenePreset;
            _logger = new PersonalizedLogger(
                logger ?? throw new ArgumentNullException(nameof(logger)),
                IGameLogger.LogSystems.SceneManager,
                nameof(SceneManager),
                this);

            if (_scenePreset == null)
            {
                _logger.LogError("ScenePreset is null - no scenes will be available");
                return;
            }

            BuildLookup();
        }

        public Scene ActiveScene =>
            UnitySceneManager.GetActiveScene();

        public async Task<Result> LoadSceneAsync(SceneType type, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            try
            {
                if (!_sceneLookup.TryGetValue(type, out var sceneData))
                {
                    _logger.LogError($"Scene with order {type} is not configured in preset {_scenePreset.name}.");
                    return Result.Fail($"Scene with order {type} is not configured in preset {_scenePreset.name}.");
                }

                if (UnitySceneManager.GetActiveScene().buildIndex == sceneData.Order)
                    return Result.Fail($"Scene with order {type} is already loaded.");

                SceneType? currentSceneType = GetCurrentSceneType();
                if (currentSceneType.HasValue && loadSceneMode == LoadSceneMode.Single)
                {
                    _logger.LogInfo($"Scene {currentSceneType.Value} unloading");
                    _onSceneUnloading.OnNext(currentSceneType.Value);
                }

                _logger.LogInfo($"Scene {type} start loading");
                await UnitySceneManager.LoadSceneAsync(sceneData.Order, loadSceneMode);
                _logger.LogInfo($"Scene {type} is loaded");

                _onSceneLoaded.OnNext(type);
                return Result.Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to load scene {type}: {e.Message}");
                return Result.Fail(e.Message);
            }
        }

        private SceneType? GetCurrentSceneType()
        {
            int currentBuildIndex = UnitySceneManager.GetActiveScene().buildIndex;
            foreach (var kvp in _sceneLookup)
            {
                if (kvp.Value.Order == currentBuildIndex)
                    return kvp.Key;
            }
            return null;
        }

        private void BuildLookup()
        {
            if(_scenePreset.Scenes == null)
            {
                return;
            }

            foreach(SceneData sceneData in _scenePreset.Scenes)
            {
                if(sceneData == null)
                {
                    continue;
                }

                if(!_sceneLookup.TryAdd(sceneData.SceneType, sceneData))
                {
                    throw new ArgumentException($"Duplicate scene order {sceneData.Order} found in preset {_scenePreset.name}. Each scene must have a unique order.");
                }
            }
        }
    }

}