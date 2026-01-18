using Core.LoadingScreens;
using Core.SceneManagers;
using Zenject;

namespace Core.StateMachines.Games.States.LoadMainMenus
{
    public class LoadMainMenuStateFactory : IFactory<LoadMainMenuState>
    {
        private readonly ISceneManager _sceneManager;
        private readonly LoadingScreenService _loadingScreenService;

        public LoadMainMenuStateFactory(
            ISceneManager sceneManager,
            LoadingScreenService loadingScreenService)
        {
            _sceneManager = sceneManager;
            _loadingScreenService = loadingScreenService;
        }

        public LoadMainMenuState Create() =>
            new(_sceneManager, _loadingScreenService);
    }
}
