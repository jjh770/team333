using System.Collections;
using UnityEngine;

public class FloraStunSkill : FloraSkillBase
{
    [Header("Stun Settings")]
    [SerializeField] private float _stunInterval = 2f;
    [SerializeField] private float _stunDuration = 0.5f;

    private Coroutine _stunRoutine;

    private void OnEnable()
    {
        _stunRoutine = StartCoroutine(StunRoutine());
    }

    private void OnDisable()
    {
        if (_stunRoutine != null)
        {
            StopCoroutine(_stunRoutine);
            _stunRoutine = null;
        }

        MonstersInRange.Clear();
    }

    private IEnumerator StunRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_stunInterval);

            foreach (var monster in MonstersInRange)
            {
                if (monster == null) continue;

                monster.ApplyStun(_stunDuration);
            }
        }
    }

    protected override void OnMonsterEnter(BadMonsterController monster) { }

    protected override void OnMonsterExit(BadMonsterController monster) { }

#if UNITY_EDITOR
    protected override void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
        Gizmos.DrawSphere(transform.position, Radius);

        Gizmos.color = new Color(1f, 0.5f, 0f, 1f);
        Gizmos.DrawWireSphere(transform.position, Radius);
    }
#endif
}
