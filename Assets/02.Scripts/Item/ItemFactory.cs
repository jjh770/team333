using System.Collections;
using UnityEngine;

public class ItemFactory : MonoBehaviour
{
    public static ItemFactory Instance { get; private set; }

    [SerializeField] private PoolManager _poolManager;

    [Header("items")]
    [SerializeField] private GameObject[] _itemPrefabs;
    [SerializeField] protected float _itemPrefabsDuration = 20;

    [Header("Settings")]
    [SerializeField] private int _preloadCount = 20;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        PreloadAllItems();
    }

    private void PreloadAllItems()
    {
        if (PoolManager.Instance == null) return;

        foreach (var item in _itemPrefabs)
        {
            if (item != null)
                PoolManager.Instance.Preload(item, _preloadCount);
        }
    }

    public GameObject Spawn(GameObject itemPrefab, Vector3 position, Quaternion rotation)
    {
        if (itemPrefab == null) return null;
        if (PoolManager.Instance == null) return null;

        GameObject item = PoolManager.Instance.Get(itemPrefab, position, rotation);

        if (item != null && _itemPrefabsDuration > 0)
        {
            StartCoroutine(ReturnAfterDelay(item, _itemPrefabsDuration));
        }

        return item;
    }

    private IEnumerator ReturnAfterDelay(GameObject effectObj, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnItem(effectObj);
    }

    public void ReturnItem(GameObject effectObj)
    {
        if (PoolManager.Instance != null && effectObj != null)
        {
            PoolManager.Instance.Return(effectObj);
        }
    }
}
