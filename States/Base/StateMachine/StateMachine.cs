using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Async;

namespace Misokatsu
{
    public class StateMachine : IStateMachine, IDisposable
    {
        string IStateMachine.Id => _id;

        private readonly string _id;

        private readonly ISubject<(IState State, bool NeedStackCurrentToPrevious)> _stateChangeRequested = new ReplaySubject<(IState State, bool NeedStackCurrentToPrevious)>(1);
        private readonly Stack<IState> _previousStates = new Stack<IState>();

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private IState _currentState = State.Empty;
        IState IStateMachine.CurrentState => _currentState;

        public StateMachine(string id) : this(id, State.Empty) { }
        public StateMachine(string id, IState initialState)
        {
            _id = id;

            _stateChangeRequested
                .StartWith((State: initialState, NeedStackCurrentToPrevious: true))
                .Select(x => ChangeStateInternalAsync(x.State, x.NeedStackCurrentToPrevious).ToObservable())
                .Concat()
                .Subscribe()
                .AddTo(_disposables);
        }

        void IStateMachine.ChangeTo(IState nextState)
        {
            ChangeState(nextState, true);
        }

        void IStateMachine.ChangeToPrevious()
        {
            if (_previousStates.Any())
            {
                ChangeState(_previousStates.Pop(), false);
            }
            else
            {
                ChangeState(State.Empty, false);
            }
        }

        private void ChangeState(IState nextState, bool needStackCurrentToPrevious)
        {
            _stateChangeRequested.OnNext((nextState, needStackCurrentToPrevious));
        }

        private async UniTask ChangeStateInternalAsync(IState nextState, bool needStackCurrentToPrevious)
        {
            var currentState = _currentState;

            await currentState.ExitAsync();

            if (needStackCurrentToPrevious)
            {
                _previousStates.Push(currentState);
            }

            _currentState = nextState;

            await nextState.EnterAsync();
        }

        void IDisposable.Dispose() => _disposables.Dispose();
    }
}
