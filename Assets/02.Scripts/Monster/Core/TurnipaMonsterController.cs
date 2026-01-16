using System.Collections.Generic;
using UnityEngine;

public class TurnipaMonsterController : BadMonsterController
{
    [Header("Split Settings")]
    [SerializeField] private GameObject _splitMonsterPrefab;
    [SerializeField] private int _splitCount = 3;
    [SerializeField] private float _splitOffset = 1f;

    private IPoolManager _poolManager;

    protected override void Awake()
    {
        base.Awake();
        _poolManager = PoolManager.Instance;
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
            
            // 자식 스스로 풀로 돌아가는 로직을 람다로 주입
            if (splitMonster.TryGetComponent<BadMonsterController>(out var controller))
            {
                System.Action<BadMonsterController> returnToPoolAction = null;

                returnToPoolAction = (deadMonster) =>
                {
                    deadMonster.OnDie -= returnToPoolAction;
                    PoolManager.Instance.Return(deadMonster.gameObject);
                };

                controller.OnDie += returnToPoolAction;
            }
        }
    }

    private Vector3 GetSplitPosition(int index)
    {
        float angle = (360f / _splitCount) * index * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * _splitOffset;
        return transform.position + offset;
    }
}