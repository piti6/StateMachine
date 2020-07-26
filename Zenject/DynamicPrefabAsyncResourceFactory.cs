using ModestTree;
using UniRx.Async;
using UnityEngine;
using Zenject;

public class DynamicPrefabAsyncResourceFactory
{
    [Inject]
    private readonly DiContainer _container = default;

    public async UniTask<T> CreateAsync<T>(string prefabPath) where T : Component
    {
        Assert.That(!string.IsNullOrEmpty(prefabPath),
          "Null or empty prefab resource name given to factory create method when instantiating object with type '{0}'.", typeof(T));

        var prefab = await Resources.LoadAsync<GameObject>(prefabPath);

        Assert.IsNotNull(prefab, $"Could not find prefab from path: {prefabPath}");

        return _container.InstantiatePrefabForComponent<T>(prefab);
    }
}
