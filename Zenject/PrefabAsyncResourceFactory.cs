using ModestTree;
using UniRx.Async;
using UnityEngine;
using Zenject;

public class PrefabAsyncResourceFactory<T> where T : Component
{
    [Inject]
    private readonly DiContainer _container = default;
    [Inject]
    private readonly string _prefabPath = default;


    public async UniTask<T> CreateAsync()
    {
        Assert.That(!string.IsNullOrEmpty(_prefabPath),
          "Null or empty prefab resource name given to factory create method when instantiating object with type '{0}'.", typeof(T));

        var prefab = await Resources.LoadAsync<GameObject>(_prefabPath);

        Assert.IsNotNull(prefab, $"Could not find prefab from path: {_prefabPath}");

        return _container.InstantiatePrefabForComponent<T>(prefab);
    }
}
