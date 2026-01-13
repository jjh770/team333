using System.Collections;
using UnityEngine;

public class ItemFactory : MonoBehaviour
{
    public static ItemFactory Instance { get; private set; }

    [SerializeField] private PoolManager _poolManager;

    [Header("items")]
    [SerializeField] private GameObject _woodItemPrefab;
    [SerializeField] private GameObject _boardItemPrefab;
    [SerializeField] private GameObject _healthUpPrefab;
    [SerializeField] protected float __itemPrefabsDuration = 20;

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

        if (_woodItemPrefab != null)
            PoolManager.Instance.Preload(_woodItemPrefab, _preloadCount);

        if (_boardItemPrefab != null)
            PoolManager.Instance.Preload(_boardItemPrefab, _preloadCount);

        if (_healthUpPrefab != null)
            PoolManager.Instance.Preload(_healthUpPrefab, _preloadCount);
    }

    public GameObject Spawn(GameObject itemPrefab, Vector3 position, Quaternion rotation)
    {
        if (itemPrefab == null) return null;
        if (PoolManager.Instance == null) return null;

        GameObject item = PoolManager.Instance.Get(itemPrefab, position, rotation);

        if (item != null && __itemPrefabsDuration > 0)
        {
            StartCoroutine(ReturnAfterDelay(item, __itemPrefabsDuration));
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
