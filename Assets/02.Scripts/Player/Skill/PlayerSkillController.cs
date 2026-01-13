using System;
using UnityEngine;

public class PlayerSkillController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private PlayerSkillData _skillData;

    [Header("Settings")]
    [SerializeField] private float _cooldown = 3f;

    private PlayerStateManager _stateManager;
    private PlayerAnimatorController _animatorController;
    private PlayerMove _playerMove;
    private PlayerMouseHelper _mouseHelper;
    private PlayerInputHandler _inputHandler;
    private TweenMovement _tweenMovement;
    private PlayerSkillRange _skillRange;

    private bool _isUnlocked = false;
    private float _lastUseTime = -999f;

    public bool IsUnlocked => _isUnlocked;
    public bool IsReady => Time.time >= _lastUseTime + _cooldown;

    public event Action OnSkillUnlocked;
    public event Action OnSkillUsed;

    private void Awake()
    {
        _stateManager = GetComponent<PlayerStateManager>();
        _animatorController = GetComponent<PlayerAnimatorController>();
        _playerMove = GetComponent<PlayerMove>();
        _mouseHelper = GetComponent<PlayerMouseHelper>();
        _inputHandler = GetComponent<PlayerInputHandler>();
        _tweenMovement = GetComponent<TweenMovement>();
        _skillRange = GetComponent<PlayerSkillRange>();
    }

    private void OnEnable()
    {
        _inputHandler.OnSkillInput += HandleSkillInput;
    }

    private void OnDisable()
    {
        _inputHandler.OnSkillInput -= HandleSkillInput;
    }

    private void HandleSkillInput()
    {
        if (!CanUseSkill()) return;

        UseSkill();
    }

    private bool CanUseSkill()
    {
        return _isUnlocked && IsReady && _stateManager.CanSkill;
    }

    public void UnlockSkill()
    {
        if (_isUnlocked) return;

        _isUnlocked = true;
        OnSkillUnlocked?.Invoke();
    }

    private void UseSkill()
    {
        _stateManager.ChangeState(PlayerState.Skill);
        _mouseHelper.RotateTowardsMouse();
        _animatorController.SkillAnimation();
        _lastUseTime = Time.time;
        OnSkillUsed?.Invoke();
    }

    #region Skill Movement

    private void StartSkillMovement()
    {
        Vector3 mouseWorldPos = _mouseHelper.GetMouseWorldPosition();

        Vector3 offset = mouseWorldPos - transform.position;
        offset.y = 0;

        float maxDistance = _skillData.SkillMaxDistance;
        Vector3 clampedOffset = Vector3.ClampMagnitude(offset, maxDistance);
        float finalHorizontalDistance = clampedOffset.magnitude;

        Vector3 moveDirection = clampedOffset.normalized;
        if (moveDirection == Vector3.zero) moveDirection = transform.forward;

        float moveDuration = _animatorController.GetCurrentAnimationDuration();

        _tweenMovement.StartParabolicMovement(
            moveDirection,
            finalHorizontalDistance,
            _skillData.SkillJumpHeight,
            moveDuration,
            _skillData.SkillMoveEase);
    }

    #endregion

    #region Animation Events

    public void OnSkillAnimationStart()
    {
        StartSkillMovement();
    }

    public void OnSkillHit()
    {
        if (_skillRange != null)
        {
            _skillRange.ExecuteSkillHit();
        }
    }

    public void OnSkillProjectile()
    {
        if (_skillRange != null)
        {
            _skillRange.FireProjectile();
        }
    }

    public void OnSkillAnimationEnd()
    {
        _stateManager.ChangeState(PlayerState.Idle);
    }

    #endregion
}
