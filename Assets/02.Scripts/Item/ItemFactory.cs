using System.Collections.Generic;
using UnityEngine;

public class ItemFactory : MonoBehaviour
{
    public static ItemFactory Instance { get; private set; }

    [SerializeField] private PoolManager _poolManager;

    [Header("items")]
    [SerializeField] private GameObject[] _itemPrefabs;

    [Header("Settings")]
    [SerializeField] private int _preloadCount = 20;

    private readonly List<GameObject> _activeItems = new();

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

        if (item != null)
        {
            _activeItems.Add(item);
        }

        return item;
    }

    public void ReturnItem(GameObject effectObj)
    {
        if (PoolManager.Instance != null && effectObj != null)
        {
            _activeItems.Remove(effectObj);
            PoolManager.Instance.Return(effectObj);
        }
    }

    public void ReturnAllActiveItems()
    {
        if (PoolManager.Instance == null) return;

        for (int i = _activeItems.Count - 1; i >= 0; i--)
        {
            var item = _activeItems[i];
            if (item != null && item.activeInHierarchy)
            {
                PoolManager.Instance.Return(item);
            }
        }
        _activeItems.Clear();
    }
}