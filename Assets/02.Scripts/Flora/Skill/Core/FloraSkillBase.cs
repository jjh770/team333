using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public abstract class FloraSkillBase : MonoBehaviour
{
    [Header("Range Settings")] 
    [SerializeField] private float _radius = 4f;
        
    protected FloraEffectPool _effectPool;
    private SphereCollider _triggerCollider;
    protected readonly HashSet<BadMonsterController> MonstersInRange =  new ();

    public float Radius => _radius;

    protected virtual void Awake()
    {
        _triggerCollider = GetComponent<SphereCollider>();
        _triggerCollider.isTrigger = true;
        _triggerCollider.radius = _radius;
    }

    public virtual void Initialize(FloraEffectPool effectPool)
    {
        _effectPool = effectPool;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<BadMonsterController>(out var monster)) return;
        
        if (MonstersInRange.Contains(monster)) return;
        
        MonstersInRange.Add(monster);
        
        monster.OnDie += HandleMonsterDeath;
        
        OnMonsterEnter(monster);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<BadMonsterController>(out var monster)) return;
        if (!MonstersInRange.Contains(monster)) return;

        MonstersInRange.Remove(monster);
        
        monster.OnDie -= HandleMonsterDeath;
        
        OnMonsterExit(monster);
    }
    
    protected virtual void OnDisable()
    {
        foreach (var monster in MonstersInRange)
        {
            if (monster != null)
            {
                monster.OnDie -= HandleMonsterDeath;
            }
        }
        
        MonstersInRange.Clear();
    }
    
    private void HandleMonsterDeath(BadMonsterController monster)
    {
        if (monster == null) return;

        monster.OnDie -= HandleMonsterDeath;
        
        MonstersInRange.Remove(monster);
        
        OnMonsterDeath(monster);
    }

    public void ResetLocalPosition()
    {
        transform.localPosition = Vector3.zero;
    }

    public void DestroySkill()
    {
        Destroy(gameObject);
    }

    protected abstract void OnMonsterEnter(BadMonsterController monster);
    protected abstract void OnMonsterExit(BadMonsterController monster);
    protected virtual void OnMonsterDeath(BadMonsterController monster) { }
    
    protected virtual void OnValidate()
    {
        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        if (sphereCollider != null)
        {
            sphereCollider.radius = _radius;
            sphereCollider.isTrigger = true;
        }
    }

#if UNITY_EDITOR
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.DrawSphere(transform.position, _radius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
#endif
}