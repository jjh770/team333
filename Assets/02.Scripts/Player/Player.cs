using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerAnimatorController))]
[RequireComponent(typeof(PlayerMove))]
[RequireComponent(typeof(PlayerDash))]
[RequireComponent(typeof(PlayerAttack))]
[RequireComponent(typeof(PlayerAttackRange))]
[RequireComponent(typeof(PlayerStateManager))]
[RequireComponent(typeof(PlayerRotateToMouse))]
[RequireComponent(typeof(PlayerStat))]

public class Player : MonoBehaviour
{
    private PlayerStat _stat;
    private PlayerAnimatorController _animatorController;
    private PlayerStateManager _playerStateManager;
    private bool _isDead;
    public bool IsDead => _isDead;

    private void Awake()
    {
        _stat = GetComponent<PlayerStat>();
        _animatorController = GetComponent<PlayerAnimatorController>();
        _playerStateManager = GetComponent<PlayerStateManager>();
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

        _playerStateManager.ChangeState(PlayerState.Die);
        _animatorController.DieAnimation();
    }
}
