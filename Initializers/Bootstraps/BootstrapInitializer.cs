using System;
using System.Threading.Tasks;
using Core.ConfigProviders;
using Core.ConfigProviders.GeneralConfigs;
using Core.GameConfigs;
using Core.GameSettings;
using Core.GameSettings.Providers;
using Core.LoadingScreens;
using Core.Loggers;
using Core.StateMachines.Games;
using Core.StateMachines.Games.States;
using Core.StateMachines.Games.States.Games;
using Core.StateMachines.Games.States.LoadGames;
using Core.StateMachines.Games.States.LoadMainMenus;
using Core.StateMachines.Games.States.MainMenus;
using Core.StateMachines.Games.States.UnloadGames;
using Cysharp.Threading.Tasks;
using FluentResults;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Core.Initializers.Bootstraps
{
    public class BootstrapInitializer : MonoBehaviour
    {
        [SerializeField] private LoadingScreenType _defaultLoadingScreenType = LoadingScreenType.Dev;

        private PersonalizedLogger _logger;
        private GameStateMachine _gameStateMachine;
        private LoadingScreenService _loadingScreenService;
        private IConfigsService _configsService;

        private LoadMainMenuStateFactory _loadMainMenuStateFactory;
        private MainMenuStateFactory _mainMenuStateFactory;
        private LoadGameStateFactory _loadGameStateFactory;
        private GameStateFactory _gameStateFactory;
        private UnloadGameStateFactory _unloadGameStateFactory;

        [Inject]
        public async void Initialize(
            GameStateMachine gameStateMachine,
            LoadMainMenuStateFactory loadMainMenuStateFactory,
            MainMenuStateFactory mainMenuStateFactory,
            LoadGameStateFactory loadGameStateFactory,
            GameStateFactory gameStateFactory,
            UnloadGameStateFactory unloadGameStateFactory,
            LoadingScreenService loadingScreenService,
            IConfigsService configsService,
            IGameLogger baseLogger)
        {
            CacheDependencies(
                gameStateMachine,
                loadMainMenuStateFactory,
                mainMenuStateFactory,
                loadGameStateFactory,
                gameStateFactory,
                unloadGameStateFactory,
                loadingScreenService,
                configsService,
                baseLogger);

            Result initResult = await InitializeCoreSystemsAsync();
            if (initResult.IsFailed)
                return;

            Result stateMachineResult = await SetupGameStateMachineAsync();
            if (stateMachineResult.IsFailed)
                return;

            await TransitionToMainMenuAsync();
        }

        private void CacheDependencies(
            GameStateMachine gameStateMachine,
            LoadMainMenuStateFactory loadMainMenuStateFactory,
            MainMenuStateFactory mainMenuStateFactory,
            LoadGameStateFactory loadGameStateFactory,
            GameStateFactory gameStateFactory,
            UnloadGameStateFactory unloadGameStateFactory,
            LoadingScreenService loadingScreenService,
            IConfigsService configsService,
            IGameLogger baseLogger)
        {
            _logger = new PersonalizedLogger(baseLogger, IGameLogger.LogSystems.Initializers, nameof(BootstrapInitializer), this);
            _gameStateMachine = gameStateMachine;
            _loadingScreenService = loadingScreenService;
            _configsService = configsService;

            _loadMainMenuStateFactory = loadMainMenuStateFactory;
            _mainMenuStateFactory = mainMenuStateFactory;
            _loadGameStateFactory = loadGameStateFactory;
            _gameStateFactory = gameStateFactory;
            _unloadGameStateFactory = unloadGameStateFactory;
        }

        private async Task<Result> InitializeCoreSystemsAsync()
        {
            Result addressablesResult = await InitializeAddressablesAsync();
            if (addressablesResult.IsFailed)
                return addressablesResult;

            await ApplyGameSettingsAsync();

            Result loadingScreenResult = await InitializeLoadingScreenAsync();
            if (loadingScreenResult.IsFailed)
                return loadingScreenResult;

            return Result.Ok();
        }

        private async Task<Result> InitializeAddressablesAsync()
        {
            try
            {
                _logger.LogInfo("Initializing Addressables...");
                await Addressables.InitializeAsync();
                _logger.LogInfo("Addressables initialized");
                return Result.Ok();
            }
            catch (Exception ex)
            {
                string message = $"Failed to initialize Addressables: {ex.Message}";
                _logger.LogError(message);
                return Result.Fail(message);
            }
        }

        private async Task ApplyGameSettingsAsync()
        {
            Result<GeneralConfig> settingsResult = await _configsService.LoadDefaultAsync<GeneralConfig>();

            if (settingsResult.IsSuccess)
                _defaultLoadingScreenType = settingsResult.Value.LoadingScreenType;
        }

        private async Task<Result> InitializeLoadingScreenAsync()
        {
            _loadingScreenService.Initialize();

            Result preloadResult = await _loadingScreenService.Preload(_defaultLoadingScreenType);

            if (preloadResult.IsFailed)
            {
                _logger.LogError($"Failed to preload loading screen: {string.Join(", ", preloadResult.Errors)}");
                return preloadResult;
            }

            return Result.Ok();
        }

        private async Task<Result> SetupGameStateMachineAsync()
        {
            Result addStatesResult = AddCoreStates();

            if (addStatesResult.IsFailed)
            {
                _logger.LogError($"Failed to add core states: {string.Join(", ", addStatesResult.Errors)}");
                return addStatesResult;
            }

            return Result.Ok();
        }

        private Result AddCoreStates()
        {
            IGameState[] states =
            {
                _loadMainMenuStateFactory.Create(),
                _mainMenuStateFactory.Create(),
                _loadGameStateFactory.Create(),
                _gameStateFactory.Create(),
                _unloadGameStateFactory.Create()
            };

            foreach (IGameState state in states)
            {
                Result result = _gameStateMachine.AddState(state);

                if (result.IsFailed)
                {
                    _logger.LogError($"Failed to add state {state.Type}: {string.Join(", ", result.Errors)}");
                    return result;
                }
            }

            return Result.Ok();
        }

        private async Task TransitionToMainMenuAsync()
        {
            Result setStateResult = await _gameStateMachine.Set(IGameState.StateType.LoadMainMenu);

            if (setStateResult.IsFailed)
                _logger.LogError($"Failed to transition to LoadMainMenu state: {string.Join(", ", setStateResult.Errors)}");
        }
    }
}