using System;
using UniRx;

namespace Misokatsu
{
    public class ControllerBase : IDisposable
    {
        public event Action OnDispose = EmptyAction;

        private readonly static Action EmptyAction = () => { };

        protected readonly CompositeDisposable _disposables = new CompositeDisposable();

        public void Dispose()
        {
            _disposables.Dispose();

            OnDispose.Invoke();
        }
    }
}
