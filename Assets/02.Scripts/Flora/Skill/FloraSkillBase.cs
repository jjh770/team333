using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public abstract class FloraSkillBase : MonoBehaviour
{
    [Header("Range Settings")]
    [SerializeField] private float _radius = 5f;

    private SphereCollider _triggerCollider;
    protected readonly HashSet<Monster> MonstersInRange =  new ();

    public float Radius => _radius;

    protected virtual void Awake()
    {
        _triggerCollider = GetComponent<SphereCollider>();
        _triggerCollider.isTrigger = true;
        _triggerCollider.radius = _radius;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Monster>(out var monster)) return;
        if (MonstersInRange.Contains(monster)) return;

        MonstersInRange.Add(monster);
        OnMonsterEnter(monster);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<Monster>(out var monster)) return;
        if (!MonstersInRange.Contains(monster)) return;

        MonstersInRange.Remove(monster);
        OnMonsterExit(monster);
    }

    public void ResetLocalPosition()
    {
        transform.localPosition = Vector3.zero;
    }

    public void DestroySkill()
    {
        Destroy(gameObject);
    }

    protected abstract void OnMonsterEnter(Monster monster);
    protected abstract void OnMonsterExit(Monster monster);

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