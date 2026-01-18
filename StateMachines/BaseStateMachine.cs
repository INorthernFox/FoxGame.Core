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
        private readonly IGameLogger _baseLogger;
        private readonly Dictionary<Type, TState> _states = new();

        private PersonalizedLogger _logger;
        private TState _currentState;

        public IState CurrentState => _currentState;

        protected PersonalizedLogger Logger => _logger ??= CreateLogger();

        protected abstract IGameLogger.LogSystems LogSystems { get; }

        protected BaseStateMachine(IGameLogger gameLogger) =>
            _baseLogger = gameLogger;

        private PersonalizedLogger CreateLogger() =>
            new(_baseLogger, LogSystems, GetType().Name, this);

        public virtual Result AddState(TState state)
        {
            if (state == null)
            {
                const string message = "Attempted to add null state";
                Logger.LogError(message);
                return Result.Fail(message);
            }

            var stateType = state.StateType ?? state.GetType();

            if (!_states.TryAdd(stateType, state))
            {
                var message = $"State {stateType.Name} already registered";
                Logger.LogWarning(message);
                return Result.Fail(message);
            }

            Logger.LogInfo($"State {stateType.Name} registered");
            return Result.Ok();
        }

        public Task<Result> Set<T>()
            where T : TState
        {
            return Set(typeof(T));
        }

        protected async Task<Result> Set(TState nextState)
        {
            Logger.LogInfo($"Start set {nextState.StateType.Name} state");

            if (_currentState == nextState)
                return Result.Fail($"{nextState.StateType.Name} state is already set");

            var previousState = _currentState;

            if (previousState != null)
            {
                if (!nextState.CanTransitionFrom(previousState.StateType))
                {
                    var message = $"Invalid transition from {previousState.StateType.Name} to {nextState.StateType.Name}";
                    Logger.LogError(message);
                    return Result.Fail(message);
                }

                Logger.LogInfo($"Exiting {previousState.StateType?.Name ?? previousState.GetType().Name}");

                try
                {
                    await previousState.Exit();
                }
                catch (Exception exception)
                {
                    Logger.LogError($"Error while exiting {previousState.StateType?.Name ?? previousState.GetType().Name}: {exception}");
                    return Result.Fail(exception.Message);
                }
            }

            Logger.LogInfo($"Entering {nextState.GetType().Name}");

            try
            {
                await nextState.Enter();
                _currentState = nextState;

                if (nextState.NextStateType == null)
                    return Result.Ok();

                Logger.LogInfo($"Starting set auto Next State {nextState.NextStateType.Name}");
                return await Set(nextState.NextStateType);
            }
            catch (Exception exception)
            {
                Logger.LogError($"Error while entering {nextState.GetType().Name}: {exception}");
                _currentState = null;
                return Result.Fail(exception.Message);
            }
        }

        private async Task<Result> Set(Type stateType)
        {
            if (stateType == null)
            {
                const string message = "Set called with null type";
                Logger.LogError(message);
                return Result.Fail(message);
            }

            if (!_states.TryGetValue(stateType, out var nextState))
            {
                var message = $"State {stateType.Name} is not registered";
                Logger.LogError(message);
                return Result.Fail(message);
            }

            return await Set(nextState);
        }
    }
}