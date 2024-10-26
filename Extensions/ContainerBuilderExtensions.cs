using Misokatsu;
using System;
using System.Linq;
using VContainer;
using VContainer.Unity;

public static class ContainerBuilderExtensions
{
    public static void BuildController<TController>(this IContainerBuilder builder, Action<IContainerBuilder> installAction)
            where TController : ControllerBase, new()
    {
        BuildController<TController>(builder, new ActionInstaller(installAction));
    }

    public static void BuildController<TController>(this IContainerBuilder builder, params IInstaller[] installers)
            where TController : ControllerBase, new()
    {
        builder.RegisterFactory(() => BuildControllerInternal<TController>(builder, installers));
    }

    public static void BuildController<TParameter1, TController>(this IContainerBuilder builder, params IInstaller[] installers)
            where TController : ControllerBase, new()
    {
        builder.RegisterFactory<TParameter1, TController>
            (param => BuildControllerInternal<TController>(builder,
                installers.Append(new ActionInstaller(builder => builder.RegisterInstance(param))).ToArray()));
    }

    public static void BuildController<TParameter1, TParameter2, TController>(this IContainerBuilder builder, params IInstaller[] installers)
        where TController : ControllerBase, new()
    {
        builder.RegisterFactory<TParameter1, TParameter2, TController>
            ((param1, param2) => BuildControllerInternal<TController>(builder,
                installers.Append(new ActionInstaller(builder =>
                {
                    builder.RegisterInstance(param1);
                    builder.RegisterInstance(param2);
                })).ToArray()));
    }

    public static void BuildController<TParameter1, TParameter2, TParameter3, TController>(this IContainerBuilder builder, params IInstaller[] installers)
        where TController : ControllerBase, new()
    {
        builder.RegisterFactory<TParameter1, TParameter2, TParameter3, TController>
            ((param1, param2, param3) => BuildControllerInternal<TController>(builder,
                installers.Append(new ActionInstaller(builder =>
                {
                    builder.RegisterInstance(param1);
                    builder.RegisterInstance(param2);
                    builder.RegisterInstance(param3);
                })).ToArray()));
    }

    public static void BuildController<TParameter1, TParameter2, TParameter3, TParameter4, TController>(this IContainerBuilder builder, params IInstaller[] installers)
        where TController : ControllerBase, new()
    {
        builder.RegisterFactory<TParameter1, TParameter2, TParameter3, TParameter4, TController>
            ((param1, param2, param3, param4) => BuildControllerInternal<TController>(builder,
                installers.Append(new ActionInstaller(builder =>
                {
                    builder.RegisterInstance(param1);
                    builder.RegisterInstance(param2);
                    builder.RegisterInstance(param3);
                    builder.RegisterInstance(param4);
                })).ToArray()));
    }

    private static TController BuildControllerInternal<TController>(IContainerBuilder builder, params IInstaller[] installers)
        where TController : ControllerBase, new()
    {
        var lifetimeScope = (LifetimeScope)builder.ApplicationOrigin;

        var controller = new TController();

        var childLifetimeScope = lifetimeScope.CreateChild(x =>
        {
            EntryPointsBuilder.EnsureDispatcherRegistered(x);
            x.RegisterInstance(controller).AsImplementedInterfaces();

            foreach (var installer in installers)
            {
                installer.Install(x);
            }
        });

        childLifetimeScope.name = typeof(TController).Name;

        Action onContollerDispose = default;
        onContollerDispose = () =>
        {
            controller.OnDispose -= onContollerDispose;

            childLifetimeScope.Dispose();
        };

        controller.OnDispose += onContollerDispose;

        childLifetimeScope.Container.Inject(controller);

        return controller;
    }
}
