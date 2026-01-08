using System;
using UnityEngine;

public class MonsterPool : MonoBehaviour
{
    public static MonsterPool Instance { get; private set; }

    [Header("Monsters")]
    [SerializeField] private GameObject[] _monsterPrefabs;
    [SerializeField] private int _preloadPerPrefab = 30;

    [Header("Spawn Settings")]
    [SerializeField] private int _minSpawnCount = 5;
    [SerializeField] private int _maxSpawnCount = 10;

    [Header("Spawn Groups")]
    [SerializeField] private SpawnGroup[] _spawnGroups;

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
        foreach (var prefab in _monsterPrefabs)
        {
            PoolManager.Instance.Preload(prefab, _preloadPerPrefab);
        }
    }

    public void SpawnGroup(int groupIndex)
    {
        if (groupIndex < 1 || groupIndex > _spawnGroups.Length) return;

        SpawnRandomInGroup(_spawnGroups[groupIndex - 1]);
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

            GameObject spawned = PoolManager.Instance.Get(prefab, point.position, point.rotation);

            if (spawned != null && spawned.TryGetComponent<Monster>(out var monster))
            {
                monster.OnDie += HandleMonsterDie;
            }
        }
    }

    private void HandleMonsterDie(Monster monster)
    {
        monster.OnDie -= HandleMonsterDie;
        PoolManager.Instance.Return(monster.gameObject);
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