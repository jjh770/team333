using System.Collections;
using UnityEngine;

public class PetAttackComponent : MonoBehaviour
{
    [SerializeField] private float _attackDamage = 5f;
    [SerializeField] private float _coolTime = 2f;
    [SerializeField] private float _attackDuration = 0.14f;

    private float _lastAttackTime;

    public bool CanAttack => Time.time >= _lastAttackTime + _coolTime;

    private bool _isAttacking;
    public bool IsAttacking => _isAttacking;

    public bool TryAttack(Transform target)
    {
        if (target == null) return false;
        if (!CanAttack) return false;

        if (target.TryGetComponent<IDamageable>(out var damageable))
        {
            Damage damage = new Damage(_attackDamage, gameObject, false);
            damageable.TryTakeDamage(damage);
            _lastAttackTime = Time.time;

            StartCoroutine(AttackRoutine());
            return true;
        }

        return false;
    }

    private IEnumerator AttackRoutine()
    {
        _isAttacking = true;
        yield return new WaitForSeconds(_attackDuration);
        _isAttacking = false;
    }
}
