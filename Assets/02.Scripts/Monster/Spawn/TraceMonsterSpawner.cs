using System;
using UnityEngine;
using UnityEngine.AI;

public class TraceMonsterSpawner : BaseMonsterSpawner, IMonsterSpawner
{
    [Header("Monsters")]
    [SerializeField] private GameObject[] _monsterPrefabs;
    [SerializeField] private int _preloadPerPrefab = 30;

    [Header("Spawn Settings")]
    [SerializeField] private int _defaultMinSpawnCount = 2;
    [SerializeField] private int _defaultMaxSpawnCount = 5;

    [Header("Spawn Groups")]
    [SerializeField] private SpawnGroup[] _spawnGroups;

    private void Start()
    {
        foreach (var prefab in _monsterPrefabs)
        {
            _poolManager.Preload(prefab, _preloadPerPrefab);
        }
    }

    public void SpawnGroup(int groupIndex)
    {
        if (groupIndex < 1 || groupIndex > _spawnGroups.Length) return;

        SpawnRandomInGroup(_spawnGroups[groupIndex - 1]);
    }

    private void SpawnRandomInGroup(SpawnGroup group)
    {
        int spawnCount = UnityEngine.Random.Range(_defaultMinSpawnCount, _defaultMaxSpawnCount + 1);
        spawnCount += group.AddSpawnCount;
        for (int i = 0; i < spawnCount; i++)
        {
            Transform point = group.GetRandomSpawnPoint();
            if (point == null) continue;

            GameObject prefab = GetRandomPrefab();
            if (prefab == null) continue;

            SpawnMonster(prefab, point.position, point.rotation);
        }
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
    [SerializeField] private int _addSpawnCount = 0;
    public int AddSpawnCount => _addSpawnCount;
    
    public Transform GetRandomSpawnPoint()
    {
        if (_spawnPoints == null || _spawnPoints.Length == 0) return null;
        return _spawnPoints[UnityEngine.Random.Range(0, _spawnPoints.Length)];
    }
}
