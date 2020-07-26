using Zenject;

public class ApplicationEntryPointInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<ApplicationEntryPoint>()
            .AsSingle();
    }
}
