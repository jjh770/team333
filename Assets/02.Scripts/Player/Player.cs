using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerAnimatorController))]
[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(PlayerMove))]
[RequireComponent(typeof(PlayerDash))]
[RequireComponent(typeof(PlayerAttack))]
[RequireComponent(typeof(PlayerAttackRange))]
[RequireComponent(typeof(PlayerStateManager))]
[RequireComponent(typeof(PlayerMouseHelper))]
[RequireComponent(typeof(PlayerStat))]
[RequireComponent(typeof(PlayerWeaponVisual))]
[RequireComponent(typeof(PlayerInteraction))]
[RequireComponent(typeof(PlayerPickUpThrow))]
[RequireComponent(typeof(PlayerTalk))]
[RequireComponent(typeof(PlayerSkillController))]
[RequireComponent(typeof(PlayerDamageComponent))]
[RequireComponent(typeof(TweenMovement))]

public class Player : MonoBehaviour
{
    private PlayerStat _stat;
    private PlayerAnimatorController _animatorController;
    private PlayerStateManager _stateManager;
    private bool _isDead;
    public bool IsDead => _isDead;

    private void Awake()
    {
        _stat = GetComponent<PlayerStat>();
        _animatorController = GetComponent<PlayerAnimatorController>();
        _stateManager = GetComponent<PlayerStateManager>();
    }

    private void OnEnable()
    {
        _isDead = false;
        _stat.OnDeath += Die;
    }

    private void OnDisable()
    {
        _stat.OnDeath -= Die;
    }

    private void Die()
    {
        if (_isDead) return;
        _isDead = true;

        _stateManager.ChangeState(PlayerState.Die);
        _animatorController.DieAnimation();
    }
}
