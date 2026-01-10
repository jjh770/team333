using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloraStunSkill : FloraSkillBase
{
    [Header("Stun Settings")]
    [SerializeField] private float _stunInterval = 2f;
    [SerializeField] private float _stunDuration = 0.5f;

    [Header("Lightning Effect Timing")]
    [SerializeField] private float _lightningStrikeDelay = 0.1f;
    
    private Coroutine _stunRoutine;

    private void OnEnable()
    {
        _stunRoutine = StartCoroutine(StunRoutine());
    }

    protected override void OnDisable()
    {
        if (_stunRoutine != null)
        {
            StopCoroutine(_stunRoutine);
            _stunRoutine = null;
        }
        
        base.OnDisable();
    }

    private IEnumerator StunRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_stunInterval);
            
            List<BadMonsterController> monsters = new(MonstersInRange);
            foreach (var monster in monsters)
            {
                if (monster == null) continue;
                Vector3 position = monster.transform.position;
                _effectPool.PlayLightningStrike(position, _lightningStrikeDelay);
            }
            
            yield return new WaitForSeconds(_lightningStrikeDelay);
            
            foreach (var monster in monsters)
            {
                if (monster == null || !monster.gameObject.activeInHierarchy) continue;
                
                Vector3 position = monster.transform.position;
                monster.ApplyStun(_stunDuration);
                
                GameObject hitEffect = _effectPool.PlayLightningHit(position, _stunDuration);
                
                if (hitEffect != null)
                {
                    hitEffect.transform.SetParent(monster.transform);
                    hitEffect.transform.localPosition = Vector3.zero;
                }
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