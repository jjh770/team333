using UnityEngine;

public class FloraSlowSkill : FloraSkillBase
{
    [Header("Slow Settings")]
    [SerializeField] private float _slowAmount = 2f;
    
    protected override void OnMonsterEnter(BadMonsterController monster)
    {
        monster.ChangeMoveSpeed(-_slowAmount);
    }

    protected override void OnMonsterExit(BadMonsterController monster)
    {
        if (!monster.TryGetComponent<MonsterStat>(out var stat)) return;
        monster.ChangeMoveSpeed(_slowAmount);
    }

    private void OnDisable()
    {
        foreach (var monster in MonstersInRange)
        {
            if (monster == null) continue;

            monster.ChangeMoveSpeed(_slowAmount);
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