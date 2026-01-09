using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public abstract class ItemBase : MonoBehaviour
{
    protected Rigidbody _rigidbody;
    protected Collider _collider;
    
    protected bool _isHeld;
    
    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    public virtual bool CanThrow => false;

    public virtual void PickUp(Transform holder)
    {
        // TODO : 수정
        _isHeld = true;
        
        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        _rigidbody.isKinematic = true;
        _rigidbody.useGravity = false;
        _collider.enabled = false;
        
        transform.SetParent(holder);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public virtual void Drop()
    {
        // TODO : 수정
        _isHeld = false;
        
        transform.SetParent(null);
        _rigidbody.isKinematic = false;
        _rigidbody.useGravity = true;
        _collider.enabled = true;
    }
}
