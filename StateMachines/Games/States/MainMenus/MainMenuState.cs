using System.Threading.Tasks;
using Core.StateMachines.Games.States.Base;
using Core.StateMachines.Games.States.LoadMainMenus;

namespace Core.StateMachines.Games.States.MainMenus
{
    public class MainMenuState : BaseGameState<MainMenuState>
    {
        public override IGameState.StateType Type => IGameState.StateType.MainMenu;

        protected override void ConfigureTransitions() =>
            AllowTransitionFrom<LoadMainMenuState>();

        public override Task Enter() => Task.CompletedTask;
    }
}
