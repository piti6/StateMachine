using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class DynamicPrefabAsyncResourceFactory
{
    [Inject]
    private readonly LifetimeScope _scope = default;

    public async UniTask<T> CreateGameObjectAsync<T>(string prefabPath, Transform parent, bool instantiateInWorldSpace = false) where T : Component
    {
        Debug.Assert(!string.IsNullOrEmpty(prefabPath), $"Null or empty prefab resource name given to factory create method when instantiating object path {prefabPath} with type '{typeof(T)}'.");

        var instance = await CreateGameObjectAsync(prefabPath, parent, instantiateInWorldSpace);
        return instance.GetComponent<T>();
    }

    public async UniTask<T> CreateGameObjectAsync<T>(string prefabPath, bool instantiateInWorldSpace = false) where T : Component
    {
        Debug.Assert(!string.IsNullOrEmpty(prefabPath), $"Null or empty prefab resource name given to factory create method when instantiating object path {prefabPath} with type '{typeof(T)}'.");

        var instance = await CreateGameObjectAsync(prefabPath, _scope.transform, instantiateInWorldSpace);
        return instance.GetComponent<T>();
    }

    public UniTask<GameObject> CreateGameObjectAsync(string prefabPath, bool instantiateInWorldSpace = false)
    {
        return CreateGameObjectAsync(prefabPath, _scope.transform, instantiateInWorldSpace);
    }

    public async UniTask<GameObject> CreateGameObjectAsync(string prefabPath, Transform parent, bool instantiateInWorldSpace = false)
    {
        Debug.Assert(!string.IsNullOrEmpty(prefabPath), $"Null or empty prefab resource name given to factory create method when instantiating object path {prefabPath}.");

        var prefab = await Resources.LoadAsync<GameObject>(prefabPath);

        Debug.Assert(prefab, $"Could not find prefab from path: {prefabPath}");

        var instance = Object.Instantiate(prefab, parent, instantiateInWorldSpace) as GameObject;
        _scope.Container.Inject(instance);

        return instance;
    }
}
