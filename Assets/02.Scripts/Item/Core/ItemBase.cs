using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public abstract class ItemBase : MonoBehaviour, IPickable
{
    protected Rigidbody _rigidbody;
    protected Collider _collider;

    protected bool _isHeld;
    public bool IsHeld => _isHeld;

    protected bool _isThrown;
    public bool IsThrown => _isThrown;

    private bool _isLocked;
    public bool IsLocked => _isLocked;

    public virtual bool HidesFloraOutline { get; }

    public void SetLocked(bool locked)
    {
        if (_isLocked == locked) return;
        _isLocked = locked;
        OnLockChanged?.Invoke(locked);
    }

    public event Action<bool> OnLockChanged;
    public event Action OnHeld;
    public event Action OnDropped;

    private const string GroundTag = "Ground";

    // IInteractable
    public Transform Transform => transform;
    public virtual InteractionType Type => InteractionType.PickUp;
    public virtual IconType IconType => IconType.None;
    public virtual bool CanInteract => !_isHeld && !_isLocked;

    public virtual void Interact(GameObject interactor) { }
    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    protected virtual void OnEnable()
    {
        // 풀링 대응: 상태 초기화
        _isHeld = false;
        _isThrown = false;
    }

    protected virtual void OnDestroy()
    {
        InteractionEvents.NotifyDestroyed(this);
    }

    // IPickable
    public void OnPickedUp(Transform holdPoint)
    {
        if (holdPoint == null) return;
        PickUp(holdPoint);
    }

    public void OnThrown(Vector3 direction, float force)
    {
        Drop();
        _isThrown = true;
        _rigidbody?.AddForce(direction * force, ForceMode.Impulse);
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag(GroundTag))
        {
            return;
        }

        _isThrown = false;
    }

    protected virtual void PickUp(Transform holder)
    {
        if (_rigidbody == null) return; // 파괴된 경우 무시

        _isHeld = true;

        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        _rigidbody.isKinematic = true;
        _rigidbody.useGravity = false;
        _collider.enabled = false;

        transform.SetParent(holder);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        OnHeld?.Invoke();
    }

    protected virtual void Drop()
    {
        _isHeld = false;

        transform.SetParent(null);
        _rigidbody.isKinematic = false;
        _rigidbody.useGravity = true;
        _collider.enabled = true;

        OnDropped?.Invoke();
    }
}
