using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Loggers;
using Core.SceneManagers.Data;
using FluentResults;
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

                _logger.LogInfo($"Scene {type} start loading");
                await UnitySceneManager.LoadSceneAsync(sceneData.Order, loadSceneMode);
                _logger.LogInfo($"Scene {type} is loaded");
                return Result.Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to load scene {type}: {e.Message}");
                return Result.Fail(e.Message);
            }
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