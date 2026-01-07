using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FloraAnimationController : MonoBehaviour
{
    private Animator _animator;
    private float _maxSpeed;
    
    private static readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed");
    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
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
        _animator.SetFloat(MoveSpeedHash, 0f);
    }

    public void SetMovementSpeed(float speed)
    {
        float normalizedSpeed = NormalizeSpeed(speed);
        _animator.SetFloat(MoveSpeedHash, normalizedSpeed);
    }

    private float NormalizeSpeed(float speed)
    {
        return _maxSpeed > 0 ? Mathf.Clamp01(speed / _maxSpeed) : 0f;
    }
}
