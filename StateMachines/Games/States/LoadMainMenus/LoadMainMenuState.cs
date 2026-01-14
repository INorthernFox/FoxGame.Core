using System;
using System.Threading.Tasks;
using Core.LoadingScreens;
using Core.SceneManagers;
using Core.SceneManagers.Data;

namespace Core.StateMachines.Games.States.LoadMainMenus
{
    public class LoadMainMenuState : IGameState
    {
        private readonly ISceneManager _sceneManager;
        private readonly LoadingScreenService _loadingScreenService;
        private readonly GameStateMachine _gameStateMachine;

        public LoadMainMenuState(
            ISceneManager sceneManager,
            LoadingScreenService loadingScreenService,
            GameStateMachine gameStateMachine)
        {
            _sceneManager = sceneManager;
            _loadingScreenService = loadingScreenService;
            _gameStateMachine = gameStateMachine;
        }

        public Type StateType => typeof(LoadMainMenuState);

        public Type NextStateType => typeof(MainMenus.MainMenuState);

        public IGameState.StateType Type => IGameState.StateType.LoadMainMenu;

        public async Task Enter()
        {
            await _loadingScreenService.Show(LoadingScreenType.Dev, "Loading Main Menu...");
            await _sceneManager.LoadSceneAsync(SceneType.MainMenu);
        }

        public Task Exit()
        {
            _loadingScreenService.Hide(LoadingScreenType.Dev);
            return Task.CompletedTask;
        }
    }
}
