using System.Collections.Generic;
using UnityEngine;

public class PlacedMonsterSpawner : BaseMonsterSpawner
{
    [Header("Bad Monster")]
    [SerializeField] private GameObject _badMonster;
    [SerializeField] private Transform[] _badMonsterPoints;
    [SerializeField] private int _badMonsterCount;

    [Header("Flora Skill Monster")]
    [SerializeField] private GameObject[] _floraSkillMonster;
    [SerializeField] private Transform[] _floraSkillMonsterPoints;

    private void Start()
    {
        SpawnPlacedMonster();
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
            GameObject instance = SpawnMonster(_badMonster, point.position, point.rotation);
            if (instance != null)
            {
                instance.transform.SetParent(this.transform);
            }
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
            GameObject instance = SpawnMonster(monster, point.position, point.rotation);
            if (instance != null)
            {
                instance.transform.SetParent(this.transform);
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
