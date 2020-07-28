using UniRx.Async;

namespace Misokatsu
{
    public class EmptyState : State
    {
        protected override UniTask OnEnter()
        {
            return UniTask.CompletedTask;
        }

        protected override UniTask OnExit()
        {
            return UniTask.CompletedTask;
        }
    }
}
