using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Misokatsu.Framework
{
    public sealed class EntryPointFactory<TEntryPoint, TInstaller, TView> : IAsyncFactory<TEntryPoint>
        where TEntryPoint : EntryPointBase, new()
        where TInstaller : IInstaller, new()
        where TView : MonoBehaviour
    {
        private readonly IObjectResolver _resolver;

        public EntryPointFactory(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        public async UniTask<TEntryPoint> CreateAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var entryPoint = new TEntryPoint();
            var holder = new GameObject($"{typeof(TEntryPoint).Name}Holder");
            entryPoint.AddTo(holder);

            var viewFactory = _resolver.Resolve<ViewFactory<TView>>();
            var view = await viewFactory.CreateAsync(cancellationToken);
            view.transform.SetParent(holder.transform);
            var childScope = _resolver.CreateScope(x =>
            {
                EntryPointsBuilder.EnsureDispatcherRegistered(x);
                x.RegisterInstance(entryPoint).AsImplementedInterfaces();
                x.RegisterInstance(view).AsImplementedInterfaces();
                new TInstaller().Install(x);
            });

            Action onEntryPointDispose = null;
            onEntryPointDispose = () =>
            {
                entryPoint.OnDispose -= onEntryPointDispose;
                childScope.Dispose();
            };

            entryPoint.OnDispose += onEntryPointDispose;
            childScope.Inject(entryPoint);

            return entryPoint;
        }
    }
}