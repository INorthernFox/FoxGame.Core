using System.Threading.Tasks;
using Core.LoadingScreens;
using Core.SceneManagers;
using Core.SceneManagers.Data;
using Core.StateMachines.Games.States.Base;
using Core.StateMachines.Games.States.MainMenus;

namespace Core.StateMachines.Games.States.LoadGames
{
    public class LoadGameState : BaseGameState<LoadGameState>
    {
        private readonly ISceneManager _sceneManager;
        private readonly LoadingScreenService _loadingScreenService;

        public LoadGameState(ISceneManager sceneManager,
            LoadingScreenService loadingScreenService)
        {
            _sceneManager = sceneManager;
            _loadingScreenService = loadingScreenService;
        }

        public override IGameState.StateType Type => IGameState.StateType.LoadGame;

        protected override void ConfigureTransitions() =>
            AllowTransitionFrom<MainMenuState>();

        public override async Task Enter()
        {
            await _loadingScreenService.Show(LoadingScreenType.Dev, "Loading Game...");
            await _sceneManager.LoadSceneAsync(SceneType.Game);
        }
        
        public override Task Exit()
        {
            _loadingScreenService.Hide(LoadingScreenType.Dev);
            return Task.CompletedTask;
        }
    }
}