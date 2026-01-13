using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Loggers;
using Core.StateMachines.Games.States;
using FluentResults;

namespace Core.StateMachines.Games
{
    public class GameStateMachine : BaseStateMachine<IGameState>
    {
        protected override IGameLogger.LogSystems LogSystems =>
            IGameLogger.LogSystems.GameStateMachine;

        private readonly Dictionary<IGameState.StateType, IGameState> _gameStates = new();

        public GameStateMachine(IGameLogger gameLogger) : base(gameLogger)
        {
        }

        public override Result AddState(IGameState state)
        {
            Result addStateResult = base.AddState(state);

            if(addStateResult.IsFailed)
                return addStateResult;

            return _gameStates.TryAdd(state.Type, state)
                ? Result.Ok()
                : Result.Fail($"Readded {state.Type.ToString()}");
        }

        public async Task<Result> Set(IGameState.StateType stateType)
        {
            if(!_gameStates.TryGetValue(stateType, out IGameState state))
            {
                return Result.Fail($"State {stateType.ToString()} not found");
            }
            
            return await Set(state);
        }
    }

}
