using UnityEngine;

public interface IPoolManager
{
    void Preload(GameObject prefab, int count);
    GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation);
    void Return(GameObject obj);
}
