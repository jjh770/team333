using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Combo Settings")]
    [SerializeField] private int _maxComboCount = 3;
    [SerializeField] private float _comboWindowTime = 2f; // 콤보 유지 시간

    private PlayerRotateToMouse _rotateToMouse;
    private PlayerAnimatorController _animatorController;
    private PlayerStateManager _stateManager;
    private PlayerAttackRange _attackRange;
    private bool _canAttack = true;
    [SerializeField] private int _comboIndex = 0; // 현재 콤보 단계 유지
    private float _lastAttackTime; // 마지막 공격 시간 기록

    private void Awake()
    {
        _animatorController = GetComponent<PlayerAnimatorController>();
        _stateManager = GetComponent<PlayerStateManager>();
        _attackRange = GetComponent<PlayerAttackRange>();
        _rotateToMouse = GetComponent<PlayerRotateToMouse>();

        _stateManager.OnStateChanged += OnStateChanged;
    }

    private void OnDestroy()
    {
        if (_stateManager != null)
        {
            _stateManager.OnStateChanged -= OnStateChanged;
        }
    }

    private void OnStateChanged(PlayerState from, PlayerState to)
    {
        // Dashing으로 변경 시 공격 상태만 종료 (콤보는 유지)
        if (to == PlayerState.Dashing && from == PlayerState.Attacking)
        {
            _stateManager.ChangeState(PlayerState.Dashing);
        }
    }

    private void Update()
    {
        CheckComboTimeout();
        HandleAttackInput();
    }

    /// <summary>
    /// 콤보 타임아웃 체크
    /// 일정 시간 공격 입력이 없으면 콤보 리셋
    /// </summary>
    private void CheckComboTimeout()
    {
        if (_comboIndex > 0 && Time.time - _lastAttackTime > _comboWindowTime)
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
        _lastAttackTime = Time.time; // 공격 시간 기록

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
        // 콤보는 리셋하지 않음 - 시간이 지나면 자동 리셋
    }

    private void AttackAnimation()
    {
        _animatorController.AttackAnimation(_comboIndex);
    }

    /// <summary>
    /// 애니메이션 이벤트: 공격 시작
    /// </summary>
    public void OnAttackAnimationStart()
    {
        StartAttackAnimation();
    }

    /// <summary>
    /// 애니메이션 이벤트: 공격 판정
    /// </summary>
    public void OnAttackHit()
    {
        if (_attackRange != null)
        {
            _attackRange.PerformAttack(_comboIndex);
        }
        else
        {
            Debug.LogWarning("PlayerAttackRange component is missing!");
        }
    }

    /// <summary>
    /// 애니메이션 이벤트: 1, 2타 종료
    /// 다음 공격 입력 허용하고 콤보 인덱스 증가
    /// </summary>
    public void OnAttackAnimationEnd()
    {
        _canAttack = true;
        FinishAttack();

        // 다음 콤보 단계로 진행
        _comboIndex++;
        if (_comboIndex >= _maxComboCount)
        {
            _comboIndex = 0; // 마지막 콤보 후 다시 1타로
        }
    }

    /// <summary>
    /// 애니메이션 이벤트: 마지막 공격 종료
    /// 콤보 리셋하고 다시 1타부터 시작
    /// </summary>
    public void OnFinishAttackAnimationEnd()
    {
        _canAttack = true;
        FinishAttack();
        ResetCombo(); // 3타 후에는 완전히 리셋
    }

    /// <summary>
    /// 공격 캔슬 (대시 등)
    /// 콤보는 유지하고 공격 상태만 종료
    /// </summary>
    public void CancelAttack()
    {
        _canAttack = true;
        // _comboIndex는 유지 - 대시 후에도 이어서 공격 가능
    }
}
