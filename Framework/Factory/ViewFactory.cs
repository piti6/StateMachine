using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Misokatsu.Framework
{
    public interface IViewFactory<TView> : IAsyncFactory<TView>
    {
    }
    
    public sealed class ViewFactory<TView> : IViewFactory<TView> where TView: Object
    {
        public ViewFactory()
        {
        }

        public async UniTask<TView> CreateAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await Resources.LoadAsync<TView>(nameof(TView)) as TView;
        }

    }
}
