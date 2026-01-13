using System.Collections;
using UnityEngine;

public class BadTraceMonsterController : BadMonsterController
{
    public override void OnDespawn()
    {
        StopAllRoutines();
        _move.Disable();
    }

    protected override void FindTarget()
    {
        if (_player == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            _player = player?.transform;
        }

        if (_flora == null)
        {
            var flora = GameObject.FindGameObjectWithTag("Flora");
            _flora = flora?.transform;
        }

        if (_player == null && _flora == null) return;

        Transform chosen = ChooseTargetByDistance(_player, _flora);

        _move.SetTarget(chosen);
        _attack.SetTarget(chosen);
    }

    public override bool TryTakeDamage(Damage damage)
    {
        if (damage.Value <= 0) return false;
        if (_isDead) return false;
        if (_stat == null) return false;

        _stat.TakeDamage(damage.Value);
        _damage.FlashWhite();
        if (damage.IsKnockBack)
        {
            _damage.ApplyKnockback(damage.Attacker.transform.position);
        }
        
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
        if (_move.IsMoving) return MonsterState.Move;
        return MonsterState.Idle;
    }

    protected override void ResetState()
    {
        _move.ResetMove();
        _move.SetSpeed(_stat.GetMoveSpeed());
        _isDead = false;
        _isStunned = false;

        ApplyState(MonsterState.Idle);
    }

    protected override void Die()
    {
        if (_isDead) return;

        _isDead = true;
        _move.Stop();
        _itemDrop.DropItem();
        MonsterEffectPool.Instance?.PlaySmokeEffect(transform.position);

        ApplyState(MonsterState.Die);

        _deathRoutine = StartCoroutine(DeathCoroutine());
    }

    protected override IEnumerator StunCoroutine(float duration)
    {
        _isStunned = true;
        _move.Stop();

        yield return new WaitForSeconds(duration);

        _isStunned = false;
        _move.Resume();
        _stunRoutine = null;
    }
}
