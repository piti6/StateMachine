using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Misokatsu
{
    public abstract class State : IState
    {
        public static readonly IState Empty = new EmptyState();

        bool IState.IsValidState => !(this is EmptyState);
        IReadOnlyDictionary<string, IStateMachine> IState.SubStateMachines => _subStateMachines;

        private IState IState => this;

        private readonly Dictionary<string, IStateMachine> _subStateMachines = new Dictionary<string, IStateMachine>();

        async UniTask IState.EnterAsync()
        {
            await OnEnter();

            if (IState.IsValidState)
            {
                foreach (var subStateMachine in _subStateMachines.Values)
                {
                    await subStateMachine.CurrentState.EnterAsync();
                }
            }
        }

        async UniTask IState.ExitAsync()
        {
            if (IState.IsValidState)
            {
                foreach (var subStateMachine in _subStateMachines.Values)
                {
                    await subStateMachine.CurrentState.ExitAsync();
                }
            }

            await OnExit();
        }

        void IState.AddSubStateMachine(string id, IState initialState)
        {
            _subStateMachines.Add(id, new StateMachine(id, initialState));
        }

        protected abstract UniTask OnEnter();
        protected abstract UniTask OnExit();
    }
}
