using System;
using System.Threading.Tasks;
using Core.SceneManagers;
using Core.SceneManagers.Data;

namespace Core.StateMachines.Games.States.Bootstraps
{
    public class BootstrapState : IGameState
    {
        private readonly ISceneManager _sceneManager;
        
        public BootstrapState(ISceneManager sceneManager)
        {
            _sceneManager = sceneManager;
        }

        public Type StateType => 
            typeof(BootstrapState);
        public Type NextStateType =>
            null;
        
        public IGameState.StateType Type => 
            IGameState.StateType.Bootstrap;
        
        public async Task Enter()
        {
            await _sceneManager.LoadSceneAsync(SceneType.Bootstrap);
        }
        
        public Task Exit()
        {
           return Task.CompletedTask;
        }
    }
}
