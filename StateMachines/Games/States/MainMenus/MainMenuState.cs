using System;
using System.Threading.Tasks;
using Core.LoadingScreens;
using Core.SceneManagers;
using Core.SceneManagers.Data;

namespace Core.StateMachines.Games.States.MainMenus
{
    public class MainMenuState : IGameState
    {
        private readonly ISceneManager _sceneManager;
        private readonly LoadingScreenService _loadingScreenService;

        public MainMenuState(
            ISceneManager sceneManager,
            LoadingScreenService loadingScreenService)
        {
            _sceneManager = sceneManager;
            _loadingScreenService = loadingScreenService;
        }

        public Type StateType =>
            typeof(MainMenuState);

        public Type NextStateType =>
            null;

        public IGameState.StateType Type =>
            IGameState.StateType.MainMenu;

        public async Task Enter()
        {
            await _loadingScreenService.Show(LoadingScreenType.Dev, "Load MainMenu Scene");
            await _sceneManager.LoadSceneAsync(SceneType.MainMenu);
            _loadingScreenService.Hide(LoadingScreenType.Dev);
        }

        public Task Exit()
        {
            return Task.CompletedTask;
        }
    }

}