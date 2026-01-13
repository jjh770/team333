using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillRange : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private PlayerSkillData _skillData;

    [Header("Skill Point")]
    [SerializeField] private Transform _skillPoint;
    [SerializeField] private LayerMask _monsterLayers;

    private HashSet<Collider> _hitEnemiesThisSkill = new HashSet<Collider>();

    public void ExecuteSkillHit()
    {
        _hitEnemiesThisSkill.Clear();
        CheckHitDetection();
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

            ApplyDamage(col.gameObject, _skillData.SkillDamage);
            _hitEnemiesThisSkill.Add(col);
        }
    }

    private void ApplyDamage(GameObject target, float damage)
    {
        Damage takeDamage = new Damage(damage, gameObject, true);
        if (target.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TryTakeDamage(takeDamage);
        }
    }

    public float SkillRange => _skillData.SkillRange;
    public float SkillDamage => _skillData.SkillDamage;

    private void OnDrawGizmosSelected()
    {
        if (_skillData == null) return;

        Transform point = _skillPoint != null ? _skillPoint : transform;

        Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);
        Gizmos.DrawWireSphere(point.position, _skillData.SkillRange);
    }
}
