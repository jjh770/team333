using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(MonsterStat))]
[RequireComponent(typeof(MonsterMoveComponent))]
[RequireComponent(typeof(MonsterAttackComponent))]
[RequireComponent(typeof(MonsterDamageComponent))]
public class MonsterController : MonoBehaviour, IPoolable, IDamageable
{
    [Header("Death")]
    [SerializeField] private float _deathAnimationDuration = 0.19f;

    [Header("Damage")]
    [SerializeField] private float _damageDuration = 0.09f;

    private Animator _animator;
    private NavMeshAgent _agent;
    private MonsterStat _stat;
    private MonsterMoveComponent _move;
    private MonsterAttackComponent _attack;
    private MonsterDamageComponent _damage;

    private MonsterState _currentState = MonsterState.Idle;
    public MonsterState CurrentState => _currentState;

    private bool _isDead;
    private bool _isDamaged;

    public bool IsDead => _isDead;
    public bool IsDamaged => _isDamaged;
    public bool IsAttacking => _attack.IsAttacking;
    public bool IsMoving => _move.IsMoving;

    public event Action<MonsterController> OnDie;

    private Coroutine _deathRoutine;
    private Coroutine _damageRoutine;

    private static readonly int s_animationHash = Animator.StringToHash("animation");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _stat = GetComponent<MonsterStat>();
        _move = GetComponent<MonsterMoveComponent>();
        _attack = GetComponent<MonsterAttackComponent>();
        _damage = GetComponent<MonsterDamageComponent>();
    }

    public void OnSpawn()
    {
        _agent.enabled = true;
        FindTarget();
        ResetState();
    }

    private void FindTarget()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            Transform player = playerObject.transform;
            _move.SetTarget(player);
            _attack.SetTarget(player);
        }
    }

    public void OnDespawn()
    {
        StopAllRoutines();
        _agent.enabled = false;
    }

    private void ResetState()
    {
        _isDead = false;
        _isDamaged = false;
        _agent.isStopped = false;
        _agent.ResetPath();
        ApplyState(MonsterState.Idle);
    }

    private void StopAllRoutines()
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
    }

    private void Update()
    {
        if (!_isDead)
        {
            _move.UpdateMove();

            if (_attack.TryAttack())
            {
                _move.LookAtTargetImmediate();
            }
        }

        UpdateState();
    }

    private void UpdateState()
    {
        MonsterState newState = GetCurrentState();

        if (_currentState != newState)
        {
            ApplyState(newState);
        }
    }

    private MonsterState GetCurrentState()
    {
        if (_isDead) return MonsterState.Die;
        if (_isDamaged) return MonsterState.Damage;
        if (IsAttacking) return MonsterState.Attack;
        if (IsMoving) return MonsterState.Move;
        return MonsterState.Idle;
    }

    private void ApplyState(MonsterState newState)
    {
        _currentState = newState;
        _animator.SetInteger(s_animationHash, (int)_currentState);
    }

    public bool TryTakeDamage(Damage damage)
    {
        if (damage.Value <= 0) return false;
        if (_isDead) return false;

        _stat.Health.Decrease(damage.Value);
        _damage.FlashWhite();

        if (!_isDamaged)
        {
            _damageRoutine = StartCoroutine(DamageCoroutine());
        }

        if (_stat.Health.IsEmpty)
        {
            Die();
        }

        return true;
    }

    private IEnumerator DamageCoroutine()
    {
        _isDamaged = true;
        yield return new WaitForSeconds(_damageDuration);
        _isDamaged = false;
        _damageRoutine = null;
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
        OnDie?.Invoke(this);
    }

    public void SetMoveSpeed(float value) => _stat.SetMoveSpeed(value);
    public void ChangeMoveSpeed(float amount) => _stat.ChangeMoveSpeed(amount);
}
