using Zenject;

public class ApplicationEntryPoint : IApplicationEntryPoint
{
    [Inject]
    private readonly SceneState.Factory _sceneStateFactory = default;

    [Inject]
    private readonly IRootStateMachine _stateMachine = default;

    void IInitializable.Initialize()
    {
        var titleSceneState = _sceneStateFactory.Create(string.Empty);
        _stateMachine.ChangeTo(titleSceneState);
    }
}
