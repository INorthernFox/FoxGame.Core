using Core.StateMachines.Games;
using Core.StateMachines.Games.States;
using UnityEngine;
using Zenject;

namespace Core.Initializers.Projects
{
    public class ProjectInitializer : MonoBehaviour
    {
        [Inject]
        public void Initialize(GameStateMachine gameStateMachine, BootstrapStateFactory bootstrapStateFactory)
        {
            BootstrapState bootstrapState = bootstrapStateFactory.Create();
            gameStateMachine.AddState(bootstrapState);
            gameStateMachine.Set<BootstrapState>();
        }
    }
}