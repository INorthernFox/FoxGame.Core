namespace Core.StateMachines.Games.States
{
    public interface IGameState : IState
    {
        public StateType Type { get; }
        
        public new enum StateType
        {
            Bootstrap = 0,
            LoadMainMenu = 1,
            MainMenu = 2,
            LoadGame = 3,
            Game = 4,
            UnloadGame = 5
        }
    }
}