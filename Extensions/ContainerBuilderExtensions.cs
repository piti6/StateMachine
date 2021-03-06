using Misokatsu;
using System;
using VContainer;
using VContainer.Unity;

public static class ContainerBuilderExtensions
{
    public static void BuildController<TController>(this IContainerBuilder builder, params IInstaller[] installers)
            where TController : ControllerBase, new()
    {
        builder.RegisterFactory(() =>
        {
            var lifetimeScope = (LifetimeScope)builder.ApplicationOrigin;

            var controller = new TController();

            var childLifetimeScope = lifetimeScope.CreateChild(x =>
            {
                x.RegisterInstance<TController>(controller).AsImplementedInterfaces();

                foreach (var installer in installers)
                {
                    installer.Install(x);
                }
            });

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
