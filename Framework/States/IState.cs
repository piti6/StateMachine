using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace Misokatsu.Framework
{
    public interface IState
    {
        bool IsValidState { get; }

        IStateMachine StateMachine { get; }
        IReadOnlyDictionary<string, IStateMachine> SubStateMachines { get; }

        void AddSubStateMachine(string id, IState initialState);
        void AddStateMachine(IStateMachine stateMachine);

        UniTask EnterAsync();
        UniTask ExitAsync();
    }
}
