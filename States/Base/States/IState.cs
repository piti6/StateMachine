using System.Collections.Generic;
using UniRx.Async;

namespace Misokatsu
{
    public interface IState
    {
        bool IsValidState { get; }

        IReadOnlyDictionary<string, IStateMachine> SubStateMachines { get; }

        void AddSubStateMachine(string id, IState initialState);

        UniTask EnterAsync();
        UniTask ExitAsync();
    }
}
