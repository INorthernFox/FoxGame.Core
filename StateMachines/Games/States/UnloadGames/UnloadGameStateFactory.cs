using Core.SceneManagers;
using Zenject;

namespace Core.StateMachines.Games.States.UnloadGames
{
    public class UnloadGameStateFactory : IFactory<UnloadGameState>
    {
        private readonly ISceneManager _sceneManager;

        public UnloadGameStateFactory(ISceneManager sceneManager)
        {
            _sceneManager = sceneManager;
        }

        public UnloadGameState Create() =>
            new (_sceneManager);
    }
}
