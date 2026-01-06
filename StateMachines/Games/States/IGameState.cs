namespace Core.StateMachines.Games.States
{
    public interface IGameState : IState
    {
        public StateType Type { get; }
        
        public new enum StateType
        {
            Bootstrap = 0,
            MainMenu = 1,
            Loading = 2,
            Game = 3,
        }
    }
}