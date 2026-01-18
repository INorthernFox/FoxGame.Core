using System.Threading.Tasks;
using Core.LoadingScreens;
using Core.SceneManagers;
using Core.SceneManagers.Data;
using Core.StateMachines.Games.States.Base;
using Core.StateMachines.Games.States.Bootstraps;
using Core.StateMachines.Games.States.UnloadGames;

namespace Core.StateMachines.Games.States.LoadMainMenus
{
    public class LoadMainMenuState : BaseGameState<LoadMainMenuState>
    {
        private readonly ISceneManager _sceneManager;
        private readonly LoadingScreenService _loadingScreenService;

        public LoadMainMenuState(
            ISceneManager sceneManager,
            LoadingScreenService loadingScreenService)
        {
            _sceneManager = sceneManager;
            _loadingScreenService = loadingScreenService;
        }

        public override IGameState.StateType Type => IGameState.StateType.LoadMainMenu;

        protected override void ConfigureTransitions()
        {
            AllowTransitionFrom<BootstrapState>();
            AllowTransitionFrom<UnloadGameState>();
        }

        public override async Task Enter()
        {
            await _loadingScreenService.Show(LoadingScreenType.Dev, "Loading Main Menu...");
            await _sceneManager.LoadSceneAsync(SceneType.MainMenu);
        }

        public override Task Exit()
        {
            _loadingScreenService.Hide(LoadingScreenType.Dev);
            return Task.CompletedTask;
        }
    }
}