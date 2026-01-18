using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Loggers;
using FluentResults;

namespace Core.StateMachines
{

    public abstract class BaseStateMachine<TState>
        where TState : class, IState
    {
        private const string LogKey = nameof(BaseStateMachine<TState>);

        private readonly IGameLogger _gameLogger;
        private readonly Dictionary<Type, TState> _states = new();

        private TState _currentState;

        public IState CurrentState =>
            _currentState;

        protected IGameLogger GameLogger => _gameLogger;

        protected abstract IGameLogger.LogSystems LogSystems { get; }

        protected BaseStateMachine(IGameLogger gameLogger)
        {
            _gameLogger = gameLogger ?? throw new ArgumentNullException(nameof(gameLogger));
        }

        public virtual Result AddState(TState state)
        {
            if(state == null)
            {
                const string message = "Attempted to add null state";
                _gameLogger.LogError(LogSystems, message, LogKey, this);
                return Result.Fail(message);
            }

            Type stateType = state.StateType ?? state.GetType();

            if(!_states.TryAdd(stateType, state))
            {
                string message = $"State {stateType.Name} already registered";
                _gameLogger.LogWarning(LogSystems, message, LogKey, this);
                return Result.Fail(message);
            }

            _gameLogger.LogInfo(LogSystems, $"State {stateType.Name} registered", LogKey, this);
            return Result.Ok();
        }

        public Task<Result> Set<T>()
            where T : TState
        {
            return Set(typeof(T));
        }

        protected async Task<Result> Set(TState nextState)
        {
            if(_currentState == nextState)
            {
                return Result.Fail($"{nextState.StateType.Name} state is already set");
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
                    return Result.Fail(exception.Message);
                }
            }

            _gameLogger.LogInfo(LogSystems, $"Entering {nextState.GetType().Name}", LogKey, this);

            try
            {
                await nextState.Enter();
                _currentState = nextState;

                if(nextState.NextStateType == null)
                    return Result.Ok();

                return await Set(nextState.NextStateType);
            }
            catch (Exception exception)
            {
                _gameLogger.LogError(LogSystems, $"Error while entering {nextState.GetType().Name}: {exception}", LogKey, this);
                _currentState = null;
                return Result.Fail(exception.Message);
            }
        }

        private async Task<Result> Set(Type stateType)
        {
            if(stateType == null)
            {
                string message = "Set called with null type";
                _gameLogger.LogError(LogSystems, message, LogKey, this);
                return Result.Fail(message);
            }

            if(!_states.TryGetValue(stateType, out TState nextState))
            {
                string message = $"State {stateType.Name} is not registered";
                _gameLogger.LogError(LogSystems, message, LogKey, this);
                return Result.Fail(message);
            }

            return await Set(nextState);
        }
    }

}