using System.Threading;
using Cysharp.Threading.Tasks;

namespace Misokatsu.Framework
{
    public interface IAsyncFactory<T>
    {
        UniTask<T> CreateAsync(CancellationToken cancellationToken = default);
    }

    public interface IAsyncFactory<in TParam, TResult>
    {
        UniTask<TResult> CreateAsync(TParam param);
    }
}
