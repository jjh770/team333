using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private PlayerAttackData _attackData;

    private PlayerRotateToMouse _rotateToMouse;
    private PlayerAnimatorController _animatorController;
    private PlayerStateManager _stateManager;
    private PlayerAttackRange _attackRange;
    private PlayerMove _playerMove;
    private PlayerDash _playerDash;
    private bool _canAttack = true;
    [SerializeField] private int _comboIndex = 0;
    private float _lastAttackTime;

    private void Awake()
    {
        _animatorController = GetComponent<PlayerAnimatorController>();
        _stateManager = GetComponent<PlayerStateManager>();
        _attackRange = GetComponent<PlayerAttackRange>();
        _rotateToMouse = GetComponent<PlayerRotateToMouse>();
        _playerMove = GetComponent<PlayerMove>();
        _playerDash = GetComponent<PlayerDash>();
        _stateManager.OnStateChanged += OnStateChanged;
        _playerDash.OnDashFinish += OnDashFinished;
    }

    private void OnDestroy()
    {
        if (_stateManager != null)
        {
            _stateManager.OnStateChanged -= OnStateChanged;
        }
        if (_playerDash != null)
        {
            _playerDash.OnDashFinish -= OnDashFinished;
        }
    }

    private void OnStateChanged(PlayerState from, PlayerState to)
    {
        // P2: 대시 캔슬 시 처리
        if (from == PlayerState.Attacking && to == PlayerState.Dashing)
        {
            // 공격 이동 중단
            _playerMove.StopAttackMovement();

            // 연속 판정 중단
            if (_attackRange != null)
            {
                _attackRange.StopAttack();
            }

            // 대시 캔슬 시 콤보 유지 (다음 타로 진행)
            _comboIndex++;
            if (_comboIndex >= _attackData.MaxComboCount)
            {
                _comboIndex = 0;
            }
        }
    }

    private void Update()
    {
        CheckComboTimeout();
        HandleAttackInput();
    }

    private void CheckComboTimeout()
    {
        if (_comboIndex > 0 && Time.time - _lastAttackTime > _attackData.ComboWindowTime)
        {
            ResetCombo();
        }
    }

    private void HandleAttackInput()
    {
        if (Input.GetMouseButtonDown(0) && _canAttack && _stateManager.CanAttack)
        {
            _rotateToMouse.RotateTowardsMouse();
            StartAttack();
        }
    }

    private void StartAttack()
    {
        _stateManager.ChangeState(PlayerState.Attacking);
        _canAttack = false;
        _lastAttackTime = Time.time;

        AttackAnimation();
    }

    private void ResetCombo()
    {
        _comboIndex = 0;
    }

    private void StartAttackAnimation()
    {
        _stateManager.ChangeState(PlayerState.Attacking);
    }

    private void FinishAttack()
    {
        _stateManager.ChangeState(PlayerState.Idle);
        _playerMove.StopAttackMovement();
    }

    private void AttackAnimation()
    {
        _animatorController.AttackAnimation(_comboIndex);
    }

    /// <summary>
    /// 애니메이션 이벤트: 공격 시작 (애니메이션 시작)
    /// </summary>
    public void OnAttackAnimationStart()
    {
        StartAttackAnimation();

        // 공격 이동 시작
        _playerMove.StartAttackMovement(_comboIndex);
    }

    /// <summary>
    /// 애니메이션 이벤트: 공격 판정 (칼을 정확히 휘두르기 시작)
    /// 연속 판정 시작 (움직이는 동안 계속 체크)
    /// </summary>
    public void OnAttackHitStart()
    {
        if (_attackRange != null)
        {
            _attackRange.StartAttack(_comboIndex);
        }
    }

    /// <summary>
    /// 애니메이션 이벤트: 공격 판정 끝 (칼을 모두 휘두름)
    /// 연속 판정 종료
    /// </summary>
    public void OnAttackHitFinish()
    {
        if (_attackRange != null)
        {
            _attackRange.StopAttack();
        }
    }

    /// <summary>
    /// 애니메이션 이벤트: 1, 2타 종료 (애니메이션 끝)
    /// </summary>
    public void OnAttackAnimationEnd()
    {
        _canAttack = true;
        FinishAttack();

        _comboIndex++;
        if (_comboIndex >= _attackData.MaxComboCount)
        {
            _comboIndex = 0;
        }
    }

    /// <summary>
    /// 애니메이션 이벤트: 3타 종료 (애니메이션 끝)
    /// </summary>
    public void OnFinishAttackAnimationEnd()
    {
        _canAttack = true;
        FinishAttack();
        ResetCombo();
    }

    // 대시 종료 이벤트
    private void OnDashFinished()
    {
        _canAttack = true;
    }
}
