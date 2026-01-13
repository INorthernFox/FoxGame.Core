using Zenject;

namespace Core.StateMachines.Games.States.Games
{
    public class GameStateFactory : IFactory<GameState>
    {
        public GameState Create() =>
            new ();
    }
}
