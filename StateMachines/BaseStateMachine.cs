using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Loggers;

namespace Core.StateMachines
{
    public class BaseStateMachine
    {
        private const string LogKey = nameof(BaseStateMachine);

        private readonly ILogger _logger;
        private readonly Dictionary<Type, IState> _states = new();

        private IState _currentState;

        public BaseStateMachine(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void AddState(IState state)
        {
            if(state == null)
            {
                _logger.LogError("Attempted to add null state", LogKey, this);
                return;
            }

            Type stateType = state.StateType ?? state.GetType();
            
            if(!_states.TryAdd(stateType, state))
            {
                _logger.LogWarning($"State {stateType.Name} already registered", LogKey, this);
                return;
            }

            _logger.LogInfo($"State {stateType.Name} registered", LogKey, this);
        }

        public Task Set<T>()
            where T : IState
        {
            return Set(typeof(T));
        }

        private async Task Set(Type stateType)
        {
            if(stateType == null)
            {
                _logger.LogError("Set called with null type", LogKey, this);
                return;
            }

            if(!_states.TryGetValue(stateType, out var nextState))
            {
                _logger.LogError($"State {stateType.Name} is not registered", LogKey, this);
                return;
            }

            if(_currentState == nextState)
            {
                _logger.LogInfo($"State {stateType.Name} already active", LogKey, this);
                return;
            }

            IState previousState = _currentState;
            
            if(previousState != null)
            {
                _logger.LogInfo($"Exiting {previousState.StateType?.Name ?? previousState.GetType().Name}", LogKey, this);
                try
                {
                    await previousState.Exit();
                }
                catch (Exception exception)
                {
                    _logger.LogError($"Error while exiting {previousState.StateType?.Name ?? previousState.GetType().Name}: {exception}", LogKey, this);
                    throw;
                }
            }

            _logger.LogInfo($"Entering {stateType.Name}", LogKey, this);
            
            try
            {
                await nextState.Enter();
                _currentState = nextState;
                
                if(nextState.NextStateType != null)
                {
                    await Set(nextState.NextStateType);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError($"Error while entering {stateType.Name}: {exception}", LogKey, this);
                _currentState = null;
                throw;
            }
        }
    }

}