using DG.Tweening;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private PlayerMoveData _moveData;
    [SerializeField] private PlayerAttackData _attackData;

    [Header("Attack Movement")]
    [SerializeField] private bool _enableAttackMovement = true;

    private Camera _mainCamera;
    private PlayerAnimatorController _animatorController;
    private PlayerStateManager _stateManager;
    private CharacterController _controller;
    private Vector3 _velocity;
    private Tweener _attackMoveTween;

    public bool CanMove { get; set; } = true;

    private void Awake()
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }

        _controller = GetComponent<CharacterController>();
        _animatorController = GetComponent<PlayerAnimatorController>();
        _stateManager = GetComponent<PlayerStateManager>();
    }

    private void OnDestroy()
    {
        _attackMoveTween?.Kill();
    }

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

            // 공격 중일 때는 이동속도 0
            float currentSpeed = _stateManager.IsState(PlayerState.Attacking) ? 0 : _moveData.MoveSpeed;
            Vector3 move = direction * currentSpeed * Time.deltaTime;

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
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _moveData.RotationSpeed * Time.deltaTime);
            }
        }
        else
        {
            if (_stateManager.IsInStates(PlayerState.Moving, PlayerState.Dashing))
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

        _velocity.y += _moveData.Gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }

    /// <summary>
    /// 예측된 다음 위치를 계산하고, 경계를 벗어나는 경우 이동 벡터를 조정
    /// CharacterController를 비활성화하지 않아 더 안전함
    /// </summary>
    private Vector3 ClampMovementToCameraBounds(Vector3 moveVector)
    {
        return CameraBoundsHelper.ClampMovementToCameraBounds(transform.position, moveVector, _mainCamera, _moveData.ViewportMargin);
    }

    /// <summary>
    /// 카메라가 움직이거나 캐릭터가 정지 상태일 때도 경계를 강제로 적용
    /// LateUpdate에서 호출되어 모든 움직임 후 최종 위치를 보정
    /// </summary>
    private void EnforceCameraBounds()
    {
        CameraBoundsHelper.ClampPositionToCameraBounds(transform, _controller, _mainCamera, _moveData.ViewportMargin);
    }

    #region Attack Movement

    /// <summary>
    /// 공격 시 전방으로 돌진 이동 시작
    /// PlayerAttack에서 호출됨
    /// </summary>
    public void StartAttackMovement(int comboIndex)
    {
        // P0: 배열 범위 체크
        if (!_enableAttackMovement) return;

        if (_controller == null)
        {
            Debug.LogError("CharacterController is null!");
            return;
        }

        // 배열 유효성 검증
        if (comboIndex < 0 || comboIndex >= _attackData.AttackMoveDistance.Length ||
            comboIndex >= _attackData.AttackMoveEase.Length)
        {
            Debug.LogError($"Invalid combo index: {comboIndex}. " +
                          $"Distance array length: {_attackData.AttackMoveDistance.Length}, " +
                          $"Ease array length: {_attackData.AttackMoveEase.Length}");
            return;
        }

        Vector3 direction = GetMovementDirection();

        if (direction.magnitude < 0.1f) return;

        _attackMoveTween?.Kill();

        // 애니메이션 길이를 이동 시간으로 사용
        float moveDuration = _animatorController.GetCurrentAttackAnimationDuration();

        Vector3 moveDirection = transform.forward;
        float previousValue = 0f;

        _attackMoveTween = DOVirtual.Float(0f, _attackData.AttackMoveDistance[comboIndex], moveDuration, (currentValue) =>
        {
            float deltaDistance = currentValue - previousValue;
            Vector3 moveVector = moveDirection * deltaDistance;

            // P1: 카메라 경계 내로 이동 제한
            Vector3 clampedMove = CameraBoundsHelper.ClampMovementToCameraBounds(
                transform.position, moveVector, _mainCamera, _moveData.ViewportMargin);

            _controller.Move(clampedMove);

            previousValue = currentValue;
        })
        .SetEase(_attackData.AttackMoveEase[comboIndex]);
    }

    /// <summary>
    /// 공격 이동 중단
    /// PlayerAttack에서 호출됨
    /// </summary>
    public void StopAttackMovement()
    {
        if (_attackMoveTween != null && _attackMoveTween.IsActive())
        {
            _attackMoveTween.Kill();
        }
    }
    #endregion
}
