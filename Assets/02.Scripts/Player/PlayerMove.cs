using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _attackMoveSpeed = 2f;
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private Camera _mainCamera;
    [Header("Camera Boundary")]
    [SerializeField] private float _viewportMargin = 0.05f;

    private PlayerAnimatorController _animatorController;
    private PlayerStateManager _stateManager;
    private CharacterController _controller;
    private Vector3 _velocity;

    public bool CanMove { get; set; } = true;

    public Vector3 GetMovementDirection()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 cameraForward = _mainCamera.transform.forward;
        Vector3 cameraRight = _mainCamera.transform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 direction = (cameraForward * vertical + cameraRight * horizontal).normalized;
        return direction;
    }

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _animatorController = GetComponent<PlayerAnimatorController>();
        _stateManager = GetComponent<PlayerStateManager>();
    }

    private void Update()
    {
        if (CanMove && _stateManager.CanMove)
        {
            HandleMovement();
        }

        ApplyGravity();
    }

    /// <summary>
    /// LateUpdate에서 카메라 이동 후 캐릭터 위치를 경계 내로 강제 클램핑
    /// 캐릭터가 움직이지 않아도 카메라가 움직이면 반응함
    /// </summary>
    private void LateUpdate()
    {
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

            Vector3 move = direction * _moveSpeed * Time.deltaTime;

            // 이동 전에 다음 위치를 예측하고 경계 내로 조정
            Vector3 clampedMove = ClampMovementToCameraBounds(move);

            if (_stateManager.IsInStates(PlayerState.Idle, PlayerState.Moving, PlayerState.Dashing))
            {
                _animatorController.MoveAnimation(true);
            }

            // 조정된 이동 벡터로 안전하게 이동
            _controller.Move(clampedMove);

            if (!_stateManager.IsState(PlayerState.Attacking))
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
            }
        }
        else
        {
            if (_stateManager.IsState(PlayerState.Moving))
            {
                _stateManager.ChangeState(PlayerState.Idle);
            }

            if (_stateManager.IsInStates(PlayerState.Idle, PlayerState.Moving))
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

        _velocity.y += _gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }

    /// <summary>
    /// 예측된 다음 위치를 계산하고, 경계를 벗어나는 경우 이동 벡터를 조정
    /// CharacterController를 비활성화하지 않아 더 안전함
    /// </summary>
    private Vector3 ClampMovementToCameraBounds(Vector3 moveVector)
    {
        Vector3 nextPosition = transform.position + moveVector;
        Vector3 viewportPos = _mainCamera.WorldToViewportPoint(nextPosition);

        bool isOutOfBounds = viewportPos.x < _viewportMargin || viewportPos.x > 1f - _viewportMargin ||
                             viewportPos.y < _viewportMargin || viewportPos.y > 1f - _viewportMargin ||
                             viewportPos.z <= 0;

        if (isOutOfBounds)
        {
            viewportPos.x = Mathf.Clamp(viewportPos.x, _viewportMargin, 1f - _viewportMargin);
            viewportPos.y = Mathf.Clamp(viewportPos.y, _viewportMargin, 1f - _viewportMargin);

            Vector3 clampedWorldPos = _mainCamera.ViewportToWorldPoint(viewportPos);

            Vector3 adjustedMove = new Vector3(
                clampedWorldPos.x - transform.position.x,
                moveVector.y,
                clampedWorldPos.z - transform.position.z
            );

            return adjustedMove;
        }

        return moveVector;
    }

    /// <summary>
    /// 카메라가 움직이거나 캐릭터가 정지 상태일 때도 경계를 강제로 적용
    /// LateUpdate에서 호출되어 모든 움직임 후 최종 위치를 보정
    /// </summary>
    private void EnforceCameraBounds()
    {
        Vector3 viewportPos = _mainCamera.WorldToViewportPoint(transform.position);

        bool isOutOfBounds = viewportPos.x < _viewportMargin || viewportPos.x > 1f - _viewportMargin ||
                             viewportPos.y < _viewportMargin || viewportPos.y > 1f - _viewportMargin ||
                             viewportPos.z <= 0;

        if (isOutOfBounds)
        {
            viewportPos.x = Mathf.Clamp(viewportPos.x, _viewportMargin, 1f - _viewportMargin);
            viewportPos.y = Mathf.Clamp(viewportPos.y, _viewportMargin, 1f - _viewportMargin);

            Vector3 clampedWorldPos = _mainCamera.ViewportToWorldPoint(viewportPos);

            // CharacterController.Move()를 사용하여 안전하게 위치 보정
            Vector3 correctionMove = new Vector3(
                clampedWorldPos.x - transform.position.x,
                0f, // Y축은 건드리지 않음
                clampedWorldPos.z - transform.position.z
            );

            // 보정이 필요한 경우에만 Move 호출
            if (correctionMove.sqrMagnitude > 0.0001f)
            {
                _controller.Move(correctionMove);
            }
        }
    }
}
