using Cysharp.Threading.Tasks;
using R3;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Misokatsu.Framework
{
    public class StateMachine : IStateMachine, IDisposable
    {
        string IStateMachine.Id => _id;

        private readonly string _id;

        private readonly Stack<IState> _previousStates = new Stack<IState>();
        private readonly Channel<(IState State, bool NeedStackCurrentToPrevious)> _stateChangeChannel = Channel.CreateSingleConsumerUnbounded<(IState State, bool NeedStackCurrentToPrevious)>();
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        protected readonly CompositeDisposable _disposables = new CompositeDisposable();

        private IState _currentState = State.Empty;
        IState IStateMachine.CurrentState => _currentState;

        public StateMachine(string id) : this(id, State.Empty) { }
        public StateMachine(string id, IState initialState)
        {
            _id = id;
            _stateChangeChannel.Writer.TryWrite((initialState, true));
            ConsumeStateChangesAsync(_cts.Token).Forget();
        }

        void IStateMachine.ChangeTo(IState nextState)
        {
            ChangeState(nextState, true);
        }

        void IStateMachine.ChangeToPrevious()
        {
            if (_previousStates.Count > 0)
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
            _stateChangeChannel.Writer.TryWrite((nextState, needStackCurrentToPrevious));
        }

        private async UniTask ChangeStateInternalAsync(IState nextState, bool needStackCurrentToPrevious)
        {
            var currentState = _currentState;

            await currentState.ExitAsync();

            if (needStackCurrentToPrevious)
            {
                _previousStates.Push(currentState);
            }

            nextState.AddStateMachine(this);
            _currentState = nextState;

            await nextState.EnterAsync();
        }

        private async UniTaskVoid ConsumeStateChangesAsync(CancellationToken ct)
        {
            await foreach (var (state, needStack) in _stateChangeChannel.Reader.ReadAllAsync(ct))
            {
                try
                {
                    await ChangeStateInternalAsync(state, needStack);
                }
                catch (Exception exception)
                {
                    FrameworkExceptionHandler.Handle(exception);
                }
            }
        }

        void IDisposable.Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
            _disposables.Dispose();
        }
    }

    public static class FrameworkExceptionHandler
    {
        public static Action<Exception> Handler { get; set; } = Debug.LogException;

        public static void Handle(Exception exception)
        {
            Handler?.Invoke(exception);
        }
    }

    internal static class FrameworkDomainReloadGuard
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            FrameworkExceptionHandler.Handler = Debug.LogException;
            State.ResetStatics();
        }
    }
}
