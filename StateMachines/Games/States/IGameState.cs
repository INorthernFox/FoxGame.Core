namespace Core.StateMachines.Games.States
{
    public interface IGameState : IState
    {
        public StateType Type { get; }
        
        public new enum StateType
        {
            Bootstrap = 0,
            MainMenu = 1,
            LoadGame = 2,
            Game = 3,
            UnloadGame = 4
        }
    }
}