using System.Collections.Generic;
using UnityEngine;

public struct ProjectileConfig
{
    public Vector3 Direction;
    public float Speed;
    public float MaxDistance;
    public float Width;
    public float Height;
    public float Depth;
    public float Damage;
    public GameObject Owner;
    public LayerMask MonsterLayers;
}

public class SkillProjectile : MonoBehaviour, IPoolable
{
    [Header("Settings")]
    [SerializeField] private LayerMask _monsterLayers;

    private float _speed;
    private float _maxDistance;
    private float _width;
    private float _height;
    private float _depth;
    private float _damage;
    private Vector3 _direction;
    private Vector3 _startPosition;
    private GameObject _owner;

    private HashSet<Collider> _hitEnemies = new HashSet<Collider>();
    private bool _isInitialized = false;

    public void OnSpawn()
    {
        _hitEnemies.Clear();
        _isInitialized = false;
    }

    public void OnDespawn()
    {
        _isInitialized = false;
    }

    public void Initialize(ProjectileConfig config)
    {
        _direction = config.Direction.normalized;
        _speed = config.Speed;
        _maxDistance = config.MaxDistance;
        _width = config.Width;
        _height = config.Height;
        _depth = config.Depth;
        _damage = config.Damage;
        _owner = config.Owner;
        _monsterLayers = config.MonsterLayers;
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
        Vector3 halfExtents = new Vector3(_width / 2f, _height / 2f, _depth / 2f);
        Collider[] hitColliders = Physics.OverlapBox(transform.position, halfExtents, transform.rotation, _monsterLayers);

        foreach (Collider col in hitColliders)
        {
            if (_hitEnemies.Contains(col))
                continue;

            DamageUtility.ApplyDamage(col.gameObject, _damage, _owner);
            _hitEnemies.Add(col);
        }
    }

    private void CheckMaxDistance()
    {
        float distanceTraveledSqr = (transform.position - _startPosition).sqrMagnitude;
        if (distanceTraveledSqr >= _maxDistance * _maxDistance)
        {
            PlayerEffectPool.Instance?.ReturnSkillProjectile(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        if (!_isInitialized) return;

        Gizmos.color = new Color(0f, 1f, 1f, 0.5f);
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(_width, _height, _depth));
    }
}
