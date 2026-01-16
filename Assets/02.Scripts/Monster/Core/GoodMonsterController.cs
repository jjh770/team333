using UnityEngine;

public class GoodMonsterController : BaseMonsterController
{
    private Transform _playerTransform;

    private void Update()
    {
        UpdateState();
        _move.UpdateMove();
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
        if (_move.IsMoving) return MonsterState.Move;
        return MonsterState.Idle;
    }

    override public void OnSpawn()
    {
        ResetState();
        FindPlayer();
        _move.SetTarget(_playerTransform);
    }

    private void FindPlayer()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerTransform = player.transform;
        }
    }

    override public void OnDespawn()
    {
        StopAllRoutines();
    }

    override protected void StopAllRoutines()
    {
        //
    }

    override protected void ApplyState(MonsterState newState)
    {
        _currentState = newState;
        _animator.SetInteger(s_animationHash, (int)_currentState);
    }

    override protected void ResetState()
    {
        ApplyState(MonsterState.Idle);
    }
}
