namespace Misokatsu.Framework
{
    public readonly struct ChangeStateMessage
    {
        public string StateMachineId { get; }
        public IState State { get; }

        public ChangeStateMessage(string stateMachineId, IState state)
        {
            StateMachineId = stateMachineId;
            State = state;
        }
    }
}
