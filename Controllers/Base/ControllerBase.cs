using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace Misokatsu
{
    public class ControllerBase : IDisposable
    {
        protected readonly CompositeDisposable _disposables = new CompositeDisposable();

        [Inject]
        private readonly GameObjectContext _currentContext = default;

        public void Dispose()
        {
            _disposables.Dispose();
            GameObject.Destroy(_currentContext.gameObject);
        }
    }
}
