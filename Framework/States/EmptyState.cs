using Cysharp.Threading.Tasks;

namespace Misokatsu.Framework
{
    public class EmptyState : State
    {
        protected override UniTask OnEnterAsync()
        {
            return UniTask.CompletedTask;
        }

        protected override UniTask OnExitAsync()
        {
            return UniTask.CompletedTask;
        }
    }
}
