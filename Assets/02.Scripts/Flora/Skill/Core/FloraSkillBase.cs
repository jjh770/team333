using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public abstract class FloraSkillBase : MonoBehaviour
{
    [Header("Range Settings")]
    [SerializeField] private float _radius = 4f;

    [Header("Duration Settings")]
    [SerializeField] private float _skillDuration = 15f;
    [SerializeField] private float _scaleInDuration = 0.3f;
    [SerializeField] private float _scaleOutDuration = 0.3f;

    [Header("Sound")]
    [SerializeField] private SoundInfo _skillSound;

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

    protected virtual void OnEnable()
    {
        PlaySkillSound();
    }

    private void PlaySkillSound()
    {
        if (_skillSound.Clip == null) return;
        SoundManager.Instance.PlaySFX(_skillSound.Clip, _skillSound.StartTime, 1f);
    }

    public virtual void Initialize(FloraEffectPool effectPool)
    {
        _effectPool = effectPool;

        transform.localScale = Vector3.zero;

        DOTween.Sequence()
            .Append(transform.DOScale(Vector3.one, _scaleInDuration).SetEase(Ease.OutBack))
            .AppendInterval(_skillDuration)
            .Append(transform.DOScale(Vector3.zero, _scaleOutDuration).SetEase(Ease.InBack))
            .AppendCallback(DestroySkill)
            .SetTarget(transform);
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
        transform.DOKill();

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