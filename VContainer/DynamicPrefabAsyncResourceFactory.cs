using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class DynamicPrefabAsyncResourceFactory
{
    [Inject]
    private readonly LifetimeScope _scope = default;

    public async UniTask<T> CreateGameObjectAsync<T>(string prefabPath) where T : Component
    {
        Debug.Assert(!string.IsNullOrEmpty(prefabPath), $"Null or empty prefab resource name given to factory create method when instantiating object path {prefabPath} with type '{typeof(T)}'.");

        var instance = await CreateGameObjectAsync(prefabPath);
        return instance.GetComponent<T>();
    }

    public async UniTask<GameObject> CreateGameObjectAsync(string prefabPath)
    {
        Debug.Assert(!string.IsNullOrEmpty(prefabPath), $"Null or empty prefab resource name given to factory create method when instantiating object path {prefabPath}.");

        var prefab = await Resources.LoadAsync<GameObject>(prefabPath);

        Debug.Assert(prefab, $"Could not find prefab from path: {prefabPath}");

        var instance = Object.Instantiate(prefab) as GameObject;
        _scope.Container.Inject(instance);

        return instance;
    }
}
