using System;
using System.Threading.Tasks;

namespace Core.StateMachines.Games.States.Games
{
    public class GameState : IGameState
    {
        public Type StateType =>
            typeof(GameState);

        public Type NextStateType =>
            null;

        public IGameState.StateType Type =>
            IGameState.StateType.Game;

        public Task Enter() =>
            Task.CompletedTask;

        public Task Exit() =>
            Task.CompletedTask;
    }
}
