using VContainer;
using VContainer.Unity;

namespace Misokatsu
{
    public class StateInstaller : IInstaller
    {
        void IInstaller.Install(IContainerBuilder builder)
        {
            builder.RegisterContainer();
            builder.Register<RootStateMachine>(Lifetime.Singleton)
                .AsImplementedInterfaces()
                .AsSelf();

            builder.RegisterFactory<string, SceneState>(container => sceneName => container.CreateInstance<SceneState>(sceneName.AsTypedParameter()), Lifetime.Singleton);
            builder.Register<DynamicPrefabAsyncResourceFactory>(Lifetime.Singleton);
        }
    }
}
