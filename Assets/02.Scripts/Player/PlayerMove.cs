using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] private float _rotationSpeed = 10f;

    [Header("Camera Boundary")]
    [SerializeField] private float _viewportMargin = 0.05f;

    private PlayerAnimatorController _animatorController;
    private PlayerStateManager _stateManager;
    private CharacterController _controller;
    private Camera _mainCamera;
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
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (CanMove && _stateManager.CanMove)
        {
            HandleMovement();
        }

        ApplyGravity();
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

            if (_stateManager.IsInStates(PlayerState.Idle, PlayerState.Moving, PlayerState.Dashing))
            {
                _animatorController.MoveAnimation(true);
            }

            _controller.Move(move);

            // 이동 후 위치를 카메라 범위 내로 직접 클램핑
            ClampPositionToCameraBounds();

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
    /// 플레이어 위치를 카메라 뷰포트 범위 내로 직접 강제 클램핑
    /// CharacterController를 비활성화한 후 transform.position 직접 수정
    /// </summary>
    private void ClampPositionToCameraBounds()
    {
        Vector3 viewportPos = _mainCamera.WorldToViewportPoint(transform.position);

        // 경계를 벗어난 경우만 처리
        bool isOutOfBounds = viewportPos.x < _viewportMargin || viewportPos.x > 1f - _viewportMargin ||
                             viewportPos.y < _viewportMargin || viewportPos.y > 1f - _viewportMargin ||
                             viewportPos.z <= 0;

        if (isOutOfBounds)
        {
            // 뷰포트 좌표를 마진 범위 내로 클램핑
            viewportPos.x = Mathf.Clamp(viewportPos.x, _viewportMargin, 1f - _viewportMargin);
            viewportPos.y = Mathf.Clamp(viewportPos.y, _viewportMargin, 1f - _viewportMargin);

            // 클램핑된 뷰포트 좌표를 월드 좌표로 변환
            Vector3 clampedWorldPos = _mainCamera.ViewportToWorldPoint(viewportPos);

            // CharacterController를 일시적으로 비활성화하고 위치 직접 수정
            _controller.enabled = false;
            transform.position = new Vector3(clampedWorldPos.x, transform.position.y, clampedWorldPos.z);
            _controller.enabled = true;
        }
    }
}
