using Core.ConfigProviders;
using Core.GameSettings;
using Core.Loggers;
using Core.StateMachines.Games;
using Core.StateMachines.Games.States;
using Core.UI.Canvases.MainMenus;
using UnityEngine;
using Zenject;

namespace Core.Initializers.MainMenus
{
    public class MainMenuInitializer : MonoBehaviour
    {
        [SerializeField] private MainMenuUiInitializer _menuUiInitializer;

        [Inject]
        public async void Initialize(
            MainMenuCanvasFactory mainMenuCanvasFactory,
            GameStateMachine gameStateMachine,
            IConfigsService configsService,
            IGameLogger baseLogger)
        {
            var logger = new PersonalizedLogger(baseLogger, IGameLogger.LogSystems.Initializers, nameof(MainMenuInitializer), this);

            if (_menuUiInitializer == null)
            {
                logger.LogError("MainMenuUiInitializer is not assigned in inspector");
                return;
            }

            logger.LogInfo("Starting MainMenuUiInitializer...");

            await _menuUiInitializer.Initialize(mainMenuCanvasFactory,  configsService, baseLogger);

            logger.LogInfo("Set MainMenu GameState");
            
            var setStateResult = await gameStateMachine.Set(IGameState.StateType.MainMenu);

            if (setStateResult.IsFailed)
                logger.LogError($"Failed to transition to MainMenu state: {string.Join(", ", setStateResult.Errors)}");

            logger.LogInfo("On Set MainMenu GameState");
        }
    }
}