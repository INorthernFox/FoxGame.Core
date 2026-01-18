using System;
using System.Threading.Tasks;
using FluentResults;

namespace Core.StateMachines
{
    public interface IState
    {
        public Type StateType { get; }
        public Type NextStateType { get; }

        public bool CanTransitionFrom(Type nextStateType);
        
        public Task Enter();
        public Task Exit();
    }
}