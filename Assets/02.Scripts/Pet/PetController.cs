using UnityEngine;

[RequireComponent(typeof(PetMovementComponent))]
[RequireComponent(typeof(PetAttackComponent))]
[RequireComponent(typeof(PetSensorComponent))]
[RequireComponent(typeof(Animator))]
public class PetController : MonoBehaviour
{
    private static readonly int s_animationHash = Animator.StringToHash("animation");

    [SerializeField] private float _lookTargetSpeed = 10f;
    [SerializeField] private float _lookDirectionSqrThreshold = 0.01f;

    private PetMovementComponent _movement;
    private PetAttackComponent _attack;
    private PetSensorComponent _sensor;
    private Animator _animator;

    private PetState _currentState;
    private bool _isInitialized;

    private void Awake()
    {
        _movement = GetComponent<PetMovementComponent>();
        _attack = GetComponent<PetAttackComponent>();
        _sensor = GetComponent<PetSensorComponent>();
        _animator = GetComponent<Animator>();
    }

    public void Initialize(Transform player)
    {
        _movement.Initialize(player);
        _isInitialized = true;
    }

    private void Update()
    {
        if (!_isInitialized) return;

        _sensor.DetectTarget();

        if (_sensor.HasTarget)
        {
            LookTarget(_sensor.CurrentTarget);
            _attack.TryAttack(_sensor.CurrentTarget);
        }
        else
        {
            _movement.LookPlayer();
        }

        _movement.FollowPlayer(Time.deltaTime);

        UpdateState();
    }

    private void LookTarget(Transform target)
    {
        if (target == null) return;

        Vector3 direction = target.position - transform.position;
        direction.y = 0;

        if (direction.sqrMagnitude > _lookDirectionSqrThreshold)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _lookTargetSpeed * Time.deltaTime);
        }
    }

    private void UpdateState()
    {
        PetState newState = GetCurrentState();

        if (_currentState != newState)
        {
            ApplyState(newState);
        }
    }

    private PetState GetCurrentState()
    {
        if (_attack.IsAttacking) return PetState.Attack;
        if (_movement.IsMoving) return PetState.Move;
        return PetState.Idle;
    }

    private void ApplyState(PetState newState)
    {
        _currentState = newState;
        _animator.SetInteger(s_animationHash, (int)_currentState);
    }
}
