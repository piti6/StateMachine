public interface IStateMachine
{
    string Id { get; }

    IState CurrentState { get; }

    void ChangeTo(IState nextState);
    void ChangeToPrevious();
}
