using VContainer;
using VContainer.Unity;

namespace Misokatsu.Framework
{
    public class StateInstaller : IInstaller
    {
        void IInstaller.Install(IContainerBuilder builder)
        {
            EntryPointsBuilder.EnsureDispatcherRegistered(builder);
            builder.Register<RootStateMachine>(Lifetime.Singleton)
                .AsImplementedInterfaces()
                .AsSelf();

            builder.RegisterFactory<string, SceneState>(container => container.CreateInstance<string, SceneState>, Lifetime.Singleton);
        }
    }
}
