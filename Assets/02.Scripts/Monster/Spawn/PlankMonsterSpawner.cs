using System;
using System.Collections;
using UnityEngine;

public class PlankMonsterSpawner : BaseMonsterSpawner, IMonsterSpawner
{
    [Header("Monsters")]
    [SerializeField] private GameObject[] _monsterPrefabs;
    [SerializeField] private int _preloadPerPrefab = 15;

    [Header("Spawn Settings")]
    [SerializeField] private int _minSpawnCount = 2;
    [SerializeField] private int _maxSpawnCount = 4;
    [SerializeField] private float _minSpawnInterval = 3f;
    [SerializeField] private float _maxSpawnInterval = 6f;

    [Header("Spawn Groups")]
    [SerializeField] private SpawnGroup[] _spawnGroups;

    private Coroutine _spawnCoroutine;
    private bool _isSpawning;

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

        StartSpawning(_spawnGroups[groupIndex - 1]);
    }

    public void StartSpawning(SpawnGroup group)
    {
        if (_isSpawning) return;

        _isSpawning = true;
        _spawnCoroutine = StartCoroutine(SpawnRoutine(group));
    }

    public void StopSpawning()
    {
        if (!_isSpawning) return;

        _isSpawning = false;
        if (_spawnCoroutine != null)
        {
            StopCoroutine(_spawnCoroutine);
            _spawnCoroutine = null;
        }
    }

    private IEnumerator SpawnRoutine(SpawnGroup group)
    {
        while (_isSpawning)
        {
            SpawnRandomInGroup(group);

            float interval = UnityEngine.Random.Range(_minSpawnInterval, _maxSpawnInterval);
            yield return new WaitForSeconds(interval);
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

            SpawnMonster(prefab, point.position, point.rotation);
        }
    }

    private GameObject GetRandomPrefab()
    {
        if (_monsterPrefabs.Length == 0) return null;
        return _monsterPrefabs[UnityEngine.Random.Range(0, _monsterPrefabs.Length)];
    }

    private void OnDisable()
    {
        StopSpawning();
    }
}
