using UnityEngine;

public abstract class MonsterMoveComponent : MonoBehaviour
{
    [SerializeField] protected float _velocityThreshold = 0.1f;

    [Header("Rotation")]
    [SerializeField] private float _rotationSpeed = 5f;
    private const float MinLookDirectionSqrMagnitude = 1e-6f;

    protected Transform _target;

    protected bool _isMoving;
    public bool IsMoving => _isMoving;

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    public abstract void UpdateMove();

    public virtual void Enable() { }
    public virtual void Disable() { }
    public virtual void Stop() { }
    public virtual void Resume() { }
    public virtual void SetSpeed(float speed) { }
    public virtual void ResetMove() { }

    protected void LookAtTarget()
    {
        if (_target == null) return;

        Vector3 direction = _target.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > MinLookDirectionSqrMagnitude)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }
    }

    public void LookAtTargetImmediate()
    {
        if (_target == null) return;

        Vector3 direction = _target.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > MinLookDirectionSqrMagnitude)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
