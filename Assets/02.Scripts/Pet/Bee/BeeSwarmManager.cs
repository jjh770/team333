using System.Collections.Generic;
using UnityEngine;

public class BeeSwarmManager : MonoBehaviour, IPoolable
{
    [Header("Prefabs")]
    [Tooltip("일반 벌")]
    [SerializeField] private GameObject _workerBeePrefab;
    [Tooltip("여왕 벌")]
    [SerializeField] private GameObject _queenBeePrefab;

    [Header("Spawn Settings")]
    [SerializeField] private int _minCount = 10;
    [SerializeField] private int _maxCount = 15;
    [SerializeField] private float _spawnRadius = 3f;

    // 활성화된 벌 목록
    public List<BeeMonsterController> ActiveBees { get; private set; } = new List<BeeMonsterController>();

    public void OnSpawn()
    {
        SpawnSwarm();
    }

    public void OnDespawn()
    {
        ActiveBees.Clear();
    }

    private void SpawnSwarm()
    {
        int totalCount = Random.Range(_minCount, _maxCount + 1);

        for (int i = 0; i < totalCount; i++)
        {
            // 첫 번째는 무조건 여왕벌, 나머지는 일벌
            GameObject prefabToUse;
            if (i == 0)
            {
                prefabToUse = _queenBeePrefab;
            }
            else
            {
                prefabToUse = _workerBeePrefab;
            }

            Vector3 spawnPosition = transform.position + (Random.insideUnitSphere * _spawnRadius);
            spawnPosition.y = transform.position.y;

            GameObject obj = PoolManager.Instance.Get(prefabToUse, spawnPosition, Quaternion.identity);

            BeeMonsterController bee = obj.GetComponent<BeeMonsterController>();
            if (bee != null)
            {
                bee.OnSpawn();
                bee.InitSwarm(this);
                bee.OnDie += HandleBeeDie;
                ActiveBees.Add(bee);
            }
        }
    }

    private void HandleBeeDie(BadMonsterController monster)
    {
        monster.OnDie -= HandleBeeDie;
        PoolManager.Instance.Return(monster.gameObject);

        // 리스트에서 제거
        BeeMonsterController bee = monster as BeeMonsterController;
        if (bee != null && ActiveBees.Contains(bee))
        {
            ActiveBees.Remove(bee);
        }

        // 모든 벌이 죽으면 자신도 반환
        if (ActiveBees.Count == 0)
        {
            PoolManager.Instance.Return(gameObject);
        }
    }
}