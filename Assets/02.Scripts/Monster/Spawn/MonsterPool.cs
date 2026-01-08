using System;
using UnityEngine;

public class MonsterPool : MonoBehaviour
{
    [Header("Monsters")]
    [SerializeField] private GameObject[] _monsterPrefabs;
    [SerializeField] private int _preloadPerPrefab = 30;

    [Header("Spawn Settings")]
    [SerializeField] private int _minSpawnCount = 5;
    [SerializeField] private int _maxSpawnCount = 10;

    [Header("Spawn Groups")]
    [SerializeField] private SpawnGroup[] _spawnGroups;

    [Header("Dependencies")]
    [SerializeField] private PoolManager _poolManagerComponent;

    private IPoolManager _poolManager;

    private void Awake()
    {
        _poolManager = _poolManagerComponent;
    }

    private void Start()
    {
        foreach (var prefab in _monsterPrefabs)
        {
            _poolManager.Preload(prefab, _preloadPerPrefab);
        }
    }

    private void Update()
    {
        // 테스트용 입력
        for (int i = 0; i < _spawnGroups.Length && i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SpawnRandomInGroup(_spawnGroups[i]);
            }
        }
    }

    private void SpawnRandomInGroup(SpawnGroup group)
    {
        int spawnCount = UnityEngine.Random.Range(_minSpawnCount, _maxSpawnCount + 1);

        for (int i = 0; i < spawnCount; i++)
        {
            Transform point = group.GetRandomSpawnPoint();
            if (point == null) continue;

            GameObject prefab = GetRandomPrefab();
            if (prefab == null) continue;

            GameObject spawned = _poolManager.Get(prefab, point.position, point.rotation);
            
            // 생성될 때 죽음 이벤트 구독
            if (spawned != null && spawned.TryGetComponent<Monster>(out var monster))
            {
                monster.OnDie += HandleMonsterDie;
            }
        }
    }

    // 죽을 때 죽음 이벤트 해제
    private void HandleMonsterDie(Monster monster)
    {
        monster.OnDie -= HandleMonsterDie;
        _poolManager.Return(monster.gameObject);
    }

    private GameObject GetRandomPrefab()
    {
        if (_monsterPrefabs.Length == 0) return null;
        return _monsterPrefabs[UnityEngine.Random.Range(0, _monsterPrefabs.Length)];
    }
}

[Serializable]
public class SpawnGroup
{
    [SerializeField] private string _groupName;
    [SerializeField] private Transform[] _spawnPoints;

    public Transform GetRandomSpawnPoint()
    {
        if (_spawnPoints == null || _spawnPoints.Length == 0) return null;
        return _spawnPoints[UnityEngine.Random.Range(0, _spawnPoints.Length)];
    }
}
