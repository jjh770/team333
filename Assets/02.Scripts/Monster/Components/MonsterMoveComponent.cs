using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class MonsterMoveComponent : MonoBehaviour
{
    [SerializeField] protected float _velocityThreshold = 0.1f;

    [Header("Rotation")]
    [SerializeField] private float _rotationSpeed = 5f;
    private const float MinLookDirectionSqrMagnitude = 1e-6f;

    protected NavMeshAgent _agent;
    protected Transform _target;

    protected bool _isMoving;
    public bool IsMoving => _isMoving;

    protected virtual void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    public abstract void UpdateMove();

    protected void UpdateMoveState()
    {
        _isMoving = _agent.velocity.sqrMagnitude > _velocityThreshold * _velocityThreshold;
    }

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
