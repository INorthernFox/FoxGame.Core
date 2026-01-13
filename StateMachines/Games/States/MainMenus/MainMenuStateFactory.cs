using Core.LoadingScreens;
using Core.SceneManagers;
using Zenject;

namespace Core.StateMachines.Games.States.MainMenus
{
    public class MainMenuStateFactory : IFactory<MainMenuState>
    {
        private readonly ISceneManager _sceneManager;
        private readonly LoadingScreenService _loadingScreenService;
        
        public MainMenuStateFactory(ISceneManager sceneManager, 
            LoadingScreenService loadingScreenService)
        {
            _sceneManager = sceneManager;
            _loadingScreenService = loadingScreenService;
        }

        public MainMenuState Create() =>
            new (_sceneManager, _loadingScreenService);
    }
}