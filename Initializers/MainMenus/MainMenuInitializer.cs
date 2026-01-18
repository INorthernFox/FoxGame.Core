using Core.Loggers;
using Core.StateMachines.Games;
using Core.StateMachines.Games.States;
using Core.StateMachines.Games.States.LoadMainMenus;
using Core.UI.MainMenus;
using FluentResults;
using UnityEngine;
using Zenject;

namespace Core.Initializers.MainMenus
{
    public class MainMenuInitializer : MonoBehaviour
    {
        private const string LogKey = nameof(MainMenuInitializer);

        [SerializeField] private MainMenuUiInitializer _menuUiInitializer;

        [Inject]
        public async void Initialize(
            MainMenuCanvasFactory mainMenuCanvasFactory,
            GameStateMachine gameStateMachine,
            IGameLogger logger)
        {
            if (_menuUiInitializer == null)
            {
                logger.LogError(IGameLogger.LogSystems.UIWindow,
                    "MainMenuUiInitializer is not assigned in inspector",
                    LogKey, this);
                return;
            }

            await _menuUiInitializer.Initialize(mainMenuCanvasFactory, logger);

            if(gameStateMachine.CurrentState?.StateType == typeof(LoadMainMenuState))
            {
                Result setStateResult = await gameStateMachine.Set(IGameState.StateType.MainMenu);
                if (setStateResult.IsFailed)
                {
                    logger.LogError(IGameLogger.LogSystems.GameStateMachine,
                        $"Failed to transition to MainMenu state: {string.Join(", ", setStateResult.Errors)}",
                        LogKey, this);
                }
            }
        }
    }
}