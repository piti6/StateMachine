using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

public class DynamicPrefabAsyncResourceFactory
{
    [Inject]
    private readonly Container _container = default;

    public async UniTask<T> CreateAsync<T>(string prefabPath) where T : Component
    {
        Debug.Assert(!string.IsNullOrEmpty(prefabPath), $"Null or empty prefab resource name given to factory create method when instantiating object path {prefabPath} with type '{typeof(T)}'.");

        var instance = await CreateAsync(prefabPath);
        return instance.GetComponent<T>();
    }

    public async UniTask<GameObject> CreateAsync(string prefabPath)
    {
        Debug.Assert(!string.IsNullOrEmpty(prefabPath), $"Null or empty prefab resource name given to factory create method when instantiating object path {prefabPath}.");

        var prefab = await Resources.LoadAsync<GameObject>(prefabPath);

        Debug.Assert(prefab, $"Could not find prefab from path: {prefabPath}");

        var instance = Object.Instantiate(prefab) as GameObject;
        _container.Inject(instance);

        return instance;
    }
}
