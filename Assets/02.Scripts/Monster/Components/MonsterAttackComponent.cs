using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MonsterStat))]
public class MonsterAttackComponent : MonoBehaviour
{
    protected IAnimationStateChanger _monsterController;

    private const float MinLookDirectionSqrMagnitude = 1e-6f;

    protected GameObject _player;
    private MonsterStat _stat;

    private float _lastAttackTime;
    private bool _isAttacking;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");

        _monsterController = GetComponent<IAnimationStateChanger>();
        _stat = GetComponent<MonsterStat>();
    }

    private void Update()
    {
        TryAttack();
    }

    private void TryAttack()
    {
        if (_isAttacking) return;

        float sqrDistanceToPlayer = (_player.transform.position - transform.position).sqrMagnitude;
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

        LookAtTarget();

        _monsterController.ChangeState(MonsterState.Attack);
        
        StartCoroutine(AttackCoroutine());
    }

    private void LookAtTarget()
    {
        Vector3 direction = _player.transform.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > MinLookDirectionSqrMagnitude)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private IEnumerator AttackCoroutine()
    {
        yield return new WaitForSeconds(_stat.AttackDuration.Value);
        _isAttacking = false;
        _monsterController.ChangeState(MonsterState.Idle);
    }
}
