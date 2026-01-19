using UnityEngine;

public class PetProjectile : MonoBehaviour, IPoolable
{
    [SerializeField] private float _speed = 15f;
    [SerializeField] private float _hitDistance = 0.5f;

    private Transform _target;
    private GameObject _hitEffectPrefab;
    private Damage _damage;
    private bool _isInitialized;

    public void Initialize(Transform target, GameObject hitEffectPrefab, Damage damage)
    {
        _target = target;
        _hitEffectPrefab = hitEffectPrefab;
        _damage = damage;
        _isInitialized = true;
    }

    private void Update()
    {
        if (!_isInitialized) return;

        if (_target == null)
        {
            ReturnToPool();
            return;
        }

        Vector3 direction = (_target.position - transform.position).normalized;
        transform.position += direction * _speed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(direction);

        float distance = Vector3.Distance(transform.position, _target.position);
        if (distance <= _hitDistance)
        {
            OnHit();
        }
    }

    private void OnHit()
    {
        if (_target.TryGetComponent<IDamageable>(out var damageable))
        {
            MonsterSound.Instance.PetAttack();
            damageable.TryTakeDamage(_damage);
        }

        if (_hitEffectPrefab != null)
        {
            PoolManager.Instance.Get(_hitEffectPrefab, _target.position, Quaternion.identity);
        }

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        _isInitialized = false;
        PoolManager.Instance.Return(gameObject);
    }

    public void OnSpawn()
    {
    }

    public void OnDespawn()
    {
        _target = null;
        _hitEffectPrefab = null;
        _isInitialized = false;
    }
}
