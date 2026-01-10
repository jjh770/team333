using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FloraAnimationController : MonoBehaviour
{
    [SerializeField] private float _smoothTime = 0.2f;

    private Animator _animator;
    private float _maxSpeed;
    private float _currentSpeed;
    private float _targetSpeed;
    private float _velocity;

    private static readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed");
    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!Mathf.Approximately(_currentSpeed, _targetSpeed))
        {
            _currentSpeed = Mathf.SmoothDamp(_currentSpeed, _targetSpeed, ref _velocity, _smoothTime);
            _animator.SetFloat(MoveSpeedHash, _currentSpeed);
        }
    }

    public void Initialize(float maxSpeed)
    {
        _maxSpeed = maxSpeed;
    }

    public void PlayMove(float speed)
    {
        _animator.SetBool(IsMovingHash, true);
        SetMovementSpeed(speed);
    }

    public void PlayIdle()
    {
        _animator.SetBool(IsMovingHash, false);
        _targetSpeed = 0f;
    }

    public void SetMovementSpeed(float speed)
    {
        _targetSpeed = NormalizeSpeed(speed);
    }

    private float NormalizeSpeed(float speed)
    {
        return _maxSpeed > 0 ? Mathf.Clamp01(speed / _maxSpeed) : 0f;
    }
    
    public void PlaySkill()
    {
        _animator.SetTrigger("Skill");
    }
}
