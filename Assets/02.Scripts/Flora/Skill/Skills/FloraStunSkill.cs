using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloraStunSkill : FloraSkillBase
{
    [Header("Stun Settings")]
    [SerializeField] private float _stunInterval = 2f;
    [SerializeField] private float _stunDuration = 0.5f;

    private readonly Dictionary<MonsterController, float> _originalSpeeds = new();
    private Coroutine _stunRoutine;
    private bool _isStunActive;
    
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

        ReleaseStun();
        _originalSpeeds.Clear();
        MonstersInRange.Clear();
    }

    private IEnumerator StunRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_stunInterval);

            ApplyStun();

            yield return new WaitForSeconds(_stunDuration);

            ReleaseStun();
        }
    }

    private void ApplyStun()
    {
        _isStunActive = true;
        _originalSpeeds.Clear();

        foreach (var monster in MonstersInRange)
        {
            if (monster == null) continue;
            if (!monster.TryGetComponent<MonsterStat>(out var stat)) continue;

            _originalSpeeds[monster] = stat.MoveSpeed.Value;
            stat.SetMoveSpeed(0f);
        }
    }

    private void ReleaseStun()
    {
        _isStunActive = false;

        foreach (var speeds in _originalSpeeds)
        {
            var monster = speeds.Key;
            var originalSpeed = speeds.Value;

            if (monster == null) continue;
            if (!monster.TryGetComponent<MonsterStat>(out var stat)) continue;

            stat.SetMoveSpeed(originalSpeed);
        }

        _originalSpeeds.Clear();
    }

    protected override void OnMonsterEnter(MonsterController monster) { }

    protected override void OnMonsterExit(MonsterController monster)
    {
        if (!_originalSpeeds.TryGetValue(monster, out var originalSpeed)) return;
        if (!monster.TryGetComponent<MonsterStat>(out var stat)) return;

        stat.SetMoveSpeed(originalSpeed);
        _originalSpeeds.Remove(monster);
    }

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
