using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    [Header("Dash Settings")]
    [SerializeField] private float _dashSpeed = 10f;
    [SerializeField] private float _dashDuration = 0.5f;
    [SerializeField] private float _dashCooldown = 1f;

    [Header("Camera Boundary")]
    [SerializeField] private float _viewportMargin = 0.05f;

    private PlayerAnimatorController _animatorController;
    private CharacterController _controller;
    private PlayerMove _playerMove;
    private PlayerStateManager _stateManager;
    private Camera _mainCamera;
    private float _dashTimer;
    private float _dashCooldownTimer;
    private Vector3 _dashDirection;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _playerMove = GetComponent<PlayerMove>();
        _animatorController = GetComponent<PlayerAnimatorController>();
        _stateManager = GetComponent<PlayerStateManager>();
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        UpdateDashCooldown();
        HandleDashInput();

        if (_stateManager.IsState(PlayerState.Dashing))
        {
            PerformDash();
        }
    }

    private void UpdateDashCooldown()
    {
        if (_dashCooldownTimer > 0)
        {
            _dashCooldownTimer -= Time.deltaTime;
        }
    }

    private void HandleDashInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _dashCooldownTimer <= 0 && _stateManager.CanDash)
        {
            Vector3 direction = _playerMove.GetMovementDirection();

            if (direction.magnitude >= 0.1f)
            {
                StartDash(direction);
            }
        }
    }

    private void StartDash(Vector3 direction)
    {
        // 상태 변경: Dashing으로 전환
        // PlayerAttack이 OnStateChanged 이벤트를 구독하여 자동으로 공격 캔슬
        _stateManager.ChangeState(PlayerState.Dashing);

        _dashTimer = _dashDuration;
        _dashDirection = direction;
        _dashCooldownTimer = _dashCooldown;
        _animatorController.DashAnimation();

        transform.rotation = Quaternion.LookRotation(direction);
    }

    private void PerformDash()
    {
        _dashTimer -= Time.deltaTime;

        if (_dashTimer <= 0)
        {
            EndDash();
            return;
        }

        Vector3 dashMove = _dashDirection * _dashSpeed * Time.deltaTime;

        // 카메라 경계 내로 이동 벡터를 클램핑
        Vector3 clampedDashMove = CameraBoundsHelper.ClampMovementToCameraBounds(
            transform.position, dashMove, _mainCamera, _viewportMargin);

        _controller.Move(clampedDashMove);
    }

    private void EndDash()
    {
        // 상태 변경: Idle로 전환 (자동으로 이동 가능)
        _stateManager.ChangeState(PlayerState.Idle);
    }

    public bool IsDashing => _stateManager.IsState(PlayerState.Dashing);
    public float CooldownRemaining => _dashCooldownTimer;
}
