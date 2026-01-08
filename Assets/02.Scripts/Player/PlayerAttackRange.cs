using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어 공격의 히트박스와 데미지 처리를 담당하는 컴포넌트
/// PlayerAttack으로부터 공격 이벤트를 받아 실제 충돌 감지 및 데미지 처리 수행
/// </summary>
public class PlayerAttackRange : MonoBehaviour
{
    [Header("Attack Range Settings")]
    [SerializeField] private float _attackRange = 3f;
    [SerializeField] private float _attackAngle = 90f;
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private LayerMask _enemyLayers;

    [Header("Damage Settings")]
    [SerializeField] private float _baseDamage = 20f;
    [SerializeField] private float[] _comboDamageMultipliers = { 1f, 1.2f, 1.5f }; // 1타, 2타, 3타 데미지 배율

    [Header("Visual Feedback")]
    [SerializeField] private bool _showGizmos = true;

    private HashSet<Collider> _hitEnemiesThisAttack = new HashSet<Collider>();

    /// <summary>
    /// 공격 실행 - PlayerAttack에서 호출
    /// </summary>
    /// <param name="comboIndex">현재 콤보 인덱스 (0, 1, 2)</param>
    public void PerformAttack(int comboIndex)
    {
        if (_attackPoint == null)
        {
            Debug.LogWarning("Attack Point is not assigned!");
            return;
        }

        // 이전 공격의 히트 기록 초기화
        _hitEnemiesThisAttack.Clear();

        // 범위 내 적 탐지
        Collider[] hitColliders = Physics.OverlapSphere(_attackPoint.position, _attackRange, _enemyLayers);

        foreach (Collider col in hitColliders)
        {
            // 이미 이번 공격에서 맞은 적은 스킵
            if (_hitEnemiesThisAttack.Contains(col))
                continue;

            // 각도 체크 (플레이어가 바라보는 방향 기준)
            Vector3 directionToTarget = (col.transform.position - _attackPoint.position).normalized;
            float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

            // 공격 각도 범위 내에 있는지 체크
            if (angleToTarget <= _attackAngle / 2f)
            {
                // 데미지 계산
                float damage = CalculateDamage(comboIndex);

                // 적에게 데미지 적용
                ApplyDamage(col.gameObject, damage, comboIndex);

                // 히트 기록
                _hitEnemiesThisAttack.Add(col);
            }
        }
    }

    /// <summary>
    /// 콤보 단계에 따른 데미지 계산
    /// </summary>
    private float CalculateDamage(int comboIndex)
    {
        // 콤보 인덱스가 배열 범위를 벗어나지 않도록 체크
        int safeIndex = Mathf.Clamp(comboIndex, 0, _comboDamageMultipliers.Length - 1);
        return _baseDamage * _comboDamageMultipliers[safeIndex];
    }

    /// <summary>
    /// 적에게 데미지 적용
    /// </summary>
    private void ApplyDamage(GameObject target, float damage, int comboIndex)
    {
        // TODO: 적의 Health 컴포넌트에 데미지 전달
        // 예: target.GetComponent<EnemyHealth>()?.TakeDamage(damage);

        Debug.Log($"Combo {comboIndex + 1} Hit: {target.name} for {damage} damage!");

        // 현재는 임시로 로그만 출력
        // 나중에 적 컴포넌트가 생기면 아래처럼 사용:
        // if (target.TryGetComponent<Enemy>(out Enemy enemy))
        // {
        //     enemy.TakeDamage(damage);
        // }
    }

    /// <summary>
    /// 특정 위치에서 공격 (스킬이나 특수 공격용)
    /// </summary>
    public void PerformAttackAtPosition(Vector3 position, int comboIndex)
    {
        _hitEnemiesThisAttack.Clear();

        Collider[] hitColliders = Physics.OverlapSphere(position, _attackRange, _enemyLayers);

        foreach (Collider col in hitColliders)
        {
            if (_hitEnemiesThisAttack.Contains(col))
                continue;

            float damage = CalculateDamage(comboIndex);
            ApplyDamage(col.gameObject, damage, comboIndex);
            _hitEnemiesThisAttack.Add(col);
        }
    }

    /// <summary>
    /// 공격 범위 시각화 (에디터 전용)
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (!_showGizmos || _attackPoint == null)
            return;

        // 공격 범위 구체
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(_attackPoint.position, _attackRange);

        // 공격 각도 범위 시각화
        Vector3 leftBoundary = Quaternion.Euler(0, -_attackAngle / 2f, 0) * transform.forward * _attackRange;
        Vector3 rightBoundary = Quaternion.Euler(0, _attackAngle / 2f, 0) * transform.forward * _attackRange;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(_attackPoint.position, _attackPoint.position + leftBoundary);
        Gizmos.DrawLine(_attackPoint.position, _attackPoint.position + rightBoundary);
    }

    // 프로퍼티
    public float AttackRange => _attackRange;
    public float AttackAngle => _attackAngle;
    public float BaseDamage => _baseDamage;
}
