//USING_ZENJECT
#if USING_ZENJECT
using UnityEngine;
using Zenject;

public class ExtendedPrefabFactory<T> : IFactory<Object, Vector3, T>
{
    readonly DiContainer _container;

    public ExtendedPrefabFactory(DiContainer container)
    {
        _container = container;
    }

    public T Create(Object prefab, Vector3 position)
    {
        var instance = _container.InstantiatePrefab(prefab, position, Quaternion.identity, null);
        instance.name = prefab.name;
        return instance.GetComponent<T>();
    }
}
#endif
