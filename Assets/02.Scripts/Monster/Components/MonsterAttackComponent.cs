using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MonsterStat))]
[RequireComponent(typeof(MonsterMoveComponent))]
public class MonsterAttackComponent : MonoBehaviour
{
    [SerializeField] private float _attackDistance = 2.5f;
    [SerializeField] private float _attackDuration = 0.14f;

    private MonsterMoveComponent _moveComponent;
    private MonsterStat _stat;

    private Transform _player;
    private float _lastAttackTime;
    private bool _isAttacking;
    public bool IsAttacking => _isAttacking;

    private void Awake()
    {
        _moveComponent = GetComponent<MonsterMoveComponent>();
        _stat = GetComponent<MonsterStat>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            _player = playerObject.transform;
        }
    }

    private void Update()
    {
        TryAttack();
    }

    private void TryAttack()
    {
        if (_isAttacking || _player == null) return;

        float sqrDistanceToPlayer = (_player.position - transform.position).sqrMagnitude;

        if (sqrDistanceToPlayer <= _attackDistance * _attackDistance)
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

        _moveComponent.LookAtTargetImmediate();

        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        yield return new WaitForSeconds(_attackDuration);
        _isAttacking = false;
    }
}
