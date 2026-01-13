using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillRange : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private PlayerSkillData _skillData;

    [Header("Skill Point")]
    [SerializeField] private Transform _skillPoint;
    [SerializeField] private LayerMask _monsterLayers;

    [Header("Projectile")]
    [SerializeField] private SkillProjectile _projectilePrefab;
    [SerializeField] private Transform _projectileSpawnPoint;

    private HashSet<Collider> _hitEnemiesThisSkill = new HashSet<Collider>();

    public void ExecuteSkillHit()
    {
        _hitEnemiesThisSkill.Clear();
        CheckHitDetection();
    }

    public void FireProjectile(bool isTriple = false)
    {
        if (_projectilePrefab == null) return;

        Transform spawnPoint = _projectileSpawnPoint != null ? _projectileSpawnPoint : transform;
        Vector3 spawnPosition = spawnPoint.position;
        Vector3 baseDirection = transform.forward;

        float[] angles = isTriple ? _skillData.TripleProjectileAngles : _skillData.SingleProjectileAngles;

        foreach (float angle in angles)
        {
            Vector3 direction = Quaternion.Euler(0f, angle, 0f) * baseDirection;
            SkillProjectile projectile = Instantiate(_projectilePrefab, spawnPosition, Quaternion.identity);
            SetupProjectile(projectile, direction);
        }
    }

    private void SetupProjectile(SkillProjectile projectile, Vector3 direction)
    {
        var config = new ProjectileConfig
        {
            Direction = direction,
            Speed = _skillData.ProjectileSpeed,
            MaxDistance = _skillData.ProjectileRange,
            Width = _skillData.ProjectileWidth,
            Height = _skillData.ProjectileHeight,
            Depth = _skillData.ProjectileDepth,
            Damage = _skillData.SkillDamage,
            Owner = gameObject,
            MonsterLayers = _monsterLayers
        };
        projectile.Initialize(config);
    }

    private void CheckHitDetection()
    {
        if (_skillPoint == null)
        {
            _skillPoint = transform;
        }

        Collider[] hitColliders = Physics.OverlapSphere(_skillPoint.position, _skillData.SkillRange, _monsterLayers);

        foreach (Collider col in hitColliders)
        {
            if (_hitEnemiesThisSkill.Contains(col))
                continue;

            DamageUtility.ApplyDamage(col.gameObject, _skillData.SkillDamage, gameObject);
            _hitEnemiesThisSkill.Add(col);
        }
    }

    public float SkillRange => _skillData.SkillRange;
    public float SkillDamage => _skillData.SkillDamage;

    private void OnDrawGizmosSelected()
    {
        if (_skillData == null) return;

        // 착지 범위 표시
        Transform point = _skillPoint != null ? _skillPoint : transform;
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);
        Gizmos.DrawWireSphere(point.position, _skillData.SkillRange);

        // 투사체 범위 표시
        Transform spawnPoint = _projectileSpawnPoint != null ? _projectileSpawnPoint : transform;
        Gizmos.color = new Color(0f, 1f, 1f, 0.5f);
        Vector3 endPoint = spawnPoint.position + transform.forward * _skillData.ProjectileRange;
        Gizmos.DrawLine(spawnPoint.position, endPoint);
        Gizmos.DrawWireCube(endPoint, new Vector3(_skillData.ProjectileWidth, _skillData.ProjectileHeight, _skillData.ProjectileDepth));
    }
}
