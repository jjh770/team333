using System.Collections.Generic;
using UnityEngine;

public class PlacedMonsterSpawner : BaseMonsterSpawner
{
    [Header("Bad Monster")]
    [SerializeField] private GameObject _badMonster;
    [SerializeField] private Transform[] _badMonsterPoints;
    [SerializeField] private int _badMonsterCount;

    [Header("Bully")]
    [SerializeField] private GameObject[] _bully;
    [SerializeField] private Transform[] _bullyPoints;
    [SerializeField] private int _bullyCount;

    private void Start()
    {
        SpawnPlacedMonster();
        SpawnBully();
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

    [ContextMenu("Spawn Bully Monsters")]
    private void SpawnBully()
    {
        if (_bully == null || _bully.Length == 0) return;
        if (_bullyPoints == null || _bullyPoints.Length == 0) return;

        foreach (var bullyPrefab in _bully)
        {
            if (bullyPrefab == null) continue;

            int spawnCount = Mathf.Clamp(_bullyCount, 0, _bullyPoints.Length);
            var spawnPoints = PickUniquePoints(_bullyPoints, spawnCount);

            foreach (var point in spawnPoints)
            {
                SpawnMonster(bullyPrefab, point.position, point.rotation);
            }
        }
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
