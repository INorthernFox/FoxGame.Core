using Core.Loggers;
using Core.StateMachines.Games;
using Core.StateMachines.Games.States;
using Core.StateMachines.Games.States.Bootstraps;
using FluentResults;
using UnityEngine;
using Zenject;

namespace Core.Initializers.Projects
{
    public class ProjectInitializer : MonoBehaviour
    {
        private const string LogKey = nameof(ProjectInitializer);

        [Inject]
        public async void Initialize(
            GameStateMachine gameStateMachine,
            BootstrapStateFactory bootstrapStateFactory,
            IGameLogger logger)
        {
            BootstrapState bootstrapState = bootstrapStateFactory.Create();

            Result addStateResult = gameStateMachine.AddState(bootstrapState);
            if (addStateResult.IsFailed)
            {
                logger.LogError(IGameLogger.LogSystems.GameStateMachine,
                    $"Failed to add BootstrapState: {string.Join(", ", addStateResult.Errors)}",
                    LogKey, this);
                return;
            }

            Result setStateResult = await gameStateMachine.Set(IGameState.StateType.Bootstrap);
            if (setStateResult.IsFailed)
            {
                logger.LogError(IGameLogger.LogSystems.GameStateMachine,
                    $"Failed to set Bootstrap state: {string.Join(", ", setStateResult.Errors)}",
                    LogKey, this);
            }
        }
    }
}