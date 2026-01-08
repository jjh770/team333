using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MonsterStat))]
[RequireComponent(typeof(MonsterMoveComponent))]
public class MonsterAttackComponent : MonoBehaviour
{
    private IAnimationStateChanger _monsterController;
    private MonsterMoveComponent _moveComponent;
    private MonsterStat _stat;

    private Transform _player;
    private float _lastAttackTime;
    private bool _isAttacking;

    private void Awake()
    {
        _monsterController = GetComponent<IAnimationStateChanger>();
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
        float attackDistance = _stat.AttackDistance.Value;

        if (sqrDistanceToPlayer <= attackDistance * attackDistance)
        {
            float attackCooldown = _stat.AttackCooltime.Value;
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
        _monsterController.ChangeState(MonsterState.Attack);

        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        yield return new WaitForSeconds(_stat.AttackDuration.Value);
        _isAttacking = false;
        _monsterController.ChangeState(MonsterState.Idle);
    }
}
