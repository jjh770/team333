using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloraBurnSkill : FloraSkillBase
{
    [Header("Dot Damage Settings")]
    [SerializeField] private float _damageInterval = 0.5f;
    [SerializeField] private float _damageAmount = 1f;
    
    private readonly Dictionary<BadMonsterController, GameObject> _activeBurnEffects = new();
    private readonly List<BadMonsterController> _monsterCache = new();
    private Coroutine _burnRoutine;

    protected override void OnEnable()
    {
        base.OnEnable();
        _burnRoutine = StartCoroutine(DotDamageRoutine());
    }

    protected override void OnDisable()
    {
        if (_burnRoutine != null)
        {
            StopCoroutine(_burnRoutine);
            _burnRoutine = null;
        }

        var monstersInRangeCopy = new List<BadMonsterController>(MonstersInRange);
        foreach (var monster in monstersInRangeCopy)
        {
            if (monster == null) continue;
            RemoveBurnEffect(monster);
        }
        
        base.OnDisable();
    }

    private IEnumerator DotDamageRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_damageInterval);
            
            _monsterCache.Clear();
            _monsterCache.AddRange(MonstersInRange);
            
            foreach (var monster in _monsterCache)
            {
                if (monster == null || !monster.gameObject.activeInHierarchy || monster.IsDead) 
                    continue;
                
                Damage damage = new Damage(_damageAmount, gameObject, false);
                monster.TryTakeDamage(damage);
            }
        }
    }

    protected override void OnMonsterEnter(BadMonsterController monster)
    {
        if (monster == null) return;

        GameObject burnEffect = _effectPool.PlayBurnEffect(monster.transform.position);
        if (burnEffect != null)
        {
            _activeBurnEffects[monster] = burnEffect;
        }
    }

    protected override void OnMonsterExit(BadMonsterController monster)
    {
        if (monster == null) return;
        
        RemoveBurnEffect(monster);
    }

    protected override void OnMonsterDeath(BadMonsterController monster)
    {
        RemoveBurnEffect(monster);
    }

    private void LateUpdate()
    {
        if (_activeBurnEffects.Count == 0) return;
        
        foreach (var activeEffect in _activeBurnEffects)
        {
            BadMonsterController monster = activeEffect.Key;
            GameObject effect = activeEffect.Value;

            if (monster != null && effect != null && monster.gameObject.activeInHierarchy)
            {
                effect.transform.position = monster.transform.position;
            }
        }
    }

    private void RemoveBurnEffect(BadMonsterController monster)
    {
        if (_activeBurnEffects.TryGetValue(monster, out GameObject effect))
        {
            if (effect != null)
            {
                _effectPool.ReturnEffect(effect);
            }
            _activeBurnEffects.Remove(monster);
        }
    }

#if UNITY_EDITOR
    protected override void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawSphere(transform.position, Radius);

        Gizmos.color = new Color(1f, 0f, 0f, 1f);
        Gizmos.DrawWireSphere(transform.position, Radius);
    }
#endif
}
