using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Loggers;
using Core.SceneManagers.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

namespace Core.SceneManagers
{
    public sealed class SceneManager : ISceneManager
    {
        private readonly ScenePreset _scenePreset;
        private readonly IGameLogger _logger;
        private readonly Dictionary<SceneType, SceneData> _sceneLookup = new();

        public SceneManager(ScenePreset scenePreset, IGameLogger logger)
        {
            _scenePreset = scenePreset;
            _logger = logger;
            BuildLookup();
        }

        private IGameLogger.LogSystems LogSystem =>
            IGameLogger.LogSystems.SceneManager;

        public Scene ActiveScene =>
            UnitySceneManager.GetActiveScene();

        public async Task<bool> LoadSceneAsync(SceneType type, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            try
            {
                if(!_sceneLookup.TryGetValue(type, out SceneData sceneData))
                {
                    _logger.LogError(LogSystem, $"Scene with order {type} is not configured in preset {_scenePreset.name}.", "LoadSceneAsync.CantFind");
                    return false;
                }

                _logger.LogInfo(LogSystem, $"Scene {type.ToString()} start loading", "LoadSceneAsync.SceneLoading");
                await UnitySceneManager.LoadSceneAsync(sceneData.Order, loadSceneMode);
                _logger.LogInfo(LogSystem, $"Scene {type.ToString()} is loaded", "LoadSceneAsync.SceneLoaded");
                return true;
            }
            catch (Exception e)
            {
                return false;
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