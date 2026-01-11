using System;
using UnityEngine;

public class PlayerSkillController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _cooldown = 3f;

    private PlayerStateManager _stateManager;
    private PlayerAnimatorController _animatorController;
    private PlayerMove _playerMove;
    private PlayerMouseHelper _mouseHelper;
    private PlayerInputHandler _inputHandler;
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


    public void OnSkillAnimationStart()
    {
        // 공격 이동 시작
        _playerMove.StartSkillMovement();
    }


    /// <summary>
    /// 스킬 애니메이션 종료 시 호출 (Animation Event)
    /// </summary>
    public void OnSkillAnimationEnd()
    {
        _stateManager.ChangeState(PlayerState.Idle);
    }
} 
