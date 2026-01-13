using System;
using System.Threading.Tasks;
using Core.SceneManagers;
using Core.SceneManagers.Data;

namespace Core.StateMachines.Games.States.LoadGames
{
    public class LoadGameState : IGameState
    {
        private readonly ISceneManager _sceneManager;
        
        public LoadGameState(ISceneManager sceneManager)
        {
            _sceneManager = sceneManager;
        }

        public Type StateType => 
            typeof(LoadGameState);
        
        public Type NextStateType => 
            null;
        
        public async Task Enter()
        {
            await _sceneManager.LoadSceneAsync(SceneType.Game);
        }

        public Task Exit() =>
            Task.CompletedTask;

        public IGameState.StateType Type => 
            IGameState.StateType.LoadGame;
    }
}