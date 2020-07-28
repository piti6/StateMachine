using Zenject;

namespace Misokatsu
{
    public class ApplicationEntryPointInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<ApplicationEntryPoint>()
                .AsSingle();
        }
    }
}
