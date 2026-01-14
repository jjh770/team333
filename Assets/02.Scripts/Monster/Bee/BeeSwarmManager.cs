using System.Collections.Generic;
using UnityEngine;

public class BeeSwarmManager : MonoBehaviour
{
    [Header("Prefabs")]
    [Tooltip("일반 벌")]
    [SerializeField] private GameObject _workerBeePrefab;
    [Tooltip("여왕 벌")]
    [SerializeField] private GameObject _queenBeePrefab;

    [Header("Spawn Settings")]
    [SerializeField] private int _minCount = 10;
    [SerializeField] private int maxCount = 15;
    [SerializeField] private float spawnRadius = 3f;

    // 활성화된 벌 목록
    public List<BeeMonsterController> ActiveBees { get; private set; } = new List<BeeMonsterController>();

    void Start()
    {
        SpawnSwarm();
    }

    private void SpawnSwarm()
    {
        int totalCount = Random.Range(_minCount, maxCount + 1);

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

            Vector3 spawnPosition = transform.position + (Random.insideUnitSphere * spawnRadius);
            spawnPosition.y = transform.position.y;

            GameObject obj = Instantiate(prefabToUse, spawnPosition, Quaternion.identity);

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
        monster.gameObject.SetActive(false);
    }

    public void RemoveBee(BeeMonsterController bee)
    {
        if (ActiveBees.Contains(bee))
        {
            ActiveBees.Remove(bee);
        }
    }
}