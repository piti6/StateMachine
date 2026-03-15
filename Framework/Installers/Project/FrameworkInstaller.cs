using MessagePipe;
using VContainer;
using VContainer.Unity;

namespace Misokatsu.Framework
{
    public class FrameworkInstaller : IInstaller
    {
        void IInstaller.Install(IContainerBuilder builder)
        {
            EntryPointsBuilder.EnsureDispatcherRegistered(builder);
            
            var options = builder.RegisterMessagePipe();
            builder.RegisterMessageBroker<ChangeStateMessage>(options);
            builder.RegisterMessageBroker<ChangeToPreviousStateMessage>(options);
            
            builder.Register<RootStateMachine>(Lifetime.Singleton)
                .AsImplementedInterfaces()
                .AsSelf();
            
            builder.Register(typeof(IFactory<>), typeof(ObjectResolverFactory<>), Lifetime.Scoped);
            builder.Register(typeof(IFactory<,>), typeof(ObjectResolverFactory<,>), Lifetime.Scoped);
            builder.Register(typeof(EntryPointFactory<,,>), Lifetime.Scoped);
            builder.Register(typeof(ViewFactory<>), Lifetime.Scoped);
            builder.Register(typeof(SceneStateFactory<,>), Lifetime.Scoped);
            builder.Register(typeof(StateFactory<,>), Lifetime.Scoped);
            builder.Register(typeof(StateFactory<>), Lifetime.Scoped);
        }
    }
}
