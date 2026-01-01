using System;
using System.Threading.Tasks;

namespace Core.StateMachines
{
    public interface IState
    {
        public Type StateType { get; }
        public Type NextStateType { get; }

        public Task Enter();
        public Task Exit();
    }
}