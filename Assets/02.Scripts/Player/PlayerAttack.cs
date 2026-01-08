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
                ResetCombo();
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

    private void FinishAttack()
    {
        _stateManager.ChangeState(PlayerState.Idle);
    }

    private void AttackAnimation()
    {
        _animatorController.AttackAnimation(_comboIndex);
    }

    // 애니메이션 이벤트에서 호출될 함수
    public void OnAttackAnimationEnd()
    {
        _attackDurationTimer = 0;
        FinishAttack();
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
