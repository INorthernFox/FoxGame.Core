using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.StateMachines.Games.States.Base
{
    public abstract class BaseGameState<T> : IGameState 
        where T : BaseGameState<T>
    {
        private HashSet<Type> _validTransitions;
        private bool _isConfigured;

        public Type StateType => typeof(T);

        public virtual Type NextStateType => null;

        public abstract IGameState.StateType Type { get; }

        protected abstract void ConfigureTransitions();

        protected void AllowTransitionFrom<TState>() where TState : IGameState =>
            _validTransitions.Add(typeof(TState));

        public bool CanTransitionFrom(Type fromStateType)
        {
            EnsureConfigured();
            return _validTransitions.Count == 0 || _validTransitions.Contains(fromStateType);
        }

        private void EnsureConfigured()
        {
            if(_validTransitions != null)
                return;
            
            _validTransitions = new HashSet<Type>();
            ConfigureTransitions();
        }

        public abstract Task Enter();

        public virtual Task Exit() => Task.CompletedTask;
    }
}