using System.Collections.Generic;
using UnityEngine;

public class TurnipaMonsterController : BadMonsterController
{
    [Header("Split Settings")]
    [SerializeField] private GameObject _splitMonsterPrefab;
    [SerializeField] private int _splitCount = 3;
    [SerializeField] private float _splitOffset = 1f;

    private IPoolManager _poolManager;
    private readonly List<BadMonsterController> _splitMonsters = new();

    protected override void Awake()
    {
        base.Awake();
        _poolManager = PoolManager.Instance;
    }

    public override void OnDespawn()
    {
        CleanupSplitMonsters();
        base.OnDespawn();
    }

    protected override void Die()
    {
        if (_isDead) return;

        Split();
        base.Die();
    }

    private void Split()
    {
        if (_splitMonsterPrefab == null) return;
        if (_poolManager == null) return;

        for (int i = 0; i < _splitCount; i++)
        {
            Vector3 spawnPosition = GetSplitPosition(i);
            GameObject splitMonster = _poolManager.Get(_splitMonsterPrefab, spawnPosition, Quaternion.identity);

            if (splitMonster.TryGetComponent<BadMonsterController>(out var controller))
            {
                controller.OnDie += HandleSplitMonsterDie;
                _splitMonsters.Add(controller);
            }
        }
    }

    private void HandleSplitMonsterDie(BadMonsterController controller)
    {
        controller.OnDie -= HandleSplitMonsterDie;
        _splitMonsters.Remove(controller);
        _poolManager?.Return(controller.gameObject);
    }

    private void CleanupSplitMonsters()
    {
        foreach (var monster in _splitMonsters)
        {
            if (monster != null)
            {
                monster.OnDie -= HandleSplitMonsterDie;
            }
        }
        _splitMonsters.Clear();
    }

    private Vector3 GetSplitPosition(int index)
    {
        float angle = (360f / _splitCount) * index * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * _splitOffset;
        return transform.position + offset;
    }
}
