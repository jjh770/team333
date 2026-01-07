using UnityEngine;

public class FloraIdleState : IFloraState
{
    private readonly FloraMovement _movement;

    public FloraIdleState(FloraMovement movement)
    {
        _movement = movement;
    }

    public void Enter()
    {
        _movement.AnimationController?.PlayIdle();
    }

    public void Update()
    {
        if (_movement.HasNextDestination())
        {
            _movement.ChangeState(_movement.MoveState);
        }
    }

    public void Exit()
    {
    }
}
