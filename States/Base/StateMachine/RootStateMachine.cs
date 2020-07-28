using System.Collections.Generic;
using System.Linq;

namespace Misokatsu
{
    public class RootStateMachine : StateMachine, IRootStateMachine
    {
        public RootStateMachine() : base("Root")
        {
        }

        void IRootStateMachine.ChangeTo(string targetId, IState nextState)
        {
            var targetStateMachine = GetTargetStateMachine(targetId, this);
            targetStateMachine.ChangeTo(nextState);
        }

        void IRootStateMachine.ChangeToPrevious(string targetId)
        {
            var targetStateMachine = GetTargetStateMachine(targetId, this);
            targetStateMachine.ChangeToPrevious();
        }

        private static IStateMachine GetTargetStateMachine(string targetId, IStateMachine rootStateMachine)
        {
            var allStateMachines = GetCurrentAndSubStateMachinesRecursively(rootStateMachine);
            return allStateMachines.First(x => x.Id == targetId);
        }

        private static IEnumerable<IStateMachine> GetCurrentAndSubStateMachinesRecursively(IStateMachine targetStateMachine)
        {
            yield return targetStateMachine;

            foreach (var subStateMachine in targetStateMachine.CurrentState.SubStateMachines.Values)
            {
                foreach (var stateMachine in GetCurrentAndSubStateMachinesRecursively(subStateMachine))
                {
                    yield return stateMachine;
                }
            }
        }
    }
}
