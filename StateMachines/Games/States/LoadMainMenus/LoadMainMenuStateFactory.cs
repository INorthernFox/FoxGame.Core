using Core.LoadingScreens;
using Core.SceneManagers;
using Zenject;

namespace Core.StateMachines.Games.States.LoadMainMenus
{
    public class LoadMainMenuStateFactory : IFactory<LoadMainMenuState>
    {
        private readonly ISceneManager _sceneManager;
        private readonly LoadingScreenService _loadingScreenService;
        private readonly GameStateMachine _gameStateMachine;

        public LoadMainMenuStateFactory(
            ISceneManager sceneManager,
            LoadingScreenService loadingScreenService,
            GameStateMachine gameStateMachine)
        {
            _sceneManager = sceneManager;
            _loadingScreenService = loadingScreenService;
            _gameStateMachine = gameStateMachine;
        }

        public LoadMainMenuState Create() =>
            new(_sceneManager, _loadingScreenService, _gameStateMachine);
    }
}
