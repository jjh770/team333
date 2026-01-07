public class FloraMoveState : IFloraState
{
    private readonly FloraMovement _movement;

    public FloraMoveState(FloraMovement movement)
    {
        _movement = movement;
    }

    public void Enter()
    {
        _movement.SetNextDestination();
        _movement.AnimationController?.PlayMove(_movement.CurrentSpeed);
    }

    public void Update()
    {
        if (_movement.HasReachedDestination())
        {
            if (_movement.Path.IsFinished)
            {
                _movement.ChangeState(_movement.IdleState);
                return;
            }

            _movement.SetNextDestination();
        }
    }

    public void Exit()
    {
    }
}