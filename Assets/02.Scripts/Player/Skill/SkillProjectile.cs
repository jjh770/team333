using System.Collections.Generic;
using UnityEngine;

public class SkillProjectile : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask _monsterLayers;

    private float _speed;
    private float _maxDistance;
    private float _width;
    private float _damage;
    private Vector3 _direction;
    private Vector3 _startPosition;
    private GameObject _owner;

    private HashSet<Collider> _hitEnemies = new HashSet<Collider>();
    private bool _isInitialized = false;

    public void Initialize(Vector3 direction, float speed, float maxDistance, float width, float damage, GameObject owner, LayerMask monsterLayers)
    {
        _direction = direction.normalized;
        _speed = speed;
        _maxDistance = maxDistance;
        _width = width;
        _damage = damage;
        _owner = owner;
        _monsterLayers = monsterLayers;
        _startPosition = transform.position;
        _isInitialized = true;

        transform.rotation = Quaternion.LookRotation(_direction);
    }

    private void Update()
    {
        if (!_isInitialized) return;

        Move();
        CheckHit();
        CheckMaxDistance();
    }

    private void Move()
    {
        transform.position += _direction * _speed * Time.deltaTime;
    }

    private void CheckHit()
    {
        Vector3 halfExtents = new Vector3(_width / 2f, 0.5f, 0.3f);
        Collider[] hitColliders = Physics.OverlapBox(transform.position, halfExtents, transform.rotation, _monsterLayers);

        foreach (Collider col in hitColliders)
        {
            if (_hitEnemies.Contains(col))
                continue;

            ApplyDamage(col.gameObject);
            _hitEnemies.Add(col);
        }
    }

    private void ApplyDamage(GameObject target)
    {
        Damage takeDamage = new Damage(_damage, _owner, true);
        if (target.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TryTakeDamage(takeDamage);
        }
    }

    private void CheckMaxDistance()
    {
        float distanceTraveled = Vector3.Distance(_startPosition, transform.position);
        if (distanceTraveled >= _maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        if (!_isInitialized) return;

        Gizmos.color = new Color(0f, 1f, 1f, 0.5f);
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(_width, 1f, 0.6f));
    }
}
