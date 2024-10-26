using R3;
using System;

namespace Misokatsu
{
    public class ControllerBase : IDisposable
    {
        public event Action OnDispose = EmptyAction;

        private static readonly Action EmptyAction = () => { };

        protected readonly CompositeDisposable _disposables = new CompositeDisposable();

        public virtual void Dispose()
        {
            _disposables.Dispose();

            OnDispose.Invoke();
        }
    }
}
