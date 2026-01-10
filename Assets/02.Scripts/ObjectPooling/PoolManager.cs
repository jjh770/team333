using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour, IPoolManager
{
    [SerializeField] private int _defaultPoolSize = 10;

    private readonly Dictionary<GameObject, Queue<GameObject>> _pools = new();
    private readonly Dictionary<GameObject, GameObject> _prefabLookup = new();

    public void Preload(GameObject prefab, int count)
    {
        if (!_pools.ContainsKey(prefab))
        {
            _pools[prefab] = new Queue<GameObject>();
        }

        for (int i = 0; i < count; i++)
        {
            GameObject obj = CreateNewObject(prefab);
            _pools[prefab].Enqueue(obj);
        }
    }

    public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!_pools.ContainsKey(prefab))
        {
            Preload(prefab, _defaultPoolSize);
        }

        GameObject obj;
        if (_pools[prefab].Count > 0)
        {
            obj = _pools[prefab].Dequeue();
        }
        else
        {
            obj = CreateNewObject(prefab);
        }

        obj.transform.SetParent(null);
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);

        var poolables = obj.GetComponentsInChildren<IPoolable>();
        foreach (var poolable in poolables)
        {
            poolable.OnSpawn();
        }

        return obj;
    }

    public void Return(GameObject obj)
    {
        if (!_prefabLookup.TryGetValue(obj, out GameObject prefab))
        {
            Destroy(obj);
            return;
        }

        var poolables = obj.GetComponentsInChildren<IPoolable>();
        foreach (var poolable in poolables)
        {
            poolable.OnDespawn();
        }

        obj.SetActive(false);
        obj.transform.SetParent(this.transform);
        _pools[prefab].Enqueue(obj);
    }

    private GameObject CreateNewObject(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab, transform);
        obj.SetActive(false);
        _prefabLookup[obj] = prefab;
        return obj;
    }
}