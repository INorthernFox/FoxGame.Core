using Core.StateMachines.Games;
using Core.StateMachines.Games.States;
using Core.StateMachines.Games.States.Bootstraps;
using UnityEngine;
using Zenject;

namespace Core.Initializers.Projects
{
    public class ProjectInitializer : MonoBehaviour
    {
        [Inject]
        public async void Initialize(
            GameStateMachine gameStateMachine,
            BootstrapStateFactory bootstrapStateFactory)
        {
            BootstrapState bootstrapState = bootstrapStateFactory.Create();

            gameStateMachine.AddState(bootstrapState);
            await gameStateMachine.Set(IGameState.StateType.Game);
        }
    }
}