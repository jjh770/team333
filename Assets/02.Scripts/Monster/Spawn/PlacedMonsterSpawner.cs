using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlacedMonsterSpawner : MonoBehaviour
{
    [Header("Bad Monster")]
    [SerializeField] private GameObject _badMonster;
    [SerializeField] private Transform[] _badMonsterPoints;
    [SerializeField] private int _badMonsterCount;

    [Header("Player Skill Monster")]
    [SerializeField] private GameObject _playerSkillMonster;
    [SerializeField] private Transform[] _playerSkillMonsterPoints;
    [SerializeField] private int _playerSkillMonsterCount = 1;

    [Header("Flora Skill Monster")]
    [SerializeField] private GameObject[] _floraSkillMonster;
    [SerializeField] private Transform[] _floraSkillMonsterPoints;

    private void Start()
    {
        SpawnPlacedMonster();
        SpawnPlayerSkillMonster();
        SpawnFloraSkillMonster();
    }

    [ContextMenu("Spawn Placed Bad Monsters")]
    private void SpawnPlacedMonster()
    {
        if (_badMonster == null) return;
        if (_badMonsterPoints == null || _badMonsterPoints.Length == 0) return;

        int spawnCount = Mathf.Clamp(_badMonsterCount, 0, _badMonsterPoints.Length);
        var spawnPoints = PickUniquePoints(_badMonsterPoints, spawnCount);

        foreach (var point in spawnPoints)
        {
            SpawnMonster(_badMonster, point.position, point.rotation);
        }
    }

    [ContextMenu("Spawn Placed Bad Monsters")]
    private void SpawnPlayerSkillMonster()
    {
        if (_playerSkillMonster == null) return;
        if (_playerSkillMonsterPoints == null || _playerSkillMonsterPoints.Length == 0) return;

        var spawnPoints = PickUniquePoints(_playerSkillMonsterPoints, _playerSkillMonsterCount);

        foreach (var point in spawnPoints)
        {
            SpawnMonster(_playerSkillMonster, point.position, point.rotation);
        }
    }

    [ContextMenu("Spawn Placed Skill Monsters")]
    private void SpawnFloraSkillMonster()
    {
        if (_floraSkillMonster == null || _floraSkillMonster.Length == 0) return;
        if (_floraSkillMonsterPoints == null || _floraSkillMonsterPoints.Length == 0) return;

        int spawnCount = _floraSkillMonster.Length;
        var spawnPoints = PickUniquePoints(_floraSkillMonsterPoints, spawnCount);

        for (int i = 0; i < spawnCount && i < spawnPoints.Count; i++)
        {
            var monster = _floraSkillMonster[i];
            if (monster == null) continue;

            var point = spawnPoints[i];
            SpawnMonster(monster, point.position, point.rotation);
        }
    }

    private void SpawnMonster(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        Vector3 navMeshPosition = GetNavMeshPosition(position);
        GameObject instance = PoolManager.Instance.Get(prefab, navMeshPosition, rotation);

        if (instance.TryGetComponent(out BadMonsterController badController))
        {
            badController.OnDie += HandleMonsterDie;
        }
    }

    private void HandleMonsterDie(BadMonsterController controller)
    {
        controller.OnDie -= HandleMonsterDie;
        PoolManager.Instance.Return(controller.gameObject);
    }

    private Vector3 GetNavMeshPosition(Vector3 position)
    {
        if (NavMesh.SamplePosition(position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return position;
    }

    private List<Transform> PickUniquePoints(Transform[] points, int count)
    {
        var list = new List<Transform>(points);
        Shuffle(list);

        if (count < list.Count) list.RemoveRange(count, list.Count - count);
        return list;
    }

    private void Shuffle<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
