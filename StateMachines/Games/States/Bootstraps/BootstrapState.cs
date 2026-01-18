using System.Threading.Tasks;
using Core.SceneManagers;
using Core.SceneManagers.Data;
using Core.StateMachines.Games.States.Base;

namespace Core.StateMachines.Games.States.Bootstraps
{
    public class BootstrapState : BaseGameState<BootstrapState>
    {
        private readonly ISceneManager _sceneManager;

        public BootstrapState(ISceneManager sceneManager) =>
            _sceneManager = sceneManager;

        public override IGameState.StateType Type => 
            IGameState.StateType.Bootstrap;

        protected override void ConfigureTransitions()
        {
            //Transition from any state is possible
        }

        public override async Task Enter() =>
            await _sceneManager.LoadSceneAsync(SceneType.Bootstrap);
    }
}
