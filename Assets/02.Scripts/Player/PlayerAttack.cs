using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float _attackCooldown = 0.1f;
    [SerializeField] private float _attackDuration = 0.5f;

    [Header("Combo Settings")]
    [SerializeField] private float _comboResetTime = 0.6f;
    [SerializeField] private int _maxComboCount = 3;

    private int _maxComboMargin = 10;
    private PlayerAnimatorController _animatorController;
    private PlayerStateManager _stateManager;
    private float _attackCooldownTimer;
    private float _attackDurationTimer;
    private bool _canAttack = true;
    private int _comboIndex = 0;
    private float _comboResetTimer;

    private void Awake()
    {
        _animatorController = GetComponent<PlayerAnimatorController>();
        _stateManager = GetComponent<PlayerStateManager>();

        // 상태 전환 검증 이벤트 구독
        _stateManager.OnValidateStateChange += ValidateStateChange;
        // 상태 변경 이벤트 구독 (대시로 인한 공격 캔슬)
        _stateManager.OnStateChanged += OnStateChanged;
    }

    private void OnDestroy()
    {
        if (_stateManager != null)
        {
            _stateManager.OnValidateStateChange -= ValidateStateChange;
            _stateManager.OnStateChanged -= OnStateChanged;
        }
    }

    private bool ValidateStateChange(PlayerState from, PlayerState to)
    {
        // 공격 중 또는 ComboWindow 동안 Idle이나 Moving으로 전환 시도 시 거부
        if (from == PlayerState.Attacking && (_attackDurationTimer > 0 || _comboResetTimer > 0))
        {
            if (to == PlayerState.Idle || to == PlayerState.Moving)
                return false;
        }
        return true;
    }

    private void OnStateChanged(PlayerState from, PlayerState to)
    {
        // Dashing으로 변경 시 공격 캔슬
        if (to == PlayerState.Dashing && from == PlayerState.Attacking)
        {
            CancelAttack();
        }
    }

    private void Update()
    {
        UpdateCooldown();
        UpdateAttackDuration();
        UpdateComboReset();
        HandleAttackInput();
    }

    private void UpdateCooldown()
    {
        if (_attackCooldownTimer > 0)
        {
            _attackCooldownTimer -= Time.deltaTime;
        }
    }

    private void UpdateAttackDuration()
    {
        if (_attackDurationTimer > 0)
        {
            _attackDurationTimer -= Time.deltaTime;
        }
    }

    private void UpdateComboReset()
    {
        if (_comboResetTimer > 0)
        {
            _comboResetTimer -= Time.deltaTime;

            if (_comboResetTimer <= 0)
            {
                // ComboWindow 종료 - 콤보 리셋 및 Idle 상태로 전환
                ResetCombo();
                if (_stateManager.IsState(PlayerState.Attacking))
                {
                    FinishAttack();
                }
            }
        }
    }

    private void HandleAttackInput()
    {
        if (Input.GetMouseButtonDown(0) && _canAttack && _attackCooldownTimer <= 0 && _stateManager.CanAttack)
        {
            StartAttack();
        }
    }

    private void StartAttack()
    {
        _stateManager.ChangeState(PlayerState.Attacking);

        _attackDurationTimer = _attackDuration;

        AttackAnimation();
        PerformAttack();
    }

    private void PerformAttack()
    {
        _attackCooldownTimer = _attackCooldown;
        _comboResetTimer = _comboResetTime;

        _comboIndex++;
        if (_comboIndex >= _maxComboMargin)
        {
            _comboIndex = 0;
        }
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
    }

    private void AttackAnimation()
    {
        _animatorController.AttackAnimation(_comboIndex);
    }

    // 애니메이션 이벤트에서 호출될 함수
    public void OnAttackAnimationStart()
    {
        StartAttackAnimation();
    }

    // 1, 2타 공격 애니메이션 종료 (콤보 연결 가능)
    public void OnAttackAnimationEnd()
    {
        _attackDurationTimer = 0;
        // FinishAttack() 호출하지 않음 - ComboWindow 유지
        // _comboResetTimer가 끝나면 자동으로 Idle로 전환
    }

    public void OnFinishAttackAnimationEnd()
    {
        _attackDurationTimer = 0;
        ResetCombo();
        FinishAttack();
    }

    // 공격 캔슬 (대시로 인한 캔슬)
    public void CancelAttack()
    {
        _attackDurationTimer = 0;
        _comboResetTimer = 0;
        ResetCombo();
    }
}
