namespace Misokatsu.Framework
{
    public interface IRootStateMachine : IStateMachine
    {
        void ChangeTo(string targetId, IState nextState);
        void ChangeToPrevious(string targetId);
    }
}
