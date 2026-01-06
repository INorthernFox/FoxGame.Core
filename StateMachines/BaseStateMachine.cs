using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Loggers;

namespace Core.StateMachines
{

    public abstract class BaseStateMachine<TState>
        where TState : class, IState
    {
        private const string LogKey = nameof(BaseStateMachine<TState>);

        private readonly IGameLogger _gameLogger;
        private readonly Dictionary<Type, TState> _states = new();

        private TState _currentState;

        protected abstract IGameLogger.LogSystems LogSystems { get; }

        protected BaseStateMachine(IGameLogger gameLogger)
        {
            _gameLogger = gameLogger ?? throw new ArgumentNullException(nameof(gameLogger));
        }

        public virtual AddStateResult AddState(TState state)
        {
            if(state == null)
            {
                const string message = "Attempted to add null state";
                _gameLogger.LogError(LogSystems, message, LogKey, this);
                return AddStateResult.Invalid(message);
            }

            Type stateType = state.StateType ?? state.GetType();

            if(!_states.TryAdd(stateType, state))
            {
                string message = $"State {stateType.Name} already registered";
                _gameLogger.LogWarning(LogSystems, message, LogKey, this);
                return AddStateResult.Invalid(message);
            }

            _gameLogger.LogInfo(LogSystems, $"State {stateType.Name} registered", LogKey, this);
            return AddStateResult.Valid();
        }

        public Task<SetStateResult> Set<T>()
            where T : TState
        {
            return Set(typeof(T));
        }

        protected async Task<SetStateResult> Set(TState nextState)
        {
            if(_currentState == nextState)
            {
                return SetStateResult.Invalid($"{nextState.StateType.Name} state is already set");
            }

            IState previousState = _currentState;

            if(previousState != null)
            {
                _gameLogger.LogInfo(LogSystems, $"Exiting {previousState.StateType?.Name ?? previousState.GetType().Name}", LogKey, this);
                try
                {
                    await previousState.Exit();
                }
                catch (Exception exception)
                {
                    _gameLogger.LogError(LogSystems, $"Error while exiting {previousState.StateType?.Name ?? previousState.GetType().Name}: {exception}", LogKey, this);
                    return SetStateResult.Invalid(exception.Message);
                }
            }

            _gameLogger.LogInfo(LogSystems, $"Entering {nextState.GetType().Name}", LogKey, this);

            try
            {
                await nextState.Enter();
                _currentState = nextState;

                if(nextState.NextStateType == null)
                    return SetStateResult.Valid();

                return await Set(nextState.NextStateType);
            }
            catch (Exception exception)
            {
                _gameLogger.LogError(LogSystems, $"Error while entering {nextState.GetType().Name}: {exception}", LogKey, this);
                _currentState = null;
                return SetStateResult.Invalid(exception.Message);
            }
        }

        private async Task<SetStateResult> Set(Type stateType)
        {
            if(stateType == null)
            {
                string message = "Set called with null type";
                _gameLogger.LogError(LogSystems, message, LogKey, this);
                return SetStateResult.Invalid(message);
            }

            if(!_states.TryGetValue(stateType, out TState nextState))
            {
                string message = $"State {stateType.Name} is not registered";
                _gameLogger.LogError(LogSystems, message, LogKey, this);
                return SetStateResult.Invalid(message);
            }

            return await Set(nextState);
        }
    }

}