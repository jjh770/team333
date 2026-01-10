using UnityEngine;

public class ItemFactory : MonoBehaviour
{
    public static ItemFactory Instance { get; private set; }

    [SerializeField] private PoolManager _poolManager;

    [Header("items")]
    [SerializeField] private GameObject[] _itemPrefabs;
    [SerializeField] private int _preloadPerPrefab = 30;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        foreach (var prefab in _itemPrefabs)
        {
            _poolManager.Preload(prefab, _preloadPerPrefab);
        }
    }

    public GameObject Spawn(GameObject itemPrefab, Vector3 position, Quaternion rotation)
    {
        if (itemPrefab == null) return null;
        if (_poolManager == null) return null;

        return _poolManager.Get(itemPrefab, position, rotation);
    }

    public void Despawn(GameObject itemInstance)
    {
        if (itemInstance == null) return;
        if (_poolManager == null) return;

        _poolManager.Return(itemInstance);
    }

    public void Preload(GameObject itemPrefab, int count)
    {
        if (itemPrefab == null) return;
        if (_poolManager == null) return;

        _poolManager.Preload(itemPrefab, count);
    }
}
