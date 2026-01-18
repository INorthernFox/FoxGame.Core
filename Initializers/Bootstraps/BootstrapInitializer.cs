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
        private const string LogKey = nameof(BootstrapInitializer);

        [SerializeField] private LoadingScreenType _defaultLoadingScreenType = LoadingScreenType.Dev;

        [Inject]
        public async void Initialize(
            GameStateMachine gameStateMachine,
            LoadMainMenuStateFactory loadMainMenuStateFactory,
            MainMenuStateFactory mainMenuStateFactory,
            LoadGameStateFactory loadGameStateFactory,
            GameStateFactory gameStateFactory,
            UnloadGameStateFactory unloadGameStateFactory,
            LoadingScreenService loadingScreenService,
            IGameLogger logger)
        {
            Result initializeAddressablesResult = await InitializeAddressablesAsync(logger);
            if(initializeAddressablesResult.IsFailed)
                return;

            loadingScreenService.Initialize();

            Result preloadResult = await loadingScreenService.Preload(_defaultLoadingScreenType);
            if(preloadResult.IsFailed)
            {
                logger.LogError(IGameLogger.LogSystems.UIWindow,
                    $"Failed to preload loading screen: {string.Join(", ", preloadResult.Errors)}",
                    LogKey, this);
            }

            Result addStatesResult = AddCoreStates(
                gameStateMachine,
                loadMainMenuStateFactory,
                mainMenuStateFactory,
                loadGameStateFactory,
                gameStateFactory,
                unloadGameStateFactory,
                logger);

            if(addStatesResult.IsFailed)
            {
                logger.LogError(IGameLogger.LogSystems.GameStateMachine,
                    $"Failed to add core states: {string.Join(", ", addStatesResult.Errors)}",
                    LogKey, this);
                return;
            }

            Result setStateResult = await gameStateMachine.Set(IGameState.StateType.LoadMainMenu);
            if(setStateResult.IsFailed)
            {
                logger.LogError(IGameLogger.LogSystems.GameStateMachine,
                    $"Failed to transition to LoadMainMenu state: {string.Join(", ", setStateResult.Errors)}",
                    LogKey, this);
            }
        }
        
        private async Task<Result> InitializeAddressablesAsync(IGameLogger logger)
        {
            try
            {
                logger.LogInfo(IGameLogger.LogSystems.ResourceManager, "Initializing Addressables...", LogKey, this);
                await Addressables.InitializeAsync();
                logger.LogInfo(IGameLogger.LogSystems.ResourceManager, "Addressables initialized", LogKey, this);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                string message = $"Failed to initialize Addressables: {ex.Message}";
                logger.LogError(IGameLogger.LogSystems.ResourceManager, message, LogKey, this);
                return Result.Fail(message);
            }
        }

        private static Result AddCoreStates(
            GameStateMachine gameStateMachine,
            LoadMainMenuStateFactory loadMainMenuStateFactory,
            MainMenuStateFactory mainMenuStateFactory,
            LoadGameStateFactory loadGameStateFactory,
            GameStateFactory gameStateFactory,
            UnloadGameStateFactory unloadGameStateFactory,
            IGameLogger logger)
        {
            var states = new IGameState[]
            {
                loadMainMenuStateFactory.Create(),
                mainMenuStateFactory.Create(),
                loadGameStateFactory.Create(),
                gameStateFactory.Create(),
                unloadGameStateFactory.Create()
            };

            foreach(IGameState state in states)
            {
                Result result = gameStateMachine.AddState(state);
                if(result.IsFailed)
                {
                    logger.LogError(IGameLogger.LogSystems.GameStateMachine,
                        $"Failed to add state {state.Type}: {string.Join(", ", result.Errors)}",
                        LogKey, null);
                    return result;
                }
            }

            return Result.Ok();
        }
    }
}