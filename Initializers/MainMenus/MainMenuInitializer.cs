using Core.Loggers;
using Core.StateMachines.Games;
using Core.StateMachines.Games.States;
using Core.StateMachines.Games.States.LoadMainMenus;
using Core.UI.MainMenus;
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
            IGameLogger logger)
        {
            await _menuUiInitializer.Initialize(mainMenuCanvasFactory, logger);

            if(gameStateMachine.CurrentState.StateType == typeof(LoadMainMenuState))
            {
                await gameStateMachine.Set(IGameState.StateType.MainMenu);
            }
        }
    }

}