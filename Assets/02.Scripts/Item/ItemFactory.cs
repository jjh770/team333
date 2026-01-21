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
        if (_poolManager == null) return;

        foreach (var item in _itemPrefabs)
        {
            if (item != null)
                _poolManager.Preload(item, _preloadCount);
        }
    }

    public GameObject Spawn(GameObject itemPrefab, Vector3 position, Quaternion rotation)
    {
        if (itemPrefab == null) return null;
        if (_poolManager == null) return null;

        GameObject item = _poolManager.Get(itemPrefab, position, rotation);

        if (item != null)
        {
            _activeItems.Add(item);
        }

        return item;
    }

    public void ReturnItem(GameObject effectObj)
    {
        if (_poolManager != null && effectObj != null)
        {
            _activeItems.Remove(effectObj);
            _poolManager.Return(effectObj);
        }
    }

    public void ReturnAllActiveItems()
    {
        if (_poolManager == null) return;

        foreach (var item in _activeItems)
        {
            if (item != null && item.activeInHierarchy)
            {
                _poolManager.Return(item);
            }
        }
        _activeItems.Clear();
    }
}