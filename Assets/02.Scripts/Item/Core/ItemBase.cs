using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public abstract class ItemBase : MonoBehaviour, IPickable
{
    protected Rigidbody _rigidbody;
    protected Collider _collider;

    protected bool _isHeld;

    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    protected virtual void OnDestroy()
    {
        InteractionEvents.NotifyDestroyed(this);
    }

    // IInteractable
    public Transform Transform => transform;
    public virtual InteractionType Type => InteractionType.PickUp;
    public virtual bool CanInteract => !_isHeld;
    public virtual void Interact(GameObject interactor) { }

    // IPickable
    public void OnPickedUp(Transform holdPoint)
    {
        if (holdPoint == null) return;
        PickUp(holdPoint);
    }

    public void OnThrown(Vector3 direction, float force)
    {
        Drop();
        _rigidbody?.AddForce(direction * force, ForceMode.Impulse);
    }

    protected virtual void PickUp(Transform holder)
    {
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

    protected virtual void Drop()
    {
        _isHeld = false;

        transform.SetParent(null);
        _rigidbody.isKinematic = false;
        _rigidbody.useGravity = true;
        _collider.enabled = true;
    }
}
