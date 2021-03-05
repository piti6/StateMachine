using Misokatsu;
using System;
using VContainer;
using VContainer.Unity;

public static class ContainerBuilderExtensions
{
    public static void BuildController<TController, TInstaller>(this IContainerBuilder builder)
            where TController : ControllerBase, new()
            where TInstaller : IInstaller, new()
    {
        builder.RegisterFactory(() =>
        {
            var installer = new TInstaller();
            var lifetimeScope = (LifetimeScope)builder.ApplicationOrigin;

            var childLifetimeScope = lifetimeScope.CreateChild(installer);
            var controller = new TController();
            Action onContollerDispose = default;
            onContollerDispose = () =>
            {
                controller.OnDispose -= onContollerDispose;

                childLifetimeScope.Dispose();
            };

            controller.OnDispose += onContollerDispose;

            childLifetimeScope.Container.Inject(controller);

            return controller;
        });
    }
}
