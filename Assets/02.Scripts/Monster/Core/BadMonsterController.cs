using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MonsterStat))]
[RequireComponent(typeof(MonsterAttackComponent))]
[RequireComponent(typeof(MonsterDamageComponent))]
public abstract class BadMonsterController : BaseMonsterController, IDamageable
{
    [Header("Death")]
    [SerializeField] protected float _deathAnimationDuration = 0.19f;

    [Header("Target")]
    [SerializeField] protected float _playerNearDistance = 4f;

    protected Transform _player;
    protected Transform _flora;

    protected MonsterStat _stat;
    protected MonsterAttackComponent _attack;
    protected MonsterDamageComponent _damage;

    protected bool _isDead;
    public bool IsDead => _isDead;

    protected bool _isStunned;
    public bool IsStunned => _isStunned;

    public event Action<BadMonsterController> OnDie;

    protected Coroutine _deathRoutine;
    protected Coroutine _damageRoutine;
    protected Coroutine _stunRoutine;

    protected override void Awake()
    {
        base.Awake();

        _stat = GetComponent<MonsterStat>();
        _attack = GetComponent<MonsterAttackComponent>();
        _damage = GetComponent<MonsterDamageComponent>();
    }

    public override void OnSpawn()
    {
        FindTarget();
        ResetState();
    }

    protected Transform ChooseTargetByDistance(Transform player, Transform flora)
    {
        if (player == null) return flora;
        if (flora == null) return player;

        float sqrNear = _playerNearDistance * _playerNearDistance;
        float playerSqr = (player.position - flora.position).sqrMagnitude;

        return playerSqr <= sqrNear ? player : flora;
    }

    protected override void StopAllRoutines()
    {
        if (_deathRoutine != null)
        {
            StopCoroutine(_deathRoutine);
            _deathRoutine = null;
        }
        if (_damageRoutine != null)
        {
            StopCoroutine(_damageRoutine);
            _damageRoutine = null;
        }
        if (_stunRoutine != null)
        {
            StopCoroutine(_stunRoutine);
            _stunRoutine = null;
            _isStunned = false;
        }
    }

    protected virtual void Update()
    {
        if (_isDead) return;

        UpdateState();

        if (_isStunned) return;
        if (_damage.IsDamaged) return;

        OnUpdate();
    }

    protected virtual void OnUpdate()
    {
        _move.UpdateMove();

        if (_attack.TryAttack())
        {
            _move.LookAtTargetImmediate();
        }
    }

    protected override void UpdateState()
    {
        MonsterState newState = GetCurrentState();

        if (_currentState != newState)
        {
            ApplyState(newState);
        }
    }

    protected override void ApplyState(MonsterState newState)
    {
        _currentState = newState;
        _animator.SetInteger(s_animationHash, (int)_currentState);
    }

    public bool TryTakeDamage(Damage damage)
    {
        if (damage.Value <= 0) return false;
        if (_isDead) return false;
        if (_stat == null) return false;

        _stat.TakeDamage(damage.Value);
        _damage.FlashWhite();
        _attack.InitCooltime();

        if (!_damage.IsDamaged)
        {
            _damageRoutine = StartCoroutine(_damage.DamageCoroutine());
        }

        if (_stat.Health.IsEmpty)
        {
            Die();
        }

        return true;
    }

    protected virtual void Die()
    {
        if (_isDead) return;

        _isDead = true;
        ApplyState(MonsterState.Die);

        _deathRoutine = StartCoroutine(DeathCoroutine());
    }

    protected virtual IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(_deathAnimationDuration);
        _deathRoutine = null;
        OnDie?.Invoke(this);
    }

    public void SetMoveSpeed(float value)
    {
        _stat.SetMoveSpeed(value);
        _move.SetSpeed(_stat.MoveSpeed.Value);
    }

    public void ChangeMoveSpeed(float amount)
    {
        _stat.ChangeMoveSpeed(amount);
        _move.SetSpeed(_stat.MoveSpeed.Value);
    }

    public virtual void ApplyStun(float duration)
    {
        if (_isDead) return;

        if (_stunRoutine != null)
        {
            StopCoroutine(_stunRoutine);
        }
        _stunRoutine = StartCoroutine(StunCoroutine(duration));
    }

    protected virtual IEnumerator StunCoroutine(float duration)
    {
        _isStunned = true;

        yield return new WaitForSeconds(duration);

        _isStunned = false;
        _stunRoutine = null;
    }
}
