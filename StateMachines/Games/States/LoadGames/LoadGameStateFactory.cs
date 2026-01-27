using Core.LoadingScreens;
using Core.SceneManagers;
using Zenject;

namespace Core.StateMachines.Games.States.LoadGames
{
    public class LoadGameStateFactory : IFactory<LoadGameState>
    {
        private readonly ISceneManager _sceneManager;
        private readonly LoadingScreenService _loadingScreenService;

        public LoadGameStateFactory(ISceneManager sceneManager, LoadingScreenService loadingScreenService)
        {
            _sceneManager = sceneManager;
            _loadingScreenService = loadingScreenService;
        }

        public LoadGameState Create() =>
            new (_sceneManager, _loadingScreenService);
    }
}
