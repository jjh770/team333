using System.Collections.Generic;
using UnityEngine;

public class FloraSlowSkill : FloraSkillBase
{
    [Header("Slow Settings")]
    [SerializeField] private float _slowAmount = 2f;
    
    private readonly Dictionary<BadMonsterController, GameObject> _activeSlowEffects = new();
    
    protected override void OnMonsterEnter(BadMonsterController monster)
    {
        if (monster == null) return;

        monster.ChangeMoveSpeed(-_slowAmount);
        
        GameObject slowEffect = _effectPool.PlaySlowEffect(monster.transform.position);
        if (slowEffect != null)
        {
            _activeSlowEffects[monster] = slowEffect;
        }
    }

    protected override void OnMonsterExit(BadMonsterController monster)
    {
        if (monster == null) return;

        monster.ChangeMoveSpeed(_slowAmount);
        RemoveSlowEffect(monster);
    }

    protected override void OnMonsterDeath(BadMonsterController monster)
    {
        RemoveSlowEffect(monster);
    }

    protected override void OnDisable()
    {
        foreach (var monster in MonstersInRange)
        {
            if (monster == null) continue;

            monster.ChangeMoveSpeed(_slowAmount);
            RemoveSlowEffect(monster);
        }

        base.OnDisable();
    }

    private void LateUpdate()
    {
        if (_activeSlowEffects.Count == 0) return;
        
        foreach (var activeEffect in _activeSlowEffects)
        {
            BadMonsterController monster = activeEffect.Key;
            GameObject effect = activeEffect.Value;

            if (monster != null && effect != null && monster.gameObject.activeInHierarchy)
            {
                effect.transform.position = monster.transform.position;
            }
        }
    }
    
    private void RemoveSlowEffect(BadMonsterController monster)
    {
        if (_activeSlowEffects.TryGetValue(monster, out GameObject effect))
        {
            if (effect != null)
            {
                _effectPool.ReturnEffect(effect);
            }
            _activeSlowEffects.Remove(monster);
        }
    }

#if UNITY_EDITOR
    protected override void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 0.5f, 1f, 0.3f);
        Gizmos.DrawSphere(transform.position, Radius);

        Gizmos.color = new Color(0f, 0.5f, 1f, 1f);
        Gizmos.DrawWireSphere(transform.position, Radius);
    }
#endif
}