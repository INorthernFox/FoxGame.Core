using Core.Loggers;
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
            BootstrapStateFactory bootstrapStateFactory,
            IGameLogger baseLogger)
        {
            var logger = new PersonalizedLogger(baseLogger, IGameLogger.LogSystems.Initializers, nameof(ProjectInitializer), this);

            var bootstrapState = bootstrapStateFactory.Create();

            var addStateResult = gameStateMachine.AddState(bootstrapState);
            if (addStateResult.IsFailed)
            {
                logger.LogError($"Failed to add BootstrapState: {string.Join(", ", addStateResult.Errors)}");
                return;
            }

            var setStateResult = await gameStateMachine.Set(IGameState.StateType.Bootstrap);
            if (setStateResult.IsFailed)
                logger.LogError($"Failed to set Bootstrap state: {string.Join(", ", setStateResult.Errors)}");
        }
    }
}