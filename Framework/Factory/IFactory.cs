using System.Threading;
using Cysharp.Threading.Tasks;

namespace Misokatsu.Framework
{
    public interface IFactory<out T>
    {
        T Create();
    }

    public interface IFactory<in TParam, out TResult>
    {
        TResult Create(TParam param);
    }
}
