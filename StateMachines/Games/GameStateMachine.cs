using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Loggers;
using Core.StateMachines.Games.States;

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

        public override AddStateResult AddState(IGameState state)
        {
            AddStateResult addStateResult = base.AddState(state);

            if(!addStateResult.Success)
                return addStateResult;

            return _gameStates.TryAdd(state.Type, state)
                ? AddStateResult.Valid()
                : AddStateResult.Invalid($"Readded {state.Type.ToString()}");
        }

        public async Task<SetStateResult> Set(IGameState.StateType stateType)
        {
            if(!_gameStates.TryGetValue(stateType, out IGameState state))
            {
                return SetStateResult.Invalid($"State {stateType.ToString()} not found");
            }
            
            return await Set(state);
        }
    }

}