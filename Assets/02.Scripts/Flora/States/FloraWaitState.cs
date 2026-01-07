public class FloraWaitState : IFloraState
{
    private readonly FloraMovement _movement;

    public FloraWaitState(FloraMovement movement)
    {
        _movement = movement;
    }

    public void Enter()
    {
        _movement.AnimationController?.PlayIdle();
    }

    public void Update()
    {
    }

    public void Exit()
    {
    }
}
