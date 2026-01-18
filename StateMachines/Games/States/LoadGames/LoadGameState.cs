using System.Threading.Tasks;
using Core.SceneManagers;
using Core.SceneManagers.Data;
using Core.StateMachines.Games.States.Base;
using Core.StateMachines.Games.States.MainMenus;

namespace Core.StateMachines.Games.States.LoadGames
{
    public class LoadGameState : BaseGameState<LoadGameState>
    {
        private readonly ISceneManager _sceneManager;

        public LoadGameState(ISceneManager sceneManager) =>
            _sceneManager = sceneManager;

        public override IGameState.StateType Type => IGameState.StateType.LoadGame;

        protected override void ConfigureTransitions() =>
            AllowTransitionFrom<MainMenuState>();

        public override async Task Enter() =>
            await _sceneManager.LoadSceneAsync(SceneType.Game);
    }
}