using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;

namespace Misokatsu.Framework
{
    public sealed class EntryPointFactory<TEntryPoint, TInstaller, TView> : IAsyncFactory<TEntryPoint>
        where TEntryPoint : EntryPointBase, new()
        where TInstaller : IInstaller, new()
    {
        private readonly IContainerBuilder _builder;
        private readonly IObjectResolver _resolver;

        public EntryPointFactory(IContainerBuilder builder, IObjectResolver resolver)
        {
            _builder = builder;
            _resolver = resolver;
        }

        public async UniTask<TEntryPoint> CreateAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var lifetimeScope = (LifetimeScope)_builder.ApplicationOrigin;
            var entryPoint = new TEntryPoint();
            
            var viewFactory = _resolver.Resolve<IAsyncFactory<TView>>();
            var view = await viewFactory.CreateAsync(cancellationToken);

            var childLifetimeScope = lifetimeScope.CreateChild(x =>
            {
                EntryPointsBuilder.EnsureDispatcherRegistered(x);
                x.RegisterInstance(entryPoint).AsImplementedInterfaces();
                x.RegisterInstance(view).AsImplementedInterfaces();
                new TInstaller().Install(x);
            });

            childLifetimeScope.name = typeof(TEntryPoint).Name;

            Action onControllerDispose = null;
            onControllerDispose = () =>
            {
                entryPoint.OnDispose -= onControllerDispose;
                childLifetimeScope.Dispose();
            };

            entryPoint.OnDispose += onControllerDispose;
            childLifetimeScope.Container.Inject(entryPoint);

            return entryPoint;
        }

    }
}
