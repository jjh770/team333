using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MonsterStat))]
[RequireComponent(typeof(MonsterAttackComponent))]
[RequireComponent(typeof(MonsterHealthComponent))]
[RequireComponent(typeof(MonsterItemDropComponent))]
[RequireComponent(typeof(MonsterHealthBar))]
[RequireComponent(typeof(MonsterSensorComponent))]

public class BadMonsterController : BaseMonsterController, IDamageable
{
    [Header("Death")]
    [SerializeField] protected float _deathAnimationDuration = 0.19f;

    [Header("Components")]
    protected MonsterStat _stat;
    protected MonsterAttackComponent _attack;
    protected MonsterHealthComponent _health;
    protected MonsterItemDropComponent _itemDrop;
    protected MonsterSensorComponent _sensor;

    // BT에서 참조할 현재 타겟
    public Transform CurrentTarget { get; set; }

    public event Action<BadMonsterController> OnDie;

    protected Coroutine _deathRoutine;
    protected Coroutine _damageRoutine;
    protected Coroutine _stunRoutine;

    protected bool _isDead;
    public bool IsDead => _isDead;

    protected override void Awake()
    {
        base.Awake();

        _stat = GetComponent<MonsterStat>();
        _attack = GetComponent<MonsterAttackComponent>();
        _health = GetComponent<MonsterHealthComponent>();
        _itemDrop = GetComponent<MonsterItemDropComponent>();
        _sensor = GetComponent<MonsterSensorComponent>();
    }

    protected virtual void OnEnable()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnStateChanged += HandleGameStateChanged;
        }
    }

    protected virtual void OnDisable()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnStateChanged -= HandleGameStateChanged;
        }
    }

    private void HandleGameStateChanged(GameState oldState, GameState newState)
    {
        // 게임이 끝날 때 한 방에 죽이기
        if (newState == GameState.Outro)
        {
            Kill();
        }
    }

    public void Kill()
    {
        Die();
    }

    public override void OnSpawn()
    {
        SetupTarget();
        ResetState();
    }

    protected void SetupTarget()
    {
        Transform target = _sensor.GetCurrentTarget();
        _move.SetTarget(target);
        _attack.SetTarget(target);
        CurrentTarget = target;
    }
    public override void OnDespawn()
    {
        StopAllRoutines();
        _move.Disable();
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
        }
    }
    protected virtual void Update()
    {
        if (_isDead) return;

        UpdateState();

        if (_health.IsDamaged) return;

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
    protected override MonsterState GetCurrentState()
    {
        if (_isDead) return MonsterState.Die;
        if (_health.IsDamaged) return MonsterState.Damage;
        if (_attack.IsAttacking) return MonsterState.Attack;
        if (_move.IsMoving) return MonsterState.Move;
        return MonsterState.Idle;
    }

    protected override void ResetState()
    {
        _move.ResetMove();
        _move.SetSpeed(_stat.GetMoveSpeed());
        _isDead = false;

        ApplyState(MonsterState.Idle);
    }

    protected override void ApplyState(MonsterState newState)
    {
        _currentState = newState;
        _animator.SetInteger(s_animationHash, (int)_currentState);
    }

    protected virtual void Die()
    {
        if (_isDead) return;
        _isDead = true;

        _move.Stop();
        _itemDrop.DropItem(_stat.Data.DropItem);
        
        ApplyState(MonsterState.Die);
        _deathRoutine = StartCoroutine(DeathCoroutine());
    }

    protected virtual IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(_deathAnimationDuration);
        _deathRoutine = null;
        OnDie?.Invoke(this);

        MonsterEffectPool.Instance?.PlaySmokeEffect(transform.position);
    }

    // ====== 외부에서 호출 ======
    public void SetMoveSpeed(float value)
    {
        _stat.SetMoveSpeed(value);
        _move.SetSpeed(_stat.GetMoveSpeed());
    }

    public void ChangeMoveSpeed(float amount)
    {
        _stat.ChangeMoveSpeed(amount);
        _move.SetSpeed(_stat.GetMoveSpeed());
    }

    public virtual void ApplyStun(float duration)
    {
        if (_isDead) return;

        if (_stunRoutine != null)
        {
            StopCoroutine(_stunRoutine);
        }
        _stunRoutine = StartCoroutine(_move.StunCoroutine(duration));
    }

    public bool TryTakeDamage(Damage damage)
    {
        if (damage.Value <= 0) return false;
        if (_isDead) return false;
        if (_stat == null) return false;

        _stat.TakeDamage(damage.Value);
        _health.FlashWhite();
        _attack.InitCooltime();

        if (damage.IsKnockBack)
        {
            _move.ApplyKnockback(damage.Attacker.transform.position);
        }

        if (!_health.IsDamaged)
        {
            _damageRoutine = StartCoroutine(_health.DamageCoroutine());
        }

        if (_stat.Health.IsEmpty)
        {
            Die();
        }

        return true;
    }
}
