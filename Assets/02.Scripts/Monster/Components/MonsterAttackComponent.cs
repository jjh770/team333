using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MonsterStat))]
public class MonsterAttackComponent : MonoBehaviour
{
    [SerializeField] private float _attackDistance = 2.5f;
    [SerializeField] private float _attackDuration = 0.14f;

    private MonsterStat _stat;
    private Transform _target;
    private IDamageable _targetDamageable;
    private float _attackCooltime;
    private float _timer = 0f;

    private bool _isAttacking;
    public bool IsAttacking => _isAttacking;

    private void Awake()
    {
        _stat = GetComponent<MonsterStat>();
    }

    private void OnEnable()
    {
        _attackCooltime = _stat.GetAttackCooltime();
        _timer = _attackCooltime;
    }

    private void Update()
    {
        if (_timer < _attackCooltime)
        {
            _timer += Time.deltaTime;
        }
    }

    public void SetTarget(Transform target)
    {
        _target = target;
        _targetDamageable = target != null ? target.GetComponent<IDamageable>() : null;
    }

    public bool TryAttack()
    {
        if (_isAttacking) return false;
        if (_target == null) return false;
        if (_timer < _attackCooltime) return false;

        float sqrDistance = (_target.position - transform.position).sqrMagnitude;
        if (sqrDistance > _attackDistance * _attackDistance) return false;

        Attack();
        _timer = 0f;

        return true;
    }

    private void Attack()
    {
        _isAttacking = true;

        if (_targetDamageable != null)
        {
            Damage damage = new Damage(_stat.GetAttackDamage(), transform.gameObject);
            _targetDamageable.TryTakeDamage(damage);
        }

        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        yield return new WaitForSeconds(_attackDuration);
        _isAttacking = false;
    }

    public void InitCooltime()
    {
        _timer = 0f;
    }
}
