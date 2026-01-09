using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(MonsterStat))]
[RequireComponent(typeof(MonsterAttackComponent))]
[RequireComponent(typeof(MonsterDamageComponent))]
public class BadMonsterController : BaseMonsterController, IDamageable
{
    [Header("Death")]
    [SerializeField] private float _deathAnimationDuration = 0.19f;

    [Header("Damage")]
    [SerializeField] private float _damageDuration = 0.09f;

    private MonsterStat _stat;
    private MonsterAttackComponent _attack;
    private MonsterDamageComponent _damage;

    private bool _isDead;
    public bool IsDead => _isDead;

    private bool _isStunned;
    public bool IsStunned => _isStunned;

    public event Action<BadMonsterController> OnDie;

    private Coroutine _deathRoutine;
    private Coroutine _damageRoutine;
    private Coroutine _stunRoutine;

    override protected void Awake()
    {
        base.Awake();

        _stat = GetComponent<MonsterStat>();
        _attack = GetComponent<MonsterAttackComponent>();
        _damage = GetComponent<MonsterDamageComponent>();
    }

    override public void OnSpawn()
    {
        FindTarget();
        ResetState();
    }

    override public void OnDespawn()
    {
        StopAllRoutines();
        _agent.enabled = false;
    }

    override protected void FindTarget()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null) return;

        Transform player = playerObject.transform;
        _move.SetTarget(player);
        _attack.SetTarget(player);
    }

    override protected void ResetState()
    {
        _agent.enabled = true;
        _agent.ResetPath();
        _agent.isStopped = false;
        _agent.speed = _stat.MoveSpeed.Value;
        _isDead = false;
        _isStunned = false;

        ApplyState(MonsterState.Idle);
    }

    override protected void StopAllRoutines()
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

    private void Update()
    {
        UpdateState();

        if (_isStunned) return;
        if (_damage.IsDamaged) return;

        _move.UpdateMove();

        if (_attack.TryAttack())
        {
            _move.LookAtTargetImmediate();
        }
    }

    override protected void UpdateState()
    {
        MonsterState newState = GetCurrentState();

        if (_currentState != newState)
        {
            ApplyState(newState);
        }
    }

    override protected MonsterState GetCurrentState()
    {
        if (_isDead) return MonsterState.Die;
        if (_isStunned) return MonsterState.Idle;
        if (_damage.IsDamaged) return MonsterState.Damage;
        if (_attack.IsAttacking) return MonsterState.Attack;
        if (_move.IsMoving) return MonsterState.Move;
        return MonsterState.Idle;
    }

    override protected void ApplyState(MonsterState newState)
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

        if (!_damage.IsDamaged)
        {
            _damageRoutine = StartCoroutine(_damage.DamageCoroutine());
            _damageRoutine = null;
        }

        if (_stat.Health.IsEmpty)
        {
            Die();
        }

        return true;
    }

    private void Die()
    {
        if (_isDead) return;

        _isDead = true;
        _agent.isStopped = true;
        _agent.ResetPath();

        ApplyState(MonsterState.Die);

        _deathRoutine = StartCoroutine(DeathCoroutine());
    }

    private IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(_deathAnimationDuration);
        _deathRoutine = null;
        OnDie.Invoke(this);
    }

    public void SetMoveSpeed(float value)
    {
        _stat.SetMoveSpeed(value);
        _agent.speed = _stat.GetMoveSpeed();
    }

    public void ChangeMoveSpeed(float amount)
    {
        _stat.ChangeMoveSpeed(amount);
        _agent.speed = _stat.GetMoveSpeed();
    }

    public void ApplyStun(float duration)
    {
        if (_stunRoutine != null)
        {
            StopCoroutine(_stunRoutine);
        }
        _stunRoutine = StartCoroutine(StunCoroutine(duration));
    }

    private IEnumerator StunCoroutine(float duration)
    {
        _isStunned = true;
        _agent.isStopped = true;
        _agent.ResetPath();

        yield return new WaitForSeconds(duration);

        _isStunned = false;
        _agent.isStopped = false;
        _stunRoutine = null;
    }
}
