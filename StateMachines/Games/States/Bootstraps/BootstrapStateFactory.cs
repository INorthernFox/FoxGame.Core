using Core.SceneManagers;
using Core.StateMachines.Games.States.Bootstraps;
using Zenject;

namespace Core.StateMachines.Games
{
    public class BootstrapStateFactory : IFactory<BootstrapState>
    {
        private readonly ISceneManager _sceneManager;
        
        public BootstrapStateFactory(ISceneManager sceneManager)
        {
            _sceneManager = sceneManager;
        }

        public BootstrapState Create() =>
            new (_sceneManager);
    }
}