using System.Collections.Generic;
using UnityEngine;

public class FloraSlowAuraSkill : FloraSkillBase
{
    [Header("Slow Settings")]
    [SerializeField] private float _slowAmount = 2f;
    
    protected override void OnMonsterEnter(Monster monster)
    {
        if (!monster.TryGetComponent<MonsterStat>(out var stat)) return;
        stat.ModifyMoveSpeed(-_slowAmount);
    }

    protected override void OnMonsterExit(Monster monster)
    {
        if (!monster.TryGetComponent<MonsterStat>(out var stat)) return;
        stat.ModifyMoveSpeed(_slowAmount);
    }

    private void OnDisable()
    {
        foreach (var monster in MonstersInRange)
        {
            if (monster == null) continue;
            if (!monster.TryGetComponent<MonsterStat>(out var stat)) continue;

            stat.ModifyMoveSpeed(_slowAmount);
        }

        MonstersInRange.Clear();
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