using UnityEngine;

public class TurnipaMonsterController : BadMonsterController
{
    [Header("Split Settings")]
    [SerializeField] private GameObject _splitMonsterPrefab;
    [SerializeField] private int _splitCount = 3;
    [SerializeField] private float _splitOffset = 1f;

    protected override void Die()
    {
        if (_isDead) return;

        Split();
        base.Die();
    }

    private void Split()
    {
        if (_splitMonsterPrefab == null) return;
        if (PoolManager.Instance == null) return;

        for (int i = 0; i < _splitCount; i++)
        {
            Vector3 spawnPosition = GetSplitPosition(i);
            GameObject splitMonster = PoolManager.Instance.Get(_splitMonsterPrefab, spawnPosition, Quaternion.identity);

            if (splitMonster.TryGetComponent<BadMonsterController>(out var controller))
            {
                controller.OnDie += HandleSplitMonsterDie;
            }
        }
    }

    // 미니 몬스터들을 위한 풀 반환
    private void HandleSplitMonsterDie(BadMonsterController controller)
    {
        controller.OnDie -= HandleSplitMonsterDie;
        PoolManager.Instance?.Return(controller.gameObject);
    }

    private Vector3 GetSplitPosition(int index)
    {
        float angle = (360f / _splitCount) * index * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * _splitOffset;
        return transform.position + offset;
    }
}
