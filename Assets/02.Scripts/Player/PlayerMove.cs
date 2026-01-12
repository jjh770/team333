using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private PlayerMoveData _moveData;

    private Camera _mainCamera;
    private PlayerAnimatorController _animatorController;
    private PlayerStateManager _stateManager;
    private CharacterController _controller;
    private PlayerInputHandler _inputHandler;
    private Vector3 _velocity;

    public bool CanMove { get; set; } = true;
    public Camera MainCamera => _mainCamera;
    public CharacterController Controller => _controller;
    public float ViewportMargin => _moveData.ViewportMargin;

    private void Awake()
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }

        _controller = GetComponent<CharacterController>();
        _animatorController = GetComponent<PlayerAnimatorController>();
        _stateManager = GetComponent<PlayerStateManager>();
        _inputHandler = GetComponent<PlayerInputHandler>();
    }

    public Vector3 GetMovementDirection()
    {
        Vector2 input = _inputHandler.MoveInput;

        Vector3 cameraForward = _mainCamera.transform.forward;
        Vector3 cameraRight = _mainCamera.transform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 direction = (cameraForward * input.y + cameraRight * input.x).normalized;
        return direction;
    }

    private void Update()
    {
        if (CanMove && _stateManager.CanMove)
        {
            HandleMovement();
        }

        ApplyGravity();
    }

    private void LateUpdate()
    {
        if (GameStateManager.Instance == null || !GameStateManager.Instance.IsPlaying) return;

        EnforceCameraBounds();
    }

    private void HandleMovement()
    {
        Vector3 direction = GetMovementDirection();

        if (direction.magnitude >= 0.1f)
        {
            if (_stateManager.IsState(PlayerState.Idle))
            {
                _stateManager.ChangeState(PlayerState.Moving);
            }

            float currentSpeed = _stateManager.IsState(PlayerState.Attacking) ? 0 : _moveData.MoveSpeed;
            Vector3 move = direction * currentSpeed * Time.deltaTime;

            Vector3 clampedMove = ClampMovementToCameraBounds(move);

            if (_stateManager.IsInStates(PlayerState.Idle, PlayerState.Moving, PlayerState.Dashing, PlayerState.PickUp))
            {
                _animatorController.MoveAnimation(true);
            }

            _controller.Move(clampedMove);

            if (!_stateManager.IsInStates(PlayerState.Attacking, PlayerState.Skill))
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _moveData.RotationSpeed * Time.deltaTime);
            }
        }
        else
        {
            if (_stateManager.IsInStates(PlayerState.Moving, PlayerState.Dashing))
            {
                _stateManager.ChangeState(PlayerState.Idle);
            }

            if (_stateManager.IsInStates(PlayerState.Idle, PlayerState.Moving, PlayerState.Attacking, PlayerState.PickUp))
            {
                _animatorController.MoveAnimation(false);
            }
        }
    }

    private void ApplyGravity()
    {
        if (_controller.isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        _velocity.y += _moveData.Gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }

    private Vector3 ClampMovementToCameraBounds(Vector3 moveVector)
    {
        return CameraBoundsHelper.ClampMovementToCameraBounds(transform.position, moveVector, _mainCamera, _moveData.ViewportMargin);
    }

    private void EnforceCameraBounds()
    {
        CameraBoundsHelper.ClampPositionToCameraBounds(transform, _controller, _mainCamera, _moveData.ViewportMargin);
    }
}
