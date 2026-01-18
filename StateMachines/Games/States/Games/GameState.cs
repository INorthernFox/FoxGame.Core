using System.Threading.Tasks;
using Core.StateMachines.Games.States.Base;
using Core.StateMachines.Games.States.LoadGames;

namespace Core.StateMachines.Games.States.Games
{
    public class GameState : BaseGameState<GameState>
    {
        public override IGameState.StateType Type => IGameState.StateType.Game;

        protected override void ConfigureTransitions() =>
            AllowTransitionFrom<LoadGameState>();

        public override Task Enter() => 
            Task.CompletedTask;
    }
}