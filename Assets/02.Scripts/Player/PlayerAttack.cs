using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private PlayerAttackData _attackData;

    [Header("Attack Movement")]
    [SerializeField] private bool _enableAttackMovement = true;

    private PlayerMouseHelper _mouseHelper;
    private PlayerAnimatorController _animatorController;
    private PlayerStateManager _stateManager;
    private PlayerAttackRange _attackRange;
    private PlayerMove _playerMove;
    private PlayerDash _playerDash;
    private PlayerInputHandler _inputHandler;
    private TweenMovement _tweenMovement;
    private PlayerEffectController _effectController;
    private PlayerSound _playerSound;

    private bool _canAttack = true;
    [SerializeField] private int _comboIndex = 0;
    private float _lastAttackTime;
    private const float AttackTimeout = 1f;
    private void Awake()
    {
        _animatorController = GetComponent<PlayerAnimatorController>();
        _stateManager = GetComponent<PlayerStateManager>();
        _attackRange = GetComponent<PlayerAttackRange>();
        _mouseHelper = GetComponent<PlayerMouseHelper>();
        _playerMove = GetComponent<PlayerMove>();
        _playerDash = GetComponent<PlayerDash>();
        _inputHandler = GetComponent<PlayerInputHandler>();
        _tweenMovement = GetComponent<TweenMovement>();
        _effectController = GetComponent<PlayerEffectController>();
        _playerSound = GetComponent<PlayerSound>();
    }

    private void OnEnable()
    {
        _stateManager.OnStateChanged += OnStateChanged;
        _playerDash.OnDashFinish += OnDashFinished;
        _inputHandler.OnAttackInput += HandleAttackInput;
    }

    private void OnDisable()
    {
        _stateManager.OnStateChanged -= OnStateChanged;
        _playerDash.OnDashFinish -= OnDashFinished;
        _inputHandler.OnAttackInput -= HandleAttackInput;
    }

    private void OnStateChanged(PlayerState from, PlayerState to)
    {
        if (from == PlayerState.Attacking && to == PlayerState.Dashing)
        {
            StopAttackMovement();

            if (_attackRange != null)
            {
                _attackRange.StopAttack();
            }

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
        CheckAttackStateTimeout();
    }

    private void CheckComboTimeout()
    {
        if (_comboIndex > 0 && Time.time - _lastAttackTime > _attackData.ComboWindowTime)
        {
            ResetCombo();
        }
    }

    private void CheckAttackStateTimeout()
    {
        if (!_stateManager.IsState(PlayerState.Attacking)) return;
        if (Time.time - _lastAttackTime < AttackTimeout) return;

        _attackRange?.StopAttack();
        _canAttack = true;
        FinishAttack();
        ResetCombo();
    }

    private void HandleAttackInput()
    {
        if (!_canAttack || !_stateManager.CanAttack) return;

        _mouseHelper.RotateTowardsMouse();
        StartAttack();
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

    private void FinishAttack()
    {
        _stateManager.ChangeState(PlayerState.Idle);
        StopAttackMovement();
    }

    private void AttackAnimation()
    {
        _animatorController.AttackAnimation(_comboIndex);
    }

    #region Attack Movement

    private void StartAttackMovement(int comboIndex)
    {
        if (!_enableAttackMovement)
        {
            return;
        }

        if (comboIndex < 0 || comboIndex >= _attackData.AttackMoveDistance.Length ||
            comboIndex >= _attackData.AttackMoveEase.Length)
        {
            Debug.LogError($"Invalid combo index: {comboIndex}");
            return;
        }

        Vector3 direction = _playerMove.GetMovementDirection();
        if (direction.magnitude < 0.1f) return;

        float moveDuration = _animatorController.GetCurrentAnimationDuration();
        _tweenMovement.StartLinearMovement(
            transform.forward,
            _attackData.AttackMoveDistance[comboIndex],
            moveDuration,
            _attackData.AttackMoveEase[comboIndex]);
    }

    private void StopAttackMovement()
    {
        _tweenMovement.Stop();
    }

    #endregion

    #region Animation Events

    public void OnAttackAnimationStart()
    {
        if (!_stateManager.IsState(PlayerState.Attacking)) return;

        StartAttackMovement(_comboIndex);
    }

    public void OnAttackHitStart()
    {
        if (_attackRange != null)
        {
            SlashStart();
            _attackRange.StartAttack(_comboIndex);
        }

        _playerSound?.PlayAttack(_comboIndex);
    }

    public void OnAttackHitFinish()
    {
        if (_attackRange != null)
        {
            _attackRange.StopAttack();
        }
    }

    public void OnAttackAnimationEnd()
    {
        if (!_stateManager.IsState(PlayerState.Attacking)) return;

        _canAttack = true;
        FinishAttack();

        _comboIndex++;
        if (_comboIndex >= _attackData.MaxComboCount)
        {
            _comboIndex = 0;
        }
    }

    public void OnFinishAttackAnimationEnd()
    {
        if (!_stateManager.IsState(PlayerState.Attacking)) return;

        _canAttack = true;
        FinishAttack();
        ResetCombo();
    }

    #endregion

    private void OnDashFinished()
    {
        _canAttack = true;
    }

    private void SlashStart()
    {
        _effectController?.PlaySlash(_comboIndex);
    }
}
