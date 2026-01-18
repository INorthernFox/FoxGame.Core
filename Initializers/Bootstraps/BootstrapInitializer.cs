using System;
using System.Threading.Tasks;
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

        [Inject]
        public async void Initialize(
            GameStateMachine gameStateMachine,
            LoadMainMenuStateFactory loadMainMenuStateFactory,
            MainMenuStateFactory mainMenuStateFactory,
            LoadGameStateFactory loadGameStateFactory,
            GameStateFactory gameStateFactory,
            UnloadGameStateFactory unloadGameStateFactory,
            LoadingScreenService loadingScreenService,
            IGameLogger baseLogger)
        {
            _logger = new PersonalizedLogger(baseLogger, IGameLogger.LogSystems.Initializers, nameof(BootstrapInitializer), this);

            var initializeAddressablesResult = await InitializeAddressablesAsync();
            if (initializeAddressablesResult.IsFailed)
                return;

            loadingScreenService.Initialize();

            var preloadResult = await loadingScreenService.Preload(_defaultLoadingScreenType);
            if (preloadResult.IsFailed)
                _logger.LogError($"Failed to preload loading screen: {string.Join(", ", preloadResult.Errors)}");

            var addStatesResult = AddCoreStates(
                gameStateMachine,
                loadMainMenuStateFactory,
                mainMenuStateFactory,
                loadGameStateFactory,
                gameStateFactory,
                unloadGameStateFactory);

            if (addStatesResult.IsFailed)
            {
                _logger.LogError($"Failed to add core states: {string.Join(", ", addStatesResult.Errors)}");
                return;
            }

            var setStateResult = await gameStateMachine.Set(IGameState.StateType.LoadMainMenu);
            if (setStateResult.IsFailed)
                _logger.LogError($"Failed to transition to LoadMainMenu state: {string.Join(", ", setStateResult.Errors)}");
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
                var message = $"Failed to initialize Addressables: {ex.Message}";
                _logger.LogError(message);
                return Result.Fail(message);
            }
        }

        private Result AddCoreStates(
            GameStateMachine gameStateMachine,
            LoadMainMenuStateFactory loadMainMenuStateFactory,
            MainMenuStateFactory mainMenuStateFactory,
            LoadGameStateFactory loadGameStateFactory,
            GameStateFactory gameStateFactory,
            UnloadGameStateFactory unloadGameStateFactory)
        {
            var states = new IGameState[]
            {
                loadMainMenuStateFactory.Create(),
                mainMenuStateFactory.Create(),
                loadGameStateFactory.Create(),
                gameStateFactory.Create(),
                unloadGameStateFactory.Create()
            };

            foreach (var state in states)
            {
                var result = gameStateMachine.AddState(state);
                if (result.IsFailed)
                {
                    _logger.LogError($"Failed to add state {state.Type}: {string.Join(", ", result.Errors)}");
                    return result;
                }
            }

            return Result.Ok();
        }
    }
}