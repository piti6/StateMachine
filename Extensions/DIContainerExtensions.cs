using Zenject;

public static class DIContainerExtensions
{
    public static NonLazyBinder
            BindSubContainerFromNewGameObjectInstaller<TController, TInstaller>(this DiContainer container)
            where TController : ControllerBase
            where TInstaller : InstallerBase
    {
        return container.BindFactory<TController, PlaceholderFactory<TController>>()
            .FromSubContainerResolve()
            .ByNewGameObjectInstaller<TInstaller>()
            .WithGameObjectName(typeof(TController).Name);
    }
}
