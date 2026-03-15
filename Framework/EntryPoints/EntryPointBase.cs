using R3;
using System;

namespace Misokatsu.Framework
{
    public class EntryPointBase : IDisposable
    {
        public event Action OnDispose = EmptyAction;

        private static readonly Action EmptyAction = () => { };

        protected readonly CompositeDisposable _disposables = new CompositeDisposable();

        private bool _disposed;
        public virtual void Dispose()
        {
            if (_disposed) return;
            
            _disposables.Dispose();

            OnDispose.Invoke();
            _disposed = true;
        }
    }
}
