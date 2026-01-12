using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어 공격의 히트박스와 데미지 처리를 담당하는 컴포넌트
/// PlayerAttack으로부터 공격 이벤트를 받아 실제 충돌 감지 및 데미지 처리 수행
/// </summary>
public class PlayerAttackRange : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private PlayerAttackData _attackData;

    [Header("Attack Point")]
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private LayerMask _monsterLayers;

    [Header("Continuous Detection")]
    [SerializeField] private bool _enableContinuousDetection = true;
    [Tooltip("프레임당 체크 간격 (0 = 매 프레임)")]
    [SerializeField] private int _checkInterval = 2;

    private HashSet<Collider> _hitEnemiesThisAttack = new HashSet<Collider>();
    private bool _isAttacking = false;
    private int _currentComboIndex = 0;
    private int _frameCounter = 0;

    private void Update()
    {
        if (_enableContinuousDetection && _isAttacking)
        {
            _frameCounter++;

            // 체크 간격마다 판정 수행
            if (_frameCounter > _checkInterval)
            {
                _frameCounter = 0;
                CheckHitDetection();
            }
        }
    }

    /// <summary>
    /// 공격 시작 - PlayerAttack에서 호출
    /// 연속 판정을 시작하고 히트 기록 초기화
    /// </summary>
    public void StartAttack(int comboIndex)
    {
        _isAttacking = true;
        _currentComboIndex = comboIndex;
        _frameCounter = 0;
        _hitEnemiesThisAttack.Clear();
    }

    /// <summary>
    /// 공격 종료 - PlayerAttack에서 호출
    /// 연속 판정을 멈추고 히트 기록 유지
    /// </summary>
    public void StopAttack()
    {
        _isAttacking = false;
    }

    /// <summary>
    /// 연속 판정 체크 (매 프레임 또는 간격마다 실행)
    /// </summary>
    private void CheckHitDetection()
    {
        if (_attackPoint == null) return;

        // 범위 내 적 탐지
        Collider[] hitColliders = Physics.OverlapSphere(_attackPoint.position, _attackData.AttackRange, _monsterLayers);

        foreach (Collider col in hitColliders)
        {
            // 중복 방지
            if (_hitEnemiesThisAttack.Contains(col))
                continue;

            // 각도 체크 (플레이어가 바라보는 방향 기준)
            Vector3 directionToTarget = (col.transform.position - _attackPoint.position).normalized;
            float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

            // 공격 각도 범위 내에 있는지 체크
            if (angleToTarget <= _attackData.AttackAngle[_currentComboIndex] / 2f)
            {
                // 데미지 계산
                float damage = CalculateDamage(_currentComboIndex);

                // 적에게 데미지 적용
                ApplyDamage(col.gameObject, damage);

                // 히트 기록 (같은 적을 다시 맞지 않도록)
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
        int safeIndex = Mathf.Clamp(comboIndex, 0, _attackData.ComboDamageMultipliers.Length - 1);
        return _attackData.AttackDamage * _attackData.ComboDamageMultipliers[safeIndex];
    }

    /// <summary>
    /// 대상에게 데미지 적용
    /// </summary>
    private void ApplyDamage(GameObject target, float damage)
    {
        Damage takeDamage = new Damage(damage, gameObject);
        if (target.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TryTakeDamage(takeDamage);
        }
    }

    // 프로퍼티
    public float AttackRange => _attackData.AttackRange;
    public float AttackAngle => _attackData.AttackAngle[_currentComboIndex];
    public float BaseDamage => _attackData.AttackDamage;
}
