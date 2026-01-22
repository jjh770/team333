using UnityEngine;

public class FloraIdleState : IFloraState
{
    private readonly FloraMovement _movement;
    private readonly float _turnSpeed = 2f;
    private Quaternion _targetRotation;
    private bool _isPathFinished;

    public FloraIdleState(FloraMovement movement)
    {
        _movement = movement;
    }

    public void Enter()
    {
        _movement.AnimationController?.PlayIdle();
        _isPathFinished = !_movement.HasNextDestination();

        if (_isPathFinished)
        {
            _targetRotation = Quaternion.Euler(0f, _movement.transform.eulerAngles.y + 180f, 0f);
        }
    }

    public void Update()
    {
        if (_movement.HasNextDestination())
        {
            _movement.ChangeState(_movement.MoveState);
            return;
        }

        if (_isPathFinished)
        {
            TurnAround();
        }
    }

    private void TurnAround()
    {
        _movement.transform.rotation = Quaternion.Slerp(
            _movement.transform.rotation,
            _targetRotation,
            _turnSpeed * Time.deltaTime);
    }

    public void Exit()
    {
    }
}
