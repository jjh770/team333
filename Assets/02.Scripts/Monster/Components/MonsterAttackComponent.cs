using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MonsterStat))]
public class MonsterAttackComponent : MonoBehaviour
{
    [SerializeField] private float _attackDistance = 2.5f;
    [SerializeField] private float _attackDuration = 0.14f;

    private MonsterStat _stat;
    private Transform _target;
    private float _lastAttackTime;

    private bool _isAttacking;
    public bool IsAttacking => _isAttacking;

    public event Action OnAttackStart;

    private void Awake()
    {
        _stat = GetComponent<MonsterStat>();
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    public void TryAttack()
    {
        if (_isAttacking || _target == null) return;

        float sqrDistance = (_target.position - transform.position).sqrMagnitude;

        if (sqrDistance <= _attackDistance * _attackDistance)
        {
            float attackCooldown = _stat.GetAttackCooltime();
            if (Time.time >= _lastAttackTime + attackCooldown)
            {
                Attack();
            }
        }
    }

    private void Attack()
    {
        _isAttacking = true;
        _lastAttackTime = Time.time;

        OnAttackStart?.Invoke();

        // Todo: 플레이어에게 데미지 입히기

        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        yield return new WaitForSeconds(_attackDuration);
        _isAttacking = false;
    }
}
