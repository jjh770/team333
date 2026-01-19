using UnityEngine;

public class MonsterItemDropComponent : MonoBehaviour
{
    [SerializeField] private float _yOffset = 0.5f;

    private Transform _spawnRoot;
    private ItemFactory _itemFactory;

    private void Awake()
    {
        _itemFactory = ItemFactory.Instance;
    }

    private void Start()
    {
        var poolManager = GameObject.FindFirstObjectByType<PoolManager>();
        if (poolManager != null)
        {
            _spawnRoot = poolManager.transform;
        }
    }

    public void DropItem(GameObject item)
    {
        if (item == null) return;
        if (_itemFactory == null) return;

        Vector3 offset = new Vector3(0f, _yOffset, 0f);
        Vector3 spawnPosition = transform.position + offset;

        Quaternion randomRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

        GameObject spawned = _itemFactory.Spawn(item, spawnPosition, randomRotation);
        
        if (spawned == null) return;

        if (_spawnRoot != null)
        {
            spawned.transform.SetParent(_spawnRoot, true);
        }
    }
}
