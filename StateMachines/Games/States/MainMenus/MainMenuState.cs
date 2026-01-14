using System;
using System.Threading.Tasks;

namespace Core.StateMachines.Games.States.MainMenus
{
    public class MainMenuState : IGameState
    {
        public Type StateType => typeof(MainMenuState);

        public Type NextStateType => null;

        public IGameState.StateType Type => IGameState.StateType.MainMenu;

        public Task Enter()
        {
            return Task.CompletedTask;
        }

        public Task Exit()
        {
            return Task.CompletedTask;
        }
    }
}
