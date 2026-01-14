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
        _spawnRoot = GameObject.FindFirstObjectByType<PoolManager>().transform;
    }

    public void DropItem(GameObject item)
    {
        if (item == null) return;
        if (ItemFactory.Instance == null) return;

        Vector3 offset = new Vector3(0f, _yOffset, 0f);
        Vector3 spawnPosition = transform.position + offset;

        GameObject spawned = _itemFactory.Spawn(item, spawnPosition, Quaternion.identity);
        if (spawned == null) return;

        if (_spawnRoot != null)
        {
            spawned.transform.SetParent(_spawnRoot, true);
        }
    }
}
