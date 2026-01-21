using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TutorialStep_TreeMonster : TutorialStepBase
{
    [Header("Monster Settings")]
    [SerializeField] private GameObject _monsterPrefab;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private float _respawnDelay = 3f;

    [Header("Clear Condition")]
    [SerializeField] private int _requiredCount = 2;

    [Header("References")]
    [SerializeField] private PoolManager _poolManager;
    [SerializeField] private FloraInventory _floraInventory;

    private readonly List<GameObject> _spawnedMonsters = new List<GameObject>();
    private int _currentCount;
    private Coroutine _respawnCoroutine;

    public int CurrentCount => _currentCount;
    public int RequiredCount => _requiredCount;
    
    private const float NAVMESH_SAMPLE_RADIUS = 5f;
    public event Action OnCountChanged;

    protected override void OnEnter()
    {
        _currentCount = 0;
        _spawnedMonsters.Clear();

        if (_floraInventory != null)
        {
            _floraInventory.OnWoodChanged += HandleWoodChanged;
        }

        SpawnMonsters();
    }

    protected override void OnExit()
    {
        if (_respawnCoroutine != null)
        {
            StopCoroutine(_respawnCoroutine);
            _respawnCoroutine = null;
        }

        if (_floraInventory != null)
        {
            _floraInventory.OnWoodChanged -= HandleWoodChanged;
        }

        DespawnAllMonsters();
    }

    protected override void CheckCompletion() { }

    private void HandleWoodChanged(float woodCount)
    {
        _currentCount = (int)woodCount;
        OnCountChanged?.Invoke();

        if (_currentCount >= _requiredCount)
        {
            Complete();
        }
    }

    private void HandleMonsterDie(BadMonsterController controller)
    {
        controller.OnDie -= HandleMonsterDie;
        GameObject monsterObj = controller.gameObject;
        _spawnedMonsters.Remove(monsterObj);
        _poolManager.Return(monsterObj);

        if (_currentCount < _requiredCount && _spawnedMonsters.Count == 0)
        {
            _respawnCoroutine = StartCoroutine(SpawnMonstersAfterDelay(_respawnDelay));
        }
    }

    private IEnumerator SpawnMonstersAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnMonsters();
    }

    private void SpawnMonsters()
    {
        if (_monsterPrefab == null || _poolManager == null) return;
        if (_spawnPoints == null || _spawnPoints.Length == 0) return;

        foreach (var point in _spawnPoints)
        {
            if (point == null) continue;

            Vector3 spawnPos = GetNavMeshPosition(point.position);
            GameObject monster = _poolManager.Get(_monsterPrefab, spawnPos, point.rotation);

            var controller = monster.GetComponentInChildren<BadMonsterController>();
            if (controller != null)
            {
                controller.OnDie += HandleMonsterDie;
            }

            _spawnedMonsters.Add(monster);
        }
    }

    private void DespawnAllMonsters()
    {
        foreach (var monster in _spawnedMonsters)
        {
            if (monster == null) continue;

            var controller = monster.GetComponentInChildren<BadMonsterController>();
            if (controller != null)
            {
                controller.OnDie -= HandleMonsterDie;
            }

            if (monster.activeInHierarchy)
            {
                _poolManager.Return(monster);
            }
        }

        _spawnedMonsters.Clear();
    }

    private Vector3 GetNavMeshPosition(Vector3 position)
    {
        if (NavMesh.SamplePosition(position, out NavMeshHit hit, NAVMESH_SAMPLE_RADIUS, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return position;
    }
}
