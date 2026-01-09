using UnityEngine;

public class BadPlacedMonsterController : BadMonsterController
{
    public override void OnDespawn()
    {
        StopAllRoutines();
    }

    protected override void FindTarget()
    {
        if (_player == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            _player = player?.transform;
        }

        _move.SetTarget(_player);
        _attack.SetTarget(_player);
    }

    protected override void OnUpdate()
    {
        _move.UpdateMove();

        if (_attack.TryAttack())
        {
            _move.LookAtTargetImmediate();
        }
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
