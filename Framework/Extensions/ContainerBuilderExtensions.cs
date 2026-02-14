using System;
using System.Collections.Generic;
using System.Linq;
using VContainer;
using VContainer.Internal;
using VContainer.Unity;

namespace Misokatsu.Framework
{
    public static class ContainerBuilderExtensions
    {
        public static RegistrationBuilder RegisterAsImplementedInterfacesAndSelf<T>(
            this IContainerBuilder builder,
            Lifetime lifetime)
        {
            return builder.Register<T>(lifetime)
                .AsImplementedInterfaces()
                .AsSelf();
        }

        public static void BuildEntryPoint<TEntryPoint>(this IContainerBuilder builder,
            Action<IContainerBuilder> installAction)
            where TEntryPoint : EntryPointBase, new()
        {
            BuildEntryPoint<TEntryPoint>(builder, new ActionInstaller(installAction));
        }

        public static void BuildEntryPoint<TEntryPoint, TInstaller>(this IContainerBuilder builder, params IInstaller[] additionalInstallers)
            where TEntryPoint : EntryPointBase, new()
            where TInstaller : new()
        {
            builder.RegisterFactory(() => BuildEntryPointInternal<TEntryPoint>(builder, additionalInstallers));
        }

        public static void BuildEntryPoint<TParameter1, TEntryPoint>(this IContainerBuilder builder,
            params IInstaller[] additionalInstallers)
            where TEntryPoint : EntryPointBase, new()
        {
            builder.RegisterFactory<TParameter1, TEntryPoint>
            (param => BuildEntryPointInternal<TEntryPoint>(builder,
                additionalInstallers.Append(new ActionInstaller(builder => builder.RegisterInstance(param))).ToArray()));
        }

        public static void BuildEntryPoint<TParameter1, TParameter2, TEntryPoint>(this IContainerBuilder builder,
            params IInstaller[] additionalInstallers)
            where TEntryPoint : EntryPointBase, new()
        {
            builder.RegisterFactory<TParameter1, TParameter2, TEntryPoint>
            ((param1, param2) => BuildEntryPointInternal<TEntryPoint>(builder,
                additionalInstallers.Append(new ActionInstaller(builder =>
                {
                    builder.RegisterInstance(param1);
                    builder.RegisterInstance(param2);
                })).ToArray()));
        }

        public static void BuildEntryPoint<TParameter1, TParameter2, TParameter3, TEntryPoint>(
            this IContainerBuilder builder, params IInstaller[] additionalInstallers)
            where TEntryPoint : EntryPointBase, new()
        {
            builder.RegisterFactory<TParameter1, TParameter2, TParameter3, TEntryPoint>
            ((param1, param2, param3) => BuildEntryPointInternal<TEntryPoint>(builder,
                additionalInstallers.Append(new ActionInstaller(x =>
                {
                    x.RegisterInstance(param1);
                    x.RegisterInstance(param2);
                    x.RegisterInstance(param3);
                })).ToArray()));
        }

        public static void BuildEntryPoint<TParameter1, TParameter2, TParameter3, TParameter4, TEntryPoint>(
            this IContainerBuilder builder, params IInstaller[] additionalInstallers)
            where TEntryPoint : EntryPointBase, new()
        {
            builder.RegisterFactory<TParameter1, TParameter2, TParameter3, TParameter4, TEntryPoint>
            ((param1, param2, param3, param4) => BuildEntryPointInternal<TEntryPoint>(builder,
                additionalInstallers.Append(new ActionInstaller(x =>
                {
                    x.RegisterInstance(param1);
                    x.RegisterInstance(param2);
                    x.RegisterInstance(param3);
                    x.RegisterInstance(param4);
                })).ToArray()));
        }

        private static TEntryPoint BuildEntryPointInternal<TEntryPoint>(IContainerBuilder builder,
            params IInstaller[] installers)
            where TEntryPoint : EntryPointBase, new()
        {
            var lifetimeScope = (LifetimeScope)builder.ApplicationOrigin;

            var entryPoint = new TEntryPoint();

            var childLifetimeScope = lifetimeScope.CreateChild(x =>
            {
                EntryPointsBuilder.EnsureDispatcherRegistered(x);
                x.RegisterInstance(entryPoint).AsImplementedInterfaces();

                foreach (var installer in installers)
                {
                    installer.Install(x);
                }
            });

            childLifetimeScope.name = typeof(TEntryPoint).Name;

            Action onContollerDispose = default;
            onContollerDispose = () =>
            {
                entryPoint.OnDispose -= onContollerDispose;

                childLifetimeScope.Dispose();
            };

            entryPoint.OnDispose += onContollerDispose;

            childLifetimeScope.Container.Inject(entryPoint);

            return entryPoint;
        }
    }
}
