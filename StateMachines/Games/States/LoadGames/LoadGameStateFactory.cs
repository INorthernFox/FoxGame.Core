using Core.SceneManagers;
using Zenject;

namespace Core.StateMachines.Games.States.LoadGames
{
    public class LoadGameStateFactory : IFactory<LoadGameState>
    {
        private readonly ISceneManager _sceneManager;

        public LoadGameStateFactory(ISceneManager sceneManager)
        {
            _sceneManager = sceneManager;
        }

        public LoadGameState Create() =>
            new (_sceneManager);
    }
}
