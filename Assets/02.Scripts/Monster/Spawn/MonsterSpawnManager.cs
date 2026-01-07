using System;
using UnityEngine;

public class MonsterSpawnManager : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject _monsterPrefab;

    [Header("Spawn Groups (1~9 키로 소환)")]
    [SerializeField] private SpawnGroup[] _spawnGroups;

    private void Update()
    {
        for (int i = 0; i < _spawnGroups.Length && i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SpawnNextInGroup(_spawnGroups[i]);
            }
        }
    }

    private void SpawnNextInGroup(SpawnGroup group)
    {
        Transform spawnPoint = group.GetNextSpawnPoint();
        if (spawnPoint == null || _monsterPrefab == null) return;

        Instantiate(_monsterPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}

[Serializable]
public class SpawnGroup
{
    [SerializeField] private string _groupName;
    [SerializeField] private Transform[] _spawnPoints;

    private int _currentIndex;

    public Transform GetNextSpawnPoint()
    {
        if (_spawnPoints == null || _spawnPoints.Length == 0) return null;

        if (_currentIndex >= _spawnPoints.Length)
        {
            _currentIndex = 0;
        }

        return _spawnPoints[_currentIndex++];
    }

    public void Reset() => _currentIndex = 0;
}
