using UnityEngine;

public class BullyMonsterController : BadMonsterController
{
    [SerializeField] private GameObject _bullyTarget;

    public override void OnDespawn()
    {
        StopAllRoutines();
    }

    protected override void FindTarget()
    {
        if (_bullyTarget == null) return;
        _move.SetTarget(_bullyTarget.transform);
        _attack.SetTarget(_bullyTarget.transform);
    }

    protected override void OnUpdate()
    {
        _move.UpdateMove();

        if (_attack.TryAttack())
        {
            _move.LookAtTargetImmediate();
        }
    }

    public override bool TryTakeDamage(Damage damage)
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

    protected override MonsterState GetCurrentState()
    {
        if (_isDead) return MonsterState.Die;
        if (_isStunned) return MonsterState.Idle;
        if (_damage.IsDamaged) return MonsterState.Damage;
        if (_attack.IsAttacking) return MonsterState.Attack;
        return MonsterState.Idle;
    }

    protected override void ResetState()
    {
        _isDead = false;
        _isStunned = false;

        ApplyState(MonsterState.Idle);
    }
}
