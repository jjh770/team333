using System.Collections.Generic;
using UnityEngine;

public class FloraSlowSkill : FloraSkillBase
{
    [Header("Slow Settings")]
    [SerializeField] private float _slowAmount = 2f;
    private readonly Dictionary<BadMonsterController, GameObject> _activeSlowEffects = new();
    
    protected override void OnMonsterEnter(BadMonsterController monster)
    {
        monster.ChangeMoveSpeed(-_slowAmount);
        
        GameObject slowEffect = _effectPool.PlaySlowEffect(monster.transform.position);
        if (slowEffect != null)
        {
            slowEffect.transform.SetParent(monster.transform);
            slowEffect.transform.localPosition = Vector3.zero;
            
            _activeSlowEffects[monster] = slowEffect;
        }
    }

    protected override void OnMonsterExit(BadMonsterController monster)
    {
        monster.ChangeMoveSpeed(_slowAmount);
        
        RemoveSlowEffect(monster);
    }

    private void OnDisable()
    {
        foreach (var monster in MonstersInRange)
        {
            if (monster == null) continue;

            monster.ChangeMoveSpeed(_slowAmount);
            RemoveSlowEffect(monster);
        }

        MonstersInRange.Clear();
    }
    
    private void RemoveSlowEffect(BadMonsterController monster)
    {
        if (_activeSlowEffects.TryGetValue(monster, out GameObject effect))
        {
            if (effect != null)
            {
                effect.transform.SetParent(null);
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