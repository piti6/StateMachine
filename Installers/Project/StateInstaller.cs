using Zenject;

namespace Misokatsu
{
    public class StateInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IRootStateMachine>().To<RootStateMachine>()
                .AsSingle();

            Container.BindFactory<string, SceneState, SceneState.Factory>();
        }
    }
}
