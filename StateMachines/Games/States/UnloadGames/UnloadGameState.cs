using System.Threading.Tasks;
using Core.SceneManagers;
using Core.SceneManagers.Data;
using Core.StateMachines.Games.States.Base;
using Core.StateMachines.Games.States.Games;

namespace Core.StateMachines.Games.States.UnloadGames
{
    public class UnloadGameState : BaseGameState<UnloadGameState>
    {
        private readonly ISceneManager _sceneManager;

        public UnloadGameState(ISceneManager sceneManager) =>
            _sceneManager = sceneManager;

        public override IGameState.StateType Type => IGameState.StateType.UnloadGame;

        protected override void ConfigureTransitions() =>
            AllowTransitionFrom<GameState>();

        public override async Task Enter() =>
            await _sceneManager.LoadSceneAsync(SceneType.MainMenu);
    }
}
