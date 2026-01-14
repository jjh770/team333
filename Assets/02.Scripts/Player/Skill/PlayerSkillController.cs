using System;
using UnityEngine;

[System.Serializable]
public class SkillEffect
{
    public GameObject SkillEffectObject;

    public void Enable()
    {
        if (SkillEffectObject != null)
        {
            SkillEffectObject.SetActive(true);
        }
    }

    public void Disable()
    {
        if (SkillEffectObject != null)
        {
            SkillEffectObject.SetActive(false);
        }
    }
}
public class PlayerSkillController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private PlayerSkillData _skillData;

    private PlayerStateManager _stateManager;
    private PlayerAnimatorController _animatorController;
    private PlayerMove _playerMove;
    private PlayerMouseHelper _mouseHelper;
    private PlayerInputHandler _inputHandler;
    private TweenMovement _tweenMovement;
    private PlayerSkillRange _skillRange;

    private int _skillLevel = 0;
    private float _lastUseTime = -999f;

    public int MaxSkillLevel => _skillData.MaxSkillLevel;
    public int SkillLevel => _skillLevel;
    public bool IsUnlocked => _skillLevel >= 1;
    public bool HasProjectile => _skillLevel >= 2;
    public bool HasTripleProjectile => _skillLevel >= 3;
    public bool IsReady => Time.time >= _lastUseTime + _skillData.Cooldown;

    public event Action OnSkillUnlocked;
    public event Action<int> OnSkillUpgraded;
    public event Action OnSkillUsed;

    [SerializeField] private SkillEffect _skillEffect;

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
        DisableSkillEffect();
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
        return IsUnlocked && IsReady && _stateManager.CanSkill;
    }

    public void UpgradeSkill()
    {
        if (_skillLevel >= MaxSkillLevel) return;

        _skillLevel++;

        if (_skillLevel == 1)
        {
            OnSkillUnlocked?.Invoke();
        }

        OnSkillUpgraded?.Invoke(_skillLevel);
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
            SkillEffectStart();
            _skillRange.ExecuteSkillHit();
        }
    }

    public void OnSkillProjectile()
    {
        if (!HasProjectile) return;

        if (_skillRange != null)
        {
            _skillRange.FireProjectile(HasTripleProjectile);
        }
    }

    public void OnSkillAnimationEnd()
    {
        _stateManager.ChangeState(PlayerState.Idle);
    }

    #endregion

    private void DisableSkillEffect()
    {
        _skillEffect?.Disable();
    }

    private void SkillEffectStart()
    {
        _skillEffect?.Disable();
        _skillEffect?.Enable();
    }
}
