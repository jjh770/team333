using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerMove : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private PlayerMoveData _moveData;
    [SerializeField] private PlayerAttackData _attackData;
    [SerializeField] private PlayerSkillData _skillData;

    [Header("Attack Movement")]
    [SerializeField] private bool _enableAttackMovement = true;

    private Camera _mainCamera;
    private PlayerAnimatorController _animatorController;
    private PlayerStateManager _stateManager;
    private CharacterController _controller;
    private PlayerMouseHelper _mouseHelper;
    private PlayerInputHandler _inputHandler;
    private Vector3 _velocity;
    private Tweener _attackMoveTween;
    private Tweener _skillMoveTween;

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
        _mouseHelper = GetComponent<PlayerMouseHelper>();
        _inputHandler = GetComponent<PlayerInputHandler>();
    }

    private void OnDestroy()
    {
        _attackMoveTween?.Kill();
        _skillMoveTween?.Kill();
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

            if (_stateManager.IsInStates(PlayerState.Idle, PlayerState.Moving, PlayerState.Dashing, PlayerState.PickUp))
            {
                _animatorController.MoveAnimation(true);
            }

            // 조정된 이동 벡터로 안전하게 이동
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

    #region Skill Movement
    public void StartSkillMovement()
    {
        if (_controller == null)
        {
            Debug.LogError("CharacterController is null!");
            return;
        }

        // 1. 마우스 위치를 월드 좌표로 변환 (바닥 평면 기준)
        Vector3 mouseWorldPos = _mouseHelper.GetMouseWorldPosition();

        // 2. 플레이어와의 거리 및 방향 계산
        Vector3 offset = mouseWorldPos - transform.position;
        offset.y = 0; // 높이 차이 제거

        // 3. 거리 제한 (최대 스킬 이동 거리로 클램프)
        float maxDistance = _skillData.SkillMaxDistance;
        Vector3 clampedOffset = Vector3.ClampMagnitude(offset, maxDistance);
        float finalHorizontalDistance = clampedOffset.magnitude;

        // 4. 이동 방향 설정 (마우스 방향 혹은 현재 보고 있는 방향)
        Vector3 moveDirection = clampedOffset.normalized;
        if (moveDirection == Vector3.zero) moveDirection = transform.forward;

        // 트윈 초기화
        _skillMoveTween?.Kill();
        float moveDuration = _animatorController.GetCurrentAnimationDuration();

        float prevHorizontal = 0f;
        float prevVertical = 0f;

        _skillMoveTween = DOVirtual.Float(0f, 1f, moveDuration, (t) =>
        {
            // 전방 이동 (클램프된 마우스 거리까지)
            float currentHorizontal = t * finalHorizontalDistance;
            float deltaHorizontal = currentHorizontal - prevHorizontal;

            // 상하 포물선 이동
            float currentVertical = 4 * _skillData.SkillJumpHeight * t * (1 - t);
            float deltaVertical = currentVertical - prevVertical;

            Vector3 moveVector = (moveDirection * deltaHorizontal) + (Vector3.up * deltaVertical);

            // 카메라 경계 제한 및 이동 적용
            Vector3 clampedMove = CameraBoundsHelper.ClampMovementToCameraBounds(
                transform.position, moveVector, _mainCamera, _moveData.ViewportMargin);

            _controller.Move(clampedMove);

            prevHorizontal = currentHorizontal;
            prevVertical = currentVertical;
        })
        .SetEase(_skillData.SkillMoveEase)
        .OnKill(() => _skillMoveTween = null);
    }
    #endregion

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
        float moveDuration = _animatorController.GetCurrentAnimationDuration();

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
        .SetEase(_attackData.AttackMoveEase[comboIndex])
        .OnKill(() => _attackMoveTween = null);
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
