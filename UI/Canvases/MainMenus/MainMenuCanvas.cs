using System.Threading.Tasks;
using Core.StateMachines.Games;
using Core.StateMachines.Games.States.LoadGames;
using FluentResults;

namespace Core.UI.Canvases.MainMenus
{
    public class MainMenuCanvas : BaseUICanvas
    {
        private readonly GameStateMachine _gameStateMachine;
        public override UICanvasType CanvasType => UICanvasType.MainMenu;

        public MainMenuCanvas(string id, GameStateMachine gameStateMachine)
        {
            _gameStateMachine = gameStateMachine;
            SetID(id);
        }

        public async Task<Result> FastStartGame()
        {
            Result startGameResult = await _gameStateMachine.Set<LoadGameState>();
            return startGameResult;
        }
    }
}