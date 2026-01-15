using System.Collections;
using UnityEngine;

public class PetAttackComponent : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private float _attackDamage = 5f;
    [SerializeField] private float _coolTime = 2f;
    [SerializeField] private float _attackDuration = 0.14f;
    [SerializeField] private float _questTreeDamageMultiplier = 10f;

    [Header("Projectile")]
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private GameObject _hitEffectPrefab;
    [SerializeField] private Transform _firePoint;

    private float _lastAttackTime;

    public bool CanAttack => Time.time >= _lastAttackTime + _coolTime;

    private bool _isAttacking;
    public bool IsAttacking => _isAttacking;

    public bool TryAttack(Transform target)
    {
        if (target == null) return false;
        if (!CanAttack) return false;
        if (!target.TryGetComponent<IDamageable>(out _)) return false;

        _lastAttackTime = Time.time;
        StartCoroutine(AttackRoutine(target));
        return true;
    }

    private IEnumerator AttackRoutine(Transform target)
    {
        _isAttacking = true;
        yield return new WaitForSeconds(_attackDuration);

        FireProjectile(target);

        _isAttacking = false;
    }

    private void FireProjectile(Transform target)
    {
        if (_projectilePrefab == null) return;

        Vector3 spawnPosition = _firePoint.position;
        GameObject projectileObj = PoolManager.Instance.Get(_projectilePrefab, spawnPosition, Quaternion.identity);

        if (projectileObj.TryGetComponent<PetProjectile>(out var projectile))
        {
            float finalDamage = CalculateDamage(target);
            Damage damage = new Damage(finalDamage, gameObject, false);
            projectile.Initialize(target, _hitEffectPrefab, damage);
        }
    }

    private float CalculateDamage(Transform target)
    {
        if (target.TryGetComponent<QuestTree>(out _))
        {
            return _attackDamage * _questTreeDamageMultiplier;
        }

        return _attackDamage;
    }
}
