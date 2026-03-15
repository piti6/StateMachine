using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Misokatsu.Framework
{
    public sealed class ViewFactory<TView> : IAsyncFactory<TView> where TView: Object
    {
        public async UniTask<TView> CreateAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var prefab = await Resources.LoadAsync(typeof(TView).Name);
            return (Object.Instantiate(prefab) as GameObject)!.GetComponentInChildren<TView>();
        }

    }
}
