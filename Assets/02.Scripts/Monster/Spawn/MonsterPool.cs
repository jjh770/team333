using UnityEngine;

public class MonsterPool : MonoBehaviour
{
    [SerializeField] private GameObject _monsterPrefab;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private int _preloadCount = 5;

    private void Start()
    {
        if (_monsterPrefab != null)
        {
            PoolManager.Instance.Preload(_monsterPrefab, _preloadCount);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Spawn();
        }
    }

    public GameObject Spawn()
    {
        Vector3 position = _spawnPoint != null ? _spawnPoint.position : transform.position;
        Quaternion rotation = _spawnPoint != null ? _spawnPoint.rotation : Quaternion.identity;

        return PoolManager.Instance.Get(_monsterPrefab, position, rotation);
    }
}
