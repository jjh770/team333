using System;
using DG.Tweening;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private PlayerDashData _dashData;

    private PlayerAnimatorController _animatorController;
    private PlayerMove _playerMove;
    private PlayerStateManager _stateManager;
    private PlayerInputHandler _inputHandler;
    private TweenMovement _tweenMovement;
    private float _dashCooldownTimer;

    public event Action<float> OnDashStart;
    public event Action OnDashFinish;

    private void Awake()
    {
        _playerMove = GetComponent<PlayerMove>();
        _animatorController = GetComponent<PlayerAnimatorController>();
        _stateManager = GetComponent<PlayerStateManager>();
        _inputHandler = GetComponent<PlayerInputHandler>();
        _tweenMovement = GetComponent<TweenMovement>();
    }

    private void OnEnable()
    {
        _inputHandler.OnDashInput += HandleDashInput;
    }

    private void OnDisable()
    {
        _inputHandler.OnDashInput -= HandleDashInput;
    }

    private void Update()
    {
        if (_dashCooldownTimer > 0)
        {
            _dashCooldownTimer -= Time.deltaTime;
        }
    }

    private void HandleDashInput()
    {
        if (_dashCooldownTimer > 0 || !_stateManager.CanDash) return;

        Vector3 direction = _playerMove.GetMovementDirection();

        if (direction.magnitude >= 0.1f)
        {
            StartDash(direction);
        }
    }

    private void StartDash(Vector3 direction)
    {
        _stateManager.ChangeState(PlayerState.Dashing);
        _dashCooldownTimer = _dashData.DashCoolDown;
        _animatorController.DashAnimation();

        transform.rotation = Quaternion.LookRotation(direction);

        float dashDistance = _dashData.DashSpeed * _dashData.DashDuration;
        _tweenMovement.StartLinearMovement(
            direction,
            dashDistance,
            _dashData.DashDuration,
            Ease.Linear,
            EndDash);

        OnDashStart?.Invoke(_dashData.DashDuration);
    }

    private void EndDash()
    {
        _stateManager.ChangeState(PlayerState.Idle);
        OnDashFinish?.Invoke();
    }

    public bool IsDashing => _stateManager.IsState(PlayerState.Dashing);
    public float CooldownRemaining => _dashCooldownTimer;
}
