using System;
using System.Threading.Tasks;
using Core.SceneManagers;
using Core.SceneManagers.Data;

namespace Core.StateMachines.Games.States.UnloadGames
{
    public class UnloadGameState : IGameState
    {
        private readonly ISceneManager _sceneManager;

        public UnloadGameState(ISceneManager sceneManager)
        {
            _sceneManager = sceneManager;
        }

        public Type StateType =>
            typeof(UnloadGameState);

        public Type NextStateType =>
            null;

        public IGameState.StateType Type =>
            IGameState.StateType.UnloadGame;

        public async Task Enter()
        {
            await _sceneManager.LoadSceneAsync(SceneType.MainMenu);
        }

        public Task Exit() =>
            Task.CompletedTask;
    }
}
