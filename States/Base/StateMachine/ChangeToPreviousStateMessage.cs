namespace Misokatsu
{
    public readonly struct ChangeToPreviousStateMessage
    {
        public string StateMachineId { get; }

        public ChangeToPreviousStateMessage(string stateMachineId)
        {
            StateMachineId = stateMachineId;
        }
    }
}
